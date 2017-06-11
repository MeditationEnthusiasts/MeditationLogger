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
using System.Text;
using System.Xml;
using System.Xml.Schema;
using MeditationEnthusiasts.MeditationLogger.Api;
using Newtonsoft.Json;
using NUnit.Framework;
using SQLite.Net.Interop;
using TestCore.Mocks;

namespace TestCore
{
    /// <summary>
    /// Test for the the API class.
    /// </summary>
    public partial class ApiTestCore
    {
        // -------- Fields --------

        /// <summary>
        /// Unit under test.
        /// </summary>
        private Api uut;

        private MockTimer mockTimer;

        private MockMusicManager mockAudio;

        private readonly ILocationDetector locationDetector;

        private readonly ISQLitePlatform sqlitePlatform;

        private const string dbLocation = "test.mlg";

        private readonly string projectDir;

        // -------- Constructor ---------

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="platform">The SQLite platform to use.</param>
        /// <param name="locationDetector">The location detector to use.</param>
        /// <param name="projectDir">Relative path where the TestCore project is located to the .dll</param>
        public ApiTestCore( ISQLitePlatform platform, ILocationDetector locationDetector, string projectDir )
        {
            this.sqlitePlatform = platform;
            this.locationDetector = locationDetector;
            this.projectDir = projectDir;
        }

        // -------- Setup / Teardown --------

        /// <summary>
        /// Should be run during the TestSetup stage.
        /// </summary>
        public void Startup()
        {
            if ( File.Exists( dbLocation ) )
            {
                File.Delete( dbLocation );
            }

            this.mockTimer = new MockTimer();
            this.mockAudio = new MockMusicManager();
            this.uut = new Api( this.locationDetector, this.mockTimer, this.mockAudio, this.sqlitePlatform );
        }

        /// <summary>
        /// Should be run during the TestTeardown stage.
        /// </summary>
        public void Reset()
        {
            this.uut.Close();

            if ( File.Exists( dbLocation ) )
            {
                File.Delete( dbLocation );
            }
        }

        // -------- Tests --------

        /// <summary>
        /// Ensures the ResetCurrent function works.
        /// </summary>
        public void DoResetCurrentTest()
        {
            uut.ResetStates();

            // Ensure stagged log is the equivalent of 
            Assert.AreEqual( new Log(), uut.CurrentLog );
        }

        /// <summary>
        /// Ensures the ValidateStagged method works.
        /// </summary>
        public void DoValidateTest()
        {
            // Ensure it doesn't validate if the start time is
            // later than the end time.
            uut.currentLog.StartTime = Log.MaxTime;
            uut.currentLog.EndTime = DateTime.MinValue;

            CheckValidationFailed( Log.EndTimeLessThanStartTimeMessage );
            uut.ResetStates();

            // Ensure everything validates if start time and end time.
            uut.currentLog.StartTime = DateTime.Now;
            uut.currentLog.EndTime = uut.CurrentLog.StartTime;
            CheckValidationPassed();
            uut.ResetStates();

            // Ensure everything validates if start time is less than end time.
            uut.currentLog.StartTime = DateTime.Now;
            uut.currentLog.EndTime = Log.MaxTime;
            CheckValidationPassed();
            uut.ResetStates();
        }

        /// <summary>
        /// Ensures the behavior is correct when StartSession is called.
        /// </summary>
        public void DoStartTest()
        {
            // Ensure we start in the idle state.
            Assert.AreEqual( Api.ApiState.Idle, this.uut.CurrentState );

            // ---- Start Session ----

            this.uut.StartSession( new SessionConfig() );
            Assert.IsTrue( mockTimer.IsRunning );
            Assert.IsFalse( mockAudio.IsPlaying ); // Default setting passed in, should not be playing.

            // Ensure the expected edit time is close to DateTime.Now;
            DateTime expectedCreationTime = DateTime.Now.ToUniversalTime();
            TimeSpan delta = expectedCreationTime - uut.currentLog.EditTime;
            double deltaTime = delta.TotalSeconds;
            Assert.LessOrEqual( deltaTime, 5 );

            // Ensure start time and edit time match the creation time.
            Assert.AreEqual( uut.CurrentLog.EditTime, uut.CurrentLog.StartTime );

            // Should be in the started time.
            Assert.AreEqual( Api.ApiState.Started, this.uut.CurrentState );

            // Validation should fail, as we haven't stopped a session yet.
            CheckValidationFailed();

            // ---- Start when started ----

            // Calling start again should result in a no-op
            DateTime oldTime = uut.CurrentLog.EditTime;
            uut.StartSession( new SessionConfig() );

            // Ensure the times didn't change.
            Assert.AreEqual( oldTime, uut.CurrentLog.StartTime );
            Assert.AreEqual( oldTime, uut.CurrentLog.EditTime );

            // Should still be in the started state.
            Assert.AreEqual( Api.ApiState.Started, this.uut.CurrentState );
            Assert.IsTrue( mockTimer.IsRunning );

            // ---- Attempt to save when we're not stopped ----

            // Lastly, ensure if we call save, we get an exception.
            Assert.Catch<InvalidOperationException>(
                delegate ()
                {
                    uut.ValidateAndSaveSession();
                }
            );
        }

