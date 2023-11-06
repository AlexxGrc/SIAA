using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SIAAPI.Models.Produccion
{
    [Table("ModelosDeProduccion")]
    public class ModelosDeProduccion
    {
        [Key]
        public int IDModeloProduccion { get; set; }
       public IEnumerable<SelectListItem> ModelosdeProduccion { get; set; }

        [Required(ErrorMessage = "El nombre del modelo del proceso es obligatorio y acepta como máximo 150 caracteres")]
        [DisplayName("Nombre del modelo del proceso")]
        [StringLength(150)]
        public string Descripcion { get; set; }


    }
    
    public class ModelosDeProduccionContext : DbContext
    {
        public ModelosDeProduccionContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<ModelosDeProduccion> ModelosDeProducciones { get; set; }
    }
    public class ModelosDeProduccionRepository
    {

        public IEnumerable<SelectListItem> GetDescripcion()
        {
            using (var context = new ModelosDeProduccionContext())
            {
                List<SelectListItem> lista = context.ModelosDeProducciones.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDModeloProduccion.ToString(),
                            Text = n.Descripcion
                        }).ToList();
                SelectListItem descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona un modelo de produccion ---"
                };
                lista.Insert(0, descripciontip);
                return new SelectList(lista, "Value", "Text");
            }
        }
    }
    
}
