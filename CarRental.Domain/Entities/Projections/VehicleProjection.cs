using System.Linq;
using System.Threading.Tasks;
using CarRental.Storage;

namespace CarRental.Domain.Entities.Projections
{
    public interface IVehicleProjection
    {
        Task<Vehicle> Project(string licenseNumber);
    }
    
    public class VehicleProjection : IVehicleProjection
    {
        private readonly Store _store;

        public VehicleProjection()
        {
            _store = new Store();
        }

        public async Task<Vehicle> Project(string licenseNumber)
        {
            var eventStream = await _store.ReadFromStream($"vehicle-{licenseNumber}");

            if (eventStream.Count == 0)
            {
                return null;
            }
            
            var events = eventStream.Select(x => EventDeserializer.Deserialize(x.Event.Data.Span.ToArray(), x.Event.EventType)).ToArray();

            return Vehicle.Replay(events);
        }
    }
}