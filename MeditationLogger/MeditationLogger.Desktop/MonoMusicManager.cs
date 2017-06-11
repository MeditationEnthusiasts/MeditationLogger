using System;
using MedEnthLogsApi;

namespace MedEnthDesktop
{
    public class MonoMusicManager : IMusicManager
    {
        // -------- Fields --------

        /// <summary>
        /// List of supported formats.
        /// </summary>
        private readonly List<string> supportedFormats = new List<string> { ".mp3", ".wav" };

        // -------- Constructor --------
        public MonoMusicManager ()
        {
            this.SupportedFormats = supportedFormats.AsReadOnly();
            this.IsPlaying = false;
            this.OnStop = null;
        }

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
            return new TimeSpan( 0, 1, 0 );
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

