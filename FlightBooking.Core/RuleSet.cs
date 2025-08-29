namespace FlightBooking.Core
{
    /// <summary>
    /// These RuleSet class represents a collection of rules that can be applied to check whether a flight can proceed or not.
    /// </summary>
    public abstract class RuleSet
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<Rule> Rules { get; set; }

        public void AddRule(Rule rule)
        {
            if (!Rules.Contains(rule))
            { Rules.Add(rule); }
        }

        public void RemoveRule(Rule rule)
        {
            Rules.Remove(rule);
        }

        public virtual bool ValidateRuleSet(FlightStats flightStats)
        {
            foreach (var rule in Rules)
            {
                if (!rule.IsValid(flightStats))
                { return false; }
            }
            return true;
        }
    }

    /// <summary>
    /// This DefaultRuleSet class includes basic rules such as SeatRule, ProfitRule, and MinimumTakeoffPercentageRule.
    /// </summary>
    public class DefaultRuleSet : RuleSet
    {
        public DefaultRuleSet()
        {
            Name = "Default RuleSet";
            Description = "This RuleSet has default criteria";
            Rules = new List<Rule>();

            AddRule(new SeatRule());
            AddRule(new ProfitRule());
            AddRule(new MinimumTakeoffPercentageRule());
        }
    }

    /// <summary>
    /// This RelaxedRuleSet class includes a combination of rules that lets a flight proceed if airline employees aboard are greater than the minimum percentage of passengers required, even if the flight is not profitable.
    /// </summary>
    public class RelaxedRuleSet : RuleSet
    {
        public RelaxedRuleSet()
        {
            Name = "Relaxed RuleSet";
            Description = "If the number of airline employees aboard is greater than the minimum percentage of passengers required, then the revenue generated doesn’t need to exceed cost";
            Rules = new List<Rule>();

            AddRule(new SeatRule());
            AddRule(new ProfitRule());
            AddRule(new MinimumTakeoffPercentageRule());
            AddRule(new AirlineEmployeeRule());
        }

        /// <summary>
        /// This is a custom implementation of ValidateRuleSet to comply with the relaxed rule logic.
        /// </summary>
        /// <param name="flightStats"></param>
        /// <returns>true if ruleset is validated</returns>
        public override bool ValidateRuleSet(FlightStats flightStats)
        {
            var seatRule = Rules.OfType<SeatRule>().FirstOrDefault();
            var minimumTakeoffPercentageRule = Rules.OfType<MinimumTakeoffPercentageRule>().FirstOrDefault();
            var profitRule = Rules.OfType<ProfitRule>().FirstOrDefault();
            var airlineEmployeeRule = Rules.OfType<AirlineEmployeeRule>().FirstOrDefault();

            if (seatRule == null || minimumTakeoffPercentageRule == null || profitRule == null || airlineEmployeeRule == null)
            { return false; }

            var isSeatRuleValid = seatRule.IsValid(flightStats);
            var isMinimumTakeoffPercentageRuleValid = minimumTakeoffPercentageRule.IsValid(flightStats);
            var isProfitRuleValid = profitRule.IsValid(flightStats);
            var isAirlineEmployeeRuleValid = airlineEmployeeRule.IsValid(flightStats);

            if (isSeatRuleValid && isMinimumTakeoffPercentageRuleValid && (isProfitRuleValid || isAirlineEmployeeRuleValid))
            { return true; }

            return false;
        }
    }
}
