namespace CodeChallenge.Api.Dtos;

public class MedicationCreateDto
{
    public string Name { get; set; } = string.Empty;
    public int DosageMg { get; set; }
    public DateTime PrescribedDate { get; set; }
    public string? Notes {get; set;}
}