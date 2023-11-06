using MultiFacturasSDK;
using SIAAPI.clasescfdi;
using SIAAPI.Facturas;

using SIAAPI.Models.CartaPorte;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using static SIAAPI.clasescfdi.ClsCartaporte2;

namespace SIAAPI.clasescfdi
{
    public class ClsCartaporte2
    {
        public DateTime FechaSalida { get; set; }
        public DateTime FechaLlegada { get; set; }

        public int IDFactura { get; set; }
        public string SerieF { get; set; }

        public int NoFactura { get; set; }
        public int IDPropietario { get; set; }

        public int IDDomicilioOrigen { get; set; }

        public int IDCliente { get; set; }
        public int IDDomicilioentrega { get; set; }
        //public string Moneda { get; set; }
        public decimal DistanciaRecorrida { get; set; }
        public int IDOperador { get; set; }
        public int IDTransporte { get; set; }

        public decimal PesoTotal { get; set; }
        public int NumTotalMercancias { get; set; }


        private char center = (char)13;
        public bool timbrarpueba = false;
        public String Tipodecombrobante = "T";

        public Empresa Emisora;
        public Empresa Receptora;

        public String Moneda = "MXN";
        public string uso = "P01";
        public string _serie;
        public string _folio;
        public decimal subtotal = 0;
        public decimal total = 0;
        public Certificados certificado;

        public DomicilioOrigen Origen = new DomicilioOrigen();
        public DomicilioDestino Destino = new DomicilioDestino();
        public MercanciasCP Listaconceptos = new MercanciasCP();
        public InformacionOperador Operador = new InformacionOperador();
        public Conceptos Listacon = new Conceptos();


        public string Regimen;

        public ClsCartaporte2()
        {
            CertificadosContext db = new CertificadosContext();
            certificado = db.certificados.Find(2);
            try
            {
                EmpresaContext emp = new EmpresaContext();
                Emisora = emp.empresas.Find(2);
            }
            catch (Exception err)
            {
                string error = err.Message;
            }

            if (Properties.Settings.Default.TimbrarPrueba == "NO")
            {
                timbrarpueba = false;
            }
            else
            {
                timbrarpueba = true;
            }

        }
        public class InformacionOperador
        {
            public string Nombre;
            public string RFC;
            public string NoLicencia;
            public string Calle;
            public string NumeroExterior;
            public string NumeroInterior;
            public string Colonia;
            public string Localidad;
            public string Referencia;
            public string Municipio;
            public string Estado;
            public string Pais;
            public string CodigoPostal;


        }
        public class MercanciaCP
        {
            public int IDrenglon;
            public int IDCaracteristica;
            public string CodigoFamiliaPS;
            public string Descripcion;
            public decimal PesoKg;
            public int IDIdentificador;
            public int Nodecajas;
            public int Iddetprefactura;
            public string materialpeligroso;
            public string Claveunidad;
            public string Unidad;
            public decimal cantidad;
            public string clavemoneda;
            public decimal valor;
        }
        public class ConceptosCarta
        {

            public string Descripcion;
            public string ID;
            public string Unidad;
            public decimal cantidad;
            public decimal valorUnitario;
            public decimal Importe;
            public string ClaveProdServ;
            public string ClaveUnidad;

        }
        public class DomicilioOrigen
        {
            public string IDUbicacion;
            public string TipoUbicacion = "Origen";
            public string RFCRemitenteDestinatario;
            public DateTime FechaHoraSalidadLlegada;
            public string Calle;
            public string NumeroExterior;
            public string NumeroInterior;
            public string Colonia;
            public string Localidad;
            public string Referencia;
            public string Municipio;
            public string Estado;
            public string Pais;
            public string CodigoPostal;

        }
        public class DomicilioDestino
        {
            public string IDUbicacion;
            public string TipoUbicacion = "Destino";
            public string RFCRemitenteDestinatario;
            public DateTime FechaHoraSalidadLlegada;
            public decimal DistanciaRecorrida;
            public string Calle;
            public string NumeroExterior;
            public string NumeroInterior;
            public string Colonia;
            public string Localidad;
            public string Referencia;
            public string Municipio;
            public string Estado;
            public string Pais;
            public string CodigoPostal;

        }

