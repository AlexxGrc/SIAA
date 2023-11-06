using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Cfdi
{
    public class AddendaEnvases
    {
        [Key]
        public int ID { get; set; }
        
        public string OrdenCompra { get; set; }
        
        public string Albaran { get; set; }    
    }
}