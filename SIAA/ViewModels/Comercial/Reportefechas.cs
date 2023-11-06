using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Comercial
{
    public class Reportefechas
    {
        [DisplayFormat(ApplyFormatInEditMode = true,
                  DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [Display(Name = "Fecha de inicio")]
        public DateTime Fechainicio { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
               DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [Display(Name = "Fecha final")]
        public DateTime Fechafinal { get; set; }

        [Display(Name = "Nota al pie del reporte")]
        public string Nota { get; set; }
    }
}