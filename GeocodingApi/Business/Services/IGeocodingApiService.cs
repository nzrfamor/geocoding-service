using Business.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IGeocodingApiService
    {
        Task<GeocodingModel> GetGeocodingDataByLatLngAsync(float lat, float lng);
        Task<GeocodingModel> GetGeocodingDataByAddressAsync(string address);
        Task<GeocodingModel> GetGeocodingDataByPlaceIdAsync(string placeId);

    }
}
