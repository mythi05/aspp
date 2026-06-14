using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aspp.Models;
using aspp.Data;

namespace aspp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractApiController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    // ==========================================================
    // 1. LẬP HỢP ĐỒNG MỚI & TỰ ĐỘNG TẠO HÓA ĐƠN
    // ==========================================================
    [HttpPost]
    public async Task<IActionResult> CreateContract([FromBody] Contract contract)
    {
        if (contract == null) return BadRequest();

        // Kiểm tra sinh viên có đang ở phòng khác không (Hợp đồng Status = 1)
        var isStayingCount = await _context.Contracts
            .CountAsync(c => c.StudentId == contract.StudentId && c.Status == 1);

        if (isStayingCount > 0)
            return BadRequest(new { message = "Sinh viên này hiện đang có một hợp đồng hiệu lực." });

        contract.Status = 1;
        contract.CreatedAt = DateTime.Now;
        if (contract.StartDate == default) contract.StartDate = DateTime.Now;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Bước A: Lưu hợp đồng cư trú
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            // Bước B: Tự động tạo hóa đơn đồng bộ (Nghiệp vụ thu trọn gói học kỳ)
            // Logic: RoomFee = Tiền phòng x 5 tháng, UtilityFee = Tiền cọc bàn giao
            decimal totalRoomFee = contract.MonthlyFee * 6;
            decimal depositFee = contract.Deposit;

            var autoInvoice = new Invoice
            {
                StudentId = contract.StudentId,
                RoomId = contract.RoomId,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,

                // Gán đúng tên trường theo Model Invoice.cs đã sửa
                RoomFee = totalRoomFee,
                UtilityFee = depositFee,
                TotalAmount = totalRoomFee + depositFee,

                Status = "Chưa thanh toán",
                DueDate = DateTime.Now.AddDays(7) // Hạn nộp là 7 ngày kể từ khi ký
            };

            _context.Invoices.Add(autoInvoice);

            // Bước C: Cập nhật trạng thái sinh viên sang "Đã xếp phòng"
            var student = await _context.Students.FindAsync(contract.StudentId);
            if (student != null)
            {
                student.Status = "Đã xếp phòng";
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(contract);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
        }
    }
    [HttpPost("terminate/{id}")]
    public async Task<IActionResult> Terminate(int id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null) return NotFound();

        contract.Status = 0; // Đánh dấu hợp đồng Đã thanh lý

        // Cập nhật trạng thái sinh viên để giải phóng chỗ ở
        var student = await _context.Students.FindAsync(contract.StudentId);
        if (student != null)
        {
            student.Status = "Chưa xếp phòng";
            student.RoomId = null;
            _context.Students.Update(student);
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
    // ==========================================================
    // 2. DANH SÁCH HỢP ĐỒNG + TÌM KIẾM + TỰ ĐỘNG QUÉT QUÁ HẠN
    // ==========================================================
    [HttpGet]
    public async Task<IActionResult> GetContracts(string? searchString)
    {
        var today = DateTime.Today;

        // Quét và cập nhật các hợp đồng đã quá ngày kết thúc nhưng chưa thanh lý
        var expiredContracts = await _context.Contracts
            .Where(c => c.Status == 1 && c.EndDate.Date < today)
            .ToListAsync();

        if (expiredContracts.Count > 0)
        {
            foreach (var c in expiredContracts) c.Status = 0;
            await _context.SaveChangesAsync();
        }

        var query = _context.Contracts
            .Include(c => c.Student)
            .Include(c => c.Room)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(c =>
                c.ContractCode.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                (c.Student != null && c.Student.FullName != null && c.Student.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                (c.Student != null && c.Student.StudentCode != null && c.Student.StudentCode.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
        }

        var result = await query
            .OrderByDescending(c => c.Status) // Hợp đồng hiệu lực lên đầu
            .ThenByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Ok(result);
    }

    // ==========================================================
    // 3. THÔNG SỐ DASHBOARD
    // ==========================================================
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var today = DateTime.Today;
        return Ok(new
        {
            ActiveContracts = await _context.Contracts.CountAsync(c => c.Status == 1),
            ExpiredContracts = await _context.Contracts.CountAsync(c => c.Status == 0),
            TotalDeposit = await _context.Contracts.Where(c => c.Status == 1).SumAsync(c => c.Deposit),
            NearExpiry = await _context.Contracts.CountAsync(c => c.Status == 1 && c.EndDate <= today.AddDays(30))
        });
    }
    // ==========================================================
    // 6. GIA HẠN / CẬP NHẬT HỢP ĐỒNG (Xử lý lỗi 405)
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContract(int id, [FromBody] Contract contractUpdate)
    {
        if (id != contractUpdate.Id)
        {
            return BadRequest(new { message = "ID không trùng khớp." });
        }

        var existingContract = await _context.Contracts.FindAsync(id);
        if (existingContract == null)
        {
            return NotFound(new { message = "Không tìm thấy hợp đồng." });
        }

        // Chỉ cập nhật ngày kết thúc (vì trường này chắc chắn có)
        existingContract.EndDate = contractUpdate.EndDate;
        existingContract.Status = 1;

        // BỎ DÒNG: existingContract.Note = contractUpdate.Note; <--- Xóa dòng này đi

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { message = "Gia hạn hợp đồng thành công." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Lỗi khi lưu dữ liệu: {ex.Message}");
        }
    }
    // ==========================================================
    // 5. XÓA HỢP ĐỒNG
    // ==========================================================
    // ==========================================================
    // 5. XÓA HỢP ĐỒNG & XÓA CÁC HÓA ĐƠN LIÊN QUAN
    // ==========================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContract(int id)
    {
        // 1. Tìm hợp đồng
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null) return NotFound(new { message = "Không tìm thấy hợp đồng." });

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 2. Tìm và xóa tất cả hóa đơn tự động của hợp đồng này 
            // (Thường là các hóa đơn có cùng StudentId và RoomId phát sinh trong chu kỳ hợp đồng)
            // Lưu ý: Chỉ nên xóa các hóa đơn "Chưa thanh toán" để tránh mất dữ liệu kế toán đã thu tiền
            var relatedInvoices = await _context.Invoices
                .Where(i => i.StudentId == contract.StudentId && i.RoomId == contract.RoomId && i.Status == "Chưa thanh toán")
                .ToListAsync();

            if (relatedInvoices.Any())
            {
                _context.Invoices.RemoveRange(relatedInvoices);
            }

            // 3. Cập nhật lại trạng thái sinh viên về "Chưa xếp phòng"
            var student = await _context.Students.FindAsync(contract.StudentId);
            if (student != null)
            {
                student.Status = "Chưa xếp phòng";
            }

            // 4. Xóa hợp đồng
            _context.Contracts.Remove(contract);

            // Lưu thay đổi
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { message = "Đã xóa hợp đồng và các hóa đơn chưa thanh toán liên quan." });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Lỗi khi xóa dữ liệu: {ex.Message}");
        }
   
}
}