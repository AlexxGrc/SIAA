using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Comercial
{
    public class ReportefenoClie
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
        public Boolean Estado { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "IDPago")]
        public string IDPago { get; set; }

        [Display(Name = "Tipo de cambio")]
        //public int TC { get; set; }
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
        public int IDTipoArticulo { get; set; }

        [Display(Name = "Formula")]
        public string Formula { get; set; }

        [Display(Name = "ClaveMoneda")]
        public string ClaveMoneda { get; set; }

        [Display(Name = "Clave FormaPago")]
        public string ClaveFormaPago { get; set; }

        [Display(Name = "Oficina")]
        public int IDOfi { get; set; }

        [Display(Name = "Monto")]
        public decimal Monto { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(Name = "ClaveDivisa")]
        public string ClaveDivisa { get; set; }

        [Display(Name = " RFCBancoEmisor")]
        public string RFCBancoEmisor { get; set; }

        [Display(Name = "NoOperacion")]
        public Int64 NoOperacion { get; set; }

        [Display(Name = "CuentaEmisor")]
        public string CuentaEmisor { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [Display(Name = "Fecha pago")]
        public DateTime FechaPago { get; set; }
        public int IDOficina { get; set; }

    }
}