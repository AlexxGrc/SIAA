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

    [Table("Propietario")]
    public class Propietario
    {
        [Key]
        [DisplayName("ID Propietario")]
        public int IDPropietario { get; set; }
        [DisplayName("RFC Propietario")]
        public string RFCPropietario { get; set; }
        [DisplayName("Nombre Propietario")]
        public string NombrePropietario { get; set; }
        [DisplayName("No. Reg. tributario")]
        public string NumRegIdTribPropietario { get; set; }
        [DisplayName("Calle")]
        public string Calle { get; set; }
        [DisplayName("No. Ext")]
        public string NumExt { get; set; }
        [DisplayName("No. Int")]
        public string NumInt { get; set; }
        [DisplayName("Residencia Fiscal")]
        public string ResidenciaFiscal { get; set; }
        [DisplayName("ID Pais")]
        public int IDPais { get; set; }
        [DisplayName("Clave Pais")]
        public string c_Pais { get; set; }
        [DisplayName("ID Estado")]
        public int IDEstado { get; set; }
        [DisplayName("Clave Estado")]
        public string c_Estado { get; set; }
        [DisplayName("ID Municipio")]
        public int IDMunicipio { get; set; }
        [DisplayName("Clave Municipio")]
        public string c_Municipio { get; set; }
        [DisplayName("ID Localidad")]
        public int IDLocalidad { get; set; }
        [DisplayName("Clave Localidad")]
        public string c_Localidad { get; set; }
        [DisplayName("ID ")]
        public int IDColonia { get; set; }
        [DisplayName("Clave Colonia")]
        public string c_Colonia { get; set; }
        [DisplayName("CP")]
        public string CP { get; set; }
        [DisplayName("Referencia")]
        public string Referencia { get; set; }
        [DisplayName("Activo")]
        public Boolean Activo { get; set; }
    }
    public class PropietarioContext : DbContext
    {
        public PropietarioContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<Propietario> Propietario { get; set; }
    }




    public class PropietarioRepository
    {
        public IEnumerable<SelectListItem> GetPropietario()
        {
            using (var context = new PropietarioContext())
            {
                List<SelectListItem> lista = context.Propietario.AsNoTracking()
                    .OrderBy(n => n.NombrePropietario)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDPropietario.ToString(),
                            Text = n.NombrePropietario
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Propietario ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetPropietario(int id)
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            using (var context = new PropietarioContext())
            {
                List<Propietario> datos = context.Propietario.OrderBy(x => x.NombrePropietario).ToList();
                foreach (Propietario item in datos)
                {
                    if (item.IDPropietario == id)
                    {
                        var elemento = new SelectListItem()
                        {
                            Value = item.IDPropietario.ToString(),
                            Text = item.NombrePropietario

                        };
                        lista.Add(elemento);
                    }
                    else
                    {
                        var elemento = new SelectListItem()
                        {
                            Value = item.IDPropietario.ToString(),
                            Text = item.NombrePropietario

                        };
                        lista.Add(elemento);
                    }

                }
                return new SelectList(lista, "Value", "Text", id);
            }
        }
    }
}