        public class MercanciasCP
        {
            public List<MercanciaCP> conceptos = new List<MercanciaCP>();
            public MercanciasCP()
            {
                conceptos = new List<MercanciaCP>();
            }
        }
        public class Conceptos
        {
            public List<ConceptosCarta> conceptos = new List<ConceptosCarta>();
            public Conceptos()
            {
                conceptos = new List<ConceptosCarta>();
            }
        }

        public MFSDK construircartaporte()
        {
            MFSDK sdk = new MFSDK();

            sdk.Iniciales.Add("complemento", "cartaporte20");
            sdk.Iniciales.Add("MODOINI", "DIVISOR");
            sdk.Iniciales.Add("version_cfdi", "3.3");
            sdk.Iniciales.Add("validacion_local", "NO");
            sdk.Iniciales.Add("cfdi", HttpContext.Current.Server.MapPath("~/sdk2/timbrados/cfdi_ejemplo_factura_carta_porte_2_autotransporte_traslado.xml"));
            sdk.Iniciales.Add("xml_debug", HttpContext.Current.Server.MapPath("~/sdk2/timbrados/sin_timbrar_ejemplo_factura_carta_porte20_autotransporte_traslado.xml"));
            //sdk.Iniciales.Add("remueve_acentos", "NO");
            //sdk.Iniciales.Add("RESPUESTA_UTF8", "SI");
            //sdk.Iniciales.Add("html_a_txt", "NO");
            //sdk.Iniciales.Add("version", "2.0");


            MFObject factura = new MFObject("factura");

            factura["LugarExpedicion"] = Origen.CodigoPostal;
            factura["fecha_expedicion"] = DateTime.Now.ToString("s");
            factura["folio"] = _folio;
            factura["serie"] = _serie;
            factura["subtotal"] = "0";
            factura["moneda"] = "XXX";
            factura["total"] = "0";
            factura["tipocomprobante"] = "T";


            MFObject emisor = new MFObject("emisor");

            if (timbrarpueba)
            {
                emisor["RegimenFiscal"] = "601";
                emisor["nombre"] = "CINDEMEX SA DE CV";
                emisor["rfc"] = "LAN7008173R5";


            }
            else
            {
                emisor["RegimenFiscal"] = Regimen;
                emisor["nombre"] = Emisora.RazonSocial.Replace("Ñ", "&ntilde").Replace("&", "&amp").Replace("'", "&apos");
                emisor["rfc"] = Emisora.RFC;


            }

            MFObject receptor = new MFObject("receptor");
            receptor["UsoCFDI"] = "P01";
            //receptor["nombre"] = Receptora.RazonSocial.Replace("Ñ", "&ntilde;").Replace("&", "&amp;").Replace("'", "&apos;");
            receptor["nombre"] = Emisora.RazonSocial;
            receptor["rfc"] = Emisora.RFC;




            MFObject conceptos = new MFObject("conceptos");
            int a = 0;
            //foreach (ConceptosCarta elemento in Listacon.conceptos)
            //{
            //    //  string clave = DTGdetalles.Rows[iciclo].Cells["clave"].Value.ToString();
            //    if (a==1)
            //    {
            MFObject concepto0 = new MFObject(a.ToString());


            //concepto0["descripcion"] = elemento.Descripcion;
            //concepto0["ID"] = elemento.ID;
            //concepto0["unidad"] = elemento.Unidad;
            //concepto0["cantidad"] = elemento.cantidad.ToString();
            //concepto0["valorunitario"] = elemento.valorUnitario.ToString();
            //concepto0["importe"] = elemento.Importe.ToString();
            //concepto0["ClaveProdServ"] = elemento.ClaveProdServ;
            //concepto0["ClaveUnidad"] = elemento.ClaveUnidad;

            concepto0["descripcion"] = "FLETE";
            concepto0["ID"] = "M7390Z";
            concepto0["unidad"] = "SERVICIO";
            concepto0["cantidad"] = "1";
            concepto0["valorunitario"] = "0";
            concepto0["importe"] = "0";
            concepto0["ClaveProdServ"] = "78101802";
            concepto0["ClaveUnidad"] = "E48";

            conceptos.AgregaSubnodo(concepto0);
            //MFObject conceptosI = new MFObject("Impuestos");
            //MFObject conceptosIT = new MFObject("Traslados");
            //concepto0["Base"] = "0";
            //concepto0["Impuesto"] = "0";
            //concepto0["TipoFactor"] = "0";



            //    }


            //    a++;
            //}




            MFObject Cartap = new MFObject("cartaporte20");
            MFObject CartapA = new MFObject("atrs");
            CartapA["TotalDistRec"] = Destino.DistanciaRecorrida.ToString();
            CartapA["TranspInternac"] = "No";

            Cartap.AgregaSubnodo(CartapA);



            MFObject nodoencabezado = new MFObject("Ubicacion");

            MFObject nodoencabezado0 = new MFObject("0");

            MFObject nodoencabezadoOrigen = new MFObject("atrs");
            nodoencabezadoOrigen["FechaHoraSalidaLlegada"] = Origen.FechaHoraSalidadLlegada.ToString("s");
            nodoencabezadoOrigen["RFCRemitenteDestinatario"] = Origen.RFCRemitenteDestinatario;
            nodoencabezadoOrigen["IDUbicacion"] = Origen.IDUbicacion;
            nodoencabezadoOrigen["TipoUbicacion"] = Origen.TipoUbicacion;

            nodoencabezado0.AgregaSubnodo(nodoencabezadoOrigen);

            MFObject nodoencabezado1 = new MFObject("domicilio");
            nodoencabezado1["Municipio"] = Origen.Municipio;
            nodoencabezado1["Localidad"] = Origen.Localidad;
            nodoencabezado1["Colonia"] = Origen.Colonia;
            nodoencabezado1["NumeroInterior"] = Origen.NumeroInterior;
            nodoencabezado1["NumeroExterior"] = Origen.NumeroExterior;
            nodoencabezado1["CodigoPostal"] = Origen.CodigoPostal;
            nodoencabezado1["Calle"] = Origen.Calle;
            nodoencabezado1["Estado"] = Origen.Estado;
            nodoencabezado1["Pais"] = Origen.Pais;
            nodoencabezado1["Referencia"] = Origen.Referencia;

            nodoencabezado0.AgregaSubnodo(nodoencabezado1);

            nodoencabezado.AgregaSubnodo(nodoencabezado0);


            MFObject nodoencabezadoDestino = new MFObject("1");
            MFObject nodoencabezadoDestinoA = new MFObject("atrs");

            nodoencabezadoDestinoA["DistanciaRecorrida"] = Destino.DistanciaRecorrida.ToString();
            nodoencabezadoDestinoA["FechaHoraSalidaLlegada"] = Destino.FechaHoraSalidadLlegada.ToString("s");
            nodoencabezadoDestinoA["RFCRemitenteDestinatario"] = Origen.RFCRemitenteDestinatario;
            nodoencabezadoDestinoA["IDUbicacion"] = Destino.IDUbicacion;
            nodoencabezadoDestinoA["TipoUbicacion"] = Destino.TipoUbicacion;

            nodoencabezadoDestino.AgregaSubnodo(nodoencabezadoDestinoA);
            //nodoencabezado.AgregaSubnodo(nodoencabezadoDestino);

            MFObject DomicilioDestino = new MFObject("domicilio");
            DomicilioDestino["Municipio"] = Destino.Municipio;
            DomicilioDestino["Localidad"] = Destino.Localidad;
            DomicilioDestino["Colonia"] = Destino.Colonia;
            DomicilioDestino["NumeroInterior"] = Destino.NumeroInterior;
            DomicilioDestino["NumeroExterior"] = Destino.NumeroExterior;
            DomicilioDestino["CodigoPostal"] = Destino.CodigoPostal;
            DomicilioDestino["Calle"] = Destino.Calle;
            DomicilioDestino["Estado"] = Destino.Estado;
            DomicilioDestino["Pais"] = Destino.Pais;
            DomicilioDestino["Referencia"] = Destino.Referencia;
            nodoencabezadoDestino.AgregaSubnodo(DomicilioDestino);

            nodoencabezado.AgregaSubnodo(nodoencabezadoDestino);






            //foreach (MercanciaCP elemento in Listaconceptos.conceptos)
            //{
            //    //  string clave = DTGdetalles.Rows[iciclo].Cells["clave"].Value.ToString();

            //     concepto0M = new MFObject(i.ToString());
            //     conceptoMer = new MFObject("Mercancia");
            //    MFObject mercaAAT = new MFObject("atrs");

            //    mercaAAT["Cantidad"] = elemento.cantidad.ToString();
            //    mercaAAT["PesoEnKg"] = elemento.PesoKg.ToString();
            //    mercaAAT["BienesTransp"] = elemento.CodigoFamiliaPS;
            //    //mercaAAT["MaterialPeligroso"] = elemento.materialpeligroso;
            //    mercaAAT["Descripcion"] = elemento.Descripcion;
            //    mercaAAT["ClaveUnidad"] = elemento.Claveunidad.ToString();

            //    conceptoMer.AgregaSubnodo(mercaAAT);




            //    MFObject conceptoc = new MFObject("CantidadTransporta");
            //    concepto0M1 = new MFObject(i.ToString());
            //   concepto0M1["Cantidad"] = elemento.cantidad.ToString();
            //   concepto0M1["IDOrigen"] = Origen.IDUbicacion.ToString();
            //   concepto0M1["IDDestino"] = Destino.IDUbicacion.ToString();
            //    conceptoc.AgregaSubnodo(concepto0M1);

            //    conceptoMer.AgregaSubnodo(conceptoc);





            //    i++;
            //}

            //concepto0M.AgregaSubnodo(conceptoMer);


            //int i = 0;
            int i = 0;
            int valor = 0;
            MFObject merca = new MFObject("Mercancias");

            MFObject mercaA = new MFObject("atrs");

            mercaA["PesoBrutoTotal"] = PesoTotal.ToString();
            mercaA["NumTotalMercancias"] = NumTotalMercancias.ToString();
            mercaA["UnidadPeso"] = "KGM";
            merca.AgregaSubnodo(mercaA);

            ParqueVehicular vehicular = new ParqueVehicularDBContext().ParqueVe.Find(IDTransporte);
            c_ConfigAutotransporte _ConfigAutotransporte = new c_ConfigAutotransporteDBContext().ConfigAutotransporte.Find(vehicular.IDVehiculo);
            MFObject auto0 = null;
            foreach (MercanciaCP elemento in Listaconceptos.conceptos)
            {
                //  string clave = DTGdetalles.Rows[iciclo].Cells["clave"].Value.ToString();

                MFObject concepto0M = new MFObject(i.ToString());
                MFObject conceptoMer = new MFObject("Mercancia");
                MFObject mercaAAT = new MFObject("atrs");

                mercaAAT["Cantidad"] = elemento.cantidad.ToString();
                mercaAAT["PesoEnKg"] = elemento.PesoKg.ToString();
                mercaAAT["BienesTransp"] = elemento.CodigoFamiliaPS;
                //mercaAAT["MaterialPeligroso"] = elemento.materialpeligroso;
                mercaAAT["Descripcion"] = elemento.Descripcion;
                mercaAAT["ClaveUnidad"] = elemento.Claveunidad.ToString();

                conceptoMer.AgregaSubnodo(mercaAAT);


                //MFObject conceptoc = new MFObject("CantidadTransporta");
                //MFObject concepto0M1 = new MFObject(i.ToString());
                //concepto0M1["Cantidad"] = elemento.cantidad.ToString();
                //concepto0M1["IDOrigen"] = Origen.IDUbicacion.ToString();
                //concepto0M1["IDDestino"] = Destino.IDUbicacion.ToString();
                //conceptoc.AgregaSubnodo(concepto0M1);

                //conceptoMer.AgregaSubnodo(conceptoc);

                concepto0M.AgregaSubnodo(conceptoMer);

                if (i == 0)
                {
                    auto0 = new MFObject(valor.ToString());
                    MFObject conceptoA = new MFObject("Autotransporte");


                    MFObject conceptoAA = new MFObject("atrs");
                    conceptoAA["NumPermisoSCT"] = vehicular.NoPermisoSCT;
                    conceptoAA["PermSCT"] = vehicular.ClavePermisoSCT;

                    conceptoA.AgregaSubnodo(conceptoAA);


                    //conceptoAA["NombreAseg"] = vehicular.Aseguradora;
                    //conceptoAA["NumPolizaSeguro"] = vehicular.PolizaSeguro;


                    MFObject conceptoAI = new MFObject("IdentificacionVehicular");
                    conceptoAI["AnioModeloVM"] = vehicular.AnnoVehiculo.ToString();
                    conceptoAI["ConfigVehicular"] = _ConfigAutotransporte.ClaveNom;
                    conceptoAI["PlacaVM"] = vehicular.PlacaVehiculo;


                    conceptoA.AgregaSubnodo(conceptoAI);

                    MFObject conceptoAS = new MFObject("Seguros");
                    conceptoAS["PolizaRespCivil"] = vehicular.PolizaSeguro;
                    conceptoAS["AseguraRespCivil"] = vehicular.Aseguradora;
                    conceptoAS["AseguraCarga"] = vehicular.Aseguradora;

                    conceptoA.AgregaSubnodo(conceptoAS);


                    concepto0M.AgregaSubnodo(conceptoA);

                    //merca.AgregaSubnodo(auto0);

                }
                i++;
                merca.AgregaSubnodo(concepto0M);



            }







            MFObject Figura = new MFObject("FiguraTransporte");
            MFObject FiguraTrans = new MFObject("TiposFigura");
            MFObject Figura0 = new MFObject("0");

            MFObject conceptoAAT = new MFObject("atrs");
            conceptoAAT["TipoFigura"] = "01";
            conceptoAAT["RFCFigura"] = Operador.RFC;
            conceptoAAT["NumLicencia"] = Operador.NoLicencia;

            Figura0.AgregaSubnodo(conceptoAAT);
            FiguraTrans.AgregaSubnodo(Figura0);


            Figura.AgregaSubnodo(FiguraTrans);

            Cartap.AgregaSubnodo(nodoencabezado);
            Cartap.AgregaSubnodo(merca);

            Cartap.AgregaSubnodo(Figura);


            sdk.AgregaObjeto(PAC3());
            sdk.AgregaObjeto(Conf2());
            sdk.AgregaObjeto(factura);

            sdk.AgregaObjeto(emisor);
            sdk.AgregaObjeto(receptor);
            sdk.AgregaObjeto(conceptos);
            sdk.AgregaObjeto(Cartap);


            return sdk;
        }


