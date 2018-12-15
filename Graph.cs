﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace GraphProj
{
    public class Graph
    {
        private Dictionary<int, Dictionary<int, int>> MainGraph;
        private bool directed;

        public Graph()
        {
            this.MainGraph = new Dictionary<int, Dictionary<int, int>>();
            directed = false;
        }

        public Graph(Stream input, bool directed)
        {
            this.MainGraph = new Dictionary<int, Dictionary<int, int>>();
            this.directed = directed;
            var inputStream = new StreamReader(input);
            while (!inputStream.EndOfStream)
            {
                var line = inputStream.ReadLine();                              //считывание строки
                var str = line.Split(":");                                      //разделение строки на узел и связи
                var key = int.Parse(str[0]);                                    //получение значения узла

                if (str[1].Trim() == "")
                {
                    if(!MainGraph.ContainsKey(key))
                    {
                        MainGraph.Add(key, new Dictionary<int, int>());
                    }
                    continue;
                }
                var kidsStrings = str[1].Trim().Split(" ");                     //получение значений связей в виде 
                                                                                //строк 
                var kids = new Dictionary<int, int>();                          //инициализация экземпляра Dictionary для связей                           
                if (!MainGraph.ContainsKey(key))                                //если в графе не имеется такой узел
                {
                    foreach (var kidsString in kidsStrings)
                    {
                        kids.Add(int.Parse(kidsString.Split("-")[0]), int.Parse(kidsString.Split("-")[1]));
                    }                                                           //заполняем kids связями
                    MainGraph.Add(key, kids);                                   //добавляем в граф
                    this.AddConnection(key, kids);                            
                }                                                              
                else                                                            //если в графе имеется такой узел
                {                                                              
                    foreach (var kidsString in kidsStrings)
                    {
                        kids.Add(int.Parse(kidsString.Split("-")[0]), int.Parse(kidsString.Split("-")[1]));
                    }                                                           //заполняем kids связми

                    foreach (var kid in kids)
                    {
                        MainGraph[key].Add(kid.Key, kid.Value);                 //добавляем kids к существующему узлу
                    }
                    this.AddConnection(key, kids);
                }
            }
        }
         

        public Graph(Graph graph)                                       
        {
            this.MainGraph = new Dictionary<int, Dictionary<int,int>>();
            var initialGraph = graph.GetGraph();
            foreach (var item in initialGraph)
            {
                var list = new Dictionary<int,int>();
                list.Union(item.Value);
                MainGraph.Add(item.Key, list);
            }
        }

        private Dictionary<int, Dictionary<int,int>> GetGraph()
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
        

        public void DeleteConnection(int key, int connectedKey)
        {
            if (MainGraph.ContainsKey(key))
            {
                MainGraph[key].Remove(connectedKey);
                if (directed == false)
                {
                    MainGraph[connectedKey].Remove(key);
                }
            }
            else
            {
                throw new Exception("There's No Such key. DeleteConnection() ERROR");
            }
        }

        public void Add(int key, Dictionary<int,int> kids)
        {
            MainGraph.Add(key, kids);
            this.AddConnection(key, kids);
        }

        private void AddConnection(int key, Dictionary<int,int> kids)
        {
            foreach (var kid in kids)
            {
                if (!MainGraph.ContainsKey(kid.Key) && directed == false)       //если граф не ориентированный,
                                                                                //то все связи работают в обе стороны
                                                                               //и нужный ключ НЕ имеется в графе
                {
                    var temp = new Dictionary<int, int>();
                    temp.Add(key, kid.Value);
                    MainGraph.Add(kid.Key, temp);
                }
                else if (MainGraph.ContainsKey(kid.Key) && directed == false)    //елси граф не ориентированный и нужный ключ имеется в графе
                {
                    MainGraph[kid.Key].Add(key, kid.Value);
                }
                else if (!MainGraph.ContainsKey(kid.Key) && directed == true)
                {
                    MainGraph.Add(kid.Key, new Dictionary<int, int>());
                }
            }
        }

        public int GetNodeDegree(int key)
        {
            if (MainGraph.ContainsKey(key))
            {
                return MainGraph[key].Count;
            }
            else
            {
                throw new Exception("There's No Such key. GetNodeDegree() ERROR");
            }
        }

        public HashSet<int> GetNotConnected(int inputKey)
        {
            var ret = new HashSet<int>();
            var keyKids = MainGraph[inputKey];
            foreach (var item in MainGraph)
            {
                if (!keyKids.ContainsKey(item.Key) && item.Key != inputKey && !item.Value.ContainsKey(inputKey))
                {
                    ret.Add(item.Key);
                }
            }
            return ret;
        }

        public void Delete(int key)
        {
            if (MainGraph.ContainsKey(key))
            {
                MainGraph.Remove(key);
                foreach (var item in MainGraph)
                {
                    item.Value.Remove(key);
                }
            }
            else
            {
                throw new Exception("There's No Such key. Delete() ERROR");
            }
            
        }

        public static bool operator ==(Graph a, Graph b)
        {
            if (a.GetGraph().Keys.Count != b.GetGraph().Keys.Count)
            {
                return false;
            }
            else
            {
                var aSorted = a.GetGraph().OrderBy(item => item.Value.Count).ToList();
                var bSorted = b.GetGraph().OrderBy(item => item.Value.Count).ToList();
                
                for (int i = 0; i < a.GetGraph().Keys.Count; i++)
                {
                    if (aSorted[i].Value.Count != bSorted[i].Value.Count)
                        return false;
                }
                return true;
            }
        }
        public static bool operator !=(Graph a, Graph b)
        {
            if (a.GetGraph().Keys.Count != b.GetGraph().Keys.Count)
            {
                return true;
            }
            else
            {
                var aSorted = a.GetGraph().OrderBy(item => item.Value.Count).ToList();
                var bSorted = b.GetGraph().OrderBy(item => item.Value.Count).ToList();
                
                for (int i = 0; i < a.GetGraph().Keys.Count; i++)
                {
                    if (aSorted[i].Value.Count != bSorted[i].Value.Count)
                        return true;
                }
                return false;
            }
        }

        public List<KeyValuePair<int, int>> FindMaxPaths(int key)
        {
            if (!MainGraph.ContainsKey(key))
                throw new Exception("There's no such key. FindMaxPaths() ERROR");

            var returnValue = new List<KeyValuePair<int,int>>();

            var visited = new Dictionary<int,bool>();
            foreach (var graphKey in MainGraph.Keys)
            {
                visited.Add(graphKey, false);
            }
            
            foreach (var graphKey in MainGraph.Keys)
            {
                if (graphKey == key)
                    continue;

                int pathCount = 0;
                findMaxPath(key, graphKey, visited, ref pathCount);
                returnValue.Add(new KeyValuePair<int, int>(graphKey, pathCount));
            }

            return returnValue;
            
            void findMaxPath(int startKey, int endKey, Dictionary<int,bool> Visited, ref int pathCount)
            {
                Visited[startKey] = true;

                if (startKey == endKey)
                {
                    pathCount++;
                }
                else
                {
                    foreach (var temp in MainGraph[startKey])
                    {
                        if (!Visited[temp.Key])
                        {
                            findMaxPath(temp.Key, endKey, Visited, ref pathCount);
                        }
                    }
                }

                Visited[startKey] = false;
            }
        }
        
    public int FindMinPathCount(int startKey, int endKey)
        {
            if (!MainGraph.ContainsKey(startKey) || !MainGraph.ContainsKey(endKey))
                throw new Exception("There's no such key. FindMinPathCount() ERROR");


            var distTo = new Dictionary<int,int>();

            foreach (var value in MainGraph)
            {
                distTo.Add(value.Key, -1);
            }
            
            
            return Bfs(startKey);

            int Bfs(int s)
            {
                var minPath = 0;
                var pathCount = 0;
                
                var queue = new Queue<int>();
                queue.Enqueue(s);
                distTo[s] = 0;

                while (queue.Count != 0)
                {
                    var v = queue.Dequeue();

                    foreach (var w in MainGraph[v])
                    {
                        if (distTo[w.Key] == -1)
                        {
                            if (minPath == 0 || distTo[v] < minPath)
                            {
                                queue.Enqueue(w.Key);       
                            }
                            
                            pathCount++;
                            distTo[w.Key] = distTo[v] + 1;
                            
                            if (w.Key == endKey)
                            {
                                if (minPath == 0)
                                {
                                    minPath = distTo[endKey];
                                }
                                else if(minPath > distTo[endKey])
                                {
                                    minPath = distTo[endKey];
                                }   
                            }
                        }
                    }
                }

                return minPath;
            }
        }
    public int FindMinPathWithWeights(int startKey, int endKey)
        {
            if (!MainGraph.ContainsKey(startKey) || !MainGraph.ContainsKey(endKey))
                throw new Exception("There's no such key. FindMinPathCount() ERROR");


            var distTo = new Dictionary<int, int>();

            foreach (var value in MainGraph)
            {
                distTo.Add(value.Key, int.MaxValue);
            }

            var visited = new Dictionary<int, bool>();
            foreach (var graphKey in MainGraph.Keys)
            {
                visited.Add(graphKey, false);
            }

            var minPath = 0;

            var queue = new Queue<int>();
            queue.Enqueue(startKey);
            distTo[startKey] = 0;

            while (queue.Count != 0)
            {
                var v = queue.Dequeue();

                if (visited[v])
                {
                    continue;
                }

                foreach (var node in MainGraph[v])
                {
                    if (distTo[node.Key] > node.Value + distTo[v])
                    {

                        distTo[node.Key] = node.Value + distTo[v];
                    }

                    queue.Enqueue(node.Key);

                }

                visited[v] = true;
            }

            return distTo[endKey];

        }
    }
}