        /// <summary>
        /// Ensures that if we start and stop with looping
        /// music the behavior is correct.
        /// </summary>
        public void DoStartStopWithLoopingMusic()
        {
            SessionConfig config = new SessionConfig();
            config.PlayMusic = true;
            config.LoopMusic = true;
            config.Length = new TimeSpan( 0, 1, 0 );
            config.AudioFile = "test.mp3";

            // Start idle
            Assert.AreEqual( Api.ApiState.Idle, this.uut.CurrentState );

            // Start the session.
            this.uut.StartSession( config );
            Assert.AreEqual( Api.ApiState.Started, this.uut.CurrentState ); // Ensure our state is started.
            Assert.IsTrue( mockAudio.IsPlaying );
            Assert.IsTrue( mockTimer.IsRunning );
            Assert.AreEqual( config.Length, mockTimer.Time );

            uut.StopSession();
            Assert.AreEqual( Api.ApiState.Stopped, this.uut.CurrentState ); // Ensure our state is stopped.
            Assert.IsFalse( mockAudio.IsPlaying );
            Assert.IsFalse( mockTimer.IsRunning );
            Assert.AreEqual( null, mockTimer.Time );
        }

        /// <summary>
        /// Ensures calling start with no music
        /// behaves correctly.
        /// </summary>
        public void DoStartWithNoMusic()
        {
            SessionConfig config = new SessionConfig();
            config.PlayMusic = false;
            config.LoopMusic = true;
            config.Length = new TimeSpan( 0, 1, 0 );
            config.AudioFile = "test.mp3";

            // Start idle
            Assert.AreEqual( Api.ApiState.Idle, this.uut.CurrentState );

            this.uut.StartSession( config );
            Assert.AreEqual( Api.ApiState.Started, this.uut.CurrentState ); // Ensure our state is started.
            Assert.IsFalse( mockAudio.IsPlaying ); // Play music is false, this should be false.
            Assert.IsTrue( mockTimer.IsRunning );
            Assert.AreEqual( config.Length, mockTimer.Time );
        }

        /// <summary>
        /// Ensures trying to start or stop a session results
        /// in an error if the music config is invalid.
        /// </summary>
        public void DoStartStopWithInvalidMusicConfig()
        {
            SessionConfig config = new SessionConfig();
            config.PlayMusic = true;
            config.LoopMusic = true;
            config.Length = new TimeSpan( 0, 1, 0 );
            config.AudioFile = "doesNotExist.mp3";

            mockAudio.ThrownFromValidate = new Exception( "I got thrown" );

            Assert.Throws<Exception>(
                delegate ()
                {
                    uut.StartSession( config );
                }
            );

            // Ensure that we are not running, and the log was reset.
            Assert.AreEqual( Api.ApiState.Idle, this.uut.CurrentState );
            Assert.IsFalse( mockAudio.IsPlaying );
            Assert.IsFalse( mockTimer.IsRunning );
            Assert.AreEqual( null, mockTimer.Time );
            Assert.AreEqual( new Log(), uut.CurrentLog );
        }

        /// <summary>
        /// Ensures starting/stopping a sesstion with "Play Once"
        /// checked works correctly.
        /// </summary>
        public void DoStartStopWithPlayOnceMusic()
        {
            SessionConfig config = new SessionConfig();
            config.PlayMusic = true;
            config.LoopMusic = false;
            config.Length = new TimeSpan( 0, 1, 0 );
            config.AudioFile = "test.mp3";

            this.mockAudio.GetLengthOfFileReturn = new TimeSpan( 1, 0, 0 );

            Assert.AreEqual( Api.ApiState.Idle, this.uut.CurrentState ); // Start Idle

            uut.StartSession( config );
            Assert.AreEqual( Api.ApiState.Started, this.uut.CurrentState ); // We are started.
            Assert.IsTrue( mockAudio.IsPlaying );
            Assert.IsTrue( mockTimer.IsRunning );

            // Ensure if play-though once that config's length is ignored, and
            // is more than the audio's playtime.
            Assert.Greater( mockTimer.Time, config.Length );
            Assert.Greater( mockTimer.Time, mockAudio.GetLengthOfFileReturn );

            uut.StopSession();
            Assert.AreEqual( Api.ApiState.Stopped, this.uut.CurrentState ); // We are Stopped.
            Assert.IsFalse( mockAudio.IsPlaying );
            Assert.IsFalse( mockTimer.IsRunning );
            Assert.AreEqual( null, mockTimer.Time );
        }

