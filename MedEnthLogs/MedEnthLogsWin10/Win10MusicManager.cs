﻿// 
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedEnthLogsApi;

namespace MedEnthLogsWin10
{
    /// <summary>
    /// Music Manager that uses Windows Universal Platform as the engine.
    /// </summary>
    public class Win10MusicManager : IMusicManager
    {
        // -------- Properties --------

        /// <summary>
        /// List of supported music formats (e.g. .mp3, .wav).
        /// </summary>
        public IReadOnlyList<string> SupportedFormats { get; private set; }

        /// <summary>
        /// True if a file is currently playing.
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// Function that is called when the file
        /// reaches the end.
        /// </summary>
        public Action OnStop { get; set; }

        // -------- Functions --------

        /// <summary>
        /// Gets how long the given audio file
        /// is in length.
        /// Throws FileNotFoundException if the given file doesn't exist.
        /// </summary>
        /// <param name="fileLocation">Where the file is located.</param>
        /// <returns>A TimeSpan that contains the length of the audio file.</returns>
        public TimeSpan GetLengthOfFile( string fileLocation )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Plays the given audio file.
        /// Validate() is called first
        /// Throws InvalidOperationException if a file is already being played.
        /// Throws FileNotFoundException if the given file doesn't exist.
        /// </summary>
        /// <param name="audioFile">The audio file to play.</param>
        public void Play( string audioFile )
        {

        }

        /// <summary>
        /// Stops playing the audio file.
        /// No-op if nothing is playing.
        /// </summary>
        public void Stop()
        {

        }

        /// <summary>
        /// Ensures everything is okay with the given audio file.
        /// Throws PlatformNotSupportedException if the file type is not supported
        /// Throws FileNotFoundException if the given audio file does not exist.
        /// </summary>
        /// <param name="audioFile">The audio file to validate.</param>
        public void Validate( string audioFile )
        {

        }
    }
}
