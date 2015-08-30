using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            api = new MedEnthLogsApi.Api( new Win32LocationDetector() );

            // Use the win32 sqlite.
            api.Open( new SQLite.Net.Platform.Win32.SQLitePlatformWin32(), "test.db" );
            api.Close();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            Application.Run( new HomePage() );
        }
    }
}
