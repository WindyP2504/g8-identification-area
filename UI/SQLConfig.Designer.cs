
namespace VTP_Induction.UI
{
    partial class SQLConfig
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
            this.groupBoxDev = new System.Windows.Forms.GroupBox();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.txtConStr = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBoxDev.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxDev
            // 
            this.groupBoxDev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBoxDev.Controls.Add(this.cboType);
            this.groupBoxDev.Controls.Add(this.txtConStr);
            this.groupBoxDev.Controls.Add(this.label1);
            this.groupBoxDev.Controls.Add(this.label12);
            this.groupBoxDev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDev.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBoxDev.Location = new System.Drawing.Point(0, 0);
            this.groupBoxDev.Name = "groupBoxDev";
            this.groupBoxDev.Size = new System.Drawing.Size(558, 108);
            this.groupBoxDev.TabIndex = 1;
            this.groupBoxDev.TabStop = false;
            this.groupBoxDev.Text = "[SQL CONFIG]";
            // 
            // cboType
            // 
            this.cboType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cboType.ForeColor = System.Drawing.Color.White;
            this.cboType.FormattingEnabled = true;
            this.cboType.Items.AddRange(new object[] {
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
            this.cboType.Location = new System.Drawing.Point(122, 31);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(134, 21);
            this.cboType.TabIndex = 77;
            // 
            // txtConStr
            // 
            this.txtConStr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtConStr.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtConStr.Location = new System.Drawing.Point(122, 70);
            this.txtConStr.Name = "txtConStr";
            this.txtConStr.Size = new System.Drawing.Size(424, 20);
            this.txtConStr.TabIndex = 74;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 16);
            this.label1.TabIndex = 72;
            this.label1.Text = "Type:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(7, 72);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(109, 14);
            this.label12.TabIndex = 53;
            this.label12.Text = "Connection String:";
            // 
            // SQLConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxDev);
            this.Name = "SQLConfig";
            this.Size = new System.Drawing.Size(558, 108);
            this.groupBoxDev.ResumeLayout(false);
            this.groupBoxDev.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxDev;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.TextBox txtConStr;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label12;
    }
}
