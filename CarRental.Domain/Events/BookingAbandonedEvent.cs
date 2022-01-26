using System;
using CarRental.Storage;

namespace CarRental.Domain.Events
{
    public class BookingAbandonedEvent : IEvent
    {
        public string BookingNumber { get; }
        public DateTimeOffset Timestamp { get; }

        public BookingAbandonedEvent(string bookingNumber, DateTimeOffset timestamp)
        {
            BookingNumber = bookingNumber;
            Timestamp = timestamp;
        }
    }
}