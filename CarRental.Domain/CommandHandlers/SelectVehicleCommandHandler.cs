using System;
using System.Threading.Tasks;
using CarRental.Domain.Commands;
using CarRental.Domain.Entities.Projections;

namespace CarRental.Domain.CommandHandlers
{
    public class SelectVehicleCommandHandler : ICommandHandler<SelectVehicleCommand>
    {
        private readonly VehicleProjection _vehicleProjection;

        public SelectVehicleCommandHandler()
        {
            _vehicleProjection = new VehicleProjection();
        }

        public async Task<EntityId> Handle(SelectVehicleCommand command)
        {
            var vehicle = await _vehicleProjection.Project(command.LicenseNumber);
            
            if(vehicle == null)
            {
                throw new Exception($"Fordon med regnr {command.LicenseNumber} kan inte hittas");
            }

            if (!vehicle.Available)
            {
                throw new Exception($"Fordon med regnr {command.LicenseNumber} är redan uthyrt");
            }

            return await BookingProcess
                .StartWith(command.BookingNumber)
                .Mutate(booking => booking.ConnectVehicle(command.LicenseNumber, vehicle.MeterReading));
        }
    }
}