using Microsoft.EntityFrameworkCore;

using CodeChallenge.Api.Models;

namespace CodeChallenge.Api.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Member> Members => Set<Member>();
    public DbSet<Medication> Medications => Set<Medication>();
}