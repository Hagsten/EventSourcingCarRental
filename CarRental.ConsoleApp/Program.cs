using System;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Domain;
using CarRental.Domain.Events;
using CarRental.Domain.Read;
using CarRental.Storage;

namespace CarRental.ConsoleApp
{
    class Program
    {
        private static readonly Store Store;

        static Program()
        {
            Store = new Store();
        }

        static void Main(string[] args)
        {
            Orchestrator.Initialize().GetAwaiter().GetResult();
            Repository.Initialize().GetAwaiter().GetResult();

            SeedVehicles().GetAwaiter().GetResult();

            Menu.RenderMenu();

            Console.Write("Menu option: ");

            string cmd;

            while ((cmd = Console.ReadLine().ToLower()) != "exit")
            {
                if (!int.TryParse(cmd, out var selectedOption))
                {
                    Console.WriteLine("Invalid option, try again.");
                    Console.Write("Menu option: ");
                    continue;
                }

                try
                {
                    switch (selectedOption)
                    {
                        case 1:
                            Menu.RenderVehicles();
                            break;
                        case 2:
                            Menu.RenderBookings();
                            break;
                        case 3:
                            Menu.RenderStartBooking().GetAwaiter().GetResult();
                            break;
                        case 4:
                            Menu.RenderBookingDetails().GetAwaiter().GetResult();
                            break;
                        case 5:
                            Menu.RenderSimulateVehicleMovement().GetAwaiter().GetResult();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.WriteLine("Tryck [Enter] för att fortsätta...");
                Console.ReadLine();

                Console.Clear();
                Menu.RenderMenu();
                Console.Write("Menu option: ");
            }

            Console.ReadLine();
        }

        private static async Task SeedVehicles()
        {
            var allEvents = await Store.ReadFromStream("$ce-vehicle");

            if (allEvents.Any())
            {
                return;
            }

            Console.WriteLine("Seeding vehicles...");

            var events = new[]
            {
                new VehicleAddedEvent("ABC123", VehicleType.Sedan, 230),
                new VehicleAddedEvent("BDG11A", VehicleType.StationWagon, 5000),
                new VehicleAddedEvent("XYZ22B", VehicleType.Truck, 1337),
                new VehicleAddedEvent("CCC111", VehicleType.StationWagon, 10000)
            };

            foreach (var @event in events)
            {
                await Store.AppendEvent($"vehicle-{@event.LicenseNumber}", new IEvent[] { @event });
            }

            Console.WriteLine("Seeding complete");
        }
    }
}
