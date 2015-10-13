using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedEnthDesktop;
using MedEnthLogsApi;
using MedEnthLogsDesktop;

namespace MedEnthLogsCli
{
    class Program
    {
        /// <summary>
        /// The executable name.
        /// </summary>
        const string exeName = "MedEnthLogsCli.exe";

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
                    Console.WriteLine( "Medition Logger Version: " + Api.Version );
                }
            }
            else if ( args.Length == 2 )
            {
                if ( ( args[0] == "import" ) || ( args[0] == "export" ) || ( args[0] == "sync" ) )
                {
                    Api api = new Api( new Win32LocationDetector(), new Win32Timer() );
                    try
                    {
                        api.Open( new SQLite.Net.Platform.Win32.SQLitePlatformWin32(), "test.db" );
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
                        api.Close();
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

        static void PrintHelp()
        {
            Console.WriteLine( "Command Line Interface for Meditation Logger." );
            Console.WriteLine();
            Console.WriteLine( "Arguments:" );
            Console.WriteLine( "--help, -h, \\?\t\tShow this message." );
            Console.WriteLine( "--version, -v\t\tShow the version." );
            Console.WriteLine( "import fileName\t\tImport the given file name to the logbook." );
            Console.WriteLine( "export fileName\t\tExport the logbook to the given filename." );
            Console.WriteLine( "sync fileName.mlg\tSync the logbook with the given .mlg file" );
            Console.WriteLine();
            Console.WriteLine( "Note, the filename extension determines how files are imported or exported." );
            Console.WriteLine( ".xml, .json, and .mlg are valid filetypes." );
        }
    }
}
