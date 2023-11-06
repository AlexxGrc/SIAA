using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SIAAPI.ViewModels.Comercial
{
    public class ReporteAlmacen
    {
        [Display(Name = "Almacén")]
        public int IdAlmacen { get; set; }

        [Display(Name = "Nota")]
        public string Nota { get; set; }

    
    }
}