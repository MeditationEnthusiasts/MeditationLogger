namespace MedEnthLogsDesktop
{
    partial class HomePage
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.Home = new System.Windows.Forms.TabPage();
            this.StartTab = new System.Windows.Forms.TabPage();
            this.ChangableStartView = new System.Windows.Forms.Panel();
            this.StartButton = new System.Windows.Forms.Button();
            this.LogbookView = new System.Windows.Forms.TabPage();
            this.ViewLogbookView = new System.Windows.Forms.FlowLayoutPanel();
            this.ManageLogbookView = new System.Windows.Forms.TabPage();
            this.ManageTabControl = new System.Windows.Forms.TabControl();
            this.EditLogView = new System.Windows.Forms.TabPage();
            this.ImportView = new System.Windows.Forms.TabPage();
            this.ImportButton = new System.Windows.Forms.Button();
            this.ImportBrowseButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ImportFileLocation = new System.Windows.Forms.TextBox();
            this.importLabel2 = new System.Windows.Forms.Label();
            this.ImportLabel1 = new System.Windows.Forms.Label();
            this.ExportView = new System.Windows.Forms.TabPage();
            this.ExportButton = new System.Windows.Forms.Button();
            this.ExportBrowseButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ExportLocationText = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SyncView = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SyncButton = new System.Windows.Forms.Button();
            this.SyncBrowseButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SyncLocationText = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.AboutView = new System.Windows.Forms.TabPage();
            this.ExportSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.ImportOpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.SyncOpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.VersionValueLabel = new System.Windows.Forms.Label();
            this.BugLabel = new System.Windows.Forms.Label();
            this.ReportABugValue = new System.Windows.Forms.LinkLabel();
            this.ViewSourceLabel = new System.Windows.Forms.Label();
            this.ViewSourceValueLabel = new System.Windows.Forms.LinkLabel();
            this.VisitSiteLabel = new System.Windows.Forms.Label();
            this.VistSiteLabel = new System.Windows.Forms.LinkLabel();
            this.MainTabControl.SuspendLayout();
            this.StartTab.SuspendLayout();
            this.LogbookView.SuspendLayout();
            this.ManageLogbookView.SuspendLayout();
            this.ManageTabControl.SuspendLayout();
            this.ImportView.SuspendLayout();
            this.ExportView.SuspendLayout();
            this.SyncView.SuspendLayout();
            this.AboutView.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.Home);
            this.MainTabControl.Controls.Add(this.StartTab);
            this.MainTabControl.Controls.Add(this.LogbookView);
            this.MainTabControl.Controls.Add(this.ManageLogbookView);
            this.MainTabControl.Controls.Add(this.AboutView);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainTabControl.Location = new System.Drawing.Point(0, 0);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.Padding = new System.Drawing.Point(0, 0);
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(609, 491);
            this.MainTabControl.TabIndex = 0;
            // 
            // Home
            // 
            this.Home.Location = new System.Drawing.Point(4, 34);
            this.Home.Name = "Home";
            this.Home.Padding = new System.Windows.Forms.Padding(3);
            this.Home.Size = new System.Drawing.Size(601, 453);
            this.Home.TabIndex = 0;
            this.Home.Text = "Home";
            this.Home.UseVisualStyleBackColor = true;
            // 
            // StartTab
            // 
            this.StartTab.Controls.Add(this.ChangableStartView);
            this.StartTab.Controls.Add(this.StartButton);
            this.StartTab.Location = new System.Drawing.Point(4, 34);
            this.StartTab.Name = "StartTab";
            this.StartTab.Padding = new System.Windows.Forms.Padding(3);
            this.StartTab.Size = new System.Drawing.Size(601, 453);
            this.StartTab.TabIndex = 4;
            this.StartTab.Text = "Start";
            this.StartTab.UseVisualStyleBackColor = true;
            this.StartTab.Enter += new System.EventHandler(this.StartTab_Enter);
            // 
            // ChangableStartView
            // 
            this.ChangableStartView.Location = new System.Drawing.Point(0, 0);
            this.ChangableStartView.Name = "ChangableStartView";
            this.ChangableStartView.Size = new System.Drawing.Size(601, 388);
            this.ChangableStartView.TabIndex = 3;
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(458, 394);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(140, 53);
            this.StartButton.TabIndex = 2;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // LogbookView
            // 
            this.LogbookView.Controls.Add(this.ViewLogbookView);
            this.LogbookView.Location = new System.Drawing.Point(4, 34);
            this.LogbookView.Name = "LogbookView";
            this.LogbookView.Padding = new System.Windows.Forms.Padding(3);
            this.LogbookView.Size = new System.Drawing.Size(601, 453);
            this.LogbookView.TabIndex = 1;
            this.LogbookView.Text = "View Logbook";
            this.LogbookView.UseVisualStyleBackColor = true;
            this.LogbookView.Click += new System.EventHandler(this.LogbookView_Click);
            this.LogbookView.Enter += new System.EventHandler(this.LogbookView_Enter);
            // 
            // ViewLogbookView
            // 
            this.ViewLogbookView.AutoScroll = true;
            this.ViewLogbookView.AutoSize = true;
            this.ViewLogbookView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ViewLogbookView.Location = new System.Drawing.Point(3, 3);
            this.ViewLogbookView.Margin = new System.Windows.Forms.Padding(5);
            this.ViewLogbookView.Name = "ViewLogbookView";
            this.ViewLogbookView.Size = new System.Drawing.Size(595, 447);
            this.ViewLogbookView.TabIndex = 0;
            // 
            // ManageLogbookView
            // 
            this.ManageLogbookView.Controls.Add(this.ManageTabControl);
            this.ManageLogbookView.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ManageLogbookView.Location = new System.Drawing.Point(4, 34);
            this.ManageLogbookView.Name = "ManageLogbookView";
            this.ManageLogbookView.Padding = new System.Windows.Forms.Padding(3);
            this.ManageLogbookView.Size = new System.Drawing.Size(601, 453);
            this.ManageLogbookView.TabIndex = 2;
            this.ManageLogbookView.Text = "Manage Logbook";
            this.ManageLogbookView.UseVisualStyleBackColor = true;
            // 
            // ManageTabControl
            // 
            this.ManageTabControl.Controls.Add(this.EditLogView);
            this.ManageTabControl.Controls.Add(this.ImportView);
            this.ManageTabControl.Controls.Add(this.ExportView);
            this.ManageTabControl.Controls.Add(this.SyncView);
            this.ManageTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ManageTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ManageTabControl.Location = new System.Drawing.Point(3, 3);
            this.ManageTabControl.Name = "ManageTabControl";
            this.ManageTabControl.SelectedIndex = 0;
            this.ManageTabControl.Size = new System.Drawing.Size(595, 447);
            this.ManageTabControl.TabIndex = 0;
            // 
            // EditLogView
            // 
            this.EditLogView.Location = new System.Drawing.Point(4, 29);
            this.EditLogView.Name = "EditLogView";
            this.EditLogView.Padding = new System.Windows.Forms.Padding(3);
            this.EditLogView.Size = new System.Drawing.Size(587, 414);
            this.EditLogView.TabIndex = 0;
            this.EditLogView.Text = "Edit Logs";
            this.EditLogView.UseVisualStyleBackColor = true;
            // 
            // ImportView
            // 
            this.ImportView.Controls.Add(this.ImportButton);
            this.ImportView.Controls.Add(this.ImportBrowseButton);
            this.ImportView.Controls.Add(this.label1);
            this.ImportView.Controls.Add(this.ImportFileLocation);
            this.ImportView.Controls.Add(this.importLabel2);
            this.ImportView.Controls.Add(this.ImportLabel1);
            this.ImportView.Location = new System.Drawing.Point(4, 29);
            this.ImportView.Name = "ImportView";
            this.ImportView.Padding = new System.Windows.Forms.Padding(3);
            this.ImportView.Size = new System.Drawing.Size(587, 414);
            this.ImportView.TabIndex = 1;
            this.ImportView.Text = "Import";
            this.ImportView.UseVisualStyleBackColor = true;
            // 
            // ImportButton
            // 
            this.ImportButton.Enabled = false;
            this.ImportButton.Location = new System.Drawing.Point(143, 158);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(242, 56);
            this.ImportButton.TabIndex = 5;
            this.ImportButton.Text = "Import";
            this.ImportButton.UseVisualStyleBackColor = true;
            this.ImportButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // ImportBrowseButton
            // 
            this.ImportBrowseButton.Location = new System.Drawing.Point(412, 95);
            this.ImportBrowseButton.Name = "ImportBrowseButton";
            this.ImportBrowseButton.Size = new System.Drawing.Size(87, 26);
            this.ImportBrowseButton.TabIndex = 4;
            this.ImportBrowseButton.Text = "browse...";
            this.ImportBrowseButton.UseVisualStyleBackColor = true;
            this.ImportBrowseButton.Click += new System.EventHandler(this.ImportBrowseButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "File to import:";
            // 
            // ImportFileLocation
            // 
            this.ImportFileLocation.Location = new System.Drawing.Point(116, 95);
            this.ImportFileLocation.Name = "ImportFileLocation";
            this.ImportFileLocation.Size = new System.Drawing.Size(290, 26);
            this.ImportFileLocation.TabIndex = 2;
            this.ImportFileLocation.TextChanged += new System.EventHandler(this.ImportFileLocation_TextChanged);
            // 
            // importLabel2
            // 
            this.importLabel2.AutoSize = true;
            this.importLabel2.Location = new System.Drawing.Point(6, 23);
            this.importLabel2.Name = "importLabel2";
            this.importLabel2.Size = new System.Drawing.Size(236, 20);
            this.importLabel2.TabIndex = 1;
            this.importLabel2.Text = "The imported file is not modified.";
            // 
            // ImportLabel1
            // 
            this.ImportLabel1.AutoSize = true;
            this.ImportLabel1.Location = new System.Drawing.Point(6, 3);
            this.ImportLabel1.Name = "ImportLabel1";
            this.ImportLabel1.Size = new System.Drawing.Size(417, 20);
            this.ImportLabel1.TabIndex = 0;
            this.ImportLabel1.Text = "Import logs from XML, JSON, or MLG files to the logbook. ";
            // 
            // ExportView
            // 
            this.ExportView.Controls.Add(this.ExportButton);
            this.ExportView.Controls.Add(this.ExportBrowseButton);
            this.ExportView.Controls.Add(this.label2);
            this.ExportView.Controls.Add(this.ExportLocationText);
            this.ExportView.Controls.Add(this.label4);
            this.ExportView.Location = new System.Drawing.Point(4, 29);
            this.ExportView.Name = "ExportView";
            this.ExportView.Padding = new System.Windows.Forms.Padding(3);
            this.ExportView.Size = new System.Drawing.Size(587, 414);
            this.ExportView.TabIndex = 2;
            this.ExportView.Text = "Export";
            this.ExportView.UseVisualStyleBackColor = true;
            // 
            // ExportButton
            // 
            this.ExportButton.Enabled = false;
            this.ExportButton.Location = new System.Drawing.Point(143, 158);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(242, 56);
            this.ExportButton.TabIndex = 11;
            this.ExportButton.Text = "Export";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // ExportBrowseButton
            // 
            this.ExportBrowseButton.Location = new System.Drawing.Point(426, 95);
            this.ExportBrowseButton.Name = "ExportBrowseButton";
            this.ExportBrowseButton.Size = new System.Drawing.Size(87, 26);
            this.ExportBrowseButton.TabIndex = 10;
            this.ExportBrowseButton.Text = "browse...";
            this.ExportBrowseButton.UseVisualStyleBackColor = true;
            this.ExportBrowseButton.Click += new System.EventHandler(this.ExportBrowseButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Export Location:";
            // 
            // ExportLocationText
            // 
            this.ExportLocationText.Location = new System.Drawing.Point(130, 95);
            this.ExportLocationText.Name = "ExportLocationText";
            this.ExportLocationText.Size = new System.Drawing.Size(290, 26);
            this.ExportLocationText.TabIndex = 8;
            this.ExportLocationText.TextChanged += new System.EventHandler(this.ExportLocationText_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(417, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Export logs to XML, JSON, or MLG files from the logbook. ";
            // 
            // SyncView
            // 
            this.SyncView.Controls.Add(this.label7);
            this.SyncView.Controls.Add(this.label6);
            this.SyncView.Controls.Add(this.SyncButton);
            this.SyncView.Controls.Add(this.SyncBrowseButton);
            this.SyncView.Controls.Add(this.label3);
            this.SyncView.Controls.Add(this.SyncLocationText);
            this.SyncView.Controls.Add(this.label5);
            this.SyncView.Location = new System.Drawing.Point(4, 29);
            this.SyncView.Name = "SyncView";
            this.SyncView.Padding = new System.Windows.Forms.Padding(3);
            this.SyncView.Size = new System.Drawing.Size(587, 414);
            this.SyncView.TabIndex = 3;
            this.SyncView.Text = "Sync";
            this.SyncView.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 43);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(317, 20);
            this.label7.TabIndex = 18;
            this.label7.Text = "will be added to the logbook, and vice versa.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(426, 20);
            this.label6.TabIndex = 17;
            this.label6.Text = "All logs that exist in MLG file that do not exist in the logbook";
            // 
            // SyncButton
            // 
            this.SyncButton.Enabled = false;
            this.SyncButton.Location = new System.Drawing.Point(143, 158);
            this.SyncButton.Name = "SyncButton";
            this.SyncButton.Size = new System.Drawing.Size(242, 56);
            this.SyncButton.TabIndex = 16;
            this.SyncButton.Text = "Sync";
            this.SyncButton.UseVisualStyleBackColor = true;
            this.SyncButton.Click += new System.EventHandler(this.SyncButton_Click);
            // 
            // SyncBrowseButton
            // 
            this.SyncBrowseButton.Location = new System.Drawing.Point(465, 95);
            this.SyncBrowseButton.Name = "SyncBrowseButton";
            this.SyncBrowseButton.Size = new System.Drawing.Size(87, 26);
            this.SyncBrowseButton.TabIndex = 15;
            this.SyncBrowseButton.Text = "browse...";
            this.SyncBrowseButton.UseVisualStyleBackColor = true;
            this.SyncBrowseButton.Click += new System.EventHandler(this.SyncBrowseButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 20);
            this.label3.TabIndex = 14;
            this.label3.Text = "Other MLG Location:";
            // 
            // SyncLocationText
            // 
            this.SyncLocationText.Location = new System.Drawing.Point(169, 95);
            this.SyncLocationText.Name = "SyncLocationText";
            this.SyncLocationText.Size = new System.Drawing.Size(290, 26);
            this.SyncLocationText.TabIndex = 13;
            this.SyncLocationText.TextChanged += new System.EventHandler(this.SyncLocationText_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(297, 20);
            this.label5.TabIndex = 12;
            this.label5.Text = "Syncs the logbook with another MLG file.";
            // 
            // AboutView
            // 
            this.AboutView.Controls.Add(this.tableLayoutPanel1);
            this.AboutView.Location = new System.Drawing.Point(4, 34);
            this.AboutView.Name = "AboutView";
            this.AboutView.Padding = new System.Windows.Forms.Padding(3);
            this.AboutView.Size = new System.Drawing.Size(601, 453);
            this.AboutView.TabIndex = 3;
            this.AboutView.Text = "About";
            this.AboutView.UseVisualStyleBackColor = true;
            // 
            // ExportSaveDialog
            // 
            this.ExportSaveDialog.Filter = "MLG Files|*.mlg|XML Files|*.xml|JSON Files|.json";
            this.ExportSaveDialog.InitialDirectory = "ExportedLog.mlg";
            // 
            // ImportOpenDialog
            // 
            this.ImportOpenDialog.Filter = "MLG Files|*.mlg|XML Files|*.xml|JSON Files|.json";
            // 
            // SyncOpenDialog
            // 
            this.SyncOpenDialog.Filter = "MLG Files|*.mlg";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.34965F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 74.65035F));
            this.tableLayoutPanel1.Controls.Add(this.VersionLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.VersionValueLabel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.ReportABugValue, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.BugLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.ViewSourceLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.ViewSourceValueLabel, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.VisitSiteLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.VistSiteLabel, 1, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(21, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 58.18182F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 41.81818F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 117F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(572, 439);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // VersionLabel
            // 
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Location = new System.Drawing.Point(3, 68);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(85, 25);
            this.VersionLabel.TabIndex = 0;
            this.VersionLabel.Text = "Version:";
            // 
            // VersionValueLabel
            // 
            this.VersionValueLabel.AutoSize = true;
            this.VersionValueLabel.Location = new System.Drawing.Point(147, 68);
            this.VersionValueLabel.Name = "VersionValueLabel";
            this.VersionValueLabel.Size = new System.Drawing.Size(55, 25);
            this.VersionValueLabel.TabIndex = 1;
            this.VersionValueLabel.Text = "0.1.0";
            // 
            // BugLabel
            // 
            this.BugLabel.AutoSize = true;
            this.BugLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BugLabel.Location = new System.Drawing.Point(3, 204);
            this.BugLabel.Name = "BugLabel";
            this.BugLabel.Size = new System.Drawing.Size(129, 25);
            this.BugLabel.TabIndex = 2;
            this.BugLabel.Text = "Report a bug:";
            // 
            // ReportABugValue
            // 
            this.ReportABugValue.AutoSize = true;
            this.ReportABugValue.Location = new System.Drawing.Point(147, 204);
            this.ReportABugValue.Name = "ReportABugValue";
            this.ReportABugValue.Size = new System.Drawing.Size(421, 50);
            this.ReportABugValue.TabIndex = 3;
            this.ReportABugValue.TabStop = true;
            this.ReportABugValue.Text = "https://bitbucket.org/meditationenthusiasts/meditation-logs-desktop/issues";
            this.ReportABugValue.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ReportABugValue_LinkClicked);
            // 
            // ViewSourceLabel
            // 
            this.ViewSourceLabel.AutoSize = true;
            this.ViewSourceLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ViewSourceLabel.Location = new System.Drawing.Point(3, 321);
            this.ViewSourceLabel.Name = "ViewSourceLabel";
            this.ViewSourceLabel.Size = new System.Drawing.Size(129, 25);
            this.ViewSourceLabel.TabIndex = 4;
            this.ViewSourceLabel.Text = "View Source:";
            // 
            // ViewSourceValueLabel
            // 
            this.ViewSourceValueLabel.AutoSize = true;
            this.ViewSourceValueLabel.Location = new System.Drawing.Point(147, 321);
            this.ViewSourceValueLabel.Name = "ViewSourceValueLabel";
            this.ViewSourceValueLabel.Size = new System.Drawing.Size(421, 50);
            this.ViewSourceValueLabel.TabIndex = 5;
            this.ViewSourceValueLabel.TabStop = true;
            this.ViewSourceValueLabel.Text = "https://bitbucket.org/meditationenthusiasts/meditation-logs-desktop/src";
            this.ViewSourceValueLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ViewSourceValueLabel_LinkClicked);
            // 
            // VisitSiteLabel
            // 
            this.VisitSiteLabel.AutoSize = true;
            this.VisitSiteLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.VisitSiteLabel.Location = new System.Drawing.Point(3, 116);
            this.VisitSiteLabel.Name = "VisitSiteLabel";
            this.VisitSiteLabel.Size = new System.Drawing.Size(94, 25);
            this.VisitSiteLabel.TabIndex = 6;
            this.VisitSiteLabel.Text = "Visit Site:";
            // 
            // VistSiteLabel
            // 
            this.VistSiteLabel.AutoSize = true;
            this.VistSiteLabel.Location = new System.Drawing.Point(147, 116);
            this.VistSiteLabel.Name = "VistSiteLabel";
            this.VistSiteLabel.Size = new System.Drawing.Size(320, 25);
            this.VistSiteLabel.TabIndex = 7;
            this.VistSiteLabel.TabStop = true;
            this.VistSiteLabel.Text = "http://www.meditationenthusiats.org";
            this.VistSiteLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.VistSiteLabel_LinkClicked);
            // 
            // HomePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(609, 491);
            this.Controls.Add(this.MainTabControl);
            this.MinimumSize = new System.Drawing.Size(625, 525);
            this.Name = "HomePage";
            this.Text = "Logger";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MainTabControl.ResumeLayout(false);
            this.StartTab.ResumeLayout(false);
            this.LogbookView.ResumeLayout(false);
            this.LogbookView.PerformLayout();
            this.ManageLogbookView.ResumeLayout(false);
            this.ManageTabControl.ResumeLayout(false);
            this.ImportView.ResumeLayout(false);
            this.ImportView.PerformLayout();
            this.ExportView.ResumeLayout(false);
            this.ExportView.PerformLayout();
            this.SyncView.ResumeLayout(false);
            this.SyncView.PerformLayout();
            this.AboutView.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage Home;
        private System.Windows.Forms.TabPage LogbookView;
        private System.Windows.Forms.TabPage ManageLogbookView;
        private System.Windows.Forms.TabPage AboutView;
        private System.Windows.Forms.TabControl ManageTabControl;
        private System.Windows.Forms.TabPage EditLogView;
        private System.Windows.Forms.TabPage ImportView;
        private System.Windows.Forms.TabPage ExportView;
        private System.Windows.Forms.TabPage SyncView;
        private System.Windows.Forms.TabPage StartTab;
        private System.Windows.Forms.Label importLabel2;
        private System.Windows.Forms.Label ImportLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ImportFileLocation;
        private System.Windows.Forms.Button ImportBrowseButton;
        private System.Windows.Forms.Button ImportButton;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.Button ExportBrowseButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ExportLocationText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.SaveFileDialog ExportSaveDialog;
        private System.Windows.Forms.OpenFileDialog ImportOpenDialog;
        private System.Windows.Forms.Button SyncButton;
        private System.Windows.Forms.Button SyncBrowseButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox SyncLocationText;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.OpenFileDialog SyncOpenDialog;
        private System.Windows.Forms.FlowLayoutPanel ViewLogbookView;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Panel ChangableStartView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label VersionValueLabel;
        private System.Windows.Forms.Label BugLabel;
        private System.Windows.Forms.LinkLabel ReportABugValue;
        private System.Windows.Forms.Label ViewSourceLabel;
        private System.Windows.Forms.LinkLabel ViewSourceValueLabel;
        private System.Windows.Forms.Label VisitSiteLabel;
        private System.Windows.Forms.LinkLabel VistSiteLabel;
    }
}

