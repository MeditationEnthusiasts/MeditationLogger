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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MedEnthDesktop;

namespace MedEnthLogsDesktop
{
    static partial class Program
    {
        // -------- Fields ---------

        private static MedEnthLogsApi.Api api;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Get Api() is in a partial class.  GetApi creates an API for the current platform.
            api = GetApi();

            // Create the folder if it doesn't exist.
            if ( Directory.Exists( Constants.DatabaseFolderLocation ) == false )
            {
                Directory.CreateDirectory( Constants.DatabaseFolderLocation );
            }

            api.Open( Path.Combine( Constants.DatabaseFolderLocation, MedEnthLogsApi.Api.LogbookFileName ) );

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            try
            {
                Application.Run( new HomePage( api, GetMusicManager() ) );
            }
            catch ( Exception e )
            {
                MessageBox.Show( e.Message, "FATAL ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            api.Close();
        }
    }
}
