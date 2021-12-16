var myFutureCar = new Car
{
    Manufacturer = "Lamborghini",
    Model = "Aventador",
    Price = 100000,
    LaunchedOn = new DateOnly(2011, 2, 28)
};

var myCurrentCar = new Car
{
    Manufacturer = "Volkswagen",
    Model = "Golf",
    Price = 2000,
    LaunchedOn = new DateOnly(1974, 5, 29)
};

// You can use Var
var (model, manufacturer) = myCurrentCar;
var (year, month, day) = myCurrentCar.LaunchedOn;

Console.WriteLine(model);
Console.WriteLine(manufacturer);
Console.WriteLine($"{day}/{month}/{year}");

// Or you can use explicit types
(string futureModel, string futureManufacturer) = myFutureCar;
(int futureYear, int futureMonth, int futureDay) = myCurrentCar.LaunchedOn;

Console.WriteLine(futureModel);
Console.WriteLine(futureManufacturer);
Console.WriteLine($"{futureDay}/{futureMonth}/{futureYear}");

var myBike = new Bike("BMX", 300, new DateOnly(1999, 5, 29));

// Record Type has built-in Deconstruct method
// You can use the _ symbol to skip a property
var (bikeModel, bikePrice, _) = myBike;

Console.WriteLine(bikeModel);
Console.WriteLine(bikePrice);

(string manufacturer, string model) GetCarDetails()
{
    return ("Lamborghini", "Aventador");
}

var details = GetCarDetails();

Console.WriteLine(details.model);
Console.WriteLine(details.manufacturer);

var (detailsManufacturer, detailsModel) = GetCarDetails();

Console.WriteLine(detailsManufacturer);
Console.WriteLine(detailsModel);

public record Bike(string Model, decimal Price, DateOnly LaunchedOn);

public class Car
{
    public string Model { get; set; }
    public string Manufacturer { get; set; }
    public decimal Price { get; set; }
    public DateOnly LaunchedOn { get; set; }

    public void Deconstruct(out string model, out string manufacturer)
    {
        model = this.Model;
        manufacturer = this.Manufacturer;
    }
}

// You can add Deconstruct to other classes via Extensions
public static class DeconstructorsExtension
{
    public static void Deconstruct(this DateOnly dateOnly, out int year, out int month, out int day)
    {
        year = dateOnly.Year;
        month = dateOnly.Month;
        day = dateOnly.Day;
    }
}