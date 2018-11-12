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
            
            //            var fs1 = new FileStream("input1.txt", FileMode.Open);
            //            var gr1 = new Graph(fs1, false);
            //            
            //          
            //            Console.WriteLine();
            //            Console.Write("Enter the key: ");
            //            var key = int.Parse(Console.ReadLine());
            //            
            //            Console.WriteLine("Degree of this key = " + gr.GetNodeDegree(key));
            //
            //            Console.Write("Not connected nodes : ");
            //            Console.WriteLine(string.Join(" ", gr.GetNotConnected(key)));
            //            
            //            gr.DeleteConnection(7,5);
            //            gr1.DeleteConnection(2,3);
            //            
            //            ВЫВОД
            //            Console.WriteLine("FIRST");
            //            gr.Print();
            //            Console.WriteLine("SECOND");
            //            gr1.Print();
            //            
            //            
            //            if (gr == gr1)
            //            {
            //                Console.WriteLine("РАВНЫ!");
            //            }
            //            else
            //            {
            //                Console.WriteLine("НЕ РАВНЫ");
            //            }

            var maxPaths = gr.FindMaxPaths(1);

            foreach (var node in maxPaths)
            {
                Console.WriteLine("{0} - {1}", node.Key, node.Value);
            }
        }
    }
}