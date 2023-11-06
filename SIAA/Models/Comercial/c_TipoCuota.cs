using SIAAPI.Models.Comercial;
using SIAAPI.Models.Cfdi;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SIAAPI.Models.Administracion
{
    [Table("c_TipoCuota")]

    public class c_TipoCuota
    {
        [Key]
        public int IDTipoCuota { get; set; }
        public string TipoCuota { get; set; }
        public virtual ICollection<Clientes> Clientes { get; set; }
        public virtual ICollection<Articulo> Articulo { get; set; }
        public virtual ICollection<Proveedor> Proveedores { get; set; }
        public virtual ICollection<Vendedor> Vendedor { get; set; }
        public virtual ICollection<TipoCambio> TipoCambio { get; set; }
        public virtual ICollection<Encfacturas> encfacturas { get; set; }
        public virtual ICollection<BancoCliente> BancoCliente { get; set; }
    }


    public class c_TipoCuotaContext : DbContext
    {
        public c_TipoCuotaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_TipoCuota> c_TipoCuota { get; set; }

    }

    public class TipoCuotaRepository
    {
        public IEnumerable<SelectListItem> GetTipoCuota()
        {
            using (var context = new c_TipoCuotaContext())
            {
                List<SelectListItem> lista = context.c_TipoCuota.AsNoTracking()
                    .OrderBy(n => n.TipoCuota)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDTipoCuota.ToString(),
                            Text = n.TipoCuota
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un tipo ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }



        public string GetTipoCuotaNombre(int _id)

        {
            c_TipoCuota elemento = new c_TipoCuotaContext().c_TipoCuota.Single(m => m.IDTipoCuota == _id);
            return elemento.TipoCuota;

        }


    }


}
