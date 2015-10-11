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
            this.SuspendLayout();
            // 
            // MinutesLabel
            // 
            this.MinutesLabel.AutoSize = true;
            this.MinutesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinutesLabel.Location = new System.Drawing.Point(17, 12);
            this.MinutesLabel.Name = "MinutesLabel";
            this.MinutesLabel.Size = new System.Drawing.Size(105, 16);
            this.MinutesLabel.TabIndex = 0;
            this.MinutesLabel.Text = "Total Minutes:";
            // 
            // MinutesValueLabel
            // 
            this.MinutesValueLabel.AutoSize = true;
            this.MinutesValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinutesValueLabel.Location = new System.Drawing.Point(128, 12);
            this.MinutesValueLabel.Name = "MinutesValueLabel";
            this.MinutesValueLabel.Size = new System.Drawing.Size(23, 15);
            this.MinutesValueLabel.TabIndex = 1;
            this.MinutesValueLabel.Text = "XX";
            // 
            // UseLocationCheckbox
            // 
            this.UseLocationCheckbox.AutoSize = true;
            this.UseLocationCheckbox.Location = new System.Drawing.Point(18, 211);
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
            this.TechniqueUsedLabel.Location = new System.Drawing.Point(17, 39);
            this.TechniqueUsedLabel.Name = "TechniqueUsedLabel";
            this.TechniqueUsedLabel.Size = new System.Drawing.Size(126, 16);
            this.TechniqueUsedLabel.TabIndex = 3;
            this.TechniqueUsedLabel.Text = "Technique Used:";
            // 
            // TechniqueUsedTextbox
            // 
            this.TechniqueUsedTextbox.Location = new System.Drawing.Point(149, 38);
            this.TechniqueUsedTextbox.Name = "TechniqueUsedTextbox";
            this.TechniqueUsedTextbox.Size = new System.Drawing.Size(150, 20);
            this.TechniqueUsedTextbox.TabIndex = 4;
            // 
            // CommentsLabel
            // 
            this.CommentsLabel.AutoSize = true;
            this.CommentsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommentsLabel.Location = new System.Drawing.Point(17, 64);
            this.CommentsLabel.Name = "CommentsLabel";
            this.CommentsLabel.Size = new System.Drawing.Size(84, 16);
            this.CommentsLabel.TabIndex = 5;
            this.CommentsLabel.Text = "Comments:";
            // 
            // CommentsTextBox
            // 
            this.CommentsTextBox.Location = new System.Drawing.Point(18, 88);
            this.CommentsTextBox.Name = "CommentsTextBox";
            this.CommentsTextBox.Size = new System.Drawing.Size(298, 106);
            this.CommentsTextBox.TabIndex = 6;
            this.CommentsTextBox.Text = "";
            // 
            // SaveView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.CommentsTextBox);
            this.Controls.Add(this.CommentsLabel);
            this.Controls.Add(this.TechniqueUsedTextbox);
            this.Controls.Add(this.TechniqueUsedLabel);
            this.Controls.Add(this.UseLocationCheckbox);
            this.Controls.Add(this.MinutesValueLabel);
            this.Controls.Add(this.MinutesLabel);
            this.Name = "SaveView";
            this.Size = new System.Drawing.Size(350, 250);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label MinutesLabel;
        private System.Windows.Forms.Label TechniqueUsedLabel;
        private System.Windows.Forms.Label CommentsLabel;
        public System.Windows.Forms.TextBox TechniqueUsedTextbox;
        public System.Windows.Forms.RichTextBox CommentsTextBox;
        public System.Windows.Forms.Label MinutesValueLabel;
        public System.Windows.Forms.CheckBox UseLocationCheckbox;
    }
}
