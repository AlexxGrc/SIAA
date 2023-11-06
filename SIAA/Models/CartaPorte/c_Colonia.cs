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
    [Table("c_Colonia")]
    public class c_Colonia
    {
        [Key]
        public int IDColonia { get; set; }
        [DisplayName("Colonia")]
        public string C_Colonia { get; set; }
        [DisplayName("Codigo Postal")]
        public string C_CodigoPostal { get; set; }
        [DisplayName("Nombre del asentamiento")]
        public string NomAsentamiento { get; set; }
    }
    public class c_ColoniaContext : DbContext
    {
        public c_ColoniaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_Colonia> colonias { get; set; }
    }
}