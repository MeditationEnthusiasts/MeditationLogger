using System;
using MedEnthLogsApi;
using NUnit.Framework;
using System.Collections.Generic;

namespace TestCommon
{
    /// <summary>
    /// Tests the Log Class.
    /// </summary>
    [TestFixture]
    public class LogBookTest
    {
        // -------- Fields --------

        Log log1;
        Log log2;
        Log log3;
        Log log4;

        // -------- Setup / Teardown --------

        [SetUp]
        public void TestSetup()
        {
            log1 = new Log();
            log1.StartTime = DateTime.Now + new TimeSpan( 1, 0, 0 );
            log1.CreateTime = log1.StartTime;

            log2 = new Log();
            log2.StartTime = DateTime.Now + new TimeSpan( 2, 0, 0 );
            log2.CreateTime = log2.StartTime;

            log3 = new Log();
            log3.StartTime = DateTime.Now + new TimeSpan( 3, 0, 0 );
            log3.CreateTime = log3.StartTime;

            log4 = new Log();
            log4.StartTime = DateTime.Now + new TimeSpan( 4, 0, 0 );
            log4.CreateTime = log4.StartTime;
        }

        // -------- Tests --------

        /// <summary>
        /// Ensures the log list is readonly.
        /// </summary>
        [Test]
        public void ReadonlyListTest()
        {
            LogBook uut = new LogBook( new List<ILog>() );

            // Expect Exception.
            Assert.Catch<NotSupportedException>(
                delegate()
                {
                    uut.Logs.Add( new Log() );
                }
            );
        }

        /// <summary>
        /// Ensures things get added in the correct order.
        /// Index 0 should be the newest log.
        /// </summary>
        [Test]
        public void OrderTest()
        {
            // Random order
            List <ILog> logs = new List<ILog> { log2, log4, log1, log3 };

            LogBook uut = new LogBook( logs );

            // Ensure most recent is index 0.
            Assert.AreEqual( log4, uut.Logs[0] );
            Assert.AreEqual( log3, uut.Logs[1] );
            Assert.AreEqual( log2, uut.Logs[2] );
            Assert.AreEqual( log1, uut.Logs[3] );

            // Ensure all logs exist.
            Assert.IsTrue( uut.LogExists( log1.CreateTime ) );
            Assert.IsTrue( uut.LogExists( log2.CreateTime ) );
            Assert.IsTrue( uut.LogExists( log3.CreateTime ) );
            Assert.IsTrue( uut.LogExists( log4.CreateTime ) );
        }

        /// <summary>
        /// Ensures a conflicting creation date results in the
        /// last one in the list gets saved.
        /// </summary>
        [Test]
        public void ConflictingCreationDateTest()
        {
            // Make conflict.
            log2.CreateTime = log1.CreateTime;

            List<ILog> logs = new List<ILog> { log1, log2 };

            LogBook uut = new LogBook( logs );

            // Only one log should be added.
            Assert.AreEqual( 1, uut.Logs.Count );
            Assert.AreEqual( log2, uut.Logs[0] );

            // Ensure log exists.
            Assert.IsTrue( uut.LogExists( log2.CreateTime ) );
        }

        /// <summary>
        /// Ensures the LogExists function works as expected.
        /// </summary>
        [Test]
        public void LogExistsTest()
        {
            // Do not add log 2
            List<ILog> logs = new List<ILog> { log1 };

            LogBook uut = new LogBook( logs );

            Assert.IsTrue( uut.LogExists( log1.CreateTime ) );
            Assert.IsFalse( uut.LogExists( log2.CreateTime ) );
        }

        /// <summary>
        /// Ensures the GetLog function works as expected.
        /// </summary>
        [Test]
        public void GetLogTest()
        {
            // Do not add log 2
            List<ILog> logs = new List<ILog> { log1 };

            LogBook uut = new LogBook( logs );

            // Ensure when we get log 1, the logs match
            // and the reference is the same.
            Log log1Clone = uut.GetLog( log1.CreateTime ) as Log;
            Assert.AreEqual( log1, log1Clone );
            Assert.AreSame( log1, log1Clone );

            // Ensure key not found exception is thrown
            // if a log doesn't exist.
            Assert.Catch<KeyNotFoundException>(
                delegate ()
                {
                    uut.GetLog( log2.CreateTime );
                }
            );
        }
    }
}
