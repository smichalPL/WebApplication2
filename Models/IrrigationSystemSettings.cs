namespace WebApplication2.Models
{
    public class IrrigationSystemSettings
    {
        public ST_WeeklyTimeSwitchInput[] Section1 { get; set; }
        public ST_WeeklyTimeSwitchInput[] Section2 { get; set; }
        public ST_WeeklyTimeSwitchInput[] Section3 { get; set; }
        public ST_WeeklyTimeSwitchInput[] Section4 { get; set; }
        public ST_WeeklyTimeSwitchInput[] Section5 { get; set; }
        public ST_WeeklyTimeSwitchInput[] Section6 { get; set; }
        public bool[] ValveSwitches { get; set; }
    }
}