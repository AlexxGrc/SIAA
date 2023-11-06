using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Comercial
{
    public class EFecha
    {
        private DateTime? fecha = DateTime.Now;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime fechaini { get { return fecha ?? DateTime.Now; } set { fecha = value; } }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime fechafin { get; set; }
    }

    public class EFechaval
    {
        private DateTime? fecha = DateTime.Now;
        public string IDProveedor { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayName("Fecha Inicial")]
        public DateTime fechaini { get { return fecha ?? DateTime.Now; } set { fecha = value; } }
        [DisplayName("Fecha Final")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime fechafin { get; set; }
    }

    public class EFechaCorte
    {
        private DateTime? fecha = DateTime.Now;
        public string IDProveedor { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayName("Fecha Corte")]
        public DateTime fechaini { get { return fecha ?? DateTime.Now; } set { fecha = value; } }
    }
}