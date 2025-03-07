namespace WebApplication2.Models
{
    public class IrrigationViewModel
    {
        public int Index { get; set; } // Indeks w tablicy stTestArray

        // Wartość zapisana w PLC – liczba milisekund (jako uint)
        public uint CzasRaw { get; set; }

        // Właściwość dla widoku – konwersja na TimeSpan
        public TimeSpan Czas
        {
            get => TimeSpan.FromMilliseconds(CzasRaw);
            set => CzasRaw = (uint)value.TotalMilliseconds;
        }

        // Pola BOOL, jeśli masz więcej wartości logicznych w strukturze PLC
        public bool bBoolTest1 { get; set; }
        public bool bBoolTest2 { get; set; }

        // Możesz dodać więcej właściwości, jeśli struktura PLC zawiera inne pola
    }
}
