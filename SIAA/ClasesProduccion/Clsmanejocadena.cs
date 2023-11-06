using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAAPI.ClasesProduccion
{
    public class Clsmanejocadena
    {
      
        public double getdouble(string variable, string cadenadep)
        {
           string  cadenadepresentacion = cadenadep;
            double retorno = 0;
            try
            {
                string[] cadenas = cadenadepresentacion.Split(',');

                foreach (var valorobject in cadenas)
                {
                    string[] cadeuni = valorobject.Split(':');
                    string llave = cadeuni[0].Trim();
                    string valor = cadeuni[1].Trim();
                    if (llave == variable)
                    {

                        retorno = double.Parse(valor);
                        return retorno;
                    }
                }


            }
            catch (Exception err)
            {
                String error = err.Message;
            }
            return retorno;
        }

        public Int16 getint(string variable, string cadenadep)
        {
            string cadenadepresentacion = cadenadep;
            Int16 retorno = 0;
            try
            {
                string[] cadenas = cadenadepresentacion.Split(',');

                foreach (var valorobject in cadenas)
                {
                    string[] cadeuni = valorobject.Split(':');
                    string llave = cadeuni[0].Trim();
                    string valor = cadeuni[1].Trim();
                    if (llave == variable)
                    {

                        retorno = Int16.Parse(Math.Round(decimal.Parse(valor),0).ToString());
                        return retorno;
                    }
                }


            }
            catch (Exception err)
            {
                String error = err.Message;
            }
            return 0;
        }


        public bool getbool(string variable, string cadenadep)
        {
           string  cadenadepresentacion = cadenadep;
          
            try
            {
                string[] cadenas = cadenadepresentacion.Split(',');

                foreach (var valorobject in cadenas)
                {
                    string[] cadeuni = valorobject.Split(':');
                    string llave = cadeuni[0].Trim();
                    string valor = cadeuni[1].Trim();
                    if (llave == variable)
                    {
                        if (valor == "false")
                        { return false; }
                        else
                        {
                            return true;
                        }

                       
                    }
                }


            }
            catch (Exception err)
            {
                String error = err.Message;
            }
            return false;
        }

        public string getstring(string variable, string cadenadep)
        {
            string cadenadepresentacion = cadenadep;

            try
            {
                string[] cadenas = cadenadepresentacion.Split(',');

                foreach (var valorobject in cadenas)
                {
                    string[] cadeuni = valorobject.Split(':');
                    string llave = cadeuni[0].Trim();
                    string valor = cadeuni[1].Trim();
                    if (llave == variable)
                    {
                        return valor;


                    }
                }


            }
            catch (Exception err)
            {
                String error = err.Message;
            }
            return "";
        }



    }
}