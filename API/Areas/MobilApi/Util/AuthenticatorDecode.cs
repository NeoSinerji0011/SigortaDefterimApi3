using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
//using WinAuth;
using ZXing;
using ZXing.Common;

namespace API.Areas.MobilApi.Util
{
    public class AuthenticatorDecode
    {

        private void AcenteListesi()
        {

            var list = new List<Insurance>();
            foreach (var insurance in Enum.GetValues(typeof(InsuranceType)))
            {
                var _enum = (InsuranceType)Enum.Parse(typeof(InsuranceType), insurance.ToString());
                list.Add(new Insurance
                {
                    Name = _enum.ToString(),
                    Type = _enum
                });
            }
        }
        private void CodeGenerator(InsuranceType insuranceType, string secretKey)
        {
            CodeGenerate(insuranceType, secretKey);
        }
        public string ParseQrCode(Bitmap path)
        {
            string result = "";
            try
            {
                using var barcodeBitmap = path;
                var reader = new BarcodeReader
                {
                    AutoRotate = true,
                    TryInverted = true,
                    Options = new DecodingOptions { TryHarder = true }
                };

                var res = reader.Decode(barcodeBitmap);
                var pattern = new Regex("secret=([A-Z].*?)[&|}]");
                if (pattern.IsMatch(res.Text))
                {
                    result = pattern.Match(res.Text).Groups[1].Value;
                }
                else
                {
                    result = "Kod Bulunamadı";
                }
                return result;
            }
            catch (Exception ex)
            {
               return result = ex.Message;
            } 
        }
        public string btnGoogleQrCodeResolve(string path)
        {
            try
            {
                var barcodeBitmap = (Bitmap)Bitmap.FromFile(path);
                return QrCodeResolve(barcodeBitmap);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string QrCodeResolve(Bitmap barcodeBitmap)
        {
            string result = string.Empty;

            BarcodeReader reader = new BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new DecodingOptions { TryHarder = true }
            };

            Result sonuc = reader.Decode(barcodeBitmap);

            if (sonuc != null && sonuc.Text != null && sonuc.Text.StartsWith("otpauth-migration://offline"))
            {
                // var psi = new ProcessStartInfo(@"C:\Users\muhac\go\bin\otpauth.exe", $"-link \"{sonuc.Text}\"");
                var pathOtPauth = Environment.CurrentDirectory + "\\go\\otpauth.exe";
                var inputData = $"-link \"{sonuc.Text}\"";
                var psi = new ProcessStartInfo(pathOtPauth, inputData);
                psi.RedirectStandardOutput = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                psi.WorkingDirectory = @"";
                var proc = Process.Start(psi);

                var myOutput = proc.StandardOutput;
                proc.WaitForExit(2000);
                if (proc.HasExited)
                {
                    string output = myOutput.ReadToEnd();

                    var lines = output.Split("\n");

                    foreach (var item in lines)
                    {
                        if (string.IsNullOrEmpty(item?.Trim())) continue;
                        var match = Regex.Match(item, @"otpauth:/\/totp\/(.*?):.*?secret=(.*)");
                        result = HttpUtility.UrlDecode(match.Groups[1].Value) + " - " + match.Groups[2] + Environment.NewLine;
                    }
                }
            }
            else if (sonuc == null && (barcodeBitmap.Width > 1500 && barcodeBitmap.Height > 1500))
            {
                Bitmap resized = new Bitmap(barcodeBitmap, new Size(barcodeBitmap.Width / 2, barcodeBitmap.Height / 2));
                QrCodeResolve(resized);
            }
            else
            {
                Console.WriteLine("Kare kod okumasından gelen kod Google Authenticator uygulamasına ait değil. Okunan değer : " + sonuc?.Text);
            }
            return result;
        }
        string CodeGenerate(InsuranceType type, string secretKey)
        {
            switch (type)
            {
                case InsuranceType.AtlasSigorta:
                case InsuranceType.BereketSigorta:
                case InsuranceType.CorpusSigorta:
                case InsuranceType.DogaSigorta:
                case InsuranceType.EthicaSigorta:
                case InsuranceType.GroupamaSigorta:
                case InsuranceType.HdiSigorta:
                case InsuranceType.NeovaSigorta:
                case InsuranceType.TmtSigorta:
                case InsuranceType.TurkNipponSigorta:
                    return GuildWars(secretKey);
                case InsuranceType.AnaSigorta:
                case InsuranceType.GriSigorta:
                case InsuranceType.KoruSigorta:
                    return Microsoft(secretKey);
                default:
                    return GuildWars(secretKey);
            }
        }
        string Microsoft(string secretKey)
        {
            //try
            //{
            //    MicrosoftAuthenticator authenticator = new MicrosoftAuthenticator();
            //    authenticator.Enroll(secretKey);
            //    return authenticator.CurrentCode;
            //}
            //catch (Exception ex)
            //{
            //    if (ex.Message.IndexOf("Illegal character") != -1)
            //    {
            //        return "Illegal character";
            //    }
            //}
            return string.Empty;
        }
        string GuildWars(string secretKey)
        {
            //try
            //{
            //    GuildWarsAuthenticator authenticator = new GuildWarsAuthenticator();
            //    authenticator.Enroll(secretKey);
            //    return authenticator.CurrentCode;
            //}
            //catch (Exception ex)
            //{
            //    if (ex.Message.IndexOf("Illegal character") != -1)
            //    {
            //        return "Illegal character";
            //    }
            //}
            return string.Empty;
        }
    }
    class Insurance
    {
        public InsuranceType Type { get; set; }
        public string Name { get; set; }
    }
    public enum InsuranceType
    {
        AtlasSigorta,
        BereketSigorta,
        CorpusSigorta,
        DogaSigorta,
        EthicaSigorta,
        GroupamaSigorta,
        HdiSigorta,
        NeovaSigorta,
        TmtSigorta,
        TurkNipponSigorta,
        AnaSigorta,
        GriSigorta,
        KoruSigorta
    }
}
