using System;
namespace VTP_Induction.Common
{
    public class ParcelLabelData
    {
        public string ParcelCode { get; set; }
        public string ItemName { get; set; }
        public string PalletCode { get; set; }
        public string InnerCtn { get; set; }
        public string Weight { get; set; }
        public DateTime CreatedTime { get; set; }
        public int CartonNo { get; set; }
    }
}
