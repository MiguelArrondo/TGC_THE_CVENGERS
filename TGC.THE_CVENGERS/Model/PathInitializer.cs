using System.Collections.Generic;
using System.Drawing;

namespace TGC.Group.Model
{
    internal class PathInitializer
    {
        public static List<Point> crearPathRojo()
        {
            Point punto = new Point(331, 366);
            List<Point> listaPuntos = new List<Point>();
            listaPuntos.Add(punto);

            for (int i = 332; i < 502; i++)
            {
                Point puntoAux = new Point(i, 366);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 365; i > 82; i--)
            {
                Point puntoAux = new Point(501, i);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 500; i > 113; i--)
            {
                Point puntoAux = new Point(i, 83);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 84; i < 367; i++)
            {
                Point puntoAux = new Point(114, i);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 115; i < 332; i++)
            {
                Point puntoAux = new Point(i, 366);
                listaPuntos.Add(puntoAux);
            }

            return listaPuntos;
        }

        public static List<Point> crearPathAzul()
        {
            Point punto = new Point(331, 366);
            List<Point> listaPuntos = new List<Point>();
            listaPuntos.Add(punto);

            for (int i = 332; i < 502; i++)
            {
                Point puntoAux = new Point(i, 366);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 365; i > 266; i--)
            {
                Point puntoAux = new Point(501, i);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 500; i < 799; i++)
            {
                Point puntoAux = new Point(i, 267);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 266; i > 130; i--)
            {
                Point puntoAux = new Point(798, i);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 797; i > 587; i--)
            {
                Point puntoAux = new Point(i, 131);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 130; i > 82; i--)
            {
                Point puntoAux = new Point(588, i);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 587; i > 113; i--)
            {
                Point puntoAux = new Point(i, 83);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 84; i < 367; i++)
            {
                Point puntoAux = new Point(114, i);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 115; i < 332; i++)
            {
                Point puntoAux = new Point(i, 366);
                listaPuntos.Add(puntoAux);
            }

            return listaPuntos;
        }

        public static List<Point> crearPathVerde()
        {
            Point punto = new Point(331, 366);
            List<Point> listaPuntos = new List<Point>();
            listaPuntos.Add(punto);

            for (int i = 332; i < 502; i++)
            {
                Point puntoAux = new Point(i, 366);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 365; i > 266; i--)
            {
                Point puntoAux = new Point(501, i);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 500; i < 799; i++)
            {
                Point puntoAux = new Point(i, 267);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 266; i < 364; i++)
            {
                Point puntoAux = new Point(798, i);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 799; i < 931; i++)
            {
                Point puntoAux = new Point(i, 363);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 364; i < 608; i++)
            {
                Point puntoAux = new Point(930, i);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 929; i > 842; i--)
            {
                Point puntoAux = new Point(i, 607);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 608; i < 711; i++)
            {
                Point puntoAux = new Point(843, i);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 842; i > 609; i--)
            {
                Point puntoAux = new Point(i, 710);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 709; i > 524; i--)
            {
                Point puntoAux = new Point(610, i);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 609; i > 330; i--)
            {
                Point puntoAux = new Point(i, 525);
                listaPuntos.Add(puntoAux);
            }

            for (int i = 524; i > 367; i--)
            {
                Point puntoAux = new Point(331, i);
                listaPuntos.Add(puntoAux);
            }

            return listaPuntos;
        }
    }
}