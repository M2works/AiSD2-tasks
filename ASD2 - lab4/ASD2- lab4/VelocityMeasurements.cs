using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    class VelocityMeasurements
    {
        /// <summary>
        /// Metoda zwraca możliwą minimalną i maksymalną wartość prędkości samochodu w momencie wypadku.
        /// </summary>
        /// <param name="measurements">Tablica zawierające wartości pomiarów urządzenia zainstalowanego w aucie Mateusza</param>
        /// <param name="isBrakingValue">Tablica zwracająca informację dla każdego z pomiarów z tablicy measurements informację bool czy dla sekwencji dającej 
        /// minimalną prędkość wynikową traktować dany pomiar jako hamujący (true) przy przyspieszający (false)</param>
        /// <returns>Struktura Velocities z informacjami o najniższej i najwyższej możliwej prędkości w momencie wypadku</returns>
        /// 
        /// <remarks>
        /// Złożoność pamięciowa algorytmu powinna być nie większa niż O(sumy_wartości_pomiarów).
        /// Złożoność czasowa algorytmu powinna być nie większa niż O(liczby_pomiarów * sumy_wartości_pomiarów).
        /// </remarks>
        public static Velocities FinalVelocities(int[] measurements, out bool[] isBrakingValue)
        {
            int maxV=0, minV=0;
            

            for (int i = 0; i < measurements.Length; i++)
                maxV += measurements[i];

            int[] speed = new int[2*maxV+1];
            int[][] direction = new int[2][];

            for (int i = 0; i < direction.Length; i++)
                direction[i] = new int[2 * maxV + 1];

            int medium = (2 * maxV + 1)/2;

            isBrakingValue = new bool[measurements.Length];

            if (medium==0)
            {
                isBrakingValue[0] = false;
                return new Velocities(0, 0);
            }

            bool[] optimalSet = new bool[measurements.Length];

            int[] distinction = new int[maxV / 2+1];
            int[] indexes = new int[maxV / 2+1];

            for (int i = 0; i < distinction.Length; i++)
            {
                distinction[i] = int.MaxValue;
                indexes[i] = int.MaxValue;
            }

            for(int i=0;i<measurements.Length;i++)
            {
                for(int j=0;j<maxV/2;j++)
                {
                    if(measurements[i]==j && distinction[j]==int.MaxValue)
                    {
                        distinction[j] = measurements[i];
                        indexes[j] = i;
                    }
                    if(distinction[j]!=int.MaxValue && j+measurements[i]<=maxV/2)
                    {
                        distinction[j + measurements[i]] = distinction[j] + measurements[i];
                        indexes[j] = i;
                    }
                }
            }

            int index=-1,count=-1;
            for(int i=maxV/2;i>=0;i--)
            {
                if(distinction[i]!=int.MaxValue)
                {
                    index = i;
                    count = distinction[i];
                    break;
                }
            }
            
            if(index==-1)
            {
                return new Velocities(measurements[0], measurements[0]);
            }

            while(count!=0)
            {
                isBrakingValue[indexes[index]] = true;
                count -= measurements[indexes[index]];
                index = distinction[count];
            }

            for (int i = 0; i < speed.Length; i++)
            {
                speed[i] = int.MaxValue;
                for(int j=0;j<direction.Length;j++)
                    direction[j][i] = int.MaxValue;
            }
            
            direction[1][medium + measurements[0]] = 0;
            speed[measurements[0] + medium] = measurements[0];
               

            for (int i = 1; i < measurements.Length; i++)
            {
                for(int j=0;j<direction[0].Length;j++)
                {
                    direction[0][j] = direction[1][j];
                }

                for (int j = 0; j < speed.Length; j++)
                {
                    if(Math.Abs(direction[0][j]) == (i - 1))
                    {
                        if (j - measurements[i] >= 0)
                        {
                            speed[j - measurements[i]] = speed[j] - measurements[i];
                            direction[1][j - measurements[i]] = -i;
                        }
                        if (j + measurements[i] < 2*maxV+1)
                        {
                            speed[j + measurements[i]] = speed[j] + measurements[i];
                            direction[1][j + measurements[i]] = i;
                        }
                    }
                }
            }

            int ind1 = 0;
            int ind2 = 0;

            for(int i=medium;i<speed.Length;i++)
            {
                if(speed[i]!=int.MaxValue && Math.Abs(direction[1][i])==measurements.Length-1)
                {
                    ind1 = i;
                    break;
                }
            }

            for(int i=medium;i>=0;i--)
            {
                if(speed[i] != int.MaxValue && Math.Abs(direction[1][i]) == measurements.Length - 1)
                {
                    ind2 = i;
                    break;
                }
            }

            if (Math.Min(medium - ind2, ind1 - medium) == medium - ind2)
                minV = medium - ind2;
            else
                minV = ind1 - medium;
            return new Velocities(minV, maxV);
        }

        /// <summary>
        /// Metoda zwraca możliwą minimalną i maksymalną wartość prędkości samochodu w trakcie całego okresu trwania podróży.
        /// </summary>
        /// <param name="measurements">Tablica zawierające wartości pomiarów urządzenia zainstalowanego w aucie Mateusza</param>
        /// <param name="isBrakingValue">W tej wersji algorytmu proszę ustawić parametr na null</param>
        /// <returns>Struktura Velocities z informacjami o najniższej i najwyższej możliwej prędkości na trasie</returns>
        /// 
        /// <remarks>
        /// Złożoność pamięciowa algorytmu powinna być nie większa niż O(sumy_wartości_pomiarów).
        /// Złożoność czasowa algorytmu powinna być nie większa niż O(liczby_pomiarów * sumy_wartości_pomiarów).
        /// </remarks>
        public static Velocities JourneyVelocities(int[] measurements, out bool[] isBrakingValue)
        {
            isBrakingValue = null;  // Nie zmieniać !!!
            int maxV = 0, minV = 0;


            for (int i = 0; i < measurements.Length; i++)
                maxV += measurements[i];

            int[] speed = new int[2 * maxV + 1];
            int[][] direction = new int[2][];

            for (int i = 0; i < direction.Length; i++)
                direction[i] = new int[2 * maxV + 1];

            int medium = (2 * maxV + 1) / 2;

            if (medium == 0)
            {
                return new Velocities(0, 0);
            }

            for (int i = 0; i < speed.Length; i++)
            {
                speed[i] = int.MaxValue;
                for (int j = 0; j < direction.Length; j++)
                    direction[j][i] = int.MaxValue;
            }

            direction[1][medium + measurements[0]] = 0;
            speed[measurements[0] + medium] = measurements[0];


            for (int i = 1; i < measurements.Length; i++)
            {
                for (int j = 0; j < direction[0].Length; j++)
                {
                    direction[0][j] = direction[1][j];
                }

                for (int j = 0; j < speed.Length; j++)
                {
                    if (Math.Abs(direction[0][j]) == (i - 1))
                    {
                        if (j - measurements[i] >= 0)
                        {
                            speed[j - measurements[i]] = speed[j] - measurements[i];
                            direction[1][j - measurements[i]] = -i;
                        }
                        if (j + measurements[i] < 2 * maxV + 1)
                        {
                            speed[j + measurements[i]] = speed[j] + measurements[i];
                            direction[1][j + measurements[i]] = i;
                        }
                    }
                }
            }

            int ind1 = 0;
            int ind2 = 0;

            for (int i = medium; i < speed.Length; i++)
            {
                if (speed[i] != int.MaxValue)
                {
                    ind1 = i;
                    break;
                }
            }

            for (int i = medium; i >= 0; i--)
            {
                if (speed[i] != int.MaxValue)
                {
                    ind2 = i;
                    break;
                }
            }

            if (Math.Min(medium - ind2, ind1 - medium) == medium - ind2)
                minV = medium - ind2;
            else
                minV = ind1 - medium;

            return new Velocities(minV, maxV);
        }
    }
}
