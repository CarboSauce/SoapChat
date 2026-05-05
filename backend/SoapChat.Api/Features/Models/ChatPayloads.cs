namespace SoapChat.Api.Features.Models;

using ChatGroupSvc = SoapChat.Api.ChatGroupService;
using MessageSvc = SoapChat.Api.MessageService;

public class ChatGroupPayload
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public UserPayload[] Members { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class MessagePayload
{
    public string? Id { get; set; }
    public ChatGroupPayload? Group { get; set; }
    public UserPayload? Sender { get; set; }
    public string? Text { get; set; }
    public DateTime SentAt { get; set; }
}

public static class ChatPayloadMapper
{
    public static UserPayload ToPayload(this ChatGroupSvc.UserResponse user)
    {
        return new UserPayload { Id = user.Id, Name = user.Name };
    }

    public static UserPayload ToPayload(this MessageSvc.UserResponse user)
    {
        return new UserPayload { Id = user.Id, Name = user.Name };
    }

    public static ChatGroupPayload ToPayload(
        this ChatGroupSvc.ChatGroupResponse group
    )
    {
        return new ChatGroupPayload
        {
            Id = group.Id,
            Name = group.Name,
            CreatedAt = group.CreatedAt,
            Members = group.Members?.Select(x => x.ToPayload()).ToArray() ?? []
        };
    }

    public static ChatGroupPayload ToPayload(
        this MessageSvc.ChatGroupResponse group
    )
    {
        return new ChatGroupPayload
        {
            Id = group.Id,
            Name = group.Name,
            CreatedAt = group.CreatedAt,
            Members = group.Members?.Select(x => x.ToPayload()).ToArray() ?? []
        };
    }

    public static MessagePayload ToPayload(
        this MessageSvc.ChatMessageResponse message
    )
    {
        return new MessagePayload
        {
            Id = message.Id,
            Group = message.Group?.ToPayload(),
            Sender = message.Sender?.ToPayload(),
            Text = message.Text,
            SentAt = message.SentAt
        };
    }
}
