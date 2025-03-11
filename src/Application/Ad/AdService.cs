using System.Text;
using Application.Infrastructure.Exceptions;
using Repository;

namespace Application.Ad;

public sealed class AdService : IAdService
{
    private readonly IAdCompanyRepository _adCompanyRepository;

    public AdService(IAdCompanyRepository adCompanyRepository)
    {
        _adCompanyRepository = adCompanyRepository;
    }

    public async Task ReadAdsDataFromFileAsync(Stream stream, CancellationToken cancellationToken)
    {
        _adCompanyRepository.PurgeData(); //  purge all previous data

        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
            int lineNumber = 0;

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                lineNumber++;

                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(':');
                if (parts.Length != 2) continue;

                var companyName = parts[0].Trim();
                if (companyName.Length == 0)
                {
                    stream.Close();
                    throw new EmptyLineException(
                        $"Company name is empty on line {lineNumber}. Please provide a valid company name.");
                }

                var regions = parts[1].Split(',').Select(r => r.Trim()).ToList();

                _adCompanyRepository.Add(companyName, regions);
            }
        }
    }

    public IReadOnlyList<AdCompanyModel> SearchAdCompaniesByRegion(string region)
    {
        if (_adCompanyRepository.Get().Count == 0)
        {
            throw new AdDataNotLoadedException(
                "\"Ad companies data is missing. Please, load data before requesting any information.\"");
        }

        var regionPrefixes = GetRegionPrefixes(region);

        return _adCompanyRepository.Get()
            .Where(ad => ad.Value.Regions
                .Any(r => regionPrefixes.Contains(r) || regionPrefixes.Any(prefix => r.EndsWith(prefix))))
            .Select(ad => new AdCompanyModel(ad.Key, ad.Value.Regions))
            .ToList();
    }

    private IReadOnlyList<string> GetRegionPrefixes(string region)
    {
        var prefixes = new List<string>();

        var parts = region.Split('/', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i <= parts.Length; i++)
        {
            prefixes.Add("/" + string.Join('/', parts.Take(i)));
        }

        return prefixes;
    }
}