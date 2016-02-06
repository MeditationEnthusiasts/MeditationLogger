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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
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

        /// <summary>
        /// Reference to the API.
        /// </summary>
        private MedEnthLogsApi.Api api;

        List<LogView> logViews;

        /// <summary>
        /// Current state of the start panel.
        /// </summary>
        StartState currentState;

        /// <summary>
        /// Where the execution direcotry is.
        ///
        /// https://msdn.microsoft.com/en-us/library/aa457089.aspx
        /// </summary>
        private static readonly string exeDirectory = Path.GetDirectoryName( Application.ExecutablePath ).Replace( '\\', '/' );

        /// <summary>
        /// Original background color (for restoring).
        /// </summary>
        private Color originalColor;

        private IMusicManager timesUpSound;

        /// <summary>
        /// Whether or not there's a task in progress or not.
        /// </summary>
        private bool isNoTaskInProgress;

        // ---- Views ----

        OptionsView optionView;
        MeditateView meditateView;
        SaveView saveView;

        // ---- Delegates ----

        private Action<int, int> exportProgressBarDelegate;
        private Action<int, int> importProgressBarDelegate;
        private Action<int, int> syncProgressBarDelegate;

        /// <summary>
        /// Costructor
        /// </summary>
        /// <param name="api">The API to use.</param>
        /// <param name="timesUpMusicManager">The music manager to use when times up (just create it).</param>
        public HomePage( MedEnthLogsApi.Api api, IMusicManager timesUpMusicManager )
        {
            InitializeComponent();

            this.isNoTaskInProgress = true;

            this.GplTextBox.Text = MedEnthLogsApi.License.MedEnthLicense;
            this.ExternalLibTextBox.Text = MedEnthLogsApi.License.ExternalLicenses;
            this.VersionValueLabel.Text = Api.VersionString;

            this.logViews = new List<LogView>();

            this.timesUpSound = timesUpMusicManager;
            this.timesUpSound.OnStop =
                delegate ()
                {
                    this.timesUpSound.Stop();
                };

            this.api = api;
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
                        this.timesUpSound.Play( exeDirectory + "/media/temple_bell.wav" );
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

            // Add progress bar controls:
            this.ExportProgressBar.Minimum = 0;
            this.exportProgressBarDelegate = delegate( int step, int totalSteps )
            {
                if ( this.InvokeRequired )
                {
                    this.BeginInvoke( this.exportProgressBarDelegate, step, totalSteps );
                }
                else
                {
                    this.ExportProgressBar.Maximum = totalSteps;
                    this.ExportProgressBar.Value = step;
                }
            };

            this.ImportProgressBar.Minimum = 0;
            this.importProgressBarDelegate = delegate( int step, int totalSteps )
            {
                if ( this.InvokeRequired )
                {
                    this.BeginInvoke( this.importProgressBarDelegate, step, totalSteps );
                }
                else
                {
                    this.ImportProgressBar.Maximum = totalSteps;
                    this.ImportProgressBar.Value = step;
                }
            };

            this.SyncProgressBar.Minimum = 0;
            this.syncProgressBarDelegate = delegate( int step, int totalSteps )
            {
                if ( this.InvokeRequired )
                {
                    this.BeginInvoke( this.syncProgressBarDelegate, step, totalSteps );
                }
                else
                {
                    this.SyncProgressBar.Maximum = totalSteps;
                    this.SyncProgressBar.Value = step;
                }
            };
        }

        readonly string mapHtmlStart = @"
