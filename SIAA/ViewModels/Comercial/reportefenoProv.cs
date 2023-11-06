using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SIAAPI.ViewModels.Comercial
{
    public class ReportefenoProv
    {
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [Display(Name = "Fecha de inicio")]
        public DateTime Fechainicio { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true,
         DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [Display(Name = "Fecha final")]
        public DateTime Fechafinal { get; set; }

        [Display(Name = "Proveedor")]
        public int IDProveedor { get; set; }

        [Display(Name = "Nota al pie del reporte")]
        public string Nota { get; set; }

        [Display(Name = "Fecha pago")]
        public DateTime FechaPago { get; set; }

        [Display(Name = "RFC")]
        public string RFC { get; set; }

        [Display(Name = "Empresa")]
        public string Empresa { get; set; }

        [Display(Name = "ClaveDivisa")]
        public string ClaveDivisa { get; set; }

        [Display(Name = "Monto")]
        public decimal Monto { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = " RFCBancoEmisor")]
        public string RFCBancoEmisor { get; set; }

        [Display(Name = "CuentaEmisor")]
        public string CuentaEmisor { get; set; }

        [Display(Name = "UUID")]
        public string UUID { get; set; }

        [Display(Name = "RutaXml")]
        public string RutaXML { get; set; }

        [Display(Name = "RutaPdf")]
        public string RutaPDF { get; set; }

        [Display(Name = "IDPago")]
        public string IDPago { get; set; }

        [Display(Name = "Serie")]
        public string SerieP { get; set; }

        [Display(Name = "Folio")]
        public int FolioP { get; set; }
    }
}