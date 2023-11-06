using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.CartaPorte
{
    [Table("c_ContenedorMaritimo")]
    public class c_ContenedorMaritimo
    {
        [Key]
        public int IDCMaritimo { get; set; }
        [DisplayName("Clave contenedor")]
        public String ClaveContenedor { get; set; }
        [DisplayName("Descripción")]
        public String Descripcion { get; set; }
        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIvigencia { get; set; }
        [DisplayName("Fecha Fin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FFvigencia { get; set; }
    }
    public class c_ContenedorMaritimoContext : DbContext
    {
        public c_ContenedorMaritimoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_ContenedorMaritimo> contenedor { get; set; }
    }
}