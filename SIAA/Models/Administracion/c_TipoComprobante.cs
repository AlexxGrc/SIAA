namespace SIAAPI.Models.Administracion
{
    using Cfdi;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    public partial class c_TipoComprobante
    {
        [Key]
        public int IDTipoComprobante { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria y de tipo Caracter")]
        [DisplayName("Código de Sat")]
        [StringLength(1)]
        public string ClaveTipoComprobante { get; set; }

        [Required(ErrorMessage = "La Descripcion es obligatoria y admite máximo 100 caracteres")]
        [DisplayName("Descripción")]
        [StringLength(100)]
        public string Descripcion { get; set; }

        [DisplayName("Valor Máximo")]
        [StringLength(50)]
        public string ValorMaximo { get; set; }

        public virtual ICollection<Folio> Folio { get; set; }
        public virtual ICollection<Encfacturas> Encfacturas { get; set; }
    }


    public class c_TipoComprobanteContext : DbContext
    {
        public c_TipoComprobanteContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_TipoComprobante> c_TipoComprobantes { get; set; }
    }

    public class TipoComprobanteRepository
    {
        public IEnumerable<SelectListItem> GetTiposComprobante()
        {
            using (var context = new c_TipoComprobanteContext())
            {
                List<SelectListItem> lista = context.c_TipoComprobantes.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDTipoComprobante.ToString(),
                            Text = n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Tipo de Comprobante  ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetTiposComprobanteEgreso()
        {
            using (var context = new c_TipoComprobanteContext())
            {
                List<SelectListItem> lista = context.c_TipoComprobantes.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDTipoComprobante.ToString(),
                            Text = n.Descripcion
                        }).ToList();
                var selectedCustomerType = lista.FirstOrDefault(d => d.Value == "I");
                if (selectedCustomerType != null)
                    selectedCustomerType.Selected = true;

                return new SelectList(lista, "Value", "Text");
            }
        }

    }
}
