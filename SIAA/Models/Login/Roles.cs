using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Login
{
    [Table("Roles")]
    public class Roles
    {
        [Key]
        public int RoleID { get; set; }
        [DisplayName("Rol")]
        [Required]
        public string ROleName { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
        
    }
    public class RolesContext : DbContext
    {
        public RolesContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<RolesContext>(null);
        }
        
        public DbSet<Roles> Roless { get; set; }
    }

}
