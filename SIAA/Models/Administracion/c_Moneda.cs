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

namespace SIAAPI.Models.Administracion
{
    [Table("c_Moneda")]

    public class c_Moneda
    {
        [Key]
        public int IDMoneda { get; set; }

        [Required(ErrorMessage = "La clave del SAT es obligatoria")]
        [DisplayName("Clave del SAT")]
        [StringLength(3)]
        public string ClaveMoneda { get; set; }


        [Required(ErrorMessage = "La descripción es obligatoria")]
        [DisplayName("Descripción")]
        [StringLength(100)]
        public string Descripcion { get; set; }

        [DisplayName("Decimales que acepta")]
        //[DefaultValue(0)]
        public int NoDecimales { get; set; }

        [DisplayName("Variación")]
        //[DefaultValue(500)]
        public int Variacion { get; set; }
        public virtual ICollection<Clientes> Clientes { get; set; }

        public virtual ICollection<Articulo> Articulo { get; set; }

        public virtual ICollection<Proveedor> Proveedores { get; set; }
        public virtual ICollection<Vendedor> Vendedor { get; set; }
        public virtual ICollection<TipoCambio> TipoCambio { get; set; }
        public virtual ICollection<Encfacturas> encfacturas { get; set; }
        public virtual ICollection<BancoCliente> BancoCliente { get; set; }
    }


    public class c_MonedaContext : DbContext
    {
        public c_MonedaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_Moneda> c_Monedas { get; set; }

    }

    public class MonedaRepository
    {
        public IEnumerable<SelectListItem> GetMoneda()
        {
            using (var context = new c_MonedaContext())
            {
                List<SelectListItem> lista = context.c_Monedas.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDMoneda.ToString(),
                            Text = n.ClaveMoneda + " | " + n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una Moneda ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }



        public string getNombreMoneda(int _id)

        {
            c_Moneda elemento = new c_MonedaContext().c_Monedas.Single(m => m.IDMoneda == _id);
            return elemento.ClaveMoneda + " " + elemento.Descripcion;

        }


    }


}
