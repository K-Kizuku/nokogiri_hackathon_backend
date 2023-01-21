using System;
using System.Collections.Immutable;

namespace nokogiri_hackathon_backend.Grains
{
	public interface IChatManager : IGrainWithGuidKey
    {
        Task RegisterAsync(Guid itemKey);
        Task UnregisterAsync(Guid itemKey);

        Task<ImmutableArray<Guid>> GetAllAsync();
    }
}

