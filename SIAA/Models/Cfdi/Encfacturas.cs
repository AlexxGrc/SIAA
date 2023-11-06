using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SIAAPI.Models.Cfdi
{
    [Table("EncFacturas")]
    public class Encfacturas
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

        public string Estado { get; set; }

        public string IDMetododepago { get; set; }

        public bool ConPagos { get; set; }

        public int IDCliente { get; set; }

        public bool Anticipo { get; set; }

        public bool NotaCredito { get;  set; }

        public int IDTipoComprobante { get; set; }
        public DateTime? FechaRevision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal Descuento { get; set; }

        public virtual ICollection<c_TipoCuota> c_TipoCuota { get; set; }


    }

    [Table("Folio")]

    public class Folio
    {

        [Key]
        public int IDFolio { get; set; }

        [DisplayName("Folio")]
        [Required(ErrorMessage = "El folio es obligatorio")]

        public int Numero { get; set; }

        [DisplayName("Serie")]
        [Required(ErrorMessage = "La serie es obligatoria")]
        [StringLength(2)]
        public string Serie { get; set; }

        public int IDTipoComprobante { get; set; }
        public virtual c_TipoComprobante c_TipoComprobante { get; set; }



    }

    public class FolioContext : DbContext
    {
        public FolioContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<FolioContext>(null);
        }
        public DbSet<Folio> Folios { get; set; }
        public DbSet<c_TipoComprobante> c_TipoComprobantes { get; set; }



    }
    [Table("FolioVentas")]

    public class FolioVentas
    {

        [Key]
        public int IDFolioVentas { get; set; }

        [DisplayName("Folio")]
        [Required(ErrorMessage = "El folio es obligatorio")]

        public int Numero { get; set; }

        [DisplayName("Serie")]
        [Required(ErrorMessage = "La serie es obligatoria")]
        [StringLength(2)]
        public string Serie { get; set; }

        public int IDTipoComprobante { get; set; }
        public virtual c_TipoComprobante c_TipoComprobante { get; set; }



    }
    public class FolioVentasContext : DbContext
    {
        public FolioVentasContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<FolioVentasContext>(null);
        }
        public DbSet<FolioVentas> FoliosV { get; set; }
        public DbSet<c_TipoComprobante> c_TipoComprobantes { get; set; }



    }

    [Table("NotasCredito")]

    public class NotasCredito
    {

        [Key]
        public int IDNota { get; set; }

       
        public int IDFacturaNota { get; set; }

        
        public int IDFacturaRelacionada { get; set; }

        public string EstadoNC { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public int IDDocumentoRelacionado { get; set; }




    }

    public class NotasCreditoContext : DbContext
    {
        public NotasCreditoContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<NotasCreditoContext>(null);
        }
        public DbSet<NotasCredito> NotasCreditos { get; set; }
       



    }
    public class EncfacturaContext : DbContext
    {
        public EncfacturaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Encfacturas> encfacturas { get; set; }
        public DbSet<Folio> Folios { get; set; }

        public DbSet<Empresa> Empresa { get; set; }

        public DbSet<c_TipoRelacion> relaciones { get; set; }
        public DbSet<c_TipoComprobante> tipocomprobantes { get; set; }

        public DbSet<c_FormaPago> FormaPagos { get; set; }

        public DbSet<c_MetodoPago> metodopagos { get; set; }

        public DbSet<c_UsoCFDI> c_UsoCFDIs { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Administracion.c_Moneda> c_Moneda { get; set; }
    }

    public class FoliosRepository
    {
        public IEnumerable<SelectListItem> GetFolios()
        {
            using (var context = new FolioContext())
            {
                List<SelectListItem> lista = context.Folios.AsNoTracking()
                    .OrderBy(n => n.Serie)
                        .Select(n =>
                        new SelectListItem
                        {
                            
                            Value = n.IDFolio.ToString(),
                            Text = n.c_TipoComprobante.ClaveTipoComprobante.ToString() + " | " + n.Serie
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona la serie en donde se va facturar ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }
    }

    public class FoliosNotaCreditoRepository
    {
        public IEnumerable<SelectListItem> GetFolios()
        {
            using (var context = new FolioContext())
            {
                
                List<SelectListItem> lista = context.Folios.AsNoTracking()
                    .OrderBy(n => n.Serie).Where(n=> n.Serie=="N")
                        .Select(n =>
                        new SelectListItem
                        {
                            
                            Value = n.IDFolio.ToString(),
                            Text = n.c_TipoComprobante.ClaveTipoComprobante.ToString() + " | " + n.Serie
                        }).ToList();
                var countrytip = new SelectListItem()
                {

                    Value = null,
                    Text = "--- Selecciona la serie en donde se va facturar ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }
    }




    /* Vista pago */

    public class VPagos
    {
        [Key]

        public int NoParcialidad { get; set; }
        public System.DateTime? FechaPago { get; set; }
		public int FolioP { get; set; }
        public decimal ImporteSaldoInsoluto { get; set; }
		public int IDPagoFactura { get; set; }
        public decimal ImportePagado { get; set; }

        public string StatusDocto { get; set; }
    }


    public class VEncPagos
    {

        [Key]
        public int ID { get; set; }
        public string Nombre_cliente { get; set; }
        public int NoFactura { get; set; }
        public decimal Total { get; set; }
    }


    public class FolioPrefactura
    {
        [Key]
        public int IDFolioVentas { get; set; }

        [DisplayName("Folio")]
        [Required(ErrorMessage = "El folio es obligatorio")]

        public int Numero { get; set; }

        [DisplayName("Serie")]
        [Required(ErrorMessage = "La serie es obligatoria")]
        [StringLength(2)]
        public string Serie { get; set; }

        public int IDTipoComprobante { get; set; }
        public virtual c_TipoComprobante c_TipoComprobante { get; set; }

        public int IDMoneda { get; set; }


        public int IDPedido { get; set; }

       public int IDTipoRelacion { get; set; }

        public int IDUsoCFDI { get; set; }
        public int IDCondicionesPago { get; set; }
        public int IDMetodoPago { get; set; }
        public int IDFormapago { get; set; }
        public string Observacion { get; set; }

    }

    public class CarritoPrefactura
        {
       
        [Key]
          public int IDCarritoPrefactura { get; set; }
          public int IDPedido { get; set; }
     
          public int IDArticulo { get; set; }
          public decimal   Cantidad{ get; set; }
          public decimal  Costo{ get; set; }
          public decimal   CantidadPedida{ get; set; }
          public decimal  Descuento{ get; set; }
          public decimal Importe{ get; set; }
          public bool IVA{ get; set; }
          public decimal ImporteIva{ get; set; }
          public decimal ImporteTotal{ get; set; }
          public string Nota{ get; set; }
          public bool Ordenado { get; set; }
          public int Caracteristica_ID{ get; set; }
          public decimal Suministro{ get; set; }
          public string Status{ get; set; }
          public string Presentacion{ get; set; }
          public string jsonPresentacion{ get; set; }
          public int UserID{ get; set; }
          public int IDDetExterna{ get; set; }
          public string Lote{ get; set; }
}
    [Table("DiferenciaFac")]
    public class DiferenciaFac
    {
        [Key]
        [DisplayName("No Mes")]
        public int IDMes { get; set; }
        [DisplayName("Mes")]
        public string Mes { get; set; }
        [DisplayName("Año Actual MXN")]
        public decimal AnoActualMXN { get; set; }
        [DisplayName("Año Actual USD")]
        public decimal AnoActualUSD { get; set; }
        [DisplayName("Año Actual TotMXN")]
        public decimal AnoActualTotMXN { get; set; }
        [DisplayName("Año Anterior MXN")]
        public decimal AnoAnteriorMXN { get; set; }
        [DisplayName("Año Anterior USD")]
        public decimal AnoAnteriorUSD { get; set; }
        [DisplayName("Año Anterior TotMXN")]
        public decimal AnoAnteriorTotMXN { get; set; }
        [DisplayName("Diferencia MXN")]
        public decimal DiferenciaMXN { get; set; }
    }
    public class DiferenciaFacContext : DbContext
    {
        public DiferenciaFacContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<DiferenciaFac> DiferenciaFacs { get; set; }
    }

    public class EncfacturasSaldos
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
        public DateTime? FechaVencimiento { get; set; }
        public decimal Descuento { get; set; }
        public decimal? ImportePagado { get; set; }
        public decimal? ImporteSaldoInsoluto { get; set; }
        //public string Telefono { get; set; }
      

    }
    public class EncfacturasSaldosContext : DbContext
    {
        public EncfacturasSaldosContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<EncfacturasSaldos> EncfacturasSaldos { get; set; }
    }


    public class EncFacturaOfVen
    {

        [Key]
        public int ID { get; set; }
        public string NoExpediente { get; set; }

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

        public string Estado { get; set; }

        public string IDMetododepago { get; set; }

        public bool ConPagos { get; set; }

        public int IDCliente { get; set; }

        public bool Anticipo { get; set; }

        public bool NotaCredito { get; set; }

        public int IDTipoComprobante { get; set; }
        public DateTime? FechaRevision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal Descuento { get; set; }
        public decimal? ImportePagado { get; set; }
        public decimal? ImporteSaldoInsoluto { get; set; }
      
        public int IDOficina { get; set; }
        public string Oficina { get; set; }
        public int IDVendedor { get; set; }
        public string Vendedor { get; set; }
        public string EstadoCliente { get; set; }

        public string TipoCliente { get; set; }
       
    }
    public class EncFacturaOfVenContext : DbContext
    {
        public EncFacturaOfVenContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<EncFacturaOfVen> EncFacturaOfVen { get; set; }
    }

    public class VEncfacturasSaldosVen
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
        public string Vendedor { get; set; }
        public string NombreOficina { get; set; }

    }
    public class VEncfacturasSaldosVenContext : DbContext
    {
        public VEncfacturasSaldosVenContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VEncfacturasSaldosVen> VEncfacturasSaldosVen { get; set; }
    }

    [Table("MotivosCancelacion")]
    public class MotivosCancelacion
    {
        [Key]
        public int IDCancelacion { get; set; }

        public string ClaveCan { get; set; }
        public string DescripcionCan { get; set; }
        public decimal VersionCFDI { get; set; }
        public DateTime FIVigencia { get; set; }
    }
    [Table("RegistroCancelacionFacturas")]
    public class RegistroCancelacionFacturas
    {
        [Key]
        public int IDRegistro { get; set; }

        public int IDFactura { get; set; }
        public DateTime Fecha { get; set; }
        public int Usuario { get; set; }
        public string FolioFiscal { get; set; }
        public string FViene { get; set; }
        public int Motivo { get; set; }
    }
    public class MotivoCancelacionContext : DbContext
    {
        public MotivoCancelacionContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<MotivosCancelacion> MotivoCancelacions { get; set; }
    }
    [Table("EstadoFacturasSat")]

    public class EstadoFacturasSat
    {

        [Key]
        public int IDEstado { get; set; }

        [DisplayName("Estado")]
        public string Estado { get; set; }

        [DisplayName("No.Factura")]
        public int IDFactura { get; set; }
        [DisplayName("Fecha")]
        public DateTime Fecha { get; set; }
        [DisplayName("Responsable")]
        public int Usuario { get; set; }
    }
    public class EstadoFactSATContext : DbContext
    {
        public EstadoFactSATContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<EstadoFacturasSat> estadoFact { get; set; }
    }
}