using aspp.Data;
using aspp.Models;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

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

            // Mặc định khi thêm mới/import là inactive (Chờ nhận phòng)
            student.Status = student.Status ?? "inactive";

            if (student.Status == "active" && student.RoomId != null)
            {
                var room = await _context.Rooms.FindAsync(student.RoomId);
                if (room == null) return BadRequest("Phòng không tồn tại");

                var count = await _context.Students
                    .CountAsync(s => s.RoomId == student.RoomId && s.Status == "active");

                if (count >= room.MaxCapacity)
                    return BadRequest("Phòng đã đầy, không thể tiếp nhận thêm");
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok(student);
        }

      [HttpPut("{id}")]
public async Task<IActionResult> UpdateStudent(int id, Student student)
{
    if (id != student.Id) return BadRequest("ID không khớp");

    var existing = await _context.Students.FindAsync(id);
    if (existing == null) return NotFound("Không tìm thấy sinh viên");

    // 1. Xử lý logic Phòng (RoomId) - Đưa ra ngoài để luôn lưu khi thay đổi
    if (student.RoomId != null)
    {
        var room = await _context.Rooms.FindAsync(student.RoomId);
        if (room == null) return BadRequest("Phòng không tồn tại");

        // Chỉ kiểm tra sức chứa nếu sinh viên ĐANG ở hoặc MUỐN chuyển sang trạng thái "active"
        if (student.Status == "active")
        {
            var count = await _context.Students
                .CountAsync(s => s.RoomId == student.RoomId && s.Status == "active" && s.Id != id);

            if (count >= room.MaxCapacity)
                return BadRequest($"Phòng {room.RoomName} đã đầy ({room.MaxCapacity}/{room.MaxCapacity})");
        }

        existing.RoomId = student.RoomId;
    }
    else
    {
        // Nếu Frontend gửi lên RoomId null (chọn "Để sau" hoặc xóa phòng)
        existing.RoomId = null;
    }

    // 2. Cập nhật các thông tin cơ bản khác
    existing.StudentCode = student.StudentCode;
    existing.FullName = student.FullName;
    existing.Email = student.Email;
    existing.Gender = student.Gender;
    existing.PhoneNumber = student.PhoneNumber;
    existing.Major = student.Major;
    existing.Status = student.Status;

    try
    {
        await _context.SaveChangesAsync();
        return Ok("Cập nhật thành công");
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
    }
}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound("Không tìm thấy sinh viên");

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return Ok("Xóa thành công");
        }
        [HttpGet("by-code/{code}")]
        public async Task<ActionResult<Student>> GetStudentByCode(string code)
        {
            // Tìm sinh viên dựa trên StudentCode và nạp luôn thông tin Phòng (Room) để lấy giá và tên
            var student = await _context.Students
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.StudentCode == code);

            if (student == null)
            {
                return NotFound(new { message = "Không tìm thấy sinh viên với mã này." });
            }

            return Ok(student);
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel()
        {
            var students = await _context.Students.Include(s => s.Room).ToListAsync();
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Students");
            string[] headers = { "Id", "StudentCode", "FullName", "Email", "Phone", "Room", "Status" };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[1, i + 1].Value = headers[i];
                ws.Cells[1, i + 1].Style.Font.Bold = true;
            }

            int row = 2;
            foreach (var s in students)
            {
                ws.Cells[row, 1].Value = s.Id;
                ws.Cells[row, 2].Value = s.StudentCode;
                ws.Cells[row, 3].Value = s.FullName;
                ws.Cells[row, 4].Value = s.Email;
                ws.Cells[row, 5].Value = s.PhoneNumber;
                ws.Cells[row, 6].Value = s.Room?.RoomName ?? "N/A";
                ws.Cells[row, 7].Value = s.Status == "active" ? "Đang ở" : (s.RoomId != null ? "Đã trả phòng" : "Chờ nhận phòng");
                row++;
            }
            ws.Cells.AutoFitColumns();
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "students.xlsx");
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File không hợp lệ." });

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets[0];

            if (ws.Dimension == null)
                return BadRequest(new { message = "Bảng tính rỗng." });

            int rowCount = ws.Dimension.Rows;
            var logs = new List<string>();
            int successCount = 0;
            int failCount = 0;

            for (int row = 2; row <= rowCount; row++)
            {
                var code = ws.Cells[row, 2].Text?.Trim();
                var fullName = ws.Cells[row, 3].Text?.Trim();
                var email = ws.Cells[row, 4].Text?.Trim();
                var phone = ws.Cells[row, 5].Text?.Trim();

                if (string.IsNullOrEmpty(code))
                {
                    logs.Add($"[Dòng {row}]: MSSV bị trống.");
                    failCount++;
                    continue;
                }

                if (await _context.Students.AnyAsync(s => s.StudentCode == code))
                {
                    logs.Add($"[Dòng {row}]: Sinh viên [{code}] đã tồn tại.");
                    failCount++;
                    continue;
                }

                try
                {
                    var student = new Student
                    {
                        StudentCode = code,
                        FullName = fullName,
                        Email = email,
                        PhoneNumber = phone,
                        Gender = "Nam",
                        Major = "Business Administration",
                        Status = "inactive", // Mới import mặc định chờ nhận phòng
                        RoomId = null        // Không có vết phòng cũ
                    };

                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();
                    successCount++;
                }
                catch (Exception ex)
                {
                    logs.Add($"[Dòng {row}]: Lỗi: {ex.Message}");
                    failCount++;
                }
            }

            return Ok(new
            {
                summary = $"Import hoàn tất: {successCount} thành công, {failCount} thất bại.",
                successCount,
                failCount,
                details = logs
            });
        }

        [HttpGet("export-pdf")]
        public async Task<IActionResult> ExportPdf()
        {
            try
            {
                var students = await _context.Students.Include(s => s.Room).ToListAsync();
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf");
                string boldFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arialbd.ttf");

                if (!System.IO.File.Exists(fontPath)) fontPath = "C:\\Windows\\Fonts\\Arial.ttf";
                if (!System.IO.File.Exists(boldFontPath)) boldFontPath = "C:\\Windows\\Fonts\\Arialbd.ttf";

                using var stream = new MemoryStream();
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                PdfFont font = PdfFontFactory.CreateFont(fontPath, iText.IO.Font.PdfEncodings.IDENTITY_H);
                PdfFont boldFont = PdfFontFactory.CreateFont(boldFontPath, iText.IO.Font.PdfEncodings.IDENTITY_H);

                document.Add(new Paragraph("DANH SÁCH SINH VIÊN KÝ TÚC XÁ")
                    .SetFont(boldFont).SetFontSize(16).SetTextAlignment(TextAlignment.CENTER));

                var table = new Table(7).UseAllAvailableWidth();
                string[] headers = { "STT", "MSSV", "Họ Tên", "Email", "SĐT", "Phòng", "Trạng thái" };

                foreach (var h in headers)
                {
                    table.AddHeaderCell(new Cell().Add(new Paragraph(h).SetFont(boldFont)).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                }

                int index = 1;
                foreach (var s in students)
                {
                    table.AddCell(new Cell().Add(new Paragraph(index++.ToString()).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(s.StudentCode ?? "").SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(s.FullName ?? "").SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(s.Email ?? "").SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(s.PhoneNumber ?? "").SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(s.Room?.RoomName ?? "-").SetFont(font)));

                    // Hiển thị trạng thái chi tiết trong PDF
                    string statusText = s.Status == "active" ? "Đang ở" : (s.RoomId != null ? "Đã trả phòng" : "Chờ nhận phòng");
                    table.AddCell(new Cell().Add(new Paragraph(statusText).SetFont(font)));
                }

                document.Add(table);
                document.Close();
                return File(stream.ToArray(), "application/pdf", "DanhSachSinhVien.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi xuất PDF: {ex.Message}");
            }
        }
    }
}