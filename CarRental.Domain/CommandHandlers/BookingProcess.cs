using System;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Domain.Entities;
using CarRental.Domain.Entities.Projections;
using CarRental.Storage;

namespace CarRental.Domain.CommandHandlers
{
    public class BookingProcess
    {
        private readonly string _bookingNumber;
        private readonly Store _store;
        private readonly BookingProjection _bookingProjection;
        private Booking _booking;

        private BookingProcess(string bookingNumber)
        {
            _bookingNumber = bookingNumber;
            _store = new Store();
            _bookingProjection = new BookingProjection();
        }

        public static BookingProcess StartWith(string bookingNumber)
        {
            return new(bookingNumber);
        }

        public async Task<EntityId> Mutate(Action<Booking> mutator)
        {
            _booking = await _bookingProjection.Project(_bookingNumber);

            mutator(_booking);

            await _store.AppendEvent($"booking-{_booking.BookingNumber}", _booking.Events.ToArray());

            return new EntityId(_booking.BookingNumber);
        }
    }
}