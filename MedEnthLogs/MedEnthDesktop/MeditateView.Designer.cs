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
            this.SuspendLayout();
            // 
            // MeditateLabel
            // 
            this.MeditateLabel.AutoSize = true;
            this.MeditateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MeditateLabel.Location = new System.Drawing.Point(89, 18);
            this.MeditateLabel.Name = "MeditateLabel";
            this.MeditateLabel.Size = new System.Drawing.Size(91, 20);
            this.MeditateLabel.TabIndex = 0;
            this.MeditateLabel.Text = "Have Fun!";
            this.MeditateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MeditateView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MeditateLabel);
            this.Name = "MeditateView";
            this.Size = new System.Drawing.Size(292, 175);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label MeditateLabel;
    }
}
