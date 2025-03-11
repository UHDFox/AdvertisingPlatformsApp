namespace Domain.Entities;

public sealed class AdCompanyEntity
{
    public string CompanyName { get; set; }

    public List<string> Regions { get; set; }

    public AdCompanyEntity(string companyName, List<string> regions)
    {
        CompanyName = companyName;
        Regions = regions;
    }
}