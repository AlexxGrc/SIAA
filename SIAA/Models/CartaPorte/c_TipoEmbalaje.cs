using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace SIAAPI.Models.CartaPorte
{
    [Table("c_TipoEmbalaje")]
    public class c_TipoEmbalaje
    {
        [Key]
        public int IDTEmbalaje { get; set; }
        [DisplayName("Clave Asignación")]
        public string ClaveDeAsignacion { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIVigencia { get; set; }
        [DisplayName("Fecha de Fin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FFVigencia { get; set; }
    }
    public class c_TipoEmbalajeContext : DbContext
    {
        public c_TipoEmbalajeContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_TipoEmbalaje> TipoEmbalaje { get; set; }
    }
}