using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace WhiteFang.Diagnostics.Import
{
    internal class WinApiImport
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryPerformanceCounter(out long perfomanceCount);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryPerformanceFrequency(out long frequency);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void GetSystemTimeAsFileTime(out FILETIME sysTime);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();
    }
}
