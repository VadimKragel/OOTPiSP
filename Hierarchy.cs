#nullable enable

using System.ComponentModel.DataAnnotations;

namespace Hierarchy
{
    enum BusLength {
        [Display(Name = "Ocобо малый")]
        XS,
        [Display(Name = "Малый")]
        S,
        [Display(Name = "Средний")]
        M,
        [Display(Name = "Большой")]
        L,
        [Display(Name = "Особо большой")]
        XL
    };
    enum BodyType
    {
        [Display(Name = "Седан")]
        Sedan,
        [Display(Name = "Хэтчбэк")]
        Hatchback,
        [Display(Name = "Лифтбэк")]
        Liftback,
        [Display(Name = "Купе")]
        Coupe,
        [Display(Name = "Кабриолет")]
        Cabriolet,
        [Display(Name = "Родстер")]
        Roadster,
        [Display(Name = "Тарга")]
        Targa,
        [Display(Name = "Лимузин")]
        Limousine,
        [Display(Name = "Внедорожник")]
        SUV,
        [Display(Name = "Кроссовер")]
        Crossover,
        [Display(Name = "Пикап")]
        Pickup,
        [Display(Name = "Фургон")]
        Van,
        [Display(Name = "Минивэн")]
        Minivan
    };
    [Display(Name ="Водитель")]
    class Driver
    {
        [Required]
        [Display(Name = "Имя")]
        public string? Name { get; set; }
        [Required]
        [Display(Name = "Фамилия")]
        public string? Surname { get; set; }
        [Required]
        [Display(Name = "Отчество")]
        public string? Patronymic { get; set; }
        [Required]
        [Range(18, Double.MaxValue, ErrorMessage = "Допустимый возраст 18+")]
        [Display(Name = "Возраст")]
        public int? Age { get; set; }
        [Required]
        [Display(Name = "Город")]
        public string? City { get; set; }
        public override string ToString()
        {
            return $"{Surname} {Name} {Age}";
        }
    }
    [Display(Name = "Транспорт")]
    abstract class Transport
    {
        [Display(Name = "Имя/ID")]
        public string? Name { get; set; }
        [Required]
        [Display(Name = "Марка")]
        public string? Brand { get; set; }
        [Required]
        [Display(Name = "Модель")]
        public string? Model { get; set; }
        [Required]
        [Display(Name = "Год выпуска")]
        public int? YearOfManufacture { get; set; }
        [Required]
        [Display(Name = "Мощность")]
        public int? Power { get; set; }
        [Required]
        [Display(Name = "Водитель")]
        public Driver? Driver { get; set; }
        public override string ToString()
        {
            string name = !string.IsNullOrEmpty(Name) ? $"{Name} : " : "";
            return $"{name} {Brand} {Model} {YearOfManufacture}";
        }
    }
    [Display(Name = "Грузовик")]
    class Truck : Transport
    {
        [Required]
        [Display(Name = "Грузоподъемность")]
        public int? LoadCapacity { get; set; }
        [Required]
        [Display(Name = "Количество осей")]
        public int? NumOfAxles { get; set; }
    }
    abstract class PassengerTransport : Transport
    {
        [Required]
        [Display(Name = "Количество мест")]
        public int? NumOfSeats { get; set; }
    }
    [Display(Name = "Автомобиль")]
    class Car : PassengerTransport
    {
        [Required]
        [Display(Name = "Тип кузова")]
        public BodyType BodyType { get; set; }
    }
    abstract class PublicTransport : PassengerTransport
    {
        [Required]
        [Display(Name = "Цена билета")]
        public int? TicketPrice { get; set; }
    }
    [Display(Name = "Автобус")]
    class Bus : PublicTransport
    {
        [Required]
        [Display(Name = "Длина")]
        public BusLength Length { get; set; }
    }
    [Display(Name = "Метро")]
    class Metro : PublicTransport
    {
        [Required]
        [Display(Name = "Средний интервал")]
        public int? AverageInterval { get; set; }
    }
    [Display(Name = "Поезд")]
    class Train : PublicTransport
    {
        [Required]
        [Display(Name = "Количество вагонов")]
        public int? NumOfCarriages { get; set; }
    }
}
