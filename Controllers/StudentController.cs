using aspp.Data;
using aspp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL =================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.Room)
                .ToListAsync();

            return Ok(students);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound("Không tìm thấy sinh viên");

            return Ok(student);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(Student student)
        {
            // Check trùng MSSV
            if (await _context.Students.AnyAsync(s => s.StudentCode == student.StudentCode))
                return BadRequest("Mã sinh viên đã tồn tại");

            // Nếu có phòng
            if (student.RoomId != null)
            {
                var room = await _context.Rooms.FindAsync(student.RoomId);

                if (room == null)
                    return BadRequest("Phòng không tồn tại");

                // ❗ Check full
                if (room.CurrentOccupancy >= room.MaxCapacity)
                    return BadRequest("Phòng đã đầy");

                // 🔥 tăng số người
                room.CurrentOccupancy += 1;
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok(student);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, Student student)
        {
            if (id != student.Id)
                return BadRequest("ID không khớp");

            var existing = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existing == null)
                return NotFound("Không tìm thấy sinh viên");

            // ===== CASE 1: đổi phòng =====
            if (existing.RoomId != student.RoomId)
            {
                // giảm phòng cũ
                if (existing.RoomId != null)
                {
                    var oldRoom = await _context.Rooms.FindAsync(existing.RoomId);
                    if (oldRoom != null && oldRoom.CurrentOccupancy > 0)
                        oldRoom.CurrentOccupancy -= 1;
                }

                // tăng phòng mới
                if (student.RoomId != null)
                {
                    var newRoom = await _context.Rooms.FindAsync(student.RoomId);

                    if (newRoom == null)
                        return BadRequest("Phòng mới không tồn tại");

                    if (newRoom.CurrentOccupancy >= newRoom.MaxCapacity)
                        return BadRequest("Phòng mới đã đầy");

                    newRoom.CurrentOccupancy += 1;
                }
            }

            // ===== CASE 2: chuyển trạng thái =====
            if (existing.Status == "active" && student.Status == "inactive")
            {
                // giảm phòng
                if (existing.RoomId != null)
                {
                    var room = await _context.Rooms.FindAsync(existing.RoomId);
                    if (room != null && room.CurrentOccupancy > 0)
                        room.CurrentOccupancy -= 1;
                }

                student.RoomId = null;
            }

            if (existing.Status == "inactive" && student.Status == "active")
            {
                if (student.RoomId == null)
                    return BadRequest("Phải chọn phòng khi chuyển sang đang ở");

                var room = await _context.Rooms.FindAsync(student.RoomId);

                if (room.CurrentOccupancy >= room.MaxCapacity)
                    return BadRequest("Phòng đã đầy");

                room.CurrentOccupancy += 1;
            }

            _context.Entry(student).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok("Cập nhật thành công");
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
                return NotFound("Không tìm thấy sinh viên");

            // 🔥 giảm số người trong phòng
            if (student.RoomId != null)
            {
                var room = await _context.Rooms.FindAsync(student.RoomId);
                if (room != null && room.CurrentOccupancy > 0)
                    room.CurrentOccupancy -= 1;
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return Ok("Xóa thành công");
        }
    }
}