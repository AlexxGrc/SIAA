using System;
using System.Collections.Generic;
using System.Text;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Cfdi;
using Generador;
using System.Web;
using System.IO;
using SIAAPI.Facturas;
using System.Xml;
using System.util;
using MultiFacturasSDK;
using Newtonsoft.Json;

namespace SIAAPI.Facturas
{

    public class ClsFactura
    {

        private char center = (char)13;
        public bool timbrarpueba = false;
        public String Tipodecombrobante = "I";
        public string tipodecambio = "1.0000";
        public Empresa Emisora;
        public Empresa Receptora;
        public string metododepago = "PUE";
   
        public string formadepago = "01";
        public String Moneda = "MXN";
        public string uso = "G01";
        public string _serie;
        public string _folio;
        public decimal Descuento = 0;
        public Certificados certificado;
        public Conceptos Listaconceptos = new Conceptos();
        public DocumentoPDF template;
        public decimal valoriva = 0.16M;
        public Cfdirelacionados cfdirelacionados = new Cfdirelacionados();
      
        public string Regimen;
       
        public Encpagosafacturas Encabezadosfacturas = new Encpagosafacturas();
   
        public ClsFactura()
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

        #region  "multifacturas"



    

        public MFSDK construirfactura2()
        {
            MFSDK sdk = new MFSDK();
            sdk.Iniciales.Add("version_cfdi", "3.3");
            sdk.Iniciales.Add("MODOINI", "DIVISOR");
            sdk.Iniciales.Add("cfdi", HttpContext.Current.Server.MapPath("~/sdk2/timbrados/ejemplo_cfdi33.xml"));
            sdk.Iniciales.Add("xml_debug", HttpContext.Current.Server.MapPath("~/sdk2/timbrados/debug_ejemplo_cfdi33.xml"));
            sdk.Iniciales.Add("remueve_acentos", "NO");
            sdk.Iniciales.Add("RESPUESTA_UTF8", "SI");
            sdk.Iniciales.Add("html_a_txt", "NO");




            MFObject emisor = new MFObject("emisor");

            if (timbrarpueba)
            {
                emisor["rfc"] = "LAN7008173R5";
                emisor["nombre"] = "CINDEMEX SA DE CV";
                emisor["RegimenFiscal"] = "601";
            }
            else
            {

                emisor["rfc"] = Emisora.RFC.Replace("&", "&amp;");
                emisor["nombre"] = Emisora.RazonSocial;
                emisor["RegimenFiscal"] = Regimen;

            }

            MFObject receptor = new MFObject("receptor");


            //if (timbrarpueba)
            //{
            //    receptor["rfc"] = "XAXX010101000";
            //    receptor["nombre"] = "PUBLICO EN GENERAL";
            //    receptor["UsoCFDI"] = "G03";
            //}
            //else
            //{
                receptor["rfc"] = Receptora.RFC;
                receptor["nombre"] = Receptora.RazonSocial;
                receptor["UsoCFDI"] = uso;

            //}


            MFObject conceptos = new MFObject("conceptos");
            decimal acudescuento = 0;
            decimal acusubtotal = 0;
            decimal acuiva = 0;
            decimal total = 0;
         

            int i = 0;
            foreach (Concepto elemento in Listaconceptos.conceptos)
            {
                //  string clave = DTGdetalles.Rows[iciclo].Cells["clave"].Value.ToString();

                MFObject concepto0 = new MFObject(i.ToString());


                concepto0["ClaveProdServ"] = elemento.ClaveProdServ;
                concepto0["NoIdentificacion"] = (i + 1).ToString();
                concepto0["Cantidad"] = elemento.Cantidad.ToString();
                concepto0["Unidad"] = elemento.Unidad.ToString();
                concepto0["ClaveUnidad"] = elemento.ClaveUnidad.ToString();
                concepto0["Descripcion="] = elemento.Descripcion;

                concepto0["ValorUnitario"] = Math.Round(elemento.ValorUnitario, 2).ToString();
                concepto0["Importe"] = Math.Round(elemento.Importe, 2).ToString();
                if (elemento.Descuento > 0)
                {
                   
                    concepto0["Descuento"] = Math.Round(elemento.Descuento, 2).ToString();
                }



                MFObject impuestosporconcepto = new MFObject("Impuestos");

                MFObject itraslados = new MFObject("Traslados");
                MFObject itraslado0 = new MFObject(i.ToString());
                itraslado0["Base"] = Math.Round((elemento.Importe- elemento.Descuento), 2).ToString();
                itraslado0["Impuesto"] = "002";

                Decimal IVA = Math.Round(Math.Round((elemento.Importe - elemento.Descuento),2) * valoriva, 2);
                if (elemento.llevaiva)
                {
                    acuiva += IVA;
                    itraslado0["Importe"] = IVA.ToString();
                    itraslado0["TasaOCuota"] = "0.160000";
                    itraslado0["TipoFactor"] = "Tasa";
                }
                else
                {
                  
                    itraslado0["TipoFactor"] = "Exento";
                }
             
                itraslados.AgregaSubnodo(itraslado0);
                impuestosporconcepto.AgregaSubnodo(itraslados);
                concepto0.AgregaSubnodo(impuestosporconcepto);


                conceptos.AgregaSubnodo(concepto0);
                acusubtotal = acusubtotal + Math.Round(elemento.Importe, 2) ;
                acudescuento = acudescuento + Math.Round(elemento.Descuento, 2);
                i++;
            }



            total = acusubtotal   - acudescuento + acuiva;
            MFObject impuestos = new MFObject("impuestos");
            impuestos["TotalImpuestosTrasladados"] = Math.Round(acuiva, 2).ToString();
            // Traslados
            MFObject itras = new MFObject("translados");
            MFObject itra0 = new MFObject("0");

            itra0["Impuesto"] = "002";




            if (acuiva > 0)
            {
                itra0["TasaOCuota"] = "0.160000";
                itra0["Importe"] = Math.Round(acuiva, 2).ToString();
                itra0["TipoFactor"] = "Tasa";

            }
            else
            {
                itra0["TipoFactor"] = "Exento";


            }
            
            itras.AgregaSubnodo(itra0);
            impuestos.AgregaSubnodo(itras);

            MFObject cfdiRelacionados1 = new MFObject("CfdisRelacionados");


            if (cfdirelacionados != null)
            {
                if (cfdirelacionados.uuid.Count > 0)
                {



                    cfdiRelacionados1["TipoRelacion"] = cfdirelacionados.relacion;
                    MFObject cfdirel = new MFObject("UUID");

                    for (int j = 0; j < cfdirelacionados.uuid.Count; j++)
                    {
                        //Se pueden agregar varios relacionados con UUID
                        if (cfdirelacionados.uuid[j] != "")
                            {
                            cfdirel[j.ToString()] = cfdirelacionados.uuid[j];
                        }

                    }

                    cfdiRelacionados1.AgregaSubnodo(cfdirel);

                }
            }


            MFObject factura = new MFObject("factura");


            factura["serie"] = _serie;
            factura["folio"] = _folio;
            factura["fecha_expedicion"] = DateTime.Now.ToString("s");
            factura["metodo_pago"] = metododepago;
            factura["forma_pago"] = formadepago;

            factura["tipocomprobante"] = Tipodecombrobante;
            factura["moneda"] = Moneda;
            factura["tipocambio"] = tipodecambio;
            factura["LugarExpedicion"] = Emisora.CP;
            factura["RegimenFiscal"] = Regimen; /// cambiar por el reginme
            factura["subtotal"] = acusubtotal.ToString();//100.00

            if (acudescuento > 0)
            {
                factura["Descuento"] = acudescuento.ToString();
            }
            factura["total"] = total.ToString();//100.00




            sdk.AgregaObjeto(PAC3());
            sdk.AgregaObjeto(Conf2());
            sdk.AgregaObjeto(factura);
            if (cfdirelacionados.uuid.Count > 0)
            {
                sdk.AgregaObjeto(cfdiRelacionados1);
            }
            sdk.AgregaObjeto(emisor);
            sdk.AgregaObjeto(receptor);
            sdk.AgregaObjeto(conceptos);
            sdk.AgregaObjeto(impuestos);
            // Muestras la estructura
            // sdk.CreaINI(System.Web.HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini"));

            return sdk;
        }


