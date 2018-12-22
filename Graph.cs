﻿using System;
 using System.Collections;
 using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
 using System.Data;
 using System.Diagnostics.Contracts;
 using System.IO;
using System.Linq;
 using System.Net;
 using System.Net.Http.Headers;
 using System.Runtime.InteropServices.WindowsRuntime;

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
                        kids.Add(int.Parse(kidsString.Split(">")[0]), int.Parse(kidsString.Split(">")[1]));
                    }                                                           //заполняем kids связями
                    MainGraph.Add(key, kids);                                   //добавляем в граф
                    this.AddConnection(key, kids);                            
                }                                                              
                else                                                            //если в графе имеется такой узел
                {                                                              
                    foreach (var kidsString in kidsStrings)
                    {
                        kids.Add(int.Parse(kidsString.Split(">")[0]), int.Parse(kidsString.Split(">")[1]));
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


            var distTo = new Dictionary<int, int>();

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
                                else if (minPath > distTo[endKey])
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

        public Dictionary<int,List<KeyValuePair<int,int>>> FindMinPathsWithWeights()
        {

            var retVal = new Dictionary<int, List<KeyValuePair<int,int>>>();
            foreach (var node in MainGraph)
            {
                var retList = new List<KeyValuePair<int,int>>();
                foreach (var anotherNode in MainGraph)
                {
                    if (anotherNode.Key == node.Key)
                    {
                        continue;
                    }
                    retList.Add(new KeyValuePair<int, int>(anotherNode.Key, FindMinPathWithWeights(node.Key,anotherNode.Key)));
                }
                retVal.Add(node.Key,retList);
            }

            return retVal;
        }
        
        public void FindMinPathsWithWeightsPrint()
        {

            var retVal = new Dictionary<int, List<KeyValuePair<int,int>>>();
            foreach (var node in MainGraph)
            {
                var retList = new List<KeyValuePair<int,int>>();
                Console.WriteLine("Min paths for {0}", node.Key);
                foreach (var anotherNode in MainGraph)
                {
                    if (anotherNode.Key == node.Key)
                    {
                        continue;
                    }

                    Console.WriteLine(node.Key+ " → " + anotherNode.Key + " = "+ FindMinPathWithWeights(node.Key,anotherNode.Key));
                }
                retVal.Add(node.Key,retList);
            }
        }

        public int FindMinPathsWithWeightsFloid(int startKey, int endKey)
        {
            var A = new List<List<KeyValuePair<int,int>>>();
            foreach (var node in MainGraph)
            {
                var temp = new List<KeyValuePair<int,int>>();
                foreach (var conn in MainGraph)
                {
                    if (conn.Key != node.Key)
                    {
                        temp.Add(new KeyValuePair<int, int>(conn.Key, conn.Value.ContainsKey(node.Key) ? conn.Value[node.Key] : int.MaxValue));
                    }
                    else
                    {
                        temp.Add(new KeyValuePair<int, int>(conn.Key,0));
                    }

                }
                A.Add(temp);
            }

            for (int k = 0; k < MainGraph.Count; k++)
            {
                for (int i = 0; i < MainGraph.Count; i++)
                {
                    for (int j = 0; j < MainGraph.Count; j++)
                    {
                        if (A[k][i].Value != int.MaxValue && A[j][k].Value != int.MaxValue)
                        {
                            if (A[k][i].Value + A[j][k].Value < A[j][i].Value)
                            {
                                A[j][i] = new KeyValuePair<int, int>(A[j][i].Key, A[k][i].Value + A[j][k].Value);
                                    
                            }
                        }

                    }
                }
            }

            var indexOfStartKey = 0;
            foreach (var node in MainGraph)
            {
                if (node.Key == startKey)
                    break;
                indexOfStartKey++;
            }

            foreach (var row in A[indexOfStartKey])
            {
                if (row.Key == endKey)
                {
                    if (row.Value == int.MaxValue)
                    {
                        throw new Exception("There's no connection between nodes " + startKey + " and " + endKey);
                    }
                    return row.Value;
                }
            }
            
            throw new Exception("There's no such keys apparently");

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


        public int FindMaxFlow(int source, int sink)
        {
            var graphWithFlows = new Dictionary<int,Dictionary<int,KeyValuePair<int,int>>>();

            foreach (var node in MainGraph)
            {
                var tempConns = new Dictionary<int, KeyValuePair<int, int>>();
                foreach (var conn in node.Value)
                {
                    var tempFlows = new KeyValuePair<int, int>(conn.Value, 0);
                    tempConns[conn.Key] = tempFlows;
                }
                graphWithFlows[node.Key] = tempConns;
            }

            var maxFlowStack = new Stack<int>();
            var maxFlow = 0;

            while (true)
            {
                bool foundAnyPath = false;
                var changesStack = new Queue<KeyValuePair<List<int>, int>>();
                foreach (var conn in graphWithFlows[source])
                {
                    if (conn.Value.Key == 0)
                    {
                        continue;
                    }
                    else
                    {
                        foundAnyPath = true;
                        var dfs = DfsWithFlows(graphWithFlows, conn.Key);
                        var tempKey = source;
                        var minFlow = int.MaxValue;
                        foreach (var edge in dfs)
                        {
                            if (graphWithFlows[tempKey][edge].Key < minFlow)
                            {
                                minFlow = graphWithFlows[tempKey][edge].Key;
                            }

                            tempKey = edge;
                        }
                        
                        changesStack.Enqueue(new KeyValuePair<List<int>, int>(dfs,minFlow));
                        maxFlowStack.Push(minFlow);
                        break;
                    }
                }

                if (changesStack.Count > 0)
                {
                    var tempKey1 = source;
                    var changes = changesStack.Dequeue();
                    foreach (var edge in changes.Key)
                    {
                        var keyValuePair = new KeyValuePair<int, int>(graphWithFlows[tempKey1][edge].Key - changes.Value, graphWithFlows[tempKey1][edge].Value + changes.Value);
                        graphWithFlows[tempKey1][edge] = keyValuePair;
                        tempKey1 = edge;
                    }
                }

                if (!foundAnyPath)
                {
                    break;
                }
            }
            while (maxFlowStack.Count > 0)
            {
                maxFlow += maxFlowStack.Pop();
            }


            List<int> DfsWithFlows(Dictionary<int,Dictionary<int,KeyValuePair<int,int>>> graph, int conn)
            {
                
                var returnValue = new List<int>();

                var visited = new Dictionary<int,bool>();
                foreach (var graphKey in graph.Keys)
                {
                    visited.Add(graphKey, false);
                }
            
                foreach (var con in graph[source])
                {
                    if (con.Key != conn)
                        continue;

                    returnValue.Add(conn);
                    FindMaxPath(con.Key, sink, visited);
                }

                return returnValue;
            
                void FindMaxPath(int startKey, int endKey, Dictionary<int,bool> Visited)
                {
                    Visited[startKey] = true;

                    if (startKey == endKey)
                    {
                        return;
                    }
                    else
                    {
                        bool goBack = false;
                        foreach (var temp in graph[startKey])
                        {
                            goBack = true;
                            if (!Visited[temp.Key] && temp.Value.Key != 0)
                            {
                                goBack = false;
                                returnValue.Add(temp.Key);
                                FindMaxPath(temp.Key, endKey, Visited);
                                return;
                            }
                        }

                        if (goBack)
                        {
                            returnValue.Add(-graph[startKey][endKey].Key);
                        }
                    }
                    Visited[startKey] = false;
                }
            }

            return maxFlow;
        }
        
        
        public void Prims()
        {
            if (directed)
            {
                throw new Exception("Graph must not be directed!");
            }

            var retVal = new List<KeyValuePair<int, int>>();

            var visited = new Dictionary<int,bool>();
            foreach (var graphKey in MainGraph.Keys)
            {
                visited.Add(graphKey, false);
            }
            
            List<int> current = new List<int>{MainGraph.Keys.ToList()[0]};
            visited[MainGraph.Keys.ToList()[0]] = true;
            for (int i = 0; i < MainGraph.Count-1; i++)
            {
                var minWeight = int.MaxValue;
                var minNode = int.MaxValue;
                var minSource = int.MaxValue;
                foreach (var node in current)
                {
                    foreach (var con in MainGraph[node])
                    {
                        if (visited[con.Key])
                        {
                            continue;
                        }
                        if (con.Value < minWeight)
                        {
                            minNode = con.Key;
                            minWeight = con.Value;
                            minSource = node;
                        }
                    }
                }   
                visited[minNode] = true;
                retVal.Add(new KeyValuePair<int, int>(minSource, minNode));
                current.Add(minNode);
            }
            
            Console.WriteLine("Spanning tree:");
            foreach (var pathPart in retVal)
            {
                Console.WriteLine(pathPart.Key + " - " + pathPart.Value);
            }
        }
    }
}
