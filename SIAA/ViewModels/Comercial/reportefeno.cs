using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Comercial
{
    public class Reportefeno
    {
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [Display(Name = "Fecha de inicio")]
        public DateTime Fechainicio { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true,
         DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [Display(Name = "Fecha final")]
        public DateTime Fechafinal { get; set; }

        [Display(Name = "Cliente")]
        public int IDCliente { get; set; }
        [Display(Name = "ID Vendedor")]
        public int IDVendedor { get; set; }

        [Display(Name = "Proveedor")]
        public int IDProveedor { get; set; }

        [Display(Name = "Nota al pie del reporte")]
        public string Nota { get; set; }

        [Display(Name = "ID Moneda")]
        public int IDMoneda { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Display(Name = "ID Pago")]
        public int IDPago { get; set; }
        [Display(Name = "Tipo de cambio")]
        public decimal TC { get; set; }


        [Display(Name = "Pagada")]
        public string Pagada { get; set; }



        [Display(Name = "Modelo")]
        public int IDModeloProduccion { get; set; }


        [Display(Name = "Familia")]
        public int IDFamilia { get; set; }


        [Display(Name = "Proceso")]
        public int IDProceso { get; set; }

        [Display(Name = "Tipo de Articulo")]
        public int IDTipoArticulo
        {
            get; set;}


        [Display(Name = "Formula")]
        public string Formula { get; set; }

        [Display(Name = "ClaveMoneda")]
        public string ClaveMoneda { get; set; }
        [Display(Name = "Clave FormaPago")]
        public string ClaveFormaPago { get; set; }

        [Display(Name = "Oficina")]
        public int IDOfi { get; set; }

    }
}