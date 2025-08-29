using FlightBooking.Core;

namespace FlightBookingProblem
{
    class Program
    {
        private static ScheduledFlight SampleScheduledFlight;

        static void Main(string[] args)
        {
            try
            {
                SampleScheduledFlight = GetBaseScheduledFlight();

                string command = "";
                do
                {
                    try
                    {
                        command = Console.ReadLine() ?? string.Empty;
                        var enteredText = command.ToLowerInvariant();
                        string[] passengerSegments = enteredText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (enteredText.Contains("print summary"))
                        {
                            RuleSet ruleSet;
                            if (enteredText.Contains("relaxed"))
                            { ruleSet = new RelaxedRuleSet(); }
                            else
                            { ruleSet = new DefaultRuleSet(); }

                            var stats = new FlightStats(SampleScheduledFlight);
                            var alternatePlanes = GetAlternatePlanes();

                            Console.WriteLine();
                            Console.WriteLine(FlightBookingLogic.GenerateFlightSummary(stats, ruleSet, alternatePlanes));
                        }
                        else if (enteredText.Contains("add general"))
                        {
                            var newPassenger = FlightBookingLogic.CreatePassenger("general", passengerSegments[2], Convert.ToInt32(passengerSegments[3]));
                            SampleScheduledFlight.AddPassenger(newPassenger);
                        }
                        else if (enteredText.Contains("add loyalty"))
                        {
                            var newPassenger = FlightBookingLogic.CreatePassenger("loyalty", passengerSegments[2], Convert.ToInt32(passengerSegments[3]), Convert.ToInt32(passengerSegments[4]), Convert.ToBoolean(passengerSegments[5]));
                            SampleScheduledFlight.AddPassenger(newPassenger);
                        }
                        else if (enteredText.Contains("add airline"))
                        {
                            var newPassenger = FlightBookingLogic.CreatePassenger("airline", passengerSegments[2], Convert.ToInt32(passengerSegments[3]));
                            SampleScheduledFlight.AddPassenger(newPassenger);
                        }
                        else if (enteredText.Contains("add discounted"))
                        {
                            var newPassenger = FlightBookingLogic.CreatePassenger("discounted", passengerSegments[2], Convert.ToInt32(passengerSegments[3]));
                            SampleScheduledFlight.AddPassenger(newPassenger);
                        }
                        else if (enteredText.Contains("exit"))
                        {
                            Environment.Exit(0);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("UNKNOWN INPUT");
                            Console.ResetColor();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: " + ex.Message);
                        Console.ReadKey();
                    }
                } while (command != "exit");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                Console.ReadKey();
            }
        }

        private static ScheduledFlight GetBaseScheduledFlight()
        {
            var tempScheduledFlight = new ScheduledFlight();
            var tempFlightRoute = new FlightRoute("London", "Paris")
            {
                BaseCost = 50,
                BasePrice = 100,
                LoyaltyPointsGained = 5,
                MinimumTakeOffPercentage = 0.7
            };
            tempScheduledFlight.SetFlightRoute(tempFlightRoute);

            var tempPlane = new Plane { Id = 123, Name = "Antonov AN-2", NumberOfSeats = 12 };
            tempScheduledFlight.SetPlane(tempPlane);

            return tempScheduledFlight;
        }

        private static List<Plane> GetAlternatePlanes()
        {
            var alternatePlanes = new List<Plane>
            {
                new Plane { Id = 124, Name = "Bombardier Q400", NumberOfSeats = 8 },
                new Plane { Id = 125, Name = "ATR 640", NumberOfSeats = 20 },
                new Plane { Id = 126, Name = "Airbus A350", NumberOfSeats = 50 }
            };

            return alternatePlanes;
        }
    }
}