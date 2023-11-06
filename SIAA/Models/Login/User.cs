using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SIAAPI.Models.Login
{
    [Table("User")]
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [DisplayName("Usuario")]
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        [DisplayName("Correo")]
        [Required]
        [StringLength(200)]
        public string EmailID { get; set; }

        [DisplayName("Contraseña")]
        [Required]
        [DataType(DataType.Password)]
        [StringLength(50)]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [StringLength(50)]
        [Display(Name = "Confirmar Contraseña")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "La contraseña y su confirmacion no son iguales")]
        public virtual string ConfirmPassword { get; set; }
        
        [StringLength(10)]
        public string Estado { get; set; }
        
        public virtual ICollection<UserRole> UserRole { get; set; }

    }
    
    public class UserContext : DbContext
    {
        public UserContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<UserContext>(null);
        }
        public DbSet<User> Users { get; set; }
    }

}

