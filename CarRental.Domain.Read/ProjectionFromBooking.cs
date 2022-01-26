using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Storage;

namespace CarRental.Domain.Read
{
    public class ProjectionFromBooking<T> where T : class
    {
        private readonly Store _store;
        private readonly Func<ICollection<object>, T> _replayer;

        public ProjectionFromBooking(Func<ICollection<object>, T> replayer)
        {
            _store = new Store();
            _replayer = replayer;
        }

        public async Task<T> Project(string entityId)
        {
            var eventStream = await _store.ReadFromStream($"booking-{entityId}");

            var events = eventStream.Select(resolvedEvent => EventDeserializer.Deserialize(resolvedEvent.Event.Data.Span.ToArray(), resolvedEvent.Event.EventType)).ToArray();

            return _replayer(events);
        }
    }
}