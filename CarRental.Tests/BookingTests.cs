using System;
using System.Collections.Generic;
using System.Linq;
using CarRental.Domain;
using CarRental.Domain.Entities;
using CarRental.Domain.Events;
using NSubstitute;
using Xunit;

namespace CarRental.Tests
{
    public class BookingTests
    {
        private readonly Booking _sut;
        private readonly IPriceCalculator _priceCalculator;

        public BookingTests()
        {
            _sut = Booking.CreateNew("19121212-1212");
            
            _priceCalculator = Substitute.For<IPriceCalculator>();
            _priceCalculator.Calculate(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>()).Returns(500M);
        }
        
        [Fact]
        public void WhenBookingIsCreated_SocialSecurityNumberIsAdded()
        {
            Assert.Equal("19121212-1212", _sut.SocialSecurityNumber);
        }

        [Fact]
        public void WhenBookingIsCreated_BookingNumberIsGenerated()
        {
            Assert.Equal(12, _sut.BookingNumber.Length);
            Assert.StartsWith("BN", _sut.BookingNumber);
        }

        [Fact]
        public void WhenBookingIsCreated_BookingCreatedEventIsEmitted()
        {
            Assert.Equal(1, _sut.Events.Count);
            Assert.Contains(_sut.Events, e => e is BookingCreatedEvent);
        }

        [Fact]
        public void WhenBookingIsUpdateWithVehicle_VehicleAddedToBookingEventIsEmitted()
        {
            _sut.ConnectVehicle("ABC123", 123);
            
            var e = (VehicleAddedToBookingEvent)_sut.Events.Single(x => x is VehicleAddedToBookingEvent);
            
            Assert.Equal("ABC123", e.LicenseNumber);
            Assert.Equal(123, e.MeterReading);
        }

        [Fact]
        public void WhenBookingIsStarted_BookingStartedEventIsEmitted()
        {
            var startedAt = DateTimeOffset.Now;
            
            _sut.Start(startedAt);

            var e = (BookingStartedEvent)_sut.Events.Single(x => x is BookingStartedEvent);

            Assert.Equal(startedAt, e.Timestamp);
        }

        [Fact]
        public void WhenVehicleIsReturned_BookingCompletedEventIsEmitted()
        {
            _sut.ConnectVehicle("ABC123", 100);
            _sut.ReturnVehicle(300, _priceCalculator);

            var e = (BookingCompletedEvent)_sut.Events.Single(x => x is BookingCompletedEvent);

            Assert.Equal(500, e.Amount);
            Assert.Equal(300, e.MeterReading);
        }

        [Fact]
        public void WhenBookingIsReplayed_TheAssumedStateShouldBePresent()
        {
            var bookingNumber = "BN123";
            var licenseNumber = "ABC123";
            var socialSecurityNumber = "19121212-1212";
            
            var booking = Booking.Replay(new List<object>
            {
                new BookingCreatedEvent(bookingNumber, socialSecurityNumber, DateTimeOffset.Now.AddMinutes(-10)),
                new VehicleAddedToBookingEvent(bookingNumber, licenseNumber, 100),
                new BookingStartedEvent(bookingNumber, DateTimeOffset.Now.AddMinutes(-5)),
                new BookingCompletedEvent(bookingNumber, 250, 300, DateTimeOffset.Now.AddMinutes(-1)),
                new BookingPayedEvent(bookingNumber, 300, DateTimeOffset.Now)
            });

            Assert.Equal(bookingNumber, booking.BookingNumber);
            Assert.Equal(licenseNumber, booking.VehicleLicenseNumber);
            Assert.Equal(100, booking.IngoingMeterReading);
            Assert.Equal(250, booking.OutgoingMeterReading);
            Assert.Equal(300, booking.Amount);
            Assert.Equal(150, booking.Distance);
            Assert.Equal(socialSecurityNumber, booking.SocialSecurityNumber);
            Assert.True(booking.Complete);
            Assert.True(booking.Payed);
        }
    }
}
