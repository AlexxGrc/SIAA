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
    [Table("EncOrdenCompra")]
    public class EncOrdenCompra
    {

        [Key]
        public int IDOrdenCompra { get; set; }
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
        [DisplayName("Almacén")]
        [Required]
        public int IDAlmacen { get; set; }
        public virtual Almacen Almacen { get; set; }
        [DisplayName("Uso CFDI")]
        public int IDUsoCFDI { get; set; }
        public virtual c_UsoCFDI c_UsoCFDI { get; set; }
        [DisplayName("Observación")]
      
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
        public bool SujetoCalidad { get; set; }

        [DisplayName("Entregar en")]
        public string Entregaren { get; set; }


        [DisplayName("Usuario")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<DetOrdenCompra> DetOrdenCompra { get; set; }



    }
    [Table("DetOrdenCompra")]
    public class DetOrdenCompra
    {

        [Key]
        public int IDDetOrdenCompra { get; set; }
        [DisplayName("Orden de Compra")]
        [Required]
        public int IDOrdenCompra { get; set; }
        public virtual EncOrdenCompra OrdenCompra { get; set; }
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
        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        public decimal Suministro { get; set; }
        public string Status { get; set; }
        [DisplayName("Almacén")]
        [Required]
        public int IDAlmacen { get; set; }
        public int IDDetExterna { get; set; }

        public virtual Almacen Almacen { get; set; }

    }


    public class OrdenCompraContext : DbContext
    {

        public OrdenCompraContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<OrdenCompraContext>(null);
        }
        //Clases internas
        public DbSet<EncOrdenCompra> EncOrdenCompras { get; set; }
        public DbSet<DetOrdenCompra> DetOrdenCompras { get; set; }
        public DbSet<Clslotemp> Clslotesmp { get; set; }

        //Clases externas 
        public DbSet<Proveedor> Proveedor { get; set; }
        public DbSet<c_MetodoPago> c_MetodoPagos { get; set; }
        public DbSet<c_FormaPago> c_FormaPagos { get; set; }
        public DbSet<c_Moneda> c_Monedas { get; set; }
        public DbSet<CondicionesPago> CondicionesPagos { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<c_UsoCFDI> c_UsoCFDIS { get; set; }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<Caracteristica> Caracteristica { get; set; }
    }
    [Table("VOrdenCompra3M")]
    public class VOrdenCompra3M
    {
        [Key]
        public int IDOrdenCompra { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [DisplayName("Fecha requerida")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaRequerida { get; set; }
        [DisplayName("IDAlmacén")]
        public int IDAlmacen { get; set; }
        [DisplayName("Almacén")]
        public string Almacen { get; set; }
        [DisplayName("ID Forma de Pago")]
        public int IDFormapago { get; set; }
        [DisplayName("Forma de Pago")]
        public string Formapago { get; set; }
        [DisplayName("ID Metodo de Pago")]
        public int IDMetodoPago { get; set; }
        [DisplayName("Metodo de Pago")]
        public string MetodoPago { get; set; }
        [DisplayName("ID Condiciones de Pago")]
        public int IDCondicionesPago { get; set; }
        [DisplayName("Condiciones de Pago")]
        public string CondicionesPago { get; set; }
        [DisplayName("ID Divisa")]
        public int IDMoneda { get; set; }
        [DisplayName("Divisa")]
        public string Moneda { get; set; }
        [DisplayName("Tipo de Cambio")]
        public decimal TipoCambio { get; set; }
        [DisplayName("Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Subtotal { get; set; }
        [DisplayName("IVA")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal IVA { get; set; }
        [DisplayName("Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Total { get; set; }
        [DisplayName("Status")]
        [StringLength(20)]
        public string Status { get; set; }
        [DisplayName("IDProveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("Proveedor")]
        public string Empresa { get; set; }
    }
    public class VOrdenCompra3MContext : DbContext
    {
        public VOrdenCompra3MContext() : base("name=DefaultConnection")
        {

        }
        [Table("VOrdenCompra")]
        public class VOrdenCompra
        {
            [Key]
            public int IDOrdenCompra { get; set; }
            [DisplayName("Fecha")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime Fecha { get; set; }
            [DisplayName("Fecha Requerida")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime FechaRequiere { get; set; }
            [DisplayName("IDProveedor")]
            public int IDProveedor { get; set; }
            [DisplayName("RFC")]
            public string RFC { get; set; }
            [DisplayName("Proveedor")]
            public string Empresa { get; set; }
            [DisplayName("ClaveMetodoPago")]
            public string ClaveMetodoPago { get; set; }
            [DisplayName("MetodoPago")]
            public string MetodoPago { get; set; }
            [DisplayName("ClaveFormaPago")]
            public string ClaveFormaPago { get; set; }
            [DisplayName("FormaPago")]
            public string FormaPago { get; set; }
            [DisplayName("ClaveCondicionesPago")]
            public string ClaveCondicionesPago { get; set; }
            [DisplayName("CondicionesPago")]
            public string CondicionesPago { get; set; }
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
            [DisplayName("Status")]
            public string EstadoOC { get; set; }
            [DisplayName("Almacen")]
            public string Almacen { get; set; }
            [DisplayName("Observación")]
            public string Observacion { get; set; }
            [DisplayName("Generado por")]
            public string Username { get; set; }
        }
        public class VOrdenCompraContext : DbContext
        {
            public VOrdenCompraContext() : base("name=DefaultConnection")
            {

            }
            public DbSet<VOrdenCompra> VOrdenCompras { get; set; }
        }
        public DbSet<VOrdenCompra3M> VOrdenCompra3M { get; set; }
    }

    [Table("VOrdenCompra")]
    public class VOrdenCompra
    {
        [Key]
        public int IDOrdenCompra { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [DisplayName("Fecha Requerida")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaRequiere { get; set; }
        [DisplayName("IDProveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("RFC")]
        public string RFC { get; set; }
        [DisplayName("Proveedor")]
        public string Empresa { get; set; }
        [DisplayName("ClaveMetodoPago")]
        public string ClaveMetodoPago { get; set; }
        [DisplayName("MetodoPago")]
        public string MetodoPago { get; set; }
        [DisplayName("ClaveFormaPago")]
        public string ClaveFormaPago { get; set; }
        [DisplayName("FormaPago")]
        public string FormaPago { get; set; }
        [DisplayName("ClaveCondicionesPago")]
        public string ClaveCondicionesPago { get; set; }
        [DisplayName("CondicionesPago")]
        public string CondicionesPago { get; set; }
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
        [DisplayName("Status")]
        public string EstadoOC { get; set; }
        [DisplayName("Almacen")]
        public string Almacen { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }
        [DisplayName("Generado por")]
        public string Username { get; set; }
    }
    /// <summary>
    /// ///////////////////////////////////////OrdenesvsRecepcion
    /// </summary>
    /// 


    public class VOrdenCompraRecepcion
    {
       
        public int IDOrdenCompra { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [DisplayName("Fecha Requerida")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaRequiere { get; set; }
        [DisplayName("IDProveedor")]
        public int IDProveedor { get; set; }
        
        [DisplayName("Proveedor")]
        public string Empresa { get; set; }
        
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
        [DisplayName("Status")]
        public string EstadoOC { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaRecepcion { get; set; }
        public int IDRecepcion { get; set; }
        public int diasDiferencia { get; set; }
    }

    [Table("VDetOC")]
    public class VDetOC
    {
        [Key]
        public int IDDetOrdenCompra { get; set; }
        [DisplayName("Orden Compra")]
        public int IDOrdenCompra { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [DisplayName("Artículo")]
        public int IDArticulo { get; set; }
        [DisplayName("Clave")]
        public String Cref { get; set; }
        [DisplayName("Articulo")]
        public String Descripcion { get; set; }
        [DisplayName("Característica")]
        public int Caracteristica_ID { get; set; }

        [DisplayName("No.")]
        public int IDPresentacion { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
        [DisplayName("Nota")]
        public string Nota { get; set; }

        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Cantidad")]
        public decimal Costo { get; set; }
        [DisplayName("Cantidad")]
        public decimal Importe { get; set; }
        [DisplayName("Estado")]
        public string Status { get; set; }
        [DisplayName("IDAlmacen")]
        public int IDAlmacen { get; set; }
        [DisplayName("Almacen")]
        public string Almacen { get; set; }

    }
    public class VOrdenCompraContext : DbContext
    {
        public VOrdenCompraContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VOrdenCompra> VOrdenCompras { get; set; }
    }



    public class VHistoriaSuajes
    {
        [Key]
        public int IDDetOrdenCompra { get; set; }
        [DisplayName("Orden Compra")]
        public int IDOrdenCompra { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }

        [DisplayName("Fecha Requerida")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaRequiere { get; set; }

        
        [DisplayName("Clave")]
        public String Cref { get; set; }
       

        [DisplayName("No.")]
        public int IDPresentacion { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
       
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
       
        [DisplayName("Importe")]
        public decimal Importe { get; set; }
        [DisplayName("Estado")]
        public string Status { get; set; }

        [DisplayName("Nota")]
        public string Nota { get; set; }


    }

    public class VHistoriaSuaje
    {
        [Key]
        [DisplayName("Orden Compra")]
        public int Orden_Compra { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [DisplayName("Fecha Requerida")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaRequiere { get; set; }
        public string Familia { get; set; }
        [DisplayName("Estado")]
        public string Status { get; set; }
        [DisplayName("Clave")]
        public String Cref { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
        [DisplayName("No.")]
        public int IDPresentacion { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Unidad")]
        public string Nombre { get; set; }
        [DisplayName("Importe")]
        public decimal Monto { get; set; }
        [DisplayName("Moneda")]
        public string ClaveMoneda { get; set; }
        [DisplayName("Nota")]
        public string Nota { get; set; }

    }
}