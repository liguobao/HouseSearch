using System;
using System.Collections.Generic;
using System.Linq;

namespace JiebaNet.Analyser
{
    public class Edge
    {
        public string Start { get; set; }
        public string End { get; set; }
        public double Weight { get; set; }
    }

    public class UndirectWeightedGraph
    {
        private static readonly double d = 0.85;

        public IDictionary<string, List<Edge>> Graph { get; set; } 
        public UndirectWeightedGraph()
        {
            Graph = new Dictionary<string, List<Edge>>();
        }

        public void AddEdge(string start, string end, double weight)
        {
            if (!Graph.ContainsKey(start))
            {
                Graph[start] = new List<Edge>();
            }

            if (!Graph.ContainsKey(end))
            {
                Graph[end] = new List<Edge>();
            }

            Graph[start].Add(new Edge(){ Start = start, End = end, Weight = weight });
            Graph[end].Add(new Edge(){ Start = end, End = start, Weight = weight });
        }

        public IDictionary<string, double> Rank()
        {
            var ws = new Dictionary<string, double>();
            var outSum = new Dictionary<string, double>();

            // init scores
            var count = Graph.Count > 0 ? Graph.Count : 1;
            var wsdef = 1.0/count;

            foreach (var pair in Graph)
            {
                ws[pair.Key] = wsdef;
                outSum[pair.Key] = pair.Value.Sum(e => e.Weight);
            }

            // TODO: 10 iterations?
            var sortedKeys = Graph.Keys.OrderBy(k => k);
            for (var i = 0; i < 10; i++)
            {
                foreach (var n in sortedKeys)
                {
                    var s = 0d;
                    foreach (var edge in Graph[n])
                    {
                        s += edge.Weight/outSum[edge.End]*ws[edge.End];
                    }
                    ws[n] = (1 - d) + d*s;
                }
            }

            var minRank = double.MaxValue;
            var maxRank = double.MinValue;

            foreach (var w in ws.Values)
            {
                if (w < minRank)
                {
                    minRank = w;
                }
                if(w > maxRank)
                {
                    maxRank = w;
                }
            }

            foreach (var pair in ws.ToList())
            {
                ws[pair.Key] = (pair.Value - minRank/10.0)/(maxRank - minRank/10.0);
            }

            return ws;
        }
    }
}