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
    [Table("c_ClaveTipoCarga")]
    public class c_ClaveTipoCarga
    {
        [Key]
        public int IDTipoCarga { get; set; }
        [DisplayName("Clave")]
        public String Clave { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Fecha Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIVigencia { get; set; }
        [DisplayName("Fecha Final")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FFVigencia { get; set; }
    }
    public class c_ClaveTipoCargaContext : DbContext
    {
        public c_ClaveTipoCargaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_ClaveTipoCarga> ClaveTipoCarga { get; set; }
    }
}