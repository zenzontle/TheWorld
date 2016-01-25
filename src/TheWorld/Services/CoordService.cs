using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace TheWorld.Services
{
    public class CoordService
    {
        private ILogger<CoordService> _logger;

        public CoordService(ILogger<CoordService> logger)
        {
            _logger = logger;
        }

        public async Task<CoordServiceResult> Lookup(string location)
        {
            CoordServiceResult result = new CoordServiceResult()
            {
                Success = false,
                Message = "Undetermined failure while looking up coordinates"
            };

            //Lookup Coordinates
            string encodedName = WebUtility.UrlEncode(location);
            string bingKey = Startup.Configuration["AppSettings:BingKey"];
            string url = $"http://dev.virtualearth.net/REST/v1/Locations?q={encodedName}&key={bingKey}";

            HttpClient client = new HttpClient();

            string json = await client.GetStringAsync(url);

            //Read out the results
            //Fragile, might need to change if the Bing API changes
            JObject results = JObject.Parse(json);
            JToken resources = results["resourceSets"][0]["resources"];
            if (!resources.HasValues)
            {
                result.Message = $"Could not find '{location}' as a location";
            }
            else
            {
                var confidence = (string)resources[0]["confidence"];
                if (confidence != "High")
                {
                    result.Message = $"Could not find a confident match for '{location}' as a location";
                }
                else
                {
                    var coords = resources[0]["geocodePoints"][0]["coordinates"];
                    result.Latitude = (double)coords[0];
                    result.Longitude = (double)coords[1];
                    result.Success = true;
                    result.Message = "Success";
                }
            }

            return result;
        }
    }
}
