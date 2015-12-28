// 
// Meditation Logger.
// Copyright (C) 2015  Seth Hendrick.
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
using System.IO;
using System.Xml;
using System.Xml.Schema;
using MedEnthLogsApi;
using MedEnthLogsDesktop;
using Newtonsoft.Json;
using NUnit.Framework;
using Test.Mocks;
using Test.TestFiles;

namespace TestCommon
{
    /// <summary>
    /// Tests the Api Class.
    /// </summary>
    [TestFixture]
    public partial class LogsApiTest
    {
        // -------- Fields --------

        /// <summary>
        /// Unit under test.
        /// </summary>
        private Api uut;

        private MockTimer mockTimer;

        private MockMusicManager mockAudio;

        private const string dbLocation = "test.mlg";

        // -------- Setup/Teardown --------

        [SetUp]
        public void TestSetup()
        {
            if ( File.Exists( dbLocation ) )
            {
                File.Delete( dbLocation );
            }

            this.mockTimer = new MockTimer();
            this.mockAudio = new MockMusicManager();
            uut = GetApi();
        }

        [TearDown]
        public void TestTeardown()
        {
            uut.Close();

            if ( File.Exists( dbLocation ) )
            {
                File.Delete( dbLocation );
            }
        }

        // -------- Tests --------

        /// <summary>
        /// Ensures the ResetCurrent function works.
        /// </summary>
        [Test]
        public void ResetCurrentTest()
        {
            uut.ResetCurrentLog();

            // Ensure stagged log is the equivalent of 
            Assert.AreEqual( new Log(), uut.CurrentLog );
        }

        /// <summary>
        /// Ensures the ValidateStagged method works.
        /// </summary>
        [Test]
        public void ValidateTest()
        {
            // Ensure it doesn't validate if the start time is
            // later than the end time.
            uut.currentLog.StartTime = Log.MaxTime;
            uut.currentLog.EndTime = DateTime.MinValue;

            CheckValidationFailed( Log.EndTimeLessThanStartTimeMessage );
            uut.ResetCurrentLog();

            // Ensure everything validates if start time and end time.
            uut.currentLog.StartTime = DateTime.Now;
            uut.currentLog.EndTime = uut.CurrentLog.StartTime;
            CheckValidationPassed();
            uut.ResetCurrentLog();

            // Ensure everything validates if start time is less than end time.
            uut.currentLog.StartTime = DateTime.Now;
            uut.currentLog.EndTime = Log.MaxTime;
            CheckValidationPassed();
            uut.ResetCurrentLog();
        }

        /// <summary>
        /// Ensures the behavior is correct when StartSession is called.
        /// </summary>
        [Test]
        public void StartTest()
        {
            uut.StartSession( new SessionConfig() );
            Assert.IsTrue( mockTimer.IsRunning );
            Assert.IsFalse( mockAudio.IsPlaying ); // Default setting passed in, should not be playing.

            // Ensure the expected edit time is close to DateTime.Now;
            DateTime expectedCreationTime = DateTime.Now.ToUniversalTime();
            TimeSpan delta = expectedCreationTime - uut.currentLog.EditTime;
            double deltaTime = delta.TotalSeconds;
            Assert.LessOrEqual( deltaTime, 5 );

            // Ensure start time and edit time match the creation time.
            Assert.AreEqual( uut.CurrentLog.EditTime, uut.CurrentLog.StartTime );
            Assert.IsTrue( uut.IsSessionInProgress );

            // Validation should fail, as we haven't stopped a session yet.
            CheckValidationFailed();

            // Calling start again should result in a no-op
            DateTime oldTime = uut.CurrentLog.EditTime;
            uut.StartSession( new SessionConfig() );

            // Ensure the times didn't change.
            Assert.AreEqual( oldTime, uut.CurrentLog.StartTime );
            Assert.AreEqual( oldTime, uut.CurrentLog.EditTime );

            // In progress should still be true
            Assert.IsTrue( uut.IsSessionInProgress );
            Assert.IsTrue( mockTimer.IsRunning );

            // Lastly, ensure if we call save, we get an exception.
            Assert.Catch<InvalidOperationException>(
                delegate ()
                {
                    uut.ValidateAndSaveSession();
                }
            );
        }