        public MFSDK construirfacturadepagos()
        {
            MFSDK sdk = new MFSDK();
            sdk.Iniciales.Add("version_cfdi", "3.3");
          
            sdk.Iniciales.Add("complemento", "pagos10");
            sdk.Iniciales.Add("MODOINI", "DIVISOR");
            sdk.Iniciales.Add("cfdi", HttpContext.Current.Server.MapPath("~/sdk2/timbrados/ejemplo_cfdi33.xml"));
            sdk.Iniciales.Add("xml_debug", HttpContext.Current.Server.MapPath("~/sdk2/timbrados/debug_ejemplo_cfdi33.xml"));
            sdk.Iniciales.Add("remueve_acentos", "NO");
            sdk.Iniciales.Add("RESPUESTA_UTF8", "SI");
            sdk.Iniciales.Add("html_a_txt", "NO");
            sdk.Iniciales.Add("validacion_local", "NO");

            string fecha = DateTime.Now.ToString("s");

            MFObject emisor = new MFObject("emisor");

            if (timbrarpueba)
            {
                emisor["rfc"] = "LAN7008173R5";
                emisor["nombre"] = "CINDEMEX SA DE CV";
                emisor["RegimenFiscal"] = "601";
            }
            else
            {

                emisor["rfc"] = Emisora.RFC;
                emisor["nombre"] = Emisora.RazonSocial;
                emisor["RegimenFiscal"] = Regimen;

            }

            MFObject receptor = new MFObject("receptor");


            //if (timbrarpueba)
            //{
            //    receptor["rfc"] = "XAXX010101000";
            //    receptor["nombre"] = "PUBLICO EN GENERAL";
            //    receptor["UsoCFDI"] = "P01";
            //}
            //else
            //{
                receptor["rfc"] = Receptora.RFC;
                receptor["nombre"] = Receptora.RazonSocial;
                receptor["UsoCFDI"] = "P01";

            //}


            MFObject conceptos = new MFObject("conceptos");


            int i = 0;
            foreach (Concepto elemento in Listaconceptos.conceptos)
            {
                //  string clave = DTGdetalles.Rows[iciclo].Cells["clave"].Value.ToString();

                MFObject concepto0 = new MFObject(i.ToString());


                concepto0["ClaveProdServ"] = elemento.ClaveProdServ;
               
                concepto0["Cantidad"] = elemento.Cantidad.ToString();
                concepto0["ClaveUnidad"] = elemento.ClaveUnidad.ToString();
           //     concepto0["Unidad"] = elemento.Unidad.ToString();
               
                concepto0["Descripcion"] = elemento.Descripcion;

                concepto0["ValorUnitario"] = Math.Round(elemento.ValorUnitario, 2).ToString();
                concepto0["Importe"] = Math.Round(elemento.Importe, 2).ToString();
                //concepto0["Descuento="] = Math.Round(elemento.Descuento, 2).ToString();



                conceptos.AgregaSubnodo(concepto0);

                i++;
            }




            MFObject pagosrelacionados = new MFObject("pagos10");
            MFObject nodoencabezado= new MFObject("Pagos");

            if (Encabezadosfacturas != null)
            {
                if (Encabezadosfacturas.encabezados.Count > 0)
                {




                    int j = 0;

                    foreach (Encpagofactura encabezado in Encabezadosfacturas.encabezados)
                    {
                        //Se pueden agregar varios relacionados con UUID
                        MFObject pago0 = new MFObject(j.ToString());

                        pago0["FechaPago"] = encabezado.FechaPago;
                        pago0["FormaDePagoP"] = encabezado.Formadepago;
                        pago0["MonedaP"] =  encabezado.Moneda;
                        pago0["Monto"] = Strdecimal (encabezado.Monto);
                        if (encabezado.Moneda != "MXN")
                        {
                            pago0["TipoCambioP"] = encabezado.TipoCambioP;
                        }
                        if (encabezado.RfcEmisorCtaOrd != string.Empty)
                        {
                            pago0["RfcEmisorCtaOrd"] = encabezado.RfcEmisorCtaOrd;
                        }
                        if ((encabezado.CtaOrdenante != string.Empty) && (encabezado.Formadepago=="03"))
                        {
                            pago0["CtaOrdenante"] = encabezado.CtaOrdenante;
                        }

                        if (encabezado.RfcEmisorCtaBen != string.Empty)
                        {
                            pago0["RfcEmisorCtaBen"] = encabezado.RfcEmisorCtaBen;
                        }
                        if (encabezado.CtaBeneficiario != string.Empty)
                        {
                            pago0["CtaBeneficiario"] = encabezado.CtaBeneficiario;
                        }
                        if (encabezado.NumOperacion != string.Empty)
                        {
                            pago0["NumOperacion"] = encabezado.NumOperacion;
                        }
                        if (encabezado.NomBancoOrdExt != string.Empty)
                        {
                            pago0["NomBancoOrdExt"] = encabezado.NomBancoOrdExt;
                        }

                        if (encabezado.TipoCadPago != string.Empty)
                        {
                            pago0["TipoCadPago"] = encabezado.TipoCadPago;
                            pago0["CertPago"] = encabezado.CertPago;
                            pago0["SelloPago"] = encabezado.SelloPago;
                            pago0["CadPago"] = encabezado.CadenaPago;
                            
                        }



                        MFObject documentorel = new MFObject("DoctoRelacionado");

                        int k = 0;

                        foreach (pagofactura pagof in encabezado.pago)

                        {




                            MFObject documento = new MFObject(k.ToString());

                            documento["IdDocumento"] = pagof.IdDocumento;
                            documento["Serie"] = pagof.Serie;
                            documento["Folio"] = pagof.Folio.ToString();
                            documento["MonedaDR"] = pagof.MonedaDR;
                            documento["MetodoDePagoDR"] = pagof.MetodoDePagoDR;

                            if (pagof.TipoCambioDR != 1 && pagof.TipoCambioDR!=0)
                            {
                                documento["TipoCambioDR"] = pagof.TipoCambioDR.ToString();
                            }
                            documento["NumParcialidad"] = pagof.NumParcialidad.ToString();
                            documento["ImpSaldoAnt"] = Strdecimal(pagof.ImpSaldoAnt);
                            documento["ImpPagado"] = Strdecimal(pagof.ImpPagado);
                            documento["ImpSaldoInsoluto"] = Strdecimal(pagof.ImpSaldoInsoluto);


                            documentorel.AgregaSubnodo(documento);

                          
                            k++;

                        }
                        pago0.AgregaSubnodo(documentorel);
                        nodoencabezado.AgregaSubnodo(pago0);
                        j++;

                    }

                    pagosrelacionados.AgregaSubnodo(nodoencabezado);

                }
            }


            MFObject factura = new MFObject("factura");
            factura["Fecha"] = fecha;

            factura["Serie"] = _serie;
            factura["Folio"] = _folio;
            factura["SubTotal"] = "0";//100.00
            factura["Moneda"] = "XXX";
            factura["Total"] = "0";//100.00

            factura["TipoDeComprobante"] = "P";

            factura["LugarExpedicion"] = Emisora.CP;






            sdk.AgregaObjeto(PAC3());
            sdk.AgregaObjeto(Conf2());
            sdk.AgregaObjeto(factura);
            sdk.AgregaObjeto(emisor);
            sdk.AgregaObjeto(receptor);
            sdk.AgregaObjeto(conceptos);
            sdk.AgregaObjeto(pagosrelacionados);


            // Muestras la estructura
            // sdk.CreaINI(System.Web.HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini"));

            return sdk;
        }

