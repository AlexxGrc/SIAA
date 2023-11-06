using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Cfdi
{
    public class VEncFacturaF
    {
        [Key]
        [Display(Name = "idFactura")]
        public int ID { get; set; }
        [Display(Name = "Serie")]
        public String Serie { get; set; }
        [Display(Name = "Numero")]
        public int numero { get; set; }
        [Display(Name = "IDCliente")]
        public int IDCliente { get; set; }
        [Display(Name = "Cliente")]
        public String Nombre_Cliente { get; set; }

        [Display(Name = "fecha")]
        public DateTime fecha { get; set; }

        [Display(Name = "subtotal")]
        public Decimal Subtotal { get; set; }
        [Display(Name = "Descuento")]
        public Decimal Descuento { get; set; }

        [Display(Name = "IVA")]
        public Decimal IVA { get; set; }

        [Display(Name = "Total")]
        public Decimal Total { get; set; }
        [Display(Name = "IDMoneda")]
        public int IDMoneda { get; set; }

        [Display(Name = "Moneda")]
        public string MonedaOrigen { get; set; }
        public decimal TC { get; set; }

        public string Estado { get; set; }

        [Display(Name = "IDMetodoPago")]
        public int IDMetodoPago { get; set; }
        public decimal ImporteSaldoAnterior { get; set; }
        public decimal ImportePagado { get; set; }
        public decimal ImporteSaldoInsoluto { get; set; } 
         public int NoParcialidad { get; set; }
        public bool pagada { get; set; }

   
}
    public class VEncFacturaFContext : DbContext
    {
        public VEncFacturaFContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VEncFacturaF> VEncFacturaFs { get; set; }

    }
}