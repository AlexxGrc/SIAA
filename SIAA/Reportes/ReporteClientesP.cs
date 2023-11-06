using iTextSharp.text; //Librerias iTextSharp para generar pdf
using iTextSharp.text.pdf;
using SIAAPI.Models.Administracion; //Referencia a las carpetas que usamos en el proyecto
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace SIAAPI.Reportes
{
    public class ReporteClientesP
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;

        PdfWriter _writer;
        int _totalColumn = 8;

        string _Titulo = "";

        PdfPTable _pdfTable = new PdfPTable(8);

        PdfPCell _PdfPCell;

        public int idClienteP { get; set; }

        public string Titulo = "";
        MemoryStream _memoryStream = new MemoryStream();

        List<ClientesP> clientes = new List<ClientesP>();
        #endregion
        public ClientesPContext db = new ClientesPContext();
        // aqui los puedes pasar como parametro al reporte

        public byte[] PrepareReport(int _idCliente)

        {
            idClienteP = _idCliente;

            clientes = this.GetCliente(idClienteP);

            #region
            _documento = new Document(PageSize.LETTER, 0f, 0f, 0f, 0f);

            _documento.SetMargins(20f, 10f, 20f, 30f);

            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEvents();
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

            _documento.Open();

            // _pdfTable.SetWidths(new float[] { 40f, 40f, 40f, 260f, 60f, 60f, 50f, 50f});
            #endregion
            this.ReportHeader();
            this.ReportBody();

            //_pdfTable.HeaderRows = 4;
            //_documento.Add(_pdfTable);
            _documento.Close();


            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"ClientesProspecto.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();
        }

        public List<ClientesP> GetCliente(int idCliente)
        {
            List<ClientesP> rep = new List<ClientesP>();
            try
            {
                if (idCliente != 0)
                {
                    // esta consulta es para cuando seleccionaron un cliente, , así que aqui no pondríamos, bueno si, si pondriamos muchos datos jajaja,               
                    string cadena = "select * from ClientesP where IDClienteP = " + idCliente;
                    rep = db.Database.SqlQuery<ClientesP>(cadena).ToList();
                }
                else
                {
                    string cadena = "select * from ClientesP";
                    rep = db.Database.SqlQuery<ClientesP>(cadena).ToList();
                }
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }

            return rep;
        }
        
        public List<Vendedor> GetVen(int idven)
        {
            List<Vendedor> esta = new List<Vendedor>();

            // obtenemoslasofiinas             
            string cadena = "select * from Vendedor where IDVendedor = " + idven;
            esta = db.Database.SqlQuery<Vendedor>(cadena).ToList();

            return esta;
        }
        public List<ContactosPros> GetContactos(int idCliente)
        {
            List<ContactosPros> esta = new List<ContactosPros>();

            // obtenemoslasofiinas             
            string cadena = "select * from ContactosPros where IDClienteP=" + idCliente;
            esta = db.Database.SqlQuery<ContactosPros>(cadena).ToList();

            return esta;
        }
        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            System.Drawing.Image returnImage = null;
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                returnImage = System.Drawing.Image.FromStream(ms, true);//Exception occurs here
            }
            catch { }
            return returnImage;

        }

        private void ReportHeader()
        {
            #region Table head

            Empresa empresa = new EmpresaContext().empresas.Find(2);

            if (idClienteP == 0)
            {
                _Titulo = "REPORTE GENERAL DE CLIENTES PROSPECTO";

            }

            if (idClienteP != 0)
            {
                _Titulo = "DETALLES DEL CLIENTE PROSPECTO";

            }

            System.Drawing.Image logo = byteArrayToImage(empresa.Logo);
            Image jpg = Image.GetInstance(logo, BaseColor.WHITE);
            Paragraph paragraph = new Paragraph();
            paragraph.Alignment = Element.ALIGN_RIGHT;
            jpg.ScaleToFit(105f, 75F); //ancho y largo de la imagen
            jpg.Alignment = Image.ALIGN_RIGHT;
            jpg.SetAbsolutePosition(50f, 715f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera (+derecha/-izquierda,+arriba/-abajo)
            _documento.Add(jpg);


            Paragraph p1 = new Paragraph();
            p1.IndentationLeft = 250f;
            p1.Leading = 20;
            p1.Add(new Phrase(_Titulo, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.SpacingAfter = 40;
            _documento.Add(p1);

            float[] anchoColumnasencabezado = { 500f, 100f };

            PdfPTable tablaencabezado = new PdfPTable(anchoColumnasencabezado);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablaencabezado.SetTotalWidth(anchoColumnasencabezado);
            tablaencabezado.SpacingBefore = 0;
            tablaencabezado.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencabezado.LockedWidth = true;
            Font _fontStyleencabezado = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

            PdfPCell _tablaencabezado3 = new PdfPCell(new Phrase("Fecha de impresión: ", _fontStyleencabezado));
            _tablaencabezado3.Border = 0;
            _tablaencabezado3.FixedHeight = 10f;
            _tablaencabezado3.HorizontalAlignment = Element.ALIGN_RIGHT;
            _tablaencabezado3.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaencabezado3.BackgroundColor = BaseColor.WHITE;
            tablaencabezado.AddCell(_tablaencabezado3);

            DateTime fecAct = DateTime.Today;
            string FA = fecAct.ToString("dd/MM/yyyy");
            PdfPCell _tablaencabezado4 = new PdfPCell(new Phrase(FA, _fontStyleencabezado));
            _tablaencabezado4.Border = 0;
            // _tablaencabezado4.FixedHeight = 10f;
            _tablaencabezado4.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablaencabezado4.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaencabezado4.BackgroundColor = BaseColor.WHITE;
            tablaencabezado.AddCell(_tablaencabezado4);

            //tablaencabezado.CompleteRow();
            _documento.Add(tablaencabezado);

            float[] anchoColumnasencart = { 100f, 100f, 100f, 100f, 100f, 100f };

            // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

            PdfPTable tablae = new PdfPTable(anchoColumnasencart);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablae.HorizontalAlignment = Element.ALIGN_LEFT;
            tablae.WidthPercentage = 100;

            // aqui podremos cambiar la fuente de lcientes y su tamanño

            Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);

            CMYKColor COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

            CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;

            Font _fontStyleEncabezado = FontFactory.GetFont("Tahoma", 13f, COLORDEFUENTEREPORTE);


            if (idClienteP != 0)///PROVEEDOR INDIVIDUAL
            {
                //encbezado para cuando sea solo un cliente

                //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("NOMBRE", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("DETALLES", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _pdfTable.CompleteRow();
                _documento.Add(tablae);
            }
            else//// CLIENTE GENERAL
            {
                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("NOMBRE", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("VENDEDOR", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("CORREO", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("TELÉFONO", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("ESTADO", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("PAIS", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _pdfTable.CompleteRow();
                _documento.Add(tablae);
            }

            #endregion

        }
        private void ReportBody()
        {
            #region Table Body
            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            if (idClienteP != 0)//individual
            {
                foreach (ClientesP clie in clientes)
                {
                    float[] anchoColumnasclientes = { 200f, 200f, 100f, 100f };

                   

                    PdfPTable tablaArticulo = new PdfPTable(anchoColumnasclientes);
                    //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                    tablaArticulo.SetTotalWidth(anchoColumnasclientes);
                    tablaArticulo.SpacingBefore = 3;
                    tablaArticulo.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.LockedWidth = true;

                    Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                    PdfPCell _PdfPCell1 = new PdfPCell(new Phrase(clie.Nombre, _fontStylecliente));
                    _PdfPCell1.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    _PdfPCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell1.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell1);


                    _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                   

                  


                    _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                    _PdfPCell = new PdfPCell(new Phrase(clie.Correo, _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);

                 

                    _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                    _PdfPCell = new PdfPCell(new Phrase(clie.Telefono, _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);

                    Vendedor vend = new VendedorContext().Vendedores.Find(clie.IDVendedor);

                
                        _PdfPCell = new PdfPCell(new Phrase(vend.Nombre, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);
                  


                    tablaArticulo.CompleteRow();

                    _documento.Add(tablaArticulo);

                    #region

                    List<ContactosPros> contactosPros = new List<ContactosPros>();
                    contactosPros = GetContactos(idClienteP);

                    float[] anchoColumnasContactos = { 150f, 150f, 150f, 150f };

                    PdfPTable tablacontactos = new PdfPTable(anchoColumnasContactos);

                    tablacontactos.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablacontactos.WidthPercentage = 100;


                    CMYKColor COLORDEREPORTECon = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

                    CMYKColor COLORDEFUENTEREPORTECon = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;

                    Font _fontStyleEncabezadoCon = FontFactory.GetFont("Tahoma", 13f, COLORDEFUENTEREPORTECon);

                    _PdfPCell = new PdfPCell(new Phrase("CONTACTO", _fontStyleEncabezadoCon));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell.BackgroundColor = COLORDEREPORTECon;
                    tablacontactos.AddCell(_PdfPCell);

                    _PdfPCell = new PdfPCell(new Phrase("MAIL", _fontStyleEncabezadoCon));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell.BackgroundColor = COLORDEREPORTECon;
                    tablacontactos.AddCell(_PdfPCell);

                    _PdfPCell = new PdfPCell(new Phrase("TELÉFONO", _fontStyleEncabezadoCon));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell.BackgroundColor = COLORDEREPORTECon;
                    tablacontactos.AddCell(_PdfPCell);

                    _PdfPCell = new PdfPCell(new Phrase("CARGO", _fontStyleEncabezadoCon));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell.BackgroundColor = COLORDEREPORTECon;
                    tablacontactos.AddCell(_PdfPCell);

                    _pdfTable.CompleteRow();
                    _documento.Add(tablacontactos);

                    foreach (ContactosPros contacpro in contactosPros)
                    {

                        float[] anchoColumnasContacto = { 150f, 150f, 150f, 150f };


                        PdfPTable tablaContactos = new PdfPTable(anchoColumnasContacto);
                        tablaContactos.SetTotalWidth(anchoColumnasContacto);
                        tablaContactos.SpacingBefore = 3;
                        tablaContactos.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaContactos.LockedWidth = true;

                        Font _fontStyleContactos = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD);

                        _PdfPCell = new PdfPCell(new Phrase(contacpro.Nombre, _fontStyleContactos));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        tablaContactos.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(contacpro.Email, _fontStyleContactos));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        tablaContactos.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(contacpro.Telefono, _fontStyleContactos));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        tablaContactos.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(contacpro.Puesto, _fontStyleContactos));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        tablaContactos.AddCell(_PdfPCell);

                        tablaContactos.CompleteRow();

                        _documento.Add(tablaContactos);
                    }
                    #endregion
                }
            }


            // añadimos la tabla al documento princlipal para que la imprimia




            #endregion


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
                    //tabla.AddCell(new Phrase("Este documento es una representación impresa de un CFDI 3.3", new Font(Font.FontFamily.HELVETICA, 10)));

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
                footerTemplate.ShowText((writer.PageNumber).ToString());
                footerTemplate.EndText();
            }

        }
        #endregion
    }
}