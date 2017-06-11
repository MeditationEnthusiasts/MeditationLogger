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
using System.IO;
using System.Runtime.CompilerServices;
using SethCS.Basic;
using SQLite.Net;
using SQLite.Net.Interop;

[assembly: InternalsVisibleTo( "MeditationLogger.Tests.Desktop" )]
[assembly: InternalsVisibleTo( "MeditationLogger.TestCore.Desktop" )]

namespace MeditationEnthusiasts.MeditationLogger.Api
{
    /// <summary>
    /// The API for talking to the backend of the application.
    /// </summary>
    public class Api
    {
        /// <summary>
        /// The current state of the API.  Whether a session is in progress or not.
        /// </summary>
        public enum ApiState
        {
            /// <summary>
            /// No session is happening.  This can happen when the app first starts up or
            /// a user just saved a session.
            /// </summary>
            Idle,

            /// <summary>
            /// The session has started.
            /// </summary>
            Started,

            /// <summary>
            /// The session has ended, but has not been saved yet.
            /// </summary>
            Stopped
        }

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
        /// Error message that appears when the logbook is null when it shouldn't be.
        /// </summary>
        internal const string nullLogbook = "Logbook still null, try populating first before calling this function.";

        /// <summary>
        /// Reference to a SQLite connection.
        /// </summary>
        private SQLiteConnection sqlite;

        /// <summary>
        /// The Audio engine to use.
        /// </summary>
        private IMusicManager musicManager;

        /// <summary>
        /// The current log being written to.
        /// internal for unit tests only.
        /// </summary>
        internal Log currentLog;

        /// <summary>
        /// The platform that is being used.
        /// </summary>
        private ISQLitePlatform platform;

        /// <summary>
        /// The version of the API.
        /// </summary>
        public const string VersionString = "0.3.0";

        /// <summary>
        /// Sematic Version Class of the version string.
        /// </summary>
        public static readonly SemanticVersion Version = SemanticVersion.Parse( VersionString );

        /// <summary>
        /// The name of the logbook located in AppData.
        /// </summary>
        public const string LogbookFileName = "logbook.mlg";

        // -------- Constructor --------

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="locationDetector">Location to the location detector.</param>
        /// <param name="timer">The timing engine to use.</param>
        /// <param name="musicManager">The music manager to use.</param>
        /// <param name="platform">The sqlite platform we are using.</param>
        public Api( ILocationDetector locationDetector, ITimer timer, IMusicManager musicManager )
        {
            if( Environment.OSVersion.Platform.Equals( PlatformID.Win32NT ) )
            {
                this.platform = new SQLite.Net.Platform.Win32.SQLitePlatformWin32();
            }
            else
            {
                // Requires the SQLite.so (shared object) files to be installed.
                this.platform = new SQLite.Net.Platform.Generic.SQLitePlatformGeneric();
            }

            this.sqlite = null;
            this.LogBook = null;
            this.LocationDetector = locationDetector;
            this.timer = timer;
            this.musicManager = musicManager;
            ResetStates();
        }

        // -------- Properties --------

        /// <summary>
        /// The current state of the API.  Whether a session is currently in progress or not.
        /// </summary>
        public ApiState CurrentState { get; private set; }

        /// <summary>
        /// Reference to the Logbook used.
        /// </summary>
        public LogBook LogBook { get; private set; }

        /// <summary>
        /// Class that detects the location.
        /// </summary>
        public ILocationDetector LocationDetector { get; private set; }

        /// <summary>
        /// The timer engine to use.
        /// </summary>
        public ITimer timer { get; private set; }

