namespace SIAAPI.Models.Administracion
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class c_origenrecurso
    {
        [Key]
        [Column("c_OrigenRecurso")]
        [StringLength(2)]
        public string c_OrigenRecurso1 { get; set; }

        [StringLength(30)]
        public string Descripcion { get; set; }

     
        public int _int { get; set; }
    }
}
