using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class CodeDecodeOwnershipType
{
    [Key]
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DecodeDescription { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;

}
