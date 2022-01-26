using System;
using System.Collections.Generic;
using CarRental.Domain.Events;
using CarRental.Storage;

namespace CarRental.Domain.Entities
{
    public class Booking
    {
        public string BookingNumber { get; private set; }
        public string VehicleLicenseNumber { get; private set; }
        public string SocialSecurityNumber { get; private set; }
        public DateTimeOffset StartDate { get; private set; }
        public int IngoingMeterReading { get; private set; }
        public int OutgoingMeterReading { get; private set; }
        public int Distance => OutgoingMeterReading - IngoingMeterReading;
        public decimal Amount { get; private set; }
        public bool Payed { get; private set; }
        public bool Complete => Amount > 0 && OutgoingMeterReading > 0;
        public bool Abandoned { get; private set; }

        public IReadOnlyCollection<IEvent> Events => _events.AsReadOnly();
        private readonly List<IEvent> _events = new();

        private Booking() { }
        
        private Booking(string socialSecurityNumber)
        {
            EnsureThatBookingIsNotYetCreated();
            
            var e = new BookingCreatedEvent(GenerateBookingNumber(), socialSecurityNumber, DateTimeOffset.Now);
            
            Apply(e);

            _events.Add(e);
        }

        public static Booking CreateNew(string socialSecurityNumber) => new(socialSecurityNumber);

        public static Booking Replay(ICollection<object> events)
        {
            var entity = new Booking();

            foreach (var e in events)
            {
                entity.Apply((dynamic)e);
            }

            return entity;
        }

        public void ConnectVehicle(string licenseNumber, int meterReading)
        {
            EnsureThatBookingIsCreated();
            EnsureThatBookingIsNotAbandoned();

            var e = new VehicleAddedToBookingEvent(BookingNumber, licenseNumber, meterReading);
            
            Apply(e);

            _events.Add(e);
        }

        public void ReturnVehicle(int meterReading, IPriceCalculator calculator)
        {
            EnsureThatBookingIsCreated();
            EnsureVehicleIsConnected();
            EnsureThatBookingIsNotYetComplete();
            EnsureThatBookingIsNotAbandoned();

            var now = DateTimeOffset.Now;
            var duration = (now - StartDate).Days;
            
            var e = new BookingCompletedEvent(BookingNumber, meterReading, calculator.Calculate(VehicleLicenseNumber, duration, (meterReading - IngoingMeterReading)), now);

            Apply(e);

            _events.Add(e);
        }

        public void Pay(decimal amount)
        {
            EnsureThatBookingIsCreated();
            EnsureThatBookingIsComplete();
            EnsureThatBookingIsNotAbandoned();

            var e = new BookingPayedEvent(BookingNumber, amount, DateTimeOffset.Now);

            Apply(e);

            _events.Add(e);
        }

        public void Abandon()
        {
            EnsureThatBookingIsCreated();
            EnsureThatBookingIsNotStarted();
            EnsureThatBookingIsNotAbandoned();
            EnsureVehicleIsNotConnected();

            var e = new BookingAbandonedEvent(BookingNumber, DateTimeOffset.Now);

            Apply(e);

            _events.Add(e);
        }

        public void Start(DateTimeOffset startedAt)
        {
            EnsureThatBookingIsCreated();
            EnsureThatBookingIsNotAbandoned();

            var e = new BookingStartedEvent(BookingNumber, startedAt);
            
            Apply(e);

            _events.Add(e);
        }

        private void Apply(BookingCreatedEvent e)
        {
            BookingNumber = e.BookingNumber;
            SocialSecurityNumber = e.SocialSecurityNumber;
        }

        private void Apply(VehicleAddedToBookingEvent e)
        {
            VehicleLicenseNumber = e.LicenseNumber;
            IngoingMeterReading = e.MeterReading;
        }

        private void Apply(BookingStartedEvent e)
        {
            StartDate = e.Timestamp;
        }

        private void Apply(BookingCompletedEvent e)
        {
            OutgoingMeterReading = e.MeterReading;
            Amount = e.Amount;
        }

        private void Apply(BookingAbandonedEvent e)
        {
            Abandoned = true;
        }

        private void Apply(BookingPayedEvent e)
        {
            Payed = true;
        }

        private void Apply(object e)
        {
            //unhandled
        }

        private static string GenerateBookingNumber()
        {
            const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[10];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = allowedChars[random.Next(allowedChars.Length)];
            }

            return $"BN{ new string(stringChars)}";
        }

        private void EnsureThatBookingIsNotStarted()
        {
            if (StartDate > DateTimeOffset.MinValue)
            {
                throw new Exception("Kan inte överge en bokning som är på börjad");
            }
        }

        private void EnsureThatBookingIsComplete()
        {
            if (!Complete)
            {
                throw new Exception("Bokningen är inte slutförd. Vänligen slutför bokning innan betalning kan ske");
            }
        }

        private void EnsureThatBookingIsNotYetComplete()
        {
            if (Complete)
            {
                throw new Exception("Bokningen är redan slutförd och bilen tillbakalämnad");
            }
        }

        private void EnsureThatBookingIsNotYetCreated()
        {
            if (!string.IsNullOrWhiteSpace(BookingNumber))
            {
                throw new Exception("Bokning finns redan, kan inte skapas på nytt");
            }
        }

        private void EnsureThatBookingIsCreated()
        {
            if (string.IsNullOrWhiteSpace(BookingNumber))
            {
                throw new Exception("Bokning finns redan, kan inte skapas på nytt");
            }
        }

        private void EnsureThatBookingIsNotAbandoned()
        {
            if (Abandoned)
            {
                throw new Exception("Bokningen är övergiven, går inte att utföra något på den");
            }
        }

        private void EnsureVehicleIsConnected()
        {
            if (string.IsNullOrWhiteSpace(VehicleLicenseNumber))
            {
                throw new Exception("Det finns ingen bil kopplad till bokningen");
            }
        }

        private void EnsureVehicleIsNotConnected()
        {
            if (!string.IsNullOrWhiteSpace(VehicleLicenseNumber))
            {
                throw new Exception("Det finns en bil kopplad till bokningen, åtgärden går inte att genomföra");
            }
        }
    }
}
