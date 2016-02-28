﻿// 
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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            if ( HttpListener.IsSupported == false )
            {
                throw new PlatformNotSupportedException(
                    "This platform does not support HTTP Listeners..."
                );
            }
            if ( api.IsOpen == false )
            {
                throw new ArgumentException(
                    "Api is not open. Please open it before calling this constructor...",
                    nameof( api )
                );
            }
            if ( consoleOutFunction == null )
            {
                this.cout = delegate ( string s ) { };
            }

            this.api = api;
            this.cout = consoleOutFunction;
            this.listener = new HttpListener();

            this.listener.Prefixes.Add( "http://*:" + port + "/" );

            this.listeningThread = new Thread(
                delegate()
                {
                    HandleRequest( this.api, this.listener, cout );
                }
            );
        }

        // -------- Properties --------

        /// <summary>
        /// Whether or not we are listening.
        /// </summary>
        public bool IsListening
        {
            get
            {
                return this.listener.IsListening;
            }
        }

        // -------- Functions --------

        /// <summary>
        /// Opens the websocket and listens for requests.
        /// No-op if already listening.
        /// </summary>
        public void Start()
        {
            // No-op if we are not listening.
            if ( IsListening == false )
            {
                this.listener.Start();
                this.listeningThread.Start();
            }
        }

        /// <summary>
        /// Closes the web socket and disposes this class.
        /// No-op if not already listening.
        /// </summary>
        public void Dispose()
        {
            // No-op if we are not listening.
            if ( IsListening )
            {
                if ( this.cout != null )
                {
                    this.cout( "Terminating server..." );
                }
                this.listener.Stop();
                this.listener.Close();
                this.listeningThread.Join();
                if ( this.cout != null )
                {
                    this.cout( "Terminating server...Done!" );
                }
            }
        }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="api">The api to use.</param>
        /// <param name="listener">Http Listener Object.</param>
        /// <param name="consoleOutFunction">The function used to print to the console.</param>
        private static void HandleRequest( Api api, HttpListener listener, Action<string> consoleOutFunction )
        {
            while ( listener.IsListening )
            {
                HttpListenerContext context = null;
                try
                {
                    context = listener.GetContext();
                }
                catch ( HttpListenerException err )
                {
                    // Error code 995 means GetContext got aborted (E.g. when shutting down).
                    // If that's the case, just start over.  The while loop will break out and
                    // the thread will exit cleanly.
                    if ( err.ErrorCode == 995 )
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

                try
                {
                    // Construct Response.
                    // Taken from https://msdn.microsoft.com/en-us/library/system.net.httplistener.begingetcontext%28v=vs.110%29.aspx

                    string responseString = string.Empty;
                    string url = request.RawUrl.ToLower();
                    if ( ( url == "/" ) || ( url == "/index.html" ) )
                    {
                        responseString = GetHomePageHtml( api );
                    }
                    else if ( url == "/about.html" )
                    {
                        responseString = GetAboutPage();
                    }
                    else if ( url == "/css/meditation_logger.css" )
                    {
                        responseString = GetCss();
                    }
                    else if ( url == "/map.html" )
                    {
                        responseString = GetMapHtml( api );
                    }
                    else if ( url == "/media/marker-icon.png" )
                    {
                        GetMarkerImage( response.OutputStream );
                    }
                    else if ( url == "/export.html" )
                    {
                        responseString = GetExportPage();
                    }
                    else if ( url == "/export/logbook.xml" )
                    {
                        XmlExporter.ExportToXml( response.OutputStream, api.LogBook );
                    }
                    else if ( url == "/export/logbook.json" )
                    {
                        JsonExporter.ExportJson( response.OutputStream, api.LogBook );
                    }
                    else
                    {
                        responseString = Get404Html();
                        response.StatusCode = Convert.ToInt32( HttpStatusCode.NotFound );
                    }

                    // Only send response if our string is not empty
                    // (Possible for an image, ExportToXml or ExportJson got called and didn't
                    // add the response string).
                    if ( responseString.Length > 0 )
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes( responseString );
                        response.ContentLength64 = buffer.Length;
                        response.OutputStream.Write( buffer, 0, buffer.Length );
                    }
                }
                catch ( Exception e )
                {
                    byte[] buffer = Encoding.UTF8.GetBytes( GetErrorHtml( e ) );
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write( buffer, 0, buffer.Length );
                    response.StatusCode = Convert.ToInt32( HttpStatusCode.InternalServerError );
                }
                finally
                {
                    response.OutputStream.Close();
                }

                consoleOutFunction(
                    "Request from: " + request.UserHostName + " " + request.UserHostAddress + " " + request.RawUrl + " (" + response.StatusCode + ")"
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
            using ( StringReader reader = new StringReader( e.Message ) )
            {
                string line;
                while ( ( line = reader.ReadLine() ) != null )
                {
                    html += "<p>" + line + "</p>" + Environment.NewLine;
                }
            }

            html += "<h2>Stack Trace:</h2>" + Environment.NewLine;

            using ( StringReader reader = new StringReader( e.StackTrace ) )
            {
                string line;
                while ( ( line = reader.ReadLine() ) != null )
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
        /// <returns>HTML for the home page in a string.</returns>
        private static string GetHomePageHtml( Api api )
        {
            string indexHtmlPath = Path.Combine( "html", "index.html" );
            string html = string.Empty;
            using ( StreamReader inFile = new StreamReader( indexHtmlPath ) )
            {
                html = inFile.ReadToEnd();
            }

            html = html.Replace( "{%TotalMinutes%}", api.LogBook.TotalTime.ToString( "N2" ) );
            html = html.Replace( "{%LongestSession%}", api.LogBook.LongestTime.ToString( "N2" ) );
            html = html.Replace( "{%TotalSessions%}", api.LogBook.Logs.Count.ToString() );

            string latestSesssionString = string.Empty;
            if ( api.LogBook.Logs.Count == 0 )
            {
                latestSesssionString = "Nothing yet.";
            }
            else
            {
                latestSesssionString = api.LogBook.Logs[0].StartTime.ToLocalTime().ToString( "MM-dd-yyyy  HH:mm" );
            }

            html = html.Replace( "{%LatestSession%}", latestSesssionString );

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
            string html = string.Empty;
            using ( StreamReader inFile = new StreamReader( mapPath ) )
            {
                html = inFile.ReadToEnd();
            }

            html = html.Replace( "{%leaflet_css%}", LeafletJS.CSS );
            html = html.Replace( "{%leaflet_js%}", LeafletJS.JavaScript );
            html = html.Replace( "{%leaflet_data%}", LeafletJS.GetMarkerHtml( api ) );

            return html;
        }

        /// <summary>
        /// Gets the text of the marker image.
        /// </summary>
        /// <param name="stream">The http response stream to write the image to.</param>
        private static void GetMarkerImage( Stream stream )
        {
            string markerPath = Path.Combine( "media", "marker-icon.png" );
            using ( FileStream inFile = new FileStream( markerPath, FileMode.Open, FileAccess.Read ) )
            {
                using ( BinaryReader br = new BinaryReader( inFile ) )
                {
                    while ( br.BaseStream.Position < br.BaseStream.Length )
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
            string html = string.Empty;
            using ( StreamReader inFile = new StreamReader( exportPath ) )
            {
                html = inFile.ReadToEnd();
            }

            return html;
        }

        /// <summary>
        /// Gets the about page HTML.
        /// </summary>
        /// <returns>The HTML for the about page.</returns>
        private static string GetAboutPage()
        {
            string indexHtmlPath = Path.Combine( "html", "about.html" );
            string html = string.Empty;
            using ( StreamReader inFile = new StreamReader( indexHtmlPath ) )
            {
                html = inFile.ReadToEnd();
            }

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
            string indexHtmlPath = Path.Combine( "html", "css", "meditation_logger.css" );
            string css = string.Empty;
            using ( StreamReader inFile = new StreamReader( indexHtmlPath ) )
            {
                css = inFile.ReadToEnd();
            }
            return css;
        }
    }
}