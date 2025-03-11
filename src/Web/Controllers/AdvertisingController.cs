using Application.Ad;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Controller]
[Route("api/[controller]")]
public sealed class AdvertisingController : Controller
{
    private readonly IAdService _service;

    public AdvertisingController(IAdService service)
    {
        _service = service;
    }

    [HttpPost("load")]
    public async Task<IActionResult> ReadAdvertisingData(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("The file is missing or empty");
        }

        await using (var stream = file.OpenReadStream())
        {
            try
            {
                await _service.ReadAdsDataFromFileAsync(stream, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                stream.Close();
                return BadRequest(e.Message);
            }
        }

        return Ok();
    }

    [HttpGet("search")]
    public IActionResult GetAdCompaniesByRegion(string region)
    {
        if (string.IsNullOrWhiteSpace(region))
        {
            return BadRequest("The region is missing or empty");
        }

        var result = _service.SearchAdCompaniesByRegion(region);

        return Ok(result);
    }
}