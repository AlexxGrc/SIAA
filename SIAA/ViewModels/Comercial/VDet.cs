using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Comercial
{
    public class VCarritoCotizacion
    {

        public int IDCarritoCotizacion { get; set; }
        [DisplayName("Requisición")]
        public int IDCotizacion { get; set; }
        [DisplayName("Artículo")]
        public string Articulo { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
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
        public decimal MinimoCompra { get; set; }
        public int IDCaracteristica { get; set; }
        public int IDArticulo { get; set; }
    }
    public class VCarritoPrefacturaR
    {

        public int IDCarritoPrefacturaR { get; set; }
        [DisplayName("Orden Compra")]
        public int IDRemision { get; set; }
        [DisplayName("Artículo")]
        public string Articulo { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
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
        public string Lote { get; set; }
        public int IDCaracteristica { get; set; }
    }

    public class VCarritoRequisicion
    {

        public int IDCarritoRequisicion { get; set; }
        [DisplayName("Requisición")]
        public int IDRequisicion { get; set; }
        [DisplayName("Artículo")]
        public string Articulo { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
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
        public decimal MinimoCompra { get; set; }
        public int IDCaracteristica { get; set; }
        public int IDArticulo { get; set; }

        public string Cref { get; set; }
    }

    public class VEncRequisicion
    {

        public int IDRequisicion { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime FechaRequiere { get; set; }

        public string Proveedor { get; set; }

        public string MetodoPago { get; set; }

        public string FormaPago { get; set; }

        public string Divisa { get; set; }

        public string CondicionesPago { get; set; }

        public string Almacen { get; set; }

        public string UsoCFDI { get; set; }

        public string Observacion { get; set; }

        public decimal Subtotal { get; set; }

        public decimal IVA { get; set; }

        public decimal Total { get; set; }

        public string Status { get; set; }

        public decimal TipoCambio { get; set; }

        public int UserID { get; set; }



    }
    public class VEncCotizacion
    {

        public int IDCotizacion { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime FechaRequiere { get; set; }

        public string Cliente { get; set; }

        public string MetodoPago { get; set; }

        public string FormaPago { get; set; }

        public string Divisa { get; set; }

        public string CondicionesPago { get; set; }

        public string Almacen { get; set; }

        public string UsoCFDI { get; set; }

        public string Observacion { get; set; }

        public decimal Subtotal { get; set; }

        public decimal IVA { get; set; }

        public decimal Total { get; set; }

        public string Status { get; set; }

        public decimal TipoCambio { get; set; }

        public int UserID { get; set; }



    }
    public class VCarritoRemision
    {

        public int IDCarritoRemision { get; set; }
        [DisplayName("Pedido")]
        public int IDPedido { get; set; }

        [DisplayName("ID Artículo")]
        public int IDArticulo { get; set; }

        [DisplayName("Artículo")]
        public string Articulo { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
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
        public string Lote { get; set; }
        public int IDCaracteristica { get; set; }

        public int IDAlmacen { get; set; }
    }
    public class VCarritoRecepcion
    {

        public int IDCarritoRecepcion { get; set; }
        [DisplayName("Orden Compra")]
        public int IDOrdenCompra { get; set; }
        [DisplayName("Artículo")]

        public string Cref { get; set; }
        public string Articulo { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
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
        public string Lote { get; set; }
        public int IDCaracteristica { get; set; }

        public int IDAlmacen { get; set; }
    }
    public class VCarritoDevolucionR
    {

        public int IDCarritoDevolucionR { get; set; }
        [DisplayName("Orden Compra")]
        public int IDRemision { get; set; }
        [DisplayName("Artículo")]
        public int IDArticulo { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
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
        public string Lote { get; set; }
        public int Caracteristica_ID { get; set; }

        public int IDAlmacen { get; set; }
    }
    public class VCarritoDevolucion
    {

        public int IDCarritoDevolucion { get; set; }
        [DisplayName("Orden Compra")]
        public int IDRecepcion { get; set; }
        [DisplayName("Artículo")]
        public string Articulo { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
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
        public string Lote { get; set; }
        public int IDCaracteristica { get; set; }

        public int IDAlmacen { get; set; }
    }
    public class VDetCotizacion
    {
        public int IDDetCotizacion { get; set; }
        public int IDAlmacen { get; set; }
        public int IDCotizacion { get; set; }
        public string Articulo { get; set; }
        public string Presentacion { get; set; }
        public string jsonPresentacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Costo { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal Descuento { get; set; }

        public decimal Importe { get; set; }

        public bool IVA { get; set; }

        public decimal ImporteIva { get; set; }

        public decimal ImporteTotal { get; set; }

        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        public string Status { get; set; }
        public decimal MinimoVenta { get; set; }
        public int Caracteristica_ID { get; set; }

    }

    public class VDetDevolucionR
    {
        public int IDDetDevolucionR { get; set; }
        public int IDDevolucionR { get; set; }
        public int IDRemision { get; set; }
        public int IDDetRemision { get; set; }
        public string Articulo { get; set; }

        public string Presentacion { get; set; }
        public string jsonPresentacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Costo { get; set; }


        public decimal Importe { get; set; }



        public decimal ImporteIva { get; set; }

        public decimal ImporteTotal { get; set; }

        public string Nota { get; set; }

        public string Status { get; set; }

        public string Lote { get; set; }

        public int IDAlmacen { get; set; }
    }
    public class VDetDevolucion
    {
        public int IDDetDevolucion { get; set; }
        public int IDDevolucion { get; set; }
        public int IDRecepcion { get; set; }
        public int IDDetRecepcion { get; set; }
        public string Articulo { get; set; }
        public string Cref { get; set; }
        public string Presentacion { get; set; }
        public string jsonPresentacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Costo { get; set; }


        public decimal Importe { get; set; }



        public decimal ImporteIva { get; set; }

        public decimal ImporteTotal { get; set; }

        public string Nota { get; set; }

        public string Status { get; set; }

        public string Lote { get; set; }

        public int IDAlmacen { get; set; }
    }
    public class VDetPedido
    {
        public int IDDetPedido { get; set; }
        public int IDPedido { get; set; }
        public string Cref { get; set; }
        public int IDArticulo { get; set; }
        public string Articulo { get; set; }
        public int IDDetExterna { get; set; }
        public string Presentacion { get; set; }
        public string jsonPresentacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Costo { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal Descuento { get; set; }

        public decimal Importe { get; set; }

        public bool IVA { get; set; }

        public decimal ImporteIva { get; set; }

        public decimal ImporteTotal { get; set; }

        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        public decimal Suministro { get; set; }
        public string Status { get; set; }
        public decimal MinimoVenta { get; set; }
        public int Caracteristica_ID { get; set; }
        public bool GeneraOrdenP { get; set; }
        public int IDRemision { get; set; }
        public int IDPrefactura { get; set; }
        public bool GeneraOrden { get; set; }

        public int IDAlmacen { get; set; }
    }
    public class VDetOrdenCompra
    {
        public int IDDetOrdenCompra { get; set; }
        public int IDOrdenCompra { get; set; }
        public int IDDetExterna { get; set; }
        public string Articulo { get; set; }

        public string Presentacion { get; set; }
        public string jsonPresentacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Costo { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal Descuento { get; set; }

        public decimal Importe { get; set; }

        public bool IVA { get; set; }

        public decimal ImporteIva { get; set; }

        public decimal ImporteTotal { get; set; }

        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        public decimal Suministro { get; set; }
        public string Status { get; set; }
        public decimal MinimoCompra { get; set; }
        public int Caracteristica_ID { get; set; }

        public int IDCarritoRecepcion { get; set; }

        public int IDAlmacen { get; set; }
    }

    public class VDetRequisiciones
    {
        public int IDDetRequisiciones { get; set; }
        public int IDRequisicion { get; set; }
        public string Articulo { get; set; }
        public string Cref { get; set; }
        public string Presentacion { get; set; }
        public string jsonPresentacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Costo { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal Descuento { get; set; }

        public decimal Importe { get; set; }

        public bool IVA { get; set; }

        public decimal ImporteIva { get; set; }

        public decimal ImporteTotal { get; set; }

        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        public decimal Suministro { get; set; }
        public string Status { get; set; }
        public decimal MinimoCompra { get; set; }
        public int Caracteristica_ID { get; set; }


    }
    //public class VDetCotizaciones
    //{
    //    public int IDDetCotizacion { get; set; }
    //    public int IDCotizacion { get; set; }
    //    public string Articulo { get; set; }
    //    public string Presentacion { get; set; }
    //    public string jsonPresentacion { get; set; }
    //    public decimal Cantidad { get; set; }
    //    public decimal Costo { get; set; }

    //    public decimal CantidadPedida { get; set; }

    //    public decimal Descuento { get; set; }

    //    public decimal Importe { get; set; }

    //    public bool IVA { get; set; }

    //    public decimal ImporteIva { get; set; }

    //    public decimal ImporteTotal { get; set; }

    //    public string Nota { get; set; }

    //    public bool Ordenado { get; set; }
    //    public decimal Suministro { get; set; }
    //    public string Status { get; set; }
    //    public decimal MinimoVenta { get; set; }
    //    public int Caracteristica_ID { get; set; }
    //}
    public class VDetPrefactura
    {
        public int IDDetPrefactura { get; set; }
        public int IDPrefactura { get; set; }

        public int IDArticulo { get; set; }
        public string Cref { get; set; }
        public string Articulo { get; set; }

        public string Presentacion { get; set; }
        public string jsonPresentacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Costo { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal Descuento { get; set; }

        public decimal Importe { get; set; }

        public bool IVA { get; set; }

        public decimal ImporteIva { get; set; }

        public decimal ImporteTotal { get; set; }

        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        public decimal Suministro { get; set; }
        public string Status { get; set; }
        public decimal MinimoVenta { get; set; }
        public string Lote { get; set; }
        public decimal Devolucion { get; set; }
        public int Caracteristica_ID { get; set; }
        public string Proviene { get; set; }

        public int IDAlmacen { get; set; }


    }

    public class VDetRemision
    {
        public int IDDetRemision { get; set; }

        public int IDExterna { get; set; }
        public int IDRemision { get; set; }
        public string Articulo { get; set; }

        public string Presentacion { get; set; }
        public string jsonPresentacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Costo { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal Descuento { get; set; }

        public decimal Importe { get; set; }

        public bool IVA { get; set; }

        public decimal ImporteIva { get; set; }

        public decimal ImporteTotal { get; set; }

        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        public decimal Suministro { get; set; }
        public string Status { get; set; }
        public decimal MinimoVenta { get; set; }
        public string Lote { get; set; }
        public decimal Devolucion { get; set; }
        public int Caracteristica_ID { get; set; }

        public int IDAlmacen { get; set; }

    }
    public class VDetRecepcion
    {
        public int IDDetRecepcion { get; set; }
        public int IDExterna { get; set; }
        public int IDRecepcion { get; set; }
        public string Articulo { get; set; }

        public string Presentacion { get; set; }
        public string jsonPresentacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Costo { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal Descuento { get; set; }

        public decimal Importe { get; set; }

        public bool IVA { get; set; }

        public decimal ImporteIva { get; set; }

        public decimal ImporteTotal { get; set; }

        public string Nota { get; set; }

        public bool Ordenado { get; set; }
        public decimal Suministro { get; set; }
        public string Status { get; set; }
        public decimal MinimoCompra { get; set; }
        public string Lote { get; set; }
        public decimal Devolucion { get; set; }
        public int Caracteristica_ID { get; set; }

        public int IDAlmacen { get; set; }

    }
    public class VEncOrdenC
    {

        public int IDOrdenCompra { get; set; }

        public string Proveedor { get; set; }
        public string Fecha { get; set; }

        public string FechaRequiere { get; set; }

    }
    public class VEncPedido
    {

        public int IDPedido { get; set; }

        public string Cliente { get; set; }
        public string Fecha { get; set; }

        public string FechaRequiere { get; set; }

    }
    public class VCambio
    {

        public decimal TC { get; set; }

    }
    public class DatoString
    {

        public string Dato { get; set; }

    }
    public class ValidarCarrito
    {
        public decimal Precio { get; set; }
        public bool Dato { get; set; }

    }
    public class DatoBool
    {
        public bool Dato { get; set; }

    }
    public class TotalDet
    {
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
    }
    public class VEntrega
    {
        public int IDEntrega { get; set; }
        public int IDCliente { get; set; }
        public string CalleEntrega { get; set; }
        public string NumExtEntrega { get; set; }
        public string NumIntentrega { get; set; }
        public string ColoniaEntrega { get; set; }
        public string MunicipioEntrega { get; set; }
        public string Estado { get; set; }
        public string CPEntrega { get; set; }
        public string ObservacionEntrega { get; set; }

    }
    public class VCarritoRemisionKit
    {
        public int IDCarritoRemisionKit { get; set; }
        [DisplayName("Pedido")]
        public int IDPedido { get; set; }
        [DisplayName("Artículo")]
        public int IDArticulo { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Precio")]
        public decimal Costo { get; set; }
        public decimal CantidadPedida { get; set; }
        public int Caracteristica_ID { get; set; }
        [DisplayName("Suministro")]
        public decimal Suministro { get; set; }
        public string jsonPresentacion { get; set; }
        public int IDDetExterna { get; set; }
        public int UserID { get; set; }
        public int IDAlmacen { get; set; }
        public int presentacion { get; set; }

    }
    public class VCarritoDevolucionRemisionKit
    {
        public int IDCarritoDevolucionRemisionKit { get; set; }

        public int IDRemision { get; set; }

        public int IDDetRemision { get; set; }

        public int IDDetRemisionKit { get; set; }
        public int IDDetDevolucion { get; set; }
        public int IDDevolucion { get; set; }
        public int IDArticulo { get; set; }

        public decimal Cantidad { get; set; }
        public decimal Suministro { get; set; }

        public decimal Costo { get; set; }

        public int Caracteristica_ID { get; set; }

        public int IDAlmacen { get; set; }

        public int UserID { get; set; }
        public string Nota { get; set; }
        public string presentacion { get; set; }
    }


}