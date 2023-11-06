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
    [Table("EncCotizacion")]
    public class EncCotizacion
    {
       
        [Key]
        public int IDCotizacion { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime Fecha { get; set; }
        [DisplayName("Fecha requerida")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime FechaRequiere { get; set; }
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
        [DisplayName("Observación")]
        [StringLength(250)]
        public string Observacion { get; set; }
        [DisplayName("Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Required]
        public decimal Subtotal { get; set; }
        [DisplayName("IVA")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Required]
        public decimal IVA { get; set; }
        [DisplayName("Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Required]
        public decimal Total { get; set; }

        [DisplayName("Status")]
        [StringLength(20)]
        public string Status { get; set; }

        [DisplayName("Tipo de Cambio")]
        public decimal TipoCambio { get; set; }

        [DisplayName("Usuario")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public int IDVendedor { get; set; }
        public virtual Vendedor Vendedor { get; set; }
        public virtual ICollection<DetCotizacion> DetCotizaciones { get; set; }
    


    }
    [Table("DetCotizacion")]
    public class DetCotizacion
    {
        [Key]
        public int IDDetCotizacion { get; set; }
        [DisplayName("Cotización")]
        [Required]
        public int IDCotizacion { get; set; }
        public virtual EncCotizacion Cotizacion { get; set; }
        [DisplayName("Artículo")]
        [Required]
        public int IDArticulo { get; set; }
        public virtual Articulo Articulo { get; set; }
        [DisplayName("Característica")]
        [Required]
        public int Caracteristica_ID { get; set; }
        //public virtual Caracteristica Caracteristica { get; set; }
        [DisplayName("Presentación")]
        [Required]
        public string Presentacion { get; set; }
        [DisplayName("jsonPresentación")]
        [Required]
        public string jsonPresentacion { get; set; }
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
        [DisplayName("Orden de Compra")]
        [Required]
        public bool Ordenado { get; set; }
        public decimal Suministro { get; set; }
        public string Status { get; set; }
        [DisplayName("Almacen")]
        [Required]
        public int IDAlmacen { get; set; }
        public virtual Almacen Almacen { get; set; }
    }


    public class CotizacionContext : DbContext
    {

        public CotizacionContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<CotizacionContext>(null);
        }
        //Clases internas
        public DbSet<EncCotizacion> EncCotizaciones { get; set; }
        public DbSet<DetCotizacion> DetCotizaciones { get; set; }
       

        //Clases externas 
        public DbSet<Clientes> Clientess { get; set; }
        public DbSet<c_MetodoPago> c_MetodoPagos { get; set; }
        public DbSet<c_FormaPago> c_FormaPagos { get; set; }
        public DbSet<c_Moneda> c_Monedas { get; set; }
        public DbSet<CondicionesPago> CondicionesPagos { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<c_UsoCFDI> c_UsoCFDIS { get; set; }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<Caracteristica> Caracteristica { get; set; }
    }

    [Table("VCotizaciones")]
    public class VCotizaciones
    {

        [Key]
        public int IDCotizacion { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [DisplayName("Fecha requerida")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaRequiere { get; set; }
        [DisplayName("IDCliente")]
        public int IDCliente { get; set; }
        [DisplayName("No Expediente")]
        public string noExpediente { get; set; }
        [DisplayName("RFC")]
        public string RFC { get; set; }
        [DisplayName("Cliente")]
        public string Cliente { get; set; }
        [DisplayName("Método de Pago")]
        public int IDMetodoPago { get; set; }
        public string MetodoPago { get; set; }
        [DisplayName("Forma de Pago")]
        public int IDFormapago { get; set; }
        public string FormaPago { get; set; }
        [DisplayName("Divisa")]
        public int IDMoneda { get; set; }
        public string ClaveMoneda { get; set; }
        [DisplayName("Condiciones de Pago")]
        public int IDCondicionesPago { get; set; }
        public string CondicionesPago { get; set; }
        [DisplayName("Uso CFDI")]
        public int IDUsoCFDI { get; set; }
        public string UsoCFDI { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }
        [DisplayName("Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Subtotal { get; set; }
        [DisplayName("IVA")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal IVA { get; set; }
        [DisplayName("Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Total { get; set; }
        [DisplayName("Total Pesos")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalPesos { get; set; }
        [DisplayName("Status")]
        public string Status { get; set; }

        [DisplayName("Tipo de Cambio")]
        public decimal TipoCambio { get; set; }

        public int IDVendedor { get; set; }
        [DisplayName("Vendedor")]
        public string Nombre { get; set; }
        [DisplayName("Oficina")]
        public string NombreOficina { get; set; }
    }
    public class VCotizacionesContext : DbContext
    {
        public VCotizacionesContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VCotizaciones> VCotizaciones { get; set; }

       
    }

}