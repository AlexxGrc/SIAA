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
    [Table("Arrendatario")]
    public class Arrendatario
    {
        [Key]
        [DisplayName("ID Arrendatario")]
        public int IDArrendatario { get; set; }
        public string RFCArrendatario { get; set; }
        public string NombreArrendatario { get; set; }
        public string NumRegIdTribArrendatario { get; set; }
        public string Calle { get; set; }
        public string NumExt { get; set; }
        public string NumInt { get; set; }
        public int IDPais { get; set; }
        public string c_Pais { get; set; }
        public int IDEstado { get; set; }
        public string c_Estado { get; set; }
        public int IDMunicipio { get; set; }
        public string c_Municipio { get; set; }
        public int IDLocalidad { get; set; }
        public string c_Localidad { get; set; }
        public int IDColonia { get; set; }
        public string c_Colonia { get; set; }
        public string CP { get; set; }
        public string Referencia { get; set; }
        public Boolean Activo { get; set; }
    }
    public class ArrendatarioContext : DbContext
    {
        public ArrendatarioContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<Arrendatario> Arrendatario { get; set; }
    }

    public class ArrendatarioRepository
    {
        public IEnumerable<SelectListItem> GetArrendatario()
        {
            using (var context = new ArrendatarioContext())
            {
                List<SelectListItem> lista = context.Arrendatario.AsNoTracking()
                    .OrderBy(n => n.NombreArrendatario)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDArrendatario.ToString(),
                            Text = n.NombreArrendatario
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Arrendatario ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetArrendatario(int id)
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            using (var context = new ArrendatarioContext())
            {
                List<Arrendatario> datos = context.Arrendatario.OrderBy(x => x.NombreArrendatario).ToList();
                foreach (Arrendatario item in datos)
                {
                    if (item.IDArrendatario == id)
                    {
                        var elemento = new SelectListItem()
                        {
                            Value = item.IDArrendatario.ToString(),
                            Text = item.NombreArrendatario

                        };
                        lista.Add(elemento);
                    }
                    else
                    {
                        var elemento = new SelectListItem()
                        {
                            Value = item.IDArrendatario.ToString(),
                            Text = item.NombreArrendatario

                        };
                        lista.Add(elemento);
                    }

                }
                return new SelectList(lista, "Value", "Text", id);
            }
        }
    }

}