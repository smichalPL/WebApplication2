using System.Collections.Generic; // Dodajemy using dla Dictionary
namespace WebApplication2.Models
{
    public class KitchenViewModel
    {
        public bool lampSwitchHMI { get; set; }
        public bool wallSocket1HMI { get; set; }
        public bool wallSocket2HMI { get; set; }
        public bool facadeBlindsUpLeftHMI { get; set; }
        public bool facadeBlindsDownLeftHMI { get; set; }
        public bool facadeBlindsStopLeftHMI { get; set; }
        public bool facadeBlindsUpRightHMI { get; set; }
        public bool facadeBlindsDownRightHMI { get; set; }
        public bool facadeBlindsStopRightHMI { get; set; }
        public bool lampRelayCeiling { get; set; }
        public bool windowOpenSensor{ get; set; }
    }
}