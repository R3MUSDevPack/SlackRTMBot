using Newtonsoft.Json;

namespace R3MUS.Devpack.SlackRTMBot
{
    public class MessageRx : BaseType
    {
        public string channel { get; set; }
        [JsonIgnore]
        public string Channel { get; set; }
        public string user { get; set; }
        [JsonIgnore]
        public string User { get; set; }
        public string text { get; set; }
        public string ts { get; set; }
        public string source_team { get; set; }
        //public string type { get; set; }
        public string team { get; set; }
    }

    public class MessageTx : BaseType
    {
        public int id { get; set; }
        public string channel { get; set; }
        //public string type { get; set; }
        public string text { get; set; }
    }

}
