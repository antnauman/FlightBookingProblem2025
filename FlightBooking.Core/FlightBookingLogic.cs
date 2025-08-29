using System.Text;

namespace FlightBooking.Core
{
    public class FlightBookingLogic
    {
        public static Passenger CreatePassenger(string type, string name, int age, int loyaltyPoints = 0, bool isUsingLoyaltyPoints = false)
        {
            if (string.IsNullOrWhiteSpace(type))
            { throw new Exception("Invalid passenger type"); }

            if (string.IsNullOrWhiteSpace(name))
            { throw new Exception("Invalid passenger name"); }

            if (age <= 0)
            { throw new Exception("Invalid passenger age"); }

            switch (type.Trim().ToLowerInvariant())
            {
                case "general":
                    return new GeneralPassenger { Name = name, Age = age };
                case "loyalty":
                    return new LoyaltyMember { Name = name, Age = age, LoyaltyPoints = loyaltyPoints, IsUsingLoyaltyPoints = isUsingLoyaltyPoints };
                case "airline":
                    return new AirlineEmployee { Name = name, Age = age };
                case "discounted":
                    return new DiscountedPassenger { Name = name, Age = age };
                default:
                    throw new Exception("Invalid passenger type");
            }
        }

        public static string GenerateFlightSummary(FlightStats flightStats, RuleSet ruleSet, IList<Plane> alternatePlanes = null)
        {
            if (flightStats == null || flightStats.ScheduledFlight == null || !flightStats.ScheduledFlight.IsValidFlight())
            { throw new Exception("Flight is invalid"); }
            var summaryBuilder = new StringBuilder();

            summaryBuilder.Append("Flight summary for " + flightStats.ScheduledFlight.FlightRoute.Title);
            summaryBuilder.Append(Statics.VERTICAL_WHITE_SPACE);
            summaryBuilder.AppendLine("Total passengers: " + flightStats.SeatsTaken);
            summaryBuilder.AppendLine(Statics.INDENTATION + "General sales: " + flightStats.ScheduledFlight.Passengers.Count(p => p is GeneralPassenger));
            summaryBuilder.AppendLine(Statics.INDENTATION + "Loyalty member sales: " + flightStats.ScheduledFlight.Passengers.Count(p => p is LoyaltyMember));
            summaryBuilder.AppendLine(Statics.INDENTATION + "Discounted member sales: " + flightStats.ScheduledFlight.Passengers.Count(p => p is DiscountedPassenger));
            summaryBuilder.AppendLine(Statics.INDENTATION + "Airline employee comps: " + flightStats.ScheduledFlight.Passengers.Count(p => p is AirlineEmployee));
            summaryBuilder.Append(Statics.VERTICAL_WHITE_SPACE);
            summaryBuilder.Append("Total expected baggage: " + flightStats.TotalExpectedBaggage);
            summaryBuilder.Append(Statics.VERTICAL_WHITE_SPACE);
            summaryBuilder.AppendLine("Total revenue from flight: " + flightStats.ProfitFromFlight);
            summaryBuilder.AppendLine("Total costs from flight: " + flightStats.CostOfFlight);
            summaryBuilder.Append((flightStats.ProfitSurplus > 0 ? "Flight generating profit of: " : "Flight losing money of: ") + flightStats.ProfitSurplus);
            summaryBuilder.Append(Statics.VERTICAL_WHITE_SPACE);
            summaryBuilder.Append("Total loyalty points given away: " + flightStats.TotalLoyaltyPointsAccrued + Statics.NEW_LINE);
            summaryBuilder.Append("Total loyalty points redeemed: " + flightStats.TotalLoyaltyPointsRedeemed + Statics.NEW_LINE);
            summaryBuilder.Append(Statics.VERTICAL_WHITE_SPACE);

            if (ruleSet.ValidateRuleSet(flightStats))
            { summaryBuilder.AppendLine("THIS FLIGHT MAY PROCEED"); }
            else
            {
                summaryBuilder.AppendLine("FLIGHT MAY NOT PROCEED");

                if (alternatePlanes.Count > 0)
                {
                    string alternatePlanesSuggestion = GetAlternateSuitablePlaneSuggestions(flightStats, ruleSet, alternatePlanes);
                    if (!string.IsNullOrWhiteSpace(alternatePlanesSuggestion))
                    { summaryBuilder.Append(alternatePlanesSuggestion); }
                }
            }
            summaryBuilder.AppendLine(Statics.HORIZONTAL_LINE);
            return summaryBuilder.ToString();
        }

        private static string GetAlternateSuitablePlaneSuggestions(FlightStats flightStats, RuleSet ruleSet, IList<Plane> alternatePlanes)
        {
            if (alternatePlanes == null || alternatePlanes.Count == 0)
            { return string.Empty; }

            var originalPlane = flightStats.ScheduledFlight.Plane;
            var alternateSuitablePlanes = new List<string>();

            try
            {
                foreach (var alternatePlane in alternatePlanes)
                {
                    flightStats.ScheduledFlight.SetPlane(alternatePlane);
                    if (ruleSet.ValidateRuleSet(flightStats))
                    { alternateSuitablePlanes.Add(alternatePlane.Name); }
                }
            }
            finally
            {
                if (originalPlane != null)
                { flightStats.ScheduledFlight.SetPlane(originalPlane); }
            }

            if (alternateSuitablePlanes.Count > 0)
            {
                var alternateSuggestionsBuilder = new StringBuilder();
                alternateSuggestionsBuilder.AppendLine("Other more suitable aircraft are:");

                foreach (var alternateSuitablePlane in alternateSuitablePlanes)
                { alternateSuggestionsBuilder.AppendLine($"{alternateSuitablePlane} could handle this flight."); }

                return alternateSuggestionsBuilder.ToString();
            }

            return string.Empty;
        }
    }
}
