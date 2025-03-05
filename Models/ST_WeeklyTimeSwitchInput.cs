using System.Runtime.InteropServices;

namespace WebApplication2.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_WeeklyTimeSwitchInput
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool bEnable;
        [MarshalAs(UnmanagedType.U1)]
        public bool bSunday;
        [MarshalAs(UnmanagedType.U1)]
        public bool bMonday;
        [MarshalAs(UnmanagedType.U1)]
        public bool bTuesday;
        [MarshalAs(UnmanagedType.U1)]
        public bool bWednesday;
        [MarshalAs(UnmanagedType.U1)]
        public bool bThursday;
        [MarshalAs(UnmanagedType.U1)]
        public bool bFriday;
        [MarshalAs(UnmanagedType.U1)]
        public bool bSaturday;
    }
}
