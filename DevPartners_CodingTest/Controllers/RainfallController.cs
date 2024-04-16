using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using DevPartners_CodingTest.Models;

namespace DevPartners_CodingTest.Controllers
{
    [Route("api/[controller]/id/stations/3680/readings")]
    [ApiController]
    public class RainfallController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public RainfallController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // GET: api/Rainfall
        [HttpGet]
        public async Task<IActionResult> GetReadings(string id)
        {
            //fetching from url
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"https://environment.data.gov.uk/flood-monitoring/id/stations/3680/readings?_sorted&_limit=100");

            //only get the "items" on the provided api
            var responseString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonDocument.Parse(responseString);
            var items = jsonResponse.RootElement.GetProperty("items");
            var readings = JsonSerializer.Deserialize<List<Rainfall>>(items.GetRawText());
            return Ok(readings);
        }

    }
}
