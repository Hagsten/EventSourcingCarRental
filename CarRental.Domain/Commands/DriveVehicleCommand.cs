namespace CarRental.Domain.Commands
{
    public class DriveVehicleCommand : ICommand
    {
        public string LicenseNumber { get; }
        public int DistanceKm { get; }

        public DriveVehicleCommand(string licenseNumber, int distanceKm)
        {
            LicenseNumber = licenseNumber;
            DistanceKm = distanceKm;
        }
    }
}