using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Domain;
using CarRental.Domain.Entities;
using CarRental.Domain.Entities.Projections;
using CarRental.Domain.Events;
using CarRental.Storage;
using NSubstitute;
using Xunit;

namespace CarRental.Tests
{
    public class PriceCalculatorTests
    {
        private readonly PriceCalculator _sut;
        private readonly IVehicleProjection _vehicleProjection;

        public PriceCalculatorTests()
        {
            _vehicleProjection = Substitute.For<IVehicleProjection>();

            _sut = new PriceCalculator(_vehicleProjection);
        }
        
        [Theory]
        [InlineData(5, 35)]
        [InlineData(5, 1000)]
        [InlineData(3, 1000)]
        [InlineData(0, 1000)]
        public void CalculatingSedan(int duration, int distanceKm)
        {
            var price = RunWith(VehicleType.Sedan, duration, distanceKm);

            Assert.Equal(RentalPriceStore.Prices.BaseDayPrice * duration, price);
        }
        
        [Theory]
        [InlineData(5, 35)]
        [InlineData(5, 1000)]
        [InlineData(3, 1000)]
        [InlineData(0, 1000)]
        public void CalculatingStationWagon(int duration, int distanceKm)
        {
            var price = RunWith(VehicleType.StationWagon, duration, distanceKm);

            Assert.Equal((RentalPriceStore.Prices.BaseDayPrice * duration * 1.3M) + (RentalPriceStore.Prices.BaseKmPrice * distanceKm), price);
        }

        [Theory]
        [InlineData(5, 35)]
        [InlineData(5, 1000)]
        [InlineData(3, 1000)]
        [InlineData(0, 1000)]
        public void CalculatingTruck(int duration, int distanceKm)
        {
            var price = RunWith(VehicleType.Truck, duration, distanceKm);

            Assert.Equal((RentalPriceStore.Prices.BaseDayPrice * duration * 1.5M) + (RentalPriceStore.Prices.BaseKmPrice * distanceKm * 1.5M), price);
        }

        private decimal RunWith(VehicleType vehicleType, int duration, int distanceKm)
        {
            _vehicleProjection.Project("ABC123").Returns(Task.FromResult(Vehicle.Replay(new List<object>
            {
                new VehicleAddedEvent("ABC123", vehicleType, 100)
            })));

            return _sut.Calculate("ABC123", duration, distanceKm);
        }
    }
}