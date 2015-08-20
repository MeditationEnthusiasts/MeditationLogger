using System;
using System.Collections.Generic;
using System.Text;
using SQLite.Net.Attributes;

namespace MedEnthLogsApi
{
    /// <summary>
    /// This class represents a specific instance
    /// of an event logged.
    /// </summary>
    public class Log
    {
        // -------- Constructor -------

        /// <summary>
        /// Constructor
        /// </summary>
        public Log()
        {
            this.Id = -1;
            this.StartTime = DateTime.MinValue;
            this.EndTime = DateTime.MinValue;
            this.Comments = string.Empty;
            this.Location = string.Empty;
        }

        // -------- Properties --------

        /// <summary>
        /// The unique ID for SQLite.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

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
        /// When the session was first recorded
        /// (UTC, the UI must convert it to local time).
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// The last time this log was edited.
        /// (UTC, the UI must convert it to local time).
        /// </summary>
        public DateTime EditTime { get; set; }

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
        /// The comments the user wrote about the session.
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Where the user had the session.
        /// This can either be GPS coordinates or a location the user
        /// specifies.
        /// </summary>
        public string Location { get; set; }

        // -------- Functions --------

        /// <summary>
        /// Returns true if ALL properties EXCEPT for ID match.
        /// Useful to tell if two logs from two different databases match.
        /// </summary>
        /// <param name="obj">The other object to compare.</param>
        /// <returns>true if all properties are equal, else false.</returns>
        public override bool Equals( object obj )
        {
            Log other = obj as Log;
            if ( other == null )
            {
                return false;
            }

            return
                ( this.CreateTime == other.CreateTime ) &&
                ( this.EditTime == other.EditTime ) &&
                ( this.EndTime == other.EndTime ) &&
                ( this.StartTime == other.StartTime ) &&
                ( this.Location == other.Location ) &&
                ( this.Comments == other.Comments );
        }

        /// <summary>
        /// Returns the hash code.
        /// The hash code is based on the Creation Date of the Log.
        /// There can never be two logs with the same creation date in a log book,
        /// making the create time the best candidate to use for the hash code.
        /// </summary>
        /// <returns>The hash code based on the creation date's hashcode.</returns>
        public override int GetHashCode()
        {
            return CreateTime.GetHashCode();
        }
    }
}
