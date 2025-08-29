namespace FlightBooking.Core
{
    /// <summary>
    /// Rule class defines a single rule that can be validated against a flight's statistics. Multiple rules can be combined into a RuleSet.
    /// </summary>
    public abstract class Rule
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public abstract bool IsValid(FlightStats flightStats);
    }

    public class ProfitRule : Rule
    {
        public ProfitRule()
        {
            Name = "Profit Rule";
            Description = "The revenue generated from the flight must exceed the cost of the flight";
        }

        public override bool IsValid(FlightStats flightStats)
        {
            if (flightStats.ProfitSurplus <= 0)
            { return false; }

            return true;
        }
    }

    public class SeatRule : Rule
    {
        public SeatRule()
        {
            Name = "Seat Rule";
            Description = "The number of passengers cannot exceed the amount of seats on the plane";
        }

        public override bool IsValid(FlightStats flightStats)
        {
            if (flightStats.SeatsTaken > flightStats.ScheduledFlight.Plane.NumberOfSeats)
            { return false; }

            return true;
        }
    }

    public class MinimumTakeoffPercentageRule : Rule
    {
        public MinimumTakeoffPercentageRule()
        {
            Name = "Minimum Takeoff Percentage Rule";
            Description = "The aircraft must have a minimum percentage of passengers booked for that route";
        }

        public override bool IsValid(FlightStats flightStats)
        {
            var minimumPassengersRequired = (int)Math.Ceiling(flightStats.ScheduledFlight.FlightRoute.MinimumTakeOffPercentage * flightStats.ScheduledFlight.Plane.NumberOfSeats);

            if (flightStats.SeatsTaken < minimumPassengersRequired)
            { return false; }

            return true;
        }
    }

    public class AirlineEmployeeRule : Rule
    {
        public AirlineEmployeeRule()
        {
            Name = "Airline Employee Rule";
            Description = "The number of airline employees aboard is greater than the minimum percentage of passengers required";
        }

        public override bool IsValid(FlightStats flightStats)
        {
            var airlineEmployeeCount = flightStats.ScheduledFlight.Passengers.Count(p => p is AirlineEmployee);
            var minimumPassengersRequired = (int)Math.Ceiling(flightStats.ScheduledFlight.FlightRoute.MinimumTakeOffPercentage * flightStats.ScheduledFlight.Plane.NumberOfSeats);

            if (airlineEmployeeCount < minimumPassengersRequired)
            { return false; }

            return true;
        }
    }
}