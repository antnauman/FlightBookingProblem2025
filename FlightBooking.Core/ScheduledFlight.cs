namespace FlightBooking.Core
{
    public class ScheduledFlight
    {
        public ScheduledFlight()
        {
            Passengers = new List<Passenger>();
        }

        public ScheduledFlight(FlightRoute flightRoute)
        {
            if (flightRoute == null)
            { throw new ArgumentException("Flight route is invalid"); }

            FlightRoute = flightRoute;
            Passengers = new List<Passenger>();
        }

        public FlightRoute FlightRoute { get; private set; }
        public Plane Plane { get; private set; }
        public List<Passenger> Passengers { get; private set; }

        public void SetFlightRoute(FlightRoute flightRoute)
        {
            if (flightRoute == null)
            { throw new ArgumentException("Flight route is invalid"); }
            
            FlightRoute = flightRoute;
        }

        public void SetPlane(Plane plane)
        {
            if (plane == null)
            { throw new ArgumentException("Plane is invalid"); }
            
            Plane = plane;
        }

        /// <summary>
        /// Adds a passenger to the flight if they do not already exist (case insensitive by name).
        /// </summary>
        /// <param name="passenger"></param>
        /// <returns>true if passenger is added</returns>
        public bool AddPassenger(Passenger passenger)
        {
            if (passenger == null)
            { throw new ArgumentException("Passenger is invalid"); }

            if (!Passengers.Exists(x => string.Equals(x.Name, passenger.Name, StringComparison.OrdinalIgnoreCase)))
            {
                Passengers.Add(passenger);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a passenger from the flight.
        /// </summary>
        /// <param name="passenger"></param>
        /// <returns>true if passenger is removed from the flight</returns>
        public bool RemovePassenger(Passenger passenger)
        {
            if (passenger == null)
            { throw new ArgumentException("Passenger is invalid"); }

            Passengers.Remove(passenger);
            return true;
        }

        /// <summary>
        /// This method only checks that the flight has a route, a plane and at least one passenger.
        /// </summary>
        /// <returns></returns>
        public bool IsValidFlight()
        {
            if (Passengers.Count <= 0) { return false; }
            if (Plane == null) { return false; }
            if (FlightRoute == null) { return false; }
            return true;
        }
    }
}