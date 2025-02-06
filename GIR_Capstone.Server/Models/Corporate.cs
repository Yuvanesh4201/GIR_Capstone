using System.ComponentModel.DataAnnotations;

public class Corporate
{
    [Key]
    public int StructureId { get; set; }
    public string MNEName { get; set; } = string.Empty;
    public int XMLSubmissionId { get; set; }
}