// 
// Meditation Logger.
// Copyright (C) 2015-2017  Seth Hendrick.
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
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using MedEnthLogsApi;

namespace MedEnthLogsApi
{
    public class HtmlInfo
    {
        // ---------------- Properties ----------------

        /// <summary>
        /// The URL that was used to get the HTML info.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The HTML Text that needs to be returned to the server.
        /// </summary>
        public string HtmlText { get; set; }

        /// <summary>
        /// The method that was used to generate the request.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The response to send back to the client.
        /// </summary>
        public byte[] ResponseBuffer { get; set; }

        /// <summary>
        /// The status code 
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The content type of the HTML.
        /// </summary>
        public string ContentType { get; set; }
    }

    /// <summary>
    /// This class makes the HTML That is rendered to the user.
    /// </summary>
    public class HttpResponseHandler
    {
        // ---------------- Fields ----------------

        /// <summary>
        /// Event to run when a quit event occurs.
        /// </summary>
        public event Action QuitEvent;

        /// <summary>
        /// The api to use.
        /// </summary>
        private Api api;

        private static Regex cssPattern =
            new Regex( @"/(?<jsOrCss>(js|css))/(?<pure>pure/)?(?<file>[\w-\d]+\.(css|js))", RegexOptions.Compiled );

        // ---------------- Constructor ----------------

        /// <summary>
        /// Constructor.s
        /// </summary>
        /// <param name="api">The API to use.</param>
        public HttpResponseHandler( Api api )
        {
            this.api = api;
        }

        // ---------------- Functions ----------------

        /// <summary>
        /// Gets what to send to the client.
        /// </summary>
        /// <param name="url">The URL to get.</param>
        /// <param name="method">The method being used.</param>
        /// <param name="httpQueryString">The HTTP query string that was sent, if any.</param>
        /// <returns>The HTML info which can be sent to the client.</returns>
        public HtmlInfo GetHtmlInfo( string url, string method, NameValueCollection httpQueryString )
        {
            url = url.ToLower();

            HtmlInfo info = new HtmlInfo();
            info.Url = url;
            info.Method = method;

            // We'll default to OK and text/html unless otherwise changed.
            info.StatusCode = HttpStatusCode.OK;
            info.ContentType = "text/html";

            GenerateResponse( info, url, method, httpQueryString );

            return info;
        }

        private HtmlInfo GenerateResponse( HtmlInfo info, string url, string method, NameValueCollection httpQueryString )
        {
            // Construct Response.
            // Taken from https://msdn.microsoft.com/en-us/library/system.net.httplistener.begingetcontext%28v=vs.110%29.aspx
            if( ( url == "/" ) || ( url == "/index.html" ) )
            {
                info.ResponseBuffer = Encoding.UTF8.GetBytes( GetHomePageHtml() );
            }
            else if( url == "/logbook.html" )
            {
                info.ResponseBuffer = Encoding.UTF8.GetBytes( GetLogbookHtml() );
            }
            else if( url == "/meditate.html" )
            {
                info.ResponseBuffer = Encoding.UTF8.GetBytes( HandleMeditateRequest( method, httpQueryString ) );
            }
            else if( url == "/about.html" )
            {
                info.ResponseBuffer = Encoding.UTF8.GetBytes( GetAboutPage() );
            }
            else if( url == "/css/meditation_logger.css" )
            {
                info.ResponseBuffer = Encoding.UTF8.GetBytes( GetCss() );
            }
            else if( url == "/map.html" )
            {
                info.ResponseBuffer = Encoding.UTF8.GetBytes( GetMapHtml( api ) );
            }
            else if( url.EndsWith( ".css" ) || url.EndsWith( ".js" ) )
            {
                string responseString = GetJsOrCssFile( url );
                if( string.IsNullOrEmpty( responseString ) )
                {
                    info.ResponseBuffer = Encoding.UTF8.GetBytes( Get404Html() );
                    info.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    info.ResponseBuffer = Encoding.UTF8.GetBytes( responseString );
                    if( url.EndsWith( ".css" ) )
                    {
                        info.ContentType = "text/css";
                    }
                    else
                    {
                        info.ContentType = "text/javascript";
                    }
                }
            }
            else if( url == "/media/marker-icon.png" )
            {
                GetMarkerImage( info );
            }
            else if( url == "/export.html" )
            {
                info.ResponseBuffer = Encoding.UTF8.GetBytes( GetExportPage() );
            }
            else if( url == "/export/logbook.xml" )
            {
                using( MemoryStream ms = new MemoryStream() )
                {
                    XmlExporter.ExportToXml( ms, api.LogBook );
                    info.ResponseBuffer = ms.ToArray();
                    info.ContentType = "text/xml";
                }
            }
            else if( url == "/export/logbook.json" )
            {
                using( MemoryStream ms = new MemoryStream() )
                {
                    JsonExporter.ExportJson( ms, api.LogBook );
                    info.ResponseBuffer = ms.ToArray();
                    info.ContentType = "text/json";
                }
            }
            else if( url == "/about/credits.txt" )
            {
                info.ResponseBuffer = Encoding.UTF8.GetBytes( GetCredits() );
                info.ContentType = "text/plain";
            }
            else if( url == "/about/license.txt" )
            {
                info.ResponseBuffer = Encoding.UTF8.GetBytes( GetLicense() );
                info.ContentType = "text/plain";
            }
            else if( url == "/quit.html" )
            {
                if( method == "POST" )
                {
                    HandleQuitEvent();
                    info.ResponseBuffer = Encoding.UTF8.GetBytes( "Service Shut down" );
                    info.ContentType = "text/plain";
                }
                else
                {
                    info.ResponseBuffer = Encoding.UTF8.GetBytes( Get404Html() );
                    info.StatusCode = HttpStatusCode.NotFound;
                }
            }
            else
            {
                info.ResponseBuffer = Encoding.UTF8.GetBytes( Get404Html() );
                info.StatusCode = HttpStatusCode.NotFound;
            }

            return info;
        }

