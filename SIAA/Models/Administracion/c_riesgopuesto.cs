namespace SIAAPI.Models.Administracion
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class c_riesgopuesto
    {
       
        [Column("c_RiesgoPuesto")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int c_RiesgoPuesto1 { get; set; }

        [StringLength(15)]
        public string Descripcion { get; set; }

        [Key]
   
        public int id { get; set; }
    }
}
