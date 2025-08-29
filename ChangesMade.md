Code Changes Overview:
Without touching FlightRoute or Plane, the following changes were made:

1. Added a Passenger class hierarchy instead of using enums.
Here defined a main abstract Passenger class and derived classes for each passenger type (GeneralPassenger, LoyaltyMember, AirlineEmployee, DiscountedPassenger). The extended classes are sealed and they change the data in each type as required. DiscountedPassenger is a new type added as per requirement, who is not allowed a bag and is charged half the ticket price.
Reason: Enum would become too rigid to handle the variations in passenger. If we had no Passenger class then enums would have made sense. But if we have a Passenger class then it is better to define the types via inheritance. It would also give more control to handle logic specific for each type of passenger.

2. Created a Rule class to define individual rules.
This class has a Name, Description of the rule and an IsValid() method to evaluate the rule. All the pre-defined rules are defined here, i.e. SeatRule, MinimumTakeoffPercentageRule, ProfitRule, and also the new AirlineEmployeeRule.
Reason: Even the default ruleset was a combination of three mini rules. So it made sense to have a definition of those mini rules first, and then combine them later to define each ruleset. This makes rule logic modular. Each rule can be applied and validated independently, tested easily, and extended easily.

3. Created a RuleSet class to combine rules.
For now the subclasses are for 'Default' and 'Relaxed' only, but more can be added later. RuleSet class has a name, description, and a list of rules. There are methods to add rules and remove rules. There is also a check to avoid duplication of rules. There is a validation method which will by default validate all the rules in the ruleset. This is used for the Default ruleset, but it can be overriden to create a custom logic, like in Relaxed ruleset. The combination of rules can make the Default ruleset and also the new 'relaxed' ruleset. All rules in a ruleset do not need to be validated for ruleset to be valid, as for relaxed ruleset we have a custom logic.

4. Created a new FlightStats class to hold the flight data.
This class represents the stats of the flight. It is designed to be used as both a utility and a composite class. The constructor will take a flightSchedule and fill in the fields.

5. ScheduledFlight class is modified to be simple and just have Plane, Passengers, FlightStats and FlightRoute along with their basic validations. A validation is added to prevent duplicate passengers to be added in the flight. This class should not be generating the summary, and should only represent the data structure of the flight. CalculateFlightStats() method is added to return the calculated stats of the flight, and there is no backing property which will hold the flight stats, as the stats can change and it can be confusing for the user. So it is better to calculate the stats every time and expose it via a method rather than a property.

6. Created a FlightBookingLogic class to handle the main logic.
This class is responsible for generating the summary and also creating different types of passengers for the CLI input. CreatePassenger() is a helper factory method to create different type of passengers at runtime. GenerateFlightSummary() is the core method of generating summary based on the stats, planes and ruleset provided. It can also get scheduledFlight instead of the stats. It also behaves like a helper method and can generate summary for any given data. This logic to decide about plane is implemented in the GetAlternateSuitablePlaneSuggestions() method, which is called while building the summary, and if the current plane cannot proceed, this method will check the ruleset against all other alternate planes. It will not inform about anything, just add it to the summary.

7. Program class is updated accordingly.
Passengers are now types instead of enums, and user can also add discounted passengers. There is an option to change the ruleset by typing 'use ruleset <ruleset name>', and the system will consider that ruleset to generate summary next time. By default it is set to 'default' but if the user wants to change it relaxed, user can write 'use ruleset relaxed' at any point and the summary will consider relaxed ruleset. There is also a new method to get available alternate planes.
Reason: Just wired up everything here to reflect the changes across the rest of the codebase. Made the CLI flexible enough to change behavior like ruleset and passenger types on the fly, which also helps test different scenarios quickly.