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
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MeditationEnthusiasts.MeditationLogger.Api;

namespace MeditationEnthusiasts.MeditationLogger.Desktop
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
        /// What sends the data back to the client.
        /// </summary>
        private HttpResponseHandler responseHandler;

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
        private Api.Api api;

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
        public HttpServer( Api.Api api, short port = DefaultPort, Action<string> consoleOutFunction = null )
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
                delegate ()
                {
                    HandleRequest();
                }
            );

            this.quitEvent = new ManualResetEvent( false );

            this.responseHandler = new HttpResponseHandler( api );
            this.responseHandler.QuitEvent += ( () => this.quitEvent.Set() );
            this.IsListening = false;
        }

        // -------- Properties --------

        /// <summary>
        /// Whether or not we are listening.
        /// </summary>
        public bool IsListening { get; private set; }

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
                this.responseHandler.InitTemplates();
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
        private void HandleRequest()
        {
            // There are bunch of exceptions that can happen here.
            // 1.  Client closes connection.  This will cause our response's stream to close.
            //     We may see errors such as "The specified network name is no longer available"
            //     or "The I/O operation has been aborted because of either a thread exit or an application request".
            //     NEITHER or these should cause the program to crash.  Simply grab the next connection and move on.
            // 2.  ObjectDisposeExceptions can happen if the above happens as well.  We'll handle those when needed.

            try
            {
                while( this.IsListening )
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
                            this.cout?.Invoke( "Server got terminated, shutting down..." );
                            continue;
                        }
                        else
                        {
                            this.cout?.Invoke( "FATAL ERROR (" + err.ErrorCode + "): " + err.ToString() );
                            throw;
                        }
                    }

                    try
                    {
                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;

                        HtmlInfo info = null;

                        try
                        {
                            NameValueCollection queryString;
                            using( StreamReader reader = new StreamReader( request.InputStream ) )
                            {
                                queryString = HttpUtility.ParseQueryString( reader.ReadToEnd() );
                            }

                            info = this.responseHandler.GetHtmlInfo(
                                request.RawUrl,
                                request.HttpMethod,
                                queryString
                            );
                        }
                        catch( Exception e )
                        {
                            info = new HtmlInfo();
                            info.ResponseBuffer = Encoding.UTF8.GetBytes( HttpResponseHandler.GetErrorHtml( e ) );
                            info.StatusCode = HttpStatusCode.InternalServerError;

                            this.cout?.Invoke( "**********" );
                            this.cout?.Invoke( "Caught Exception when determining response: " + e.Message );
                            this.cout?.Invoke( e.StackTrace );
                            this.cout?.Invoke( "**********" );
                        }
                        finally
                        {
                            try
                            {
                                if( info != null )
                                {
                                    // Images are g-zipped.  Add appropriate header.
                                    if( info.ContentType.StartsWith( "image" ) )
                                    {
                                        response.AddHeader( "Content-Encoding", "gzip" );
                                    }

                                    response.StatusCode = Convert.ToInt32( info.StatusCode );
                                    response.ContentLength64 = info.ResponseBuffer.Length;
                                    response.OutputStream.Write( info.ResponseBuffer, 0, info.ResponseBuffer.Length );
                                }
                            }
                            catch( Exception e )
                            {
                                this.cout?.Invoke( "**********" );
                                this.cout?.Invoke( "Caught Exception when writing response: " + e.Message );
                                this.cout?.Invoke( e.StackTrace );
                                this.cout?.Invoke( "**********" );
                            }
                            response.OutputStream.Close();
                        }

                        this.cout?.Invoke(
                            request.HttpMethod + " from: " + request.UserHostName + " " + request.UserHostAddress + " " + request.RawUrl + " (" + response.StatusCode + ")"
                        );
                    } // End request/response try{} 
                    catch( Exception err )
                    {
                        this.cout?.Invoke( "**********" );
                        this.cout?.Invoke( "Caught exception when handling resposne: " + err.Message );
                        this.cout?.Invoke( "This can happen for several expected reasons.  We're okay!" );
                        this.cout?.Invoke( err.StackTrace );
                        this.cout?.Invoke( "**********" );
                    }
                } // End while IsListening loop.
            }
            catch( Exception e )
            {
                this.cout?.Invoke( "**********" );
                this.cout?.Invoke( "FATAL Exception in HTTP Listener.  Aborting web server, but the picframe will still run: " + e.Message );
                this.cout?.Invoke( e.StackTrace );
                this.cout?.Invoke( "**********" );
            }

            // Our thread is exiting, notify all threads waiting on it.
            this.quitEvent.Set();
        }
    }
}
