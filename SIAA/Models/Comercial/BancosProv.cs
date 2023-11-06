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
    //Nombre de la tabla
    [Table("BancosProv")]
    public class BancosProv
    {
        //Primary Key IDProspecto
        [Key]
        public int IDBancosProv { get; set; }

        //IDBanco: ID de banco
        [DisplayName("Banco")]
        [Required]
        public int IDBanco { get; set; }
        public virtual c_Banco Banco { get; set; }
        //Cuenta: Cuenta bancaria
        [DisplayName("Cuenta")]
        [StringLength(15)]
        [Required]
        public string Cuenta { get; set; }

        //CuentaClabe: Número Clabe (Clave Bancaria Estandarizada)
        [DisplayName("Número Clave")]
        [StringLength(20)]
 
        public string CuentaClabe { get; set; }

        //IDMoneda: ID de moneda (divisa)
        [DisplayName("Divisa")]
        [Required]
        public int IDMoneda { get; set; }
        public virtual c_Moneda Moneda { get; set; }
        //IDProveedor: ID de proveedor
        [DisplayName("Proveedor")]
        [Required]
        public int IDProveedor { get; set; }
        public virtual Proveedor Proveedor { get; set; }
    }
    public class BancosProvContext : DbContext
    {
        public BancosProvContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<BancosProv> BancosProvs { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Administracion.c_Banco> c_Banco { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Administracion.c_Moneda> c_Moneda { get; set; }
    }
}