using SIAAPI.Models.Cfdi;
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
    [Table("c_TipoCadenaPago")]
    public class c_TipoCadenaPago
    {
        [Key]
        public int IDTipoCadenaPago { get; set;}

        [Required(ErrorMessage = "La clave del SAT es obligatoria")]
        [DisplayName("Clave del SAT")]
        public string Clave { get; set; }

        [Required(ErrorMessage = "La descripciónn es obligatoria")]
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        public virtual ICollection<PagoFactura> PagoFactura { get; set; }
    }
    public class c_TipoCadenaPagoContext : DbContext
    {
        public c_TipoCadenaPagoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_TipoCadenaPago> c_TipoCadenaPagos { get; set; }
    }

    public class c_TipoCadenaPagoRepository
    {
        public c_TipoCadenaPagoContext db = new c_TipoCadenaPagoContext();
        public IEnumerable<SelectListItem> GetTipoCadenaPago()
        {

            string cadenasql = "select IDTipoCadenaPago,Clave, Descripcion from [dbo].[c_TipoCadenaPago] order by Descripcion";
            List<SelectListItem> lista = db.Database.SqlQuery<c_TipoCadenaPago>(cadenasql).ToList()
                 .OrderBy(n => n.Descripcion)
                    .Select(n =>
                    new SelectListItem
                    {
                        Value = n.IDTipoCadenaPago.ToString(),
                        Text = n.Clave + " | " + n.Descripcion
                    }).ToList();
            var countrytip = new SelectListItem()
            {
                Value = null,
                Text = "--- Selecciona un tipo de cadena de Pago---"
            };
            lista.Insert(0, countrytip);
            return new SelectList(lista, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetTipoCadenaPagoE()
        {

            string cadenasql = "select IDTipoCadenaPago,Clave, Descripcion from [dbo].[c_TipoCadenaPago] order by Descripcion";
            List<SelectListItem> lista = db.Database.SqlQuery<c_TipoCadenaPago>(cadenasql).ToList()
                 .OrderBy(n => n.Descripcion)
                    .Select(n =>
                    new SelectListItem
                    {
                        Value = n.Clave,
                        Text = n.Clave + " | " + n.Descripcion
                    }).ToList();
            var countrytip = new SelectListItem()
            {
                Value = null,
                Text = "--- Selecciona un tipo de cadena de Pago---"
            };
            lista.Insert(0, countrytip);
            return new SelectList(lista, "Value", "Text");
        }
    }
}