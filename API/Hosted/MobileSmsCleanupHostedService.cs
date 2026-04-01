using System;
using System.Threading;
using System.Threading.Tasks;
using API.Areas.MobilApi.Helper;
using API.Areas.MobilApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SigortaDefterimV2API.Hosted
{
    /// <summary>
    /// MobileSms kayıtlarını SmsTarihi (TR saati) üzerinden yaklaşık 1 dakika sonra siler.
    /// </summary>
    public class MobileSmsCleanupHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan Retention = TimeSpan.FromMinutes(1);

        public MobileSmsCleanupHostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                        var threshold = Utils.getTRDateTime().Subtract(Retention);
                        await db.Database.ExecuteSqlRawAsync(
                            "DELETE FROM MobileSms WHERE SmsTarihi < {0}",
                            threshold);
                    }
                }
                catch
                {
                    // Sessiz: tablo/sütun yoksa veya geçici DB hatası uygulamayı düşürmemeli
                }

                try
                {
                    await Task.Delay(CheckInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }
}
