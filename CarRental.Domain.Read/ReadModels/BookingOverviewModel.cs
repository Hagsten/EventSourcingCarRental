using System;
using System.Collections.Generic;
using CarRental.Domain.Events;

namespace CarRental.Domain.Read.ReadModels
{
    public class BookingOverviewModel
    {
        private DateTimeOffset _startedAt;
        private DateTimeOffset _endedAt;
        private int _duration;
        
        public string BookingNumber { get; private set; }
        public string StartedAt => _startedAt.ToString("yyyy-MM-dd HH:mm");
        public string VehicleLicenseNumber { get; private set; }
        public string RentedBy { get; private set; }

        public int DurationInDays => _endedAt != DateTimeOffset.MinValue ? _duration : (DateTimeOffset.Now - _startedAt).Days;
        public string EndedAt => _endedAt != DateTimeOffset.MinValue ? _endedAt.ToString("yyyy-MM-dd HH:mm") : string.Empty;
        public int DistanceKm { get; private set; }
        public int IngoingMeterReading { get; private set; }
        public bool HasEnded => _endedAt != DateTimeOffset.MinValue || IsAbandoned;
        public bool IsAbandoned { get; private set; }
        
        private BookingOverviewModel()
        { }

        public static BookingOverviewModel Create() => new();

        public static BookingOverviewModel Replay(ICollection<object> events)
        {
            var model = new BookingOverviewModel();

            foreach (var e in events)
            {
                model.Apply((dynamic)e);
            }

            return model;
        }
        
        public BookingOverviewModel UpdateWith(object evt)
        {
            Apply((dynamic)evt);

            return this;
        }

        private void Apply(BookingCreatedEvent e)
        {
            BookingNumber = e.BookingNumber;
            RentedBy = e.SocialSecurityNumber;
        }

        private void Apply(VehicleAddedToBookingEvent e)
        {
            VehicleLicenseNumber = e.LicenseNumber;
            IngoingMeterReading = e.MeterReading;
        }

        private void Apply(BookingStartedEvent e)
        {
            _startedAt = e.Timestamp;
        }

        private void Apply(BookingCompletedEvent e)
        {
            _duration = (e.Timestamp - _startedAt).Days;
            _endedAt = e.Timestamp;
            DistanceKm = e.MeterReading - IngoingMeterReading;
        }
        
        private void Apply(BookingAbandonedEvent e)
        {
            IsAbandoned = true;
        }

        private void Apply(object e)
        {
            //unhandled
        }
    }
}
