namespace WebApplication2.Models
{
    public class Zmienna
    {
        public string Nazwa { get; set; }
        public string Typ { get; set; } // Opcjonalne: jeśli chcesz przechowywać typ zmiennej
        public object Wartosc { get; set; }
    }
}
