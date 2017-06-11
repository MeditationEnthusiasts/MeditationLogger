// 
// Meditation Logger.
// Copyright (C) 2016  Seth Hendrick.
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
using System.Diagnostics;
using System.IO;
using System.Net;
using MedEnthDesktop;
using MeditationEnthusiasts.MeditationLogger.Api;
using NUnit.Framework;
using TestCore;
using TestCore.Mocks;

namespace TestDesktop
{
    /// <summary>
    /// Regression tests for the Desktop Command Line.
    /// </summary>
    [TestFixture]
    public class CliTests
    {
        // -------- Fields --------

        /// <summary>
        /// The port the HTTP Server listens on.
        /// </summary>
        private const int port = 10013;

        /// <summary>
        /// The URL to send requests to.
        /// </summary>
        private static readonly string url = "http://localhost:" + port;

        /// <summary>
        /// If a logbook already exists, we don't want to accidently nuke it.
        /// back it up so some poor tester doesn't lose their logs.
        /// </summary>
        private string backedUpLogbook;

        /// <summary>
        /// Default location of the logbook.
        /// </summary>
        private string logbookLocation;

        /// <summary>
        /// Api (needed to read from the database).
        /// </summary>
        private Api api;

        // -------- Setup / Teardown --------

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            // Backup the original log file (Don't want some poor tester losing their log).
            this.backedUpLogbook = null;
            this.logbookLocation = Path.Combine( Constants.DatabaseFolderLocation, Api.LogbookFileName );

            if ( File.Exists ( logbookLocation ) )
            {
                // Move the original file.
                this.backedUpLogbook = Path.Combine( 
                    Constants.DatabaseFolderLocation, 
                    "Backed_up_logbook_" + DateTime.Now.ToString( "MM_dd_yyyy_HH_mm_ss" ) + ".mlg"
                );
                File.Move( logbookLocation, backedUpLogbook );
            }
        }

        [TestFixtureTearDown]
        public void TestFixtureTeardown()
        {
            // Delete logbook, and restore the backed-up logbook (if it exists).
            if ( File.Exists( logbookLocation ) )
            {
                File.Delete( logbookLocation );
            }
            if ( backedUpLogbook != null )
            {
                File.Move( backedUpLogbook, logbookLocation );
            }

        }

        [SetUp]
        public void TestSetup()
        {
            // Delete logbook before starting.
            if ( File.Exists( logbookLocation ) )
            {
                File.Delete( logbookLocation );
            }

            this.api = new Api(
                LogsApiTest.LocationDetector,
                new MockTimer(),
                new MockMusicManager(),
                LogsApiTest.Platform
            );
        }

        [TearDown]
        public void TestTeardown()
        {
            this.api = null;

            // Delete logbook before moving on.
            if ( File.Exists( logbookLocation ) )
            {
                File.Delete( logbookLocation );
            }
        }

        // -------- Tests --------

        /// <summary>
        /// Does the Import/Export test for XML files.
        /// </summary>
        [Test]
        public void CliImportExportXmlTest()
        {
            DoImportExportTest( "cliTest.xml" );
        }

        /// <summary>
        /// Does the Import/Export test for JSON files.
        /// </summary>
        [Test]
        public void CliImportExportJsonTest()
        {
            DoImportExportTest( "cliTest.json" );
        }

        /// <summary>
        /// Does the Import/Export test for MLG files.
        /// </summary>
        [Test]
        public void CliImportExportMlgTest()
        {
            DoImportExportTest( "cliTest.mlg" );
        }

        /// <summary>
        /// Does the sync test via the cli.
        /// </summary>
        [Test]
        public void CliSyncTest()
        {
            // First, create 3 entries in the database.
            LogBook originalLogbook = ApiTestCore.DoSaveTest( this.api, 3, this.logbookLocation );

            string logbook2Location = "logbook2.mlg";
            try
            {
                // Then, create 3 entries in a random logbook.
                LogBook newLogbook = ApiTestCore.DoSaveTest( this.api, 3, logbook2Location );

                // Call the process and do a sync.
                int exitCode = LaunchProcess( "sync " + logbook2Location );
                Assert.AreEqual( 0, exitCode );

                // Check the sync.
                ApiTestCore.CheckSync( this.api, this.logbookLocation, originalLogbook, logbook2Location, newLogbook );
            }
            finally
            {
                File.Delete( logbook2Location );
            }
        }

