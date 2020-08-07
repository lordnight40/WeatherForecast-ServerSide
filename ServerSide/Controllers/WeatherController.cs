using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Grabber;
using ServerSide.Service;

namespace ServerSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly string WeatherUri = "https://sinoptik.com.ru";
        private DbService dbService;

        public WeatherController() => dbService = new DbService();

        [Route("get")]
        [HttpGet]
        public async Task<IActionResult> Get(string city, string date)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest(error: new { success = false, error = "Не указан город" });

            string preparedCity = city.ToLower();
            string queryString  = Uri.EscapeUriString($"{WeatherUri}/погода-{preparedCity}/{date}");

            PageGrabber grabber = new PageGrabber(queryString);
            var result = await grabber.GetData();

            try
            {
                await dbService.Save(result, city, date);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = "При выполнении запроса произошла ошибка." });
            }

            return new JsonResult(new { success = true, message = result });
        }
    }
}
