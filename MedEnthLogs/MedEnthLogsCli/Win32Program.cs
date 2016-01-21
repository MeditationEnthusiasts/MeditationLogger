// 
// Meditation Logger.
// Copyright (C) 2015-2016  Seth Hendrick.
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
using MedEnthDesktop;
using MedEnthLogsApi;
using MedEnthLogsDesktop;

namespace MedEnthLogsCli
{
    static partial class Program
    {
        /// <summary>
        /// Gets a linux API.
        /// </summary>
        /// <returns>The Linux API.</returns>
        static Api GetApi()
        {
            return new MedEnthLogsApi.Api(
                new Win32LocationDetector(),
                new Win32Timer(),
                new NAudioMusicManager(),
                new SQLite.Net.Platform.Win32.SQLitePlatformWin32()
            );
        }

        /// <summary>
        /// Returns the Music Manager to use for Linux.
        /// </summary>
        /// <returns>The music manager for linux.</returns>
        static IMusicManager GetMusicManager()
        {
            return new NAudioMusicManager();
        }
    }
}

