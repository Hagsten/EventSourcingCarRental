namespace CarRental.Domain.Commands
{
    public class PayBookingCommand : ICommand
    {
        public string BookingNumber { get; }
        public decimal Amount { get; }

        public PayBookingCommand(string bookingNumber, decimal amount)
        {
            BookingNumber = bookingNumber;
            Amount = amount;
        }
    }
}