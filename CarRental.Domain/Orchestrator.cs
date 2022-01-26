using System.Threading.Tasks;
using CarRental.Domain.CommandHandlers;
using CarRental.Domain.Commands;
using CarRental.Domain.Entities.Projections;
using CarRental.Domain.Events;
using CarRental.Storage;

namespace CarRental.Domain
{
    public static class Orchestrator
    {
        private static readonly BookingProjection BookingProjection;

        static Orchestrator()
        {
            BookingProjection = new BookingProjection();
        }
        
        public static async Task Initialize()
        {
            await BeginListening();
        }

        private static async Task BeginListening()
        {
            await Subscription.SubscribeToLiveEvent(nameof(VehicleAddedToBookingEvent), async resolvedEvent =>
            {
                var evt = (VehicleAddedToBookingEvent)EventDeserializer.Deserialize(resolvedEvent.Event.Data.Span.ToArray(), resolvedEvent.Event.EventType);

                await CommandDispatcher.Dispatch(new CheckOutVehicleCommand(evt.LicenseNumber));
            });

            await Subscription.SubscribeToLiveEvent(nameof(BookingCompletedEvent), async resolvedEvent =>
            {
                var evt = (BookingCompletedEvent)EventDeserializer.Deserialize(resolvedEvent.Event.Data.Span.ToArray(), resolvedEvent.Event.EventType);

                var booking = await BookingProjection.Project(evt.BookingNumber);
                
                await CommandDispatcher.Dispatch(new CheckInVehicleCommand(booking.VehicleLicenseNumber));
            });
        }
    }
}