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
using System.Text;
using System.Threading.Tasks;
using MedEnthDesktop;
using MedEnthLogsApi;
using MedEnthLogsDesktop;
using SethCS.IO;

namespace MedEnthLogsCli
{
    public static partial class Program
    {
        /// <summary>
        /// The executable name.
        /// </summary>
        const string exeName = "MedEnthLogsCli.exe";

        /// <summary>
        /// Opens and returns the API.
        /// </summary>
        /// <returns></returns>
        static Api OpenApi()
        {
            Api api = GetApi();
            string dbLocation = Constants.DatabaseFolderLocation;

            if ( Directory.Exists( dbLocation ) == false )
            {
                Directory.CreateDirectory( dbLocation );
            }

            api.Open( Path.Combine( dbLocation, Api.LogbookFileName ) );

            return api;
        }

        /// <summary>
        /// Main Function
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>0 for success, else failure.</returns>
        static int Main( string[] args )
        {
            if ( args.Length == 1 )
            {
                if ( ( args[0] == "--help" ) || ( args[0] == "-h" ) || ( args[0] == "/?" ) )
                {
                    PrintHelp();
                }
                else if ( ( args[0] == "--version" ) || ( args[0] == "-v" ) )
                {
                    Console.WriteLine( "Medition Logger Version: " + Api.VersionString );
                }
                else if ( ( args[0] == "--license" ) || args[0] == "-l" )
                {
                    Console.WriteLine( MedEnthLogsApi.License.MedEnthLicense );
                }
                else if ( ( args[0] == "--external" ) || args[0] == "-e" )
                {
                    Console.WriteLine( MedEnthLogsApi.License.ExternalLicenses );
                }
                else if ( args[0] == "meditate" )
                {
                    return DoSession();
                }
            }
            else if ( args.Length == 2 )
            {
                if ( ( args[0] == "import" ) || ( args[0] == "export" ) || ( args[0] == "sync" ) )
                {
                    Api api = null;
                    try
                    {
                        api = OpenApi();
                        api.PopulateLogbook();

                        switch ( args[0] )
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
                                return 0;
                        }
                    }
                    catch ( Exception err )
                    {
                        Console.WriteLine( err.Message );
                        return 1;
                    }
                    finally
                    {
                        api?.Close();
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

            return 0;
        }

        /// <summary>
        /// Prints the help to the command line.
        /// </summary>
        static void PrintHelp()
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
        static int DoSession()
        {
            Api api = null;
            try
            {
                api = OpenApi();

                // Setup
                SessionConfig config = new SessionConfig();
                config.PlayMusic = false;
                config.LoopMusic = false;
                config.Length = null;
                config.AudioFile = string.Empty;

                api.timer.OnUpdate = delegate( string time ) { }; // No-op.

                api.timer.OnComplete = delegate() { }; // No-op.

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
                if ( choice.HasValue && ( choice.Value == 1 ) )
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
            catch ( Exception err )
            {
                Console.WriteLine( err.Message );
                return 1;
            }
            finally
            {
                api?.Close();
            }

            return 0;
        }
    }
}
