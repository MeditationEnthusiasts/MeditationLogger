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
    partial class SaveView
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
            this.MinutesLabel = new System.Windows.Forms.Label();
            this.MinutesValueLabel = new System.Windows.Forms.Label();
            this.UseLocationCheckbox = new System.Windows.Forms.CheckBox();
            this.TechniqueUsedLabel = new System.Windows.Forms.Label();
            this.TechniqueUsedTextbox = new System.Windows.Forms.TextBox();
            this.CommentsLabel = new System.Windows.Forms.Label();
            this.CommentsTextBox = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MinutesLabel
            // 
            this.MinutesLabel.AutoSize = true;
            this.MinutesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinutesLabel.Location = new System.Drawing.Point(3, 0);
            this.MinutesLabel.Name = "MinutesLabel";
            this.MinutesLabel.Size = new System.Drawing.Size(105, 16);
            this.MinutesLabel.TabIndex = 0;
            this.MinutesLabel.Text = "Total Minutes:";
            // 
            // MinutesValueLabel
            // 
            this.MinutesValueLabel.AutoSize = true;
            this.MinutesValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinutesValueLabel.Location = new System.Drawing.Point(153, 0);
            this.MinutesValueLabel.Name = "MinutesValueLabel";
            this.MinutesValueLabel.Size = new System.Drawing.Size(23, 15);
            this.MinutesValueLabel.TabIndex = 1;
            this.MinutesValueLabel.Text = "XX";
            // 
            // UseLocationCheckbox
            // 
            this.UseLocationCheckbox.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.UseLocationCheckbox, 2);
            this.UseLocationCheckbox.Location = new System.Drawing.Point(3, 213);
            this.UseLocationCheckbox.Name = "UseLocationCheckbox";
            this.UseLocationCheckbox.Size = new System.Drawing.Size(95, 17);
            this.UseLocationCheckbox.TabIndex = 2;
            this.UseLocationCheckbox.Text = "Save Location";
            this.UseLocationCheckbox.UseVisualStyleBackColor = true;
            // 
            // TechniqueUsedLabel
            // 
            this.TechniqueUsedLabel.AutoSize = true;
            this.TechniqueUsedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TechniqueUsedLabel.Location = new System.Drawing.Point(3, 30);
            this.TechniqueUsedLabel.Name = "TechniqueUsedLabel";
            this.TechniqueUsedLabel.Size = new System.Drawing.Size(126, 16);
            this.TechniqueUsedLabel.TabIndex = 3;
            this.TechniqueUsedLabel.Text = "Technique Used:";
            // 
            // TechniqueUsedTextbox
            // 
            this.TechniqueUsedTextbox.Location = new System.Drawing.Point(153, 33);
            this.TechniqueUsedTextbox.Name = "TechniqueUsedTextbox";
            this.TechniqueUsedTextbox.Size = new System.Drawing.Size(150, 20);
            this.TechniqueUsedTextbox.TabIndex = 4;
            // 
            // CommentsLabel
            // 
            this.CommentsLabel.AutoSize = true;
            this.CommentsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommentsLabel.Location = new System.Drawing.Point(3, 60);
            this.CommentsLabel.Name = "CommentsLabel";
            this.CommentsLabel.Size = new System.Drawing.Size(84, 16);
            this.CommentsLabel.TabIndex = 5;
            this.CommentsLabel.Text = "Comments:";
            // 
            // CommentsTextBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.CommentsTextBox, 2);
            this.CommentsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommentsTextBox.Location = new System.Drawing.Point(3, 93);
            this.CommentsTextBox.Name = "CommentsTextBox";
            this.CommentsTextBox.Size = new System.Drawing.Size(344, 114);
            this.CommentsTextBox.TabIndex = 6;
            this.CommentsTextBox.Text = "";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.MinutesLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.CommentsTextBox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.MinutesValueLabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.UseLocationCheckbox, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.CommentsLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.TechniqueUsedLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.TechniqueUsedTextbox, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(350, 250);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // SaveView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SaveView";
            this.Size = new System.Drawing.Size(350, 250);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label MinutesLabel;
        private System.Windows.Forms.Label TechniqueUsedLabel;
        private System.Windows.Forms.Label CommentsLabel;
        public System.Windows.Forms.TextBox TechniqueUsedTextbox;
        public System.Windows.Forms.RichTextBox CommentsTextBox;
        public System.Windows.Forms.Label MinutesValueLabel;
        public System.Windows.Forms.CheckBox UseLocationCheckbox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
