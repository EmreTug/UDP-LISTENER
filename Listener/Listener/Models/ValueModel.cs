
using Newtonsoft.Json;
using System.Collections.Generic;
namespace Listener.Models
{
    
    /*
     json içerisindeki verileri eşitlemek için model tanımlanır.
     https://app.quicktype.io/ kullanılmıştır.
     */

    public partial class ValueModel
    {
        [JsonProperty("IP")]
        public string Ip { get; set; }

        [JsonProperty("PORT")]
        public int Port { get; set; }

        [JsonProperty("PACKETS")]
        public List<Packet> Packets { get; set; }

    }

    public partial class Packet
    {
        [JsonProperty("PacketName")]
        public string PacketName { get; set; }
        [JsonProperty("ID")]
        public int id { get; set; }

        [JsonProperty("Values")]
        public List<Value> Values { get; set; }
    }

    public partial class Value
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public ValueUnion ValueValue { get; set; }
    }

    public partial struct ValueUnion
    {
        public bool? Bool;
        public int? Integer;
        public string String;

        public static implicit operator ValueUnion(bool Bool) => new ValueUnion { Bool = Bool };
        public static implicit operator ValueUnion(int Integer) => new ValueUnion { Integer = Integer };
        public static implicit operator ValueUnion(string String) => new ValueUnion { String = String };
    }



}


