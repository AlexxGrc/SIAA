using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace SIAAPI.Models.Comercial
{
    //Nombre de la tabla
    [Table("ContactosProv")]
    public class ContactosProv
    {
        //Primary Key IDProveedor
        [Key]
        public int IDContactoProv { get; set; }
        
        
        public int IDProveedor { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        //Nombre: Nombre de contacto
        [DisplayName("Nombre")]
        [StringLength(150)]
        [Required]
        public string Nombre { get; set; }

        //Email: Email de contacto
        [DisplayName("Correo")]
        [StringLength(100)]
     
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
    
        public string Observacion { get; set; }

    }
    public class ContactosProvContext : DbContext
    {
        public ContactosProvContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<ContactosProv> ContactosProvs { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }

    }
}