        [Test]
        public void StartStopWithLoopingMusic()
        {
            SessionConfig config = new SessionConfig();
            config.PlayMusic = true;
            config.LoopMusic = true;
            config.Length = new TimeSpan( 0, 1, 0 );
            config.AudioFile = "test.mp3";

            uut.StartSession( config );
            Assert.IsTrue( uut.IsSessionInProgress );
            Assert.IsTrue( mockAudio.IsPlaying );
            Assert.IsTrue( mockTimer.IsRunning );
            Assert.AreEqual( config.Length, mockTimer.Time );

            uut.StopSession();
            Assert.IsFalse( uut.IsSessionInProgress );
            Assert.IsFalse( mockAudio.IsPlaying );
            Assert.IsFalse( mockTimer.IsRunning );
            Assert.AreEqual( null, mockTimer.Time );
        }

        [Test]
        public void StartWithNoMusic()
        {
            SessionConfig config = new SessionConfig();
            config.PlayMusic = false;
            config.LoopMusic = true;
            config.Length = new TimeSpan( 0, 1, 0 );
            config.AudioFile = "test.mp3";

            uut.StartSession( config );
            Assert.IsTrue( uut.IsSessionInProgress );
            Assert.IsFalse( mockAudio.IsPlaying ); // Play music is false, this should be false.
            Assert.IsTrue( mockTimer.IsRunning );
            Assert.AreEqual( config.Length, mockTimer.Time );
        }

        [Test]
        public void StartStopWithInvalidMusicConfig()
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
            Assert.IsFalse( uut.IsSessionInProgress );
            Assert.IsFalse( mockAudio.IsPlaying );
            Assert.IsFalse( mockTimer.IsRunning );
            Assert.AreEqual( null, mockTimer.Time );
            Assert.AreEqual( new Log(), uut.CurrentLog );
        }

        [Test]
        public void StartStopWithPlayOnceMusic()
        {
            SessionConfig config = new SessionConfig();
            config.PlayMusic = true;
            config.LoopMusic = false;
            config.Length = new TimeSpan( 0, 1, 0 );
            config.AudioFile = "test.mp3";

            this.mockAudio.GetLengthOfFileReturn = new TimeSpan( 1, 0, 0 );

            uut.StartSession( config );
            Assert.IsTrue( uut.IsSessionInProgress );
            Assert.IsTrue( mockAudio.IsPlaying );
            Assert.IsTrue( mockTimer.IsRunning );

            // Ensure if play-though once that config's length is ignored, and
            // is more than the audio's playtime.
            Assert.Greater( mockTimer.Time, config.Length );
            Assert.Greater( mockTimer.Time, mockAudio.GetLengthOfFileReturn );

            uut.StopSession();
            Assert.IsFalse( uut.IsSessionInProgress );
            Assert.IsFalse( mockAudio.IsPlaying );
            Assert.IsFalse( mockTimer.IsRunning );
            Assert.AreEqual( null, mockTimer.Time );
        }

        [Test]
        public void StartStopWithPlayOnceNullLength()
        {
            SessionConfig config = new SessionConfig();
            config.PlayMusic = true;
            config.LoopMusic = false;
            config.Length = null;
            config.AudioFile = "test.mp3";

            this.mockAudio.GetLengthOfFileReturn = new TimeSpan( 1, 0, 0 );

            uut.StartSession( config );
            Assert.IsTrue( uut.IsSessionInProgress );
            Assert.IsTrue( mockAudio.IsPlaying );
            Assert.IsTrue( mockTimer.IsRunning );

            // Ensure if play-though once that config's length is ignored, and
            // is more than the audio's playtime.
            Assert.NotNull( mockTimer.Time );
            Assert.Greater( mockTimer.Time, mockAudio.GetLengthOfFileReturn );

            uut.StopSession();
            Assert.IsFalse( uut.IsSessionInProgress );
            Assert.IsFalse( mockAudio.IsPlaying );
            Assert.IsFalse( mockTimer.IsRunning );
            Assert.AreEqual( null, mockTimer.Time );
        }

