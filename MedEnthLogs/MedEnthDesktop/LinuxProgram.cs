using System;
using MedEnthDesktop;
using MedEnthLogsApi;

namespace MedEnthLogsDesktop
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
                new LinuxLocationDetector(),
                new Win32Timer(),
                new LinuxMusicManager(),
                new SQLite.Net.Platform.Generic.SQLitePlatformGeneric()
            );
        }

        /// <summary>
        /// Returns the Music Manager to use for Linux.
        /// </summary>
        /// <returns>The music manager for linux.</returns>
        static IMusicManager GetMusicManager()
        {
            return new LinuxMusicManager();
        }
    }
}

