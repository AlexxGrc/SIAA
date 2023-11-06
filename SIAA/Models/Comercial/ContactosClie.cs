using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Comercial
{

    //Nombre de la tabla
    [Table("ContactosClie")]
    public class ContactosClie
    {

        [Key]
        public int IDContactoClie { get; set; }
        [DisplayName("Cliente")]
        public int IDCliente { get; set; }
        public virtual Clientes Clientes { get; set; }
        //Nombre: Nombre de contacto
        [DisplayName("Nombre")]
        [StringLength(150)]
        [Required]
        public string Nombre { get; set; }

        //Email: Email de contacto
        [DisplayName("Correo")]
        [StringLength(100)]
        [Required]
        public string Email { get; set; }

        //Telefono: Telefono de contacto
        [DisplayName("Teléfono")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "El campo Telefóno debe contener un mínimo de 10 dígitos")]
        [Required]
        public string Telefono { get; set; }

        //Puesto: Puesto de contacto
        [DisplayName("Puesto")]
        [StringLength(50)]
        [Required]
        public string Puesto { get; set; }

        //Observacion: Observacion de contacto
        [DisplayName("Observación")]
        [StringLength(150)]
        [Required]
        public string Observacion { get; set; }
    }
    public class ContactosClieContext : DbContext
    {
        public ContactosClieContext() : base("name=DefaultConnection")
        {

        }

        public DbSet<ContactosClie> ContactosClies { get; set; }
    }

}