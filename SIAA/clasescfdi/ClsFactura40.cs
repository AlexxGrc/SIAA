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


namespace SIAAPI.Facturas
{

    public class ClsFactura40
    {

        private char center = (char)13;
        public bool timbrarpueba = false;
        public String Tipodecombrobante = "I";
        public string tipodecambio = "1.0000";
        public Empresa Emisora;
        public Empresa Receptora;
        public string metododepago = "PUE";
        public string condicionesPago = "";
        public string formadepago = "01";
        public String Moneda = "MXN";
        public string uso = "G01";
        public string _serie;
        public string _folio;
        public decimal Descuento = 0;
        public Certificados certificado;
        public Conceptos40 Listaconceptos = new Conceptos40();
        //public DocumentoPDF40 template;
        public decimal valoriva = 0.16M;
        public Cfdirelacionados40 cfdirelacionados = new Cfdirelacionados40();

        public string Regimen;
        public string RegimenFiscalReceptor;
        //public string DomicilioFiscalReceptor;
        public Encpagosafacturas40 Encabezadosfacturas = new Encpagosafacturas40();

        public ClsFactura40()
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
            sdk.Iniciales.Add("version_cfdi", "4.0");
            sdk.Iniciales.Add("MODOINI", "DIVISOR");
            sdk.Iniciales.Add("cfdi", HttpContext.Current.Server.MapPath("~/sdk2/timbrados/cfdi_ejemplo_factura4_2.xml"));
            sdk.Iniciales.Add("xml_debug", HttpContext.Current.Server.MapPath("~/sdk2/timbrados/sin_timbrar_ejemplo_factura4_2.xml"));
            sdk.Iniciales.Add("remueve_acentos", "NO");
            sdk.Iniciales.Add("RESPUESTA_UTF8", "SI");
            sdk.Iniciales.Add("html_a_txt", "NO");




            MFObject emisor = new MFObject("emisor");

            if (timbrarpueba)
            {
                emisor["RegimenFiscal"] = "601";
                emisor["rfc"] = "LAN7008173R5";
                emisor["nombre"] = "CINDEMEX SA DE CV";

            }
            else
            {
                emisor["RegimenFiscal"] = Regimen;
                emisor["rfc"] = Emisora.RFC.Replace("&", "&amp;");
                emisor["nombre"] = Emisora.Nombre40.Trim();


            }

            MFObject receptor = new MFObject("receptor");


            receptor["RegimenFiscalReceptor"] = RegimenFiscalReceptor;
            receptor["DomicilioFiscalReceptor"] = Receptora.CP;
            receptor["UsoCFDI"] = uso;
            receptor["nombre"] = Receptora.RazonSocial;
            receptor["rfc"] = Receptora.RFC;






            MFObject conceptos = new MFObject("conceptos");
            decimal acudescuento = 0;
            decimal acusubtotal = 0;
            decimal acuiva = 0;
            decimal total = 0;


            int i = 0;
            foreach (Concepto40 elemento in Listaconceptos.conceptos)
            {
                //  string clave = DTGdetalles.Rows[iciclo].Cells["clave"].Value.ToString();

                MFObject concepto0 = new MFObject(i.ToString());
                concepto0["ClaveProdServ"] = elemento.ClaveProdServ;
                concepto0["NoIdentificacion"] = elemento.NoIdentificacion;
                concepto0["Cantidad"] = elemento.Cantidad.ToString();
                concepto0["ClaveUnidad"] = elemento.ClaveUnidad.ToString();
                concepto0["Descripcion="] = elemento.Descripcion;
                concepto0["ValorUnitario"] = Math.Round(elemento.ValorUnitario, 2).ToString();
                concepto0["Importe"] = Math.Round(elemento.Importe, 2).ToString();
                concepto0["ObjetoImp"] = "02";

              

                if (elemento.Descuento > 0)
                {

                    concepto0["Descuento"] = Math.Round(elemento.Descuento, 2).ToString();
                }



                MFObject impuestosporconcepto = new MFObject("Impuestos");

                MFObject itraslados = new MFObject("Traslados");
                MFObject itraslado0 = new MFObject(i.ToString());

                Decimal IVA = Math.Round(Math.Round((elemento.Importe - elemento.Descuento), 2) * valoriva, 2);
                if (elemento.llevaiva)
                {
                    acuiva += IVA;
                    itraslado0["TasaOCuota"] = "0.160000";
                    itraslado0["Importe"] = IVA.ToString();

                    itraslado0["TipoFactor"] = "Tasa";
                }
                else
                {

                    itraslado0["TipoFactor"] = "Exento";
                }

                itraslado0["Base"] = Math.Round((elemento.Importe - elemento.Descuento), 2).ToString();
                itraslado0["Impuesto"] = "002";


                itraslados.AgregaSubnodo(itraslado0);
                impuestosporconcepto.AgregaSubnodo(itraslados);
                concepto0.AgregaSubnodo(impuestosporconcepto);


                conceptos.AgregaSubnodo(concepto0);
                acusubtotal = acusubtotal + Math.Round(elemento.Importe, 2);
                acudescuento = acudescuento + Math.Round(elemento.Descuento, 2);
                i++;
            }



