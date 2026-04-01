using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Metadata.Providers;
using System.Web.Http.ModelBinding;
using API.Areas.MobilApi.Helper;
using API.Areas.MobilApi.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SigortaDefterimV2API.Models.Responses;

namespace API.Controllers
{
    public class HomeController : Controller
    { 
        public IActionResult Index()
        { 
            return View();
        }
        public IActionResult Test([FromQuery]string path)
        {
            try
            {
                new AuthenticatorDecode().QrCodeResolve((Bitmap)Bitmap.FromFile(@"C:\Users\MesutCalik\Desktop\example.png"));
            }
            catch (Exception ex)
            {
                Utils.WriteFile(Directory.GetCurrentDirectory() + "/files/log/error.txt",ex.Message);
            }
          
            //new AuthenticatorDecode().ParseQrCode((@"C:\Users\MesutCalik\Desktop\example.png"));
            return Ok("");
        }
    }
}
