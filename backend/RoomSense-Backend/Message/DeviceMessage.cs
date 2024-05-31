// Ignore Spelling: Timestamp

namespace RoomSense_Backend.Message
{
    public class DeviceMessage
    {
        public required string DeviceType { get; set; }
        public required string DeviceLocation { get; set; }

        private readonly Payload Payload;

        public DeviceMessage(Payload payload, string DeviceLocation)
        {
            if (string.IsNullOrEmpty(DeviceLocation))
            {
                throw new ArgumentException($"'{nameof(DeviceLocation)}' cannot be null or empty.", nameof(DeviceLocation));
            }

            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            this.DeviceLocation = DeviceLocation;
        }

        public  string DeviceId => Payload.DeviceId;
        public  string Value => Payload.DeviceId;
        public  string Timestamp => Payload.DeviceId;
    }
}
