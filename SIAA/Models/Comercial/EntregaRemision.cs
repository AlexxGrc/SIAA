
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
    [Table("EntregaRemision")]
    public class EntregaRemision
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "La fecha de entrega es obligatoria")]
        [DisplayName("Fecha de entrega ")]
        [DataType(DataType.DateTime)]
        public DateTime Fecha { get; set; }


        [DisplayName("Ruta ")]
        public int IDRuta { get; set; }
        public virtual Ruta ruta { get; set; }


        [DisplayName("Chofer ")]
        public int IDChofer { get; set; }
        public virtual Chofer chofer { get; set; }




        public string Status { get; set; }
        public DateTime? FechaFinalizacion { get; set; }


    }



    [Table("VEntregaR")]
    public class VEntregaR
    {
        [Key]
        public int ID { get; set; }
        public DateTime Fecha { get; set; }
        public string Chofer { get; set; }
        public string Ruta { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public string Status { get; set; }
    }


    public class EncRemisionEntrega
    {
        [DisplayName("ID Remision")]
        [Key]
        public int IDRemision { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVAl { get; set; }
        public decimal Total { get; set; }

        public int IDCliente { get; set; }
        public string Cliente { get; set; }
        public string Entrega { get; set; }

    }
    [Table("DetEntregaRemision")]
    public class DetEntregaRemision
    {

        [Key]
        public int IDDetEntregaR { get; set; }
        public int IDEntregaR { get; set; }
        public int IDRemision { get; set; }
        public bool Entregado { get; set; }
        public int IDFactura { get; set; }

    }

    [Table("VDetEntregaRemision")]
    public class VDetEntregaRemision
    {

        public int IDEntregar { get; set; }
        public int IDRemision { get; set; }
        public string Cliente { get; set; }
        public string Entrega { get; set; }
        public string NoFactura { get; set; }

    }


    public class EntregaRemisionesContext : DbContext
    {
        public EntregaRemisionesContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<EntregaRemision> EntregaRemisiones { get; set; }

        public DbSet<VEntregaR> VEntregas { get; set; }
        public DbSet<DetEntregaRemision> detEntregas { get; set; }
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

}