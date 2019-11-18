using System;
using System.IO;
using WhiteFang.Threading;

namespace WhiteFang.Reader
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                return -1;
            }
            
            var file = args[0];
            var slot = args[1];

            using (var reader = new StreamReader(file))
            {
                try
                {
                    while (!reader.EndOfStream)
                    {
                        if (!MailSlotService.Write(slot, reader.ReadLine()))
                        {
                            return -1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    Console.Error.WriteLine(ex.StackTrace);
                    return -1;
                }
            }

            return 0;
        }
    }
}
