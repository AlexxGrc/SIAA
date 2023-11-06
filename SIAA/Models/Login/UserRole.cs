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
    [Table("UserRole")]
    public class UserRole
    {
        [Key]
        public int UserRolesID { get; set; }
        [Required]
        [DisplayName("Rol")]
        public int RoleID { get; set; }
       
        public virtual Roles Roles { get; set; }
        [Required]
        [DisplayName("Usuario")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
    }
    public class UserRoleContext : DbContext
    {
        public UserRoleContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<UserRoleContext>(null);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Roles> Roless { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}