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
            if (await _context.Students.AnyAsync(s => s.StudentCode == student.StudentCode))
                return BadRequest("Mã sinh viên đã tồn tại");

            student.Status = student.Status ?? "active";

            if (student.Status == "active" && student.RoomId != null)
            {
                var room = await _context.Rooms.FindAsync(student.RoomId);

                if (room == null)
                    return BadRequest("Phòng không tồn tại");

                if (room.CurrentOccupancy >= room.MaxCapacity)
                    return BadRequest("Phòng đã đầy");

                room.CurrentOccupancy += 1;
            }

            if (student.Status == "inactive")
            {
                student.RoomId = null;
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

            // 🔥 CASE 1: ĐÃ RỜI → reset phòng ngay
            if (student.Status == "inactive")
            {
                if (existing.RoomId != null)
                {
                    var room = await _context.Rooms.FindAsync(existing.RoomId);
                    if (room != null && room.CurrentOccupancy > 0)
                        room.CurrentOccupancy -= 1;
                }

                student.RoomId = null;
            }
            else
            {
                // 🔥 CASE 2: đổi phòng
                if (existing.RoomId != student.RoomId)
                {
                    if (existing.RoomId != null)
                    {
                        var oldRoom = await _context.Rooms.FindAsync(existing.RoomId);
                        if (oldRoom != null && oldRoom.CurrentOccupancy > 0)
                            oldRoom.CurrentOccupancy -= 1;
                    }

                    if (student.RoomId != null)
                    {
                        var newRoom = await _context.Rooms.FindAsync(student.RoomId);

                        if (newRoom == null)
                            return BadRequest("Phòng không tồn tại");

                        if (newRoom.CurrentOccupancy >= newRoom.MaxCapacity)
                            return BadRequest("Phòng đã đầy");

                        newRoom.CurrentOccupancy += 1;
                    }
                }
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