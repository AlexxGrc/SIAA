using SIAAPI.Models.Administracion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Comercial
{
    [Table("Cobro")]
    public class Cobro
    {
        [Key]
        public int IDCobro { get; set; }
        [DisplayName("Cliente")]
        public int IDCliente { get; set; }
        public virtual Clientes Clientes { get; set; }
        [DisplayName("Calle")]
        [Required]
        [StringLength(100)]
        public string CalleCobro { get; set; }
        [DisplayName("No. Exterior")]
        [Required]
        public string NumExtCobro { get; set; }
        [DisplayName("No. Interior")]
    
        public string NumIntCobro { get; set; }
        [DisplayName("Colonia")]
        [Required]
        [StringLength(100)]
        public string ColoniaCobro { get; set; }
        [DisplayName("Municipio")]
        [Required]
        [StringLength(100)]
        public string MunicipioCobro { get; set; }
        [DisplayName("C.P.")]
        [Required]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "El campo C.P. debe contener 5 dígitos")]
        public string CPCobro { get; set; }
        [DisplayName("Estado")]
        [Required]
        public int IDEstado { get; set; }
        public virtual Estados Estados { get; set; }

        [DisplayName("Observación")]
        [StringLength(250)]
        public string Observacion { get; set; }



    }
    public class CobroContext : DbContext
    {
        public CobroContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Cobro> Cobros { get; set; }
        public DbSet<Estados> Estados { get; set; }
        public DbSet<Clientes> Clientess { get; set; }
    }
}