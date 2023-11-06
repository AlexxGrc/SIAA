using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Models.Administracion
{
    [Table("Chofer")]
    public class Chofer
    {
        [Key]
        public int IDChofer { get; set; }

        [Required(ErrorMessage = "El nombre del Operador es obligatorio")]
        [DisplayName("Nombre del Operador ")]
        [StringLength(150)]
        public string Nombre { get; set; }

        public bool Activo { get; set; }
        [Required(ErrorMessage = "RFC del Operador")]
        [DisplayName("RFC")]
        [StringLength(13, MinimumLength = 12, ErrorMessage = "El campo RFC debe contener max 13 dígitos")]
        public string RFC { get; set; }
        [DisplayName("No. Licencia ")]
        public string NoLicencia { get; set; }
        public string Calle { get; set; }
        public string NumExt { get; set; }
        public string NumInt { get; set; }
        [DisplayName("País ")]
        public int IDPais { get; set; }
        [DisplayName("Estado ")]
        public int IDEstado { get; set; }
        [DisplayName("Municipio ")]
        public int IDMunicipio { get; set; }
        [DisplayName("Localidad ")]
        public int IDLocalidad { get; set; }
        [DisplayName("Colonia ")]
        public int IDColonia { get; set; }
        [DisplayName("C.P.")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "El campo C.P. debe contener 5 dígitos")]
        [Required]
        public string CP { get; set; }
        [DisplayName("Referencia ")]
        public string Referencia { get; set; }

        public virtual ICollection<EntregaRemision> entrega { get; set; }
    }

    public class ChoferRepository
    {
        public IEnumerable<SelectListItem> GetChofer()
        {
            using (var context = new ChoferesContext())
            {
                List<SelectListItem> lista = context.Choferes.AsNoTracking()
                    .OrderBy(n => n.Nombre)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDChofer.ToString(),
                            Text = n.Nombre
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Chofer---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }
        public IEnumerable<SelectListItem> GetChofer(int IDChofer)
        {
            using (var context = new ChoferesContext())
            {
                List<SelectListItem> lista = context.Choferes.Where(s => s.IDChofer == IDChofer)
                    .OrderBy(n => n.Nombre)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDChofer.ToString(),
                            Text = n.Nombre,
                            Selected = true
                        }).ToList();

                return new SelectList(lista, "Value", "Text");
            }
        }
    }

    public class ChoferesContext : DbContext
    {
        public ChoferesContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ChoferesContext>(null);
        }
        public DbSet<Chofer> Choferes { get; set; }
        public DbSet<VOperadores> VOperadores { get; set; }
    }

    [Table("VOperadores")]
    public class VOperadores
    {
        [Key]
        public int IDChofer { get; set; }
        public string Nombre { get; set; }

        public string RFC { get; set; }

        public string NoLicencia { get; set; }
        public string Calle { get; set; }
        public string NumExt { get; set; }
        public string NumInt { get; set; }

        public string c_Pais { get; set; }

        public string c_Estado { get; set; }

        public string c_Municipio { get; set; }

        public string c_Localidad { get; set; }

        public string c_Colonia { get; set; }

        public string CP { get; set; }

        public string Referencia { get; set; }

    }

}