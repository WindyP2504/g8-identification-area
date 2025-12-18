using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace VTP_Induction.UI
{
    public partial class panelDevice : UserControl
    {
        private Bitmap pictureDeviceOff;
        private Bitmap pictureDeviceOn;
        private CDevice cDevice;
        private List<CDevice> lDevice;

        public panelDevice(CDevice device, Bitmap pictureDeviceOff, Bitmap pictureDeviceOn)
        {
            InitializeComponent();
            this.cDevice = device;
            this.labelDevName.Text = device.m_sName;
            this.pictureDeviceOff = pictureDeviceOff;
            this.pictureDeviceOn = pictureDeviceOn;
        }
        public panelDevice(List<CDevice> ldevice, Bitmap pictureDeviceOff, Bitmap pictureDeviceOn)
        {
            InitializeComponent();
            lDevice = new List<CDevice>();
            foreach (var item in ldevice)
            {
                lDevice.Add(item);
            }
            this.labelDevName.Text = ldevice[0].m_sName;
            this.pictureDeviceOff = pictureDeviceOff;
            this.pictureDeviceOn = pictureDeviceOn;
        }
        public void RefreshDev()
        {
            if (cDevice != null)
            {
                if (cDevice.m_bConnection)
                {
                    this.labelDevName.ForeColor = System.Drawing.Color.GreenYellow;
                    this.buttonDev.Image = pictureDeviceOn;
                }
                else
                {
                    this.labelDevName.ForeColor = System.Drawing.Color.Gray;
                    this.buttonDev.Image = pictureDeviceOff;
                }
            }
            else if (lDevice != null)
            {
                bool m_bConnection = true;
                foreach (var item in lDevice)
                {
                    m_bConnection &= item.m_bConnection;
                }
                if (m_bConnection)
                {
                    this.labelDevName.ForeColor = System.Drawing.Color.GreenYellow;
                    this.buttonDev.Image = pictureDeviceOn;
                }
                else
                {
                    this.labelDevName.ForeColor = System.Drawing.Color.Gray;
                    this.buttonDev.Image = pictureDeviceOff;
                }
            }
        }

        private void labelDevName_DoubleClick(object sender, EventArgs e)
        {
            if (cDevice != null)
            {
                if (cDevice.m_bConnection)
                {
                    if (MessageBox.Show("Do you want to disconnect " + cDevice.m_sName + "?", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    {
                        cDevice.Disconnect();
                    }
                }
                else
                {
                    if (MessageBox.Show("Do you want to connect " + cDevice.m_sName + "?", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    {
                        if (!cDevice.Connect())
                        {
                            MessageBox.Show("Connect fail!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }

        }

        private void labelDevName_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void labelDevName_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void labelDevName_SizeChanged(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            String s = lbl.Text;
            Font f = lbl.Font;
            //calculate the length of string
            Size txtSize = TextRenderer.MeasureText(s, f);
            this.Width = txtSize.Width + this.buttonDev.Width + 10;
        }

        private void buttonDev_DoubleClick(object sender, EventArgs e)
        {
            if (cDevice != null)
            {
                if (cDevice.m_sName.Contains("ROBOT") || cDevice.m_sName.Contains("IOMODULE") || cDevice.m_sName.Contains("MOTION") || cDevice.m_sName.Contains("PLC"))
                {
                    //Show UI IO
                    //frmIOSimulator frmIO = new frmIOSimulator(cDevice);
                    //frmIO.Show();
                }
            }
            else if (lDevice != null)
            {
                //frmMutilStatusDev frmDev = new frmMutilStatusDev(lDevice, this.pictureDeviceOff, this.pictureDeviceOn);
                //frmDev.Show();
            }
        }

        private void buttonDev_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void buttonDev_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }
    }
}
