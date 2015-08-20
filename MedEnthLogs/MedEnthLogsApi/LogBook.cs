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
            ResetStagged();
        }

        // --------- Properties --------

        /// <summary>
        /// Readonly List of logs.
        /// </summary>
        public IList<Log> Logs { get; private set; }
    
        /// <summary>
        /// The current session being recorded.
        /// Edit this log with information from the user before saving.
        /// This will be the log that gets saved to the database after
        /// a session has taken place.
        /// </summary>
        public Log StaggedLog { get; private set; }

        // -------- Functions --------

        /// <summary>
        /// Ensures the stagged log is okay before being saved to the database.
        /// </summary>
        /// <param name="resultStr">What is wrong with the log.  Empty if no error.</param>
        /// <returns>True if there's an error, else false.</returns>
        public bool ValidateStagged( out string resultStr )
        {
            bool success = true;
            resultStr = string.Empty;

            if ( this.StaggedLog.EndTime < this.StaggedLog.StartTime )
            {
                success = false;
                resultStr = "Log End Time is less than the start time.";
            }

            return success;
        }

        /// <summary>
        /// Resets the stagged log to default values.
        /// </summary>
        public void ResetStagged()
        {
            this.StaggedLog = new Log();
        }

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
