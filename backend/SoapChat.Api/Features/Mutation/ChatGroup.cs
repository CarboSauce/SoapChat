namespace SoapChat.Api.Features.Mutation;

using System.Security.Claims;
using System.ServiceModel;
using HotChocolate;
using SoapChat.Api.ChatGroupService;
using SoapChat.Api.Features.Models;

[MutationType]
public static class ChatGroupMutation
{
    public static async Task<ChatGroupPayload> CreateGroup(
        string name,
        IHttpContextAccessor httpContextAccessor,
        ChatGroupServiceClient chatGroupServiceClient
    )
    {
        try
        {
            var userId =
                httpContextAccessor
                    .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                    ?.Value ?? throw new GraphQLException("Not authenticated");

            var resp = await chatGroupServiceClient.CreateGroupAsync(name);
            if (resp == null)
                throw new GraphQLException("Failed to create group");

            var updated = await chatGroupServiceClient.AddMemberAsync(
                resp.Id,
                userId
            );

            return updated == null
                ? throw new GraphQLException("Failed to add creator to group")
                : updated.ToPayload();
        }
        catch (FaultException ex)
        {
            throw new GraphQLException(ex.Message);
        }
    }

    public static async Task<ChatGroupPayload> AddMember(
        string groupId,
        string userId,
        ChatGroupServiceClient chatGroupServiceClient
    )
    {
        try
        {
            var resp = await chatGroupServiceClient.AddMemberAsync(
                groupId,
                userId
            );
            return resp == null
                ? throw new GraphQLException("Group not found")
                : resp.ToPayload();
        }
        catch (FaultException ex)
        {
            throw new GraphQLException(ex.Message);
        }
    }

    public static async Task<ChatGroupPayload> RemoveMember(
        string groupId,
        string userId,
        ChatGroupServiceClient chatGroupServiceClient
    )
    {
        try
        {
            var resp = await chatGroupServiceClient.RemoveMemberAsync(
                groupId,
                userId
            );
            return resp == null
                ? throw new GraphQLException("Group not found")
                : resp.ToPayload();
        }
        catch (FaultException ex)
        {
            throw new GraphQLException(ex.Message);
        }
    }
}
