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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MedEnthDesktop;
using MedEnthLogsApi;

namespace MedEnthLogsDesktop
{
    public partial class HomePage : Form
    {
        enum StartState
        {
            Idle,    // No activity is happeneing.
            Start,   // The activity has started.
            End      // The activity has ended, but not been saved yet.
        }

        // ------- Fields --------

        MedEnthLogsApi.Api api;

        List<LogView> logViews;

        StartState currentState;

        /// <summary>
        /// Where the execution direcotry is.
        ///
        /// https://msdn.microsoft.com/en-us/library/aa457089.aspx
        /// </summary>
        private static readonly string exeDirectory = Path.GetDirectoryName( Assembly.GetExecutingAssembly().GetName().CodeBase ).Replace( '\\', '/' );

        // ---- Views ----

        OptionsView optionView;
        MeditateView meditateView;
        SaveView saveView;

        public HomePage( MedEnthLogsApi.Api api )
        {
            InitializeComponent();

            this.api = api;
            ReloadLogs();
            this.currentState = StartState.Idle;

            // Setup Option View
            this.optionView = new OptionsView();
            this.optionView.Visible = true;
            ChangableStartView.Controls.Add( this.optionView );

            // Setup meditate view
            this.meditateView = new MeditateView();
            this.meditateView.Visible = false;
            ChangableStartView.Controls.Add( this.meditateView );

            this.api.Timer.OnComplete =
                delegate ()
                {
                    if ( this.currentState == StartState.Start )
                    {
                        GoToNextState();
                    }
                };

            this.api.Timer.OnUpdate =
                delegate ( string updateString )
                {
                    this.meditateView.TimerLabel.Text = updateString;
                };

            this.saveView = new SaveView();
            this.saveView.Visible = false;
            ChangableStartView.Controls.Add( this.saveView );
        }

