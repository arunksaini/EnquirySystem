using Newtonsoft.Json.Linq;

namespace RestAPI.Common
{
    public class Entity
    {
        public Entity(string id, string type, string timestamp, string changes)
        {
            Id = int.Parse(id);
            Type = type;
            TimeStamp = long.Parse(timestamp);
            Changes = JObject.Parse(changes);
        }

        public Entity(int id, string type, long timestamp)
        {
            Id = id;
            Type = type;
            TimeStamp = timestamp;
        }

        public int Id { get; set; }
        public string Type { get; set; }
        public JObject Changes { get; set; }
        public long TimeStamp { get; set; }
    }
}