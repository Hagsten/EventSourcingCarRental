namespace CarRental.Domain.Commands
{
    public class CheckOutVehicleCommand : ICommand
    {
        public string LicenseNumber { get; }

        public CheckOutVehicleCommand(string licenseNumber)
        {
            LicenseNumber = licenseNumber;
        }
    }
}