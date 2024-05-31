namespace RoomSense_Backend.Entity
{
    public class Reading
    {
        public int Id { get; set; }
        public float Value { get; set; }
        public DateTime Timestamp { get; set; }

        public int SensorId { get; set; }
        public Sensor Sensor { get; set; }
    }
}
