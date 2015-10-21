﻿// 
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
        double expectedTotalTime;

        // -------- Setup / Teardown --------

        [SetUp]
        public void TestSetup()
        {
            log1 = new Log();
            log1.StartTime = new DateTime( 2015, 1, 1, 0, 0, 0 );
            log1.EndTime = log1.StartTime + new TimeSpan( 1, 0, 0 );
            log1.CreateTime = log1.StartTime;

            log2 = new Log();
            log2.StartTime = new DateTime( 2015, 1, 2, 0, 0, 0 );
            log2.EndTime = log2.StartTime + new TimeSpan( 2, 0, 0 );
            log2.CreateTime = log2.StartTime;

            log3 = new Log();
            log3.StartTime = new DateTime( 2015, 1, 3, 0, 0, 0 );
            log3.EndTime = log3.StartTime + new TimeSpan( 3, 0, 0 );
            log3.CreateTime = log3.StartTime;

            // Make log4 the most recent, and have the longest session.
            log4 = new Log();
            log4.StartTime = new DateTime( 2015, 1, 4, 0, 0, 0 );
            log4.EndTime = log4.StartTime + new TimeSpan( 4, 0, 0 );
            log4.CreateTime = log4.StartTime;

            expectedTotalTime = log1.Duration.TotalMinutes +
                                log2.Duration.TotalMinutes +
                                log3.Duration.TotalMinutes +
                                log4.Duration.TotalMinutes;
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

            // Ensure the total time and longest time are what they should be.
            Assert.AreEqual( this.expectedTotalTime, uut.TotalTime, 1.0 );
            Assert.AreEqual( log4.Duration.TotalMinutes, uut.LongestTime, 0.1 );
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
