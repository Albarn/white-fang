using System;
using WhiteFang.Threading.Import;

namespace WhiteFang.Threading
{
    public class SemaphoreService
    {
        public static IntPtr Create(string name, int limit)
        {
#if DEBUG
            return IntPtr.Zero;
#endif

            var options = new LPSECURITY_ATTRIBUTES();
            var semaphore = WinApiImport.CreateSemaphore(ref options, limit, limit, name);

            return semaphore;
        }

        public static void Close(IntPtr semaphore)
        {
#if !DEBUG
            WinApiImport.CloseHandle(semaphore);
#endif
        }

        public static void Wait(IntPtr semaphore)
        {
#if DEBUG
            return;
#endif

            while(WinApiImport.WaitForSingleObject(semaphore, int.MaxValue) != 0)
            {

            }
        }

        public static void Release(IntPtr semaphore)
        {
#if !DEBUG
            WinApiImport.ReleaseSemaphore(semaphore, 1, IntPtr.Zero);
#endif
        }
    }
}
