// 
// Meditation Logger.
// Copyright (C) 2015-2016  Seth Hendrick.
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
using SQLite.Net;
using SQLite.Net.Interop;

namespace MedEnthLogsApi
{
    /// <summary>
    /// Handles Importing / Exporting the logbook from .mlg.
    /// </summary>
    public class MlgExporter
    {
        /// <summary>
        /// Exports the given log book to the given outfile as an mlg.
        /// </summary>
        /// <param name="outFile">Where to export the mlg to.</param>
        /// <param name="logBook">The logbook to export.</param>
        /// <param name="platform">The sqlite platform to use.</param>
        public static void ExportMlg( string outFile, LogBook logBook, ISQLitePlatform platform )
        {
            using ( SQLiteConnection sqlite = new SQLiteConnection( platform, outFile, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite ) )
            {
                sqlite.CreateTable<Log>();
                foreach ( Log log in logBook.Logs )
                {
                    sqlite.Insert( log );
                }
                sqlite.Commit();
                sqlite.Close();
            }
        }

        /// <summary>
        /// Imports the given mlg file to the given logbook.
        /// </summary>
        /// <param name="inFile">mlg file to import.</param>
        /// <param name="logBook">The logbook to check for duplicates.</param>
        /// <param name="platform">The sqlite platform to use.</param>
        /// <param name="logSqlite">The sqlite connection to import the logs to.</param>
        public static void ImportMlg( string inFile, LogBook logBook, ISQLitePlatform platform, SQLiteConnection logSqlite )
        {
            List<Log> logs = new List<Log>();

            using ( SQLiteConnection sqlite = new SQLiteConnection( platform, inFile, SQLiteOpenFlags.ReadOnly ) )
            {
                var query = sqlite.Table<Log>().Where( x => x.Id > 0 );
                foreach ( Log q in query )
                {
                    Log log = q;

                    // We ignore GUID and Edit time in the file,
                    // and create them here.
                    Guid guid = Guid.NewGuid();

                    // Keep looking until we have a unique guid.
                    while ( logBook.LogExists( guid ) || ( logs.Find( i => i.Guid == guid ) != null ) )
                    {
                        guid = Guid.NewGuid();
                    }

                    log.Guid = guid;
                    log.EditTime = DateTime.Now;
                    log.Validate();
                    logs.Add( log );
                }
                sqlite.Close();
            }

            if ( logs.Count != 0 )
            {
                foreach ( Log newLog in logs )
                {
                    logSqlite.Insert( newLog );
                }

                logSqlite.Commit();
            }
        }
    }
}
