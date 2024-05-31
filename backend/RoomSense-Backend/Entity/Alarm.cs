namespace RoomSense_Backend.Entity
{
    public class Alarm
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
