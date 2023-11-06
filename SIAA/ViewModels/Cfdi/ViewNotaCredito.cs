using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Cfdi
{
    public class ViewNotaCredito
    {
        [Key]
        public int IDNotadeCredito { get; set; }

        [Required]
        [Display(Name = "Cliente")]
        public int IDCliente { get; set; }


        [Required]
        [Display(Name = "Tipo de Comprobante")]
        public string IDTipoComprobante { get; set; }

        [Required]
        [Display(Name = "Forma de Pago")]
        public int IDFormaPago { get; set; }


        [Required]
        [Display(Name = "Tipo de Relación")]
        public string IDTipoRelacion { get; set; }

        [Required]
        [Display(Name = "Método de Pago")]
        public int IDMetodoPago { get; set; }


        [Required]
        [Display(Name = "Uso del CFDI")]
        public string IDUsoCFDI { get; set; }


        [Required]
        [Display(Name = "Serie")]
        public int serie { get; set; }

        [Required]
        public string uuid { get; set; }

        public string nombre { get; set; }

        public string rfc { get; set; }

        public int IDFactura { get; set; }
        [Required]
        [Range(0.01, Double.PositiveInfinity)]
        [Display(Name = "Cantidad total de la Nota de Credito ")]


        public decimal Monto { get; set; }

        public string Moneda { get; set; }
        [Display(Name = "Concepto de la nota de Crédito ")]
        public string Observacion { get; set; }

    }

}