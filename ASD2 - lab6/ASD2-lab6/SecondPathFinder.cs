using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Pathfinder
{
    public static class Lab06GraphExtender
    {

        /// <summary>
        /// Algorytm znajdujący drugą pod względem długości najkrótszą ścieżkę między a i b.
        /// Możliwe, że jej długość jest równa najkrótszej (jeśli są dwie najkrótsze ścieżki,
        /// algorytm zwróci jedną z nich).
        /// Dopuszczamy, aby na ścieżce powtarzały się wierzchołki/krawędzie.
        /// Można założyć, że a!=b oraz że w grafie nie występują pętle.
        /// </summary>
        /// <remarks>
        /// Wymagana złożoność do O(D), gdzie D jest złożonością implementacji alogorytmu Dijkstry w bibliotece Graph.
        /// </remarks>
        /// <param name="g"></param>
        /// <param name="path">null jeśli druga ścieżka nie istnieje, wpp ściezka jako ciąg krawędzi</param>
        /// <returns>null jeśli druga ścieżka nie istnieje, wpp długość znalezionej ścieżki</returns>
        public static double? FindSecondShortestPath(this Graph g, int a, int b, out Edge[] path)
        {

            PathsInfo[] pi;
            PathsInfo[] pi2;
            Graph G = g.Clone();
            G.DijkstraShortestPaths(a, out pi);

            if (double.IsNaN(pi[b].Dist))
            {
                path = null;
                return null;
            }

            Edge[] firstPath = PathsInfo.ConstructPath(a, b, pi);

            List<Edge> finalPath = new List<Edge>();

            if (G.Directed)
                G = G.Reverse();

            G.DijkstraShortestPaths(b, out pi2);

            if (pi2 == null)
            {
                path = null;
                return null;
            }

            int u = -1, v = -1;

            double secondShortest = double.PositiveInfinity;

            for (int i = firstPath.Length - 1; i >= 0; i--)
            {
                foreach (Edge ed in g.OutEdges(firstPath[i].From))
                {
                    if (ed.To != firstPath[i].To)
                    {
                        double current;

                        if (double.IsNaN(pi2[ed.To].Dist))
                            continue;

                        if ((current = (ed.Weight + pi[firstPath[i].From].Dist + pi2[ed.To].Dist)) < secondShortest)
                        {
                            secondShortest = current;
                            u = firstPath[i].From;
                            v = ed.To;
                        }
                    }
                }
            }

            if (u == -1)
            {
                path = null;
                return null;
            }

            int ind = 0;
            while (firstPath[ind].From != u)
                finalPath.Add(firstPath[ind++]);

            Edge[] second = PathsInfo.ConstructPath(b, v, pi2);

            finalPath.Add(new Edge(u, v, g.GetEdgeWeight(u, v)));
            ind = v;
            Edge? last;
            while ((last = pi2[ind].Last) != null)
            {
                Edge e = pi2[ind].Last.Value;
                finalPath.Add(new Edge(e.To, e.From, e.Weight));
                ind = last.Value.From;
            }
            path = finalPath.ToArray();
            return secondShortest;
        }

        /// <summary>
        /// Algorytm znajdujący drugą pod względem długości najkrótszą ścieżkę między a i b.
        /// Możliwe, że jej długość jest równa najkrótszej (jeśli są dwie najkrótsze ścieżki,
        /// algorytm zwróci jedną z nich).
        /// Wymagamy, aby na ścieżce nie było powtórzeń wierzchołków ani krawędzi.  
        /// Można założyć, że a!=b oraz że w grafie nie występują pętle.
        /// </summary>
        /// <remarks>
        /// Wymagana złożoność to O(nD), gdzie D jest złożonością implementacji algorytmu Dijkstry w bibliotece Graph.
        /// </remarks>
        /// <param name="g"></param>
        /// <param name="path">null jeśli druga ścieżka nie istnieje, wpp ściezka jako ciąg krawędzi</param>
        /// <returns>null jeśli druga ścieżka nie istnieje, wpp długość tej ścieżki</returns>
        public static double? FindSecondSimpleShortestPath(this Graph g, int a, int b, out Edge[] path)
        {

            PathsInfo[] pi;
            PathsInfo[] pi2;
            Graph G = g.Clone();
            G.DijkstraShortestPaths(a, out pi);

            if (double.IsNaN(pi[b].Dist))
            {
                path = null;
                return null;
            }

            Edge[] firstPath = PathsInfo.ConstructPath(a, b, pi);
            
            double secondShortest = double.PositiveInfinity;

            path = null;

            for (int i = firstPath.Length - 1; i >= 0; i--)
            {
                G.DelEdge(firstPath[i]);
                
                G.DijkstraShortestPaths(a, out pi2);
                if (double.IsNaN(pi2[b].Dist))
                    continue;

                if (pi2[b].Dist < secondShortest)
                {
                    secondShortest = pi2[b].Dist;
                    path = PathsInfo.ConstructPath(a, b, pi2);
                }

                G.AddEdge(firstPath[i]);
            }

            if(secondShortest==double.PositiveInfinity)
                return null;
            
            return secondShortest;
        }
    }
}
