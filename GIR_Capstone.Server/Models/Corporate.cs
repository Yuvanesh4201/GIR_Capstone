using System.ComponentModel.DataAnnotations;

public class Corporate
{
    [Key]
    public int StructureId { get; set; }
    public string MneName { get; set; } = string.Empty;
    public int XmlSubmissionId { get; set; }
}