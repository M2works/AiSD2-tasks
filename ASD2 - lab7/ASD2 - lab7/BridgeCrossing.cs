using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class BridgeCrossing
    {

        /// <summary>
        /// Metoda rozwiązuje zadanie optymalnego przechodzenia przez most.
        /// </summary>
        /// <param name="_times">Tablica z czasami przejścia poszczególnych osób</param>
        /// <param name="strategy">Strategia przekraczania mostu: lista list identyfikatorów kolejnych osób,
        /// które przekraczają most (na miejscach parzystych przejścia par przez most,
        /// na miejscach nieparzystych powroty jednej osoby z latarką). Jeśli istnieje więcej niż jedna strategia
        /// realizująca przejście w optymalnym czasie wystarczy zwrócić dowolną z nich.</param>
        /// <returns>Minimalny czas, w jakim wszyscy turyści mogą pokonać most</returns>
        public static int CrossBridge(int[] times, out List<List<int>> strategy)
        {
            if (times.Length == 1)
            {
                strategy = new List<List<int>>();
                List<int> onlyOne = new List<int>();
                onlyOne.Add(0);
                strategy.Add(onlyOne);
                return times[0];
            }
            else
            {
                states = new bool[times.Length];
                minTime = int.MaxValue;
                allpeople = times.Length;
                lowerLimit = int.MaxValue;
                for(int i=0;i<times.Length;i++)
                    if (times[i] < lowerLimit)
                        lowerLimit = times[i];
                if (times.Length == 2)
                    iterationsLeft = 1;
                else
                    iterationsLeft = times.Length * 2 - 1;
                
                peoplePassed = 0;
                RecCB(0, ref times);                

                strategy = ourStrategy;
                return minTime;
            }
        }
        static List<List<int>> currentStrategy = new List<List<int>>();
        static List<List<int>> ourStrategy;
        static bool[] states;
        static int allpeople;
        static int minTime;
        static int lowerLimit;
        static int peoplePassed;
        static int iterationsLeft;

        public static void RecCB(int currentTime, ref int[] times)
        {
            if (currentTime >= minTime)
                return;

            for (int i = 0; i < allpeople; i++)
            {
                if (states[i])
                    continue;

                states[i] = true;
                peoplePassed++;
                for (int ind1 =i+1; ind1 < allpeople; ind1++)
                {
                    if (!states[ind1])
                    {
                        currentTime += Math.Max(times[i], times[ind1]);
                        peoplePassed++;
                        states[ind1] = true;
                        List<int> list1 = new List<int>();
                        list1.Add(i);
                        list1.Add(ind1);
                        currentStrategy.Add(list1);

                        if (peoplePassed == allpeople)
                        {
                            if (minTime > currentTime)
                            {
                                minTime = currentTime;
                                ourStrategy = new List<List<int>>(currentStrategy.Count);
                                for (int j = 0; j < currentStrategy.Count; j++)
                                {
                                    List<int> partial = new List<int>();
                                    for (int j2 = 0; j2 < currentStrategy[j].Count; j2++)
                                        partial.Add(currentStrategy[j][j2]);
                                    ourStrategy.Add(partial);
                                }
                            }
                        }
                        else
                        {
                            int minind = int.MaxValue;
                            for (int ind2 = 0; ind2 < allpeople; ind2++)
                                if (states[ind2])
                                {
                                    if (minind == int.MaxValue || times[ind2] < times[minind])
                                        minind = ind2;
                                }
                            states[minind] = false;
                            List<int> RTL = new List<int>();
                            RTL.Add(minind);
                            currentStrategy.Add(RTL);
                            currentTime += times[minind];
                            peoplePassed--;
                            RecCB(currentTime, ref times);
                            peoplePassed++;
                            states[minind] = true;
                            currentTime -= times[minind];
                            currentStrategy.RemoveAt(currentStrategy.Count - 1);
                        }
                        states[ind1] = false;
                        peoplePassed--;
                        currentStrategy.RemoveAt(currentStrategy.Count - 1);
                        currentTime -= Math.Max(times[i], times[ind1]);
                    }
                }
                peoplePassed--;
                states[i] = false;
            }
        }
        // MOŻESZ DOPISAĆ POTRZEBNE POLA I METODY POMOCNICZE
        // MOŻESZ NAWET DODAĆ CAŁE KLASY (ALE NIE MUSISZ)

    }
}