namespace SoapChat.Api.Features.Query;

using System.ServiceModel;
using HotChocolate;
using SoapChat.Api.ChatGroupService;
using SoapChat.Api.Features.Models;

[QueryType]
public static class ChatGroupQuery
{
    public static async Task<ChatGroupPayload> GetGroup(
        string id,
        ChatGroupServiceClient chatGroupServiceClient
    )
    {
        try
        {
            var resp = await chatGroupServiceClient.GetGroupAsync(id);
            return resp == null
                ? throw new GraphQLException("Group not found")
                : resp.ToPayload();
        }
        catch (FaultException ex)
        {
            throw new GraphQLException(ex.Message);
        }
    }

    public static async Task<ChatGroupPayload[]> SearchGroupsByMember(
        string userId,
        ChatGroupServiceClient chatGroupServiceClient
    )
    {
        try
        {
            var resp = await chatGroupServiceClient.SearchGroupsByMemberAsync(
                userId
            );
            return resp?.Select(x => x.ToPayload()).ToArray() ?? [];
        }
        catch (FaultException ex)
        {
            throw new GraphQLException(ex.Message);
        }
    }
}
