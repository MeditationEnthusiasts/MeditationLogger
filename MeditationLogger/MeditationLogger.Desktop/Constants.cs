using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeditationEnthusiasts.MeditationLogger.Desktop
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
