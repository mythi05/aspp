using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aspp.Models;
using aspp.Data;

namespace aspp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomTransactionApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RoomTransactionApiController(AppDbContext context) { _context = context; }

        // ==========================================================
        // 1. LẤY LỊCH SỬ GIAO DỊCH
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.RoomTransactions
                .Include(t => t.Student)
                .Include(t => t.Room)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new
                {
                    t.Id,
                    StudentCode = t.Student != null ? t.Student.StudentCode : "N/A",
                    StudentName = t.Student != null ? t.Student.FullName : "N/A",
                    RoomName = t.Room != null ? t.Room.RoomName : "N/A",
                    t.TransactionType,
                    t.TransactionDate,
                    t.Note
                }).ToListAsync();
            return Ok(data);
        }

        // ==========================================================
        // 2. KIỂM TRA HỢP ĐỒNG (QUAN TRỌNG ĐỂ ĐỒNG BỘ FRONTEND)
        // ==========================================================
        [HttpGet("validate/{studentCode}")]
        public async Task<IActionResult> ValidateContract(string studentCode)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);

            if (student == null) return NotFound(new { message = "Không tìm thấy sinh viên." });

            // Tìm hợp đồng đang có hiệu lực (Status = 1)
            var activeContract = await _context.Contracts
                .Include(c => c.Room)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync(c => c.StudentId == student.Id && c.Status == 1);

            if (activeContract == null)
                return BadRequest(new { message = "Sinh viên chưa có hợp đồng hiệu lực hoặc đã thanh lý." });

            return Ok(new
            {
                studentId = student.Id,
                fullName = student.FullName,
                roomId = activeContract.RoomId,
                roomName = activeContract.Room?.RoomName,
                contractCode = activeContract.ContractCode,
                // Check xem SV đã check-in thực tế chưa
                isCheckIn = student.Status == "active"
            });
        }

        // ==========================================================
        // 3. XỬ LÝ CHECK-IN (Dựa trên phòng của Hợp đồng)
        // ==========================================================
        // Trong RoomTransactionApiController.cs -> Method CheckIn
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInReq req)
        {
            using var trans = await _context.Database.BeginTransactionAsync();
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentCode == req.StudentCode);
                if (student == null) return NotFound("Không tìm thấy sinh viên");

                var contract = await _context.Contracts.FirstOrDefaultAsync(c => c.StudentId == student.Id && c.Status == 1);
                if (contract == null) return BadRequest("Không tìm thấy hợp đồng hiệu lực");

                // --- BỔ SUNG DÒNG NÀY ---
                contract.CheckInDate = DateTime.Now;
                _context.Contracts.Update(contract);

                // Cập nhật trạng thái sinh viên
                student.Status = "active";
                student.RoomId = contract.RoomId;
                _context.Students.Update(student);

                // Lưu log giao dịch
                var log = new RoomTransaction
                {
                    StudentId = student.Id,
                    RoomId = contract.RoomId,
                    TransactionType = "Check-in",
                    TransactionDate = DateTime.Now,
                    Note = req.Note ?? "Nhận phòng thực tế"
                };

                _context.RoomTransactions.Add(log);
                await _context.SaveChangesAsync();
                await trans.CommitAsync();

                return Ok(new { message = "Xác nhận nhận phòng thành công." });
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }

        // ==========================================================
        // 4. XỬ LÝ CHECK-OUT
        // ==========================================================
        [HttpPost("checkout/{studentCode}")]
        public async Task<IActionResult> Checkout(string studentCode)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentCode == studentCode);
            if (student == null) return NotFound("Không tìm thấy sinh viên");

            // 1. Tìm hợp đồng đang hiệu lực
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.StudentId == student.Id && c.Status == 1);

            // 2. Cập nhật trạng thái sinh viên (Rời phòng thực tế)
            student.Status = "Chưa xếp phòng"; // Frontend sẽ dựa vào cái này để hiện "Đã trả phòng thực tế"
            student.RoomId = null;
            _context.Students.Update(student);

            // 3. Lưu log giao dịch
            var transaction = new RoomTransaction
            {
                StudentId = student.Id,
                RoomId = contract?.RoomId ?? 0,
                TransactionType = "Check-out",
                TransactionDate = DateTime.Now,
                Note = "Sinh viên dọn ra khỏi phòng thực tế"
            };
            _context.RoomTransactions.Add(transaction);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Trả phòng thực tế thành công" });
        }
        public class CheckInReq
        {
            public string StudentCode { get; set; } = "";
            public string? Note { get; set; }
        }
    }
}