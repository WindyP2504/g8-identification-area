namespace VTP_Induction.UI
{
    partial class APIwms
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
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxNameDev = new System.Windows.Forms.TextBox();
            this.checkBoxUseDev = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBoxDev.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxDev
            // 
            this.groupBoxDev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBoxDev.Controls.Add(this.txtUrl);
            this.groupBoxDev.Controls.Add(this.label2);
            this.groupBoxDev.Controls.Add(this.textBoxNameDev);
            this.groupBoxDev.Controls.Add(this.checkBoxUseDev);
            this.groupBoxDev.Controls.Add(this.label12);
            this.groupBoxDev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDev.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBoxDev.Location = new System.Drawing.Point(0, 0);
            this.groupBoxDev.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxDev.Name = "groupBoxDev";
            this.groupBoxDev.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxDev.Size = new System.Drawing.Size(435, 200);
            this.groupBoxDev.TabIndex = 2;
            this.groupBoxDev.TabStop = false;
            this.groupBoxDev.Text = "[RCS]";
            // 
            // txtUrl
            // 
            this.txtUrl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtUrl.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtUrl.Location = new System.Drawing.Point(80, 102);
            this.txtUrl.Margin = new System.Windows.Forms.Padding(4);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(265, 22);
            this.txtUrl.TabIndex = 76;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 103);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 21);
            this.label2.TabIndex = 75;
            this.label2.Text = "IP :";
            // 
            // textBoxNameDev
            // 
            this.textBoxNameDev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBoxNameDev.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.textBoxNameDev.Location = new System.Drawing.Point(80, 56);
            this.textBoxNameDev.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxNameDev.Name = "textBoxNameDev";
            this.textBoxNameDev.Size = new System.Drawing.Size(265, 22);
            this.textBoxNameDev.TabIndex = 74;
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
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(4, 57);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(52, 18);
            this.label12.TabIndex = 53;
            this.label12.Text = "Name:";
            // 
            // APIwms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxDev);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "APIwms";
            this.Size = new System.Drawing.Size(435, 200);
            this.groupBoxDev.ResumeLayout(false);
            this.groupBoxDev.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxDev;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxNameDev;
        private System.Windows.Forms.CheckBox checkBoxUseDev;
        private System.Windows.Forms.Label label12;


    }
}
