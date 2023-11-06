using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.ViewModels.Cfdi
{
    public class ViewComplento
    {
        [Key]
        public int IDComplemento { get; set; }

        [Required]
        [Display(Name = "Forma de Pago")]
        public int IDFormaPago { get; set; }
   

        [Required]
        [Display(Name = "Método de Pago")]
        public int IDMetodoPago { get; set; }


        [Required]
        [Display(Name = "Tipo de Relación")]
        public int IDTipoRelacion { get; set; }


        [Required]
        [Display(Name = "Uso del CFDI")]
        public int IDUsoCFDI { get; set; }


        [Required]
        [Display(Name = "Tipo Comprobante")]
        public int IDTipoComprobante { get; set; }

        [Required]
        [Display(Name = "Serie")]
        public int serie { get; set; }

        [Required]
        public string UUID { get; set; }

        
        public int idEncFac { get; set; }



    }
}