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
    public class ReportePorArticulo
    {
        
        private Document _documento;
        Font _fontStyle;
        
        PdfWriter _writer;
        int _totalColumn = 8;

        string _Titulo = "";
        int _idarticulo = 0;

        PdfPTable _pdfTable = new PdfPTable(8);
        //PdfPTable tablae = new PdfPTable(6);


        PdfPCell _PdfPCell;
        PdfPCell _PdfPCellMat;

        public int idArticulo { get; set; }

        public string Titulo = "";
        MemoryStream _memoryStream = new MemoryStream();

        List<ArticuloRe> articulos = new List<ArticuloRe>();

        public ArticuloReContext db = new ArticuloReContext();

        public CMYKColor COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;
        public CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;
        // aqui los puedes pasar como parametro al reporte

        public byte[] PrepareReport(int idArticulo)

        {
            _idarticulo = idArticulo;
            articulos = this.GetArticulo(idArticulo);

            
            _documento = new Document(PageSize.LETTER, 0f, 0f, 0f, 0f);

            _documento.SetMargins(20f, 10f, 20f, 30f);

            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEvents();
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

            _documento.Open();

            // _pdfTable.SetWidths(new float[] { 40f, 40f, 40f, 260f, 60f, 60f, 50f, 50f});
            
            this.ReportHeader();
            this.ReportBody();

            //_pdfTable.HeaderRows = 4;
            //_documento.Add(_pdfTable);
            _documento.Close();


            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"Reporte por artículos.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();


        }
        
        public List<ArticuloRe> GetArticulo(int idArticulo)
        {
            List<ArticuloRe> rep = new List<ArticuloRe>();
            try
            {
                if (idArticulo != 0)
                {
                    string cadena = "select idarticulo, esKit, cref,descripcion,IDMoneda,IDFamilia,IDClaveUnidad,IDTipoArticulo, namefoto from Articulo where idArticulo = " + idArticulo + "order by descripcion";
                    rep = db.Database.SqlQuery<ArticuloRe>(cadena).ToList();
                }
                else
                {
                    string cadena = "select idarticulo, cref,descripcion,IDMoneda,IDFamilia,IDClaveUnidad,IDTipoArticulo from Articulo order by descripcion";
                    rep = db.Database.SqlQuery<ArticuloRe>(cadena).ToList();
                }
                //proveedor = db.Database.SqlQuery<Proveedor>("select * from Proveedores").ToList();
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }

            return rep;
        }
        
        public List<Kit> GetKit(int idArticulo)
        {
            List<Kit> rep = new List<Kit>();
            try
            {
                if (idArticulo != 0)
                {
                    string cadena = "SELECT a.descripcion,k.IDArticuloComp, k.Cantidad,k.porporcentaje, k.porcantidad FROM Articulo as a inner join kit as k on a.IDArticulo=k.IDArticuloComp where k.IDArticulo=" + idArticulo + "order by a.Descripcion";
                    rep = db.Database.SqlQuery<Kit>(cadena).ToList();
                }
                else
                {
                    string cadena = "select cref,descripcion,IDMoneda,IDFamilia,IDClaveUnidad,IDTipoArticulo from Articulo order by Descripcion";
                    rep = db.Database.SqlQuery<Kit>(cadena).ToList();
                }
                //proveedor = db.Database.SqlQuery<Proveedor>("select * from Proveedores").ToList();
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }

            return rep;
        }

        public List<Caracteristica> GetCaracteris(int idArticulo)
        {
            List<Caracteristica> rep = new List<Caracteristica>();
            try
            {
                if (idArticulo != 0)
                {
                    string cadena = "select * from Caracteristica where Articulo_idArticulo = " + idArticulo + "order by IDPresentacion, [Version]";
                    rep = db.Database.SqlQuery<Caracteristica>(cadena).ToList();
                }
                else
                {
                    string cadena = "select cref,descripcion,IDMoneda,IDFamilia,IDClaveUnidad,IDTipoArticulo from Articulo order by Descripcion";
                    rep = db.Database.SqlQuery<Caracteristica>(cadena).ToList();
                }
                //proveedor = db.Database.SqlQuery<Proveedor>("select * from Proveedores").ToList();
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }

            return rep;
        }

        public List<MatrizPrecio> GetMatriz(int idArticulo)
        {
            List<MatrizPrecio> rep = new List<MatrizPrecio>();
            try
            {
                if (idArticulo != 0)
                { //IDArticulo, RangInf, RangSup, Precio
                    string cadena = "select * from MatrizPrecio where IDArticulo =" + idArticulo;
                    rep = db.Database.SqlQuery<MatrizPrecio>(cadena).ToList();
                }
                else
                {
                    string cadena = "select cref,descripcion,IDMoneda,IDFamilia,IDClaveUnidad,IDTipoArticulo from Articulo order by Descripcion";
                    rep = db.Database.SqlQuery<MatrizPrecio>(cadena).ToList();
                }

            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }

            return rep;
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


            //Empresa empresa = new EmpresaContext().empresas.Find(2);


            if (_idarticulo == 0)
            {
                _Titulo = "REPORTE GENERAL DE ARTÍCULOS";

            }

            if (_idarticulo != 0)
            {
                _Titulo = "REPORTE POR ARTÍCULO";

            }

           

            float[] anchoColumnal = { 150f, 450f };

            PdfPTable tablal = new PdfPTable(anchoColumnal);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablal.SetTotalWidth(anchoColumnal);
            tablal.SpacingBefore = 0;
            tablal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablal.LockedWidth = true;
            Font _fontStyleencabezadol = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

            Empresa empresa = new EmpresaContext().empresas.Find(2);//se usa para buscar el logo de la empresa en la bd
            System.Drawing.Image logo = byteArrayToImage(empresa.Logo);
            Image jpg = Image.GetInstance(logo, BaseColor.WHITE);
            Paragraph paragraph = new Paragraph();
            paragraph.Alignment = Element.ALIGN_RIGHT;
            jpg.ScaleToFit(105f, 75F); //ancho y largo de la imagen
            jpg.Alignment = Image.ALIGN_RIGHT;
            jpg.SetAbsolutePosition(50f, 715f); //posición de la imagen comenzado en 0,0 parte inferir izquiera (+derecha/-izquierda,+arriba/-abajo)
            //_documento.Add(jpg);
            tablal.AddCell(jpg);

            paragraph.Clear();//ahora utilizo la clase Paragraph 
            paragraph.Font = new Font(FontFactory.GetFont("Arial", 18, Font.BOLD));
            paragraph.Alignment = Element.ALIGN_CENTER;
            paragraph.Add(_Titulo);
            PdfPCell cell2 = new PdfPCell();
            cell2.Border = Rectangle.NO_BORDER;
            cell2.PaddingTop = -7;
            cell2.AddElement(paragraph);
            cell2.Colspan = 3;
            paragraph.Clear();
            tablal.AddCell(cell2);
            _documento.Add(tablal);


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



            float[] anchoColumnasencart = { 80f, 150f, 55f, 125f, 90f, 50f, 50f };

            // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

            PdfPTable tablae = new PdfPTable(anchoColumnasencart);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablae.HorizontalAlignment = Element.ALIGN_LEFT;
            tablae.WidthPercentage = 100;

            // aqui podremos cambiar la fuente de lcientes y su tamanño

            Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);

            //CMYKColor COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

            //CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;

            Font _fontStyleEncabezado = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);


            if (_idarticulo != 0) //Indiviual
            {

                //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("CREF", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("Descripción", _fontStyleEncabezado));
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
            else//general
            {
               // _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("CREF", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

               // _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("Descripción", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("Tipo artículo", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("Familia", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

               // _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("Tipo unidad", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

               // _fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                _PdfPCell = new PdfPCell(new Phrase("Moneda", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);

                _PdfPCell = new PdfPCell(new Phrase("Mínimo de Venta", _fontStyleEncabezado));
                _PdfPCell.Border = 0;
                _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell.BackgroundColor = COLORDEREPORTE;
                tablae.AddCell(_PdfPCell);



                _pdfTable.CompleteRow();
                _documento.Add(tablae);
            }


            //_fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
            //_PdfPCell = new PdfPCell(new Phrase("   ", _fontStyle)); 
            //_PdfPCell.Colspan = _totalColumn;

            //_PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_PdfPCell.Border = 0;
            //_PdfPCell.BackgroundColor = BaseColor.WHITE;
            //_PdfPCell.ExtraParagraphSpace = 0;
            //_pdfTable.AddCell(_PdfPCell);

                  
    }


        public List<Caracteristica> GetlistCaracteristica(int idarticulo)
        {
            List<Caracteristica> presenta = new List<Caracteristica>();
            try
            {
                string cadenaf = "select* from dbo.Caracteristica where Articulo_IDArticulo = " + idarticulo + " order by ID";
                presenta = db.Database.SqlQuery<Caracteristica>(cadenaf).ToList();
            }
            catch (SqlException err)
            {
                //string mensajedeerror = err.Message;
            }
            return presenta;
        }




        private void ReportBody()
        {

             
            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            //  
            if (_idarticulo != 0)//individual
            {
                foreach (ArticuloRe articulo in articulos)
                {

                    try
                    {
                        Image jpg = Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/Upload/" + articulo.nameFoto + ""));

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
                    // creamos una tabla para imprimir los los datos del cliente
                    // como son 4 columnas a imprimir 600entre las 4 

                    float[] anchoColumnasclientes = { 80f, 220f};

                    // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor


                    string moneda = new c_MonedaContext().c_Monedas.Find(articulo.IDMoneda).ClaveMoneda;
                    string TIPOA = new ArticuloContext().TipoArticulo.Find(articulo.IDTipoArticulo).Descripcion;
                    string CLAVEU = new c_ClaveUnidadContext().c_ClaveUnidades.Find(articulo.IDClaveUnidad).Nombre;
                    string FAMILIA = new FamiliaContext().Familias.Find(articulo.IDFamilia).Descripcion;
                    //string PRESENTACION = new ArticuloContext().Caracteristica.Find(articulo.IDArticulo).Presentacion;


                    //iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(articulo.nameFoto);
                    //imagen.BorderWidth = 0;
                    //imagen.Alignment = Element.ALIGN_RIGHT;
                    //float percentage = 0.0f;
                    //percentage = 150 / imagen.Width;
                    //imagen.ScalePercent(percentage * 100);

                    PdfPTable tablaArticulo = new PdfPTable(anchoColumnasclientes);
                    //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                    tablaArticulo.SetTotalWidth(anchoColumnasclientes);
                    tablaArticulo.SpacingBefore = 3;
                    tablaArticulo.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.LockedWidth = true;

                    Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL);  // aqui podremos cambiar la fuente de lcientes y su tamanño
                    Font _fontStyleEnc = new Font(FontFactory.GetFont("Tahoma", 10f, 1));

                    PdfPCell _PdfPCell1 = new PdfPCell(new Phrase(articulo.Cref, _fontStylecliente));
                    _PdfPCell1.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell1.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell1);

                    PdfPCell _PdfPCell4 = new PdfPCell(new Phrase(articulo.Descripcion, _fontStylecliente));
                    _PdfPCell4.Border = 0;
                    _PdfPCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell4.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell4.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell4);

                    if (articulo.esKit == true)
                    {

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase("Éste artículo es kit", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tablaArticulo.AddCell(_PdfPCell);

                        _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        _PdfPCell = new PdfPCell(new Phrase("", _fontStylecliente));
                        _PdfPCell.Border = 0;
                        tablaArticulo.AddCell(_PdfPCell);
                    }


                    _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);

                    //-----
                    _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                    _PdfPCell = new PdfPCell(new Phrase("Tipo de artículo:", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);

                    PdfPCell _PdfPCell5 = new PdfPCell(new Phrase(TIPOA, _fontStylecliente));
                    _PdfPCell5.Border = 0;
                    _PdfPCell5.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell5.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell5.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell5);
                    //-----

                    _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                    _PdfPCell = new PdfPCell(new Phrase("Familia:", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);

                    PdfPCell _PdfPCell6 = new PdfPCell(new Phrase(FAMILIA, _fontStylecliente));
                    _PdfPCell6.Border = 0;
                    _PdfPCell6.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell6.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell6.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell6);

                    _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                    _PdfPCell = new PdfPCell(new Phrase("Tipo unidad:", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);
                    PdfPCell _PdfPCell7 = new PdfPCell(new Phrase(CLAVEU, _fontStylecliente));
                    _PdfPCell7.Border = 0;
                    _PdfPCell7.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell7.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell7.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell7);

                    _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                    _PdfPCell = new PdfPCell(new Phrase("Moneda:", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);

                    PdfPCell _PdfPCell8 = new PdfPCell(new Phrase(moneda, _fontStylecliente));
                    _PdfPCell8.Border = 0;
                    _PdfPCell8.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell8.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell8.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell8);

                    _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);

                    _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);

                    // añadimos la tabla al documento princlipal para que la imprimia
                    //_documento.Add(imagen);
                    _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                    _PdfPCell = new PdfPCell(new Phrase("Presentación: ", _fontStyle));
                    _PdfPCell.Border = 0;
                    tablaArticulo.AddCell(_PdfPCell);

                    _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                    _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _PdfPCell.Border = 0;

                    tablaArticulo.AddCell(_PdfPCell);
                                       
                    _documento.Add(tablaArticulo);

                    
                    List<Caracteristica> caracteristica = new List<Caracteristica>();
                    caracteristica = this.GetCaracteris(articulo.IDArticulo);

                    foreach (Caracteristica cara in caracteristica)
                    {
                        // creamos una tabla para imprimir los los datos del cliente
                        // como son 4 columnas a imprimir 600 entre las 4 

                        float[] ancho = { 300f };
                        PdfPTable tablac = new PdfPTable(ancho);
                        //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                        tablac.SetTotalWidth(ancho);
                        tablac.SpacingBefore = 3;
                        tablac.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablac.LockedWidth = true;

                        //if (cara.IDPresentacion >=1 )
                        //{
                        //    _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                        //    _PdfPCell = new PdfPCell(new Phrase("Presentación:", _fontStyle));
                        //    _PdfPCell.Border = 0;
                        //    tablac.AddCell(_PdfPCell);
                        //}
                      

                      
                        PdfPCell _PdfPCellc = new PdfPCell(new Phrase(cara.Presentacion, _fontStylecliente));
                        _PdfPCellc.Border = 0;
                        _PdfPCellc.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCellc.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCellc.BackgroundColor = BaseColor.WHITE;
                        tablac.AddCell(_PdfPCellc);


                        tablac.CompleteRow();
                        _documento.Add(tablac);
                    }

                    
                    List<MatrizPrecio> matriz = new List<MatrizPrecio>();
                    matriz = this.GetMatriz(articulo.IDArticulo);
                    //string cadena = "select count(idarticulo)  as Dato from MatrizPrecio where idarticulo =" + idArticulo + " group by idarticulo";
                    //matriz = db.Database.SqlQuery<MatrizPrecio>(cadena).ToList();

                    string cadena = "select count(idarticulo) as Dato FROM MatrizPrecio where idArticulo =" + articulo.IDArticulo;
                    ClsDatoEntero contarArticulo = db.Database.SqlQuery<ClsDatoEntero>(cadena).ToList()[0];
                    int cuenta = contarArticulo.Dato;

                    if (cuenta != 0 )//Condicion
                    {
                        float[] anchoColumnasencart = {200f, 200f, 200f};
                        PdfPTable tablaEncMatriz = new PdfPTable(anchoColumnasencart);

                        tablaEncMatriz.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaEncMatriz.WidthPercentage = 100;

                        CMYKColor COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

                        CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;

                        Font _fontStyleEncabezado = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
                        Font _fontStyleEncabezado2 = FontFactory.GetFont("Tahoma", 10f, 1);

                        //float[] anchoEncVacia2 = { 600f };
                        //PdfPTable tablaEncVacia2 = new PdfPTable(anchoEncVacia2);

                        //tablaEncVacia2.HorizontalAlignment = Element.ALIGN_LEFT;
                        //tablaEncVacia2.WidthPercentage = 100;


                        //_PdfPCell = new PdfPCell(new Phrase(" ", _fontStyleEncabezado2));
                        //_PdfPCell.Border = 0;
                        //_PdfPCell.BackgroundColor = BaseColor.WHITE;
                        //tablaEncVacia2.AddCell(_PdfPCell);

                        //tablaEncVacia2.CompleteRow();
                        //_documento.Add(tablaEncVacia2);

                        float[] anchoEncVacia1 = { 600f };
                        PdfPTable tablaEncVacia1 = new PdfPTable(anchoEncVacia1);

                        tablaEncVacia1.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaEncVacia1.WidthPercentage = 100;

                        _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyleEncabezado2));
                        _PdfPCell.Border = 0;
                        _PdfPCell.BackgroundColor = BaseColor.WHITE;
                        tablaEncVacia1.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase("Matriz de costos", _fontStyleEncabezado2));
                        _PdfPCell.Border = 0;
                        _PdfPCell.BackgroundColor = BaseColor.WHITE;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaEncVacia1.AddCell(_PdfPCell);

                        tablaEncVacia1.CompleteRow();
                        _documento.Add(tablaEncVacia1);

                        _PdfPCell = new PdfPCell(new Phrase("Rango inferior", _fontStyleEncabezado));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell.BackgroundColor = COLORDEREPORTE;
                        tablaEncMatriz.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase("Rango superior", _fontStyleEncabezado));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell.BackgroundColor = COLORDEREPORTE;
                        tablaEncMatriz.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase("Precio", _fontStyleEncabezado));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell.BackgroundColor = COLORDEREPORTE;
                        tablaEncMatriz.AddCell(_PdfPCell);


                        tablaEncMatriz.CompleteRow();

                        _documento.Add(tablaEncMatriz);

                        foreach (MatrizPrecio matPrec in matriz)
                        {

                            float[] anchoColContMatriz = {200f, 200f, 200f};

                            PdfPTable tablaContMatriz = new PdfPTable(anchoColContMatriz);
                            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                            tablaContMatriz.SetTotalWidth(anchoColContMatriz);
                            tablaContMatriz.SpacingBefore = 3;
                            tablaContMatriz.HorizontalAlignment = Element.ALIGN_LEFT;
                            tablaContMatriz.LockedWidth = true;

                           // Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                            PdfPCell _PdfPCell1Matriz = new PdfPCell(new Phrase(matPrec.RangInf.ToString(), _fontStylecliente));
                            _PdfPCell1Matriz.Border = 0;
                            _PdfPCell1Matriz.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell1Matriz.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _PdfPCell1Matriz.BackgroundColor = BaseColor.WHITE;
                            tablaContMatriz.AddCell(_PdfPCell1Matriz);
                            
                            PdfPCell _PdfPCell2Matriz = new PdfPCell(new Phrase(matPrec.RangSup.ToString(), _fontStylecliente));
                            _PdfPCell2Matriz.Border = 0;
                            _PdfPCell2Matriz.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell2Matriz.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _PdfPCell2Matriz.BackgroundColor = BaseColor.WHITE;
                            tablaContMatriz.AddCell(_PdfPCell2Matriz);

                            PdfPCell _PdfPCell3Matriz = new PdfPCell(new Phrase(matPrec.Precio.ToString("C"), _fontStylecliente));
                            _PdfPCell3Matriz.Border = 0;
                            _PdfPCell3Matriz.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell3Matriz.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _PdfPCell3Matriz.BackgroundColor = BaseColor.WHITE;
                            tablaContMatriz.AddCell(_PdfPCell3Matriz);
                            

                            tablaContMatriz.CompleteRow();
                            _documento.Add(tablaContMatriz);


                        }

                        if (articulo.esKit == true) //Separacion para kit
                        {
                            float[] anchoEncVacia = { 600f };
                            PdfPTable tablaEncVacia = new PdfPTable(anchoEncVacia);

                            tablaEncVacia.HorizontalAlignment = Element.ALIGN_LEFT;
                            tablaEncVacia.WidthPercentage = 100;

                            _PdfPCell = new PdfPCell(new Phrase(" ", _fontStyleEncabezado));
                            _PdfPCell.Border = 0;
                            _PdfPCell.BackgroundColor = BaseColor.WHITE;
                            tablaEncVacia.AddCell(_PdfPCell);


                            _PdfPCell = new PdfPCell(new Phrase("Contenido kit", _fontStyleEncabezado2));
                            _PdfPCell.Border = 0;
                            _PdfPCell.BackgroundColor = BaseColor.WHITE;
                            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            tablaEncVacia.AddCell(_PdfPCell);

                            tablaEncVacia.CompleteRow();
                            _documento.Add(tablaEncVacia);

                            //PdfPTable tablaEncVacia3 = new PdfPTable(anchoEncVacia);

                            //_PdfPCell = new PdfPCell(new Phrase("Contenido kit", _fontStyleEncabezado2));
                            //_PdfPCell.Border = 0;
                            //_PdfPCell.BackgroundColor = BaseColor.WHITE;
                            //_PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            //tablaEncVacia3.AddCell(_PdfPCell);

                            //tablaEncVacia3.CompleteRow();
                            //_documento.Add(tablaEncVacia3);
                        }



                    }
                    if (articulo.esKit == true)
                    {
                        //---------------------
                        float[] anchoColumnasencart = {150f, 150f, 150f, 150f};
                        PdfPTable tablaEncKit = new PdfPTable(anchoColumnasencart);

                        tablaEncKit.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaEncKit.WidthPercentage = 100;

                        CMYKColor COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

                        CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;

                        Font _fontStyleEncabezado = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);

                        _PdfPCell = new PdfPCell(new Phrase("CREF", _fontStyleEncabezado));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell.BackgroundColor = COLORDEREPORTE;
                        tablaEncKit.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase("Decripción", _fontStyleEncabezado));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell.BackgroundColor = COLORDEREPORTE;
                        tablaEncKit.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase("Cantidad", _fontStyleEncabezado));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell.BackgroundColor = COLORDEREPORTE;
                        tablaEncKit.AddCell(_PdfPCell);

                        _PdfPCell = new PdfPCell(new Phrase("Por", _fontStyleEncabezado));
                        _PdfPCell.Border = 0;
                        _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell.BackgroundColor = COLORDEREPORTE;
                        tablaEncKit.AddCell(_PdfPCell);

                        tablaEncKit.CompleteRow();

                        _documento.Add(tablaEncKit);
                        //---------------------

                        List<Kit> kit = new List<Kit>();
                        kit = this.GetKit(articulo.IDArticulo);

                        int cont = 0;


                        foreach (Kit k in kit)
                        {
                            cont++;

                            //--------------------------------
                            float[] anchoColContMatriz = {150f, 150f, 150f, 150f};

                            string des = new ArticuloContext().Articulo.Find(k.IDArticuloComp).Descripcion;
                            string cref = new ArticuloContext().Articulo.Find(k.IDArticuloComp).Cref;

                            PdfPTable tablaContEncKit = new PdfPTable(anchoColContMatriz);
                            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                            tablaContEncKit.SetTotalWidth(anchoColContMatriz);
                            tablaContEncKit.SpacingBefore = 3;
                            tablaContEncKit.HorizontalAlignment = Element.ALIGN_LEFT;
                            tablaContEncKit.LockedWidth = true;

                            // Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                            PdfPCell _PdfPCell1Kit = new PdfPCell(new Phrase(cref, _fontStylecliente));
                            _PdfPCell1Kit.Border = 0;
                            _PdfPCell1Kit.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell1Kit.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _PdfPCell1Kit.BackgroundColor = BaseColor.WHITE;
                            tablaContEncKit.AddCell(_PdfPCell1Kit);

                            PdfPCell _PdfPCell2Kit = new PdfPCell(new Phrase(des, _fontStylecliente));
                            _PdfPCell2Kit.Border = 0;
                            _PdfPCell2Kit.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell2Kit.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _PdfPCell2Kit.BackgroundColor = BaseColor.WHITE;
                            tablaContEncKit.AddCell(_PdfPCell2Kit);

                            PdfPCell _PdfPCell3Kit = new PdfPCell(new Phrase(k.Cantidad.ToString(), _fontStylecliente));
                            _PdfPCell3Kit.Border = 0;
                            _PdfPCell3Kit.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell3Kit.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _PdfPCell3Kit.BackgroundColor = BaseColor.WHITE;
                            tablaContEncKit.AddCell(_PdfPCell3Kit);

                            if (k.porcantidad == true)
                            {
                                PdfPCell _PdfPCell4Kit = new PdfPCell(new Phrase("cantidad", _fontStylecliente));
                                _PdfPCell4Kit.Border = 0;
                                _PdfPCell4Kit.HorizontalAlignment = Element.ALIGN_LEFT;
                                _PdfPCell4Kit.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _PdfPCell4Kit.BackgroundColor = BaseColor.WHITE;
                                tablaContEncKit.AddCell(_PdfPCell4Kit);
                            }
                            if (k.porporcentaje == true)
                            {
                                PdfPCell _PdfPCell5Kit = new PdfPCell(new Phrase("porcentaje", _fontStylecliente));
                                _PdfPCell5Kit.Border = 0;
                                _PdfPCell5Kit.HorizontalAlignment = Element.ALIGN_LEFT;
                                _PdfPCell5Kit.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _PdfPCell5Kit.BackgroundColor = BaseColor.WHITE;
                                tablaContEncKit.AddCell(_PdfPCell5Kit);
                            }
                            
                            tablaContEncKit.CompleteRow();
                            _documento.Add(tablaContEncKit);
                            //--------------------------------
                            
                        }

                    }

                }
            }
            if (_idarticulo == 0)
            {
                foreach (ArticuloRe articulo in articulos)
                {

                    // creamos una tabla para imprimir los los datos del cliente
                    // como son 4 columnas a imprimir 600entre las 4 

                    float[] anchoColumnasclientes = {80f, 150f, 55f, 125f, 90f, 50f, 50f};

                    // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor
                    
                    string moneda = new c_MonedaContext().c_Monedas.Find(articulo.IDMoneda).ClaveMoneda;
                    string TIPOA = new ArticuloContext().TipoArticulo.Find(articulo.IDTipoArticulo).Descripcion;
                    string CLAVEU = new c_ClaveUnidadContext().c_ClaveUnidades.Find(articulo.IDClaveUnidad).Nombre;
                    string FAMILIA = new FamiliaContext().Familias.Find(articulo.IDFamilia).Descripcion;

                    PdfPTable tablaArticulo = new PdfPTable(anchoColumnasclientes);
                    //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                    tablaArticulo.SetTotalWidth(anchoColumnasclientes);
                    tablaArticulo.SpacingBefore = 3;
                    tablaArticulo.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaArticulo.LockedWidth = true;

                    Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                    PdfPCell _PdfPCell1 = new PdfPCell(new Phrase(articulo.Cref, _fontStylecliente));
                    _PdfPCell1.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell1.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell1);
                                        
                    PdfPCell _PdfPCell4 = new PdfPCell(new Phrase(articulo.Descripcion, _fontStylecliente));
                    _PdfPCell4.Border = 0;
                    _PdfPCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell4.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell4.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell4);

                    PdfPCell _PdfPCell5 = new PdfPCell(new Phrase(TIPOA, _fontStylecliente));
                    _PdfPCell5.Border = 0;
                    _PdfPCell5.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell5.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell5.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell5);

                    PdfPCell _PdfPCell6 = new PdfPCell(new Phrase(FAMILIA, _fontStylecliente));
                    _PdfPCell6.Border = 0;
                    _PdfPCell6.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell6.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell6.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell6);

                    PdfPCell _PdfPCell7 = new PdfPCell(new Phrase(CLAVEU, _fontStylecliente));
                    _PdfPCell7.Border = 0;
                    _PdfPCell7.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell7.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell7.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell7);

                    PdfPCell _PdfPCell8 = new PdfPCell(new Phrase(moneda, _fontStylecliente));
                    _PdfPCell8.Border = 0;
                    _PdfPCell8.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell8.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell8.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell8);
                    decimal minimov = 0;
                    try
                    {
                        minimov = new ArticuloContext().Articulo.Find(articulo.IDArticulo).MinimoVenta;
                        
                    }
                    catch ( Exception err )
                    {

                    }
                    

                    PdfPCell _PdfPCell9 = new PdfPCell(new Phrase(minimov.ToString(), _fontStylecliente));
                    _PdfPCell8.Border = 0;
                    _PdfPCell8.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell8.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell8.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell9);



                    tablaArticulo.CompleteRow();

                    // añadimos la tabla al documento princlipal para que la imprimia

                    _documento.Add(tablaArticulo);



                    //Presentaciones del articulo
                    List<Caracteristica> presenta = new List<Caracteristica>();
                    presenta = GetlistCaracteristica(articulo.IDArticulo);
                    var numero = 0;
                    foreach (Caracteristica pres in presenta)
                    {
                        float[] anchoColumnasPresenta = { 50f, 450f };
                        numero += 1;

                        PdfPTable tablaPresenta = new PdfPTable(anchoColumnasPresenta);
                        //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

                        tablaPresenta.SetTotalWidth(anchoColumnasPresenta);
                        tablaPresenta.SpacingBefore = 10;
                        tablaPresenta.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tablaPresenta.LockedWidth = true;
                        Font _fontStylePresenta = new Font(Font.FontFamily.TIMES_ROMAN, 6, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

                        PdfPCell _tablaPresenta1 = new PdfPCell(new Phrase(numero.ToString(), _fontStylePresenta));
                        _tablaPresenta1.Border = 0;
                        _tablaPresenta1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        _tablaPresenta1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _tablaPresenta1.BackgroundColor = BaseColor.WHITE; ;
                        _tablaPresenta1.FixedHeight = 10f;
                        tablaPresenta.AddCell(_tablaPresenta1);

                        PdfPCell _tablaPresenta2 = new PdfPCell(new Phrase(pres.Presentacion, _fontStylePresenta));
                        _tablaPresenta2.Border = 0;
                        _tablaPresenta2.FixedHeight = 10f;
                        _tablaPresenta2.HorizontalAlignment = Element.ALIGN_LEFT;
                        _tablaPresenta2.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _tablaPresenta2.BackgroundColor = BaseColor.WHITE; ;
                        tablaPresenta.AddCell(_tablaPresenta2);

                        tablaPresenta.CompleteRow();
                        _documento.Add(tablaPresenta);
                    }

                }
            }
            
        }


       
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
                    cb.SetFontAndSize(bf, 7);
                    cb.SetTextMatrix(document.PageSize.GetRight(70), document.PageSize.GetBottom(30));
                    //cb.MoveText(500,30);
                    //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
                    cb.ShowText(text);
                    cb.EndText();
                    float len = bf.GetWidthPoint(text, 7);
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

                //headerTemplate.BeginText();
                //headerTemplate.SetFontAndSize(bf, 12);
                //headerTemplate.SetTextMatrix(0, 0);
                //headerTemplate.ShowText((writer.PageNumber - 1).ToString());
                //headerTemplate.EndText();

                footerTemplate.BeginText();
                footerTemplate.SetFontAndSize(bf, 7);
                //footerTemplate.MoveText(550,30);
                footerTemplate.SetTextMatrix(0, 0);
                footerTemplate.ShowText((writer.PageNumber).ToString());
                footerTemplate.EndText();
            }
        }        
    }

    public class ArticuloRe
    {
        [Key]
        public int IDArticulo { get; set; }

        public string Cref { get; set; }

        public string Descripcion { get; set; }
        public bool esKit { get; set; }
        public int IDMoneda { get; set; }
        public int IDFamilia { get; set; }
        public int IDTipoArticulo { get; set; }
        public int IDClaveUnidad { get; set; }
        public string Presentacion { get; set; }
        public string nameFoto { get; set; }

    }


    public class ArticuloReContext : DbContext
    {
        public ArticuloReContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ArticuloReContext>(null);
        }
        public DbSet<Caracteristica> Caracteristica { get; set; }
        //public DbSet<ClientesPedido> Clientes { get; set; }
        //public DbSet<detpedidoReporte> DetPedido { get; set; }

    }
}