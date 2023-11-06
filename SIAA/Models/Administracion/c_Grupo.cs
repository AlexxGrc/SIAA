using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web.Mvc;

namespace SIAAPI.Models.Administracion
{
   

    public partial class c_Grupo
    {
        [Key]
        public int IDGrupo { get; set; }
        [DisplayName("Clave Grupo")]
        [StringLength(2)]
        [Required]
        public string ClaveGrupo { get; set; }
        [DisplayName("Descripción")]
        [Required]
        [StringLength(200)]
        public string Descripcion { get; set; }

        public virtual ICollection<Clientes> Clientes { get; set; }
    }
    public class c_GrupoContext : DbContext
    {
        public c_GrupoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_Grupo> c_Grupos { get; set; }
    }
    public class GruposRepository
    {
        public IEnumerable<SelectListItem> GetGrupos()
        {
            using (var context = new c_GrupoContext())
            {
                List<SelectListItem> lista = context.c_Grupos.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDGrupo.ToString(),
                            Text = n.ClaveGrupo+"|"+n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Grupo ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }
    }

    }
