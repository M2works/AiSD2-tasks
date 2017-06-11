using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Lab14
{
    /// <summary>
    /// Klasa reprezentująca znalezione dopasowanie dwóch ciągów aminokwasów.
    /// </summary>
    public class Alignment
    {
        /// <summary>
        /// Początek podciągu v, który został dopasowany
        /// </summary>
        public int VStart { get; set; }
        /// <summary>
        /// Koniec podciągu v, który został dopasowany (używamy konwencji z stl, tj. to jest indeks pierwszego elementu za dopasowanym podciągiem.
        /// </summary>
        public int VEnd { get; set; }
        /// <summary>
        /// Początek podciągu w, który został dopasowany
        /// </summary>
        public int WStart { get; set; }
        /// <summary>
        /// Koniec podciągu w, który został dopasowany (używamy konwencji z stl, tj. to jest indeks pierwszego elementu za dopasowanym podciągiem.
        /// </summary>
        public int WEnd { get; set; }

        public string AlignedV { get; set; }
        public string AlignedW { get; set; }
    }

    /// <summary>
    /// Klasa znajdująca optymalne dopasowanie dwóch ciągów
    /// </summary>
    public class AlignmentFinder
    {
        // pomocniczy typ przydatny do śledzenia ostatnich operacji
        private enum LastOp
        {
            Skip,
            Change,
            GapV,
            GapW
        }

        /// <summary>
        /// Macierz oceny.
        /// Koszt zamiany znaku c1 na c2 odczytujemy Matrix[c1,c2], macierz ta jest jedynie opakowaniem słownika indeksowanego parami znaków.
        /// </summary>
        public ScoringMatrix Matrix
        {
            get;
        }
        /// <summary>
        /// Koszt wstawienia/usunięcia elementu.
        /// </summary>
        public int Epsilon
        {
            get;
        }

        public AlignmentFinder()
        {
            Matrix = ScoringMatrix.Simple;
            Epsilon = -1;
        }
        public AlignmentFinder(ScoringMatrix sm, int epsilon)
        {
            Matrix = sm;
            Epsilon = epsilon;
        }

        /// <summary>
        /// Funkcja znajdująca najlepsze dopasowanie ciągów (bez uwzględniania podciągów).
        /// </summary>
        /// <param name="v">pierwszy ciąg wejściowy</param>
        /// <param name="w">drugi ciąg wejściowy</param>
        /// <param name="alignment">obiekt opisujący najlepsze dopasowanie.
        /// Uwaga, w wersji bez uwzględniania podciągów ustaw:
        /// al.VStart = 0;
        /// al.WStart = 0;
        /// al.VEnd = v.Length;
        /// al.WEnd = w.Length;
        /// </param>
        /// <returns>wartość najlepszego dopasowania</returns>
        public int FindAlignment(string v, string w, out Alignment alignment)
        {


            alignment = new Alignment();
            alignment.VStart = 0;
            alignment.WStart = 0;
            alignment.VEnd = v.Length;
            alignment.WEnd = w.Length;

            int[,] assessments = new int[w.Length + 1, v.Length + 1];
            LastOp[,] operations = new LastOp[w.Length + 1, v.Length + 1];

            List<char> sbv = new List<char>();
            List<char> sbw = new List<char>();

            operations[0, 0] = LastOp.Skip;

            for (int i = 1; i < w.Length + 1; i++)
            {
                assessments[i, 0] = i * Epsilon;
                operations[i, 0] = LastOp.GapV;
            }
            for (int i = 1; i < v.Length + 1; i++)
            {
                assessments[0, i] = i * Epsilon;
                operations[0, 1] = LastOp.GapW;
            }

            for (int i = 1; i < w.Length + 1; i++)
                for (int j = 1; j < v.Length + 1; j++)
                {
                    int cost = Matrix[w[i - 1], v[j - 1]];
                    if (assessments[i - 1, j] + Epsilon > assessments[i, j - 1] + Epsilon)
                    {
                        if (assessments[i - 1, j - 1] + cost > assessments[i - 1, j] + Epsilon)
                        {
                            assessments[i, j] = assessments[i - 1, j - 1] + cost;
                            operations[i, j] = LastOp.Change;
                        }
                        else
                        {
                            assessments[i, j] = assessments[i - 1, j] + Epsilon;
                            operations[i, j] = LastOp.GapV;
                        }
                    }
                    else
                    {
                        if (assessments[i - 1, j - 1] + cost > assessments[i, j - 1] + Epsilon)
                        {
                            assessments[i, j] = assessments[i - 1, j - 1] + cost;
                            operations[i, j] = LastOp.Change;
                        }
                        else
                        {
                            assessments[i, j] = assessments[i, j - 1] + Epsilon;
                            operations[i, j] = LastOp.GapW;
                        }
                    }
                }

            int iv = v.Length, iw = w.Length;

            for (; iw > 0 || iv > 0;)
            {
                if (operations[iw, iv] == LastOp.Change)
                {
                    sbv.Add(v[iv - 1]);
                    sbw.Add(w[iw - 1]);
                    iv--; iw--;
                }
                else
                {
                    if (operations[iw, iv] == LastOp.GapV)
                    {
                        sbv.Add('-');
                        sbw.Add(w[iw - 1]);
                        iw--;
                    }
                    else
                    {
                        sbw.Add('-');
                        sbv.Add(v[iv - 1]);
                        iv--;
                    }
                }
            }

            sbv.Reverse();
            sbw.Reverse();

            alignment.AlignedV = String.Concat(sbv.ToArray());
            alignment.AlignedW = String.Concat(sbw.ToArray());

            return assessments[w.Length,v.Length];


        }

        /// <summary>
        /// Funkcja znajdująca najlepsze dopasowanie podciągów.
        /// </summary>
        /// <param name="v">pierwszy ciąg wejściowy</param>
        /// <param name="w">drugi ciąg wejściowy</param>
        /// <param name="alignment">obiekt opisujący najlepsze dopasowanie podciągów.
        /// Uwaga, w wersji z podciągami ustaw:
        /// al.VStart = indeks pierwszego elementu optymalnego podciągu v, który dopasowywaliśmy
        /// al.WStart = indeks pierwszego elementu optymalnego podciągu w, który dopasowywaliśmy
        /// al.VEnd = indeks pierwszego elementu za optymalnym podciągiem v, który dopasowywaliśmy
        /// al.WEnd = indeks pierwszego elementu za optymalnym podciągiem w, który dopasowywaliśmy
        /// </param>
        /// <returns>wartość najlepszego dopasowania</returns>
        public int FindSubsequenceAlignment(string v, string w, out Alignment alignment)
        {
            alignment = new Alignment();

            int globalMax = int.MinValue;

            int[,] assessments = new int[w.Length + 1, v.Length + 1];
            LastOp[,] operations = new LastOp[w.Length + 1, v.Length + 1];

            List<char> sbv = new List<char>();
            List<char> sbw = new List<char>();
            
            for (int i = 1; i < w.Length + 1; i++)
            {
                assessments[i, 0] = i * Epsilon;
                operations[i, 0] = LastOp.GapV;
            }
            for (int i = 1; i < v.Length + 1; i++)
            {
                assessments[0, i] = i * Epsilon;
                operations[0, 1] = LastOp.GapW;
            }

            for (int i = 1; i < w.Length + 1; i++)
                for (int j = 1; j < v.Length + 1; j++)
                {
                    int cost = Matrix[w[i - 1], v[j - 1]];
                    int maxValue;
                    if (assessments[i - 1, j] + Epsilon > assessments[i, j - 1] + Epsilon)
                    {
                        if (assessments[i - 1, j - 1] + cost > assessments[i - 1, j] + Epsilon)
                        {
                            maxValue = assessments[i - 1, j - 1] + cost;
                            operations[i, j] = LastOp.Change;
                        }
                        else
                        {
                            maxValue = assessments[i - 1, j] + Epsilon;
                            operations[i, j] = LastOp.GapV;
                        }
                    }
                    else
                    {
                        if (assessments[i - 1, j - 1] + cost > assessments[i, j - 1] + Epsilon)
                        {
                            maxValue = assessments[i - 1, j - 1] + cost;
                            operations[i, j] = LastOp.Change;
                        }
                        else
                        {
                            maxValue = assessments[i, j - 1] + Epsilon;
                            operations[i, j] = LastOp.GapW;
                        }
                    }

                    if (maxValue < cost)
                    {
                        assessments[i, j] = cost;
                        operations[i, j] = LastOp.Skip;
                        if (cost > globalMax)
                        {
                            globalMax = cost;
                            alignment.WEnd = i;
                            alignment.VEnd = j;
                        }
                    }
                    else
                    {
                        assessments[i, j] = maxValue;
                        if(maxValue>globalMax)
                        {
                            globalMax = maxValue;
                            alignment.WEnd = i;
                            alignment.VEnd = j;
                        }
                    }
                }

            int iv = alignment.VEnd, iw = alignment.WEnd;

            bool skipDetected = false;
            for (; !skipDetected && iv>0 && iw>0;)
            {
                if (operations[iw, iv] == LastOp.Change)
                {
                    sbv.Add(v[iv - 1]);
                    sbw.Add(w[iw - 1]);
                    iv--; iw--;
                }
                else
                {
                    if (operations[iw, iv] == LastOp.GapV)
                    {
                        sbv.Add('-');
                        sbw.Add(w[iw - 1]);
                        iw--;
                    }
                    else
                    {
                        if (operations[iw, iv] == LastOp.GapW)
                        {
                            sbw.Add('-');
                            sbv.Add(v[iv - 1]);
                            iv--;
                        }
                        else
                        {
                            sbv.Add(v[iv - 1]);
                            sbw.Add(w[iw - 1]);
                            iv--;
                            iw--;
                            skipDetected = true;
                        }
                    }
                }
            }
            alignment.VStart = iv;
            alignment.WStart = iw;

            sbv.Reverse();
            sbw.Reverse();

            alignment.AlignedV = String.Concat(sbv.ToArray());
            alignment.AlignedW = String.Concat(sbw.ToArray());

            return globalMax;
        }

    }

}
