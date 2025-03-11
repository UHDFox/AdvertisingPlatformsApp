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

                var parts = line.Split(':', StringSplitOptions.TrimEntries);
                if (parts.Length != 2) continue;

                var companyName = parts[0];
                if (companyName.Length == 0)
                {
                    stream.Close();
                    throw new EmptyLineException(
                        $"Company name is empty on line {lineNumber}. Please provide a valid company name.");
                }

                var regionsParsed = parts[1];

                var regions = regionsParsed.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

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

       var adCompanies = _adCompanyRepository.Get();
       
       return adCompanies[region].Select(x => 
           new AdCompanyModel(x.CompanyName, x.Regions)).ToList();
    }
}