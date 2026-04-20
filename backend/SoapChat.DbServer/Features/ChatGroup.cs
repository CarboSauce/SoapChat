namespace SoapChat.DbServer.Features;

using System;
using System.Collections.Generic;
using System.Linq;
using CoreWCF;
using LiteDB;

[ServiceContract]
public interface IChatGroupService
{
    [OperationContract]
    ChatGroup CreateGroup(string name);

    [OperationContract]
    ChatGroup GetGroup(string id);

    [OperationContract]
    ChatGroup[] SearchGroupsByMember(string userId);

    [OperationContract]
    ChatGroup AddMember(string groupId, string userId);

    [OperationContract]
    ChatGroup RemoveMember(string groupId, string userId);
}

public class ChatGroupService(LiteDbContext dbContext) : IChatGroupService
{
    private readonly LiteDatabase context = dbContext.Context;

    public ChatGroup CreateGroup(string name)
    {
        var groups = context.GetCollection<ChatGroup>(SchemaNames.Groups);
        var users = context.GetCollection<User>(SchemaNames.Users);

        groups.EnsureIndex(x => x.Name);
        groups.EnsureIndex(x => x.Members);

        var group = new ChatGroup { Name = name };

        groups.Insert(group);
        return group;
    }

    public ChatGroup GetGroup(string id)
    {
        var gid = new ObjectId(id);
        return context
            .GetCollection<ChatGroup>(SchemaNames.Groups)
            .FindById(gid);
    }

    public ChatGroup[] SearchGroupsByMember(string userId)
    {
        var uid = new ObjectId(userId);
        var col = context.GetCollection<ChatGroup>(SchemaNames.Groups);
        var groups = col.Find(x => x.Members.Any(m => m.Id == uid)).ToArray();
        return groups;
    }

    public ChatGroup AddMember(string groupId, string userId)
    {
        var gid = new ObjectId(groupId);
        var uid = new ObjectId(userId);
        var groups = context.GetCollection<ChatGroup>(SchemaNames.Groups);
        var users = context.GetCollection<User>(SchemaNames.Users);

        var group =
            groups.FindById(gid)
            ?? throw new KeyNotFoundException("Group not found");
        var user =
            users.FindById(uid)
            ?? throw new KeyNotFoundException("User not found");

        if (group.Members.Any(m => m.Id == uid))
        {
            return group;
        }

        group.Members.Add(user);
        groups.Update(group);

        return group;
    }

    public ChatGroup RemoveMember(string groupId, string userId)
    {
        var gid = new ObjectId(groupId);
        var uid = new ObjectId(userId);
        var groups = context.GetCollection<ChatGroup>(SchemaNames.Groups);

        var group =
            groups.FindById(gid)
            ?? throw new KeyNotFoundException("Group not found");
        var removed = group.Members.RemoveAll(m => m.Id == uid) > 0;
        if (removed)
            groups.Update(group);

        return group;
    }
}
