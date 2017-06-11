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

namespace MeditationEnthusiasts.MeditationLogger.Api
{
    /// <summary>
    /// Interface for finding the location of the session.
    /// Interface since how Win32 does this is different from Win10.
    /// </summary>
    public interface ILocationDetector
    {
        /// <summary>
        /// Latitude Position.
        /// </summary>
        decimal Latitude { get; }

        /// <summary>
        /// Longitude Position.
        /// </summary>
        decimal Longitude { get; }

        /// <summary>
        /// True if we have a valid position, else false.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Refreshes the position information.
        /// </summary>
        /// <returns>True if we got a position.</returns>
        bool RefreshPosition();
    }
}
