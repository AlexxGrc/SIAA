
using System.Collections.Generic;

using System.Xml;
//Elaboro M. en C.C. Israel Villar Medina 10/08/2018

namespace SIAAPI.clasescfdi
{
    public class EmisorPago
    {

        public string rfc = string.Empty;
       

        public string Nombre { get; internal set; }
        public string RegimenFiscal { get; internal set; }
   


    }


    public class ReceptorPago
    {

        public string rfc = string.Empty;


        public string Nombre { get; internal set; }
        public string usoCFDI { get; internal set; }



    }
    public class ProductoPago
    {

        public int cantidad = 1;
        public string descripcion = "Pago";
        public string unidad = "ACTIVIDAD";
        public float valorUnitario = 0.00f;
        public float importe = 0.00f;
        public string ClaveProducto = "84111506";
        public string c_unidad = "ACT";
    }

    public class DocumentoRelacionadoPago
    {
        public string IdDocumento { get; set; }
      

        public string Folio { get; set; }


        public string MonedaDR { get; set; }

        public string MetodoDePagoDR { get; set; }
        public string NumParcialidad { get; set; }
        public decimal ImpSaldoAnt { get; set; }

        public decimal ImpPagado { get; set; }

        public decimal ImpSaldoInsoluto { get; set; }
    }


    public class PagoP
    {
        public string FechaPago { get; set; }
        public string FormaDePagoP { get; set; }

        public string MonedaP { get; set; }

        public decimal Monto { get; set; }


        public string RfcEmisorCtaOrd = string.Empty;

        public string CtaOrdenante = string.Empty;

        public string RfcEmisorCtaBen = string.Empty;

        public string CtaBeneficiario = string.Empty;

        public string NumOperacion = string.Empty;

        public string TipoCadPago = string.Empty;

        public string CadenaPago = string.Empty;

        public string CertPago = string.Empty;

        public string SelloPago = string.Empty;

        public List<DocumentoRelacionadoPago> documentoRelacionado = new List<DocumentoRelacionadoPago>();
    }

    public class Complemento
    {
       
        public List< PagoP> Pagos =new List<PagoP>();

        public TimbreFiscalDigitalPago TimbreFiscalDigital = new TimbreFiscalDigitalPago();

        public Complemento()
        {
            Pagos = new List<PagoP>();
        }
    }

    public class Facturapago
    {
        public string Fecha { get; set; }
        public string Serie { get; set; }
        public int Folio { get; set; }

        public decimal SubTotal = 0;

        public string Moneda = "XXX";

        public decimal Total = 0;

        public string TipoDeComprobante = "P";

        public string LugarExpedicion { get; set; } 
    }


    public class TimbreFiscalDigitalPago
    {
       
       

        public string Tfd { get; set; }
        public string SchemaLocation { get; set; }
        public string Version { get; set; }

        public string UUID { get; set; }

        public string FechaTimbrado { get; set; }
        public string RfcProvCertif { get; set; }

        public string SelloCFD { get; set; }

        public string NoCertificadoSAT { get; set; }

        public string SelloSAT { get; set; }

    }


    public class ComprobantePago
    {

        public EmisorPago Emisor = new EmisorPago();

        public ReceptorPago Receptor = new ReceptorPago();

        public ProductoPago Concepto = new ProductoPago();

        public Complemento Complemento = new Complemento();
        
        public string Xsi { get; set; }
       
        public string Cfdi { get; set; }
        
     
        public string SchemaLocation { get; set; }
       
        public string Version { get; set; }
       
        public string Fecha { get; set; }
       
        public string Serie { get; set; }
       
        public string Folio { get; set; }
       
        public string SubTotal { get; set; }
      
        public string Moneda { get; set; }
      
        public string Total { get; set; }
        
        public string TipoDeComprobante { get; set; }
       
        public string LugarExpedicion { get; set; }
        
        public string Certificado { get; set; }
      
        public string NoCertificado { get; set; }
     
        public string Sello { get; set; }
    }


    public class ClsXmlPagos
    {
        public string Xml = string.Empty;

        public ComprobantePago Comprobante = new ComprobantePago();

        public XmlDocument xDoc;

      

        public ClsXmlPagos()
        {

        }

        public ClsXmlPagos(string _xml)
        {
            Xml = _xml;
            LeerAtributosXML();
        }

        public void LeerAtributosXML()
        {
            xDoc = new XmlDocument();

            xDoc.LoadXml(Xml);

            getNodoCfdiComprobante();
            getNodoTimbreFiscalDigital();
            getNodoEmisor();
            getNodoReceptor();
            getNodoPagos();
            

        }