        /// <summary>
        /// Ensures calling stop before start results in a no-op
        /// </summary>
        [Test]
        public void StopTestBeforeStart()
        {
            Log oldLog = uut.currentLog.Clone();

            // Sanity check, ensure we are not in progress.
            Assert.IsFalse( uut.IsSessionInProgress );
            Assert.IsFalse( mockTimer.IsRunning );

            uut.StopSession();
            Assert.AreEqual( oldLog, uut.currentLog );

            // Sanity check, ensure they are not the same reference
            Assert.AreNotSame( oldLog, uut.currentLog );

            // Ensure we are still not in progress.
            Assert.IsFalse( uut.IsSessionInProgress );
            Assert.IsFalse( mockTimer.IsRunning );
        }

        [Test]
        public void StopTest()
        {
            // First, start the session.
            uut.StartSession( new SessionConfig() );
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
            Assert.IsFalse( uut.IsSessionInProgress );
        }

        /// <summary>
        /// Ensures calling save with no database open
        /// results in an exception.
        /// </summary>
        [Test]
        public void SaveWithNoDatabase()
        {
            Assert.Catch<InvalidOperationException>(
                delegate ()
                {
                    uut.ValidateAndSaveSession();
                },
                Api.DatabaseNotOpenMessage
            );
        }

        /// <summary>
        /// Ensures calling save without calling start
        /// results in an exception.
        /// </summary>
        [Test]
        public void SaveWithNotStarting()
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
        /// Tries to save to the database.
        /// </summary>
        [Test]
        public void SaveLogDefaults()
        {
            DoSaveTest();
        }

        /// <summary>
        /// Tries to save to the database with comments.
        /// </summary>
        [Test]
        public void SaveLogOnlyComments()
        {
            DoSaveTest( null, "This is a comment" );
        }

        /// <summary>
        /// Tries to save to the database with technique
        /// </summary>
        [Test]
        public void SaveLogOnlyTechnique()
        {
            DoSaveTest( "SomeTechnqiue" );
        }

        /// <summary>
        /// Tries to save to the database with only location.
        /// </summary>
        [Test]
        public void SaveLongOnlyLocation()
        {
            DoSaveTest( null, null, 10.0M, 11.0M );
        }

        /// <summary>
        /// Tries to save to the database.
        /// </summary>
        [Test]
        public void SaveLogWithCommentsAndTechnique()
        {
            DoSaveTest( "SomeTechnqiue", "This is a comment" );
        }

        /// <summary>
        /// Tries to save to the database with everything.
        /// </summary>
        [Test]
        public void SaveLogEverything()
        {
            DoSaveTest( "SomeTechnique", "This is a comment", 10.0M, 11.0M );
        }

        /// <summary>
        /// Ensures the save fails when only latitiude is passed in.
        /// </summary>
        [Test]
        public void SaveLogWithOnlyLatitude()
        {
            DoSaveTestOneLocation( 10.0M, null );
        }

        /// <summary>
        /// Ensures the save fails when only longitude is passed in.
        /// </summary>
        [Test]
        public void SaveLogWithOnlyLongitude()
        {
            DoSaveTestOneLocation( null, 10.0M );
        }

        /// <summary>
        /// Ensures calling PopulateLogbook with no database
        /// opened results in an exception.
        /// </summary>
        [Test]
        public void PopulateLogBookWithNoDatabase()
        {
            Assert.Catch<InvalidOperationException>(
                delegate ()
                {
                    uut.PopulateLogbook();
                },
                Api.DatabaseNotOpenMessage
            );
        }

        [Test]
        public void PopulateLogBookMultipleTimes()
        {
            DoSaveTest( 10 );
        }

        // ---- Xml Tests ----
        
        // -- XML Schema Tests --

