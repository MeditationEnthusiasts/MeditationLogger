// 
// Meditation Logger.
// Copyright (C) 2015  Seth Hendrick.
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
using SQLite.Net;
using SQLite.Net.Interop;

[assembly: InternalsVisibleTo( "Test" )]
namespace MedEnthLogsApi
{
    /// <summary>
    /// The API for talking to the backend of the application.
    /// </summary>
    public class Api
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
        public const string Version = "0.1.0";

        // -------- Constructor --------

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="locationDetector">Location to the location detector.</param>
        /// <param name="timer">The timing engine to use.</param>
        /// <param name="musicManager">The music manager to use.</param>
        /// <param name="platform">The sqlite platform we are using.</param>
        public Api( ILocationDetector locationDetector, ITimer timer, IMusicManager musicManager, ISQLitePlatform platform )
        {
            this.sqlite = null;
            this.LogBook = null;
            this.platform = platform;
            this.LocationDetector = locationDetector;
            this.timer = timer;
            this.musicManager = musicManager;
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
        /// Class that detects the location.
        /// </summary>
        public ILocationDetector LocationDetector { get; private set; }

        /// <summary>
        /// The timer engine to use.
        /// </summary>
        public ITimer timer { get; private set; }

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
            if ( this.sqlite != null )
            {
                this.Close();
            }

            this.sqlite = new SQLiteConnection( this.platform, path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite );
            this.sqlite.CreateTable<Log>();
            this.sqlite.Commit();
        }

        /// <summary>
        /// Resets the current log to default values.
        /// </summary>
        public void ResetCurrentLog()
        {
            this.currentLog = new Log();
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
            if ( config == null )
            {
                throw new ArgumentNullException( "config" );
            }

            try
            {
                if ( this.IsSessionInProgress == false )
                {
                    this.currentLog = new Log();
                    this.currentLog.StartTime = DateTime.Now.ToUniversalTime();
                    this.currentLog.EditTime = this.currentLog.StartTime;

                    TimeSpan? length = config.Length;

                    if ( config.PlayMusic )
                    {
                        this.musicManager.Validate( config.AudioFile );
                        if ( config.LoopMusic )
                        {
                            this.musicManager.OnStop =
                                delegate ()
                                {
                                    if ( this.IsSessionInProgress )
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

                    this.IsSessionInProgress = true;
                }
            }
            catch ( Exception )
            {
                this.currentLog = new Log();
                throw;
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
                this.timer.StopAndResetTimer();
                this.currentLog.EndTime = DateTime.Now.ToUniversalTime();
                this.currentLog.EditTime = currentLog.EndTime;
                this.musicManager.Stop();
                this.IsSessionInProgress = false;
            }
        }

        /// <summary>
        /// Validates and saves the CurrentSession to the database.
        /// 
        /// Throws InvalidOperationException if a session is in progress or database is not opened.
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

            this.currentLog.Latitude = latitude;
            this.currentLog.Longitude = longitude;

            // If validating the log fails, throw.
            this.CurrentLog.Validate();

            // Otherwise, Edit the log one last time and save it to the database.
            if ( technique != null )
            {
                this.currentLog.Technique = technique;
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

            List<ILog> logs = new List<ILog>();

            var query = this.sqlite.Table<Log>().Where( x => x.Id > 0 );
            foreach( Log q in query )
            {
                logs.Add( q );
            }

            this.LogBook = new LogBook( logs );
        }

        // ---- Import Functions ----

        /// <summary>
        /// Performs an Import.
        /// The type of Import (XML, JSON, MLG) depends on the
        /// file extension (case doesn't matter).
        /// </summary>
        /// <param name="fileName">Where to import the file to.</param>
        public void Import( string fileName )
        {
            string[] splitString = fileName.Split( '.' );
            switch ( splitString[splitString.Length - 1].ToLower() )
            {
                case "xml":
                    using ( FileStream outFile = new FileStream( fileName, FileMode.Open, FileAccess.Read ) )
                    {
                        XmlExporter.ImportFromXml( outFile, this.LogBook, this.sqlite );
                    }
                    break;

                case "json":
                    using ( FileStream outFile = new FileStream( fileName, FileMode.Open, FileAccess.Read ) )
                    {
                        JsonExporter.ImportFromJson( outFile, this.LogBook, this.sqlite );
                    }
                    break;
                case "mlg":
                    MlgExporter.ImportMlg( fileName, this.LogBook, this.platform, this.sqlite );
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
        public void Export( string fileName )
        {
            string[] splitString = fileName.Split( '.' );
            switch ( splitString[splitString.Length - 1].ToLower() )
            {
                case "xml":
                    using ( FileStream outFile = new FileStream( fileName, FileMode.Create, FileAccess.Write ) )
                    {
                        XmlExporter.ExportToXml( outFile, this.LogBook );
                    }
                    break;

                case "json":
                    using ( FileStream outFile = new FileStream( fileName, FileMode.Create, FileAccess.Write ) )
                    {
                        JsonExporter.ExportJson( outFile, this.LogBook );
                    }
                    break;
                case "mlg":
                    MlgExporter.ExportMlg( fileName, this.LogBook, this.platform );
                    break;
                default:
                    throw new ArgumentException(
                        "Invalid filename passed into Export, can only be .xml, .json, .mlg.",
                        "fileName"
                    );
            }
        }

        /// <summary>
        /// Closes the sqlite connection.  Does not clear the logbook.
        /// </summary>
        public void Close()
        {
            if ( this.sqlite != null )
            {
                this.sqlite.Close();
                this.sqlite = null;
            }
        }
    }
}
