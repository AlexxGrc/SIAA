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
    [Table("Carrito")]
    public class VCarritoV
    {
        [Key]
        public int IDCarrito { get; set; }
        public int usuario { get; set; }
        public int IDCaracteristica { get; set; }
        public decimal Precio { get; set; }
        public decimal Cantidad { get; set; }
        public string jsonPresentacion { get; set; }
        public string Presentacion { get; set; }
        public int IDArticulo { get; set; }
        public int IDMoneda { get; set; }
        public string Descripcion { get; set; }

        public decimal Importe { get; set; }
        public string Nota { get; set; }
        public decimal ImporteIva { get; internal set; }
        public string Moneda { get; set; }
        public decimal MinimoCompra { get; set; }
        public decimal MinimoVenta { get; set; }
        public string Unidad { get; set; }
        public int IDCliente { get; set; }
        [DisplayName("Precio MXN")]
        public decimal preciomex { get; set; }
        public int IDAlmacen { get; set; }
    }
    [Table("CarritoC")]
    public class CarritoC
    {

        [Key]
        public int IDCarrito { get; set; }
        public int usuario { get; set; }
        public int IDCaracteristica { get; set; }
        public decimal Precio { get; set; }

        public string Nota { get; set; }
        public int IDMoneda { get; set; }

        public int IDProveedor { get; set; }

    }

    public class VCarrito
    {
        [Key]
        public int IDCarrito { get; set; }
        public int usuario { get; set; }
        public int IDCaracteristica { get; set; }
        public decimal Precio { get; set; }
        public decimal Cantidad { get; set; }
        public int IDPresentacion { get; set; }
        public string jsonPresentacion { get; set; }
        public string Presentacion { get; set; }
        public int IDArticulo { get; set; }
        public int IDMoneda { get; set; }

        public string Cref { get; set; }
        public string Descripcion { get; set; }

        public decimal Importe { get; set; }
        public string Nota { get; set; }
        public decimal ImporteIva { get; internal set; }
        public string Moneda { get; set; }
        public decimal MinimoCompra { get; set; }
        public decimal MinimoVenta {get;set;}
        public string Unidad { get; set; }
        [DisplayName("Precio MXN")]
        public decimal preciomex { get; set; }
 public int IDAlmacen { get; set; }
    }

    [Table("Carrito")]
    public class Carrito
    {
        [Key]
        public int IDCarrito { get; set; }
        public int usuario { get; set; }
        public int IDCaracteristica { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string Nota { get; set; }

        public int IDCLiente { get; set; }

        public int IDMoneda { get; set; }
        public int IDOficina { get; set; }


    }
    //[Table("Carrito")]
    //public class CarritoV
    //{
    //    [Key]
    //    public int IDCarrito { get; set; }
    //    public int usuario { get; set; }
    //    public int IDCaracteristica { get; set; }
    //    public decimal Cantidad { get; set; }
    //    public decimal Precio { get; set; }
    //    public string Nota { get; set; }
    //    public int IDCliente { get; set; }
    //    public int IDArti { get; set; }

    //}
    [Table("CarritoPrefacturaR")]
    public class CarritoPrefacturaR
    {
        [Key]
        public int IDCarritoPrefacturaR { get; set; }
        [DisplayName("Orden Compra")]
        public int IDRemision { get; set; }
        [DisplayName("Artículo")]
        public int IDArticulo { get; set; }

        [DisplayName("Presentación")]
        public int Caracteristica_ID { get; set; }
        public string jsonPresentacion { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Precio")]
        public decimal Costo { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal Descuento { get; set; }
        [DisplayName("Importe")]
        public decimal Importe { get; set; }

        public bool IVA { get; set; }
        [DisplayName("IVA")]
        public decimal ImporteIva { get; set; }
        [DisplayName("Total")]
        public decimal ImporteTotal { get; set; }
        [DisplayName("Nota")]
        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        [DisplayName("Suministro")]
        public decimal Suministro { get; set; }

        public int IDDetExterna { get; set; }
    }
    [Table("CarritoRecepcion")]
    public class CarritoRecepcion
    {
        [Key]
        public int IDCarritoRecepcion { get; set; }
        [DisplayName("Orden Compra")]
        public int IDOrdenCompra { get; set; }
        [DisplayName("Artículo")]
        public int IDArticulo { get; set; }

        [DisplayName("Presentación")]
        public int Caracteristica_ID { get; set; }
        public string jsonPresentacion { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Precio")]
        public decimal Costo { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal Descuento { get; set; }
        [DisplayName("Importe")]
        public decimal Importe { get; set; }

        public bool IVA { get; set; }
        [DisplayName("IVA")]
        public decimal ImporteIva { get; set; }
        [DisplayName("Total")]
        public decimal ImporteTotal { get; set; }
        [DisplayName("Nota")]
        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        [DisplayName("Suministro")]
        public decimal Suministro { get; set; }

        public int IDDetExterna { get; set; }
     public int UserID { get; set; }
    }
    [Table("CarritoRequisicion")]
    public class CarritoRequisicion
    {

        [Key]
        public int IDCarritoRequisicion { get; set; }
        [DisplayName("Requisición")]
        public int IDRequisicion { get; set; }
        [DisplayName("Artículo")]
        public int IDArticulo { get; set; }

        [DisplayName("Presentación")]
        public int Caracteristica_ID { get; set; }
        public string jsonPresentacion { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Precio")]
        public decimal Costo { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal Descuento { get; set; }
        [DisplayName("Importe")]
        public decimal Importe { get; set; }

        public bool IVA { get; set; }
        [DisplayName("IVA")]
        public decimal ImporteIva { get; set; }
        [DisplayName("Total")]
        public decimal ImporteTotal { get; set; }
        [DisplayName("Nota")]
        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        [DisplayName("Suministro")]
        public decimal Suministro { get; set; }

        public int IDDetExterna { get; set; }


    }
    [Table("CarritoDevolucionR")]
    public class CarritoDevolucionR
    {
        [Key]
        public int IDCarritoDevolucionR { get; set; }
        [DisplayName("Orden Compra")]
        public int IDRemision { get; set; }
        [DisplayName("Artículo")]
        public int IDArticulo { get; set; }

        [DisplayName("Presentación")]
        public int Caracteristica_ID { get; set; }
        public string jsonPresentacion { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Precio")]
        public decimal Costo { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal Descuento { get; set; }
        [DisplayName("Importe")]
        public decimal Importe { get; set; }

        public bool IVA { get; set; }
        [DisplayName("IVA")]
        public decimal ImporteIva { get; set; }
        [DisplayName("Total")]
        public decimal ImporteTotal { get; set; }
        [DisplayName("Nota")]
        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        [DisplayName("Suministro")]
        public decimal Suministro { get; set; }

        public int IDDetExterna { get; set; }
    }
    [Table("CarritoDevolucion")]
    public class CarritoDevolucion
    {
        [Key]
        public int IDCarritoDevolucion { get; set; }
        [DisplayName("Orden Compra")]
        public int IDRecepcion { get; set; }
        [DisplayName("Artículo")]
        public int IDArticulo { get; set; }

        [DisplayName("Presentación")]
        public int Caracteristica_ID { get; set; }
        public string jsonPresentacion { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Precio")]
        public decimal Costo { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal Descuento { get; set; }
        [DisplayName("Importe")]
        public decimal Importe { get; set; }

        public bool IVA { get; set; }
        [DisplayName("IVA")]
        public decimal ImporteIva { get; set; }
        [DisplayName("Total")]
        public decimal ImporteTotal { get; set; }
        [DisplayName("Nota")]
        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        [DisplayName("Suministro")]
        public decimal Suministro { get; set; }

        public int IDDetExterna { get; set; }
    }
    [Table("CarritoCotizacion")]
    public class CarritoCotizacion
    {

        [Key]
        public int IDCarritoCotizacion { get; set; }
        [DisplayName("Cotización")]
        public int IDCotizacion { get; set; }
        [DisplayName("Artículo")]
        public int IDArticulo { get; set; }

        [DisplayName("Presentación")]
        public int Caracteristica_ID { get; set; }
        public string jsonPresentacion { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Precio")]
        public decimal Costo { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal Descuento { get; set; }
        [DisplayName("Importe")]
        public decimal Importe { get; set; }

        public bool IVA { get; set; }
        [DisplayName("IVA")]
        public decimal ImporteIva { get; set; }
        [DisplayName("Total")]
        public decimal ImporteTotal { get; set; }
        [DisplayName("Nota")]
        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        [DisplayName("Suministro")]
        public decimal Suministro { get; set; }

        public int IDDetExterna { get; set; }


    }
    [Table("CarritoRemision")]
    public class CarritoRemision
    {
        [Key]
        public int IDCarritoRemision { get; set; }
        [DisplayName("Pedido")]
        public int IDPedido { get; set; }
        [DisplayName("Artículo")]
        public int IDArticulo { get; set; }

        [DisplayName("Presentación")]
        public int Caracteristica_ID { get; set; }
        public string jsonPresentacion { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Precio")]
        public decimal Costo { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal Descuento { get; set; }
        [DisplayName("Importe")]
        public decimal Importe { get; set; }

        public bool IVA { get; set; }
        [DisplayName("IVA")]
        public decimal ImporteIva { get; set; }
        [DisplayName("Total")]
        public decimal ImporteTotal { get; set; }
        [DisplayName("Nota")]
        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        [DisplayName("Suministro")]
        public decimal Suministro { get; set; }

        public int IDDetExterna { get; set; }
    }

    [Table("MovimientoArticulo")]
    public class ArticulosComprados
    {
        [Key]
        public int ID { get; set;}
        public int IDArticulo { get; set; }
        public int IDCaracteristica { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public string Cref { get; set; }
        public string Cliente { get; set; }
        public string Descripcion { get; set; }
        public string Presentacion { get; set; }
        public string nameFoto { get; set; }

        public decimal Cantidad { get; set; }

        public int IDCliente { get; set; }

    }

    public class ArticulosCompradosContext : DbContext
    {
        public ArticulosCompradosContext() : base("name=DefaultConnection")
        {
            // Database.SetInitializer<CarritoContext>(null);
        }
        public DbSet<ArticulosComprados> ArticulosComprados { get; set; }
    }
    public class CarritoContext : DbContext
    {
        public CarritoContext() : base("name=DefaultConnection")
        {
           // Database.SetInitializer<CarritoContext>(null);
        }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<Caracteristica> Caracteristica { get; set; }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<CarritoRecepcion> CarritoRecepciones { get; set; }
        public DbSet<CarritoRequisicion> CarritoRequisiciones { get; set; }
        public DbSet<CarritoDevolucion> CarritoDevoluciones { get; set; }
        public DbSet<CarritoDevolucionR> CarritoDevolucionRes { get; set; }
        public DbSet<CarritoCotizacion> CarritoCotizaciones { get; set; }
        public DbSet<CarritoRemision> CarritoRemisiones { get; set; }
        public DbSet<CarritoPrefacturaR> CarritoPrefacturaRs { get; set; }
        public DbSet<CarritoVale> CarritoVales { get; set; }

        public DbSet<CarritoLote> CarritoLotes { get; set; }

    }

    [Table("CarritoLote")]
    public class CarritoLote
    {
        [Key]
        public int IDCarritoLote { get; set; }
        public int usuario { get; set; }
        public int IDMateriaP { get; set; }
        public string Lote { get; set; }
        public decimal MetrosDisponibles { get; set; }
        public int IDAlmacen { get; set; }


    }
    [Table("CarritoVale")]
    public class CarritoVale
    {
        [Key]
        public int IDCarritoVale { get; set; }
        public int usuario { get; set; }
        public int IDCaracteristica { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public int IDAlmacen { get; set; }


    }
    public class VCarritoVale
    {
        [Key]
        public int IDCarritoVale { get; set; }
        public int usuario { get; set; }
        public int IDCaracteristica { get; set; }
        public decimal Precio { get; set; }
        public decimal Cantidad { get; set; }

        public string Presentacion { get; set; }
        public int IDArticulo { get; set; }
        public string Moneda { get; set; }

        public string Cref { get; set; }
        //public string Descripcion { get; set; }

        public decimal Importe { get; set; }

        //public decimal ImporteIva { get; internal set; }
        //public string Moneda { get; set; }

        public string Unidad { get; set; }

        public int IDAlmacen { get; set; }
    }
    public class VCarritoValeSuaje
    {
        [Key]
        public int IDCarritoValeSuaje { get; set; }
        public int usuario { get; set; }
        public int IDCaracteristica { get; set; }
        public decimal Precio { get; set; }
        public decimal Cantidad { get; set; }

        public string Presentacion { get; set; }
        public int IDArticulo { get; set; }
        public string Moneda { get; set; }

        public string Cref { get; set; }
        //public string Descripcion { get; set; }

        public decimal Importe { get; set; }

        //public decimal ImporteIva { get; internal set; }
        //public string Moneda { get; set; }

        public string Unidad { get; set; }

        public int IDAlmacen { get; set; }
    }
}