// 
// Meditation Logger.
// Copyright (C) 2015-2016  Seth Hendrick.
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MedEnthLogsApi;

namespace MedEnthDesktop.Server
{
    public class HttpServer : IDisposable
    {
        // -------- Fields --------

        /// <summary>
        /// Default port to listen on.
        /// </summary>
        public const short DefaultPort = 80;

        /// <summary>
        /// Reference to http listener.
        /// </summary>
        private readonly HttpListener listener;

        /// <summary>
        /// Action that is taken when the server wants to print something to some
        /// text-based console.  The string argument is the string we want to print.
        /// </summary>
        private Action<string> cout;

        /// <summary>
        /// Thread that does the listening.
        /// </summary>
        private Thread listeningThread;

        /// <summary>
        /// The api to use.
        /// </summary>
        private Api api;

        private readonly ManualResetEvent quitEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="api">The API to use.</param>
        /// <param name="port">The port to lisen on.</param>
        /// <param name="consoleOutFunction">
        /// Function to print out console information to.
        /// Set to null for no action.
        /// </param>
        /// <exception cref="PlatformNotSupportedException">This platform doesn't support an HTTP Server.</exception>
        /// <exception cref="ArgumentException">Api is not open.</exception>
        public HttpServer( Api api, short port = DefaultPort, Action<string> consoleOutFunction = null )
        {
            if( HttpListener.IsSupported == false )
            {
                throw new PlatformNotSupportedException(
                    "This platform does not support HTTP Listeners..."
                );
            }
            if( api.IsOpen == false )
            {
                throw new ArgumentException(
                    "Api is not open. Please open it before calling this constructor...",
                    nameof( api )
                );
            }
            if( consoleOutFunction == null )
            {
                this.cout = delegate ( string s )
                {
                };
            }

            this.api = api;
            this.cout = consoleOutFunction;
            this.listener = new HttpListener();

            this.listener.Prefixes.Add( "http://*:" + port + "/" );

            this.listeningThread = new Thread(
                delegate()
                {
                    HandleRequest( this.api, this.listener, this, cout );
                }
            );

            this.quitEvent = new ManualResetEvent( false );
            this.IsListening = false;
        }

        // -------- Properties --------

        /// <summary>
        /// Whether or not we are listening.
        /// </summary>
        public bool IsListening{ get; private set; }

        // -------- Functions --------

        /// <summary>
        /// Opens the websocket and listens for requests.
        /// No-op if already listening.
        /// </summary>
        public void Start()
        {
            // No-op if we are not listening.
            if( this.IsListening == false )
            {
                this.listener.Start();
                this.listeningThread.Start();
                this.IsListening = true;
            }
        }

        /// <summary>
        /// Closes the web socket and disposes this class.
        /// No-op if not already listening.
        /// </summary>
        public void Dispose()
        {
            // No-op if we are not listening.
            if( this.IsListening )
            {
                if( this.cout != null )
                {
                    this.cout( "Terminating server..." );
                }
                this.IsListening = false;
                this.listeningThread.Join();

                this.listener.Stop();
                this.listener.Close();
                if( this.cout != null )
                {
                    this.cout( "Terminating server...Done!" );
                }

                this.quitEvent.Set();
            }
        }

