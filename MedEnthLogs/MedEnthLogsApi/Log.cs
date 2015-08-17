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
    }
}
