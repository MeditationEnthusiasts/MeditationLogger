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
            this.SuspendLayout();
            // 
            // MeditateLabel
            // 
            this.MeditateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.MeditateLabel.AutoSize = true;
            this.MeditateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MeditateLabel.Location = new System.Drawing.Point(200, 55);
            this.MeditateLabel.Name = "MeditateLabel";
            this.MeditateLabel.Size = new System.Drawing.Size(91, 20);
            this.MeditateLabel.TabIndex = 0;
            this.MeditateLabel.Text = "Have Fun!";
            this.MeditateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TimerLabel
            // 
            this.TimerLabel.AllowDrop = true;
            this.TimerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.TimerLabel.AutoSize = true;
            this.TimerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimerLabel.Location = new System.Drawing.Point(174, 145);
            this.TimerLabel.Name = "TimerLabel";
            this.TimerLabel.Size = new System.Drawing.Size(153, 39);
            this.TimerLabel.TabIndex = 1;
            this.TimerLabel.Text = "xx:xx:xx";
            this.TimerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MeditateView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.Controls.Add(this.TimerLabel);
            this.Controls.Add(this.MeditateLabel);
            this.Name = "MeditateView";
            this.Size = new System.Drawing.Size(500, 250);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label MeditateLabel;
        public System.Windows.Forms.Label TimerLabel;
    }
}
