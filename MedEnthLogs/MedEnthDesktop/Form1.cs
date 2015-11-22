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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using MedEnthDesktop;
using MedEnthDesktop.Properties;
using MedEnthLogsApi;
using SethCS.Basic;

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

        /// <summary>
        /// Original background color (for restoring).
        /// </summary>
        private Color originalColor;

        private NAudioMusicManager timesUpSound;

        // ---- Views ----

        OptionsView optionView;
        MeditateView meditateView;
        SaveView saveView;

        public HomePage( MedEnthLogsApi.Api api )
        {
            InitializeComponent();

            this.GplTextBox.Text = MedEnthLogsApi.License.MedEnthLicense;
            this.ExternalLibTextBox.Text = MedEnthLogsApi.License.ExternalLicenses;
            this.VersionValueLabel.Text = Api.VersionString;

            this.timesUpSound = new NAudioMusicManager();
            this.timesUpSound.OnStop =
                delegate ()
                {
                    this.timesUpSound.Stop();
                };

            this.api = api;
            ReloadLogs();
            this.currentState = StartState.Idle;

            // Setup Option View
            this.optionView = new OptionsView();
            this.optionView.Visible = true;
            this.optionView.Dock = DockStyle.Fill;
            ChangableStartView.Controls.Add( this.optionView );

            // Setup meditate view
            this.meditateView = new MeditateView();
            this.meditateView.Visible = false;
            this.meditateView.Dock = DockStyle.Fill;
            ChangableStartView.Controls.Add( this.meditateView );

            this.originalColor = this.meditateView.BackColor;

            this.api.timer.OnComplete =
                delegate ()
                {
                    if ( this.currentState == StartState.Start )
                    {
                        this.timesUpSound.Play( "media/temple_bell.wav" );
                        GoToNextState();
                    }
                };

            this.api.timer.OnUpdate =
                delegate ( string updateString )
                {
                    this.meditateView.TimerLabel.Text = updateString;
                };

            this.saveView = new SaveView();
            this.saveView.Visible = false;
            this.saveView.Dock = DockStyle.Fill;
            ChangableStartView.Controls.Add( this.saveView );
        }

        readonly string mapHtmlStart = @"
<!doctype html>
<head>
    <meta http-equiv=""content-type"" content=""text/html; charset=utf-8"" />
    <link type=""text/css"" rel=""stylesheet"" href=""" + exeDirectory + @"/html/css/leaflet.css""/>
    <title>Meditation Map</title>
    <style>
    " + Resources.leaflet_css + @"
    </style>
    <!-- Plug in the map information -->
    <script type = ""text/javascript"">
        " + Resources.leaflet_js + @"

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
                iconUrl: """ + exeDirectory + @"/media/marker-icon.png"",
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

        /// <summary>
        /// Reloads the logs from memory and renders them to the UI.
        /// </summary>
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
            this.TotalMinutesValueLabel.Text = this.api.LogBook.TotalTime.ToString( "F", CultureInfo.InvariantCulture ) + " Minutes.";
            this.LongestSessionValueLabel.Text = this.api.LogBook.LongestTime.ToString( "F", CultureInfo.InvariantCulture ) + " Minutes.";
            this.TotalSessionsValueLabel.Text = this.api.LogBook.Logs.Count.ToString() + " Sessions.";
            if ( this.api.LogBook.Logs.Count == 0 )
            {
                this.LastSessionValueLabel.Text = "Nothing yet.";
            }
            else
            {
                this.LastSessionValueLabel.Text = this.api.LogBook.Logs[0].StartTime.ToLocalTime().ToString( "MM-dd-yyyy  HH:mm" );
            }
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
            else
            {
                try
                {
                    this.api.Sync( SyncLocationText.Text );
                    ReloadLogs();
                    this.SyncLocationText.Text = string.Empty;
                }
                catch ( Exception err )
                {
                    MessageBox.Show(
                        err.Message,
                        "Error when syncing to logbook.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
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
                // No-op
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
                        SessionConfig config = new SessionConfig();
                        if ( this.optionView.EnableTimerCheckbox.Checked )
                        {
                            TimeSpan timeToGo = new TimeSpan(
                                Convert.ToInt32( this.optionView.HourListBox.Value ),
                                Convert.ToInt32( this.optionView.MinuteListBox.Value ),
                                0
                            );
                            config.Length = timeToGo;
                        }
                        else
                        {
                            config.Length = null;
                        }

                        config.PlayMusic = ( this.optionView.LoopMusicRadioButton.Checked ) || 
                                           ( this.optionView.MusicPlayOnceRadioButton.Checked );
                        config.LoopMusic = this.optionView.LoopMusicRadioButton.Checked;
                        config.AudioFile = this.optionView.MusicLocationTextBox.Text;

                        // Will throw exception (and not start the timers) if validation fails.
                        this.api.StartSession( config );

                        // Switch View.
                        this.optionView.Visible = false;
                        this.meditateView.UpdateBackground();
                        this.meditateView.Visible = true;
                        this.StartTableLayout.BackColor = this.meditateView.BackColor;
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

                        this.StartTableLayout.BackColor = originalColor;

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

                        this.timesUpSound.Stop();

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

        private void CheckForUpdatesButton_Click( object sender, EventArgs e )
        {
            try
            {
                if ( CheckForUpdate() )
                {
                    DialogResult result = MessageBox.Show(
                        "There's a new version.  Open browser to download?",
                        "An update is available.",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if ( result == DialogResult.Yes )
                    {
                        System.Diagnostics.Process.Start( "http://app.meditationenthusiasts.org/software/logger/latest/win32/" );
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Software is at the latest version.",
                        "Already up to date.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch ( Exception err )
            {
                MessageBox.Show(
                    err.Message,
                    "Error when checking for updates",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Checks to see if there's an update to the app or not.
        /// </summary>
        /// <returns>True if there's an update, else false.</returns>
        private bool CheckForUpdate()
        {
            WebRequest request = WebRequest.Create( "http://app.meditationenthusiasts.org/software/logger/latest/version.txt" );
            request.Method = "GET";

            using ( HttpWebResponse response = request.GetResponse() as HttpWebResponse )
            {
                if ( response.StatusCode != HttpStatusCode.OK )
                {
                    throw new ApplicationException(
                        "http request returned invalid status: " + response.StatusCode
                    );
                }

                using ( StreamReader reader = new StreamReader( response.GetResponseStream() ) )
                {
                    string versionStr = reader.ReadToEnd();
                    versionStr = versionStr.Trim();

                    SemanticVersion version;
                    if ( SemanticVersion.TryParse( versionStr, out version ) )
                    {
                        // If version from the server if newer than this program's
                        // return true.
                        if ( version > Api.Version )
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
