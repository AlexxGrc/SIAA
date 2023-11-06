namespace SIAAPI.Models.Administracion
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    public partial class c_UsoCFDI
    {
        [Key]
        public int IDUsoCFDI { get; set; }

        [Index]
        [Required(ErrorMessage = "La clave del SAT es obligatoria")]
        [DisplayName("Código del Sat")]
        [StringLength(3)]
        public string ClaveCFDI { get; set; }

        [Required(ErrorMessage = "La descrición es obligatoria")]
        [DisplayName("Descripción")]
        [StringLength(100)]
        public string Descripcion { get; set; }

        

       
    }

    public class c_UsoCFDIContext : DbContext
    {
        public c_UsoCFDIContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_UsoCFDI> c_UsoCFDIS { get; set; }
    }

    public class UsoCfdiRepository
    {
        public IEnumerable<SelectListItem> GetusosCfdi()
        {
            using (var context = new c_UsoCFDIContext())
            {
                List<SelectListItem> lista = context.c_UsoCFDIS.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDUsoCFDI.ToString(),
                            Text = n.ClaveCFDI.ToString() +" | "+ n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un uso del CFDI ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }


        public IEnumerable<SelectListItem> GetusosCfdiDevoluciones()
        {
            using (var context = new c_UsoCFDIContext())
            {
                List<SelectListItem> lista = context.c_UsoCFDIS.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDUsoCFDI.ToString(),
                            Text = n.ClaveCFDI.ToString() + " | " + n.Descripcion
                        }).ToList();
                var selectedCustomerType = lista.FirstOrDefault(d => d.Value == "G02");
                if (selectedCustomerType != null)
                    selectedCustomerType.Selected = true;
          
                return new SelectList(lista, "Value", "Text");
            }
        }
    }
}
