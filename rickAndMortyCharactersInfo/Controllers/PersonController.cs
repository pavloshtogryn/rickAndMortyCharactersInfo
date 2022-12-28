using Microsoft.AspNetCore.Mvc;
using rickAndMortyCharactersInfo.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using rickAndMortyCharactersInfo.Helper;

namespace rickAndMortyCharactersInfo.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        static HttpClient httpClient = new HttpClient();
        static readonly string baseUrlForPerson = @"https://rickandmortyapi.com/api/character/?name=";
        static readonly string baseUrlForEpisode = @"https://rickandmortyapi.com/api/episode/?name=";

        [Route("check-person")]
        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> CheckPersonInEpisode(string personName, string episodeName)
        {
            string urlForEpisode = baseUrlForEpisode + episodeName;

            HttpResponseMessage episodeResponse = await httpClient.GetAsync(urlForEpisode);

            if (!episodeResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)episodeResponse.StatusCode);
            }

           
            string urlForPerson = baseUrlForPerson + personName;

            HttpResponseMessage personResponse = await httpClient.GetAsync(urlForPerson);

            if (!personResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)personResponse.StatusCode);
            }

           
            Person personFromResponse = await personResponse.GetPersonFromFilteredResponse();

            bool personPresentInEpisode = await CharacterPresentInEpisode(episodeResponse, personFromResponse.Id);

            return personPresentInEpisode;
        }

        [Route("[controller]")]
        [HttpGet()]
        [ProducesResponseType(typeof(Person), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Person>> GetPersonInfoByName([FromQuery(Name = "name")] string personName)
        {
            string urlForPerson = baseUrlForPerson + personName;

            HttpResponseMessage personResponse = await httpClient.GetAsync(urlForPerson);

            if (!personResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)personResponse.StatusCode);
            }

            Person personFromResponse = await personResponse.GetPersonFromFilteredResponse();

            if(String.IsNullOrEmpty(personFromResponse.Origin.Url)) 
            {
                return personFromResponse;
            }
            HttpResponseMessage originResponse = await httpClient.GetAsync(personFromResponse.Origin.Url);

            string responseContent = string.Empty;
            using (var sr = new StreamReader(await originResponse.Content.ReadAsStreamAsync()))
            {
                responseContent = sr.ReadToEnd();
            }

            personFromResponse.Origin = JsonConvert.DeserializeObject<Origin>(responseContent);

            return personFromResponse;
        }


        private async Task<bool> CharacterPresentInEpisode(HttpResponseMessage episodeResponse, int characterId)
        {

            string responseContent = string.Empty;
            using (var sr = new StreamReader(await episodeResponse.Content.ReadAsStreamAsync()))
            {
                responseContent = sr.ReadToEnd();
            }

            JObject jsonObj = JObject.Parse(responseContent);

            FilteredApiResponseEpisode apiResponse = jsonObj.ToObject<FilteredApiResponseEpisode>();

            return apiResponse.Results[0].Characters.Any(x => Int32.TryParse(Regex.Match(x.ToString(), @"\/[0-9]+").ToString().Substring(1), out var curentCharcterId) && curentCharcterId == characterId);

        }
    }
}
