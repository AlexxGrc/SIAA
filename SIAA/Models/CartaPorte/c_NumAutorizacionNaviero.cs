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
    [Table("c_NumAutorizacionNaviero")]
    public class c_NumAutorizacionNaviero
    {
        [Key]
        public int IDANaviero { get; set; }
        [DisplayName("Numero Autorizado")]
        public string NumAutorizado { get; set; }
        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIVigencia { get; set; }

        [DisplayName("Fecha de Terminación")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FFVigencia { get; set; }
    }
    public class c_NumAutorizacionNavieroContext : DbContext
    {
        public c_NumAutorizacionNavieroContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_NumAutorizacionNaviero> numAut { get; set; }
    }
}