            total = acusubtotal - acudescuento + acuiva;
            MFObject impuestos = new MFObject("impuestos");
            impuestos["TotalImpuestosTrasladados"] = Math.Round(acuiva, 2).ToString();
            // Traslados
            MFObject itras = new MFObject("translados");
            MFObject itra0 = new MFObject("0");

            itra0["Impuesto"] = "002";
            itra0["Base"] = Math.Round(acusubtotal, 2).ToString();



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


            if (metododepago == "PPD")
            {
                formadepago = "99";
            }
            factura["forma_pago"] = formadepago;

            factura["metodo_pago"] = metododepago;
            factura["folio"] = _folio;
            factura["LugarExpedicion"] = Emisora.CP;
            factura["serie"] = _serie;
            factura["fecha_expedicion"] = DateTime.Now.ToString("s");
            //factura["metodo_pago"] = metododepago;
            factura["condicionesDePago"] = "";
            factura["tipocomprobante"] = Tipodecombrobante;
            factura["moneda"] = Moneda;

            factura["tipocambio"] = tipodecambio;
            factura["Exportacion"] = "01";
            //factura["RegimenFiscal"] = Regimen; /// cambiar por el reginme
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
            foreach (Concepto40 elemento in Listaconceptos.conceptos)
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
            MFObject nodoencabezado = new MFObject("Pagos");

            if (Encabezadosfacturas != null)
            {
                if (Encabezadosfacturas.encabezados.Count > 0)
                {




                    int j = 0;

                    foreach (Encpagofactura40 encabezado in Encabezadosfacturas.encabezados)
                    {
                        //Se pueden agregar varios relacionados con UUID
                        MFObject pago0 = new MFObject(j.ToString());

                        pago0["FechaPago"] = encabezado.FechaPago;
                        pago0["FormaDePagoP"] = encabezado.Formadepago;
                        pago0["MonedaP"] = encabezado.Moneda;
                        pago0["Monto"] = Strdecimal(encabezado.Monto);
                        if (encabezado.Moneda != "MXN")
                        {
                            pago0["TipoCambioP"] = encabezado.TipoCambioP;
                        }
                        if (encabezado.RfcEmisorCtaOrd != string.Empty)
                        {
                            pago0["RfcEmisorCtaOrd"] = encabezado.RfcEmisorCtaOrd;
                        }
                        if ((encabezado.CtaOrdenante != string.Empty) && (encabezado.Formadepago == "03"))
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

                        foreach (pagofactura40 pagof in encabezado.pago)

                        {




                            MFObject documento = new MFObject(k.ToString());

                            documento["IdDocumento"] = pagof.IdDocumento;
                            documento["Serie"] = pagof.Serie;
                            documento["Folio"] = pagof.Folio.ToString();
                            documento["MonedaDR"] = pagof.MonedaDR;
                            documento["MetodoDePagoDR"] = pagof.MetodoDePagoDR;

                            if (pagof.TipoCambioDR != 1 && pagof.TipoCambioDR != 0)
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
            SDKRespuesta respuesta = sdk.Timbrar(HttpContext.Current.Server.MapPath("~/sdk24/timbrar32.bat"), HttpContext.Current.Server.MapPath("~/sdk24/timbrados/"), "factura", false);

            try
            {
                bool pasa = false;
                if (respuesta.Codigo_MF_Texto.Contains("OK"))
                {
                    pasa = true;

                   
                }

                if (!pasa)
                {
                    throw new Exception("RESPUESTA SAT " + respuesta.Codigo_MF_Texto);
                   
                }
            }
            catch (Exception err)
            {

            }

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

        public SDKRespuesta Cancelarxsdk(string uuid, string rfc)
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
                sdk.Iniciales.Add("b64Cer", HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelcertificado) + ".pem");
                sdk.Iniciales.Add("b64Key", HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certificado.Nombredelkey) + ".pem");
                sdk.Iniciales.Add("password", certificado.PassCertificado);
            }

            sdk.Iniciales.Add("uuid", uuid);

            sdk.Iniciales.Add("rfc", rfc);

            sdk.AgregaObjeto(this.PACcancelacion());

            SDKRespuesta respuesta = timbrar(sdk);
            return respuesta;
        }


        #endregion

    }


    public class pagofactura40
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

    public class Encpagofactura40
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

        public List<pagofactura40> pago = new List<pagofactura40>();

    }



    public class Encpagosafacturas40
    {
        public List<Encpagofactura40> encabezados = new List<Encpagofactura40>();
        public Encpagosafacturas40()
        {
            encabezados = new List<Encpagofactura40>();
        }
    }
    public class Concepto40
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

    public class Conceptos40
    {
        public List<Concepto40> conceptos = new List<Concepto40>();
        public Conceptos40()
        {
            conceptos = new List<Concepto40>();
        }
    }

    public class Cfdirelacionados40
    {
        public string relacion;
        public List<string> uuid = new List<string>();
    }






}

