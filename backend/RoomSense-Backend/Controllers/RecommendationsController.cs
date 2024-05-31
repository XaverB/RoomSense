using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomSense_Backend.Dto;
using RoomSense_Backend.Entity;

namespace RoomSense_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationsController : ControllerBase
    {
        private readonly MonitoringContext _context;

        public RecommendationsController(MonitoringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecommendationDto>>> GetAllRecommendations(int page = 1, int pageSize = 10)
        {
            var recommendations = await _context.Recommendations
                .Include(r => r.Room)
                .OrderByDescending(r => r.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RecommendationDto
                {
                    Id = r.Id,
                    RoomId = r.RoomId,
                    RoomName = r.Room.Name,
                    Message = r.Message,
                    Timestamp = r.Timestamp
                })
                .ToListAsync();

            if (recommendations == null || recommendations.Count == 0)
            {
                return NotFound();
            }

            return Ok(recommendations);
        }

        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<RecommendationDto>>> GetRecommendationsByRoom(int roomId, int page = 1, int pageSize = 10)
        {
            var recommendations = await _context.Recommendations
                .Include(r => r.Room)
                .Where(r => r.RoomId == roomId)
                .OrderByDescending(r => r.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RecommendationDto
                {
                    Id = r.Id,
                    RoomId = r.RoomId,
                    RoomName = r.Room.Name,
                    Message = r.Message,
                    Timestamp = r.Timestamp
                })
                .ToListAsync();

            if (recommendations == null || recommendations.Count == 0)
            {
                return NotFound();
            }

            return Ok(recommendations);
        }
    }
}
