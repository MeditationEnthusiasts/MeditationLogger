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

using MeditationEnthuisasts.MeditationLogger.TestCore;
using NUnit.Framework;

namespace MeditationEnthusiasts.MeditationLogger.Tests.Desktop
{
    /// <summary>
    /// Tests the Api Class.
    /// </summary>
    [TestFixture]
    public partial class LogsApiTest
    {
        // -------- Fields --------

        private ApiTestCore apiTestCore;

        // -------- Setup/Teardown --------

        [SetUp]
        public void TestSetup()
        {
            this.apiTestCore = new ApiTestCore( LocationDetector, TestCoreDir );
            this.apiTestCore.Startup();
        }

        [TearDown]
        public void TestTeardown()
        {
            this.apiTestCore.Reset();
        }

        // -------- Tests --------

        /// <summary>
        /// Ensures the ResetCurrent function works.
        /// </summary>
        [Test]
        public void ResetCurrentTest()
        {
            this.apiTestCore.DoResetCurrentTest();
        }

        /// <summary>
        /// Ensures the ValidateStagged method works.
        /// </summary>
        [Test]
        public void ValidateTest()
        {
            this.apiTestCore.DoValidateTest();
        }

        /// <summary>
        /// Ensures the behavior is correct when StartSession is called.
        /// </summary>
        [Test]
        public void StartTest()
        {
            this.apiTestCore.DoStartTest();
        }

        /// <summary>
        /// Ensures that if we start and stop with looping
        /// music the behavior is correct.
        /// </summary>
        [Test]
        public void StartStopWithLoopingMusic()
        {
            this.apiTestCore.DoStartStopWithLoopingMusic();
        }

        /// <summary>
        /// Ensures calling start with no music
        /// behaves correctly.
        /// </summary>
        [Test]
        public void StartWithNoMusic()
        {
            this.apiTestCore.DoStartWithNoMusic();
        }

        /// <summary>
        /// Ensures trying to start or stop a session results
        /// in an error if the music config is invalid.
        /// </summary>
        [Test]
        public void StartStopWithInvalidMusicConfig()
        {
            this.apiTestCore.DoStartStopWithInvalidMusicConfig();
        }

        /// <summary>
        /// Ensures starting/stopping a sesstion with "Play Once"
        /// checked works correctly.
        /// </summary>
        [Test]
        public void StartStopWithPlayOnceMusic()
        {
            this.apiTestCore.DoStartStopWithPlayOnceMusic();
        }

        /// <summary>
        /// Ensures that if a session is started/stoped with the
        /// length set to null, the behavior is correct.
        /// </summary>
        [Test]
        public void StartStopWithPlayOnceNullLength()
        {
            this.apiTestCore.DoStartStopWithPlayOnceNullLength();
        }

        /// <summary>
        /// Ensures calling stop before start results in a no-op
        /// </summary>
        [Test]
        public void StopTestBeforeStart()
        {
            this.apiTestCore.DoStopTestBeforeStart();
        }

        /// <summary>
        /// Ensures that the session stops correctly.
        /// </summary>
        [Test]
        public void StopTest()
        {
            this.apiTestCore.DoStopTest();
        }

        // ---- Save Tests ----

        /// <summary>
        /// Ensures calling save with no database open
        /// results in an exception.
        /// </summary>
        [Test]
        public void SaveWithNoDatabase()
        {
            this.apiTestCore.DoSaveWithNoDatabase();
        }

        /// <summary>
        /// Ensures calling save without calling start
        /// results in an exception.
        /// </summary>
        [Test]
        public void SaveWithNotStarting()
        {
            this.apiTestCore.DoSaveWithNotStarting();
        }

        /// <summary>
        /// Tries to save to the database the default log values.
        /// </summary>
        [Test]
        public void SaveLogDefaults()
        {
            this.apiTestCore.DoSaveWithDefaultLogs();
        }

