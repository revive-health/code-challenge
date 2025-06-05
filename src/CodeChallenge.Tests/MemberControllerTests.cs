using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
}