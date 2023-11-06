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
    [Table("c_TipoCarro")]
    public class c_TipoCarro
    {

        [Key]
        public int IDTipoCarro { get; set; }
        [DisplayName("Clave")]
        public string Clave { get; set; }
        [DisplayName("Tipo Carro")]
        public string TipoCarro { get; set; }
        [DisplayName("Descripcion")]
        public string Descripcion { get; set; }

        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIVigencia { get; set; }

        [DisplayName("Fecha de Terminación")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FFVigencia { get; set; }
    }
    public class c_TipoCarroContext : DbContext
    {
        public c_TipoCarroContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_TipoCarro> carro { get; set; }
    }
}