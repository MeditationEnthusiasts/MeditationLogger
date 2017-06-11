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

using MeditationEnthuisasts.MeditationLogger.TestCore;
using NUnit.Framework;

namespace MeditationEnthusiasts.MeditationLogger.Tests.Desktop
{
    /// <summary>
    /// Tests the Log Class.
    /// </summary>
    [TestFixture]
    public class LogTest
    {
        // -------- Fields --------

        private LogTestCore testCore;

        // -------- Setup/Teardown --------

        [SetUp]
        public void TestSetup()
        {
            testCore = new LogTestCore();
        }

        // -------- Test Functions --------

        /// <summary>
        /// Ensures that the duration property is the end time minus the start time.
        /// </summary>
        [Test]
        public void TestDuration()
        {
            this.testCore.DoTestDuration();
        }

        /// <summary>
        /// Tests the GetHashCode function.
        /// </summary>
        [Test]
        public void TestGetHashCode()
        {
            this.testCore.DoTestGetHashCode();
        }

        /// <summary>
        /// Ensures we can't set the comments to null,
        /// and what we set is what we get.
        /// </summary>
        [Test]
        public void CommentsTest()
        {
            this.testCore.DoCommentsTest();
        }

        /// <summary>
        /// Ensures we can't set the technique to null,
        /// and what we set is what we get.
        /// </summary>
        [Test]
        public void TechniqueTest()
        {
            this.testCore.DoTechniqueTest();
        }

        /// <summary>
        /// Tests the Log's equal function, operator==, and operator!=
        /// </summary>
        [Test]
        public void EqualsTest()
        {
            this.testCore.DoEqualsTest();
        }

        /// <summary>
        /// Ensures the clone method creates a new instance,
        /// but all properties match.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            this.testCore.DoCloneTest();
        }

        [Test]
        public void ValidationPassTest()
        {
            this.testCore.DoValidationPassTest();
        }

        [Test]
        public void ValidationFailTest()
        {
            this.testCore.DoValidationFailTest();
        }

        // ---- Sync Tests ----

        /// <summary>
        /// Ensures the santity checks of Log.Sync work correctly.
        /// </summary>
        [Test]
        public void LogSyncExceptionCheckTest()
        {
            this.testCore.DoLogSyncExceptionCheckTest();
        }

        /// <summary>
        /// Ensures all is well if both logs are equal.
        /// </summary>
        [Test]
        public void LogSyncBothEqual()
        {
            this.testCore.DoLogSyncBothEqual();
        }

        /// <summary>
        /// Ensures the sync works when log1 is older than log2 (log2 should take log 1's place).
        /// </summary>
        [Test]
        public void LogSyncBothLog1Older()
        {
            this.testCore.DoLogSyncBothLog1Older();
        }

        /// <summary>
        /// Ensures the sync works when log2 is older than log1 (log1 should take log 2's place).
        /// </summary>
        [Test]
        public void LogSyncBothLog2Older()
        {
            this.testCore.DoLogSyncBothLog2Older();
        }
    }
}