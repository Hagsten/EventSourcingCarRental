using System.Linq;
using System.Threading.Tasks;
using CarRental.Domain.Commands;
using CarRental.Domain.Entities.Projections;
using CarRental.Storage;

namespace CarRental.Domain.CommandHandlers
{
    public class DriveVehicleCommandHandler : ICommandHandler<DriveVehicleCommand>
    {
        private readonly Store _store;
        private readonly VehicleProjection _vehicleProjection;

        public DriveVehicleCommandHandler()
        {
            _store = new Store();
            _vehicleProjection = new VehicleProjection();
        }

        public async Task<EntityId> Handle(DriveVehicleCommand command)
        {
            var vehicle = await _vehicleProjection.Project(command.LicenseNumber);

            vehicle.Drive(command.DistanceKm);

            await _store.AppendEvent($"vehicle-{vehicle.LicenseNumber}", vehicle.Events.ToArray());

            return new EntityId(vehicle.LicenseNumber);
        }
    }
}