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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MedEnthDesktop;

namespace MedEnthLogsDesktop
{
    static class Program
    {
        private static MedEnthLogsApi.Api api;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            api = new MedEnthLogsApi.Api(
                new Win32LocationDetector(),
                new Win32Timer(),
                new NAudioMusicManager()
            );

            // Use the win32 sqlite.
            string dbLocation = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
            dbLocation = Path.Combine( dbLocation, "MeditationLoggerDesktop" );

            if ( Directory.Exists( dbLocation ) == false )
            {
                Directory.CreateDirectory( dbLocation );
            }

            api.Open( new SQLite.Net.Platform.Win32.SQLitePlatformWin32(), Path.Combine( dbLocation, "logbook.mlg" ) );

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            Application.Run( new HomePage( api ) );

            api.Close();
        }
    }
}
