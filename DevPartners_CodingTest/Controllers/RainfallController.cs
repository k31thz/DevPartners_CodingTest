using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using DevPartners_CodingTest.Models;

namespace DevPartners_CodingTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RainfallController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public RainfallController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // GET: api/Rainfall
        [HttpGet("id/{stationId}/readings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReadings([FromRoute] string stationId, [FromQuery] int count = 10)
        {
            try
            {
                //fetching from url
                var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"https://environment.data.gov.uk/flood-monitoring/id/stations/{stationId}/readings?_sorted&_limit=100");


                if (response.IsSuccessStatusCode)
                {
                    //only get the "items" on the provided api
                    // If the status code is successful, parse the response
                    var responseString = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonDocument.Parse(responseString);
                    var items = jsonResponse.RootElement.GetProperty("items");
                    var readings = JsonSerializer.Deserialize<List<Rainfall>>(items.GetRawText());

                    if (readings.Count == 0)
                    {
                        return NoContent(); // If there are no readings, return a NoContent result
                    }
                    return Ok(readings); // Otherwise, return an Ok result with the readings
                }
                else
                {
                    return BadRequest(); // If the status code is BadRequest, return a BadRequest result
                }
            } catch (Exception e)
            {
                return StatusCode(500, $"An unexpected error occurred: {e.Message}");
            }
        }

    }
}
