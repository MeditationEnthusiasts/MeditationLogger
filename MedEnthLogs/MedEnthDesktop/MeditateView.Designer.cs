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

namespace MedEnthDesktop
{
    partial class MeditateView
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
            this.MeditateLabel = new System.Windows.Forms.Label();
            this.TimerLabel = new System.Windows.Forms.Label();
            this.MeditateViewTable = new System.Windows.Forms.TableLayoutPanel();
            this.MeditateViewTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // MeditateLabel
            // 
            this.MeditateLabel.AutoSize = true;
            this.MeditateLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.MeditateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MeditateLabel.Location = new System.Drawing.Point(3, 46);
            this.MeditateLabel.Name = "MeditateLabel";
            this.MeditateLabel.Size = new System.Drawing.Size(494, 37);
            this.MeditateLabel.TabIndex = 0;
            this.MeditateLabel.Text = "Have Fun!";
            this.MeditateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TimerLabel
            // 
            this.TimerLabel.AllowDrop = true;
            this.TimerLabel.AutoSize = true;
            this.TimerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TimerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimerLabel.Location = new System.Drawing.Point(3, 166);
            this.TimerLabel.Name = "TimerLabel";
            this.TimerLabel.Size = new System.Drawing.Size(494, 73);
            this.TimerLabel.TabIndex = 1;
            this.TimerLabel.Text = "xx:xx:xx";
            this.TimerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MeditateViewTable
            // 
            this.MeditateViewTable.ColumnCount = 1;
            this.MeditateViewTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MeditateViewTable.Controls.Add(this.MeditateLabel, 0, 0);
            this.MeditateViewTable.Controls.Add(this.TimerLabel, 0, 2);
            this.MeditateViewTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MeditateViewTable.Location = new System.Drawing.Point(0, 0);
            this.MeditateViewTable.Name = "MeditateViewTable";
            this.MeditateViewTable.RowCount = 3;
            this.MeditateViewTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.MeditateViewTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.MeditateViewTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.MeditateViewTable.Size = new System.Drawing.Size(500, 250);
            this.MeditateViewTable.TabIndex = 2;
            // 
            // MeditateView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.Controls.Add(this.MeditateViewTable);
            this.Name = "MeditateView";
            this.Size = new System.Drawing.Size(500, 250);
            this.MeditateViewTable.ResumeLayout(false);
            this.MeditateViewTable.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label MeditateLabel;
        public System.Windows.Forms.Label TimerLabel;
        private System.Windows.Forms.TableLayoutPanel MeditateViewTable;
    }
}
