using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Comercial
{
    public class ClsDatoEntero
    {
        [Key]
       public int Dato { get; set; }
    }

    public class ClsDatoDecimal
    {
        [Key]
        public decimal Dato { get; set; }
    }
    public class ClsDatoString
    {
        [Key]
        public string Dato { get; set; }
    }

    public class ClsDatoBool
    {
        [Key]
        public bool Dato { get; set; }
    }

}