using iTextSharp.text; //Librerias iTextSharp para generar pdf
using iTextSharp.text.pdf;
using SIAAPI.Models.Administracion; //Referencia a las carpetas que usamos en el proyecto
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using SIAAPI.Models.Comercial;
using System.Data;
using SIAAPI.Models.Inventarios;

namespace SIAAPI.Reportes
{
    public class ReporteCostoPromedio
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;

        PdfWriter _writer;
        int _totalColumn = 8;

        string _Titulo = "";
        public int idalm { get; set; }
        public int idfam { get; set; }

        PdfPTable _pdfTable = new PdfPTable(8);
        //PdfPTable tablae = new PdfPTable(6);


        PdfPCell _PdfPCell;

       
        

        public string Titulo = "";
        MemoryStream _memoryStream = new MemoryStream();

        List<EncReporteAVG> datosPasan = new List<EncReporteAVG>();
       
        List<VPrecioAVG> datosRep = new List<VPrecioAVG>();

        #endregion

        // aqui los puedes pasar como parametro al reporte

        public byte[] PrepareReport(int IDAlmacen, int IDFamilia)

        {
            idalm = IDAlmacen;
            idfam = IDFamilia;
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
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"ReporteCostoPromedio.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();


        }
       
        public List<EncReporteCP> GetDatosEncAVG(int IDAlmacen, int IDFamilia)
        {
            EncReporteCPContext dba = new EncReporteCPContext();
           EncReporteCP Encabezado = new EncReporteCP();
            idalm = IDAlmacen;
            idfam = IDFamilia;

            if (idalm == 0 && idfam == 0)
            {
                Encabezado.Rep = 100;
                Encabezado.QueAlmacen = "Todos los almacenes";
                Encabezado.QueFamilia = "Todas las familias";
            }
            if (idalm == 0 && idfam != 0)
            {
                Encabezado.Rep = 101;
                Encabezado.QueAlmacen = "Todos los almacenes";
                string fam = new FamiliaContext().Familias.Find(idfam).Descripcion;
                Encabezado.QueFamilia = "Familia: " + fam;
            }
            if (idalm != 0 && idfam == 0)
            {
                Encabezado.Rep = 110;
                Encabezado.QueFamilia = "Todas las familias";
                string alm = new AlmacenContext().Almacenes.Find(idalm).Descripcion;
                Encabezado.QueAlmacen = Encabezado.QueAlmacen = "Almacen: " + alm;
            }
            if (idalm != 0 && idfam != 0)
            {
                Encabezado.Rep = 111;
                string fam = new FamiliaContext().Familias.Find(idfam).Descripcion;
                Encabezado.QueFamilia = "Familia: " + fam;
                string alm = new AlmacenContext().Almacenes.Find(idalm).Descripcion;
                Encabezado.QueAlmacen = Encabezado.QueAlmacen = "Almacen: " + alm;
            }
            List<EncReporteCP> Listencabezado = new List<EncReporteCP>
            {
                new EncReporteCP
                {
                    Rep = Encabezado.Rep,
                    QueFamilia = Encabezado.QueFamilia,
                    QueAlmacen = Encabezado.QueAlmacen
                }
            };
            return Listencabezado;
        }
  

