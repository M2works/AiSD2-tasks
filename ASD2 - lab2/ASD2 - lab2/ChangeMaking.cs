
using System;


namespace ASD
{

    class ChangeMaking
    {

        /// <summary>
        /// Metoda wyznacza rozwiązanie problemu wydawania reszty przy pomocy minimalnej liczby monet
        /// bez ograniczeń na liczbę monet danego rodzaju
        /// </summary>
        /// <param name="amount">Kwota reszty do wydania</param>
        /// <param name="coins">Dostępne nominały monet</param>
        /// <param name="change">Liczby monet danego nominału użytych przy wydawaniu reszty</param>
        /// <returns>Minimalna liczba monet potrzebnych do wydania reszty</returns>
        /// <remarks>
        /// coins[i]  - nominał monety i-tego rodzaju
        /// change[i] - liczba monet i-tego rodzaju (nominału) użyta w rozwiązaniu
        /// Jeśli dostepnymi monetami nie da się wydać danej kwoty to metochange = null,
        /// a metoda również zwraca null
        ///
        /// Wskazówka/wymaganie:
        /// Dodatkowa uzyta pamięć powinna (musi) być proporcjonalna do wartości amount ( czyli rzędu o(amount) )
        /// </remarks>
        public static int? NoLimitsDynamic(int amount, int[] coins, out int[] change)
        {
            int?[] count = new int?[amount + 1];
            int?[] changes = new int?[amount + 1];

            for (int i = 0; i < count.Length; i++)
            {
                count[i] = null;
                changes[i] = 0;
            }

            for (int i = 0; i < coins.Length; i++)
            {
                if (coins[i] < amount)
                    count[coins[i]] = 1;
            }

            int ind = 0;
            int? min;
            for (int i = 2; i < count.Length; i++)
            {
                min = -1;
                if (count[i] != null) continue;
                for (int k = 0; k < coins.Length; k++)
                    if (i - coins[k] >= 1)
                        if (count[i - coins[k]] != null)
                            if (count[i - coins[k]] < min || min == -1)
                            {
                                min = count[i - coins[k]] + 1;
                                ind = coins[k];
                            }
                if (min != -1)
                {
                    count[i] = min;
                    changes[i] = ind;
                }
            }

            int counter = 0;
            ind = amount;
            while (changes[ind] != 0)
            {
                counter++;
                ind -= (int)changes[ind];
            }
            counter++;

            change = new int[coins.Length];
            ind = amount;

            if (changes[ind] == 0) change = null;
            else
            {
                for (int i = 0; i < counter - 1; i++, ind -= (int)changes[ind])
                {
                    if (changes[ind] > 0)
                        for (int j = 0; j < coins.Length; j++)
                            if (coins[j] == changes[ind])
                                change[j]++;
                }

                for (int j = 0; j < coins.Length; j++)
                    if (coins[j] == ind)
                        change[j]++;
            }
            return count[amount];

            // zmienić
        }