        public string Strdecimal(decimal valor) // acompleta a dos digitos el valor
        {
            string cc = Math.Round(valor, 2).ToString();
            string[] cadena = cc.Split((char)46); // separo los decimales si los hay
            if (cadena.Length == 1)
            { return (cadena[0] + ".00"); }
            else if (cadena.Length == 2)
            {
                string caddecimales = cadena[1];
                if (caddecimales.Length == 1)
                {
                    return cadena[0] + "." + cadena[1] + "0";
                }
                if (caddecimales.Length == 2)
                {
                    return cadena[0] + "." + cadena[1];
                }
            }
            return valor.ToString();
        }

        public string Strdecimal4(decimal valor) // acompleta a dos digitos el valor
        {
            string cc = Math.Round(valor, 4).ToString();
            string[] cadena = cc.Split((char)46); // separo los decimales si los hay
            if (cadena.Length == 1)
            { return (cadena[0] + ".0000"); }
            else if (cadena.Length == 2)
            {
                string caddecimales = cadena[1];
                if (caddecimales.Length == 1)
                {
                    return cadena[0] + "." + cadena[1] + "000";
                }
                if (caddecimales.Length == 2)
                {
                    return cadena[0] + "." + cadena[1] + "00";
                }

                if (caddecimales.Length == 3)
                {
                    return cadena[0] + "." + cadena[1] + "0";
                }
                if (caddecimales.Length == 3)
                {
                    return cadena[0] + "." + cadena[1];
                }
            }
            return valor.ToString();
        }


