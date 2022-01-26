using CarRental.Storage;

namespace CarRental.Domain.Events
{
    public class VehicleAddedToBookingEvent : IEvent
    {
        public string BookingNumber { get; }
        public string LicenseNumber { get; }
        public int MeterReading { get; }

        public VehicleAddedToBookingEvent(string bookingNumber, string licenseNumber, int meterReading)
        {
            BookingNumber = bookingNumber;
            LicenseNumber = licenseNumber;
            MeterReading = meterReading;
        }
    }
}