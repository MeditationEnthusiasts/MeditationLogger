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

namespace MeditationEnthusiasts.MeditationLogger.Desktop
{
    partial class LogView
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
            this.StartDateLabel = new System.Windows.Forms.Label();
            this.DurationLabel = new System.Windows.Forms.Label();
            this.DurationValueLabel = new System.Windows.Forms.Label();
            this.TechniqueValueLabel = new System.Windows.Forms.Label();
            this.TechniqueLabel = new System.Windows.Forms.Label();
            this.CommentLabel = new System.Windows.Forms.Label();
            this.CommentValueTextBox = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // StartDateLabel
            // 
            this.StartDateLabel.AutoSize = true;
            this.StartDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartDateLabel.Location = new System.Drawing.Point(12, 11);
            this.StartDateLabel.Name = "StartDateLabel";
            this.StartDateLabel.Size = new System.Drawing.Size(99, 20);
            this.StartDateLabel.TabIndex = 0;
            this.StartDateLabel.Text = "06/13/1991";
            // 
            // DurationLabel
            // 
            this.DurationLabel.AutoSize = true;
            this.DurationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DurationLabel.Location = new System.Drawing.Point(12, 45);
            this.DurationLabel.Name = "DurationLabel";
            this.DurationLabel.Size = new System.Drawing.Size(83, 20);
            this.DurationLabel.TabIndex = 1;
            this.DurationLabel.Text = "Duration:";
            // 
            // DurationValueLabel
            // 
            this.DurationValueLabel.AutoSize = true;
            this.DurationValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DurationValueLabel.Location = new System.Drawing.Point(101, 45);
            this.DurationValueLabel.Name = "DurationValueLabel";
            this.DurationValueLabel.Size = new System.Drawing.Size(113, 20);
            this.DurationValueLabel.TabIndex = 2;
            this.DurationValueLabel.Text = "XXXX Minutes";
            // 
            // TechniqueValueLabel
            // 
            this.TechniqueValueLabel.AutoSize = true;
            this.TechniqueValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TechniqueValueLabel.Location = new System.Drawing.Point(115, 74);
            this.TechniqueValueLabel.Name = "TechniqueValueLabel";
            this.TechniqueValueLabel.Size = new System.Drawing.Size(240, 20);
            this.TechniqueValueLabel.TabIndex = 4;
            this.TechniqueValueLabel.Text = "SomethingSomethingSomething";
            // 
            // TechniqueLabel
            // 
            this.TechniqueLabel.AutoSize = true;
            this.TechniqueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TechniqueLabel.Location = new System.Drawing.Point(12, 74);
            this.TechniqueLabel.Name = "TechniqueLabel";
            this.TechniqueLabel.Size = new System.Drawing.Size(97, 20);
            this.TechniqueLabel.TabIndex = 3;
            this.TechniqueLabel.Text = "Technique:";
            // 
            // CommentLabel
            // 
            this.CommentLabel.AutoSize = true;
            this.CommentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommentLabel.Location = new System.Drawing.Point(12, 103);
            this.CommentLabel.Name = "CommentLabel";
            this.CommentLabel.Size = new System.Drawing.Size(99, 20);
            this.CommentLabel.TabIndex = 5;
            this.CommentLabel.Text = "Comments:";
            // 
            // CommentValueTextBox
            // 
            this.CommentValueTextBox.Location = new System.Drawing.Point(19, 129);
            this.CommentValueTextBox.Name = "CommentValueTextBox";
            this.CommentValueTextBox.Size = new System.Drawing.Size(361, 56);
            this.CommentValueTextBox.TabIndex = 6;
            // 
            // LogView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.CommentValueTextBox);
            this.Controls.Add(this.CommentLabel);
            this.Controls.Add(this.TechniqueValueLabel);
            this.Controls.Add(this.TechniqueLabel);
            this.Controls.Add(this.DurationValueLabel);
            this.Controls.Add(this.DurationLabel);
            this.Controls.Add(this.StartDateLabel);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "LogView";
            this.Size = new System.Drawing.Size(600, 229);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label StartDateLabel;
        private System.Windows.Forms.Label DurationLabel;
        private System.Windows.Forms.Label DurationValueLabel;
        private System.Windows.Forms.Label TechniqueValueLabel;
        private System.Windows.Forms.Label TechniqueLabel;
        private System.Windows.Forms.Label CommentLabel;
        private System.Windows.Forms.Label CommentValueTextBox;
    }
}
