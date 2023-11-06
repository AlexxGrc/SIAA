using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SIAAPI.ClasesProduccion
{
    public class ClsCambioPlaneacion
    {
        [DisplayName("Cotizacion")]
        public int IDCotizacion { get; set; }
        [DisplayName("Maquina en prensa")]
        public int IDMaquinaPrensa { get; set; }

        [DisplayName("Maquina en Embobinado")]
        public int IDMaquinaEmbobinado { get; set; }

        [DisplayName("Suaje 1")]
        public int IDSuaje1 { get; set; }
        [DisplayName("Suaje 2")]
        public int IDSuaje2 { get; set; }
        [DisplayName("Cyrel")]
        public string cyrel { get; set; }
 
    }
}