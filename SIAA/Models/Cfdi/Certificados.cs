using SIAAPI.Models.Comercial;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System;

namespace SIAAPI.Models.Cfdi
{
    public class Certificados
    {
        [Key]
        public int IDCertificado { get; set; }
        [Required]
        public string PassCertificado { get; set; }
        [Required]
        public string Nombredelcertificado { get; set; }
        [Required]
        public string Nombredelkey { get; set; }
        [Required]
        public string UsuarioMultifacturas { get; set;}
        [Required]
        public string PassMultifacturas { get; set; }

        [Required(ErrorMessage = "Ingrese fecha de vencimiento")]
        [Display(Name = "Fecha de vencimiento:")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
       //[RegularExpression("(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|50)\\d\\d", ErrorMessage = "Fecha Invalida")]
        public System.DateTime FechaVigenciaCertificado { get; set; }

       
    }

    public class CertificadosContext : DbContext
    {
        public CertificadosContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Certificados> certificados { get; set; }
    }
}