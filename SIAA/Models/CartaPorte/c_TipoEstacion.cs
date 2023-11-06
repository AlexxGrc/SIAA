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
    [Table("c_TipoEstacion")]
    public class c_TipoEstacion
    {
        [Key]
        public int IDTEstacion { get; set; }
        [DisplayName("Clave Tipo Estación")]
        public string ClaveEstacion { get; set; }
        [DisplayName ("Descripción")]
        public string DescripcionEstacion { get; set; }
        [DisplayName("Clave Tansporte")]
        public string ClaveTransporte { get; set; }
        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIVigencia { get; set; }
        [DisplayName("Fecha de Fin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime?  FFVigencia { get; set; }
    }

    public class c_TipoEstacionContext : DbContext
    {
        public c_TipoEstacionContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_TipoEstacion> TipoEstacion { get; set; }
    }
}