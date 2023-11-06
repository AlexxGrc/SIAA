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
    public class ReporteProveedor
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;

        PdfWriter _writer;
        int _totalColumn = 8;

        string _Titulo = "";


        PdfPTable _pdfTable = new PdfPTable(8);
        //PdfPTable tablae = new PdfPTable(6);


        PdfPCell _PdfPCell;

        public int idProveedor { get; set; }

        public string Titulo = "";
        MemoryStream _memoryStream = new MemoryStream();

        List<Proveedor> proveedores = new List<Proveedor>();
        #endregion
        public ClientesContext db = new ClientesContext();
        // aqui los puedes pasar como parametro al reporte

        public byte[] PrepareReport(int _idProveedor)

        {
            idProveedor = _idProveedor;

            proveedores = this.GetProveedor(idProveedor);

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
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"Proveedores.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();
        }
        
        public List<Proveedor> GetProveedor(int idProveedor)
        {
            List<Proveedor> rep = new List<Proveedor>();
            try
            {
                if (idProveedor != 0)
                {
                    // esta consulta es para cuando seleccionaron un cliente, , así que aqui no pondríamos, bueno si, si pondriamos muchos datos jajaja,               
                    string cadena = "select * from Proveedores where idProveedor = " + idProveedor;
                    rep = db.Database.SqlQuery<Proveedor>(cadena).ToList();
                }
                else
                {
                    string cadena = "select * from Proveedores";
                    rep = db.Database.SqlQuery<Proveedor>(cadena).ToList();
                }
                //proveedor = db.Database.SqlQuery<Proveedor>("select * from Proveedores").ToList();
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }

            return rep;
        }


        List<BancosProv> bancospro = new List<BancosProv>();

        public List<BancosProv> GetBancos(int idProveedor)
        {
            List<BancosProv> esta = new List<BancosProv>();

            // obtenemoslasofiinas             
            string cadena = "select * from BancosProv where IDProveedor=" + idProveedor;
            esta = db.Database.SqlQuery<BancosProv>(cadena).ToList();

            return esta;
        }

        public List<ContactosProv> GetContactos(int idProveedor)
        {
            List<ContactosProv> esta = new List<ContactosProv>();

            // obtenemoslasofiinas             
            string cadena = "select * from ContactosProv where IDProveedor=" + idProveedor;
            esta = db.Database.SqlQuery<ContactosProv>(cadena).ToList();

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
            
            if (idProveedor == 0)
            {
                _Titulo = "REPORTE GENERAL DE PROVEEDORES";

            }

            if (idProveedor != 0)
            {
                _Titulo = "DETALLES DEL PROVEEDOR";

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


            if (idProveedor != 0)///PROVEEDOR INDIVIDUAL
            {
                //encbezado para cuando sea solo un cliente

                //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("EMPRESA", _fontStyleEncabezado));
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
            else//// PROVEEDOR GENERAL
            {
                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("EMPRESA", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("DIRECCIÓN", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("MUNICIPIO", _fontStyle));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("COLONIA", _fontStyle));
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
                _PdfPCell = new PdfPCell(new Phrase("TELÉFONO", _fontStyle));
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
            

                if (idProveedor != 0)//individual
                {

                    foreach (Proveedor proveedor in proveedores)
                    {

                        float[] anchoColumnasclientes = { 250f, 100f, 20f, 230f };
                    
                    string nombreestado = new Models.Administracion.EstadosContext().Estados.Find(proveedor.IDEstado).Estado;
                    string nombrepais = new Models.Administracion.EstadosContext().Paises.Find(proveedor.IDPais).Pais;
                    string iva = new c_TipoIVAContext().c_TiposIVA.Find(proveedor.IDTipoIVA).Descripcion;


                    PdfPTable tablaArticulo = new PdfPTable(anchoColumnasclientes);
                        //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                        tablaArticulo.SetTotalWidth(anchoColumnasclientes);
                        tablaArticulo.SpacingBefore = 3;
                        tablaArticulo.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaArticulo.LockedWidth = true;

                        Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                        PdfPCell _PdfPCell1 = new PdfPCell(new Phrase(proveedor.Empresa, _fontStylecliente));
                        _PdfPCell1.Border = 0;
                        _PdfPCell1.HorizontalAlignment = Element.ALIGN_CENTER;
                        _PdfPCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell1.BackgroundColor = BaseColor.WHITE;
                        tablaArticulo.AddCell(_PdfPCell1);


                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        ///////////         CALLE
                        _PdfPCell = new PdfPCell(new Phrase("Calle", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(proveedor.Calle, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Numero exterior
                        _PdfPCell = new PdfPCell(new Phrase("No. exterior", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(proveedor.NoExt, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Numero interior
                        _PdfPCell = new PdfPCell(new Phrase("No. interior", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(proveedor.NoInt, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Colonia
                        _PdfPCell = new PdfPCell(new Phrase("Colonia", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(proveedor.Colonia, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Municipio
                        _PdfPCell = new PdfPCell(new Phrase("Municipio", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(proveedor.Municipio, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         CP
                        _PdfPCell = new PdfPCell(new Phrase("C.P.", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(proveedor.NoExt, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Estado
                        _PdfPCell = new PdfPCell(new Phrase("Estado", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(nombreestado, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Nombre del pais
                        _PdfPCell = new PdfPCell(new Phrase("Nombre del país", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(nombrepais, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Telefono 1
                        _PdfPCell = new PdfPCell(new Phrase("Teléfono 1", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(proveedor.Telefonouno, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Telefono 2
                        _PdfPCell = new PdfPCell(new Phrase("Teléfono 2", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(proveedor.Telefonodos, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Producto
                        _PdfPCell = new PdfPCell(new Phrase("Producto", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(proveedor.Producto, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         RFC
                        _PdfPCell = new PdfPCell(new Phrase("RFC", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(proveedor.RFC, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         IVA
                        _PdfPCell = new PdfPCell(new Phrase("IVA", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(iva, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Estatus
                        _PdfPCell = new PdfPCell(new Phrase("Estatus", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(proveedor.Status, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        /////////////         Forma de pago
                        //_PdfPCell = new PdfPCell(new Phrase("Forma de Pago", _fontStylecliente));
                        //_PdfPCell.Border = 0;
                        //_PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        //tablaArticulo.AddCell(_PdfPCell);

                        //_PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        //_PdfPCell.Border = 0;
                        //tablaArticulo.AddCell(_PdfPCell);

                        //_fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        //_PdfPCell = new PdfPCell(new Phrase(proveedor.FormaPago.ToString(), _fontStyle));
                        //_PdfPCell.Border = 0;
                        //tablaArticulo.AddCell(_PdfPCell);

                        //_fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        //_PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        //_PdfPCell.Border = 0;
                        //tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Metodo de pago
                        //_PdfPCell = new PdfPCell(new Phrase("Método de Pago", _fontStylecliente));
                        //_PdfPCell.Border = 0;
                        //_PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        //tablaArticulo.AddCell(_PdfPCell);

                        //_PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        //_PdfPCell.Border = 0;
                        //tablaArticulo.AddCell(_PdfPCell);

                        //_fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        //_PdfPCell = new PdfPCell(new Phrase(proveedor.MetodoPago.ToString(), _fontStyle));
                        //_PdfPCell.Border = 0;
                        //tablaArticulo.AddCell(_PdfPCell);

                        //_fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        //_PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        //_PdfPCell.Border = 0;
                        //tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Condiciones de pago
                        //_PdfPCell = new PdfPCell(new Phrase("Condiciones de Pago", _fontStylecliente));
                        //_PdfPCell.Border = 0;
                        //_PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        //tablaArticulo.AddCell(_PdfPCell);

                        //_PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        //_PdfPCell.Border = 0;
                        //tablaArticulo.AddCell(_PdfPCell);

                        //_fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        //_PdfPCell = new PdfPCell(new Phrase(proveedor.CondicionesPago.ToString(), _fontStyle));
                        //_PdfPCell.Border = 0;
                        //tablaArticulo.AddCell(_PdfPCell);

                        //_fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        //_PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        //_PdfPCell.Border = 0;
                        //tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Confianza
                        _PdfPCell = new PdfPCell(new Phrase("Confianza", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(proveedor.Confianza, _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Servicio
                        _PdfPCell = new PdfPCell(new Phrase("Servicio", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(proveedor.Servicio.ToString(), _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        ///////////         Calidad
                        _PdfPCell = new PdfPCell(new Phrase("Calidad", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaArticulo.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(proveedor.Calidad.ToString(), _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);


                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);
                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);
                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);

                        tablaArticulo.CompleteRow();

                        _documento.Add(tablaArticulo);

                    #region

                    List<BancosProv> bancosProvs = new List<BancosProv>();
                    bancosProvs = GetBancos(idProveedor);

                    float[] anchoColumnasencart = { 150f, 150f, 150f, 150f };

                    PdfPTable tablae = new PdfPTable(anchoColumnasencart);

                    tablae.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablae.WidthPercentage = 100;


                    CMYKColor COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

                    CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;

                    Font _fontStyleEncabezado = FontFactory.GetFont("Tahoma", 13f, COLORDEFUENTEREPORTE);

                    _PdfPCell = new PdfPCell(new Phrase("BANCO", _fontStyleEncabezado));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell.BackgroundColor = COLORDEREPORTE;
                    tablae.AddCell(_PdfPCell);

                    _PdfPCell = new PdfPCell(new Phrase("MONEDA", _fontStyleEncabezado));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell.BackgroundColor = COLORDEREPORTE;
                    tablae.AddCell(_PdfPCell);

                    _PdfPCell = new PdfPCell(new Phrase("CUENTA", _fontStyleEncabezado));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell.BackgroundColor = COLORDEREPORTE;
                    tablae.AddCell(_PdfPCell);

                    _PdfPCell = new PdfPCell(new Phrase("CUENTACLABE", _fontStyleEncabezado));
                    _PdfPCell.Border = 0;
                    _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell.BackgroundColor = COLORDEREPORTE;
                    tablae.AddCell(_PdfPCell);

                    _pdfTable.CompleteRow();
                    _documento.Add(tablae);

                    foreach (BancosProv bancpro in bancosProvs)
                    {
                        float[] anchoColumnasBancos = { 150f, 150f, 150f, 150f };

                        string moneda = new c_MonedaContext().c_Monedas.Find(bancpro.IDMoneda).ClaveMoneda;
                        string banco = new c_BancoContext().c_Bancos.Find(bancpro.IDBanco).Nombre;

                        PdfPTable tablaBancos = new PdfPTable(anchoColumnasBancos);
                        tablaBancos.SetTotalWidth(anchoColumnasBancos);
                        tablaBancos.SpacingBefore = 3;
                        tablaBancos.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaBancos.LockedWidth = true;

                        Font _fontStyleBancos = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD);

                        _PdfPCell = new PdfPCell(new Phrase(banco, _fontStyleBancos));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        tablaBancos.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(moneda, _fontStyleBancos));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        tablaBancos.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(bancpro.Cuenta, _fontStyleBancos));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        tablaBancos.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase(bancpro.CuentaClabe, _fontStyleBancos));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        tablaBancos.AddCell(_PdfPCell);

                        //_PdfPCell = new PdfPCell(new Phrase(" ", _fontStyleBancos));
                        //_PdfPCell.Border = 0;
                        //tablaBancos.AddCell(_PdfPCell);
                        //_PdfPCell = new PdfPCell(new Phrase(" ", _fontStyleBancos));
                        //_PdfPCell.Border = 0;
                        //tablaBancos.AddCell(_PdfPCell);
                        //_PdfPCell = new PdfPCell(new Phrase(" ", _fontStyleBancos));
                        //_PdfPCell.Border = 0;
                        //tablaBancos.AddCell(_PdfPCell);
                        //_PdfPCell = new PdfPCell(new Phrase(" ", _fontStyleBancos));
                        //_PdfPCell.Border = 0;
                        //tablaBancos.AddCell(_PdfPCell);

                        tablaBancos.CompleteRow();

                        _documento.Add(tablaBancos);
                    }
                    #endregion

                    #region

                    List<ContactosProv> contactosProvs = new List<ContactosProv>();
                    contactosProvs = GetContactos(idProveedor);

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

                    foreach (ContactosProv contacpro in contactosProvs)
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

                if (idProveedor == 0) //// reporte general de proveedores
                {
                    foreach (Proveedor proveedor in proveedores)
                    {

                        // creamos una tabla para imprimir los los datos del PROVEEDOR
                        // como son 4 columnas a imprimir 600entre las 4 

                        float[] anchoColumnasclientes = { 100f, 100f, 100f, 100f, 100f, 100f };

                        // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor


                        PdfPTable tablaArticulo = new PdfPTable(anchoColumnasclientes);
                        //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                        tablaArticulo.SetTotalWidth(anchoColumnasclientes);
                        tablaArticulo.SpacingBefore = 3;
                        tablaArticulo.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaArticulo.LockedWidth = true;

                        Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                        PdfPCell _PdfPCell1 = new PdfPCell(new Phrase(proveedor.Empresa, _fontStylecliente));
                        _PdfPCell1.Border = 0;
                        _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell1.BackgroundColor = BaseColor.WHITE;
                        tablaArticulo.AddCell(_PdfPCell1);


                        PdfPCell _PdfPCell4 = new PdfPCell(new Phrase(proveedor.Calle + " " + proveedor.NoExt, _fontStylecliente));
                        _PdfPCell4.Border = 0;
                        _PdfPCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell4.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell4.BackgroundColor = BaseColor.WHITE;
                        tablaArticulo.AddCell(_PdfPCell4);

                        PdfPCell _PdfPCell5 = new PdfPCell(new Phrase(proveedor.Municipio, _fontStylecliente));
                        _PdfPCell5.Border = 0;
                        _PdfPCell5.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell5.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell5.BackgroundColor = BaseColor.WHITE;
                        tablaArticulo.AddCell(_PdfPCell5);

                        PdfPCell _PdfPCell6 = new PdfPCell(new Phrase(proveedor.Colonia, _fontStylecliente));
                        _PdfPCell6.Border = 0;
                        _PdfPCell6.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell6.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell6.BackgroundColor = BaseColor.WHITE;
                        tablaArticulo.AddCell(_PdfPCell6);

                        PdfPCell _PdfPCell7 = new PdfPCell(new Phrase(proveedor.RFC, _fontStylecliente));
                        _PdfPCell7.Border = 0;
                        _PdfPCell7.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell7.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell7.BackgroundColor = BaseColor.WHITE;
                        tablaArticulo.AddCell(_PdfPCell7);

                        PdfPCell _PdfPCell8 = new PdfPCell(new Phrase(proveedor.Telefonouno, _fontStylecliente));
                        _PdfPCell8.Border = 0;
                        _PdfPCell8.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell8.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell8.BackgroundColor = BaseColor.WHITE;
                        tablaArticulo.AddCell(_PdfPCell8);

                        tablaArticulo.CompleteRow();

                        // añadimos la tabla al documento princlipal para que la imprimia

                        _documento.Add(tablaArticulo);
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