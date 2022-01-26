using System;
using System.Collections.Generic;
using CarRental.Domain.Events;
using CarRental.Storage;

namespace CarRental.Domain.Entities
{
    public class Vehicle
    {
        public string LicenseNumber { get; private set; }
        public int MeterReading { get; private set; }
        public VehicleType VehicleType { get; private set; }
        public bool Available { get; private set; }

        public IReadOnlyCollection<IEvent> Events => _events.AsReadOnly();
        private readonly List<IEvent> _events = new();

        private Vehicle() { }

        public static Vehicle Replay(ICollection<object> events)
        {
            var entity = new Vehicle();

            foreach (var e in events)
            {
                entity.Apply((dynamic)e);
            }

            return entity;
        }

        public void Drive(int distanceKm)
        {
            var e = new VehicleDrivenEvent(LicenseNumber, distanceKm);
            
            Apply(e);
            
            _events.Add(e);
        }

        public void CheckOut()
        {
            var e = new VehicleCheckedOutEvent(LicenseNumber, DateTimeOffset.Now);

            Apply(e);

            _events.Add(e);
        }

        public void CheckIn()
        {
            var e = new VehicleReturnedEvent(LicenseNumber, DateTimeOffset.Now);

            Apply(e);

            _events.Add(e);
        }

        private void Apply(VehicleAddedEvent e)
        {
            LicenseNumber = e.LicenseNumber;
            VehicleType = e.VehicleType;
            MeterReading = e.InitialMeterReading;
            Available = true;
        }

        private void Apply(VehicleDrivenEvent e)
        {
            MeterReading += e.DistanceKm;
        }

        private void Apply(VehicleCheckedOutEvent e)
        {
            EnsureAvailable();

            Available = false;
        }

        private void Apply(VehicleReturnedEvent e)
        {
            if (Available)
            {
                throw new Exception($"Fordon {LicenseNumber} är redan tillgänglig");
            }

            Available = true;
        }

        private void EnsureAvailable()
        {
            if (!Available)
            {
                throw new Exception($"Fordon {LicenseNumber} är inte tillgängligt just nu");
            }
        }

        private void Apply(object e)
        {
            //unhandled
        }
    }
}