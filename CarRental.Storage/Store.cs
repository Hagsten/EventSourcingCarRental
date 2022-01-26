using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.Client;
using Newtonsoft.Json;

namespace CarRental.Storage
{
    public class Store
    {
        private readonly EventStoreClient _client;

        public Store()
        {
            _client = new EventStoreClient(EventStoreClientSettings.Create("esdb://localhost:2113?tls=false"));
        }

        public async Task AppendEvent(string stream, IEvent[] events)
        {
            var eventData = events.Select(e => new EventData(
                    Uuid.NewUuid(),
                    e.GetType().Name,
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e))))
                .ToArray();

            await _client.AppendToStreamAsync(
                stream,
                StreamState.Any,
                eventData, o =>
                {
                    o.TimeoutAfter = TimeSpan.FromMinutes(5);
                });

        }

        public async Task<IReadOnlyCollection<ResolvedEvent>> ReadFromStream(string stream)
        {
            var state = _client.ReadStreamAsync(Direction.Forwards, stream, StreamPosition.Start, resolveLinkTos: true);

            var readState = await state.ReadState;

            if (readState == ReadState.StreamNotFound)
            {
                return new List<ResolvedEvent>();
            }

            return await state.ToListAsync();
        }
    }
}
