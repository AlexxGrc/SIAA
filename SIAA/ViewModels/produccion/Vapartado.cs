using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.produccion
{
   
    public class Vapartado
    {
    
      public int orden { get; set; }
        public int idarticulo { get; set; }
        public int idcaracteristica { get; set; }
        public decimal cantidad { get; set; }

        public string Nombre { get; set; }
    }
}