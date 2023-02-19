using Business.Models;
using Business.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using System.Threading.Tasks;
using System.Net.Http;
using Moq;
using Moq.Protected;
using System.Threading;
using Microsoft.Extensions.Logging;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeocodingApiTests.Services
{
    [TestFixture]
    public class GeocodingApiServiceTest
    {
        IGeocodingApiService geocodingService;
        [SetUp]
        public void SetUp()
        {
            //arrange
            var services = new ServiceCollection();
            services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
            services.AddHttpClient("GeocodingApi", c => c.BaseAddress = new Uri("https://maps.googleapis.com"));
            services.AddEnyimMemcached(setup => {
                setup.Servers.Add(new Server
                {
                    Address = "127.0.0.1",
                    Port = 11211
                });
            });
            services.AddScoped<IGeocodingApiService, GeocodingApiService>();
            var serviceProvider = services.BuildServiceProvider();

            geocodingService = serviceProvider.GetService<IGeocodingApiService>();
        }

        [TestCase(0,"Lviv")]
        [TestCase(0, "Ukraine, Lviv")]
        [TestCase(0, "lviv, ukraine")]
        [TestCase(1, "Kyiv")]
        [TestCase(1, "Ukraine, Kyiv")]
        [TestCase(1, "kyiv, ukraine")]
        [TestCase(2, "rivne")]
        [TestCase(2, "Ukraine, rivne")]
        [TestCase(2, "ukraine. Rivne")]
        public async Task GeocodingApiService_GetByAddress_ReturnsGeocodingModel(int id, string address)
        {
            //arrange
            var expected = GetTestGeocodingModelsByAddress[id];

            //act
            var actual = await geocodingService.GetGeocodingDataByAddressAsync(address);

            //assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase(0, 49.842003f, 24.031921f)]
        [TestCase(1, 50.44975f, 30.525293f)]
        [TestCase(2, 50.620346f, 26.251707f)]
        public async Task GeocodingApiService_GetByLatLng_ReturnsGeocodingModel(int id, float lat, float lng)
        {
            //arrange
            var expected = GetTestGeocodingModelsByLatLng[id];

            //act
            var actual = await geocodingService.GetGeocodingDataByLatLngAsync(lat, lng);

            //assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase(0, "ChIJV5oQCXzdOkcR4ngjARfFI0I")]
        [TestCase(1, "ChIJBUVa4U7P1EAR_kYBF9IxSXY")]
        [TestCase(2, "ChIJ14l8LapsL0cRhOTE26uBdAI")]
        public async Task GeocodingApiService_GetByPlaceId_ReturnsGeocodingModel(int id, string placeId)
        {
            //arrange
            var expected = GetTestGeocodingModelsByAddress[id];

            //act
            var actual = await geocodingService.GetGeocodingDataByPlaceIdAsync(placeId);

            //assert
            actual.Should().BeEquivalentTo(expected);
        }

        public List<GeocodingModel> GetTestGeocodingModelsByAddress =>
             new List<GeocodingModel>()
             {
                 new GeocodingModel
                 {
                     FullAddress = "Lviv, Lviv Oblast, Ukraine, 79000",
                     PlaceId = "ChIJV5oQCXzdOkcR4ngjARfFI0I",
                     Latitude = 49.839684f,
                     Longitude = 24.029716f
                 },

                 new GeocodingModel
                 {
                     FullAddress = "Kyiv, Ukraine, 02000",
                     PlaceId = "ChIJBUVa4U7P1EAR_kYBF9IxSXY",
                     Latitude = 50.4501f,
                     Longitude = 30.5234f
                 },

                 new GeocodingModel
                 {
                     FullAddress = "Rivne, Rivne Oblast, Ukraine",
                     PlaceId = "ChIJ14l8LapsL0cRhOTE26uBdAI",
                     Latitude = 50.6199f,
                     Longitude = 26.251617f
                 },
             };
        public List<GeocodingModel> GetTestGeocodingModelsByLatLng =>
             new List<GeocodingModel>()
             {
                 new GeocodingModel
                 {
                     FullAddress = "пл. Ринок, 1, кімн. 327, L'viv, L'vivs'ka oblast, Ukraine, 79000",
                     PlaceId = "ChIJ6ctoqW3dOkcRK_a570Ojqfg",
                     Latitude = 49.842003f,
                     Longitude = 24.031921f
                 },

                 new GeocodingModel
                 {
                     FullAddress = "Khreschatyk St, 17, Kyiv, Ukraine, 02000",
                     PlaceId = "ChIJI46RylbO1EAR9PW4GEFOxJw",
                     Latitude = 50.44975f,
                     Longitude = 30.525293f
                 },

                 new GeocodingModel
                 {
                     FullAddress = "Maydan Nezalezhnosti, 2, Rivne, Rivnens'ka oblast, Ukraine, 33000",
                     PlaceId = "ChIJadDVblITL0cRs8ND5dgcJsQ",
                     Latitude = 50.620346f,
                     Longitude = 26.251707f
                 },
             };
    }

}
