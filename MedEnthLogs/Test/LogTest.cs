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
        /// Ensures we can't set the comments to null,
        /// and what we set is what we get.
        /// </summary>
        [Test]
        public void CommentsTest()
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
        /// Ensures we can't set the location to null,
        /// and what we set is what we get.
        /// </summary>
        [Test]
        public void LocationTest()
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

            // Change Technique
            other.Technique = "My Room";
            CheckLogsNotEqual( uut, other );
            other.Technique = uut.Technique;

            // Check passing in nulls.
            Assert.IsFalse( uut.Equals( null ) );
        }

        /// <summary>
        /// Ensures the clone method creates a new instance,
        /// but all properties match.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            Log clone = uut.Clone();
            Assert.AreNotSame( clone, uut );
            Assert.AreEqual( clone, uut );
        }

        // -------- Test Helpers --------

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
    }
}