        // ---- Meditate Tests ----
        
        /// <summary>
        /// Tests to make sure we can save a session with nothing extra saved.
        /// </summary>
        [Test]
        public void CliMeditateTestNoCommentsOrTechnique()
        {
            DoMeditateProcessTest( string.Empty, string.Empty, false );
        }

        /// <summary>
        /// Tests to make sure we can save a session with everything in it.
        /// </summary>
        [Test]
        public void CliMeditateTestWithCommentsAndTechnique()
        {
            DoMeditateProcessTest( "A Technique", "A comment" + Environment.NewLine + "with new lines", true );
        }

        // ---- HTTP Server Tests ----

        /// <summary>
        /// Ensures all the pages can get got successfully with a GET request.
        /// </summary>
        [Test]
        public void GetRequestTest()
        {
            Assert.Ignore( "Ignoring for now, need to figure out why we can't GET request to the same machine.... strange." );

            List<string> pages = new List<string> {
                "/",
                HttpResponseHandler.IndexUrl,
                HttpResponseHandler.LogbookUrl,
                HttpResponseHandler.MeditateUrl,
                HttpResponseHandler.AboutUrl,
                "/js/leaflet.js",
                "/css/leaflet.css",
                HttpResponseHandler.MapUrl,
                "/media/marker-icon.png",
                HttpResponseHandler.ExportUrl,
                HttpResponseHandler.ExportXmlUrl,
                HttpResponseHandler.ExportJsonUrl,
                HttpResponseHandler.CreditsUrl,
                HttpResponseHandler.LicenseUrl
            };

            using ( ServerLauncher server = new ServerLauncher( port ) )
            {
                try
                {
                    foreach( string page in pages )
                    {
                        HttpWebRequest request = WebRequest.CreateHttp( url + page );
                        request.Method = "GET";

                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                        Assert.AreEqual( HttpStatusCode.OK, response.StatusCode );
                    }

                    // Ensure a bad page results in a 404.
                    {
                        HttpWebRequest request = WebRequest.CreateHttp( url + "/derp.html" );
                        request.Method = "GET";

                        try
                        {
                            request.GetResponse();
                        }
                        catch( WebException err )
                        {
                            HttpWebResponse response = (HttpWebResponse)err.Response;
                            Assert.AreEqual( HttpStatusCode.NotFound, response.StatusCode );
                        }
                    }
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                    throw;
                }
            }
        }

        // -------- Test Helpers ---------

        /// <summary>
        /// Does the import/export test from the CLI.
        /// </summary>
        /// <param name="exportedFile"></param>
        private void DoImportExportTest( string exportedFile )
        {
            // First, create 5 entries in the database.
            LogBook originalLogbook = ApiTestCore.DoSaveTest( this.api, 5, this.logbookLocation );

            // Next, go ahead and do the export.
            int exitCode = LaunchProcess( "export " + exportedFile );
            Assert.AreEqual( 0, exitCode );

            Assert.IsTrue( File.Exists( exportedFile ) );
            try
            {
                // Next, delete the logbook.  We'll then do an import afterwards.
                File.Delete( this.logbookLocation );
                Assert.IsFalse( File.Exists( this.logbookLocation ) );

                // Now, go ahead and do an import.
                exitCode = LaunchProcess( "import " + exportedFile );
                Assert.AreEqual( 0, exitCode );

                // Lastly, open the logbook and compare to the original.  They should match.
                this.api.Open( this.logbookLocation );
                this.api.PopulateLogbook();

                ApiTestCore.AreLogbooksEqual( originalLogbook, api.LogBook );
            }
            finally
            {
                File.Delete( exportedFile );
                this.api.Close();
            }
        }

