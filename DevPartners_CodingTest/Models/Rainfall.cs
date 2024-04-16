using System.Text.Json.Serialization;
namespace DevPartners_CodingTest.Models
{
    public class Rainfall
    {
        [JsonPropertyName("@id")]
        public string Id { get; set; }
        public string dateTime { get; set; }
        public string measure { get; set; }
        public double value { get; set; }
    }
}
