using Domain.Entities;

namespace Repository;

public interface IAdCompanyRepository
{
    public void Add(string companyName, List<string> regions);

    public Dictionary<string, List<AdCompanyEntity>> Get();

    public void PurgeData();
}