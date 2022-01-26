using System.Collections.Generic;
using CarRental.Domain.Events;

namespace CarRental.Domain.Read.ReadModels
{
    public class BookingAmountModel
    {
        public decimal Amount { get; private set; }
        
        private BookingAmountModel() 
        { }

        public static BookingAmountModel Replay(ICollection<object> events)
        {
            var model = new BookingAmountModel();

            foreach (var e in events)
            {
                model.Apply((dynamic)e);
            }

            return model;
        }

        private void Apply(BookingCompletedEvent e)
        {
            Amount = e.Amount;
        }

        private void Apply(object e)
        {
            //unhandled
        }
    }
}