namespace CarRental.Domain.Commands
{
    public class CreateBookingCommand : ICommand
    {
        public string SocialSecurityNumber { get; }

        public CreateBookingCommand(string socialSecurityNumber)
        {
            SocialSecurityNumber = socialSecurityNumber;
        }
    }
}
