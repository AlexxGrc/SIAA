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


    [Table("Origen")]
    public class Origen
    {
        [Key]
        public int IDOrigen { get; set; }
        [DisplayName("Empresa")]
        public int IDEmpresa { get; set; }
        [DisplayName("RFC")]
        public string RFCRemitente { get; set; }
        [DisplayName("Nombre")]
        public string NombreRemitente { get; set; }
        [DisplayName("Calle")]
        public string Calle { get; set; }
        [DisplayName("Num. Exterior")]
        public string NumExt { get; set; }
        [DisplayName("Num. Interior")]
        public string NumInt { get; set; }
        [DisplayName("Pais")]
        public int IDPais { get; set; }
        [DisplayName("Clave Pais")]
        public string c_Pais { get; set; }
        [DisplayName("Estado")]
        public int IDEstado { get; set; }
        [DisplayName("Clave Estado")]
        public string c_Estado { get; set; }
        [DisplayName("Municipio")]
        public int IDMunicipio { get; set; }
        [DisplayName("Clave Estado")]
        public string c_Municipio { get; set; }
        [DisplayName("Localidad")]
        public int IDLocalidad { get; set; }
        [DisplayName("Clave Localidad")]
        public string c_Localidad { get; set; }
        [DisplayName("Colonia")]
        public int IDColonia { get; set; }
        [DisplayName("Clave Colonia")]
        public string c_Colonia { get; set; }
        [DisplayName("Codigo Postal")]
        [StringLength(5)]
        public string CP { get; set; }
        [DisplayName("Referencia")]
        public string Referencia { get; set; }
        [DisplayName("Activo")]
        public Boolean Activo { get; set; }
    }


    public class OrigenContext : DbContext
    {
        public OrigenContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Origen> Origen { get; set; }
        public DbSet<VOrigen> VOrigen { get; set; }
    }

    //[Table("VOrigen")]
    //public class VOrigen
    //{
    //    [Key]
    //    public int IDDomicilioOrigen { get; set; }
    //    public string NumExt { get; set; }
    //    public string Calle { get; set; }
    //    public string Estado { get; set; }
    //    public string Pais { get; set; }
    //    public string Localidad { get; set; }
    //    public string Municipio { get; set; }
    //}



    [Table("VOrigen")]
    public class VOrigen
    {
        [Key]
        public int IDDomicilioOrigen { get; set; }
        public string NumExt { get; set; }
        public string Calle { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string Localidad { get; set; }
        public string Municipio { get; set; }
    }



    public class ReporitoryOrigen
    {
        public IEnumerable<SelectListItem> GetDomicilioOrigen()
        {
            List<SelectListItem> lista;

            using (var context = new OrigenContext())
            {
                string cadenasql = "Select * from [dbo].[VOrigen]";

                lista = context.Database.SqlQuery<VOrigen>(cadenasql).ToList().OrderBy(n => n.IDDomicilioOrigen).Select(n => new SelectListItem
                {
                    Value = n.IDDomicilioOrigen.ToString(),
                    Text = n.Estado + " " + n.Municipio + " " + n.NumExt
                }).ToList();
                var countrytip = new SelectListItem()
                {

                    Value = null,
                    Text = "Seleccione un domicilio de origen"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }

    }

}
