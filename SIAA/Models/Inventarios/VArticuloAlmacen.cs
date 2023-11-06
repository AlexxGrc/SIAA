using SIAAPI.Models.Comercial;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;


namespace SIAAPI.Models.Inventarios
{
    [Table("VArticuloAlmacen")]
    public class VArticuloAlmacen
    {
        [Key]
        public string IDV { get; set; }

        public int IDCaracteristica { get; set; }

        public string Referencia { get; set; }

        public string Producto { get; set; }

        public string Presentacion { get; set; }
        public decimal Existencia { get; set; }

        public int IDAlmacenSalida { get; set; }
        public string Almacen { get; set; }
        
    }

    public class VArticuloAlmacenContext : DbContext
    {
        public VArticuloAlmacenContext() : base("name=DefaultConnection")
        {
          //Database.SetInitializer<VArticuloAlmacenContext>(null);
        }
        public DbSet<VArticuloAlmacen> VArticuloAlmacenes { get; set; }
    }
    public class VArticuloAlmacenRepository
    {
        public IEnumerable<SelectListItem> GetVArticuloAlmacen()
    {
        using (var context = new VArticuloAlmacenContext())
        {
            List<SelectListItem> listaArtAlm = context.VArticuloAlmacenes.AsNoTracking()
                .OrderBy(n => n.IDAlmacenSalida).ThenBy(n => n.Producto)
                    .Select(n =>
                    new SelectListItem
                    {
                        Value = n.Referencia.ToString(),
                        Text = n.Referencia + "\t|" + n.Producto
                    }).ToList();
            var descripciontip = new SelectListItem()
            {
                Value = "0",
                Text = "--- Selecciona un Producto ---"
            };
                listaArtAlm.Insert(0, descripciontip);
            return new SelectList(listaArtAlm, "Value", "Text");
        }
    }
    }
   
}