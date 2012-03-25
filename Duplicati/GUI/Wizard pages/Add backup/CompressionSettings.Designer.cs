namespace Duplicati.GUI.Wizard_pages.Add_backup
{
    partial class CompressionSettings
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompressionSettings));
            this.label1 = new System.Windows.Forms.Label();
            this.cmbCompression = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbCompressionInfo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cmbCompression
            // 
            this.cmbCompression.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCompression.FormattingEnabled = true;
            resources.ApplyResources(this.cmbCompression, "cmbCompression");
            this.cmbCompression.Name = "cmbCompression";
            this.cmbCompression.SelectedIndexChanged += new System.EventHandler(this.cmbCompression_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tbCompressionInfo
            // 
            resources.ApplyResources(this.tbCompressionInfo, "tbCompressionInfo");
            this.tbCompressionInfo.Name = "tbCompressionInfo";
            // 
            // CompressionSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbCompressionInfo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbCompression);
            this.Controls.Add(this.label1);
            this.Name = "CompressionSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbCompression;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbCompressionInfo;
    }
}
