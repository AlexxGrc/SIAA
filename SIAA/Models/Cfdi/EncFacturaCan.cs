using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

using System.Web.Mvc;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;



namespace SIAAPI.Models.Cfdi
{
    [Table("EncFacturaCan")]
    public class EncFacturaCan
    {
        [Key]
        public int IDCan { get; set; }
        public int IDFactura { get; set; }
        public string EstadoFactura { get; set; }
        public DateTime? FechaConsulta { get; set; }
        public string EstadoCFDI { get; set; }
      
        public int IDUsuarioConsulta { get; set; }
    }
    [Table("AcuseCancelacionF")]
    public class AcuseCancelacionF
    {
        [Key]
        public int IDAcuse { get; set; }
        public int IDFactura { get; set; }
        public string Acuse { get; set; }
        public DateTime? Fecha { get; set; }
     
    }
    public class AcuseCancelacionFContext : DbContext
    {
        public AcuseCancelacionFContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<AcuseCancelacionF> AcuseCancelacionFac { get; set; }

    }
    public class EncFacturaCanContext : DbContext
    {
        public EncFacturaCanContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<EncFacturaCan> EncFacturaCan { get; set; }

    }
    [Table("VEncFacturaCan")]
    public class VEncFacturaCan
    {
        [Key]
        public int ID { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Serie { get; set; }
        public int Numero { get; set; }
        public string Nombre_cliente { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        [Required(ErrorMessage = "La factura requiere un UUID")]
        public string UUID { get; set; }
        public string RutaXML { get; set; }
        public bool pagada { get; set; }
        public decimal TC { get; set; }
        public string Moneda { get; set; }
        public int IDMoneda { get; set; }
        public virtual c_Moneda c_Moneda { get; set; }
        public string Prefactura { get; set; }
        public string Estado { get; set; }
        public string IDMetododepago { get; set; }
        public bool ConPagos { get; set; }
        public int IDCliente { get; set; }
        public bool Anticipo { get; set; }
        public bool NotaCredito { get; set; }
        public int IDTipoComprobante { get; set; }
        public DateTime? FechaRevision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal Descuento { get; set; }
        public decimal? ImportePagado { get; set; }
        public decimal? ImporteSaldoInsoluto { get; set; }
        public string Telefono { get; set; }
    }

    public class VEncFacturaCanContext : DbContext
    {
        public VEncFacturaCanContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VEncFacturaCan> VEncFacturaCan { get; set; }
    }
}