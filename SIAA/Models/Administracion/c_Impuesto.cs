namespace SIAAPI.Models.Administracion
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    public partial class c_Impuesto
    {

        [Key]
        public int IDImpuesto { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria")]
        [DisplayName("Código de Sat")]
        [StringLength(3)]
        public string ClaveImpuesto { get; set; }


        [Required(ErrorMessage = "La descripción es obligatoria")]
        [DisplayName("Descripción")]
        [StringLength(3)]
        public string Descripcion { get; set; }
        
        [DisplayName("Retención")]
        public bool Retencion { get; set; }
        
        [DisplayName("Traslado")]
        public bool Traslado { get; set; }

        [StringLength(10)]
        [DisplayName("Tipo")]
        public string Tipo { get; set; }
        
    }

       public class c_ImpuestoContext: DbContext
    {
        public c_ImpuestoContext() : base ("name=DefaultConnection")
            {

        }
        public DbSet<c_Impuesto> c_Impuestos { get; set;}
    }

    public class c_ImpuestoRepository
    {
       

        public IEnumerable<SelectListItem> GetDescripcion()
        {
            using (var context = new c_ImpuestoContext())
            {
                List<SelectListItem> lista = context.c_Impuestos.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDImpuesto.ToString(),
                            Text = n.Descripcion
                        }).ToList();
                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona el impuesto ---"
                };
                lista.Insert(0, descripciontip);
                return new SelectList(lista, "Value", "Text");
            }
        }
    }

    public class tipoimpuestoRepository
    {
       

        public IEnumerable<SelectListItem> GetTipos()
        {

            List<SelectListItem> lista = new List<SelectListItem>();

            SelectListItem elemento =new SelectListItem()

                        {
                            Value = "Local",
                            Text = "Local",
                        };
                        lista.Insert(0, elemento);
            SelectListItem elemento1 = new SelectListItem()
                            {
                                Value = "Federal",
                                Text = "Federal"
                        };
                        lista.Insert(0, elemento1);
            SelectListItem elemento2 = new SelectListItem()
                        {
                            Value = null,
                            Text = "Selecciona un tipo"
                        };
                        lista.Insert(0, elemento2);


            return new SelectList(lista, "Value", "Text");
            }
        }
    }
    


    