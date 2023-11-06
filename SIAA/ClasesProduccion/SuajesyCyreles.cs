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
using SIAAPI.Models.Inventarios;

namespace SIAAPI.ClasesProduccion
{
    public class SuajesyCyreles
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;

        PdfWriter _writer;

        PdfPTable _pdfTable = new PdfPTable(8);
        PdfPTable tablae = new PdfPTable(13);

        string _Titulo = "PROGRAMA DE PRODUCCIÓN";
        PdfPCell _PdfPCell;

        public  List<OrdenProduccion> ordenes = new List<OrdenProduccion>();
        MemoryStream _memoryStream = new MemoryStream();
        CMYKColor COLORDEREPORTE;

        CMYKColor COLORDEFUENTEREPORTE;

        #endregion
        Font _fontStyleEncabezado = null;
        public List<OrdenProduccion> GetOP( string Estado)
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
                     cadena = "select [IDOrden],[IDModeloProduccion],[IDCliente] ,[IDArticulo] ,[IDCaracteristica] ,[Descripcion] ,[Presentacion] ,[Indicaciones] ,[FechaCompromiso] ,[FechaInicio] ,[FechaProgramada] ,[FechaRealdeInicio] ,[FechaRealdeTerminacion] ,[Cantidad] ,[IDPedido] ,[IDDetPedido] , 9999 as Prioridad  ,[UserID] ,[Liberar] ,[FechaCreacion] ,[EstadoOrden] ,[idhe], arrastre from ordenproduccion where Estadoorden='" + Estado + "'  order by Prioridad";
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
            _documento.SetMargins(25, 10f, 40f, 60f);
            ////_documento.SetMargins(20f, 10f, 20f, 30f);

            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEventsSuaj();
            //_pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

            _documento.Open();
           
           
            AgregarDatosEmisor();
           
            this.ReportHeader();
            this.ReportBody();

            //_pdfTable.HeaderRows = 4;
            //_documento.Add(_pdfTable);
            _documento.Close();
            COLORDEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;

             COLORDEFUENTEREPORTE = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;

