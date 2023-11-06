using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Inventarios
{
    public class InventarioView
    {
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime FechaOrden { set; get; }

        [Display(Name ="Numero Orden")]
        public int Orderid { set; get; }

        [Display(Name ="Lote")]
        public string Lote { set; get; }

        [Display(Name ="Almacen Destino")]
        public List <VArticuloAlmacen> Almacen { set; get; }




    }
}