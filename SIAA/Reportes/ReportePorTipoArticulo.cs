using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data.Entity;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Comercial;

//↓↑--

namespace SIAAPI.Reportes
{
    public class ReportePorTipoArticulo
    {
        //--Variables iTextSharp
        Document documento;
        PdfWriter escritor;
        PdfPTable tablaPdf;//se usa tambien en prepare report
        PdfPTable tablaPdf1;
        PdfPCell celdaPdf;
        
        TipoArticuloGetSetContext db = new TipoArticuloGetSetContext();
        MemoryStream flujoMemoria = new MemoryStream();
        static CMYKColor COLORDEREPORTE = new ClsColoresReporte(Properties.Settings.Default.ColorReporte).color;
        static CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(Properties.Settings.Default.ColorFuenteEncabezado).color;
        Font fontFecha = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // fuente para: encabezado documento (titulo y fecha de impresión)
        Font fontEncCont = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE); // fuente para:
        Font fontCont = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL); // fuente para:
        //Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD); //fuente para:
        //Font _fontStyleencabezado = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD); //fuente para:
        //Font _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);
        //--Variables
        string _Titulo = "";
        public int IDTipoArticulo { get; set; }//variable que se usa en la vista debe ir como en la BD

        public byte[] prepararReporte(int _IDTipoArticulo)

        {
            IDTipoArticulo = _IDTipoArticulo;

            

            documento = new Document(PageSize.LETTER, 20f, 10f, 20f, 30f);
            //documento.SetMargins(20f, 10f, 20f, 30f);
            tablaPdf = new PdfPTable(8);
            tablaPdf1 = new PdfPTable(5);
            escritor = PdfWriter.GetInstance(documento, HttpContext.Current.Response.OutputStream);
            escritor.PageEvent = new ITextEvents();
            tablaPdf.WidthPercentage = 100;
            tablaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaPdf1.WidthPercentage = 100;
            tablaPdf1.HorizontalAlignment = Element.ALIGN_LEFT;
            documento.Open();

            this.encabezadoReporte();
            this.cuerpoReporte();

            documento.Close();

            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"Reporte por tipo artículo.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(documento);
            HttpContext.Current.Response.End();

            return flujoMemoria.ToArray();
        }

        public List<TipoArticuloGetSet> GetTipoArticulo(int idTipoArticulo)
        {
            List<TipoArticuloGetSet> rep = new List<TipoArticuloGetSet>();
            try
            {
                if (idTipoArticulo != 0)
                {
                    string cadena = "select distinct idarticulo, cref,descripcion,IDMoneda,IDFamilia,IDClaveUnidad,IDTipoArticulo, namefoto from Articulo where IDTipoArticulo =" + idTipoArticulo + " order by Descripcion";
                    rep = db.Database.SqlQuery<TipoArticuloGetSet>(cadena).ToList();
                }
                else
                {
                    string cadena = "select distinct idarticulo, cref,descripcion,IDMoneda,IDFamilia,IDClaveUnidad,IDTipoArticulo, namefoto from Articulo order by IDTipoArticulo, Descripcion";
                    rep = db.Database.SqlQuery<TipoArticuloGetSet>(cadena).ToList();
                }
            }
            catch (SqlException err)
            {
                string mensajeError = err.Message;
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

        private void encabezadoReporte()
        {
            _Titulo = "REPORTE GENERAL TIPO ARTÍCULO";
            //↓--Logo desde BD
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
            documento.Add(tablal);


            float[] anchoColumnasencabezado = { 500f, 100f };
            PdfPTable tablaencabezado = new PdfPTable(anchoColumnasencabezado);

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
            documento.Add(tablaencabezado);
            //↑--Creacion de tabla para encabezado documento

            //↓--Creacion de tabla para encabezado de tabla de contenido
            float[] anchoColEncCont = { 100f, 250f, 70f, 80f, 50f, 50f };//ancho de las columas del encabezado de tabla de contenido
            PdfPTable tablaEncCont = new PdfPTable(anchoColEncCont);

            tablaEncCont.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaEncCont.WidthPercentage = 100;

            celdaPdf = new PdfPCell(new Phrase("CREF", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Descripción", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Tipo artículo", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Tipo unidad", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Moneda", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);

            celdaPdf = new PdfPCell(new Phrase("Mínimo de Venta", fontEncCont));
            celdaPdf.Border = 0;
            celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            celdaPdf.BackgroundColor = COLORDEREPORTE;
            tablaEncCont.AddCell(celdaPdf);






            //tablaPdf.CompleteRow();//tablaPdf esta declarada arriba 
            documento.Add(tablaEncCont);
            //↑--Creacion de tabla para encabezado de tabla de contenido
        }




        public List<TipoArticulo> getTA(int idTipoArticulo)
        {
            List<TipoArticulo> TP = new List<TipoArticulo>();
            try
            {
                if (idTipoArticulo != 0)
                {
                    string cadena = "Select *from TipoArticulo where idTipoArticulo=" + idTipoArticulo;
                    TP = db.Database.SqlQuery<TipoArticulo>(cadena).ToList();
                }
                else if (idTipoArticulo==0)
                {
                    string cadena = "Select *from TipoArticulo";
                    TP = db.Database.SqlQuery<TipoArticulo>(cadena).ToList();
                }
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return TP;
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

        List<TipoArticulo> TP = new List<TipoArticulo>();
        private void cuerpoReporte()
        {
            //_fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            TP = this.getTA(IDTipoArticulo);

            foreach (TipoArticulo TIP in TP)
            {
                Font _fontStylefam = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);
                CMYKColor COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;
                CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;
                Font _fontStyleEncabezado = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);

                float[] anchoColumnasAlmacen = { 580f };

                PdfPTable tablafamilia = new PdfPTable(anchoColumnasAlmacen);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

                tablafamilia.SetTotalWidth(anchoColumnasAlmacen);
                tablafamilia.SpacingBefore = 10;
                tablafamilia.HorizontalAlignment = Element.ALIGN_LEFT;
                tablafamilia.LockedWidth = true;
                tablafamilia.WidthPercentage = 100;
                tablafamilia.HorizontalAlignment = Element.ALIGN_LEFT;
                Font _fontStyleAlmacen = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

                PdfPCell _tablafamilia1 = new PdfPCell(new Phrase( TIP.Descripcion, _fontStyleEncabezado));
                _tablafamilia1.Border = 0;
                _tablafamilia1.HorizontalAlignment = Element.ALIGN_CENTER;
                _tablafamilia1.VerticalAlignment = Element.ALIGN_MIDDLE;
                _tablafamilia1.BackgroundColor = COLORDEREPORTE;
                _tablafamilia1.FixedHeight = 20f;
                tablafamilia.AddCell(_tablafamilia1);

                
                tablafamilia.CompleteRow();
                documento.Add(tablafamilia);


                List<TipoArticuloGetSet> articulos = new List<TipoArticuloGetSet>();//Lo usa el foreach
                articulos = this.GetTipoArticulo(TIP.IDTipoArticulo);
                foreach (TipoArticuloGetSet articulo in articulos)
            {
                float[] anchoColCont = { 100f, 250f, 70f, 80f, 50f, 50f };

                string moneda = new c_MonedaContext().c_Monedas.Find(articulo.IDMoneda).ClaveMoneda;
                string tipoart = new ArticuloContext().TipoArticulo.Find(articulo.IDTipoArticulo).Descripcion;
                string claveuni = new c_ClaveUnidadContext().c_ClaveUnidades.Find(articulo.IDClaveUnidad).Nombre;

                PdfPTable tablaCont = new PdfPTable(anchoColCont);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaCont.SetTotalWidth(anchoColCont);
                tablaCont.SpacingBefore = 3;
                tablaCont.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaCont.LockedWidth = true;

                //Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                PdfPCell celdaPdf = new PdfPCell(new Phrase(articulo.Cref, fontCont));
                celdaPdf.Border = 0;
                celdaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf);

                PdfPCell celdaPdf1 = new PdfPCell(new Phrase(articulo.Descripcion, fontCont));
                celdaPdf1.Border = 0;
                celdaPdf1.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf1.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf1.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf1);

                PdfPCell celdaPdf2 = new PdfPCell(new Phrase(tipoart, fontCont));
                celdaPdf2.Border = 0;
                celdaPdf2.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf2.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf2.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf2);

                PdfPCell celdaPdf3 = new PdfPCell(new Phrase(claveuni, fontCont));
                celdaPdf3.Border = 0;
                celdaPdf3.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf3.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf3.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf3);

                PdfPCell celdaPdf4 = new PdfPCell(new Phrase(moneda, fontCont));
                celdaPdf4.Border = 0;
                celdaPdf4.HorizontalAlignment = Element.ALIGN_LEFT;
                celdaPdf4.VerticalAlignment = Element.ALIGN_MIDDLE;
                celdaPdf4.BackgroundColor = BaseColor.WHITE;
                tablaCont.AddCell(celdaPdf4);

                    decimal minimov = 0;
                    try
                    {
                        minimov = new ArticuloContext().Articulo.Find(articulo.IDArticulo).MinimoVenta;

                    }
                    catch (Exception err)
                    {

                    }


                    PdfPCell _PdfPCell9 = new PdfPCell(new Phrase(minimov.ToString(), fontCont));
                    _PdfPCell9.Border = 0;
                    _PdfPCell9.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell9.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell9.BackgroundColor = BaseColor.WHITE;
                    tablaCont.AddCell(_PdfPCell9);




                    tablaCont.CompleteRow();

                // añadimos la tabla al documento princlipal para que la imprimia
                //_documento.Add(imagen);
                documento.Add(tablaCont);


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
                        documento.Add(tablaPresenta);
                    }


                }
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
        #endregion
    }

    public class TipoArticuloGetSet
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

    public class TipoArticuloGetSetContext : DbContext
    {
        public TipoArticuloGetSetContext() : base("name=DefaultConnection")

        {
            Database.SetInitializer<UnidadGetSetContext>(null);
        }
        public DbSet<Caracteristica> Caracteristica { get; set; }
        public DbSet<Familia> Familia { get; set; }
        public DbSet<c_Moneda> Moneda { get; set; }
        public DbSet<Articulo> ArticuloC { get; set; }
    }
}