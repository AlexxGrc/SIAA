namespace SIAAPI.Models.Comercial
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Web.Mvc;
    using System.Linq;

    [Table("c_PeriocidadPago")]
    public partial class c_PeriodicidadPago
    {
       [Key]
        public int IDPeriocidadPago { get; set; }
        public IEnumerable<SelectListItem> periocidadpago { get; set; }

        [Required(ErrorMessage = "La clave del SAT es obligatoria")]
        [DisplayName("Código del Sat")]
        [StringLength(2)]
        public string ClavePeriocidad { get; set; }

        [Required(ErrorMessage = "La Descripción es obligatoria")]
        [DisplayName("Descripción")]
        [StringLength(50)]
        public string Descripcion { get; set; }
        public virtual ICollection<Vendedor> Vendedor { get; set; }
    }

    public class c_PeriodicidadPagoContext : DbContext
    {
        public c_PeriodicidadPagoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_PeriodicidadPago> c_PeriocidadPagos { get; set; }
    }

    public class c_PeriodicidadPagoRepository
    {
       

        public IEnumerable<SelectListItem> GetDescripcion()
        {
            using (var context = new c_PeriodicidadPagoContext())
            {
                List<SelectListItem> listaPeriocidadPago = context.c_PeriocidadPagos.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDPeriocidadPago.ToString(),
                            Text = n.Descripcion
                        }).ToList();
                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona la periocidad ---"
                };
                listaPeriocidadPago.Insert(0, descripciontip);
                return new SelectList(listaPeriocidadPago, "Value", "Text");
            }
        }
    }

}
