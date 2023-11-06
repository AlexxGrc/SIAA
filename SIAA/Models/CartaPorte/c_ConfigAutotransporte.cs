using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Models.CartaPorte
{
    [Table("c_ConfigAutotransporte")]
    public class c_ConfigAutotransporte
    {
        [Key]
        public int IDConfAutoT { get; set; }
        [DisplayName("Clave")]
        public string ClaveNom { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Numero de ejes")]
        public string NoEjes { get; set; }
        [DisplayName("Numero de llantas")]
        public string NoLlantas { get; set; }
        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha_In { get; set; }
        [DisplayName("Fecha de Fin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Fecha_Fin { get; set; }

    }
    public class c_ConfigAutotransporteDBContext : DbContext
    {
        public c_ConfigAutotransporteDBContext() : base("name = DefaultConnection")
        {

        }
        public DbSet<c_ConfigAutotransporte> ConfigAutotransporte { get; set; }
    }

    public class ConfigAutotransporteRepository
    {
        public IEnumerable<SelectListItem> GetConfigAutotransporte()
        {
            using (var context = new c_ConfigAutotransporteDBContext())
            {
                List<SelectListItem> lista = context.ConfigAutotransporte.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDConfAutoT.ToString(),
                            Text = n.ClaveNom + "|" + n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un tipo de vehículo ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetConfigAutotransporte(int id)
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            using (var context = new c_ConfigAutotransporteDBContext())
            {
                List<c_ConfigAutotransporte> datos = context.ConfigAutotransporte.OrderBy(x => x.Descripcion).ToList();
                foreach (c_ConfigAutotransporte item in datos)
                {
                    if (item.IDConfAutoT == id)
                    {
                        var elemento = new SelectListItem()
                        {
                            Value = item.IDConfAutoT.ToString(),
                            Text = item.ClaveNom + " " + item.Descripcion

                        };
                        lista.Add(elemento);
                    }
                    else
                    {
                        var elemento = new SelectListItem()
                        {
                            Value = item.IDConfAutoT.ToString(),
                            Text = item.ClaveNom + " | " + item.Descripcion

                        };
                        lista.Add(elemento);
                    }

                }
                return new SelectList(lista, "Value", "Text", id);
            }
        }
    }

}