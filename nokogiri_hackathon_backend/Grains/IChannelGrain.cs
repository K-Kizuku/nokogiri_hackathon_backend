using System;
using Orleans.Runtime;
using nokogiri_hackathon_backend.Models;

namespace nokogiri_hackathon_backend.Grains;

public interface IChannelGrain : IGrainWithStringKey
{
    Task<StreamId> Join(string name);
    Task<StreamId> Leave(string name);
    Task<bool> Message(ChatItem msg);
    Task<ChatItem[]> ReadHistory(int numberOfMessages);
    Task<string[]> GetMembers();
    Task<Guid> GetRoomGuid();
    Task<int> GetMembersCount();
}

