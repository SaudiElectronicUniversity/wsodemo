using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Wso2MvcDemo.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiController> _logger;

    public ApiController(IHttpClientFactory httpClientFactory, ILogger<ApiController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpGet("userinfo")]
    public async Task<IActionResult> GetUserInfo()
    {
        try
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = _httpClientFactory.CreateClient("WSO2API");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync("/oauth2/userinfo");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                return Ok(userInfo);
            }

            return StatusCode((int)response.StatusCode, "Failed to retrieve user info");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user info from WSO2");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("validate-token")]
    public async Task<IActionResult> ValidateToken()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        return Ok(new
        {
            HasToken = !string.IsNullOrEmpty(accessToken),
            Token = accessToken?.Substring(0, Math.Min(20, accessToken.Length)) + "...",
            User = User.Identity?.Name,
            IsAuthenticated = User.Identity?.IsAuthenticated
        });
    }
}
