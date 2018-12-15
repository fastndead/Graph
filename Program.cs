using System;
using System.Collections.Generic;
using System.IO;

namespace GraphProj
{
    class Program
    {
        static void Main(string[] args)
        {
            var fs = new FileStream("input.txt", FileMode.Open);
            var gr = new Graph(fs, false);

            Console.WriteLine(gr.FindMinPathWithWeights(1,5));
            
        }
    }
}