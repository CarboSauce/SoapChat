namespace SoapChat.Api.Features.Models;

public class UserPayload
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

public class AuthPayload
{
    public UserPayload? User { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
}
