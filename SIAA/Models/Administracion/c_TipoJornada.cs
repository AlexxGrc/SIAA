namespace SIAAPI.Models.Administracion
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;

    public partial class c_TipoJornada
    {
        [Key]
        
        public int IdTipoJornada { get; set; }

        [Required]
        [StringLength(2)]
        [DisplayName("Clave Jornada")]
        public string ClaveTipoComprobante { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }

     }
    
     public class c_TipoJornadaContext : DbContext
    {
        public c_TipoJornadaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_TipoJornada> c_TipoJornadas{ get; set; }
    }
     
}
