using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using WhiteFang.Threading.Import;

namespace WhiteFang.Threading
{
    public static class MailSlotService
    {
        private const string SlotPrefix = "\\\\.\\mailslot\\";

        private static readonly IntPtr invalidHandle;
        private static readonly Dictionary<string, IntPtr> slots;
        private static readonly Dictionary<string, IntPtr> files;

        static MailSlotService()
        {
            invalidHandle = new IntPtr(-1);
            slots = new Dictionary<string, IntPtr>();
            files = new Dictionary<string, IntPtr>();
        }

        public static bool Create(string name)
        {
            var slot = WinApiImport.CreateMailslot(SlotPrefix + name, 0, int.MaxValue, new LPSECURITY_ATTRIBUTES());

            if(slot == invalidHandle)
            {
                return false;
            }

            slots[name] = slot;
            return true;
        }

        public static bool Write(string name, string message)
        {
            var file = GetFile(name);
            if(file == invalidHandle)
            {
                return false;
            }

            var size = (message.Length + 1) * sizeof(char);
            var res = WinApiImport.WriteFile(file, message, size, out var written, new OVERLAPPED());
            if(res == IntPtr.Zero)
            {
                return false;
            }
            return true;
        }

        public static string Read(string name)
        {
            var hEvent = WinApiImport.CreateEvent(new LPSECURITY_ATTRIBUTES(), false, false, name);
            if(hEvent == IntPtr.Zero)
            {
                return string.Empty;
            }

            var ov = new OVERLAPPED { hEvent = hEvent };
            var info = WinApiImport.GetMailslotInfo(slots[name], 0, out var size, out var _, 0);
            if (info == IntPtr.Zero || size <= 0)
            {
                return string.Empty;
            }

            var msgPointer = Marshal.AllocHGlobal(size);
            var res = WinApiImport.ReadFile(slots[name], msgPointer, size, out var read, ref ov);
            if(res == IntPtr.Zero)
            {
                return string.Empty;
            }

            var message = Marshal.PtrToStringAnsi(msgPointer);
            return message;
        }

        public static void Close(string name)
        {
            var slot = slots[name];
            WinApiImport.CloseHandle(slot);
            slots.Remove(name);
            files.Remove(name);
        }

        private static IntPtr GetFile(string name)
        {
            if (files.ContainsKey(name))
            {
                return files[name];
            }

            var file = WinApiImport.CreateFile(
                SlotPrefix + name,
                WinApiImport.GenericWriteAccess,
                WinApiImport.FileShareRead,
                new LPSECURITY_ATTRIBUTES(),
                (int)FileMode.Open,
                WinApiImport.FileAttributeNormal,
                IntPtr.Zero);

            if(file != invalidHandle)
            {
                files[name] = file;
            }

            return file;
        }
    }
}
