namespace SIAAPI.Models.Comercial
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;

    public partial class VBancos
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IDBancosProv { get; set; }

   
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IDProveedor { get; set; }

        [DisplayName("Banco")]
        [StringLength(30)]
        public string Nombre { get; set; }
        [DisplayName("No. Cuenta")]
        [StringLength(15)]
        public string Cuenta { get; set; }
        [DisplayName("No. Clabe")]
        [StringLength(20)]
        public string CuentaClabe { get; set; }
        [StringLength(3)]
        public string ClaveMoneda { get; set; }
        [StringLength(3)]
        public string ClaveBanco { get; set; }
        [DisplayName("Divisa")]
        [StringLength(100)]
        public string Descripcion { get; set; }
    }
    public class VBancosContext : DbContext
    {
        public VBancosContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VBancos> VBancoss { get; set; }

    }
}