        /// <summary>
        /// Blocks the calling thread(s) until the quit event is triggered.
        /// </summary>
        public void WaitForQuitEvent()
        {
            this.quitEvent.WaitOne();
        }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="api">The api to use.</param>
        /// <param name="listener">Http Listener Object.</param>
        /// <param name="server">The server object to use.</param>
        /// <param name="consoleOutFunction">The function used to print to the console.</param>
        private static void HandleRequest( Api api, HttpListener listener, HttpServer server, Action<string> consoleOutFunction )
        {
            while( server.IsListening )
            {
                HttpListenerContext context = null;
                try
                {
                    context = listener.GetContext();
                }
                catch( HttpListenerException err )
                {
                    // Error code 995 means GetContext got aborted (E.g. when shutting down).
                    // If that's the case, just start over.  The while loop will break out and
                    // the thread will exit cleanly.
                    if( err.ErrorCode == 995 )
                    {
                        continue;
                    }
                    else
                    {
                        throw;
                    }
                }

                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                string responseString = string.Empty;

                try
                {
                    // Construct Response.
                    // Taken from https://msdn.microsoft.com/en-us/library/system.net.httplistener.begingetcontext%28v=vs.110%29.aspx
                    string url = request.RawUrl.ToLower();
                    if( ( url == "/" ) || ( url == "/index.html" ) )
                    {
                        responseString = GetHomePageHtml( api );
                    }
                    else if( url == "/logbook.html" )
                    {
                        responseString = GetLogbookHtml( api );
                    }
                    else if( url == "/meditate.html" )
                    {
                        bool isPost = request.HttpMethod == "POST"; // Whether or not this is a post request or not.
                        responseString = HandleMeditateRequest( api, request );
                    }
                    else if( url == "/about.html" )
                    {
                        responseString = GetAboutPage();
                    }
                    else if( url == "/css/meditation_logger.css" )
                    {
                        responseString = GetCss();
                    }
                    else if( url == "/map.html" )
                    {
                        responseString = GetMapHtml( api );
                    }
                    else if( url.EndsWith( ".css" ) || url.EndsWith( ".js" ) )
                    {
                        responseString = GetJsOrCssFile( url );
                        if( string.IsNullOrEmpty( responseString ) )
                        {
                            responseString = Get404Html();
                            response.StatusCode = Convert.ToInt32( HttpStatusCode.NotFound );
                        }
                    }
                    else if( url == "/media/marker-icon.png" )
                    {
                        GetMarkerImage( response.OutputStream );
                    }
                    else if( url == "/export.html" )
                    {
                        responseString = GetExportPage();
                    }
                    else if( url == "/export/logbook.xml" )
                    {
                        XmlExporter.ExportToXml( response.OutputStream, api.LogBook );
                    }
                    else if( url == "/export/logbook.json" )
                    {
                        JsonExporter.ExportJson( response.OutputStream, api.LogBook );
                    }
                    else if( url == "/quit.html" )
                    {
                        if( request.HttpMethod == "POST" )
                        {
                            HandleQuitEvent( server );
                            responseString = "Service Shut down";
                        }
                        else
                        {
                            responseString = Get404Html();
                            response.StatusCode = Convert.ToInt32( HttpStatusCode.NotFound );
                        }
                    }
                    else
                    {
                        responseString = Get404Html();
                        response.StatusCode = Convert.ToInt32( HttpStatusCode.NotFound );
                    }
                }
                catch( Exception e )
                {
                    responseString = GetErrorHtml( e );
                    response.StatusCode = Convert.ToInt32( HttpStatusCode.InternalServerError );

                    consoleOutFunction( "**********" );
                    consoleOutFunction( "Caught Exception when determining response: " + e.Message );
                    consoleOutFunction( e.StackTrace );
                    consoleOutFunction( "**********" );
                }
                finally
                {
                    try
                    {
                        // Only send response if our string is not empty
                        // (Possible for an image, ExportToXml or ExportJson got called and didn't
                        // add the response string).
                        if( responseString.Length > 0 )
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes( responseString );
                            response.ContentLength64 = buffer.Length;
                            response.OutputStream.Write( buffer, 0, buffer.Length );
                        }
                    }
                    catch( Exception e )
                    {
                        consoleOutFunction( "**********" );
                        consoleOutFunction( "Caught Exception when writing response: " + e.Message );
                        consoleOutFunction( e.StackTrace );
                        consoleOutFunction( "**********" );
                    }
                    response.OutputStream.Close();
                }

                consoleOutFunction(
                    request.HttpMethod + " from: " + request.UserHostName + " " + request.UserHostAddress + " " + request.RawUrl + " (" + response.StatusCode + ")"
                );
            }
        }

        // ---- HTML Functions ----

        /// <summary>
        /// Gets the internal server error html.
        /// </summary>
        /// <param name="e">Exception caught.</param>
        /// <returns>The internal server error html.</returns>
        private static string GetErrorHtml( Exception e )
        {
            string html =
                @"<!DOCTYPE html>
<html>
<head>
    <title>Meditation Enthusiasts Logger</title>
    <meta http-equiv=""content-type"" content=""text/html; charset = utf-8"" />
</head>
<body>
    <h1>500: Internal System Error</h1>
    <h2>Error:</h2>
";
            using( StringReader reader = new StringReader( e.Message ) )
            {
                string line;
                while( ( line = reader.ReadLine() ) != null )
                {
                    html += "<p>" + line + "</p>" + Environment.NewLine;
                }
            }

            html += "<h2>Stack Trace:</h2>" + Environment.NewLine;

            using( StringReader reader = new StringReader( e.StackTrace ) )
            {
                string line;
                while( ( line = reader.ReadLine() ) != null )
                {
                    html += "<p>" + line + "</p>" + Environment.NewLine;
                }
            }

            html += @"
</body>
</html>
";
            return html;
        }

