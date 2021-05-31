using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASD
{
    public class Lab12 : System.MarshalByRefObject
    {
        // etap 1

        /// <summary>
        /// Verify if a given polygon is y-monotone
        /// </summary>
        /// <param name="points">An array of points defining polygon, given in clock-wise manner.</param>
        /// <returns>
        ///     true - a given polygon is y-monotone, otherwise false is returned
        /// </returns>
        public bool IsYMonotonePolygon((double, double)[] points)
        {
            int counter = 0;
            for (int i = 0; i < points.Length; i++)
            {
                if (i < points.Length - 2)
                {
                    if (points[i + 1].Item2 < points[i].Item2 && points[i + 1].Item2 < points[i + 2].Item2)
                        counter++;
                    if (counter > 1)
                        return false;
                }
                else if (i == points.Length - 2)
                {
                    if (points[i + 1].Item2 < points[i].Item2 && points[i + 1].Item2 < points[0].Item2)
                        counter++;
                    if (counter > 1)
                        return false;
                }
                else if (i == points.Length - 1)
                {
                    if (points[0].Item2 < points[i].Item2 && points[0].Item2 < points[1].Item2)
                        counter++;
                    if (counter > 1)
                        return false;
                }
            }

            int minIdx = -1;
            int maxIdx = -1;
            double min = double.MaxValue;
            double max = double.MinValue;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].Item2 > max)
                {
                    max = points[i].Item2;
                    maxIdx = i;
                }
                if (points[i].Item2 < min)
                {
                    min = points[i].Item2;
                    minIdx = i;
                }
            }

            int idx = maxIdx;
            double before = double.MinValue;
            while (idx != minIdx)
            {
                if (points[idx].Item1 < before)
                    return false;
                before = points[idx].Item1;
                idx++;
                if (idx == points.Length)
                    idx = 0;
            }

            return true;
        }

        // etap 2

        private double CrossLike((double, double) p1, (double, double) p2, (double, double) p3)
        {
            return (p2.Item2 - p1.Item2) * (p3.Item1 - p2.Item1) + (p1.Item1 - p2.Item1) * (p3.Item2 - p2.Item2);
        }

        /// <summary>
        /// Function triangulating a given edge-visible polygon.
        /// </summary>
        /// <param name="points">An array of points defining polygon, given in clock-wise manner.</param>
        /// <param name="edge_i">An index of point which is defining an edge from which a polygon is visible.</param>
        /// <param name="edge_j">An index of point which is defining an edge from which a polygon is visible.</param>
        /// <returns>
        ///     A list of 3-length arrays defining triangles. An order of points in triangle does not matter.
        /// </returns>
        public List<(double, double)[]> TriangulationForEdgeVisiblePolygon((double, double)[] points, int edge_i, int edge_j)
        {
            List<(double, double)[]> lista = new List<(double, double)[]>();
            int i = 0;
            List<(double, double)> pom = new List<(double, double)>();

            for (int j = edge_i; j < points.Length; j++)
                pom.Add(points[j]);
            for (int k = 0; k < edge_i; k++)
                pom.Add(points[k]);

            // nalezy zalozyc, ze wierzcholki podane sa zgodnie z ruchem wskazowek zegara
            while (true)
            {
                if(i == -1)
                {
                    i = TriangulationMov(points, pom, lista, edge_i, edge_j, pom.Count - 1, 0, 1, i);
                    if (i == int.MaxValue)
                    {
                        return lista;
                    }
                }
                else if(i == -2)
                {
                    i = TriangulationMov(points, pom, lista, edge_i, edge_j, pom.Count - 2, pom.Count - 1, 0, i);
                    if (i == int.MaxValue)
                    {
                        return lista;
                    }  
                }
                else if(i == -3)
                {
                    i = pom.Count - 3;
                    i = TriangulationMov(points, pom, lista, edge_i, edge_j, pom.Count - 3, pom.Count - 2, pom.Count - 1, i);
                    if (i == int.MaxValue)
                    {
                        return lista;
                    }

                }
                else if (i == pom.Count - 1)
                {
                    i = TriangulationMov(points, pom, lista, edge_i, edge_j, pom.Count - 1, 0, 1, i);
                    if (i == int.MaxValue)
                    {
                        return lista;
                    }
                    if (i == pom.Count)
                        i = 0;
                }
                else if (i == pom.Count - 2)
                {
                    i = TriangulationMov(points, pom, lista, edge_i, edge_j, pom.Count - 2, pom.Count - 1, 0, i);
                    if (i == int.MaxValue)
                    {
                        return lista;
                    }
                } 
                else
                {
                    i = TriangulationMov(points, pom, lista, edge_i, edge_j, i, i + 1, i + 2, i);
                    if (i == int.MaxValue)
                    {
                        return lista;
                    }

                }
            }
        }

        public int TriangulationMov((double, double)[] points, List<(double, double)> pom, List<(double, double)[]> lista, int edge_i, int edge_j, int x1, int x2, int x3, int i)
        {
            if (CrossLike(pom[x1], pom[x2], pom[x3]) <= 0)
            {
                return i + 1;
            }
            else
            {
                lista.Add(new (double,double)[3] { pom[x1], pom[x2], pom[x3] });

                if ((pom[x1].Equals(points[edge_i]) && pom[x2].Equals(points[edge_j])) || (pom[x1].Equals(points[edge_j]) && pom[x2].Equals(points[edge_i]))
                || (pom[x1].Equals(points[edge_i]) && pom[x3].Equals(points[edge_j])) || (pom[x1].Equals(points[edge_j]) && pom[x3].Equals(points[edge_i]))
                || (pom[x2].Equals(points[edge_i]) && pom[x3].Equals(points[edge_j])) || (pom[x2].Equals(points[edge_j]) && pom[x3].Equals(points[edge_i])))
                    return int.MaxValue;

                pom.RemoveAt(x2);

                if (pom[x1].Equals(points[edge_i]))
                    return 0;
                else
                    return i - 1;
            }
        }

        // etap 3

        /// <summary>
        /// Function triangulating a given y-monotone polygon.
        /// </summary>
        /// <param name="points">An array of points defining polygon, given in clock-wise manner.</param>
        /// <returns>
        ///     A list of 3-length arrays defining triangles. An order of points in triangle does not matter.
        /// </returns>
        public List<(double, double)[]> TriangulationForYMonotonePolygon((double, double)[] points)
        {
            // nalezy zalozyc, ze wierzcholki podane sa zgodnie z ruchem wskazowek zegara
            List<(double, double)> C1 = new List<(double, double)>();
            List<(double, double)> C2 = new List<(double, double)>();
            List<(double, double)> CM = new List<(double, double)>();
            List<(double, double)[]> OutList = new List<(double, double)[]>();

            int idx;
            int minIdx = -1;
            int maxIdx = -1;
            double min = double.MaxValue;
            double max = double.MinValue;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].Item2 > max)
                {
                    max = points[i].Item2;
                    maxIdx = i;
                }
                if (points[i].Item2 < min)
                {
                    min = points[i].Item2;
                    minIdx = i;
                }
            }

            idx = minIdx;
            while (idx != maxIdx)
            {
                C1.Add(points[idx]);
                idx++;
                if (idx == points.Length)
                    idx = 0;
            }

            idx = maxIdx;
            while (idx != minIdx)
            {
                C2.Add(points[idx]);
                idx++;
                if (idx == points.Length)
                    idx = 0;
            }

            int C1_c = C1.Count;
            int C2_c = C2.Count;

            int[] tab = new int[C1.Count + C2.Count];
            int j = 0;

            int idxC1 = 0;
            int idxC2 = C2.Count - 1;
            while (C1_c != 0 || C2_c != 0)
            {
                if (C1_c == 0)
                {
                    for (int i = idxC2; i > -1; i--)
                    {
                        CM.Add(C2[i]);
                        C2_c--;
                        tab[j] = 2;
                        j++;
                    }
                }
                else if (C2_c == 0)
                {
                    for (int i = idxC1; i < C1.Count; i++)
                    {
                        CM.Add(C1[i]);
                        C1_c--;
                        tab[j] = 1;
                        j++;
                    }
                }
                else if (C1[idxC1].Item2 < C2[idxC2].Item2)
                {
                    CM.Add(C1[idxC1]);
                    idxC1++;
                    C1_c--;
                    tab[j] = 1;
                    j++;
                }
                else
                {
                    CM.Add(C2[idxC2]);
                    idxC2--;
                    C2_c--;
                    tab[j] = 2;
                    j++;
                }
            }

            int before = 0;
            int num = tab[tab.Length - 2];

            if (num == 1)
                tab[tab.Length - 1] = 2;
            else
                tab[tab.Length - 1] = 1;

            for (int i = 1; i < CM.Count - 1; i++)
            {
                if (tab[i] == 1 && tab[i + 1] == 2)
                {
                    var pom = CM.Skip(before).Take(i + 2 - before).ToArray();
                    before = i;
                    var pom2 = TriangulationForEdgeVisiblePolygon(pom, 0, pom.Length - 1);
                    OutList.AddRange(pom2);
                }
                else if (tab[i] == 2 && tab[i + 1] == 1)
                {
                    var pom = CM.Skip(before).Take(i + 2 - before).Reverse().ToArray();
                    before = i;
                    var pom2 = TriangulationForEdgeVisiblePolygon(pom, 0, pom.Length - 1);
                    OutList.AddRange(pom2);
                }
            }

            return OutList;
        }
    }
}
