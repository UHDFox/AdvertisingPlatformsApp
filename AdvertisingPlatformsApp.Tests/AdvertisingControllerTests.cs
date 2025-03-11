using Application.Ad;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers;

namespace AdvertisingPlatformsApp.Tests;

public sealed class AdvertisingControllerTests
{
    private readonly Mock<IAdService> _adServiceMock;

    private readonly AdvertisingController _advertisingController;
    
    public AdvertisingControllerTests()
    {
        _adServiceMock = new Mock<IAdService>();
        _advertisingController = new AdvertisingController(_adServiceMock.Object);
    }
    
    [Fact]
    public async Task ReadAdvertisingData_FileIsNull_ReturnsBadRequest()
    {
        // Arrange
        IFormFile? file = null;

        // Act
        var result = await _advertisingController.ReadAdvertisingData(file);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("The file is missing or empty", badRequestResult.Value);
    }

    [Fact]
    public async Task ReadAdvertisingData_FileIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        IFormFile? file = null;
        
        // Act
        var result = await _advertisingController.ReadAdvertisingData(file);
        
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        Assert.Equal("The file is missing or empty", badRequestResult.Value);
    }
    
    [Fact]
    public async Task ReadAdvertisingData_ThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(100);
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        _adServiceMock.Setup(s => s.ReadAdsDataFromFileAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Object reference not set to an instance of an object."));

        // Act
        var result = await _advertisingController.ReadAdvertisingData(fileMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Object reference not set to an instance of an object.", badRequestResult.Value);
    }
    
    [Fact]
    public async Task ReadAdvertisingData_Success_ReturnsOk()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(100);
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        var adServiceMock = new Mock<IAdService>();
        adServiceMock.Setup(s => s.ReadAdsDataFromFileAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var controller = new AdvertisingController(adServiceMock.Object);
        
        var httpContext = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext 
        };

        // Act
        var result = await controller.ReadAdvertisingData(fileMock.Object);

        // Assert
        var okResult = Assert.IsType<OkResult>(result); 
        Assert.NotNull(okResult);
    }
    // тесты для GetAdCompaniesByRegion
    
    [Fact]
    public void GetAdCompaniesByRegion_RegionIsNull_ReturnsBadRequest()
    {
        // Arrange
        string? region = null;

        // Act
        var result = _advertisingController.GetAdCompaniesByRegion(region);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("The region is missing or empty", badRequestResult.Value);
    }
    
    [Fact]
    public void GetAdCompaniesByRegion_RegionIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        string region = "";

        // Act
        var result = _advertisingController.GetAdCompaniesByRegion(region);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("The region is missing or empty", badRequestResult.Value);
    }
    
    [Fact]
    public void GetAdCompaniesByRegion_Success_ReturnsOk()
    {
        // Arrange
        string region = "North America";

        var adCompanies = new List<AdCompanyModel>
        {
            new AdCompanyModel("Company1", new List<string> { "North America", "USA" }),
            new AdCompanyModel("Company2", new List<string> { "North America", "Canada" })
        };

        _adServiceMock.Setup(s => s.SearchAdCompaniesByRegion(region)).Returns(adCompanies);

        // Act
        var result = _advertisingController.GetAdCompaniesByRegion(region);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedList = Assert.IsType<List<AdCompanyModel>>(okResult.Value);
        Assert.Equal(2, returnedList.Count);
    }
    
    [Fact]
    public void GetAdCompaniesByRegion_NoCompaniesFound_ReturnsOk()
    {
        // Arrange
        string region = "NonExistentRegion";

        var adCompanies = new List<AdCompanyModel>();

        _adServiceMock.Setup(s => s.SearchAdCompaniesByRegion(region)).Returns(adCompanies);

        // Act
        var result = _advertisingController.GetAdCompaniesByRegion(region);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedList = Assert.IsType<List<AdCompanyModel>>(okResult.Value);
        Assert.Empty(returnedList);
    }
}
    
