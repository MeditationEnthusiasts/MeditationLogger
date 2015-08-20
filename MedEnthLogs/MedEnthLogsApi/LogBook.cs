using System;
using System.Collections.Generic;

namespace MedEnthLogsApi
{
    /// <summary>
    /// This class represents a book of logs.
    /// </summary>
    public class LogBook
    {
        // -------- Fields --------

        /// <summary>
        /// List of logs.
        /// </summary>
        List<Log> logs;

        // -------- Constructor --------

        /// <summary>
        /// Constructor.
        /// Creates an empty log list.
        /// </summary>
        public LogBook()
        {
            this.logs = new List<Log>();
            this.Logs = this.logs.AsReadOnly();
        }

        // --------- Properties --------

        /// <summary>
        /// Readonly List of logs.
        /// </summary>
        public IList<Log> Logs { get; private set; }

        // -------- Functions --------

        /// <summary>
        /// Loads the logs from the specified file location.
        /// </summary>
        /// <param name="logLocation">
        /// The location of the logs.
        /// The file name extension of this determines how the logs are loaded.
        /// </param>
        public void LoadLogs( string fileLocation )
        {
            throw new NotImplementedException( "LoadLogs not implemented yet." );
        }
    }
}
