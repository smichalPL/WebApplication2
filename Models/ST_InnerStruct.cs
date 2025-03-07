using System.Runtime.InteropServices;

namespace WebApplication2.Models
{
   // [StructLayout(LayoutKind.Sequential)]
    public struct ST_InnerStruct
    {
       // [MarshalAs(UnmanagedType.U1)]
        public bool bBoolTest1;
        //[MarshalAs(UnmanagedType.U1)]
        public bool bBoolTest2;
        public uint Czas; // 4 bajty reprezentujące TIME_OF_DAY

        public TimeSpan CzasTimeSpan => TimeSpan.FromMilliseconds(Czas);
    }
}
