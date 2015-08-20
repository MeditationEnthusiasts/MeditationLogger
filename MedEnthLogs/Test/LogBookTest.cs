using System;
using MedEnthLogsApi;
using NUnit.Framework;

namespace TestCommon
{
    /// <summary>
    /// Tests the Log Class.
    /// </summary>
    [TestFixture]
    public class LogBookTest
    {
        // -------- Fields --------

        /// <summary>
        /// Unit under test.
        /// </summary>
        LogBook uut;

        // -------- Setup/Teardown --------

        [SetUp]
        public void TestSetup()
        {
            uut = new LogBook();
        }

        // -------- Tests --------

        /// <summary>
        /// Ensures the log list is readonly.
        /// </summary>
        [Test]
        public void ReadonlyListTest()
        {
            // Expect Exception.
            Assert.Catch<NotSupportedException>(
                delegate()
                {
                    uut.Logs.Add( new Log() );
                }
            );
        }
    }
}
