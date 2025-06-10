namespace CodeChallenge.Api.Dtos;

public class MemberSummaryResponseDto
{
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; } 
    public int MedicationCount { get; set; }
}