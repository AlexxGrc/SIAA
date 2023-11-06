using SIAAPI.Models.Administracion;
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
    [Table("Enccotizapros")]
    public class Enccotizapros
    {
        [Key]
        public int id { get; set; }
        [Required]
        [DisplayName("Cliente Prospecto")]
        public int idprospecto { get; set; }

        //  public virtual ClientesP Prospecto { get; set; }
        [DisplayName("Condiciones de pago")]
        [Required]
        public int idcondiciones { get; set; }

        //  public virtual CondicionesPago Condiciones { get; set; }
        [DisplayName("En Atencion")]
        public string atencion { get; set; }

        [DisplayName("Vigencia")]
        [Required]
        public int     Vigencia { get; set; }

        [DisplayName("Moneda")]
        public int idMoneda { get; set; }
        public virtual c_Moneda Moneda { get; set; }

        [DisplayName("Fecha")]
        public DateTime Fecha { get; set; }
        [DisplayName("Vendedor")]
        [Required]
        public int idvendedor { get; set; }
        public virtual Vendedor vendedor { get; set; }
      

        public decimal Subtotal { get; set; }

        public decimal iva { get; set; }

        public decimal Total { get; set; }
        public string username { get; set; }

        public string Observacion { get; set; }

        public List<detcotizapro> conceptos { get; set; }
    }

    [Table("detcotizapro")]

    public class detcotizapro
    {
        [Key]
        public int id { get; set; }

        public int idcotizapros { get; set; }
        public virtual Enccotizapros cotiza { get; set; }
        public decimal cantidad { get; set; }
        public string Concepto { get; set; }

        public string Unidad { get; set; }
        public decimal Precio  {get;set;}

        public decimal Importe { get; set; }
        public bool iva { get; set; }
    }

    public class CotizacionProspectoContext : DbContext
    {

        public CotizacionProspectoContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<CotizacionContext>(null);
        }
        //Clases internas
        public DbSet<Enccotizapros> Enccotizaprospecto { get; set; }
        public DbSet<detcotizapro> DetCotizacionesProspecto { get; set; }


 
        public DbSet<c_Moneda> c_Monedas { get; set; }
        public DbSet<CondicionesPago> CondicionesPagos { get; set; }
       
       
    }

    public class VCotizacionesPros
        {
        [Key]
        public int ID { get; set; }
        public   DateTime Fecha { get; set; }
           public  int idprospecto { get; set; }
           public string Cliente { get; set; }
      
        public decimal Subtotal { get; set; }
      
        public decimal IVA { get; set; }

        public decimal Total { get; set; }
    
        public decimal TipoCambio { get; set; }
     
        public int IDMoneda { get; set; }
     
         public string ClaveMoneda { get; set; }
     
        public decimal TotalPesos { get; set; }
     
        public string Estado { get; set; }
    
         public int IDCondiciones { get; set; }
     
        public string CondicionesPago { get; set; }
      
        public int IDVendedor { get; set; }
      
        public string Nombre { get; set; }
    
        public string NombreOficina { get; set; }
      
        public string Observacion { get; set; }
}

    public class VCotizacionesProsContext : DbContext
    {
        public VCotizacionesProsContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VCotizacionesPros> VCotizacionesPros { get; set; }

        public DbSet<c_Moneda> c_Monedas { get; set; }
        public DbSet<CondicionesPago> CondicionesPagos { get; set; }
    }
    [Table("VDetCotizaciones")]
    public class VDetCotizaciones
    {
        [Key]
        public int IDDetCotizacion { get; set; }
        [DisplayName("Cotización")]
        public int IDCotizacion { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [DisplayName("Cliente")]
        public string Cliente { get; set; }
        [DisplayName("Cref")]
        public string Cref { get; set; }
        [DisplayName("IDArtículo")]
        public int IDArticulo { get; set; }
        [DisplayName("Artículo")]
        public string Articulo { get; set; }
        [DisplayName("Característica")]
        public int Caracteristica_ID { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
        [DisplayName("Cantidad Pedida")]
        public decimal CantidadPedida { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Costo")]
        public decimal Costo { get; set; }
        [DisplayName("Descuento")]
        public decimal Descuento { get; set; }
        [DisplayName("Importe")]
        public decimal Importe { get; set; }

        [DisplayName("Importe IVA")]
        public decimal ImporteIva { get; set; }
        [DisplayName("Importe Total")]
        public decimal ImporteTotal { get; set; }
        public string Status { get; set; }
        [DisplayName("Nota")]
        [Required]
        public string Nota { get; set; }
    }
    public class VDetCotizacionesContext : DbContext
    {
        public VDetCotizacionesContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VDetCotizaciones> VDetCotizaciones { get; set; }
    }
    [Table("VDetCotizacionePros")]

    public class VDetCotizacionePros
    {
        [Key]
        public int ID { get; set; }

        public int IDCotiza { get; set; }
        public DateTime Fecha { get; set; }
        public int IDProspecto { get; set; }
        public bool EsCliente { get; set; }
        public string Cliente { get; set; }
        public decimal cantidad { get; set; }
        public string Unidad { get; set; }
        public string Concepto { get; set; }
        public decimal Precio { get; set; }

        public decimal Importe { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
    }

    public class VDetCotizacioneProsContext : DbContext
    {

        public VDetCotizacioneProsContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<VDetCotizacioneProsContext>(null);
        }
        //Clases internas
        public DbSet<VDetCotizacionePros> VDetCotizacionePros { get; set; }

    }
}