using System.Collections.Generic; // Dodajemy using dla Dictionary
namespace WebApplication2.Models
{
    public class OutdoorViewModel
    {
        public bool lampSwitch1HMI { get; set; }
        public bool lampSwitch2HMI { get; set; }
        public bool wallSocket1HMI { get; set; }
        public bool wallSocket2HMI { get; set; }
        public bool wallSocket3HMI { get; set; }
        public bool lampRelayCeiling1 { get; set; }
        public bool lampRelayCeiling2 { get; set; }
    }
}