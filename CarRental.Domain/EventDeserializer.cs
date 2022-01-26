using System;
using System.Text;
using CarRental.Domain.Events;
using Newtonsoft.Json;

namespace CarRental.Domain
{
    public static class EventDeserializer
    {
        public static object Deserialize(byte[] e, string eventType)
        {
            var value = Encoding.UTF8.GetString(e);

            return eventType switch
            {
                nameof(VehicleAddedEvent) => Deserialize<VehicleAddedEvent>(value),
                nameof(BookingCreatedEvent) => Deserialize<BookingCreatedEvent>(value),
                nameof(VehicleAddedToBookingEvent) => Deserialize<VehicleAddedToBookingEvent>(value),
                nameof(BookingStartedEvent) => Deserialize<BookingStartedEvent>(value),
                nameof(BookingCompletedEvent) => Deserialize<BookingCompletedEvent>(value),
                nameof(VehicleReturnedEvent) => Deserialize<VehicleReturnedEvent>(value),
                nameof(BookingPayedEvent) => Deserialize<BookingPayedEvent>(value),
                nameof(VehicleDrivenEvent) => Deserialize<VehicleDrivenEvent>(value),
                nameof(VehicleCheckedOutEvent) => Deserialize<VehicleCheckedOutEvent>(value),
                nameof(BookingAbandonedEvent) => Deserialize<BookingAbandonedEvent>(value),

                _ => throw new NotImplementedException()
            };
        }

        private static TEvent Deserialize<TEvent>(string value)
        {
            return JsonConvert.DeserializeObject<TEvent>(value);
        }
    }
}
