using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.clasescfdi;
using SIAAPI.ClasesProduccion;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Inventarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace SIAAPI.Reportes
{


    public class CrearEtiquetaPreArt
    {
        MemoryStream _memoryStream = new MemoryStream();
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;



        List<VArtCaracteristica> lotes = new List<VArtCaracteristica>(); //Objeto que contendra la información del documento pdf
        int Caracteristica { get; set; }
        int presentacion { get; set; }


        public CMYKColor colordefinido;
        public string nombreDocumento = string.Empty;
        public CrearEtiquetaPreArt(System.Drawing.Image logo, List<VArtCaracteristica> _lotes, int _Caracteristica, int _presentacion)
        {
            lotes = _lotes;
            Caracteristica = _Caracteristica;
            presentacion = _presentacion;
            ClsColoresReporte colorr = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            colordefinido = colorr.color;
            //Trabajos con el documento XML
            _documento = new Document(new Rectangle(300, 320));
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Etiqueta Presentación" + presentacion + ".pdf"); ;


            if (File.Exists(nombreDocumento))
            {
                File.Delete(nombreDocumento);
            }

            //Creamos el documento
            _writer = PdfWriter.GetInstance(_documento, new FileStream(nombreDocumento, System.IO.FileMode.CreateNew));
            _writer.PageEvent = new ITextEventsEtiqMP(); // invoca la clase que esta mas abajo correspondiente al pie de pagina

            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();


            //    AgregarLogo(logo);
            //  AgregarCuadro();
            //  AgregarDatosEmisor("");
            AgregarDatosFactura();

            AgregarDatosReceptorEmisor();

            AgregarDatosProductos();


            //     AgregarSellos();

            //Cerramoe el documento
            _documento.Close();

            //HttpContext.Current.Response.ContentType = "pdf/application";
            //HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"EtiquetasRecepcion+" + Recepcion  +".pdf" + "\"");
            //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //HttpContext.Current.Response.Write(_documento);
            //HttpContext.Current.Response.End();

        }







        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {

            if (logoEmpresa == null)
                return;
            //Agrego la imagen al documento
            //Image imagen = Image.GetInstance(logoEmpresa, BaseColor.BLACK);
            //imagen.ScaleToFit(140,100);
            //imagen.Alignment = Element.ALIGN_TOP;
            //Chunk logo = new Chunk(imagen, 1, -40);
            //_documento.Add(logo);

            try
            {
                // Image jpg = Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/logo.jpg"));
                Image jpg = Image.GetInstance(logoEmpresa, BaseColor.BLACK);
                Paragraph paragraph = new Paragraph();
                paragraph.Alignment = Element.ALIGN_RIGHT;
                jpg.ScaleToFit(140f, 100F); //ancho y largo de la imagen
                jpg.Alignment = Image.ALIGN_RIGHT;
                jpg.SetAbsolutePosition(30f, 300f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera
                _documento.Add(jpg);
                //  doc.Add(paragraph);
            }
            catch (Exception err)
            {

            }
        }

        private void AgregarCuadro()
        {
            _cb = _writer.DirectContentUnder;


            //Agrego cuadro al documento
            _cb.SetColorStroke(new CMYKColor(0, 29, 50, 70)); //Color de la linea
                                                              //  _cb.SetColorFill(new CMYKColor(0, 29, 50, 70)); // Color del relleno
            _cb.SetLineWidth(3.5f); //Tamano de la linea
            _cb.Rectangle(240, 130, 50, 100);
            //  _cb.FillStroke();
        }

        private void AgregarDatosEmisor(String Telefono)
        {

        }

        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("Etiqueta PA");
            _documento.AddSubject("Etiqueta PA");
            _documento.AddTitle("Etiqueta PA");
            _documento.SetMargins(5, 5, 5, 5);
        }

        private void AgregarDatosFactura()
        {
            //Datos de la factura

        }

        private void AgregarDatosReceptorEmisor()
        {


        }


        private void AgregarDatosProductos()
        {



            foreach (VArtCaracteristica lote in lotes)
            {

                float[] tamanoColumnasProductos = { 285f };
                PdfPTable tablaProductosT = new PdfPTable(tamanoColumnasProductos);
                //tablaProductos.SpacingBefore = 1;
                //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaProductosT.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                tablaProductosT.DefaultCell.BorderWidthLeft = 0.0f;
                tablaProductosT.DefaultCell.BorderWidthRight = 0.0f;
                tablaProductosT.DefaultCell.BorderWidthBottom = 0.0f;
                tablaProductosT.DefaultCell.BorderWidthTop = 0.0f;
                tablaProductosT.SetTotalWidth(tamanoColumnasProductos);
                tablaProductosT.HorizontalAlignment = Element.ALIGN_CENTER;
                tablaProductosT.LockedWidth = true;
                //tablaProductosT.SpacingBefore = 10;

                
                PdfPTable tablaProductos = new PdfPTable(tamanoColumnasProductos);
                //tablaProductos.SpacingBefore = 1;
                //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaProductos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductos.DefaultCell.BorderWidthLeft = 0.0f;
                tablaProductos.DefaultCell.BorderWidthRight = 0.0f;
                tablaProductos.DefaultCell.BorderWidthBottom = 0.0f;
                tablaProductos.DefaultCell.BorderWidthTop = 0.0f;
                tablaProductos.SetTotalWidth(tamanoColumnasProductos);
                tablaProductos.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductos.LockedWidth = true;
                //tablaProductos.SpacingBefore = 30;


                //_documento.NewPage();
                Paragraph pt = new Paragraph();
                //pt.IndentationLeft = 200f;
              
                //pt.Leading = 10;
                pt.Alignment = Element.ALIGN_CENTER;
                Paragraph p3 = new Paragraph();
                //p3.IndentationLeft = 200f;
                //  p2.SpacingAfter = 18;
                p3.Leading = 10;
                p3.Alignment = Element.ALIGN_CENTER;

                //p3.Add(new Phrase("Clave del Artículo", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                pt.Add(new Phrase(lote.Cref, new Font(Font.FontFamily.HELVETICA, 60)));
                pt.Add(new Phrase(" \nDescripción", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
                pt.Add(new Phrase("\n"+lote.Descripcion, new Font(Font.FontFamily.HELVETICA, 9)));


                Paragraph p2 = new Paragraph();
                //p2.IndentationLeft = 210f;
                //  p2.SpacingAfter = 18;
                p2.Leading = 10;
                p2.Alignment = Element.ALIGN_CENTER;

                p2.Add(new Phrase("No. Presentación: ", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
                p2.Add(new Phrase("\n" + presentacion, new Font(Font.FontFamily.HELVETICA, 16)));

               

                p3.Add(new Phrase("\n\nPresentación", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));

                tablaProductosT.AddCell(pt);
                tablaProductos.AddCell(p2);
                tablaProductos.AddCell(p3);


                Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("Select * from Caracteristica where ID=" + Caracteristica).ToList().FirstOrDefault();



                SuajeCaracteristicas suajec = new SuajeCaracteristicas();

                FormulaSiaapi.Formulas formula = new FormulaSiaapi.Formulas();
                suajec.Eje = 0;

                try
                {
                    suajec.Eje = decimal.Parse(formula.getvalor("EJE", cara.Presentacion).ToString());
                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.Eje = 0;
                }
                suajec.Avance = 0;
                try
                {
                    suajec.Avance = decimal.Parse(formula.getvalor("AVANCE", cara.Presentacion).ToString());
                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.Avance = 0;
                }

                suajec.Gapeje = 0;
                try
                {
                    suajec.Gapeje = decimal.Parse(formula.getvalor("GAP EJE", cara.Presentacion).ToString());

                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.Gapeje = 0;
                }



                suajec.Gapavance = 3;
                try
                {
                    suajec.Gapavance = decimal.Parse(formula.getvalor("GAP AVANCE", cara.Presentacion).ToString());

                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.Gapavance = 2;
                }

                suajec.CavidadEje = 2;

                try
                {
                    suajec.CavidadEje = int.Parse(formula.getvalor("REP EJE", cara.Presentacion).ToString());


                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.CavidadEje = 2;
                }
                suajec.RepAvance = 2;

                try
                {
                    suajec.RepAvance = int.Parse(formula.getvalor("REP AVANCE", cara.Presentacion).ToString());
                    if (suajec.RepAvance == 0)
                    {
                        suajec.RepAvance = int.Parse(formula.getvalor("REPETICIONES AVANCE", cara.Presentacion).ToString());
                        if (suajec.RepAvance == 0)
                        {
                            suajec.RepAvance = int.Parse(formula.getvalor("REPETICIONES AL AVANCE", cara.Presentacion).ToString());
                            if (suajec.RepAvance == 0)
                            {
                                suajec.RepAvance = int.Parse(formula.getvalor("CAV AVA", cara.Presentacion).ToString());
                                if (suajec.RepAvance == 0)
                                {
                                    suajec.RepAvance = int.Parse(formula.getvalor("CAV AVANCE", cara.Presentacion).ToString());
                                    if (suajec.RepAvance == 0)
                                    {
                                        suajec.RepAvance = int.Parse(formula.getvalor("CAVIDADES AVANCE", cara.Presentacion).ToString());
                                        if (suajec.RepAvance == 0)
                                        {
                                            suajec.RepAvance = int.Parse(formula.getvalor("CAVIDADES AL AVANCE", cara.Presentacion).ToString());

                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.RepAvance = 2;
                }



                suajec.TH = 0;
                try
                {
                    suajec.TH = int.Parse(formula.getvalor("TH", cara.Presentacion).ToString());

                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.TH = 0;
                }

                string ALMADIENTES = "";
                 if (suajec.TH == 0)
                {
                    suajec.Alma = formula.getValorCadena("ALMA", cara.Presentacion).ToString();
                }




                suajec.Corte = "";
                try
                {
                    suajec.Corte = formula.getValorCadena("CORTE", cara.Presentacion).ToString();

                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.Corte = "";
                }
                suajec.Material = "";
                try
                {
                    suajec.Material = formula.getValorCadena("MATERIAL", cara.Presentacion).ToString();

                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.Material = "";
                }




                //_documento.NewPage();
                Paragraph p4 = new Paragraph();
                p4.IndentationLeft = 200f;
                //  p2.SpacingAfter = 18;
                p4.Leading = 10;
                p4.Alignment = Element.ALIGN_CENTER;


                if (suajec.Eje > 0 || suajec.Avance > 0)
                {
                    try
                    {

                        p4.Add(new Phrase("\n EJE: " + suajec.Eje, new Font(Font.FontFamily.HELVETICA, 9)));
                    }
                    catch (Exception err)
                    {

                    }
                    try
                    {
                        p4.Add(new Phrase("\n AVANCE: " + suajec.Avance, new Font(Font.FontFamily.HELVETICA, 9)));
                    }
                    catch (Exception err)
                    {

                    }
                    try
                    {
                        p4.Add(new Phrase("\n GAP EJE: " + suajec.Gapeje, new Font(Font.FontFamily.HELVETICA, 9)));
                    }
                    catch (Exception err)
                    {

                    }
                    try
                    {
                        p4.Add(new Phrase("\n GAP AVANCE:" + suajec.Gapavance, new Font(Font.FontFamily.HELVETICA, 9)));
                    }
                    catch (Exception err)
                    {

                    }

                    try
                    {
                        p4.Add(new Phrase("\n REP EJE: " + suajec.CavidadEje, new Font(Font.FontFamily.HELVETICA, 9)));
                    }
                    catch (Exception err)
                    {

                    }

                    try
                    {
                        p4.Add(new Phrase("\n REP AVANCE:" + suajec.RepAvance, new Font(Font.FontFamily.HELVETICA, 9)));
                    }
                    catch (Exception err)
                    {

                    }


                   if(suajec.TH == 0)
                    {
                        try
                        {
                            p4.Add(new Phrase("\n TH: " + suajec.Alma, new Font(Font.FontFamily.HELVETICA, 9)));
                        }
                        catch (Exception err)
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            p4.Add(new Phrase("\n TH: " + suajec.TH, new Font(Font.FontFamily.HELVETICA, 9)));
                        }
                        catch (Exception err)
                        {

                        }
                    }

                    
                    try
                    {
                        p4.Add(new Phrase("\n CORTE: " + suajec.Corte, new Font(Font.FontFamily.HELVETICA, 9)));
                    }
                    catch (Exception err)
                    {

                    }
                    try
                    {
                        p4.Add(new Phrase("\n MATERIAL: " + suajec.Material, new Font(Font.FontFamily.HELVETICA, 9)));
                    }
                    catch (Exception err)
                    {

                    }

                    p4.Add(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 9)));
                    p4.Add(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 9)));
                    p4.Add(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 9)));
                    //p3.Add(new Phrase("\n\nlote Interno" + lote.LoteInterno, new Font(Font.FontFamily.HELVETICA, 8)));
                    //p3.Add(new Phrase("\n\nTicket" + lote.Facturaprov, new Font(Font.FontFamily.HELVETICA, 8)));

                }
                else
                {
                    string[] atributos;
                    atributos = cara.Presentacion.Split(',');
                    try
                    {
                        int contador = 1;
                        foreach (string atributo in atributos)
                        {
                            string[] llave = atributo.Split(':');
                            p4.Add(new Phrase(llave[0]+":"+llave[1] +"\n", new Font(Font.FontFamily.HELVETICA, 9)));
                            contador = contador + 1;
                            if (contador>9)
                            {
                                throw new Exception("no mas atributos");
                            }
                        }
                    }
                    catch(Exception errr)
                    {
                        string errror = errr.Message;
                    }
                }


                float[] tamanoColumnasatributos = { 140f,140f };
                PdfPTable tablaAtributos = new PdfPTable(tamanoColumnasatributos);
                //tablaProductos.SpacingBefore = 1;
                //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaAtributos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaAtributos.DefaultCell.BorderWidthLeft = 0.0f;
                tablaAtributos.DefaultCell.BorderWidthRight = 0.0f;
                tablaAtributos.DefaultCell.BorderWidthBottom = 0.0f;
                tablaAtributos.DefaultCell.BorderWidthTop = 0.0f;
                tablaAtributos.SetTotalWidth(tamanoColumnasatributos);
                tablaAtributos.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaAtributos.LockedWidth = true;
                tablaAtributos.SpacingBefore = 0;

                tablaAtributos.AddCell(p4);



                StringBuilder codigoQR = new StringBuilder();
                codigoQR.Append("Clave producto: \n" + lote.Cref + "\n Descripción: \n" + lote.Descripcion + "\n Presentación: \n" + lote.Presentacion);


                BarcodeQRCode pdfCodigoQR = new BarcodeQRCode(codigoQR.ToString(), 1, 1, null);
                Image img = pdfCodigoQR.GetImage();
                img.SpacingAfter = 0.0f;
                img.SpacingBefore = 0.0f;
                img.BorderWidth = 1.0f;

                tablaAtributos.AddCell(img);

                tablaProductos.AddCell(tablaAtributos);

                //_documento.NewPage();
                


                //img.ScalePercent(100, 78);



                //StringBuilder codigoQR2 = new StringBuilder();
                //codigoQR2.Append("Cref\n" + lote.Cref + "\n Descripción \n" + lote.Descripcion + "\n Presentación \n" + lote.Presentacion);


                //BarcodeQRCode pdfCodigoQR2 = new BarcodeQRCode(codigoQR2.ToString(), 1, 1, null);
                //Image img2 = pdfCodigoQR2.GetImage();
                //img2.SpacingAfter = 0.0f;
                //img2.SpacingBefore = 0.0f;
                //img2.BorderWidth = 1.0f;
                //tabla1.AddCell(img2);

                _documento.Add(tablaProductosT);
                _documento.Add(tablaProductos);
               

                _documento.Add(Chunk.NEXTPAGE);
            }

            //img.border









        }






        #region Extensión de la clase pdfPageEvenHelper
        public class ITextEventsEtiqMP : PdfPageEventHelper
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

            public int Remision { get; set; }
            #endregion


            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                try
                {
                    PrintTime = DateTime.Now;
                    bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb = writer.DirectContent;
                    headerTemplate = cb.CreateTemplate(30, 30);
                    footerTemplate = cb.CreateTemplate(30, 30);
                }
                catch (DocumentException)
                { }
                catch (System.IO.IOException)
                { }
            }

            public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
            {
                base.OnEndPage(writer, document);

                iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

                iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);

                Phrase p1Header = new Phrase("Sample Header Here", baseFontNormal);

                ////Create PdfTable object
                //PdfPTable pdfTab = new PdfPTable(3);

                ////We will have to create separate cells to include image logo and 2 separate strings
                ////Row 1
                //PdfPCell pdfCell1 = new PdfPCell();
                //PdfPCell pdfCell2 = new PdfPCell(p1Header);
                //PdfPCell pdfCell3 = new PdfPCell();
                String text = "";// "Página " + writer.PageNumber + " de ";


                ////Add paging to header
                //{
                //    cb.BeginText();
                //    cb.SetFontAndSize(bf, 12);
                //    cb.SetTextMatrix(document.PageSize.GetRight(10), document.PageSize.GetTop(10));
                //    cb.ShowText(text);
                //    cb.EndText();
                //    float len = bf.GetWidthPoint(text, 12);
                //    //Adds "12" in Page 1 of 12
                //    cb.AddTemplate(headerTemplate, document.PageSize.GetRight(10) + len, document.PageSize.GetTop(10));
                //}

                //    Add paging to footer
                {
                    cb.BeginText();
                    cb.SetFontAndSize(bf, 9);
                    cb.SetTextMatrix(document.PageSize.GetRight(10), document.PageSize.GetBottom(10));
                    //cb.MoveText(500,30);
                    //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
                    cb.ShowText(text);
                    cb.EndText();
                    float len = bf.GetWidthPoint(text, 9);
                    cb.AddTemplate(footerTemplate, document.PageSize.GetRight(70) + len, document.PageSize.GetBottom(30));

                    float[] anchoColumasTablaTotales = { 280f };
                    PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
                    tabla.DefaultCell.Border = Rectangle.NO_BORDER;
                    tabla.SetTotalWidth(anchoColumasTablaTotales);
                    tabla.HorizontalAlignment = Element.ALIGN_CENTER;
                    tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    tabla.LockedWidth = true;
                    //  tabla.AddCell(new Phrase("Recepcion " + Remision, new Font(Font.FontFamily.HELVETICA, 10)));

                    tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(10), writer.DirectContent);

                }



                ////Row 2
                //PdfPCell pdfCell4 = new PdfPCell(new Phrase("Sub Header Description", baseFontNormal));
                ////Row 3


                //PdfPCell pdfCell5 = new PdfPCell(new Phrase("Date:" + PrintTime.ToShortDateString(), baseFontBig));
                //PdfPCell pdfCell6 = new PdfPCell();
                //PdfPCell pdfCell7 = new PdfPCell(new Phrase("TIME:" + string.Format("{0:t}", DateTime.Now), baseFontBig));


                ////set the alignment of all three cells and set border to 0
                //pdfCell1.HorizontalAlignment = Element.ALIGN_CENTER;
                //pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
                //pdfCell3.HorizontalAlignment = Element.ALIGN_CENTER;
                //pdfCell4.HorizontalAlignment = Element.ALIGN_CENTER;
                //pdfCell5.HorizontalAlignment = Element.ALIGN_CENTER;
                //pdfCell6.HorizontalAlignment = Element.ALIGN_CENTER;
                //pdfCell7.HorizontalAlignment = Element.ALIGN_CENTER;


                //pdfCell2.VerticalAlignment = Element.ALIGN_BOTTOM;
                //pdfCell3.VerticalAlignment = Element.ALIGN_MIDDLE;
                //pdfCell4.VerticalAlignment = Element.ALIGN_TOP;
                //pdfCell5.VerticalAlignment = Element.ALIGN_MIDDLE;
                //pdfCell6.VerticalAlignment = Element.ALIGN_MIDDLE;
                //pdfCell7.VerticalAlignment = Element.ALIGN_MIDDLE;


                //pdfCell4.Colspan = 3;



                //pdfCell1.Border = 0;
                //pdfCell2.Border = 0;
                //pdfCell3.Border = 0;
                //pdfCell4.Border = 0;
                //pdfCell5.Border = 0;
                //pdfCell6.Border = 0;
                //pdfCell7.Border = 0;


                ////add all three cells into PdfTable
                //pdfTab.AddCell(pdfCell1);
                //pdfTab.AddCell(pdfCell2);
                //pdfTab.AddCell(pdfCell3);
                //pdfTab.AddCell(pdfCell4);
                //pdfTab.AddCell(pdfCell5);
                //pdfTab.AddCell(pdfCell6);
                //pdfTab.AddCell(pdfCell7);

                //pdfTab.TotalWidth = document.PageSize.Width - 80f;
                //pdfTab.WidthPercentage = 70;
                ////pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;


                ////call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
                ////first param is start row. -1 indicates there is no end row and all the rows to be included to write
                ////Third and fourth param is x and y position to start writing
                //  pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
                ////set pdfContent value

                ////DIbuja una linea para separar el encabezado
                //cb.MoveTo(20, document.PageSize.Height - 142);
                //cb.LineTo(document.PageSize.Width - 20, document.PageSize.Height - 142);
                //cb.Stroke();

                //Move the pointer and draw line to separate footer section from rest of page
                //cb.MoveTo(40, document.PageSize.GetBottom(50));
                //cb.LineTo(document

            }



            //}

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