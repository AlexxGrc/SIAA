using SIAAPI.Models.Administracion;
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
  

    [Table("EncFacturaProv")]
    public class EncfacturaProv
    {

        [Key]
        public int ID { get; set; }

         public int IDProveedor { get; set; }

        public int NoRecepcion { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime FechaRevision { get; set; }
        public DateTime FechaVencimiento { get; set; }


        public string Serie { get; set; }

        public String Numero { get; set; }


        public string Nombre_Proveedor { get; set; }

        public decimal Subtotal { get; set; }

        public decimal IVA { get; set; }

        public decimal Total { get; set; }

        [Required(ErrorMessage = "La factura requiere un UUID")]
        public string UUID { get; set; }

        public string RutaXML { get; set; }

        public string RutaPDF { get; set; }

        public decimal TC { get; set; }

        public int IDMoneda { get; set; }

        public virtual c_Moneda c_Moneda { get; set; }

        public string Moneda { get; set; }

        public string Metododepago { get; set; }

        public string Estado { get; set; }

        public bool pagada { get; set; }


        public string Formadepago { get; set; }  

        public bool ConPagos { get; set; }


    }

    public class EncfacturaProvContext : DbContext
    {
        public DbSet<EncfacturaProv> EncfacturaProveedores { get; set; }
        public EncfacturaProvContext() : base("name=DefaultConnection")
        {
            
        }
    }

    [Table("VEncFacturaProvSaldo")]
    public class VEncFacturaProvSaldo
    {

        [Key]
        public int ID { get; set; }

        public int IDProveedor { get; set; }

       

        public int NoRecepcion { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime FechaRevision { get; set; }
        public DateTime FechaVencimiento { get; set; }


        public string Serie { get; set; }

        public String Numero { get; set; }


        public string Nombre_Proveedor { get; set; }

        public decimal Subtotal { get; set; }

        public decimal IVA { get; set; }

        public decimal Total { get; set; }

        [Required(ErrorMessage = "La factura requiere un UUID")]
        public string UUID { get; set; }

        public string RutaXML { get; set; }

        public string RutaPDF { get; set; }

        public decimal TC { get; set; }

        public int IDMoneda { get; set; }
        
        public virtual c_Moneda c_Moneda { get; set; }

        public string Moneda { get; set; }

        public string Metododepago { get; set; }

        public string Estado { get; set; }

        public bool pagada { get; set; }


        public string Formadepago { get; set; }

        public bool ConPagos { get; set; }
        private decimal? _importep { get; set; }

        private decimal? _importesi { get; set; }

        public decimal? ImportePagado
        {
            get
            {
                if (_importep == null)
                { return -1; }

                else
                {
                    return _importep;
                }
            }
            set
            {
                if ((value == null))
                {
                    _importep = -1;
                }
                else
                {
                    _importep = value;
                }
            }
        }
        public decimal? ImporteSaldoInsoluto
        {
            get
            {
                if (_importesi == null)
                { return -1; }

                else
                {
                    return _importesi;
                }
            }
            set
            {
                if ((value == null))
                {
                    _importesi = -1;
                }
                else
                {
                    _importesi = value;
                }
            }
        }
    }

    public class VEncFacturaProvSaldoContext : DbContext
    {
        public DbSet<VEncFacturaProvSaldo> VEncFacturaProvSaldo { get; set; }
        public VEncFacturaProvSaldoContext() : base("name=DefaultConnection")
        {

        }
    }

    [Table("RecepcionAdd")]
    public class RecepcionAdd
    {
        [Key]
        public int ID { get; set; }
        [System.ComponentModel.DisplayName("Pedido")]
        public int IDFactura { get; set; }
        [DisplayName("RutaArchivo")]
        public string RutaArchivo { get; set; }
        [DisplayName("NombreArchivo")]
        public string nombreArchivo { get; set; }
    }
    [Table("FechaSubFacturaP")]
    public class FechaSubFacturaP
    {
        [Key]
        public int IDFacturaP { get; set; }
        public DateTime Fecha { get; set; }
        public int Usuario { get; set; }
         public int IDOrdenC { get; set; }
    }
    public class FechaSubFacturaPContext : DbContext
    {
        public FechaSubFacturaPContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<FechaSubFacturaP> fechaSubFacturas { get; set; }
    }
    public class RecepcionAddContext : DbContext
    {
        public RecepcionAddContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<RecepcionAdd> RecepcionAdd { get; set; }
    }

    [Table("VHistorialPagoProv")]
    public class VHistorialPagoProv
    {
        [Key]
        public int ID { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public DateTime? Fecha { get; set; }
        public int IDProveedor { get; set; }
        public string Nombre_Proveedor { get; set; }
        public Decimal subtotal { get; set; }
        public Decimal iva { get; set; }
        public Decimal total { get; set; }
        public string Moneda { get; set; }
        public decimal TCFactura { get; set; }
        public int IDPagoFacturaProv { get; set; }
        public string SerieP { get; set; }
        public int FolioP { get; set; }
        public DateTime? FechaPago { get; set; }
        public int NoParcialidad { get; set; }
        public Decimal importepagado { get; set; }
        public decimal TCPago { get; set; }
        public Decimal importeSaldoInsoluto { get; set; }
    }
    public class VHistorialPagoProvContext : DbContext
    {
        public VHistorialPagoProvContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VHistorialPagoProv> VHistorialPagoProv { get; set; }
    }
}