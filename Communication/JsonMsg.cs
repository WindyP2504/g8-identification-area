using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VTP_Induction
{

    public class JsonMsg
    {
        public struct JsonMsg201
        {
            public JsonMsg201_body data;
        }

        public struct JsonMsg201_body
        {
            public string getBillPost;
        }

        public static string getJsonData201(string jsonMsg)
        {
            JObject jObject = (JObject)JsonConvert.DeserializeObject(jsonMsg);
            JObject jObject2 = (JObject)JsonConvert.DeserializeObject(jObject["data"].ToString());
            return jObject2["getBillPost"].ToString();
        }
    }
}