        //public void EscribeEnArchivo(string contenido, string rutaArchivo, bool sobrescribir = true)
        //{
        //    StreamWriter sw = new StreamWriter(rutaArchivo, !sobrescribir);
        //    //    sw.Write(contenido);
        //    string[] lineas = contenido.Split((char)13);
        //    for (int j = 0; j < lineas.Length; j++)
        //    {
        //        string linea = lineas[j];
        //        sw.WriteLine(linea);
        //    }
        //    sw.Close();

        //    //using (FileStream fs = File.Create(rutaArchivo))
        //    //{
        //    //    Byte[] info = new UTF8Encoding(true).GetBytes(contenido);
        //    //    fs.Write(info, 0, info.Length);
        //    //    fs.Close();
        //    //}
        //}

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


        //public WSmultifacturas.RespuestaWS timbrar(string Archivoenb64)
        //{
        //    WSmultifacturas.RespuestaWS respuesta = new WSmultifacturas.RespuestaWS();
        //    try
        //    {
        //        //string ini = Convert.ToBase64String(plainTextBytes);
        //        if (timbrarpueba)
        //        {
        //            string cer = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/demo.cer")));
        //            string key = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/demo.key")));
        //            WSmultifacturas.TimbradoRemotodeINI ws = new WSmultifacturas.TimbradoRemotodeINI();

