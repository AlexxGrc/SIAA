
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using SIAAPI.Models.Comercial;

namespace SIAAPI.Models.Administracion
{
    [Table("c_ClaveProductoServicio")]
    public class c_ClaveProductoServicio
    {
        [Key]
        public int IDProdServ { get; set; }
   

        [DisplayName("Clave del Producto o Servicio")]
        [StringLength(10, ErrorMessage = "Has excedido el límite de 10 caracteres")]
          public string ClaveProdServ { get; set; }

        [DisplayName("Descripción")]
        [StringLength(255, ErrorMessage = "Has excedido el límite de 255 caracteres")]
        public string Descripcion { get; set; }

        public virtual ICollection<Familia> Familia { get; set; }
    }
    public class c_ClaveProductoServicioContext : DbContext
    {
        public c_ClaveProductoServicioContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_ClaveProductoServicio> c_ClaveProductoServicios { get; set; }

    }

    public class ProductoServicioRepository
    {
        public IEnumerable<SelectListItem> GetProductosoServicios()
        {
            using (var context = new c_ClaveProductoServicioContext())
            {
                List<SelectListItem> lista = context.c_ClaveProductoServicios.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDProdServ.ToString(),
                            Text = n.ClaveProdServ +"|"+ n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Producto del SAT ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

    }
}

