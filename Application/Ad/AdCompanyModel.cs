namespace Application.Ad;

public sealed class AdCompanyModel
{
    public string CompanyName { get; set; }

    public List<string> Regions { get; set; }

    public AdCompanyModel(string companyName, List<string> regions)
    {
        CompanyName = companyName;
        Regions = regions;
    }
}