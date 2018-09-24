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

            Console.WriteLine();
            Console.Write("Enter the key: ");
            var key = int.Parse(Console.ReadLine());
            try
            {
                Console.WriteLine("Degree of this key = " + gr.GetNodeDegree(key));
                Console.WriteLine("Not Connected Nodes: ");
                gr.GetNotConnectedNodes(key).ForEach(t => Console.Write(t + " "));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}