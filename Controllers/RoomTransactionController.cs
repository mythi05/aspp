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

        public RoomTransactionApiController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // 1. GET ALL (lọc theo loại)
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> GetTransactions(string type = "Check-in")
        {
            var data = await _context.RoomTransactions
                .Include(t => t.Student)
                .Include(t => t.Room)
                .Where(t => t.TransactionType == type)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return Ok(data);
        }

        // ==========================================================
        // 2. DASHBOARD
        // ==========================================================
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var checkinToday = await _context.RoomTransactions
                .CountAsync(t => t.TransactionType == "Check-in"
                              && t.TransactionDate.Date == DateTime.Today);

            var checkoutToday = await _context.RoomTransactions
                .CountAsync(t => t.TransactionType == "Check-out"
                              && t.TransactionDate.Date == DateTime.Today);

            var total = await _context.RoomTransactions.CountAsync();

            return Ok(new
            {
                CheckinToday = checkinToday,
                CheckoutToday = checkoutToday,
                TotalTransactions = total
            });
        }

        // ==========================================================
        // 3. CHECK-IN
        // ==========================================================
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] RoomTransaction transaction)
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                transaction.TransactionType = "Check-in";
                transaction.TransactionDate = DateTime.Now;

                _context.RoomTransactions.Add(transaction);

                // update student
                var student = await _context.Students.FindAsync(transaction.StudentId);
                if (student != null)
                    student.RoomId = transaction.RoomId;

                // update room
                var room = await _context.Rooms.FindAsync(transaction.RoomId);
                if (room != null)
                    room.DangO++;

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return BadRequest("Lỗi check-in: " + ex.Message);
            }
        }

        // ==========================================================
        // 4. CHECK-OUT
        // ==========================================================
        [HttpPost("checkout/{studentId}")]
        public async Task<IActionResult> CheckOut(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null || student.RoomId == null)
                return BadRequest("Sinh viên chưa có phòng");

            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var transaction = new RoomTransaction
                {
                    StudentId = student.Id,
                    RoomId = student.RoomId.Value,
                    TransactionType = "Check-out",
                    TransactionDate = DateTime.Now,
                    HandledBy = "Admin",
                    Note = "Trả phòng"
                };

                _context.RoomTransactions.Add(transaction);

                var room = await _context.Rooms.FindAsync(student.RoomId);
                if (room != null)
                    room.DangO--;

                student.RoomId = null;

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return BadRequest("Lỗi check-out: " + ex.Message);
            }
        }

        // ==========================================================
        // 5. DELETE
        // ==========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var item = await _context.RoomTransactions.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.RoomTransactions.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }
    }
}
