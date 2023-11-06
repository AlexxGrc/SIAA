using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Comercial
{
    [Table("VSuajesCompras")]
    public class VSuajesCompras
    {
        [Key]
        public int IDOrdenCompra { get; set; }
        public DateTime Fecha { get; set; }
        public decimal TipoCambio { get; set; }
        public string ClaveMoneda { get; set; }
        public string Empresa { get; set; }
        public int IDArticulo { get; set; }
        public string Cref { get; set; }
        public string Descripcion { get; set; }
        public decimal Costo { get; set; }
        public decimal Cantidad { get; set; }
        public string Observacion{ get; set; }
        public string Status { get; set; }
    }
    public class VSuajesComprasContext : DbContext
    {
        public VSuajesComprasContext() : base("name=DefaultConnection")
        {

        }
        public virtual DbSet<VSuajesCompras> VSuajesComprass { get; set; }
    }

    [Table("VSuajesFacturas")]
    public class VSuajesFacturas
    {
        [Key]
        public int IDPrefactura { get; set; }
        public int IDFacturaDigital { get; set; }
        public string SerieDigital { get; set; }
        public string NumeroDigital { get; set; }
        public DateTime Fecha { get; set; }
        public decimal TC { get; set; }
        public string ClaveMoneda { get; set; }
        public string Nombre_Cliente { get; set; }
        public int IDArticulo { get; set; }
        public string Cref { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Costo { get; set; }
        public decimal Importe { get; set; }
        public string Observacion { get; set; }
    }
    public class VSuajesFacturasContext : DbContext
    {
        public VSuajesFacturasContext() : base("name=DefaultConnection")
        {

        }
        public virtual DbSet<VSuajesFacturas> VSuajesFacturass { get; set; }
    }

    [Table("VClienteArticuloCompra")]
    public class VClienteArticuloCompra
    {
        [Key]
        public int IDPrefactura { get; set; }
        public int IDFacturaDigital { get; set; }
        public string SerieDigital { get; set; }
        public string NumeroDigital { get; set; }
        public DateTime Fecha { get; set; }
        public int IDCliente { get; set; }
        public string Nombre_Cliente { get; set; }
        public int IDArticulo { get; set; }
        public string Cref { get; set; }
        public string Descripcion { get; set; }
        public string Presentacion { get; set; }
        public decimal Cantidad { get; set; }
    }
    public class VClienteArticuloCompraContext : DbContext
    {
        public VClienteArticuloCompraContext() : base("name=DefaultConnection")
        {

        }
        public virtual DbSet<VClienteArticuloCompra> VClienteArticuloComprass { get; set; }
    }
}