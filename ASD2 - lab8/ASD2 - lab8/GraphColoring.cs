using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace ASD
{
    public static class GraphColoring
    {
        /// <summary>
        /// Ogólna metoda - kolorowanie zachłanne na podstawie ustalonego ciągu wierzchołków.
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <param name="order">Porządek wierzchołków, w jakim mają być one przetwarzane. W przypadku null użyj pierwotnego numerowania wierzchołków</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] GreedyColoring(this Graph g, int[] order=null)
        {
            int[] lastOrder = new int[g.VerticesCount];
            if (null==order)
            {
                for (int i = 0; i < g.VerticesCount; i++)
                {
                    int max = 0;
                    bool[] already = new bool[g.VerticesCount + 1];
                    foreach (Edge e in g.OutEdges(i))
                        already[lastOrder[e.To]] = true;

                    for (int j = 1; j < g.VerticesCount + 1; j++)
                    {
                        if (already[j] == false)
                        {
                            max = j;
                            break;
                        }
                    }
                    lastOrder[i] = max;
                }
            }
            else
            {
                for (int i = 0; i < g.VerticesCount; i++)
                {
                    int max = 0;
                    bool [] already = new bool[g.VerticesCount+1];
                    foreach (Edge e in g.OutEdges(order[i]))
                        already[lastOrder[e.To]] = true; 

                    for(int j=1;j<g.VerticesCount+1;j++)
                    {
                        if(already[j]==false)
                        {
                            max = j;
                            break;
                        }
                    }
                    lastOrder[order[i]] = max;
                }
            }
            return lastOrder;
        }

        /// <summary>
        /// Przybliżone kolorowanie metodą BFS
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] BFSColoring(this Graph g)
        {
            int[] lastorder = new int[g.VerticesCount];
            int cc;
            g.GeneralSearchAll<EdgesQueue>(
                (x) =>
                {
                    if (lastorder[x]==0)
                    {
                        int max=0;
                        bool[] already = new bool[g.VerticesCount + 1];
                        foreach(Edge e in g.OutEdges(x))
                        {
                            if (lastorder[e.To] != 0)
                                already[lastorder[e.To]] = true;
                        }
                        for (int i = 1; i < already.Length; i++)
                            if (!already[i])
                            {
                                max = i;
                                break;
                            }
                        lastorder[x] = max;
                    }
                    return true;
                }, null, null,out cc,null
                    );
            return lastorder;
        }

        /// <summary>
        /// Przybliżone kolorowanie metodą LargestBackDegree
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] LargestBackDegree(this Graph g)
        {
            return null;
        }

        /// <summary>
        /// Przybliżone kolorowanie metodą ColorDegreeOrdering
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] ColorDegreeOrdering(this Graph g)
        {
            bool[] colored = new bool[g.VerticesCount];
            int[] colordegree = new int[g.VerticesCount];
            int[] lastOrder = new int[g.VerticesCount];
            List<int> vert = new List<int>();
            for (int i = 0; i < g.VerticesCount; i++)
                vert.Add(i);
            
            while (vert.Count!=0)
            {
                int max = vert[0];
                for (int i = 1; i < vert.Count; i++)
                        if(colordegree[max]<colordegree[vert[i]])
                            max = vert[i];

                bool[] already = new bool[g.VerticesCount+1];
                foreach (Edge e in g.OutEdges(max))
                {
                    colordegree[e.To]++;
                    already[lastOrder[e.To]] = true;
                }

                int minColor = 0;
                for(int j=1;j<g.VerticesCount+1;j++)
                {
                    if (!already[j])
                    {
                        minColor = j;
                        break;
                    }
                }
                lastOrder[max] = minColor;
                vert.Remove(max);
            }
            return lastOrder;
                 
        }

        /// <summary>
        /// Przybliżone kolorowanie metodą Incremental
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] Incremental(this Graph g)
        {
            int[] lastOrder = new int[g.VerticesCount];
            bool[] colored = new bool[g.VerticesCount];
            int coloredn = 0;
            int color = 1;
            while (coloredn != g.VerticesCount)
            {
                for (int i = 0; i < g.VerticesCount; i++)
                {
                    if (lastOrder[i] == 0)
                    {
                        bool lets = true;
                        foreach (Edge e in g.OutEdges(i))
                        {
                            if (lastOrder[e.To] == color)
                            {
                                lets = false;
                                break;
                            }
                        }
                        if (lets)
                        {
                            lastOrder[i] = color;
                            coloredn++;
                        }
                    }
                }
                color++;
            }
            return lastOrder;
        }
    }
}