        // ---- HTML Functions ----

        /// <summary>
        /// Gets the internal server error html.
        /// </summary>
        /// <param name="e">Exception caught.</param>
        /// <returns>The internal server error html.</returns>
        public static string GetErrorHtml( Exception e )
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
        private string GetHomePageHtml()
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
        /// <param name="queryString">The HTTP query string for the post request, if any.</param>
        /// <returns>HTML for the meditation page in a string.</returns>
        private string HandleMeditateRequest( string method, NameValueCollection queryString )
        {
            string errorString = string.Empty;
            if( method == "POST" )
            {
                errorString = HandleMeditatePostRequest( queryString );
            }

            string html = string.Empty;
            switch( api.CurrentState )
            {
                case Api.ApiState.Idle:
                    html = GetIdleStateHtml();
                    break;

                case Api.ApiState.Started:
                    html = GetStartedStateHtml();
                    break;

                case Api.ApiState.Stopped:
                    html = GetEndedStateHtml( errorString );
                    break;
            }

            return html;
        }

        /// <summary>
        /// Handles the quit event.
        /// </summary>
        private void HandleQuitEvent()
        {
            this.QuitEvent?.Invoke();
        }

        /// <summary>
        /// Handles a post request by changing the API state.
        /// </summary>
        /// <param name="queryString">The HTTP query string for the post request, if any.</param>
        /// <returns>An error string.  string.Empty for no error.</returns>
        private string HandleMeditatePostRequest( NameValueCollection queryString )
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
        private string GetIdleStateHtml()
        {
            string idleHtml = Path.Combine( "html", "start.html" );

            string html = ReadFile( idleHtml );
            html = AddCommonHtml( html );

            return html;
        }

        /// <summary>
        /// Gets the started state HTML.
        /// </summary>
        /// <returns>The started state html for the meditate page.</returns>
        private string GetStartedStateHtml()
        {
            string startHtml = Path.Combine( "html", "meditate.html" );
            string html = ReadFile( startHtml );

            // In case the user navigates from the page or the browser crashes,
            // get the diff between now and when we started the session.
            // Make the total seconds of that diff the starting time on the webpage
            // we send back.
            TimeSpan diff = DateTime.UtcNow - api.CurrentLog.StartTime;
            html = html.Replace( "{%startingSeconds%}", diff.TotalSeconds.ToString() );
            html = AddCommonHtml( html );
            return html;
        }

        /// <summary>
        /// Gets the ended state HTML.
        /// </summary>
        /// <param name="errorString">The error string to display (if any).</param>
        /// <returns>The ended state html for the meditate page.</returns>
        private string GetEndedStateHtml( string errorString )
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
            html = AddCommonHtml( html );
            return html;
        }

