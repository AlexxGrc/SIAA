using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Cfdi
{
    public class ViewAnticipo
    {
        [Key]
        public int IDComplemento { get; set; }

        [Required]
        [Display(Name = "Cliente")]
        public int IDCliente { get; set; }


        [Required]
        [Display(Name = "Forma de Pago")]
        public int IDFormaPago { get; set; }

        [Required]
        [Display(Name = "Uso del Cfdi")]
        public int IDUsoCfdi { get; set; }



        [Required]
        [Display(Name = "Serie")]
        public int serie { get; set; }

        public double Anticipo { get; set; }
        public int IDMoneda { get; set; }
    }
}