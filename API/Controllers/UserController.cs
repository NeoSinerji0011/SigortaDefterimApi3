using API.Models.Database;
using API.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SigortaDefterimV2API.Models;
using SigortaDefterimV2API.Models.Database;
using SigortaDefterimV2API.Models.Responses;
using SigortaDefterimV2API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SigortaDefterimV2API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Uygulamadan JSON Web Token almak için kullanılan Endpointtir.
        /// Otorizasyon gerektiren diğer tüm istekler alınan JWT değeri "Authorization" headerında "Bearer {Token}" şeklinde yollanarak talep edilecektir.
        /// Başarılı şekilde giriş yapan kullanıcının Email adresi JWT içerisinde mevcuttur.
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        [SwaggerResponse(401, Type = typeof(Message))]
        [SwaggerResponse(200, Type = typeof(LoginResponse))]
        [Produces("application/json")]
        public IActionResult Login([FromQuery]LoginInput userData)
        {
            if (string.IsNullOrEmpty(userData.Email) || string.IsNullOrEmpty(userData.Password))
            {
                return BadRequest("Invalid input parameter, Input : " + userData.ToString());
            }
            else
            {
                LoginResponse response = _userService.Login(userData);

                if (response.kullanici == null)
                {
                    return BadRequest(new Message { message = "Email or Password is incorrect." });
                }
                else
                {
                    return Ok(response);
                }
            }
        }

        /// <summary>
        /// İlk etapta kullanıcının emailine şifre sıfırlama onay emaili yollar.
        /// Kullanıcı bu emaildeki onay linkine tıklarsa kullanıcının şifresini rastgele bir şifreyle değiştirerek bu yeni şifreyi kullanıcıya e-posta olarak yollar.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public IActionResult ResetPassword([FromQuery]string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid input parameter, Input : " + email);
            }
            else
            {
                Kullanici kullanici = _userService.GetKullanici(email);

                if (kullanici == null)
                {
                    return BadRequest(new {statusCode=400, message = "Email bulunamadı" });
                }
                else
                {
                    bool isEmailSent = _userService.ResetPassword(kullanici.Id, email, kullanici.Adsoyad);
                    if (isEmailSent)
                    {
                        return Ok(new { statusCode = 200, message = "İşlem Tamamlandı.Epostanızı kontrol ediniz." });
                    }
                    else
                    {
                        return Ok(new { statusCode = 500, message = "Email gönderilemedi.Tekrar deneyiniz." });
                        //return Problem(detail: "Email couldn't send.", statusCode: 500);
                    }
                }
            }
        }


        /// <summary>
        /// İlk etapta kullanıcının emailine şifre sıfırlama onay emaili yollar.
        /// Kullanıcı bu emaildeki onay linkine tıklarsa kullanıcının şifresini rastgele bir şifreyle değiştirerek bu yeni şifreyi kullanıcıya e-posta olarak yollar.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        [SwaggerResponse(401, Type = typeof(Message))]
        [SwaggerResponse(200, Type = typeof(Message))]
        [Produces("application/json")]
        public IActionResult RefreshToken([FromBody] Kullanici kullanici)
        {
            var result=_userService.refreshSessionToken(kullanici.Guvenlik);
            return Ok(new Message {message=(result=="-1" || result == "" ? "":result),statusCode= (result == "" ? MessageStatus.Failed : result == "-1"?MessageStatus.Error:MessageStatus.Success) });
        }

        /// <summary>
        /// Yeni kullanıcı kaydı oluşturmak için kullanılan Endpointtir.
        /// Kullanıcı kaydı başarılı bir şekilde oluşturulursa dönen cevaptaki JSON Objesinde ki statusCode 200 değerini alır.
        /// Eğer email zaten mevcutsa 400 değerini alır.
        /// Eğer Tc zaten mevcutsa 401 değerini alır.
        /// </summary>
        /// <param name="kullanici"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Register")]
        [SwaggerResponse(200, Type = typeof(RegisterResponse))]
        public IActionResult Register([FromBody]Kullanici kullanici)
        {
            RegisterResponse response = _userService.Register(kullanici);
            return Ok(response);
        }
        /// <summary>
        /// kimlik seçimine göre Tc ile  ad soyad getirme işlemi(mobil üye ol sayfası). 
        /// </summary>
        /// <param name="kullanici"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetAdSoyad")]
        [SwaggerResponse(200, Type = typeof(PoliceSigortali))]
        public IActionResult GetAdSoyad([FromQuery] Kullanici kullanici)
        {
            PoliceSigortali response = _userService.GetAdSoyad(kullanici.Tc,kullanici.Durum);//kullanici.Durum:kimlik seçimi değişkeni için kullanılmıştır.
            return Ok(response);
        }
        /// <summary>
        /// kullanıcı kaydı güncelleme fonksiyonu  
        /// </summary>
        /// <param name="kullanici"></param>
        /// <returns></returns> 
        [HttpPost("UserUpdate")]
        [SwaggerResponse(200, Type = typeof(RegisterResponse))] 
        [SwaggerResponse(404, Type = typeof(RegisterResponse))]
        [Produces("application/json")]
        public IActionResult UserUpdate([FromBody] Kullanici kullanici)
        {
            RegisterResponse response = _userService.UserUpdate(kullanici);
            return Ok(response);
        }

        /// <summary>
        /// geri dönüş mesaj işlemi  
        /// </summary>
        /// <param name="kullanici"></param>
        /// <returns></returns> 
        [HttpPost("SendFeedBack")]
        [SwaggerResponse(200, Type = typeof(Message))]
        [SwaggerResponse(404, Type = typeof(Message))]
        [Produces("application/json")]
        public IActionResult SendFeedBack([FromBody] UserResponse userResponse)
        {
            Message response =_userService.SendFeedBack(userResponse);
            return Ok(response);
        }
        /// <summary>
        /// sigortadefterim.com iletişim metodu  
        /// </summary>
        /// <param name="kullanici"></param>
        /// <returns></returns> 
        [HttpPost("WebSiteContact")]  
        [AllowAnonymous]
        public IActionResult WebSiteContact()
        {
            UserResponse userResponse = new UserResponse();
            userResponse.message= Request.Form["message"];
            userResponse.adsoyad= Request.Form["adsoyad"];
            userResponse.email= Request.Form["email"];

            Message response = _userService.WebSiteContact(userResponse);
            return Ok(response);
        }
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpGet("ConfirmResetPassword")]
        [Produces("text/html")]
        [AllowAnonymous]
        public IActionResult ConfirmResetPassword([FromQuery] string PasswordVerificationToken)
        {
            PasswordResetInfoModel passwordResetInfo = new PasswordResetInfoModel();

            KullaniciSifremiUnuttum isTokenValid = _userService.GetToken(PasswordVerificationToken);
            if (isTokenValid != null)
            {
                //yeni şifreyi gönder.
                bool isPasswordSend = _userService.PasswordSend(isTokenValid.KullaniciId);
                if (isPasswordSend)
                {
                    // şifre yollandı başarılı
                    passwordResetInfo.Status = "success";
                    _userService.ResetPasswordVerificationToken(PasswordVerificationToken);
                }
                else
                {
                    // işlem gerçekleşemedi.
                    passwordResetInfo.Status = "fail";
                }
            }
            else
            {
                //geçersiz token
                passwordResetInfo.Status = "invalid";
            }

            var Result = new ViewResult
            {
                ViewName = "~/Views/PasswordResetInfo.cshtml",
                ViewData = new ViewDataDictionary(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                {
                    Model = passwordResetInfo,
                },
            };

            return Result;
        }
        [HttpGet("ConfirmRegister")]
        [Produces("text/html")]
        [AllowAnonymous]
        public IActionResult ConfirmRegister([FromQuery] string Token)
        {
            PasswordResetInfoModel passwordResetInfo = new PasswordResetInfoModel(); 
            switch (_userService.ConfirmRegister(Token))
            {
                case 1:
                    passwordResetInfo.Status = "success";break;
                case 2:
                    passwordResetInfo.Status = "fail";break;
                default:
                    passwordResetInfo.Status = "invalid";
                    break;
            } 
            var Result = new ViewResult
            {
                ViewName = "~/Views/ConfirmRegister.cshtml",
                ViewData = new ViewDataDictionary(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                {
                    Model = passwordResetInfo,
                },
            };

            return Result;
        }
        [HttpGet("test")]
        [Produces("text/html")]
        [AllowAnonymous]
        public IActionResult test([FromQuery] string Token)
        {
            
            _userService.refreshTestToken(Token);
            //var asd = _userService.refreshTestToken(Token);
            return Ok(/*_mobileAppService.test("40858174782")*/);
        }
       
    }
}