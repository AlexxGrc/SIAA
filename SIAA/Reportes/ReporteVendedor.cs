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
    public class ReporteVendedor
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;
        Font _fontStyles;

        PdfWriter _writer;
        int _totalColumn = 8;

        string _Titulo = "";


        PdfPTable _pdfTable = new PdfPTable(8);
        //PdfPTable tablae = new PdfPTable(6);


        PdfPCell _PdfPCell;

        public int IDVendedor { get; set; }

        public string Titulo = "";
        MemoryStream _memoryStream = new MemoryStream();

        List<Vendedor> vendedores = new List<Vendedor>();
        #endregion
        public ClientesContext db = new ClientesContext();
        // aqui los puedes pasar como parametro al reporte

        public byte[] PrepareReport(int _idVendedor)

        {
            IDVendedor = _idVendedor;

            //vendedores = this.GetVendedorof(idVendedor);

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
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"Vendedores.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();
        }

        public List<Oficina> GetOficina()
        {
            List<Oficina> of = new List<Oficina>();

            // obtenemoslasofiinas             
            string cadena = "select * from oficina";
            of = db.Database.SqlQuery<Oficina>(cadena).ToList();

            return of;
        }

        public List<Oficina> GetOfi(int idofi)
        {
            List<Oficina> esta = new List<Oficina>();

            // obtenemoslasofiinas             
            string cadena = "select * from oficina where IDOficina=" + idofi;
            esta = db.Database.SqlQuery<Oficina>(cadena).ToList();

            return esta;
        }

        public List<Vendedor> GetVendedor(int idVendedor)
        {
            List<Vendedor> representante = new List<Vendedor>();
            try
            {
                if (idVendedor != 0)
                {

                    string cadena = "select * from Vendedor where IDVendedor = " + IDVendedor;
                    representante = db.Database.SqlQuery<Vendedor>(cadena).ToList();
                }
                else
                {
                    string cadena = "select * from Vendedor";
                    representante = db.Database.SqlQuery<Vendedor>(cadena).ToList();
                }
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return representante;
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

            if (IDVendedor == 0)
            {
                _Titulo = "REPORTE GENERAL DE VENDEDORES";

            }

            if (IDVendedor != 0)
            {
                _Titulo = "DETALLES DEL VENDEDOR";

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


            if (IDVendedor != 0)///PROVEEDOR INDIVIDUAL
            {
                //encbezado para cuando sea solo un cliente

                //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("EMPRESA", _fontStyleEncabezado));
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
                _PdfPCell = new PdfPCell(new Phrase("", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("DETALLES", _fontStyleEncabezado));
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
                _PdfPCell = new PdfPCell(new Phrase("", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _pdfTable.CompleteRow();
                _documento.Add(tablae);
            }
            else//// PROVEEDOR GENERAL
            {
                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("NOMBRE VENDEDOR", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("RFC", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("CORREO", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("TELEFONO", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("CUOTA VENDEDOR", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
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

            List<Vendedor> vendedores = new List<Vendedor>();
            vendedores = GetVendedor(IDVendedor);

            if (IDVendedor != 0)//individual
            {
                foreach (Vendedor vendedor in vendedores)
                {

                    try
                    {
                        Image jpg = Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/Upload/" + vendedor.Photo + ""));

                        Paragraph paragraph = new Paragraph();
                        paragraph.Alignment = Element.ALIGN_RIGHT;
                        jpg.ScaleToFit(150f, 150f);
                        jpg.Alignment = Image.ALIGN_RIGHT;
                        jpg.SetAbsolutePosition(350f, 535f); //(x - izquierda + derecha ,y - abajo + arriba)
                        _documento.Add(jpg);
                        _documento.Add(paragraph);
                    }
                    catch (Exception err)
                    {

                    }

                    string moneda = new c_MonedaContext().c_Monedas.Find(vendedor.IDMoneda).Descripcion;

                    string periodo = new c_PeriodicidadPagoContext().c_PeriocidadPagos.Find(vendedor.IDPeriocidadPago).Descripcion;
                    string ofi = new OficinaContext().Oficinas.Find(vendedor.IDOficina).NombreOficina;

                    float[] anchoColumnasclientes = { 200f, 175f, 10f, 165f };

                    // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor


                    PdfPTable tablaArticulo = new PdfPTable(anchoColumnasclientes);
                    //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                    tablaArticulo.SetTotalWidth(anchoColumnasclientes);
                    tablaArticulo.SpacingBefore = 3;
                    tablaArticulo.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.LockedWidth = true;

                    Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                    PdfPCell _PdfPCell1 = new PdfPCell(new Phrase("", _fontStylecliente));
                    _PdfPCell1.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell1.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell1);


                    _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
                    _fontStyles = FontFactory.GetFont("Tahoma", 10f, 1);

                    _PdfPCell = new PdfPCell(new Phrase("RFC", _fontStyles));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase(vendedor.RFC, _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);

                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("Nombre", _fontStyles));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase(vendedor.Nombre, _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);

                    ///////////////////////////////////////////////////////////////////////////

                    _PdfPCell = new PdfPCell(new Phrase("Cuota", _fontStyles));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);


                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase(vendedor.CuotaVendedor.ToString(), _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);


                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("Divisa de comisión: " + moneda, _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);


                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("Esquema de comisión", _fontStyles));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);


                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase(vendedor.EsquemaComision, _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);


                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("Porcentaje de comisión: ", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);

                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("Periodicidad de pago", _fontStyles));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);

                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase(periodo, _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);



                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("Oficina", _fontStyles));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);

                    List<Oficina> idofic = new List<Oficina>();
                    idofic = GetOfi(vendedor.IDOficina);
                    foreach (Oficina name in idofic)
                    {

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);
                        _PdfPCell = new PdfPCell(new Phrase(name.NombreOficina, _fontStyle));
                        _PdfPCell.Border = 0;
                        _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaArticulo.AddCell(_PdfPCell);
                        _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                        _PdfPCell.Border = 0;
                        _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaArticulo.AddCell(_PdfPCell);
                        _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                        _PdfPCell.Border = 0;
                        _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaArticulo.AddCell(_PdfPCell);
                    }

                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("Correo", _fontStyles));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);

                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase(vendedor.Mail, _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);

                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("Teléfono", _fontStyles));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);

                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase(vendedor.Telefono, _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);

                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("Notas", _fontStyles));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);

                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase(vendedor.Notas, _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);
                    _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _PdfPCell.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.AddCell(_PdfPCell);

                    tablaArticulo.CompleteRow();

                    _documento.Add(tablaArticulo);
                }
            }

            if (IDVendedor == 0) //// reporte general de proveedores
            {
                List<Oficina> ofi = new List<Oficina>();
                ofi = GetOficina();
                foreach (Oficina oficina in ofi)
                {

                    Font _fontStylefam = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);
                    CMYKColor COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;
                    CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;
                    Font _fontStyleEncabezado = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);

                    float[] anchoColumnasPresenta = { 150f, 450f };

                    PdfPTable tablaPresenta = new PdfPTable(anchoColumnasPresenta);

                    tablaPresenta.SetTotalWidth(anchoColumnasPresenta);
                    tablaPresenta.SpacingBefore = 10;
                    tablaPresenta.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tablaPresenta.LockedWidth = true;
                    Font _fontStylePresenta = new Font(Font.FontFamily.TIMES_ROMAN, 6, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

                    PdfPCell _tablaPresenta1 = new PdfPCell(new Phrase("OFICINA:" + oficina.NombreOficina, _fontStylePresenta)); //IDOficina.ToString()
                    _tablaPresenta1.Border = 0;
                    _tablaPresenta1.HorizontalAlignment = Element.ALIGN_CENTER;
                    _tablaPresenta1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _tablaPresenta1.BackgroundColor = COLORDEREPORTE;
                    _tablaPresenta1.FixedHeight = 10f;
                    tablaPresenta.AddCell(_tablaPresenta1);

                    PdfPCell _tablaPresenta2 = new PdfPCell(new Phrase("RESPONSABLE" + oficina.Responsable, _fontStylePresenta));
                    _tablaPresenta2.Border = 0;
                    _tablaPresenta2.FixedHeight = 10f;
                    _tablaPresenta2.HorizontalAlignment = Element.ALIGN_CENTER;
                    _tablaPresenta2.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _tablaPresenta2.BackgroundColor = COLORDEREPORTE;

                    tablaPresenta.AddCell(_tablaPresenta2);

                    tablaPresenta.CompleteRow();
                    _documento.Add(tablaPresenta);

                    List<Vendedor> vendedore = new List<Vendedor>();
                    vendedore = GetVendedor(IDVendedor);//GetVendedorof

                    foreach (Vendedor vendedor in vendedore)
                    {

                        // creamos una tabla para imprimir los los datos del PROVEEDOR
                        // como son 4 columnas a imprimir 600entre las 4 

                        float[] anchoColumnasclientes = { 100f, 100f, 200f, 100f, 100f };

                        // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor


                        PdfPTable tablaArticulo = new PdfPTable(anchoColumnasclientes);
                        //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                        tablaArticulo.SetTotalWidth(anchoColumnasclientes);
                        tablaArticulo.SpacingBefore = 3;
                        tablaArticulo.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaArticulo.LockedWidth = true;

                        Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                        PdfPCell _PdfPCell1 = new PdfPCell(new Phrase(vendedor.Nombre, _fontStylecliente));
                        _PdfPCell1.Border = 0;
                        _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell1.BackgroundColor = BaseColor.WHITE;
                        tablaArticulo.AddCell(_PdfPCell1);


                        PdfPCell _PdfPCell4 = new PdfPCell(new Phrase(vendedor.RFC, _fontStylecliente));
                        _PdfPCell4.Border = 0;
                        _PdfPCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell4.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell4.BackgroundColor = BaseColor.WHITE;
                        tablaArticulo.AddCell(_PdfPCell4);

                        PdfPCell _PdfPCell5 = new PdfPCell(new Phrase(vendedor.Mail, _fontStylecliente));
                        _PdfPCell5.Border = 0;
                        _PdfPCell5.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell5.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell5.BackgroundColor = BaseColor.WHITE;
                        tablaArticulo.AddCell(_PdfPCell5);

                        PdfPCell _PdfPCell6 = new PdfPCell(new Phrase(vendedor.Telefono, _fontStylecliente));
                        _PdfPCell6.Border = 0;
                        _PdfPCell6.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell6.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell6.BackgroundColor = BaseColor.WHITE;
                        tablaArticulo.AddCell(_PdfPCell6);

                        PdfPCell _PdfPCell7 = new PdfPCell(new Phrase(vendedor.CuotaVendedor.ToString(), _fontStylecliente));
                        _PdfPCell7.Border = 0;
                        _PdfPCell7.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell7.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell7.BackgroundColor = BaseColor.WHITE;
                        tablaArticulo.AddCell(_PdfPCell7);

                        //PdfPCell _PdfPCell8 = new PdfPCell(new Phrase(vendedor.Perfil, _fontStylecliente));
                        //_PdfPCell8.Border = 0;
                        //_PdfPCell8.HorizontalAlignment = Element.ALIGN_LEFT;
                        //_PdfPCell8.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //_PdfPCell8.BackgroundColor = BaseColor.WHITE;
                        //tablaArticulo.AddCell(_PdfPCell8);


                        tablaArticulo.CompleteRow();

                        // añadimos la tabla al documento princlipal para que la imprimia

                        _documento.Add(tablaArticulo);
                    }

                }

            }
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