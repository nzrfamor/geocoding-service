using Business.Models;
using Business.Models.DataFromGoogleMaps;
using Enyim.Caching;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Business.Services
{
    public class GeocodingApiService : IGeocodingApiService
    {
        private readonly ILogger logger;
        private readonly IMemcachedClient memCache;
        private readonly HttpClient client;
        private readonly string googleKey = "AIzaSyDjwaDbUCconUi74AyiI_57SHZnk7ZMqKg";
        public GeocodingApiService(IHttpClientFactory clientFactory, ILogger<GeocodingApiService> _logger, IMemcachedClient _memCache)
        {
            client = clientFactory.CreateClient("GeocodingApi");
            logger = _logger;
            memCache = _memCache;
            logger.LogInformation("Initialization");
        }
        public async Task<GeocodingModel> GetGeocodingDataByAddressAsync(string address)
        {
            char[] separators = new char[] { ' ', ',' };
            string[] subs = address.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            string stringRequest = String.Join('+', subs);
            GeocodingModel geocodingModel = memCache.Get<GeocodingModel>(stringRequest.ToUpper());
            if (geocodingModel != null)
            {
                logger.LogInformation($"Geocoding data is taken from the cache");
                return geocodingModel;
            }
            else
            {
                logger.LogInformation($"Try to get Geocoding data from the Google Api");
                var url = string.Format($"/maps/api/geocode/json?address={stringRequest}&key={googleKey}");
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {

                    logger.LogInformation($"Geocoding data is taken from the Google Api");
                    var stringResponse = await response.Content.ReadAsStringAsync();

                    var resultData = JsonSerializer.Deserialize<Result>(stringResponse, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).results.FirstOrDefault();

                    geocodingModel = new GeocodingModel
                    {
                        FullAddress = resultData.formatted_address,
                        PlaceId = resultData.place_id,
                        Latitude = resultData.geometry.location["lat"],
                        Longitude = resultData.geometry.location["lng"]
                    };
                    memCache.Add(stringRequest.ToUpper(), geocodingModel, 600);
                    return geocodingModel;
                }
                else
                {
                    logger.LogInformation($"Failed to get data from Google Api: {response.ReasonPhrase}");
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }
        }

        public async Task<GeocodingModel> GetGeocodingDataByLatLngAsync(float lat, float lng)
        {
            GeocodingModel geocodingModel = memCache.Get<GeocodingModel>(lat.ToString() + "-" + lng.ToString());
            if (geocodingModel != null)
            {
                logger.LogInformation($"Geocoding data is taken from the cache");
                return geocodingModel;
            }
            else
            {
                logger.LogInformation($"Try to get Geocoding data from the Google Api");
                var url = string.Format($"/maps/api/geocode/json?latlng={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}&key={googleKey}");
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation($"Geocoding data is taken from the Google Api");
                    var stringResponse = await response.Content.ReadAsStringAsync();

                    var resultData = JsonSerializer.Deserialize<Result>(stringResponse, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).results.FirstOrDefault();

                    geocodingModel = new GeocodingModel
                    {
                        FullAddress = resultData.formatted_address,
                        PlaceId = resultData.place_id,
                        Latitude = resultData.geometry.location["lat"],
                        Longitude = resultData.geometry.location["lng"]
                    };

                    memCache.Add(lat.ToString() + "-" + lng.ToString(), geocodingModel, 600);
                    return geocodingModel;
                }
                else
                {
                    logger.LogInformation($"Failed to get data from Google Api: {response.ReasonPhrase}");
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }
        }

        public async Task<GeocodingModel> GetGeocodingDataByPlaceIdAsync(string placeId)
        {
            GeocodingModel geocodingModel = memCache.Get<GeocodingModel>(placeId);
            if (geocodingModel != null)
            {
                logger.LogInformation($"Geocoding data is taken from the cache");
                return geocodingModel;
            }
            else
            {
                logger.LogInformation($"Try to get Geocoding data from the Google Api");
                var url = string.Format($"/maps/api/geocode/json?place_id={placeId}&key={googleKey}");
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation($"Geocoding data is taken from the Google Api");
                    var stringResponse = await response.Content.ReadAsStringAsync();

                    var resultData = JsonSerializer.Deserialize<Result>(stringResponse, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).results.FirstOrDefault();

                    geocodingModel = new GeocodingModel
                    {
                        FullAddress = resultData.formatted_address,
                        PlaceId = resultData.place_id,
                        Latitude = resultData.geometry.location["lat"],
                        Longitude = resultData.geometry.location["lng"]
                    };

                    memCache.Add(placeId, geocodingModel, 600);
                    return geocodingModel;
                }
                else
                {
                    logger.LogInformation($"Failed to get data from Google Api: {response.ReasonPhrase}");
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }
        }
    }
}
