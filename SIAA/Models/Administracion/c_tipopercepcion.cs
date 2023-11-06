namespace SIAAPI.Models.Administracion
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class c_tipopercepcion
    {
        [Key]
        [Column("c_TipoPercepcion", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int c_TipoPercepcion1 { get; set; }

        [StringLength(100)]
        public string Descripcion { get; set; }

        [Key]
        [Column(Order = 1)]
        public int id { get; set; }
    }
}
