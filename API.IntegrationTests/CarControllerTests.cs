using FluentAssertions;
using SigortaDefterimV2API.IntegrationTests;
using SigortaDefterimV2API.Models.Database;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace API.IntegrationTests
{
    public class CarControllerTests : IntegrationTest
    {
        [Fact]
        public async void GetAracMarka_ReturnsEmptyResponse_WithoutAnyAracMarkas()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync("/api/Car/GetAracMarka");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<AracMarka>>()).Should().BeEmpty();
        }

        [Fact]
        public async void GetAracTip_ReturnsNotFound_WithoutAnyAracTips()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync("/api/Car/GetAracTip?MarkaKodu=003");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void GetAracKullanimSekli_ReturnsEmptyResponse_WithoutAnyAracKullanimSeklis()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync("/api/Car/GetAracKullanimSekli");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<AracKullanimSekli>>()).Should().BeEmpty();
        }

        [Fact]
        public async void GetAracKullanimTarzi_ReturnsNotFound_WithoutAnyAracKullanimTarzis()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync("/api/Car/GetAracKullanimTarzi?KullanimSekliKodu=0");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}