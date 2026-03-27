using aspp.Data;
using DormitoryAPI.Models;
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

        // =========================
        // GET: api/student
        // Lấy tất cả sinh viên
        // =========================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.Room)
                .ToListAsync();

            return Ok(students);
        }

        // =========================
        // GET: api/student/{id}
        // Lấy sinh viên theo ID
        // =========================
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

        // =========================
        // POST: api/student
        // Thêm sinh viên
        // =========================
        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(Student student)
        {
            // Check trùng MSSV
            if (await _context.Students.AnyAsync(s => s.StudentCode == student.StudentCode))
            {
                return BadRequest("Mã sinh viên đã tồn tại");
            }

            // Check phòng có tồn tại không (nếu có RoomId)
            if (student.RoomId != null)
            {
                var room = await _context.Rooms.FindAsync(student.RoomId);
                if (room == null)
                    return BadRequest("Phòng không tồn tại");
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        // =========================
        // PUT: api/student/{id}
        // Cập nhật sinh viên
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, Student student)
        {
            if (id != student.Id)
                return BadRequest("ID không khớp");

            var existingStudent = await _context.Students.FindAsync(id);
            if (existingStudent == null)
                return NotFound("Không tìm thấy sinh viên");

            // Update dữ liệu
            existingStudent.StudentCode = student.StudentCode;
            existingStudent.FullName = student.FullName;
            existingStudent.Email = student.Email;
            existingStudent.Gender = student.Gender;
            existingStudent.PhoneNumber = student.PhoneNumber;
            existingStudent.Major = student.Major;
            existingStudent.Status = student.Status;
            existingStudent.RoomId = student.RoomId;

            await _context.SaveChangesAsync();

            return Ok("Cập nhật thành công");
        }

        // =========================
        // DELETE: api/student/{id}
        // Xóa sinh viên
        // =========================
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
