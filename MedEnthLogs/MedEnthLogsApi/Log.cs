﻿// 
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
using SQLite.Net.Attributes;

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

        // -------- Constructor -------

        /// <summary>
        /// Constructor
        /// </summary>
        public Log()
        {
            this.Id = -1;
            this.EndTime = DateTime.MinValue;

            // Make start time ahead of end time, 
            // this will make the log in an invalid state, as the
            // start time is ahead of the end time which is not allowed.
            this.StartTime = DateTime.MaxValue;

            this.Guid = Guid.NewGuid();

            this.EditTime = DateTime.MinValue;
            this.Comments = string.Empty;
            this.Technique = string.Empty;
            this.Latitude = null;
            this.Longitude = null;
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
                if ( value == null )
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
                if ( value == null )
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
            if ( other == null )
            {
                return false;
            }

            return
                ( this.EditTime == other.EditTime ) &&
                ( this.EndTime == other.EndTime ) &&
                ( this.StartTime == other.StartTime ) &&
                ( this.Technique == other.Technique ) &&
                ( this.Comments == other.Comments ) &&
                ( this.Latitude == other.Latitude ) &&
                ( this.Longitude == other.Longitude );
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
            if ( this.EndTime < this.StartTime )
            {
                throw new LogValidationException( EndTimeLessThanStartTimeMessage );
            }
            else if ( ( this.Latitude == null ) && ( this.Longitude != null ) )
            {
                throw new LogValidationException( LongitudeSetNoLatitude );
            }
            else if ( ( this.Longitude == null ) && ( this.Latitude != null ) )
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
            return (Log) this.MemberwiseClone();
        }
    }
}
