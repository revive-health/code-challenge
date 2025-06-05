using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using CodeChallenge.Api.Data;
using CodeChallenge.Api.Models;
using CodeChallenge.Api.Dtos;

namespace CodeChallenge.Api.Controllers;

[ApiController]
[Route("members")]
public class MembersController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public MembersController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Member>>> GetAll() =>
        await _db.Members.Include(m => m.Medications).ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Member>> Get(int id)
    {
        var member = await _db.Members.Include(m => m.Medications).FirstOrDefaultAsync(m => m.Id == id);
        return member is null ? NotFound() : Ok(member);
    }

    [HttpPost]
    public async Task<ActionResult<Member>> Create(MemberCreateDto dto)
    {
        var member = new Member
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth
        };

        _db.Members.Add(member);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = member.Id }, member);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Member updated)
    {
        if (id != updated.Id) return BadRequest();

        var exists = await _db.Members.AnyAsync(m => m.Id == id);
        if (!exists) return NotFound();

        _db.Entry(updated).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var member = await _db.Members.FindAsync(id);
        if (member is null) return NotFound();

        _db.Members.Remove(member);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{memberId}/medications")]
    public async Task<ActionResult<IEnumerable<Medication>>> GetMedications(int memberId)
    {
        var member = await _db.Members.Include(m => m.Medications).FirstOrDefaultAsync(m => m.Id == memberId);
        return member is null ? NotFound() : Ok(member.Medications);
    }

    [HttpPost("{memberId}/medications")]
    public async Task<ActionResult<Medication>> AddMedication(int memberId, MedicationCreateDto dto)
    {
        var member = await _db.Members.FindAsync(memberId);
        if (member is null) return NotFound();

        var medication = new Medication
        {
            Name = dto.Name,
            DosageMg = dto.DosageMg,
            PrescribedDate = dto.PrescribedDate,
            MemberId = memberId
        };

        _db.Medications.Add(medication);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMedications), new { memberId }, medication);
    }
}