// Ignore Spelling: Timestamp

using System.Text.Json.Serialization;

namespace RoomSense_Backend.Message
{
    public class Payload
    {
        [JsonPropertyName("device")]
        public required string Device { get; set; }

        [JsonPropertyName("value")]
        public required string Value { get; set; }

        [JsonPropertyName("timestamp")]
        public required string Timestamp { get; set; }
    }
}
