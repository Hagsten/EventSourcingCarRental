using System.Threading.Tasks;
using CarRental.Domain.Commands;
using CarRental.Domain.Entities.Projections;

namespace CarRental.Domain.CommandHandlers
{
    public class ReturnVehicleCommandHandler : ICommandHandler<ReturnVehicleCommand>
    {
        private readonly VehicleProjection _vehicleProjection;
        private readonly PriceCalculator _calculator;

        public ReturnVehicleCommandHandler()
        {
            _vehicleProjection = new VehicleProjection();
            _calculator = new PriceCalculator(_vehicleProjection);
        }

        public async Task<EntityId> Handle(ReturnVehicleCommand command)
        {
            return await BookingProcess
                .StartWith(command.BookingNumber)
                .Mutate(booking =>
                {
                    var vehicle = _vehicleProjection.Project(booking.VehicleLicenseNumber).GetAwaiter().GetResult();
                    booking.ReturnVehicle(vehicle.MeterReading, _calculator);
                });
        }
    }
}