        /// <summary>
        /// Does the meditation process test.
        /// </summary>
        /// <param name="expectedTechnique">The technique we expect.</param>
        /// <param name="expectedComments">The comments we expect.</param>
        /// <param name="expectLocation">Whether or not we want to save the location</param>
        private void DoMeditateProcessTest( string expectedTechnique, string expectedComments, bool expectLocation )
        {
            string standardInput =
                Environment.NewLine + // Enter to start.
                Environment.NewLine + // Enter to end.
                expectedTechnique + Environment.NewLine + // Add expected technique
                expectedComments.Replace( Environment.NewLine, "  " ) + Environment.NewLine + // Add comments.
                ( expectLocation ? "1" : "0" ) + Environment.NewLine;

            Assert.AreEqual( 0, LaunchProcess( "meditate", standardInput ) );

            // Open the logbook and make sure everything works.
            try
            {
                this.api.Open( this.logbookLocation );
                this.api.PopulateLogbook();

                Assert.AreEqual( expectedTechnique, this.api.LogBook.Logs[0].Technique );
                Assert.AreEqual( expectedComments, this.api.LogBook.Logs[0].Comments );
                if ( expectLocation )
                {
                    Assert.IsNotNull( this.api.LogBook.Logs[0].Latitude );
                    Assert.IsNotNull( this.api.LogBook.Logs[0].Longitude );
                }
                else
                {
                    Assert.IsNull( this.api.LogBook.Logs[0].Latitude );
                    Assert.IsNull( this.api.LogBook.Logs[0].Longitude );
                }
            }
            finally
            {
                this.api.Close();
            }
        }

        /// <summary>
        /// Launches the CLI process.
        /// </summary>
        /// <param name="arguments">The arguments to pass into the program.</param>
        /// <param name="standardInput">The input to send to the process.</param>
        /// <returns>The exit code of the process.</returns>
        private int LaunchProcess( string arguments, string standardInput = null )
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = arguments;
            startInfo.RedirectStandardInput = ( standardInput != null );
            startInfo.UseShellExecute = false;

            // TestDesktop has a reference to the CLI, so it will appear in the same Dir as the test .dll.
            startInfo.FileName = "MedEnthLogsCli.exe";

            int exitCode = -1;
            using ( Process process = Process.Start( startInfo ) )
            {
                if ( standardInput != null )
                {
                    using( StreamWriter writer = process.StandardInput )
                    {
                        writer.Write( standardInput );
                    }
                }
                process.WaitForExit();
                exitCode = process.ExitCode;
            }

            return exitCode;
        }

        // -------- Helper Classes --------

        /// <summary>
        /// Class that Starts/Stops the server.
        /// </summary>
        private class ServerLauncher : IDisposable
        {
            // -------- Fields --------

            /// <summary>
            /// The server process.
            /// </summary>
            private Process serverProcess;

            // -------- Fields --------

            /// <summary>
            /// Constructor.  Launches the server.
            /// </summary>
            public ServerLauncher( int port )
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = "launch_server " + port;
                startInfo.RedirectStandardInput = true;
                startInfo.UseShellExecute = false;

                // TestDesktop has a reference to the CLI, so it will appear in the same Dir as the test .dll.
                startInfo.FileName = "MedEnthLogsCli.exe";

                this.serverProcess = Process.Start( startInfo );
            }

            // -------- Functions --------

            /// <summary>
            /// Dispose.  Closes the server.
            /// </summary>
            public void Dispose()
            {
                if ( this.serverProcess != null )
                {
                    HttpWebRequest request = WebRequest.CreateHttp( url + "/quit.html" );
                    request.Method = "POST";

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    Assert.AreEqual( HttpStatusCode.OK, response.StatusCode );

                    this.serverProcess.WaitForExit();
                    Assert.AreEqual( 0, this.serverProcess.ExitCode );
                }
            }
        }
    }
}
