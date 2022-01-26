using System;
using CarRental.Storage;

namespace CarRental.Domain.Events
{
    public class BookingCompletedEvent : IEvent
    {
        public string BookingNumber { get; }
        public int MeterReading { get; }
        public decimal Amount { get; }
        public DateTimeOffset Timestamp { get; }

        public BookingCompletedEvent(string bookingNumber, int meterReading, decimal amount, DateTimeOffset timestamp)
        {
            BookingNumber = bookingNumber;
            MeterReading = meterReading;
            Amount = amount;
            Timestamp = timestamp;
        }
    }
}