namespace WebApplication2.Models
{
    public class WeeklyTimeSwitchCompositeViewModel
    {
        public List<WeeklyTimeSwitchInputViewModel> Section0 { get; set; }
        public List<WeeklyTimeSwitchInputViewModel> Section1 { get; set; }
        public List<WeeklyTimeSwitchInputViewModel> Section2 { get; set; }
        public List<WeeklyTimeSwitchInputViewModel> Section3 { get; set; }
        public List<WeeklyTimeSwitchInputViewModel> Section4 { get; set; }
        public List<WeeklyTimeSwitchInputViewModel> Section5 { get; set; }
        public List<WeeklyTimeSwitchInputViewModel> Section6 { get; set; }

        public bool[] ValveSwitch { get; set; }
    }

}
