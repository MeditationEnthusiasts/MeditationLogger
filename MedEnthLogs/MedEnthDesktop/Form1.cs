using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        MedEnthLogsApi.Api api;

        List<LogView> logViews;

        StartState currentState;

        public HomePage( MedEnthLogsApi.Api api )
        {
            InitializeComponent();
            this.api = api;
            this.api.PopulateLogbook();
            this.logViews = new List<LogView>();
            foreach ( ILog log in this.api.LogBook.Logs )
            {
                this.logViews.Add(
                    new LogView( log )
                );
            }
            this.currentState = StartState.Idle;
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
                foreach ( LogView view in this.logViews )
                {
                    view.Width = this.ViewLogbookView.Width;
                    this.ViewLogbookView.Controls.Add( view );
                }
            }
        }

        // -------- Start Tab --------

        private void StartButton_Click( object sender, EventArgs e )
        {
            try
            {
                switch ( this.currentState )
                {
                    case StartState.Idle:
                        this.api.StartSession();
                        this.StartButton.Text = "Finish";
                        this.currentState = StartState.Start;
                        break;
                    case StartState.Start:
                        this.api.StopSession();
                        this.StartButton.Text = "Save";
                        this.currentState = StartState.End;
                        break;
                    case StartState.End:
                        this.api.ValidateAndSaveSession( "Mindfullness", "This is a comment" );
                        this.api.PopulateLogbook();
                        this.StartButton.Text = "Start";
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
    }
}
