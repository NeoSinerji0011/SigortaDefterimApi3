using Microsoft.AspNetCore.Mvc;
using SigortaDefterimV2API.Models.Inputs;
using SigortaDefterimV2API.Services;
using System;
using System.Collections.Generic;
using System.Web.Http;
using WinAuth;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;

namespace API.Controllers
{ 
    [ApiController]
    [Route("api/[controller]")]
    public class NeoConnectController : ControllerBase
    { 
        [HttpGet("get_code")]
        public IActionResult GetPolicies([FromQuery] NeoConnectInput input)
        {  
            return Ok(getAuthenticatorCode(input.SirketKod,input.SecretKey));
        }
        string getAuthenticatorCode(int SirketKod, string SecretKey)
        {
            try
            {
                switch (SirketKod)
                {
                    case SigortaSirketKod.ATLAS:
                    case SigortaSirketKod.BEREKET:
                    case SigortaSirketKod.SSDOGASIGORTA:
                    case SigortaSirketKod.ETHICA:
                    case SigortaSirketKod.GROUPAMA:
                    case SigortaSirketKod.HDI:
                    case SigortaSirketKod.NEOVA:
                    case SigortaSirketKod.TMTSİGORTA:
                    case SigortaSirketKod.TURKNIPPON:
                        return GuildWars(SecretKey);
                   
                    case SigortaSirketKod.ANA_SİGORTA:
                    case SigortaSirketKod.GRİ_SİGORTA:
                    case SigortaSirketKod.KORU:
                        return Microsoft(SecretKey);
                    
                    default:
                        return GuildWars(SecretKey);
                }

            }
            catch (Exception)
            { }

            return "";
        }
        string GuildWars(string secretKey)
        {
            try
            {
                GuildWarsAuthenticator authenticator = new GuildWarsAuthenticator();
                authenticator.Enroll(secretKey);
                return authenticator.CurrentCode;
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("Illegal character") != -1)
                {
                    return "Illegal character";
                }
            }
            return string.Empty;
        }
        string Microsoft(string secretKey)
        {
            try
            {
                MicrosoftAuthenticator authenticator = new MicrosoftAuthenticator();
                authenticator.Enroll(secretKey);
                return authenticator.CurrentCode;
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("Illegal character") != -1)
                {
                    return "Illegal character";
                }
            }
            return string.Empty;
        }

    }
    public class SigortaSirketKod
    {
        public const int HDI = 1;
        public const int AXA = 2;
        public const int MAPFRE = 3;
        public const int ANADOLU = 4;
        public const int CORPUS = 5;
        public const int AIG = 6;
        public const int AEGON = 7;
        public const int METLIFE = 8;
        public const int RAY = 9;
        public const int SOMPOJAPAN = 10;
        public const int TURKNIPPON = 11;
        public const int ACEEUROPEANGROUP = 12;
        public const int AK = 13;
        public const int ALLIANZ = 14;
        public const int ANKARA = 15;
        public const int ATRADIUS = 16;
        public const int UNICO = 17;
        public const int BNPPARIBASCARDIF = 18;
        public const int COFACE = 19;
        public const int DUBAISTARR = 20;
        public const int ERGO = 21;
        public const int EULERHERMES = 22;
        public const int EUREKO = 23;
        public const int EGE = 24;
        public const int GENERALI = 25;
        public const int GROUPAMA = 26;
        public const int GUNES = 27;
        public const int HALK = 28;
        public const int HUR = 29;
        public const int ISIK = 30;
        public const int LIBERTY = 31;
        public const int NEOVA = 32;
        public const int SBN = 33;
        public const int ZIRAAT = 34;
        public const int ZURICH = 35;
        public const int KORU = 36;
        // public const int DEMIR = 37;
        public const int ACIBADEMSAGLIKHAYAT = 38;
        public const int CIVHAYAT = 39;
        public const int GULFWEB = 40;
        public const int MAPFREGENELYASAM = 41;
        public const int NEWLIFEYASAM = 42;
        public const int AllianzHayatEmeklilik = 43;
        public const int ANADOLUHAYATEMEKLILIK = 44;
        public const int ASYAEMEKLILIKVEHAYAT = 45;
        public const int AXAHAYATEMEKLILIK = 46;
        public const int AVIVASAEMEKLILIKHAYAT = 47;
        public const int FINANSEMEKLILIKHAYAT = 48;
        public const int BNPPARIBASCARDIFEMEKLILIK = 49;
        public const int GarantiEmeklilikHayat = 50;
        public const int HALKHAYATEMEKLILIK = 51;
        public const int INGEMEKLILIK = 52;
        public const int VAKIFEMEKLILIK = 53;
        public const int ZIRAATHAYATEMEKLILIK = 54;
        public const int ORIENT = 55;
        public const int SSDOGASIGORTA = 56;
        public const int MAPFRETronWeb = 57;
        public const int SOMPOJAPANSFS = 58;
        public const int RAYSFS = 59;
        public const int MAPFREDASK = 60;
        public const int ANADOLUSWEEP = 61;
        public const int ANKARASFS = 62;
        public const int ETHICA = 63;
        public const int QUICK = 64;
        public const int SBMONLINE = 65;
        public const int ERGOESKI = 66;
        public const int UNİCOOKYANUS = 68;
        public const int BEREKET = 67;
        public const int EUREKOUZUN = 71;
        public const int ATLAS = 72;
        public const int MAGDEBURGER = 73;
        public const int BEREKETSİGORTA = 74;
        public const int QUICKSIGORTA = 75;
        public const int GENERALI_PORTAL = 85;
        public const int MAPFRE_PORTAL = 86;
        public const int TURKIYE = 92;
        public const int MAPFRE_KOLAY_TRAFIK = 93;
        public const int GRİ_SİGORTA = 94;
        public const int ANA_SİGORTA = 95;
        public const int MAGDEBURGERHIZLI = 96;
        public const int TMTSİGORTA = 91;
        public const int AREXSIGORTA = 97;
        public const int PRIVESIGORTA = 98;
        public const int AVEONGLOBAL = 99;
        public const int HEPIYISIGORTA = 100;

    }

}
