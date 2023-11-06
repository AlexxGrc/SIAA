namespace SIAAPI.Models.Administracion
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    public partial class c_TipoRelacion
    {
        [Key]
        public int IDTipoRelacion { get; set; }
        [StringLength(3)]
        [DisplayName("Clave de Relación")]
        public string ClaveTipoRelacion { get; set; }

        [StringLength(100)]
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
    }


    public class c_TipoRelacionContext : DbContext
    {
        public c_TipoRelacionContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_TipoRelacion> c_TipoRelaciones { get; set; }
    }

    public class TipoRelacionRepository
    {
        public IEnumerable<SelectListItem> GetTiposderelacion()
        {
            using (var context = new c_TipoRelacionContext())
            {
                List<SelectListItem> lista = context.c_TipoRelaciones.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDTipoRelacion.ToString(),
                            Text = n.ClaveTipoRelacion.ToString() +" | " +n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Tipo de Relacion ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

    }
}