        /// <summary>
        /// Gets the logbook page's html to display.
        /// </summary>
        /// <returns>HTML for the logbook page in a string.</returns>
        private string GetLogbookHtml()
        {
            string logbookPath = Path.Combine( "html", "logbook.html" );
            string html = ReadFile( logbookPath );
            html = AddCommonHtml( html );

            string logHtml = string.Empty;
            if( api.LogBook.Logs.Count == 0 )
            {
                logHtml = "<p>No Sessions Yet.</p>";
            }
            else
            {
                foreach( ILog log in api.LogBook.Logs )
                {
                    string listCommentString = log.Comments;
                    if( listCommentString.Length >= 100 )
                    {
                        listCommentString = listCommentString.Substring( 0, 100 ) + "...";
                    }

                    // Show the technique's first letter in a window so it could easily be eye-balled.
                    char techniqueLetter;
                    if( string.IsNullOrEmpty( log.Technique ) )
                    {
                        techniqueLetter = '-';
                    }
                    else
                    {
                        techniqueLetter = log.Technique[0];
                    }

                    logHtml +=
@"
        <div class=""email-item pure-g"" id=""" + log.Guid + @""" onclick=""logSelected('" + log.Guid + @"');"">
            <div class=""pure-u"" style=""padding-right:0.5em;""> <!-- Minutes in a small div -->
                <h2>" + char.ToUpper( techniqueLetter ) + @"</h2>
            </div>
            <div class=""pure-u-3-4"">
                <h5 class=""email-name"">" + log.StartTime.ToLocalTime().ToString( "MM-dd-yyyy HH:mm" ) + @"</h5>
                <h4 class=""email-subject"">" + log.Duration.TotalMinutes.ToString( "F", CultureInfo.InvariantCulture ) + @" min.</h4>
                <p class=""email-desc"">" + listCommentString + @"</p>
            </div>
        </div>
";
                }
            }

            string jsData =
@"<script>
    var logData = " + JsonExporter.ExportJsonToString( api.LogBook ) + @"
</script>";
            html = html.Replace( "{%LogbookList%}", logHtml );
            html = html.Replace( "{%LogbookData%}", jsData );

            if( api.LogBook.Logs.Count == 0 )
            {
                html = html.Replace( "{%MainTechnique%}", "-" );
                html = html.Replace( "{%MainDuration%}", "-" );
                html = html.Replace( "{%MainStartTime%}", "-" );
                html = html.Replace( "{%MainEndTime%}", "-" );
                html = html.Replace( "{%MainComments%}", string.Empty );
            }
            else
            {
                ILog firstLog = api.LogBook.Logs[0];
                string techniqueString;
                if( string.IsNullOrEmpty( firstLog.Technique ) )
                {
                    techniqueString = "Unknown Technique";
                }
                else
                {
                    techniqueString = firstLog.Technique;
                }
                html = html.Replace( "{%MainTechnique%}", techniqueString );
                html = html.Replace( "{%MainDuration%}", firstLog.Duration.TotalMinutes.ToString( "F", CultureInfo.InvariantCulture ) );
                html = html.Replace( "{%MainStartTime%}", firstLog.StartTime.ToLocalTime().ToString( "MM-dd-yyyy HH:mm" ) );
                html = html.Replace( "{%MainEndTime%}", firstLog.EndTime.ToLocalTime().ToString( "MM-dd-yyyy HH:mm" ) );
                html = html.Replace( "{%MainComments%}", firstLog.Comments );
            }

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
            html = AddCommonHtml( html );
            html = html.Replace( "{%leaflet_data%}", LeafletJS.GetMarkerHtml( api ) );

            return html;
        }

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
        /// <param name="info">The Html Info to update.</param>
        private static void GetMarkerImage( HtmlInfo info )
        {
            string markerPath = Path.Combine( "media", "marker-icon.png" );
            GetImage( markerPath, info );
            info.ContentType = "image/png";
        }

        private static void GetImage( string path, HtmlInfo info )
        {
            if( File.Exists( path ) == false )
            {
                info.ResponseBuffer = Encoding.UTF8.GetBytes( Get404Html() );
                info.StatusCode = HttpStatusCode.NotFound;
                return;
            }

            byte[] pictureContents;

            using( MemoryStream ms = new MemoryStream() )
            {
                using( GZipStream gzip = new GZipStream( ms, CompressionMode.Compress, true ) )
                {
                    using( BinaryReader br = new BinaryReader( File.Open( path, FileMode.Open, FileAccess.Read ) ) )
                    {
                        byte[] buffer = br.ReadBytes( 1028 );
                        while( buffer.Length > 0 )
                        {
                            gzip.Write( buffer, 0, buffer.Length );
                            buffer = br.ReadBytes( 1028 );
                        }
                    }
                }

                pictureContents = ms.ToArray();
            }

            info.ResponseBuffer = pictureContents;
        }

        /// <summary>
        /// Gets the license text.
        /// </summary>
        /// <returns>The license text.</returns>
        private static string GetLicense()
        {
            return License.MedEnthLicense;
        }

        /// <summary>
        /// Gets the credits text.
        /// </summary>
        /// <returns>The credits text.</returns>
        private static string GetCredits()
        {
            return License.ExternalLicenses;
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
            html = AddCommonHtml( html );

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
