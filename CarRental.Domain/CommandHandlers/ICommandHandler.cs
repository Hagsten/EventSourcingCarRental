using System.Threading.Tasks;
using CarRental.Domain.Commands;

namespace CarRental.Domain.CommandHandlers
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task<EntityId> Handle(TCommand command);
    }
}