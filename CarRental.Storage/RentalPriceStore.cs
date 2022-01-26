namespace CarRental.Storage
{
    public static class RentalPriceStore
    {
        //Simulerar hämtning av variabelt pris. Hårdkodat här i avsaknad av lämplig DB
        public static RentalPrice Prices => new(500, 5);
    }

    public record RentalPrice(int BaseDayPrice, int BaseKmPrice);
}