    public List<TempInvProm> GetDatosRepAVG(int IDAlmacen, int IDFamilia)
        {
            TempInvProm costo = new TempInvProm();
            string cadenaSql = "";
            idalm = IDAlmacen;
            idfam = IDFamilia;
            if (idalm == 0 && idfam == 0)
            {
               cadenaSql = "select * from TempInvProm order by idFamilia, Cref"; 
            }
            if (idalm == 0 && idfam != 0)
            {
                cadenaSql = "select * from TempInvProm where IDFamilia=" + idfam + " order by idFamilia, Cref";
            }
            if (idalm != 0 && idfam == 0)
            {
                cadenaSql = "select * from TempInvProm where IDalmacen=" + idalm + " order by idFamilia, Cref";
             }
            if (idalm != 0 && idfam != 0)
            {
                cadenaSql = "select * from TempInvProm where IDFamilia=" + idfam + " and IDalmacen=" + idalm + " order by idFamilia, Cref";
            }
            TempInvPromContext dba = new TempInvPromContext();
            var datosRep = dba.Database.SqlQuery<TempInvProm>(cadenaSql).ToList();
            return datosRep;
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

//se modifico de aquí
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
            paragraph.Add("Costo Promedio");
            PdfPCell cell2 = new PdfPCell();
            cell2.Border = Rectangle.NO_BORDER;
            cell2.PaddingTop = -7;
            cell2.AddElement(paragraph);
            cell2.Colspan = 3;
            paragraph.Clear();
            tablal.AddCell(cell2);
            _documento.Add(tablal);
//Aquí termina la modificación del encabezado

            float[] anchoColumnasencabezado = { 50f, 50f, 150f, 300f };

            List<EncReporteCP> datosEnc = new List<EncReporteCP>();
            datosEnc = GetDatosEncAVG(idalm, idfam);
            

            foreach (EncReporteCP E in datosEnc)
            {
                PdfPTable tablaencabezado = new PdfPTable(anchoColumnasencabezado);
                tablaencabezado.DefaultCell.Border = Rectangle.NO_BORDER;

            tablaencabezado.SetTotalWidth(anchoColumnasencabezado);
            tablaencabezado.SpacingBefore = 0;
            tablaencabezado.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaencabezado.LockedWidth = true;
            Font _fontStyleencabezado = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

            PdfPCell _tablaencabezado1 = new PdfPCell(new Phrase("Fecha Impresion:", _fontStyleencabezado));
            _tablaencabezado1.Border = 0;
            _tablaencabezado1.FixedHeight = 10f;
            _tablaencabezado1.HorizontalAlignment = Element.ALIGN_RIGHT;
            _tablaencabezado1.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaencabezado1.BackgroundColor = BaseColor.WHITE;
            tablaencabezado.AddCell(_tablaencabezado1);

            DateTime fecAct = DateTime.Today;
            string FA = fecAct.ToString("dd/MM/yyyy");
            PdfPCell _tablaencabezado2 = new PdfPCell(new Phrase(FA, _fontStyleencabezado));
            _tablaencabezado2.Border = 0;
            // _tablaencabezado4.FixedHeight = 10f;
            _tablaencabezado2.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablaencabezado2.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaencabezado2.BackgroundColor = BaseColor.WHITE;
            tablaencabezado.AddCell(_tablaencabezado2);
           PdfPCell _tablaencabezado3= new PdfPCell(new Phrase(E.QueAlmacen, _fontStyleencabezado));
           _tablaencabezado3.Border = Rectangle.NO_BORDER;
           _tablaencabezado3.FixedHeight = 10f;
           _tablaencabezado3.HorizontalAlignment = Element.ALIGN_LEFT;
           _tablaencabezado3.VerticalAlignment = Element.ALIGN_MIDDLE;
           _tablaencabezado3.BackgroundColor = BaseColor.WHITE;
           tablaencabezado.AddCell(_tablaencabezado3);
           PdfPCell _tablaencabezado4 = new PdfPCell(new Phrase(E.QueFamilia, _fontStyleencabezado));
           _tablaencabezado4.Border = 0;
           _tablaencabezado4.FixedHeight = 10f;
           _tablaencabezado4.HorizontalAlignment = Element.ALIGN_RIGHT;
           _tablaencabezado4.VerticalAlignment = Element.ALIGN_MIDDLE;
           _tablaencabezado4.BackgroundColor = BaseColor.WHITE;
           tablaencabezado.AddCell(_tablaencabezado4);

                //tablaencabezado.CompleteRow();
                _documento.Add(tablaencabezado);
            }

            //No debe exceder de 600 Float
            float[] anchoColumnasencart = { 50f, 70f, 100f, 50f, 50f, 50f, 50f, 50f, 40f, 40f, 40f };

            // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

            PdfPTable tablae = new PdfPTable(anchoColumnasencart);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablae.HorizontalAlignment = Element.ALIGN_LEFT;
            tablae.WidthPercentage = 100;

            // aqui podremos cambiar la fuente de lcientes y su tamanño

            Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);

            CMYKColor COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

            CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;

