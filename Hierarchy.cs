namespace Hierarchy
{
    class Driver
    {
        public string name;
        public string surname;
        public string patronymic;
        public int age;
        public string city;
    }
    abstract class Transport
    {
        public string brand;           
        public string model;
        public int yearOfManufacture;
        public int power;
        public Driver driver;
    }
    class Truck : Transport
    {
        public int loadCapacity;
        public int numOfAxles;
    }
    abstract class PassengerTransport : Transport
    {
        public int numOfSeats;
    }
    class Car : PassengerTransport
    {
        public string bodyType;
    }
    abstract class PublicTransport : PassengerTransport
    {
        public int ticketPrice;
    }
    class Bus : PublicTransport
    {
        public int length;
    }
    class Metro : PublicTransport
    {
        public int averageInterval;
    }
    class Train : PublicTransport
    {
        public int numOfCarriages;
    }
}
