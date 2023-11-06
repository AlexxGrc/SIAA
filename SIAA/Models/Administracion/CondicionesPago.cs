using SIAAPI.Models.Comercial;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace SIAAPI.Models.Administracion
{
    [Table("CondicionesPago")]
   public class CondicionesPago
    {
        [Key]
        public int IDCondicionesPago { get; set; }

        [DisplayName("Clave")]
        [Index("IX_ClaConPag", 1, IsUnique = true)]
        [Required]
        [StringLength(3)]
        public string ClaveCondicionesPago { get; set; }

        [DisplayName("Descripción")]
        [Index("IX_DesConPag", 2, IsUnique = true)]
        [Required]
        [StringLength(50)]
        public string Descripcion { get; set; }

        [DisplayName("Día de Crédito")]
        [Required]
        
        public decimal DiasCredito { get; set; }

        public virtual ICollection<Clientes> Clientes { get; set; }
        public virtual ICollection<Proveedor> Proveedor { get; set; }

        public virtual ICollection<Enccotizapros> Enccotizapros { get; set; }
    }
    public class CondicionesPagoContext : DbContext
    {
        public CondicionesPagoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<CondicionesPago> CondicionesPagos { get; set; }


    }
}
