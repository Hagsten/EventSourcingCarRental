using System.Collections.Generic;
using CarRental.Domain.Events;

namespace CarRental.Domain.Read.ReadModels
{
    public class VehicleOverviewModel
    {
        public int MeterReading { get; private set; }
        public VehicleType VehicleType { get; private set; }
        public string LicenseNumber { get; private set; }
        public bool Available { get; private set; }

        private VehicleOverviewModel()
        { }

        public static VehicleOverviewModel Create() => new();

        public static VehicleOverviewModel Replay(ICollection<object> events)
        {
            var model = new VehicleOverviewModel();

            foreach (var e in events)
            {
                model.Apply((dynamic)e);
            }

            return model;
        }

        public VehicleOverviewModel UpdateWith(object evt)
        {
            Apply((dynamic)evt);

            return this;
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
            Available = false;
        }

        private void Apply(VehicleReturnedEvent e)
        {
            Available = true;
        }

        private void Apply(object e)
        {
            //unhandled
        }
    }
}