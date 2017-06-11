using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace Lab9
{
public struct MuseumRoutes
    {
        public MuseumRoutes(int count, int[][] routes)
            {
            this.liczba = count;
            this.trasy = routes;
            }

        public readonly int liczba;
        public readonly int[][] trasy;
    }


static class Muzeum
    {
        /// <summary>
        /// Znajduje najliczniejszy multizbiór tras
        /// </summary>
        /// <returns>Znaleziony multizbiór</returns>
        /// <param name="g">Graf opisujący muzeum</param>
        /// <param name="cLevel">Tablica o długości równej liczbie wierzchołków w grafie -- poziomy ciekawości wystaw</param>
        /// <param name="entrances">Wejścia</param>
        /// <param name="exits">Wyjścia</param>
        public static MuseumRoutes FindRoutes(Graph g, int[] cLevel, int[] entrances, int[] exits)
            {
            Graph museumGraph = new AdjacencyMatrixGraph(true, g.VerticesCount * 2+2);

            for(int v = 0;v < g.VerticesCount;v++)
            {
                foreach (Edge e in g.OutEdges(v))
                    museumGraph.AddEdge(new Edge(v,g.VerticesCount+e.To,double.PositiveInfinity));

                museumGraph.AddEdge(new Edge(g.VerticesCount + v, v,cLevel[v]));
            }

            foreach (int ent in entrances)
                museumGraph.AddEdge(museumGraph.VerticesCount - 2, ent+g.VerticesCount, double.PositiveInfinity);

            foreach (int exit in exits)
                museumGraph.AddEdge(exit, museumGraph.VerticesCount - 1, double.PositiveInfinity);

            Graph routes;
            int paths = (int)museumGraph.FordFulkersonDinicMaxFlow(museumGraph.VerticesCount - 2, museumGraph.VerticesCount - 1, out routes, MaxFlowGraphExtender.BFMaxPath);

            List<int> differentEntrances = new List<int>();
            for (int i = 0; i < entrances.Length; i++)
            {
                if (!differentEntrances.Contains(entrances[i]))
                    differentEntrances.Add(entrances[i]);
                else
                    differentEntrances.Add(-1);
            }

            int[] entries = new int[differentEntrances.Count];
            for (int i = 0; i < entrances.Length; i++)
            {
                if (differentEntrances[i] != -1)
                    entries[i] = (int)routes.GetEdgeWeight(entrances[i] + g.VerticesCount, entrances[i]);
                else
                    entries[i] = 0;
            }

            int sumOfEntries = 0;

            for (int i = 0; i < entries.Length; i++)
                sumOfEntries += entries[i];
            
            int[][] rts = new int[sumOfEntries][];

            List<int> singleRoute = new List<int>();

            int actualRoute = 0;

            for (int i = 0; i < entries.Length; i++)
            {
                while (entries[i] != 0)
                {
                    int current = entrances[i] + g.VerticesCount;
                    bool toBreak = false;

                    entries[i]--;

                    while (true)
                    {
                        if (routes.OutDegree(current) == 0)
                        {
                            toBreak = true;
                            break;
                        }
                        foreach (Edge e in routes.OutEdges(current))
                        {
                            if (e.Weight == 0)
                            {
                                routes.DelEdge(e);
                                continue;
                            }

                            if (e.To + g.VerticesCount == current)
                                singleRoute.Add(e.To);

                            routes.ModifyEdgeWeight(e.From, e.To, -1);

                            current = e.To;

                            foreach (int ex in exits)
                                if (current == ex)
                                {
                                    toBreak = true;
                                    break;
                                }
                            break;
                        }
                        if (toBreak)
                            break;
                    }
                    rts[actualRoute++] = singleRoute.ToArray();
                    singleRoute.Clear();

                }
            }

            MuseumRoutes mr = new MuseumRoutes(paths, rts);
            return mr;
            }


    }
}

