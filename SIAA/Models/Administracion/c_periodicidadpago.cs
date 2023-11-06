namespace SIAAPI.Models.Administracion
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class c_periodicidadpago
    {

        [Column("c_PeriodicidadPago")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int c_PeriodicidadPago1 { get; set; }

        [StringLength(50)]
        public string Descripcion { get; set; }

        [Key]
    
        public int id { get; set; }
    }
}
