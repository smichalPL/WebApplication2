using System.Collections.Generic; // Dodajemy using dla Dictionary
namespace WebApplication2.Models
{
    public class SalonViewModel
    {
        public bool lampSwitch1HMI { get; set; }
        public bool lampSwitch2HMI { get; set; }
        public bool lampSwitch3HMI { get; set; }
        public bool wallSocket1HMI { get; set; }
        public bool wallSocket2HMI { get; set; }
        public bool wallSocket3HMI { get; set; }
        public bool wallSocket4HMI { get; set; }
        public bool facadeBlindsUpHMI { get; set; }
        public bool facadeBlindsDownHMI { get; set; }
        public bool facadeBlindsStopHMI { get; set; }
        public bool facadeBlindsUpHsLeftHMI { get; set; }
        public bool facadeBlindsDownHsLeftHMI { get; set; }
        public bool facadeBlindsStopHsLeftHMI { get; set; }
        public bool facadeBlindsUpHsRightHMI { get; set; }
        public bool facadeBlindsDownHsRightHMI { get; set; }
        public bool facadeBlindsStopHsRightHMI { get; set; }
        public bool lampRelayCeiling1 { get; set; }
        public bool lampRelayCeiling2 { get; set; }
        public bool lampRelayCeiling3 { get; set; }
        public bool windowOpenSensor{ get; set; }
    }
}