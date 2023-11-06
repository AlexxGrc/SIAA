using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Comercial
{
    public class VMFacDigital
    {
      
       public int Idprefacturafactura { get; set; }

        [Required]
        [Display(Name = "Serie")]
        public int serie { get; set; }


    }
}