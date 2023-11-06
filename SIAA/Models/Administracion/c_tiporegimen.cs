namespace SIAAPI.Models.Administracion
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class c_tiporegimen
    {
        [Key]
        [Column("c_TipoRegimen", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int c_TipoRegimen1 { get; set; }

        [StringLength(100)]
        public string Descripcion { get; set; }

        [Key]
        [Column("int", Order = 1)]
        public int _int { get; set; }
    }
}
