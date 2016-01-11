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
using MedEnthDesktop;
using MedEnthLogsApi;
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
        /// Launches the CLI process.
        /// </summary>
        /// <param name="arguments">The arguments to pass into the program.</param>
        /// <returns>The exit code of the process.</returns>
        private int LaunchProcess( string arguments )
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = arguments;

            // TestDesktop has a reference to the CLI, so it will appear in the same Dir as the test .dll.
            startInfo.FileName = "MedEnthLogsCli.exe";

            int exitCode = -1;
            using ( Process process = Process.Start( startInfo ) )
            {
                process.WaitForExit();
                exitCode = process.ExitCode;
            }

            return exitCode;
        }
    }
}