        public MFObject PAC3()
        {

            MFObject pac = new MFObject("PAC");

            if (timbrarpueba)
            {
                pac["usuario"] = "DEMO700101XXX";
                pac["pass"] = "DEMO700101XXX";
                pac["produccion"] = "NO";
            }
            else
            {
                pac["usuario"] = certificado.UsuarioMultifacturas;
                pac["pass"] = certificado.PassMultifacturas;

                pac["produccion"] = "SI";

            }
            return pac;
        }

        public MFObject Conf2()
        {
            MFObject conf = new MFObject("conf");

            if (timbrarpueba)
            {
                conf["cer"] = HttpContext.Current.Server.MapPath("~/CertificadosClientes/demo.cer");
                conf["key"] = HttpContext.Current.Server.MapPath("~/CertificadosClientes/demo.key");
                conf["pass"] = "12345678a";
            }
            else
            {
                conf["cer"] = HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelcertificado);
                conf["key"] = HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelkey);
                conf["pass"] = certificado.PassCertificado;
            }

            return conf;
        }

        public SDKRespuesta timbrar(MFSDK sdk)
        {
            SDKRespuesta respuesta = null;
            try
            {
                Random rnd = new Random();
                int rndx = rnd.Next(0, 1000);
                // throw new Exception("error intencional00");
                var folder = System.Web.HttpContext.Current.Server.MapPath("~/ErrorTimbrado/error factura" + rndx);

                //if (!Directory.Exists(folder))
                //{
                //    Directory.CreateDirectory(folder);

                //    string filePathE = System.IO.Path.Combine(folder, "Enviaado" + DateTime.Now.ToShortDateString() + "-" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ".txt");

                //    using (StreamWriter writer = File.CreateText(filePathE))
                //    {
                //        writer.WriteLine("codigo de texto: " + sdk.Ini);

                //    }

                //}

                respuesta = sdk.Timbrar(HttpContext.Current.Server.MapPath("~/sdk2/timbrar32.bat"), HttpContext.Current.Server.MapPath("~/sdk2/timbrados/"), "factura", false);

                //string filePath = System.IO.Path.Combine(folder, "Respuesta" + DateTime.Now.ToShortDateString() + "-" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ".txt");

                //using (StreamWriter writer = File.CreateText(filePath))
                //{
                //    writer.WriteLine("codigo de texto: "+respuesta.Codigo_MF_Texto);

                //}



            }
            catch (Exception err)
            {
                //crear codigo cree un txt lamado errortimbrar
                // err.mensake;

                Random rnd = new Random();
                int rndx = rnd.Next(0, 1000);


                var folder = System.Web.HttpContext.Current.Server.MapPath("~/ErrorTimbrado/error factura" + rndx);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);

                    string filePath = System.IO.Path.Combine(folder, "fichero.txt");

                    using (StreamWriter writer = File.CreateText(filePath))
                    {
                        writer.WriteLine(err);

                    }

                }


            }
            Random rnd2 = new Random();
            int rndx2 = rnd2.Next(0, 1000);


            var folder2 = System.Web.HttpContext.Current.Server.MapPath("~/ErrorTimbrado/error factura" + rndx2);
            if (!Directory.Exists(folder2))
            {
                Directory.CreateDirectory(folder2);

                string filePath2 = System.IO.Path.Combine(folder2, "fichero2.txt");

                using (StreamWriter writer = File.CreateText(filePath2))
                {
                    writer.WriteLine(respuesta.Codigo_MF_Texto);

                }

            }
            return respuesta;
        }

        //public bool cancelarbyid(string uuid, string xml)
        //{
        //    string usuario = "DEMO700101XXX";
        //    string pass = "DEMO700101XXX";
        //    string rfc = "LAN7008173R5";

        //    string cer = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/demo.cer")));
        //    string key = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/demo.key")));
        //    string passcer = "12345678a";


        //    if (!timbrarpueba)
        //    {
        //        usuario = certificado.UsuarioMultifacturas;
        //        pass = certificado.PassMultifacturas;
        //        cer = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelcertificado)));
        //        key = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelkey)));


        //        passcer = Convert.ToBase64String(Encoding.UTF8.GetBytes(certificado.PassCertificado));
        //        rfc = Emisora.RFC;
        //    }

        //    xml = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml));
        //    //  SoapCancelar.RespuestaNET respuesta = new SoapCancelar.RespuestaNET();
        //    using (SoapCancelar.wservicePortTypeClient cliente = new SoapCancelar.wservicePortTypeClient())
        //    {
        //        try
        //        {
        //            var inputEncoding = Encoding.GetEncoding("iso-8859-1");
        //            System.Text.Encoding iso_8859_1 = System.Text.Encoding.GetEncoding("iso-8859-1");
        //            System.Text.Encoding utf_8 = System.Text.Encoding.UTF8;

        //            //XmlDocument doc = new XmlDocument();

        //            //  cliente.cancelar(rfc, pass, uuid, cer, key, passcer);
        //            ////  doc.LoadXml();

        //            //  doc.Save(HttpContext.Current.Server.MapPath("~/Documentostemporales/acuse.xml"));

        //            byte[] isoBytes = iso_8859_1.GetBytes(cliente.cancelar(rfc, pass, uuid, cer, key, passcer));

        //            byte[] utf8Bytes = System.Text.Encoding.Convert(iso_8859_1, utf_8, isoBytes); ;

        //            string respuesta = System.Text.Encoding.UTF8.GetString(utf8Bytes);

        //            if (respuesta.Contains("OK"))
        //            {
        //                //  MessageBox.Show("La factura fue cancelada ante el sat");
        //                return true;
        //            }
        //            else
        //            {
        //                //MessageBox.Show(respuesta.Codigo_MF_Texto);
        //                return false;
        //            }
        //        }
        //        catch (Exception err)
        //        {
        //            if (err.Message.Contains("UUID CANCELADO CORRECTAMENTE"))
        //            {
        //                return true;
        //            }
        //            else
        //            { return false; }
        //        }
        //    }






        

    }


    public class Ejemplocartaporte
    {
        ClsCartaporte2 facturaejemplo = new ClsCartaporte2();

        public void EjemploFacturacartaporte()
        {
            facturaejemplo.Regimen = "601";
            facturaejemplo.timbrarpueba = true;
            facturaejemplo.Tipodecombrobante = "T";


            facturaejemplo._serie = "C";
            facturaejemplo._folio = "1";





            Empresa emisor = new Empresa();
            emisor.RFC = "VCO1707276L0";
            emisor.RazonSocial = "VIGMA CONSULTORES SAS DE CV";
            emisor.Calle = "PEDRO GARCIA RANGEL ";
            emisor.NoExt = "184";
            emisor.NoInt = "";
            emisor.CP = "43760";
            facturaejemplo.Emisora = emisor;

            Empresa Receptor = new Empresa();
            Receptor.RFC = "XAXX010101000";
            Receptor.RazonSocial = "PUBLICO EN GENERAL";
            Receptor.Calle = "PEDRO GARCIA RANGEL ";
            Receptor.NoExt = "184";
            Receptor.NoInt = "";
            Receptor.CP = "43760";
            facturaejemplo.Receptora = Receptor;


            List<MercanciaCP> conceptos = new List<MercanciaCP>();
            MercanciaCP concepto1 = new MercanciaCP();
            concepto1.CodigoFamiliaPS = "ALMG";
            concepto1.cantidad = 1;
            concepto1.Descripcion = "ALMG";
            concepto1.Claveunidad = "";
            concepto1.materialpeligroso = "NO";
            concepto1.PesoKg = 1;
            concepto1.valor = 1;
            concepto1.clavemoneda = "MXN"; //prefactura.c_Moneda.ClaveMoneda;




            conceptos.Add(concepto1);


            facturaejemplo.Listaconceptos.conceptos = conceptos;

            //  facturaejemplo.timbrarpueba = true;
            // public Cfdirelacionados cfdirelacionados = null;
            // public Certificados certificado;










        }



        public string timbrarfacturaejemplocartaporte()
        {
            MultiFacturasSDK.MFSDK X = facturaejemplo.construircartaporte();
            //  facturaejemplo.EscribeEnArchivo(strini, HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini"), true);
            //   string ini = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini")));
            SDKRespuesta respuesta = facturaejemplo.timbrar(X);
            //WSmultifacturas.RespuestaWS respuesta = facturaejemplo.timbrar(ini);

            return respuesta.Codigo_MF_Texto + " xml->" + respuesta.CFDI;
        }

    }

}