        /// <summary>
        /// Ensures that if a session is started/stoped with the
        /// length set to null, the behavior is correct.
        /// </summary>
        public void DoStartStopWithPlayOnceNullLength()
        {
            SessionConfig config = new SessionConfig();
            config.PlayMusic = true;
            config.LoopMusic = false;
            config.Length = null;
            config.AudioFile = "test.mp3";

            this.mockAudio.GetLengthOfFileReturn = new TimeSpan( 1, 0, 0 );

            Assert.AreEqual( Api.ApiState.Idle, this.uut.CurrentState ); // Start Idle

            uut.StartSession( config );
            Assert.AreEqual( Api.ApiState.Started, this.uut.CurrentState ); // We are started.
            Assert.IsTrue( mockAudio.IsPlaying );
            Assert.IsTrue( mockTimer.IsRunning );

            // Ensure if play-though once that config's length is ignored, and
            // is more than the audio's playtime.
            Assert.NotNull( mockTimer.Time );
            Assert.Greater( mockTimer.Time, mockAudio.GetLengthOfFileReturn );

            uut.StopSession();
            Assert.AreEqual( Api.ApiState.Stopped, this.uut.CurrentState ); // We are Stopped.
            Assert.IsFalse( mockAudio.IsPlaying );
            Assert.IsFalse( mockTimer.IsRunning );
            Assert.AreEqual( null, mockTimer.Time );
        }

        /// <summary>
        /// Ensures calling stop before start results in a no-op
        /// </summary>
        public void DoStopTestBeforeStart()
        {
            Log oldLog = uut.currentLog.Clone();

            // Sanity check, ensure we are idle
            Assert.AreEqual( Api.ApiState.Idle, this.uut.CurrentState ); // Start Idle
            Assert.IsFalse( mockTimer.IsRunning );

            uut.StopSession();
            Assert.AreEqual( oldLog, uut.currentLog );

            // Sanity check, ensure they are not the same reference
            Assert.AreNotSame( oldLog, uut.currentLog );

            // Ensure we are still not in progress.
            Assert.AreEqual( Api.ApiState.Idle, this.uut.CurrentState ); // Still in Idle.
            Assert.IsFalse( mockTimer.IsRunning );
        }

        /// <summary>
        /// Ensures that the session stops correctly.
        /// </summary>
        public void DoStopTest()
        {
            Assert.AreEqual( Api.ApiState.Idle, this.uut.CurrentState ); // Start Idle

            // First, start the session.
            uut.StartSession( new SessionConfig() );
            Assert.AreEqual( Api.ApiState.Started, this.uut.CurrentState ); // Ensure we are started.
            Assert.IsTrue( mockTimer.IsRunning );
            Assert.IsFalse( mockAudio.IsPlaying ); // Default config, should not be playing.

            DateTime oldEditTime = uut.CurrentLog.EditTime;

            // Now, end it.
            uut.StopSession();
            Assert.IsFalse( mockTimer.IsRunning );

            // Ensure the expected end time is close to DateTime.Now;
            DateTime expectedEndTime = DateTime.Now.ToUniversalTime();
            TimeSpan delta = expectedEndTime - uut.currentLog.EndTime;
            double deltaTime = delta.TotalSeconds;
            Assert.LessOrEqual( deltaTime, 5 );

            // Ensure the edit time is greater or equal 
            // to what it was when we called start.
            Assert.GreaterOrEqual( uut.CurrentLog.EditTime, oldEditTime );

            // Ensure the session is no longer in progress.
            Assert.AreEqual( Api.ApiState.Stopped, this.uut.CurrentState ); // Ensure we are stopped.
        }

        // ---- Save Tests ----

        /// <summary>
        /// Ensures calling save with no database open
        /// results in an exception.
        /// </summary>
        public void DoSaveWithNoDatabase()
        {
            // Start idle.
            Assert.AreEqual( Api.ApiState.Idle, this.uut.CurrentState );

            Assert.Catch<InvalidOperationException>(
                delegate ()
                {
                    uut.ValidateAndSaveSession();
                },
                Api.DatabaseNotOpenMessage
            );

            // Ensure we are still idle.
            Assert.AreEqual( Api.ApiState.Idle, this.uut.CurrentState );
        }

        /// <summary>
        /// Ensures calling save without calling start
        /// results in an exception.
        /// </summary>
        public void DoSaveWithNotStarting()
        {
            // First, open the database.
            try
            {
                uut.Open( dbLocation );

                // We never called start or stop. We should fail validation.
                CheckValidationFailed();
            }
            finally
            {
                uut.Close();
            }
        }

        /// <summary>
        /// Tries to save to the database the default log values.
        /// </summary>
        public void DoSaveWithDefaultLogs()
        {
            DoSaveTest();
        }

        /// <summary>
        /// Tries to save to the database with comments.
        /// </summary>
        public void DoSaveLogOnlyComments()
        {
            DoSaveTest( null, "This is a comment" );
        }

        /// <summary>
        /// Tries to save to the database with technique
        /// </summary>
        public void DoSaveLogOnlyTechnique()
        {
            DoSaveTest( "SomeTechnqiue" );
        }

        /// <summary>
        /// Tries to save to the database with only location.
        /// </summary>
        public void DoSaveLongOnlyLocation()
        {
            DoSaveTest( null, null, 10.0M, 11.0M );
        }

        /// <summary>
        /// Tries to save to the database with a technique and a comment.
        /// </summary>
        public void DoSaveLogWithCommentsAndTechnique()
        {
            DoSaveTest( "SomeTechnqiue", "This is a comment" );
        }

        /// <summary>
        /// Tries to save to the database with everything.
        /// </summary>
        public void DoSaveLogEverything()
        {
            DoSaveTest( "SomeTechnique", "This is a comment", 10.0M, 11.0M );
        }

