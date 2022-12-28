using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace rickAndMortyCharactersInfo.Models
{
    public class Person
    {
        public string Name { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public int Id { get; set; }

        public string Status { get; set; }

        public string Species { get; set; }

        public string Type { get; set; }

        public string Gender { get; set; }

        public Origin? Origin { get; set; }

    }
}
