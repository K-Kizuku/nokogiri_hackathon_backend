using System;
using Orleans.Runtime;
using Orleans.Streams;
using nokogiri_hackathon_backend.Models;

namespace nokogiri_hackathon_backend.Grains;

public class ChannelGrain : Grain, IChannelGrain
{
    private readonly List<ChatItem> _messages = new(256);
    private readonly List<string> _onlineMembers = new(256);
    public readonly Guid room = Guid.NewGuid();

    private IAsyncStream<ChatItem> _stream = null!;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider("chat");

        var streamId = StreamId.Create(
            "ChatRoom", this.GetPrimaryKeyString());

        _stream = streamProvider.GetStream<ChatItem>(
            streamId);

        return base.OnActivateAsync(cancellationToken);
    }

    public async Task<StreamId> Join(string name)
    {
        _onlineMembers.Add(name);

        await _stream.OnNextAsync(
            new ChatItem("system", $"{name} さんが入室しました."));

        return _stream.StreamId;
    }

    public async Task<StreamId> Leave(string name)
    {
        _onlineMembers.Remove(name);

        // 退出時の処理．メッセージを送るか要検討
        //await _stream.OnNextAsync(
        //    new ChatItem(
        //        "System",
        //        $"{name} leaves the chat..."));

        return _stream.StreamId;
    }

    public async Task<bool> Message(ChatItem msg)
    {
        _messages.Add(msg);

        await _stream.OnNextAsync(msg);

        return true;
    }

    public Task<string[]> GetMembers() => Task.FromResult(_onlineMembers.ToArray());

    public Task<ChatItem[]> ReadHistory(int numberOfMessages)
    {
        var response = _messages
            .OrderByDescending(x => x.Created)
            .Take(numberOfMessages)
            .OrderBy(x => x.Created)
            .ToArray();

        return Task.FromResult(response);
    }

    public Task<Guid> GetRoomGuid()
    {
        return Task.FromResult(this.room);
    }

    //public void InitRoomGuid()
    //{
    //    this.room = Guid.NewGuid();
    //}

    public Task<int> GetMembersCount()
    {
        return Task.FromResult(_onlineMembers.Count);
    }

}

