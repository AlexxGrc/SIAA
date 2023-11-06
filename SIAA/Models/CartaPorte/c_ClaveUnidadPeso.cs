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
    [Table ("c_ClaveUnidadPeso")]
    public class c_ClaveUnidadPeso
    {
    [Key]
    public int IDPeso { get; set; }
        [DisplayName("Clave Unidad")]
    public string ClaveUnidad { get; set; }
        [DisplayName("Nombre")]
    public string Nombre { get; set; }
        [DisplayName("Descripción")]
    public string Descripcion { get; set; }
        [DisplayName("Nota")]
    public string Nota { get; set; }
    [DisplayName("Fecha de Inicio")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime FIVigencia { get; set; }
    [DisplayName("Fecha de Fin")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? FFVigencia { get; set; }
        [DisplayName("Simbolo")]
    public string Simbolo { get; set; }
        [DisplayName("Bandera")]
    public string Bandera { get; set; }
    }
    public class c_ClaveUnidadPesoContext : DbContext
    {
        public c_ClaveUnidadPesoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_ClaveUnidadPeso> UnidadPeso { get; set; }
    }
}