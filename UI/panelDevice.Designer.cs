namespace VTP_Induction.UI
{
    partial class panelDevice
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
            this.labelDevName = new System.Windows.Forms.Label();
            this.buttonDev = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.buttonDev)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelDevName
            // 
            this.labelDevName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDevName.Font = new System.Drawing.Font("Tahoma", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDevName.ForeColor = System.Drawing.Color.GreenYellow;
            this.labelDevName.Location = new System.Drawing.Point(0, 0);
            this.labelDevName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDevName.Name = "labelDevName";
            this.labelDevName.Size = new System.Drawing.Size(153, 45);
            this.labelDevName.TabIndex = 9;
            this.labelDevName.Text = "Text";
            this.labelDevName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelDevName.SizeChanged += new System.EventHandler(this.labelDevName_SizeChanged);
            this.labelDevName.DoubleClick += new System.EventHandler(this.labelDevName_DoubleClick);
            this.labelDevName.MouseEnter += new System.EventHandler(this.labelDevName_MouseEnter);
            this.labelDevName.MouseLeave += new System.EventHandler(this.labelDevName_MouseLeave);
            // 
            // buttonDev
            // 
            this.buttonDev.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonDev.Location = new System.Drawing.Point(4, 4);
            this.buttonDev.Margin = new System.Windows.Forms.Padding(0);
            this.buttonDev.Name = "buttonDev";
            this.buttonDev.Size = new System.Drawing.Size(56, 45);
            this.buttonDev.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.buttonDev.TabIndex = 10;
            this.buttonDev.TabStop = false;
            this.buttonDev.DoubleClick += new System.EventHandler(this.buttonDev_DoubleClick);
            this.buttonDev.MouseEnter += new System.EventHandler(this.buttonDev_MouseEnter);
            this.buttonDev.MouseLeave += new System.EventHandler(this.buttonDev_MouseLeave);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.labelDevName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(60, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(153, 45);
            this.panel1.TabIndex = 11;
            // 
            // panelDevice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonDev);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "panelDevice";
            this.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Size = new System.Drawing.Size(217, 53);
            ((System.ComponentModel.ISupportInitialize)(this.buttonDev)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDevName;
        private System.Windows.Forms.PictureBox buttonDev;
        private System.Windows.Forms.Panel panel1;
    }
}