        /// <summary>
        /// Tries to save to the database with comments.
        /// </summary>
        [Test]
        public void SaveLogOnlyComments()
        {
            this.apiTestCore.DoSaveLogOnlyComments();
        }

        /// <summary>
        /// Tries to save to the database with technique
        /// </summary>
        [Test]
        public void SaveLogOnlyTechnique()
        {
            this.apiTestCore.DoSaveLogOnlyTechnique();
        }

        /// <summary>
        /// Tries to save to the database with only location.
        /// </summary>
        [Test]
        public void SaveLongOnlyLocation()
        {
            this.apiTestCore.DoSaveLongOnlyLocation();
        }

        /// <summary>
        /// Tries to save to the database.
        /// </summary>
        [Test]
        public void SaveLogWithCommentsAndTechnique()
        {
            this.apiTestCore.DoSaveLogWithCommentsAndTechnique();
        }

        /// <summary>
        /// Tries to save to the database with everything.
        /// </summary>
        [Test]
        public void SaveLogEverything()
        {
            this.apiTestCore.DoSaveLogEverything();
        }

        /// <summary>
        /// Ensures the save fails when only latitiude is passed in.
        /// </summary>
        [Test]
        public void SaveLogWithOnlyLatitude()
        {
            this.apiTestCore.DoSaveLogWithOnlyLatitude();
        }

        /// <summary>
        /// Ensures the save fails when only longitude is passed in.
        /// </summary>
        [Test]
        public void SaveLogWithOnlyLongitude()
        {
            this.apiTestCore.DoSaveLogWithOnlyLongitude();
        }

        // ---- Populate Logbook Tests ----

        /// <summary>
        /// Ensures calling PopulateLogbook with no database
        /// opened results in an exception.
        /// </summary>
        [Test]
        public void PopulateLogBookWithNoDatabase()
        {
            this.apiTestCore.DoPopulateLogBookWithNoDatabase();
        }

        [Test]
        public void PopulateLogBookMultipleTimes()
        {
            this.apiTestCore.DoPopulateLogBookMultipleTimes();
        }

        // ---- Xml Tests ----

        // -- XML Schema Tests --

        /// <summary>
        /// Tests the XML schema with all values.
        /// </summary>
        [Test]
        public void XmlSchemaTest()
        {
            this.apiTestCore.DoXmlSchemaTest();
        }

        /// <summary>
        /// Tests the XML schema with optional values.
        /// </summary>
        [Test]
        public void XmlSchemaTestNoValues()
        {
            this.apiTestCore.DoXmlSchemaTestNoValues();
        }

        // -- XML Export --

        /// <summary>
        /// Ensures the exporting and importing of
        /// logs via XML works.
        /// </summary>
        [Test]
        public void XmlExportImportTest()
        {
            this.apiTestCore.DoXmlExportImportTest();
        }

        // -- XML import --

        /// <summary>
        /// Ensures importing an XML file without the LogBook or Log tag results in a failure,
        /// AND the database is not updated.
        /// </summary>
        [Test]
        public void XmlImportBadLogbookTest()
        {
            this.apiTestCore.XmlImportBadLogbookTest();
        }

        /// <summary>
        /// Ensures importing an XML file with no start or no end time results in a failure,
        /// AND the database is not updated.  Also ensures having StartTime > EndTime results
        /// in a failure.
        /// </summary>
        [Test]
        public void XmlImportNoStartTime()
        {
            this.apiTestCore.DoXmlImportNoStartTime();
        }

        /// <summary>
        /// Ensures having a missing latitude while having a longitude
        /// (or vice vera) results in a failure.
        /// </summary>
        [Test]
        public void XmlImportBadMissingLat()
        {
            this.apiTestCore.DoXmlImportBadMissingLat();
        }

        /// <summary>
        /// Ensures importing an XML file with no logs results
        /// in no error.  Nothing is added.
        /// </summary>
        [Test]
        public void XmlImportNoLogs()
        {
            this.apiTestCore.DoXmlImportNoLogs();
        }

