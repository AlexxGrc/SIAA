using SIAAPI.Models.Comercial;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SIAAPI.Models.Administracion
{


    public partial class c_RegimenFiscal
    {

        [Key]
        public int IDRegimenFiscal { get; set; }

        [Required(ErrorMessage = "La clave del SAT es obligatoria")]
        [DisplayName("Clave Régimen Fiscal")]
        public int ClaveRegimenFiscal { get; set; }

        [Required(ErrorMessage = "La Descripción es obligatoria y como máximo 100 caracteres")]
        [DisplayName("Descripción")]
        [StringLength(100)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio y como máximo 15 caracteres")]
        [DisplayName("Persona Física(SI/NO)")]
        [StringLength(2)]
        public string AplicaFisica { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio y como máximo 15 caracteres")]
        [DisplayName("Persona Moral(SI/NO)")]
        [StringLength(2)]
        public string AplicaMoral { get; set; }
        public virtual ICollection<Clientes> Clientes { get; set; }
        public virtual ICollection<Proveedor> Proveedores { get; set; }

        public virtual ICollection<Empresa> Empresa { get; set; }

    }
    public class c_RegimenFiscalContext : DbContext
    {
        public c_RegimenFiscalContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_RegimenFiscal> c_RegimenFiscales { get; set; }
    }

    public class c_RegimenFiscalRepository
    {
        public IEnumerable<SelectListItem> GetRegimenes()
        {
            using (var context = new c_RegimenFiscalContext())
            {
                List<SelectListItem> regimenes = context.c_RegimenFiscales.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDRegimenFiscal.ToString(),
                            Text = n.ClaveRegimenFiscal+"|"+n.Descripcion
                        }).ToList();
                var renglon = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Régimen Fiscal ---"
                };
                regimenes.Insert(0, renglon);
                return new SelectList(regimenes, "Value", "Text");
            }
        }



    }
}
