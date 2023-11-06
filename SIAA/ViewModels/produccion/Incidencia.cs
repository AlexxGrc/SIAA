using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.produccion
{
    public class V_Incidencias
    {
        public int IDReportes { get; set; }
        public int IDBitacora { get; set; }
        public int IDOrden { get; set; }
        public int IDProceso { get; set; }
        public string Proceso { get; set; }
        public DateTime FechaInicioParo { get; set; }
        public DateTime FechaTerminacionParo { get; set; }
        public string TiempoFalla { get; set; }
        public string Falla { get; set; }
    }

}