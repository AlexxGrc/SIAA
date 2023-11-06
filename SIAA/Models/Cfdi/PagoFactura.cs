﻿using SIAAPI.Models.Administracion;
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

namespace SIAAPI.Models.Cfdi
{
    /* Tabla PagoFactura */
    [Table("PagoFactura")]

    public class PagoFactura
    {
        [Key]
        public int IDPagoFactura { get; set; }
        [DisplayName("Cliente")]
        public int IDCliente { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true,
               DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de operación")]
        public DateTime FechaPago { get; set; }
        [DisplayName("Forma de Pago")]
        public int IDFormaPago { get; set; }
        [DisplayName("Moneda")]
        public int IDMoneda { get; set; }

        [Range(0.01, 1000.00,
                    ErrorMessage = "El tipo de cambio debe estar entre 0.01 y 1000.00")]
        [Display(Name = "Tipo de cambio")]
        public decimal TC { get; set; }
        [DisplayName("Número de operación bancaria")]
        public Int64 NoOperacion { get; set; }

        [DisplayName("Monto Total del Pago")]
        [Range(0.01, Int32.MaxValue, ErrorMessage = "El valor  0  no es válido para el monto")]
        public decimal Monto { get; set; }
        [DisplayName("Banco Emisor")]
        public int IDBancoCliente { get; set; }
      //  public virtual BancoCliente BancoCliente { get; set; }

        [DisplayName("Banco Beneficiario")]
        public int IDBancoEmpresa { get; set; }
     //   public virtual BancoEmpresa BancoEmpresa { get; set; }

        [DisplayName("Tipo Cadena de Pago")]
        public int IDTipoCadenaPago { get; set; }
        public virtual c_TipoCadenaPago c_TipoCadenaPagos { get; set; }

        [DisplayName("Observación")]
        public string Observacion { get; set; }
        public bool Estado { get; set; }
        public virtual ICollection<PagoFacturaSPEI> PagoFacturaSPEIs { get; set; }
        [DisplayName("UUID")]
        public string UUID { get; set; }
        [DisplayName("XML")]
        public string RutaXML { get; set; }
        [DisplayName("Serie")]
        public string Serie { get; set; }
        [DisplayName("Folio")]
        public int Folio { get; set; }
        //[DisplayFormat(ApplyFormatInEditMode = true,
        //       DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        //[DisplayName("Fecha de Cancelación")]
        public Nullable<DateTime> FechaCancelacion { get; set; }
        [DisplayName("Estado")]
        public string StatusPago { get; set; }
        [DisplayName("Observación de la cancelación")]
        public string ObsCancela { get; set; }

        public bool liquidada { get; set; }


    }
    public class PagoFacturaContext : DbContext
    {
        public PagoFacturaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<PagoFactura> PagoFacturas { get; set; }
        public DbSet<Clientes> Clientess { get; set; }
        public DbSet<BancoEmpresa> BancoEmpresas { get; set; }
        public DbSet<BancoCliente> BancoClientes { get; set; }
        public DbSet<PagoFacturaSPEI> PagoFacturaSPEIs { get; set; }
        public DbSet<DocumentoRelacionado> DocumentosRelacionados { get; set; }
        public DbSet<Encfacturas> Encfacturas { get; set; }
        

        public System.Data.Entity.DbSet<SIAAPI.Models.Administracion.c_TipoCadenaPago> c_TipoCadenaPago { get; set; }
    }



    /* Vista PagoFactura */
    [Table("VPagoFactura")]

    public class VPagoFactura
    {
        [Key]
        public Int64 ID { get; set; }
        public int IDPagoFactura { get; set; }
       
