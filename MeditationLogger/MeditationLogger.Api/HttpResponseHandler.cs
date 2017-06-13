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
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using MedEnthLogsApi.Razor;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace MeditationEnthusiasts.MeditationLogger.Api
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
            new Regex( @"/(?<jsOrCss>(js|css))/(?<dir>(pure/)|(fullcalendar/)|(qtip/))?(?<file>[\w-\d\.]+\.(css|js))", RegexOptions.Compiled );

        // -------- Page Names --------

        public const string IndexUrl = "/index.html";
        public const string MeditateUrl = "/meditate.html";
        public const string LogbookUrl = "/logbook.html";
        public const string MapUrl = "/map.html";
        public const string CalendarUrl = "/calendar.html";
        public const string ExportUrl = "/export.html";
        public const string ExportXmlUrl = "/export/logbook.xml";
        public const string ExportJsonUrl = "/export/logbook.json.xml";
        public const string CreditsUrl = "/about/credits.html";
        public const string LicenseUrl = "/about/license.txt";
        public const string AboutUrl = "/about.html";

        // -------- Templates --------

        /// <summary>
        /// Template for the common head html.
        /// </summary>
        private string commonHeadTemplate;

        /// <summary>
        /// Template for the nav bar.
        /// </summary>
        private string navBarTemplate;

        // -------- Constant Page Returns --------

        // These pages will NEVER change once compiled.  Might as well
        // cache them.

        private string meditateStartPageHtml;
        private string exportPageHtml;
        private string aboutPageHtml;
        private string creditsPageHtml;

        // -------- Razor Keys --------

        // These are keys to the razor engine templates that are NOT page names.

        private const string startMeditateKey = "start" + MeditateUrl;
        private const string endMeditateKey = "end" + MeditateUrl;

        // ---------------- Constructor ----------------

        /// <summary>
        /// Constructor.s
        /// </summary>
        /// <param name="api">The API to use.</param>
        public HttpResponseHandler( Api api )
        {
            this.api = api;
            TemplateServiceConfiguration config = new TemplateServiceConfiguration();
            config.Language = Language.CSharp;
            config.EncodedStringFactory = new HtmlEncodedStringFactory();
            config.BaseTemplateType = typeof( RawHtmlTemplate<> );

            IRazorEngineService service = RazorEngineService.Create( config );
            Engine.Razor = service;
#if DEBUG
            config.Debug = true;
#endif
        }

        // ---------------- Functions ----------------

        /// <summary>
        /// Inits all the templates.
        /// </summary>
        public void InitTemplates()
        {
            this.commonHeadTemplate = GetCommonHeaderHtml();
            this.navBarTemplate = GetNavbarHtml();

            // Compile Templates

            // Compile the index page.
            {
                string indexPageTemplate = ReadFile( Path.Combine( "html", "index.cshtml" ) );
                Engine.Razor.Compile(
                    indexPageTemplate,
                    IndexUrl,
                    null
                );
            }

            // Meditate start page will NEVER change, just cache it.
            {
                string meditateStartPageTemplate = ReadFile( Path.Combine( "html", "start.cshtml" ) );
                this.meditateStartPageHtml = Engine.Razor.RunCompile(
                    meditateStartPageTemplate,
                    startMeditateKey,
                    null,
                    new
                    {
                        CommonHead = this.commonHeadTemplate,
                        NavBar = this.navBarTemplate
                    }
                );
            }

            // Compile the meditate page.
            {
                string medtatePageTemplate = ReadFile( Path.Combine( "html", "meditate.cshtml" ) );
                Engine.Razor.Compile(
                    medtatePageTemplate,
                    MeditateUrl,
                    null
                );
            }

            // Compile the finish meditating page.
            {
                string meditateEndPageTemplate = ReadFile( Path.Combine( "html", "end.cshtml" ) );
                Engine.Razor.Compile(
                    meditateEndPageTemplate,
                    endMeditateKey,
                    null
                );
            }

            // Compile the logbook page
            {
                string logbookTemplate = ReadFile( Path.Combine( "html", "logbook.cshtml" ) );
                Engine.Razor.Compile(
                    logbookTemplate,
                    LogbookUrl,
                    null
                );
            }

            // Compile the map page.
            {
                string mapTemplate = ReadFile( Path.Combine( "html", "map.cshtml" ) );
                Engine.Razor.Compile(
                    mapTemplate,
                    MapUrl,
                    null
                );
            }

            // Compile the calendar page.
            {
                string calTemplate = ReadFile( Path.Combine( "html", "calendar.cshtml" ) );
                Engine.Razor.Compile(
                    calTemplate,
                    CalendarUrl,
                    null
                );
            }

            // Export page will NEVER change, just cache it.
            {
                string exportPageTemplate = ReadFile( Path.Combine( "html", "export.cshtml" ) );
                this.exportPageHtml = Engine.Razor.RunCompile(
                    exportPageTemplate,
                    ExportUrl,
                    null,
                    new
                    {
                        CommonHead = this.commonHeadTemplate,
                        NavBar = this.navBarTemplate
                    }
                );
            }

            // About page will NEVER change, just cache it.
            {
                string aboutPageTemplate = ReadFile( Path.Combine( "html", "about.cshtml" ) );
                this.aboutPageHtml = Engine.Razor.RunCompile(
                    aboutPageTemplate,
                    AboutUrl,
                    null,
                    new
                    {
                        CommonHead = this.commonHeadTemplate,
                        NavBar = this.navBarTemplate,
                        VersionString = Api.VersionString
                    }
                );
            }

            // Credits page will NEVER change, just cache it.
            {
                string creditsTemplate = ReadFile( Path.Combine( "html", "credits.cshtml" ) );
                this.creditsPageHtml = Engine.Razor.RunCompile(
                    creditsTemplate,
                    CreditsUrl,
                    null,
                    new
                    {
                        CommonHead = this.commonHeadTemplate,
                        NavBar = this.navBarTemplate
                    }
                );
            }
        }

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
            if( ( url == "/" ) || ( url == IndexUrl ) )
            {
                info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( GetHomePageHtml() );
            }
            else if( url == LogbookUrl )
            {
                info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( GetLogbookHtml() );
            }
            else if( url == MeditateUrl )
            {
                info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( HandleMeditateRequest( method, httpQueryString ) );
            }
            else if( url == AboutUrl )
            {
                info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( this.aboutPageHtml );
            }
            else if( url == "/css/meditation_logger.css" )
            {
                info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( GetCss() );
            }
            else if( url == MapUrl )
            {
                info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( GetMapHtml() );
            }
            else if( url == CalendarUrl )
            {
                info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( GetCalendarHtml() );
            }
            else if( url.EndsWith( ".css" ) || url.EndsWith( ".js" ) )
            {
                string responseString = GetJsOrCssFile( url );
                if( string.IsNullOrEmpty( responseString ) )
                {
                    info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( Get404Html() );
                    info.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( responseString );
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
            else if( url == ExportUrl )
            {
                info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( this.exportPageHtml );
            }
            else if( url == ExportXmlUrl )
            {
                using( MemoryStream ms = new MemoryStream() )
                {
                    XmlExporter.ExportToXml( ms, api.LogBook );
                    info.ResponseBuffer = ms.ToArray();
                    info.ContentType = "text/xml";
                }
            }
            else if( url == ExportJsonUrl )
            {
                using( MemoryStream ms = new MemoryStream() )
                {
                    JsonExporter.ExportJson( ms, api.LogBook );
                    info.ResponseBuffer = ms.ToArray();
                    info.ContentType = "text/json";
                }
            }
            else if( url == CreditsUrl )
            {
                info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( this.creditsPageHtml );
            }
            else if( url == LicenseUrl )
            {
                info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( License.MedEnthLicense );
                info.ContentType = "text/plain";
            }
            else if( url == "/quit.html" )
            {
                if( method == "POST" )
                {
                    HandleQuitEvent();
                    info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( "Service Shut down" );
                    info.ContentType = "text/plain";
                }
                else
                {
                    info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( Get404Html() );
                    info.StatusCode = HttpStatusCode.NotFound;
                }
            }
            else
            {
                info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( Get404Html() );
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
        /// <returns>HTML for the home page in a string.</returns>
        private string GetHomePageHtml()
        {
            return Engine.Razor.Run(
                IndexUrl,
                null,
                new
                {
                    CommonHead = this.commonHeadTemplate,
                    NavBar = this.navBarTemplate,
                    LogBook = this.api.LogBook
                }
            );
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
            return this.meditateStartPageHtml;
        }

        /// <summary>
        /// Gets the started state HTML.
        /// </summary>
        /// <returns>The started state html for the meditate page.</returns>
        private string GetStartedStateHtml()
        {
            // In case the user navigates from the page or the browser crashes,
            // get the diff between now and when we started the session.
            // Make the total seconds of that diff the starting time on the webpage
            // we send back.
            TimeSpan diff = DateTime.UtcNow - this.api.CurrentLog.StartTime;

            return Engine.Razor.Run(
                MeditateUrl,
                null,
                new
                {
                    CommonHead = this.commonHeadTemplate,
                    NavBar = this.navBarTemplate,
                    StartingSeconds = diff.TotalSeconds.ToString()
                }
            );
        }

        /// <summary>
        /// Gets the ended state HTML.
        /// </summary>
        /// <param name="errorString">The error string to display (if any).</param>
        /// <returns>The ended state html for the meditate page.</returns>
        private string GetEndedStateHtml( string errorString )
        {
            // Insert the total minutes meditated to the html.
            if( string.IsNullOrEmpty( errorString ) == false )
            {
                errorString = "Error: " + errorString;
            }

            return Engine.Razor.Run(
                endMeditateKey,
                null,
                new
                {
                    CommonHead = this.commonHeadTemplate,
                    NavBar = this.navBarTemplate,
                    ErrorString = errorString,
                    MinutesMeditated = api.CurrentLog.Duration.TotalMinutes.ToString( "N2" )
                }
            );
        }

        /// <summary>
        /// Gets the logbook page's html to display.
        /// </summary>
        /// <returns>HTML for the logbook page in a string.</returns>
        private string GetLogbookHtml()
        {
            string jsData =
@"<script>
    var logData = " + JsonExporter.ExportJsonToString( api.LogBook ) + @"
</script>";

            return Engine.Razor.Run(
                LogbookUrl,
                null,
                new
                {
                    CommonHead = this.commonHeadTemplate,
                    NavBar = this.navBarTemplate,
                    LogBook = this.api.LogBook,
                    LogData = jsData
                }
            );
        }

        /// <summary>
        /// Gets the map view HTML.
        /// </summary>
        /// <returns>The html of the map page.</returns>
        private string GetMapHtml()
        {
            return Engine.Razor.Run(
                MapUrl,
                null,
                new
                {
                    CommonHead = this.commonHeadTemplate,
                    NavBar = this.navBarTemplate,
                    LogBook = this.api.LogBook
                }
            );
        }

        /// <summary>
        /// Gets the calendar view HTML.
        /// </summary>
        /// <returns>The html of the calendar page.</returns>
        private string GetCalendarHtml()
        {
            return Engine.Razor.Run(
                CalendarUrl,
                null,
                new
                {
                    CommonHead = this.commonHeadTemplate,
                    NavBar = this.navBarTemplate,
                    LogBook = this.api.LogBook
                }
            );
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
                    cssMatch.Groups["dir"].Value,
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
                info.ResponseBuffer = System.Text.Encoding.UTF8.GetBytes( Get404Html() );
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
        /// Gets the 404 html.
        /// </summary>
        /// <returns>HTML for the 404 page.</returns>
        private static string Get404Html()
        {
            return
@"
<!doctype html>
<!-- Unit Test: 404 not found -->
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
    }
}