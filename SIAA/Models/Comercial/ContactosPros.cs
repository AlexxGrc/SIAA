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
    [Table("ContactosPros")]
    public class ContactosPros
    {
        //Primary Key IDProspecto
        [Key]
        public int IDProspecto { get; set; }
        [DisplayName("ClienteP")]
        public int IDClienteP { get; set; }
        public virtual ClientesP ClientesP { get; set; }

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
        [DisplayName("Telefono")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "El campo Telefóno debe contener un mínimo de 10 dígitos")]
        [Required]
        public string Telefono { get; set; }

        //Puesto: Puesto de contacto
        [DisplayName("Puesto")]
        [StringLength(50)]
        [Required]
        public string Puesto { get; set; }

        //Observacion: Observacion de contacto
        [DisplayName("Observacion")]
        [StringLength(150)]
        [Required]
        public string Observacion { get; set; }
    }
    public class ContactosProsContext : DbContext
    {
        public ContactosProsContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<ContactosPros> ContactosPross { get; set; }
        
    }
}