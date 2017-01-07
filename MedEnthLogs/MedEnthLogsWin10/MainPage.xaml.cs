// 
// Meditation Logger.
// Copyright (C) 2015-2017  Seth Hendrick.
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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MedEnthLogsApi;
using SQLite.Net.Attributes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MedEnthLogsWin10
{
    /// <summary>
    /// This page is the Home Screen.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // -------- Fields --------

        /// <summary>
        /// Reference to the API.
        /// </summary>
        private Api api;

        // -------- Constructor --------

        public MainPage()
        {
            this.InitializeComponent();

            this.api = new Api(
                new Win10LocationDetector(),
                null,
                new Win10MusicManager(),
                new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT()
            );

            // This will put the database in app data.
            string folder = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            this.api.Open( folder + @"\test.db" );
            this.api.Close();
        }

        private void StartMeditatingButton_Click( object sender, RoutedEventArgs e )
        {
            this.Frame.Navigate( typeof( MeditationPage ) );
        }
    }
}