        /// <summary>
        /// Tests the XML schema with all values.
        /// </summary>
        [Test]
        public void XmlSchemaTest()
        {
            const string fileName = "XmlSchemaTest1.xml";

            DoSaveTest( 5 );

            uut.Export( fileName );
            try
            {
                XmlReader reader = XmlReader.Create( Path.Combine( projectDir, "..", "MedEnthLogsApi", "schemas", "LogXmlSchema.xsd" ) );

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
        [Test]
        public void XmlSchemaTestNoValues()
        {
            const string fileName = "XmlSchemaTest2.xml";

            DoSaveTest();
            DoSaveTest();
            DoSaveTest();

            uut.Export( fileName );
            try
            {
                XmlReader reader = XmlReader.Create( Path.Combine( projectDir, "..", "MedEnthLogsApi", "schemas", "LogXmlSchema.xsd" ) );

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
        [Test]
        public void XmlExportImportTest()
        {
            const string fileName = "XmlImportExport.xml";
            DoImportExportTest( fileName );
        }

        // -- XML import --

        /// <summary>
        /// Ensures importing an XML file without the LogBook or Log tag results in a failure,
        /// AND the database is not updated.
        /// </summary>
        [Test]
        public void XmlImportBadLogbookTest()
        {
            DoBadImportTest<XmlException>( Path.Combine( projectDir, "TestFiles", "BadLogBook.xml" ) );
            DoBadImportTest<XmlException>( Path.Combine( projectDir, "TestFiles", "BadLogBook.xml" )  );
        }

        /// <summary>
        /// Ensures importing an XML file with no start or no end time results in a failure,
        /// AND the database is not updated.  Also ensures having StartTime > EndTime results
        /// in a failure.
        /// </summary>
        [Test]
        public void XmlImportNoStartTime()
        {
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "MissingStartTime.xml" ) );
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "MissingEndTime.xml" ) ) ;
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "BadLogStartEnd.xml" ) );
        }

        /// <summary>
        /// Ensures having a missing latitude while having a longitude
        /// (or vice vera) results in a failure.
        /// </summary>
        [Test]
        public void XmlImportBadMissingLat()
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
        [Test]
        public void XmlImportNoLogs()
        {
            DoGoodImportTest( 0, Path.Combine( projectDir, "TestFiles", "NoLogs.xml" ) );
        }

        /// <summary>
        /// Ensures importing an XML file with just start time
        /// and end time is valid.
        /// </summary>
        [Test]
        public void XmlImportJustStartAndEnd()
        {
            DoGoodImportTest( 5, Path.Combine( projectDir, "TestFiles", "JustStartAndEnd.xml" ) );
        }

        /// <summary>
        /// Ensures importing an XML file with lat and long
        /// set to not numbers results in no error, but both
        /// having null values.
        /// </summary>
        [Test]
        public void XmlImportBadLatLong()
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
        [Test]
        public void JsonExportImportTest()
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
        [Test]
        public void JsonImportMalformedJsonTest()
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
        [Test]
        public void JsonImportNoStartTime()
        {
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "MissingStartTime.json" ) );
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "MissingEndTime.json" ) );
            DoBadImportTest<LogValidationException>( Path.Combine( projectDir, "TestFiles", "BadLogStartEnd.json" ) );
        }
        
        /// <summary>
        /// Ensures having a missing latitude while having a longitude
        /// (or vice vera) results in a failure.
        /// </summary>
        [Test]
        public void JsonImportBadMissingLat()
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
        [Test]
        public void JsonImportNoLogs()
        {
            DoGoodImportTest( 0, Path.Combine( projectDir, "TestFiles", "NoLogs.json" ) );
        }

        /// <summary>
        /// Ensures importing a Json file with just start time
        /// and end time is valid.
        /// </summary>
        [Test]
        public void JsonImportJustStartAndEnd()
        {
            DoGoodImportTest( 5, Path.Combine( projectDir, "TestFiles", "JustStartAndEnd.json" ) );
        }
        
        /// <summary>
        /// Ensures importing an XML file with lat and long
        /// set to not numbers results in no error, but both
        /// having null values.
        /// </summary>
        [Test]
        public void JsonImportBadLatLong()
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
        [Test]
        public void MlgExportImportTest()
        {
            const string fileName = "MlgImportExport.mlg";
            DoImportExportTest( fileName );
        }

        // ---- MLG Sync ----

        /// <summary>
        /// Ensures that the MLG can sync with each other when
        /// all EditTimes are different.
        /// </summary>
        [Test]
        public void MlgSyncDoSync()
        {
            const string newDbLocation = "newTestMlg.mlg";

            try
            {
                // Create an inital logbook.
                DoSaveTest( 3, dbLocation );

                LogBook oldBook = this.uut.LogBook;

                DoSaveTest( 3, newDbLocation );

                LogBook newBook = this.uut.LogBook;

                uut.Open( newDbLocation );
                uut.Sync( dbLocation );
                uut.Close();

                CheckSync( dbLocation, oldBook, newDbLocation, newBook );
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
        [Test]
        public void MlgSyncDifferentEdits()
        {
            DoSaveTest( 2, dbLocation );
            LogBook localBook = uut.LogBook;

            // External log 1 is older than the one in the local book.
            Log extLog1 = new Log ( localBook.Logs[0] );
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
        [Test]
        public void SyncCheckTest()
        {
            // Ensures something is thrown when we didn't open the database.
            Assert.Throws<InvalidOperationException>(
                delegate()
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

        /// <summary>
        /// Ensures the insert function works correctly when adding a completely new log
        /// and inserting an existing log.
        /// </summary>
        [Test]
        public void AddNewLogTest()
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

                Log newLog = new Log ( uut.LogBook.Logs[0] );
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
        /// <param name="oldBookLocation">The old logbook .mlg location.</param>
        /// <param name="oldBook">The old logbook object saved in memory.</param>
        /// <param name="newBookLocation">The new logbook .mlg location.</param>
        /// <param name="newBook">The new logbook object saved in memory.</param>
        private void CheckSync( string oldBookLocation, LogBook oldBook, string newBookLocation, LogBook newBook )
        {
            try
            {
                foreach ( string mlgToCheck in new string [] { oldBookLocation, newBookLocation } )
                {
                    uut.Open( mlgToCheck );
                    uut.PopulateLogbook();
                    LogBook syncedLogbook = uut.LogBook;

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
                uut.Close();
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
            DoSaveTest( 5 );

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

                // Ensure we are not the same logbook reference.
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
            finally
            {
                uut.Close();
                File.Delete( newDb );
                File.Delete( fileName );
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
                uut.StartSession( new SessionConfig() );
                uut.StopSession();
                uut.ValidateAndSaveSession( technique, comments, latitude, longitude );

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
        private void DoSaveTest( int numberOfEntries, string location = dbLocation )
        {
            uut.Open( location );
            try
            {
                for ( int i = 0; i < numberOfEntries; ++i )
                {
                    uut.StartSession( new SessionConfig() );
                    uut.StopSession();

                    string technique = "SomeTechnique" + i;
                    string comments = "Some Comment" + i;
                    decimal latitude = i;
                    decimal longitude = i;
                    uut.ValidateAndSaveSession( technique, comments, latitude, longitude );

                    uut.PopulateLogbook();

                    // Most recent logs are in index 0.
                    Assert.AreEqual( uut.CurrentLog, uut.LogBook.Logs[0] );
                    Assert.AreNotSame( uut.CurrentLog, uut.LogBook.Logs[0] );

                    Assert.AreEqual(
                        technique,
                        uut.LogBook.Logs[0].Technique
                    );
                    Assert.AreEqual(
                        comments,
                        uut.LogBook.Logs[0].Comments
                    );

                    uut.ResetCurrentLog();
                }
            }
            finally
            {
                uut.Close();
            }
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
                uut.StartSession( new SessionConfig() );
                uut.StopSession();

                Assert.Catch<LogValidationException>(
                    delegate ()
                    {
                        uut.ValidateAndSaveSession( null, null, latitude, longitude );
                    }
                );

                // Ensure it wasnt saved.
                uut.PopulateLogbook();
                Assert.AreEqual( 0, uut.LogBook.Logs.Count );
            }
            finally
            {
                uut.Close();
            }
        }
    }
}
