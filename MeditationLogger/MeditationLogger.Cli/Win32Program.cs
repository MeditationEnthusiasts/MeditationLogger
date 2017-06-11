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

using MeditationEnthusiasts.MeditationLogger.Api;
using MeditationEnthusiasts.MeditationLogger.Desktop;

namespace MeditationEnthusiasts.MeditationLogger.Cli
{
    static partial class Program
    {
        /// <summary>
        /// Gets a linux API.
        /// </summary>
        /// <returns>The Linux API.</returns>
        private static Api.Api GetApi()
        {
            return new Api.Api(
                new Win32LocationDetector(),
                new Win32Timer(),
                new NAudioMusicManager()
            );
        }

        /// <summary>
        /// Returns the Music Manager to use for Linux.
        /// </summary>
        /// <returns>The music manager for linux.</returns>
        private static IMusicManager GetMusicManager()
        {
            return new NAudioMusicManager();
        }
    }
}