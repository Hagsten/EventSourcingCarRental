using CarRental.Storage;

namespace CarRental.Domain.Events
{
    public class VehicleAddedEvent : IEvent
    {
        public string LicenseNumber { get; }
        public VehicleType VehicleType { get; }
        public int InitialMeterReading { get; }

        public VehicleAddedEvent(string licenseNumber, VehicleType vehicleType, int initialMeterReading)
        {
            LicenseNumber = licenseNumber;
            VehicleType = vehicleType;
            InitialMeterReading = initialMeterReading;
        }
    }
}
