using System;
using CarRental.Storage;

namespace CarRental.Domain.Events
{
    public class BookingCreatedEvent : IEvent
    {
        public string BookingNumber { get; }
        public string SocialSecurityNumber { get; }
        public DateTimeOffset Timestamp { get; }

        public BookingCreatedEvent(string bookingNumber, string socialSecurityNumber, DateTimeOffset timestamp)
        {
            BookingNumber = bookingNumber;
            SocialSecurityNumber = socialSecurityNumber;
            Timestamp = timestamp;
        }
    }
}