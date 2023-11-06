using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Cfdi;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using SIAAPI.Models.Comisiones;

namespace SIAAPI.Models
{
    [Table("c_Meses")]
    public class c_Meses
    {
        [Key]
        public int IDMes { get; set; }
        public string Mes { get; set; }

        public virtual ICollection<CierreVentas> cierreventa { get; set; }

    }

    public class c_MesesContext : DbContext
    {
        public c_MesesContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_Meses> c_Meses { get; set; }

    }

    public class MesesRepository
    {
        public IEnumerable<SelectListItem> GetMeses()
        {
            using (var context = new c_MesesContext())
            {
                List<SelectListItem> lista = context.c_Meses.AsNoTracking()
                    .OrderBy(n => n.IDMes)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDMes.ToString(),
                            Text = n.Mes
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Seleccionar mes ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }



        public string GetMesNombre(int _id)

        {
            c_Meses elemento = new c_MesesContext().c_Meses.Single(m => m.IDMes == _id);
            return elemento.Mes;

        }


    }
}