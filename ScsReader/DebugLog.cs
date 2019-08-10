using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader
{
    public class DebugOutput
    {
        public static bool Active { get; set; } = false;

        internal static void WriteLine(string str)
        {
            if(Active) Console.WriteLine(str);
        }
    }
}
