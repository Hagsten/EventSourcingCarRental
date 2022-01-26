using System;
using CarRental.Storage;

namespace CarRental.Domain.Events
{
    public class BookingPayedEvent : IEvent
    {
        public string BookingNumber { get; }
        public decimal Amount { get; }
        public DateTimeOffset Timestamp { get; }

        public BookingPayedEvent(string bookingNumber, decimal amount, DateTimeOffset timestamp)
        {
            BookingNumber = bookingNumber;
            Amount = amount;
            Timestamp = timestamp;
        }
    }
}