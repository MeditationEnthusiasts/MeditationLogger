using System;
using SQLite.Net.Attributes;

namespace MedEnthLogsApi
{
    /// <summary>
    /// This class represents a specific instance
    /// of an event logged.
    /// </summary>
    public class Log : ILog
    {
        // -------- Fields ---------

        /// <summary>
        /// Comments about the session.
        /// </summary>
        private string comments;

        /// <summary>
        /// Where the session took place.
        /// </summary>
        private string location;

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

            // Make Creation time ahead of edit time, 
            // this will make the log in an invalid state, as the
            // creation time is ahead of the edit time which is not allowed.
            this.CreateTime = DateTime.MaxValue;

            this.EditTime = DateTime.MinValue;
            this.Comments = string.Empty;
            this.Location = string.Empty;
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
        /// Where the user had the session.
        /// This can either be GPS coordinates or a location the user
        /// specifies.
        /// </summary>
        public string Location
        {
            get
            {
                return location;
            }
            set
            {
                if ( value == null )
                {
                    throw new ArgumentNullException( "value", "Log location can not be null" );
                }

                location = value;
            }
        }

        // -------- Functions --------

        /// <summary>
        /// Returns true if ALL properties EXCEPT for ID match.
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

        /// <summary>
        /// Returns a Clone of this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public Log Clone()
        {
            return (Log) this.MemberwiseClone();
        }
    }
}
