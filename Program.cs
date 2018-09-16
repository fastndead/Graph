using System;
using System.Collections.Generic;
using System.IO;

namespace GraphProj
{
    class Program
    {
        static void Main(string[] args)
        {
            var fs = new FileStream("input.txt", FileMode.OpenOrCreate);
            var gr = new Graph(fs); 
            gr.Print();
        }
    }
}