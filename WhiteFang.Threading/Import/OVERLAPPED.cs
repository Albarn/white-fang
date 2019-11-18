using System;

namespace WhiteFang.Threading.Import
{
    internal struct OVERLAPPED
    {
        public ulong Internal;
        public ulong InternalHigh;
        public long DUMMYUNIONNAME;
        public IntPtr hEvent;
    }
}
