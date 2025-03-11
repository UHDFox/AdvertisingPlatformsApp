using System.Text;
using Domain.Entities;

namespace Repository;

public sealed class AdCompanyRepository : IAdCompanyRepository
{
    private static readonly Dictionary<string, AdCompanyEntity> _adCompanies 
        = new Dictionary<string, AdCompanyEntity>();

    public void Add(string companyName, List<string> regions)
    {
        _adCompanies[companyName] = new AdCompanyEntity(companyName, regions);
    }

    public Dictionary<string, AdCompanyEntity> Get()
    {
        return _adCompanies;
    }

    public void PurgeData()
    {
        _adCompanies.Clear();
    }
}