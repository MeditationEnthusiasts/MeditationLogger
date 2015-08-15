using System;
using System.Collections.Generic;
using System.Text;
using SQLite.Net;
using SQLite.Net.Interop;
using System.IO;

namespace MedEnthLogsApi
{
    public class MedEnthLogsApi
    {
        // -------- Fields --------

        /// <summary>
        /// Reference to a SQLite connection.
        /// </summary>
        private SQLiteConnection sqlite;

        // -------- Constructor --------

        /// <summary>
        /// Constructor
        /// </summary>
        public MedEnthLogsApi()
        {
            this.sqlite = null;
            this.LogBook = null;
        }

        // -------- Properties --------

        /// <summary>
        /// Reference to the Logbook used.
        /// </summary>
        public LogBook LogBook { get; private set; }

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
