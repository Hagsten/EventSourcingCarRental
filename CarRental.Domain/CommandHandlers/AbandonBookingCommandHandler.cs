using System.Threading.Tasks;
using CarRental.Domain.Commands;

namespace CarRental.Domain.CommandHandlers
{
    public class AbandonBookingCommandHandler : ICommandHandler<AbandonBookingCommand>
    {
        public async Task<EntityId> Handle(AbandonBookingCommand command)
        {
            return await BookingProcess
                .StartWith(command.BookingNumber)
                .Mutate(booking => booking.Abandon());
        }
    }
}