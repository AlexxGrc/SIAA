using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SIAAPI.Models.Administracion
{
    [Table("TipoVendedor")]
    public partial class TipoVendedor
    {
        [Key]
        public int IDTipoVendedor { get; set; }
        


        [Required(ErrorMessage = "La descripcion es obligatoria")]
        [DisplayName("Descripción")]
        [StringLength(100)]
        public string DescripcionVendedor { get; set; }
        public virtual ICollection<Vendedor> Vendedor { get; set; }
       
    }

    public class TipoVendedorContext : DbContext
    {
        public TipoVendedorContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<TipoVendedor> TipoVendedores { get; set; }
    }

    public class TipoVendedorRepository
    {
        private SelectListItem descripcionvendedortip;

        public IEnumerable<SelectListItem> GetDescripcionVendedor()
        {
            using (var context = new TipoVendedorContext())
            {
                List<SelectListItem> listaTipoVendedor = context.TipoVendedores.AsNoTracking()
                    .OrderBy(n => n.DescripcionVendedor)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDTipoVendedor.ToString(),
                            Text = n.DescripcionVendedor
                        }).ToList();
                var descripcionvendedortip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona el Tipo ---"
                };
                listaTipoVendedor.Insert(0, descripcionvendedortip);
                return new SelectList(listaTipoVendedor, "Value", "Text");
            }
        }
    }
     
}
