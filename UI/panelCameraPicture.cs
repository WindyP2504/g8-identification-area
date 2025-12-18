using System.Drawing;
using System.Windows.Forms;

namespace VTP_Induction.UI
{
    public partial class panelCameraPicture : UserControl
    {
        public PictureBox PicImgCamera;
        public panelCameraPicture(int Index)
        {
            InitializeComponent();
            panelCameraPictureBox.Controls.Clear();
            PicImgCamera = new PictureBox();
            PicImgCamera.SizeMode = PictureBoxSizeMode.StretchImage;
            PicImgCamera.Dock = DockStyle.Fill;

            panelCameraPictureBox.Controls.Add(PicImgCamera);

        }
        public void AddImage(Bitmap PictureBitMap)
        {

            PicImgCamera.Image = PictureBitMap;
        }
        public void CameraPanelResize()
        {
            // panelCameraPicture.Size = new Size(width, height);
        }
    }
}