        /// <summary>
        /// Ensures the save fails when only latitiude is passed in.
        /// </summary>
        public void DoSaveLogWithOnlyLatitude()
        {
            DoSaveTestOneLocation( 10.0M, null );
        }

        /// <summary>
        /// Ensures the save fails when only longitude is passed in.
        /// </summary>
        public void DoSaveLogWithOnlyLongitude()
        {
            DoSaveTestOneLocation( null, 10.0M );
        }

        // ---- Populate Logbook Tests ----

        /// <summary>
        /// Ensures calling PopulateLogbook with no database
        /// opened results in an exception.
        /// </summary>
        public void DoPopulateLogBookWithNoDatabase()
        {
            Assert.Catch<InvalidOperationException>(
                delegate ()
                {
                    uut.PopulateLogbook();
                },
                Api.DatabaseNotOpenMessage
            );
        }

        /// <summary>
        /// Does the save test 10 times.
        /// </summary>
        public void DoPopulateLogBookMultipleTimes()
        {
            DoSaveTest( this.uut, 10 );
        }

        // ---- Xml Tests ----

        // -- XML Schema Tests --

        /// <summary>
        /// Tests the XML schema with all values.
        /// </summary>
        public void DoXmlSchemaTest()
        {
            const string fileName = "XmlSchemaTest1.xml";

            DoSaveTest( this.uut, 5 );

            uut.Export( fileName );
            try
            {
                XmlReader reader = XmlReader.Create( Path.Combine( projectDir, "..", "MeditationLogger.Api", "Schemas", "LogXmlSchema.xsd" ) );

                XmlSchema schema = XmlSchema.Read( reader, null );

                XmlDocument doc = new XmlDocument();
                doc.Load( fileName );
                doc.Schemas.Add( schema );

                Assert.DoesNotThrow(
                    delegate ()
                    {
                        doc.Validate( null );
                    }
                );
            }
            finally
            {
                File.Delete( fileName );
            }
        }

        /// <summary>
        /// Tests the XML schema with optional values.
        /// </summary>
        public void DoXmlSchemaTestNoValues()
        {
            const string fileName = "XmlSchemaTest2.xml";

            DoSaveTest();
            DoSaveTest();
            DoSaveTest();

            uut.Export( fileName );
            try
            {
                XmlReader reader = XmlReader.Create( Path.Combine( projectDir, "..", "MeditationLogger.Api", "Schemas", "LogXmlSchema.xsd" ) );

                XmlSchema schema = XmlSchema.Read( reader, null );

                XmlDocument doc = new XmlDocument();
                doc.Load( fileName );
                doc.Schemas.Add( schema );

                Assert.DoesNotThrow(
                    delegate ()
                    {
                        doc.Validate( null );
                    }
                );
            }
            finally
            {
                File.Delete( fileName );
            }
        }

        // -- XML Export --

        /// <summary>
        /// Ensures the exporting and importing of
        /// logs via XML works.
        /// </summary>
        public void DoXmlExportImportTest()
        {
            const string fileName = "XmlImportExport.xml";
            DoImportExportTest( fileName );
        }

        // -- XML import --

        /// <summary>
        /// Ensures importing an XML file without the LogBook or Log tag results in a failure,
        /// AND the database is not updated.
        /// </summary>
        public void XmlImportBadLogbookTest()
        {
            DoBadImportTest<XmlException>( Path.Combine( projectDir, "TestFiles", "BadLogBook.xml" ) );
            DoBadImportTest<XmlException>( Path.Combine( projectDir, "TestFiles", "BadLogBook.xml" ) );
        }

