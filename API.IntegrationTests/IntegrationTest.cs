using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SigortaDefterimV2API.Models;
using SigortaDefterimV2API.Models.Responses;
using SigortaDefterimV2API.Services;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.IntegrationTests
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;
        protected readonly TestServer server;

        protected IntegrationTest()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new WebHostBuilder()
                .UseStartup<TestStartup>()
                .UseConfiguration(config);

            server = new TestServer(builder);

            TestClient = server.CreateClient();

            //var appFactory = new WebApplicationFactory<Startup>();
            //TestClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        private async Task<string> GetJwtAsync()
        {
            string email = "test12@gmail.com";
            string password = "222222";

            var response = await TestClient.PostAsJsonAsync("/api/User/Register", new Kullanici
            {
                Adres = "Sükrüpasa Mahallesi Teknopark Binasi",
                Adsoyad = "Test Deneme",
                Durum = "1",
                Eposta = email,
                Guvenlik = "qerjtkktyrjhhjthr",
                Onaykodu = "152215",
                Resim = "noavatar.png",
                Sifre = password,
                Tc = "55551234565",
                Telefon = "555555555"
            });

            var registrationResponse = await response.Content.ReadAsAsync<RegisterResponse>();

            var login = await TestClient.PostAsync("/api/User/Login?Email=" + email + "&Password=" + password, null);

            var loginResponse = await login.Content.ReadAsAsync<LoginResponse>();
            return loginResponse.Token;
        }
    }
}