﻿using System;
using MedEnthLogsApi;
using NUnit.Framework;

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
        MedEnthLogsApi.MedEnthLogsApi uut;

        // -------- Setup/Teardown --------

        [SetUp]
        public void TestSetup()
        {
            uut = new MedEnthLogsApi.MedEnthLogsApi();
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
            uut.currentLog.CreateTime = DateTime.Now;
            uut.currentLog.EditTime = uut.CurrentLog.CreateTime;
            CheckValidationFailed( MedEnthLogsApi.MedEnthLogsApi.EndTimeLessThanStartTimeMessage );
            uut.ResetCurrentLog();

            // Ensure it doesn't validate if the edit time is less than
            // the creationtime.
            uut.currentLog.StartTime = DateTime.Now;
            uut.currentLog.EndTime = uut.CurrentLog.StartTime;
            uut.currentLog.CreateTime = DateTime.MaxValue;
            uut.currentLog.EditTime = DateTime.MinValue;
            CheckValidationFailed( MedEnthLogsApi.MedEnthLogsApi.EditTimeLessThanCreationTimeMessage );
            uut.ResetCurrentLog();

            // Ensure everything validates if start time and end time match
            // and create time and edit time match.
            uut.currentLog.StartTime = DateTime.Now;
            uut.currentLog.EndTime = uut.CurrentLog.StartTime;
            uut.currentLog.CreateTime = DateTime.Now;
            uut.currentLog.EditTime = uut.CurrentLog.CreateTime;
            CheckValidationPassed();
            uut.ResetCurrentLog();

            // Ensure everything validates if start time is less than end time
            // and create time is less than edit time.
            uut.currentLog.StartTime = DateTime.Now;
            uut.currentLog.EndTime = DateTime.MaxValue;
            uut.currentLog.CreateTime = DateTime.Now;
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
            uut.StartSession();

            // Ensure the expected creation time is close to DateTime.Now;
            DateTime expectedCreationTime = DateTime.Now.ToUniversalTime();
            TimeSpan delta = expectedCreationTime - uut.currentLog.CreateTime;
            double deltaTime = delta.TotalSeconds;
            Assert.LessOrEqual( deltaTime, 5 );

            // Ensure start time and edit time match the creation time.
            Assert.AreEqual( uut.CurrentLog.CreateTime, uut.CurrentLog.StartTime );
            Assert.AreEqual( uut.CurrentLog.CreateTime, uut.CurrentLog.EditTime );
            Assert.IsTrue( uut.IsSessionInProgress );

            // Validation should fail, as we haven't stopped a session yet.
            CheckValidationFailed();

            // Calling start again should result in a no-op
            DateTime oldTime = uut.CurrentLog.CreateTime;
            uut.StartSession();

            // Ensure the times didn't change.
            Assert.AreEqual( oldTime, uut.CurrentLog.CreateTime );
            Assert.AreEqual( oldTime, uut.CurrentLog.StartTime );
            Assert.AreEqual( oldTime, uut.CurrentLog.EditTime );

            // In progress should still be true
            Assert.IsTrue( uut.IsSessionInProgress );

            // Lastly, ensure if we call save, we get an exception.
            Assert.Catch<InvalidOperationException>(
                delegate ()
                {
                    uut.ValidateAndSaveSession();
                }
            );
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

            uut.StopSession();
            Assert.AreEqual( oldLog, uut.currentLog );

            // Sanity check, ensure they are not the same reference
            Assert.AreNotSame( oldLog, uut.currentLog );

            // Ensure we are still not in progress.
            Assert.IsFalse( uut.IsSessionInProgress );
        }

        [Test]
        public void StopTest()
        {
            // First, start the session.
            uut.StartSession();

            DateTime oldEditTime = uut.CurrentLog.EditTime;

            // Now, end it.
            uut.StopSession();

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
                MedEnthLogsApi.MedEnthLogsApi.DatabaseNotOpenMessage
            );
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
                MedEnthLogsApi.MedEnthLogsApi.DatabaseNotOpenMessage
            );
        }

        // -------- Test Helpers ---------

        /// <summary>
        /// Calls uut.ValidateStagged and ensures it does validate
        /// </summary>
        private void CheckValidationPassed()
        {
            Assert.DoesNotThrow(
                delegate()
                {
                    uut.ValidateCurrentLog();
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
                        uut.ValidateCurrentLog();
                    }
                );
            }
            else
            {
                Assert.Catch<LogValidationException>(
                    delegate ()
                    {
                        uut.ValidateCurrentLog();
                    },
                    expectedErrorStr
                );
            }
        }
    }
}