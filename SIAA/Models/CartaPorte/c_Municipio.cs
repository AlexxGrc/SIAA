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
    [Table(" c_Municipio")]
    public class c_Municipio
    {
        [Key]
        public int IDMunicipio { get; set; }
        [DisplayName("Municipio")]
        public string C_Municipio { get; set; }
        [DisplayName("Estado")]
        public string C_Estado { get; set; }
        [DisplayName("Descripcion")]
        public string Descripcion { get; set; }
    }
    public class c_MunicipioContext : DbContext
    {
        public c_MunicipioContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_Municipio> municipio { get; set; }
    }
}