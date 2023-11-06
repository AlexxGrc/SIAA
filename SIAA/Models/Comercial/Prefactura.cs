using SIAAPI.Models.Administracion;
using SIAAPI.Models.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Models.Comercial
{
    [Table("EncPrefactura")]
    public class EncPrefactura
    {
        [Key]
        public int IDPrefactura { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime Fecha { get; set; }

        [DisplayName("Documento Factura")]
        [Required]
        public string DocumentoFactura { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }

        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }

        [DisplayName("Cliente")]
        [Required]
        public int IDCliente { get; set; }
        public virtual Clientes Clientes { get; set; }
        [DisplayName("Método de Pago")]
        [Required]
        public int IDMetodoPago { get; set; }
        public virtual c_MetodoPago c_MetodoPago { get; set; }
        [DisplayName("Forma de Pago")]
        [Required]
        public int IDFormapago { get; set; }
        public virtual c_FormaPago c_FormaPago { get; set; }
        [DisplayName("Divisa")]
        [Required]
        public int IDMoneda { get; set; }
        public virtual c_Moneda c_Moneda { get; set; }
        [DisplayName("Condiciones de Pago")]
        [Required]
        public int IDCondicionesPago { get; set; }
        public virtual CondicionesPago CondicionesPago { get; set; }
        [DisplayName("Almacen")]
        [Required]
        public int IDAlmacen { get; set; }
        public virtual Almacen Almacen { get; set; }
        [DisplayName("Uso CFDI")]
        public int IDUsoCFDI { get; set; }
        public virtual c_UsoCFDI c_UsoCFDI { get; set; }

        public decimal TipoCambio { get; set; }
        public string Status { get; set; }
        [DisplayName("Usuario")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
        [DisplayName("Vendedor")]
        public int IDVendedor { get; set; }
        public virtual Vendedor Vendedor { get; set; }

        [DisplayName("Serie")]
        public string Serie { get; set; }
        [DisplayName("Numero")]
        public int Numero { get; set; }
        [DisplayName("Factura Digital")]
        public int IDFacturaDigital { get; set; }
        [DisplayName("Serie Digital")]
        public string SerieDigital { get; set; }
        [DisplayName("Numero Digital")]
        public string NumeroDigital { get; set; }
        [DisplayName("UUID")]
        public string UUID { get; set; }
        //[DisplayName("Factura Anticipo")]
        //public int IDFacturaAnticipo { get; set; }
        //[DisplayName("UUID Anticipo")]
        //public string UUIDAnticipo { get; set; }
        [DisplayName("Entrega")]
        public string Entrega { get; set; }
        [DisplayName("Tipo de Relación")]
        public int IDTipoRelacion { get; set; }
        public virtual c_TipoRelacion c_TipoRelacion { get; set; }

        [DisplayName("Descuento x Anticipo")]
        public decimal Descuento { get; set; }

        
    }
    [Table("DetPrefactura")]
    public class DetPrefactura
    {
        [Key]
        public int IDDetPrefactura { get; set; }
        public int IDPrefactura { get; set; }
        public virtual EncPrefactura Prefactura { get; set; }

        [DisplayName("Pedido")]
        [Required]
      
       
        public int IDExterna { get; set; }
        public int IDDetExterna { get; set; }
        [DisplayName("Artículo")]
        [Required]
        public int IDArticulo { get; set; }
        public virtual Articulo Articulo { get; set; }
        [DisplayName("Característica")]
        [Required]
        public int Caracteristica_ID { get; set; }
        [DisplayName("Cantidad")]
        [Required]
        public decimal Cantidad { get; set; }
        [DisplayName("Costo")]
        [Required]
        public decimal Costo { get; set; }
        [DisplayName("Cantidad Pedida")]
        [Required]
        public decimal CantidadPedida { get; set; }
        [DisplayName("Descuento")]
        public decimal Descuento { get; set; }
        [DisplayName("Importe")]
        [Required]
        public decimal Importe { get; set; }
        [DisplayName("IVA")]
        [Required]
        public bool IVA { get; set; }
        [DisplayName("Importe IVA")]
        [Required]
        public decimal ImporteIva { get; set; }
        [DisplayName("Importe Total")]
        [Required]
        public decimal ImporteTotal { get; set; }
        [DisplayName("Nota")]
        [Required]
        public string Nota { get; set; }
        [DisplayName("Ordenado")]
        [Required]
        public bool Ordenado { get; set; }
        [DisplayName("Suministro")]
        [Required]
        public decimal Suministro { get; set; }

        public string Status { get; set; }
        public string Lote { get; set; }

        public decimal Devolucion { get; set; }
        public int IDAlmacen{ get; set; }
        public virtual Almacen Almacen { get; set; }

     public string presentacion { get; set; }
       
      public string jsonPresentacion { get; set; }

        public string Proviene { get; set; }

    }


    [Table("elementosprefactura")]
    public class elementosprefactura
    {
        [Key]
        public int idelementos { get; set; }
        public string documento { get; set; }
        public int iddocumento { get;  set; }
        public int iddetdocumento { get; set; }
    
        public decimal cantidad { get; set; }
        public int iddetprefactura { get; set; }
        public int idprefactura { get; set; }

    }

    [Table("PrefacturaAnticipo")]
    public class PrefacturaAnticipo
    {
        [Key]
        public int IDPrefacturaAnticipo { get; set; }
        [DisplayName("No. Prefactura")]
        public int IDPrefactura { get; set; }
        [DisplayName("No. Factura")]
        public int IDFacturaAnticipo { get; set; }
        [DisplayName("UUID de Anticipo")]
        public string UUIDAnticipo { get; set; }
        
     
    }
    public class PrefacturaContext : DbContext
    {

        public PrefacturaContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<PrefacturaContext>(null);
        }
        //Clases internas
        public DbSet<EncPrefactura> EncPrefactura { get; set; }
        public DbSet<DetPrefactura> DetPrefactura { get; set; }
        //Clases externas 
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<c_MetodoPago> c_MetodoPagos { get; set; }
        public DbSet<c_FormaPago> c_FormaPagos { get; set; }
        public DbSet<c_Moneda> c_Monedas { get; set; }
        public DbSet<CondicionesPago> CondicionesPagos { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<Caracteristica> Caracteristica { get; set; }

        public DbSet<PrefacturaAnticipo> PrefacturaAnticipo { get; set; }

        public DbSet<elementosprefactura> elementosprefacturas { get; set; }

    }
}