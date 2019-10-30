using System;
using System.Runtime.InteropServices;

namespace WhiteFang.Threading.Import
{
    internal class WinApiImport
    {
        public const uint SemaphoreAllAccess = 0x1F0003;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateSemaphore(ref LPSECURITY_ATTRIBUTES options, long initCnt, long maxCnt, string name);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReleaseSemaphore(IntPtr semaphore, long increment, IntPtr previousCnt);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WaitForSingleObject(IntPtr handle, int milliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetLastError();
    }
}