            _fontStyleEncabezado = FontFactory.GetFont("Tahoma", 13f, COLORDEFUENTEREPORTE);
            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"ProgramadeSuajes.pdf" + "\"");
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
        private void AgregarDatosEmisor()
        {
            float[] anchoColumnasTablaDatos = { 600f };
            PdfPTable tablaDatosPrincipal = new PdfPTable(anchoColumnasTablaDatos);
            tablaDatosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.SpacingBefore = 30;
            tablaDatosPrincipal.SetTotalWidth(anchoColumnasTablaDatos);
            tablaDatosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosPrincipal.LockedWidth = true;

            //Datos del receptor
            float[] anchoColumnaspro = { 200f, 200f, 200f };
            PdfPTable tablapro = new PdfPTable(anchoColumnaspro);
            tablapro.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapro.SetTotalWidth(anchoColumnaspro);
            tablapro.HorizontalAlignment = Element.ALIGN_LEFT;
            tablapro.LockedWidth = true;

           

            Empresa empresa = new EmpresaContext().empresas.Find(2);
            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            try
            {
                Image jpg = iTextSharp.text.Image.GetInstance(logoempresa, System.Drawing.Imaging.ImageFormat.Jpeg);

                Paragraph paragraph = new Paragraph();
                paragraph.Alignment = Element.ALIGN_RIGHT;
                jpg.ScaleToFit(140f, 130F); //ancho y largo de la imagen
                jpg.Alignment = Image.ALIGN_RIGHT;
                jpg.SetAbsolutePosition(20f, 720f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera
                                                    //_documento.Add(jpg);
                                                    //  doc.Add(paragraph);
                tablapro.AddCell(jpg);
            }
            catch (Exception err)
            {
                tablapro.AddCell("");
            }




            PdfPCell celda0 = new PdfPCell(new Phrase(_Titulo, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            celda0.Border = Rectangle.NO_BORDER;
            celda0.HorizontalAlignment = Element.ALIGN_CENTER;
            tablapro.AddCell(celda0);
            PdfPCell celda1 = new PdfPCell(new Phrase("\nFECHA " +DateTime.Now + "\nCÓDIGO: F56-19 ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            celda1.Border = Rectangle.NO_BORDER;
            celda1.HorizontalAlignment = Element.ALIGN_CENTER;
            tablapro.AddCell(celda1);


            _documento.Add(tablapro);
        }



        private void ReportHeader()
        {
            #region Table head

            Empresa empresa = new EmpresaContext().empresas.Find(2);


           

            float[] anchoColumnasencart = { 50f, 40f, 290f, 70f, 70f, 70f };

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
            _PdfPCell = new PdfPCell(new Phrase("ACOMODO", _fontStyleEncabezado2));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
              _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("EMBOBINADO", _fontStyleEncabezado2));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = COLORDEREPORTE;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, COLORDEFUENTEREPORTE);
            _PdfPCell = new PdfPCell(new Phrase("CYREL", _fontStyleEncabezado2));
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

               

                Phrase p1 = new Phrase(text +"\n", _fontStylecliente);

               


                _documento.Add(p1);

                 _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 14, Font.NORMAL,new BaseColor( System.Drawing.Color.Blue) );
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
                        if (ProcesoProgramado !=0 )
                        {
                            float[] anchoColumnasordenes = { 50f, 40f, 290f, 70f, 70f, 70f };

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


                            //Caracteristica carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where ID=" + orden.IDCaracteristica).FirstOrDefault();

                            //OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(orden.IDOrden);

                            FormulaSiaapi.Formulas atri = new FormulaSiaapi.Formulas();
                            string Embobinado = "";
                            try
                            {
                                Embobinado = atri.getValorCadena("EMBOBINADO", orden.Presentacion);
                            }
                            catch (Exception err)
                            {

                            }

                            string Cyrel = "";
                            try
                            {
                                Cyrel = atri.getValorCadena("CYREL", orden.Presentacion);
                            }
                            catch (Exception err)
                            {

                            }

                            PdfPCell _PdfPCellSB = new PdfPCell(new Phrase(""));
                            _PdfPCellSB.Border = 0;
                            _PdfPCellSB.HorizontalAlignment = Element.ALIGN_CENTER;
                            _PdfPCellSB.VerticalAlignment = Element.ALIGN_MIDDLE;

                            tablae.AddCell(_PdfPCellSB);


                            PdfPCell _PdfPCellS3 = new PdfPCell(new Phrase(Embobinado));
                            _PdfPCellS3.Border = 0;
                            _PdfPCellS3.HorizontalAlignment = Element.ALIGN_CENTER;
                            _PdfPCellS3.VerticalAlignment = Element.ALIGN_MIDDLE;

                            tablae.AddCell(_PdfPCellS3);

                            PdfPCell _PdfPCellS4 = new PdfPCell(new Phrase(Cyrel));
                            _PdfPCellS4.Border = 0;
                            _PdfPCellS4.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCellS4.VerticalAlignment = Element.ALIGN_MIDDLE;

                            tablae.AddCell(_PdfPCellS4);






                            ///  veamos los suajes
                            ///  

                            List<ArticuloProduccion> suajes = new ArticuloProduccionRealContext().Database.SqlQuery<ArticuloProduccion>("select * from ArticuloProduccion where idorden=" + orden.IDOrden + " and Idtipoarticulo=2").ToList();

                            float[] anchoColumnassuajes = { 80, 290f, 70f, 70f, 70f };

                            PdfPTable tablasuaje = new PdfPTable(anchoColumnassuajes);
                            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                            tablasuaje.DefaultCell.Border = Rectangle.NO_BORDER;
                            tablasuaje.HorizontalAlignment = Element.ALIGN_LEFT;
                            tablasuaje.WidthPercentage = 100;


                            foreach (ArticuloProduccion elementosuaje in suajes)
                            {
                                SuajeCaracteristicas suaje = new SuajeCaracteristicas();
                                Articulo zrt = new ArticuloContext().Articulo.Find(elementosuaje.IDArticulo);
                                string cref = zrt.Cref;
                                suaje = getSuaje(elementosuaje.IDCaracteristica);


                                string cadena2suaje = string.Empty;
                                try
                                {
                                    cadena2suaje = " " + " CAV EJE: " + suaje.CavidadEje + " EJE: " + suaje.Eje + " AVANCE: " + suaje.Avance + " TH " + suaje.TH;
                                }
                                catch (Exception err)
                                {

                                }

                                Font _fontStylesuaje = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);

                                Font _fontStylecref = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL);

                                Phrase frasesuaje = new Phrase(cadena2suaje + "\n", _fontStylesuaje);
                                Phrase pcref = new Phrase(cref, _fontStylecref);


                                PdfPCell _PdfPCellS1 = new PdfPCell(pcref);
                                _PdfPCellS1.Border = 0;
                                _PdfPCellS1.HorizontalAlignment = Element.ALIGN_RIGHT;
                                _PdfPCellS1.VerticalAlignment = Element.ALIGN_MIDDLE;

                                tablasuaje.AddCell(_PdfPCellS1);

                                PdfPCell _PdfPCellS2 = new PdfPCell(frasesuaje);
                                _PdfPCellS2.Border = 0;
                                _PdfPCellS2.HorizontalAlignment = Element.ALIGN_LEFT;
                                //     _PdfPCellS2.VerticalAlignment = Element.ALIGN_MIDDLE;

                                tablasuaje.AddCell(_PdfPCellS2);


                                Caracteristica carasuajcompleto = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id =" + elementosuaje.IDCaracteristica).FirstOrDefault();

                                string acomodo = "";

                                string Ejemaq = string.Empty;

                                try

                                {
                                    Ejemaq = atri.getValorCadena("EJE MAQ", carasuajcompleto.Presentacion);
                                }
                                catch (Exception err)
                                {
                                    Ejemaq = string.Empty;
                                }

                                if (Ejemaq != string.Empty && Ejemaq.Contains("13.5"))
                                {
                                    acomodo = "13.5";
                                }
                                else
                                {
                                    acomodo = "Centrar";
                                }


                                PdfPCell _PdfPCellSA = new PdfPCell(new Phrase(acomodo, _fontStylesuaje));
                                _PdfPCellSA.Border = 0;
                                _PdfPCellSA.HorizontalAlignment = Element.ALIGN_CENTER;

                                tablasuaje.AddCell(_PdfPCellSA);

                                tablasuaje.CompleteRow();

                            }

                            tablae.CompleteRow();
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


                        float[] anchoColumnasordenes = { 50f, 40f, 290f, 70f, 70f, 70f };

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


                        Caracteristica carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where ID=" + orden.IDCaracteristica).FirstOrDefault();

                        FormulaSiaapi.Formulas atri = new FormulaSiaapi.Formulas();
                        string Embobinado = "";
                        try
                        {
                            Embobinado = atri.getValorCadena("EMBOBINADO", orden.Presentacion);
                        }
                        catch (Exception err)
                        {

                        }

                        string Cyrel = "";
                        try
                        {
                            Cyrel = atri.getValorCadena("CYREL", orden.Presentacion);
                        }
                        catch (Exception err)
                        {

                        }

                        PdfPCell _PdfPCellSB = new PdfPCell(new Phrase(""));
                        _PdfPCellSB.Border = 0;
                        _PdfPCellSB.HorizontalAlignment = Element.ALIGN_CENTER;
                        _PdfPCellSB.VerticalAlignment = Element.ALIGN_MIDDLE;

                        tablae.AddCell(_PdfPCellSB);


                        PdfPCell _PdfPCellS3 = new PdfPCell(new Phrase(Embobinado));
                        _PdfPCellS3.Border = 0;
                        _PdfPCellS3.HorizontalAlignment = Element.ALIGN_CENTER;
                        _PdfPCellS3.VerticalAlignment = Element.ALIGN_MIDDLE;

                        tablae.AddCell(_PdfPCellS3);

                        PdfPCell _PdfPCellS4 = new PdfPCell(new Phrase(Cyrel));
                        _PdfPCellS4.Border = 0;
                        _PdfPCellS4.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCellS4.VerticalAlignment = Element.ALIGN_MIDDLE;

                        tablae.AddCell(_PdfPCellS4);






                        ///  veamos los suajes
                        ///  

                        List<ArticuloProduccion> suajes = new ArticuloProduccionRealContext().Database.SqlQuery<ArticuloProduccion>("select * from ArticuloProduccion where idorden=" + orden.IDOrden + " and Idtipoarticulo=2").ToList();

                        float[] anchoColumnassuajes = { 80, 290f, 70f, 70f, 70f };

                        PdfPTable tablasuaje = new PdfPTable(anchoColumnassuajes);
                        //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                        tablasuaje.DefaultCell.Border = Rectangle.NO_BORDER;
                        tablasuaje.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablasuaje.WidthPercentage = 100;


                        foreach (ArticuloProduccion elementosuaje in suajes)
                        {
                            SuajeCaracteristicas suaje = new SuajeCaracteristicas();
                            Articulo zrt = new ArticuloContext().Articulo.Find(elementosuaje.IDArticulo);
                            string cref = zrt.Cref;
                            suaje = getSuaje(elementosuaje.IDCaracteristica);


                            string cadena2suaje = string.Empty;
                            try
                            {
                                cadena2suaje = " " + " CAV EJE: " + suaje.CavidadEje + " EJE: " + suaje.Eje + " AVANCE: " + suaje.Avance + " TH " + suaje.TH;
                            }
                            catch (Exception err)
                            {

                            }

                            Font _fontStylesuaje = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);

                            Font _fontStylecref = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL);

                            Phrase frasesuaje = new Phrase(cadena2suaje + "\n", _fontStylesuaje);
                            Phrase pcref = new Phrase(cref, _fontStylecref);


                            PdfPCell _PdfPCellS1 = new PdfPCell(pcref);
                            _PdfPCellS1.Border = 0;
                            _PdfPCellS1.HorizontalAlignment = Element.ALIGN_RIGHT;
                            _PdfPCellS1.VerticalAlignment = Element.ALIGN_MIDDLE;

                            tablasuaje.AddCell(_PdfPCellS1);

                            PdfPCell _PdfPCellS2 = new PdfPCell(frasesuaje);
                            _PdfPCellS2.Border = 0;
                            _PdfPCellS2.HorizontalAlignment = Element.ALIGN_LEFT;
                            //     _PdfPCellS2.VerticalAlignment = Element.ALIGN_MIDDLE;

                            tablasuaje.AddCell(_PdfPCellS2);


                            Caracteristica carasuajcompleto = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id =" + elementosuaje.IDCaracteristica).FirstOrDefault();

                            string acomodo = "";

                            string Ejemaq = string.Empty;

                            try

                            {
                                Ejemaq = atri.getValorCadena("EJE MAQ", carasuajcompleto.Presentacion);
                            }
                            catch (Exception err)
                            {
                                Ejemaq = string.Empty;
                            }

                            if (Ejemaq != string.Empty && Ejemaq.Contains("13.5"))
                            {
                                acomodo = "13.5";
                            }
                            else
                            {
                                acomodo = "Centrar";
                            }


                            PdfPCell _PdfPCellSA = new PdfPCell(new Phrase(acomodo, _fontStylesuaje));
                            _PdfPCellSA.Border = 0;
                            _PdfPCellSA.HorizontalAlignment = Element.ALIGN_CENTER;

                            tablasuaje.AddCell(_PdfPCellSA);

                            tablasuaje.CompleteRow();

                        }

                        tablae.CompleteRow();
                        _documento.Add(tablae);
                        _documento.Add(tablasuaje);
                    }
                }

            }



        }

        public SuajeCaracteristicas getSuaje(int IDSuaje)
        {
            Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("Select * from Caracteristica where ID=" + IDSuaje).ToList().FirstOrDefault();
            SuajeCaracteristicas suajec = new SuajeCaracteristicas();
            FormulaSiaapi.Formulas formula = new FormulaSiaapi.Formulas();
            suajec.Eje = 0;

            try
            {
                suajec.Eje = decimal.Parse(formula.getvalor("EJE", cara.Presentacion).ToString());
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Eje = 0;
            }


            suajec.Avance = 0;
            try
            {
                suajec.Avance = decimal.Parse(formula.getvalor("AVANCE", cara.Presentacion).ToString());
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Avance = 0;
            }



            suajec.CavidadAvance = 2;

            try
            {
                suajec.CavidadAvance = int.Parse(formula.getvalor("REP AVANCE", cara.Presentacion).ToString());
                if (suajec.CavidadAvance == 0)
                {
                    suajec.CavidadAvance = int.Parse(formula.getvalor("REPETICIONES AVANCE", cara.Presentacion).ToString());
                    if (suajec.CavidadAvance == 0)
                    {
                        suajec.CavidadAvance = int.Parse(formula.getvalor("REPETICIONES AL AVANCE", cara.Presentacion).ToString());
                        if (suajec.CavidadAvance == 0)
                        {
                            suajec.CavidadAvance = int.Parse(formula.getvalor("CAV AVA", cara.Presentacion).ToString());
                            if (suajec.CavidadAvance == 0)
                            {
                                suajec.CavidadAvance = int.Parse(formula.getvalor("CAV AVANCE", cara.Presentacion).ToString());
                                if (suajec.CavidadAvance == 0)
                                {
                                    suajec.CavidadAvance = int.Parse(formula.getvalor("CAVIDADES AVANCE", cara.Presentacion).ToString());
                                    if (suajec.CavidadAvance == 0)
                                    {
                                        suajec.CavidadAvance = int.Parse(formula.getvalor("CAVIDADES AL AVANCE", cara.Presentacion).ToString());

                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.CavidadAvance = 2;
            }




            suajec.CavidadEje = 2;

            try
            {
                suajec.CavidadEje = int.Parse(formula.getvalor("REP EJE", cara.Presentacion).ToString());
                if (suajec.CavidadEje == 0)
                {
                    suajec.CavidadEje = int.Parse(formula.getvalor("REPETICIONES EJE", cara.Presentacion).ToString());
                    if (suajec.CavidadEje == 0)
                    {
                        suajec.CavidadEje = int.Parse(formula.getvalor("REPETICIONES AL EJE", cara.Presentacion).ToString());
                        if (suajec.CavidadEje == 0)
                        {
                            suajec.CavidadEje = int.Parse(formula.getvalor("CAV EJE", cara.Presentacion).ToString());
                            if (suajec.CavidadEje == 0)
                            {
                                suajec.CavidadEje = int.Parse(formula.getvalor("CAVIDADES EJE", cara.Presentacion).ToString());
                                if (suajec.CavidadEje == 0)
                                {
                                    suajec.CavidadEje = int.Parse(formula.getvalor("CAVIDADES AL EJE", cara.Presentacion).ToString());

                                }
                            }
                        }
                    }
                }

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.CavidadEje = 2;
            }




            suajec.Gapeje = 0;
            try
            {
                suajec.Gapeje = decimal.Parse(formula.getvalor("GAP EJE", cara.Presentacion).ToString());
                if (suajec.Gapeje == 0M)
                {
                    suajec.Gapeje = decimal.Parse(formula.getvalor("GAP AL EJE", cara.Presentacion).ToString());

                }
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Gapeje = 0;
            }




            suajec.Gapavance = 3;
            try
            {
                suajec.Gapavance = decimal.Parse(formula.getvalor("GAP AVANCE", cara.Presentacion).ToString());

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Gapavance = 2;
            }




            suajec.RepAvance = 0;
            try
            {
                suajec.RepAvance = int.Parse(formula.getvalor("REP AVANCE", cara.Presentacion).ToString());


            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.RepAvance = 0;
            }

            suajec.Corte = "";
            try
            {
                suajec.Corte = formula.getValorCadena("CORTE", cara.Presentacion).ToString();

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Corte = "";
            }
            suajec.Material = "";
            try
            {
                suajec.Material = formula.getValorCadena("MATERIAL", cara.Presentacion).ToString();

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Material = "";
            }


            suajec.Material = "";
            try
            {
                suajec.Material = formula.getValorCadena("MATERIAL", cara.Presentacion).ToString();

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Material = "";
            }




            suajec.TH = 0;


            //elemento.TH = suajec.TH;

            if (suajec.TH == 0)
            {

                try
                {
                    suajec.TH = int.Parse(formula.getValorCadena("TH", cara.Presentacion).ToString());
                    if (suajec.TH == 0)
                    {
                        suajec.TH = int.Parse(formula.getValorCadena("DIENTES", cara.Presentacion).ToString());
                        if (suajec.TH == 0)
                        {
                            suajec.TH = int.Parse(formula.getValorCadena("DIENTES_TH", cara.Presentacion).ToString());
                            if (suajec.TH == 0)
                            {
                                suajec.TH = int.Parse(formula.getValorCadena("NO DE DIENTES", cara.Presentacion).ToString());
                                if (suajec.TH == 0)
                                {
                                    suajec.TH = int.Parse(formula.getValorCadena("NUMERO DE DIENTES", cara.Presentacion).ToString());
                                    if (suajec.TH == 0)
                                    {
                                        suajec.TH = int.Parse(formula.getValorCadena("No DIENTES", cara.Presentacion).ToString());
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.TH = 0;
                }
            }
            if (suajec.TH == 0)
            {
                try
                {
                    suajec.Alma = formula.getValorCadena("ALMA", cara.Presentacion).ToString();

                    string alma = suajec.Alma.Replace(" TH", "");

                    suajec.TH = int.Parse(alma);

                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.TH = 0;
                }


            }
            if (suajec.TH == 0)
            {
                try
                {
                    suajec.TH = int.Parse(formula.getvalor("DIENTES_TH", cara.Presentacion).ToString());

                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.TH = 0;
                }
            }

            return suajec;
        }
        #endregion
        #region Extensión de la clase pdfPageEvenHelper
        public class ITextEventsSuaj : PdfPageEventHelper
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
                String TextRevision = "REV. 2";



                cb.BeginText();
                cb.SetFontAndSize(bf, 9);
                cb.SetTextMatrix(document.PageSize.GetRight(70), document.PageSize.GetBottom(30));
                //cb.MoveText(500,30);
                //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
                cb.ShowText(text);
                cb.EndText();
                float len = bf.GetWidthPoint(text, 9);
                cb.AddTemplate(footerTemplate, document.PageSize.GetRight(70) + len, document.PageSize.GetBottom(30));

                float[] anchoColumasTablaTotales = { 150f, 300f, 100f };
                PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
                tabla.DefaultCell.Border = Rectangle.NO_BORDER;
                tabla.SetTotalWidth(anchoColumasTablaTotales);
                tabla.HorizontalAlignment = Element.ALIGN_CENTER;
                tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                tabla.LockedWidth = true;
                tabla.AddCell(new Phrase("Class Labels  S. de R.L. de C.V \n Fecha de Revisión: 16-12-2020", new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.AddCell(new Phrase("" + "\n" + TextRevision, new Font(Font.FontFamily.HELVETICA, 8)));
               
                //tabla.AddCell(new Phrase(TextRevision, new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(40), writer.DirectContent);





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






