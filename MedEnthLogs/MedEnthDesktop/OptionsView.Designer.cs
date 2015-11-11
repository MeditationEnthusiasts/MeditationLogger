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

namespace MedEnthDesktop
{
    partial class OptionsView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.EnableTimerCheckbox = new System.Windows.Forms.CheckBox();
            this.OptionsToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.NoMusicRadioButton = new System.Windows.Forms.RadioButton();
            this.LoopMusicRadioButton = new System.Windows.Forms.RadioButton();
            this.MusicPlayOnceRadioButton = new System.Windows.Forms.RadioButton();
            this.ColonLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.MusicOptionsGroup = new System.Windows.Forms.GroupBox();
            this.MusicBrowseButton = new System.Windows.Forms.Button();
            this.MusicLocationLabel = new System.Windows.Forms.Label();
            this.MusicLocationTextBox = new System.Windows.Forms.TextBox();
            this.TimerOptionsGroup = new System.Windows.Forms.GroupBox();
            this.MusicOpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.OptionDescriptionLabel = new System.Windows.Forms.Label();
            this.OptionsViewTable = new System.Windows.Forms.TableLayoutPanel();
            this.HourListBox = new System.Windows.Forms.NumericUpDown();
            this.MinuteListBox = new System.Windows.Forms.NumericUpDown();
            this.MusicOptionsGroup.SuspendLayout();
            this.TimerOptionsGroup.SuspendLayout();
            this.OptionsViewTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HourListBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinuteListBox)).BeginInit();
            this.SuspendLayout();
            // 
            // EnableTimerCheckbox
            // 
            this.EnableTimerCheckbox.AutoSize = true;
            this.EnableTimerCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnableTimerCheckbox.Location = new System.Drawing.Point(7, 28);
            this.EnableTimerCheckbox.Name = "EnableTimerCheckbox";
            this.EnableTimerCheckbox.Size = new System.Drawing.Size(121, 24);
            this.EnableTimerCheckbox.TabIndex = 0;
            this.EnableTimerCheckbox.Text = "Enable Timer";
            this.OptionsToolTip.SetToolTip(this.EnableTimerCheckbox, "Enable a countdown timer for the  session.\r\nSession will end when the timer runs " +
        "out. \r\nLeave unchecked for unlimited time.");
            this.EnableTimerCheckbox.UseVisualStyleBackColor = true;
            this.EnableTimerCheckbox.CheckedChanged += new System.EventHandler(this.EnableTimerCheckbox_CheckedChanged);
            // 
            // OptionsToolTip
            // 
            this.OptionsToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // NoMusicRadioButton
            // 
            this.NoMusicRadioButton.AutoSize = true;
            this.NoMusicRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NoMusicRadioButton.Location = new System.Drawing.Point(13, 34);
            this.NoMusicRadioButton.Margin = new System.Windows.Forms.Padding(10);
            this.NoMusicRadioButton.Name = "NoMusicRadioButton";
            this.NoMusicRadioButton.Size = new System.Drawing.Size(82, 20);
            this.NoMusicRadioButton.TabIndex = 0;
            this.NoMusicRadioButton.TabStop = true;
            this.NoMusicRadioButton.Text = "No Music";
            this.OptionsToolTip.SetToolTip(this.NoMusicRadioButton, "No music is played.");
            this.NoMusicRadioButton.UseVisualStyleBackColor = true;
            this.NoMusicRadioButton.CheckedChanged += new System.EventHandler(this.NoMusicRadioButton_CheckedChanged);
            // 
            // LoopMusicRadioButton
            // 
            this.LoopMusicRadioButton.AutoSize = true;
            this.LoopMusicRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoopMusicRadioButton.Location = new System.Drawing.Point(13, 71);
            this.LoopMusicRadioButton.Margin = new System.Windows.Forms.Padding(10);
            this.LoopMusicRadioButton.Name = "LoopMusicRadioButton";
            this.LoopMusicRadioButton.Size = new System.Drawing.Size(95, 20);
            this.LoopMusicRadioButton.TabIndex = 1;
            this.LoopMusicRadioButton.TabStop = true;
            this.LoopMusicRadioButton.Text = "Loop Music";
            this.OptionsToolTip.SetToolTip(this.LoopMusicRadioButton, "Music is played in a loop until the session ends.");
            this.LoopMusicRadioButton.UseVisualStyleBackColor = true;
            // 
            // MusicPlayOnceRadioButton
            // 
            this.MusicPlayOnceRadioButton.AutoSize = true;
            this.MusicPlayOnceRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MusicPlayOnceRadioButton.Location = new System.Drawing.Point(13, 108);
            this.MusicPlayOnceRadioButton.Margin = new System.Windows.Forms.Padding(10);
            this.MusicPlayOnceRadioButton.Name = "MusicPlayOnceRadioButton";
            this.MusicPlayOnceRadioButton.Size = new System.Drawing.Size(88, 20);
            this.MusicPlayOnceRadioButton.TabIndex = 2;
            this.MusicPlayOnceRadioButton.TabStop = true;
            this.MusicPlayOnceRadioButton.Text = "Play Once";
            this.OptionsToolTip.SetToolTip(this.MusicPlayOnceRadioButton, "Music is played once.  When the music stops, so does the session.");
            this.MusicPlayOnceRadioButton.UseVisualStyleBackColor = true;
            this.MusicPlayOnceRadioButton.CheckedChanged += new System.EventHandler(this.MusicPlayOnceRadioButton_CheckedChanged);
            // 
            // ColonLabel
            // 
            this.ColonLabel.AutoSize = true;
            this.ColonLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ColonLabel.Location = new System.Drawing.Point(75, 58);
            this.ColonLabel.Name = "ColonLabel";
            this.ColonLabel.Size = new System.Drawing.Size(13, 18);
            this.ColonLabel.TabIndex = 3;
            this.ColonLabel.Text = ":";
            this.ColonLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "Hour";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(91, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "Minute";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MusicOptionsGroup
            // 
            this.MusicOptionsGroup.Controls.Add(this.MusicBrowseButton);
            this.MusicOptionsGroup.Controls.Add(this.MusicLocationLabel);
            this.MusicOptionsGroup.Controls.Add(this.MusicLocationTextBox);
            this.MusicOptionsGroup.Controls.Add(this.MusicPlayOnceRadioButton);
            this.MusicOptionsGroup.Controls.Add(this.LoopMusicRadioButton);
            this.MusicOptionsGroup.Controls.Add(this.NoMusicRadioButton);
            this.MusicOptionsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MusicOptionsGroup.Location = new System.Drawing.Point(259, 10);
            this.MusicOptionsGroup.Margin = new System.Windows.Forms.Padding(10);
            this.MusicOptionsGroup.Name = "MusicOptionsGroup";
            this.MusicOptionsGroup.Size = new System.Drawing.Size(337, 241);
            this.MusicOptionsGroup.TabIndex = 6;
            this.MusicOptionsGroup.TabStop = false;
            this.MusicOptionsGroup.Text = "Music Options";
            // 
            // MusicBrowseButton
            // 
            this.MusicBrowseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MusicBrowseButton.Location = new System.Drawing.Point(205, 181);
            this.MusicBrowseButton.Margin = new System.Windows.Forms.Padding(10);
            this.MusicBrowseButton.Name = "MusicBrowseButton";
            this.MusicBrowseButton.Size = new System.Drawing.Size(80, 32);
            this.MusicBrowseButton.TabIndex = 5;
            this.MusicBrowseButton.Text = "Browse...";
            this.MusicBrowseButton.UseVisualStyleBackColor = true;
            this.MusicBrowseButton.Click += new System.EventHandler(this.MusicBrowseButton_Click);
            // 
            // MusicLocationLabel
            // 
            this.MusicLocationLabel.AutoSize = true;
            this.MusicLocationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MusicLocationLabel.Location = new System.Drawing.Point(13, 149);
            this.MusicLocationLabel.Margin = new System.Windows.Forms.Padding(10);
            this.MusicLocationLabel.Name = "MusicLocationLabel";
            this.MusicLocationLabel.Size = new System.Drawing.Size(100, 16);
            this.MusicLocationLabel.TabIndex = 4;
            this.MusicLocationLabel.Text = "Music Location:";
            // 
            // MusicLocationTextBox
            // 
            this.MusicLocationTextBox.Location = new System.Drawing.Point(13, 182);
            this.MusicLocationTextBox.Margin = new System.Windows.Forms.Padding(10);
            this.MusicLocationTextBox.Name = "MusicLocationTextBox";
            this.MusicLocationTextBox.Size = new System.Drawing.Size(172, 20);
            this.MusicLocationTextBox.TabIndex = 3;
            // 
            // TimerOptionsGroup
            // 
            this.TimerOptionsGroup.Controls.Add(this.MinuteListBox);
            this.TimerOptionsGroup.Controls.Add(this.HourListBox);
            this.TimerOptionsGroup.Controls.Add(this.EnableTimerCheckbox);
            this.TimerOptionsGroup.Controls.Add(this.label2);
            this.TimerOptionsGroup.Controls.Add(this.label1);
            this.TimerOptionsGroup.Controls.Add(this.ColonLabel);
            this.TimerOptionsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TimerOptionsGroup.Location = new System.Drawing.Point(10, 10);
            this.TimerOptionsGroup.Margin = new System.Windows.Forms.Padding(10);
            this.TimerOptionsGroup.Name = "TimerOptionsGroup";
            this.TimerOptionsGroup.Size = new System.Drawing.Size(229, 241);
            this.TimerOptionsGroup.TabIndex = 7;
            this.TimerOptionsGroup.TabStop = false;
            this.TimerOptionsGroup.Text = "Timer Options";
            // 
            // MusicOpenDialog
            // 
            this.MusicOpenDialog.Filter = "MP3 Files|*.mp3|Wave Files|*.wav";
            // 
            // OptionDescriptionLabel
            // 
            this.OptionDescriptionLabel.AutoSize = true;
            this.OptionDescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OptionDescriptionLabel.Location = new System.Drawing.Point(3, 261);
            this.OptionDescriptionLabel.Name = "OptionDescriptionLabel";
            this.OptionDescriptionLabel.Size = new System.Drawing.Size(171, 15);
            this.OptionDescriptionLabel.TabIndex = 9;
            this.OptionDescriptionLabel.Text = "These are what the options do";
            // 
            // OptionsViewTable
            // 
            this.OptionsViewTable.ColumnCount = 2;
            this.OptionsViewTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.25412F));
            this.OptionsViewTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.74588F));
            this.OptionsViewTable.Controls.Add(this.MusicOptionsGroup, 1, 0);
            this.OptionsViewTable.Controls.Add(this.OptionDescriptionLabel, 0, 1);
            this.OptionsViewTable.Controls.Add(this.TimerOptionsGroup, 0, 0);
            this.OptionsViewTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OptionsViewTable.Location = new System.Drawing.Point(0, 0);
            this.OptionsViewTable.Name = "OptionsViewTable";
            this.OptionsViewTable.RowCount = 2;
            this.OptionsViewTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.49746F));
            this.OptionsViewTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.50254F));
            this.OptionsViewTable.Size = new System.Drawing.Size(606, 394);
            this.OptionsViewTable.TabIndex = 10;
            // 
            // HourListBox
            // 
            this.HourListBox.Location = new System.Drawing.Point(17, 60);
            this.HourListBox.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.HourListBox.Name = "HourListBox";
            this.HourListBox.Size = new System.Drawing.Size(50, 20);
            this.HourListBox.TabIndex = 6;
            // 
            // MinuteListBox
            // 
            this.MinuteListBox.Location = new System.Drawing.Point(94, 60);
            this.MinuteListBox.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.MinuteListBox.Name = "MinuteListBox";
            this.MinuteListBox.Size = new System.Drawing.Size(50, 20);
            this.MinuteListBox.TabIndex = 7;
            // 
            // OptionsView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.Controls.Add(this.OptionsViewTable);
            this.Name = "OptionsView";
            this.Size = new System.Drawing.Size(606, 394);
            this.Load += new System.EventHandler(this.OptionsView_Load);
            this.Enter += new System.EventHandler(this.OptionsView_Enter);
            this.MusicOptionsGroup.ResumeLayout(false);
            this.MusicOptionsGroup.PerformLayout();
            this.TimerOptionsGroup.ResumeLayout(false);
            this.TimerOptionsGroup.PerformLayout();
            this.OptionsViewTable.ResumeLayout(false);
            this.OptionsViewTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HourListBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinuteListBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip OptionsToolTip;
        public System.Windows.Forms.CheckBox EnableTimerCheckbox;
        private System.Windows.Forms.Label ColonLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox MusicOptionsGroup;
        private System.Windows.Forms.Button MusicBrowseButton;
        private System.Windows.Forms.Label MusicLocationLabel;
        private System.Windows.Forms.GroupBox TimerOptionsGroup;
        private System.Windows.Forms.OpenFileDialog MusicOpenDialog;
        private System.Windows.Forms.Label OptionDescriptionLabel;
        public System.Windows.Forms.RadioButton NoMusicRadioButton;
        public System.Windows.Forms.RadioButton LoopMusicRadioButton;
        public System.Windows.Forms.RadioButton MusicPlayOnceRadioButton;
        public System.Windows.Forms.TextBox MusicLocationTextBox;
        private System.Windows.Forms.TableLayoutPanel OptionsViewTable;
        public System.Windows.Forms.NumericUpDown MinuteListBox;
        public System.Windows.Forms.NumericUpDown HourListBox;
    }
}
