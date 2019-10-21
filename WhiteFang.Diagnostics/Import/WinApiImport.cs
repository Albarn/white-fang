using System;
using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace WhiteFang.Diagnostics.Import
{
    internal class WinApiImport
    {
        public const uint QueryInformationThreadAccess = 0x0040;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryPerformanceCounter(out long perfomanceCount);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryPerformanceFrequency(out long frequency);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessTimes(IntPtr proc, out FILETIME creationTime, out FILETIME exitTime, out FILETIME kernelTime, out FILETIME userTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetThreadTimes(IntPtr thread, out FILETIME creationTime, out FILETIME exitTime, out FILETIME kernelTime, out FILETIME userTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(uint desiredAccess, bool inheritHandle, uint threadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void GetSystemTime(out SYSTEMTIME sysTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void GetSystemTimeAsFileTime(out FILETIME sysTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetTickCount();

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern int timeGetTime();

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern int timeGetSystemTime(out MMTIME time, ref uint size);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetLastError();
    }
}