        //            string pas = "12345678a";
        //            respuesta = ws.timbrarini1("LAN7008173R5", Archivoenb64, cer, key, pas);
        //        }
        //        else
        //        {
        //            string cer = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelcertificado)));
        //            string key = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelkey)));

        //            WSmultifacturas.TimbradoRemotodeINI ws = new WSmultifacturas.TimbradoRemotodeINI();
        //            try
        //            {

        //                respuesta = ws.timbrarini1(Emisora.RFC, Archivoenb64, cer, key, certificado.PassCertificado);
        //            }
        //            catch (Exception err)
        //            {
        //                return respuesta;
        //            }
        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        string mensaje = err.Message;
        //    }
        //    return respuesta;
        //}



        //public string PAC()
        //{
        //    StringBuilder pac = new StringBuilder();
        //    pac.Append("[PAC]");
        //    if (timbrarpueba)
        //    {

        //        pac.Append("usuario=DEMO700101XXX");
        //        pac.Append("pass=DEMO700101XXX");
        //        pac.Append("produccion=NO");
        //    }
        //    else
        //    {

        //        pac.Append("usuario=" + certificado.UsuarioMultifacturas);
        //        pac.Append("pass=" + certificado.PassMultifacturas);
        //        pac.Append("produccion=SI");
        //    }
        //    return pac.ToString();
        //}

        //public string PAC2()
        //{
        //    StringBuilder pac = new StringBuilder();
        //    pac.Append("[PAC]");
        //    if (timbrarpueba)
        //    {
        //        pac.Append("usuario=DEMO700101XXX");
        //        pac.Append("pass=DEMO700101XXX");
        //    }
        //    else
        //    {
        //        pac.Append("usuario=" + certificado.UsuarioMultifacturas);
        //        pac.Append("pass= " + certificado.PassMultifacturas);


        //    }
        //    return pac.ToString();
        //}

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

        public MFObject PACcancelacion()
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

        //public String Conf()
        //{

        //    StringBuilder conf = new StringBuilder();
        //    conf.Append("[conf]" + center);
        //    if (timbrarpueba)
        //    {
        //        conf.Append("cer=" + HttpContext.Current.Server.MapPath("~/CertificadosClientes/demo.cer") + center);
        //        conf.Append("key=" + HttpContext.Current.Server.MapPath("~/CertificadosClientes/demo.key") + center);
        //        conf.Append("pass=12345678a" + center);
        //    }
        //    else
        //    {
        //        conf.Append("cer=" + HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelcertificado) + center);
        //        conf.Append("key=" + HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelkey) + center);
        //        conf.Append("pass=" + certificado.PassCertificado + center);
        //    }
        //    return conf.ToString();
        //}

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
            SDKRespuesta respuesta = sdk.Timbrar(HttpContext.Current.Server.MapPath("~/sdk2/timbrar32.bat"), HttpContext.Current.Server.MapPath("~/sdk2/timbrados/"), "factura", false);
            return respuesta;
        }
        public SDKRespuesta consulta_saldo()
        {
            MFSDK sdk = new MFSDK();
            sdk.Iniciales.Add("SALDO", "SI");
            sdk.AgregaObjeto(PAC3());
            SDKRespuesta respuesta = sdk.Timbrar(HttpContext.Current.Server.MapPath("~/sdk2/timbrar32.bat"), HttpContext.Current.Server.MapPath("~/sdk2/timbrados/"), "factura", false);
            return respuesta;
        }
        public SDKRespuesta consultaEstadoFactura(string rutaXML)
        {
            MFSDK sdk = new MFSDK();
            sdk.Iniciales.Add("modulo", "consultarestatuscfdi");
            sdk.Iniciales.Add("factura_xml", rutaXML);
            sdk.AgregaObjeto(PAC3());
            SDKRespuesta respuesta = sdk.Timbrar(HttpContext.Current.Server.MapPath("~/sdk2/timbrar32.bat"), HttpContext.Current.Server.MapPath("~/sdk2/timbrados/"), "factura", false);
            return respuesta;
        }
        public SDKRespuesta Cancelarxsdk(string cadenaxml)
        {
            MFSDK sdk = new MFSDK();
            sdk.Iniciales.Add("cfdi", cadenaxml);
            sdk.Iniciales.Add("cancelar", "SI");
            sdk.AgregaObjeto(PAC3());
            sdk.AgregaObjeto(Conf2());
            SDKRespuesta respuesta = timbrar(sdk);
            return respuesta;


        }

