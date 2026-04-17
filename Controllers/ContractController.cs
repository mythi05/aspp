using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aspp.Models;
using aspp.Data;

namespace aspp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContractApiController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // 1. GET ALL + SEARCH + AUTO EXPIRE
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> GetContracts(string? searchString)
        {
            var today = DateTime.Now;

            // Auto update hết hạn
            var expired = await _context.Contracts
                .Where(c => c.Status == 1 && c.EndDate < today)
                .ToListAsync();

            if (expired.Any())
            {
                foreach (var c in expired)
                {
                    c.Status = 0;
                }
                await _context.SaveChangesAsync();
            }

            var query = _context.Contracts
                .Include(c => c.Student)
                .Include(c => c.Room)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c =>
                    c.ContractCode.Contains(searchString) ||
                    c.Student!.FullName!.Contains(searchString) ||
                    c.Student.StudentCode!.Contains(searchString));
            }

            var result = await query
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();

            return Ok(result);
        }

        // ==========================================================
        // 2. DASHBOARD (THỐNG KÊ)
        // ==========================================================
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var total = await _context.Contracts.CountAsync();
            var active = await _context.Contracts.CountAsync(c => c.Status == 1);
            var expired = await _context.Contracts.CountAsync(c => c.Status == 0);
            var deposit = await _context.Contracts.SumAsync(c => c.Deposit);

            return Ok(new
            {
                TotalContracts = total,
                ActiveContracts = active,
                ExpiredContracts = expired,
                TotalDeposit = deposit
            });
        }

        // ==========================================================
        // 3. GET BY ID
        // ==========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContract(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Student)
                .Include(c => c.Room)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null)
                return NotFound();

            return Ok(contract);
        }

        // ==========================================================
        // 4. CREATE
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> CreateContract([FromBody] Contract contract)
        {
            contract.Status = 1; // đang hiệu lực

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return Ok(contract);
        }

        // ==========================================================
        // 5. TERMINATE
        // ==========================================================
        [HttpPost("terminate/{id}")]
        public async Task<IActionResult> Terminate(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
                return NotFound();

            contract.Status = 0;
            contract.EndDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(contract);
        }

        // ==========================================================
        // 6. DELETE
        // ==========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
                return NotFound();

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }
    }
}
