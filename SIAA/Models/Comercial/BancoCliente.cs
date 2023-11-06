using SIAAPI.Models.Administracion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIAAPI.Models.Comercial
{
        [Table("BancoCliente")]
      public class BancoCliente
        {
            [Key]
            public int IDBancoCliente { get; set; }
            [DisplayName("Cliente")]
            public int IDCliente { get; set; }
            public virtual Clientes Clientes { get; set; }

            [DisplayName("Banco")]
            [Required]
            public int IDBanco { get; set; }
            public virtual c_Banco c_Banco { get; set; }

            [DisplayName("Cuenta Bancaria")]

        [StringLength(20)]
            public string CuentaBanco { get; set; }

        [DisplayName("Divisa")]
        [Required]
        public int IDMoneda { get; set; }
        public virtual c_Moneda c_Moneda { get; set; }

        public virtual ICollection<c_TipoCuota> c_TipoCuota { get; set; }

    }

    public class VBancoCliente
    {
        public int IDBancoCliente { get; set; }
      
        public string Cliente { get; set; }

       
        public string Banco { get; set; }

       
        public string CuentaBanco { get; set; }

      
        public string Moneda { get; set; }


    }

    public class BancoClienteContext : DbContext
        {
            public BancoClienteContext() : base("name=DefaultConnection")
            {

            }
            public DbSet<BancoCliente> BancoClientes { get; set; }

        public DbSet<c_Banco> c_Bancos { get; set; }

        public DbSet<Clientes> Clientess { get; set; }

        public DbSet<c_Moneda> c_Monedas{ get; set; }
    }
    }
