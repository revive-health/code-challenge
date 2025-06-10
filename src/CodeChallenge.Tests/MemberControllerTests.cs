using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

using CodeChallenge.Api.Controllers;
using CodeChallenge.Api.Dtos;
using CodeChallenge.Api.Models;
using CodeChallenge.Api.Data;


namespace MemberMedicationApi.Tests;

public class MembersControllerTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // new DB each test
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CreateMember_ReturnsCreatedMember()
    {
        using var db = GetInMemoryDbContext();
        var controller = new MembersController(db);
        var dto = new MemberCreateDto
        {
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var member = Assert.IsType<Member>(created.Value);
        Assert.Equal("Jane", member.FirstName);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenMemberDoesNotExist()
    {
        using var db = GetInMemoryDbContext();
        var controller = new MembersController(db);

        var result = await controller.Get(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Get_ReturnsMember_WhenExists()
    {
        using var db = GetInMemoryDbContext();
        var controller = new MembersController(db);
        var member = new Member { FirstName = "Test", LastName = "User", DateOfBirth = DateTime.Today };
        db.Members.Add(member);
        await db.SaveChangesAsync();

        var result = await controller.Get(member.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<Member>(okResult.Value);
        Assert.Equal("Test", returned.FirstName);
    }

    [Fact]
    public async Task AddMedication_AddsSuccessfully()
    {
        using var db = GetInMemoryDbContext();
        var controller = new MembersController(db);
        var member = new Member { FirstName = "Med", LastName = "User", DateOfBirth = DateTime.Today };
        db.Members.Add(member);
        await db.SaveChangesAsync();

        var dto = new MedicationCreateDto
        {
            Name = "Ibuprofen",
            DosageMg = 200,
            PrescribedDate = DateTime.Today
        };

        var result = await controller.AddMedication(member.Id, dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var med = Assert.IsType<Medication>(created.Value);
        Assert.Equal("Ibuprofen", med.Name);
    }

    [Fact]
    public async Task AddMedication_WithNote_AddsSuccessfully()
    {
        using var db = GetInMemoryDbContext();
        var controller = new MembersController(db);
        var member = new Member { FirstName = "Test", LastName = "User", DateOfBirth = DateTime.Today };
        db.Members.Add(member);
        await db.SaveChangesAsync();

        var dto = new MedicationCreateDto
        {
            Name = "Ibuprofen",
            DosageMg = 200,
            PrescribedDate = DateTime.Today,
            Notes = "Take as needed"
        };

        var result = await controller.AddMedication(member.Id, dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var med = Assert.IsType<Medication>(created.Value);
        Assert.False(string.IsNullOrEmpty(med.Notes));
    }

    [Fact]
    public async Task AddMedication_ReturnsNotFound_WhenMemberDoesNotExist()
    {
        using var db = GetInMemoryDbContext();
        var controller = new MembersController(db);
        var dto = new MedicationCreateDto
        {
            Name = "Amoxicillin",
            DosageMg = 500,
            PrescribedDate = DateTime.Today
        };

        var result = await controller.AddMedication(42, dto);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetMedications_MemberDoesNotExist_ReturnsNotFound()
    {
        using var db = GetInMemoryDbContext();
        var controller = new MembersController(db);
        var member = new Member { Id= 1, FirstName = "Test", LastName = "User", DateOfBirth = new DateTime(1990,01,01) };
        db.Members.Add(member);
        await db.SaveChangesAsync();

        var result = await controller.GetMedications(2, null, null);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetMedications_ValidMemberNoDateFilter_ReturnsAllMeds()
    {
        using var db = GetInMemoryDbContext();
        var controller = new MembersController(db);
        var member = new Member { Id = 1, FirstName = "Test", LastName = "User", DateOfBirth = new DateTime(1990,01,01) };
        db.Members.Add(member);
        var meds = new List<Medication> {
            new Medication {Id = 1, Name = "aspirin", DosageMg = 50, PrescribedDate = new DateTime(2024, 01, 01), MemberId = 1 },
            new Medication {Id = 2, Name = "amoxicillin", DosageMg = 50, PrescribedDate = new DateTime(2024, 01, 01), MemberId = 1 },
            new Medication {Id = 3, Name = "acetaminophen", DosageMg = 50, PrescribedDate = new DateTime(2024, 01, 01), MemberId = 23 },
        };
        db.Medications.AddRange(meds);
        await db.SaveChangesAsync();

        var result = await controller.GetMedications(1, null, null);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var medsList = Assert.IsType<List<Medication>>(okResult.Value);
        Assert.Equal(2, medsList.Count);
    }

    [Fact]
    public async Task GetMedications_ValidMemberWithDateFilter_ReturnsFilteredMeds()
    {
        using var db = GetInMemoryDbContext();
        var controller = new MembersController(db);
        var member = new Member { Id = 1, FirstName = "Test", LastName = "User", DateOfBirth = new DateTime(1990,01,01) };
        db.Members.Add(member);
        var meds = new List<Medication> {
            new Medication {Id = 1, Name = "aspirin", DosageMg = 50, PrescribedDate = new DateTime(2024, 01, 01), MemberId = 1 },
            new Medication {Id = 2, Name = "amoxicillin", DosageMg = 50, PrescribedDate = new DateTime(2025, 01, 01), MemberId = 1 },
            new Medication {Id = 3, Name = "acetaminophen", DosageMg = 50, PrescribedDate = new DateTime(2025, 01, 02), MemberId = 1 },
        };
        db.Medications.AddRange(meds);
        await db.SaveChangesAsync();

        var result = await controller.GetMedications(1, new DateTime(2024, 01, 02), null);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var medsList = Assert.IsType<List<Medication>>(okResult.Value);
        Assert.Equal(2, medsList.Count);
    }

    [Fact]
    public async Task GetMemberSummary_ValidData_ReturnsSummary()
    {
        using var db = GetInMemoryDbContext();
        var controller = new MembersController(db);
        var member = new Member { Id = 1, FirstName = "Test", LastName = "User", DateOfBirth = new DateTime(1990,01,01) };
        db.Members.Add(member);
        var meds = new List<Medication> {
            new Medication {Id = 1, Name = "aspirin", DosageMg = 50, PrescribedDate = new DateTime(2024, 01, 01), MemberId = 1 },
            new Medication {Id = 2, Name = "amoxicillin", DosageMg = 50, PrescribedDate = new DateTime(2025, 01, 01), MemberId = 1 },
            new Medication {Id = 3, Name = "acetaminophen", DosageMg = 50, PrescribedDate = new DateTime(2025, 01, 02), MemberId = 1 },
        };
        db.Medications.AddRange(meds);
        await db.SaveChangesAsync();

        var result = await controller.GetMemberSummary(1);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var summary = Assert.IsType<MemberSummaryResponseDto>(okResult.Value);
        Assert.Equal(35, summary.Age);
        Assert.Equal(3, summary.MedicationCount);
        Assert.Equal("Test User", summary.FullName);
    }

    [Fact]
    public async Task GetMembersByMeds_ValidSearch_ReturnsMembers()
    {
        using var db = GetInMemoryDbContext();
        var controller = new MembersController(db);
        var members = new List<Member> {
            new Member { Id = 1, FirstName = "Test", LastName = "User", DateOfBirth = new DateTime(1990,01,01) },
            new Member { Id = 2, FirstName = "Test", LastName = "User2", DateOfBirth = new DateTime(1995,01,01) },
            new Member { Id = 3, FirstName = "Test", LastName = "User3", DateOfBirth = new DateTime(1970,01,01) },
            new Member { Id = 4, FirstName = "Test", LastName = "User4", DateOfBirth = new DateTime(1950,01,01) }
        };
    
        db.Members.AddRange(members);
        var meds = new List<Medication> {
            new Medication {Id = 1, Name = "aspirin", DosageMg = 50, PrescribedDate = new DateTime(2024, 01, 01), MemberId = 1 },
            new Medication {Id = 2, Name = "Aspirin", DosageMg = 50, PrescribedDate = new DateTime(2020, 01, 01), MemberId = 2 },
            new Medication {Id = 3, Name = "ASPIRIN", DosageMg = 50, PrescribedDate = new DateTime(2025, 01, 02), MemberId = 3 },
            new Medication {Id = 4, Name = "drug", DosageMg = 50, PrescribedDate = new DateTime(2025, 01, 02), MemberId = 4 }
        };
        db.Medications.AddRange(meds);
        await db.SaveChangesAsync();

        var result = await controller.GetMembersByMedicationName("aspirin");
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var summary = Assert.IsType<List<Member>>(okResult.Value);
        Assert.Equal(3, summary.Count);
    }
}