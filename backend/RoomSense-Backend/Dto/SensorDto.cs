namespace RoomSense_Backend.Dto
{
    public class SensorDto
    {
        public int Id { get; set; }
        public required string Type { get; set; }
        public int RoomId { get; set; }
        public required string RoomName { get; set; }
    }
}
