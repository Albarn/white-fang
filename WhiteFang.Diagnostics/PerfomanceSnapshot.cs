using Newtonsoft.Json;
using WhiteFang.Diagnostics.Import;

namespace WhiteFang.Diagnostics
{
    public class PerfomanceSnapshot
    {
        public long PerfomanceCount;

        public long Frequency;

        public ulong SystemFileAsFileTime;

        public void Update()
        {
            WinApiImport.QueryPerformanceCounter(out PerfomanceCount);
            WinApiImport.QueryPerformanceFrequency(out Frequency);
            WinApiImport.GetSystemTimeAsFileTime(out var fileTime);
            SystemFileAsFileTime = (ulong)fileTime.dwLowDateTime << 32 + fileTime.dwHighDateTime;
        }

        public PerfomanceSnapshot Difference(PerfomanceSnapshot snapshot)
        {
            var res = new PerfomanceSnapshot
            {
                PerfomanceCount = PerfomanceCount - snapshot.PerfomanceCount,
                Frequency = Frequency - snapshot.Frequency,
                SystemFileAsFileTime = SystemFileAsFileTime - snapshot.SystemFileAsFileTime
            };

            return res;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
