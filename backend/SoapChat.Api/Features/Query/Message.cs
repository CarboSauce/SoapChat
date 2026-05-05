namespace SoapChat.Api.Features.Query;

using System.ServiceModel;
using HotChocolate;
using SoapChat.Api.Features.Models;
using SoapChat.Api.MessageService;

[QueryType]
public static class MessageQuery
{
    public static async Task<MessagePayload> GetMessage(
        string id,
        MessageServiceClient messageServiceClient
    )
    {
        try
        {
            var resp = await messageServiceClient.GetMessageAsync(id);
            return resp == null
                ? throw new GraphQLException("Message not found")
                : resp.ToPayload();
        }
        catch (FaultException ex)
        {
            throw new GraphQLException(ex.Message);
        }
    }

    public static async Task<MessagePayload[]> GetMessages(
        string groupId,
        int skip,
        int limit,
        MessageServiceClient messageServiceClient
    )
    {
        try
        {
            var resp = await messageServiceClient.GetMessagesAsync(
                groupId,
                skip,
                limit
            );
            return resp?.Select(x => x.ToPayload()).ToArray() ?? [];
        }
        catch (FaultException ex)
        {
            throw new GraphQLException(ex.Message);
        }
    }

    public static async Task<MessagePayload[]> GetNewerMessages(
        string groupId,
        string lastMessageId,
        MessageServiceClient messageServiceClient
    )
    {
        try
        {
            var resp = await messageServiceClient.GetNewerMessagesAsync(
                groupId,
                lastMessageId
            );
            return resp?.Select(x => x.ToPayload()).ToArray() ?? [];
        }
        catch (FaultException ex)
        {
            throw new GraphQLException(ex.Message);
        }
    }
}
