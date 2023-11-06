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
    [Table("c_ClaveProdServCP")]
    public class c_ClaveProdServCP
    {

        [Key]
        [DisplayName("Clave Producto")]
        public int ClaveP { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Palabras Similares")]
        public string PalabrasS { get; set; }
        [DisplayName("Material Peligroso")]
        public string MaterialP { get; set; }
        [DisplayName("Fecha de inicio de vigencia")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIVigencia { get; set; }
        [DisplayName("Fecha de fin de vigencia")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FFVigencia { get; set; }
    }
    public class c_ClaveProdServCPContext : DbContext
    {
        public c_ClaveProdServCPContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<c_ClaveProdServCP> clave { get; set; }
        //public System.Data.Entity.DbSet<SIAAPI.Models.CartaPorte.c_ClaveProdServCP> c_ClaveProdServCP { get; set; }
    }

}