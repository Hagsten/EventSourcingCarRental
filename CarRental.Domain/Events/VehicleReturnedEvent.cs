using System;
using CarRental.Storage;

namespace CarRental.Domain.Events
{
    public class VehicleReturnedEvent : IEvent
    {
        public string LicenseNumber { get; }
        public DateTimeOffset Timestamp { get; }

        public VehicleReturnedEvent(string licenseNumber, DateTimeOffset timestamp)
        {
            LicenseNumber = licenseNumber;
            Timestamp = timestamp;
        }
    }
}