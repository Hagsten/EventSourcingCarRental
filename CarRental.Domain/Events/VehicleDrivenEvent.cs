using CarRental.Storage;

namespace CarRental.Domain.Events
{
    public class VehicleDrivenEvent : IEvent
    {
        public string LicenseNumber { get; }
        public int DistanceKm { get; }

        public VehicleDrivenEvent(string licenseNumber, int distanceKm)
        {
            LicenseNumber = licenseNumber;
            DistanceKm = distanceKm;
        }
    }
}