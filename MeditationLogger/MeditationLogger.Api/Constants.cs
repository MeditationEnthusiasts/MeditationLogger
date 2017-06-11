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

using System;
using System.IO;

namespace MeditationEnthusiasts.MeditationLogger.Api
{
    /// <summary>
    /// Constants for the Desktop application.
    /// </summary>
    public static class Constants
    {
        // -------- Fields ---------
        /// <summary>
        /// Location of the database.
        /// </summary>
        public static readonly string DatabaseFolderLocation;

        // -------- Constructor --------

        /// <summary>
        /// Static Constructor.
        /// </summary>
        static Constants()
        {
            string dbLocation = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
            dbLocation = Path.Combine( dbLocation, "MeditationLoggerDesktop" );

            DatabaseFolderLocation = dbLocation;
        }
    }
}