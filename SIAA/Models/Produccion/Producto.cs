using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Produccion
{

    [Table("ValeProducto")]
    public class ValeProducto
    {
        [Key]
        public int IDValeProducto { get; set; }
        [DisplayName("Fecha ")]
        public DateTime Fecha { get; set; }
        [DisplayName("Fecha Devolución")]
        public Nullable<DateTime> FechaDevolucion { get; set; }
        public int IDArticulo { get; set; }
        public virtual Articulo Articulo { get; set; }
        public int IDCaracteristica { get; set; }
        //public virtual Caracteristica Caracteristica { get; set; }
        public int IDProceso { get; set; }
        public virtual Proceso Proceso { get; set; }
        public int IDOrden { get; set; }
        public virtual OrdenProduccion OrdenProduccion { get; set;}
        public decimal Cantidad {get;set;}
        public int IDClaveUnidad { get; set; }
        public virtual c_ClaveUnidad Unidad { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public int IDTrabajador { get; set; }
        public virtual Trabajador Trabajador { get; set; }
        public decimal Devuelto { get; set; }
        public int IDAlmacen { get; set; }
        public virtual Almacen Almacen { get; set; }
    }
    public class ValeProductoContext : DbContext
    {
        public ValeProductoContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ValeProductoContext>(null);
        }
        public DbSet<ValeProducto> ValeProductos { get; set; }

    }
   
}

