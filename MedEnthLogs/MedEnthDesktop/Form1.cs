using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
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
            this.meditateView = new MeditateView(
                delegate()
                {
                    if ( this.currentState == StartState.Start )
                    {
                        GoToNextState();
                    }
                }
            );
            this.meditateView.Visible = false;
            ChangableStartView.Controls.Add( this.meditateView );

            this.saveView = new SaveView();
            this.saveView.Visible = false;
            ChangableStartView.Controls.Add( this.saveView );
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
                    string[] fileNameParts = ExportLocationText.Text.Split( '.' );
                    if ( fileNameParts[fileNameParts.Length - 1].ToLower() == "xml" )
                    {
                        using ( FileStream outFile = new FileStream( ExportLocationText.Text, FileMode.OpenOrCreate, FileAccess.Write ) )
                        {
                            this.api.ExportToXml( outFile );
                            ExportLocationText.Text = string.Empty;
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Can only export .xml, .json, or .mlg.  Not " + fileNameParts[fileNameParts.Length - 1],
                            "Error exporting log",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
                catch ( Exception err )
                {
                    MessageBox.Show( 
                        err.Message,
                        "Error exporting log",
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
                this.ViewLogbookView.Controls.Clear();
                foreach ( LogView view in this.logViews )
                {
                    view.Width = this.ViewLogbookView.Width;
                    this.ViewLogbookView.Controls.Add( view );
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
                        this.api.StartSession();

                        if ( this.optionView.EnableTimerCheckbox.Checked )
                        {
                            TimeSpan timeToGo = new TimeSpan(
                                int.Parse( this.optionView.HourListBox.SelectedItem.ToString() ),
                                int.Parse( this.optionView.MinuteListBox.SelectedItem.ToString() ),
                                0
                            );
                            this.meditateView.StartTimer( timeToGo );
                        }
                        else
                        {
                            this.meditateView.StartTimer( null );
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
                        this.meditateView.StopAndResetTimer();

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
                        this.api.ValidateAndSaveSession(
                            this.saveView.TechniqueUsedTextbox.Text,
                            this.saveView.CommentsTextBox.Text
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
            System.Diagnostics.Process.Start( "https://bitbucket.org/meditationenthusiasts/meditation-logs-desktop/issues" );
        }

        private void ViewSourceValueLabel_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            System.Diagnostics.Process.Start( "https://bitbucket.org/meditationenthusiasts/meditation-logs-desktop/src" );
        }
    }
}
