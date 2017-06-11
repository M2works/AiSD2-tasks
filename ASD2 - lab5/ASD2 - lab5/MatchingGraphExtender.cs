using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;

namespace ASD
{
    public static class MatchingGraphExtender
    {
        public class Cell<T>
        {
            public Cell(T val, Cell<T> c)
            {
                value = val;
                next = c;
            }
            public T value;
            public Cell<T> next;
        }
        public class Stark<T>
        {
            Cell<T> first;
            int countElems;
            public Stark()
                {
                    first = null;
                    countElems = 0;
                }
            
            public void Put(T val)
            {
                first = new Cell<T>(val, first);
                countElems++;
            }
            public T Get()
            {
                T value = first.value;
                first = first.next;
                countElems--;
                return value;
            }
            public T Peek()
            {
                return first.value;
            }
            public bool isEmpty()
            {
                return countElems == 0;
            }
            public int Count()
            {
                return countElems;
            }
            public T[] ToArrayR()
            {
                T[] array = new T[countElems];
                int n = array.Length;
                for (int i = 0; i < n; i++)
                    array[n - 1 - i]=this.Get();
                return array;
            }
            public T[] ToArray()
            {
                T[] array = new T[countElems];
                int n = array.Length;
                for (int i = 0; i < n; i++)
                    array[i] = this.Get();
                return array;
            }

        } 

        static bool DFSfc(this Graph g, int v, int w, ref int end, ref int first, ref Stark<int> vl, ref bool[] visited, ref Stark<Edge> cycleL)
        {
            int u;
            visited[w] = true;
            foreach (Edge e in g.OutEdges(w))
            {
                u = e.To;
                if (u != vl.Peek())
                {
                    vl.Put(w);
                    cycleL.Put(e);
                    if (visited[u])
                    {
                        end = w;
                        first = u;
                        return true;
                    }
                    if (!visited[u] && DFSfc(g, v, u, ref end, ref first, ref vl, ref visited, ref cycleL)) return true;
                    vl.Get();
                }
            }
            return false;

        }      
        
        /// <summary>
        /// Podział grafu na cykle. Zakładamy, że dostajemy graf nieskierowany i wszystkie wierzchołki grafu mają parzyste stopnie
        /// (nie trzeba sprawdzać poprawności danych).
        /// </summary>
        /// <param name="G">Badany graf</param>
        /// <returns>Tablica cykli; krawędzie każdego cyklu powinny być uporządkowane zgodnie z kolejnością na cyklu, zaczynając od dowolnej</returns>
        /// <remarks>
        /// Metoda powinna działać w czasie O(m)
        /// </remarks>
        public static Edge[][] cyclePartition(this Graph G)
        {
                        
            Graph g = G.Clone();
            int v = 0;
            Stark<int> vertices = new Stark<int>();
            Stark<Edge> cycleL = new Stark<Edge>();
            Stark<Edge> cycleLR = new Stark<Edge>();

            List<Edge[]> cycles = new List<Edge[]>();
            bool[] visited = new bool[G.VerticesCount];
            bool found = false;
            int end = -1,first=-1;
            vertices.Put(-1);
            while (g.EdgesCount != 0)
            {
                found = g.DFSfc(v, v,ref end, ref first, ref vertices, ref visited, ref cycleL);
                if (!found)
                {
                    v = (v + 1) % g.VerticesCount;
                    while(vertices.Count()!=1)
                        visited[vertices.Get()] = false;
                    continue;
                }

                int ind = cycleL.Count() - 1;
                while (cycleL.Peek().From != first)
                {
                    g.DelEdge(cycleL.Peek());
                    cycleLR.Put(cycleL.Get());
                }

                g.DelEdge(cycleL.Peek());
                cycleLR.Put(cycleL.Get());
                

                while (vertices.Count() != 1)
                    visited[vertices.Get()] = false;

                cycles.Add(cycleLR.ToArray());
                v = end;
                //}
            }

            Edge[][] ccls = cycles.ToArray();
            return ccls;        
        }

        /// <summary>
        /// Szukanie skojarzenia doskonałego w grafie nieskierowanym o którym zakładamy, że jest dwudzielny i 2^r-regularny
        /// (nie trzeba sprawdzać poprawności danych)
        /// </summary>
        /// <param name="G">Badany graf</param>
        /// <returns>Skojarzenie doskonałe w G</returns>
        /// <remarks>
        /// Metoda powinna działać w czasie O(m), gdzie m jest liczbą krawędzi grafu G
        /// </remarks>
        public static Graph perfectMatching(this Graph G)
        {
           
            Graph g = G.Clone();
            int outDegree = g.OutDegree(0);

            while (outDegree != 1)
            {
                outDegree /= 2;
                Edge[][] cycles = g.cyclePartition();
                List<Edge[]> cyclesL = new List<Edge[]>();

                for (int j = 0; j < cycles.Length; j++)
                    cyclesL.Add(cycles[j]);
                                
                for (int j = 0; j < cycles.Length; j++)
                {
                    int cycleLen = cyclesL[j].Length;

                    List<Edge> cycle = new List<Edge>();
                    for (int k = 0; k < cycleLen; k++)
                    {
                        if (k % 2 == 0)
                            g.DelEdge(cyclesL[j][k]);
                    }
                        cyclesL[j] = cycle.ToArray();
                }
            }

            return g;
        }
        
    }
}
