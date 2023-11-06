using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Reportes
{
    public class InventarioPorAlmacenA
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;
        PdfWriter _writer;
        int _totalColumn = 8;



        PdfPTable _pdfTable = new PdfPTable(8);
        PdfPTable tablae = new PdfPTable(8);


        PdfPCell _PdfPCell;
        [DisplayName("Almacen")]

        public int idalmacen { get; set; }

        [DisplayName("Familia")]
        public int idfamilia { get; set; }


        MemoryStream _memoryStream = new MemoryStream();


        List<ReporteInventarioArt> inventario = new List<ReporteInventarioArt>();

        #endregion

        public InventarioAlmacenContext db = new InventarioAlmacenContext();
        // aqui los puedes pasar como parametro al reporte

        public byte[] PrepareReport(int almacen, int familia)
        {
            idfamilia = familia;
            idalmacen = almacen;

            inventario = this.GetInvAlm(idalmacen, idfamilia);

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
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"ReporteArticulos.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();


        }

        //Obtener la lista del encabezado
        public ReporteInventarioAlmacen GetInvAlmEnc(int idalmacen, int idfamilia)
        {
            ReporteInventarioAlmacen encabezado = new ReporteInventarioAlmacen();

            VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambioCadena(GETDATE(),'USD','MXN') as TC").ToList()[0];
            encabezado.TC = cambio.TC;
            if (idalmacen == 0 && idfamilia == 0)
            {
                encabezado.Rep = 100;
                encabezado.Titulo = "REPORTE GENERAL DE ARTÍCULOS";

            }
            if (idalmacen == 0 && idfamilia != 0)
            {
                encabezado.Rep = 101;
                string cadenaE = "select IDFamilia, CCodFam, Descripcion from familia  where IDfamilia = " + idfamilia + "";
                ReporteInventarioFam fam = db.Database.SqlQuery<ReporteInventarioFam>(cadenaE).ToList()[0];
                encabezado.Familia = fam.Descripcion;
                encabezado.Titulo = "REPORTE GENERAL DE ARTÍCULOS, DE LA FAMILIA " + encabezado.Familia + "";
            }
            if (idalmacen != 0 && idfamilia == 0)
            {
                encabezado.Rep = 110;
                string cadenaE = "select IDAlmacen, CodAlm, Descripcion  from almacen  where IDAlmacen = " + idalmacen + "";
                ReporteInventarioAlm enc = db.Database.SqlQuery<ReporteInventarioAlm>(cadenaE).ToList()[0];
                encabezado.Almacen = enc.Descripcion;
                encabezado.Titulo = "REPORTE GENERAL DE ARTÍCULOS, DEL ALMACEN " + encabezado.Almacen + "";
            }
            if (idalmacen != 0 && idfamilia != 0)
            {
                encabezado.Rep = 111;
                string cadenaE = "select IDFamilia, CCodFam, Descripcion from familia  where IDfamilia = " + idfamilia + "";
                ReporteInventarioFam fam = db.Database.SqlQuery<ReporteInventarioFam>(cadenaE).ToList()[0];
                encabezado.Familia = fam.Descripcion;
                string cadenaE1 = "select IDAlmacen, CodAlm, Descripcion  from almacen  where IDAlmacen = " + idalmacen + "";
                ReporteInventarioAlm enc = db.Database.SqlQuery<ReporteInventarioAlm>(cadenaE1).ToList()[0];
                encabezado.Almacen = enc.Descripcion;
                encabezado.Titulo = "REPORTE GENERAL DE ARTÍCULOS, DEL ALMACEN " + encabezado.Almacen + " DE LA FAMILIA " + encabezado.Familia + "";
            }

            
            return encabezado;
        }

        //Obtener la lista de articulos del inventario
        public List<ReporteInventarioArt> GetInvAlm(int idalmacen, int idfamilia)
        {

            List<ReporteInventarioArt> inventario = new List<ReporteInventarioArt>();
            try
            {
                if (idalmacen == 0 && idfamilia == 0)
                {
                    string cadena = "select * from ReporteInventarioArt  where existencia>0 order by IDAlmacen, IDFamilia, Cref";
                    //"select I.IDInventarioAlmacen, I.IDAlmacen, C.ID as IDCaracteristica, C.Articulo_IDArticulo as IDArticulo, F.IDFamilia, I.Existencia, I.PorLlegar, I.Apartado, I.Disponibilidad from[dbo].[InventarioAlmacen] as I inner join (dbo.Caracteristica as C inner join(dbo.Articulo as A inner join dbo.Familia as F on A.IDFamilia = F.IDFamilia)on C.Articulo_IDArticulo = A.IDArticulo ) on I.IDCaracteristica = C.ID order by IDAlmacen, IDFamilia";
                    inventario = db.Database.SqlQuery<ReporteInventarioArt>(cadena).ToList();

                }
                if (idalmacen == 0 && idfamilia !=0)
                {
                    string cadena = "select * from ReporteInventarioArt where existencia>0 and  IDFamilia = "+ idfamilia +" order by IDAlmacen, IDFamilia, Cref";
                    //"select I.IDInventarioAlmacen, I.IDAlmacen, C.ID as IDCaracteristica, C.Articulo_IDArticulo as IDArticulo, F.IDFamilia, I.Existencia, I.PorLlegar, I.Apartado, I.Disponibilidad from[dbo].[InventarioAlmacen] as I inner join (dbo.Caracteristica as C inner join(dbo.Articulo as A inner join dbo.Familia as F on A.IDFamilia = F.IDFamilia)on C.Articulo_IDArticulo = A.IDArticulo ) on I.IDCaracteristica = C.ID where F.IDFamilia = " + idfamilia + " order by IDAlmacen, IDFamilia";
                    inventario = db.Database.SqlQuery<ReporteInventarioArt>(cadena).ToList();
                }
                if (idalmacen !=0 && idfamilia == 0)
                {
                    string cadena = "select * from ReporteInventarioArt where existencia>0 and IDAlmacen = "+ idalmacen+"  order by IDAlmacen, IDFamilia, Cref";
                    //"select I.IDInventarioAlmacen, I.IDAlmacen, C.ID as IDCaracteristica, C.Articulo_IDArticulo as IDArticulo, F.IDFamilia, I.Existencia, I.PorLlegar, I.Apartado, I.Disponibilidad from[dbo].[InventarioAlmacen] as I inner join (dbo.Caracteristica as C inner join(dbo.Articulo as A inner join dbo.Familia as F on A.IDFamilia = F.IDFamilia)on C.Articulo_IDArticulo = A.IDArticulo ) on I.IDCaracteristica = C.ID where I.IDAlmacen = " + idalmacen + " order by IDAlmacen, IDFamilia";
                    inventario = db.Database.SqlQuery<ReporteInventarioArt>(cadena).ToList();
                }
                if (idalmacen !=0 && idfamilia !=0)
                {
                    string cadena = " select * from ReporteInventarioArt  where existencia>0 and  IDAlmacen = " + idalmacen + " and IDFamilia= " + idfamilia + " order by IDAlmacen, IDFamilia, Cref";
                        ////"select I.IDInventarioAlmacen, I.IDAlmacen, C.ID as IDCaracteristica, C.Articulo_IDArticulo as IDArticulo, F.IDFamilia, I.Existencia, I.PorLlegar, I.Apartado, I.Disponibilidad from[dbo].[InventarioAlmacen] as I inner join (dbo.Caracteristica as C inner join(dbo.Articulo as A inner join dbo.Familia as F on A.IDFamilia = F.IDFamilia)on C.Articulo_IDArticulo = A.IDArticulo ) on I.IDCaracteristica = C.ID where F.IDFamilia = " + idfamilia + " and I.IDAlmacen = " + idalmacen + " order by IDAlmacen, IDFamilia";
                    inventario = db.Database.SqlQuery<ReporteInventarioArt>(cadena).ToList();
                }

            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return inventario;
        }

        //Obtener la lista de todos los almacenes
        public List<Almacen> Getlistalm(int idalmacen)
        {
            List<Almacen> almacen = new List<Almacen>();
            try
            {
                if (idalmacen == 0)
                {
                    string cadenaa = " select distinct A.* from dbo.Almacen as A inner join ReporteInventarioArt  as R on A.IDAlmacen = R.IDAlmacen order by Descripcion";
                    almacen = db.Database.SqlQuery<Almacen>(cadenaa).ToList();
                }
                else
                {
                    string cadenaa = "select * from dbo.Almacen  where IDAlmacen = "+ idalmacen +"order by Descripcion";
                    almacen = db.Database.SqlQuery<Almacen>(cadenaa).ToList();
    
                }
                }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return almacen;
        }

        //Obtener la lista de todos las familias
        public List<Familia> Getlistfam(int idfamilia, int _idalm)
        {
            List<Familia> familia = new List<Familia>();
            try
            {
                if (idfamilia == 0)
                {
                    string cadenaf = "select  distinct F.* from [dbo].[Familia]  as F inner join ReporteInventarioArt  as R on F.IDFamilia = R.IDFamilia where R.IDAlmacen = " + _idalm + " order by Descripcion";
                    familia = db.Database.SqlQuery<Familia>(cadenaf).ToList();
                }
                else
                {
                    string cadenaf = " select  distinct F.* from [dbo].[Familia]  as F inner join ReporteInventarioArt  as R on F.IDFamilia = R.IDFamilia where R.IDAlmacen = "+ _idalm + " and F.IDFamilia = "+ idfamilia +" order by Descripcion";
                    familia = db.Database.SqlQuery<Familia>(cadenaf).ToList();

                }
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return familia;
        }



        private void ReportHeader()
        {
            #region Table head

            ReporteInventarioAlmacen enc = new ReporteInventarioAlmacen();
            enc = GetInvAlmEnc(this.idalmacen, idfamilia);

            float[] anchoColumnal = { 60f, 520f };

            PdfPTable tablal = new PdfPTable(anchoColumnal);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablal.SetTotalWidth(anchoColumnal);
            tablal.SpacingBefore = 0;
            tablal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablal.LockedWidth = true;
            Font _fontStyleencabezadol = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

            Empresa empresa = new EmpresaContext().empresas.Find(2);
            System.Drawing.Image logo = byteArrayToImage(empresa.Logo);
            Image jpg = Image.GetInstance(logo, BaseColor.WHITE);

            jpg.ScaleToFit(60f, 45F); //ancho y largo de la imagen
            jpg.Alignment = Image.ALIGN_LEFT;
            jpg.SetAbsolutePosition(50f, 715f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera (+derecha/-izquierda,+arriba/-abajo)
                                                //_documento.Add(jpg);
            tablal.AddCell(jpg);

            Paragraph paragraph = new Paragraph();
            paragraph.Alignment = Element.ALIGN_RIGHT;
            paragraph.Clear();//ahora utilizo la clase Paragraph 
            paragraph.Font = new Font(FontFactory.GetFont("Arial", 18, Font.BOLD));
            paragraph.Alignment = Element.ALIGN_CENTER;
            paragraph.Add(enc.Titulo);
            PdfPCell cell2 = new PdfPCell();
            cell2.Border = Rectangle.NO_BORDER;
            cell2.PaddingTop = -7;
            cell2.AddElement(paragraph);
            cell2.Colspan = 3;
            paragraph.Clear();
            tablal.AddCell(cell2);
            _documento.Add(tablal);



            float[] anchoColumnasencabezado = { 100f, 50f, 350f, 100f };

            PdfPTable tablaencabezado = new PdfPTable(anchoColumnasencabezado);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablaencabezado.SetTotalWidth(anchoColumnasencabezado);
            tablaencabezado.SpacingBefore = 0;
            tablaencabezado.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencabezado.LockedWidth = true;
            Font _fontStyleencabezado = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

            PdfPCell _tablaencabezado1 = new PdfPCell(new Phrase("Precio Dolar: ", _fontStyleencabezado));
            _tablaencabezado1.Border = 0;
            _tablaencabezado1.FixedHeight = 20f;
            _tablaencabezado1.HorizontalAlignment = Element.ALIGN_RIGHT;
            _tablaencabezado1.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaencabezado1.BackgroundColor = BaseColor.WHITE;
            tablaencabezado.AddCell(_tablaencabezado1);

            string _tc = enc.TC.ToString("C");
            PdfPCell _tablaencabezado2 = new PdfPCell(new Phrase(_tc, _fontStyleencabezado));
            _tablaencabezado2.Border = 0;
            _tablaencabezado2.FixedHeight = 10f;
            _tablaencabezado2.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablaencabezado2.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaencabezado2.BackgroundColor = BaseColor.WHITE;
            tablaencabezado.AddCell(_tablaencabezado2);


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
            _tablaencabezado4.FixedHeight = 10f;
            _tablaencabezado4.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablaencabezado4.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaencabezado4.BackgroundColor = BaseColor.WHITE;
            tablaencabezado.AddCell(_tablaencabezado4);

            tablaencabezado.CompleteRow();
            _documento.Add(tablaencabezado);




            float[] anchoColumnasInvEnc = { 80f, 140f, 150f, 50f, 50f, 50f, 50f };

            PdfPTable tablaInvEnc = new PdfPTable(anchoColumnasInvEnc);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablaInvEnc.SetTotalWidth(anchoColumnasInvEnc);
            tablaInvEnc.SpacingBefore = 0;
            tablaInvEnc.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaInvEnc.LockedWidth = true;
            Font _fontStyleInvEnc = new Font(Font.FontFamily.TIMES_ROMAN, 8, Font.NORMAL | Font.UNDERLINE, BaseColor.WHITE);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

            PdfPCell _tablaInvEnc1 = new PdfPCell(new Phrase("Clave", _fontStyleInvEnc));
            _tablaInvEnc1.Border = 0;
            _tablaInvEnc1.FixedHeight = 20f;
            _tablaInvEnc1.HorizontalAlignment = Element.ALIGN_RIGHT;
            _tablaInvEnc1.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaInvEnc1.BackgroundColor = BaseColor.DARK_GRAY;
            tablaInvEnc.AddCell(_tablaInvEnc1);


            PdfPCell _tablaInvEnc2 = new PdfPCell(new Phrase("Articulo", _fontStyleInvEnc));
            _tablaInvEnc2.Border = 0;
            _tablaInvEnc2.FixedHeight = 10f;
            _tablaInvEnc2.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablaInvEnc2.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaInvEnc2.BackgroundColor = BaseColor.DARK_GRAY;
            tablaInvEnc.AddCell(_tablaInvEnc2);
        //    _documento.Add(tablaInvEnc);

            //PdfPCell _tablaInvEnc3 = new PdfPCell(new Phrase("Ver.", _fontStyleInvEnc));
            //_tablaInvEnc3.Border = 0;
            //_tablaInvEnc3.FixedHeight = 10f;
            //_tablaInvEnc3.HorizontalAlignment = Element.ALIGN_LEFT;
            //_tablaInvEnc3.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_tablaInvEnc3.BackgroundColor = BaseColor.DARK_GRAY;
            //tablaInvEnc.AddCell(_tablaInvEnc3);
        //    _documento.Add(tablaInvEnc);

            PdfPCell _tablaInvEnc4 = new PdfPCell(new Phrase("Presentación", _fontStyleInvEnc));
            _tablaInvEnc4.Border = 0;
            _tablaInvEnc4.FixedHeight = 10f;
            _tablaInvEnc4.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablaInvEnc4.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaInvEnc4.BackgroundColor = BaseColor.DARK_GRAY;
            tablaInvEnc.AddCell(_tablaInvEnc4);


            PdfPCell _tablaInvEnc5 = new PdfPCell(new Phrase("Existencia", _fontStyleInvEnc));
            _tablaInvEnc5.Border = 0;
            _tablaInvEnc5.FixedHeight = 10f;
            _tablaInvEnc5.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablaInvEnc5.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaInvEnc5.BackgroundColor = BaseColor.DARK_GRAY;
            tablaInvEnc.AddCell(_tablaInvEnc5);


            PdfPCell _tablaInvEnc6 = new PdfPCell(new Phrase("Por llegar", _fontStyleInvEnc));
            _tablaInvEnc6.Border = 0;
            _tablaInvEnc6.FixedHeight = 10f;
            _tablaInvEnc6.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablaInvEnc6.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaInvEnc6.BackgroundColor = BaseColor.DARK_GRAY;
            tablaInvEnc.AddCell(_tablaInvEnc6);


            PdfPCell _tablaInvEnc7 = new PdfPCell(new Phrase("Apartado", _fontStyleInvEnc));
            _tablaInvEnc7.Border = 0;
            _tablaInvEnc7.FixedHeight = 10f;
            _tablaInvEnc7.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablaInvEnc7.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaInvEnc7.BackgroundColor = BaseColor.DARK_GRAY;
            tablaInvEnc.AddCell(_tablaInvEnc7);


            PdfPCell _tablaInvEnc8 = new PdfPCell(new Phrase("Disponible", _fontStyleInvEnc));
            _tablaInvEnc8.Border = 0;
            _tablaInvEnc8.FixedHeight = 10f;
            _tablaInvEnc8.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablaInvEnc8.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaInvEnc8.BackgroundColor = BaseColor.DARK_GRAY;
            tablaInvEnc.AddCell(_tablaInvEnc8);
            tablaInvEnc.CompleteRow();
            _documento.Add(tablaInvEnc);
        


        //_pdfTable.CompleteRow();

       

            #endregion

        }

        private void ReportBody()
        {
            #region Table Body
            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

                List<Almacen> alm = new List<Almacen>();
            alm = Getlistalm(idalmacen);
            // aqui haces una tabla par imprimir el encabezado de almacen
            foreach (Almacen dato in alm)
            {
                int _idalm = dato.IDAlmacen;
                string _codAlm = dato.CodAlm;
                string _desAlm = dato.Descripcion;
                float[] anchoColumnasAlmacen = { 100f, 500f };

                PdfPTable tablaAlmacen = new PdfPTable(anchoColumnasAlmacen);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

                tablaAlmacen.SetTotalWidth(anchoColumnasAlmacen);
                tablaAlmacen.SpacingBefore = 0;
                tablaAlmacen.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaAlmacen.LockedWidth = true;
                Font _fontStyleAlmacen = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

                PdfPCell _tablaAlmacen1 = new PdfPCell(new Phrase("Clave: " + _codAlm, _fontStyleAlmacen));
                _tablaAlmacen1.Border = 0;
                _tablaAlmacen1.FixedHeight = 20f;
                _tablaAlmacen1.HorizontalAlignment = Element.ALIGN_RIGHT;
                _tablaAlmacen1.VerticalAlignment = Element.ALIGN_MIDDLE;
                _tablaAlmacen1.BackgroundColor = BaseColor.GRAY;
                tablaAlmacen.AddCell(_tablaAlmacen1);


                PdfPCell _tablaAlmacen2 = new PdfPCell(new Phrase("Almacen: " + _desAlm, _fontStyleAlmacen));
                _tablaAlmacen2.Border = 0;
                _tablaAlmacen2.FixedHeight = 10f;
                _tablaAlmacen2.HorizontalAlignment = Element.ALIGN_LEFT;
                _tablaAlmacen2.VerticalAlignment = Element.ALIGN_MIDDLE;
                _tablaAlmacen2.BackgroundColor = BaseColor.GRAY;
                tablaAlmacen.AddCell(_tablaAlmacen2);
                _documento.Add(tablaAlmacen);

                List<Familia> fam = new List<Familia>();
                fam = Getlistfam(idfamilia, _idalm);
                // aqui haces una tabla par imprimir el encabezado de familia
                foreach (Familia datof in fam)
                {
                    int _idfam = datof.IDFamilia;
                    string _codfam = datof.CCodFam;
                    string _desfam = datof.Descripcion;
                    float[] anchoColumnasFamilia = { 100f, 500f };

                    PdfPTable tablaFamilia = new PdfPTable(anchoColumnasFamilia);
                    //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

                    tablaFamilia.SetTotalWidth(anchoColumnasFamilia);
                    tablaFamilia.SpacingBefore = 0;
                    tablaFamilia.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaFamilia.LockedWidth = true;
                    Font _fontStyleFamilia = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

                    PdfPCell _tablaFamilia1 = new PdfPCell(new Phrase("Clave: " + _codfam, _fontStyleFamilia));
                    _tablaFamilia1.Border = 0;
                    _tablaFamilia1.FixedHeight = 20f;
                    _tablaFamilia1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _tablaFamilia1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _tablaFamilia1.BackgroundColor = BaseColor.LIGHT_GRAY;
                    tablaFamilia.AddCell(_tablaFamilia1);

                    
                    PdfPCell _tablaFamilia2 = new PdfPCell(new Phrase("Familia: " + _desfam, _fontStyleFamilia));
                    _tablaFamilia2.Border = 0;
                    _tablaFamilia2.FixedHeight = 10f;
                    _tablaFamilia2.HorizontalAlignment = Element.ALIGN_LEFT;
                    _tablaFamilia2.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _tablaFamilia2.BackgroundColor = BaseColor.LIGHT_GRAY;
                    tablaFamilia.AddCell(_tablaFamilia2);
                    _documento.Add(tablaFamilia);

                    List<ReporteInventarioArt> inv = new List<ReporteInventarioArt>();
                    inv = GetInvAlm(_idalm, _idfam);
                    // aqui haces una tabla par imprimir el encabezado de familia

                    ArticuloContext db = new ArticuloContext();
                    foreach (ReporteInventarioArt datoi in inv)
                    {

                        

                        string _Cref = datoi.Cref;
                        string _art = datoi.Articulo;
                        string _ver = datoi.version.ToString();
                        string _pres = datoi.Presentacion;
                        string _Exi = datoi.Existencia.ToString(); ;
                        string _Por = datoi.Porllegar.ToString(); ;
                        string _Apa = datoi.Apartado.ToString(); ;
                        string _Dis = datoi.Disponibilidad.ToString(); ;

                        Articulo articulo = db.Articulo.Where(s => s.Cref == _Cref).ToList().FirstOrDefault();
                        float[] anchoColumnasInventario = { 80f, 140f,  150f, 50f, 50f, 50f, 50f };

                        PdfPTable tablaInventario = new PdfPTable(anchoColumnasInventario);
                        //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

                        tablaInventario.SetTotalWidth(anchoColumnasInventario);
                        tablaInventario.SpacingBefore = 0;
                        tablaInventario.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaInventario.LockedWidth = true;
                        Font _fontStyleInventario = new Font(Font.FontFamily.TIMES_ROMAN,10 , Font.NORMAL);  
                        Font _fontStyleInventario1 = new Font(Font.FontFamily.TIMES_ROMAN, 5, Font.NORMAL);

                        PdfPCell _tablaInventario1 = new PdfPCell(new Phrase(_Cref, _fontStyleInventario));
                        _tablaInventario1.Border = 0;
                       // _tablaInventario1.FixedHeight = 20f;
                        _tablaInventario1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        _tablaInventario1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _tablaInventario1.BackgroundColor = BaseColor.WHITE;
                        tablaInventario.AddCell(_tablaInventario1);


                        PdfPCell _tablaInventario2 = new PdfPCell(new Phrase(_art, _fontStyleInventario));
                        _tablaInventario2.Border = 0;
                      //  _tablaInventario2.FixedHeight = 10f;
                        _tablaInventario2.HorizontalAlignment = Element.ALIGN_LEFT;
                        _tablaInventario2.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _tablaInventario2.BackgroundColor = BaseColor.WHITE;
                        tablaInventario.AddCell(_tablaInventario2);
                 //       _documento.Add(tablaInventario);

                        //PdfPCell _tablaInventario3 = new PdfPCell(new Phrase(_ver, _fontStyleInventario));
                        //_tablaInventario3.Border = 0;
                        //_tablaInventario3.FixedHeight = 10f;
                        //_tablaInventario3.HorizontalAlignment = Element.ALIGN_LEFT;
                        //_tablaInventario3.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //_tablaInventario3.BackgroundColor = BaseColor.WHITE;
                        //tablaInventario.AddCell(_tablaInventario3);
                      //  _documento.Add(tablaInventario);

                        PdfPCell _tablaInventario4 = new PdfPCell(new Phrase(_pres, _fontStyleInventario1));
                        _tablaInventario4.Border = 0;
                        //_tablaInventario4.FixedHeight = 10f;
                        _tablaInventario4.HorizontalAlignment = Element.ALIGN_LEFT;
                        _tablaInventario4.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _tablaInventario4.BackgroundColor = BaseColor.WHITE;
                        tablaInventario.AddCell(_tablaInventario4);


                        PdfPCell _tablaInventario5 = new PdfPCell(new Phrase(_Exi, _fontStyleInventario));
                        _tablaInventario5.Border = 0;
                       // _tablaInventario5.FixedHeight = 10f;
                        _tablaInventario5.HorizontalAlignment = Element.ALIGN_RIGHT;
                        _tablaInventario5.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _tablaInventario5.BackgroundColor = BaseColor.WHITE;
                        tablaInventario.AddCell(_tablaInventario5);


                        PdfPCell _tablaInventario6 = new PdfPCell(new Phrase(_Por, _fontStyleInventario));
                        _tablaInventario6.Border = 0;
                       // _tablaInventario6.FixedHeight = 10f;
                        _tablaInventario6.HorizontalAlignment = Element.ALIGN_RIGHT;
                        _tablaInventario6.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _tablaInventario6.BackgroundColor = BaseColor.WHITE;
                        tablaInventario.AddCell(_tablaInventario6);


                        PdfPCell _tablaInventario7 = new PdfPCell(new Phrase(_Apa, _fontStyleInventario));
                        _tablaInventario7.Border = 0;
                     //   _tablaInventario7.FixedHeight = 10f;
                        _tablaInventario7.HorizontalAlignment = Element.ALIGN_RIGHT;
                        _tablaInventario7.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _tablaInventario7.BackgroundColor = BaseColor.WHITE;
                        tablaInventario.AddCell(_tablaInventario7);


                        PdfPCell _tablaInventario8 = new PdfPCell(new Phrase(_Dis, _fontStyleInventario));
                        _tablaInventario8.Border = 0;
                 //       _tablaInventario8.FixedHeight = 10f;
                        _tablaInventario8.HorizontalAlignment = Element.ALIGN_RIGHT;
                        _tablaInventario8.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _tablaInventario8.BackgroundColor = BaseColor.WHITE;
                        tablaInventario.AddCell(_tablaInventario8);
                        tablaInventario.CompleteRow();

                        _documento.Add(tablaInventario);



                        if (articulo.IDTipoArticulo == 6)
                        {
                            SqlConnection conexion = (SqlConnection)new ArticuloFEContext().Database.Connection;

                            try
                            {
                                conexion.Open();
                            }
                            catch (Exception erro)
                            {

                            }
                            IDataReader datos = new SqlCommand("select IDARTICulo,idcaracteristica, ancho, largo, count(id) as numerocintas,MetrosDisponibles from clslotemp where idalmacen="+ _idalm + " and idArticulo=" + articulo.IDArticulo + " and IDCaracteristica=" +datoi.IDCaracteristica + " and metrosdisponibles>0  group by IDARTICulo,idcaracteristica, ancho, metrosdisponibles,largo", conexion).ExecuteReader();
                            try
                            {
                                while (datos.Read())
                                {
                                    int Cintas = int.Parse(datos["numerocintas"].ToString());
                                    int ancho = int.Parse(datos["ancho"].ToString());

                                    decimal metrosdisponibleslineales = decimal.Parse( datos["MetrosDisponibles"].ToString()) / decimal.Parse((ancho * 0.001M).ToString());
                                    int largo = int.Parse(Math.Round(metrosdisponibleslineales,0).ToString());



                                    PdfPTable tablaCostocintas = new PdfPTable(anchoColumnasInventario);
                                    //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                                    tablaCostocintas.SetTotalWidth(anchoColumnasInventario);

                                    tablaCostocintas.HorizontalAlignment = Element.ALIGN_LEFT;
                                    tablaCostocintas.LockedWidth = true;
                                    tablaCostocintas.DefaultCell.Border = Rectangle.NO_BORDER;


                                    PdfPCell _PdfPCell1C = new PdfPCell(new Phrase(""));
                                    _PdfPCell1C.Border = Rectangle.NO_BORDER;
                                    tablaCostocintas.AddCell(_PdfPCell1C);

                                    PdfPCell _PdfPCell4C = new PdfPCell(new Phrase(Cintas + " Cintas  de " + ancho + " X " + largo, new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)));
                                    _PdfPCell4C.Border = Rectangle.NO_BORDER;
                                    tablaCostocintas.AddCell(_PdfPCell4C);

                                    
                                    tablaCostocintas.CompleteRow();
                                   _documento.Add(tablaCostocintas);
                                }
                            }
                            catch (Exception errcintas)
                            {

                            }
                            SqlConnection.ClearAllPools();
                        }
                        //else
                        //{
                        //    _documento.Add(tablaInventario);
                        //}


                    } // fin de cada articulo

                    

                }




                }
            
           #endregion
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



        public class ReporteInventarioAlmacen
        {
            [Key]
            public int Rep { get; set; }

            [DisplayName("Precio dolar:")]
            public decimal TC { get; set; }
            public string Titulo { get; set; }
            public string Almacen { get; set; }
            public string Familia { get; set; }

        }

        public class ReporteInventarioAlm
        {
            [Key]
            public int IDAlmacen { get; set; }

            [DisplayName("Código de Almacen")]
            public string CodAlm { get; set; }

            [DisplayName("Descripción")]
            public string Descripcion { get; set; }
        }
        public class ReporteInventarioFam
        {
            [Key]
            public int IDFamilia { get; set; }
            [Display(Name = "Código de la Familia")]
            public string CCodFam { get; set; }
            [Display(Name = "Descripción")]
            public string Descripcion { get; set; }
        }

        public class ReporteInventarioArt
        {
            [Key]
            public int IDInventarioAlmacen { get; set; }
            public int IDAlmacen { get; set; }
            public int IDFamilia { get; set; }
            public int IDArticulo { get; set; }
            public string Cref { get; set; }
            public string Articulo { get; set; }
            public int IDMoneda { get; set; }
            public int IDCaracteristica { get; set; }
            public int version { get; set; }
            public string Presentacion { get; set; }
            public decimal Existencia { get; set; }
            public decimal Porllegar { get; set; }
            public decimal Apartado { get; set; }
            public decimal Disponibilidad { get; set; }
        }
        public class InventarioPorAlmacenContext : DbContext
        {
            public InventarioPorAlmacenContext() : base("name=DefaultConnection")
            {
                Database.SetInitializer<InventarioPorAlmacenContext>(null);
            }
            public DbSet<ReporteInventarioAlmacen> ReporteInventarioAlmacens { get; set; }
            public DbSet<ReporteInventarioAlm> ReporteInventarioAlms { get; set; }
            public DbSet<ReporteInventarioFam> ReporteInventarioFams { get; set; }
            public DbSet<ReporteInventarioArt> ReporteInventarioArts { get; set; }
        }
    }
}
