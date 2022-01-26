using System;
using System.Threading.Tasks;
using EventStore.Client;

namespace CarRental.Storage
{
    public class Subscription
    {
        private static readonly EventStoreClient _client;

        static Subscription()
        {
            _client = new EventStoreClient(EventStoreClientSettings.Create("esdb://localhost:2113?tls=false&keepAliveInterval=-1&keepAliveTimeout=-1"));
        }

        public static async Task SubscribeWithCatchUp(string stream, Func<ResolvedEvent, Task> handler)
        {
            await _client.SubscribeToStreamAsync(stream,
                async (subscription, evnt, cancellationToken) =>
                {
                    try
                    {
                        await handler(evnt);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in subscr. " + e.Message);
                    }
                }, true);
        }

        public static async Task SubscribeToLiveEvent(string eventType, Func<ResolvedEvent, Task> handler)
        {
            await _client.SubscribeToAllAsync(Position.End, async (subscription, evnt, cancellationToken) =>
            {
                await handler(evnt);
            }, filterOptions: new SubscriptionFilterOptions(EventTypeFilter.RegularExpression(eventType)));
        }
    }
}
