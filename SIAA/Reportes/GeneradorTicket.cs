using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;

namespace SIAAPI.Reportes
{


    public class GeneradorTicket
    {
        MemoryStream _memoryStream = new MemoryStream();
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;
        public EncfacturaProv Encorden; //Objeto que contendra la información del documento pdf



        public GeneradorTicket(System.Drawing.Image logo, EncfacturaProv _Encorden)
        {
            Encorden = _Encorden;
            ObtenerLetras();
            //Trabajos con el documento XML
            _documento = new Document(PageSize.LETTER);
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            //string nombreDocumento = rutaPDF;

            //Creamos el documento
            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEventsOC(); // invoca la clase que esta mas abajo correspondiente al pie de pagina

            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();
            _cb = _writer.DirectContentUnder;


            AgregarLogo(logo);
            AgregarCuadro();

            Empresa EMPRESA = new EmpresaContext().empresas.Find(2);

            AgregarDatosEmisor(EMPRESA);
            AgregarDatosFactura();

            AgregarDatosReceptorEmisor();

            AgregarDatosticket();
            AgregarTotales();

            Agregarpie();

            //     AgregarSellos();

            //Cerramoe el documento
            _documento.Close();

            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"TICKETRecepcion" + Encorden.ID + ".pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

        }

        private void Agregarpie()
        {
            float[] anchoColumasTablapie = { 600f };
            PdfPTable tablapie = new PdfPTable(anchoColumasTablapie);
            tablapie.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            //tablapie.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapie.SetTotalWidth(anchoColumasTablapie);
            tablapie.HorizontalAlignment = Element.ALIGN_CENTER;
            tablapie.LockedWidth = true;

            tablapie.AddCell(new Phrase("Horario de entrega de Lunes a Viernes de 8:00 am a 5:30 pm.", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));


            tablapie.AddCell(new Phrase("El proveedor se obliga a vender y entregar los productos y/o en proporcionar los servicios especificados en este documento, de acuerdo con los términos, condiciones y cláusulas de la Orden de compra, y de sus cláusulas especiales, modificaciones o complementos.Todo lo anterior, constituye un acuerdo final y completo entre las partes.Los términos, condiciones y cláusulas de esta Orden de Compra se encuentran en este documento.Las obligaciones del comprador quedan expresamente limitadas en los términos, condiciones y clausulas aquí contenidas.Cualquier otro término, condición o cláusula que proponga el Proveedor no será valida a menos que sea aceptada por escrito por el Comprador.Nos reservamos el derecho a realizar Auditoria en las instalaciones del Proveedor.\n\nDocumentos para recepcion indispensables\n-Este ticket \n-Orden de Compra \n-Certificado de Calidad", new Font(Font.FontFamily.HELVETICA, 6)));

            PdfPCell celda03 = new PdfPCell(new Phrase("Entregó: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            celda03.MinimumHeight = 70;

            tablapie.AddCell(celda03);

            PdfPCell celda04 = new PdfPCell(new Phrase("Aceptacion del almacen: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            celda04.MinimumHeight = 70;

            tablapie.AddCell(celda04);

           
          //  tablapie.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));





            _documento.Add(tablapie);
        }



        #region Leer datos del .xml





        #endregion

        #region Escribir datos en el .pdf

        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {
            if (logoEmpresa == null)
                return;
            //Agrego la imagen al documento
            Image imagen = Image.GetInstance(logoEmpresa, BaseColor.BLACK);
            imagen.ScaleToFit(120, 100);
            //  imagen.Alignment = Element.ALIGN_TOP;
            imagen.SetAbsolutePosition(15f, (_documento.PageSize.Height - 40));
            //    Chunk logo = new Chunk(imagen, 1, imagen.Height +10);
            _documento.Add(imagen);
        }

        private void AgregarCuadro()
        {
            
          

            ////Agrego cuadro al documento
            //_cb.SetColorStroke(new CMYKColor(0, 29, 50, 70)); //Color de la linea
            //_cb.SetColorFill(new CMYKColor(0, 29, 50, 70)); // Color del relleno
            //_cb.SetLineWidth(3.5f); //Tamano de la linea
            //_cb.Rectangle(408, 694, 20, 100);
            //_cb.FillStroke();
        }

        private void AgregarDatosEmisor(SIAAPI.Models.Comercial.Empresa empresa)
        {
            //Datos del emisor
            Paragraph p1 = new Paragraph();
            p1.IndentationLeft = 150f;

            p1.Leading = 9;

            p1.Add(new Phrase(empresa.RazonSocial, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + empresa.RFC, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + empresa.Telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            //p1.Add("\n");
            //p1.Add(new Phrase("" + empresa., new Font(Font.FontFamily.HELVETICA, 8)));

            p1.Add("\n");
            p1.SpacingAfter = -70;
            p1.Add("\n");
            p1.Add("\n");

            //if (_templatePDF.emisor.telefono != string.Empty)
            //{
            //    p1.Add(new Phrase("Tel." + _templatePDF.emisor.telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            //    p1.Add("\n");
            //}
            _documento.Add(p1);
        }

        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("ticket de entrega");

            _documento.AddTitle("ticket de entrega");
            _documento.SetMargins(5, 5, 5, 5);
        }

        private void AgregarDatosFactura()
        {

        }

        private void AgregarDatosReceptorEmisor()
        {
            float[] anchoColumnasTablaDatos = { 600f };
            PdfPTable tablaDatosPrincipal = new PdfPTable(anchoColumnasTablaDatos);
            tablaDatosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.SetTotalWidth(anchoColumnasTablaDatos);
            tablaDatosPrincipal.SpacingBefore = 100;
            tablaDatosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosPrincipal.LockedWidth = true;


            PdfPCell celda0 = new PdfPCell(new Phrase("PROVEEDOR: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            PdfPCell celda1 = new PdfPCell(new Phrase(Encorden.Nombre_Proveedor.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));

            PdfPCell celda2 = new PdfPCell(new Phrase("FACTURA "+Encorden.UUID.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));

            celda0.Border = Rectangle.NO_BORDER;
            celda1.Border = Rectangle.NO_BORDER;
            celda2.Border = Rectangle.NO_BORDER;

            tablaDatosPrincipal.AddCell(celda0);
            tablaDatosPrincipal.AddCell(celda1);
            tablaDatosPrincipal.AddCell(celda2);

            _documento.Add(tablaDatosPrincipal);


        }

        private string ReturnClavePago(string clave)
        {
            string valor = "";

            valor = clave;

            return valor.ToString().ToUpper();
        }



        private void AgregarDatosticket()
        {
            float[] anchoColumnasTablaProductos = { 600f };
            PdfPTable tablaProductosPrincipal = new PdfPTable(anchoColumnasTablaProductos);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosPrincipal.SetTotalWidth(anchoColumnasTablaProductos);
            tablaProductosPrincipal.SpacingBefore = 15;
            tablaProductosPrincipal.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaProductosPrincipal.LockedWidth = true;


            PdfPCell celda0 = new PdfPCell(new Phrase("TICKET ", new Font(Font.FontFamily.HELVETICA, 32, Font.BOLD)));
            PdfPCell celda1 = new PdfPCell(new Phrase(Encorden.ID.ToString(), new Font(Font.FontFamily.HELVETICA, 32, Font.BOLD)));

            celda0.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            celda1.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            //   celda0.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            //   celda1.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            celda0.MinimumHeight = 140;
            celda1.MinimumHeight = 140;

            celda0.Border = 0;
                celda1.Border = 0;




            tablaProductosPrincipal.AddCell(celda0);
            tablaProductosPrincipal.AddCell(celda1);


            iTextSharp.text.pdf.Barcode128 bc = new Barcode128();
            bc.TextAlignment = Element.ALIGN_CENTER;
            bc.Code = Encorden.ID.ToString();  //el id de la factura es el numero de ticket
            bc.StartStopText = false;
            bc.CodeType = iTextSharp.text.pdf.Barcode128.CODE128;
            bc.Extended = true;
            //bc.Font = null;

            iTextSharp.text.Image PatImage1 = bc.CreateImageWithBarcode(_cb, iTextSharp.text.BaseColor.BLACK, iTextSharp.text.BaseColor.BLACK);
            PatImage1.ScaleToFit(320, 60);



            PdfPCell barcideimage = new PdfPCell(PatImage1);
            //barcideimage.Colspan = 2;
            barcideimage.HorizontalAlignment = 1;
            barcideimage.Border = 0;

            tablaProductosPrincipal.AddCell(barcideimage);
            //
          

            _documento.Add(tablaProductosPrincipal);
        }

        private void AgregarTotales()
        {

        }

        private void AgregarSellos()
        {



        }




        private void ObtenerLetras()
        {




        }
        #endregion








        #region Extensión de la clase pdfPageEvenHelper
        public class ITextEventsOC : PdfPageEventHelper
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
                String TextRevision = "REV. 1";


                {
                    cb.BeginText();
                    cb.SetFontAndSize(bf, 9);
                    cb.SetTextMatrix(document.PageSize.GetRight(70), document.PageSize.GetBottom(30));
                    //cb.MoveText(500,30);
                    //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
                    //cb.ShowText(text);
                    cb.EndText();
                    float len = bf.GetWidthPoint(text, 9);
                    cb.AddTemplate(footerTemplate, document.PageSize.GetRight(70) + len, document.PageSize.GetBottom(30));

                    float[] anchoColumasTablaTotales = { 480f, 120f };
                    PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
                    tabla.DefaultCell.Border = Rectangle.NO_BORDER;
                    tabla.SetTotalWidth(anchoColumasTablaTotales);
                    tabla.HorizontalAlignment = Element.ALIGN_CENTER;
                    tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    tabla.LockedWidth = true;
                    tabla.AddCell(new Phrase("Este documento es ticket para entregar mercancia ", new Font(Font.FontFamily.HELVETICA, 8)));
                    tabla.AddCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 8)));
                    tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                    tabla.AddCell(new Phrase(TextRevision, new Font(Font.FontFamily.HELVETICA, 8)));
                    tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(40), writer.DirectContent);

                }



            }

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);

                footerTemplate.BeginText();
                footerTemplate.SetFontAndSize(bf, 9);
                //footerTemplate.MoveText(550,30);
                footerTemplate.SetTextMatrix(0, 0);
                //footerTemplate.ShowText((writer.PageNumber - 1).ToString());
                footerTemplate.EndText();
            }
        }
        #endregion



    }
}



