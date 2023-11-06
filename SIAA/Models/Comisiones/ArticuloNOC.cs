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

namespace SIAAPI.Models.Comisiones
{
    [Table("ArticuloNOC")]
    public class ArticuloNOC
    {
        [Key]
        public int IDArticuloNOC { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Articulo")]
        public int IDArticulo { get; set; }
        public virtual ICollection<Articulo> Articulo_IC { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("CREF")]
        public string CREF { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }

    }

    public class ArticuloNOCContext : DbContext
    {
        public ArticuloNOCContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ArticuloNOCContext>(null);
        }
        public DbSet<ArticuloNOC> ArticuloNOC_BD { get; set; }
        public DbSet<Articulo> Articulo_BD { get; set; }

    }

    public class ArticuloNOCRepository
    {
        public IEnumerable<SelectListItem> GetArticulosNOC()
        {
            using (var context = new ArticuloNOCContext())
            {
                List<SelectListItem> lista = context.ArticuloNOC_BD.AsNoTracking()
                    .OrderBy(n => n.CREF)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.CREF,
                            Text = n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Seleccionar artículo ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }



        public string GetArticuloNCCREF(int _id)

        {
            ArticuloNOC elemento = new ArticuloNOCContext().ArticuloNOC_BD.Single(m => m.IDArticuloNOC == _id);
            return elemento.CREF;

        }


    }
}