using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.Models.Administracion;
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
    public class ReportePorFamilia
    {
        
        private Document _documento;
        Font _fontStyle;
        PdfWriter _writer;
        int _totalColumn = 8;

        string _Titulo = "";
        
        PdfPTable _pdfTable = new PdfPTable(8);
        PdfPTable tablae = new PdfPTable(5);
        
        PdfPCell _PdfPCell;

        public int IDArticulo { get; set; }
        public int IDFamilia { get; set; }
        public int IDMoneda { get; set; }
        
        MemoryStream _memoryStream = new MemoryStream();

        List<ArticuloFE> articulos = new List<ArticuloFE>();
       
        public ArticuloFEContext db = new ArticuloFEContext();
        // aqui los puedes pasar como parametro a l reporte

        public byte[] PrepareReport(int _IDFamilia)

        {
            IDFamilia = _IDFamilia;

            articulos = this.GetFamilia(IDFamilia);

            #region
            _documento = new Document(PageSize.LETTER, 0f, 0f, 0f, 0f);

            _documento.SetMargins(20f, 10f, 20f, 30f);

            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEvents();
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            tablae.WidthPercentage = 100;
            tablae.HorizontalAlignment = Element.ALIGN_LEFT;
            _documento.Open();

            // _pdfTable.SetWidths(new float[] { 40f, 40f, 40f, 260f, 60f, 60f, 50f, 50f});
            #endregion
            this.ReportHeader();
            this.ReportBody();

            //  _pdfTable.HeaderRows = 4;
            //_documento.Add(_pdfTable);
            _documento.Close();
            
            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"Reporte por familia.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();


        }
        
        public List<ArticuloFE> GetFamilia(int idFamilia)
        {
            List<ArticuloFE> rep = new List<ArticuloFE>();
            try
            {
                if (idFamilia != 0)
                {
                    string cadena = "select idarticulo, cref,descripcion,IDMoneda,IDFamilia,IDClaveUnidad,IDTipoArticulo, namefoto, MinimoVenta from Articulo where idfamilia = " + idFamilia + " and obsoleto = 0 order by cref";
                    rep = db.Database.SqlQuery<ArticuloFE>(cadena).ToList();
                }
                else
                {
                    string cadena = "select distinct idarticulo, cref,descripcion,IDMoneda,IDFamilia,IDClaveUnidad,IDTipoArticulo, namefoto, MinimoVenta from Articulo where obsoleto = 0 order by cref";
                    rep = db.Database.SqlQuery<ArticuloFE>(cadena).ToList();
                }
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }

            return rep;
        }

        //Obtener la lista de todos las familias
        public List<Familia> Getlistfamilia(int idfamilia)
        {
            List<Familia> familia = new List<Familia>();
            try
            {
                if (idfamilia == 0)
                {
                    string cadenaf = "select * from [dbo].[Familia] order by Descripcion";
                    familia = db.Database.SqlQuery<Familia>(cadenaf).ToList();
                }
                else
                {
                    string cadenaf = "select * from [dbo].[Familia] where IDFamilia = " + idfamilia + " order by Descripcion";
                    familia = db.Database.SqlQuery<Familia>(cadenaf).ToList();

                }
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return familia;
        }
        
              //Obtener la lista de todos las presentaciones del articulo
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

        private void ReportHeader()
        {
            _Titulo = "REPORTE GENERAL POR FAMILIA";

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

            float[] anchoColumnasencart = { 100f, 250f, 70f, 50f, 50f, 80f};

            PdfPTable tablae = new PdfPTable(anchoColumnasencart);

            tablae.HorizontalAlignment = Element.ALIGN_LEFT;
            tablae.WidthPercentage = 100;

            Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);
            CMYKColor COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;
            CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;
            Font _fontStyleEncabezado = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);

            _PdfPCell = new PdfPCell(new Phrase("CREF", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("Descripción", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("TipoArtículo", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("Tipo unidad", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

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

        private void ReportBody()
        {
            //_fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            List<Familia> fam = new List<Familia>();
            fam = Getlistfamilia(IDFamilia);
            foreach (Familia familias in fam)
            {

                // aqui podremos cambiar la fuente de lcientes y su tamanño

                Font _fontStylefam = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);
                CMYKColor COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;
                CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;
                Font _fontStyleEncabezado = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);

                string _codfam = familias.CCodFam;
                string _desFam = familias.Descripcion;
                float[] anchoColumnasAlmacen = { 200f, 380f };

                PdfPTable tablafamilia = new PdfPTable(anchoColumnasAlmacen);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

                tablafamilia.SetTotalWidth(anchoColumnasAlmacen);
                tablafamilia.SpacingBefore = 10;
                tablafamilia.HorizontalAlignment = Element.ALIGN_LEFT;
                tablafamilia.LockedWidth = true;
                tablafamilia.WidthPercentage = 100;
                tablafamilia.HorizontalAlignment = Element.ALIGN_LEFT;
                Font _fontStyleAlmacen = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

                PdfPCell _tablafamilia1 = new PdfPCell(new Phrase("Código: " + _codfam, _fontStyleEncabezado));
                _tablafamilia1.Border = 0;
                _tablafamilia1.HorizontalAlignment = Element.ALIGN_CENTER;
                _tablafamilia1.VerticalAlignment = Element.ALIGN_MIDDLE;
                _tablafamilia1.BackgroundColor = COLORDEREPORTE;
                _tablafamilia1.FixedHeight = 20f;
                tablafamilia.AddCell(_tablafamilia1);

                PdfPCell _tablafamilia2 = new PdfPCell(new Phrase("Familia: " + _desFam, _fontStyleEncabezado));
                _tablafamilia2.Border = 0;
                _tablafamilia2.FixedHeight = 10f;
                _tablafamilia2.HorizontalAlignment = Element.ALIGN_LEFT;
                _tablafamilia2.VerticalAlignment = Element.ALIGN_MIDDLE;
                _tablafamilia2.BackgroundColor = COLORDEREPORTE;
                tablafamilia.AddCell(_tablafamilia2);
                tablafamilia.CompleteRow();
                _documento.Add(tablafamilia);


                List<ArticuloFE> articulos = new List<ArticuloFE>();
                articulos = GetFamilia(IDFamilia);
                foreach (ArticuloFE articulo in articulos)
                {
                    // creamos una tabla para imprimir los los datos del articulo
                    // como son 5 columnas a imprimir 600entre las 5

                    float[] anchoColumnasclientes = { 100f, 250f, 70f, 50f, 50f, 80f };



                    string moneda = new c_MonedaContext().c_Monedas.Find(articulo.IDMoneda).ClaveMoneda;
                    string familia = new FamiliaContext().Familias.Find(articulo.IDFamilia).Descripcion;
                    string CLAVEU = new c_ClaveUnidadContext().c_ClaveUnidades.Find(articulo.IDClaveUnidad).Nombre;
                    string tipo = new ArticuloContext().TipoArticulo.Find(articulo.IDTipoArticulo).Descripcion;

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

                    PdfPCell _PdfPCell5 = new PdfPCell(new Phrase(tipo, _fontStylecliente));
                    _PdfPCell5.Border = 0;
                    _PdfPCell5.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell5.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell5.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell5);

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

                    PdfPCell _PdfPCell9 = new PdfPCell(new Phrase(articulo.MinimoVenta.ToString(), _fontStylecliente));
                    _PdfPCell9.Border = 0;
                    _PdfPCell9.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell9.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell9.BackgroundColor = BaseColor.WHITE;
                    tablaArticulo.AddCell(_PdfPCell9);

                    tablaArticulo.CompleteRow();
                    _documento.Add(tablaArticulo);
                    
                    //Presentaciones del articulo
                    List<Caracteristica> presenta = new List<Caracteristica>();
                    presenta = GetlistCaracteristica(articulo.IDArticulo);
                    var numero = 0;
                    foreach (Caracteristica pres in presenta)
                    {
                    float[] anchoColumnasPresenta = { 50f, 450f};
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
            public override void OnEndPage(PdfWriter writer, Document document)
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
    
    public class ArticuloFE
    {
        [Key]
        public int IDArticulo { get; set; }

        public string Cref { get; set; }

        public string Descripcion { get; set; }

        public int IDMoneda { get; set; }
        public int IDFamilia { get; set; }
        public int IDTipoArticulo { get; set; }
        public int IDClaveUnidad { get; set; }
        public string Presentacion { get; set; }
        public string nameFoto { get; set; }
        public decimal MinimoVenta { get; set; }

        //public virtual Vendedor vendedor { get; set; }
    }
    
    public class ArticuloFEContext : DbContext
    {
        public ArticuloFEContext() : base("name=DefaultConnection")

        {
            Database.SetInitializer<ArticuloFEContext>(null);
        }
        public DbSet<Caracteristica> Caracteristica { get; set; }
        public DbSet<Familia> Familia { get; set; }
        public DbSet<c_Moneda> Moneda { get; set; }
        public DbSet<Articulo> ArticuloC { get; set; }
    }

}