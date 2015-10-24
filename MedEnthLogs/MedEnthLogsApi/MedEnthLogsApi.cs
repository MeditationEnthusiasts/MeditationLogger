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
using System.Xml;
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

        const string xmlNameSpace = "http://app.meditationenthusiasts.org/schemas/logs/2015/LogXmlSchema.xsd";

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
        /// The current log being written to.
        /// internal for unit tests only.
        /// </summary>
        internal Log currentLog;

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
        public Api( ILocationDetector locationDetector, ITimer timer, IMusicManager musicManager )
        {
            this.sqlite = null;
            this.LogBook = null;
            this.LocationDetector = locationDetector;
            this.Timer = timer;
            this.MusicManager = musicManager;
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
        public ITimer Timer { get; private set; }

        /// <summary>
        /// The Audio engine to use.
        /// </summary>
        public IMusicManager MusicManager { get; private set; }

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
        /// This creates a log (and by extension, sets the creation time to now),
        /// and sets the start time to now as well.
        /// No-op if session in progress.
        /// </summary>
        /// <param name="length">How long to meditate for.  Null for unlimited.</param>
        public void StartSession( TimeSpan? length = null )
        {
            if ( this.IsSessionInProgress == false )
            {
                this.currentLog = new Log();
                this.currentLog.CreateTime = DateTime.Now.ToUniversalTime();
                this.currentLog.StartTime = this.currentLog.CreateTime;
                this.currentLog.EditTime = this.currentLog.CreateTime;
                this.IsSessionInProgress = true;
                this.Timer.StartTimer( length );
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
                this.Timer.StopAndResetTimer();
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
                        ImportFromXml( outFile );
                    }
                    break;

                case "json":
                    using ( FileStream outFile = new FileStream( fileName, FileMode.Open, FileAccess.Read ) )
                    {
                        ImportFromJson( outFile );
                    }
                    break;
                case "mlg":
                    ImportFromMlg( fileName );
                    break;
                default:
                    throw new ArgumentException(
                        "Invalid filename passed into Import, can only be .xml, .json, .mlg.",
                        "fileName"
                    );
            }
        }

        /// <summary>
        /// Imports logs from XML to the database.
        /// This will not repopulate the logbook itself.  You must call that after
        /// to get the latest logs.
        /// </summary>
        /// <param name="outFile">The stream the outputs the file.</param>
        public void ImportFromXml( Stream outFile )
        {
            XmlDocument doc = new XmlDocument();
            doc.Load( outFile );

            List<Log> logs = new List<Log>();

            XmlElement rootNode = doc.DocumentElement;
            if ( rootNode.Name != "logbook" )
            {
                throw new XmlException(
                    "Root node should be named \"logbook\".  Got: " + rootNode.Name
                );
            }

            foreach ( XmlNode node in rootNode.ChildNodes )
            {
                if ( node.Name != "log" )
                {
                    throw new XmlException(
                        "Element is not a log.  Got: " + node.Name
                    );
                }

                Log log = new Log();

                foreach ( XmlAttribute attr in node.Attributes )
                {
                    switch ( attr.Name )
                    {
                        case ( "StartTime" ):
                            log.StartTime = DateTime.Parse( attr.Value );
                            break;

                        case ( "EndTime" ):
                            log.EndTime = DateTime.Parse( attr.Value );
                            break;

                        case ( "Technique" ):
                            log.Technique = attr.Value;
                            break;

                        case ( "Comments" ):
                            log.Comments = attr.Value;
                            break;

                        case ( "Latitude" ):
                            // Try to parse the latitude.  If fails, just make it empty.
                            decimal lat;
                            if ( decimal.TryParse( attr.Value, out lat ) )
                            {
                                log.Latitude = lat;
                            }
                            else
                            {
                                log.Latitude = null;
                            }
                            break;

                        case ( "Longitude" ):
                            // Try to parse the Longitude.  If fails, just make it empty.
                            decimal lon;
                            if ( decimal.TryParse( attr.Value, out lon ) )
                            {
                                log.Longitude = lon;
                            }
                            else
                            {
                                log.Longitude = null;
                            }
                            break;
                    }
                }

                DateTime creationTime = DateTime.Now.ToUniversalTime();

                // Keep looking until we have a unique creation date.
                while ( this.LogBook.LogExists( creationTime ) || ( logs.Find( i => i.CreateTime == creationTime ) != null ) )
                {
                    creationTime = DateTime.Now.ToUniversalTime();
                }
                log.CreateTime = creationTime;
                log.EditTime = creationTime;

                // Ensure the log is good.
                log.Validate();

                // Add to list.
                logs.Add( log );
            }

            // Last thing to do is add the new logs to the database.
            if ( logs.Count != 0 )
            {
                foreach ( Log newLog in logs )
                {
                    this.sqlite.Insert( newLog );
                }

                this.sqlite.Commit();
            }
        }

        private void ImportFromJson( FileStream outFile )
        {
            throw new NotImplementedException();
        }

        private void ImportFromMlg( string fileName )
        {
            throw new NotImplementedException();
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
            switch(splitString[splitString.Length - 1].ToLower())
            {
                case "xml":
                    using ( FileStream outFile = new FileStream( fileName, FileMode.Create, FileAccess.Write ) )
                    {
                        ExportToXml( outFile );
                    }
                    break;

                case "json":
                    using ( FileStream outFile = new FileStream( fileName, FileMode.Create, FileAccess.Write ) )
                    {
                        ExportToJson( outFile );
                    }
                    break;
                case "mlg":
                    ExportToMlg( fileName );
                    break;
                default:
                    throw new ArgumentException(
                        "Invalid filename passed into Export, can only be .xml, .json, .mlg.",
                        "fileName"
                    );
            }
        }

        /// <summary>
        /// Exports the loaded logbook to XML.
        /// </summary>
        /// <param name="outFile">The stream which outputs the file.</param>
        public void ExportToXml( Stream outFile )
        {
            XmlDocument doc = new XmlDocument();

            // Create declaration.
            XmlDeclaration dec = doc.CreateXmlDeclaration( "1.0", "UTF-8", null );

            // Add declaration to document.
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore( dec, root );

            XmlNode logbookNode = doc.CreateNode( XmlNodeType.Element, "logbook", xmlNameSpace );

            foreach ( Log log in this.LogBook.Logs )
            {
                XmlNode logNode = doc.CreateNode( XmlNodeType.Element, "log", xmlNameSpace );

                // Reducing scope so I don't accidently add the wrong attribute.
                {
                    XmlAttribute creationTime = doc.CreateAttribute( "CreationTime" );
                    creationTime.Value = log.CreateTime.ToString( "o" );
                    logNode.Attributes.Append( creationTime );
                }
                {
                    XmlAttribute editTime = doc.CreateAttribute( "EditTime" );
                    editTime.Value = log.EditTime.ToString( "o" );
                    logNode.Attributes.Append( editTime );
                }
                {
                    XmlAttribute startTime = doc.CreateAttribute( "StartTime" );
                    startTime.Value = log.StartTime.ToString( "o" );
                    logNode.Attributes.Append( startTime );
                }
                {
                    XmlAttribute endTime = doc.CreateAttribute( "EndTime" );
                    endTime.Value = log.EndTime.ToString( "o" );
                    logNode.Attributes.Append( endTime );
                }
                {
                    XmlAttribute technique = doc.CreateAttribute( "Technique" );
                    technique.Value = log.Technique;
                    logNode.Attributes.Append( technique );
                }
                {
                    XmlAttribute comments = doc.CreateAttribute( "Comments" );
                    comments.Value = log.Comments;
                    logNode.Attributes.Append( comments );
                }
                {
                    XmlAttribute lat = doc.CreateAttribute( "Latitude" );
                    lat.Value = log.Latitude.HasValue ? log.Latitude.ToString() : string.Empty;
                    logNode.Attributes.Append( lat );
                }
                {
                    XmlAttribute lon = doc.CreateAttribute( "Longitude" );
                    lon.Value = log.Longitude.HasValue ? log.Longitude.ToString() : string.Empty;
                    logNode.Attributes.Append( lon );
                }

                logbookNode.AppendChild( logNode );
            }

            doc.InsertAfter( logbookNode, dec );
            doc.Save( outFile );
        }

        /// <summary>
        /// Exports the loaded logbook to json.
        /// </summary>
        /// <param name="outFile">The stream which outputs the file.</param>
        public void ExportToJson( Stream outFile )
        {
            // Use JSon.net package.
            throw new NotImplementedException( "Not implemented yet." );
        }

        /// <summary>
        /// Exports the loaded logbook to MLG (SQLite database).
        /// </summary>
        /// <param name="fileName"></param>
        public void ExportToMlg( string fileName )
        {
            throw new NotImplementedException( "Not implemented yet." );
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
