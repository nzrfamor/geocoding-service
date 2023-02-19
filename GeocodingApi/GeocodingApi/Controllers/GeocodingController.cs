using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GeocodingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeocodingController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IGeocodingApiService geocodingService;

        public GeocodingController(IGeocodingApiService _geocodingService, ILogger<GeocodingController> _logger)
        {
            geocodingService = _geocodingService;
            logger = _logger;
            logger.LogInformation("Initialization");
        }

        // GET: api/geocoding/address?address={}
        [HttpGet("/address")]
        public async Task<ActionResult<GeocodingModel>> GetByAddress([FromQuery] string address)
        {
            try
            {
                logger.LogInformation($"Try to get Geocoding data by address: {address}");
                return new JsonResult(await geocodingService.GetGeocodingDataByAddressAsync(address));
            }
            catch(Exception ex)
            {
                logger.LogCritical(ex, "Failed to get Geocoding data");
                return BadRequest();
            }
        }

        // GET: api/geocoding/latlng?lat={}&lng={}
        [HttpGet("/lat-lng")]
        public async Task<ActionResult<GeocodingModel>> GetByLatLng([FromQuery] float lat, [FromQuery] float lng)
        {
            try
            {
                logger.LogInformation($"Try to get Geocoding Data by latitude and Longitude: {lat} , {lng}");
                return new JsonResult(await geocodingService.GetGeocodingDataByLatLngAsync(lat, lng));
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Failed to get Geocoding data");
                return BadRequest();
            }
        }

        // GET: api/geocoding/placeId?placeId={}
        [HttpGet("/placeId")]
        public async Task<ActionResult<GeocodingModel>> GetByPlaceId([FromQuery] string placeId)
        {
            try
            {
                logger.LogInformation($"Try to get Geocoding Data by Place Id: {placeId}");
                return new JsonResult(await geocodingService.GetGeocodingDataByPlaceIdAsync(placeId));
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Failed to get Geocoding data");
                return BadRequest();
            }
        }
    }
}
