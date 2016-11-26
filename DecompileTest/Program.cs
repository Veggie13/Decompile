using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decompile;
using System.IO;

namespace DecompileTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Exe exe = Exe.Load(@"..\..\..\Release\Test.exe");
            //var data = Exe.ReadData(header, content);
            Console.ReadLine();
        }
    }
}
