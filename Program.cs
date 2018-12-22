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
            var gr = new Graph(fs, true);

            //Console.WriteLine(gr.FindMinPathWithWeights(1,5));

            Console.WriteLine(gr.FindMaxFlow(1,5));
            
            
        }
    }
}