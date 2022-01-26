using System.Linq;
using System.Threading.Tasks;
using CarRental.Domain.Commands;
using CarRental.Domain.Entities.Projections;
using CarRental.Storage;

namespace CarRental.Domain.CommandHandlers
{
    public class CheckInVehicleCommandHandler : ICommandHandler<CheckInVehicleCommand>
    {
        private readonly Store _store;
        private readonly VehicleProjection _vehicleProjection;

        public CheckInVehicleCommandHandler()
        {
            _store = new Store();
            _vehicleProjection = new VehicleProjection();
        }

        public async Task<EntityId> Handle(CheckInVehicleCommand command)
        {
            var vehicle = await _vehicleProjection.Project(command.LicenseNumber);

            vehicle.CheckIn();

            await _store.AppendEvent($"vehicle-{vehicle.LicenseNumber}", vehicle.Events.ToArray());

            return new EntityId(vehicle.LicenseNumber);
        }
    }
}