using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ST_WeeklyTimeSwitchInputArray
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public ST_WeeklyTimeSwitchInput[] Inputs;
}
