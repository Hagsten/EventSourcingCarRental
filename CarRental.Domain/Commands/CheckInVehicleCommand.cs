namespace CarRental.Domain.Commands
{
    public class CheckInVehicleCommand : ICommand
    {
        public string LicenseNumber { get; }

        public CheckInVehicleCommand(string licenseNumber)
        {
            LicenseNumber = licenseNumber;
        }
    }
}