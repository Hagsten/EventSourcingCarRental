using System.Threading.Tasks;
using CarRental.Domain.Commands;

namespace CarRental.Domain.CommandHandlers
{
    public class PayBookingCommandHandler : ICommandHandler<PayBookingCommand>
    {
        public async Task<EntityId> Handle(PayBookingCommand command)
        {
            return await BookingProcess
                .StartWith(command.BookingNumber)
                .Mutate(booking => booking.Pay(command.Amount));
        }
    }
}