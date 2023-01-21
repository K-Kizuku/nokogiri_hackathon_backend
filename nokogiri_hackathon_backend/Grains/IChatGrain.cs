using nokogiri_hackathon_backend.Models;
namespace nokogiri_hackathon_backend.Grains;


public interface IChatGrain : IGrainWithGuidKey
{
    Task SetAsync(ChatItem item);

    Task ClearAsync();

    Task<ChatItem?> GetAsync();
}

