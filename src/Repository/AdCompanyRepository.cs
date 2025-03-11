using System.Text;
using Domain.Entities;

namespace Repository;

public sealed class AdCompanyRepository : IAdCompanyRepository
{
    private static readonly Dictionary<string, List<AdCompanyEntity>> _regions 
        = new Dictionary<string, List<AdCompanyEntity>>(StringComparer.OrdinalIgnoreCase);

    public void Add(string companyName, List<string> regions)
    {
        foreach (var region in regions)
        {
            if (!_regions.ContainsKey(region))
            {
                _regions[region] = new List<AdCompanyEntity>();
            }

            _regions[region].Add(new AdCompanyEntity(companyName, regions));
        }
    }

    public IReadOnlyDictionary<string, List<AdCompanyEntity>> Get()
    {
        return _regions;
    }

    public void PurgeData()
    {
        _regions.Clear();
    }
}