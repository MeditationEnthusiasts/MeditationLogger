﻿//
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

using System.IO;
using MeditationEnthusiasts.MeditationLogger.Api;
using MeditationEnthusiasts.MeditationLogger.Desktop;
using NUnit.Framework;

namespace MeditationEnthusiasts.MeditationLogger.Tests.Desktop
{
    /// <summary>
    /// This class is platform-specific for Win32 Desktop.
    /// </summary>
    public partial class LogsApiTest
    {
        /// <summary>
        /// Where the TestCore project is located relative to the .dll.
        /// </summary>
        public static readonly string TestCoreDir;

        /// <summary>
        /// The location detector to use.
        /// </summary>
        public static readonly ILocationDetector LocationDetector = new Win32LocationDetector();

        static LogsApiTest()
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