using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAAPI.clasescfdi
{
    public class ClsOperacionesFecha
    {
        public DateTime fechaini { get; set; }
        public DateTime fechafin { get; set; }
        TimeSpan span;
        public int idCondicionesPagp { get; set; }
        public int getDiferencia()
        {            
            span = fechafin.Subtract(fechaini);
            return (int)span.TotalDays;
        }


        public bool get7()
        {
            bool dato = false;
            if (getDiferencia() >= 1 && getDiferencia() <= 7)
            {
                dato = true;
            }


            return dato;
        }

        public bool get814()
        {
            bool dato = false;
            if (getDiferencia() >= 8 && getDiferencia() <= 14)
            {
                dato = true;
            }


            return dato;
        }

        public bool get1421()
        {
            bool dato = false;
            if (getDiferencia() >= 14 && getDiferencia() <= 21)
            {
                dato = true;
            }


            return dato;
        }

        public bool get2230()
        {
            bool dato = false;
            if (getDiferencia() >= 22 && getDiferencia() <= 30)
            {
                dato = true;
            }


            return dato;
        }

        public bool get30()
        {
            bool dato = false;
           if (getDiferencia() >= 1  && getDiferencia() <= 30)
            {
                dato = true;
            }


            return dato;
        }
        public bool get3160()
        {
            bool dato = false;
            if (getDiferencia() >= 31 && getDiferencia() <= 60)
            {
                dato = true;
            }
            
            return dato;
        }
        public bool get6190()
        {
            bool dato = false;
            if (getDiferencia() >= 61 && getDiferencia() <= 90)
            {
                dato = true;
            }
           
            return dato;
        }
        public bool get91mas()
        {
            bool dato = false;
            if (getDiferencia() >= 91)
            {
                dato = true;
            }
          
            return dato;
        }
        public bool get3190()
        {
            bool dato = false;
            if (getDiferencia() >= 31 && getDiferencia() <= 90)
            {
                dato = true;
            }
           
            return dato;
        }

        public bool getcorriente()
        {
            bool dato = false;
            if (getDiferencia() <=0)
            {
                dato = true;
            }

            return dato;
        }

        public DateTime getNuevaFecha(DateTime fecha, int dias)
        {
            DateTime answer = fecha.AddDays(dias);
            return answer;
        }

    }
}