using System;

namespace WebApplication2.Models
{
    public struct ST_WeeklyTimeSwitchInput
    {
        public bool bEnable;
        public bool bSunday;
        public bool bMonday;
        public bool bTuesday;
        public bool bWednesday;
        public bool bThursday;
        public bool bFriday;
        public bool bSaturday;
        // Załóżmy, że TOD traktujemy jako uint (4 bajty) i konwertujemy na TimeSpan
        public uint tTimeOnRaw;
        public uint tTimeOffRaw;

        // Właściwości pomocnicze dla widoku (konwersja z TOD)
        public TimeSpan tTimeOn => TimeSpan.FromMilliseconds(tTimeOnRaw);
        public TimeSpan tTimeOff => TimeSpan.FromMilliseconds(tTimeOffRaw);
    }
}