        readonly string mapHtmlStart = @"
<!doctype html>
<head>
    <meta http-equiv=""content-type"" content=""text/html; charset=utf-8"" />
    <link type=""text/css"" rel=""stylesheet"" href=""" + exeDirectory + @"/html/css/leaflet.css""/>
    <script src=""" + exeDirectory + @"/html/js/leaflet.js""></script>
    <title>Meditation Map</title>
    <!-- Plug in the map information -->
    <script type = ""text/javascript"">
        window.onload=function()
        {
            // Create Map
            var map = L.map( 'map' ).setView([51.505, -0.09], 13);

            // Pull from the OSM API
            var osmURL = ""http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"";

            // In order to use open street map, we need to attribute to it.
            var osmAttrib = 'Map Data &copy; <a href=""http://openstreetmap.org/copyright"">OpenStreetMap</a> contributors';

            // Create the OSM layer.
            var osm = new L.TileLayer( osmURL, { minZoom: 0, maxZoom: 18, attribution: osmAttrib});

            // Set the map to start at RIT at zoom level 15.
            map.setView(new L.LatLng(43.085, -77.678419), 3);

            // Add the osm layer to the map
            map.addLayer(osm);

            // Create the icon.
            var icon = L.icon({
                iconUrl: """ + exeDirectory + @"/html/media/marker-icon.png"",
                iconSize: [25, 41],
                iconAnchor:[12.5, 41],
                popupAnchor:[0, -30]
            });

            // Insert the data.
        ";

        const string mapHtmlEnd = @"
        }
    </script>
</head>
<body>
    <div class=""center"" id=""map"" style=""height:350px;""/>
</body>
</html>";

        /// <summary>
        /// Gets the html of all the log's locations.
        /// </summary>
        /// <returns></returns>
        private string GetPositionHtml()
        {
            string js = string.Empty;
            foreach ( ILog log in this.api.LogBook.Logs )
            {
                if ( ( log.Latitude == null ) || ( log.Longitude == null ) )
                {
                    continue;
                }

                js += @"
var markerHTML" + log.Id + @" = '<div class = ""left"" style=""overflow: auto; color: black; "">' + 
                                '<p><strong>" + log.StartTime.ToLocalTime().ToString( "MM-dd-yyyy HH:mm" ) + @"</strong></p>' + 
                                '<p><strong>Duration:</strong> " + log.Duration.TotalMinutes.ToString( "F", CultureInfo.InvariantCulture ) + @" minutes</p>' + 
                                '<p><strong>Technique:</strong> " + log.Technique + @"</p>' +
                                '<p><strong>Comments:</strong> " + log.Comments + @"</p>';

                var newPopup" + log.Id + @" = L.popup({maxwidth:500}).setContent(markerHTML" + log.Id + @");
var newMarker" + log.Id + @" = L.marker([" + log.Latitude + ", " + log.Longitude + @"]).setIcon(icon).addTo(map).bindPopup(newPopup" + log.Id + @");
";
            }

            return js;
        }

        private void ReloadLogs()
        {
            this.api.PopulateLogbook();
            this.logViews = new List<LogView>();
            foreach ( ILog log in this.api.LogBook.Logs )
            {
                this.logViews.Add(
                    new LogView( log )
                );
            }

            this.MapViewBrowser.DocumentText = mapHtmlStart + GetPositionHtml() + mapHtmlEnd;
        }

        private void Form1_Load( object sender, EventArgs e )
        {
        }

        // -------- Sync View --------

        private void SyncLocationText_TextChanged( object sender, EventArgs e )
        {
            if (
                string.IsNullOrEmpty( SyncLocationText.Text ) ||
                string.IsNullOrWhiteSpace( SyncLocationText.Text )
            )
            {
                SyncButton.Enabled = false;
            }
            else
            {
                SyncButton.Enabled = true;
            }
        }

        private void SyncBrowseButton_Click( object sender, EventArgs e )
        {
            DialogResult openResult = SyncOpenDialog.ShowDialog();
            if ( openResult == DialogResult.OK )
            {
                SyncLocationText.Text = SyncOpenDialog.FileName;
            }
        }

        private void SyncButton_Click( object sender, EventArgs e )
        {
            if (
                string.IsNullOrEmpty( SyncLocationText.Text ) ||
                string.IsNullOrWhiteSpace( SyncLocationText.Text )
            )
            {
                MessageBox.Show( "No Sync file selected." );
            }
        }

        // -------- Import View --------

        private void ImportFileLocation_TextChanged( object sender, EventArgs e )
        {
            if (
                string.IsNullOrEmpty( ImportFileLocation.Text ) ||
                string.IsNullOrWhiteSpace( ImportFileLocation.Text )
            )
            {
                ImportButton.Enabled = false;
            }
            else
            {
                ImportButton.Enabled = true;
            }
        }

        private void ImportBrowseButton_Click( object sender, EventArgs e )
        {
            DialogResult openResult = ImportOpenDialog.ShowDialog();
            if ( openResult == DialogResult.OK )
            {
                ImportFileLocation.Text = ImportOpenDialog.FileName;
            }
        }

        private void ImportButton_Click( object sender, EventArgs e )
        {
            if (
                string.IsNullOrEmpty( ImportFileLocation.Text ) ||
                string.IsNullOrWhiteSpace( ImportFileLocation.Text )
            )
            {
                MessageBox.Show( "No import file selected." );
            }
            else
            {
                try
                {
                    this.api.Import( ImportFileLocation.Text );
                    ReloadLogs();
                    this.ImportFileLocation.Text = string.Empty;
                }
                catch ( Exception err )
                {
                    MessageBox.Show(
                        err.Message,
                        "Error importing to logbook.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        // -------- Export View --------

        private void ExportLocationText_TextChanged( object sender, EventArgs e )
        {
            if (
                string.IsNullOrEmpty( ExportLocationText.Text ) ||
                string.IsNullOrWhiteSpace( ExportLocationText.Text )
            )
            {
                ExportButton.Enabled = false;
            }
            else
            {
                ExportButton.Enabled = true;
            }
        }

        private void ExportBrowseButton_Click( object sender, EventArgs e )
        {
            DialogResult saveResult = ExportSaveDialog.ShowDialog();
            if ( saveResult == DialogResult.OK )
            {
                ExportLocationText.Text = ExportSaveDialog.FileName;
            }
        }

        private void ExportButton_Click( object sender, EventArgs e )
        {
            if (
                string.IsNullOrEmpty( ExportLocationText.Text ) ||
                string.IsNullOrWhiteSpace( ExportLocationText.Text )
            )
            {
                MessageBox.Show( "No export file selected" );
            }
            else
            {
                try
                {
                    this.api.Export( ExportLocationText.Text );
                    this.ExportLocationText.Text = string.Empty;
                }
                catch ( Exception err )
                {
                    MessageBox.Show( 
                        err.Message,
                        "Error exporting logbook.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        // -------- View Logbook Tab --------

        private void LogbookView_Enter( object sender, EventArgs e )
        {
            if ( this.logViews.Count == 0 )
            {

            }
            else
            {
                this.StandardLogView.Controls.Clear();
                foreach ( LogView view in this.logViews )
                {
                    view.Width = this.StandardLogView.Width;
                    this.StandardLogView.Controls.Add( view );
                }
            }
        }

        // -------- Start Tab --------

        private void StartTab_Enter( object sender, EventArgs e )
        {
            switch ( this.currentState )
            {
                case StartState.Idle:
                    this.optionView.Visible = true;
                    this.meditateView.Visible = false;
                    this.saveView.Visible = false;
                    break;
                case StartState.Start:
                    this.optionView.Visible = false;
                    this.meditateView.Visible = true;
                    this.saveView.Visible = false;
                    break;
                case StartState.End:
                    this.optionView.Visible = false;
                    this.meditateView.Visible = false;
                    this.saveView.Visible = true;
                    break;
            }
        }

        private void StartButton_Click( object sender, EventArgs e )
        {
            GoToNextState();
        }

        private void GoToNextState()
        {
            try
            {
                switch ( this.currentState )
                {
                    case StartState.Idle:
                        // API calls
                        if ( this.optionView.EnableTimerCheckbox.Checked )
                        {
                            TimeSpan timeToGo = new TimeSpan(
                                int.Parse( this.optionView.HourListBox.SelectedItem.ToString() ),
                                int.Parse( this.optionView.MinuteListBox.SelectedItem.ToString() ),
                                0
                            );
                            this.api.StartSession( timeToGo );
                        }
                        else
                        {
                            this.api.StartSession( null );
                        }

                        // Switch View.
                        this.optionView.Visible = false;
                        this.meditateView.Visible = true;
                        this.saveView.Visible = false;
                        this.StartButton.Text = "Finish";

                        // Update State
                        this.currentState = StartState.Start;
                        break;
                    case StartState.Start:
                        // API Calls
                        this.api.StopSession();

                        // Switch View
                        this.optionView.Visible = false;
                        this.meditateView.Visible = false;
                        this.saveView.Visible = true;
                        this.saveView.MinutesValueLabel.Text =
                            this.api.CurrentLog.Duration.TotalMinutes.ToString( "F", CultureInfo.InvariantCulture );
                        this.StartButton.Text = "Save";

                        // Update State
                        this.currentState = StartState.End;
                        break;
                    case StartState.End:
                        // API Calls
                        decimal? latitude = null;
                        decimal? longitude = null;

                        if ( this.saveView.UseLocationCheckbox.Checked )
                        {
                            this.api.LocationDetector.RefreshPosition();
                            if ( this.api.LocationDetector.IsReady )
                            {
                                latitude = this.api.LocationDetector.Latitude;
                                longitude = this.api.LocationDetector.Longitude;
                            }
                            else
                            {
                                MessageBox.Show(
                                    "Could not get location. Try saving again, or uncheck the location box to save without location.",
                                    "Error getting location.",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                );

                                // Do not change states.
                                break;
                            }
                        }

                        this.api.ValidateAndSaveSession(
                            this.saveView.TechniqueUsedTextbox.Text,
                            this.saveView.CommentsTextBox.Text,
                            latitude,
                            longitude
                        );
                        ReloadLogs();

                        // Switch View
                        this.saveView.Visible = false;
                        this.optionView.Visible = true;
                        this.meditateView.Visible = false;
                        this.saveView.TechniqueUsedTextbox.Text = string.Empty;
                        this.saveView.CommentsTextBox.Text = string.Empty;
                        this.StartButton.Text = "Start";

                        // Update State.
                        this.currentState = StartState.Idle;
                        break;
                }
            }
            catch ( Exception err )
            {
                string errorStr = "Error when ";
                switch ( this.currentState )
                {
                    case StartState.Idle:
                        errorStr = errorStr + "starting the session.";
                        break;
                    case StartState.Start:
                        errorStr = errorStr + "ending the session.";
                        this.currentState = StartState.End;
                        break;
                    case StartState.End:
                        errorStr = errorStr + "saving the session.";
                        this.currentState = StartState.Idle;
                        break;
                }
                MessageBox.Show( err.Message, errorStr, MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        private void LogbookView_Click( object sender, EventArgs e )
        {

        }

        // --------- About View Events ---------

        private void VistSiteLabel_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            System.Diagnostics.Process.Start( "http://meditationenthusiasts.org" );
        }

        private void ReportABugValue_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            System.Diagnostics.Process.Start( "https://dev.meditationenthusiasts.org/mantis/" );
        }

        private void ViewSourceValueLabel_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            System.Diagnostics.Process.Start( "https://bitbucket.org/meditationenthusiasts/meditation-logs-desktop/src" );
        }

        private void tableLayoutPanel1_Paint( object sender, PaintEventArgs e )
        {

        }

        private void ViewWikiValueLabel_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            System.Diagnostics.Process.Start( "https://dev.meditationenthusiasts.org/dokuwiki/doku.php?id=mantis:meditation_logger:start" );
        }
    }
}
