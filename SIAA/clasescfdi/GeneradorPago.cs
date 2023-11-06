using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.Reportes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SIAAPI.clasescfdi
{
    public class PDFPago
    {
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;
       //Objeto que contendra la información del documento pdf
        public ClsXmlPagos documento;
        public string nombreDocumento = string.Empty;
        public PDFPago(string rutaXML, System.Drawing.Image logo, int folio,string Telefono = "")
        {

            documento = new ClsXmlPagos(rutaXML);
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Complementopago"+ folio + ".pdf");
            _documento = new Document(PageSize.LETTER);
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            //string nombreDocumento = rutaPDF;

            _writer = PdfWriter.GetInstance(_documento, new FileStream(nombreDocumento, System.IO.FileMode.Create));
            _writer.PageEvent = new ITextEvents();

            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();

            AgregarLogo(logo);
            AgregarCuadro();
            AgregarDatosEmisor(Telefono);
            AgregarDatosFactura();
            AgregarDatosReceptor();
            AgregarDatosProductos();
            AgregarDatosPagos();
            AgregarTotales();
            AgregarSellos();

            //Cerramoe el documento
            _documento.Close();

            //HttpContext.Current.Response.ContentType = "pdf/application";
            //HttpContext.Current.Response.AddHeader("content-disposition", "attachment;" +
            //        "filename=Factura" + documento.Comprobante.Serie + documento.Comprobante.Folio + ".pdf");
            //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //HttpContext.Current.Response.Write(_documento);
            //HttpContext.Current.Response.End();

        }


        public PDFPago(string rutaXML, System.Drawing.Image logo, string folio, string Telefono = "")
        {

            documento = new ClsXmlPagos(rutaXML);
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Complementopago" + folio + ".pdf");
            _documento = new Document(PageSize.LETTER);
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            //string nombreDocumento = rutaPDF;

            _writer = PdfWriter.GetInstance(_documento, new FileStream(nombreDocumento, System.IO.FileMode.Create));
            _writer.PageEvent = new ITextEvents();

            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();

            AgregarLogo(logo);
            AgregarCuadro();
            AgregarDatosEmisor(Telefono);
            AgregarDatosFactura();
            AgregarDatosReceptor();
            AgregarDatosProductos();
            AgregarDatosPagos();
            AgregarTotales();
            AgregarSellos();

            //Cerramoe el documento
            _documento.Close();

            //HttpContext.Current.Response.ContentType = "pdf/application";
            //HttpContext.Current.Response.AddHeader("content-disposition", "attachment;" +
            //        "filename=Factura" + documento.Comprobante.Serie + documento.Comprobante.Folio + ".pdf");
            //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //HttpContext.Current.Response.Write(_documento);
            //HttpContext.Current.Response.End();

        }


        private void AgregarDatosEmisor(String Telefono)
        {
            //Datos del emisor
            Paragraph p1 = new Paragraph();
            p1.IndentationLeft = 150f;

            p1.Leading = 9;
            p1.Add(new Phrase("DATOS DEL EMISOR", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            p1.Add("\n");
            p1.Add(new Phrase(documento.Comprobante.Emisor.Nombre, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + documento.Comprobante.Emisor.rfc, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + Telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.Add("\n");
              

            p1.Add(new Phrase(DecodificadorSAT.getRegimen(documento.Comprobante.Emisor.RegimenFiscal), new Font(Font.FontFamily.HELVETICA, 6)));
            p1.Add("\n");

            p1.Add(new Phrase("\n\nTipo de Comprobante ".ToUpper() + " :" +  documento.Comprobante.TipoDeComprobante , new Font(Font.FontFamily.HELVETICA, 10)));

            p1.SpacingAfter = -65;

            _documento.Add(p1);
        }

        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {
            if (logoEmpresa == null)
                return;
            //Agrego la imagen al documento
            Image imagen = Image.GetInstance(logoEmpresa, BaseColor.BLACK);
            imagen.ScaleToFit(140, 100);
            imagen.Alignment = Element.ALIGN_TOP;
            Chunk logo = new Chunk(imagen, 1, -70);
            _documento.Add(logo);
        }

        private void AgregarCuadro()
        {
            CMYKColor Colortitulos;

            Colortitulos = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

            _cb = _writer.DirectContentUnder;
          

            //Agrego cuadro al documento
            _cb.SetColorStroke(Colortitulos); //Color de la linea
            _cb.SetColorFill(Colortitulos); // Color del relleno
            _cb.SetLineWidth(3.5f); //Tamano de la linea
            _cb.Rectangle(378, 694, 20, 100);
            _cb.FillStroke();
        }



        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("PAGOS 3.3");
            _documento.AddSubject("DOCUMENTO CREADO APARTIR DE UN XML");
            _documento.AddTitle("FACTURA DE PAGO");
            _documento.SetMargins(5, 5, 5, 5);
        }
        private void AgregarDatosFactura()
        {
            //Datos de la factura
            Paragraph p2 = new Paragraph();
            p2.IndentationLeft = 403f;
            p2.SpacingAfter = 18;
            p2.Leading = 10;
            p2.Alignment = Element.ALIGN_CENTER;

            p2.Add(new Phrase("COMPLEMENTO RECEPCION ",  new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase(" PAGO: " + documento.Comprobante.Serie + " " + documento.Comprobante.Folio, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase("FOLIO FISCAL (UUID): ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase(documento.Comprobante.Complemento.TimbreFiscalDigital.UUID.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            p2.Add("\n");
            p2.Add(new Phrase("NO. DE SERIE DEL CERTIFICADO DEL SAT:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase(documento.Comprobante.Complemento.TimbreFiscalDigital.NoCertificadoSAT, new Font(Font.FontFamily.HELVETICA, 8)));
            p2.Add("\n");
          
       
            p2.Add(new Phrase("FECHA Y HORA DE EMISIÓN DE CFDI:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase(documento.Comprobante.Complemento.TimbreFiscalDigital.FechaTimbrado, new Font(Font.FontFamily.HELVETICA, 8)));
            p2.Add("\n");
            _documento.Add(p2);
        }


        private void AgregarDatosReceptor()
        {
            float[] anchoColumasTablaDatosEmisorReceptor = { 600f };
            PdfPTable tablaDatosEmisorReceptor = new PdfPTable(anchoColumasTablaDatosEmisorReceptor);
            tablaDatosEmisorReceptor.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosEmisorReceptor.SetTotalWidth(anchoColumasTablaDatosEmisorReceptor);
            tablaDatosEmisorReceptor.SpacingBefore = 5;
            tablaDatosEmisorReceptor.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosEmisorReceptor.LockedWidth = true;

            //Datos del receptor
            float[] anchoColumnas = { 100f, 400F };
            PdfPTable tabla = new PdfPTable(anchoColumnas);
            tabla.DefaultCell.Border = Rectangle.NO_BORDER;
            tabla.SetTotalWidth(anchoColumnas);
            tabla.HorizontalAlignment = Element.ALIGN_LEFT;
            tabla.LockedWidth = true;
            tabla.AddCell(new Phrase("RECEPTOR: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tabla.AddCell(new Phrase(documento.Comprobante.Receptor.Nombre.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));

            tabla.AddCell(new Phrase("RFC: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tabla.AddCell(new Phrase(documento.Comprobante.Receptor.rfc.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));


           
           

            tablaDatosEmisorReceptor.AddCell(tabla);
        
            _documento.Add(tablaDatosEmisorReceptor);
        }

       
      
        private void AgregarDatosProductos()
        {
            CMYKColor Colortitulos;

            Colortitulos = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

            float[] anchoColumnasTablaProductos = { 600f };
            PdfPTable tablaProductosPrincipal = new PdfPTable(anchoColumnasTablaProductos);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosPrincipal.SetTotalWidth(anchoColumnasTablaProductos);
            tablaProductosPrincipal.SpacingBefore = 15;
            tablaProductosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosPrincipal.LockedWidth = true;


            //Datos de los productos
            float[] tamanoColumnas = { 60f,  50f, 70f, 285f, 75f, 60f };
            PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);
            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;
            tablaProductosTitulos.DefaultCell.BackgroundColor= Colortitulos;


            tablaProductosTitulos.AddCell(new Phrase("CLAVE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
           
            tablaProductosTitulos.AddCell(new Phrase("CANTIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("CLAVE UNIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("DESCRIPCION", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("VALOR UNITARIO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaProductosTitulos.AddCell(new Phrase("IMPORTE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        



            float[] tamanoColumnasProductos = { 60f,  50f, 70f, 285f, 75f, 60f };
            PdfPTable tablaProductos = new PdfPTable(tamanoColumnas);
            //tablaProductos.SpacingBefore = 1;
            //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductos.DefaultCell.BorderWidthLeft = 0.1f;
            tablaProductos.DefaultCell.BorderWidthRight = 0.0f;
            tablaProductos.DefaultCell.BorderWidthBottom = 0.0f;
            tablaProductos.DefaultCell.BorderWidthTop = 0.0f;
            tablaProductos.SetTotalWidth(tamanoColumnas);
            tablaProductos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductos.LockedWidth = true;
            ProductoPago p = documento.Comprobante.Concepto;
     
                tablaProductos.AddCell(new Phrase(p.ClaveProducto, new Font(Font.FontFamily.HELVETICA, 8)));   
                tablaProductos.AddCell(new Phrase(p.cantidad.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.c_unidad, new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.descripcion, new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdauni = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdauni.Phrase = new Phrase(p.valorUnitario.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdauni);
                PdfPCell celdaimporte = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdaimporte.Phrase = new Phrase(p.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdaimporte);
               


            PdfPCell celdaTitulos = new PdfPCell(tablaProductosTitulos);
            tablaProductosPrincipal.AddCell(celdaTitulos);
            PdfPCell celdaProductos = new PdfPCell(tablaProductos);
            celdaProductos.MinimumHeight = 20;
            tablaProductosPrincipal.AddCell(celdaProductos);
            _documento.Add(tablaProductosPrincipal);
        }


        private void AgregarDatosPagos()
        {
            CMYKColor Colortitulos;

            Colortitulos = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

            foreach (PagoP pago in documento.Comprobante.Complemento.Pagos)
            {
                float[] anchoColumnasTablaencabezado = { 80f, 520f };
                PdfPTable tablaPagosEncabezado = new PdfPTable(anchoColumnasTablaencabezado);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaPagosEncabezado.SetTotalWidth(anchoColumnasTablaencabezado);
                tablaPagosEncabezado.SpacingBefore = 15;
                tablaPagosEncabezado.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaPagosEncabezado.LockedWidth = true;


                tablaPagosEncabezado.AddCell(new Phrase("FECHA: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaPagosEncabezado.AddCell(new Phrase(pago.FechaPago, new Font(Font.FontFamily.HELVETICA, 8)));


                tablaPagosEncabezado.AddCell(new Phrase("MONTO: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaPagosEncabezado.AddCell(new Phrase(pago.Monto.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));


                tablaPagosEncabezado.AddCell(new Phrase("MONEDA: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaPagosEncabezado.AddCell(new Phrase(pago.MonedaP, new Font(Font.FontFamily.HELVETICA, 8)));




                if (pago.RfcEmisorCtaOrd != string.Empty)
                {


                    string Bancoe = DecodificadorSAT.getBanco(documento.Comprobante.Complemento.Pagos[0].RfcEmisorCtaOrd);
                    if (Bancoe != string.Empty)
                    {
                        tablaPagosEncabezado.AddCell(new Phrase("BANCO EMISOR: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                        tablaPagosEncabezado.AddCell(new Phrase(Bancoe, new Font(Font.FontFamily.HELVETICA, 8)));
                    }

                    if (pago.CtaOrdenante != string.Empty)
                    {
                        tablaPagosEncabezado.AddCell(new Phrase("CUENTA: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                        tablaPagosEncabezado.AddCell(new Phrase(pago.CtaOrdenante, new Font(Font.FontFamily.HELVETICA, 8)));
                    }

                    string BancoR = DecodificadorSAT.getBanco(pago.RfcEmisorCtaBen);

                    if (BancoR != string.Empty)
                    {
                        tablaPagosEncabezado.AddCell(new Phrase("BANCO RECEPTOR: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                        tablaPagosEncabezado.AddCell(new Phrase(BancoR, new Font(Font.FontFamily.HELVETICA, 8)));
                    }

                    if (pago.CtaBeneficiario != string.Empty)
                    {
                        tablaPagosEncabezado.AddCell(new Phrase("CUENTA: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                        tablaPagosEncabezado.AddCell(new Phrase(pago.CtaBeneficiario, new Font(Font.FontFamily.HELVETICA, 8)));
                    }

                    if (pago.NumOperacion != string.Empty)
                    {
                        tablaPagosEncabezado.AddCell(new Phrase("NUMERO DE OPERACION: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                        tablaPagosEncabezado.AddCell(new Phrase(pago.NumOperacion, new Font(Font.FontFamily.HELVETICA, 8)));
                    }


                    if (pago.TipoCadPago == "01")
                    {
                        tablaPagosEncabezado.AddCell(new Phrase("TIPO DE CADENA: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                        tablaPagosEncabezado.AddCell(new Phrase("01 SPEI", new Font(Font.FontFamily.HELVETICA, 8)));

                        tablaPagosEncabezado.AddCell(new Phrase("CADENA: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                        tablaPagosEncabezado.AddCell(new Phrase(pago.CadenaPago, new Font(Font.FontFamily.HELVETICA, 8)));

                        tablaPagosEncabezado.AddCell(new Phrase("SELLO: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                        tablaPagosEncabezado.AddCell(new Phrase(pago.SelloPago, new Font(Font.FontFamily.HELVETICA, 8)));

                        tablaPagosEncabezado.AddCell(new Phrase("CERTIFICADO: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                        tablaPagosEncabezado.AddCell(new Phrase(pago.CertPago, new Font(Font.FontFamily.HELVETICA, 8)));


                    }

                }


                float[] anchoColumnasTablaProductos = { 600f };
                PdfPTable tablaPagosPrincipal = new PdfPTable(anchoColumnasTablaProductos);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaPagosPrincipal.SetTotalWidth(anchoColumnasTablaProductos);
                tablaPagosPrincipal.SpacingBefore = 15;
                tablaPagosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaPagosPrincipal.LockedWidth = true;


                

                //Datos de los productos
                float[] tamanoColumnas = { 80f, 160f, 80f, 80f, 80f, 120f };
                PdfPTable tablaPagosTitulos = new PdfPTable(tamanoColumnas);
                //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaPagosTitulos.SetTotalWidth(tamanoColumnas);
                tablaPagosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaPagosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaPagosTitulos.LockedWidth = true;
                tablaPagosTitulos.DefaultCell.BackgroundColor = Colortitulos;




                tablaPagosTitulos.AddCell(new Phrase("PARCIALIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaPagosTitulos.AddCell(new Phrase("UUID", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

                tablaPagosTitulos.AddCell(new Phrase("FACTURA", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaPagosTitulos.AddCell(new Phrase("MONEDA", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaPagosTitulos.AddCell(new Phrase("METODO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

                tablaPagosTitulos.AddCell(new Phrase("IMPORTE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));


                PdfPCell celdaencabezado = new PdfPCell(tablaPagosEncabezado);
                tablaPagosPrincipal.AddCell(celdaencabezado);
                PdfPCell celdaTitulos = new PdfPCell(tablaPagosTitulos);
                tablaPagosPrincipal.AddCell(celdaTitulos);

                _documento.Add(tablaPagosPrincipal);
                foreach (DocumentoRelacionadoPago item in pago.documentoRelacionado)
                {



                    PdfPTable tablaPagos = new PdfPTable(tamanoColumnas);
                    //tablaProductos.SpacingBefore = 1;
                    //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
                    tablaPagos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaPagos.DefaultCell.BorderWidthLeft = 0.1f;
                    tablaPagos.DefaultCell.BorderWidthRight = 0.0f;
                    tablaPagos.DefaultCell.BorderWidthBottom = 0.0f;
                    tablaPagos.DefaultCell.BorderWidthTop = 0.0f;
                    tablaPagos.SetTotalWidth(tamanoColumnas);
                    tablaPagos.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaPagos.LockedWidth = true;

                    tablaPagos.AddCell(new Phrase(item.NumParcialidad, new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaPagos.AddCell(new Phrase(item.IdDocumento, new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaPagos.AddCell(new Phrase(item.Folio, new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaPagos.AddCell(new Phrase(item.MonedaDR, new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaPagos.AddCell(new Phrase(item.MetodoDePagoDR, new Font(Font.FontFamily.HELVETICA, 8)));

                    PdfPCell celdaimporte = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                    celdaimporte.Phrase = new Phrase(item.ImpPagado.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                    tablaPagos.AddCell(celdaimporte);

                    _documento.Add(tablaPagos);

                }

                //PdfPCell celdaPagos = new PdfPCell(tablaPagos);
                //celdaPagos.MinimumHeight = pago.documentoRelacionado.Count * 20;
                //tablaPagosPrincipal.AddCell(celdaPagos);


                
            }

           
         
          
        }

        private void AgregarTotales()
        {

            float[] anchoColumasTablaTotales = { 420f, 180f };
            PdfPTable tablaTotales = new PdfPTable(anchoColumasTablaTotales);
            tablaTotales.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaTotales.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaTotales.SetTotalWidth(anchoColumasTablaTotales);
            tablaTotales.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaTotales.LockedWidth = true;

            float[] anchoColumnas = { 110, 70f };
            PdfPTable tablaImportes = new PdfPTable(anchoColumnas);
            tablaImportes.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaImportes.SetTotalWidth(anchoColumnas);
            tablaImportes.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            //tablaImportes.HorizontalAlignment = Element.ALIGN_RIGHT;
            tablaImportes.LockedWidth = true;
            


            tablaImportes.AddCell(new Phrase("Total:".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaImportes.AddCell(new Phrase(float.Parse(0.ToString()).ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));

            //tablaTotales.AddCell(new Phrase("IMPORTE CON LETRA: " + _templatePDF.totalEnLetra.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            tablaTotales.AddCell(tablaImportes);
            _documento.Add(tablaTotales);
        }

        private void AgregarSellos()
        {
            StringBuilder cadenaOriginal = new StringBuilder();
            cadenaOriginal.Append("||");
            cadenaOriginal.Append("1.0|");
            cadenaOriginal.Append(documento.Comprobante.Complemento.TimbreFiscalDigital.UUID + "|");
            cadenaOriginal.Append(documento.Comprobante.Complemento.TimbreFiscalDigital.FechaTimbrado+ "|");
            cadenaOriginal.Append(documento.Comprobante.Complemento.TimbreFiscalDigital.SelloSAT + "|");
        

            float[] anchoColumnas = { 500f, 100f };
            PdfPTable tablaSellosQR = new PdfPTable(anchoColumnas);
            tablaSellosQR.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaSellosQR.SpacingBefore = 10.0f;
            tablaSellosQR.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaSellosQR.SetTotalWidth(anchoColumnas);
            //tablaSellosQR.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaSellosQR.LockedWidth = true;

            float[] anchoColumnas1 = { 500f };
            PdfPTable tablaSellos = new PdfPTable(anchoColumnas1);
            tablaSellos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaSellos.DefaultCell.VerticalAlignment = Element.ALIGN_TOP;
            tablaSellos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaSellos.SetTotalWidth(anchoColumnas1);
            tablaSellos.HorizontalAlignment = Element.ALIGN_CENTER;
            //tablaSellos.LockedWidth = true;
            tablaSellos.AddCell(new Phrase("SELLO DIGITAL DEL CFDI:", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            tablaSellos.AddCell(new Phrase(documento.Comprobante.Complemento.TimbreFiscalDigital.SelloCFD, new Font(Font.FontFamily.HELVETICA, 4)));
            tablaSellos.AddCell(new Phrase("SELLO DIGITAL DEL SAT", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            tablaSellos.AddCell(new Phrase(documento.Comprobante.Complemento.TimbreFiscalDigital.SelloSAT, new Font(Font.FontFamily.HELVETICA, 4)));
            tablaSellos.AddCell(new Phrase("CADENA ORIGINAL DEL COMPLEMENTO DE CERTIFICACIÓN DIGITAL DEL SAT:", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            tablaSellos.AddCell(new Phrase(cadenaOriginal.ToString(), new Font(Font.FontFamily.HELVETICA, 4)));

            //Agregamos el codigo QR al documento
            StringBuilder codigoQR = new StringBuilder();
            codigoQR.Append("https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?id=" + documento.Comprobante.Complemento.TimbreFiscalDigital.UUID);
            codigoQR.Append("&re=" + documento.Comprobante.Emisor.rfc); //RFC del Emisor
            codigoQR.Append("&rr=" + documento.Comprobante.Receptor.rfc); //RFC del receptor
            codigoQR.Append("&tt=" + documento.Comprobante.Total); //Total del comprobante 10 enteros y 6 decimales
            //codigoQR.Append("&fe=" + _templatePDF.selloDigitalCFDI.Substring(_templatePDF.selloDigitalCFDI.Length - 8, 8)); //UUID del comprobante

            BarcodeQRCode pdfCodigoQR = new BarcodeQRCode(codigoQR.ToString(), 1, 1, null);
            Image img = pdfCodigoQR.GetImage();
            img.SpacingAfter = 0.0f;
            img.SpacingBefore = 0.0f;
            img.BorderWidth = 1.0f;
            //img.ScalePercent(100, 78);
            //img.border

            tablaSellosQR.AddCell(tablaSellos);
            tablaSellosQR.AddCell(img);

            _documento.Add(tablaSellosQR);
        }



        #region Extensión de la clase pdfPageEvenHelper
        public class ITextEvents : PdfPageEventHelper
        {

            // This is the contentbyte object of the writer
            PdfContentByte cb;

            // we will put the final number of pages in a template
            PdfTemplate headerTemplate, footerTemplate;

            // this is the BaseFont we are going to use for the header / footer
            BaseFont bf = null;

            // This keeps track of the creation time
            DateTime PrintTime = DateTime.Now;


            #region Fields
            private string _header;
            #endregion

            #region Properties
            public string Header
            {
                get { return _header; }
                set { _header = value; }
            }
            #endregion

            #endregion
            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                try
                {
                    PrintTime = DateTime.Now;
                    bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb = writer.DirectContent;
                    headerTemplate = cb.CreateTemplate(100, 100);
                    footerTemplate = cb.CreateTemplate(50, 50);
                }
                catch (DocumentException)
                { }
                catch (System.IO.IOException)
                { }
            }

            public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
            {
                base.OnEndPage(writer, document);

              
                String text = "Página " + writer.PageNumber + " ";
         
    
                    cb.BeginText();
                    cb.SetFontAndSize(bf, 9);
                    cb.SetTextMatrix(document.PageSize.GetRight(70), document.PageSize.GetBottom(30));
                    //cb.MoveText(500,30);
                    //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
                    cb.ShowText(text);
                    cb.EndText();
                    float len = bf.GetWidthPoint(text, 9);
                    cb.AddTemplate(footerTemplate, document.PageSize.GetRight(70) + len, document.PageSize.GetBottom(30));

                    float[] anchoColumasTablaTotales = { 600f };
                    PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
                    tabla.DefaultCell.Border = Rectangle.NO_BORDER;
                    tabla.SetTotalWidth(anchoColumasTablaTotales);
                    tabla.HorizontalAlignment = Element.ALIGN_CENTER;
                    tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    tabla.LockedWidth = true;
                    tabla.AddCell(new Phrase("Este documento es una representación de pagos 3.3", new Font(Font.FontFamily.HELVETICA, 10)));

                    tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(40), writer.DirectContent);

             



             
            }

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);

                //headerTemplate.BeginText();
                //headerTemplate.SetFontAndSize(bf, 12);
                //headerTemplate.SetTextMatrix(0, 0);
                //headerTemplate.ShowText((writer.PageNumber - 1).ToString());
                //headerTemplate.EndText();

                footerTemplate.BeginText();
                footerTemplate.SetFontAndSize(bf, 9);
                //footerTemplate.MoveText(550,30);
                footerTemplate.SetTextMatrix(0, 0);
                footerTemplate.ShowText((writer.PageNumber - 1).ToString());
                footerTemplate.EndText();
            }

            
        }

    }
}