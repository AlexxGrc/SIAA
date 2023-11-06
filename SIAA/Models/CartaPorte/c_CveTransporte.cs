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
    [Table("c_CveTransporte")]
    public class c_CveTransporte
    {
        [Key]
        public int IDCve { get; set; }
        [DisplayName("Clave")]
        public string Clave { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIvigencia { get; set; }

        [DisplayName("Fecha de Terminación")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FFvigencia { get; set; }

        [DisplayName("Versión")]
        public string Version { get; set; }
        [DisplayName("Revisión")]
        public int Revision { get; set; }

        [DisplayName("Fecha de Publicación")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fpublicacion { get; set; }
    }
    public class c_CveTransporteContext : DbContext
    {
        public c_CveTransporteContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_CveTransporte> transport { get; set; }
    }

}