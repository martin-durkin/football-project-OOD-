using Newtonsoft.Json;
using System.Collections.Generic;

namespace football_project
{
    public class SquadResponse
    {

        //[JsonProperty("players")] tells Newtonsoft.Json "when you see a key called players in the JSON,
        //map it to this property"
        
        [JsonProperty("players")]
        public List<SquadPlayerWrapper> Players { get; set; }


        [JsonProperty("manager")]
        public SquadManager Manager { get; set; }
    }


    public class SquadPlayerWrapper
    {

        [JsonProperty("player")]
        public SquadPlayer Player { get; set; }
    }

    public class SquadPlayer
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }
    }
    public class SquadManager
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("country")]
        public SquadManagerCountry Country { get; set; }
    }
    public class SquadManagerCountry
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }


}