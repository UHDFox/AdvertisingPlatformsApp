using System.Text;
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

        using (var stream = file.OpenReadStream())
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
    public async Task<IActionResult> GetAdCompaniesByRegion(string region)
    {
        if (string.IsNullOrWhiteSpace(region))
        {
            return BadRequest("The region is missing or empty");
        }

        var result = _service.SearchAdCompaniesByARegion(region);

        return Ok(result);
    }


    private async Task<Dictionary<string, List<string>>> ParseFileAsync(IFormFile file)
    {
        Dictionary<string, List<string>> ads = new();

        using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
        {
            while (!stream.EndOfStream)
            {
                var line = await stream.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(':');
                if (parts.Length != 2) continue;

                string companyName = parts[0].Trim();
                List<string> regions = parts[1].Split(',').Select(r => r.Trim()).ToList();

                ads[companyName] = regions;
            }
        }

        return ads;
    }
}