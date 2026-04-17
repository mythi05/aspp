using aspp.Data;
using aspp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomController(AppDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL =================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetRooms()
        {
            var rooms = await _context.Rooms.ToListAsync();

            // 🔥 đếm 1 lần (tránh N+1 query)
            var studentCounts = await _context.Students
                .Where(s => s.Status == "active")
                .GroupBy(s => s.RoomId)
                .Select(g => new { RoomId = g.Key, Count = g.Count() })
                .ToListAsync();

            var result = rooms.Select(r =>
            {
                var occupancy = studentCounts
                    .FirstOrDefault(x => x.RoomId == r.Id)?.Count ?? 0;

                return new
                {
                    r.Id,
                    r.RoomName,
                    r.Building,
                    r.Floor,
                    r.RoomType,
                    r.MaxCapacity,
                    r.Price,
                    CurrentOccupancy = occupancy,
                    Status = occupancy >= r.MaxCapacity ? "Đã đầy" : "Còn trống"
                };
            });

            return Ok(result);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return NotFound("Không tìm thấy phòng");

            var occupancy = await _context.Students
                .CountAsync(s => s.RoomId == id && s.Status == "active");

            return Ok(new
            {
                room.Id,
                room.RoomName,
                room.Building,
                room.Floor,
                room.RoomType,
                room.MaxCapacity,
                room.Price,
                CurrentOccupancy = occupancy,
                Status = occupancy >= room.MaxCapacity ? "Đã đầy" : "Còn trống"
            });
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<ActionResult<Room>> CreateRoom(Room room)
        {
            if (await _context.Rooms.AnyAsync(r => r.RoomName == room.RoomName))
                return BadRequest("Phòng đã tồn tại");

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return Ok(room);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, Room room)
        {
            if (id != room.Id)
                return BadRequest("ID không khớp");

            var existing = await _context.Rooms.FindAsync(id);
            if (existing == null)
                return NotFound("Không tìm thấy phòng");

            existing.RoomName = room.RoomName;
            existing.Building = room.Building;
            existing.Floor = room.Floor;
            existing.RoomType = room.RoomType;
            existing.MaxCapacity = room.MaxCapacity;
            existing.Price = room.Price;

            await _context.SaveChangesAsync();

            return Ok("Cập nhật thành công");
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return NotFound("Không tìm thấy phòng");

            var hasStudents = await _context.Students
                .AnyAsync(s => s.RoomId == id && s.Status == "active");

            if (hasStudents)
                return BadRequest("Phòng vẫn còn sinh viên đang ở");

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return Ok("Xóa thành công");
        }
    }
}