        [DisplayFormat(ApplyFormatInEditMode = true,
               DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de operación")]
        public DateTime FechaPago { get; set; }

        [DisplayName("ID Cliente")]
        public int IDCliente { get; set; }
        [DisplayName("Nombre del Cliente")]
        public string Nombre { get; set; }
        [DisplayName("RFC del Cliente")]
        public string RFC { get; set; }

        [DisplayName("Clave Forma de Pago")]
        public string ClaveFormaPago { get; set; }
        [DisplayName("Forma de Pago")]
        public string Descripcion { get; set; }

        [DisplayName("Clave Divisa")]
        public string ClaveDivisa { get; set; }
        [DisplayName("Divisa")]
        public string Divisa { get; set; }

        [Range(0.01, 1000.00,
                    ErrorMessage = "El tipo de cambio debe estar entre 0.01 y 1000.00")]
        [Display(Name = "Tipo de cambio")]
        public decimal TC { get; set; }
        [DisplayName("Número de operación bancaria")]
        public Int64 NoOperacion { get; set; }

        [DisplayName("Monto Total del Pago")]
        public decimal Monto { get; set; }

        //[DisplayName("RFC Banco Emisor")]
        //public string RFCBancoEmisor { get; set; }

        //[DisplayName("Nombre Banco Emisor")]
        //public string NombreBancoEmisor { get; set; }

        //[DisplayName("RFC Banco Beneficiario")]
        //public string RFCBancoReceptor { get; set; }
        //[DisplayName("Nombre Banco Beneficiario")]
        //public string NombreBancoReceptor { get; set; }

        [DisplayName("Tipo Cadena de Pago")]
        public string TipoCadenaPago { get; set; }
        
        [DisplayName("Observación")]
        public string Observacion { get; set; }
        public bool Estado { get; set; }

        public string RutaXML { get; set; }
     
        public string StatusPago { get; set; }
        [DisplayName("Observación de Cancelación")]
        public string ObsCancela { get; set; }
        [DisplayName("Folio")]
        public string folio { get; set; }
    }
    public class VPagoFacturaContext : DbContext
    {
        public VPagoFacturaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VPagoFactura> VPagoFacturas { get; set; }
    }
    //public class trcrMdl
    //{
    //    public VPagoFactura  m1 { get; set; }
    //    public List<VPagoFacturaEfe> m2 { get; set; }
    //}

   

    /* Vista PagoFactura Efectivo*/
    [Table("VPagoFacturaEfe")]

    public class VPagoFacturaEfe
    {
        [Key]
        public Int64 ID { get; set; }
        public int IDPagoFactura { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true,
               DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de operación")]
        public DateTime FechaPago { get; set; }

        [DisplayName("ID Cliente")]
        public int IDCliente { get; set; }
        [DisplayName("Nombre del Cliente")]
        public string Nombre { get; set; }
        [DisplayName("RFC del Cliente")]
        public string RFC { get; set; }

        [DisplayName("Clave Forma de Pago")]
        public string ClaveFormaPago { get; set; }
        [DisplayName("Forma de Pago")]
        public string Descripcion { get; set; }

        [DisplayName("Clave Divisa")]
        public string ClaveDivisa { get; set; }
        [DisplayName("Divisa")]
        public string Divisa { get; set; }

        [Range(0.01, 1000.00,
                    ErrorMessage = "El tipo de cambio debe estar entre 0.01 y 1000.00")]
        [Display(Name = "Tipo de cambio")]
        public decimal TC { get; set; }
        [DisplayName("Número de operación bancaria")]
       
        public decimal Monto { get; set; }

        [DisplayName("Observación")]
        public string Observacion { get; set; }
        public bool Estado { get; set; }

        public string RutaXML { get; set; }

        [DisplayName("Estado")]
        public string StatusPago { get; set; }
        [DisplayName("Observación de Cancelación")]
        public string ObsCancela { get; set; }
        [DisplayName("Folio")]
        public string folio { get; set; }
    }

    public class VPagoFacturaEfeContext : DbContext
    {
        public VPagoFacturaEfeContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VPagoFacturaEfe> VPagoFacturaEfes { get; set; }
    }




    /* Tabla SaldoFactura */
    [Table("SaldoFactura")]
    public class SaldoFactura
    {
        [Key]
        public int IDSaldoFactura { get; set; }
        [DisplayName("No de Factura")]
        public int IDFactura { get; set; }
        public string Serie { get; set; }
        public int Numero { get; set; }
        public decimal Total { get; set; }
        public decimal ImporteSaldoAnterior { get; set; }
        public decimal ImportePagado { get; set; }
        public decimal ImporteSaldoInsoluto { get; set; }
    }
    public class SaldoFacturaContext : DbContext
    {
        public SaldoFacturaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<SaldoFactura> SaldoFacturas { get; set; }
        public DbSet<PagoFactura> PagoFacturas { get; set; }
    }
    
    [Table("VEncFactura")]
    public class VEncFactura
    {
        [Key]
        public int ID { get; set; }
        public string Serie { get; set; }

        public int Numero { get; set; }

        public string Nombre_Cliente { get; set; }

        public decimal Subtotal { get; set; }

        public decimal IVA { get; set; }

        public decimal Total { get; set; }
        public decimal TC { get; set; }
        public string Moneda { get; set; }
        public int IDMoneda { get; set; }
        public string IDMetododepago { get; set; }
        public decimal ImporteSaldoAnterior { get; set; }
        public decimal ImportePagado { get; set; }
        public decimal ImporteSaldoInsoluto { get; set; }
        public int NoParcialidad { get; set; }

    }

    /* VistaDatosCliente */
    [Table("VDatosCliente")]
    public class VDatosCliente
    {
        [Key]
        public int IDCliente { get; set; }
        [DisplayName("CP Cliente")]
        public string RFC { get; set; }
        public string Nombre { get; set; }
        public string ClaveRegimenFiscal { get; set; }
        public string RegimenFiscal { get; set; }
        public string CP { get; set; }

    }

 
    [Table("PagoFacturaSPEI")]
    public class PagoFacturaSPEI
    {
        [Key]
        public int IDPagoFacturaSPEI { get; set; }
        [DisplayName("No de Factura")]
        public int IDPagoFactura { get; set; }
        public virtual PagoFactura PagoFactura { get; set; }
        [DisplayName("Certificado de pago")]
        public string CertificadoPago { get; set; }
        [DisplayName("Identificador de la cadena de pago")]
        public string IDTipoCadenaPago { get; set; }
        [DisplayName("Sello de pago")]
        public string SelloPago { get; set; }

        [DisplayName("Estado")]
        public string StatusDocto { get; set; }

    }
    public class PagoFacturaSPEIContext : DbContext
    {
        public PagoFacturaSPEIContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<PagoFacturaSPEI> PagoFacturaSPEIs { get; set; }
        public DbSet<PagoFactura> PagoFacturas { get; set; }
    }

    /* Tabla DocumentoRelacionado */
    [Table("DocumentoRelacionado")]
    public class DocumentoRelacionado
    {
        [Key]
        public int IDDocumentoRelacionado { get; set; }
        [DisplayName("No de Factura")]
        public int IDPagoFactura { get; set; }
        public int IDCliente { get; set; }
        public virtual Clientes Clientess { get; set; }
        public int IDFactura { get; set; }
        public virtual Encfacturas Encfacturas { get; set; }
        public string Serie { get; set; }
        public int Numero { get; set; }
        public int IDMoneda { get; set; }
        public decimal TC { get; set; }
        public string IDMetododepago { get; set; }

        public decimal ImporteSaldoInsoluto { get; set; }
        public decimal ImportePagado { get; set; }
        public int NoParcialidad { get; set; }

        [DisplayName("Estado")]
        public string StatusDocto { get; set; }

        public decimal SaldoAnterior { get; set; }
    }

    public class DetallesDR
    {
        [Key]
        public int IDFactura { get; set; }
        public string Serie { get; set; }
        public int Numero { get; set; }
        public int IDMoneda { get; set; }
        public string ClaveMoneda { get; set; }
        public string Descripcion{ get; set; }
        public decimal TC { get; set; }
        public string IDMetododepago { get; set; }
        public decimal ImporteSaldoInsoluto { get; set; }
        public decimal ImportePagado { get; set; }
        public int NoParcialidad { get; set; }

        



    }
    public class DocumentoRelacionadoContext : DbContext
    {
        public DocumentoRelacionadoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<DocumentoRelacionado> DocumentosRelacionados { get; set; }
        public DbSet<PagoFactura> PagoFacturas { get; set; }
        public DbSet<Encfacturas> Encfacturas { get; set; }
    }


    public class VDocumentoR
    {
        [Key]
        public int ID { get; set; }
        public int IDPagoFactura { get; set; }
        public DateTime FechaPago { get; set; }
        public string Serie { get; set; }

        public int Numero { get; set; }
        public string Nombre_Cliente { get; set; }
        public bool pagada { get; set; }
        public decimal TC { get; set; }
        public int IDMoneda { get; set; }
        public string Moneda { get; set; }
        public string Estado { get; set; }
        public string IDMetododepago { get; set; }
        public int IDFormapago { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public decimal ImporteSaldoAnterior { get; set; }
        public decimal ImportePagado { get; set; }
        public decimal ImporteSaldoInsoluto { get; set; }
        public int NoParcialidad { get; set; }
        public string UUID { get; set; }


    }
    public class VDocumentoRContext : DbContext
    {
        public VDocumentoRContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VDocumentoR> VDocumentoRs { get; set; }
    }

    [Table("EstadoFacturasPagosSat")]
    public class EstadoFacturasPagosSat
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
    public class EstadoFacturasPagosSatContext : DbContext
    {
        public EstadoFacturasPagosSatContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<EstadoFacturasPagosSat> estadoFacturasPagos { get; set; }
    }



}

/* Vista PagoFactura electrónico y efectivo para timbrado*/
[Table("VPagoFacturaC")]

public class VPagoFacturaC
{
    [Key]
    public Int64 ID { get; set; }
    public int IDPagoFactura { get; set; }

    [DisplayFormat(ApplyFormatInEditMode = true,
           DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
    [DisplayName("Fecha de operación")]
    public DateTime FechaPago { get; set; }

    [DisplayName("ID Cliente")]
    public int IDCliente { get; set; }
    [DisplayName("Nombre del Cliente")]
    public string Nombre { get; set; }
    [DisplayName("RFC del Cliente")]
    public string RFC { get; set; }

    [DisplayName("Clave Forma de Pago")]
    public string ClaveFormaPago { get; set; }
    [DisplayName("Forma de Pago")]
    public string Descripcion { get; set; }

    [DisplayName("Clave Divisa")]
    public string ClaveDivisa { get; set; }
    [DisplayName("Divisa")]
    public string Divisa { get; set; }

    [Range(0.01, 1000.00,
                ErrorMessage = "El tipo de cambio debe estar entre 0.01 y 1000.00")]
    [Display(Name = "Tipo de cambio")]
    public decimal TC { get; set; }
    [DisplayName("Número de operación bancaria")]
    public Int64 NoOperacion { get; set; }

    [DisplayName("Monto Total del Pago")]
    public decimal Monto { get; set; }

    [DisplayName("RFC Banco Emisor")]
    public string RFCBancoEmisor { get; set; }

    [DisplayName("Nombre Banco Emisor")]
    public string NombreBancoEmisor { get; set; }
    [DisplayName("Razon Social Emisor")]
    public string RazonSocialEmisor { get; set; }
    [DisplayName("Cuenta Banco Emisor")]
    public string CuentaEmisor { get; set; }

    [DisplayName("RFC Banco Beneficiario")]
    public string RFCBancoReceptor { get; set; }
    [DisplayName("Nombre Banco Beneficiario")]
    public string NombreBancoReceptor { get; set; }
    [DisplayName("Razon Social Beneficiario")]
    public string RazonSocialReceptor { get; set; }
    [DisplayName("Cuenta Banco Receptor")]
    public string CuentaReceptor { get; set; }
    [DisplayName("Clave Cadena de Pago")]
    public string Clave { get; set; }
    //Si TipoCadenaPago= SPEI entonces incluir CertificadoPago, IDTipoCadenaPago, SelloPago 
    [DisplayName("Tipo Cadena de Pago")]
    public string TipoCadenaPago { get; set; }
    [DisplayName("Certificado de Pago")]
    public string CertificadoPago { get; set; }
    [DisplayName("Tipo Cadena de Pago")]
    public string IDTipoCadenaPago { get; set; }


    [DisplayName("Observación")]
    public string Observacion { get; set; }
    public bool Estado { get; set; }

    public string RutaXML { get; set; }
	  public string StatusPago { get; set; }
    [DisplayName("Serie")]
    public string Serie { get; set; }
    [DisplayName("Folio")]
    public int Folio { get; set; }
}
public class VPagoFacturaCContext : DbContext
{
    public VPagoFacturaCContext() : base("name=DefaultConnection")
    {

    }
    public DbSet<VPagoFacturaC> VPagoFacturasC { get; set; }
}


public class CancelaPago
{
    [Key]
    public int IDPagoFactura { get; set; }
    [DisplayName("Motivo de la cancelación:")]
    public string ObsCancela { get; set; }
}

public class EliminaPago
{
    [Key]
    public int IDPagoFactura { get; set; }
}
public class EliminaPagoEfe
{
    [Key]
    public int IDPagoFactura { get; set; }
}

public class Saldos
{
    [Key]
    public int IDSaldoFactura { get; set; }
    [DisplayName("No de Factura")]
    public int IDFactura { get; set; }
    public string Serie { get; set; }
    public int Numero { get; set; }
    public decimal ImportePagado { get; set; }
    public decimal ImporteSaldoInsoluto { get; set; }
}

[Table("VPagoClie")]

public class VPagoClie
{
    [Key]
    public int IDPagoFactura { get; set; }
    [DisplayName("Cliente")]
    public int IDCliente { get; set; }
    [DisplayName("RFC")]
    public string RFC { get; set; }
    [DisplayName("Nombre")]
    public string Nombre { get; set; }

    [DisplayName("Fecha de Pago")]
    public DateTime? FechaPago { get; set; }
    [DisplayName("Monto Total del Pago")]
    public decimal Monto { get; set; }
    public int IDMoneda { get; set; }
    [DisplayName("Moneda")]
    public string ClaveMoneda { get; set; }
    [Display(Name = "Tipo de cambio")]
    public decimal TC { get; set; }
    public int IDFormaPago { get; set; }
    [DisplayName("Clave Forma de Pago")]
    public string ClaveFormaPago { get; set; }
    [DisplayName("Forma de Pago")]
    public string FormaPago { get; set; }
    [DisplayName("Número de operación bancaria")]
    public Int64 NoOperacion { get; set; }
    public int IDBancoCliente { get; set; }
    [DisplayName("Banco Emisor")]
    public string BancoCliente { get; set; }
    public int IDBancoEmpresa { get; set; }
    [DisplayName("Banco Beneficiario")]
    public string BancoEmpresa { get; set; }
    [DisplayName("Observación")]
    public string Observacion { get; set; }
    public bool Estado { get; set; }
    [DisplayName("UUID")]
    public string UUID { get; set; }
    [DisplayName("XML")]
    public string RutaXML { get; set; }
    [DisplayName("Serie")]
    public string Serie { get; set; }

    [DisplayName("Folio")]
    public int Folio { get; set; }
    [DisplayName("Fecha Cancelación ")]
    public string FechaCancelacion { get; set; }
    //public Nullable<DateTime> FechaCancelacion { get; set; }
    [DisplayName("Estado")]
    public string StatusPago { get; set; }
    [DisplayName("Observación de la cancelación")]
    public string ObsCancela { get; set; }
    [DisplayName("Tipo Cadena Pago")]
    public string TipoCadenaPago { get; set; }
    [DisplayName("Certificado Pago")]
    public string CertificadoPago { get; set; }
    [DisplayName("Cadena Pago")]
    public string IDTipoCadenaPago { get; set; }
    [DisplayName("Sello Pago")]
    public string SelloPago { get; set; }
    [Display(Name = "ID Oficina")]
    public int IDOficina { get; set; }

    [Display(Name = "Oficina")]
    public string NombreOficina { get; set; }
    [Display(Name = "NoExpediente ")]
    public string NoExpediente { get; set; }

}
public class VPagoClieContext : DbContext
{
    public VPagoClieContext() : base("name=DefaultConnection")
    {

    }
    public DbSet<VPagoClie> VPagoClie { get; set; }
}

[Table("VPagoClieDoctos")]
public class VPagoClieDoctos
{
    [Key]
    public int IDDocumentoRelacionado { get; set; }
    [DisplayName("No de Factura")]
    public int IDPagoFactura { get; set; }
    [DisplayName("Fecha de Pago")]
    public DateTime FechaPago { get; set; }
    [DisplayName("RFC")]
    public string RFC { get; set; }
    [DisplayName("Cliente")]
    public string Nombre { get; set; }
    [DisplayName("IDFactura")]
    public int IDFactura { get; set; }
    [DisplayName("Fecha de Pago")]
    public DateTime FechaFactura { get; set; }
    [DisplayName("Serie")]
    public string Serie { get; set; }
    [DisplayName("Numero")]
    public int Numero { get; set; }
    [DisplayName("Número de operación bancaria")]
    public Int64 NoOperacion { get; set; }
    public decimal Subtotal { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public int IDMoneda { get; set; }
    [DisplayName("Moneda")]
    public string ClaveMoneda { get; set; }
    public decimal TC { get; set; }
    public string IDMetododepago { get; set; }
    public decimal SaldoAnterior { get; set; }
    public decimal ImportePagado { get; set; }
    public decimal ImporteSaldoInsoluto { get; set; }

    public int NoParcialidad { get; set; }

    [DisplayName("Estado")]
    public string StatusDocto { get; set; }
    public string IDMetodoPago { get; set; }
    public string MetodoPago { get; set; }

}
public class VPagoClieDoctosContext : DbContext
{
    public VPagoClieDoctosContext() : base("name=DefaultConnection")
    {

    }
    public DbSet<VPagoClieDoctos> VPagoClieDoctos { get; set; }
}

