using SIAAPI.Models.Administracion;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Models.Cfdi
{

    /* Tabla PagoFacturaProveedor */
    [Table("PagoFacturaProv")]

    public class PagoFacturaProv
    {
        [Key]
        public int IDPagoFacturaProv { get; set; }
        [DisplayName("Serie")]
        public string SerieP { get; set; }
        [DisplayName("Folio")]
        public int FolioP { get; set; }

        [DisplayName("IDProveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("Nombre")]
        public string Empresa { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true,
               DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de pago")]
        public DateTime FechaPago { get; set; }
        [DisplayName("Forma de Pago")]
        public string ClaveFormaPago { get; set; }

        [DisplayName("Moneda")]
        public string ClaveMoneda { get; set; }
        [DisplayName("Tipo de Cambio")]
        public decimal TC { get; set; }

        [DisplayName("Número de operación bancaria")]
        public Int64 NoOperacion { get; set; }

        [DisplayName("Monto Total del Pago")]
        [Required]
        public decimal Monto { get; set; }
        [DisplayName("RFC Banco Emisor")]
        public string RFCBancoEmpresa { get; set; }
        [DisplayName("Cuenta Emisora")]
        public string CuentaEmpresa { get; set; }
        [DisplayName("RFC Banco Receptor")]
        public string RFCBancoProv { get; set; }
        [DisplayName("Cuenta Receptora")]
        public string CuentaProv { get; set; }
        [DisplayName("Tipo Cadena de Pago")]
        public string IDTipoCadenaPago { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }
        [DisplayName("Estado")]
        public string Estado { get; set; }
        public bool EstadoP { get; set; }
        public string UUID { get; set; }
        public string RutaXML { get; set; }
        public string RutaPDF { get; set; }


    }


    public class PagoFacturaProvContext : DbContext
    {
        public PagoFacturaProvContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<PagoFacturaProv> PagoFacturasProv { get; set; }

    }

    /* Tabla Vista PagoFacturaProveedor pagos Electrónicos */
    [Table("[VPagoFacturaProvElec]")]
    public class VPagoFacturaProvElec
    {
        [Key]
        public int IDPagoFacturaProv { get; set; }
        [DisplayName("Serie")]
        public string SerieP { get; set; }
        [DisplayName("Folio")]
        public int FolioP { get; set; }

        [DisplayName("IDProveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("Proveedor")]
        public string Empresa { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true,
               DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de pago")]
        public DateTime FechaPago { get; set; }
        [DisplayName("Clave Forma de Pago")]
        public string ClaveFormaPago { get; set; }
        [DisplayName("Forma de Pago")]
        public string FormaPago { get; set; }
        [DisplayName("Clave Moneda")]
        public string ClaveMoneda { get; set; }
        [DisplayName("Moneda")]
        public string Moneda { get; set; }
        [DisplayName("Tipo de Cambio")]
        public decimal TC { get; set; }

        [DisplayName("No. operación bancaria")]
        public Int64 NoOperacion { get; set; }

        [DisplayName("Monto Total del Pago")]
        [Required]
        public decimal Monto { get; set; }
        [DisplayName("RFC Banco Emisor")]
        public string RFCBancoEmpresa { get; set; }
        [DisplayName("Cuenta Emisora")]
        public string CuentaEmpresa { get; set; }
        [DisplayName("RFC Banco Receptor")]
        public string RFCBancoProv { get; set; }
        [DisplayName("Cuenta Receptora")]
        public string CuentaProv { get; set; }
        [DisplayName("ID Tipo Cadena de Pago")]
        public string IDTipoCadenaPago { get; set; }
        [DisplayName("Tipo Cadena de Pago")]
        public string TipoCadenaPago { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }
        [DisplayName("Estado")]
        public string Estado { get; set; }
        public bool EstadoP { get; set; }
        public string UUID { get; set; }
        public string RutaXML { get; set; }
        public string RutaPDF { get; set; }

    }
    public class VPagoFacturaProvElecContext : DbContext
    {
        public VPagoFacturaProvElecContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VPagoFacturaProvElec> VPagoFacturaProvElecs { get; set; }

    }

    [Table("VPagoProveedor")]
    public class VPagoProveedor
    {
        [Key]
        public int IDPagoFacturaProv { get; set; }
        [DisplayName("Serie Pago")]
        public string SerieP { get; set; }
        [DisplayName("Folio Pago")]
        public int FolioP { get; set; }
        [DisplayName("IDProveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("RFC")]
        public string RFC { get; set; }
        [DisplayName("Nombre")]
        public string Empresa { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
               DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de pago")]
        public DateTime FechaPago { get; set; }
        [DisplayName("Forma de Pago")]
        public string ClaveFormaPago { get; set; }
        [DisplayName("Forma de Pago")]
        public string FormaPago { get; set; }
        [DisplayName("Número de operación bancaria")]
        public Int64 NoOperacion { get; set; }
        [DisplayName("Monto Total del Pago")]
        public decimal Monto { get; set; }
        [DisplayName("Moneda")]
        public string ClaveMoneda { get; set; }
        [DisplayName("Tipo de Cambio")]
        public decimal TC { get; set; }

        [DisplayName("RFC Banco Emisor")]
        public string RFCBancoEmpresa { get; set; }
        [DisplayName("Cuenta Emisora")]
        public string CuentaEmpresa { get; set; }
        [DisplayName("RFC Banco Receptor")]
        public string RFCBancoProv { get; set; }
        [DisplayName("Banco Receptor")]
        public string BancoProv { get; set; }
        [DisplayName("Cuenta Receptora")]
        public string CuentaProv { get; set; }
        [DisplayName("Tipo Cadena de Pago")]
        public string IDTipoCadenaPago { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }
        [DisplayName("Estado")]
        public string Estado { get; set; }
        public string UUID { get; set; }
    }
    public class VPagoProveedorContext : DbContext
    {
        public VPagoProveedorContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<VPagoProveedor> VPagoProveedor { get; set; }
    }

    //Excel PagoFacturaProveedor Hoja 2
    [Table("VPagoProvDocto")]
    public class VPagoProvDocto
    {
        [Key]
        public int IDDocumentoRelacionadoProv { get; set; }
        [DisplayName("ID Pago Factura")]
        public int IDPagoFacturaProv { get; set; }
        [DisplayName("Serie Pago")]
        public string SerieP { get; set; }
        [DisplayName("Folio Pago")]
        public int FolioP { get; set; }
        [DisplayName("ID Proveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("Empresa")]

        public string Empresa { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
               DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de pago")]
        public DateTime FechaPago { get; set; }
        [DisplayName("Serie Factura")]
        public string SerieF { get; set; }
        [DisplayName("No de Factura")]
        public string NumeroF { get; set; }
        [DisplayName("Total")]
        public decimal Total { get; set; }

        [DisplayName("Importe Saldo Insoluto")]
        public decimal ImporteSaldoInsoluto { get; set; }
        [DisplayName("Importe Pagado")]
        public decimal ImportePagado { get; set; }
        [DisplayName("No. Parcialidad")]
        public int NoParcialidad { get; set; }
        [DisplayName("Clave Divisa")]
        public string ClaveMoneda { get; set; }
        [DisplayName("Forma de Pago")]
        public string ClaveFormapago { get; set; }
        [DisplayName("Clave Método de Pago")]
        public string ClaveMetododepago { get; set; }

        [DisplayName("Estado")]
        public string Estado { get; set; }
    }

    public class VPagoProvDoctoContext : DbContext
    {
        public VPagoProvDoctoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VPagoProvDocto> VPagoProvDocto { get; set; }

    }

    [Table("PagoProvAdd")]
    public class PagoProvAdd
    {
        [Key]
        public int ID { get; set; }
        [System.ComponentModel.DisplayName("Pagos")]
        public int IDPagoFacturaProv { get; set; }
        [DisplayName("RutaArchivo")]
        public string RutaArchivo { get; set; }
        [DisplayName("NombreArchivo")]
        public string nombreArchivo { get; set; }
    }
    public class PagoProvAddContext : DbContext
    {
        public PagoProvAddContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<PagoProvAdd> PagoProvAdd { get; set; }
    }






    /* Tabla Vista PagoFacturaProveedor pagos Efectivo */
    [Table("[VPagoFacturaProvEfe]")]

    public class VPagoFacturaProvEfe
    {
        [Key]
        public int IDPagoFacturaProv { get; set; }
        [DisplayName("Serie")]
        public string SerieP { get; set; }
        [DisplayName("Folio")]
        public int FolioP { get; set; }

        [DisplayName("IDProveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("Nombre")]
        public string Empresa { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true,
               DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de pago")]
        public DateTime FechaPago { get; set; }
        [DisplayName("Clave Forma de Pago")]
        public string ClaveFormaPago { get; set; }
        [DisplayName("Forma de Pago")]
        public string FormaPago { get; set; }
        [DisplayName("Clave Divisa")]
        public string ClaveMoneda { get; set; }
        [DisplayName("Divisa")]
        public string Moneda { get; set; }
        [DisplayName("Tipo de Cambio")]
        public decimal TC { get; set; }
        [DisplayName("Monto Total del Pago")]
        [Required]
        public decimal Monto { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }
        [DisplayName("Estado")]
        public string Estado { get; set; }
        public bool EstadoP { get; set; }
        public string UUID { get; set; }
        public string RutaXML { get; set; }
        public string RutaPDF { get; set; }

    }


    public class VPagoFacturaProvEfeContext : DbContext
    {
        public VPagoFacturaProvEfeContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VPagoFacturaProvEfe> VPagoFacturaProvEfes { get; set; }

    }



    /* Tabla SaldoFactura Proveedor */
    [Table("SaldoFacturaProv")]
    public class SaldoFacturaProv
    {
        [Key]
        public int IDSaldoFactura { get; set; }
        [DisplayName("ID de Factura")]
        public int IDFacturaProv { get; set; }
        public virtual EncfacturaProv EncFacturasProv { get; set; }
        [DisplayName("No de Proveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("Empresa")]
        public string Empresa { get; set; }
        [DisplayName("No. de Factura")]

        public string Serie { get; set; }
        public int Numero { get; set; }
        [DisplayName("Total")]
        public decimal Total { get; set; }
        [DisplayName("Importe Saldo Anterior")]
        public decimal ImporteSaldoAnterior { get; set; }
        [DisplayName("Importe Pagado")]
        public decimal ImportePagado { get; set; }
        [DisplayName("Importe Saldo Insoluto")]
        public decimal ImporteSaldoInsoluto { get; set; }
        [DisplayName("Estado")]
        public string Estado { get; set; }
    }


    public class SaldoFacturaProvContext : DbContext
    {
        public SaldoFacturaProvContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<SaldoFacturaProv> SaldoFacturasProv { get; set; }
        //public DbSet<PagoFacturaProv> PagoFacturasProv { get; set; }
    }

    [Table("PagoFacturaSPEIProv")]
    public class PagoFacturaSPEIProv
    {
        [Key]
        public int IDPagoFacturaSPEIProv { get; set; }
        [DisplayName("No de Factura")]
        public int IDPagoFacturaProv { get; set; }
        public virtual PagoFacturaProv PagoFacturasProv { get; set; }
        [DisplayName("Certificado de pago")]
        public string CertificadoPago { get; set; }
        [DisplayName("Identificador de la cadena de pago")]
        public string IDTipoCadenaPago { get; set; }
        [DisplayName("Sello de pago")]
        public string SelloPago { get; set; }
        [DisplayName("Estado")]
        public string Estado { get; set; }
    }


    public class PagoFacturaSPEIProvContext : DbContext
    {
        public PagoFacturaSPEIProvContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<PagoFacturaSPEIProv> PagoFacturaSPEIsProv { get; set; }

    }

    /* Tabla DocumentoRelacionado */
    [Table("DocumentoRelacionadoProv")]
    public class DocumentoRelacionadoProv
    {
        [Key]
        public int IDDocumentoRelacionadoProv { get; set; }
        [DisplayName("ID Pago Factura")]
        public int IDPagoFacturaProv { get; set; }
        public virtual PagoFacturaProv PagoFacturasProv { get; set; }
        [DisplayName("ID Proveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("Empresa")]

        public string Empresa { get; set; }
        [DisplayName("ID de Factura")]
        public int IDFacturaProv { get; set; }
        [DisplayName("Serie")]
        public string Serie { get; set; }
        [DisplayName("No de Factura")]
        public string Numero { get; set; }
        [DisplayName("Clave Divisa")]
        public string ClaveMoneda { get; set; }
        [DisplayName("Forma de Pago")]
        public string ClaveFormapago { get; set; }
        [DisplayName("Clave Método de Pago")]
        public string ClaveMetododepago { get; set; }
        [DisplayName("Importe Saldo Insoluto")]
        public decimal ImporteSaldoInsoluto { get; set; }
        [DisplayName("Importe Pagado")]
        public decimal ImportePagado { get; set; }
        [DisplayName("No. Parcialidad")]
        public int NoParcialidad { get; set; }
        [DisplayName("Estado")]
        public string Estado { get; set; }
    }

    public class DocumentoRelacionadoProvContext : DbContext
    {
        public DocumentoRelacionadoProvContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<DocumentoRelacionadoProv> DocumentoRelacionadosProv { get; set; }

    }


    /* Tabla DocumentoRelacionado */
    [Table("VDocumentoRProv")]
    public class VDocumentoRProv
    {
        [Key]
        public int IDDocumentoRelacionadoProv { get; set; }
        [DisplayName("No de Factura")]
        public int IDPagoFacturaProv { get; set; }
        public virtual PagoFacturaProv PagoFacturasProv { get; set; }
        [DisplayName("ID Proveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("Empresa")]
        public string Empresa { get; set; }
        [DisplayName("Serie")]
        public string Serie { get; set; }
        [DisplayName("Número")]
        public string Numero { get; set; }
        [DisplayName("Clave Divisa")]
        public string ClaveDivisa { get; set; }
        [DisplayName("Divisa")]
        public string Divisa { get; set; }
        [DisplayName("Clave de Forma de PAgo")]
        public string ClaveFormaPago { get; set; }
        [DisplayName("Forma de Pago")]
        public string FormaPago { get; set; }
        [DisplayName("Clave de Método de Pago")]

        public decimal ImportePagado { get; set; }
        [DisplayName("Importe Saldo Insoluto")]
        public decimal ImporteSaldoInsoluto { get; set; }
        [DisplayName("No. de Parcialidad")]
        public int NoParcialidad { get; set; }
        [DisplayName("Estado")]
        public string Estado { get; set; }
        [DisplayName("UUID")]
        public string UUID { get; set; }

        public int idFacturaProv { get; set; }
    }
    public class VDocumentoRProvContext : DbContext
    {
        public VDocumentoRProvContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VDocumentoRProv> VDocumentoRProvs { get; set; }

    }
    [Table("PagoFacturaDet")]
    public class PagoFacturaDet
    {
        [Key]
        public int IDDocumentoRelacionadoProv { get; set; }
        [DisplayName("ID Pago Factura")]
        public int IDPagoFacturaProv { get; set; }

        [DisplayName("ID Proveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("Empresa")]

        public string Empresa { get; set; }

        [DisplayName("Serie")]
        public string Serie { get; set; }
        [DisplayName("No de Factura")]
        public string Numero { get; set; }
        [DisplayName("Clave Divisa")]
        public string ClaveDivisa { get; set; }
        [DisplayName("Importe Pagado")]
        public decimal ImportePagado { get; set; }
        [DisplayName("Importe Saldo Insoluto")]
        public decimal ImporteSaldoInsoluto { get; set; }
        [DisplayName("No. Parcialidad")]

        public int NoParcialidad { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true,
              DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de operación")]
        public DateTime FechaPago { get; set; }
        [DisplayName("No. Operación")]
        public Int64 NoOperacion { get; set; }
        [DisplayName("RFCBancoProv")]
        public string RFCBancoProv { get; set; }
        [DisplayName("Cuenta Proveedor")]
        public string CuentaProv { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }

        [DisplayName("forma de Pago")]
        public string Formapago { get; set; }
        [DisplayName("Método de Pago")]
        public string ClaveMetododepago { get; set; }
        public string MetodoPago { get; set; }
        public int idFacturaProv { get; set; }

    }

    public class PagoFacturaDetContext : DbContext
    {
        public PagoFacturaDetContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<PagoFacturaDet> PagoFacturaDet { get; set; }

    }

    public class CancelaPago
    {
        [Key]
        public int IDPagoFacturaProv { get; set; }
        public string UUID { get; set; }
        [DisplayName("Motivo de la cancelación:")]
        [Required(ErrorMessage = "Ingrese el motivo de la cancelación")]
        public string Observacion { get; set; }
    }


    /* Tabla PagoFacturaProveedor */
    [Table("VPagoFacturaProv")]

    public class VPagoFacturaProv
    {
        [Key]
        public int IDPagoFacturaProv { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
               DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de pago")]
        public DateTime FechaPago { get; set; }
        [DisplayName("Proveedor")]
        public string Empresa { get; set; }
        [DisplayName("Monto Total del Pago")]
        public decimal Monto { get; set; }
        [DisplayName("Moneda")]
        public string ClaveMoneda { get; set; }
        public string UUID { get; set; }
    }


    public class VPagoFacturaProvContext : DbContext
    {
        public VPagoFacturaProvContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VPagoFacturaProv> VPagoFacturaProvs { get; set; }

    }

    public class VPagosProv
    {
        [Key]

        public int NoParcialidad { get; set; }
        public System.DateTime FechaPago { get; set; }

        public decimal ImporteSaldoInsoluto { get; set; }
        public decimal ImportePagado { get; set; }

        public string Estado { get; set; }
        public int FolioP { get; set; }
    }



    public class VEncPagosProv
    {

        [Key]
        public int ID { get; set; }
        public string Nombre_Proveedor { get; set; }
        public string UUID { get; set; }
        public string NoFactura { get; set; }
        public decimal Total { get; set; }

    }

    public class CancelaPagoProv
    {
        [Key]
        public int IDPagoFactura { get; set; }
        [DisplayName("Motivo de la cancelación:")]
        public string Observacion { get; set; }
        public string UUID { get; set; }
    }


    /* Vista PagoFactura Efectivo*/
    [Table("VPagoFacturaEfeProv")]

    public class VPagoFacturaEfeProv
    {
        [Key]
        //public Int64 ID { get; set; }
        public int ID { get; set; }
        public int IDPagoFacturaprov { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true,
               DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de operación")]
        public DateTime FechaPago { get; set; }
        public string SerieP { get; set; }
        public int FolioP { get; set; }
        [DisplayName("ID Proveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("Nombre del Proveedor")]
        public string Empresa { get; set; }
        [DisplayName("RFC del Proveedor")]
        public string RFC { get; set; }

        [DisplayName("Clave Forma de Pago")]
        public string ClaveFormaPago { get; set; }
        [DisplayName("Forma de Pago")]
        public string Descripcion { get; set; }

        [DisplayName("Clave Divisa")]
        public string ClaveDivisa { get; set; }
        [DisplayName("Divisa")]
        public string Divisa { get; set; }

        public decimal Monto { get; set; }

        [DisplayName("Observación")]
        public string Observacion { get; set; }
        public string Estado { get; set; }
        public string UUID { get; set; }
        public string RutaXML { get; set; }
        public string RutaPDF { get; set; }
    }

    public class VPagoFacturaEfeProvContext : DbContext
    {
        public VPagoFacturaEfeProvContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VPagoFacturaEfeProv> VPagoFacturaEfesProv { get; set; }
    }

    public class VBcoProv
    {
        [Key]
        public int IDBanco { get; set; }
        [DisplayName("No de Factura")]
        public string Banco { get; set; }

    }
    public class VBcoProvContext : DbContext
    {
        public VBcoProvContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VBcoProv> VBcoProv { get; set; }

    }

    /* Tabla PagoFacturaProveedor */
    [Table("VPagoFacturaProvT")]

    public class VPagoFacturaProvT
    {
        [Key]
        public int ID { get; set; }
        public int IDPagoFacturaProv { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public Nullable<DateTime> FechaPago { get; set; }
        [DisplayName("Serie")]
        public string SerieP { get; set; }
        [DisplayName("Folio")]
        public int FolioP { get; set; }

        [DisplayName("IDProveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("Nombre")]
        public string Empresa { get; set; }
        public string RFC { get; set; }
        [DisplayName("Clave Forma de Pago")]
        public string ClaveFormaPago { get; set; }
        public string Descripcion { get; set; }
        [DisplayName("Divisa")]
        public string ClaveDivisa { get; set; }
        public string Divisa { get; set; }
        [DisplayName("Número de operación bancaria")]
        public Int64 NoOperacion { get; set; }

        [DisplayName("Monto Total del Pago")]
        public decimal Monto { get; set; }
        [DisplayName("RFC Banco Emisor")]
        public string RFCBancoEmisor { get; set; }
        public string NombreBancoEmisor { get; set; }
        [DisplayName("Cuenta Emisora")]
        public string CuentaEmisor { get; set; }
        public string RFCBancoReceptor { get; set; }
        public string NombreBancoReceptor { get; set; }
        public string CuentaReceptor { get; set; }
        public string IDTipoCadenaPago { get; set; }
        public string TipoCadenaPago { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }
        [DisplayName("Estado")]
        public string Estado { get; set; }
        public string UUID { get; set; }
        public string RutaXML { get; set; }
        public string RutaPDF { get; set; }

    }


    public class VPagoFacturaProvTContext : DbContext
    {
        public VPagoFacturaProvTContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VPagoFacturaProvT> VPagoFacturaProvT { get; set; }

    }

    [Table("VEncFacturaProv")]

    public class VEncFacturaProv
    {
        [Key]
        public int ID { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public int IDProveedor { get; set; }
        public string Nombre_Proveedor { get; set; }
        public decimal TC { get; set; }
        public int IDMoneda { get; set; }

        public string Moneda { get; set; }
        public string MetododePago { get; set; }
        public string FormadePago { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public decimal ImporteSaldoAnterior { get; set; }
        public decimal ImportePagado { get; set; }
        public decimal ImporteSaldoInsoluto { get; set; }
        public int NoParcialidad { get; set; }
    }
    public class VEncFacturaProvContext : DbContext
    {
        public VEncFacturaProvContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VEncFacturaProv> VEncFacturaProvs { get; set; }

    }


    [Table("VBcoProveedor")]

    public class VBcoProveedor
    {
        [Key]
        public int ID { get; set; }
        public int IDProveedor { get; set; }
        public int IDBancosProv { get; set; }
        public int IDBanco { get; set; }
        public string ClaveBanco { get; set; }
        public string RFC { get; set; }
        public string Nombre { get; set; }
        public string Cuenta { get; set; }
        public string CuentaClabe { get; set; }
        public string ClaveMoneda { get; set; }
        public string Descripcion { get; set; }
    }
    public class VBcoProveedorContext : DbContext
    {
        public VBcoProveedorContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VBcoProveedor> VBcoProveedores { get; set; }

    }

    public class ResumenFacP
    {
        public string Moneda { get; set; }
        public string MonedaOrigen { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public decimal TotalenPesos { get; set; }

    }



public class EliminaPagoProv
{
    [Key]
    public int IDPagoFacturaProv { get; set; }
}

public class SaldosPr
{
    [Key]
    public int IDSaldoFactura { get; set; }
    [DisplayName("No de Factura")]
    public int IDFacturaProv { get; set; }
    public string Serie { get; set; }
    public string Numero { get; set; }
    public decimal ImportePagado { get; set; }
    public decimal ImporteSaldoInsoluto { get; set; }
}
public class ProveedorAllRepository
{
    public IEnumerable<SelectListItem> GetProveedor()
    {
        using (var context = new ProveedorContext())
        {
            List<SelectListItem> listaProveedor = context.Proveedores.AsNoTracking()
                .OrderBy(n => n.Empresa)
                    .Select(n =>
                    new SelectListItem
                    {
                        Value = n.IDProveedor.ToString(),
                        Text = n.Empresa /*+ "\t|" + n.RFC*/
                    }).ToList();
            var descripciontip = new SelectListItem()
            {
                Value = "0",
                Text = "--- Seleccione un Proveedor ---"
            };
            listaProveedor.Insert(0, descripciontip);
            return new SelectList(listaProveedor, "Value", "Text");
        }
    }
    public IEnumerable<SelectListItem> GetProveedorNombres()
    {
        using (var context = new ProveedorContext())
        {
            List<SelectListItem> listaProveedor = context.Proveedores.AsNoTracking()
                .OrderBy(n => n.Empresa)
                    .Select(n =>
                    new SelectListItem
                    {
                        Value = n.Empresa,
                        Text = n.Empresa /*+ "\t|" + n.RFC*/
                    }).ToList();
            var descripciontip = new SelectListItem()
            {
                Value = "0",
                Text = "--- Seleccione un Proveedor ---"
            };
            listaProveedor.Insert(0, descripciontip);
            return new SelectList(listaProveedor, "Value", "Text");
        }
    }

}


}

