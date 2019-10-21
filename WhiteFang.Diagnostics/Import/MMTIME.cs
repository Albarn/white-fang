using System.Runtime.InteropServices;

namespace WhiteFang.Diagnostics.Import
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MMTIME
    {
        public uint wType;
        public midi u;
    }
}
