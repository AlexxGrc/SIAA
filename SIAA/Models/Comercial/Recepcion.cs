using SIAAPI.Models.Administracion;
using SIAAPI.Models.Inventarios;
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
    [Table("EncRecepcion")]
    public class EncRecepcion
    {

        [Key]
        public int IDRecepcion { get; set; }
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

        [DisplayName("Proveedor")]
        [Required]
        public int IDProveedor { get; set; }
        public virtual Proveedor Proveedor { get; set; }
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
        public decimal TipoCambio { get; set; }
        public string Status { get; set; }
        public int Ticket { get; set; }

    }
    [Table("DetRecepcion")]
    public class DetRecepcion
    {
        [Key]
        public int IDDetRecepcion { get; set; }
        [DisplayName("Orden de Compra")]
        [Required]
        public int IDRecepcion { get; set; }
        public virtual EncRecepcion Recepcion { get; set; }
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

        public string Presentacion { get; set; }

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
        public int IDAlmacen { get; set; }
        public virtual Almacen Almacen { get; set; }
    }

    public class DetDevolucion
    {
        [Key]
        public int IDDetDevolucion { get; set; }
        [DisplayName("Devolución")]
        public int IDDevolucion { get; set; }
        public virtual EncDevolucion Devolucion { get; set; }
        public int IDRecepcion { get; set; }
        public virtual EncRecepcion Recepcion { get; set; }
        public int IDDetRecepcion { get; set; }
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


    [Table("EncDevolucion")]
    public class EncDevolucion
    {

        [Key]
        public int IDDevolucion { get; set; }
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

        [DisplayName("Proveedor")]
        [Required]
        public int IDProveedor { get; set; }
        public virtual Proveedor Proveedor { get; set; }
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
        public decimal TipoCambio { get; set; }
        public string Status { get; set; }


    }
    public class RecepcionContext : DbContext
    {

        public RecepcionContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<RecepcionContext>(null);
        }
        //Clases internas
        public DbSet<EncRecepcion> EncRecepciones { get; set; }
        public DbSet<DetRecepcion> DetRecepciones { get; set; }
        public DbSet<EncDevolucion> EncDevoluciones { get; set; }
        public DbSet<DetDevolucion> DetDevoluciones { get; set; }
        //Clases externas 
        public DbSet<Proveedor> Proveedor { get; set; }
        public DbSet<c_MetodoPago> c_MetodoPagos { get; set; }
        public DbSet<c_FormaPago> c_FormaPagos { get; set; }
        public DbSet<c_Moneda> c_Monedas { get; set; }
        public DbSet<CondicionesPago> CondicionesPagos { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<Caracteristica> Caracteristica { get; set; }

        public DbSet<Clslotemp> lotes { get; set; }
    }

    [Table("VRecepcion")]
    public class VRecepcion
    {

        [Key]
        public int IDRecepcion { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [DisplayName("IDProveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("RFC")]
        public string RFC { get; set; }
        [DisplayName("Proveedor")]
        public string Empresa { get; set; }
        [DisplayName("Factura")]
        public string DocumentoFactura { get; set; }
        [DisplayName("Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Subtotal { get; set; }
        [DisplayName("IVA")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal IVA { get; set; }
        [DisplayName("Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Total { get; set; }

        [DisplayName("Divisa")]
        public string ClaveMoneda { get; set; }
        [DisplayName("Tipo de Cambio")]
        public decimal TipoCambio { get; set; }
        [DisplayName("Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalPesos { get; set; }
        [DisplayName("Estado")]
        public string EstadoRec { get; set; }
        [DisplayName("Almacen")]
        public string Almacen { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }
        [DisplayName("Generado por")]
        public string Username { get; set; }
    }
    public class VRecepcionContext : DbContext
    {
        public VRecepcionContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VRecepcion> VRecepciones { get; set; }
    }


    [Table("VRecepcionDet")]
    public class VRecepcionDet
    {

        [Key]
        public int IDDetRecepcion { get; set; }
        public int IDRecepcion { get; set; }
        [DisplayName("Factura")]
        public string DocumentoFactura { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [DisplayName("Proveedor")]
        public string Empresa { get; set; }
        [DisplayName("Clave")]
        public string Cref { get; set; }
        [DisplayName("Artículo")]
        public string Descripcion { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
        [DisplayName("Lote")]
        public string Lote { get; set; }
        [DisplayName("Cantidad Pedida")]
        public decimal CantidadPedida { get; set; }
        [DisplayName("Suministro")]
        public decimal Suministro { get; set; }
        [DisplayName("Devolucion")]
        public decimal Devolucion { get; set; }
        [DisplayName("Costo")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Costo { get; set; }
        [DisplayName("Importe")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Importe { get; set; }
        [DisplayName("IVA")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal ImporteIVA { get; set; }
        [DisplayName("Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal ImporteTotal { get; set; }
        [DisplayName("Almacén")]
        public string Almacen { get; set; }
        [DisplayName("Nota")]
        public string Nota { get; set; }
    }
    public class VRecepcionDetContext : DbContext
    {
        public VRecepcionDetContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VRecepcionDet> VRecepcionDet { get; set; }
    }



    [Table("Promedio")]
    public class Promedio
    {

        [Key]
        public int ID { get; set; }
        public int IDArticulo { get; set; }

        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaUltimaCompra { get; set; }

        [DisplayName("Costo")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal UltimoCosto { get; set; }
        public int IDUltimaMoneda { get; set; }
        public decimal PromedioenPesos { get; set; }
        public decimal Promedioendls { get; set; }

    }
    public class PromedioContext : DbContext
    {
        public PromedioContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Promedio> Promedios { get; set; }
    }


    [Table("ReporteMateriaP2Meses")]
    public class ReporteMateriaP2Meses
    {
        [Key]
        public int IDRecepcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Cref { get; set; }

        public string Descripcion { get; set; }
        public string Presentacion { get; set; }

        public decimal Cantidad { get; set; }



    }
    public class ReporteMateriaP2MesesContext : DbContext
    {
        public ReporteMateriaP2MesesContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ReporteMateriaP2MesesContext>(null);
        }
        public DbSet<ReporteMateriaP2Meses> vReporte { get; set; }

    }

}