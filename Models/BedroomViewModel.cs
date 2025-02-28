using System.Collections.Generic; // Dodajemy using dla Dictionary
namespace WebApplication2.Models
{
    public class BedroomViewModel
     {
        public bool lampSwitchLeftHMI { get; set; }
        public bool wallSocketHMI { get; set; }
        public bool facadeBlindsUpHMI { get; set; }
        public bool facadeBlindsDownHMI { get; set; }
        public bool facadeBlindsStopHMI { get; set; }
        public bool lampRelayCeiling { get; set; }
        public bool windowOpenSensor { get; set; }
        // ... inne zmienne dla sypialni
    }
}