        /// <summary>
        /// Ensures importing an XML file with no start or no end time results in a failure,
        /// AND the database is not updated.  Also ensures having StartTime > EndTime results
        /// in a failure.
        /// </summary>
        public void DoXmlImportNoStartTime()
        {
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "MissingStartTime.xml" ) );
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "MissingEndTime.xml" ) );
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "BadLogStartEnd.xml" ) );
        }

        /// <summary>
        /// Ensures having a missing latitude while having a longitude
        /// (or vice vera) results in a failure.
        /// </summary>
        public void DoXmlImportBadMissingLat()
        {
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "MissingLat.xml" ) );
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "MissingLong.xml" ) );
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "InvalidLat.xml" ) );
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "InvalidLong.xml" ) );
        }

        /// <summary>
        /// Ensures importing an XML file with no logs results
        /// in no error.  Nothing is added.
        /// </summary>
        public void DoXmlImportNoLogs()
        {
            DoGoodImportTest( 0, Path.Combine( projectDir, "TestFiles", "NoLogs.xml" ) );
        }

        /// <summary>
        /// Ensures importing an XML file with just start time
        /// and end time is valid.
        /// </summary>
        public void DoXmlImportJustStartAndEnd()
        {
            DoGoodImportTest( 5, Path.Combine( projectDir, "TestFiles", "JustStartAndEnd.xml" ) );
        }

        /// <summary>
        /// Ensures importing an XML file with lat and long
        /// set to not numbers results in no error, but both
        /// having null values.
        /// </summary>
        public void DoXmlImportBadLatLong()
        {
            DoGoodImportTest( 1, Path.Combine( projectDir, "TestFiles", "InvalidLatLong.xml" ) );
            Assert.IsNull( uut.LogBook.Logs[0].Latitude );
            Assert.IsNull( uut.LogBook.Logs[0].Longitude );
        }

        // ---- JSON tests ----

        // -- JSON Export --

        /// <summary>
        /// Ensures the exporting and importing of
        /// logs via XML works.
        /// </summary>
        public void DoJsonExportImportTest()
        {
            const string fileName = "JsonImportExport.json";
            DoImportExportTest( fileName );
        }

        // -- JSON import --

        /// <summary>
        /// Ensures importing a malformed json logbook results in failues,
        /// and the database is not updated.
        /// There are also cases where malformed json are okay.  Those should pass.
        /// </summary>
        public void DoJsonImportMalformedJsonTest()
        {
            DoBadImportTest<JsonReaderException>( Path.Combine( projectDir, "TestFiles", "MalformedJsonNoClosingArray.json" ) );
            DoBadImportTest<JsonReaderException>( Path.Combine( projectDir, "TestFiles", "MalformedJsonMissingComma.json" ) );
            DoBadImportTest<JsonReaderException>( Path.Combine( projectDir, "TestFiles", "MalformedJsonMissingCommaOnProperty.json" ) );
            DoGoodImportTest( 7, Path.Combine( projectDir, "TestFiles", "MalformedJsonExtraComma.json" ) );
            DoGoodImportTest( 7, Path.Combine( projectDir, "TestFiles", "MalformedJsonExtraCommaOnProperty.json" ) );
        }

        /// <summary>
        /// Ensures importing a Json file with no start or no end time results in a failure,
        /// AND the database is not updated.  Also ensures having StartTime > EndTime results
        /// in a failure.
        /// </summary>
        public void DoJsonImportNoStartTime()
        {
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "MissingStartTime.json" ) );
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "MissingEndTime.json" ) );
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "BadLogStartEnd.json" ) );
        }

        /// <summary>
        /// Ensures having a missing latitude while having a longitude
        /// (or vice vera) results in a failure.
        /// </summary>
        public void DoJsonImportBadMissingLat()
        {
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "MissingLat.json" ) );
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "MissingLong.json" ) );
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "InvalidLat.json" ) );
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "InvalidLong.json" ) );
        }

        /// <summary>
        /// Ensures importing a Json file with no logs results
        /// in no error.  Nothing is added.
        /// </summary>
        public void DoJsonImportNoLogs()
        {
            DoGoodImportTest( 0, Path.Combine( projectDir, "TestFiles", "NoLogs.json" ) );
        }

        /// <summary>
        /// Ensures importing a Json file with just start time
        /// and end time is valid.
        /// </summary>
        public void DoJsonImportJustStartAndEnd()
        {
            DoGoodImportTest( 5, Path.Combine( projectDir, "TestFiles", "JustStartAndEnd.json" ) );
        }

        /// <summary>
        /// Ensures importing an XML file with lat and long
        /// set to not numbers results in no error, but both
        /// having null values.
        /// </summary>
        public void DoJsonImportBadLatLong()
        {
            DoGoodImportTest( 1, Path.Combine( projectDir, "TestFiles", "InvalidLatLong.json" ) );
            Assert.IsNull( uut.LogBook.Logs[0].Latitude );
            Assert.IsNull( uut.LogBook.Logs[0].Longitude );
        }

        // ---- MLG import/export ----

        /// <summary>
        /// Ensures the exporting and importing of
        /// logs via MLG works.
        /// </summary>
        public void DoMlgExportImportTest()
        {
            const string fileName = "MlgImportExport.mlg";
            DoImportExportTest( fileName );
        }

        // ---- MLG Sync ----

        /// <summary>
        /// Ensures that the MLG can sync with each other when
        /// all EditTimes are different.
        /// </summary>
        public void DoMlgSyncDoSync()
        {
            const string newDbLocation = "newTestMlg.mlg";

            try
            {
                // Create an inital logbook.
                DoSaveTest( this.uut, 3, dbLocation );

                LogBook oldBook = this.uut.LogBook;

                DoSaveTest( this.uut, 3, newDbLocation );

                LogBook newBook = this.uut.LogBook;

                uut.Open( newDbLocation );
                uut.Sync( dbLocation );
                uut.Close();

                CheckSync( this.uut, dbLocation, oldBook, newDbLocation, newBook );
            }
            finally
            {
                uut.Close();
                File.Delete( newDbLocation );
            }
        }

        /// <summary>
        /// Tests to make sure sync works fine when both logbooks
        /// have log with same GUIDs.
        /// </summary>
        public void DoMlgSyncDifferentEdits()
        {
            DoSaveTest( this.uut, 2, dbLocation );
            LogBook localBook = uut.LogBook;

            // External log 1 is older than the one in the local book.
            Log extLog1 = new Log( localBook.Logs[0] );
            extLog1.Latitude = 100M;
            extLog1.Longitude = 150M;
            extLog1.Comments = "External Log 1";
            extLog1.EditTime = DateTime.MinValue;

            // External log 2 is newer than the one in the local book, it
            // should replace the log in the original.
            Log extLog2 = new Log( localBook.Logs[1] );
            extLog2.Latitude = 25M;
            extLog2.Longitude = 50M;
            extLog2.Comments = "External Log 2";
            extLog2.EditTime = Log.MaxTime;

            // Create external mlg file.
            const string extMlg = "external.mlg";

            uut.Open( extMlg );
            try
            {
                uut.InsertLog( extLog1 );
                uut.InsertLog( extLog2 );

                uut.PopulateLogbook();
                LogBook extLogbook = uut.LogBook;

                uut.Close();

                uut.Open( dbLocation );
                uut.PopulateLogbook();
                uut.Sync( extMlg );
                uut.Close();

                // Now, ensure everything was synced correctly.
                // Both the local and external logbooks should have 2
                // entries.  The first one is the local's log1 (has newer edit time),
                // and the second one should have the external's log2.

                uut.Open( dbLocation );
                uut.PopulateLogbook();
                Assert.AreEqual( localBook.Logs[0], uut.LogBook.Logs[0] );
                Assert.AreEqual( extLog2, uut.LogBook.Logs[1] );
                uut.Close();

                uut.Open( extMlg );
                uut.PopulateLogbook();
                Assert.AreEqual( localBook.Logs[0], uut.LogBook.Logs[0] );
                Assert.AreEqual( extLog2, uut.LogBook.Logs[1] );
                uut.Close();
            }
            finally
            {
                File.Delete( extMlg );
            }
        }

        /// <summary>
        /// Ensures the sync pre-checks work.
        /// </summary>
        public void DoSyncCheckTest()
        {
            // Ensures something is thrown when we didn't open the database.
            Assert.Throws<InvalidOperationException>(
                delegate ()
                {
                    uut.Sync( dbLocation );
                },
                Api.DatabaseNotOpenMessage
            );

            // Ensures something is thrown when we didn't populate the logbook.
            try
            {
                uut.Open( dbLocation );
                Assert.Throws<InvalidOperationException>(
                    delegate ()
                    {
                        uut.Sync( dbLocation );
                    },
                    Api.nullLogbook
                );
            }
            finally
            {
                uut.Close();
            }
        }

        // -- Modifying database tests --

        /// <summary>
        /// Ensures the insert function works correctly when adding a completely new log
        /// and inserting an existing log.
        /// </summary>
        public void DoAddNewLogTest()
        {
            Log log1 = new Log();
            try
            {
                uut.Open( dbLocation );
                uut.PopulateLogbook();
                uut.InsertLog( log1 );
                uut.PopulateLogbook();

                Assert.AreEqual( 1, uut.LogBook.Logs.Count );
                Assert.AreEqual( log1, uut.LogBook.Logs[0] );

                // Now, just change a few things of the log, but not the ID.
                // sqlite should update the log, and not add a new one.

                Log newLog = new Log( uut.LogBook.Logs[0] );
                newLog.Comments = "New Log!";
                newLog.Latitude = 1.0M;
                newLog.Longitude = 2.0M;

                uut.InsertLog( newLog );
                uut.PopulateLogbook();
                Assert.AreEqual( 1, uut.LogBook.Logs.Count );
                Assert.AreEqual( newLog, uut.LogBook.Logs[0] );
            }
            finally
            {
                uut.Close();
            }
        }

        // -------- Test Helpers ---------

        /// <summary>
        /// Checks to see if the sync was successful.
        /// </summary>
        /// <param name="api">The api to use.</param>
        /// <param name="oldBookLocation">The old logbook .mlg location.</param>
        /// <param name="oldBook">The old logbook object saved in memory.</param>
        /// <param name="newBookLocation">The new logbook .mlg location.</param>
        /// <param name="newBook">The new logbook object saved in memory.</param>
        public static void CheckSync( Api api, string oldBookLocation, LogBook oldBook, string newBookLocation, LogBook newBook )
        {
            try
            {
                foreach ( string mlgToCheck in new string[] { oldBookLocation, newBookLocation } )
                {
                    api.Open( mlgToCheck );
                    api.PopulateLogbook();
                    LogBook syncedLogbook = api.LogBook;

                    // First, ensure all logs in the old book exist.
                    foreach ( Log oldLog in oldBook.Logs )
                    {
                        Assert.IsTrue( syncedLogbook.LogExists( oldLog.Guid ) );
                        Assert.AreEqual( oldLog, syncedLogbook.GetLog( oldLog.Guid ) );
                    }

                    // Second, ensure all logs in the new book exist.
                    foreach ( Log newLog in newBook.Logs )
                    {
                        Assert.IsTrue( syncedLogbook.LogExists( newLog.Guid ) );
                        Assert.AreEqual( newLog, syncedLogbook.GetLog( newLog.Guid ) );
                    }

                    // Lastly, ensure all logs in the synced logbook did not magically appear;
                    // they should have came from somewhere.
                    foreach ( Log log in syncedLogbook.Logs )
                    {
                        if ( oldBook.LogExists( log.Guid ) )
                        {
                            Assert.AreEqual( log, syncedLogbook.GetLog( log.Guid ) );
                        }
                        else if ( newBook.LogExists( log.Guid ) )
                        {
                            Assert.AreEqual( log, syncedLogbook.GetLog( log.Guid ) );
                        }
                        else
                        {
                            Assert.Fail( "Could not find log in synced logbook." );
                        }
                    }
                }
            }
            finally
            {
                api.Close();
            }
        }

        /// <summary>
        /// Does the import/export test for the given XML/JSON/CSV file.
        /// </summary>
        /// <param name="fileName">The XML/JSON/CSV file to test.</param>
        private void DoImportExportTest( string fileName )
        {
            const string newDb = "test2.db";
            DoSaveTest();
            DoSaveTest();
            DoSaveTest( this.uut, 5 );

            uut.Export( fileName );

            LogBook oldBook = this.uut.LogBook;

            // Now, create a new database, and import the file file.
            // it should match the old logbook.
            uut.Open( newDb );
            try
            {
                uut.PopulateLogbook();
                // Ensure we are not the same logbook.
                Assert.AreNotSame( oldBook, uut.LogBook );

                uut.Import( fileName );
                uut.PopulateLogbook();
                LogBook newBook = uut.LogBook;

                AreLogbooksEqual( oldBook, newBook );
            }
            finally
            {
                uut.Close();
                File.Delete( newDb );
                File.Delete( fileName );
            }
        }

        /// <summary>
        /// Checks to make sure the two given logbooks contain the same logs.
        /// </summary>
        /// <param name="oldBook">The older logbook to check.</param>
        /// <param name="newBook">The newer logbook to check.</param>
        public static void AreLogbooksEqual( LogBook oldBook, LogBook newBook )
        {
            // Ensure we are not the same logbook.
            Assert.AreNotSame( oldBook, newBook );

            // Now, iterate through the logs.  Compared to the old logbook,
            // Everything should be the same EXCEPT for GUID and edit time,
            // which should be newer (assuming our computer didnt time travel).
            Assert.AreEqual( oldBook.Logs.Count, newBook.Logs.Count );

            for ( int i = 0; i < oldBook.Logs.Count; ++i )
            {
                Assert.AreEqual( oldBook.Logs[i].StartTime, newBook.Logs[i].StartTime );
                Assert.AreEqual( oldBook.Logs[i].EndTime, newBook.Logs[i].EndTime );
                Assert.AreEqual( oldBook.Logs[i].Latitude, newBook.Logs[i].Latitude );
                Assert.AreEqual( oldBook.Logs[i].Longitude, newBook.Logs[i].Longitude );
                Assert.AreEqual( oldBook.Logs[i].Technique, newBook.Logs[i].Technique );
                Assert.AreEqual( oldBook.Logs[i].Comments, newBook.Logs[i].Comments );
                Assert.AreNotEqual( newBook.Logs[i].Guid, oldBook.Logs[i].Guid );
                Assert.GreaterOrEqual( newBook.Logs[i].EditTime, oldBook.Logs[i].EditTime );
                Assert.AreNotEqual( newBook.Logs[i].Guid, new Guid() ); // Ensure the new GUID is not 0.
            }
        }

        /// <summary>
        /// Give this function a good XML/JSON/CSV file, and it will ensure
        /// the XML/JSON/CSV  file is good since it will import everything.
        /// </summary>
        /// <param name="newElements">How many new elements are going to be added.</param>
        /// <param name="importFile">The good XML/JSON/CSV file.</param>
        private void DoGoodImportTest( int newElements, string importFile )
        {
            try
            {
                uut.Open( dbLocation );

                uut.PopulateLogbook();

                int originalSize = uut.LogBook.Logs.Count;

                Assert.DoesNotThrow(
                    delegate ()
                    {
                        uut.Import( importFile );
                    }
                );

                uut.PopulateLogbook();

                // Ensure new logs were added.
                Assert.AreEqual( originalSize + newElements, uut.LogBook.Logs.Count );
            }
            finally
            {
                uut.Close();
            }
        }

        /// <summary>
        /// Give this function a bad XML/JSON/CSV file, and it will ensure
        /// the XML/JSON/CSV file is bad since it will throw the given exception.
        /// </summary>
        /// <param name="importFile">The good XML/JSON/CSV file.</param>
        /// <typeparam name="exceptionType">The exeption that is expected to be thrown.</typeparam>
        private void DoBadImportTest<exceptionType>( string importFile ) where exceptionType : Exception
        {
            try
            {
                uut.Open( dbLocation );

                uut.PopulateLogbook();

                int originalSize = uut.LogBook.Logs.Count;

                Assert.Throws<exceptionType>(
                    delegate ()
                    {
                        uut.Import( importFile );
                    }
                );

                uut.PopulateLogbook();

                // Ensure no new logs were added.
                Assert.AreEqual( originalSize, uut.LogBook.Logs.Count );
            }
            finally
            {
                uut.Close();
            }
        }

        /// <summary>
        /// Calls uut.ValidateStagged and ensures it does validate
        /// </summary>
        private void CheckValidationPassed()
        {
            Assert.DoesNotThrow(
                delegate ()
                {
                    uut.CurrentLog.Validate();
                }
            );
        }

        /// <summary>
        /// Calls uut.ValidateStagged and ensures it doesn't validate.
        /// </summary>
        /// <param name="expectedErrorStr">The expected error string. Set to null if don't-care.</param>
        private void CheckValidationFailed( string expectedErrorStr = null )
        {
            if ( expectedErrorStr == null )
            {
                Assert.Catch<LogValidationException>(
                    delegate ()
                    {
                        uut.CurrentLog.Validate();
                    }
                );
            }
            else
            {
                Assert.Catch<LogValidationException>(
                    delegate ()
                    {
                        uut.CurrentLog.Validate();
                    },
                    expectedErrorStr
                );
            }
        }

        /// <summary>
        /// Tries to save to the database
        /// </summary>
        /// <param name="technique">The technique to save</param>
        /// <param name="comments">The comments to save</param>
        /// <param name="latitude">The latitude to save</param>
        /// <param name="longitude">The longitude to save.</param>
        private void DoSaveTest( string technique = null, string comments = null, decimal? latitude = null, decimal? longitude = null )
        {
            uut.Open( dbLocation );
            try
            {
                Assert.AreEqual( Api.ApiState.Idle, this.uut.CurrentState ); // Start idle.
                uut.StartSession( new SessionConfig() );
                Assert.AreEqual( Api.ApiState.Started, this.uut.CurrentState ); // Should be started.
                uut.StopSession();
                Assert.AreEqual( Api.ApiState.Stopped, this.uut.CurrentState ); // Should be stopped.
                uut.ValidateAndSaveSession( technique, comments, latitude, longitude );
                Assert.AreEqual( Api.ApiState.Idle, this.uut.CurrentState ); // Should be idle again.

                uut.PopulateLogbook();

                Assert.AreEqual( uut.CurrentLog, uut.LogBook.Logs[0] );
                Assert.AreNotSame( uut.CurrentLog, uut.LogBook.Logs[0] );

                Assert.AreEqual(
                    ( technique == null ) ? string.Empty : technique,
                    uut.LogBook.Logs[0].Technique
                );
                Assert.AreEqual(
                    ( comments == null ) ? string.Empty : comments,
                    uut.LogBook.Logs[0].Comments
                );
            }
            finally
            {
                uut.Close();
            }
        }

        /// <summary>
        /// Does the save test for the number of entries.
        /// </summary>
        /// <param name="numberOfEntries">The number of entries to add.</param>
        /// <param name="location">The location to save to.</param>
        /// <returns>Logbook of the database after saving the data.</returns>
        public static LogBook DoSaveTest( Api api, int numberOfEntries, string location = dbLocation )
        {
            LogBook logBook = null;

            api.Open( location );
            try
            {
                for ( int i = 0; i < numberOfEntries; ++i )
                {
                    Assert.AreEqual( Api.ApiState.Idle, api.CurrentState ); // Start idle.
                    api.StartSession( new SessionConfig() );
                    Assert.AreEqual( Api.ApiState.Started, api.CurrentState ); // Now started
                    api.StopSession();
                    Assert.AreEqual( Api.ApiState.Stopped, api.CurrentState ); // Now Stopped

                    string technique = "SomeTechnique" + i;
                    string comments = "Some Comment" + i;
                    decimal latitude = i;
                    decimal longitude = i;
                    api.ValidateAndSaveSession( technique, comments, latitude, longitude );
                    Assert.AreEqual( Api.ApiState.Idle, api.CurrentState ); // Should be idle after saving.

                    api.PopulateLogbook();

                    // Most recent logs are in index 0.
                    Assert.AreEqual( api.CurrentLog, api.LogBook.Logs[0] );
                    Assert.AreNotSame( api.CurrentLog, api.LogBook.Logs[0] );

                    Assert.AreEqual(
                        technique,
                        api.LogBook.Logs[0].Technique
                    );
                    Assert.AreEqual(
                        comments,
                        api.LogBook.Logs[0].Comments
                    );

                    api.ResetStates();

                    logBook = api.LogBook;
                }
            }
            finally
            {
                api.Close();
            }

            return logBook;
        }

        /// <summary>
        /// Tries to save to the database with only latitude or longitude set, not both.
        /// Expect failure.
        /// </summary>
        /// <param name="latitude">The latitude to try.</param>
        /// <param name="longitude">The longitude to try.</param>
        private void DoSaveTestOneLocation( decimal? latitude, decimal? longitude )
        {
            uut.Open( dbLocation );
            try
            {
                Assert.AreEqual( Api.ApiState.Idle, this.uut.CurrentState ); // Start idle.
                uut.StartSession( new SessionConfig() );
                Assert.AreEqual( Api.ApiState.Started, this.uut.CurrentState ); // Ensure we are started.
                uut.StopSession();
                Assert.AreEqual( Api.ApiState.Stopped, this.uut.CurrentState ); // Ensure we are stopped.

                Assert.Catch<LogValidationException>(
                    delegate ()
                    {
                        uut.ValidateAndSaveSession( null, null, latitude, longitude );
                    }
                );

                // Ensure it wasnt saved.
                uut.PopulateLogbook();
                Assert.AreEqual( 0, uut.LogBook.Logs.Count );
                Assert.AreEqual( Api.ApiState.Stopped, this.uut.CurrentState ); // Ensure we are still stopped (never saved).
            }
            finally
            {
                uut.Close();
            }
        }
    }
}
