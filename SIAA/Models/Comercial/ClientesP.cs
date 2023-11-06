using SIAAPI.Models.Administracion;
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
    //Tabla Clientes
    [Table("ClientesP")]

    public class ClientesP
    {

        [Key]
        public int IDClienteP { get; set; }
        [DisplayName("Nombre")]
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }
        

        [DisplayName("Correo")]
  
        [StringLength(100)]
        public string Correo { get; set; }


      

        [DisplayName("Teléfono")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "El campo Telefóno debe contener un mínimo de 10 dígitos")]
       
        public string Telefono { get; set; }

       
        [DisplayName("Vendedor")]
        [Required]
        public int IDVendedor { get; set; }
        public virtual Vendedor Vendedor { get; set; }


        [DisplayName("User ID")]

      
        public int UserID { get; set; }

        public DateTime fechaAlta { get; set; }


       

    }




    public class ClientesPContext : DbContext
    {

        public ClientesPContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ClientesPContext>(null);
        }
        public DbSet<ClientesP> ClientesPs { get; set; }

        public DbSet<ContactosPros> ContactosPross { get; set; }
 
        public DbSet<Vendedor> Vendedores { get; set; }
 
    }

}
