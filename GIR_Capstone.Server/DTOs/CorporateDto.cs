public class CorporateDto
{
    public Guid Structure_Id { get; set; }
    public string MNEName { get; set; } = string.Empty;

}

public class CorporateRequestModel
{
   public string CorporateId { get; set; }
}
