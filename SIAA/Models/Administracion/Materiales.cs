using SIAAPI.Models;
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
    [Table("Materiales")]
    public class Materiales
    {
        [Key]
        public int ID { get; set; }
        [DisplayName("Clave")]
        [StringLength(255)]
        public string Clave{ get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Familia")]
        public string Fam { get; set; }
        [DisplayName("Ancho")]
        public int Ancho { get; set; }
        [DisplayName("Largo")]
        public int Largo { get; set; }
        [DisplayName("Adhesivo")]
        public string Adhesivo { get; set; }
        [DisplayName("Moneda")]
        public string Moneda { get; set; }
        [DisplayName("Precio")]
        public decimal Precio { get; set; }
        [DisplayName("Master")]
        public bool Completo { get; set; }
        [DisplayName("Calibre")]
        public decimal Calibre { get; set; }
        [DisplayName("Solicitar precio")]
        public bool Solicitarprecio { get; set; }
        [DisplayName("Calibre Especial")]
        public decimal CalibreEsp { get; set; }
        [DisplayName("Tcompra")]
        public string Tcompra { get; set; }
        [DisplayName("Fecha verificación precio")]
        public DateTime PrecioAct { get; set; }
        [DisplayName("Proveedor")]
        public string Proveedor { get; set; }
        [DisplayName("Respaldo")]
        public string Respaldo { get; set; }
        [DisplayName("Gramaje")]
        public string Gramaje { get; set; }
        [DisplayName("Clave Etiqueta")]
        public string ClaveEt { get; set; }
        [DisplayName("Plazo")]
        public int Plazo { get; set; }
        [DisplayName("Obsoleto")]
        public bool Obsoleto { get; set; }
    }

    public class MaterialesContext : DbContext
    {
        public MaterialesContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Materiales> Materiales { get; set; }
    }

    public class MaterialesRepository
    {
        public IEnumerable<SelectListItem> GetMaterialesbyDescripcion()
        {
            using (var context = new MaterialesContext())
            {
                List<SelectListItem> ma = context.Materiales.AsNoTracking().Include(p => p.ID)
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.ID.ToString(),
                            Text =  n.Descripcion
                        }).ToList();
                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona un material ---"
                };
                ma.Insert(0, descripciontip);
                return new SelectList(ma, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetMaterialesbyClave()
        {
            using (var context = new MaterialesContext())
            {
                List<SelectListItem> ma = context.Materiales.AsNoTracking().Include(p => p.ID)
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.Descripcion,
                            Text = n.Clave + "    |   "+ n.Descripcion
                        }).ToList();
                var descripciontip = new SelectListItem()
                {
                    Value = "Todas",
                    Text = "Todas"
                };
                //listaPeriocidadPago.Insert(0, descripciontip);
                return new SelectList(ma, "Value", "Text");
            }
        }






    }




    }