namespace R3MUS.Devpack.SlackRTMBot
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class eveapi
    {
        /// <remarks/>
        public string currentTime { get; set; }

        /// <remarks/>
        public eveapiResult result { get; set; }

        /// <remarks/>
        public string cachedUntil { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte version { get; set; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class eveapiResult
    {
        /// <remarks/>
        public string serverOpen { get; set; }

        /// <remarks/>
        public ushort onlinePlayers { get; set; }
    }
}
