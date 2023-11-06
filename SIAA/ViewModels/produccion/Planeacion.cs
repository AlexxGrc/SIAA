using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.produccion
{
 
public class ArticuloPlan
    {
        public string IDarticulo { get; set; }
        public string IDCaracteristica { get; set; }
        public string Formuladerelacion { get; set; }
        public string Observacion { get; set; }
        public string codigodebarras { get; set; }
    }

    public class Elemento
    {
        public string elemento { get; set; }
        public List<ArticuloPlan> articulos { get; set; }
    }

    public class ProcesoPla
    {
        public string proceso { get; set; }
        public List<Elemento> elementos { get; set; }
    }

    public class RangodeCosto
    {
        public int Rangoinf { get; set; }
        public int RangoSup { get; set; }
        public int Costo { get; set; }
    }

    public class Planeacion
    {
        public string IDArticulo { get; set; }
        public string IDCaracteristica { get; set; }
        public string IDCliente { get; set; }
        public string IDProspecto { get; set; }
        public string Modelodeproduccion { get; set; }
        public List<ProcesoPla> procesos { get; set; }
        public List<RangodeCosto> RangodeCostos { get; set; }
    }
}