        /// <summary>
        /// obtiene la informacion del comprobante como lo es total
        /// </summary>
        private void getNodoCfdiComprobante()
        {

            if (xDoc.GetElementsByTagName("cfdi:Comprobante") == null)
                return;

                XmlNodeList comprobante = xDoc.GetElementsByTagName("cfdi:Comprobante");
                if (((XmlElement)comprobante[0]).GetAttribute("Serie") != null)
                    Comprobante.Serie = ((XmlElement)comprobante[0]).GetAttribute("Serie");
                if (((XmlElement)comprobante[0]).GetAttribute("Folio") != null)
                    Comprobante.Folio = ((XmlElement)comprobante[0]).GetAttribute("Folio");
                if (((XmlElement)comprobante[0]).GetAttribute("Fecha") != null)
                    Comprobante.Fecha = ((XmlElement)comprobante[0]).GetAttribute("Fecha");
                if (((XmlElement)comprobante[0]).GetAttribute("Sello") != null)
                    Comprobante.Sello = ((XmlElement)comprobante[0]).GetAttribute("Sello");
                if (((XmlElement)comprobante[0]).GetAttribute("SubTotal") != null)
                {
                    Comprobante.SubTotal = ((XmlElement)comprobante[0]).GetAttribute("SubTotal");
                }
                if (((XmlElement)comprobante[0]).GetAttribute("Moneda") != null)
                    Comprobante.Moneda = ((XmlElement)comprobante[0]).GetAttribute("Moneda");

                if (((XmlElement)comprobante[0]).GetAttribute("Total") != null)
                {
                    Comprobante.Total = ((XmlElement)comprobante[0]).GetAttribute("Total");
                }

                if (((XmlElement)comprobante[0]).GetAttribute("TipoDeComprobante") != null)
                { Comprobante.TipoDeComprobante = ((XmlElement)comprobante[0]).GetAttribute("TipoDeComprobante"); }

                if (((XmlElement)comprobante[0]).GetAttribute("TipoCambio") != null)
                { Comprobante.LugarExpedicion = ((XmlElement)comprobante[0]).GetAttribute("LugarExpedicion"); }

                if (((XmlElement)comprobante[0]).GetAttribute("Certificado") != null)
                {
                    Comprobante.Certificado = ((XmlElement)comprobante[0]).GetAttribute("Certificado");
                }


                if (((XmlElement)comprobante[0]).GetAttribute("NoCertificado") != null)
                {
                    Comprobante.NoCertificado = ((XmlElement)comprobante[0]).GetAttribute("NoCertificado");
                }

        

            
          }


        /// <summary>
        /// Obtiene que empresa es la emisora
        /// </summary>
        private void getNodoEmisor()
        {
            //Trabajamos con Emisor
            if (xDoc.GetElementsByTagName("cfdi:Emisor") == null)
                return;

            XmlNodeList emisor = xDoc.GetElementsByTagName("cfdi:Emisor");
            Comprobante.Emisor.rfc = ((XmlElement)emisor[0]).GetAttribute("rfc");
            Comprobante.Emisor.Nombre = ((XmlElement)emisor[0]).GetAttribute("Nombre");
            Comprobante.Emisor.RegimenFiscal = ((XmlElement)emisor[0]).GetAttribute("RegimenFiscal");


        }

        /// <summary>
        /// Obtiene la empresa Receptera
        /// </summary>

        private void getNodoReceptor()
        {
            //Trabajamos con receptor
            XmlNodeList receptor = xDoc.GetElementsByTagName("cfdi:Receptor");
            Comprobante.Receptor.Nombre = ((XmlElement)receptor[0]).GetAttribute("Nombre");
            Comprobante.Receptor.rfc = ((XmlElement)receptor[0]).GetAttribute("Rfc");
            Comprobante.Receptor.usoCFDI = ((XmlElement)receptor[0]).GetAttribute("UsoCFDI");
        }



        /// <summary>
        /// Obtiene los datos del timbre fiscal
        /// </summary>
        private void getNodoTimbreFiscalDigital()
        {
            XmlNodeList tfDigital = xDoc.GetElementsByTagName("tfd:TimbreFiscalDigital");
            if (tfDigital.Count <= 0)
                return;

            if (((XmlElement)tfDigital[0]).GetAttribute("UUID") != null)
                Comprobante.Complemento.TimbreFiscalDigital.UUID = ((XmlElement)tfDigital[0]).GetAttribute("UUID");


            if (((XmlElement)tfDigital[0]).GetAttribute("FechaTimbrado") != null)
                Comprobante.Complemento.TimbreFiscalDigital.FechaTimbrado = ((XmlElement)tfDigital[0]).GetAttribute("FechaTimbrado");

            if (((XmlElement)tfDigital[0]).GetAttribute("RfcProvCertif") != null)
                Comprobante.Complemento.TimbreFiscalDigital.RfcProvCertif = ((XmlElement)tfDigital[0]).GetAttribute("RfcProvCertif");


            if (((XmlElement)tfDigital[0]).GetAttribute("SelloCFD") != null)
                Comprobante.Complemento.TimbreFiscalDigital.SelloCFD = ((XmlElement)tfDigital[0]).GetAttribute("SelloCFD");



            if (((XmlElement)tfDigital[0]).GetAttribute("NoCertificadoSAT") != null)
                Comprobante.Complemento.TimbreFiscalDigital.NoCertificadoSAT = ((XmlElement)tfDigital[0]).GetAttribute("NoCertificadoSAT");


            if (((XmlElement)tfDigital[0]).GetAttribute("SelloSAT") != null)
                Comprobante.Complemento.TimbreFiscalDigital.SelloSAT = ((XmlElement)tfDigital[0]).GetAttribute("SelloSAT");

            if (((XmlElement)tfDigital[0]).GetAttribute("SelloCFD") != null)
                Comprobante.Complemento.TimbreFiscalDigital.SelloCFD = ((XmlElement)tfDigital[0]).GetAttribute("SelloCFD");
        }


