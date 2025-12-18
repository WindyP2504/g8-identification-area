namespace VTP_Induction.UI
{
    partial class ScaleRS232
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
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxNameDev = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxDatabits = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.checkBoxUseDev = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBoxPort = new System.Windows.Forms.ComboBox();
            this.comboBoxBaudrate = new System.Windows.Forms.ComboBox();
            this.comboBoxParity = new System.Windows.Forms.ComboBox();
            this.comboBoxStopBits = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.groupBoxDev.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBoxDev);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(412, 239);
            this.panel1.TabIndex = 0;
            // 
            // groupBoxDev
            // 
            this.groupBoxDev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBoxDev.Controls.Add(this.comboBoxStopBits);
            this.groupBoxDev.Controls.Add(this.comboBoxParity);
            this.groupBoxDev.Controls.Add(this.comboBoxBaudrate);
            this.groupBoxDev.Controls.Add(this.comboBoxPort);
            this.groupBoxDev.Controls.Add(this.label2);
            this.groupBoxDev.Controls.Add(this.textBoxNameDev);
            this.groupBoxDev.Controls.Add(this.label1);
            this.groupBoxDev.Controls.Add(this.comboBoxDatabits);
            this.groupBoxDev.Controls.Add(this.label18);
            this.groupBoxDev.Controls.Add(this.label14);
            this.groupBoxDev.Controls.Add(this.checkBoxUseDev);
            this.groupBoxDev.Controls.Add(this.label11);
            this.groupBoxDev.Controls.Add(this.label12);
            this.groupBoxDev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDev.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBoxDev.Location = new System.Drawing.Point(0, 0);
            this.groupBoxDev.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxDev.Name = "groupBoxDev";
            this.groupBoxDev.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxDev.Size = new System.Drawing.Size(412, 239);
            this.groupBoxDev.TabIndex = 0;
            this.groupBoxDev.TabStop = false;
            this.groupBoxDev.Text = "[CAS EC II SCALE ]";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 210);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 21);
            this.label2.TabIndex = 75;
            this.label2.Text = "Stop Bits:";
            // 
            // textBoxNameDev
            // 
            this.textBoxNameDev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBoxNameDev.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.textBoxNameDev.Location = new System.Drawing.Point(141, 47);
            this.textBoxNameDev.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxNameDev.Name = "textBoxNameDev";
            this.textBoxNameDev.Size = new System.Drawing.Size(177, 22);
            this.textBoxNameDev.TabIndex = 74;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 83);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 21);
            this.label1.TabIndex = 72;
            this.label1.Text = "Port:";
            // 
            // comboBoxDatabits
            // 
            this.comboBoxDatabits.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.comboBoxDatabits.ForeColor = System.Drawing.Color.White;
            this.comboBoxDatabits.FormattingEnabled = true;
            this.comboBoxDatabits.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.comboBoxDatabits.Location = new System.Drawing.Point(141, 176);
            this.comboBoxDatabits.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxDatabits.Name = "comboBoxDatabits";
            this.comboBoxDatabits.Size = new System.Drawing.Size(177, 24);
            this.comboBoxDatabits.TabIndex = 68;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.BackColor = System.Drawing.Color.Transparent;
            this.label18.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(8, 176);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(79, 21);
            this.label18.TabIndex = 67;
            this.label18.Text = "Databits:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(8, 147);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(58, 21);
            this.label14.TabIndex = 65;
            this.label14.Text = "Parity:";
            // 
            // checkBoxUseDev
            // 
            this.checkBoxUseDev.AutoSize = true;
            this.checkBoxUseDev.Location = new System.Drawing.Point(12, 23);
            this.checkBoxUseDev.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxUseDev.Name = "checkBoxUseDev";
            this.checkBoxUseDev.Size = new System.Drawing.Size(68, 21);
            this.checkBoxUseDev.TabIndex = 58;
            this.checkBoxUseDev.Text = "Active";
            this.checkBoxUseDev.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(8, 117);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(82, 20);
            this.label11.TabIndex = 55;
            this.label11.Text = "Baudrate:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(9, 51);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(52, 18);
            this.label12.TabIndex = 53;
            this.label12.Text = "Name:";
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.comboBoxPort.ForeColor = System.Drawing.Color.White;
            this.comboBoxPort.FormattingEnabled = true;
            this.comboBoxPort.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10"});
            this.comboBoxPort.Location = new System.Drawing.Point(141, 77);
            this.comboBoxPort.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(177, 24);
            this.comboBoxPort.TabIndex = 77;
            // 
            // comboBoxBaudrate
            // 
            this.comboBoxBaudrate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.comboBoxBaudrate.ForeColor = System.Drawing.Color.White;
            this.comboBoxBaudrate.FormattingEnabled = true;
            this.comboBoxBaudrate.Items.AddRange(new object[] {
            "1200",
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "38400",
            "57600",
            "115220"});
            this.comboBoxBaudrate.Location = new System.Drawing.Point(141, 113);
            this.comboBoxBaudrate.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxBaudrate.Name = "comboBoxBaudrate";
            this.comboBoxBaudrate.Size = new System.Drawing.Size(177, 24);
            this.comboBoxBaudrate.TabIndex = 78;
            // 
            // comboBoxParity
            // 
            this.comboBoxParity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.comboBoxParity.ForeColor = System.Drawing.Color.White;
            this.comboBoxParity.FormattingEnabled = true;
            this.comboBoxParity.Items.AddRange(new object[] {
            "None",
            "Even",
            "Odd"});
            this.comboBoxParity.Location = new System.Drawing.Point(141, 145);
            this.comboBoxParity.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxParity.Name = "comboBoxParity";
            this.comboBoxParity.Size = new System.Drawing.Size(177, 24);
            this.comboBoxParity.TabIndex = 79;
            // 
            // comboBoxStopBits
            // 
            this.comboBoxStopBits.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.comboBoxStopBits.ForeColor = System.Drawing.Color.White;
            this.comboBoxStopBits.FormattingEnabled = true;
            this.comboBoxStopBits.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2"});
            this.comboBoxStopBits.Location = new System.Drawing.Point(141, 207);
            this.comboBoxStopBits.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxStopBits.Name = "comboBoxStopBits";
            this.comboBoxStopBits.Size = new System.Drawing.Size(177, 24);
            this.comboBoxStopBits.TabIndex = 80;
            // 
            // ScaleRS232
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ScaleRS232";
            this.Size = new System.Drawing.Size(412, 239);
            this.panel1.ResumeLayout(false);
            this.groupBoxDev.ResumeLayout(false);
            this.groupBoxDev.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBoxDev;
        private System.Windows.Forms.CheckBox checkBoxUseDev;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxNameDev;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxDatabits;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox comboBoxStopBits;
        private System.Windows.Forms.ComboBox comboBoxParity;
        private System.Windows.Forms.ComboBox comboBoxBaudrate;
        private System.Windows.Forms.ComboBox comboBoxPort;

    }
}
