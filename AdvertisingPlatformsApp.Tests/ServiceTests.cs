using System.Text;
using Application.Ad;
using Application.Infrastructure.Exceptions;
using Domain.Entities;
using Moq;
using Repository;

namespace AdvertisingPlatformsApp.Tests;

public sealed class ServiceTests
{
    private readonly Mock<IAdCompanyRepository> _adCompanyRepositoryMock;
    private readonly AdService _adService;

    public ServiceTests()
    {
        _adCompanyRepositoryMock = new Mock<IAdCompanyRepository>();
        _adService = new AdService(_adCompanyRepositoryMock.Object);
    }

    [Fact]
    public async Task ReadAdsDataFromFileAsync_SuccessfullyReadsData()
    {
        // Arrange
        var fileContent = "Company1:Region1,Region2\nCompany2:Region3,Region4";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        
        _adCompanyRepositoryMock.Setup(r => r.PurgeData()); // Mocking the PurgeData method
        _adCompanyRepositoryMock.Setup(r => r.Add(It.IsAny<string>(), It.IsAny<List<string>>())); // Mocking the Add method

        // Act
        await _adService.ReadAdsDataFromFileAsync(stream, CancellationToken.None);

        // Assert
        _adCompanyRepositoryMock.Verify(r => r.PurgeData(), Times.Once);
        _adCompanyRepositoryMock.Verify(r => r.Add("Company1", It.Is<List<string>>(regions => regions.Count == 2)), Times.Once);
        _adCompanyRepositoryMock.Verify(r => r.Add("Company2", It.Is<List<string>>(regions => regions.Count == 2)), Times.Once);
    }
    
    [Fact]
    public async Task ReadAdsDataFromFileAsync_ThrowsEmptyLineException_WhenCompanyNameIsEmpty()
    {
        // Arrange
        var fileContent = ":Region1,Region2"; // Empty company name
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        _adCompanyRepositoryMock.Setup(r => r.PurgeData()); // Mocking the PurgeData method

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EmptyLineException>(() => _adService.ReadAdsDataFromFileAsync(stream, CancellationToken.None));
        Assert.Equal("Company name is empty on line 1. Please provide a valid company name.", exception.Message);
    }
    
    [Fact]
    public async Task ReadAdsDataFromFileAsync_IgnoringEmptyLines()
    {
        // Arrange
        var fileContent = "\n\n";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        _adCompanyRepositoryMock.Setup(r => r.PurgeData()); // Mocking the PurgeData method

        // Act
        await _adService.ReadAdsDataFromFileAsync(stream, CancellationToken.None);

        // Assert
        _adCompanyRepositoryMock.Verify(r => r.PurgeData(), Times.Once);
        _adCompanyRepositoryMock.Verify(r => r.Add(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
    }

    // Тесты для SearchAdCompaniesByRegion
    
    [Fact]
    public void SearchAdCompaniesByRegion_Success_ReturnsAdCompanies()
    {
        // Arrange
        var region = "Region1";
        var companyData = new Dictionary<string, List<AdCompanyEntity>>
        {
            { "Region1", new List<AdCompanyEntity>
                {
                    new AdCompanyEntity("Company1", new List<string> { "Region1", "Region2" }),
                    new AdCompanyEntity("Company2", new List<string> { "Region1", "Region3" })
                }
            }
        };

        _adCompanyRepositoryMock.Setup(r => r.Get()).Returns(companyData);

        // Act
        var result = _adService.SearchAdCompaniesByRegion(region);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.CompanyName == "Company1");
        Assert.Contains(result, c => c.CompanyName == "Company2");
    }

    [Fact]
    public void SearchAdCompaniesByRegion_ThrowsAdDataNotLoadedException_WhenNoDataLoaded()
    {
        // Arrange
        _adCompanyRepositoryMock.Setup(r => r.Get()).Returns(new Dictionary<string, List<AdCompanyEntity>>());

        // Act & Assert
        var exception = Assert.Throws<AdDataNotLoadedException>(() => _adService.SearchAdCompaniesByRegion("Region1"));
        Assert.Equal("\"Ad companies data is missing. Please, load data before requesting any information.\"", exception.Message);
    }

    [Fact]
    public void SearchAdCompaniesByRegion_ReturnsEmptyList_WhenRegionNotFound()
    {
        // Arrange
        var region = "NonExistentRegion";
        var companyData = new Dictionary<string, List<AdCompanyEntity>>
        {
            { "Region1", new List<AdCompanyEntity>
                {
                    new AdCompanyEntity("Company1", new List<string> { "Region1", "Region2" })
                }
            }
        };

        _adCompanyRepositoryMock.Setup(r => r.Get()).Returns(companyData);

        // Act
        var result = _adService.SearchAdCompaniesByRegion(region);

        // Assert
        Assert.Empty(result);
    }

}
