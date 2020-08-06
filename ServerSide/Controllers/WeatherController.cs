using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Grabber;

namespace ServerSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly string WeatherUri = "https://sinoptik.com.ru";

        [Route("get")]
        [HttpGet]
        public async Task<IActionResult> Get(string city, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest(error: new { success = false, message = "Не указан город" }) ;


            string preparedDate = date.ToString("yyyy-MM-dd");
            string preparedCity = city.ToLower();
            string queryString  = Uri.EscapeUriString($"{WeatherUri}/погода-{preparedCity}/{preparedDate}");

            PageGrabber grabber = new PageGrabber(queryString);
            var result = await grabber.GetData();

            return new JsonResult(result);
        }
    }
}
