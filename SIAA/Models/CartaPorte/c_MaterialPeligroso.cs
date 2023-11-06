using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIAAPI.Models.CartaPorte
{
    [Table("c_MaterialPeligroso")]
    public class c_MaterialPeligroso
    {
        [Key]
        public int IDMatP { get; set; }
        [DisplayName("Clave Material Peligroso")]
        public string ClaveMatP { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Clase o div.")]
        public string ClaseDiv { get; set; }
        [DisplayName("Peligro secundario")]
        public string PeligSec { get; set; }
        [DisplayName("Grupo de emb/env ONU")]
        public string GrupoEmb_Env { get; set; }
        [DisplayName("Disp. espec.")]
        public string DispEsp { get; set; }
        [DisplayName("Cantidades limitadas")]
        public string CantLim { get; set; }
        [DisplayName("Cantidades exceptuadas")]
        public string CantExp { get; set; }
        [DisplayName("Inst. de emb/env")]
        public string Embalaje { get; set; }
        [DisplayName("Disp. espec.")]
        public string RIG { get; set; }
        [DisplayName("Inst. de transp.")]
        public string Cisternas { get; set; }
        [DisplayName("Disp. espec")]
        public string Contenedores { get; set; }
        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha_In { get; set; }
        [DisplayName("Fecha de Fin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Fecha_Fin { get; set; }

    }
    public class c_MaterialPeligrosoDBContext : DbContext
    {
        public c_MaterialPeligrosoDBContext() : base("name = DefaultConnection")
        {

        }
        public DbSet<c_MaterialPeligroso> MaterialP { get; set; }
    }

}