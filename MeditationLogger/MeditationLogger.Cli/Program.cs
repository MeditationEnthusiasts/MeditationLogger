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
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using MeditationEnthusiasts.MeditationLogger.Api;
using MeditationEnthusiasts.MeditationLogger.Desktop;
using MeditationEnthusiasts.MeditationLogger.Server;
using SethCS.IO;

namespace MeditationEnthusiasts.MeditationLogger.Cli
{
    public static partial class Program
    {
        /// <summary>
        /// The executable name.
        /// </summary>
        private const string exeName = "MedEnthLogsCli.exe";

        /// <summary>
        /// Opens and returns the API.
        /// </summary>
        /// <returns></returns>
        private static Api.Api OpenApi()
        {
            Api.Api api = GetApi();
            string dbLocation = Constants.DatabaseFolderLocation;

            if( Directory.Exists( dbLocation ) == false )
            {
                Directory.CreateDirectory( dbLocation );
            }

            api.Open( Path.Combine( dbLocation, Api.Api.LogbookFileName ) );

            api.timer.OnUpdate = delegate ( string time )
            {
            }; // No-op.
            api.timer.OnComplete = delegate ()
            {
            }; // No-op.

            return api;
        }

        /// <summary>
        /// Main Function
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>0 for success, else failure.</returns>
        private static int Main( string[] args )
        {
            if( args.Length == 1 )
            {
                if( ( args[0] == "--help" ) || ( args[0] == "-h" ) || ( args[0] == "/?" ) )
                {
                    PrintHelp();
                }
                else if( ( args[0] == "--version" ) || ( args[0] == "-v" ) )
                {
                    Console.WriteLine( "Medition Logger Version: " + Api.Api.VersionString );
                }
                else if( ( args[0] == "--license" ) || args[0] == "-l" )
                {
                    Console.WriteLine( MeditationEnthusiasts.MeditationLogger.Api.License.MedEnthLicense );
                }
                else if( ( args[0] == "--external" ) || args[0] == "-e" )
                {
                    Console.WriteLine( MeditationEnthusiasts.MeditationLogger.Api.License.ExternalLicenses );
                }
                else if( args[0] == "meditate" )
                {
                    return DoSession();
                }
                else if( args[0] == "launch_server" )
                {
                    int returnCode = LaunchServer( MeditationServer.DefaultPort, args );
                    if( returnCode == ErrorCodes.AdminNeeded )
                    {
                        PrintAdminMessage( MeditationServer.DefaultPort );
                    }
                    return returnCode;
                }
            }
            else if( args.Length == 2 )
            {
                if( ( args[0] == "import" ) || ( args[0] == "export" ) || ( args[0] == "sync" ) )
                {
                    Api.Api api = null;
                    try
                    {
                        api = OpenApi();
                        api.PopulateLogbook();

                        switch( args[0] )
                        {
                            case "import":
                                api.Import( args[1] );
                                break;

                            case "export":
                                api.Export( args[1] );
                                break;

                            case "sync":
                                api.Sync( args[1] );
                                break;

                            default:
                                PrintHelp();
                                return ErrorCodes.Success;
                        }
                    }
                    catch( Exception err )
                    {
                        Console.WriteLine( err.Message );
                        return ErrorCodes.ApiError;
                    }
                    finally
                    {
                        api?.Close();
                    }
                }
                else if( args[0] == "launch_server" )
                {
                    short port;
                    if( short.TryParse( args[1], out port ) )
                    {
                        int code = LaunchServer( port, args );
                        if( code == ErrorCodes.AdminNeeded )
                        {
                            PrintAdminMessage( port );
                        }
                        return code;
                    }
                    else
                    {
                        Console.WriteLine( args[1] + " is not an interger." );
                        return ErrorCodes.UserError;
                    }
                }
                else
                {
                    PrintHelp();
                }
            }
            else
            {
                PrintHelp();
            }

            return ErrorCodes.Success;
        }

        /// <summary>
        /// Prints the help to the command line.
        /// </summary>
        private static void PrintHelp()
        {
            Console.WriteLine( "Command Line Interface for Meditation Logger." );
            Console.WriteLine();
            Console.WriteLine( "Arguments:" );
            Console.WriteLine( "--help, -h, \\?\t\tShow this message." );
            Console.WriteLine( "--version, -v\t\tShow the version." );
            Console.WriteLine( "--license, -l\t\tShows this program's license" );
            Console.WriteLine( "--external, -e\t\tShows info and license about the external libraries" );
            Console.WriteLine( "import fileName\t\tImport the given file name to the logbook." );
            Console.WriteLine( "export fileName\t\tExport the logbook to the given filename." );
            Console.WriteLine( "sync fileName.mlg\tSync the logbook with the given .mlg file" );
            Console.WriteLine( "meditate\t\tStart a simple meditation session." );
            Console.WriteLine();
            Console.WriteLine( "Note, the filename extension determines how files are imported or exported." );
            Console.WriteLine( ".xml, .json, and .mlg are valid filetypes." );
        }

