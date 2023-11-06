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
    [Table("c_ClaveProdSTCC")]
    public class c_ClaveProdSTCC
    {
        [Key]
        public int IDClaveSTCC { get; set; }
        [DisplayName("Clave")]
        public string Clave { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Fecha de Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FIVigencia { get; set; }
        [DisplayName("Fecha de Terminación")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FFVigencia { get; set; }
    }
    public class c_ClaveProdSTCCContext : DbContext
    {
        public c_ClaveProdSTCCContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_ClaveProdSTCC> produc { get; set; }
    }


    public class ProductoSTCCRepository
    {
        public IEnumerable<SelectListItem> GetProductosSTCC()
        {
            using (var context = new c_ClaveProdSTCCContext())
            {
                List<SelectListItem> lista = context.produc.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDClaveSTCC.ToString(),
                            Text = n.Clave + "|" + n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Producto de la STCC---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }


    }
}