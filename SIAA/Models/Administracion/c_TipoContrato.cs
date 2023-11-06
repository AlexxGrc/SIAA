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

    public partial class c_TipoContrato
    {
        [Key]
        public int IDTipoContrato { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria y de alfanumérica")]
        [DisplayName("Código del Sat")]
        public string ClaveTipoContrato { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria y admite máximo 100 caracteres")]
        [DisplayName("Descripción")]
        [StringLength(100)]
        public string Descripcion { get; set; }
        public virtual ICollection<Vendedor> Vendedor { get; set; }

    }
   
      public class c_TipoContratoContext : DbContext
    {
        public c_TipoContratoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_TipoContrato> c_TipoContratos { get; set; }
    }


    public class c_TipoContratoRepository
    {
    

        public IEnumerable<SelectListItem> GetDescripcion()
        {
            using (var context = new c_TipoContratoContext())
            {
                List<SelectListItem> listaTipoContrato = context.c_TipoContratos.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDTipoContrato.ToString(),
                            Text = n.Descripcion
                        }).ToList();
                SelectListItem descripcioncontratotip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona el Tipo ---"
                };
                listaTipoContrato.Insert(0, descripcioncontratotip);
                return new SelectList(listaTipoContrato, "Value", "Text");
            }
        }
    }

    }
