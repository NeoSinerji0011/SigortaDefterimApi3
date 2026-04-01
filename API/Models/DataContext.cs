using API.Models.Database;
using Microsoft.EntityFrameworkCore;
using SigortaDefterimV2API.Models.Database;

namespace SigortaDefterimV2API.Models
{
    public partial class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<TVMKullanicilar> TVMKullanicilar { get; set; }
        public DbSet<Kullanici> Kullanici { get; set; }
        public DbSet<KullaniciSifremiUnuttum> KullaniciSifremiUnuttum { get; set; }
        public DbSet<MobilKarsilamaEkrani> MobilKarsilamaEkrani { get; set; }
        public DbSet<MobilKaydirmaEkrani> MobilKaydirmaEkrani { get; set; }
        public virtual DbSet<MobilTeklifPolice> MobilTeklifPolice { get; set; }
        public DbSet<AracMarka> AracMarka { get; set; }
        public DbSet<AracTip> AracTip { get; set; }
        public DbSet<AracKullanimTarzi> AracKullanimTarzi { get; set; }
        public DbSet<AracKullanimSekli> AracKullanimSekli { get; set; }
        public DbSet<AracModel> AracModel { get; set; }
        public DbSet<MobilPoliceHasar> MobilPoliceHasar { get; set; }
        public DbSet<Il> Il{ get; set; }
        public DbSet<Ilce> Ilce{ get; set; }
        public DbSet<SigortaSirketleri> SigortaSirketleri { get; set; }
        public DbSet<MobilPoliceDosya> MobilPoliceDosya { get; set; }
        public DbSet<Brans> Brans{ get; set; }
        public DbSet<Meslek> Meslek{ get; set; }
        public DbSet<MobilAcente> MobilAcente{ get; set; }
        public DbSet<MobilUlke> MobilUlke { get; set; }
        public DbSet<MobilUlkeTipi> MobilUlkeTipi { get; set; }
        public DbSet<PoliceSigortali> PoliceSigortali { get; set; }
        public DbSet<PoliceArac> PoliceArac { get; set; }
        public DbSet<PoliceGenel> PoliceGenel { get; set; }
        public DbSet<TVMDetay> TVMDetay { get; set; }
        public DbSet<Bildirim> Bildirim { get; set; }
        public DbSet<MobilIletisim> MobilIletisim { get; set; }
        public DbSet<MobilMessage> MobilMessage { get; set; }
        public DbSet<MobilMessageDosya> MobilMessageDosya { get; set; }
        public DbSet<MobilMessageOturum> MobilMessageOturum { get; set; }
    }
}