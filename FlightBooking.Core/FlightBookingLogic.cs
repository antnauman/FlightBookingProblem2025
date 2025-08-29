using System.Text;

namespace FlightBooking.Core
{
    /// <summary>
    /// This class has the logic of creating passengers for flight, generating flight summaries and suggesting alternate planes if a flight cannot proceed.
    /// </summary>
    public class FlightBookingLogic
    {
        /// <summary>
        /// This is just a helper method to create passengers of different types based on the parameters provided.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <param name="loyaltyPoints"></param>
        /// <param name="isUsingLoyaltyPoints"></param>
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

        /// <summary>
        /// This method generates a flight summary based on the scheduled flight and the ruleset provided. If the flight cannot proceed, it suggests alternate planes if any are suitable.
        /// </summary>
        /// <param name="flightStats"></param>
        /// <param name="ruleSet"></param>
        /// <param name="alternatePlanes"></param>
        /// <returns>Summary as a string</returns>
        public static string GenerateFlightSummary(ScheduledFlight scheduledFlight, RuleSet ruleSet, IList<Plane> alternatePlanes = null)
        {
            if (scheduledFlight == null || !scheduledFlight.IsValidFlight())
            { throw new Exception("Flight is invalid"); }

            var flightStats = scheduledFlight.CalculateFlightStats();
            return GenerateFlightSummary(flightStats, ruleSet, alternatePlanes ?? new List<Plane>());
        }

        /// <summary>
        /// This method generates a flight summary based on the flight statistics and the ruleset provided. If the flight cannot proceed, it suggests alternate planes if any are suitable.
        /// </summary>
        /// <param name="flightStats"></param>
        /// <param name="ruleSet"></param>
        /// <param name="alternatePlanes"></param>
        /// <returns>Summary as a string</returns>
        public static string GenerateFlightSummary(FlightStats flightStats, RuleSet ruleSet, IList<Plane> alternatePlanes = null)
        {
            if (flightStats == null || flightStats.ScheduledFlight == null || !flightStats.ScheduledFlight.IsValidFlight())
            { throw new Exception("Flight is invalid"); }
            var summaryBuilder = new StringBuilder();

            summaryBuilder.AppendLine("Flight summary for " + flightStats.ScheduledFlight.FlightRoute.Title);
            summaryBuilder.AppendLine("Total passengers: " + flightStats.SeatsTaken);
            summaryBuilder.AppendLine("General sales: " + flightStats.ScheduledFlight.Passengers.Count(p => p is GeneralPassenger));
            summaryBuilder.AppendLine("Loyalty member sales: " + flightStats.ScheduledFlight.Passengers.Count(p => p is LoyaltyMember));
            summaryBuilder.AppendLine("Discounted member sales: " + flightStats.ScheduledFlight.Passengers.Count(p => p is DiscountedPassenger));
            summaryBuilder.AppendLine("Airline employee comps: " + flightStats.ScheduledFlight.Passengers.Count(p => p is AirlineEmployee));
            summaryBuilder.AppendLine("Total expected baggage: " + flightStats.TotalExpectedBaggage);
            summaryBuilder.AppendLine("Total revenue from flight: " + flightStats.ProfitFromFlight);
            summaryBuilder.AppendLine("Total costs from flight: " + flightStats.CostOfFlight);
            summaryBuilder.AppendLine((flightStats.ProfitSurplus > 0 ? "Flight generating profit of: " : "Flight losing money of: ") + flightStats.ProfitSurplus);
            summaryBuilder.AppendLine("Total loyalty points given away: " + flightStats.TotalLoyaltyPointsAccrued);
            summaryBuilder.AppendLine("Total loyalty points redeemed: " + flightStats.TotalLoyaltyPointsRedeemed);

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

        /// <summary>
        /// This method checks each of the alternate planes provided to see if any of them would allow the flight to proceed based on the ruleset provided.
        /// </summary>
        /// <param name="flightStats"></param>
        /// <param name="ruleSet"></param>
        /// <param name="alternatePlanes"></param>
        /// <returns>Alternate planes list in a string</returns>
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
