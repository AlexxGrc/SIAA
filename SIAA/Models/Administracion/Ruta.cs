using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Models.Administracion
{
    [Table("Ruta")]
    public class Ruta
    {
        [Key]
        public int IDRuta { get; set; }

        [Required(ErrorMessage = "La descripción de la ruta es obligatoria")]
        [DisplayName("Descripción de la ruta ")]
        [StringLength(200)]
        public string Descripcion { get; set; }

        public virtual ICollection<EntregaRemision> entrega { get; set; }
    }

    public class RutaRepository
    {
        public IEnumerable<SelectListItem> GetRuta()
        {
            using (var context = new RutasContext())
            {
                List<SelectListItem> lista = context.Rutas.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDRuta.ToString(),
                            Text = n.Descripcion

                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una Ruta---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetRuta(int IDRuta)
        {
            using (var context = new RutasContext())
            {
                List<SelectListItem> lista = context.Rutas.Where(s => s.IDRuta == IDRuta)
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDRuta.ToString(),
                            Text = n.Descripcion,
                            Selected = true

                        }).ToList();


                return new SelectList(lista, "Value", "Text");
            }
        }
    }

    public class RutasContext : DbContext
    {
        public RutasContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<RutasContext>(null);
        }
        public DbSet<Ruta> Rutas { get; set; }
    }
}