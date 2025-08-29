using FlightBooking.Core;

namespace FlightBooking.Tests
{
    public class BasicTests
    {
        private ScheduledFlight BuildSampleFlight()
        {
            var route = new FlightRoute("London", "Paris")
            {
                BaseCost = 50,
                BasePrice = 100,
                LoyaltyPointsGained = 5,
                MinimumTakeOffPercentage = 0.7
            };

            var scheduledFlight = new ScheduledFlight(route);
            scheduledFlight.SetPlane(new Plane { Id = 123, Name = "Antonov AN-2", NumberOfSeats = 12 });

            scheduledFlight.AddPassenger(new GeneralPassenger { Name = "Steve", Age = 30 });
            scheduledFlight.AddPassenger(new GeneralPassenger { Name = "Mark", Age = 12 });
            scheduledFlight.AddPassenger(new GeneralPassenger { Name = "James", Age = 36 });
            scheduledFlight.AddPassenger(new GeneralPassenger { Name = "Jane", Age = 32 });
            scheduledFlight.AddPassenger(new LoyaltyMember { Name = "John", Age = 29, LoyaltyPoints = 1000, IsUsingLoyaltyPoints = true });
            scheduledFlight.AddPassenger(new LoyaltyMember { Name = "Sarah", Age = 45, LoyaltyPoints = 1250, IsUsingLoyaltyPoints = false });
            scheduledFlight.AddPassenger(new LoyaltyMember { Name = "Jack", Age = 60, LoyaltyPoints = 50, IsUsingLoyaltyPoints = false });
            scheduledFlight.AddPassenger(new AirlineEmployee { Name = "Trevor", Age = 47 });
            scheduledFlight.AddPassenger(new GeneralPassenger { Name = "Alan", Age = 34 });
            scheduledFlight.AddPassenger(new GeneralPassenger { Name = "Suzy", Age = 21 });

            return scheduledFlight;
        }

        [Fact]
        public void SampleFlight_ComputesStats()
        {
            var sampleFlight = BuildSampleFlight();
            var flightStats = new FlightStats(sampleFlight);

            Assert.Equal(10, flightStats.SeatsTaken);
            Assert.Equal(13, flightStats.TotalExpectedBaggage);         // 8 general + 2 loyalty (2 bags each) + 1 employee = 8 + 4 + 1 = 13
            Assert.Equal(800, flightStats.ProfitFromFlight);            // 6 general + 2 loyalty cash = 8 * 100 = 800
            Assert.Equal(500, flightStats.CostOfFlight);                // 10 * 50 = 500
            Assert.Equal(300, flightStats.ProfitSurplus);               // 800 - 500 = 300
            Assert.Equal(10, flightStats.TotalLoyaltyPointsAccrued);    // 2 paying loyalty * 5 = 10
            Assert.Equal(100, flightStats.TotalLoyaltyPointsRedeemed);  // 1 using points * 100 = 100
        }

        [Fact]
        public void DiscountedPassengerBehaviour()
        {
            var flightRoute = new FlightRoute("Edinburgh", "Manchester")
            {
                BaseCost = 50,
                BasePrice = 100,
                LoyaltyPointsGained = 5,
                MinimumTakeOffPercentage = 0.0
            };

            var scheduledFlight = new ScheduledFlight(flightRoute);
            scheduledFlight.SetPlane(new Plane { Id = 1, Name = "Plane01", NumberOfSeats = 5 });
            scheduledFlight.AddPassenger(new DiscountedPassenger { Name = "Passenger01", Age = 25 });

            var s = new FlightStats(scheduledFlight);
            Assert.Equal(1, s.SeatsTaken);
            Assert.Equal(0, s.TotalLoyaltyPointsAccrued);
            Assert.Equal(0, s.TotalLoyaltyPointsRedeemed);
            Assert.Equal(0, s.TotalExpectedBaggage); // 0 bags
            Assert.Equal(50, s.ProfitFromFlight);    // half price
            Assert.Equal(50, s.CostOfFlight);
        }

        [Fact]
        public void SampleFlight_DefaultRuleSet()
        {
            var sampleFlight = BuildSampleFlight();
            var flightStats = new FlightStats(sampleFlight);

            var ruleSet = new DefaultRuleSet();
            Assert.True(ruleSet.ValidateRuleSet(flightStats));
        }

        [Fact]
        public void VerifyRelaxedRuleSet()
        {
            var flightRoute = new FlightRoute("Edinburgh", "Manchester")
            {
                BaseCost = 100,
                BasePrice = 100,
                LoyaltyPointsGained = 5,
                MinimumTakeOffPercentage = 0.5
            };
            
            var scheduledFlight = new ScheduledFlight(flightRoute);
            scheduledFlight.SetPlane(new Plane { Id = 2, Name = "Plane02", NumberOfSeats = 4 });

            scheduledFlight.AddPassenger(new GeneralPassenger { Name = "Passenger01", Age = 30 });
            scheduledFlight.AddPassenger(new AirlineEmployee { Name = "Passenger02", Age = 30 }); // also counts toward waiver

            var flightStats = new FlightStats(scheduledFlight);

            // Default ruleset should fail as it is not profitable
            Assert.False(new DefaultRuleSet().ValidateRuleSet(flightStats));

            // Relaxed ruleset should fail here
            Assert.False(new RelaxedRuleSet().ValidateRuleSet(flightStats));

            // Add one more airline employee and Relaxed ruleset should now pass
            scheduledFlight.AddPassenger(new AirlineEmployee { Name = "Passenger03", Age = 31 });
            flightStats = new FlightStats(scheduledFlight);
            Assert.True(new RelaxedRuleSet().ValidateRuleSet(flightStats));
        }

        [Fact]
        public void FlightBookingLogicSummary_RestoresPlane()
        {
            var sampleFlight = BuildSampleFlight();
            var flightStats = new FlightStats(sampleFlight);

            var alternatePlanes = new List<Plane>
            {
                new Plane { Id = 200, Name = "Bombardier Q400", NumberOfSeats = 8  },
                new Plane { Id = 201, Name = "ATR 640", NumberOfSeats = 20 },
                new Plane { Id = 202, Name = "De Havilland DHC-6", NumberOfSeats = 19 }
            };

            var rules = new DefaultRuleSet();
            var plane = sampleFlight.Plane;
            var summary = FlightBookingLogic.GenerateFlightSummary(flightStats, rules, alternatePlanes);

            Assert.Contains("Flight summary for London to Paris", summary);
            Assert.Contains("Total passengers: 10", summary);
            Assert.Contains("General sales: 6", summary);
            Assert.Contains("Loyalty member sales: 3", summary);
            Assert.Contains("Discounted member sales: 0", summary);
            Assert.Contains("Airline employee comps: 1", summary);
            Assert.Contains("Total expected baggage: 13", summary);
            Assert.Contains("Total revenue from flight: 800", summary);
            Assert.Contains("Total costs from flight: 500", summary);
            Assert.Contains("Flight generating profit of: 300", summary);
            Assert.Contains("Total loyalty points given away: 10", summary);
            Assert.Contains("Total loyalty points redeemed: 100", summary);
            Assert.Contains("THIS FLIGHT MAY PROCEED", summary);
            Assert.Equal(plane, sampleFlight.Plane);
        }
    }
}
