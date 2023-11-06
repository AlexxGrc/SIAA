using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web.Mvc;

namespace SIAAPI.Models.Produccion
{
    [Table("ModeloProceso")]
    public class ModeloProceso
    {
            [Key]
            public int IDModeloProceso { get; set; }
            

        [Required(ErrorMessage = "Elija un modelo")]
            [DisplayName("Clave del Modelo de producción")]
            public int ModelosDeProduccion_IDModeloProduccion { get; set; }
         //  public virtual ModelosDeProduccion ModelosDeProduccion { get; set; }
            

        [Required(ErrorMessage = "Elija un proceso")]
            [DisplayName("Clave del proceso que incluye")]
            public int Proceso_IDProceso { get; set; }

          //  public virtual Proceso Proceso { get; set; }

        [Required(ErrorMessage = "Establezca el orden del proceso")]
            [DisplayName("Orden que lleve en el proceso")]
            public int orden { get; set; }


        }

        public class ModeloProcesoContext : DbContext
        {
            public ModeloProcesoContext() : base("name=DefaultConnection")
            {

            }
            public DbSet<ModeloProceso> ModeloProceso { get; set; }

        }
   
}
