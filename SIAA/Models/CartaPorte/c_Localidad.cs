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
   [Table ("c_Localidad")]
    public class c_Localidad
    {
        [Key]
        public int IDLocalidad { get; set; }
        [DisplayName("Localidad")]
        public string C_Localidad { get; set; }
        [DisplayName("Estado")]
        public string C_Estado { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }

    }
    public class c_LocalidadContext : DbContext
    {
        public c_LocalidadContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_Localidad> Localidad { get; set; }
    }
}