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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.Room)
                .ToListAsync();

            return Ok(students);
        }

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

        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(Student student)
        {
            if (await _context.Students.AnyAsync(s => s.StudentCode == student.StudentCode))
                return BadRequest("Mã sinh viên đã tồn tại");

            student.Status = student.Status ?? "active";

            if (student.Status == "inactive")
            {
                student.RoomId = null;
            }

            if (student.Status == "active" && student.RoomId != null)
            {
                var count = await _context.Students
                    .CountAsync(s => s.RoomId == student.RoomId && s.Status == "active");

                var room = await _context.Rooms.FindAsync(student.RoomId);

                if (room == null)
                    return BadRequest("Phòng không tồn tại");

                if (count >= room.MaxCapacity)
                    return BadRequest("Phòng đã đầy");
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok(student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, Student student)
        {
            if (id != student.Id)
                return BadRequest("ID không khớp");

            var existing = await _context.Students.FindAsync(id);

            if (existing == null)
                return NotFound("Không tìm thấy sinh viên");

            if (student.Status == "inactive")
            {
                student.RoomId = null;
            }

            if (student.Status == "active" && student.RoomId != null)
            {
                var count = await _context.Students
                    .CountAsync(s => s.RoomId == student.RoomId && s.Status == "active" && s.Id != id);

                var room = await _context.Rooms.FindAsync(student.RoomId);

                if (room == null)
                    return BadRequest("Phòng không tồn tại");

                if (count >= room.MaxCapacity)
                    return BadRequest("Phòng đã đầy");
            }

            existing.StudentCode = student.StudentCode;
            existing.FullName = student.FullName;
            existing.Email = student.Email;
            existing.Gender = student.Gender;
            existing.PhoneNumber = student.PhoneNumber;
            existing.Major = student.Major;
            existing.Status = student.Status;
            existing.RoomId = student.RoomId;

            await _context.SaveChangesAsync();

            return Ok("Cập nhật thành công");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
                return NotFound("Không tìm thấy sinh viên");

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return Ok("Xóa thành công");
        }
    }
}