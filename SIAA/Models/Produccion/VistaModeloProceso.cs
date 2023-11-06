using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SIAAPI.Models.Produccion
{
    [Table("VModeloProceso")]
    public class VistaModeloProceso
    {
        [Key]
        public int IDModeloProceso { get; set; }

        public int IDModeloProduccion { get; set; }

        [DisplayName("Modelo de Produccion")]
        [StringLength(150)]
        public string Descripcion { get; set; }

        public int IDProceso { get; set; }

        [DisplayName("Nombre del Proceso")]
        [StringLength(150)]
        public string NombreProceso { get; set; }

        [DisplayName("Orden del proceso")]
        public int orden { get; set; }

    }
    public class VistaModeloProcesoContext : DbContext
    {
        public VistaModeloProcesoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VistaModeloProceso> VistaModeloProcesos { get; set; }
      

    }

    public class VistaModeloRepository
    {
        public IEnumerable<SelectListItem> GetProcesos(int id)
        {
            using (var context = new VistaModeloProcesoContext())
            {
                List<SelectListItem> lista = context.VistaModeloProcesos.AsNoTracking()
                    .OrderBy(n => n.orden).Where(n=> n.IDModeloProduccion==id )
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDProceso.ToString(),
                            Text = n.NombreProceso
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Proceso---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetProcesosSolamente(int id)
        {
            using (var context = new VistaModeloProcesoContext())
            {
                List<SelectListItem> lista = context.VistaModeloProcesos.AsNoTracking()
                    .OrderBy(n => n.orden).Where(n => n.IDModeloProduccion == id)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDProceso.ToString(),
                            Text = n.NombreProceso
                        }).ToList();
               
                return new SelectList(lista, "Value", "Text");
            }
        }

    }
}