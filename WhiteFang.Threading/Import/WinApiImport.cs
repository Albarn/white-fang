using System;
using System.Runtime.InteropServices;

namespace WhiteFang.Threading.Import
{
    internal class WinApiImport
    {
        public const int SemaphoreAllAccess = 0x1F0003;
        public const int GenericWriteAccess = 1073741824;
        public const int FileShareRead = 0x00000001;
        public const int FileAttributeNormal = 0x80;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateSemaphore(ref LPSECURITY_ATTRIBUTES options, long initCnt, long maxCnt, string name);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReleaseSemaphore(IntPtr semaphore, long increment, IntPtr previousCnt);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WaitForSingleObject(IntPtr handle, int milliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateMailslot(string name, int capacity, int timeOut, LPSECURITY_ATTRIBUTES options);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(string name, int access, int shareMode, LPSECURITY_ATTRIBUTES options, int creationDisposition, int attributes, IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr WriteFile(IntPtr file, string buffer, int bytesNumber, out int writtenBytesNumber, OVERLAPPED overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateEvent(LPSECURITY_ATTRIBUTES options, bool manual, bool initialState, string name);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr ReadFile(IntPtr file, IntPtr buffer, int bytesNumber, out int readBytesNumber, ref OVERLAPPED overlapped);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetMailslotInfo(IntPtr slot, int size, out int nextSize, out int messageCnt, int timeOut);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();
    }
}
