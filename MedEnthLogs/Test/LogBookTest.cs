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
        /// <summary>
        /// Unit under test.
        /// </summary>
        LogBook uut;

        [SetUp]
        public void TestSetup()
        {
            uut = new LogBook();
        }

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
