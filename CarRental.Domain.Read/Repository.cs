using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Domain.Read.ReadModels;
using CarRental.Storage;

namespace CarRental.Domain.Read
{
    public static class Repository
    {
        private static readonly Dictionary<string, VehicleOverviewModel> _vehicles = new();
        private static readonly Dictionary<string, BookingOverviewModel> _bookings = new();

        public static IReadOnlyCollection<VehicleOverviewModel> Vehicles => _vehicles.Select(x => x.Value).ToList().AsReadOnly();
        public static IReadOnlyCollection<BookingOverviewModel> BookingOverview => _bookings.Select(x => x.Value).ToList().AsReadOnly();

        public static async Task Initialize()
        {
            await BeginPopulateVehicles();
            await BeginPopulateBookings();
        }

        private static async Task BeginPopulateVehicles()
        {
            await Subscription.SubscribeWithCatchUp("$ce-vehicle", resolvedEvent =>
            {
                var evt = EventDeserializer.Deserialize(resolvedEvent.Event.Data.Span.ToArray(), resolvedEvent.Event.EventType);
                var licenseNumber = resolvedEvent.Event.EventStreamId.Split('-')[1];

                if (!_vehicles.ContainsKey(licenseNumber))
                {
                    _vehicles[licenseNumber] = VehicleOverviewModel.Create().UpdateWith(evt);
                }
                else
                {
                    _vehicles[licenseNumber].UpdateWith(evt);
                }

                return Task.CompletedTask;
            });
        }

        private static async Task BeginPopulateBookings()
        {
            await Subscription.SubscribeWithCatchUp("$ce-booking", resolvedEvent =>
            {
                var evt = EventDeserializer.Deserialize(resolvedEvent.Event.Data.Span.ToArray(), resolvedEvent.Event.EventType);
                var bookingNumber = resolvedEvent.Event.EventStreamId.Split('-')[1];

                if (!_bookings.ContainsKey(bookingNumber))
                {
                    _bookings[bookingNumber] = Domain.Read.ReadModels.BookingOverviewModel.Create().UpdateWith(evt);
                }
                else
                {
                    _bookings[bookingNumber].UpdateWith(evt);
                }

                return Task.CompletedTask;
            });
        }
    }
}
