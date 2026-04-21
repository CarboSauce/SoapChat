namespace SoapChat.Api.Features.Query;

public record User(string Id, string Username);

[QueryType]
public static class Query
{
    public static User GetUser() => new User("stub", "stub");
}
