namespace VTP_Induction.UI
{
    partial class CameraConfig
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBoxDev = new System.Windows.Forms.GroupBox();
            this.textBoxNameDev = new System.Windows.Forms.TextBox();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxExporSureTime = new System.Windows.Forms.TextBox();
            this.cboCamMode = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.CheckBoxUseBarcodeTCP = new System.Windows.Forms.CheckBox();
            this.checkBoxUseDev = new System.Windows.Forms.CheckBox();
            this.textBoxCameraPort = new System.Windows.Forms.TextBox();
            this.textBoxCameraIP = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBoxDev.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBoxDev);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(309, 194);
            this.panel1.TabIndex = 0;
            // 
            // groupBoxDev
            // 
            this.groupBoxDev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBoxDev.Controls.Add(this.textBoxNameDev);
            this.groupBoxDev.Controls.Add(this.textBoxID);
            this.groupBoxDev.Controls.Add(this.label1);
            this.groupBoxDev.Controls.Add(this.textBoxExporSureTime);
            this.groupBoxDev.Controls.Add(this.cboCamMode);
            this.groupBoxDev.Controls.Add(this.label18);
            this.groupBoxDev.Controls.Add(this.label14);
            this.groupBoxDev.Controls.Add(this.CheckBoxUseBarcodeTCP);
            this.groupBoxDev.Controls.Add(this.checkBoxUseDev);
            this.groupBoxDev.Controls.Add(this.textBoxCameraPort);
            this.groupBoxDev.Controls.Add(this.textBoxCameraIP);
            this.groupBoxDev.Controls.Add(this.label11);
            this.groupBoxDev.Controls.Add(this.label12);
            this.groupBoxDev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDev.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBoxDev.Location = new System.Drawing.Point(0, 0);
            this.groupBoxDev.Name = "groupBoxDev";
            this.groupBoxDev.Size = new System.Drawing.Size(309, 194);
            this.groupBoxDev.TabIndex = 0;
            this.groupBoxDev.TabStop = false;
            this.groupBoxDev.Text = "[CAMERA HIK ]";
            // 
            // textBoxNameDev
            // 
            this.textBoxNameDev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBoxNameDev.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.textBoxNameDev.Location = new System.Drawing.Point(103, 46);
            this.textBoxNameDev.Name = "textBoxNameDev";
            this.textBoxNameDev.Size = new System.Drawing.Size(186, 20);
            this.textBoxNameDev.TabIndex = 74;
            // 
            // textBoxID
            // 
            this.textBoxID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBoxID.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.textBoxID.Location = new System.Drawing.Point(103, 74);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(186, 20);
            this.textBoxID.TabIndex = 73;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 16);
            this.label1.TabIndex = 72;
            this.label1.Text = "ID :";
            // 
            // textBoxExporSureTime
            // 
            this.textBoxExporSureTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBoxExporSureTime.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.textBoxExporSureTime.Location = new System.Drawing.Point(103, 130);
            this.textBoxExporSureTime.Name = "textBoxExporSureTime";
            this.textBoxExporSureTime.Size = new System.Drawing.Size(186, 20);
            this.textBoxExporSureTime.TabIndex = 69;
            // 
            // cboCamMode
            // 
            this.cboCamMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cboCamMode.ForeColor = System.Drawing.Color.White;
            this.cboCamMode.FormattingEnabled = true;
            this.cboCamMode.Items.AddRange(new object[] {
            "ON",
            "OFF",
            "TRIGGERSOFTWARE"});
            this.cboCamMode.Location = new System.Drawing.Point(103, 159);
            this.cboCamMode.Name = "cboCamMode";
            this.cboCamMode.Size = new System.Drawing.Size(186, 21);
            this.cboCamMode.TabIndex = 68;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.BackColor = System.Drawing.Color.Transparent;
            this.label18.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(3, 159);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(86, 16);
            this.label18.TabIndex = 67;
            this.label18.Text = "TriggerMode:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(3, 130);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(94, 16);
            this.label14.TabIndex = 65;
            this.label14.Text = "ExposureTime:";
            // 
            // CheckBoxUseBarcodeTCP
            // 
            this.CheckBoxUseBarcodeTCP.AutoSize = true;
            this.CheckBoxUseBarcodeTCP.Location = new System.Drawing.Point(76, 19);
            this.CheckBoxUseBarcodeTCP.Name = "CheckBoxUseBarcodeTCP";
            this.CheckBoxUseBarcodeTCP.Size = new System.Drawing.Size(112, 17);
            this.CheckBoxUseBarcodeTCP.TabIndex = 59;
            this.CheckBoxUseBarcodeTCP.Text = "Use TCP Barcode";
            this.CheckBoxUseBarcodeTCP.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseDev
            // 
            this.checkBoxUseDev.AutoSize = true;
            this.checkBoxUseDev.Location = new System.Drawing.Point(9, 19);
            this.checkBoxUseDev.Name = "checkBoxUseDev";
            this.checkBoxUseDev.Size = new System.Drawing.Size(56, 17);
            this.checkBoxUseDev.TabIndex = 58;
            this.checkBoxUseDev.Text = "Active";
            this.checkBoxUseDev.UseVisualStyleBackColor = true;
            // 
            // textBoxCameraPort
            // 
            this.textBoxCameraPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.textBoxCameraPort.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.textBoxCameraPort.Location = new System.Drawing.Point(238, 102);
            this.textBoxCameraPort.Name = "textBoxCameraPort";
            this.textBoxCameraPort.Size = new System.Drawing.Size(51, 20);
            this.textBoxCameraPort.TabIndex = 57;
            // 
            // textBoxCameraIP
            // 
            this.textBoxCameraIP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBoxCameraIP.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.textBoxCameraIP.Location = new System.Drawing.Point(103, 102);
            this.textBoxCameraIP.Name = "textBoxCameraIP";
            this.textBoxCameraIP.Size = new System.Drawing.Size(134, 20);
            this.textBoxCameraIP.TabIndex = 56;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 104);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 13);
            this.label11.TabIndex = 55;
            this.label11.Text = "Camera IP:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(3, 46);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(42, 14);
            this.label12.TabIndex = 53;
            this.label12.Text = "Name:";
            // 
            // CameraConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "CameraConfig";
            this.Size = new System.Drawing.Size(309, 194);
            this.panel1.ResumeLayout(false);
            this.groupBoxDev.ResumeLayout(false);
            this.groupBoxDev.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBoxDev;
        private System.Windows.Forms.TextBox textBoxCameraPort;
        private System.Windows.Forms.TextBox textBoxCameraIP;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox CheckBoxUseBarcodeTCP;
        private System.Windows.Forms.CheckBox checkBoxUseDev;
        private System.Windows.Forms.TextBox textBoxExporSureTime;
        private System.Windows.Forms.ComboBox cboCamMode;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxNameDev;

    }
}
