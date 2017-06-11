
using System.Collections.Generic;
using System.Collections;
using System;
using ASD.Graphs;
using System.Linq;

namespace lab10
{

public struct AlmostMatchingSolution
    {
        public AlmostMatchingSolution(int edgesCount, List<Edge> solution)
            {
            this.edgesCount=edgesCount;
            this.solution=solution;
            }

        public readonly int edgesCount;
        public readonly List<Edge> solution;
    }



public class AlmostMatching
    {
        public static int maxLines;
        public static int countLines;
        public static int currentRisk;
        public static int sumWages;
        public static int finalSumWages;
        public static List<Edge> finalSet;
        public static int _allowedCollisions;
        public static Stack<Edge> usedEdges;
        public static int countUsedEdges;
        public static int[] wagi;
        public static List<Edge> edges;
        /// <summary>
        /// Zwraca najliczniejszy możliwy zbiór krawędzi, którego poziom
        /// ryzyka nie przekracza limitu. W ostatnim etapie zwracać
        /// zbiór o najmniejszej sumie wag ze wszystkich najliczniejszych.
        /// </summary>
        /// <returns>Liczba i lista linek (krawędzi)</returns>
        /// <param name="g">Graf linek</param>
        /// <param name="allowedCollisions">Limit ryzyka</param>
        public static AlmostMatchingSolution LargestS(Graph g, int allowedCollisions)
            {
            _allowedCollisions = allowedCollisions;
            currentRisk = 0;
            maxLines = 0;
            countLines = 0;
            sumWages = 0;
            countUsedEdges = 0;
            finalSumWages = int.MaxValue;
            finalSet = new List<Edge>();
            usedEdges = new Stack<Edge>();
            wagi = new int[g.VerticesCount];
            edges = new List<Edge>();

            for(int v=0;v<g.VerticesCount;v++)
            {
                foreach (Edge e in g.OutEdges(v))
                    if (e.To>e.From)
                        edges.Add(e);
            }

            RECAMS3(g,0);

             return new AlmostMatchingSolution(maxLines, finalSet);
            }


        public static void RECAMS3(Graph g, int currentEdge)
        {
            int countRiskChanges = 0;
            if (currentRisk > _allowedCollisions)
                return;

            if (currentEdge == edges.Count)
            {
                if (maxLines <= countLines)
                {
                    maxLines = countLines;
                    if (countLines == finalSet.Count)
                    {
                        if (sumWages < finalSumWages)
                        {
                            finalSet = new List<Edge>(usedEdges);
                            finalSumWages = sumWages;
                        }
                    }
                    else
                    if (countLines > finalSet.Count)
                    {
                        finalSet = new List<Edge>(usedEdges);
                        finalSumWages = sumWages;
                    }
                }
                return;
            }
            
            usedEdges.Push(edges[currentEdge]);
            wagi[edges[currentEdge].From]++;
            sumWages += (int)edges[currentEdge].Weight;
            if (wagi[edges[currentEdge].From] > 1)
                countRiskChanges++;

            wagi[edges[currentEdge].To]++;
            if (wagi[edges[currentEdge].To] > 1)
                countRiskChanges++;
            countLines++;

            currentRisk += countRiskChanges;

            RECAMS3(g, currentEdge + 1);
            
            usedEdges.Pop();
            wagi[edges[currentEdge].From]--;
            sumWages -= (int)edges[currentEdge].Weight;

            wagi[edges[currentEdge].To]--;

            currentRisk -= countRiskChanges;
            countLines--;

            RECAMS3(g, currentEdge + 1);
        }

        
    }

}


