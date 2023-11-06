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

namespace SIAAPI.Models.Comercial
{
    [Table("Oficina")]
    public partial class Oficina
    {
        [Key]
        public int IDOficina { get; set; }
      

        [Required(ErrorMessage = "El Nombre de la oficina es obligatorio")]
        [DisplayName("Nombre de la oficina")]
        [StringLength(50)]
        public string NombreOficina { get; set; }

        [DisplayName("Responsable")]
        [StringLength(100)]
        public string Responsable { get; set; }

        [DisplayName("Teléfono")]
        [StringLength(15)]
        public string Telefono { get; set; }

        [DisplayName("Extensión")]
        public int Extension { get; set; }
        public virtual ICollection<Clientes> Clientes { get; set; }
        public virtual ICollection<Vendedor> Vendedor { get; set; }
    }

    public class OficinaContext : DbContext
    {
        public OficinaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Oficina> Oficinas { get; set; }
    }

    public class OficinaRepository
    {
       

        public IEnumerable<SelectListItem> GetOficina()
        {
            using (var context = new OficinaContext())
            {
                List<SelectListItem> listaOficina = context.Oficinas.AsNoTracking()
                    .OrderBy(n => n.NombreOficina)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDOficina.ToString(),
                            Text = n.NombreOficina
                        }).ToList();
                SelectListItem oficinatip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona una Oficina ---"
                };
                listaOficina.Insert(0, oficinatip);
                return new SelectList(listaOficina, "Value", "Text");
            }
        }
    }

}
