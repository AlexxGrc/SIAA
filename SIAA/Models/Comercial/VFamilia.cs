namespace SIAAPI.Models.Comercial
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;

    [Table("VFamilia")]
    public partial class VFamilia
    {
        [Key]

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IDFamilia { get; set; }

        [Display(Name = "C�digo de la Familia")]
        [StringLength(10)]
        public string CCodFam { get; set; }
        [Display(Name = "Descripci�n")]

        [StringLength(100)]
        public string Descripcion { get; set; }

        [Display(Name = "C�digo SAT")]
        [StringLength(10)]
        public string ClaveSat { get; set; }

        [Display(Name = "Producto SAT")]
        [StringLength(255)]
        public string ProductoSat { get; set; }

        public int IDClaveSTCC { get; set; }
        [Display(Name = "C�digo STCC")]
        public string ClaveSTCC { get; set; }

        [Display(Name = "Producto STCC")]
        [StringLength(255)]
        public string ProductoSTCC { get; set; }
        [Display(Name = "Factor M�nimo de Ganancia")]
        public decimal FactorMinimoGanancia { get; set; }
        [Display(Name = "Tipo de Art�culo")]
        public string TipoArticulo { get; set; }

    }


    public class VFamiliaContext : DbContext
    {
        public VFamiliaContext() : base("name=DefaultConnection")
        {

        }
        public virtual DbSet<VFamilia> VFamilias { get; set; }
    }
}
