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

    public Dictionary<string, List<AdCompanyEntity>> Get()
    {
        return _regions;
    }

    public void PurgeData()
    {
        _regions.Clear();
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