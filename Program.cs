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
            Console.WriteLine("FIRST");
            gr.Print();
            
            Console.WriteLine("SECOND");
            var fs1 = new FileStream("input1.txt", FileMode.Open);
            var gr1 = new Graph(fs1, false);
            gr1.Print();
          
//            Console.WriteLine();
//            Console.Write("Enter the key: ");
//            var key = int.Parse(Console.ReadLine());
//            
//            Console.WriteLine("Degree of this key = " + gr.GetNodeDegree(key));
//
//            Console.Write("Not connected nodes : ");
//            Console.WriteLine(string.Join(" ", gr.GetNotConnected(key)));

            if (gr == gr1)
            {
                Console.WriteLine("РАВНЫ!");
            }
            else
            {
                Console.WriteLine("НЕ РАВНЫ");
            }
        }
    }
}