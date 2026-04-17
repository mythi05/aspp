using aspp.Data;
using aspp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Role
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Roles.ToListAsync());
        }

        // POST: api/Role
        [HttpPost]
        public async Task<IActionResult> Create(Role r)
        {
            _context.Roles.Add(r);
            await _context.SaveChangesAsync();
            return Ok(r);
        }
    }
}