        /// <summary>
        /// Ensures importing an XML file with just start time
        /// and end time is valid.
        /// </summary>
        [Test]
        public void XmlImportJustStartAndEnd()
        {
            this.apiTestCore.DoXmlImportJustStartAndEnd();
        }

        /// <summary>
        /// Ensures importing an XML file with lat and long
        /// set to not numbers results in no error, but both
        /// having null values.
        /// </summary>
        [Test]
        public void XmlImportBadLatLong()
        {
            this.apiTestCore.DoXmlImportBadLatLong();
        }

        // ---- JSON tests ----

        // -- JSON Export --

        /// <summary>
        /// Ensures the exporting and importing of
        /// logs via XML works.
        /// </summary>
        [Test]
        public void JsonExportImportTest()
        {
            this.apiTestCore.DoJsonExportImportTest();
        }

        // -- JSON import --

        /// <summary>
        /// Ensures importing a malformed json logbook results in failues,
        /// and the database is not updated.
        /// There are also cases where malformed json are okay.  Those should pass.
        /// </summary>
        [Test]
        public void JsonImportMalformedJsonTest()
        {
            this.apiTestCore.DoJsonImportMalformedJsonTest();
        }

        /// <summary>
        /// Ensures importing a Json file with no start or no end time results in a failure,
        /// AND the database is not updated.  Also ensures having StartTime > EndTime results
        /// in a failure.
        /// </summary>
        [Test]
        public void JsonImportNoStartTime()
        {
            this.apiTestCore.DoJsonImportNoStartTime();
        }

        /// <summary>
        /// Ensures having a missing latitude while having a longitude
        /// (or vice vera) results in a failure.
        /// </summary>
        [Test]
        public void JsonImportBadMissingLat()
        {
            this.apiTestCore.DoJsonImportBadMissingLat();
        }

        /// <summary>
        /// Ensures importing a Json file with no logs results
        /// in no error.  Nothing is added.
        /// </summary>
        [Test]
        public void JsonImportNoLogs()
        {
            this.apiTestCore.DoJsonImportNoLogs();
        }

        /// <summary>
        /// Ensures importing a Json file with just start time
        /// and end time is valid.
        /// </summary>
        [Test]
        public void JsonImportJustStartAndEnd()
        {
            this.apiTestCore.DoJsonImportJustStartAndEnd();
        }

        /// <summary>
        /// Ensures importing an XML file with lat and long
        /// set to not numbers results in no error, but both
        /// having null values.
        /// </summary>
        [Test]
        public void JsonImportBadLatLong()
        {
            this.apiTestCore.DoJsonImportBadLatLong();
        }

        // ---- MLG import/export ----

        /// <summary>
        /// Ensures the exporting and importing of
        /// logs via MLG works.
        /// </summary>
        [Test]
        public void MlgExportImportTest()
        {
            this.apiTestCore.DoMlgExportImportTest();
        }

        // ---- MLG Sync ----

        /// <summary>
        /// Ensures that the MLG can sync with each other when
        /// all EditTimes are different.
        /// </summary>
        [Test]
        public void MlgSyncDoSync()
        {
            this.apiTestCore.DoMlgSyncDoSync();
        }

        /// <summary>
        /// Tests to make sure sync works fine when both logbooks
        /// have log with same GUIDs.
        /// </summary>
        [Test]
        public void MlgSyncDifferentEdits()
        {
            this.apiTestCore.DoMlgSyncDifferentEdits();
        }

        /// <summary>
        /// Ensures the sync pre-checks work.
        /// </summary>
        [Test]
        public void SyncCheckTest()
        {
            this.apiTestCore.DoSyncCheckTest();
        }

        // -- Modifying database tests --

        /// <summary>
        /// Ensures the insert function works correctly when adding a completely new log
        /// and inserting an existing log.
        /// </summary>
        [Test]
        public void AddNewLogTest()
        {
            this.apiTestCore.DoAddNewLogTest();
        }
    }
}