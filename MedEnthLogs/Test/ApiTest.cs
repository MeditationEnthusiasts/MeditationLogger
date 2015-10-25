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
using MedEnthLogsApi;
using MedEnthLogsDesktop;
using NUnit.Framework;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Test.TestFiles;
using Test.Mocks;

namespace TestCommon
{
    /// <summary>
    /// Tests the Log Class.
    /// </summary>
    [TestFixture]
    public class LogsApiTest
    {
        // -------- Fields --------

        /// <summary>
        /// Unit under test.
        /// </summary>
        private Api uut;

        private MockTimer mockTimer;

        private MockMusicManager mockAudio;

        private const string dbLocation = "test.db";

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
            uut = new Api( new Win32LocationDetector(), this.mockTimer, this.mockAudio );
        }

        [TearDown]
        public void TestTeardown()
        {
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
            uut.currentLog.StartTime = DateTime.MaxValue;
            uut.currentLog.EndTime = DateTime.MinValue;
            uut.currentLog.CreationTime = DateTime.Now;
            uut.currentLog.EditTime = uut.CurrentLog.CreationTime;
            CheckValidationFailed( Log.EndTimeLessThanStartTimeMessage );
            uut.ResetCurrentLog();

            // Ensure it doesn't validate if the edit time is less than
            // the creationtime.
            uut.currentLog.StartTime = DateTime.Now;
            uut.currentLog.EndTime = uut.CurrentLog.StartTime;
            uut.currentLog.CreationTime = DateTime.MaxValue;
            uut.currentLog.EditTime = DateTime.MinValue;
            CheckValidationFailed( Log.EditTimeLessThanCreationTimeMessage );
            uut.ResetCurrentLog();

            // Ensure everything validates if start time and end time match
            // and create time and edit time match.
            uut.currentLog.StartTime = DateTime.Now;
            uut.currentLog.EndTime = uut.CurrentLog.StartTime;
            uut.currentLog.CreationTime = DateTime.Now;
            uut.currentLog.EditTime = uut.CurrentLog.CreationTime;
            CheckValidationPassed();
            uut.ResetCurrentLog();

            // Ensure everything validates if start time is less than end time
            // and create time is less than edit time.
            uut.currentLog.StartTime = DateTime.Now;
            uut.currentLog.EndTime = DateTime.MaxValue;
            uut.currentLog.CreationTime = DateTime.Now;
            uut.currentLog.EditTime = DateTime.MaxValue;
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

            // Ensure the expected creation time is close to DateTime.Now;
            DateTime expectedCreationTime = DateTime.Now.ToUniversalTime();
            TimeSpan delta = expectedCreationTime - uut.currentLog.CreationTime;
            double deltaTime = delta.TotalSeconds;
            Assert.LessOrEqual( deltaTime, 5 );

            // Ensure start time and edit time match the creation time.
            Assert.AreEqual( uut.CurrentLog.CreationTime, uut.CurrentLog.StartTime );
            Assert.AreEqual( uut.CurrentLog.CreationTime, uut.CurrentLog.EditTime );
            Assert.IsTrue( uut.IsSessionInProgress );

            // Validation should fail, as we haven't stopped a session yet.
            CheckValidationFailed();

            // Calling start again should result in a no-op
            DateTime oldTime = uut.CurrentLog.CreationTime;
            uut.StartSession( new SessionConfig() );

            // Ensure the times didn't change.
            Assert.AreEqual( oldTime, uut.CurrentLog.CreationTime );
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
                uut.Open( new SQLite.Net.Platform.Win32.SQLitePlatformWin32(), dbLocation );

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
        
        // -- XML Schema Tests

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
                XmlReader reader = XmlReader.Create( @"..\..\..\MedEnthLogsApi\schemas\LogXmlSchema.xsd" );

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
                XmlReader reader = XmlReader.Create( @"..\..\..\MedEnthLogsApi\schemas\LogXmlSchema.xsd" );

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
            const string newDb = "test2.db";
            DoSaveTest();
            DoSaveTest();
            DoSaveTest( 5 );

            uut.Export( fileName );

            LogBook oldBook = this.uut.LogBook;

            // Now, create a new database, and import the xml file.
            // it should match the old logbook.
            uut.Open( new SQLite.Net.Platform.Win32.SQLitePlatformWin32(), newDb );
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
                // Everything should be the same EXCEPT for creation time and edit time,
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
                    Assert.GreaterOrEqual( newBook.Logs[i].CreationTime, oldBook.Logs[i].CreationTime );
                    Assert.GreaterOrEqual( newBook.Logs[i].EditTime, oldBook.Logs[i].EditTime );
                }
            }
            finally
            {
                uut.Close();
                File.Delete( newDb );
                File.Delete( fileName );
            }
        }

        // -- XML import --

        /// <summary>
        /// Ensures importing an XML file without the LogBook or Log tag results in a failure,
        /// AND the database is not updated.
        /// </summary>
        [Test]
        public void XmlImportBadLogbookTest()
        {
            DoBadXmlTest<XmlException>( @"..\..\TestFiles\BadLogBook.xml" );
            DoBadXmlTest<XmlException>( @"..\..\TestFiles\BadLogName.xml" );
        }

        /// <summary>
        /// Ensures importing an XML file with no start or no end time results in a failure,
        /// AND the database is not updated.  Also ensures having StartTime > EndTime results
        /// in a failure.
        /// </summary>
        [Test]
        public void XmlImportNoStartTime()
        {
            DoBadXmlTest<LogValidationException>( @"..\..\TestFiles\MissingStartTime.xml" );
            DoBadXmlTest<LogValidationException>( @"..\..\TestFiles\MissingEndTime.xml" );
            DoBadXmlTest<LogValidationException>( @"..\..\TestFiles\BadLogStartEnd.xml" );
        }

        /// <summary>
        /// Ensures having a missing latitude while having a longitude
        /// (or vice vera) results in a failure.
        /// </summary>
        [Test]
        public void XmlImportBadMissingLat()
        {
            DoBadXmlTest<LogValidationException>( @"..\..\TestFiles\MissingLat.xml" );
            DoBadXmlTest<LogValidationException>( @"..\..\TestFiles\MissingLong.xml" );
            DoBadXmlTest<LogValidationException>( @"..\..\TestFiles\InvalidLat.xml" );
            DoBadXmlTest<LogValidationException>( @"..\..\TestFiles\InvalidLong.xml" );
        }

        /// <summary>
        /// Ensures importing an XML file with creation time > edit time results
        /// in no error.  The reason for this is we ignore the Creation and Edit time,
        /// and just set them to DateTime.Now.
        /// </summary>
        [Test]
        public void XmlImportBadCreationEditTime()
        {
            DoGoodXmlTest( 1, @"..\..\TestFiles\BadLogCreationEdit.xml" );
        }

        /// <summary>
        /// Ensures importing an XML file with no logs results
        /// in no error.  Nothing is added.
        /// </summary>
        [Test]
        public void XmlImportNoLogs()
        {
            DoGoodXmlTest( 0, @"..\..\TestFiles\NoLogs.xml" );
        }

        /// <summary>
        /// Ensures importing an XML file with just start time
        /// and end time is valid.
        /// </summary>
        [Test]
        public void XmlImportJustStartAndEnd()
        {
            DoGoodXmlTest( 5, @"..\..\TestFiles\JustStartAndEnd.xml" );
        }

        /// <summary>
        /// Ensures importing an XML file with lat and long
        /// set to not numbers results in no error, but both
        /// having null values.
        /// </summary>
        [Test]
        public void XmlImportBadLatLong()
        {
            DoGoodXmlTest( 1, @"..\..\TestFiles\InvalidLatLong.xml" );
            Assert.IsNull( uut.LogBook.Logs[0].Latitude );
            Assert.IsNull( uut.LogBook.Logs[0].Longitude );
        }

        // -------- Test Helpers ---------

        /// <summary>
        /// Give this function a good XML file, and it will ensure
        /// the XML file is good since it will import everything.
        /// </summary>
        /// <param name="newElements">How many new elements are going to be added.</param>
        /// <param name="xmlFile">The good XML file.</param>
        private void DoGoodXmlTest( int newElements, string xmlFile )
        {
            try
            {
                uut.Open( new SQLite.Net.Platform.Win32.SQLitePlatformWin32(), dbLocation );

                uut.PopulateLogbook();

                int originalSize = uut.LogBook.Logs.Count;

                Assert.DoesNotThrow(
                    delegate ()
                    {
                        uut.Import( xmlFile );
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
        /// Give this function a bad XML file, and it will ensure
        /// the XML file is bad since it will throw the given exception.
        /// </summary>
        /// <param name="xmlFile">The bad XML file.</param>
        /// <typeparam name="exceptionType">The exeption that is expected to be thrown.</typeparam>
        private void DoBadXmlTest<exceptionType>( string xmlFile ) where exceptionType : Exception
        {
            try
            {
                uut.Open( new SQLite.Net.Platform.Win32.SQLitePlatformWin32(), dbLocation );

                uut.PopulateLogbook();

                int originalSize = uut.LogBook.Logs.Count;

                Assert.Throws<exceptionType>(
                    delegate ()
                    {
                        uut.Import( xmlFile );
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
            uut.Open( new SQLite.Net.Platform.Win32.SQLitePlatformWin32(), dbLocation );
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
        private void DoSaveTest( int numberOfEntries )
        {
            uut.Open( new SQLite.Net.Platform.Win32.SQLitePlatformWin32(), dbLocation );
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
            uut.Open( new SQLite.Net.Platform.Win32.SQLitePlatformWin32(), dbLocation );
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