        public SDKRespuesta Cancelarxsdk( string uuid, string rfc)
        {

            /***************************************cambios 2018 *************************************/
            MFSDK sdk = new MFSDK();
            sdk.Iniciales.Add("modulo", "cancelacion2018");
            sdk.Iniciales.Add("accion", "cancelar");


            // if (timbrarpueba)
            //{
            //    sdk.Iniciales.Add("b64Cer" , Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/demo.cer"))));
            //    sdk.Iniciales.Add("b64Key" , Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/demo.key"))));
            //    sdk.Iniciales.Add("password", "12345678a");
            //}
            //else
            //{
            //    sdk.Iniciales.Add("b64Cer", Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelcertificado))));
            //    sdk.Iniciales.Add("b64Key", Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelkey))));
            //    sdk.Iniciales.Add("password", certificado.PassCertificado);
            //}

            if (timbrarpueba)
            {
                sdk.Iniciales.Add("b64Cer", HttpContext.Current.Server.MapPath("~/CertificadosClientes/demo.cer.pem"));
                sdk.Iniciales.Add("b64Key", HttpContext.Current.Server.MapPath("~/CertificadosClientes/demo.key.pem"));
                sdk.Iniciales.Add("password", "12345678a");
            }
            else
            {
                sdk.Iniciales.Add("b64Cer", HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelcertificado)+".pem");
                sdk.Iniciales.Add("b64Key", HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelkey)+".pem");
                sdk.Iniciales.Add("password", certificado.PassCertificado);
            }

            sdk.Iniciales.Add("uuid", uuid);

            sdk.Iniciales.Add("rfc", rfc);
         
            sdk.AgregaObjeto(this.PACcancelacion());
 
            SDKRespuesta respuesta = timbrar(sdk);
            return respuesta;
        }
        //public bool cancelar(string cadenaxml)
        //{
        //    StringBuilder Iniciales = new StringBuilder();
        //    System.IO.File.WriteAllText(HttpContext.Current.Server.MapPath("~/CertificadosClientes/facturaacancelar.xml"), cadenaxml);

        //    Iniciales.Append("cfdi=" + HttpContext.Current.Server.MapPath("~/CertificadosClientes/facturaacancelar.xml"));
        //    Iniciales.Append("cancelar=SI");
        //    Iniciales.Append(PAC());
        //    Iniciales.Append(Conf());

        //    string strini = Iniciales.ToString();
        //    EscribeEnArchivo(strini, HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini"), true);
        //    string ini = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini")));
        //    WSmultifacturas.RespuestaWS respuesta = timbrar(ini);

        //    //SDKRespuesta respuesta = timbrar(sdk);
        //    if (respuesta.codigo_mf_texto == "OK")
        //    {
        //        //MessageBox.Show("La factura fue cancelada ante el sat");
        //        return true;
        //    }
        //    else
        //    {
        //        //MessageBox.Show(respuesta.Codigo_MF_Texto);
        //        return false;
        //    }
        //    return true;
        //}

        class RootObject
        {
            public Data data { get; set; }
            public string status { get; set; }

        }
        public class Uuid
        {
            [JsonProperty("uuid")]
            public string uuid { get; set; }
        }

        public class Data
        {
            public string acuse { get; set; }
            public Uuid uuid { get; set; }
        }

        public string Status(string resultado)
        {

            var json = resultado;
            string variable = "";
            var obj = JsonConvert.DeserializeObject<RootObject>(json);


            variable = obj.data.acuse;



            return variable;
        }
        public bool cancela40(string uuid, string uuidsusti, string motivo, int IDFactura, int UserID, string viene)
        {
            MotivoCancelacionContext db = new MotivoCancelacionContext();

            string usuario = "DEMO700101XXX";
            string pass = "DEMO700101XXX";
            string rfc = "LAN7008173R5";

            string cer = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/demo.cer")));
            string key = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/demo.key")));
            string passcer = "12345678a";


            if (!timbrarpueba)
            {
                usuario = certificado.UsuarioMultifacturas;
                pass = certificado.PassMultifacturas;
                cer = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelcertificado)));
                key = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelkey)));
                //passcer = Convert.ToBase64String(Encoding.UTF8.GetBytes(certificado.PassCertificado));
                passcer = certificado.PassCertificado;
                rfc = Emisora.RFC;
            }



            WScancela.Cancelarcfdi40SAT cliente = new WScancela.Cancelarcfdi40SAT();
            WScancela.datos datos = new WScancela.datos();

            datos.accion = "cancelar";
            datos.b64Cer = cer;
            datos.b64Key = key;
            datos.motivo = motivo;
            datos.pass = pass;
            datos.password = passcer;
            datos.produccion = "SI";
            datos.usuario = usuario;
            datos.uuid = uuid;
            datos.folioSustitucion = uuidsusti;
            datos.rfc = rfc;

            WScancela.respuesta respuesta = cliente.cancelarCfdi(datos);
            string acuse = "";
            try
            {


                if (respuesta.codigo_mf_texto.Contains("OK"))
                {

                    acuse = Status(respuesta.mensaje_original);
                    acuse = acuse.Replace("'", "''");
                    if (viene == "Factura")
                    {
                        string insert = "insert into EstadoFacturasSat (Estado, IDFactura,Fecha,Usuario) values " +
                       "('C'," + IDFactura + ",sysdatetime()," + UserID + ")";

                        db.Database.ExecuteSqlCommand(insert);

                        string insert2 = "insert into AcuseCancelacionF (IDFactura, Acuse,Fecha) values " +
                      "(" + IDFactura + ",'" + acuse + "',sysdatetime())";

                        db.Database.ExecuteSqlCommand(insert2);
                    }
                    else
                    {
                        string insert = "insert into EstadoFacturasPagosSAT (Estado, IDFactura,Fecha,Usuario) values " +
                      "('C'," + IDFactura + ",sysdatetime()," + UserID + ")";

                        db.Database.ExecuteSqlCommand(insert);
                        string insert2 = "insert into AcuseCancelacionP (IDFactura, Acuse,Fecha) values " +
                    "(" + IDFactura + ",'" + acuse + "',sysdatetime())";

                        db.Database.ExecuteSqlCommand(insert2);
                    }
                    //  MessageBox.Show("La factura fue cancelada ante el sat");
                    return true;
                }
                else
                {
                    //MessageBox.Show(respuesta.Codigo_MF_Texto);
                    return false;
                }
            }
            catch (Exception err)
            {
                if (err.Message.Contains("UUID CANCELADO CORRECTAMENTE"))
                {


                    if (viene == "Factura")
                    {
                        string insert = "insert into EstadoFacturasSat (Estado, IDFactura,Fecha,Usuario) values " +
                       "('C'," + IDFactura + ",sysdatetime()," + UserID + ")";

                        db.Database.ExecuteSqlCommand(insert);
                        string insert2 = "insert into AcuseCancelacionF (IDFactura, Acuse,Fecha) values " +
                    "(" + IDFactura + ",'" + acuse + "',sysdatetime())";

                        db.Database.ExecuteSqlCommand(insert2);
                    }
                    else
                    {
                        string insert = "insert into EstadoFacturasPagosSAT (Estado, IDFactura,Fecha,Usuario) values " +
                      "('C'," + IDFactura + ",sysdatetime()," + UserID + ")";

                        db.Database.ExecuteSqlCommand(insert);
                        string insert2 = "insert into AcuseCancelacionP (IDFactura, Acuse,Fecha) values " +
                    "(" + IDFactura + ",'" + acuse + "',sysdatetime())";

                        db.Database.ExecuteSqlCommand(insert2);
                    }
                    return true;
                }
                else
                { return false; }
            }

            //try
            //{

            //    WScancela.respuesta respuesta = cliente.cancelarCfdi(datos);

            //    if (respuesta.codigo_mf_texto.Contains("OK"))
            //    {


            //        if (viene == "Factura")
            //        {
            //            string insert = "insert into EstadoFacturasSat (Estado, IDFactura,Fecha,Usuario) values " +
            //           "('C'," + IDFactura + ",sysdatetime()," + UserID + ")";

            //            db.Database.ExecuteSqlCommand(insert);
            //        }
            //        else
            //        {
            //            string insert = "insert into EstadoFacturasPagosSAT (Estado, IDFactura,Fecha,Usuario) values " +
            //          "('C'," + IDFactura + ",sysdatetime()," + UserID + ")";

            //            db.Database.ExecuteSqlCommand(insert);
            //        }
            //        //  MessageBox.Show("La factura fue cancelada ante el sat");
            //        return true;
            //    }
            //    else
            //    {
            //        //MessageBox.Show(respuesta.Codigo_MF_Texto);
            //        return false;
            //    }
            //}
            //catch (Exception err)
            //{
            //    if (err.Message.Contains("UUID CANCELADO CORRECTAMENTE"))
            //    {


            //        if (viene == "Factura")
            //        {
            //            string insert = "insert into EstadoFacturasSat (Estado, IDFactura,Fecha,Usuario) values " +
            //           "('C'," + IDFactura + ",sysdatetime()," + UserID + ")";

            //            db.Database.ExecuteSqlCommand(insert);
            //        }
            //        else
            //        {
            //            string insert = "insert into EstadoFacturasPagosSAT (Estado, IDFactura,Fecha,Usuario) values " +
            //          "('C'," + IDFactura + ",sysdatetime()," + UserID + ")";

            //            db.Database.ExecuteSqlCommand(insert);
            //        }
            //        return true;
            //    }
            //    else
            //    { return false; }
            //}
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
              
            
        //        passcer = Convert.ToBase64String(Encoding.ASCII.GetBytes(certificado.PassCertificado));
        //        rfc = Emisora.RFC;
        //    }

        // //   xml = Convert.ToBase64String(Encoding.ASCII.GetBytes(xml));
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




        //            try
        //            {
        //                Random rnd = new Random();
        //                int rndx = rnd.Next(0, 1000);


        //                var folder = System.Web.HttpContext.Current.Server.MapPath("~/RespuestaCancela/Respuesta factura" + rndx);
        //                if (!Directory.Exists(folder))
        //                {
        //                    Directory.CreateDirectory(folder);

        //                    string filePath = System.IO.Path.Combine(folder, "fichero.txt");

        //                    using (StreamWriter writer = File.CreateText(filePath))
        //                    {
        //                        writer.WriteLine(respuesta);

        //                    }

        //                }
        //            } catch (Exception err) { }


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
        //            if (err.Message.Contains("No fue posible cancelar el CFDI, intentolo mas tarde"))
        //            {
        //                return false;
        //            }
        //            else
        //            {


        //                try
        //                {
        //                    Random rnd = new Random();
        //                    int rndx = rnd.Next(0, 1000);


        //                    var folder = System.Web.HttpContext.Current.Server.MapPath("~/RespuestaCancela/error factura" + rndx);
        //                    if (!Directory.Exists(folder))
        //                    {
        //                        Directory.CreateDirectory(folder);

        //                        string filePath = System.IO.Path.Combine(folder, "fichero.txt");

        //                        using (StreamWriter writer = File.CreateText(filePath))
        //                        {
        //                            writer.WriteLine(err);

        //                        }

        //                    }
        //                }
        //                catch (Exception erroe) { }


        //                return false; }
        //        }
        //    }






        //}

        #endregion

    }


    public class pagofactura
    {
        public string IdDocumento { get; set; }
        public string Serie { get; set; }
        public int Folio { get; set; }
        public string MonedaDR { get; set; }
        public string MetodoDePagoDR { get; set; }
        public int NumParcialidad { get; set; }
        public decimal ImpSaldoAnt { get; set; }
        public decimal ImpPagado { get; set; }
        public decimal ImpSaldoInsoluto { get; set; }
        public decimal TipoCambioDR { get; set; }
    } 

    public class Encpagofactura
    {
        public string FechaPago;
        public string Formadepago;

        public string Moneda;

        public decimal Monto;

        public string TipoCambioP = string.Empty;

        public string RfcEmisorCtaOrd = string.Empty;

        public string CtaOrdenante = string.Empty;

        public string RfcEmisorCtaBen = string.Empty;

        public string CtaBeneficiario = string.Empty;


        public string NumOperacion = string.Empty;

        public string TipoCadPago = string.Empty;

        public string CertPago = string.Empty;

        public string CadenaPago = string.Empty;

        public string SelloPago = string.Empty;

        public string NomBancoOrdExt = string.Empty;

        public List<pagofactura> pago = new List<pagofactura>();

    }

 

    public class Encpagosafacturas
    {
        public List<Encpagofactura> encabezados = new List<Encpagofactura>();
        public Encpagosafacturas()
        {
            encabezados = new List<Encpagofactura>();
        }
    }
    public class Concepto
    {
        public string ClaveProdServ;
        public string NoIdentificacion;
        public decimal Cantidad;
        public string Unidad;
        public string ClaveUnidad;
        public string Descripcion;
        public decimal ValorUnitario;
        public decimal Importe;
        public decimal Descuento;
        public bool llevaiva = true;
    }

    public class Conceptos
    {
        public List<Concepto> conceptos = new List<Concepto>();
        public Conceptos()
        {
            conceptos = new List<Concepto>();
        }
    }

    public class Cfdirelacionados
    {
        public string relacion;
        public List<string> uuid = new List<string>();
    }


    public class EjemploFactura
    {
        ClsFactura facturaejemplo = new ClsFactura();

        public EjemploFactura()
        {
            facturaejemplo.Regimen = "601";
            facturaejemplo.timbrarpueba = true;
            facturaejemplo.Tipodecombrobante = "I";
            facturaejemplo.tipodecambio = "1";

            facturaejemplo.metododepago = "PUE";
            facturaejemplo.formadepago = "01";
            facturaejemplo._serie = "A";
            facturaejemplo._folio = "1";

            facturaejemplo.valoriva = 0.16M;

            facturaejemplo.Regimen = "601";

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


            List<Concepto> conceptos = new List<Concepto>();
            Concepto concepto1 = new Concepto();
            concepto1.ClaveProdServ = "43212109";
            concepto1.NoIdentificacion = "1";
            concepto1.Cantidad = 2;
            concepto1.ClaveUnidad = "MIL";
            concepto1.Unidad = "MIL";
            concepto1.Descripcion = "IMPRESORA X";
            concepto1.ValorUnitario = 1000;
            concepto1.Importe = 2000;
            concepto1.llevaiva = true;
            concepto1.Descuento = 0;
            Concepto concepto2 = new Concepto();
            concepto2.ClaveProdServ = "43212109";
            concepto2.NoIdentificacion = "2";
            concepto2.Cantidad = 2;
            concepto2.ClaveUnidad = "MIL";
            concepto2.Unidad = "MIL";
            concepto2.Descripcion = "IMPRESORA X";
            concepto2.ValorUnitario = 1000;
            concepto2.Importe = 2000;
            concepto2.llevaiva = true;
            concepto2.Descuento = 0;

            conceptos.Add(concepto1);
            conceptos.Add(concepto2);

            facturaejemplo.Listaconceptos.conceptos = conceptos;

            //  facturaejemplo.timbrarpueba = true;
            // public Cfdirelacionados cfdirelacionados = null;
            // public Certificados certificado;
        }
        public string timbrarfacturaejemplo()
        {
            MultiFacturasSDK.MFSDK X = facturaejemplo.construirfactura2();
            //  facturaejemplo.EscribeEnArchivo(strini, HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini"), true);
            //   string ini = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini")));
            SDKRespuesta respuesta = facturaejemplo.timbrar(X);
            //WSmultifacturas.RespuestaWS respuesta = facturaejemplo.timbrar(ini);

            return respuesta.Codigo_MF_Texto + " XML ->" + respuesta.CFDI;
        }

    }


    public class EjemploFacturaPagos
    {
        ClsFactura facturaejemplo = new ClsFactura();

        public EjemploFacturaPagos()
        {
            facturaejemplo.Regimen = "601";
            facturaejemplo.timbrarpueba = true;
            facturaejemplo.Tipodecombrobante = "P";
            facturaejemplo.tipodecambio = "1";

            facturaejemplo._serie = "P";
            facturaejemplo._folio = "1";

            facturaejemplo.valoriva = 0.16M;

            facturaejemplo.Regimen = "601";

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


            List<Concepto> conceptos = new List<Concepto>();
            Concepto concepto1 = new Concepto();
            concepto1.ClaveProdServ = "84111506";
            concepto1.NoIdentificacion = "1";
            concepto1.Cantidad = 1;
            concepto1.ClaveUnidad = "ACT";
            concepto1.Unidad = "ACTIVIDAD";
            concepto1.Descripcion = "Pago";
            concepto1.ValorUnitario = 0;
            concepto1.Importe = 0;
            concepto1.llevaiva = true;
            concepto1.Descuento = 0;


            conceptos.Add(concepto1);


            facturaejemplo.Listaconceptos.conceptos = conceptos;

            //  facturaejemplo.timbrarpueba = true;
            // public Cfdirelacionados cfdirelacionados = null;
            // public Certificados certificado;

            Encpagofactura encabezado1 = new Encpagofactura();
            encabezado1.FechaPago = "2017-09-08T10:17:56";
            encabezado1.Formadepago = "03";
            encabezado1.Moneda = "MXN";
            encabezado1.Monto = (Decimal)300.00;
            encabezado1.RfcEmisorCtaOrd = "BNM840515VB1";
            encabezado1.CtaOrdenante = "1234567890";
            encabezado1.RfcEmisorCtaBen = "BNM840515VB1";
            encabezado1.CtaBeneficiario = "1234567890";

            pagofactura pago1 = new pagofactura();
            pago1.IdDocumento = "710bfe66-6770-4978-8c45-f7c41db598c0".ToUpper();
            pago1.Serie = "A";
            pago1.Folio = 1;
            pago1.MonedaDR = "MXN";
            pago1.MetodoDePagoDR = "PPD";
            pago1.NumParcialidad = 1;
            pago1.ImpSaldoInsoluto = (Decimal)1000;
            pago1.ImpSaldoAnt = (Decimal)1100;
            pago1.ImpPagado = (Decimal)100;



            pagofactura pago2 = new pagofactura();
            pago2.IdDocumento = "f8bd8daf-a1d9-48c5-a3e8-2ec13b04b352".ToUpper();
            pago2.Serie = "A";
            pago2.Folio = 2;
            pago2.MonedaDR = "MXN";
            pago2.MetodoDePagoDR = "PPD";
            pago2.NumParcialidad = 1;
            pago2.ImpSaldoInsoluto = (Decimal)1000;
            pago2.ImpSaldoAnt = (Decimal)1200;
            pago2.ImpPagado = (Decimal)200;

            encabezado1.pago.Add(pago1);
            encabezado1.pago.Add(pago2);

            facturaejemplo.Encabezadosfacturas.encabezados.Add(encabezado1);
          




        }



        public string timbrarfacturaejemplopagos()
        {
            MultiFacturasSDK.MFSDK X = facturaejemplo.construirfacturadepagos();
            //  facturaejemplo.EscribeEnArchivo(strini, HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini"), true);
            //   string ini = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/CertificadosClientes/factura.ini")));
            SDKRespuesta respuesta = facturaejemplo.timbrar(X);
            //WSmultifacturas.RespuestaWS respuesta = facturaejemplo.timbrar(ini);

            return respuesta.Codigo_MF_Texto + " xml->" + respuesta.CFDI;
        }

    }

}

