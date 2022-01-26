using System;

namespace CarRental.Domain.Commands
{
    public class StartBookingCommand : ICommand
    {
        public string BookingNumber { get; }
        public DateTimeOffset StartedAt { get; }

        public StartBookingCommand(string bookingNumber, DateTimeOffset startedAt)
        {
            BookingNumber = bookingNumber;
            StartedAt = startedAt;
        }
    }
}