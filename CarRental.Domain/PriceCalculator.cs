using CarRental.Domain.Entities.Projections;
using CarRental.Storage;

namespace CarRental.Domain
{
    public interface IPriceCalculator
    {
        decimal Calculate(string licenseNumber, int durationInDays, int distanceKm);
    }
    
    public class PriceCalculator : IPriceCalculator
    {
        private readonly IVehicleProjection _vehicleProjection;
        
        private readonly int _baselineRentPerDay = RentalPriceStore.Prices.BaseDayPrice;
        private readonly int _baselineRentPerKilometer = RentalPriceStore.Prices.BaseKmPrice;

        public PriceCalculator(IVehicleProjection projection)
        {
            _vehicleProjection = projection;
        }

        public decimal Calculate(string licenseNumber, int durationInDays, int distanceKm)
        {
            var vehicle = _vehicleProjection.Project(licenseNumber).GetAwaiter().GetResult();
            
            return vehicle switch
            {
                { VehicleType: VehicleType.Sedan } => _baselineRentPerDay * durationInDays,
                { VehicleType: VehicleType.StationWagon } => (_baselineRentPerDay * durationInDays * 1.3M) + (_baselineRentPerKilometer * distanceKm),
                { VehicleType: VehicleType.Truck } => (_baselineRentPerDay * durationInDays * 1.5M) + (_baselineRentPerKilometer * distanceKm * 1.5M),
                _ => 0M
            };
        }
    }
}
