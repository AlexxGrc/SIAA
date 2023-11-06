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
    [Table("c_ConfigMaritima")]
    public class c_ConfigMaritima
    {
        [Key]
        public int IDCMaritima { get; set; }
        [DisplayName("Clave")]
        public string Clave { get; set; }
        [DisplayName("Descripcion")]
        public string Descripcion { get; set; }
        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIVigencia { get; set; }

        [DisplayName("Fecha de Terminación")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FFVigencia { get; set; }
    }
    public class c_ConfigMaritimaContext : DbContext
    {
        public c_ConfigMaritimaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_ConfigMaritima> config { get; set; }
    }
}