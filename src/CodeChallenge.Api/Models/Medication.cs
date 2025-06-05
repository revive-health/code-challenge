using System.Text.Json.Serialization;

namespace CodeChallenge.Api.Models;

public class Medication
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DosageMg { get; set; }
    public DateTime PrescribedDate { get; set; }

    public int MemberId { get; set; }
    [JsonIgnore]
    public Member Member { get; set; } = null!;
}
