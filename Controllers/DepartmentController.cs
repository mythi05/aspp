using aspp.Data;
using aspp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartmentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Department
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Departments.ToListAsync());
        }

        // POST: api/Department
        [HttpPost]
        public async Task<IActionResult> Create(Department d)
        {
            _context.Departments.Add(d);
            await _context.SaveChangesAsync();
            return Ok(d);
        }
    }
}