        /// <summary>
        /// Get the home page's html to display.
        /// </summary>
        /// <param name="api">Reference to an API object.</param>
        /// <returns>HTML for the home page in a string.</returns>
        private static string GetHomePageHtml( Api api )
        {
            string indexHtmlPath = Path.Combine( "html", "index.html" );
            string html = ReadFile( indexHtmlPath );

            html = html.Replace( "{%TotalMinutes%}", api.LogBook.TotalTime.ToString( "N2" ) );
            html = html.Replace( "{%LongestSession%}", api.LogBook.LongestTime.ToString( "N2" ) );
            html = html.Replace( "{%TotalSessions%}", api.LogBook.Logs.Count.ToString() );

            string latestSesssionString = string.Empty;
            if( api.LogBook.Logs.Count == 0 )
            {
                latestSesssionString = "Nothing yet.";
            }
            else
            {
                latestSesssionString = api.LogBook.Logs[0].StartTime.ToLocalTime().ToString( "MM-dd-yyyy  HH:mm" );
            }

            html = html.Replace( "{%LatestSession%}", latestSesssionString );

            html = AddCommonHtml( html );

            return html;
        }

        /// <summary>
        /// Gets the meditation page's html based on the current state.
        /// </summary>
        /// <param name="api">Reference to an API object.</param>
        /// <param name="request">The http request brought to handle</param>
        /// <returns>HTML for the meditation page in a string.</returns>
        private static string HandleMeditateRequest( Api api, HttpListenerRequest request )
        {
            string errorString = string.Empty;
            if( request.HttpMethod == "POST" )
            {
                errorString = HandleMeditatePostRequest( api, request );
            }

            string html = string.Empty;
            switch( api.CurrentState )
            {
                case Api.ApiState.Idle:
                    html = GetIdleStateHtml();
                    break;

                case Api.ApiState.Started:
                    html = GetStartedStateHtml( api );
                    break;

                case Api.ApiState.Stopped:
                    html = GetEndedStateHtml( api, errorString );
                    break;
            }

            return html;
        }

        /// <summary>
        /// Handles the quit event.
        /// </summary>
        /// <param name="server">server object to quit.</param>
        private static void HandleQuitEvent( HttpServer server )
        {
            server.quitEvent.Set();
        }

