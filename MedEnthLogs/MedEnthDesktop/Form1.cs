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

namespace MedEnthLogsDesktop
{
    public partial class HomePage : Form
    {
        MedEnthLogsApi.Api api;

        public HomePage( MedEnthLogsApi.Api api )
        {
            InitializeComponent();
            this.api = api;
            this.api.PopulateLogbook();
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
    }
}
