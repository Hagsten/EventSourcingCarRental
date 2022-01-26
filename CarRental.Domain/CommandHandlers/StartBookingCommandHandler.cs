using System.Threading.Tasks;
using CarRental.Domain.Commands;

namespace CarRental.Domain.CommandHandlers
{
    public class StartBookingCommandHandler : ICommandHandler<StartBookingCommand>
    {
        public async Task<EntityId> Handle(StartBookingCommand command)
        {
            return await BookingProcess
                .StartWith(command.BookingNumber)
                .Mutate(booking => booking.Start(command.StartedAt));
        }
    }
}