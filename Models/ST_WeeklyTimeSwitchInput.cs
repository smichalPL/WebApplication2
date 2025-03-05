using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ST_WeeklyTimeSwitchInput
{
    [MarshalAs(UnmanagedType.I1)] public bool bEnable;
    [MarshalAs(UnmanagedType.I1)] public bool bSunday;
    [MarshalAs(UnmanagedType.I1)] public bool bMonday;
    [MarshalAs(UnmanagedType.I1)] public bool bTuesday;
    [MarshalAs(UnmanagedType.I1)] public bool bWednesday;
    [MarshalAs(UnmanagedType.I1)] public bool bThursday;
    [MarshalAs(UnmanagedType.I1)] public bool bFriday;
    [MarshalAs(UnmanagedType.I1)] public bool bSaturday;
}
