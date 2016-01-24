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
using System.IO;
using SQLite.Net;
using SQLite.Net.Interop;

namespace MedEnthLogsApi
{
    public class MlgSync
    {
        /// <summary>
        /// Syncs the given logbook/sqlite combination with the external
        /// mlg file.
        /// </summary>
        /// <param name="logBook">The main logbook that is read in by the app.</param>
        /// <param name="sqlite">The sqlite connection with the means to save the logbook.</param>
        /// <param name="mlgToSync">The external MLG file that we want to sync with.</param>
        /// <param name="platform">Platform we are using.</param>
        /// <param name="onStep">
        /// Action to take on each step during the process. Parameter 1 is the current step
        /// we are on.  Parameter 2 is the total number of steps the function will take.
        /// Null for no-op.
        /// </param>
        public static void Sync( LogBook logBook, SQLiteConnection sqlite, string mlgToSync, ISQLitePlatform platform, Action<int, int> onStep = null )
        {
            // Checks:
            // 1. Make sure the file exists.
            if ( File.Exists( mlgToSync ) == false )
            {
                throw new FileNotFoundException(
                    "The file " + mlgToSync + " does not exist.  Unable to sync."
                );
            }

            // 2. Make sure we are dealing with an .mlg file.
            if ( Path.GetExtension( mlgToSync ).ToLower() != ".mlg" )
            {
                throw new ArgumentException(
                    "File to sync must be an .mlg file. Got: " + Path.GetExtension( mlgToSync ),
                    nameof( mlgToSync )
                );
            }

            // With the checks done, we now must sync this current logbook with external one.

            // 1. Open the SQLiteConnection.
            using ( SQLiteConnection externalConnection = new SQLiteConnection( platform, mlgToSync, SQLiteOpenFlags.ReadWrite ) )
            {
                // Next, create an external logbook.
                LogBook externalBook = LogBook.FromSqlite( externalConnection );

                int totalSteps = externalBook.Logs.Count + logBook.Logs.Count;
                int step = 1;

                // Now, iterate through the local logbook and see if the given log
                // exists in external logbook.  If it does not, add it to the external database.
                // If it does, sync both logs and save them to both databases.
                foreach ( Log log in logBook.Logs )
                {
                    if ( externalBook.LogExists( log.Guid ) )
                    {
                        Log oldLog = log.Clone();
                        Log extLog = new Log( externalBook.GetLog( log.Guid ) );

                        Log.Sync( ref oldLog, ref extLog );

                        sqlite.InsertOrReplace( oldLog );
                        externalConnection.InsertOrReplace( extLog );
                    }
                    else
                    {
                        externalConnection.Insert( log );
                    }

                    if ( onStep != null )
                    {
                        onStep( step++, totalSteps );
                    }
                }

                // Next, iterate through all the logs in the external book.
                // If it doesn't exist in the local one, add it.
                // We already took care of syncing during the first iteration.
                foreach ( Log externalLog in externalBook.Logs )
                {
                    if ( logBook.LogExists( externalLog.Guid ) == false )
                    {
                        sqlite.Insert( externalLog );
                    }

                    if ( onStep != null )
                    {
                        onStep( step++, totalSteps );
                    }
                }

                // Lastly, commit both sqlites.
                externalConnection.Commit();
                sqlite.Commit();
            }
        }
    }
}
