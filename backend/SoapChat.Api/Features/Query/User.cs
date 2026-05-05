namespace SoapChat.Api.Features.Query;

using System.Security.Claims;
using HotChocolate;
using SoapChat.Api.UserService;

public record User(string Id, string Username);

[QueryType]
public static class Query
{
    public static async Task<User> GetUser(
        string id,
        UserServiceClient userServiceClient
    )
    {
        var resp = await userServiceClient.GetUserAsync(id);
        return resp == null
            ? throw new GraphQLException("User not found")
            : new User(resp.Id, resp.Name);
    }

    public static async Task<User[]> SearchUsers(
        string nameSearch,
        UserServiceClient userServiceClient
    )
    {
        var resps = await userServiceClient.SearchUsersAsync(nameSearch);
        return resps?.Select(u => new User(u.Id, u.Name)).ToArray() ?? [];
    }

    public static async Task<User> Me(
        ClaimsPrincipal claims,
        UserServiceClient userServiceClient
    )
    {
        var sub = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(sub))
            throw new GraphQLException("Not authenticated");

        var resp = await userServiceClient.GetUserAsync(sub);
        return resp == null
            ? throw new GraphQLException("User not found")
            : new User(resp.Id, resp.Name);
    }
}
