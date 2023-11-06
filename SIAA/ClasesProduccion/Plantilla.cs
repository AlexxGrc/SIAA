using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace SIAAPI.ClasesProduccion
{
      //  [Serializable()]
        //[XmlRoot(ElementName = "ArticuloXML")]
            public class ArticuloXML
         {
            //[XmlElement(ElementName = "IDArticulo")]
            public string IDArticulo { get; set; }
            //[XmlElement(ElementName = "IDCaracteristica")]
            public string IDCaracteristica { get; set; }
            //[XmlElement(ElementName = "Formula")]
            public string Formula { get; set; }
            //[XmlElement(ElementName = "FactorCierre")]
            public string FactorCierre { get; set; }
            //[XmlElement(ElementName = "IDTipoArticulo")]
            public string IDTipoArticulo { get; set; }
            //[XmlElement(ElementName = "IDProceso")]
            public string IDProceso { get; set; }
            //[XmlElement(ElementName = "Indicaciones")]
            public string Indicaciones { get; set; }

                       public ArticuloXML()
                    {

                    }
        }

   
    public class Plantilla
        {

        public string Nombre { get; set; }

        public List<ArticuloXML> Articulos { get; set; }
       

        public Plantilla()
        {
            Articulos = new List<ArticuloXML>();
        }
    }

   
}