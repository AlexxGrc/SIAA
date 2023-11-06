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
    [Table("ReferenciaPagoCliente")]
    public class ReferenciaPagoCliente
    {
        [Key]
        public int IDReferenciaPagoCliente { get; set; }
        [DisplayName("Cliente")]
        public int IDCliente { get; set; }
        public virtual Clientes Clientes { get; set; }
        [DisplayName("Lunes")]
        public bool DiaRevLu { get; set; }

        [DisplayName("Martes")]
        public bool DiaRevMa { get; set; }

        [DisplayName("Miércoles")]
        public bool DiaRevMi { get; set; }

        [DisplayName("Jueves")]
        public bool DiaRevJu { get; set; }

        [DisplayName("Viernes")]
        public bool DiaRevVi { get; set; }

        [DisplayName("Lunes")]
        public bool DiaPagLu { get; set; }

        [DisplayName("Martes")]
        public bool DiaPagMa { get; set; }

        [DisplayName("Miércoles")]
        public bool DiaPagMi { get; set; }

        [DisplayName("Jueves")]
        public bool DiaPagJu { get; set; }

        [DisplayName("Viernes")]
        public bool DiaPagVi { get; set; }

        [DisplayName("Moroso")]
        public bool Moroso { get; set; }
        [DisplayName("Límite de crédito")]
        [Required]

        public decimal Limitedecredito { get; set; }
        [DisplayName("Riesgo alcanzado")]
        [Required]
        public decimal RiesgoAlcanzado { get; set; }

        public string Observacion { get; set; }
        [DisplayName("Abierto")]
        public bool DiaRevAb { get; set; }

        [DisplayName("Abierto")]
        public bool DiaPagAb { get; set; }
    }

    public class ReferenciaPagoClienteContext : DbContext
    {
        public ReferenciaPagoClienteContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ReferenciaPagoClienteContext>(null);
        }
        public DbSet<ReferenciaPagoCliente> ReferenciaPagoClientes { get; set; }


        public System.Data.Entity.DbSet<SIAAPI.Models.Comercial.Clientes> Clientes { get; set; }


    }
}
