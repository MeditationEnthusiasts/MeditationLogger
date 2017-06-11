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
using System.Collections.Generic;
using MeditationEnthusiasts.MeditationLogger.Api;

namespace TestCore.Mocks
{
    public class MockMusicManager : IMusicManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MockMusicManager()
        {
            this.IsPlaying = false;
            this.OnStop = null;
            this.GetLengthOfFileReturn = new TimeSpan( 0, 0, 0, 0, 0 );
            this.ThrownFromValidate = null;
        }

        // -------- Properties --------

        /// <summary>
        /// List of supported music formats (e.g. .mp3, .wav).
        /// </summary>
        public IReadOnlyList<string> SupportedFormats { get; set; }

        /// <summary>
        /// True if a file is currently playing.
        /// </summary>
        public bool IsPlaying { get; set; }

        /// <summary>
        /// Function that is called when the file
        /// reaches the end.
        /// </summary>
        public Action OnStop { get; set; }

        /// <summary>
        /// What is returned from GetLengthOfFile
        /// </summary>
        public TimeSpan GetLengthOfFileReturn { get; set; }

        /// <summary>
        /// This Exception is thrown when Validate is called.
        /// Leave null to not throw exceptions.
        /// </summary>
        public Exception ThrownFromValidate { get; set; }

        // -------- Functions --------

        /// <summary>
        /// Returns GetLengthOfFileReturn.
        /// </summary>
        /// <param name="fileLocation">The file location (not used in mock).</param>
        /// <returns>this.GetLengthOfFileReturn</returns>
        public TimeSpan GetLengthOfFile( string fileLocation )
        {
            return this.GetLengthOfFileReturn;
        }

        /// <summary>
        /// Plays the given audio file.
        /// Validate() is called first
        /// </summary>
        /// <param name="audioFile">The audio file to play (Not used in mock).</param>
        public void Play( string audioFile )
        {
            Validate( audioFile );
            this.IsPlaying = true;
        }

        /// <summary>
        /// Stops playing the audio file.
        /// No-op if nothing is playing.
        /// </summary>
        public void Stop()
        {
            this.IsPlaying = false;
        }

        /// <summary>
        /// Throws ThrownFromValidate, unless its set to null.
        /// </summary>
        /// <param name="audioFile">The audio file to validate (Not used in mock).</param>
        public void Validate( string audioFile )
        {
            if ( this.ThrownFromValidate != null )
            {
                throw this.ThrownFromValidate;
            }
        }
    }
}
