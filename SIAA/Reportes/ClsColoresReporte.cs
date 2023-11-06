using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace SIAAPI.Reportes
{
    public class ClsColoresReporte
    {
       public CMYKColor color { get; set; }

        public ClsColoresReporte (string _color)
        {
            if(_color== "Cafe")
            {
                color = new CMYKColor(0, 29, 50, 70);
            }
            if (_color == "Morado claro")
                {
                    color = new CMYKColor(100, 100, 0, 41);
                }
            if (_color == "Azul")
            {
                Color tem = Color.Blue;

                color = ConvertRgbToCmyk(tem.R, tem.G, tem.B);

            }
            if (_color == "Azul oscuro")
            {
                Color tem = Color.DarkBlue;
                
                color = ConvertRgbToCmyk(tem.R,tem.G,tem.B);
            }
            if (_color == "Marron dorado")
            {
                color = ConvertRgbToCmyk(153, 101, 21);
            }
            if (_color == "Rojo")
            {
                color = ConvertRgbToCmyk(254, 0, 0);
            }

            if (_color == "Negro")
            {
                color = ConvertRgbToCmyk(0, 0, 0);
            }
            if (_color == "Blanco")
            {
                color = ConvertRgbToCmyk(255, 255, 255);
            }
        }

        public ClsColoresReporte(Color _color)
        {
           
                Color tem = Color.DarkBlue;

                color = ConvertRgbToCmyk(_color.R, _color.G, _color.B);
      


        }
        public static CMYKColor ConvertRgbToCmyk(int r, int g, int b)
        {
            float c;
            float m;
            float y;
            float k;
            float rf;
            float gf;
            float bf;

            rf = r / 255F;
            gf = g / 255F;
            bf = b / 255F;

            k = ClampCmyk(1 - Math.Max(Math.Max(rf, gf), bf));
            c = ClampCmyk((1 - rf - k) / (1 - k));
            m = ClampCmyk((1 - gf - k) / (1 - k));
            y = ClampCmyk((1 - bf - k) / (1 - k));

            return new CMYKColor(c, m, y, k);
        }

        private static float ClampCmyk(float value)
        {
            if (value < 0 || float.IsNaN(value))
            {
                value = 0;
            }

            return value;
        }
    }
}