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

            Console.Write("Enter start Key: "); int startKey = int.Parse(Console.ReadLine());
            Console.Write("Enter end Key: "); int endKey = int.Parse(Console.ReadLine());
            Console.WriteLine(gr.FindMinPathCount(startKey,endKey));
            

           // Console.WriteLine("Min Path between {0} and {1} = {2}",startKey,endKey,minPath);
        }
    }
}