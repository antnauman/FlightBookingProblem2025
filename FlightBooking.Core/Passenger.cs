namespace FlightBooking.Core
{
    public abstract class Passenger
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public virtual int AllowedBags => 1;
        public virtual double PriceFactor => 1;
    }

    public sealed class GeneralPassenger : Passenger
    {
    }

    public sealed class LoyaltyMember : Passenger
    {
        public int LoyaltyPoints { get; set; }
        public bool IsUsingLoyaltyPoints { get; set; }

        public override int AllowedBags => 2;
        public override double PriceFactor => IsUsingLoyaltyPoints ? 0 : 1;
    }

    public sealed class AirlineEmployee : Passenger
    {
        public override double PriceFactor => 0;
    }

    public sealed class DiscountedPassenger : Passenger
    {
        public override int AllowedBags => 0;
        public override double PriceFactor => 0.5;
    }
}
