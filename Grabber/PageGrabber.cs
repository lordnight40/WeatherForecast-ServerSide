using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace Grabber
{
    public class PageGrabber
    {
        private string uri;

        private PageGrabber() { }
        public PageGrabber(string uri) => this.uri = uri;

        private async Task<string> LoadPage()
        {
            if (string.IsNullOrWhiteSpace(uri)) return string.Empty;
            try
            {
                HttpClient httpClient = new HttpClient();

                var response = await httpClient.GetAsync(uri);
                return await response.Content.ReadAsStringAsync();
            } catch (HttpRequestException)
            {
                return string.Empty;
            }
        }

        public async Task<List<WeatherRecord>> GetData()
        {
            string html = await LoadPage();
            if (string.IsNullOrWhiteSpace(html)) return null;

            HtmlDocument document = new HtmlDocument();

            document.LoadHtml(html);
            var weatherContainers = document.DocumentNode.SelectNodes(
                ".//div[@class='weather__content_article clearfix']" +
                "/div[@class='weather__content_article-left']" +
                "/div[@class='weather__article_main clearfix']" +
                "/div[@class='weather__article_main_right']" +
                "/ul[@class='weather__article_main_right-table clearfix']" +
                "/li"
            );
            if (weatherContainers == null) return null;

            List<WeatherRecord> record = new List<WeatherRecord>();

            foreach (var container in weatherContainers)
            {
                string time = container.SelectSingleNode(".//div[@class='table__time_hours']").InnerText.Trim() ?? string.Empty;
                string temp = container.SelectSingleNode(".//div[@class='table__temp']").InnerText.Trim() ?? string.Empty;
                string pressure = container.SelectSingleNode(".//div[@class='table__pressure']").InnerText.Trim() ?? string.Empty;
                string humidity = container.SelectSingleNode(".//div[@class='table__humidity']").InnerText.Trim() ?? string.Empty;
                string windSpeed = container.SelectSingleNode(".//div[@data-tooltip='']").InnerText.Trim() ?? string.Empty;
                string precipitation = container.SelectSingleNode(".//div[@class='table__precipitation']").InnerText.Trim() ?? string.Empty;

                record.Add(new WeatherRecord
                {
                    Time = time,
                    Temperature = temp,
                    Pressure = pressure,
                    Humidity = humidity,
                    WindSpeed = windSpeed,
                    Precipiation = precipitation
                });
            }

            return record;
        }
    }
}