<!doctype html>
<head>
    <meta http-equiv=""content-type"" content=""text/html; charset=utf-8"" />
    <link type=""text/css"" rel=""stylesheet"" href=""" + exeDirectory + @"/html/css/leaflet.css""/>
    <title>Meditation Map</title>
    <style>
    " + LeafletJS.CSS + @"

        // Use to make the map full screen.
        body {
            padding: 0;
            margin: 0;
        }
        html, body, #map {
            height: 98%;
            width: 99%;
        }
    </style>
    <!-- Plug in the map information -->
    <script type = ""text/javascript"">
        " + LeafletJS.JavaScript + @"

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
    <div class=""center"" id=""map""/>
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

                // Replace new lines with spaces so the javascript doesn't get broken.
                string commentString = log.Comments.Replace( "\n", @"  " );

                js += @"
var markerHTML" + log.Id + @" = '<div class = ""left"" style=""overflow: auto; color: black; "">' + 
                                '<p><strong>" + log.StartTime.ToLocalTime().ToString( "MM-dd-yyyy HH:mm" ) + @"</strong></p>' + 
                                '<p><strong>Duration:</strong> " + log.Duration.TotalMinutes.ToString( "F", CultureInfo.InvariantCulture ) + @" minutes</p>' + 
                                '<p><strong>Technique:</strong> " + log.Technique + @"</p>' +
                                '<p><strong>Comments:</strong> " + commentString + @"</p>';

                var newPopup" + log.Id + @" = L.popup({maxwidth:500}).setContent(markerHTML" + log.Id + @");
var newMarker" + log.Id + @" = L.marker([" + log.Latitude + ", " + log.Longitude + @"]).setIcon(icon).addTo(map).bindPopup(newPopup" + log.Id + @");
";
            }

            return js;
        }

        /// <summary>
        /// Reloads the logs from memory in a different thread.
        /// </summary>
        /// <returns></returns>
        private Task ReloadLogsAsync()
        {
            return Task.Run(
                delegate()
                {
                    ReloadLogs();
                }
            );
        }

        /// <summary>
        /// Reloads the logs from memory and renders them to the UI.
        /// </summary>
        private void ReloadLogs()
        {
            UpdateUiForReloadingLogs();
            this.api.PopulateLogbook();
            UpdateUiAfterLoadingLogs();
        }

        /// <summary>
        /// Updates the UI prior to reloading logs.
        /// </summary>
        private void UpdateUiForReloadingLogs()
        {
            if ( this.InvokeRequired )
            {
                this.BeginInvoke( new Action( this.UpdateUiForReloadingLogs ) );
            }
            else
            {
                this.logViews.Clear();
                this.TotalMinutesValueLabel.Text = "Loading...";
                this.LongestSessionValueLabel.Text = "Loading...";
                this.TotalSessionsValueLabel.Text = "Loading...";
                this.LastSessionValueLabel.Text = "Loading...";
            }
        }

        /// <summary>
        /// Updates the UI after reloading logs.
        /// </summary>
        private void UpdateUiAfterLoadingLogs()
        {
            if ( this.InvokeRequired )
            {
                this.BeginInvoke( new Action( this.UpdateUiAfterLoadingLogs ) );
            }
            else
            {
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
        }

        private async void Form1_Load( object sender, EventArgs e )
        {
            await ReloadLogsAsync();
        }

        // -------- Sync View --------

        private void SyncLocationText_TextChanged( object sender, EventArgs e )
        {
            EnableSyncButton();
        }

        private void SyncBrowseButton_Click( object sender, EventArgs e )
        {
            DialogResult openResult = SyncOpenDialog.ShowDialog();
            if ( openResult == DialogResult.OK )
            {
                SyncLocationText.Text = SyncOpenDialog.FileName;
            }
        }

        private void SyncLocationText_KeyPress( object sender, KeyPressEventArgs e )
        {
            if ( e.KeyChar == (char)Keys.Enter )
            {
                if (
                    string.IsNullOrEmpty( SyncLocationText.Text ) ||
                    string.IsNullOrWhiteSpace( SyncLocationText.Text )
                )
                {
                    // No-op if text field is empty.
                    return;
                }
                HandleSyncEvent();
            }
        }

        private void SyncButton_Click( object sender, EventArgs e )
        {
            HandleSyncEvent();
        }

        /// <summary>
        /// Handles an event when the user triggers a sync.
        /// </summary>
        private async void HandleSyncEvent()
        {
            // No-op if things things are being written to.
            if ( isNoTaskInProgress == false )
            {
                return;
            }
            else if (
                string.IsNullOrEmpty( SyncLocationText.Text ) ||
                string.IsNullOrWhiteSpace( SyncLocationText.Text )
            )
            {
                MessageBox.Show( "No Sync file selected." );
            }
            else
            {
                this.EnableButtons( false );
                this.SyncButton.Text = "Syncing...";
                try
                {
                    await DoSync();
                    this.SyncProgressBar.Value = this.SyncProgressBar.Maximum;

                    // Show success.
                    MessageBox.Show(
                        "Logbook Synced Successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Asterisk
                    );

                    this.SyncLocationText.Text = string.Empty;
                }
                catch ( Exception err )
                {
                    // Show failure.
                    MessageBox.Show(
                        err.Message,
                        "Error when syncing to logbook.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }

                this.SyncProgressBar.Value = this.SyncProgressBar.Minimum;
                this.SyncButton.Text = "Sync";
                this.EnableButtons( true );
            }
        }

        private Task DoSync()
        {
            return Task.Run(
                delegate ()
                {
                    this.api.Sync( SyncLocationText.Text, this.syncProgressBarDelegate );
                    ReloadLogs();
                }
            );
        }

        // -------- Import View --------

        private void ImportFileLocation_TextChanged( object sender, EventArgs e )
        {
            EnableImportButton();
        }

        private void ImportBrowseButton_Click( object sender, EventArgs e )
        {
            DialogResult openResult = ImportOpenDialog.ShowDialog();
            if ( openResult == DialogResult.OK )
            {
                ImportFileLocation.Text = ImportOpenDialog.FileName;
            }
        }

        private void ImportFileLocation_KeyPress( object sender, KeyPressEventArgs e )
        {
            if ( e.KeyChar == ( char ) Keys.Enter )
            {
                if (
                    string.IsNullOrEmpty( ImportFileLocation.Text ) ||
                    string.IsNullOrWhiteSpace( ImportFileLocation.Text )
                )
                {
                    // No-Op if there's nothing in the text field.
                    return;
                }

                HandleImportEvent();
            }
        }

        private void ImportButton_Click( object sender, EventArgs e )
        {
            HandleImportEvent();
        }

        /// <summary>
        /// Handles an event when the user triggers an import.
        /// </summary>
        private async void HandleImportEvent()
        {
            // No-op if things things are being written to.
            if ( isNoTaskInProgress == false )
            {
                return;
            }
            else if (
                string.IsNullOrEmpty( ImportFileLocation.Text ) ||
                string.IsNullOrWhiteSpace( ImportFileLocation.Text )
            )
            {
                MessageBox.Show( "No import file selected." );
            }
            else
            {
                this.EnableButtons( false );
                this.ImportButton.Text = "Importing...";
                try
                {
                    await DoImport();
                    this.ImportProgressBar.Value = this.ImportProgressBar.Maximum;

                    MessageBox.Show(
                        "File imported to logbook successfully!",
                        "Success.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Asterisk
                    );

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

                this.ImportProgressBar.Value = this.ImportProgressBar.Minimum;
                this.ImportButton.Text = "Import";
                this.EnableButtons( true );
            }
        }

        /// <summary>
        /// Does the import.
        /// </summary>
        /// <returns></returns>
        private Task DoImport()
        {
            return Task.Run(
                delegate ()
                {
                    this.api.Import( ImportFileLocation.Text, importProgressBarDelegate );
                    ReloadLogs();
                }
            );
        }

        // -------- Export View --------

        private void ExportLocationText_TextChanged( object sender, EventArgs e )
        {
            EnableExportButton();
        }

        private void ExportLocationText_KeyPress( object sender, KeyPressEventArgs e )
        {
            if ( e.KeyChar == (char) Keys.Return )
            {
                if (
                     string.IsNullOrEmpty( ExportLocationText.Text ) ||
                     string.IsNullOrWhiteSpace( ExportLocationText.Text )
                )
                {
                    // No-op if there's nothing in the export text field.
                    return;
                }

                HandleExportEvent();
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
            HandleExportEvent();
        }

        /// <summary>
        /// Handles the user triggers an event that causes an export.
        /// </summary>
        private async void HandleExportEvent()
        {
            // No-op if things things are being written to.
            if ( isNoTaskInProgress == false )
            {
                return;
            }
            else if (
                 string.IsNullOrEmpty( ExportLocationText.Text ) ||
                 string.IsNullOrWhiteSpace( ExportLocationText.Text )
            )
            {
                MessageBox.Show( "No export file selected" );
            }
            else
            {
                this.EnableButtons( false );
                this.ExportButton.Text = "Exporting...";
                try
                {
                    await DoExport();
                    this.ExportProgressBar.Value = this.ExportProgressBar.Maximum;

                    MessageBox.Show(
                        "Logbook exported to file successfully!",
                        "Success.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Asterisk
                    );

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
                    this.ExportProgressBar.Value = this.ExportProgressBar.Minimum;
                }

                this.ExportProgressBar.Value = this.ExportProgressBar.Minimum;
                this.ExportButton.Text = "Export";
                this.EnableButtons( true );
            }
        }

        /// <summary>
        /// Does the export in a background thread.
        /// </summary>
        /// <returns>The task to await on.</returns>
        private Task DoExport()
        {
            return Task.Run(
                delegate ()
                {
                    this.api.Export( ExportLocationText.Text, this.exportProgressBarDelegate );
                }
            );
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

        private async void GoToNextState()
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
                            int minutes = Convert.ToInt32( this.optionView.MinuteListBox.Value );
                            int hours = Convert.ToInt32( this.optionView.HourListBox.Value );

                            // Do not start the session if we are in a non-guided timed session and our timer is set to 00:00.
                            if ( ( minutes <= 0 ) && ( hours <= 0 ) && ( this.optionView.MusicPlayOnceRadioButton.Checked == false ) )
                            {
                                MessageBox.Show(
                                    "Timer must be longer than 00:00",
                                    "Error when starting.",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation
                                );

                                // Do not go to next state.
                                break;
                            }

                            TimeSpan timeToGo = new TimeSpan(
                                hours,
                                minutes,
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

                        try
                        {
                            this.StartButton.Text = "Saving...";
                            this.EnableButtons( false );
                            await DoSave( this.saveView.TechniqueUsedTextbox.Text, this.saveView.CommentsTextBox.Text, latitude, longitude );
                        }
                        finally
                        {
                            this.StartButton.Text = "Save";
                            this.EnableButtons( true );
                        }

                        // Switch View
                        this.saveView.Visible = false;
                        this.optionView.Visible = true;
                        this.meditateView.Visible = false;
                        this.saveView.TechniqueUsedTextbox.Text = string.Empty;
                        this.saveView.CommentsTextBox.Text = string.Empty;
                        this.StartButton.Text = "Start";

                        // Issue 26:  Set the location checkbox back to unchecked after saving.
                        // (location should be opt-in not opt-out).
                        this.saveView.UseLocationCheckbox.Checked = false;

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
                        break;
                }
                MessageBox.Show( err.Message, errorStr, MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        /// <summary>
        /// Saves the current log from the information on the save view.
        /// </summary>
        /// <param name="technique">Technique used.</param>
        /// <param name="comments">Comments addded.</param>
        /// <param name="latitude">The latitude to save.</param>
        /// <param name="longitude">The longitude to save.</param>
        private Task DoSave( string technique, string comments, decimal? latitude, decimal? longitude )
        {
            return Task.Run(
                delegate ()
                {
                    this.api.ValidateAndSaveSession(
                        technique,
                        comments,
                        latitude,
                        longitude
                    );
                    ReloadLogs();
                }
            );
        }

        private void LogbookView_Click( object sender, EventArgs e )
        {

        }

        // --------- About View Events ---------

        private void VistSiteLabel_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            System.Diagnostics.Process.Start( "https://meditationenthusiasts.org" );
        }

        private void ReportABugValue_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            System.Diagnostics.Process.Start( "https://meditationenthusiasts.org/development/mantis/" );
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
            System.Diagnostics.Process.Start( "https://meditationenthusiasts.org/development/dokuwiki/doku.php?id=mantis:meditation_logger:start" );
        }

        private async void CheckForUpdatesButton_Click( object sender, EventArgs e )
        {
            this.CheckForUpdatesButton.Enabled = false;
            string originalUpdateString = this.CheckForUpdatesButton.Text;
            this.CheckForUpdatesButton.Text = "Checking...";
            try
            {
                if ( await CheckForUpdate() )
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

            this.CheckForUpdatesButton.Text = originalUpdateString;
            this.CheckForUpdatesButton.Enabled = true;
        }

        // -------- Helper Functions --------

        /// <summary>
        /// Checks to see if there's an update to the app or not.
        /// </summary>
        /// <returns>True if there's an update, else false.</returns>
        private Task<bool> CheckForUpdate()
        {
            return Task.Run(
                delegate ()
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
            );
        }

        /// <summary>
        /// Enables all the buttons.
        /// </summary>
        /// <param name="enable">Whether or not to enable the buttons.</param>
        private void EnableButtons( bool enable )
        {
            if ( this.InvokeRequired )
            {
                this.BeginInvoke( new Action<bool>( EnableButtons ), enable );
            }
            else
            {
                this.isNoTaskInProgress = enable;
                EnableImportButton();
                EnableExportButton();
                EnableSyncButton();
                this.StartButton.Enabled = enable;
            }
        }

        /// <summary>
        /// Enables the import button if it should be enabled.
        /// </summary>
        private void EnableImportButton()
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
                ImportButton.Enabled = true && this.isNoTaskInProgress;
            }
        }

        /// <summary>
        /// Enables the export button if it should be enabled.
        /// </summary>
        private void EnableExportButton()
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
                ExportButton.Enabled = true && this.isNoTaskInProgress;
            }
        }

        /// <summary>
        /// Enables the sync button if it should be enabled.
        /// </summary>
        private void EnableSyncButton()
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
                SyncButton.Enabled = true && isNoTaskInProgress;
            }
        }
    }
}
