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

    private async Task<Dictionary<string, List<string>>> ParseRegionsFromALocalFileAsync(string filePath)
    {
        var regions = new Dictionary<string, List<string>>();

        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found: " + filePath);
            return regions;
        }

        using var reader = new StreamReader(filePath, Encoding.UTF8);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var lineData = line.Split(':');
            if (lineData.Length != 2) continue;

            var advertisingCompany = lineData[0].Trim();
            var lineRegions = lineData[1].Trim().Split(',').Select(r => r.Trim()).ToList();

            regions[advertisingCompany] = lineRegions;
        }

        return regions;
    }
}