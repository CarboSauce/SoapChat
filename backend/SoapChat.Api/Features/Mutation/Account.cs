using System.IO;
using System.Security.Claims;
using System.ServiceModel;
using HotChocolate;
using HotChocolate.AspNetCore;
using SoapChat.Api.ChatGroupService;
using SoapChat.Api.Features.Models;
using SoapChat.Api.MessageService;
using SoapChat.Api.Services;
using SoapChat.Api.UserService;

namespace SoapChat.Api.Features.Mutation;

[MutationType]
public static class Account
{
    public static async Task<AuthPayload> Login(
        string username,
        string password,
        UserServiceClient userServiceClient,
        JwtTokenGenerator tokenGenerator,
        IHttpContextAccessor httpContextAccessor
    )
    {
        try
        {
            var user = await userServiceClient.ValidateCredentialsAsync(
                username,
                password
            );

            if (user == null)
                throw new GraphQLException("Invalid credentials");

            var token = tokenGenerator.GenerateToken(user.Id, user.Name);

            var httpContext = httpContextAccessor.HttpContext;
            httpContext?.Response.Cookies.Append(
                "AuthToken",
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(1)
                }
            );

            return new AuthPayload
            {
                Token = token,
                User = new UserPayload { Id = user.Id, Name = user.Name },
                Message = "Login successful"
            };
        }
        catch (FaultException ex)
        {
            throw new GraphQLException(ex.Message);
        }
    }

    public static async Task<AuthPayload> Register(
        string username,
        string password,
        UserServiceClient userServiceClient,
        JwtTokenGenerator tokenGenerator,
        IHttpContextAccessor httpContextAccessor
    )
    {
        try
        {
            var user = await userServiceClient.CreateUserAsync(
                username,
                password
            );

            if (user == null)
                throw new GraphQLException("Failed to create user");

            var token = tokenGenerator.GenerateToken(user.Id, user.Name);

            var httpContext = httpContextAccessor.HttpContext;
            httpContext?.Response.Cookies.Append(
                "AuthToken",
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(1)
                }
            );

            return new AuthPayload
            {
                Token = token,
                User = new UserPayload { Id = user.Id, Name = user.Name },
                Message = "Registration successful"
            };
        }
        catch (FaultException ex)
        {
            throw new GraphQLException(ex.Message);
        }
    }
}
