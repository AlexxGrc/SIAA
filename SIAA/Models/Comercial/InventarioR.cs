using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Comercial
{
    public class InventarioR
    {
       
        public int IDInventario { get; set; }

       
       public string CRef { get; set; }
      
        public string Articulo { get; set; }
        public string TipoArticulo { get; set; }

        public decimal Cantidad { get; set; }

        public decimal Costo { get; set; }
        public decimal Precio { get; set; }

    }

  
}
