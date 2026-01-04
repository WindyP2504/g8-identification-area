using System.Collections.Generic;
using Newtonsoft.Json;

public class OrderTaskRequest
{
    [JsonProperty("Task_ID")]
    public string Task_ID { get; set; }

    [JsonProperty("PO_ID")]
    public long PO_ID { get; set; }

    [JsonProperty("WH_Code")]
    public string WH_Code { get; set; }

    [JsonProperty("Line_ID")]
    public string LineId { get; set; }

    [JsonProperty("info_detail")]
    public List<InforDetail> InforDetail { get; set; }

    [JsonProperty("fromSystem")]
    public string FromSystem { get; set; }
}

public class InforDetail
{
    [JsonProperty("Item_Code")]
    public string ItemCode { get; set; }

    [JsonProperty("Pallet_ID")]
    public string PalletId { get; set; }

    [JsonProperty("Location")]
    public string Location { get; set; }

    [JsonProperty("Carton_List")]
    public List<string> CartonList { get; set; }

    [JsonProperty("Ctn")]
    public int Ctn { get; set; }

    [JsonProperty("Qty")]
    public int Qty { get; set; }

    [JsonProperty("Pcs")]
    public int Pcs { get; set; }

    [JsonProperty("Inner_Carton")]
    public int Inner_Carton { get; set; }

    [JsonProperty("Inner_Pallet")]
    public int Inner_Pallet{ get; set; }

    [JsonProperty("Weight")]
    public int Weight { get; set; }
}

public class DonePalletRequest
{
    public long PO_ID { get; set; }
    public string Line_ID { get; set; }
    public string Pallet_ID { get; set; }
    public string Location { get; set; }
    public string fromSystem { get; set; }
}

