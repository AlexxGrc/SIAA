using SIAAPI.Models.Comercial;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SIAAPI.Models.Administracion
{
    public partial class c_FormaPago
    {
        [Key]
        public int IDFormaPago { get; set; }


        [Required(ErrorMessage = "La clave es obligatoria")]
        [DisplayName("Código del Sat")]
        [StringLength(5)]
        public string ClaveFormaPago { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [DisplayName("Descripción")]
        [StringLength(100)]
        public string Descripcion { get; set; }

        public virtual ICollection<Clientes> Clientes { get; set; }
        public virtual ICollection<Proveedor> Proveedores { get; set; }
    }

    public class c_FormaPagoContext : DbContext
    {
        public c_FormaPagoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_FormaPago> c_FormaPagos { get; set; }

    }


    public class FormaPagoRepository
    {
        public IEnumerable<SelectListItem> GetFormasdepago()
        {
            using (var context = new c_FormaPagoContext())
            {
                List<SelectListItem> lista = context.c_FormaPagos.AsNoTracking()
                    .OrderBy(n => n.ClaveFormaPago)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDFormaPago.ToString(),
                            Text = n.ClaveFormaPago + "|" + n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una Forma de pago ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }
        public IEnumerable<SelectListItem> GetFormasdepagoElectronica()
        {
            using (var context = new c_FormaPagoContext())
            {
                List<SelectListItem> lista = context.c_FormaPagos.AsNoTracking()
                    .Where(n => n.ClaveFormaPago != "01")
                    .OrderBy(n => n.ClaveFormaPago)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDFormaPago.ToString(),
                            Text = n.ClaveFormaPago + "|" + n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una Forma de pago ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetFormasdepagoOtra()
        {
            using (var context = new c_FormaPagoContext())
            {
                List<SelectListItem> lista = context.c_FormaPagos.AsNoTracking()
                    .Where(n => n.ClaveFormaPago == "01" || n.ClaveFormaPago == "08" || n.ClaveFormaPago == "17")
                    .OrderBy(n => n.ClaveFormaPago)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDFormaPago.ToString(),
                            Text = n.ClaveFormaPago + "|" + n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una Forma de pago ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

    }
}