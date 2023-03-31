using Hierarchy;

namespace Creators
{
    abstract class Creator
    {
        public abstract object Create();
    }
    class CreatorDriver : Creator
    {
        public override Driver Create()
        {
            return new Driver();
        }
    }
    abstract class CreatorTransport : Creator
    {
        public override abstract Transport Create();
    }
    class CreatorTruck : CreatorTransport
    {
        public override Transport Create()
        {
            return new Truck();
        }
    }
    class CreatorCar : CreatorTransport
    {
        public override Transport Create()
        {
            return new Car();
        }
    }
    class CreatorBus : CreatorTransport
    {
        public override Transport Create()
        {
            return new Bus();
        }
    }
    class CreatorMetro : CreatorTransport
    {
        public override Transport Create()
        {
            return new Metro();
        }
    }
    class CreatorTrain : CreatorTransport
    {
        public override Transport Create()
        {
            return new Train();
        }
    }
   
}