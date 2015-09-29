using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MedEnthDesktop
{
    public partial class OptionsView : UserControl
    {
        public OptionsView()
        {
            InitializeComponent();
        }

        private void dateTimePicker1_ValueChanged( object sender, EventArgs e )
        {

        }

        private void OptionsView_Load( object sender, EventArgs e )
        {
            this.NoMusicRadioButton.Checked = true;
            UpdateButtons();
        }

        private void OptionsView_Enter( object sender, EventArgs e )
        {
            UpdateButtons();
        }

        private void EnableTimerCheckbox_CheckedChanged( object sender, EventArgs e )
        {
            UpdateButtons();
        }

        private void MusicBrowseButton_Click( object sender, EventArgs e )
        {
            DialogResult result = MusicOpenDialog.ShowDialog();
            if ( result == DialogResult.OK )
            {
                this.MusicLocationTextBox.Text = MusicOpenDialog.FileName;
            }
        }

        private void MusicPlayOnceRadioButton_CheckedChanged( object sender, EventArgs e )
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            this.EnableTimerCheckbox.Enabled = ( MusicPlayOnceRadioButton.Checked == false );
            this.HourListBox.Enabled = this.EnableTimerCheckbox.Checked && ( MusicPlayOnceRadioButton.Checked == false );
            this.MinuteListBox.Enabled = this.EnableTimerCheckbox.Checked && ( MusicPlayOnceRadioButton.Checked == false );

            if ( NoMusicRadioButton.Checked || LoopMusicRadioButton.Checked )
            {
                if ( this.EnableTimerCheckbox.Checked )
                {
                    this.OptionDescriptionLabel.Text =
                        "Meditate until the timer runs out.  After the timer runs out, the session will end.";
                }
                else
                {
                    this.OptionDescriptionLabel.Text =
                        "Meditate as long as you want.  The timer counts up.";
                }
            }
            else if ( MusicPlayOnceRadioButton.Checked )
            {
                this.OptionDescriptionLabel.Text = "Meditate until the music runs out.  Good for guided meditations.";
            }
        }
    }
}
