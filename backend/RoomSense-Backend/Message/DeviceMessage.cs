// Ignore Spelling: Timestamp

namespace RoomSense_Backend.Message
{
    public class DeviceMessage
    {
        public required string DeviceId { get; set; }
        public required string DeviceType { get; set; }
        public required string DeviceLocation { get; set; }
        public required string Value { get; set; }
        public required string Timestamp { get; set; }
    }
}
