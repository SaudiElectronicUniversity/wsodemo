namespace Wso2MvcDemo.Models;

public class ProfileViewModel
{
    public IEnumerable<dynamic>? UserClaims { get; set; }
    public string? AccessToken { get; set; }
    public string? IdToken { get; set; }
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

