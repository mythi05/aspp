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
        [HttpGet("available-rooms")]
        public async Task<ActionResult<IEnumerable<object>>> GetAvailableRooms()
        {
            // 1. Lấy toàn bộ danh sách phòng
            var rooms = await _context.Rooms.ToListAsync();

            // 2. Đếm số sinh viên đang ở thực tế trong mỗi phòng
            var studentCounts = await _context.Students
                .Where(s => s.Status == "active")
                .GroupBy(s => s.RoomId)
                .Select(g => new { RoomId = g.Key, Count = g.Count() })
                .ToListAsync();

            // 3. Trả về thông tin phòng kèm số chỗ đã ngồi
            var result = rooms.Select(r => new
            {
                r.Id,
                r.RoomName,
                r.MaxCapacity,
                CurrentOccupancy = studentCounts.FirstOrDefault(x => x.RoomId == r.Id)?.Count ?? 0
            })
            .Where(r => r.CurrentOccupancy < r.MaxCapacity) // Chỉ hiện phòng còn chỗ
            .ToList();

            return Ok(result);
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
        [HttpGet("{id}/students")]
        public async Task<ActionResult<IEnumerable<object>>> GetStudentsInRoom(int id)
        {
            // Kiểm tra phòng có tồn tại không
            var roomExists = await _context.Rooms.AnyAsync(r => r.Id == id);
            if (!roomExists) return NotFound("Không tìm thấy phòng");

            var students = await _context.Students
                .Where(s => s.RoomId == id && s.Status == "active")
                .Select(s => new {
                    s.Id,
                    s.FullName,
                    s.StudentCode,
                    s.Gender,
                    // Bỏ JoinDate nếu báo lỗi, hoặc thay đúng tên trường trong DB của bạn
                })
                .ToListAsync();

            return Ok(students);
        }
        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return NotFound("Không tìm thấy phòng này trên hệ thống.");

            // Kiểm tra xem có sinh viên nào đang ở (Status = active) không
            var hasStudents = await _context.Students
                .AnyAsync(s => s.RoomId == id && s.Status == "active");

            if (hasStudents)
            {
                // Trả về lỗi 400 nếu phòng không trống
                return BadRequest("Không thể xóa! Phòng này đang có sinh viên cư trú.");
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return Ok("Đã xóa phòng thành công.");
        }
    }
}