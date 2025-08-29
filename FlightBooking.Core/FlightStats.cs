namespace FlightBooking.Core
{
    public class FlightStats
    {
        public double CostOfFlight { get; set; }
        public double ProfitFromFlight { get; set; }
        public int TotalLoyaltyPointsAccrued { get; set; }
        public int TotalLoyaltyPointsRedeemed { get; set; }
        public int TotalExpectedBaggage { get; set; }
        public int SeatsTaken { get; set; }
        public double ProfitSurplus { get { return ProfitFromFlight - CostOfFlight; } }
        public ScheduledFlight ScheduledFlight { get; set; }

        public FlightStats()
        { }

        public FlightStats(ScheduledFlight scheduledFlight)
        {
            if (scheduledFlight == null || scheduledFlight.Plane == null || scheduledFlight.FlightRoute == null)
            { throw new Exception("Scheduled flight is incomplete"); }

            this.ScheduledFlight = scheduledFlight;

            var route = scheduledFlight.FlightRoute;

            foreach (var passenger in scheduledFlight.Passengers)
            {
                if (passenger is LoyaltyMember loyaltyMember)
                {
                    if (loyaltyMember.IsUsingLoyaltyPoints)
                    { TotalLoyaltyPointsRedeemed += (int)route.BasePrice; }
                    else
                    { TotalLoyaltyPointsAccrued += route.LoyaltyPointsGained; }
                }

                ProfitFromFlight += route.BasePrice * passenger.PriceFactor;
                CostOfFlight += route.BaseCost;
                TotalExpectedBaggage += passenger.AllowedBags;
                SeatsTaken++;
            }
        }
    }
}
