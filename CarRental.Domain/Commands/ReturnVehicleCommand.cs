namespace CarRental.Domain.Commands
{
    public class ReturnVehicleCommand : ICommand
    {
        public string BookingNumber { get; }

        public ReturnVehicleCommand(string bookingNumber)
        {
            BookingNumber = bookingNumber;
        }
    }
}