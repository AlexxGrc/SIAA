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
    [Table("c_Estaciones")]
    public class c_Estaciones
    {
        [Key]
        public int IDEstacion { get; set; }
        [DisplayName("Clave Transporte")]
        public string Clave { get; set; }
        [DisplayName("Clave Identificación")]
        public string Clave_ID { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Nacionalidad")]
        public string Nacionalidad { get; set; }
        [DisplayName("Designador IATA")]
        public string Designador { get; set; }
        [DisplayName("Lìnea Ferrea")]
        public string Linea { get; set; }

        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIVigencia { get; set; }

        [DisplayName("Fecha de Terminación")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FFVigencia { get; set; }
    }
    public class c_EstacionesContext : DbContext
    {
        public c_EstacionesContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_Estaciones> estacion { get; set; }
    }
}