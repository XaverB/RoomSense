﻿// Ignore Spelling: Timestamp

namespace RoomSense_Backend.Message
{
    public class Payload
    {
        public required string DeviceId { get; set; }
        public required string Value { get; set; }
        public required string Timestamp { get; set; }
    }
}
