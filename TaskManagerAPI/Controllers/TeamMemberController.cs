using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;


namespace TaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamMemberController : ControllerBase
    {
        private readonly TaskManagerContext _context;
        private readonly ILogger<TeamMemberController> _logger;

        public TeamMemberController(TaskManagerContext context, ILogger<TeamMemberController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamMemberDTO>>> GetTeamMembers()
        {
            var members = await _context.TeamMembers
                .Select(m => new TeamMemberDTO
                {
                    MemberId = m.MemberId,
                    Name = m.Name,
                    Email = m.Email
                })
                .ToListAsync();

            return members;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamMemberDTO>> GetTeamMember(int id)
        {
            var member = await _context.TeamMembers
                .Where(m => m.MemberId == id)
                .Select(m => new TeamMemberDTO
                {
                    MemberId = m.MemberId,
                    Name = m.Name,
                    Email = m.Email
                })
                .FirstOrDefaultAsync();

            if (member == null)
            {
                _logger.LogWarning("Team member with ID {MemberId} not found", id);
                return NotFound();
            }

            return member;
        }

        [HttpPost]
        public async Task<ActionResult<TeamMemberDTO>> PostTeamMember(TeamMemberDTO memberDTO)
        {
            var member = new TeamMember
            {
                Name = memberDTO.Name,
                Email = memberDTO.Email
            };

            _context.TeamMembers.Add(member);
            await _context.SaveChangesAsync();

            memberDTO.MemberId = member.MemberId;

            _logger.LogInformation("Created new team member with ID {MemberId}", member.MemberId);
            return CreatedAtAction(nameof(GetTeamMember), new { id = member.MemberId }, memberDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeamMember(int id, TeamMemberDTO memberDTO)
        {
            if (id != memberDTO.MemberId)
            {
                _logger.LogWarning("Team member ID {MemberId} does not match route ID {RouteId}", memberDTO.MemberId, id);
                return BadRequest();
            }

            var member = await _context.TeamMembers.FindAsync(id);
            if (member == null)
            {
                _logger.LogWarning("Team member with ID {MemberId} not found", id);
                return NotFound();
            }

            member.Name = memberDTO.Name;
            member.Email = memberDTO.Email;

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamMemberExists(id))
                {
                    _logger.LogWarning("Team member with ID {MemberId} not found during update", id);
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            _logger.LogInformation("Updated team member with ID {MemberId}", member.MemberId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamMember(int id)
        {
            var member = await _context.TeamMembers.FindAsync(id);
            if (member == null)
            {
                _logger.LogWarning("Team member with ID {MemberId} not found during deletion", id);
                return NotFound();
            }

            _context.TeamMembers.Remove(member);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted team member with ID {MemberId}", id);
            return NoContent();
        }

        private bool TeamMemberExists(int id)
        {
            return _context.TeamMembers.Any(e => e.MemberId == id);
        }
    }
}
