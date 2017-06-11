using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    public partial struct Point
    {

        /// <summary>
        /// Metoda zwraca odległość w przestrzeni Euklidesowej między dwoma punktami w przestrzeni dwuwymiarowej
        /// </summary>
        /// <param name="p1">Pierwszy punkt w przestrzeni dwuwymiarowej</param>
        /// <param name="p2">Drugi punkt w przestrzeni dwuwymiarowej</param>
        /// <returns>Odległość w przestrzeni Euklidesowej między dwoma punktami w przestrzeni dwuwymiarowej</returns>
        /// <remarks>
        /// 1) Algorytm powinien mieć złożoność O(1)
        /// </remarks>
        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));
        }

    }

    public class ByY : IComparer<Point>
    {
        public int Compare(Point p1, Point p2)
        {
            if (p1.y.CompareTo(p2.y) == 0)
                return p1.x.CompareTo(p2.x);
            else return p1.y.CompareTo(p2.y);
        }
    }

    class SweepClosestPair
    {

        /// <summary>
        /// Metoda zwraca dwa najbliższe punkty w dwuwymiarowej przestrzeni Euklidesowej
        /// </summary>
        /// <param name="points">Chmura punktów</param>
        /// <param name="minDistance">Odległość pomiędzy najbliższymi punktami</param>
        /// <returns>Para najbliższych punktów. Kolejność nie ma znaczenia</returns>
        /// <remarks>
        /// 1) Algorytm powinien mieć złożoność O(n^2), gdzie n to liczba punktów w chmurze
        /// </remarks>
        public static Tuple<Point, Point> FindClosestPointsBrute(List<Point> points, out double minDistance)
        {
            Tuple<Point, Point> best= new Tuple<Point, Point>(points[0],points[1]);
            minDistance = double.PositiveInfinity;
            double distance=double.PositiveInfinity;
            for(int i=0;i<points.Count;i++)
                for(int j=i+1;j<points.Count;j++)
                {
                    distance = Point.Distance(points[i], points[j]);
                    if(distance<minDistance)
                    {
                        minDistance = distance;
                        best = new Tuple<Point, Point>(points[i], points[j]);
                    }
                }
            return best;
        }

        /// <summary>
        /// Metoda zwraca dwa najbliższe punkty w dwuwymiarowej przestrzeni Euklidesowej
        /// </summary>
        /// <param name="points">Chmura punktów</param>
        /// <param name="minDistance">Odległość pomiędzy najbliższymi punktami</param>
        /// <returns>Para najbliższych punktów. Kolejność nie ma znaczenia</returns>
        /// <remarks>
        /// 1) Algorytm powinien mieć złożoność n*logn, gdzie n to liczba punktów w chmurze
        /// </remarks>
        public static Tuple<Point, Point> FindClosestPoints(List<Point> points, out double minDistance)
        {
            Point[] best = new Point[2];
            IComparer<Point> byy = new ByY();
            SortedSet<Point> sorted = new SortedSet<Point>(byy);

            points.Sort((p1, p2) =>
            {
                if (p1.x.CompareTo(p2.x) == 0)
                    return p1.y.CompareTo(p2.y);
                else return p1.x.CompareTo(p2.x);
            });

            sorted.Add(points[0]);
            sorted.Add(points[1]);
            best[0] = points[0];
            best[1] = points[1];
            double distance = Point.Distance(points[0], points[1]);
            int index = 0;

            for (int i = 2; i < points.Count; i++)
            {
                while(true)
                {
                    if (points[i].x - points[index].x > distance)
                    {
                        sorted.Remove(points[index]);
                        index++;
                    }
                    else
                        break;
                }

                foreach (Point p in sorted.GetViewBetween(new Point(0, points[i].y - distance), new Point(0, points[i].y + distance)))
                {
                    if (distance > Point.Distance(p, points[i]))
                    {
                        distance = Point.Distance(p, points[i]);
                        best[0] = p;
                        best[1] = points[i];
                    }
                }
                sorted.Add(points[i]);
            }
            minDistance = distance;
            return new Tuple<Point, Point>(best[0], best[1]);

        }

    }

}
