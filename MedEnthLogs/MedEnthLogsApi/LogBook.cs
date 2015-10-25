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

namespace MedEnthLogsApi
{
    /// <summary>
    /// This class represents a book of logs.
    /// </summary>
    public class LogBook
    {
        // -------- Fields --------

        /// <summary>
        /// Table of logs whose key is the creation time of the logs.
        /// Useful for quick lookups to see if a log exists.
        /// </summary>
        private Dictionary<DateTime, ILog> logTable;

        /// <summary>
        /// A list of logs in the order from which the start time is,
        /// where the most current is index 0, and the earliest is 
        /// the last element in the list.
        /// </summary>
        private List<ILog> logTableByStartTime;

        // -------- Constructor --------

        /// <summary>
        /// Constructor.
        /// 
        /// Note, to save on memory, the logs are not copied from
        /// the logs parameter.  If you want the logbook to contain copies,
        /// create a new list with copies and pass that list in.
        /// 
        /// Note, if any logs have conflicting CreationDates, the one that appears
        /// later in the list will be saved, and the older one discarded.
        /// Please ensure you check for this before calling this if you
        /// want to keep both.
        /// </summary>
        /// <param name="logs">Logs to add to the log book.</param>
        public LogBook( IList<ILog> logs )
        {
            // Populate Dictionary.
            this.logTable = new Dictionary<DateTime, ILog>();
            foreach ( ILog log in logs )
            {
                logTable[log.CreationTime] = log;
            }

            // Sort the logs.
            logTableByStartTime = new List<ILog>( logTable.Values );
            logTableByStartTime.Sort(
                delegate ( ILog a, ILog b )
                {
                    return b.StartTime.CompareTo( a.StartTime );
                }
            );

            this.Logs = logTableByStartTime.AsReadOnly();

            this.TotalTime = 0;
            this.LongestTime = 0;
            foreach ( ILog log in this.Logs )
            {
                this.TotalTime += log.Duration.TotalMinutes;
                this.LongestTime = Math.Max( log.Duration.TotalMinutes, this.LongestTime );
            }
        }

        // --------- Properties --------

        /// <summary>
        /// Readonly List of logs.
        /// The order of the list is based on the start time of the session.
        /// The latest session (the one with the greater date) is at index 0.
        /// Earlier sessions are towards the end of the list.
        /// </summary>
        public IList<ILog> Logs { get; private set; }

        /// <summary>
        /// The total time of all the logs.
        /// </summary>
        public double TotalTime { get; private set; }

        /// <summary>
        /// The longest session time of all logs.
        /// </summary>
        public double LongestTime { get; private set; }

        // --------- Functions --------

        /// <summary>
        /// Checks to see if a log exists based on
        /// The creation time.
        /// </summary>
        /// <param name="creationTime">The time to check.</param>
        /// <returns>True if the log exists, else false.</returns>
        public bool LogExists( DateTime creationTime )
        {
            return this.logTable.ContainsKey( creationTime );
        }

        /// <summary>
        /// Returns the reference to a log based on the creation time.
        /// 
        /// Throws KeyNotFoundException if time does not exist.
        /// </summary>
        /// <param name="creationTime">The creation time that should contain the log.</param>
        /// <returns>The log that was created at the specified time.</returns>
        public ILog GetLog( DateTime creationTime )
        {
            return logTable[creationTime];
        }
    }
}
