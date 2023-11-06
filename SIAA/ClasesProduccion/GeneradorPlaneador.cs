
using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.PlaneacionProduccion;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SIAAPI.ClasesProduccion
{
    public class GeneradorPlaneador
    {
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;
        Empresa empresa = new Empresa();
        //Objeto que contendra la información del documento pdf

        public string nombreDocumento = string.Empty;
        public int numerodeplaneacion = 1;
        public int version = 1;
        public HEspecificacion Hespecificacion = new HEspecificacion();
        public ModelosDeProduccion Modelo = new ModelosDeProduccion();


        public GeneradorPlaneador(System.Drawing.Image logo, int _folio, int _version, string Telefono = "")
        {
            numerodeplaneacion = _folio;
            version = _version;

            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/Planeacion.pdf");
            _documento = new Document(PageSize.LETTER);
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            //string nombreDocumento = rutaPDF;

            _writer = PdfWriter.GetInstance(_documento, new FileStream(nombreDocumento, System.IO.FileMode.Create));
            _writer.PageEvent = new ITextEvents();


            _documento.Open();

            AgregarLogo(logo);
            AgregarCuadro();
            AgregarDatosEmisor(Telefono);



            _documento.Close();

        }

        public void setModelodeproduccion(int id)
        {
            Modelo = new ModelosDeProduccionContext().ModelosDeProducciones.Find(id);
        }

        private void AgregarDatosEmisor(string telefono)
        {

            //Datos del emisor
            Paragraph p1 = new Paragraph();
            p1.IndentationLeft = 150f;

            p1.Leading = 9;
            p1.Add(new Phrase("FABRICANTE", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            p1.Add("\n");
            p1.Add(new Phrase(empresa.RazonSocial, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("Departamento de Producción", new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.Add("\n");



            p1.SpacingAfter = -45;

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
            Chunk logo = new Chunk(imagen, 1, -40);
            _documento.Add(logo);
        }

        private void AgregarCuadro()
        {
            _cb = _writer.DirectContentUnder;


            //Agrego cuadro al documento
            _cb.SetColorStroke(new CMYKColor(0, 29, 50, 70)); //Color de la linea
            _cb.SetColorFill(new CMYKColor(0, 29, 50, 70)); // Color del relleno
            _cb.SetLineWidth(3.5f); //Tamano de la linea
            _cb.Rectangle(378, 694, 20, 100);
            _cb.FillStroke();
        }

        private void AgregarDatosFactura()
        {
            //Datos de la factura
            Paragraph p2 = new Paragraph();
            p2.IndentationLeft = 403f;
            p2.SpacingAfter = 18;
            p2.Leading = 10;
            p2.Alignment = Element.ALIGN_CENTER;

            p2.Add(new Phrase("PLANEACION DE LA PRODUCCION ", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase(" PLANEACION: "  + numerodeplaneacion, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase("FECHA DE CREACION:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase(Hespecificacion.FechaEspecificacion.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
            p2.Add("\n");


            p2.Add(new Phrase("FECHA DE IMPRESION:" , new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase(DateTime.Now.ToLocalTime().ToString() , new Font(Font.FontFamily.HELVETICA, 8)));
            p2.Add("\n");
            _documento.Add(p2);
        }

        public void datosdelarticulo()
        {
            Paragraph p2 = new Paragraph();
            p2.IndentationLeft = 403f;
            p2.SpacingAfter = 18;
            p2.Leading = 10;
            p2.Alignment = Element.ALIGN_CENTER;

            p2.Add(new Phrase("ARTICULO A PRODUCIR ", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase("DESCRIPCION: " + Hespecificacion.Descripcion, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase("PRESENTACION: " + Hespecificacion.Presentacion, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add("\n");
            p2.Add(new Phrase("MODELO DE PRODUCCION " + Modelo.Descripcion, new Font(Font.FontFamily.HELVETICA, 8)));
            p2.Add("\n\n");
            p2.Add("--------------------------------------------------------------------------------------------------------------------");
            p2.Add("PROCESO DE PRODUCCION\n");
            p2.Add("                PROCESOS\n");
            p2.Add("                        ARTICULOS");
            p2.Add("--------------------------------------------------------------------------------------------------------------------");








            _documento.Add(p2);


        }

        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("Planeacion,Produccion");
            _documento.AddSubject("Planeacion de la produccion");
            _documento.AddTitle("Planeacion");
            _documento.SetMargins(5, 5, 5, 5);
        }
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

           
            //PdfPCell pdfCell3 = new PdfPCell();
            String text = "Página " + writer.PageNumber + " de ";


            {
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
                tabla.AddCell(new Phrase("Este documento es una representación impresa de un CFDI 3.3", new Font(Font.FontFamily.HELVETICA, 10)));

                tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(40), writer.DirectContent);

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
            //pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
            ////set pdfContent value

            ////Move the pointer and draw line to separate header section from rest of page
            //cb.MoveTo(40, document.PageSize.Height - 100);
            //cb.LineTo(document.PageSize.Width - 40, document.PageSize.Height - 100);
            //cb.Stroke();

            //Move the pointer and draw line to separate footer section from rest of page
            //cb.MoveTo(40, document.PageSize.GetBottom(50));
            //cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(50));
            //cb.Stroke();
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
    #endregion