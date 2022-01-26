using System;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Domain;
using CarRental.Domain.CommandHandlers;
using CarRental.Domain.Commands;
using CarRental.Domain.Read;
using CarRental.Domain.Read.ReadModels;
using ConsoleTables;

namespace CarRental.ConsoleApp
{
    public static class Menu
    {
        public static void RenderMenu()
        {
            Console.WriteLine("Vad vill du göra?");

            var table = new ConsoleTable("Option", "Name");
            table.AddRow("1", "Visa fordon")
                .AddRow("2", "Visa bokningar")
                .AddRow("3", "Starta en bokning")
                .AddRow("4", "Öppna en specifik bokning")
                .AddRow("5", "Simulera en körsträcka");

            table.Write(Format.Alternative);
            Console.WriteLine();
        }

        public static void RenderVehicles()
        {
            ConsoleTable
                .From(Repository.Vehicles)
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .Write(Format.Alternative);
        }

        public static void RenderBookings()
        {
            ConsoleTable
                .From(Repository.BookingOverview.Where(x => !x.IsAbandoned).OrderByDescending(x => x.HasEnded))
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .Write(Format.Alternative);
        }

        public static async Task RenderStartBooking()
        {
            var bookingNumber = await DispatchCreateBookingCommand();

            await DispatchSelectVehicleCommand(bookingNumber);

            await DispatchStartBookingCommand(bookingNumber);
        }

        public static async Task RenderBookingDetails()
        {
            RenderBookings();

            var booking = FetchBooking();

            if (booking == null)
            {
                return;
            }
            
            if (string.IsNullOrWhiteSpace(booking.VehicleLicenseNumber))
            {
                await RenderAbandonBooking(booking);

                return;
            }

            var vehicle = Repository.Vehicles.Single(x => x.LicenseNumber == booking.VehicleLicenseNumber);

            new ConsoleTable("Egenskap", "Värde")
                .AddRow("Bokningsnummer", booking.BookingNumber)
                .AddRow("Fordon", $"{booking.VehicleLicenseNumber} ({vehicle.VehicleType})")
                .AddRow("Ingående mätarställning", $"{booking.IngoingMeterReading} km")
                .AddRow("Personnummer", booking.RentedBy)
                .AddRow("Utlämnad", booking.StartedAt)
                .AddRow("Status", booking.HasEnded ? "Slutförd" : "Pågående")
                .Write(Format.Alternative);

            Console.WriteLine();

            if (booking.HasEnded)
            {
                Console.Write("Bokningen är slutförd, tryck [Enter] för att återgå");
                Console.ReadLine();
                return;
            }

            Console.Write("Vill du lämna tillbaka fordonet? [Y] / [N]");

            var response = CollectInput();

            if (response == "Y" || response == "y")
            {
                await CommandDispatcher.Dispatch(new ReturnVehicleCommand(booking.BookingNumber));

                Console.Write("Tack, tryck enter för ta del av fakturan");
                Console.ReadLine();

                await RenderBill(booking, vehicle);

                await RenderPayment(booking);
            }
            else
            {
                Console.WriteLine("Okej, återgår till huvudmenyn...");
            }
        }

        public static async Task RenderSimulateVehicleMovement()
        {
            RenderVehicles();
            Console.Write("Ange registreringenummer: ");

            var licenseNumber = CollectInput();

            var vehicle = Repository.Vehicles.FirstOrDefault(x => x.LicenseNumber == licenseNumber);

            if (vehicle == null)
            {
                Console.WriteLine($"Fordon med registreringenummer {licenseNumber} kan inte hittas. Tryck [Enter] för att återgå.");
                Console.ReadLine();
                return;
            }

            Console.Write("Ange körsträcka i kilometer: ");

            var distanceKm = Convert.ToInt32(CollectInput());

            await CommandDispatcher.Dispatch(new DriveVehicleCommand(licenseNumber, distanceKm));

            Console.Write("Tack, körningen är registrerad.");
        }

        private static string CollectInput()
        {
            var input = Console.ReadLine();

            while (input == null)
            {
                Console.WriteLine("Felaktig inmatning, försök igen: ");
                input = Console.ReadLine();
            }

            return input;
        }

        private static async Task DispatchStartBookingCommand(EntityId bookingNumber)
        {
            Console.Write("Tack, bokningen är nu redo. Tryck [Enter] för att påbörja bokningen.");
            Console.ReadLine();

            await CommandDispatcher.Dispatch(new StartBookingCommand(bookingNumber.Value, SimulateStartDate()));

            Console.WriteLine("Tack, bokningen är nu påbörjad.");
        }

        private static async Task DispatchSelectVehicleCommand(EntityId bookingNumber)
        {
            Console.Write("Ange fordonets registreringsnummer: ");

            await CommandDispatcher.Dispatch(new SelectVehicleCommand(bookingNumber.Value, CollectInput()));
        }

        private static async Task<EntityId> DispatchCreateBookingCommand()
        {
            Console.Write("Ange ditt personnummer: ");

            var bookingNumber = await CommandDispatcher.Dispatch(new CreateBookingCommand(CollectInput()));

            Console.WriteLine($"Bokning med bokningsnummer {bookingNumber.Value} påbörjad.");

            return bookingNumber;
        }

        private static async Task RenderPayment(BookingOverviewModel booking)
        {
            Console.Write("Tryck [Enter] för att betala");
            Console.ReadLine();

            await CommandDispatcher.Dispatch(new PayBookingCommand(booking.BookingNumber, 1000M));

            Console.WriteLine("Tack för din betalning, bokningen är nu slutförd. Tack för denna gång!");
        }

        private static async Task RenderBill(BookingOverviewModel booking, VehicleOverviewModel vehicle)
        {
            var bookingAmount =
                await new ProjectionFromBooking<BookingAmountModel>(BookingAmountModel.Replay).Project(booking.BookingNumber);

            Console.WriteLine($"{Environment.NewLine}==== Faktura gällande {booking.BookingNumber} ====");
            Console.WriteLine($"Att betala: {bookingAmount.Amount} kr");

            new ConsoleTable("Summering", "")
                .AddRow("Fordon", $"{booking.VehicleLicenseNumber} ({vehicle.VehicleType})")
                .AddRow("Tidsperiod", $"{booking.StartedAt} - {booking.EndedAt}")
                .AddRow("Längd (dgr)", booking.DurationInDays)
                .AddRow("Körsträcka", booking.DistanceKm)
                .Write(Format.Alternative);
        }

        private static async Task RenderAbandonBooking(BookingOverviewModel booking)
        {
            Console.Write("Bokningen verkar ha hamnat i kläm. Vill överge den? [Y] / [N]");
            var r = CollectInput();

            if (r == "Y" || r == "y")
            {
                await CommandDispatcher.Dispatch(new AbandonBookingCommand(booking.BookingNumber));
                Console.Write("Tack, bokningen är nu övergiven");
                Console.ReadLine();
            }
        }

        private static BookingOverviewModel FetchBooking()
        {
            Console.Write("Ange bokningsnummer: ");

            var bookingNumber = CollectInput();

            var booking = Repository.BookingOverview.SingleOrDefault(x => x.BookingNumber == bookingNumber);

            if (booking == null || booking.IsAbandoned)
            {
                Console.WriteLine($"Bokningen {bookingNumber} kunde inte hittas");

                return null;
            }

            return booking;
        }

        private static DateTimeOffset SimulateStartDate()
        {
            var rnd = new Random();

            var daysBack = rnd.Next(1, 14);

            return DateTimeOffset.Now.AddDays(-daysBack);
        }
    }
}
