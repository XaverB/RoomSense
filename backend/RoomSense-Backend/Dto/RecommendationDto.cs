// Ignore Spelling: Timestamp

namespace RoomSense_Backend.Dto
{
    public class RecommendationDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public required string RoomName { get; set; }
        public required string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
