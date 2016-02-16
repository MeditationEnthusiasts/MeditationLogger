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
                    if ( ( request.RawUrl == "/" ) || ( request.RawUrl == "/index.html" ) )
                    {
                        responseString = GetHomePageHtml( api );
                    }
                    else if ( request.RawUrl == "/about.html" )
                    {
                        responseString = GetAboutPage();
                    }
                    else
                    {
                        responseString = Get404Html();
                        response.StatusCode = Convert.ToInt32( HttpStatusCode.NotFound );
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes( responseString );
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write( buffer, 0, buffer.Length );
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
        /// Gets the header of the html file.
        /// </summary>
        /// <param name="title">The title to name the page.</param>
        /// <param name="backgroundColor">The background color of the page.</param>
        /// <returns>The HTML header.</returns>
        private static string GetHeader( string title, string backgroundColor )
        {
            return
                @"
<!doctype html>
<head>
    <meta http-equiv=""content-type"" content=""text/html; charset=utf-8"" />
    <title>" + title + @"</title>
    <style>
        body
        {
            background-color:" + backgroundColor + @"
        }
    </style>
</head>
<body>
";
        }

        /// <summary>
        /// Gets the footer of the html file.
        /// </summary>
        /// <returns>The HTML footer.</returns>
        private static string GetFooter()
        {
            return
@"
</body>
</html>
";
        }

        /// <summary>
        /// Get the home page's html to display.
        /// </summary>
        /// <returns>HTML for the home page in a string.</returns>
        private static string GetHomePageHtml( Api api )
        {
            string latestSesssionString = string.Empty;
            if ( api.LogBook.Logs.Count == 0 )
            {
                latestSesssionString = "Nothing yet.";
            }
            else
            {
                latestSesssionString = api.LogBook.Logs[0].StartTime.ToLocalTime().ToString( "MM-dd-yyyy  HH:mm" );
            }

            return
                GetHeader( "Meditation Enthusiasts Logger", "#ffaaaa" ) +
@"
    <h1>Meditation Logger</h1>
    <table>
        <tr>
            <td><strong>Total Minutes:</strong></td>
            <td>" + api.LogBook.TotalTime.ToString( "N2" ) + @"</td>
        </tr>
        <tr>
            <td><strong>Longest Session:</strong></td>
            <td>" + api.LogBook.LongestTime.ToString( "N2" ) + @"</td>
        </tr>
        <tr>
            <td><strong>Total Sessions:</strong></td>
            <td>" + api.LogBook.Logs.Count + @"</td>
        </tr>
        <tr>
            <td><strong>Last Session:</strong></td>
            <td>" + latestSesssionString + @"</td>
        </tr>
    </table>
" +
                GetFooter();
        }

        /// <summary>
        /// Gets the about page.
        /// </summary>
        /// <returns>HTML for the about page.</returns>
        private static string GetAboutPage()
        {
            return GetHeader( "About Meditation Logger", "#00ffff" ) + @"

    <h1>Meditation Logger</h1>
    <table>
        <tr>
            <td><strong>Version:</strong></td>
            <td>" + Api.VersionString + @"</td>
        </tr>
        <tr>
            <td><strong>Visit Site:</strong></td>
            <td><a href=""https://meditationenthusiasts.org"" target=""_blank"">https://meditationenthusiasts.org</a></td>
        </tr>
        <tr>
            <td><strong>Report a Bug:</strong></td>
            <td><a href=""https://meditationenthusiasts.org/development/mantis/"" target=""_blank"">https://meditationenthusiasts.org/development/mantis/</a></td>
        </tr>
        <tr>
            <td><strong>View Wiki:</strong></td>
            <td><a href=""https://meditationenthusiasts.org/development/dokuwiki/doku.php?id=mantis:meditation_logger:start"" target=""_blank"">https://meditationenthusiasts.org/development/dokuwiki/doku.php?id=mantis:meditation_logger:start</a></td>
        </tr>
        <tr>
            <td><strong>View Source:</strong></td>
            <td><a href=""https://bitbucket.org/meditationenthusiasts/meditation-logs-desktop/src"" target=""_blank"">https://bitbucket.org/meditationenthusiasts/meditation-logs-desktop/src</a></td>
        </tr>
    </table>
"
            + GetFooter();
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
        <title>Meditation Enthusiasts</title>
    </head>
    <body>
        <h1>404 Not Found</h1>
    </body>
    </html>
";
        }
    }
}
