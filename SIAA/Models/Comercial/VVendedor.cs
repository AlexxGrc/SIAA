using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace SIAAPI.Models.Comercial
{
    [Table("VVendedor")]
    public class VVendedor
    {
       [Key]
        public int IDVendedor { get; set; }

        
        [DisplayName("RFC")]
        [StringLength(15)]
        public string RFC { get; set; }

        [DisplayName("Nombre")]
        [StringLength(150)]
        public string Nombre { get; set; }

        [DisplayName("Cuota")]
        public decimal CuotaVendedor { get; set; }

        [DisplayName("Tipo de contrato")]
        [StringLength(100)]
        public string Contrato { get; set; }

        [DisplayName("Autorizado a cotizar")]
        public bool AutorizadoACotizar { get; set; }

        [DisplayName("Activo")]
        public bool Activo { get; set; }
       
        [DisplayName("Porcentaje de comisión")]
        public bool Porcentajecomision { get; set; }

        
        [DisplayName("Tipo de vendedor")]
        [StringLength(100)]
        public string DescripcionVendedor { get; set; }

        [DisplayName("Periocidad de Pago")]
        [StringLength(50)]
        public string PeriocidadDePago { get; set; }

        [DisplayName("Oficina")]
        [StringLength(50)]
        public string NombreOficina { get; set; }

        [DisplayName("E-mail")]
        [StringLength(100)]
        public string Mail { get; set; }


        [DisplayName("Telefono")]
        [StringLength(15)]
        public string Telefono { get; set; }

        [DisplayName("Perfil")]
        [StringLength(50)]
        public string Perfil { get; set; }

         [DisplayName("Foto")]
        [Column(TypeName = "image")]
        public byte[] Photo { get; set; }

        [DisplayName("Notas")]
        [StringLength(250)]
        public string Notas { get; set; }
    }

      public class VVendedorContext : DbContext
   {
       public VVendedorContext() : base("name=DefaultConnection")
       {

       }
       public DbSet<VVendedor> VVendedores { get; set; }
   }
}
