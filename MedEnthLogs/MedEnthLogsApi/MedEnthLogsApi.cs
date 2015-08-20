using System;
using System.Collections.Generic;
using System.Text;
using SQLite.Net;
using SQLite.Net.Interop;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo( "Test" )]
namespace MedEnthLogsApi
{
    /// <summary>
    /// Exception thrown when a log fails validation.
    /// </summary>
    public class LogValidationException : Exception
    {
        public LogValidationException( string message ) :
            base( message )
        {

        }
    }

    public class MedEnthLogsApi
    {
        // -------- Fields --------

        /// <summary>
        /// Error message that appears in save if the database is not open.
        /// internal for unit tests.
        /// </summary>
        internal const string DatabaseNotOpenMessage = "Database not open, can not perform operation.";

        /// <summary>
        /// Error message that appears if save is called, but there's a session currently in progress.
        /// internal for unit tests.
        /// </summary>
        internal const string SessionInProgressMessage = "Can not save a session that is currently in progress";

        /// <summary>
        /// Error message that appears
        /// in validate if the end time is less than that start time.
        /// internal for unit tests.
        /// </summary>
        internal const string EndTimeLessThanStartTimeMessage = "Log End Time is less than the start time.";

        /// <summary>
        /// Error message that appears
        /// in validate if the edit time is less than the creation time.
        /// internal for unit tests.
        /// </summary>
        internal const string EditTimeLessThanCreationTimeMessage = "Log edit Time is less than the creation time.";

        /// <summary>
        /// Reference to a SQLite connection.
        /// </summary>
        private SQLiteConnection sqlite;

        /// <summary>
        /// The current log being written to.
        /// internal for unit tests only.
        /// </summary>
        internal Log currentLog;

        // -------- Constructor --------

        /// <summary>
        /// Constructor
        /// </summary>
        public MedEnthLogsApi()
        {
            this.sqlite = null;
            this.LogBook = null;
            ResetCurrentLog();
        }

        // -------- Properties --------

        /// <summary>
        /// Reference to the Logbook used.
        /// </summary>
        public LogBook LogBook { get; private set; }

        /// <summary>
        /// Whether or not a session is in Progress.
        /// That is, start has been called, but stop has not.
        /// </summary>
        public bool IsSessionInProgress { get; private set; }

        /// <summary>
        /// The current log being written to.
        /// Readonly property.
        /// </summary>
        public ILog CurrentLog
        {
            get
            {
                return this.currentLog;
            }
        }

        // -------- Functions --------

        /// <summary>
        /// Opens the given SQLite database, and populates
        /// the Logbook.
        /// </summary>
        /// <param name="platform">The sqlite platform we are using.</param>
        /// <param name="path">The path to the sqlite database.</param>
        /// <remarks>If a databse is already open, it will be closed first.</remarks>
        public void Open( ISQLitePlatform platform, string path )
        {
            if ( this.sqlite != null )
            {
                this.Close();
            }

            this.sqlite = new SQLiteConnection( platform, path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite );
        }

        /// <summary>
        /// Resets the current log to default values.
        /// </summary>
        public void ResetCurrentLog()
        {
            this.currentLog = new Log();
        }

        /// <summary>
        /// Ensures the current log is okay before being saved to the database.
        /// Throws LogValidationException if its not.
        /// </summary>
        internal void ValidateCurrentLog()
        {
            if ( this.currentLog.EndTime < this.currentLog.StartTime )
            {
                throw new LogValidationException( EndTimeLessThanStartTimeMessage );
            }
            else if ( this.currentLog.EditTime < this.currentLog.CreateTime )
            {
                throw new LogValidationException( EditTimeLessThanCreationTimeMessage );
            }
        }

        /// <summary>
        /// Starts the session.
        /// This creates a log (and by extension, sets the creation time to now),
        /// and sets the start time to now as well.
        /// No-op if session in progress.
        /// </summary>
        public void StartSession()
        {
            if ( this.IsSessionInProgress == false )
            {
                this.currentLog = new Log();
                this.currentLog.CreateTime = DateTime.Now.ToUniversalTime();
                this.currentLog.StartTime = this.currentLog.CreateTime;
                this.currentLog.EditTime = this.currentLog.CreateTime;
                this.IsSessionInProgress = true;
            }
        }

        /// <summary>
        /// Stops the session.
        /// No-op if not started.
        /// </summary>
        public void StopSession()
        {
            if ( this.IsSessionInProgress == true )
            {
                this.currentLog.EndTime = DateTime.Now.ToUniversalTime();
                this.currentLog.EditTime = currentLog.EndTime;
                this.IsSessionInProgress = false;
            }
        }

        /// <summary>
        /// Validates and saves the CurrentSession to the database.
        /// 
        /// Throws InvalidOperationException if a session is in progress or database is not opened.
        /// 
        /// Throws LogValidationException if the CurrentLog is not valid.
        /// </summary>
        /// <param name="location">The location information for the session, if any (null for no location).</param>
        /// <param name="comments">The comments for the session, if any (null for no comments).</param>
        public void ValidateAndSaveSession( string location = null, string comments = null )
        {
            // If sqlite is not open, throw exeption.
            if ( this.sqlite == null )
            {
                throw new InvalidOperationException(
                    DatabaseNotOpenMessage
                );
            }
            // If a session is in progress, throw.
            else if ( IsSessionInProgress )
            {
                throw new InvalidOperationException(
                    SessionInProgressMessage
                );
            }
            // If validating the log fails, throw.
            this.ValidateCurrentLog();

            // Otherwise, Edit the log one last time and save it to the database.
            if ( location != null )
            {
                this.currentLog.Location = location;
                this.currentLog.EditTime = DateTime.Now.ToUniversalTime();
            }

            if ( comments != null )
            {
                this.currentLog.Comments = comments;
                this.currentLog.EditTime = DateTime.Now.ToUniversalTime();
            }

            this.sqlite.Insert( this.currentLog );
            this.sqlite.Commit();
        }

        /// <summary>
        /// Reads the database and populates the logbook.
        /// </summary>
        public void PopulateLogbook()
        {
            // If sqlite is not open, throw exeption.
            if ( this.sqlite == null )
            {
                throw new InvalidOperationException(
                    DatabaseNotOpenMessage
                );
            }
        }

        /// <summary>
        /// Closes the sqlite connection, and clears the logbook.
        /// </summary>
        public void Close()
        {
            if ( this.sqlite != null )
            {
                this.sqlite.Close();
                this.sqlite = null;
                this.LogBook = null;
            }
        }
    }
}
