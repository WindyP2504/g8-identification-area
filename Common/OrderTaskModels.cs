using System.Collections.Generic;
using Newtonsoft.Json;

public class OrderTaskRequest
{
    [JsonProperty("order_id")]
    public long OrderId { get; set; }

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
}
