using System.Collections.Generic; // Dodajemy using dla Dictionary

namespace WebApplication2.Models
{
    public class PlcVariablesViewModel
    {
        public bool MyBoolVariable { get; set; }
        public int iCounter { get; set; }
        public string sTekst { get; set; }
        public int iTemperature { get; set; }
        public int iPressure { get; set; }
        public bool MomentarySwitch { get; set; }
        public bool ToggleSwitch { get; set; }

        //public Dictionary<string, bool> ButtonStates { get; set; } = new Dictionary<string, bool>(); // Dodajemy słownik
    }
}