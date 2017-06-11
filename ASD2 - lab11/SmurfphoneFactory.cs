using System;
using ASD.Graphs;

namespace ASD
{


    public static class SmurfphoneFactory
    {
        /// <summary>
        /// Metoda zwraca największą możliwą do wyprodukowania liczbę smerfonów
        /// </summary>
        /// <param name="providers">Dostawcy</param>
        /// <param name="factories">Fabryki</param>
        /// <param name="distanceCostMultiplier">współczynnik kosztu przewozu</param>
        /// <param name="productionCost">Łączny koszt produkcji wszystkich smerfonów</param>
        /// <param name="transport">Tablica opisująca ilości transportowanych surowców miedzy poszczególnymi dostawcami i fabrykami</param>
        /// <param name="maximumProduction">Maksymalny rozmiar produkcji</param>
        public static double CalculateFlow(Provider[] providers, Factory[] factories, double distanceCostMultiplier, out double productionCost, out int[,] transport, int maximumProduction = int.MaxValue)
        {
            int providersCount = providers.Length, factoriesCount = factories.Length;
            int s = providersCount + factoriesCount*2, t = providersCount + factoriesCount*2 + 1;
            int S = s + 2;
            Graph g = new AdjacencyMatrixGraph(true, providersCount + factoriesCount * 2 + 3);
            Graph c = new AdjacencyMatrixGraph(true, providersCount + factoriesCount * 2 + 3);

            for (int v=0;v<providersCount;v++)
            {
                g.AddEdge(s, v, providers[v].Capacity);
                c.AddEdge(s, v, providers[v].Cost);
            }

            for(int v=providersCount;v<providersCount+factoriesCount;v++)
            {
                g.AddEdge(v, t, factories[v - providersCount].Limit);
                c.AddEdge(v, t, factories[v - providersCount].LowerCost);
            }

            for(int v=providersCount+factoriesCount;v<providersCount+2*factoriesCount;v++)
            {
                g.AddEdge(v, t, int.MaxValue);
                c.AddEdge(v, t, factories[v - providersCount - factoriesCount].HigherCost);
            }

            if (maximumProduction == int.MaxValue)
                g.AddEdge(S, s, int.MaxValue);
            else
                g.AddEdge(S, s, maximumProduction);
            c.AddEdge(S, s, 0);

            for (int v=0;v< providersCount;v++)
            {
                for (int w = providersCount; w < providersCount + factoriesCount; w++)
                {
                    double countLength = Math.Sqrt(Math.Pow(providers[v].Position.X - factories[w - providersCount].Position.X, 2) + Math.Pow(providers[v].Position.Y - factories[w - providersCount].Position.Y, 2));

                    g.AddEdge(v, w, int.MaxValue);
                    c.AddEdge(v, w, Math.Ceiling(distanceCostMultiplier*countLength));
                    
                    g.AddEdge(v, w+factoriesCount, int.MaxValue);
                    c.AddEdge(v, w+factoriesCount, Math.Ceiling(distanceCostMultiplier*countLength));
                }
            }
            
            Graph flow;
            double maxFlow = g.MinCostFlow(c, S, t, out productionCost, out flow);

            transport = new int[providersCount, factoriesCount];

            for(int v=0;v< providersCount;v++)
            {
                for(int w=providersCount;w<providersCount+factoriesCount;w++)
                    if(!flow.GetEdgeWeight(v,w).IsNaN())
                        transport[v, w - providersCount] = (int)flow.GetEdgeWeight(v, w);

                for(int w=providersCount+factoriesCount;w<providersCount+2*factoriesCount;w++)
                    if (!flow.GetEdgeWeight(v, w).IsNaN())
                        transport[v, w - providersCount-factoriesCount] += (int)flow.GetEdgeWeight(v, w);
            }

            return maxFlow;
        }
    }
}
