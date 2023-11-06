using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Inventarios
{
    [NotMapped]
    public class TraspasoOrder:VArticuloAlmacen
    {
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime FechaOrden { set; get; }

        [Display(Name = "Lote")]
        public string Lote { set; get; }

        [Display(Name = "Almacen Destino")]
        public int AlmacenDestino { set; get; }
        public virtual Almacen Almacenes { get; set; }

        [Display(Name = "Cantidad")]
        public decimal CantidadTraspaso { set; get; }

        [Display(Name = "Observación")]
        public string Observacion { set; get; }
        public virtual ICollection<Almacen> GetAlmacenes { set; get; }
    }
 
}