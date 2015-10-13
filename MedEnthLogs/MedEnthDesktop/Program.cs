﻿using System;
using System.Collections.Generic;
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
            api = new MedEnthLogsApi.Api( new Win32LocationDetector(), new Win32Timer() );

            // Use the win32 sqlite.
            api.Open( new SQLite.Net.Platform.Win32.SQLitePlatformWin32(), "test.db" );

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            Application.Run( new HomePage( api ) );

            api.Close();
        }
    }
}
