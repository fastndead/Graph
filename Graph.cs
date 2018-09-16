using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;

namespace GraphProj
{
    public class Graph
    {
        private Dictionary<int, List<int>> MainGraph = new Dictionary<int, List<int>>();

        public Graph()
        {
            Dictionary<int, List<int>> MainGraph = new Dictionary<int, List<int>>();
        }

        public Graph(FileStream input)
        {
            StreamReader inputStream = new StreamReader(input);
            while (!inputStream.EndOfStream)
            {
                string line = inputStream.ReadLine();
                var str = line.Split(":");
                var key = int.Parse(str[0]);
                var kidsStrings = str[1].Trim().Split(" ");
                var kids = new List<int>();
                if (!MainGraph.ContainsKey(key))
                {
                    try
                    {
                        foreach (var t in kidsStrings)
                        {
                            kids.Add(int.Parse(t));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: " + e);
                        return;
                    }
                    
                    MainGraph.Add(key, kids);
                }
                else
                {
                    try
                    {
                        foreach (var t in kidsStrings)
                        {
                            kids.Add(int.Parse(t));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: " + e);
                        return;
                    }
                    MainGraph[key].AddRange(kids);
                }
            }
            
        }

        public Graph(Graph graph)
        {
            this.MainGraph = new Dictionary<int, List<int>>();
            var initialGraph = graph.GetGraph();
            foreach (var item in initialGraph)
            {
                var list = new List<int>();
                list.AddRange(item.Value);
                MainGraph.Add(item.Key, list);
            }
        }

        private Dictionary<int, List<int>> GetGraph()
        {
            return MainGraph;
        }
        
        public void Print()
        {
            foreach (var variable in MainGraph)
            {
                Console.Write("KEY: {0}, VALUE: ", variable.Key);
                foreach (var elem in variable.Value)
                {
                    Console.Write(elem + ", ");
                }
                Console.WriteLine();
            }
        }

        public void AddConnection(int key, params int[] kids)
        {
            if (MainGraph.ContainsKey(key))
            {
                MainGraph[key].AddRange(kids);
            }
            else
            {
                throw new Exception("There's No Such key. AddConnection() ERROR");
            }
        }

        public void DeleteConnection(int key, int connectedKey)
        {
            if (MainGraph.ContainsKey(key))
            {
                MainGraph[key].Add(connectedKey);
            }
            else
            {
                throw new Exception("There's No Such key. DeleteConnection() ERROR");
            }
        }

        public void Add(int key, List<int> kids)
        {
            MainGraph.Add(key, kids);
        }

        public void Delete(int key)
        {
            if (MainGraph.ContainsKey(key))
            {
                MainGraph.Remove(key);
            }
            else
            {
                throw new Exception("There's No Such key. Delete() ERROR");
            }
            
        }
        
    }
}
