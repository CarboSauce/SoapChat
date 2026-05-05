using System.Security.Claims;

namespace SoapChat.Api.Features.Mutation;

using System.ServiceModel;
using HotChocolate;
using SoapChat.Api.Features.Models;
using SoapChat.Api.MessageService;

[MutationType]
public static class MessageMutation
{
    public static async Task<MessagePayload> CreateMessage(
        string groupId,
        string text,
        ClaimsPrincipal claims,
        MessageServiceClient messageServiceClient
    )
    {
        try
        {
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new GraphQLException("Not authenticated");
            var resp = await messageServiceClient.CreateMessageAsync(
                groupId,
                userId,
                text
            );
            return resp == null
                ? throw new GraphQLException("Failed to create message")
                : resp.ToPayload();
        }
        catch (FaultException ex)
        {
            throw new GraphQLException(ex.Message);
        }
    }
}
