namespace CarRental.Domain.Commands
{
    public class AbandonBookingCommand : ICommand
    {
        public string BookingNumber { get; }

        public AbandonBookingCommand(string bookingNumber)
        {
            BookingNumber = bookingNumber;
        }
    }
}