        /// <summary>
        /// Whether or not the sqlite connection is open.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                // If sqlite is not null, then we are open.
                return this.sqlite != null;
            }
        }

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
        /// <param name="path">The path to the sqlite database.</param>
        /// <remarks>If a databse is already open, it will be closed first.</remarks>
        public void Open( string path )
        {
            if( this.sqlite != null )
            {
                this.Close();
            }

            this.sqlite = new SQLiteConnection( this.platform, path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite );
            this.sqlite.CreateTable<Log>();
            this.sqlite.Commit();
        }

        /// <summary>
        /// Resets this class to the idle state.
        /// </summary>
        public void ResetStates()
        {
            this.currentLog = new Log();
            this.CurrentState = ApiState.Idle;
        }

        /// <summary>
        /// Inserts the given log to the database.
        /// This will not show up in the logbook, call
        /// PopulateLogbook() to do that.
        ///
        /// If the log's GUID already exists in the logbook,
        /// it will be overwritten.
        /// </summary>
        /// <param name="log">The log to add.</param>
        public void InsertLog( Log log )
        {
            if( log == null )
            {
                throw new ArgumentNullException(
                    nameof( log )
                );
            }
            // If sqlite is not open, throw exeption.
            else if( this.sqlite == null )
            {
                throw new InvalidOperationException(
                    DatabaseNotOpenMessage
                );
            }
            else if( this.LogBook == null )
            {
                throw new InvalidOperationException(
                    nullLogbook
                );
            }

            if( this.LogBook.LogExists( log.Guid ) )
            {
                log.Id = this.LogBook.GetLog( log.Guid ).Id;
                this.sqlite.InsertOrReplace( log );
            }
            else
            {
                this.sqlite.Insert( log );
            }
            this.sqlite.Commit();
        }

        /// <summary>
        /// Starts the session.
        /// This creates a log (and by extension, sets the edit time to now),
        /// and sets the start time to now as well.
        /// No-op if session in progress.
        /// </summary>
        /// <param name="config">The config for the session.</param>
        public void StartSession( SessionConfig config )
        {
            if( config == null )
            {
                throw new ArgumentNullException( "config" );
            }

            try
            {
                if( this.CurrentState == ApiState.Idle )
                {
                    this.currentLog = new Log();
                    this.currentLog.StartTime = DateTime.UtcNow;
                    this.currentLog.EditTime = this.currentLog.StartTime;

                    TimeSpan? length = config.Length;

                    if( config.PlayMusic )
                    {
                        this.musicManager.Validate( config.AudioFile );
                        if( config.LoopMusic )
                        {
                            this.musicManager.OnStop =
                                delegate ()
                                {
                                    // If our state is still in the started state, and we want
                                    // to loop the music, stop the current music in progress, then
                                    // restart it.
                                    if( this.CurrentState == ApiState.Started )
                                    {
                                        this.musicManager.Stop();
                                        this.musicManager.Play( config.AudioFile );
                                    }
                                };
                        }
                        else
                        {
                            TimeSpan audioLength = this.musicManager.GetLengthOfFile( config.AudioFile ) + new TimeSpan( 0, 0, 2 );
                            length = new TimeSpan( audioLength.Hours, audioLength.Minutes, audioLength.Seconds ); // Truncate milliseconds.
                        }
                        this.musicManager.Play( config.AudioFile );
                    }
                    this.timer.StartTimer( length );

                    this.CurrentState = ApiState.Started;
                }
            }
            catch( Exception )
            {
                this.currentLog = new Log();
                throw;
            }
        }

        /// <summary>
        /// Stops the session.
        /// No-op if current state is not started.
        /// </summary>
        public void StopSession()
        {
            if( this.CurrentState == ApiState.Started )
            {
                this.timer.StopAndResetTimer();
                this.currentLog.EndTime = DateTime.UtcNow;
                this.currentLog.EditTime = currentLog.EndTime;
                this.musicManager.Stop();
                this.CurrentState = ApiState.Stopped;
            }
        }

        /// <summary>
        /// Validates and saves the CurrentSession to the database.
        ///
        /// Throws InvalidOperationException if the current state is not "stopped" or database is not opened.
        ///
        /// Throws LogValidationException if the CurrentLog is not valid.
        ///
        /// This does NOT reset the current log.  Call ResetCurrentLog() for that.
        ///
        /// If either latitude or longitude is null, but the other is not, a LogValidationException will be thrown.
        /// </summary>
        /// <param name="technique">The technique information for the session, if any (null for no technique).</param>
        /// <param name="comments">The comments for the session, if any (null for no comments).</param>
        /// <param name="latitude">The latitude location of the session.  Null for no location.</param>
        /// <param name="longitude">The longitude location of the session.  Null for no location.</param>
        public void ValidateAndSaveSession( string technique = null, string comments = null, decimal? latitude = null, decimal? longitude = null )
        {
            // If sqlite is not open, throw exeption.
            if( this.sqlite == null )
            {
                throw new InvalidOperationException(
                    DatabaseNotOpenMessage
                );
            }
            // If a session is in progress, throw.
            else if( this.CurrentState != ApiState.Stopped )
            {
                throw new InvalidOperationException(
                    SessionInProgressMessage
                );
            }

            this.currentLog.Latitude = latitude;
            this.currentLog.Longitude = longitude;

            // If validating the log fails, throw.
            this.CurrentLog.Validate();

            // Otherwise, Edit the log one last time and save it to the database.
            if( technique != null )
            {
                this.currentLog.Technique = technique;
                this.currentLog.EditTime = DateTime.UtcNow;
            }

            if( comments != null )
            {
                this.currentLog.Comments = comments;
                this.currentLog.EditTime = DateTime.UtcNow;
            }

            this.sqlite.Insert( this.currentLog );
            this.sqlite.Commit();

            this.CurrentState = ApiState.Idle;
        }

        /// <summary>
        /// Reads the database and populates the logbook.
        /// </summary>
        public void PopulateLogbook()
        {
            // If sqlite is not open, throw exeption.
            if( this.sqlite == null )
            {
                throw new InvalidOperationException(
                    DatabaseNotOpenMessage
                );
            }

            this.LogBook = LogBook.FromSqlite( this.sqlite );
        }

        // ---- Import Functions ----

        /// <summary>
        /// Performs an Import.
        /// The type of Import (XML, JSON, MLG) depends on the
        /// file extension (case doesn't matter).
        ///
        /// This does not repopulate the logbook automatically,
        /// call "PopulateLogbook" after this function to do that.
        /// </summary>
        /// <param name="fileName">Where to import the file to.</param>
        /// <param name="onStep">
        /// Action to take on each step during the process. Parameter 1 is the current step
        /// we are on.  Parameter 2 is the total number of steps the function will take.
        /// Null for no-op.
        /// </param>
        public void Import( string fileName, Action<int, int> onStep = null )
        {
            string[] splitString = fileName.Split( '.' );
            switch( splitString[splitString.Length - 1].ToLower() )
            {
                case "xml":
                    using( FileStream outFile = new FileStream( fileName, FileMode.Open, FileAccess.Read ) )
                    {
                        XmlExporter.ImportFromXml( outFile, this.LogBook, this.sqlite, onStep );
                    }
                    break;

                case "json":
                    using( FileStream outFile = new FileStream( fileName, FileMode.Open, FileAccess.Read ) )
                    {
                        JsonExporter.ImportFromJson( outFile, this.LogBook, this.sqlite, onStep );
                    }
                    break;

                case "mlg":
                    MlgExporter.ImportMlg( fileName, this.LogBook, this.platform, this.sqlite, onStep );
                    break;

                default:
                    throw new ArgumentException(
                        "Invalid filename passed into Import, can only be .xml, .json, .mlg.",
                        "fileName"
                    );
            }
        }

        // ---- Export Functions ----

        /// <summary>
        /// Performs an Export.
        /// The type of export (XML, JSON, MLG) depends on the
        /// file extension (case doesn't matter).
        /// </summary>
        /// <param name="fileName">Where to export the file to.</param>
        /// <param name="onStep">
        /// Action to take on each step during the process. Parameter 1 is the current step
        /// we are on.  Parameter 2 is the total number of steps the function will take.
        /// Null for no-op.
        /// </param>
        public void Export( string fileName, Action<int, int> onStep = null )
        {
            string[] splitString = fileName.Split( '.' );
            switch( splitString[splitString.Length - 1].ToLower() )
            {
                case "xml":
                    using( FileStream outFile = new FileStream( fileName, FileMode.Create, FileAccess.Write ) )
                    {
                        XmlExporter.ExportToXml( outFile, this.LogBook, onStep );
                    }
                    break;

                case "json":
                    using( FileStream outFile = new FileStream( fileName, FileMode.Create, FileAccess.Write ) )
                    {
                        JsonExporter.ExportJson( outFile, this.LogBook, onStep );
                    }
                    break;

                case "mlg":
                    MlgExporter.ExportMlg( fileName, this.LogBook, this.platform, onStep );
                    break;

                default:
                    throw new ArgumentException(
                        "Invalid filename passed into Export, can only be .xml, .json, .mlg.",
                        "fileName"
                    );
            }
        }

        /// <summary>
        /// Syncs the current logbook with the external logbook.
        /// This does not repopulate the logbook automatically,
        /// call "PopulateLogbook" after this function to do that.
        /// </summary>
        /// <param name="externalLogbook">The external logbook to sync with.</param>
        /// <param name="onStep">
        /// Action to take on each step during the process. Parameter 1 is the current step
        /// we are on.  Parameter 2 is the total number of steps the function will take.
        /// Null for no-op.
        /// </param>
        public void Sync( string externalLogbook, Action<int, int> onStep = null )
        {
            // If sqlite is not open, throw exeption.
            if( this.sqlite == null )
            {
                throw new InvalidOperationException(
                    DatabaseNotOpenMessage
                );
            }
            else if( this.LogBook == null )
            {
                throw new InvalidOperationException(
                    nullLogbook
                );
            }

            MlgSync.Sync( this.LogBook, this.sqlite, externalLogbook, platform, onStep );
        }

        /// <summary>
        /// Closes the sqlite connection.  Does not clear the logbook.
        /// </summary>
        public void Close()
        {
            if( this.sqlite != null )
            {
                this.sqlite.Close();
                this.sqlite = null;
            }
        }
    }
}