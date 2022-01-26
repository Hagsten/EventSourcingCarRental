using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Domain.Commands;

namespace CarRental.Domain.CommandHandlers
{
    public static class CommandDispatcher
    {
        private static readonly Dictionary<Type, Func<ICommand, Task<EntityId>>> CommandHandlers = new()
        {
            { typeof(CreateBookingCommand), command => new CreateBookingCommandHandler().Handle((CreateBookingCommand)command) },
            { typeof(SelectVehicleCommand), command => new SelectVehicleCommandHandler().Handle((SelectVehicleCommand)command) },
            { typeof(StartBookingCommand), command => new StartBookingCommandHandler().Handle((StartBookingCommand)command) },
            { typeof(ReturnVehicleCommand), command => new ReturnVehicleCommandHandler().Handle((ReturnVehicleCommand)command) },
            { typeof(PayBookingCommand), command => new PayBookingCommandHandler().Handle((PayBookingCommand)command) },
            { typeof(DriveVehicleCommand), command => new DriveVehicleCommandHandler().Handle((DriveVehicleCommand)command) },
            { typeof(CheckOutVehicleCommand), command => new CheckOutVehicleCommandHandler().Handle((CheckOutVehicleCommand)command) },
            { typeof(CheckInVehicleCommand), command => new CheckInVehicleCommandHandler().Handle((CheckInVehicleCommand)command) },
            { typeof(AbandonBookingCommand), command => new AbandonBookingCommandHandler().Handle((AbandonBookingCommand)command) }
        };

        public static async Task<EntityId> Dispatch<TCommand>(TCommand command) where TCommand : ICommand
        {
            var type = command.GetType();

            if (!CommandHandlers.ContainsKey(type))
            {
                throw new Exception($"There is no command handler registered for command {type.Name}");
            }

            return await CommandHandlers[type](command);
        }
    }
}
