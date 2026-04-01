using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SigortaDefterimV2API.Models.Database;
using SigortaDefterimV2API.Models.Inputs.Car;
using SigortaDefterimV2API.Models.Responses;
using SigortaDefterimV2API.Services;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private CarService _carService;

        public CarController(CarService carService)
        {
            _carService = carService;
        }

        /// <summary>
        ///  AracMarka tablosu üzerindeki tüm mevcut kayıtları döndürür.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("GetAracMarka")]
        [SwaggerResponse(200, Type = typeof(List<AracMarka>))]
        [Produces("application/json")]
        public IActionResult GetAracMarka()
        {
            List<AracMarka> aracMarkaList = _carService.GetAracMarka();
            return Ok(aracMarkaList);
        }

        /// <summary>
        ///  AracTip tablosundan parametre olarak verilen MarkaKodu ile eşleşen tüm mevcut kayıtları döndürür
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        [HttpGet("GetAracTip")]
        [SwaggerResponse(200, Type = typeof(List<AracTip>))]
        [SwaggerResponse(404)]
        [Produces("application/json")]
        public IActionResult GetAracTip([FromQuery]GetAracTipInput input)
        {
            List<AracTip> aracTipList = _carService.GetAracTip(input.MarkaKodu);
            if (aracTipList.Count > 0)
            {
                return Ok(aracTipList);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// AracKullanimSekli tablosu üzerindeki tüm mevcut kayıtları döndürür.
        /// </summary>
        /// <returns></returns>

        [HttpGet("GetAracKullanimSekli")]
        [SwaggerResponse(200, Type = typeof(List<AracKullanimSekli>))]
        [Produces("application/json")]
        public IActionResult GetAracKullanimSekli()
        {
            List<AracKullanimSekli> aracKullanimSekliList = _carService.GetAracKullanimSekli();
            return Ok(aracKullanimSekliList);
        }

        /// <summary>
        /// AracKullanimTarzi tablosundan parametre olarak verilen KullanimSekliKodu ile eşleşen tüm kayıtları  döndürür
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("GetAracKullanimTarzi")]
        [SwaggerResponse(200, Type = typeof(List<AracKullanimTarzi>))]
        [SwaggerResponse(404)]
        [Produces("application/json")]
        public IActionResult GetAracKullanimTarzi([FromQuery]GetAracKullanimTarziInput input)
        {
            List<AracKullanimTarzi> aracKullanimTarziList = _carService.GetAracKullanimTarzi(input.KullanimSekliKodu);
            if (aracKullanimTarziList.Count > 0)
            {
                return Ok(aracKullanimTarziList);
            }
            else
            {
                return NotFound();
            }
        }
    }
}