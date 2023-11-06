using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace SIAAPI.Models.Comisiones
{
    [Table("PenalizacionFactura")]
    public class Penalizacion_Factura
    {
        [Key]

        public int IdPenalizacionFactura { get; set; }

        
        [DisplayName("IdFactura")]
        public int IdFactura { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Motivo")]
        public string Motivo { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Monto")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Mes")]
        public int Mes { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Periodo")]
        public int Periodo { get; set; }

    }



    public class Penalizacion_FacturaContext : DbContext
    {
        public Penalizacion_FacturaContext() : base("name=DefaultConnection")
        {

        }

        public DbSet<Penalizacion_Factura> PFacturas { get; set; }
    }

}

