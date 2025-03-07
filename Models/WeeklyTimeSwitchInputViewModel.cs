namespace WebApplication2.Models
{
    public class WeeklyTimeSwitchInputViewModel
    {
        public int Index { get; set; }
        public bool bEnable { get; set; }
        public bool bSunday { get; set; }
        public bool bMonday { get; set; }
        public bool bTuesday { get; set; }
        public bool bWednesday { get; set; }
        public bool bThursday { get; set; }
        public bool bFriday { get; set; }
        public bool bSaturday { get; set; }
        public TimeSpan tTimeOn { get; set; }
        public TimeSpan tTimeOff { get; set; }
    }

}
