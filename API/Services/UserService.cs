using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SigortaDefterimV2API.Helpers;
using SigortaDefterimV2API.Models;
using SigortaDefterimV2API.Services;
using SigortaDefterimV2API.Models.Database;
using SigortaDefterimV2API.Models.Responses;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using static SigortaDefterimV2API.Helpers.Constants;
using static SigortaDefterimV2API.Helpers.ExtensionMethods;
using System.Collections.Generic;
using System.IO;
using API.Models.Database;
using API.Models.Responses;

namespace SigortaDefterimV2API.Services
{
    public class UserService
    {
        private DataContext _context;
        private readonly AppSettings _appSettings;
        JwtSecurityTokenHandler tokenHandler;
        byte[] key;

        public UserService(DataContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
            tokenHandler = new JwtSecurityTokenHandler();
            key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        }

        public Kullanici GetKullanici(string email)
        {
            return _context.Kullanici.Where(kullanici => kullanici.Eposta == email).FirstOrDefault();
        }

        public LoginResponse Login(LoginInput user)
        {
            user.Password = Encryption.getMd5Hash(user.Password); // Get password hash
            Kullanici _user = _context.Kullanici.Where(kullanici => kullanici.Eposta == user.Email && kullanici.Sifre == user.Password && kullanici.Durum == "1").FirstOrDefault();

            var resToken = createNewToken();
            if (_user != null)
            {
                _user.Guvenlik = resToken;
                _context.SaveChanges();
                _user.Sifre = null; // Don't return user Password hash
            }
            return new LoginResponse { kullanici = _user, Token = resToken };
        }
        string createNewToken()
        {
             tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public RegisterResponse Register(Kullanici kullanici)
        {
            RegisterResponse response = new RegisterResponse();
            kullanici.Sifre = Encryption.getMd5Hash(kullanici.Sifre);

            Kullanici _kullanici = _context.Kullanici.Where(user => user.Eposta == kullanici.Eposta).FirstOrDefault();
            if (_kullanici != null) // if Email exists in database
            {
                response.statusCode = RegisterStatus.EmailExists;
                response.message = "Bu Email adresi ile daha önce kayıt olunmuş!";
            }
            else
            {
                _kullanici = _context.Kullanici.Where(user => user.Tc == kullanici.Tc).FirstOrDefault();
                if (_kullanici != null) // If Tc exists in database.
                {
                    response.statusCode = RegisterStatus.TcExists;
                    response.message = "Bu Tc No ile daha önce kayıt olunmuş!";
                }
                else // If Email and Tc doesnt exist save it to database
                {
                    response.statusCode = RegisterStatus.Success;
                    response.message = "Üyelik Oluşturuldu.\nÜyelik işlemini tamamlamak için emailinizi kontrol ediniz.";
                    if (!string.IsNullOrEmpty(kullanici.Resim))
                        kullanici.Resim = new UtilityService(3).fileUrl(kullanici.Resim)[0];
                    else
                        kullanici.Resim = "https://sigortadefterimv2api.azurewebsites.net/Userphoto/noavatar.png";
                    kullanici.Guvenlik = Guid.NewGuid().ToString();
                    kullanici.Durum = "0";
                    _context.Kullanici.Add(kullanici);
                    _context.SaveChanges();
                    kullanici.Sifre = "";
                    response.kullanici = kullanici;
                    string subject = "SigortaDefterim Yeni Üyelik Aktivasyon";
                    string body = "Sn. " + kullanici.Adsoyad + ",<br>";
                    body += "Uygulamamıza Hoşgeldiniz.sigortadefterim.com üyeliği ücretsiz olup, hesabınız başarıyla oluşturulmuştur.<br>";
                    body += "Poliçelerinizi yaptırdığınız acenteleriniz portalımıza üye ise,üye acenteler üzerinden yaptırdığınız tüm poliçe kayıtlarınızı otomatik olarak görebileceksiniz.<br>";
                    body += "Diğer durumda, poliçe bilgilerinizi alt menüde ki <b>(+)</b> butonuna basarak <b>Poliçe Ekle</b> adımından kendiniz girerek poliçelerinizi takip edebilir ve uygulamanın tüm imkanlarından yararlanabilirsiniz.Lütfen uygulamamızı puanlamayı unutmayınız.<br>";
                    body += "E-posta adresinizi doğrulamak için e-posta Doğrula linkine tıklayınız. <br>";
                    body += "Bağlantı çalışmazsa, aşağıdaki bağlantıyı kopyalayıp doğrudan bir web tarayıcısına yapıştırınız: <br> <br>";
                    body += "https://sigortadefterimv2api.azurewebsites.net/api/User/ConfirmRegister?Token=" + kullanici.Guvenlik + "<br> <br> <br>";
                    body += "Saygılarımızla <br> <br> <br>";
                    body += "<img src=\"https://sigortadefterimv2api.azurewebsites.net/Images/logosmall.png\"/  width=\"250\" height=\"55\"> <br>";
                    body += "Neosinerji Bilgi Teknolojileri A.Ş hizmetidir";

                    UtilityService.SendEmail(kullanici.Eposta, subject, body, new List<Attachment>(), true);
                }
            }

            return response;
        }

        public Message SendFeedBack(UserResponse UserResponse)
        {
            Message response = new Message();
            response.statusCode = MessageStatus.Success;
            response.message = "E-Mailiniz Gönderilmiştir";
            try
            {
                UtilityService.FeedBackMail(UserResponse, true);
            }
            catch (Exception)
            {
                response.statusCode = MessageStatus.Failed;
                response.message = "Mail gönderilirken bir hata oluştu lütfen tekrar deneyiniz";
            }

            return response;
        }
        public Message WebSiteContact(UserResponse UserResponse)
        {
            Message response = new Message();
            response.statusCode = MessageStatus.Success;
            response.message = "E-Mailiniz Gönderilmiştir";
            try
            {
                UtilityService.FeedBackMail(UserResponse, true,true);
            }
            catch (Exception)
            {
                response.statusCode = MessageStatus.Failed;
                response.message = "Mail gönderilirken bir hata oluştu lütfen tekrar deneyiniz";
            }

            return response;
        }
        public RegisterResponse UserUpdate(Kullanici kullanici)
        {
            RegisterResponse response = new RegisterResponse();
            response.statusCode = RegisterStatus.Failed;
            response.message = "Güncelleme yapılırken bir hata oluştu lütfen tekrar deneyiniz.";
            Kullanici _kullanici = _context.Kullanici.Where(user => user.Tc == kullanici.Tc).FirstOrDefault();
            if (_kullanici != null) // if Email exists in database
            {
                if (!string.IsNullOrEmpty(kullanici.Tc_Es))
                    _kullanici.Tc_Es = kullanici.Tc_Es;
                if (!string.IsNullOrEmpty(kullanici.Tc_Cocuk))
                    _kullanici.Tc_Cocuk = kullanici.Tc_Cocuk;
                if (!string.IsNullOrEmpty(kullanici.Tc_Diger))
                    _kullanici.Tc_Diger = kullanici.Tc_Diger;
                if (!string.IsNullOrEmpty(kullanici.Telefon))
                    _kullanici.Telefon = kullanici.Telefon;
                if (!string.IsNullOrEmpty(kullanici.gsm_1))
                    _kullanici.gsm_1 = kullanici.gsm_1;
                if (!string.IsNullOrEmpty(kullanici.gsm_2))
                    _kullanici.gsm_2 = kullanici.gsm_2;
                if (!string.IsNullOrEmpty(kullanici.Adres))
                    _kullanici.Adres = kullanici.Adres;
                if (!string.IsNullOrEmpty(kullanici.Resim))
                {
                    new UtilityService(3).deleteUserPhoto(_kullanici.Resim);
                    _kullanici.Resim = new UtilityService(3).fileUrl(kullanici.Resim)[0];
                }
                if (!string.IsNullOrEmpty(kullanici.Sifre))
                {
                    kullanici.Sifre = Encryption.getMd5Hash(kullanici.Sifre);
                    _kullanici.Sifre = kullanici.Sifre;
                }
                _context.SaveChanges();
                response.statusCode = RegisterStatus.Success;
                response.message = "Güncelleme yapıldı.";
                response.kullanici = _kullanici;
            }

            return response;
        }
        public byte ConfirmRegister(string Token)
        {
            Kullanici kullanici = _context.Kullanici.Where(x => x.Guvenlik == Token).SingleOrDefault();
            if (kullanici != null)
            {
                kullanici.Durum = "1";
                kullanici.Guvenlik = Guid.NewGuid().ToString();
                int result = _context.SaveChanges();
                return result > 0 ? (byte)1 : (byte)2;
            }
            return 0;
        }
        public PoliceSigortali GetAdSoyad(string tckn, string type)
        {
            PoliceSigortali kullanici = null;
            if (type == "0")
            {
                kullanici = _context.PoliceSigortali.Where(x => x.KimlikNo == tckn).OrderByDescending(x => x.PoliceId).Take(1).SingleOrDefault();
            }
            return kullanici != null ? kullanici : new PoliceSigortali();
        }

        public bool ResetPassword(int id, string email, string adSoyad)
        {
            string token = Guid.NewGuid().ToString();
            bool isEmailSent = false;

            try
            {
                string emailSubject = "Sigortadefterim - Şifre Sıfırlama İsteği";
                string resetPasswordUrl = "https://sigortadefterimv2api.azurewebsites.net/api/User/ConfirmResetPassword?PasswordVerificationToken=" + token;
                string emailBody = "Şifrenizi sıfırlamak için <a href=\"" + resetPasswordUrl + "\">buraya tıklayınız.</a>";
                UtilityService.SendEmail(email, emailSubject, emailBody, null, true);

                isEmailSent = true;
            }
            catch (Exception e)
            {
                isEmailSent = false;
            }

            if (isEmailSent)
            {
                KullaniciSifremiUnuttum kullaniciSifremiUnuttum = new KullaniciSifremiUnuttum
                {
                    KullaniciId = id,
                    PasswordVerificationToken = token,
                    Status = KullaniciSifremiUnuttumTipleri.LinkGonderildi,
                    SendDate = TurkeyDateTime.Now
                };

                _context.KullaniciSifremiUnuttum.Add(kullaniciSifremiUnuttum);
                _context.SaveChanges();
            }

            return isEmailSent;
        }


        public KullaniciSifremiUnuttum GetToken(string PasswordVerificationToken)
        {
            KullaniciSifremiUnuttum result = _context.KullaniciSifremiUnuttum.Where(KullaniciSifremiUnuttum => KullaniciSifremiUnuttum.PasswordVerificationToken == PasswordVerificationToken).FirstOrDefault(); 
            return result;
        }
        public void ResetPasswordVerificationToken(string PasswordVerificationToken)
        {
            KullaniciSifremiUnuttum result = _context.KullaniciSifremiUnuttum.Where(KullaniciSifremiUnuttum => KullaniciSifremiUnuttum.PasswordVerificationToken == PasswordVerificationToken).FirstOrDefault();
            result.PasswordVerificationToken = Guid.NewGuid().ToString();
            _context.SaveChanges(); 
        }
        public bool PasswordSend(int id)
        {
            string newPassword = Encryption.Generate(6);
            string hashPassword = Encryption.getMd5Hash(newPassword);

            Kullanici kullanici = _context.Kullanici.Where(k => k.Id == id).FirstOrDefault();
            KullaniciSifremiUnuttum kullaniciSifremiUnuttum = _context.KullaniciSifremiUnuttum.Where(k => k.KullaniciId == id && k.Status == KullaniciSifremiUnuttumTipleri.LinkGonderildi).FirstOrDefault();

            if (kullanici != null && kullaniciSifremiUnuttum != null)
            {

                try
                {

                    #region Kullanıcı update

                    kullanici.Sifre = hashPassword;
                    _context.Kullanici.Update(kullanici);
                    _context.SaveChanges();

                    #endregion

                    #region kullaniciSifremiUnuttum update

                    kullaniciSifremiUnuttum.ResetDate = TurkeyDateTime.Now;
                    kullaniciSifremiUnuttum.Status = KullaniciSifremiUnuttumTipleri.SifreResetlendi;
                    kullaniciSifremiUnuttum.YeniSifre = hashPassword;
                    _context.KullaniciSifremiUnuttum.Update(kullaniciSifremiUnuttum);
                    _context.SaveChanges();

                    #endregion

                    #region send email new password 


                    string emailSubject = "Sigortadefterim - Şifreniz Sıfırlanmıştır.";
                    string emailBody = "Yeni Şifreniz: " + newPassword;
                    UtilityService.SendEmail(kullanici.Eposta, emailSubject, emailBody, null, true);


                    #endregion

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }

            }

            return false;

        }

        public string refreshSessionToken(string token)
        {
            var res = _context.Kullanici.Where(x => x.Guvenlik == token).FirstOrDefault();
            if (res == null)
                return "";
             
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out SecurityToken validatedToken);
                token = "-1";
            }
            catch
            { 
                token = createNewToken();
                res.Guvenlik = token;
                _context.SaveChanges();
            }


            return token;
        }
        //test metod
        public string refreshTestToken(string token)
        {
            var asd = DateTime.Now;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var accountId = jwtToken.Claims.First(x => x.Type == "email").Value;


                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Email, Guid.NewGuid().ToString())
                    }),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token1 = tokenHandler.CreateToken(tokenDescriptor);
                //return tokenHandler.WriteToken(token1);
                return "";
            }
            catch
            {
                // return null if validation fails
                return "";
            }
        }
        public bool ValidateCurrentToken(string token)
        {
            //var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%"; 
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));



            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
