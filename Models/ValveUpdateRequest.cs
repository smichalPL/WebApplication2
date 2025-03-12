using System.Text.Json.Serialization;

namespace WebApplication2.Models
{
    public class ValveUpdateRequest
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("state")]
        public bool State { get; set; }
    }

}
