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
using SQLite.Net.Attributes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MedEnthLogsWin10
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // -------- Fields --------

        /// <summary>
        /// Reference to the API.
        /// </summary>
        private MedEnthLogsApi.MedEnthLogsApi api;

        // -------- Constructor --------

        public MainPage()
        {
            this.InitializeComponent();

            this.api = new MedEnthLogsApi.MedEnthLogsApi();

            // This will put the database in app data.
            string folder = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            this.api.Open( new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), folder + @"\test.db" );
            this.api.Close();
        }
    }
}
