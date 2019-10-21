using System.Runtime.InteropServices;

namespace WhiteFang.Diagnostics.Import
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct midi
    {
        public int songptrpos;
    }
}
