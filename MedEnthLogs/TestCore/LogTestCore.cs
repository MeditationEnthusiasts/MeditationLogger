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
using MeditationEnthusiasts.MeditationLogger.Api;
using NUnit.Framework;

namespace TestCore
{
    public class LogTestCore
    {
        // -------- Fields --------

        /// <summary>
        /// Unit under test.
        /// </summary>
        private Log uut;

        // --------- Constructor --------

        /// <summary>
        /// Constructor
        /// </summary>
        public LogTestCore()
        {
            this.uut = new Log();
        }

        // -------- Tests --------

        /// <summary>
        /// Ensures that the duration property is the end time minus the start time.
        /// </summary>
        public void DoTestDuration()
        {
            DateTime start = DateTime.MinValue;
            DateTime end = Log.MaxTime;

            uut.StartTime = start;
            uut.EndTime = end;

            TimeSpan expected = end - start;

            Assert.AreEqual( expected, uut.Duration );
        }

        /// <summary>
        /// Tests the GetHashCode function.
        /// </summary>
        public void DoTestGetHashCode()
        {
            // Ensures that the hash code of the log is the same as the hashcode
            // of the guid.  Two logs in a logbook can not have the same guid.
            Assert.AreEqual( uut.Guid.GetHashCode(), uut.GetHashCode() );
        }

        /// <summary>
        /// Ensures we can't set the comments to null,
        /// and what we set is what we get.
        /// </summary>
        public void DoCommentsTest()
        {
            // Expect Exception.
            Assert.Catch<ArgumentNullException>(
                delegate ()
                {
                    uut.Comments = null;
                }
            );

            string expectedComments = "Did stuff";
            uut.Comments = expectedComments;
            Assert.AreEqual( expectedComments, uut.Comments );
        }

        /// <summary>
        /// Ensures we can't set the technique to null,
        /// and what we set is what we get.
        /// </summary>
        public void DoTechniqueTest()
        {
            // Expect Exception.
            Assert.Catch<ArgumentNullException>(
                delegate ()
                {
                    uut.Technique = null;
                }
            );

            string expectedLocation = "My Room";
            uut.Technique = expectedLocation;
            Assert.AreEqual( expectedLocation, uut.Technique );
        }

        /// <summary>
        /// Tests the Log's equal function, operator==, and operator!=
        /// </summary>
        public void DoEqualsTest()
        {
            Log other = new Log();

            // Ensure when we create a new log, the GUIDs do not match.
            // Operator == doesn't care if guids match or not.
            Assert.AreNotEqual( uut.Guid, other.Guid );

            // Ensure uut and other match.
            CheckLogsEqual( uut, other );

            // Ensure changing the ID does not cause the equality to fail
            other.Id = uut.Id + 10;
            CheckLogsEqual( uut, other );

            // Now, start changing everything, and we should assert false.

            // Change Start Time.
            other.StartTime = DateTime.Now;
            CheckLogsNotEqual( uut, other );
            other.StartTime = uut.StartTime;

            // Change End Time.
            other.EndTime = DateTime.Now;
            CheckLogsNotEqual( uut, other );
            other.EndTime = uut.EndTime;

            // Change Edit Time.
            other.EditTime = DateTime.Now;
            CheckLogsNotEqual( uut, other );
            other.EditTime = uut.EditTime;

            // Change Comments.
            other.Comments = "Hello world!";
            CheckLogsNotEqual( uut, other );
            other.Comments = uut.Comments;

            // Change Technique.
            other.Technique = "My Room";
            CheckLogsNotEqual( uut, other );
            other.Technique = uut.Technique;

            // Change Latitude, with one being null.
            other.Latitude = 10.0M;
            uut.Latitude = null;
            CheckLogsNotEqual( uut, other );
            CheckLogsNotEqual( other, uut );
            // Check Latitude again, with both being not-null.
            uut.Latitude = 11.0M;
            CheckLogsNotEqual( uut, other );
            CheckLogsNotEqual( other, uut );
            other.Latitude = null;
            uut.Latitude = null;

            // Change Longitude, with one being null.
            other.Longitude = 10.0M;
            uut.Longitude = null;
            CheckLogsNotEqual( uut, other );
            CheckLogsNotEqual( other, uut );
            // Check Longitude again, with both being not-null.
            uut.Longitude = 11.0M;
            CheckLogsNotEqual( uut, other );
            CheckLogsNotEqual( other, uut );
            other.Longitude = null;
            uut.Longitude = null;

            // Check passing in nulls.
            Assert.IsFalse( uut.Equals( null ) );
        }

        /// <summary>
        /// Ensures the clone method creates a new instance,
        /// but all properties match.
        /// </summary>
        public void DoCloneTest()
        {
            Log clone = uut.Clone();
            Assert.AreNotSame( clone, uut );
            Assert.AreEqual( clone, uut );
        }

        public void DoValidationPassTest()
        {
            // Reset.
            uut = new Log();

            // Start time == End Time should pass.
            uut.EditTime = DateTime.Now;
            uut.StartTime = DateTime.Now;
            uut.EndTime = uut.StartTime;
            ValidationPassedTest( uut );

            // Start time < End Time should pass.
            uut.EndTime = uut.StartTime + new TimeSpan( 0, 0, 1 );
            ValidationPassedTest( uut );

            // Reset.
            uut = new Log();
            // Set the times so the thing validates.
            uut.EditTime = DateTime.Now;
            uut.StartTime = DateTime.Now;
            uut.EndTime = DateTime.Now;

            // Null lat/long should pass.
            uut.Latitude = null;
            uut.Longitude = null;
            ValidationPassedTest( uut );

            // Lat/Lon with values should pass.
            uut.Latitude = 1.0M;
            uut.Longitude = 1.0M;
            ValidationPassedTest( uut );
        }

