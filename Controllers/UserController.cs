using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;

        public UserController(UserDbContext context)
        {
            _context = context;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { Message = $"User with ID {id} not found." });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving user", Details = ex.Message });
            }
        }


        // GET: api/user/{id}
        // [HttpGet("{id}")]
        // public async Task<ActionResult<User>> GetUser(int id)
        // {
        //     var user = await _context.Users.FindAsync(id);
        //     if (user == null) return NotFound();
        //     return user;
        // }

        // POST: api/user
        // [HttpPost]
        // public async Task<ActionResult<User>> CreateUser(User user)
        // {
        //     _context.Users.Add(user);
        //     await _context.SaveChangesAsync();
        //     return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        // }
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error creating user", Details = ex.Message });
            }
        }

        // // PUT: api/user/{id}
        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        // {
        //     if (id != updatedUser.Id) return BadRequest();

        //     _context.Entry(updatedUser).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!_context.Users.Any(u => u.Id == id)) return NotFound();
        //         throw;
        //     }

        //     return NoContent();
        // }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return BadRequest(new { Message = "ID mismatch between route and payload." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(updatedUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(u => u.Id == id))
                {
                    return NotFound(new { Message = $"User with ID {id} not found." });
                }
                return StatusCode(500, new { Message = "Concurrency error during update." });
            }
        }
        
        // DELETE: api/user/{id}
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteUser(int id)
        // {
        //     var user = await _context.Users.FindAsync(id);
        //     if (user == null) return NotFound();

        //     _context.Users.Remove(user);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { Message = $"User with ID {id} not found." });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { Message = $"User with ID {id} deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error deleting user", Details = ex.Message });
            }
        }

        // Additional methods can be added here as needed

    }
}
