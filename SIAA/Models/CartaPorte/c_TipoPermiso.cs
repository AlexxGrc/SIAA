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
    [Table("c_TipoPermiso")]
    public class c_TipoPermiso
    {
        [Key]
        public int IDTipoPermiso { get; set; }
        [DisplayName("Clave")]
        public string Clave { get; set; }
        [DisplayName("Descripcion")]
        public string Descripcion { get; set; }
        [DisplayName("Clave Transporte")]
        public string ClaveT { get; set; }

        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIVigencia { get; set; }

        [DisplayName("Fecha de Terminación")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FFVigencia { get; set; }
    }
    public class c_TipoPermisoContext : DbContext
    {
        public c_TipoPermisoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_TipoPermiso> permiso { get; set; }
    }

    public class TipoPermisoRepository
    {
        public IEnumerable<SelectListItem> GetTipoPermiso()
        {
            using (var context = new c_TipoPermisoContext())
            {
                List<SelectListItem> lista = context.permiso.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDTipoPermiso.ToString(),
                            Text = n.Clave + "|" + n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un tipo de permiso---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetTipoPermiso(int IDTipoPermiso)
        {
            List<SelectListItem> lista= new List<SelectListItem>();
            
            using (var context = new c_TipoPermisoContext())
            {
                List<c_TipoPermiso> datos = context.permiso.OrderBy(x => x.Descripcion).ToList();
                foreach(c_TipoPermiso item in datos)
                {
                    if (item.IDTipoPermiso==IDTipoPermiso)
                    {
                        var elemento = new SelectListItem()
                        {
                            Value = item.IDTipoPermiso.ToString(),
                            Text = item.Clave + " | "+ item.Descripcion
                            
                        };
                        lista.Add(elemento);
                    }
                    else
                    {
                        var elemento = new SelectListItem()
                        {
                            Value = item.IDTipoPermiso.ToString(),
                            Text = item.Clave + " " + item.Descripcion
                           
                        };
                        lista.Add(elemento);
                    }
                  
                }             
                return new SelectList(lista, "Value", "Text", IDTipoPermiso);
            }
        }
    }

}