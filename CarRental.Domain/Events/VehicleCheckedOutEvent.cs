using System;
using CarRental.Storage;

namespace CarRental.Domain.Events
{
    public class VehicleCheckedOutEvent : IEvent
    {
        public string LicenseNumber { get; }
        public DateTimeOffset Timestamp { get; }

        public VehicleCheckedOutEvent(string licenseNumber, DateTimeOffset timestamp)
        {
            LicenseNumber = licenseNumber;
            Timestamp = timestamp;
        }
    }
}