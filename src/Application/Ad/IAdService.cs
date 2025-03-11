namespace Application.Ad;

public interface IAdService
{
    public Task ReadAdsDataFromFileAsync(Stream stream, CancellationToken cancellationToken);

    public IReadOnlyList<AdCompanyModel> SearchAdCompaniesByRegion(string region);
}