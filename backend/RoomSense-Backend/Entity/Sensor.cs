namespace RoomSense_Backend.Entity
{
    public class Sensor
    {
        public int Id { get; set; }
        public string Type { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        public ICollection<Reading> Readings { get; set; }
    }
}
