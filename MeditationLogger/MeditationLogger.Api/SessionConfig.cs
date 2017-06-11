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
    public class SessionConfig
    {
        /// <summary>
        /// Constructor.
        /// Sets the properties to the default session settings
        /// </summary>
        public SessionConfig()
        {
            this.Length = null;
            this.AudioFile = string.Empty;
            this.PlayMusic = false;
            this.LoopMusic = false;
        }

        // -------- Properties --------

        /// <summary>
        /// How long the session should last (after this time, the session ends
        /// (time counts down to zero).
        /// Null for unlimited (time counts up from zero).
        /// Ignored if PlayMusic is true and Loop is false (The program will use the audio
        /// file's length instead).
        /// </summary>
        public TimeSpan? Length { get; set; }

        /// <summary>
        /// The audio file to play.  Ignored if PlayMusic is set to false.
        /// </summary>
        public string AudioFile { get; set; }

        /// <summary>
        /// Set to true to play music during the session.
        /// Set to false to not play music during the session.
        /// </summary>
        public bool PlayMusic { get; set; }

        /// <summary>
        /// Set to true to loop the music over and over during the session.
        /// Set to false to play the music once.
        /// Ignored if PlayMusic is set to false.
        /// </summary>
        public bool LoopMusic { get; set; }
    }
}