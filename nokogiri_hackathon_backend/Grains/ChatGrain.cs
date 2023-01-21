using System;
using Orleans.Runtime;
using System.IO;
using nokogiri_hackathon_backend.Models;

namespace nokogiri_hackathon_backend.Grains
{
    public class ChatGrain : Grain, IChatGrain
    {
        private readonly ILogger<ChatGrain> _logger;
        //private readonly IPersistentState<State> _state;

        private string GrainType => nameof(ChatGrain);
        private Guid GrainKey => this.GetPrimaryKey();

        public ChatGrain(
            ILogger<ChatGrain> logger)
            //[PersistentState("State")] IPersistentState<State> state)
        {
            _logger = logger;
            //_state = state;
        }

        public Task<ChatItem?> GetAsync() => Task.FromResult(_state.State.Item);

        public async Task SetAsync(ChatItem item)
        {
            // Ensure the key is consistent
            if (item.Key != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // Save the item
            _state.State.Item = item;
            await _state.WriteStateAsync();

            // Register the item with its owner list
            await GrainFactory.GetGrain<IChatManager>(item.OwnerKey)
                .RegisterAsync(item.Key);

            // For sample debugging
            _logger.LogInformation(
                "{@GrainType} {@GrainKey} now contains {@Todo}",
                GrainType, GrainKey, item);

            // Notify listeners - best effort only
            var streamId = StreamId.Create(nameof(IChatGrain), item.OwnerKey);
            this.GetStreamProvider("MemoryStreams").GetStream<TodoNotification>(streamId)
                .OnNextAsync(new TodoNotification(item.Key, item))
                .Ignore();
        }

        public async Task ClearAsync()
        {
            // Fast path for already cleared state
            if (_state.State.Item is null) return;

            // Hold on to the keys
            var itemKey = _state.State.Item.Key;
            var ownerKey = _state.State.Item.OwnerKey;

            // Unregister from the registry
            await GrainFactory.GetGrain<IChatManager>(ownerKey)
                .UnregisterAsync(itemKey);

            // Clear the state
            await _state.ClearStateAsync();

            // For sample debugging
            _logger.LogInformation(
                "{@GrainType} {@GrainKey} is now cleared",
                GrainType, GrainKey);

            // Notify listeners - best effort only
            var streamId = StreamId.Create(nameof(IChatGrain), ownerKey);
            this.GetStreamProvider("MemoryStreams").GetStream<TodoNotification>(streamId)
                .OnNextAsync(new TodoNotification(itemKey, null))
                .Ignore();

            // No need to stay alive anymore
            DeactivateOnIdle();
        }

        [GenerateSerializer]
        public class State
        {
            [Id(0)]
            public ChatItem? Item { get; set; }
        }
    }

}

