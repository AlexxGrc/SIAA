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
    public class c_CodigoTransporteAereo
    {
        [Key]
        public int IDTransAereo { get; set; }
        [DisplayName("Clave")]
        public String Clave { get; set; }
        [DisplayName("Nacionalidad")]
        public String Nacionalidad { get; set; }
        [DisplayName("Aerolínea")]
        public String Aerolinea { get; set; }
        [DisplayName("Designador")]
        public String Designador { get; set; }
        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIvigencia { get; set; }
        [DisplayName("Fecha de Fin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FFvigencia { get; set; }
    }
    public class c_CodigoTransporteAereoContext : DbContext
    {
        public c_CodigoTransporteAereoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_CodigoTransporteAereo>  Transporte { get; set; }
    }
}