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

namespace MeditationEnthusiasts.MeditationLogger.Api
{
    /// <summary>
    /// Interface to a log object.
    /// </summary>
    public interface ILog
    {
        // -------- Properties --------

        /// <summary>
        /// ID of the log.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// When the session starts
        /// (UTC, the UI must convert it to local time).
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// When the session ends
        /// (UTC, the UI must convert it to local time).
        /// </summary>
        DateTime EndTime { get; }

        /// <summary>
        /// Unique GUID for the log objet.
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// The last time this log was edited.
        /// (UTC, the UI must convert it to local time).
        /// </summary>
        DateTime EditTime { get; }

        /// <summary>
        /// How long the session lasted.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The Technique Used.
        /// specifies.
        /// </summary>
        string Technique { get; }

        /// <summary>
        /// The comments the user wrote about the session.
        /// </summary>
        string Comments { get; }

        /// <summary>
        /// The latitude of where the session took place.
        /// null if no location specified.
        /// </summary>
        decimal? Latitude { get; set; }

        /// <summary>
        /// The longitude of where the session took place.
        /// null if no location specified.
        /// </summary>
        decimal? Longitude { get; set; }

        // -------- Functions --------

        /// <summary>
        /// Ensures the log is in a good state.  Should be called before saving it.
        /// Throws Exceptions if not.
        /// </summary>
        void Validate();
    }
}