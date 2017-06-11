using System.Collections.Generic;
using System;
using static ASD.Geometry;

namespace ASD
{
    public static class SutherlandHodgman
    {
        /// <summary>
        /// Oblicza pole wielokata przy pomocy formuly Gaussa
        /// </summary>
        /// <param name="polygon">Kolejne wierzcholki wielokata</param>
        /// <returns>Pole wielokata</returns>
        public static double PolygonArea(this Point[] polygon)
        {
            double area=0;
            for (int i = 2; i < polygon.Length; i++)
                area += Point.CrossProduct(polygon[i - 1] - polygon[0], polygon[i] - polygon[0]);
            return Math.Abs(area / 2);
        }

        /// <summary>
        /// Sprawdza, czy dwa punkty leza po tej samej stronie prostej wyznaczonej przez odcinek s
        /// </summary>
        /// <param name="p1">Pierwszy punkt</param>
        /// <param name="p2">Drugi punkt</param>
        /// <param name="s">Odcinek wyznaczajacy prosta</param>
        /// <returns>
        /// True, jesli punkty leza po tej samej stronie prostej wyznaczonej przez odcinek 
        /// (lub co najmniej jeden z punktow lezy na prostej). W przeciwnym przypadku zwraca false.
        /// </returns>
        public static bool IsSameSide(Point p1, Point p2, Segment s)
        {
            double d1, d2,d12;
            d1 = Point.CrossProduct(s.ps - s.pe, p2 - s.pe);
            d2 = Point.CrossProduct(s.ps - s.pe, p1 - s.pe);

            d12 = d1 * d2;

            if (d12 >= 0)
                return true;
            return false;
        }

        /// <summary>Oblicza czesc wspolna dwoch wielokatow przy pomocy algorytmu Sutherlanda–Hodgmana</summary>
        /// <param name="subjectPolygon">Wielokat obcinany (wklesly lub wypukly)</param>
        /// <param name="clipPolygon">Wielokat obcinajacy (musi byc wypukly i zakladamy, ze taki jest)</param>
        /// <returns>Czesc wspolna wielokatow</returns>
        /// <remarks>
        /// - mozna zalozyc, ze 3 kolejne punkty w kazdym z wejsciowych wielokatow nie sa wspolliniowe
        /// - wynikiem dzialania funkcji moze byc tak naprawde wiele wielokatow (sytuacja taka moze wystapic,
        ///   jesli wielokat obcinany jest wklesly)
        /// - jesli wielokat obcinany i obcinajacy zawieraja wierzcholki o tych samych wspolrzednych,
        ///   w wynikowym wielokacie moge one byc zduplikowane
        /// - wierzcholki wielokata obcinanego, przez ktore przechodza krawedzie wielokata obcinajacego
        ///   zostaja zduplikowane w wielokacie wyjsciowym
        /// </remarks>
        public static Point[] GetIntersectedPolygon(Point[] subjectPolygon, Point[] clipPolygon)
        {
            double x=0, y=0;
            for(int i=0;i<clipPolygon.Length;i++)
            {
                x += clipPolygon[i].x;
                y += clipPolygon[i].y;
            }

            x /= clipPolygon.Length;
            y /= clipPolygon.Length;

            Point center = new Point(x, y);
            double centerPosition = Point.CrossProduct(clipPolygon[1] - clipPolygon[0], center - clipPolygon[0]);

            List<Point> output = new List<Point>(subjectPolygon);

            for (int i = 0; i < clipPolygon.Length; i++)
            {
                List<Point> input = new List<Point>(output);
                output.Clear();
                Point pp = input[input.Count - 1];
                foreach (Point p in input)
                {
                    Segment e = new Segment(clipPolygon[i % clipPolygon.Length], clipPolygon[(i + 1) % clipPolygon.Length]);
                    double pointPosition = Point.CrossProduct(e.pe - e.ps, p - e.ps);
                    double ppPosition = Point.CrossProduct(e.pe - e.ps, pp - e.ps);

                    if (IsSameSide(p,center, e))
                    {
                        if (!IsSameSide(pp,center, e))
                        {
                            output.Add(GetIntersectionPoint(new Segment(pp, p), new Segment(e.pe, e.ps)));
                        }
                        output.Add(new Point(p.x,p.y));
                    }
                    else
                    {
                        if (centerPosition*ppPosition > 0)
                        {
                            output.Add(GetIntersectionPoint(new Segment(pp, p), new Segment(e.pe, e.ps)));
                        }
                    }
                    pp = new Point(p.x,p.y);
                }
            }

            List<Point> outputWithoutDuplicates = new List<Point>();
            for (int i = 0; i < output.Count; i++)
                if (!outputWithoutDuplicates.Contains(output[i]))
                    outputWithoutDuplicates.Add(output[i]);

            return outputWithoutDuplicates.ToArray();
        }

        /// <summary>
        /// Zwraca punkt przeciecia dwoch prostych wyznaczonych przez odcinki
        /// </summary>
        /// <param name="seg1">Odcinek pierwszy</param>
        /// <param name="seg2">Odcinek drugi</param>
        /// <returns>Punkt przeciecia prostych wyznaczonych przez odcinki</returns>
        public static Point GetIntersectionPoint(Segment seg1, Segment seg2)
        {
            Point direction1 = new Point(seg1.pe.x - seg1.ps.x, seg1.pe.y - seg1.ps.y);
            Point direction2 = new Point(seg2.pe.x - seg2.ps.x, seg2.pe.y - seg2.ps.y);
            double dotPerp = (direction1.x * direction2.y) - (direction1.y * direction2.x);

            Point c = new Point(seg2.ps.x - seg1.ps.x, seg2.ps.y - seg1.ps.y);
            double t = (c.x * direction2.y - c.y * direction2.x) / dotPerp;

            return new Point(seg1.ps.x + (t * direction1.x), seg1.ps.y + (t * direction1.y));
        }
    }
}
