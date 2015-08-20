using System;
using MedEnthLogsApi;
using NUnit.Framework;

namespace TestCommon
{
    /// <summary>
    /// Tests the Log Class.
    /// </summary>
    [TestFixture]
    public class LogTest
    {
        // -------- Fields --------

        /// <summary>
        /// Unit under test.
        /// </summary>
        Log uut;

        // -------- Setup/Teardown --------

        [SetUp]
        public void TestSetup()
        {
            uut = new Log();
        }

        // -------- Test Functions --------

        /// <summary>
        /// Ensures that the duration property is the end time minus the start time.
        /// </summary>
        [Test]
        public void TestDuration()
        {
            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MaxValue;

            uut.StartTime = start;
            uut.EndTime = end;

            TimeSpan expected = end - start;

            Assert.AreEqual( expected, uut.Duration );
        }

        /// <summary>
        /// Tests the GetHashCode function.
        /// </summary>
        [Test]
        public void TestGetHashCode()
        {
            // Ensures that the hash code of the log is the same as the hashcode
            // of the creation time.  Two logs in a logbook can not have the same creation
            // time.
            Assert.AreEqual( uut.CreateTime.GetHashCode(), uut.GetHashCode() );
        }

        /// <summary>
        /// Tests the Log's equal function, operator==, and operator!=
        /// </summary>
        [Test]
        public void EqualsTest()
        {
            Log other = new Log();

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

            // Change Create Time.
            other.CreateTime = DateTime.Now;
            CheckLogsNotEqual( uut, other );
            other.CreateTime = uut.CreateTime;

            // Change Edit Time.
            other.EditTime = DateTime.Now;
            CheckLogsNotEqual( uut, other );
            other.EditTime = uut.EditTime;

            // Change Comments.
            other.Comments = "Hello world!";
            CheckLogsNotEqual( uut, other );
            other.Comments = uut.Comments;

            // Change Location
            other.Location = "My Room";
            CheckLogsNotEqual( uut, other );
            other.Location = uut.Location;

            // Check passing in nulls.
            Assert.IsFalse( uut.Equals( null ) );
        }

        // -------- Test Helpers --------

        /// <summary>
        /// Ensures the two logs are the same.
        /// </summary>
        /// <param name="log1">The first log to compare.</param>
        /// <param name="log2">The second log to compare.</param>
        public void CheckLogsEqual( Log log1, Log log2 )
        {
            Assert.IsTrue( log1.Equals( log2 ) );
            Assert.IsTrue( log2.Equals( log1 ) );
        }

        /// <summary>
        /// Ensures the two logs are the NOT the same.
        /// </summary>
        /// <param name="log1">The first log to compare.</param>
        /// <param name="log2">The second log to compare.</param>
        public void CheckLogsNotEqual( Log log1, Log log2 )
        {
            Assert.IsFalse( log1.Equals( log2 ) );
            Assert.IsFalse( log2.Equals( log1 ) );
        }
    }
}