        /// <summary>
        /// Do a simple meditation session.
        /// </summary>
        private static int DoSession()
        {
            Api.Api api = null;
            try
            {
                api = OpenApi();

                // Setup
                SessionConfig config = new SessionConfig();
                config.PlayMusic = false;
                config.LoopMusic = false;
                config.Length = null;
                config.AudioFile = string.Empty;

                // Wait for the user to press enter to start the session.
                Console.Write( "Press Enter To Begin..." );
                Console.Out.Flush();
                Console.ReadLine();
                api.StartSession( config );

                // Stop the session when the user tells us to.
                Console.WriteLine();
                Console.Write( "Happy Meditating! Press Enter when complete..." );
                Console.Out.Flush();
                Console.ReadLine();
                api.StopSession();

                Console.WriteLine();
                Console.WriteLine( "Mintues Meditated: " + api.CurrentLog.Duration.TotalMinutes );
                Console.WriteLine();

                // Get information from the user.
                Console.Write( "Enter Technique: " );
                Console.Out.Flush();
                string technique = Console.ReadLine();

                Console.Write( "Enter Comments: " );
                Console.Out.Flush();
                string comments = Console.ReadLine().Replace( "  ", Environment.NewLine );

                // Save the location.
                int? choice = ConsoleHelpers.ShowListPrompt( new List<string> { "No", "Yes" }, true, "Save Location (Enter Number):" );

                decimal? lat = null;
                decimal? lon = null;
                if( choice.HasValue && ( choice.Value == 1 ) )
                {
                    api.LocationDetector.RefreshPosition();
                    lat = api.LocationDetector.Latitude;
                    lon = api.LocationDetector.Longitude;
                }
                api.ValidateAndSaveSession( technique, comments, lat, lon );

                Console.WriteLine();
                Console.WriteLine( "Session Saved, see you next time!" );
                Console.Out.Flush();
            }
            catch( Exception err )
            {
                Console.WriteLine( err.Message );
                return ErrorCodes.ApiError;
            }
            finally
            {
                api?.Close();
            }

            return ErrorCodes.Success;
        }

        /// <summary>
        /// Launches the server on the given port.
        /// </summary>
        /// <param name="port">Port to listen on.  Defaulted to 80</param>
        /// <returns></returns>
        private static int LaunchServer( short port, string[] args )
        {
            // Required for the Razor Engine:
            // https://github.com/Antaris/RazorEngine
            if( AppDomain.CurrentDomain.IsDefaultAppDomain() )
            {
                // RazorEngine cannot clean up from the default appdomain...
                Console.WriteLine( "Switching to second AppDomain, for RazorEngine..." );
                AppDomainSetup adSetup = new AppDomainSetup();
                adSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                AppDomain current = AppDomain.CurrentDomain;
                // You only need to add strongnames when your appdomain is not a full trust environment.
                var strongNames = new StrongName[0];

                AppDomain domain = AppDomain.CreateDomain(
                    "MeditationLoggerDomain", null,
                    current.SetupInformation,
                    new PermissionSet( PermissionState.Unrestricted ),
                    strongNames
                );

                int exitCode = domain.ExecuteAssembly(
                    Assembly.GetExecutingAssembly().Location,
                    args
                );

                // RazorEngine will cleanup.
                AppDomain.Unload( domain );
                return exitCode;
            }

            Api.Api api = null;
            try
            {
                Console.WriteLine( "Opening Database..." );
                api = OpenApi();
                api.PopulateLogbook();
                Console.WriteLine( "Opening Database...Done!" );
                Console.WriteLine();

                using(
                    MeditationServer server =
                        new MeditationServer(
                            api,
                            port,
                            delegate ( string message )
                            {
                                Console.WriteLine( message );
                            }
                        ) )
                {
                    server.Start();
                    Console.WriteLine( "Happy Meditating! Post to /quit.html to quit..." );
                    Console.Out.Flush();
                    server.WaitForQuitEvent();
                }
            }
            catch( Exception err )
            {
                if( err.Message == "Access is denied" )
                {
                    return ErrorCodes.AdminNeeded;
                }
                else
                {
                    Console.WriteLine( err.Message );
                    return ErrorCodes.HttpError;
                }
            }
            finally
            {
                api?.Close();
            }

            return ErrorCodes.Success;
        }

        /// <summary>
        /// Prompts the user permission to get Admin Access to launch the server.
        /// </summary>
        private static void PrintAdminMessage( int port )
        {
            Console.WriteLine( "Admin is needed to launch the server." );
            Console.WriteLine( "Please relaunch the process as admin (right click->Run As Administrator)" );
            Console.WriteLine( "Or, run the following command in an admin command prompt:" );
            Console.WriteLine( "netsh http add urlacl url=\"http://*:{0}/\" user=everyone", port );
        }
    }
}