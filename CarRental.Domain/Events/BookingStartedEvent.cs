using System;
using CarRental.Storage;

namespace CarRental.Domain.Events
{
    public class BookingStartedEvent : IEvent
    {
        public string BookingNumber { get; }
        public DateTimeOffset Timestamp { get; }

        public BookingStartedEvent(string bookingNumber, DateTimeOffset timestamp)
        {
            BookingNumber = bookingNumber;
            Timestamp = timestamp;
        }
    }
}