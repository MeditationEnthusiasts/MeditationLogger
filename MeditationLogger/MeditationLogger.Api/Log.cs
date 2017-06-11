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
using SQLite.Net.Attributes;

namespace MeditationEnthusiasts.MeditationLogger.Api
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

    /// <summary>
    /// This class represents a specific instance
    /// of an event logged.
    /// </summary>
    public class Log : ILog
    {
        // -------- Fields ---------

        /// <summary>
        /// Error message that appears
        /// in validate if the end time is less than that start time.
        /// internal for unit tests.
        /// </summary>
        internal const string EndTimeLessThanStartTimeMessage = "Log End Time is less than the start time.";

        /// <summary>
        /// Error message that appears
        /// if the latitude is set on the log, but not the longitude.
        /// </summary>
        internal const string LatitudeSetNoLongitude = "Latitude set on log, but not longitude";

        /// <summary>
        /// Error message that appears
        /// if the longitude is set on the log, but not the latitude.
        /// </summary>
        internal const string LongitudeSetNoLatitude = "Longitude set on long, but not latitude";

        /// <summary>
        /// Comments about the session.
        /// </summary>
        private string comments;

        /// <summary>
        /// The technique used.
        /// </summary>
        private string technique;

        /// <summary>
        /// The string for start time when importing/exporting.
        /// </summary>
        public const string StartTimeString = "StartTime";

        /// <summary>
        /// The string for end time when importing/exporting.
        /// </summary>
        public const string EndTimeString = "EndTime";

        /// <summary>
        /// The string for Guid when importing/exporting.
        /// </summary>
        public const string GuidString = "Guid";

        /// <summary>
        /// The string for edit time when importing/exporting.
        /// </summary>
        public const string EditTimeString = "EditTime";

        /// <summary>
        /// The string for Comments when importing/exporting.
        /// </summary>
        public const string CommentsString = "Comments";

        /// <summary>
        /// The string for Technique when importing/exporting.
        /// </summary>
        public const string TechniqueString = "Technique";

        /// <summary>
        /// The string for Latitude when importing/exporting.
        /// </summary>
        public const string LatitudeString = "Latitude";

        /// <summary>
        /// The string for Latitude when importing/exporting.
        /// </summary>
        public const string LongitudeString = "Longitude";

        /// <summary>
        /// The maximum time we support.
        /// Mainly here for unit testing purposes.
        /// The MaxTime is able to fit in a sqlite database.  .Net's DateTime.MaxValue doesn't always.
        /// On Linux, sqlite seems to insert it as 0.
        /// Another example is here: http://stackoverflow.com/questions/6127123/net-datetime-maxvalue-is-different-once-it-is-stored-in-database
        /// </summary>
        public static readonly DateTime MaxTime = new DateTime( 5000, 1, 1, 0, 0, 0 ).ToUniversalTime(); // Year 5000 is good enough.

        // -------- Constructor -------

        /// <summary>
        /// Constructor
        /// </summary>
        public Log()
        {
            this.Id = -1;

            // Fun fact!  DateTime.MinValue seems to return local time, not UTC time.
            this.EndTime = DateTime.MinValue.ToUniversalTime();

            // Make start time ahead of end time,
            // this will make the log in an invalid state, as the
            // start time is ahead of the end time which is not allowed.
            this.StartTime = MaxTime;

            this.Guid = Guid.NewGuid();

            this.EditTime = DateTime.MinValue.ToUniversalTime();
            this.Comments = string.Empty;
            this.Technique = string.Empty;
            this.Latitude = null;
            this.Longitude = null;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="log">The log to copy from.</param>
        public Log( ILog log )
        {
            this.Id = log.Id;
            this.EndTime = log.EndTime;
            this.StartTime = log.StartTime;
            this.Guid = log.Guid;
            this.EditTime = log.EditTime;
            this.Comments = log.Comments;
            this.Technique = log.Technique;
            this.Latitude = log.Latitude;
            this.Longitude = log.Longitude;
        }

        // -------- Properties --------

        /// <summary>
        /// How long the session lasted.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return EndTime - StartTime;
            }
        }

        /// <summary>
        /// The unique ID for SQLite.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        /// <summary>
        /// When the session starts
        /// (UTC, the UI must convert it to local time).
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// When the session ends
        /// (UTC, the UI must convert it to local time).
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Unique GUID for the log objet.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// The last time this log was edited.
        /// (UTC, the UI must convert it to local time).
        /// </summary>
        public DateTime EditTime { get; set; }

        /// <summary>
        /// The comments the user wrote about the session.
        /// </summary>
        public string Comments
        {
            get
            {
                return comments;
            }
            set
            {
                if( value == null )
                {
                    throw new ArgumentNullException( "value", "Log comments can not be null" );
                }

                comments = value;
            }
        }

        /// <summary>
        /// The Technique of the session.
        /// </summary>
        public string Technique
        {
            get
            {
                return technique;
            }
            set
            {
                if( value == null )
                {
                    throw new ArgumentNullException( "value", "Log technique can not be null" );
                }

                technique = value;
            }
        }

        /// <summary>
        /// The latitude of where the session took place.
        /// null if no location specified.
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// The longitude of where the session took place.
        /// null if no location specified.
        /// </summary>
        public decimal? Longitude { get; set; }

        // -------- Functions --------

        /// <summary>
        /// Returns true if ALL properties EXCEPT for ID and GUID match.
        /// Useful to tell if two logs from two different databases match.
        /// </summary>
        /// <param name="obj">The other object to compare.</param>
        /// <returns>true if all properties are equal, else false.</returns>
        public override bool Equals( object obj )
        {
            ILog other = obj as ILog;
            if( other == null )
            {
                return false;
            }

            return
                ( this.EditTime.Equals( other.EditTime ) ) &&
                ( this.EndTime.Equals( other.EndTime ) ) &&
                ( this.StartTime.Equals( other.StartTime ) ) &&
                ( this.Technique.Equals( other.Technique ) ) &&
                ( this.Comments.Equals( other.Comments ) ) &&
                ( this.Latitude.Equals( other.Latitude ) ) &&
                ( this.Longitude.Equals( other.Longitude ) );
        }

        public override string ToString()
        {
            return
                "Guid: " + this.Guid.ToString() + Environment.NewLine +
                "Edit Time: " + this.EditTime.ToString() + Environment.NewLine +
                "Start Time: " + this.StartTime.ToString() + Environment.NewLine +
                "End Time: " + this.EndTime.ToString() + Environment.NewLine +
                "Technique: " + this.Technique + Environment.NewLine +
                "Latitude:" + this.Latitude + Environment.NewLine +
                "Longitude: " + this.Longitude + Environment.NewLine +
                "Comments: " + this.Comments + Environment.NewLine;
        }

        /// <summary>
        /// Returns the hash code.
        /// The hash code is based on the Guid of the Log.
        /// There can never be two logs with the same Guid in a log book,
        /// making the guid the best candidate to use for the hash code.
        /// </summary>
        /// <returns>The hash code based on the Guid's hashcode.</returns>
        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        /// <summary>
        /// Ensures the log is in a good state.  Should be called before saving it.
        /// Throws LogValidationException if:
        /// Start time > End Time.
        /// Latitude exists, longitude does not.
        /// Longitude exists, latitude does not.
        /// </summary>
        public void Validate()
        {
            if( this.EndTime < this.StartTime )
            {
                throw new LogValidationException( EndTimeLessThanStartTimeMessage );
            }
            else if( ( this.Latitude == null ) && ( this.Longitude != null ) )
            {
                throw new LogValidationException( LongitudeSetNoLatitude );
            }
            else if( ( this.Longitude == null ) && ( this.Latitude != null ) )
            {
                throw new LogValidationException( LatitudeSetNoLongitude );
            }
        }

        /// <summary>
        /// Returns a Clone of this object.
        /// Note, this means the Guid will be the same on the original and the clone.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public Log Clone()
        {
            return (Log)this.MemberwiseClone();
        }

        /// <summary>
        /// Syncs the two logs.  The one with the most recent EditTime
        /// is what both logs become.
        /// ID is not updated.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the GUIDs of the logs do not match.</exception>
        /// <param name="log1">Reference to the first log to sync.</param>
        /// <param name="log2">Reference to the second log to sync.</param>
        public static void Sync( ref Log log1, ref Log log2 )
        {
            // Can only sync logs that match GUIDs
            if( log1.Guid != log2.Guid )
            {
                throw new InvalidOperationException(
                    "Can only sync logs that have matching GUIDs"
                );
            }
            // No-op if they already are equal.
            else if( log1.Equals( log2 ) )
            {
                return;
            }

            // If log1 is older than log2, make log1 become log2.
            else if( log1.EditTime < log2.EditTime )
            {
                log1.Comments = log2.Comments;
                log1.Latitude = log2.Latitude;
                log1.Longitude = log2.Longitude;
                log1.StartTime = log2.StartTime;
                log1.EndTime = log2.EndTime;
                log1.EditTime = log2.EditTime;
                log1.Technique = log2.Technique;
            }
            // If log2 is older than log1, make log2 become log1.
            else if( log1.EditTime > log2.EditTime )
            {
                log2.Comments = log1.Comments;
                log2.Latitude = log1.Latitude;
                log2.Longitude = log1.Longitude;
                log2.StartTime = log1.StartTime;
                log2.EndTime = log1.EndTime;
                log2.EditTime = log1.EditTime;
                log2.Technique = log1.Technique;
            }

            // Note we are not changing the ID.  This is on purpose
            // so sqlite can still figure out where to put the log in.

            // No-op if the edit times match.  We can't determine which is the most recent,
            // so don't mess with them.
        }
    }
}