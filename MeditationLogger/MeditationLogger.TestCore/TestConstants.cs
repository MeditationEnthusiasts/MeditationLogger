//
// Meditation Logger.
// Copyright (C) 2017  Seth Hendrick.
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

using System.IO;
using NUnit.Framework;

namespace MeditationEnthuisasts.MeditationLogger.TestCore
{
    public static class TestConstants
    {
        // ---------------- Fields ----------------

        /// <summary>
        /// Where the TestCore project is located relative to the .dll.
        /// </summary>
        public static readonly string TestCoreDir;

        // ---------------- Constructor ----------------

        /// <summary>
        /// Constructor.
        /// </summary>
        static TestConstants()
        {
            TestCoreDir = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "..",
                "..",
                "..",
                "MeditationLogger.TestCore"
            );
        }
    }
}
