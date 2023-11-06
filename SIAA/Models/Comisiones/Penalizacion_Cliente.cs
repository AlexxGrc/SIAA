using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using SIAAPI.Models.Comercial;


namespace SIAAPI.Models.Comisiones
{
    [Table("PenalizacionCliente")]
    public class Penalizacion_Cliente
    {
        [Key]

        public int IdPenalizacionCliente { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("IdCliente")]
        public int IdCliente { get; set; }
        public virtual Clientes cliente { get; set; }

        [DisplayName("Cliente")]
        public string Cliente { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Motivo")]
        public string Motivo { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Monto")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Aplicado")]
        public decimal Aplicado { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Resta")]
        public decimal Resta { get; set; }

        [DisplayName("Fecha")]
        public System.DateTime fecha { get; set; }

    }



    public class Penalizacion_ClienteContext : DbContext
    {
        public Penalizacion_ClienteContext() : base("name=DefaultConnection")
        {

        }

        public DbSet<Penalizacion_Cliente> PClientes { get; set; }
        public DbSet<Clientes> Clientes { get; set; }
        //public DbSet<Clientes> ClientesP { get; set; }
    }

}

