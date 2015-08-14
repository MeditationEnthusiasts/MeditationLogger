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
        /// <summary>
        /// Unit under test.
        /// </summary>
        Log uut;

        [SetUp]
        public void TestSetup()
        {
            uut = new Log();
        }

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
    }
}
