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
            this.LogbookView = new System.Windows.Forms.TabPage();
            this.ManageLogbookView = new System.Windows.Forms.TabPage();
            this.AboutView = new System.Windows.Forms.TabPage();
            this.ManageTabControl = new System.Windows.Forms.TabControl();
            this.EditLogView = new System.Windows.Forms.TabPage();
            this.ImportView = new System.Windows.Forms.TabPage();
            this.ExportView = new System.Windows.Forms.TabPage();
            this.MainTabControl.SuspendLayout();
            this.ManageLogbookView.SuspendLayout();
            this.ManageTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.Home);
            this.MainTabControl.Controls.Add(this.LogbookView);
            this.MainTabControl.Controls.Add(this.ManageLogbookView);
            this.MainTabControl.Controls.Add(this.AboutView);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainTabControl.Location = new System.Drawing.Point(0, 0);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.Padding = new System.Drawing.Point(0, 0);
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(507, 432);
            this.MainTabControl.TabIndex = 0;
            // 
            // Home
            // 
            this.Home.Location = new System.Drawing.Point(4, 34);
            this.Home.Name = "Home";
            this.Home.Padding = new System.Windows.Forms.Padding(3);
            this.Home.Size = new System.Drawing.Size(499, 394);
            this.Home.TabIndex = 0;
            this.Home.Text = "Home";
            this.Home.UseVisualStyleBackColor = true;
            // 
            // LogbookView
            // 
            this.LogbookView.Location = new System.Drawing.Point(4, 34);
            this.LogbookView.Name = "LogbookView";
            this.LogbookView.Padding = new System.Windows.Forms.Padding(3);
            this.LogbookView.Size = new System.Drawing.Size(499, 394);
            this.LogbookView.TabIndex = 1;
            this.LogbookView.Text = "View Logbook";
            this.LogbookView.UseVisualStyleBackColor = true;
            // 
            // ManageLogbookView
            // 
            this.ManageLogbookView.Controls.Add(this.ManageTabControl);
            this.ManageLogbookView.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ManageLogbookView.Location = new System.Drawing.Point(4, 34);
            this.ManageLogbookView.Name = "ManageLogbookView";
            this.ManageLogbookView.Padding = new System.Windows.Forms.Padding(3);
            this.ManageLogbookView.Size = new System.Drawing.Size(499, 394);
            this.ManageLogbookView.TabIndex = 2;
            this.ManageLogbookView.Text = "Manage Logbook";
            this.ManageLogbookView.UseVisualStyleBackColor = true;
            // 
            // AboutView
            // 
            this.AboutView.Location = new System.Drawing.Point(4, 34);
            this.AboutView.Name = "AboutView";
            this.AboutView.Padding = new System.Windows.Forms.Padding(3);
            this.AboutView.Size = new System.Drawing.Size(499, 394);
            this.AboutView.TabIndex = 3;
            this.AboutView.Text = "About";
            this.AboutView.UseVisualStyleBackColor = true;
            // 
            // ManageTabControl
            // 
            this.ManageTabControl.Controls.Add(this.EditLogView);
            this.ManageTabControl.Controls.Add(this.ImportView);
            this.ManageTabControl.Controls.Add(this.ExportView);
            this.ManageTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ManageTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ManageTabControl.Location = new System.Drawing.Point(3, 3);
            this.ManageTabControl.Name = "ManageTabControl";
            this.ManageTabControl.SelectedIndex = 0;
            this.ManageTabControl.Size = new System.Drawing.Size(493, 388);
            this.ManageTabControl.TabIndex = 0;
            // 
            // EditLogView
            // 
            this.EditLogView.Location = new System.Drawing.Point(4, 34);
            this.EditLogView.Name = "EditLogView";
            this.EditLogView.Padding = new System.Windows.Forms.Padding(3);
            this.EditLogView.Size = new System.Drawing.Size(485, 350);
            this.EditLogView.TabIndex = 0;
            this.EditLogView.Text = "Edit Logs";
            this.EditLogView.UseVisualStyleBackColor = true;
            // 
            // ImportView
            // 
            this.ImportView.Location = new System.Drawing.Point(4, 34);
            this.ImportView.Name = "ImportView";
            this.ImportView.Padding = new System.Windows.Forms.Padding(3);
            this.ImportView.Size = new System.Drawing.Size(485, 350);
            this.ImportView.TabIndex = 1;
            this.ImportView.Text = "Import";
            this.ImportView.UseVisualStyleBackColor = true;
            // 
            // ExportView
            // 
            this.ExportView.Location = new System.Drawing.Point(4, 29);
            this.ExportView.Name = "ExportView";
            this.ExportView.Padding = new System.Windows.Forms.Padding(3);
            this.ExportView.Size = new System.Drawing.Size(485, 355);
            this.ExportView.TabIndex = 2;
            this.ExportView.Text = "Export";
            this.ExportView.UseVisualStyleBackColor = true;
            // 
            // HomePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(507, 432);
            this.Controls.Add(this.MainTabControl);
            this.Name = "HomePage";
            this.Text = "Meditation Enthusiasts Logger";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MainTabControl.ResumeLayout(false);
            this.ManageLogbookView.ResumeLayout(false);
            this.ManageTabControl.ResumeLayout(false);
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
    }
}