        public void DoValidationFailTest()
        {
            // A default log should fail throw.
            ValidationFailedTest( uut, Log.EndTimeLessThanStartTimeMessage );

            // Reset.
            uut = new Log();

            // Start time > End Time should pass.
            uut.StartTime = DateTime.Now;
            uut.EndTime = uut.StartTime - new TimeSpan( 0, 0, 1 );
            ValidationFailedTest( uut, Log.EndTimeLessThanStartTimeMessage );

            // Reset.
            uut = new Log();

            // Having one null and one not should fail.
            uut.Latitude = null;
            uut.Longitude = 10.0M;
            ValidationFailedTest( uut, Log.LatitudeSetNoLongitude );

            // Lat/Lon with values should pass.
            uut.Latitude = 1.0M;
            uut.Longitude = null;
            ValidationFailedTest( uut, Log.LongitudeSetNoLatitude );
        }

        // ---- Sync Tests ----

        /// <summary>
        /// Ensures the santity checks of Log.Sync work correctly.
        /// </summary>
        public void DoLogSyncExceptionCheckTest()
        {
            // Both new logs have different GUIDs.  We'll get an exception.
            Log log1 = new Log();
            Log log2 = new Log();

            Assert.Throws<InvalidOperationException>(
                delegate ()
                {
                    Log.Sync( ref log1, ref log2 );
                }
            );
        }

        /// <summary>
        /// Ensures all is well if both logs are equal.
        /// </summary>
        public void DoLogSyncBothEqual()
        {
            Log log1 = new Log();
            Log log2 = log1.Clone();
            log2.Id = log1.Id + 1;

            Log.Sync( ref log1, ref log2 );

            // Ensure both logs are the same, and their IDs didn't change.
            Assert.AreEqual( log1, log2 );
            Assert.AreEqual( log1.Id + 1, log2.Id );
            Assert.AreEqual( log2.Id - 1, log1.Id );
        }

        /// <summary>
        /// Ensures the sync works when log1 is older than log2 (log2 should take log 1's place).
        /// </summary>
        public void DoLogSyncBothLog1Older()
        {
            Log log1 = new Log();
            log1.EditTime = DateTime.MinValue;

            Log log2 = log1.Clone();
            log2.Id = log1.Id + 1;
            FillWithData( ref log2 );

            Log expectedLog = log2;

            Log.Sync( ref log1, ref log2 );

            // Ensure both logs are the same, and their IDs didn't change.
            Assert.AreEqual( log1, log2 );
            Assert.AreEqual( log1.Id + 1, log2.Id );
            Assert.AreEqual( log2.Id - 1, log1.Id );

            // Ensure log2 is the one both logs are set to.
            Assert.AreEqual( expectedLog, log2 );
            Assert.AreEqual( expectedLog, log1 );
        }

        /// <summary>
        /// Ensures the sync works when log2 is older than log1 (log1 should take log 2's place).
        /// </summary>
        public void DoLogSyncBothLog2Older()
        {
            Log log1 = new Log();
            log1.EditTime = Log.MaxTime;

            Log log2 = log1.Clone();
            log2.Id = log1.Id + 1;
            FillWithData( ref log2 );

            Log expectedLog = log1;

            Log.Sync( ref log1, ref log2 );

            // Ensure both logs are the same, and their IDs didn't change.
            Assert.AreEqual( log1, log2 );
            Assert.AreEqual( log1.Id + 1, log2.Id );
            Assert.AreEqual( log2.Id - 1, log1.Id );

            // Ensure log2 is the one both logs are set to.
            Assert.AreEqual( expectedLog, log2 );
            Assert.AreEqual( expectedLog, log1 );
        }

        // -------- Test Helpers --------

        /// <summary>
        /// Fills the given log with data.
        /// </summary>
        /// <param name="log">The log to fill.</param>
        private void FillWithData( ref Log log )
        {
            log.Comments = "Some Comment.";
            log.StartTime = DateTime.UtcNow;
            log.EndTime = log.StartTime + new TimeSpan( 1, 0, 0 );
            log.EditTime = DateTime.UtcNow;
            log.Latitude = 1.0M;
            log.Longitude = 2.0M;
            log.Technique = "Some Technique";
        }

        /// <summary>
        /// Ensures the two logs are the same.
        /// </summary>
        /// <param name="log1">The first log to compare.</param>
        /// <param name="log2">The second log to compare.</param>
        private void CheckLogsEqual( Log log1, Log log2 )
        {
            Assert.IsTrue( log1.Equals( log2 ) );
            Assert.IsTrue( log2.Equals( log1 ) );
        }

        /// <summary>
        /// Ensures the two logs are the NOT the same.
        /// </summary>
        /// <param name="log1">The first log to compare.</param>
        /// <param name="log2">The second log to compare.</param>
        private void CheckLogsNotEqual( Log log1, Log log2 )
        {
            Assert.IsFalse( log1.Equals( log2 ) );
            Assert.IsFalse( log2.Equals( log1 ) );
        }

        /// <summary>
        /// Ensures the given log passes validation.
        /// </summary>
        /// <param name="log">The log to check.</param>
        private void ValidationPassedTest( Log log )
        {
            // A default log should not throw.
            Assert.DoesNotThrow(
                delegate ()
                {
                    log.Validate();
                }
            );
        }

        /// <summary>
        /// Ensures the given log fails validation.
        /// </summary>
        /// <param name="log">The log to check.</param>
        /// <param name="errMessage">The error message to see.</param>
        private void ValidationFailedTest( Log log, string errMessage )
        {
            // A default log should not throw.
            Assert.Throws<LogValidationException>(
                delegate ()
                {
                    log.Validate();
                },
                errMessage
            );
        }
    }
}
