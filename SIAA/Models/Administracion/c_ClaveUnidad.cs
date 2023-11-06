using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace SIAAPI.Models.Administracion
{
    [Table("c_ClaveUnidad")]
    public partial class c_ClaveUnidad
    {
        [Key]
        
        public int IDClaveUnidad { get; set; }

        [Required]
        [DisplayName("Clave de la Unidad")]
        [StringLength(5)]
        public string ClaveUnidad { get; set; }

        [StringLength(150)]
        [DisplayName("Nombre")]
        public string Nombre { get; set; }

    }
    public class c_ClaveUnidadContext : DbContext
    {
        public c_ClaveUnidadContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_ClaveUnidad> c_ClaveUnidades { get; set; }

    }
}