        /// <summary>
        /// Metoda wyznacza rozwiązanie problemu wydawania reszty przy pomocy minimalnej liczby monet
        /// z uwzględnieniem ograniczeń na liczbę monet danego rodzaju
        /// </summary>
        /// <param name="amount">Kwota reszty do wydania</param>
        /// <param name="coins">Dostępne nominały monet</param>
        /// <param name="limits">Liczba dostępnych monet danego nomimału</param>
        /// <param name="change">Liczby monet danego nominału użytych przy wydawaniu reszty</param>
        /// <returns>Minimalna liczba monet potrzebnych do wydania reszty</returns>
        /// <remarks>
        /// coins[i]  - nominał monety i-tego rodzaju
        /// limits[i] - dostepna liczba monet i-tego rodzaju (nominału)
        /// change[i] - liczba monet i-tego rodzaju (nominału) użyta w rozwiązaniu
        /// Jeśli dostepnymi monetami nie da się wydać danej kwoty to change = null,
        /// a metoda również zwraca null
        ///
        /// Wskazówka/wymaganie:
        /// Dodatkowa uzyta pamięć powinna (musi) być proporcjonalna do wartości iloczynu amount*(liczba rodzajów monet)
        /// ( czyli rzędu o(amount*(liczba rodzajów monet)) )
        /// </remarks>
        public static int? Dynamic(int amount, int[] coins, int[] limits, out int[] change)
        {
            int?[,] dynamicTable = new int?[coins.Length, amount + 1];
            int[,] changes = new int[coins.Length, amount + 1];
            
            for (int j = 0; j < changes.GetLength(1); j++)
                changes[0, j] = -1;

            change = new int[coins.Length];

            for (int i = 0; i < dynamicTable.GetLength(0); i++)
                for (int j = 0; j < dynamicTable.GetLength(1); j++)
                    dynamicTable[i, j] = null;

            int[] coins2 = new int[coins.Length];
            int[] limits2 = new int[limits.Length];
            int[] indexes = new int[coins.Length];

            for(int i=0;i<coins.Length;i++)
            {
                coins2[i] = coins[i];
                limits2[i] = limits[i];
                indexes[i] = i;
            }

            for (int i = 1; i < coins.Length; i++)
                for (int j = 0; j < coins.Length - i; j++)
                {
                    if (coins2[j] < coins2[j + 1])
                    {
                        int tmp = coins2[j];
                        coins2[j] = coins2[j + 1];
                        coins2[j + 1] = tmp;

                        tmp = limits2[j];
                        limits2[j] = limits2[j + 1];
                        limits2[j + 1] = tmp;

                        tmp = indexes[j];
                        indexes[j] = indexes[j + 1];
                        indexes[j + 1] = tmp;
                    }
                }

            int last = -1;

            for (int i = 0; i < dynamicTable.GetLength(0); i++)
            {
                if (i != 0)
                {
                    for (int j = 0; j < dynamicTable.GetLength(1); j++)
                    {
                        dynamicTable[i, j] = dynamicTable[i - 1, j];
                    }

                    if (last != -1)
                    {
                        int k = last;
                        for (int j = 1; j <= limits2[i]; j++)
                        {
                            if (last + j * coins2[i] <= amount)
                            {
                                dynamicTable[i, last + j * coins2[i]] = dynamicTable[i, last] + j;
                                changes[i, last + j * coins2[i]] = j;
                            }
                            else
                                break;
                        }
                    }

                }

                for (int j = 1; j < dynamicTable.GetLength(1); j++)
                {
                    if (i == 0)
                    {
                        if (j == coins2[i])
                        {
                            if (limits2[i] >= 1)
                            {
                                dynamicTable[i, j] = 1;
                                changes[i, j] = 1;
                                last = j;
                            }
                        }
                        else
                        {
                            if (j - coins2[i] > 0)
                                if (dynamicTable[i, j - coins2[i]] != null)
                                {
                                    if (changes[i, j - coins2[i]] < limits2[i])
                                    {
                                        dynamicTable[i, j] = dynamicTable[i, j - coins2[i]] + 1;
                                        changes[i, j] = changes[i, j - coins2[i]]+1;
                                        last = j;
                                    }
                                }

                        }
                    }
                    else
                    {
                        if (j == coins2[i])
                        {
                            if (limits2[i] >= 1)
                            {
                                changes[i, j] = 1;
                                dynamicTable[i, j] = 1;
                                if(j>last)
                                    last = j;
                            }
                        }
                        else
                        {
                            if (j - coins2[i] > 0)
                                if (dynamicTable[i, j - coins2[i]] != null)
                                {
                                    if (dynamicTable[i, j] != null)
                                    {
                                        if (dynamicTable[i, j] > dynamicTable[i, j - coins2[i]])
                                        {
                                            if (changes[i, j - coins2[i]] < limits2[i])
                                            {
                                                dynamicTable[i, j] = dynamicTable[i, j - coins2[i]] + 1;
                                                changes[i, j] = changes[i,j-coins2[i]]+1;
                                                if(j>last)
                                                    last = j;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (changes[i, j - coins2[i]] < limits2[i])
                                        {
                                            dynamicTable[i, j] = dynamicTable[i, j - coins2[i]] + 1;
                                            if (changes[i, j - coins2[i]] != -1)
                                                changes[i, j] = changes[i, j - coins2[i]] + 1;
                                            else
                                                changes[i, j] = 1;
                                            if (j >last)
                                                last = j;
                                        }
                                    }
                               }
                         }
                    }
                }
                    
                }            

            if (dynamicTable[coins.Length - 1, amount] == null) change = null;
            else
            {
                int current = amount;
                for(int i=coins.Length-1;i>=0;i--)
                {
                    if (changes[i, current] != -1)
                        for (int j = changes[i, current]; j > 0; j--)
                        {
                            change[indexes[i]]++;
                            current -= coins2[i];
                        }
                }
            }
            return dynamicTable[coins.Length - 1, amount];

            }
        }
    
    }

