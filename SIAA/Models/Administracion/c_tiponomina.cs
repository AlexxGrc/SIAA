namespace SIAAPI.Models.Administracion
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class c_tiponomina
    {
        [Key]
        [Column("c_TipoNomina", Order = 0)]
        [StringLength(1)]
        public string c_TipoNomina1 { get; set; }

        [StringLength(50)]
        public string Descripcion { get; set; }

        [Key]
        [Column(Order = 1)]
        public int id { get; set; }
    }
}
