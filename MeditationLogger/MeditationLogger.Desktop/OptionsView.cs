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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeditationEnthusiasts.MeditationLogger.Desktop
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
            if( result == DialogResult.OK )
            {
                this.MusicLocationTextBox.Text = MusicOpenDialog.FileName;
            }
        }

        private void MusicPlayOnceRadioButton_CheckedChanged( object sender, EventArgs e )
        {
            UpdateButtons();
        }

        private void NoMusicRadioButton_CheckedChanged( object sender, EventArgs e )
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            this.EnableTimerCheckbox.Enabled = ( MusicPlayOnceRadioButton.Checked == false );
            this.HourListBox.Enabled = this.EnableTimerCheckbox.Checked && ( MusicPlayOnceRadioButton.Checked == false );
            this.MinuteListBox.Enabled = this.EnableTimerCheckbox.Checked && ( MusicPlayOnceRadioButton.Checked == false );

            this.MusicLocationTextBox.Enabled = this.NoMusicRadioButton.Checked == false;
            this.MusicBrowseButton.Enabled = this.NoMusicRadioButton.Checked == false;

            if( NoMusicRadioButton.Checked || LoopMusicRadioButton.Checked )
            {
                if( this.EnableTimerCheckbox.Checked )
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
            else if( MusicPlayOnceRadioButton.Checked )
            {
                this.OptionDescriptionLabel.Text = "Meditate until the music runs out.  Good for guided meditations.";
            }
        }
    }
}