using FluentAssertions;
using SigortaDefterimV2API.IntegrationTests;
using SigortaDefterimV2API.Models.Database;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;

namespace API.IntegrationTests
{
    public class MobileAppControllerTests : IntegrationTest
    {
        [Fact]
        public async void GetLandingPages_ReturnsEmptyResponse_WithoutAnyLandingPages()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync("/api/MobileApp/GetLandingPages");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<MobilKarsilamaEkrani>>()).Should().BeEmpty();
        }

        [Fact]
        public async void GetSliderPages_ReturnsEmptyResponse_WithoutAnySliderPages()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync("/api/MobileApp/GetSliderPages");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<MobilKaydirmaEkrani>>()).Should().BeEmpty();
        }
    }
}