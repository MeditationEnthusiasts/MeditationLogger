using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MedEnthDesktop.Server
{
    public class HttpServer : IDisposable
    {
        // -------- Fields --------

        /// <summary>
        /// Default port to listen on.
        /// </summary>
        public const Int16 DefaultPort = 80;

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
        /// Constructor
        /// </summary>
        /// <param name="port">The port to lisen on.</param>
        /// <param name="consoleOutFunction">
        /// Function to print out console information to.
        /// Set to null for no action.
        /// </param>
        public HttpServer( Int16 port = DefaultPort, Action<string> consoleOutFunction = null )
        {
            if ( HttpListener.IsSupported == false )
            {
                throw new PlatformNotSupportedException(
                    "This platform does not support HTTP Listeners..."
                );
            }

            if ( consoleOutFunction == null )
            {
                this.cout = delegate ( string s ) { };
            }

            this.cout = consoleOutFunction;
            this.listener = new HttpListener();

            this.listener.Prefixes.Add( "http://*:" + port + "/" );

            this.listeningThread = new Thread(
                delegate()
                {
                    HandleRequest( this.listener, cout );
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
        /// <param name="result">Async Result Object.</param>
        /// <param name="consoleOutFunction">The function used to print to the console.</param>
        private static void HandleRequest( HttpListener listener, Action<string> consoleOutFunction )
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
                    if ( request.RawUrl == "/" )
                    {
                        responseString = GetHomePageHtml();
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

        /// <summary>
        /// Get the home page's html to display.
        /// </summary>
        /// <returns>HTML for the home page in a string.</returns>
        private static string GetHomePageHtml()
        {
            return
            @"
                <!doctype html>
                <head>
                    <title>Meditation Enthusiasts</title>
                </head>
                <body>
                    <h1>Home Page</h1>
                </body>
                </html>
            ";
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
