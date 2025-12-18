using System;
using System.Windows.Forms;

namespace VTP_Induction
{
    public partial class frmRobotWarning : Form
    {
        public frmRobotWarning()
        {
            InitializeComponent();
            ReloadUserLanguage();
            this.AcceptButton = button1;
        }

        public frmRobotWarning(string s)
        {
            InitializeComponent();
            ReloadUserLanguage();
            this.label2.Text = s;
            this.AcceptButton = button1;
        }

        public frmRobotWarning(string s, string caption, bool isOkBtnEnable = true, bool isCancelBtnEnable = true)
        {
            InitializeComponent();
            ReloadUserLanguage();

            this.label2.Text = s;
            this.label1.Text = caption;
            this.AcceptButton = button1;
            if (isOkBtnEnable)
            {
                button1.Visible = true;
            }
            else
            {
                button1.Visible = false;
            }

            if (isCancelBtnEnable)
            {
                button2.Visible = true;
            }
            else
            {
                button2.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ReloadUserLanguage()
        {
            this.label1.Text = "Request to initialize robot!";
            this.label2.Text = "Do you want to initialize Robot?";
            this.button1.Text = "[&OK]";
            this.button2.Text = "[&CANCEL]";
        }


    }
}
