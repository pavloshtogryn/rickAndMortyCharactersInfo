using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using rickAndMortyCharactersInfo.Models;

namespace rickAndMortyCharactersInfo.Helper
{
    public static class PersonHelper
    {
        public static async Task<Person> GetPersonFromFilteredResponse(this HttpResponseMessage response)
        {
            string responseContent = string.Empty;
            using (var sr = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                responseContent = sr.ReadToEnd();
            }

            JObject jsonObj = JObject.Parse(responseContent);

            FilteredApiResponsePerson apiResponse = jsonObj.ToObject<FilteredApiResponsePerson>();

            return apiResponse.Results[0];
        }
    }
}
