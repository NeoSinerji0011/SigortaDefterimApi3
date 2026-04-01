using SigortaDefterimV2API.Models;
using SigortaDefterimV2API.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Services
{
    public class CarService
    {
        private DataContext _context;

        public CarService(DataContext context)
        {
            _context = context;
        }

        public List<AracMarka> GetAracMarka()
        {
            var result = _context.AracMarka.Where(aracMarka => true).ToList();
            return result;
        }

        public List<AracTip> GetAracTip(string markaKodu)
        {
            var result = _context.AracTip.Where(aracTip => aracTip.MarkaKodu == markaKodu).ToList();
            return result;
        }

        public List<AracKullanimSekli> GetAracKullanimSekli()
        {
            var result = _context.AracKullanimSekli.Where(aracKullanımSekli => true).ToList();
            return result;
        }

        public List<AracKullanimTarzi> GetAracKullanimTarzi(Int16 kullanimSekliKodu)
        {
            var result = _context.AracKullanimTarzi.Where(aracKullanimTarzi => aracKullanimTarzi.KullanimSekliKodu == kullanimSekliKodu).ToList();
            return result;
        }
    }
}