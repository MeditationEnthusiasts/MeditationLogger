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
            this.HourListBox = new System.Windows.Forms.ListBox();
            this.MinuteListBox = new System.Windows.Forms.ListBox();
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.MusicOpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.OptionDescriptionLabel = new System.Windows.Forms.Label();
            this.MusicOptionsGroup.SuspendLayout();
            this.TimerOptionsGroup.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
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
            // HourListBox
            // 
            this.HourListBox.Enabled = false;
            this.HourListBox.FormattingEnabled = true;
            this.HourListBox.Items.AddRange(new object[] {
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24"});
            this.HourListBox.Location = new System.Drawing.Point(7, 58);
            this.HourListBox.Name = "HourListBox";
            this.HourListBox.Size = new System.Drawing.Size(62, 30);
            this.HourListBox.TabIndex = 1;
            this.OptionsToolTip.SetToolTip(this.HourListBox, "Hours to meditate for.\r\n");
            // 
            // MinuteListBox
            // 
            this.MinuteListBox.Enabled = false;
            this.MinuteListBox.FormattingEnabled = true;
            this.MinuteListBox.Items.AddRange(new object[] {
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50",
            "51",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59"});
            this.MinuteListBox.Location = new System.Drawing.Point(94, 58);
            this.MinuteListBox.Name = "MinuteListBox";
            this.MinuteListBox.Size = new System.Drawing.Size(62, 30);
            this.MinuteListBox.TabIndex = 2;
            this.OptionsToolTip.SetToolTip(this.MinuteListBox, "Minutes to meditate for.");
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
            this.OptionsToolTip.SetToolTip(this.MusicPlayOnceRadioButton, "Music is played in a loop until the session ends.");
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
            this.label1.Location = new System.Drawing.Point(14, 91);
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
            this.MusicOptionsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MusicOptionsGroup.Controls.Add(this.MusicBrowseButton);
            this.MusicOptionsGroup.Controls.Add(this.MusicLocationLabel);
            this.MusicOptionsGroup.Controls.Add(this.MusicLocationTextBox);
            this.MusicOptionsGroup.Controls.Add(this.MusicPlayOnceRadioButton);
            this.MusicOptionsGroup.Controls.Add(this.LoopMusicRadioButton);
            this.MusicOptionsGroup.Controls.Add(this.NoMusicRadioButton);
            this.MusicOptionsGroup.Location = new System.Drawing.Point(214, 10);
            this.MusicOptionsGroup.Margin = new System.Windows.Forms.Padding(10);
            this.MusicOptionsGroup.Name = "MusicOptionsGroup";
            this.MusicOptionsGroup.Size = new System.Drawing.Size(298, 226);
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
            this.TimerOptionsGroup.Controls.Add(this.EnableTimerCheckbox);
            this.TimerOptionsGroup.Controls.Add(this.label2);
            this.TimerOptionsGroup.Controls.Add(this.HourListBox);
            this.TimerOptionsGroup.Controls.Add(this.label1);
            this.TimerOptionsGroup.Controls.Add(this.ColonLabel);
            this.TimerOptionsGroup.Dock = System.Windows.Forms.DockStyle.Left;
            this.TimerOptionsGroup.Location = new System.Drawing.Point(10, 10);
            this.TimerOptionsGroup.Margin = new System.Windows.Forms.Padding(10);
            this.TimerOptionsGroup.Name = "TimerOptionsGroup";
            this.TimerOptionsGroup.Size = new System.Drawing.Size(184, 226);
            this.TimerOptionsGroup.TabIndex = 7;
            this.TimerOptionsGroup.TabStop = false;
            this.TimerOptionsGroup.Text = "Timer Options";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.TimerOptionsGroup);
            this.flowLayoutPanel1.Controls.Add(this.MusicOptionsGroup);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(600, 250);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // OptionDescriptionLabel
            // 
            this.OptionDescriptionLabel.AutoSize = true;
            this.OptionDescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OptionDescriptionLabel.Location = new System.Drawing.Point(13, 260);
            this.OptionDescriptionLabel.Name = "OptionDescriptionLabel";
            this.OptionDescriptionLabel.Size = new System.Drawing.Size(171, 15);
            this.OptionDescriptionLabel.TabIndex = 9;
            this.OptionDescriptionLabel.Text = "These are what the options do";
            // 
            // OptionsView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.Controls.Add(this.OptionDescriptionLabel);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "OptionsView";
            this.Size = new System.Drawing.Size(606, 381);
            this.Load += new System.EventHandler(this.OptionsView_Load);
            this.Enter += new System.EventHandler(this.OptionsView_Enter);
            this.MusicOptionsGroup.ResumeLayout(false);
            this.MusicOptionsGroup.PerformLayout();
            this.TimerOptionsGroup.ResumeLayout(false);
            this.TimerOptionsGroup.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolTip OptionsToolTip;
        public System.Windows.Forms.CheckBox EnableTimerCheckbox;
        private System.Windows.Forms.Label ColonLabel;
        public System.Windows.Forms.ListBox MinuteListBox;
        public System.Windows.Forms.ListBox HourListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox MusicOptionsGroup;
        private System.Windows.Forms.RadioButton MusicPlayOnceRadioButton;
        private System.Windows.Forms.RadioButton LoopMusicRadioButton;
        private System.Windows.Forms.RadioButton NoMusicRadioButton;
        private System.Windows.Forms.Button MusicBrowseButton;
        private System.Windows.Forms.Label MusicLocationLabel;
        private System.Windows.Forms.TextBox MusicLocationTextBox;
        private System.Windows.Forms.GroupBox TimerOptionsGroup;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.OpenFileDialog MusicOpenDialog;
        private System.Windows.Forms.Label OptionDescriptionLabel;
    }
}
