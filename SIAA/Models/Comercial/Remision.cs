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
    [Table("EncRemision")]
    public class EncRemision
    {

        [Key]
        public int IDRemision { get; set; }
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

        //public IList<SelectListItem> IDMetodoPago { get; set; }
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
        [DisplayName("Usuario")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
        [DisplayName("Vendedor")]
        public int IDVendedor { get; set; }
        public virtual Vendedor Vendedor { get; set; }
        public decimal TipoCambio { get; set; }
        public string Status { get; set; }
        [DisplayName("Entrega")]
        public string Entrega { get; set; }

    }
    [Table("DetRemision")]
    public class DetRemision
    {
        [Key]
        public int IDDetRemision { get; set; }
        [DisplayName("Pedido")]
        [Required]
        public int IDRemision { get; set; }
        public virtual EncRemision Remision { get; set; }
        public int IDExterna { get; set; }
        public int IDDetExterna { get; set; }
        [DisplayName("Artículo")]
        [Required]
        public int IDArticulo { get; set; }
        public virtual Articulo Articulo { get; set; }
        [DisplayName("Característica")]
        [Required]
        public int Caracteristica_ID { get; set; }
        //public virtual Caracteristica Caracteristica { get; set; }
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

        public int IDAlmacen { get; set; }
        public virtual Almacen Almacen { get; set; }
        [DisplayName("Presentación")]
        [Required]
        public string Presentacion { get; set; }
        [DisplayName("jsonPresentación")]
        [Required]
        public string jsonPresentacion { get; set; }

        public string Lote { get; set; }

        public decimal Devolucion { get; set; }
    }
    [Table("RemisionMP")]
    public class RemisionMP
    {
        [Key]
        public int IDRemisionMP { get; set; }
        public int IDRemision { get; set; }
        public int IDDetRemision { get; set; }

        public string LoteInterno { get; set; }
    }
    public class DetDevolucionR
    {
        [Key]
        public int IDDetDevolucionR { get; set; }
        [DisplayName("Devolución")]
        public int IDDevolucionR { get; set; }
        public virtual EncDevolucionR DevolucionR { get; set; }
        public int IDRemision { get; set; }
        public virtual EncRemision Remision { get; set; }
        public int IDDetRemision { get; set; }
        [DisplayName("Artículo")]
        [Required]
        public int IDArticulo { get; set; }
        public virtual Articulo Articulo { get; set; }
        public int IDAlmacen { get; set; }
        public virtual Almacen Almacen { get; set; }
        [DisplayName("Característica")]
        [Required]
        public int Caracteristica_ID { get; set; }
        public virtual Caracteristica Caracteristica { get; set; }
        [DisplayName("Cantidad")]
        [Required]
        public decimal Cantidad { get; set; }
        [DisplayName("Costo")]
        [Required]
        public decimal Costo { get; set; }


        [DisplayName("Importe")]
        [Required]
        public decimal Importe { get; set; }

        [DisplayName("Importe IVA")]
        [Required]
        public decimal ImporteIva { get; set; }
        [DisplayName("Importe Total")]
        [Required]
        public decimal ImporteTotal { get; set; }
        [DisplayName("Nota")]
        [Required]
        public string Nota { get; set; }

        public string Status { get; set; }
        public string Lote { get; set; }
    }


    [Table("EncDevolucionR")]
    public class EncDevolucionR
    {

        [Key]
        public int IDDevolucionR { get; set; }
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
        [DisplayName("Usuario")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
        [DisplayName("Vendedor")]
        public int IDVendedor { get; set; }
        public virtual Vendedor Vendedor { get; set; }
        public decimal TipoCambio { get; set; }
        public string Status { get; set; }


    }
    public class RemisionContext : DbContext
    {

        public RemisionContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<RemisionContext>(null);
        }
        //Clases internas
        public DbSet<EncRemision> EncRemisiones { get; set; }
        public DbSet<RemisionMP> RemisionesMP { get; set; }
        public DbSet<DetRemision> DetRemisiones { get; set; }
        public DbSet<EncDevolucionR> EncDevolucioneRs { get; set; }
        public DbSet<DetDevolucionR> DetDevolucioneRs { get; set; }
        //Clases externas 
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<c_MetodoPago> c_MetodoPagos { get; set; }
        public DbSet<c_FormaPago> c_FormaPagos { get; set; }
        public DbSet<c_Moneda> c_Monedas { get; set; }
        public DbSet<CondicionesPago> CondicionesPagos { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<Caracteristica> Caracteristica { get; set; }
    }
}

public class VRemisionClie
{

    [Key]
    [Display(Name = "IDRemision")]
    public int IDRemision { get; set; }
    [Display(Name = "Fecha Remision")]
    public DateTime FechaRemision { get; set; }
    [Display(Name = "IDCliente")]
    public int IDCliente { get; set; }
    [Display(Name = "No. Expediente")]
    public string noExpediente { get; set; }
    [Display(Name = "Cliente")]
    public String Nombre { get; set; }

    [Display(Name = "subtotal")]
    public Decimal Subtotal { get; set; }
    [Display(Name = "IVA")]
    public Decimal IVA { get; set; }

    [Display(Name = "Total")]
    public Decimal Total { get; set; }
    [Display(Name = "IDMoneda")]
    public int IDMoneda { get; set; }
    [Display(Name = "Moneda")]
    public string ClaveMoneda { get; set; }

    [Display(Name = "TipoCambio")]
    public decimal TipoCambio { get; set; }
    [Display(Name = "Total Pesos")]
    public Decimal TotalPesos { get; set; }
    [Display(Name = "Estado Remision")]
    public String EstadoRemision { get; set; }
    [Display(Name = "DocumentoFactura")]
    public String DocumentoFactura { get; set; }
    [Display(Name = "IDVendedor")]
    public int IDVendedor { get; set; }

    [Display(Name = "Vendedor")]
    public String Vendedor { get; set; }
    [Display(Name = "IDOficina")]
    public int IDOficina { get; set; }

    [Display(Name = "Oficina")]
    public String Oficina { get; set; }
    [Display(Name = "Observacion")]
    public string Observacion { get; set; }
    [Display(Name = "Entrega")]
    public string Entrega { get; set; }

}
public class VRemisionClieContext : DbContext
{
    public VRemisionClieContext() : base("name=DefaultConnection")
    {

    }
    public DbSet<VRemisionClie> VRemisionClies { get; set; }
}