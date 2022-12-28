using Newtonsoft.Json;

namespace rickAndMortyCharactersInfo.Models
{
    public class Origin
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Dimension { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string Url { get; set; }
    }
}
