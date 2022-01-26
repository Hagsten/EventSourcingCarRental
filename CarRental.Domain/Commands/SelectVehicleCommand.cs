namespace CarRental.Domain.Commands
{
    public class SelectVehicleCommand : ICommand
    {
        public string BookingNumber { get; }
        public string LicenseNumber { get; }

        public SelectVehicleCommand(string bookingNumber, string licenseNumber)
        {
            BookingNumber = bookingNumber;
            LicenseNumber = licenseNumber;
        }
    }
}