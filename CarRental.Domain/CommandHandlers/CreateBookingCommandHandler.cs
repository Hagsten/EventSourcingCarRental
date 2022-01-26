using System.Linq;
using CarRental.Domain.Commands;
using System.Threading.Tasks;
using CarRental.Domain.Entities;
using CarRental.Storage;

namespace CarRental.Domain.CommandHandlers
{
    public class CreateBookingCommandHandler : ICommandHandler<CreateBookingCommand>
    {
        private readonly Store _store;

        public CreateBookingCommandHandler()
        {
            _store = new Store();
        }

        public async Task<EntityId> Handle(CreateBookingCommand command)
        {
            var booking = Booking.CreateNew(command.SocialSecurityNumber);

            await _store.AppendEvent($"booking-{booking.BookingNumber}", booking.Events.ToArray());

            return new EntityId(booking.BookingNumber);
        }
    }
}
