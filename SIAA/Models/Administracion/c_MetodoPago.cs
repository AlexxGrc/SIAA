namespace SIAAPI.Models.Administracion
{
    using Comercial;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Web.Mvc;

    public partial class c_MetodoPago
    {
        [Key]
        public int IDMetodoPago { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria y de 3 caracteres")]
        [DisplayName("Código de Sat")]
        [StringLength(3)]
        public string ClaveMetodoPago { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria y admite como máximo 50 caracteres")]
        [DisplayName("Descripción")]
        [StringLength(50)]
         public string Descripcion { get; set; }
        public virtual ICollection<Clientes> Clientes { get; set; }

       
    }

    public class c_MetodoPagoContext : DbContext
    {
        public c_MetodoPagoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_MetodoPago> c_MetodoPagos { get; set; }
    }

    public class MetodoPagoRepository
    {
        public IEnumerable<SelectListItem> GetMetodosdepago()
        {
            using (var context = new c_MetodoPagoContext())
            {
                List<SelectListItem> lista = context.c_MetodoPagos.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDMetodoPago.ToString(),
                            Text = n.ClaveMetodoPago +" | " + n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Metodo de pago ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

    }
}
