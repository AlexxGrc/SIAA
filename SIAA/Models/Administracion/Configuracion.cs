using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Administracion
{
    public class Configuracion
    {
        

        public string SIAAPI_com_multifacturas_pac1_Timbrado_Remoto_de_INI { get; set; }
        public string CorreofirmaFactura { get; set; }
        public string TimbrarPrueba { get; set; }
        public string Nombredelaaplicacion { get; set; }
        public string ValorIVA { get; set; }
        public int IDProveedorDesconocido { get; set; }
        public decimal Rango1gain { get; set; }
        public decimal Rango2gain { get; set; }
        public decimal Rango3gain { get; set; }
        public decimal Rango4gain { get; set; }
        public decimal CostoM2default { get; set; }
        public decimal costodeempaque { get; set; }
        public string paginaoficial { get; set; }
        public string Eslogan { get; set; }
        public string ColorReporte { get; set; }
        public decimal TCcotizador { get; set; }
        public string ColorFuenteEncabezado { get; set; }
        public string AtributoGapje { get; set; }
        public string AtributoGapAvance { get; set; }
        public string AtributoCavidadesEje { get; set; }
        public string AtributoEje { get; set; }
        public string AtributoAvance { get; set; }
        public string AtributoAnchoCinta { get; set; }
        public string AtributoAnchoCintaImpresa { get; set; }
        public decimal factorcostoindirecto { get; set; }
        public decimal ValorDienteMacizo { get; set; }
        public decimal ValorDienteFlex { get; set; }
        public decimal ValorPulMacizo { get; set; }
        public decimal ValorPulFlexible { get; set; }
        public int ValorMtsXkg { get; set; }
  
    }
}