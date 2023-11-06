using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIAAPI.Models.Produccion;

namespace SIAAPI.ClasesProduccion
{
    public class clsprioridad
    {

        public static void verificarconflicto()
        {
            new OrdenProduccionContext().Database.ExecuteSqlCommand("update OrdenProduccion set prioridad =0 where estadoOrden='Conflicto'");

        }


    }


   
}