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
    [Table ("c_SubTipoRem")]
    public class c_SubTipoRem
    {
        [Key]
        public int IDTRemolque { get; set; }
        [DisplayName("Clave Remolque")]
        public string ClaveTipoRemolque { get; set; }
        [DisplayName("Remolque o semi")]
        public string RemoSemi { get; set; }
        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIVigencia { get; set; }
        [DisplayName("Fecha de Fin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FFVigencia { get; set; }
    }
    public class c_SubTipoRemContext : DbContext
    {
        public c_SubTipoRemContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_SubTipoRem> TipoRem{ get; set; }
    }

    public class TipoRemRepository
    {
        public IEnumerable<SelectListItem> GetTipoRem()
        {
            using (var context = new c_SubTipoRemContext())
            {
                List<SelectListItem> lista = context.TipoRem.AsNoTracking()
                    .OrderBy(n => n.RemoSemi)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDTRemolque.ToString(),
                            Text = n.ClaveTipoRemolque + "|" + n.RemoSemi
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un tipo de remolque ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetTipoRem(int id)
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            using (var context = new c_SubTipoRemContext())
            {
                List<c_SubTipoRem> datos = context.TipoRem.OrderBy(x => x.RemoSemi).ToList();
                foreach (c_SubTipoRem item in datos)
                {
                    if (item.IDTRemolque == id)
                    {
                        var elemento = new SelectListItem()
                        {
                            Value = item.IDTRemolque.ToString(),
                            Text = item.ClaveTipoRemolque + " " + item.RemoSemi

                        };
                        lista.Add(elemento);
                    }
                    else
                    {
                        var elemento = new SelectListItem()
                        {
                            Value = item.IDTRemolque.ToString(),
                            Text = item.ClaveTipoRemolque + " | " + item.RemoSemi

                        };
                        lista.Add(elemento);
                    }

                }
                return new SelectList(lista, "Value", "Text", id);
            }
        }
    }

}