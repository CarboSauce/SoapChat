namespace SoapChat.DbServer.Features;

using System;
using System.Collections.Generic;
using System.Linq;
using CoreWCF;
using LiteDB;

[ServiceContract]
public interface IMessageService
{
    [OperationContract]
    ChatMessageResponse CreateMessage(
        string groupId,
        string senderId,
        string text
    );

    [OperationContract]
    ChatMessageResponse GetMessage(string id);

    [OperationContract]
    ChatMessageResponse[] GetMessages(string groupId, int skip, int limit);

    [OperationContract]
    ChatMessageResponse[] GetNewerMessages(
        string groupId,
        string lastMessageId
    );
}

public class MessageService(LiteDbContext dbContext) : IMessageService
{
    private readonly LiteDatabase context = dbContext.Context;

    public ChatMessageResponse CreateMessage(
        string groupId,
        string senderId,
        string text
    )
    {
        var messages = context.GetCollection<ChatMessage>(SchemaNames.Messages);
        var groups = context.GetCollection<ChatGroup>(SchemaNames.Groups);
        var users = context.GetCollection<User>(SchemaNames.Users);

        messages.EnsureIndex(x => x.SentAt);

        var gid = new ObjectId(groupId);
        var uid = new ObjectId(senderId);

        var group =
            groups.FindById(gid)
            ?? throw new KeyNotFoundException("Group not found");
        var user =
            users.FindById(uid)
            ?? throw new KeyNotFoundException("User not found");

        var msg = new ChatMessage
        {
            Group = group,
            Sender = user,
            Text = text,
            SentAt = DateTime.UtcNow
        };

        messages.Insert(msg);
        return msg.ToResponse();
    }

    public ChatMessageResponse GetMessage(string id)
    {
        var mid = new ObjectId(id);
        var message = context
            .GetCollection<ChatMessage>(SchemaNames.Messages)
            .Include(u => u.Sender)
            .Include(u => u.Group)
            .FindById(mid);
        return message?.ToResponse()
            ?? throw new KeyNotFoundException("Message not found");
    }

    public ChatMessageResponse[] GetMessages(
        string groupId,
        int skip,
        int limit
    )
    {
        var gid = new ObjectId(groupId);
        var col = context.GetCollection<ChatMessage>(SchemaNames.Messages);
        var query = col.Include(u => u.Sender)
            .Include(u => u.Group)
            .Find(x => x.Group.Id == gid)
            .OrderBy(x => x.SentAt)
            .Skip(skip)
            .Take(limit)
            .ToArray();
        return query.Select(x => x.ToResponse()).ToArray();
    }

    public ChatMessageResponse[] GetNewerMessages(
        string groupId,
        string lastMessageId
    )
    {
        var gid = new ObjectId(groupId);
        var col = context.GetCollection<ChatMessage>(SchemaNames.Messages);

        var lastId = new ObjectId(lastMessageId);
        var lastMsg =
            col.FindById(lastId)
            ?? throw new KeyNotFoundException("Last message not found");

        if (lastMsg.Group == null || lastMsg.Group.Id != gid)
            throw new ArgumentException(
                "The provided lastMessageId does not belong to the specified group"
            );

        // find messages in the same group with SentAt greater than the last message SentAt
        // or with the same SentAt but higher id to handle identical timestamps
        var msgs = col.Include(u => u.Sender)
            .Include(u => u.Group)
            .Find(x => x.Group.Id == gid)
            .Where(x =>
                x.SentAt > lastMsg.SentAt
                || (x.SentAt == lastMsg.SentAt && x.Id > lastId)
            )
            .OrderBy(x => x.SentAt)
            .ToArray();

        return msgs.Select(x => x.ToResponse()).ToArray();
    }
}