        /// <summary>
        /// Obtiene los pagos
        /// </summary>
        private void getNodoPagos()
        {
            

            if (xDoc.GetElementsByTagName("pago10:Pagos") == null)
                return;
            XmlNodeList pagos = xDoc.GetElementsByTagName("pago10:Pagos");

            if (((XmlElement)pagos[0]).GetElementsByTagName("pago10:Pago") == null)
                return;
            XmlNodeList lista = ((XmlElement)pagos[0]).GetElementsByTagName("pago10:Pago");
           
            foreach (XmlElement nodo in lista)
            {
                PagoP p = new PagoP();

                p.FechaPago= nodo.GetAttribute("FechaPago");
                p.FormaDePagoP= nodo.GetAttribute("FormaDePagoP");
                p.MonedaP = nodo.GetAttribute("MonedaP");
                p.Monto = decimal.Parse( nodo.GetAttribute("Monto"));




                /// si fue un deposito bancario llenara estos atributos
                if (((XmlElement)nodo).GetAttribute("RfcEmisorCtaOrd") != null)
                    p.RfcEmisorCtaOrd = ((XmlElement)nodo).GetAttribute("RfcEmisorCtaOrd");
                if (((XmlElement)nodo).GetAttribute("CtaOrdenante") != null)
                    p.CtaOrdenante = ((XmlElement)nodo).GetAttribute("CtaOrdenante");

                if (((XmlElement)nodo).GetAttribute("RfcEmisorCtaBen") != null)
                    p.RfcEmisorCtaBen = ((XmlElement)nodo).GetAttribute("RfcEmisorCtaBen");


                if (((XmlElement)nodo).GetAttribute("CtaBeneficiario") != null)
                    p.CtaBeneficiario = ((XmlElement)nodo).GetAttribute("CtaBeneficiario");


                if (((XmlElement)nodo).GetAttribute("NumOperacion") != null)
                    p.NumOperacion = ((XmlElement)nodo).GetAttribute("NumOperacion");

                if (((XmlElement)nodo).GetAttribute("TipoCadPago") != null)
                    p.TipoCadPago = ((XmlElement)nodo).GetAttribute("TipoCadPago");

                if (((XmlElement)nodo).GetAttribute("CertPago") != null)
                    p.CertPago = ((XmlElement)nodo).GetAttribute("CertPago");

                if (((XmlElement)nodo).GetAttribute("CadenaPago") != null)
                    p.CertPago = ((XmlElement)nodo).GetAttribute("CadenaPago");


                if (((XmlElement)nodo).GetAttribute("SelloPago") != null)
                    p.CertPago = ((XmlElement)nodo).GetAttribute("SelloPago");



              XmlNodeList documentos = nodo.GetElementsByTagName("pago10:DoctoRelacionado");

                foreach (XmlElement nododocumento in documentos)
                {

                    DocumentoRelacionadoPago pr = new DocumentoRelacionadoPago();
                    if (((XmlElement)nododocumento).GetAttribute("IdDocumento") != null)
                        pr.IdDocumento   = ((XmlElement)nododocumento).GetAttribute("IdDocumento");


                    pr.Folio = nododocumento.GetAttribute("Folio");
                    pr.NumParcialidad= nododocumento.GetAttribute("NumParcialidad");
                    pr.MonedaDR = nododocumento.GetAttribute("MonedaDR");
                    pr.MetodoDePagoDR = nododocumento.GetAttribute("MetodoDePagoDR");
                    pr.ImpPagado = decimal.Parse( nododocumento.GetAttribute("ImpPagado"));
                    pr.ImpSaldoAnt = decimal.Parse(nododocumento.GetAttribute("ImpSaldoAnt"));
                    pr.ImpSaldoInsoluto = decimal.Parse(nododocumento.GetAttribute("ImpSaldoInsoluto"));
                    p.documentoRelacionado.Add(pr);

                }

                    Comprobante.Complemento.Pagos.Add(p);
            }
        }



    }
}