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
    public async Task<ActionResult<List<Medication>>> GetMedications(int memberId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        bool memberExists = await _db.Members.AnyAsync(m => m.Id == memberId);
        if(!memberExists) return NotFound();

        var medsQuery = _db.Medications.Where(m => m.MemberId == memberId).AsQueryable();

        if(fromDate.HasValue){
            medsQuery = medsQuery.Where(m => m.PrescribedDate >= fromDate.Value);
        }

        if(toDate.HasValue){
            medsQuery = medsQuery.Where(m => m.PrescribedDate <= toDate.Value);
        }

        var results = await medsQuery.ToListAsync();
        return Ok(results);
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
            Notes = dto.Notes ?? null,
            MemberId = memberId
        };

        _db.Medications.Add(medication);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMedications), new { memberId }, medication);
    }

    [HttpGet("{memberId}/summary")]
    public async Task<ActionResult<Medication>> GetMemberSummary(int memberId)
    {
        var member = await _db.Members.Include(m => m.Medications).FirstOrDefaultAsync(m => m.Id == memberId);
        if(member == null) return NotFound();

        var summary = new MemberSummaryResponseDto
        {
            FullName = member.FirstName + " " + member.LastName,
            Age = (DateTime.Today - member.DateOfBirth).Days / 365,
            MedicationCount = member.Medications.Count
        };

        return Ok(summary);
    }

    [HttpGet("medications/search")]                  
    public async Task<ActionResult<List<Member>>> GetMembersByMedicationName([FromQuery] string name){
        if(string.IsNullOrEmpty(name)) return BadRequest("Please provide a medication name.");

        var normalizedName = name.ToLower();
        var memberList = await _db.Medications.Where(m => m.Name.ToLower() == normalizedName)
                                        .Include(m => m.Member)
                                        .Select(m => m.Member)
                                        .Distinct()
                                        .ToListAsync();

        return Ok(memberList);
    }
}