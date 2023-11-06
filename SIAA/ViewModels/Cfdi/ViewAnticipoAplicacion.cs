using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Cfdi
{
    public class ViewAnticipoAplicacion
    {
        [Key]
        public int IDComplemento { get; set; }

        [Required]
        [StringLength(36, ErrorMessage = "El UUID debe ser de 36 caracteres. ")]
        [Display(Name = "UUID DE LA FACTURA CON ANTICIPO RELACIONADO")]
        public string  UUID { get; set; }


        [Required]
        [Display(Name = "Serie")]
        public int serie { get; set; }

      
    }

    [Table("AplicacionAnticipos")]

    public class AplicacionAnticipos
    {
        [Key]

       public  int  IDAplicacion { get; set; }
       public int  IDFacturaAnticipo { get; set; }
        public DateTime Fecha { get; set; }

        public decimal Subtotal { get; set; }
        
        public decimal Iva { get; set; }

        public decimal Total { get; set; }
      
        public int IDPREFACTURAAPLICADA { get; set; }
    }


    public class aplicaranticipo
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "UUID DE LA FACTURA CON ANTICIPO RELACIONADO")]
        public string UUID { get; set; }
        //    public string  IDFactura { get; set; } // factura del  anticipo
        [Display(Name = "MONTO CON TODO E IMPUESTO")]
        public decimal Monto { get; set; }

        public int Prefactura { get; set; }

    }
    public class AplicacionAnticiposContext : DbContext
    {
        public AplicacionAnticiposContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<AplicacionAnticipos> AplicacionAnticiposs { get; set; }

    }

}