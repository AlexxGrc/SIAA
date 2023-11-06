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
using SIAAPI.Models.Produccion;
using SIAAPI.Reportes;


namespace SIAAPI.ClasesProduccion
{
    public class ProgTintas
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;

        PdfWriter _writer;

        PdfPTable _pdfTable = new PdfPTable(8);
        PdfPTable tablae = new PdfPTable(13);

        string _Titulo = "PROGRAMA DE SUAJES";
        PdfPCell _PdfPCell;

        public List<OrdenProduccion> ordenes = new List<OrdenProduccion>();
        MemoryStream _memoryStream = new MemoryStream();
        CMYKColor COLORDEREPORTE;

        CMYKColor COLORDEFUENTEREPORTE;

        #endregion
        Font _fontStyleEncabezado = null;
        public List<OrdenProduccion> GetOP(string Estado)
        {
            try
            {

                OrdenProduccionContext db = new OrdenProduccionContext();

                string cadena = "";
                if (Estado == "Programado")
                {
                    cadena = "select distinct o.idorden, p.prioridad, o.*from ordenproduccion as o inner join prioridades as p on o.idorden=p.idorden where p.estado='" + Estado + "'  and o.prioridad>0order by p.Prioridad, o.idorden";
                }
                if (Estado == "Lista")
                {
                    cadena = "select [IDOrden],[IDModeloProduccion],[IDCliente] ,[IDArticulo] ,[IDCaracteristica] ,[Descripcion] ,[Presentacion] ,[Indicaciones] ,[FechaCompromiso] ,[FechaInicio] ,[FechaProgramada] ,[FechaRealdeInicio] ,[FechaRealdeTerminacion] ,[Cantidad] ,[IDPedido] ,[IDDetPedido] , 9999 as Prioridad  ,[UserID] ,[Liberar] ,[FechaCreacion] ,[EstadoOrden] ,[idhe],Arrastre from ordenproduccion where Estadoorden='" + Estado + "'  order by Prioridad";
                }
                ordenes = db.Database.SqlQuery<OrdenProduccion>(cadena).ToList();
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return ordenes;
        }

        public byte[] PrepareReport()

        {



            _documento = new Document(PageSize.LETTER);

            //_documento.SetMargins(20f, 10f, 20f, 30f);

            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEventsTin();
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

            _documento.Open();

            // _pdfTable.SetWidths(new float[] { 40f, 40f, 40f, 260f, 60f, 60f, 50f, 50f});

            this.ReportHeader();
            this.ReportBody();

            //_pdfTable.HeaderRows = 4;
            //_documento.Add(_pdfTable);
            _documento.Close();
            COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

            COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;

            _fontStyleEncabezado = FontFactory.GetFont("Tahoma", 13f, COLORDEFUENTEREPORTE);
            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"Programadetintas.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();


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

            float[] anchoColumnasencart = { 60f, 40f, 290f, 70f, 70f, 50f };

            // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

            PdfPTable tablae = new PdfPTable(anchoColumnasencart);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablae.HorizontalAlignment = Element.ALIGN_LEFT;
            tablae.WidthPercentage = 100;

            // aqui podremos cambiar la fuente de lcientes y su tamanño

            Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);

            CMYKColor COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

            CMYKColor COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;
            CMYKColor COLORDEFUENTEREPORTE2 = new ClsColoresReporte("Negro").color;
            Font _fontStyleEncabezado = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            Font _fontStyleEncabezado2 = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE2);


