namespace CodeChallenge.Api.Models;

public class Member : Crud
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public List<Medication> Medications { get; set; } = new();
}