        /// <summary>
        /// Handles a post request by changing the API state.
        /// </summary>
        /// <param name="api">Reference to an API object.</param>
        /// <param name="request">The request to handle.</param>
        /// <returns>An error string.  string.Empty for no error.</returns>
        private static string HandleMeditatePostRequest( Api api, HttpListenerRequest request )
        {
            switch( api.CurrentState )
            {
                case Api.ApiState.Idle:
                    // If we are idle, start the session.
                    // For webserver sessions, we don't play music nor do we set a length.
                    SessionConfig config = new SessionConfig();
                    config.PlayMusic = false;
                    config.Length = null;

                    api.StartSession( config );
                    break;

                case Api.ApiState.Started:
                    // If we are started, end the session.
                    api.StopSession();
                    break;

                case Api.ApiState.Stopped:
                    NameValueCollection queryString;
                    using( StreamReader reader = new StreamReader( request.InputStream ) )
                    {
                        queryString = HttpUtility.ParseQueryString( reader.ReadToEnd() );
                    }

                    string technique = queryString.Get( "technique" );
                    string latStr = queryString.Get( "lat" );
                    string longStr = queryString.Get( "lon" );
                    string comments = queryString.Get( "comments" );

                    decimal? lat = null;
                    decimal? lon = null;

                    decimal parsedLat;
                    if( decimal.TryParse( latStr, out parsedLat ) )
                    {
                        lat = parsedLat;
                    }

                    decimal parsedLon;
                    if( decimal.TryParse( longStr, out parsedLon ) )
                    {
                        lon = parsedLon;
                    }

                    // If we are stopped, save the session based on the information from the post request.

                    try
                    {
                        api.ValidateAndSaveSession( technique, comments, lat, lon );
                        api.PopulateLogbook();
                    }
                    catch( LogValidationException err )
                    {
                        return err.Message;
                    }
                    break;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the idle state HTML.
        /// </summary>
        /// <returns>The idle state html for the meditate page.</returns>
        private static string GetIdleStateHtml()
        {
            string idleHtml = Path.Combine( "html", "start.html" );
            return ReadFile( idleHtml );
        }

        /// <summary>
        /// Gets the started state HTML.
        /// </summary>
        /// <param name="api">Reference to the API object.</param>
        /// <returns>The started state html for the meditate page.</returns>
        private static string GetStartedStateHtml( Api api )
        {
            string startHtml = Path.Combine( "html", "meditate.html" );
            string html = ReadFile( startHtml );

            // In case the user navigates from the page or the browser crashes,
            // get the diff between now and when we started the session.
            // Make the total seconds of that diff the starting time on the webpage
            // we send back.
            TimeSpan diff = DateTime.UtcNow - api.CurrentLog.StartTime;
            html = html.Replace( "{%startingSeconds%}", diff.TotalSeconds.ToString() );
            return html;
        }

        /// <summary>
        /// Gets the ended state HTML.
        /// </summary>
        /// <param name="api">Reference to the API object.</param>
        /// <param name="errorString">The error string to display (if any).</param>
        /// <returns>The ended state html for the meditate page.</returns>
        private static string GetEndedStateHtml( Api api, string errorString )
        {
            string endHtml = Path.Combine( "html", "end.html" );
            string html = ReadFile( endHtml );

            // Insert the total minutes meditated to the html.
            if( string.IsNullOrEmpty( errorString ) )
            {
                html = html.Replace( "{%ErrorString%}", string.Empty );
            }
            else
            {
                html = html.Replace( "{%ErrorString%}", "Error: " + errorString );
            }
            html = html.Replace( "{%minutesMeditated%}", api.CurrentLog.Duration.TotalMinutes.ToString( "N2" ) );
            return html;
        }

        /// <summary>
        /// Gets the logbook page's html to display.
        /// </summary>
        /// <param name="api">Reference to an API object.</param>
        /// <returns>HTML for the logbook page in a string.</returns>
        private static string GetLogbookHtml( Api api )
        {
            string logbookPath = Path.Combine( "html", "logbook.html" );
            string html = ReadFile( logbookPath );

            string logHtml = string.Empty;
            if( api.LogBook.Logs.Count == 0 )
            {
                logHtml = "<p>No Sessions Yet.</p>";
            }
            else
            {
                foreach( ILog log in api.LogBook.Logs )
                {
                    logHtml +=
@"
        <div class=""logbookEntry"">
            <p><strong>" + log.StartTime.ToLocalTime().ToString( "MM-dd-yyyy HH:mm" ) + @"</strong></p>
            <table cellspacing=""0"" cellpadding=""5px"">
                <tr>
                    <td><strong>Duration:</strong></td>
                    <td>" + log.Duration.TotalMinutes.ToString( "F", CultureInfo.InvariantCulture ) + @" minutes</td>
                </tr>
                <tr>
                    <td><strong>Technique:</strong></td>
                    <td>" + log.Technique + @"</td>
                </tr>
                <tr>
                    <td><strong>Comments:</strong></td>
                </tr>
            </table>
            <div>
                <blockquote>
                    " + log.Comments + @"
                </blockquote>
            </div>
        </div>
";
                }
            }

            html = html.Replace( "{%LogbookTable%}", logHtml );

            return html;
        }

        /// <summary>
        /// Gets the map view HTML.
        /// </summary>
        /// <param name="api">The API object.</param>
        /// <returns>The html of the map page.</returns>
        private static string GetMapHtml( Api api )
        {
            string mapPath = Path.Combine( "html", "map.html" );

            string html = ReadFile( mapPath );
            html = html.Replace( "{%leaflet_data%}", LeafletJS.GetMarkerHtml( api ) );

            return html;
        }

        private static Regex cssPattern = new Regex( @"/(?<jsOrCss>(js|css))/(?<pure>pure/)?(?<file>[\w-\d]+\.(css|js))", RegexOptions.Compiled );

        /// <summary>
        /// Reads in the given JS or CSS file.
        /// </summary>
        /// <param name="url">The URL to grab.</param>
        /// <returns>The CSS or JS contents.</returns>
        private static string GetJsOrCssFile( string url )
        {
            string filePath;
            Match cssMatch = cssPattern.Match( url );
            if( cssMatch.Success )
            {
                filePath = Path.Combine(
                    "html",
                    cssMatch.Groups["jsOrCss"].Value,
                    cssMatch.Groups["pure"].Value,
                    cssMatch.Groups["file"].Value
                );
            }
            else
            {
                return string.Empty;
            }

            return ReadFile( filePath );
        }

        /// <summary>
        /// Reads the common header html from the file system and returns the contents.
        /// </summary>
        /// <returns>The common header html.</returns>
        private static string GetCommonHeaderHtml()
        {
            string filePath = Path.Combine( "html", "common_head.html" );
            return ReadFile( filePath );
        }

        /// <summary>
        /// Reads the navbar html from the file system and returns the contents.
        /// </summary>
        /// <returns>The nav var html.</returns>
        private static string GetNavbarHtml()
        {
            string filePath = Path.Combine( "html", "navbar.html" );
            return ReadFile( filePath );
        }

        /// <summary>
        /// Gets the text of the marker image.
        /// </summary>
        /// <param name="stream">The http response stream to write the image to.</param>
        private static void GetMarkerImage( Stream stream )
        {
            string markerPath = Path.Combine( "media", "marker-icon.png" );
            using( FileStream inFile = new FileStream( markerPath, FileMode.Open, FileAccess.Read ) )
            {
                using( BinaryReader br = new BinaryReader( inFile ) )
                {
                    while( br.BaseStream.Position < br.BaseStream.Length )
                    {
                        stream.WriteByte( br.ReadByte() );
                    }
                }
            }
        }

        /// <summary>
        /// Gets the export page HTML.
        /// </summary>
        /// <returns>The HTML for the export page.</returns>
        private static string GetExportPage()
        {
            string exportPath = Path.Combine( "html", "export.html" );
            string html = ReadFile( exportPath );

            html = AddCommonHtml( html );

            return html;
        }

        /// <summary>
        /// Gets the about page HTML.
        /// </summary>
        /// <returns>The HTML for the about page.</returns>
        private static string GetAboutPage()
        {
            string aboutHtmlPath = Path.Combine( "html", "about.html" );
            string html = ReadFile( aboutHtmlPath );

            html = html.Replace( "{%VersionString%}", Api.VersionString );

            return html;
        }

        /// <summary>
        /// Gets the 404 html.
        /// </summary>
        /// <returns>HTML for the 404 page.</returns>
        private static string Get404Html()
        {
            return
@"
<!doctype html>
<head>
    <meta http-equiv=""content-type"" content=""text/html; charset=utf-8"" />
    <title>Meditation Logger.  Not Found.</title>
    <style>
        body
        {
            background-color:ffffff
        }
    </style>
</head>
<body>

        <h1>404 Not Found</h1>

</body>
</html>
";
        }

        /// <summary>
        /// Gets the CSS file.
        /// </summary>
        /// <returns>The CSS string.</returns>
        private static string GetCss()
        {
            string cssFile = Path.Combine( "html", "css", "meditation_logger.css" );
            return ReadFile( cssFile );
        }

        /// <summary>
        /// Reads the given file to the end.
        /// Returns string.Empty if the file does not exist.
        /// </summary>
        /// <param name="path">The path to read.</param>
        /// <returns>The string of the file.</returns>
        private static string ReadFile( string path )
        {
            string fileConents = string.Empty;
            if( File.Exists( path ) )
            {
                using( StreamReader inFile = new StreamReader( path ) )
                {
                    fileConents = inFile.ReadToEnd();
                }
            }

            return fileConents;
        }

        /// <summary>
        /// Adds common HTML to the given HTML and returns the modified
        /// HTML with the common HTML merged in it.
        /// </summary>
        /// <param name="html">The HTML that needs common things added to it.</param>
        /// <returns>The same HTML passed in, but with common things added.</returns>
        private static string AddCommonHtml( string html )
        {
            html = html.Replace( "{%NavBar%}", GetNavbarHtml() );
            html = html.Replace( "{%CommonHead%}", GetCommonHeaderHtml() );

            return html;
        }
    }
}
