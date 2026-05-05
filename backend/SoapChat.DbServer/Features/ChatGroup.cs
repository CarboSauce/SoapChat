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
    ChatGroupResponse CreateGroup(string name);

    [OperationContract]
    ChatGroupResponse GetGroup(string id);

    [OperationContract]
    ChatGroupResponse[] SearchGroupsByMember(string userId);

    [OperationContract]
    ChatGroupResponse AddMember(string groupId, string userId);

    [OperationContract]
    ChatGroupResponse RemoveMember(string groupId, string userId);
}

public class ChatGroupService(LiteDbContext dbContext) : IChatGroupService
{
    private readonly LiteDatabase context = dbContext.Context;

    public ChatGroupResponse CreateGroup(string name)
    {
        var groups = context.GetCollection<ChatGroup>(SchemaNames.Groups);

        groups.EnsureIndex(x => x.Name);
        groups.EnsureIndex(x => x.Members);

        var group = new ChatGroup { Name = name };

        groups.Insert(group);
        return group.ToResponse();
    }

    public ChatGroupResponse GetGroup(string id)
    {
        var gid = new ObjectId(id);
        var group = context
            .GetCollection<ChatGroup>(SchemaNames.Groups)
            .Include(u => u.Members)
            .FindById(gid);
        return group?.ToResponse()
            ?? throw new KeyNotFoundException("Group not found");
    }

    public ChatGroupResponse[] SearchGroupsByMember(string userId)
    {
        var uid = new ObjectId(userId);
        var col = context.GetCollection<ChatGroup>(SchemaNames.Groups);
        var groups = col.Include(u => u.Members)
            .FindAll()
            .Where(x => x.Members.Any(m => m.Id == uid))
            .ToArray();
        return groups.Select(x => x.ToResponse()).ToArray();
    }

    public ChatGroupResponse AddMember(string groupId, string userId)
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
            return group.ToResponse();
        }

        group.Members.Add(user);
        groups.Update(group);

        return group.ToResponse();
    }

    public ChatGroupResponse RemoveMember(string groupId, string userId)
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

        return group.ToResponse();
    }
}
