using API.Models.Database;
using FluentAssertions;
using SigortaDefterimV2API.Models.Inputs;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SigortaDefterimV2API.IntegrationTests
{
    public class PolicyControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetPolicies_ReturnsEmptyResponse_WithoutAnyPolicies()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync("/api/Policy/GetPolicies?KimlikNo=32132132132&BransKodu=4");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<MobilTeklifPolice>>()).Should().BeEmpty();
        }

        [Fact]
        public async Task GetPolicy_ReturnsNotFoundResponse_WithoutAnyPolicies()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync("/api/Policy/GetPolicy?PolicyId=14");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void UpdateCarInfo_ReturnsNotFound_WithoutAnyPolicies()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.PutAsJsonAsync("/api/Policy/UpdateCarInfo", new UpdateCarInfoInput
            {
                PolicyId = 13,
                ModelYili = 2010,
                MarkaKodu = "061",
                TipKodu = "096",
                AracKullanimTarzi = "111+10"
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}