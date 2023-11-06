using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Comercial
{
    [Table("MatrizPrecioProveedor")]
    public class MatrizPrecioProveedor
    {
        [Key]
        public int IDMatrizPrecio{ get; set;}
        [DisplayName("Proveedor")]
        public int IDProveedor {get;set;}
        public virtual Proveedor Proveedor { get; set; }
        [DisplayName("Rango Inferior")]
        public decimal RangInf {get;set;}
        [DisplayName("Rango Superior")]
        public decimal RangSup {get;set;}
        [DisplayName("Precio")]
        public decimal Precio {get;set;}
        [DisplayName("Artículo")]
        public int IDArticulo {get;set;}

        public int IDMoneda { get; set; }
        public virtual Articulo Articulo { get; set; }
    }
 

    public class MatrizPrecioProveedorContext :DbContext
    {
        public MatrizPrecioProveedorContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<MatrizPrecioClienteContext>(null);
        }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<MatrizPrecioProveedor> MPP { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
    }

    [Table("RangoPlaneacionNivelesProv")]
    public class VRangoPlaneacionNivelesProv
    {
        [Key]
        public int IDRPN{ get; set; }
        public int IDProveedores { get; set; }
        public int IDMoneda { get; set; }
        public decimal TC { get; set; }
        public decimal RangoInf { get; set; }
        public decimal RangoSup { get; set; }
        public decimal Costo { get; set; }
        public decimal Porcentaje { get; set; }
        public string Descripcion { get; set; }
        public string Presentacion { get; set; }
        public int IDArticulo { get; set; }
        public int IDCaracteristica { get; set; }
        public string nameFoto { get; set; }
    }
}