            Font _fontStyleEncabezado = FontFactory.GetFont("Tahoma", 6f, COLORDEFUENTEREPORTE);




            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Clave", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Artículo", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Presentación", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Fecha de última compra", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Costo registrado", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);
            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Costo promedio", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);
            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Existencia", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);
            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Costo Inventario", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);
            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Moneda", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);
            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("Almacen", _fontStyleEncabezado));
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
            _pdfTable.CompleteRow();




            _documento.Add(tablae);

            #endregion

        }
        private void ReportBody()
        {

            #region Table Body


            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            List<TempInvProm> datoscp = new List<TempInvProm>();
            datoscp = GetDatosRepAVG(idalm, idfam);

            decimal acumuladodls = 0;
            decimal Acumladopesos = 0;
            decimal acumuladodlsTOTAL = 0;
            decimal AcumladopesosTOTAL = 0;
            decimal totalendls = 0;
            decimal totalenpesos = 0;
            foreach (TempInvProm cp in datoscp)
            {

                Articulo articulo = new ArticuloContext().Database.SqlQuery<Articulo>("select * from Articulo where idarticulo='" + cp.IDArticulo + "'").ToList().FirstOrDefault();


                // creamos una tabla para imprimir los los datos del cliente
                // como son 4 columnas a imprimir 600entre las 4 

                float[] anchoColumnasCostoProm = { 50f, 70f, 100f, 50f, 50f, 50f, 50f, 50f, 30f, 40f, 40f };

                // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

                
                PdfPTable tablaCostoProm = new PdfPTable(anchoColumnasCostoProm);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaCostoProm.SetTotalWidth(anchoColumnasCostoProm);
                tablaCostoProm.SpacingBefore = 3;
                tablaCostoProm.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaCostoProm.LockedWidth = true;

                Font _fontStylecliente = new Font(Font.FontFamily.TIMES_ROMAN, 6, Font.NORMAL);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                PdfPCell _PdfPCell1 = new PdfPCell(new Phrase(cp.cref, _fontStylecliente));
                _PdfPCell1.Border = 0;
                _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell1.BackgroundColor = BaseColor.WHITE;
                tablaCostoProm.AddCell(_PdfPCell1);

                PdfPCell _PdfPCell4 = new PdfPCell(new Phrase (cp.Descripcion, _fontStylecliente));
                _PdfPCell4.Border = 0;
                _PdfPCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell4.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell4.BackgroundColor = BaseColor.WHITE;
                tablaCostoProm.AddCell(_PdfPCell4);

                _fontStyle = FontFactory.GetFont("TIMES_ROMAN", 4f, Font.NORMAL);
                _PdfPCell = new PdfPCell(new Phrase(cp.Presentacion, _fontStyle));
                _PdfPCell.Border = 0;
                tablaCostoProm.AddCell(_PdfPCell);

                _PdfPCell = new PdfPCell(new Phrase(cp.FechaUltimaCompra.ToShortDateString().ToString(), _fontStylecliente));
                _PdfPCell.Border = 0;
                tablaCostoProm.AddCell(_PdfPCell);
                Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select  * from Caracteristica where ID=" + cp.IDCaracteristica).FirstOrDefault();
                Articulo arti = new ArticuloContext().Articulo.Find(cara.Articulo_IDArticulo);
                decimal costoart = 0;
                try
                {
                    ClsDatoDecimal dato = new ArticuloContext().Database.SqlQuery<ClsDatoDecimal>("select dbo.getcosto(0," + cara.Articulo_IDArticulo + ",1) as Dato").FirstOrDefault();
                    costoart = dato.Dato;
                }
                catch (Exception err )
                {

                }
                
                _PdfPCell = new PdfPCell(new Phrase(costoart.ToString(), _fontStylecliente));
                _PdfPCell.Border = 0;
                tablaCostoProm.AddCell(_PdfPCell);
                decimal promedio = 0;
                if (arti.c_Moneda.ClaveMoneda == "MXN")
                {
                    promedio = cp.PromedioenPesos;
                }
                else if (arti.c_Moneda.ClaveMoneda == "USD")
                {
                    promedio = cp.Promedioendls;
                }
                _PdfPCell = new PdfPCell(new Phrase(promedio.ToString(), _fontStylecliente));
                _PdfPCell.Border = 0;
                tablaCostoProm.AddCell(_PdfPCell);

                _PdfPCell = new PdfPCell(new Phrase(cp.existencia.ToString(), _fontStylecliente));
                _PdfPCell.Border = 0;
                tablaCostoProm.AddCell(_PdfPCell);
                decimal COSTOA = 0;
                
                if (arti.c_Moneda.ClaveMoneda == "MXN")
                {
                    COSTOA = cp.CostoInvPesos;
                }
                else if (arti.c_Moneda.ClaveMoneda == "USD")
                {
                    COSTOA = cp.CostoInvDls;
                }

                _PdfPCell = new PdfPCell(new Phrase(COSTOA.ToString(), _fontStylecliente));
                _PdfPCell.Border = 0;
                tablaCostoProm.AddCell(_PdfPCell);
 
                if (arti.c_Moneda.ClaveMoneda == "USD")
                {
                    acumuladodls += cp.CostoInvDls;
                }
               
                if (arti.c_Moneda.ClaveMoneda == "MXN")
                {
                    Acumladopesos += cp.CostoInvPesos;
                }
                acumuladodlsTOTAL += cp.CostoInvDls;
                AcumladopesosTOTAL += cp.CostoInvPesos;

                _PdfPCell = new PdfPCell(new Phrase(arti.c_Moneda.ClaveMoneda, _fontStylecliente));
                _PdfPCell.Border = 0;
                tablaCostoProm.AddCell(_PdfPCell);
                _fontStyle = FontFactory.GetFont("TIMES_ROMAN", 5f, Font.NORMAL);
                _PdfPCell = new PdfPCell(new Phrase(cp.Almacen, _fontStyle));
                _PdfPCell.Border = 0;
                tablaCostoProm.AddCell(_PdfPCell);
                _fontStyle = FontFactory.GetFont("TIMES_ROMAN", 5f, Font.NORMAL);
                _PdfPCell = new PdfPCell(new Phrase(cp.Familia, _fontStyle));
                _PdfPCell.Border = 0;
                tablaCostoProm.AddCell(_PdfPCell);

                _documento.Add(tablaCostoProm);

                if (articulo.IDTipoArticulo == 6)
                {
                    SqlConnection conexion = (SqlConnection) new ArticuloFEContext().Database.Connection;
                    
                    try
                    {
                        conexion.Open();
                    }
                    catch (Exception erro)
                    {

                    }
                    IDataReader datos = new SqlCommand("select IDARTICulo,idcaracteristica, ancho, largo, count(id) as numerocintas from clslotemp where idArticulo=" + articulo.IDArticulo + " and IDCaracteristica=" + cp.IDCaracteristica + " and metrosdisponibles>0  group by IDARTICulo,idcaracteristica, ancho, largo" ,conexion).ExecuteReader();
                    try
                    {
                         while  (datos.Read())
                                  {
                                int Cintas = int.Parse(datos["numerocintas"].ToString());
                                int ancho = int.Parse(datos["ancho"].ToString());
                                int largo  =  int.Parse(datos["largo"].ToString());



                            PdfPTable tablaCostocintas = new PdfPTable(anchoColumnasCostoProm);
                                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                                tablaCostocintas.SetTotalWidth(anchoColumnasCostoProm);
                               
                                tablaCostocintas.HorizontalAlignment = Element.ALIGN_LEFT;
                                tablaCostocintas.LockedWidth = true;
                                tablaCostocintas.DefaultCell.Border = Rectangle.NO_BORDER;


                                PdfPCell _PdfPCell1C = new PdfPCell(new Phrase("", _fontStylecliente));
                            _PdfPCell1C.Border = Rectangle.NO_BORDER;
                                tablaCostocintas.AddCell(_PdfPCell1C);

                                PdfPCell _PdfPCell4C = new PdfPCell(new Phrase(Cintas +" Bobinas  de "+ ancho+ " X "+largo, _fontStylecliente));
                            _PdfPCell4C.Border = Rectangle.NO_BORDER;
                                tablaCostocintas.AddCell(_PdfPCell4C);



                                _fontStyle = FontFactory.GetFont("TIMES_ROMAN", 4f, Font.NORMAL);

                                _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));




                                _PdfPCell.Border = 0;
                                tablaCostocintas.AddCell(_PdfPCell);

                                _PdfPCell = new PdfPCell(new Phrase("", _fontStylecliente));
                                _PdfPCell.Border = 0;
                                tablaCostocintas.AddCell(_PdfPCell);

                                _PdfPCell = new PdfPCell(new Phrase("", _fontStylecliente));
                                _PdfPCell.Border = 0;
                                tablaCostocintas.AddCell(_PdfPCell);

                                _PdfPCell = new PdfPCell(new Phrase("", _fontStylecliente));
                                _PdfPCell.Border = 0;
                                tablaCostocintas.AddCell(_PdfPCell);

                                _PdfPCell = new PdfPCell(new Phrase("".ToString(), _fontStylecliente));
                                _PdfPCell.Border = 0;
                                tablaCostocintas.AddCell(_PdfPCell);

                                tablaCostocintas.CompleteRow();
                          
                            _documento.Add(tablaCostocintas);
                            }
                    }
                    catch(Exception errcintas)
                    {

                    } 
                }
                //else
                //{
                //    _documento.Add(tablaCostoProm);
                //}

                SqlConnection.ClearAllPools();

            }

            Paragraph totales = new Paragraph();
            totales.Add(new Phrase("Total de iventario en USD "+ Math.Round(acumuladodls,2).ToString("C") +"\n", FontFactory.GetFont("TIMES_ROMAN", 12f, Font.BOLD)));
            totales.Add(new Phrase("Total de iventario en PESOS " + Math.Round(Acumladopesos, 2).ToString("C")+ "\n", FontFactory.GetFont("TIMES_ROMAN", 12f, Font.BOLD)));
            totales.Add(new Phrase("Total  en USD " + Math.Round(acumuladodlsTOTAL, 2).ToString("C") + "\n", FontFactory.GetFont("TIMES_ROMAN", 12f, Font.BOLD)));
            totales.Add(new Phrase("Total  en PESOS " + Math.Round(AcumladopesosTOTAL, 2).ToString("C"), FontFactory.GetFont("TIMES_ROMAN", 12f, Font.BOLD)));


            _documento.Add(totales);

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

                //headerTemplate.BeginText();
                //headerTemplate.SetFontAndSize(bf, 12);
                //headerTemplate.SetTextMatrix(0, 0);
                //headerTemplate.ShowText((writer.PageNumber - 1).ToString());
                //headerTemplate.EndText();

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


    public class VPrecioAVG
    {
        [Key]
        [Display(Name = "ID")]
        public int ID { get; set; }
        [Display(Name = "ID Caracteristica")]
        public int Caracteristica_ID { get; set; }
        [Display(Name = "Clave")]
        public String Cref { get; set; }
        [Display(Name = "Articulo")]
        public String Articulo { get; set; }
        [Display(Name = "Presentación")]
        public String Presentacion { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Fecha Ultima Compra")]
        public DateTime FechaUltimaCompra { get; set; }
        [Display(Name = "Ultimo Costo")]
        public Decimal UltimoPrecio{ get; set; }
        [Display(Name = "Costo Promedio")]
        public Decimal PrecioPromedio{ get; set; }
        [Display(Name = "Existencia")]
        public Decimal Existencia { get; set; }
        [Display(Name = "Costo Inventario")]
        public Decimal Costo { get; set; }
        [Display(Name = "Moneda")]
        public String ClaveMoneda { get; set; }
        [Display(Name = "ID Almacen")]
        public int IDAlmacen { get; set; }
        [Display(Name = "Almacen")]
        public String Almacen { get; set; }
        [Display(Name = "ID Familia")]
        public int IDFamilia { get; set; }
        [Display(Name = "Familia")]
        public String Familia { get; set; }
        [Display(Name = "Stock Minimo")]
        public Decimal StockMin { get; set; }
        [Display(Name = "Stock Maximo")]
        public Decimal StockMax { get; set; }
        [Display(Name = "Minimo de Venta")]
        public Decimal MinimoVenta { get; set; }
        [Display(Name = "Minimo de Compra")]
        public Decimal MinimoCompra { get; set; }

        [Display(Name = "Unidad")]
        public string Unidad { get; set; }
    }
    public class VPrecioAVGContext : DbContext
    {
        public VPrecioAVGContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VPrecioAVG> VPrecioAVG { get; set; }
    }
    
        public class EncReporteCP
    {
        [Key]
        [Display(Name = "ID")]
        public int Rep { get; set; }
        [Key]
        [Display(Name = "Almacen")]
        public string QueAlmacen { get; set; }
        [Key]
        [Display(Name = "Familia")]
        public string QueFamilia { get; set; }
    }
    public class EncReporteCPContext : DbContext
    {
        public EncReporteCPContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<EncReporteCP> EncReporteCP { get; set; }
    }

    public class EncReporteAVG
    {
        [Key]
     public int IDAlmacen { get; set; }
     public int IDFamilia { get; set; }
    }
    public class EncReporteAVGContext : DbContext
    {
        public EncReporteAVGContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<EncReporteAVG> EncReporteAVG { get; set; }
    }


    
}






