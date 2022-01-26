using System.Linq;
using System.Threading.Tasks;
using CarRental.Storage;

namespace CarRental.Domain.Entities.Projections
{
    public class BookingProjection
    {
        private readonly Store _store;

        public BookingProjection()
        {
            _store = new Store();
        }

        public async Task<Booking> Project(string bookingNumber)
        {
            var eventStream = await _store.ReadFromStream($"booking-{bookingNumber}");

            if (eventStream.Count == 0)
            {
                return null;
            }

            var events = eventStream.Select(x => EventDeserializer.Deserialize(x.Event.Data.Span.ToArray(), x.Event.EventType)).ToArray();

            return Booking.Replay(events);
        }
    }
}