            //encbezado para cuando sea solo un cliente

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("PRIORIDAD", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("ORDEN", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("CLIENTE", _fontStyleEncabezado));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("", _fontStyleEncabezado2));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("", _fontStyleEncabezado2));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("", _fontStyleEncabezado2));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            _pdfTable.CompleteRow();
            _documento.Add(tablae);

            #endregion

        }

        #region Table Body
        private void ReportBody()
        {




            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);

            //  
            List<VMaquinaProceso> maquinas = new VMaquinaProcesoContext().Database.SqlQuery<VMaquinaProceso>("select * from Vmaquinaproceso where idproceso=5 and idtipoarticulo=3").ToList();


            foreach (VMaquinaProceso maquina in maquinas)

            {
                string text = maquina.Descripcion;



                Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 18, Font.NORMAL);



                Phrase p1 = new Phrase(text + "\n", _fontStylecliente);




                _documento.Add(p1);

                _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 14, Font.NORMAL, new BaseColor(System.Drawing.Color.Blue));
                Phrase p2 = new Phrase("\tProgramadas" + "\n", _fontStylecliente);

                _documento.Add(p2);

                List<OrdenProduccion> ordenes = GetOP("Programado");

                foreach (OrdenProduccion orden in ordenes)
                {
                    //verifica si tiene la maquina de la lista
                    ClsDatoEntero registro = new ArticuloProduccionRealContext().Database.SqlQuery<ClsDatoEntero>("select count(*) as Dato from ArticuloProduccion where idorden=" + orden.IDOrden + " and idarticulo=" + maquina.IDArticulo).FirstOrDefault();
                    if (registro.Dato == 1)
                    {
                        int PrioridadOrden = 0;
                        int ProcesoProgramado = 0;
                        SIAAPI.Models.Produccion.OrdenProduccionContext db = new SIAAPI.Models.Produccion.OrdenProduccionContext();
                        try
                        {

                            SIAAPI.Models.Comercial.ClsDatoEntero cuenta = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select count(idorden) as Dato from prioridades where estado='Programado' and IDOrden=" + orden.IDOrden + "and IDProceso=" + maquina.IDProceso + " and IDMaquina=" + maquina.IDArticulo).ToList().FirstOrDefault();
                            ProcesoProgramado = cuenta.Dato;
                            SIAAPI.Models.Comercial.ClsDatoEntero CuentaProcesoXOrden = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select Prioridad as Dato from prioridades where estado='Programado' and IDOrden=" + orden.IDOrden + "and IDProceso=" + maquina.IDProceso + " and IDMaquina=" + maquina.IDArticulo).ToList().FirstOrDefault();
                            PrioridadOrden = CuentaProcesoXOrden.Dato;

                        }
                        catch (Exception err)
                        {

                        }
                        if (ProcesoProgramado != 0)
                        {
                            float[] anchoColumnasordenes = { 60f, 40f, 290f, 70f, 70f, 50f };

                            PdfPTable tablae = new PdfPTable(anchoColumnasordenes);
                            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                            tablae.DefaultCell.Border = Rectangle.NO_BORDER;
                            tablae.HorizontalAlignment = Element.ALIGN_LEFT;
                            tablae.WidthPercentage = 100;


                            PdfPCell _PdfPCell21 = new PdfPCell(new Phrase(PrioridadOrden.ToString()));
                            _PdfPCell21.Border = 0;
                            _PdfPCell21.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell21.VerticalAlignment = Element.ALIGN_MIDDLE;

                            tablae.AddCell(_PdfPCell21);

                            PdfPCell _PdfPCell22 = new PdfPCell(new Phrase(orden.IDOrden.ToString()));
                            _PdfPCell22.Border = 0;
                            _PdfPCell22.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell22.VerticalAlignment = Element.ALIGN_MIDDLE;

                            tablae.AddCell(_PdfPCell22);


                            Clientes cliente = new ClientesContext().Clientes.Find(orden.IDCliente);

                            PdfPCell _PdfPCell23 = new PdfPCell(new Phrase(cliente.Nombre));
                            _PdfPCell23.Border = 0;
                            _PdfPCell23.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell23.VerticalAlignment = Element.ALIGN_MIDDLE;

                            tablae.AddCell(_PdfPCell23);


                            tablae.CompleteRow();






                            ///  veamos los suajes
                            ///  

                            List<ArticuloProduccion> tintas = new ArticuloProduccionRealContext().Database.SqlQuery<ArticuloProduccion>("select * from ArticuloProduccion where idorden=" + orden.IDOrden + " and Idtipoarticulo=7").ToList();

                            float[] anchoColumnassuajes = { 80, 290f, 70f, 70f, 70f };

                            PdfPTable tablasuaje = new PdfPTable(anchoColumnassuajes);
                            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                            tablasuaje.DefaultCell.Border = Rectangle.NO_BORDER;
                            tablasuaje.HorizontalAlignment = Element.ALIGN_LEFT;
                            tablasuaje.WidthPercentage = 100;


                            foreach (ArticuloProduccion elementotinta in tintas)
                            {

                                Articulo zrt = new ArticuloContext().Articulo.Find(elementotinta.IDArticulo);




                                Font _fontStylesuaje = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);

                                Font _fontStylecref = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL);


                                Phrase pcref = new Phrase("", _fontStylecref);


                                PdfPCell _PdfPCellS1 = new PdfPCell(pcref);
                                _PdfPCellS1.Border = 0;
                                _PdfPCellS1.HorizontalAlignment = Element.ALIGN_RIGHT;
                                _PdfPCellS1.VerticalAlignment = Element.ALIGN_MIDDLE;

                                tablasuaje.AddCell(_PdfPCellS1);

                                PdfPCell _PdfPCellS2 = new PdfPCell(new Phrase(zrt.Descripcion, _fontStylecref));
                                _PdfPCellS2.Border = 0;
                                _PdfPCellS2.HorizontalAlignment = Element.ALIGN_LEFT;
                                //     _PdfPCellS2.VerticalAlignment = Element.ALIGN_MIDDLE;

                                tablasuaje.AddCell(_PdfPCellS2);




                                PdfPCell _PdfPCellSA = new PdfPCell(new Phrase(elementotinta.Cantidad + " KG", _fontStylesuaje));
                                _PdfPCellSA.Border = 0;
                                _PdfPCellSA.HorizontalAlignment = Element.ALIGN_CENTER;

                                tablasuaje.AddCell(_PdfPCellSA);

                                tablasuaje.CompleteRow();

                            }


                            _documento.Add(tablae);
                            _documento.Add(tablasuaje);
                        }

                            
                    }
                }

                _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 14, Font.NORMAL, new BaseColor(System.Drawing.Color.Blue));
                Phrase p3 = new Phrase("\tListas" + "\n", _fontStylecliente);

                _documento.Add(p3);

                ordenes = GetOP("Lista");


                foreach (OrdenProduccion orden in ordenes)
                {
                    //verifica si tiene la maquina de la lista
                    ClsDatoEntero registro = new ArticuloProduccionRealContext().Database.SqlQuery<ClsDatoEntero>("select count(*) as Dato from ArticuloProduccion where idorden=" + orden.IDOrden + " and idarticulo=" + maquina.IDArticulo).FirstOrDefault();
                    if (registro.Dato == 1)
                    {


                        float[] anchoColumnasordenes = { 60f, 40f, 290f, 70f, 70f, 50f };

                        PdfPTable tablae = new PdfPTable(anchoColumnasordenes);
                        //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                        tablae.DefaultCell.Border = Rectangle.NO_BORDER;
                        tablae.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablae.WidthPercentage = 100;


                        PdfPCell _PdfPCell21 = new PdfPCell(new Phrase(""));
                        _PdfPCell21.Border = 0;
                        _PdfPCell21.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell21.VerticalAlignment = Element.ALIGN_MIDDLE;

                        tablae.AddCell(_PdfPCell21);

                        PdfPCell _PdfPCell22 = new PdfPCell(new Phrase(orden.IDOrden.ToString()));
                        _PdfPCell22.Border = 0;
                        _PdfPCell22.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell22.VerticalAlignment = Element.ALIGN_MIDDLE;

                        tablae.AddCell(_PdfPCell22);


                        Clientes cliente = new ClientesContext().Clientes.Find(orden.IDCliente);

                        PdfPCell _PdfPCell23 = new PdfPCell(new Phrase(cliente.Nombre));
                        _PdfPCell23.Border = 0;
                        _PdfPCell23.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell23.VerticalAlignment = Element.ALIGN_MIDDLE;

                        tablae.AddCell(_PdfPCell23);


                        tablae.CompleteRow();






                        ///  veamos los suajes
                        ///  

                        List<ArticuloProduccion> tintas = new ArticuloProduccionRealContext().Database.SqlQuery<ArticuloProduccion>("select * from ArticuloProduccion where idorden=" + orden.IDOrden + " and Idtipoarticulo=7").ToList();

                        float[] anchoColumnassuajes = { 80, 290f, 70f, 70f, 70f };

                        PdfPTable tablasuaje = new PdfPTable(anchoColumnassuajes);
                        //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                        tablasuaje.DefaultCell.Border = Rectangle.NO_BORDER;
                        tablasuaje.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablasuaje.WidthPercentage = 100;


                        foreach (ArticuloProduccion elementotinta in tintas)
                        {

                            Articulo zrt = new ArticuloContext().Articulo.Find(elementotinta.IDArticulo);




                            Font _fontStylesuaje = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);

                            Font _fontStylecref = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL);


                            Phrase pcref = new Phrase("", _fontStylecref);


                            PdfPCell _PdfPCellS1 = new PdfPCell(pcref);
                            _PdfPCellS1.Border = 0;
                            _PdfPCellS1.HorizontalAlignment = Element.ALIGN_RIGHT;
                            _PdfPCellS1.VerticalAlignment = Element.ALIGN_MIDDLE;

                            tablasuaje.AddCell(_PdfPCellS1);

                            PdfPCell _PdfPCellS2 = new PdfPCell(new Phrase(zrt.Descripcion, _fontStylecref));
                            _PdfPCellS2.Border = 0;
                            _PdfPCellS2.HorizontalAlignment = Element.ALIGN_LEFT;
                            //     _PdfPCellS2.VerticalAlignment = Element.ALIGN_MIDDLE;

                            tablasuaje.AddCell(_PdfPCellS2);




                            PdfPCell _PdfPCellSA = new PdfPCell(new Phrase(elementotinta.Cantidad + " KG", _fontStylesuaje));
                            _PdfPCellSA.Border = 0;
                            _PdfPCellSA.HorizontalAlignment = Element.ALIGN_CENTER;

                            tablasuaje.AddCell(_PdfPCellSA);

                            tablasuaje.CompleteRow();

                        }


                        _documento.Add(tablae);
                        _documento.Add(tablasuaje);
                    }
                }

            }



        }

        #endregion
        #region Extensión de la clase pdfPageEvenHelper
        public class ITextEventsTin : PdfPageEventHelper
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
}









