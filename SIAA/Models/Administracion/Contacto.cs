using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Administracion
{
    [Table("Contacto")]

    public class Contacto
    {
        [Key]
        public int IDContacto { get; set; }
        [Required(ErrorMessage = "Departamento Obligatorio")]
        [DisplayName("Departamento")]
        public string Departamento { get; set; }
        [DisplayName("Fecha")]
        public Nullable<DateTime> Fecha { get; set; }
        [DisplayName("Tipo de Usuario")]
        public string TipoUsuario { get; set; }
        [DisplayName("RFC")]
        public string RFC { get; set; }

        [Required(ErrorMessage = "Nombre Obligatorio")]
        [DisplayName("Nombre del Contacto")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "E-mail Obligatorio")]
        [DisplayName("E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Télefono Obligatorio")]
        [DisplayName("Télefono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Mensaje Obligatorio")]
        [DisplayName("Mensaje")]
        public string Mensaje { get; set; }
        [DisplayName("Atendido")]
        public Boolean Atendido { get; set; }
        [DisplayName("Por")]
        public string Por { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }
        [DisplayName("Fecha de Atención")]
        public Nullable<DateTime> FechaAtencion { get; set; }

    }


    public class ContactoContext : DbContext
    {
        public ContactoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Contacto> Contacto { get; set; }
    }

    [Table("VMensajes")]

    public class VMensajes
    {
        [Key]
        public int IDContacto { get; set; }
        [Required(ErrorMessage = "Departamento Obligatorio")]
        [DisplayName("Para Departamento")]
        public string Para { get; set; }
        [DisplayName("Fecha")]
        public Nullable<DateTime> Fecha { get; set; }
        [DisplayName("De Usuario")]
        public string DeUsuario { get; set; }
        [DisplayName("RFC")]
        public string RFC { get; set; }

        [Required(ErrorMessage = "Nombre Obligatorio")]
        [DisplayName("Nombre del Contacto")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "E-mail Obligatorio")]
        [DisplayName("E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Télefono Obligatorio")]
        [DisplayName("Télefono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Mensaje Obligatorio")]
        [DisplayName("Mensaje")]
        public string Mensaje { get; set; }
        [DisplayName("Atendido")]
        public Boolean Atendido { get; set; }


    }

    public class VMensajesContext : DbContext
    {
        public VMensajesContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VMensajes> VMensajes { get; set; }
    }


}