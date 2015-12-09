using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var filebytes = File.ReadAllBytes(@"C:\Users\marcel\Documents\Downloads\FundB2015.p12");
            var base64 = Convert.ToBase64String(filebytes);
        }
    }
}
