using SIAAPI.Models.Administracion;
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
    [Table("MatrizPrecioCliente")]
    public class MatrizPrecioCliente
    {
        [Key]
        public int IDMatrizPrecio{ get; set;}
        [DisplayName("Cliente")]
        public int IDCliente {get;set;}
        public virtual Clientes Clientes { get; set; }
        [DisplayName("Rango Inferior")]
        public decimal RangInf {get;set;}
        [DisplayName("Rango Superior")]
        public decimal RangSup {get;set;}
        [DisplayName("Precio")]
        public decimal Precio {get;set;}
        [DisplayName("Artículo")]
        public int IDArticulo {get;set;}
        public virtual Articulo Articulo { get; set; }

        public int IDMoneda { get; set; }
        public virtual c_Moneda c_Moneda { get; set; }
    }
 

    public class MatrizPrecioClienteContext :DbContext
    {
        public MatrizPrecioClienteContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<MatrizPrecioClienteContext>(null);
        }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<MatrizPrecioCliente> MPC { get; set; }
        public DbSet<Clientes> Clientes { get; set; }

        public DbSet<c_Moneda> c_Monedas { get; set; }
    }

    [Table("RangoPlaneacionNiveles")]
    public class VRangoPlaneacionNiveles
    {
        [Key]
        public int IDRPN{ get; set; }
        public int IDCliente { get; set; }
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