using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Administracion
{
    [Table("c_TipoIVA")]
    public class c_TipoIVA
    {
        [Key]
        public int IDTipoIVA { get; set; }


        [Required(ErrorMessage = "La Tasa es obligatoria")]
        [DisplayName("Tasa")]
        public decimal Tasa { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [DisplayName("Descripción")]
        [StringLength(30)]
        public string Descripcion { get; set; }
        
    }

    public class c_TipoIVAContext : DbContext
    {
        public c_TipoIVAContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_TipoIVA> c_TiposIVA { get; set; }
    }
}