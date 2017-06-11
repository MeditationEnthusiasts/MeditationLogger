// 
// Meditation Logger.
// Copyright (C) 2015-2017  Seth Hendrick.
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

using NUnit.Framework;
using MeditationLogger.TestCore;

namespace MeditationEnthusiasts.MeditationLogger.Tests.Desktop
{
    /// <summary>
    /// Tests the Log Class.
    /// </summary>
    [TestFixture]
    public class LogBookTest
    {
        // -------- Fields --------

        private LogBookTestCore testCore;

        // -------- Setup / Teardown --------

        [SetUp]
        public void TestSetup()
        {
            this.testCore = new LogBookTestCore();
        }

        // -------- Tests --------

        /// <summary>
        /// Ensures the log list is readonly.
        /// </summary>
        [Test]
        public void ReadonlyListTest()
        {
            this.testCore.DoReadonlyListTest();
        }

        /// <summary>
        /// Ensures things get added in the correct order.
        /// Index 0 should be the newest log.
        /// </summary>
        [Test]
        public void OrderTest()
        {
            this.testCore.DoOrderTest();
        }

        /// <summary>
        /// Ensures a conflicting guid results in the
        /// last one in the list gets saved.
        /// </summary>
        [Test]
        public void ConflictingGuidTest()
        {
            this.testCore.DoConflictingGuidTest();
        }

        /// <summary>
        /// Ensures the LogExists function works as expected.
        /// </summary>
        [Test]
        public void LogExistsTest()
        {
            this.testCore.DoLogExistsTest();
        }

        /// <summary>
        /// Ensures the GetLog function works as expected.
        /// </summary>
        [Test]
        public void GetLogTest()
        {
            this.testCore.DoGetLogTest();
        }
    }
}
