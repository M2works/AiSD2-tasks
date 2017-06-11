
using System;
using System.Collections.Generic;
using ASD.Graphs;

namespace ASD
{

public static class Lab03GraphExtender
    {

    /// <summary>
    /// Wyszukiwanie cykli w grafie
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <param name="cycle">Znaleziony cykl</param>
    /// <returns>Informacja czy graf jest acykliczny</returns>
    /// <remarks>
    /// 1) Algorytm powinien dzia³aæ zarówno dla grafów skierowanych, jak i nieskierowanych
    /// 2) Grafu nie wolno zmieniaæ
    /// 3) Jeœli graf zawiera cykl to parametr cycle powinien byæ tablic¹ krawêdzi tworz¹cych dowolny z cykli.
    ///    Krawêdzie musz¹ byæ umieszczone we w³aœciwej kolejnoœci (tak jak w cyklu, mo¿na rozpocz¹æ od dowolnej)
    /// 4) Jeœli w grafie nie ma cyklu to parametr cycle ma wartoœæ null.
    /// </remarks>
    public static bool FindCycle(this Graph g, out Edge[] cycle)
        {
            List<int> cycleParts = new List<int>();

            int[] next = new int[g.VerticesCount];
            bool[] used = new bool[g.VerticesCount];

            for (int i = 0; i < next.Length; i++)
                next[i] = -1;

            if(g.VerticesCount==g.EdgesCount+1)
            {
                cycle = null;
                return false;
            }

            bool hasCycle = false;

            for (int i = 0; i < g.VerticesCount; i++)
            {
                next[i] = -1;

                Predicate<int> compare = v =>
                {
                    used[v] = true;
                    cycleParts.Add(v);
                    foreach (Edge e in g.OutEdges(v))
                    {
                        if (used[e.To] && e.To != i)
                            continue;

                        if (!used[e.To])
                            next[e.To] = v;

                        if ((g.Directed && e.To == i) || (!g.Directed && e.To == i && next[v] != e.To))
                        {
                            hasCycle = true;
                            return false;
                        }
                    }
                    return true;

                };

                Predicate<int> erase = v =>
                {
                    cycleParts.RemoveAt(cycleParts.Count-1);

                    used[v] = false;
                    next[v] = -1;

                    return true;
                };

                g.DFSearchFrom(i, compare, erase, null);

                if (hasCycle == true)
                {
                    cycle = new Edge [cycleParts.Count];
                    cycle[cycleParts.Count - 1] = new Edge(cycleParts[cycleParts.Count-1], i);

                    for (int j=cycleParts.Count-1;j>0;j--)
                    {
                        cycle[j-1] = new Edge(cycleParts[j - 1], cycleParts[j]);
                    }
                    return hasCycle;
                }
            }
                

            cycle = null;
            return hasCycle;
        }

    /// <summary>
    /// Wyznaczanie centrum drzewa
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <param name="center">Znalezione centrum</param>
    /// <returns>Informacja czy badany graf jest drzewem</returns>
    /// <remarks>
    /// 1) Dla grafów skierowanych metoda powinna zg³aszaæ wyj¹tek ArgumentException
    /// 2) Grafu nie wolno zmieniaæ
    /// 3) Parametr center to 1-elementowa lub 2-elementowa tablica zawieraj¹ca numery wierzcho³ków stanowi¹cych centrum.
    ///    (w przypadku 2 wierzcho³ków ich kolejnoœæ jest dowolna)
    /// </remarks>
    public static bool TreeCenter(this Graph g, out int[] center)
        {
            
            if (g.Directed)
                throw new ArgumentException("Graf skierowany");
            

            Graph copiedGraph = g.Clone();
            int verticesCount = copiedGraph.VerticesCount;
            
            if(copiedGraph.EdgesCount!=copiedGraph.VerticesCount-1)
            {
                center = null;
                return false;
            }

            if(verticesCount==0)
            {
                center = null;
                return false;
            }

            List<int> verticesLeft = new List<int>();
            List<int> leaves = new List<int>();

            for (int v = 0; v < verticesCount; v++)
                verticesLeft.Add(v);

            while (verticesCount > 2)
            {
                bool tree=false;
                leaves.Clear();
                for (int v = 0; v < verticesCount; v++)
                {
                    if (copiedGraph.OutDegree(verticesLeft[v]) == 0)
                    {
                        center = null;
                        return false;
                    }

                    if (copiedGraph.OutDegree(verticesLeft[v]) == 1 )
                    {
                        leaves.Add(verticesLeft[v]);
                        tree = true;
                    }
                }

                if (tree == false)
                {
                    center = null;
                    return false;
                }

                for (int i=0;i<leaves.Count;i++)
                {
                    foreach(Edge e in copiedGraph.OutEdges(leaves[i]))
                    {
                        int vertex = e.From;
                        bool deleted=copiedGraph.DelEdge(e);
                        if (deleted)
                            verticesLeft.Remove(vertex);
                    }
                }
                verticesCount -= leaves.Count;
                
            }

            center = new int[verticesCount];
            for (int i = 0; i < verticesCount; i++)
                center[i] = verticesLeft[i];

            return true;
        }

    }

}
