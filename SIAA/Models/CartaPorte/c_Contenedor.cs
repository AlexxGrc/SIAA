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
    [Table ("c_Contenedor")]
    public class c_Contenedor
    {
        [Key]
        public int IDContenedor { get; set; }
        [DisplayName("Colonia")]
        public string C_Colonia { get; set; }
        [DisplayName("Código Postal")]
        public int C_CodigoPostal { get; set; }
        [DisplayName("Norma de Asentamiento")]
        public string NomAsentamiento { get; set; }
        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIVigencia { get; set; }
        [DisplayName("Fecha de Fin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FFVigencia { get; set; }
    }
    public class c_ContenedorContext : DbContext
    {
        public c_ContenedorContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_Contenedor> Contenedor { get; set; }
    }
}