using System;

namespace WhiteFang.Threading.Import
{
    internal struct LPSECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }
}
