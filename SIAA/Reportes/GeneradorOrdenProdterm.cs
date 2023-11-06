using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.ClasesProduccion;
using SIAAPI.Models.Calidad;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.PlaneacionProduccion;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace SIAAPI.Reportes
{

    public class 
        CreaOrdenTermoPDF
    {
       OrdenProduccion orden = new OrdenProduccion();
        MemoryStream _memoryStream = new MemoryStream();
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;
        public CMYKColor colordefinido;
        public CMYKColor colorencabezadodefinido;
        public String Embobinado = "A";
        ClsCotizador especificacion = new ClsCotizador();
        decimal Vueltas = 1;
        decimal Rollos = 1;
        decimal Progrenmetros = 1;
        public string nombreDocumento = string.Empty;

        public CreaOrdenTermoPDF(System.Drawing.Image logo,int id)
        {
            orden = new OrdenProduccionContext().OrdenesProduccion.Find(id);
            _documento = new Document(PageSize.LETTER.Rotate());
            nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/OrdenProduccion" + id + ".pdf");

            _documento.SetMargins(5,5,10,50);

            try
            {
                if (File.Exists(nombreDocumento))
                {
                    nombreDocumento = HttpContext.Current.Server.MapPath("~/Documentostemporales/OrdenProduccion" + id + "" + DateTime.Now.Day + DateTime.Now.Day + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".pdf"); ;
                }

            }
            catch (Exception ERR)
            {
                string mensajederror = ERR.Message;
            }


            _writer = PdfWriter.GetInstance(_documento, new FileStream(nombreDocumento, System.IO.FileMode.Create));

            _writer.PageEvent = new ITextEventsOrdenProducciontermo(); // invoca la clase que esta mas abajo correspondiente al pie de pagina
            ClsColoresReporte colorr = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            colordefinido = colorr.color;

            ClsColoresReporte colorencabezado = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado);



            colorencabezadodefinido = colorencabezado.color;

       

            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();


            AgregarLogo(logo);

            AgregarDatosEntrega(id);
            AgregarTitulo(id);
            AgregarDatosGenerales(id);
            AgregarEspecificacionesTecnicas(id);
            AgregarDatosArticulos(id);
            /*AgregarDatosCalidad(id)*/;
            AgregarDatosLiberacion(id);
            //Cerramos el documento
            _documento.Close();

            //HttpContext.Current.Response.ContentType = "pdf/application";
            //HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"OrdenProduccion-" + id+ ".pdf" + "\"");
            //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //HttpContext.Current.Response.Write(_documento);
            //HttpContext.Current.Response.End();

        }

   


        #region Leer datos del .xml





        #endregion

        #region Escribir datos en el .pdf

        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {
            if (logoEmpresa == null)
                return;
            //Agrego la imagen al documento
            Image imagen = Image.GetInstance(logoEmpresa, BaseColor.BLACK);
            imagen.ScaleToFit(60, 40);
            imagen.Alignment = Element.ALIGN_TOP;
            Chunk logo = new Chunk(imagen, 1, -40);

         
            _documento.Add(logo);
        }
        
        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("Orden Producción");
            _documento.AddSubject("Orden Producción");
            _documento.AddTitle("ORDEN PRODUCCIÓN");
            //_documento.SetMargins(5, 5, 5, 5);
        }

        private void AgregarDatosGenerales(int id)
        {

            float[] anchoColumnasTablaDatos = { 600f };
            PdfPTable tablaDatosPrincipal = new PdfPTable(anchoColumnasTablaDatos);
            tablaDatosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.SetTotalWidth(anchoColumnasTablaDatos);
            tablaDatosPrincipal.SpacingBefore = 15;
            tablaDatosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosPrincipal.LockedWidth = true;
            PdfPCell celdanada = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            celdanada.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.AddCell(celdanada);
            //_documento.Add(tablaDatosPrincipal);


            //Datos de los productos
            float[] tamanoColumnas = { 780f};
            PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);

            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;

            
            PdfPCell celda00 = new PdfPCell(new Phrase("DATOS GENERALES", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));


            celda00.HorizontalAlignment = Element.ALIGN_CENTER;
            //celda00.BackgroundColor = colordefinido;

            tablaProductosTitulos.AddCell(celda00);
            //_documento.Add(tablaProductosTitulos);

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




           
            Clientes cliente = new ClientesContext().Clientes.Find(orden.Cliente.IDCliente);
            Vendedor vendedor = new VendedorContext().Vendedores.Find(cliente.IDVendedor);
            Articulo articulo = new ArticuloContext().Articulo.Find(orden.IDArticulo);
            //Datos del receptor
            float[] anchoColumnas = { 60f, 100f, 65f, 200f, 105f, 140f, 60f, 50f };
            PdfPTable tabla = new PdfPTable(anchoColumnas);
            tabla.DefaultCell.Border = Rectangle.NO_BORDER;
            tabla.SetTotalWidth(anchoColumnas);
            tabla.HorizontalAlignment = Element.ALIGN_LEFT;
            tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tabla.LockedWidth = true;


            PdfPTable tabla1 = new PdfPTable(anchoColumnas);
            tabla1.DefaultCell.Border = Rectangle.NO_BORDER;
            tabla1.SetTotalWidth(anchoColumnas);
            tabla1.HorizontalAlignment = Element.ALIGN_LEFT;
            tabla1.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tabla1.LockedWidth = true;



            PdfPCell celda0 = new PdfPCell(new Phrase("Pedido", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
            PdfPCell celda1 = new PdfPCell(new Phrase(orden.IDPedido.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda2 = new PdfPCell(new Phrase("Clave", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
            PdfPCell celda3 = new PdfPCell(new Phrase(articulo.Cref, new Font(Font.FontFamily.HELVETICA, 8)));

            celda1.HorizontalAlignment = Element.ALIGN_CENTER;

           
            
          

            PdfPCell celda4 = new PdfPCell(new Phrase("Fecha Entrega", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
            PdfPCell celda5 = new PdfPCell(new Phrase(orden.FechaCompromiso.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda6 = new PdfPCell(new Phrase("Cliente: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
            if ( cliente.Mayorista)
            {
                celda6 = new PdfPCell(new Phrase("Distibuidor", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
            }
            PdfPCell celda7 = new PdfPCell(new Phrase(cliente.Nombre, new Font(Font.FontFamily.HELVETICA, 8)));


           

            PdfPCell celda8 = new PdfPCell(new Phrase("Cantidad", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, colorencabezadodefinido)));
            PdfPCell celda9 = new PdfPCell(new Phrase(orden.Cantidad.ToString() +" "+ articulo.c_ClaveUnidad.ClaveUnidad, new Font(Font.FontFamily.HELVETICA, 16)));
            PdfPCell celda10 = new PdfPCell(new Phrase("Vendedor", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
            PdfPCell celda11 = new PdfPCell(new Phrase(vendedor.Nombre, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda12 = new PdfPCell(new Phrase("Fact. Exacta", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
            String factuexacta = "No";
            if (cliente.FacturacionExacta)
            {
                 factuexacta = "Si";
            }
            PdfPCell celda13 = new PdfPCell(new Phrase(factuexacta, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda14 = new PdfPCell(new Phrase("Cert.Calidad", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, colorencabezadodefinido)));
            String certificado = "No";
            if (cliente.CertificadoCalidad)
            {
                certificado = "Si";
            }
            PdfPCell celda15 = new PdfPCell(new Phrase(certificado, new Font(Font.FontFamily.HELVETICA, 8)));


            celda9.HorizontalAlignment = Element.ALIGN_CENTER;
            //celda8.Border = Rectangle.NO_BORDER;
            //celda9.Border = Rectangle.NO_BORDER;
            //celda10.Border = Rectangle.NO_BORDER;
            //celda11.Border = Rectangle.NO_BORDER;


            celda0.BackgroundColor = colordefinido;
            celda2.BackgroundColor = colordefinido;
            celda4.BackgroundColor = colordefinido;
            celda6.BackgroundColor = colordefinido;
            celda8.BackgroundColor = colordefinido;
            celda10.BackgroundColor = colordefinido;
            celda12.BackgroundColor = colordefinido;
            celda14.BackgroundColor = colordefinido;


            tabla.AddCell(celda0);
            tabla1.AddCell(celda1);
            tabla.AddCell(celda2);
            tabla1.AddCell(celda3);
            tabla.AddCell(celda4);
            tabla1.AddCell(celda5);
            tabla.AddCell(celda6);
            tabla1.AddCell(celda7);



            tabla.AddCell(celda8);
            tabla1.AddCell(celda9);
            tabla.AddCell(celda10);
            tabla1.AddCell(celda11);
            tabla.AddCell(celda12);
            tabla1.AddCell(celda13);
            tabla.AddCell(celda14);
            tabla1.AddCell(celda15);

            _documento.Add(tabla);
            _documento.Add(tabla1);


           
        }
        private void AgregarEspecificacionesTecnicas(int id)
        {
            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(id);
            Clientes cliente = new ClientesContext().Clientes.Find(orden.Cliente.IDCliente);
            Vendedor vendedor = new VendedorContext().Vendedores.Find(cliente.IDVendedor);
            Articulo articulo = new ArticuloContext().Articulo.Find(orden.IDArticulo);
            DetPedido detpedido = new PedidoContext().DetPedido.Find(orden.IDDetPedido);
            EncPedido pedido = new PedidoContext().EncPedidos.Find(orden.IDPedido);

            float[] anchoColumnasTablaDatos = { 600f };
            PdfPTable tablaDatosPrincipal = new PdfPTable(anchoColumnasTablaDatos);
            tablaDatosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.SetTotalWidth(anchoColumnasTablaDatos);
            tablaDatosPrincipal.SpacingBefore = 5;
            tablaDatosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosPrincipal.LockedWidth = true;
            PdfPCell celdanada = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            celdanada.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.AddCell(celdanada);
            

            //Datos de los productos
            float[] tamanoColumnas = { 780f };
            PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);
            tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;
            PdfPCell celda00 = new PdfPCell(new Phrase("ESPECIFICACIONES TÉCNICAS", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
            celda00.BackgroundColor = colordefinido;
            celda00.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaProductosTitulos.AddCell(celda00);
            _documento.Add(tablaProductosTitulos);

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //Paragraph pd = new Paragraph();
            //pd.Alignment = Element.ALIGN_LEFT;
            //pd.Leading = 10;
            //pd.Add(new Phrase("Artículo: \n", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //pd.SpacingAfter = 1;
            //_documento.Add(pd);
            
            Paragraph p3 = new Paragraph();
            p3.Alignment = Element.ALIGN_LEFT;
            p3.Leading = 10;
            p3.Add(new Phrase("\n" + orden.Descripcion, new Font(Font.FontFamily.HELVETICA, 13)));
            p3.SpacingAfter = 1;
            _documento.Add(p3);

            //Paragraph pc = new Paragraph();
            //pc.Alignment = Element.ALIGN_LEFT;
            //pc.Leading = 12;
            //pc.Add(new Phrase("Presentación: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //pc.SpacingAfter = 1;
            //_documento.Add(pc);
            ////////////////////////////////////////////////////////////////////////////////////////////
            string[] arraydatos;
            arraydatos = orden.Presentacion.Split(',');
            int cuantos = arraydatos.Length;

            string acc = null;
            string valor = null;
            float[] tamanoenc = new float[cuantos + 1]; ;
            for (int i = 0; i < arraydatos.Length; i++)
            {
                tamanoenc[i] = 50f;

            }
            tamanoenc[cuantos] = 100f;
            

            PdfPTable tablaencpre = new PdfPTable(tamanoenc);
            PdfPTable tablaencpre1 = new PdfPTable(tamanoenc);


            for (int i = 0; i < arraydatos.Length; i++)
            {
                string cuenta = arraydatos[i];
                string[] arraydatoscortados;
                arraydatoscortados = cuenta.Split(':');
                acc =  arraydatoscortados[0] /*+ ": " + arraydatoscortados[1] + "   "*/;
                valor = arraydatoscortados[1];
                
                if (acc=="EMBOBINADO")
                {
                    if (valor=="")
                    {
                        valor = "A";
                    }
                    Embobinado = valor;
                }

                tablaencpre.SetTotalWidth(tamanoenc);
                tablaencpre.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaencpre.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaencpre.LockedWidth = true;
                tablaencpre1.SetTotalWidth(tamanoenc);
                tablaencpre1.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaencpre1.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaencpre1.LockedWidth = true;
                PdfPCell celdaprenc = new PdfPCell(new Phrase(acc, new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD, colorencabezadodefinido)));
                PdfPCell celdavalor = new PdfPCell(new Phrase(valor, new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
                celdaprenc.BackgroundColor = colordefinido;
                tablaencpre.AddCell(celdaprenc);
                tablaencpre1.AddCell(celdavalor);





            }

            PdfPCell celdaprenc2 = new PdfPCell(new Phrase("Indicaciones", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celdavalor2 = new PdfPCell(new Phrase(orden.Indicaciones, new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            tablaencpre.AddCell(celdaprenc2);
            tablaencpre1.AddCell(celdavalor2);

            _documento.Add(tablaencpre);
            _documento.Add(tablaencpre1);
            ///////////////////////////////////////////////////////////////////////////////////////////


            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            try
            {
                Image jpg = Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/Upload/" + articulo.nameFoto + ""));

                Paragraph paragraph = new Paragraph();
                paragraph.Alignment = Element.ALIGN_RIGHT;
                jpg.ScaleToFit(100f, 100f);
                jpg.Alignment = Image.ALIGN_RIGHT;
                jpg.SetAbsolutePosition(600f, 400f);
                _documento.Add(jpg);
                _documento.Add(paragraph);
            }
            catch (Exception err)
            {

            }
           


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            float[] anchoColumnas = { 55f, 150F, 55f, 150F, 70f, 150f };
            PdfPTable tabla = new PdfPTable(anchoColumnas);
            tabla.DefaultCell.Border = Rectangle.NO_BORDER;
            tabla.SetTotalWidth(anchoColumnas);
            tabla.HorizontalAlignment = Element.ALIGN_LEFT;
            tabla.LockedWidth = true;



            PdfPCell celda0 = new PdfPCell(new Phrase("Observación Pedido: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda1 = new PdfPCell(new Phrase(pedido.Observacion, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda2 = new PdfPCell(new Phrase("Observación Específica: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda3 = new PdfPCell(new Phrase(detpedido.Nota, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda4 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda5 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

            celda0.Border = Rectangle.NO_BORDER;
            celda1.Border = Rectangle.NO_BORDER;
            celda2.Border = Rectangle.NO_BORDER;
            celda3.Border = Rectangle.NO_BORDER;
            celda4.Border = Rectangle.NO_BORDER;
            celda5.Border = Rectangle.NO_BORDER;

            tabla.AddCell(celda0);
            tabla.AddCell(celda1);
            tabla.AddCell(celda2);
            tabla.AddCell(celda3);
            //tabla.AddCell(celda4);
            tabla.AddCell(celda5);

            _documento.Add(tabla);

            

        }
        //private void AgregarDatosCalidad(int id)
        //{
        //    float[] anchoColumnasTablaDatos = { 600f };
        //    PdfPTable tablaDatosPrincipal = new PdfPTable(anchoColumnasTablaDatos);
        //    tablaDatosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
        //    tablaDatosPrincipal.SetTotalWidth(anchoColumnasTablaDatos);
        //    tablaDatosPrincipal.SpacingBefore = 5;
        //    tablaDatosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaDatosPrincipal.LockedWidth = true;
        //    PdfPCell celdanada = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
        //    celdanada.Border = Rectangle.NO_BORDER;
        //    tablaDatosPrincipal.AddCell(celdanada);
        //    _documento.Add(tablaDatosPrincipal);

        //    OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(id);
        //    Clientes cliente = new ClientesContext().Clientes.Find(orden.Cliente.IDCliente);
        //    Vendedor vendedor = new VendedorContext().Vendedores.Find(cliente.IDVendedor);
        //    Articulo articulo = new ArticuloContext().Articulo.Find(orden.IDArticulo);

        //    float[] tamanoColumnas = { 780f };
        //    PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);
        //    tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
        //    tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaProductosTitulos.LockedWidth = true;


        //    PdfPCell celda00 = new PdfPCell(new Phrase("CALIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));

        //    celda00.HorizontalAlignment = Element.ALIGN_CENTER;


        //    celda00.BackgroundColor = colordefinido;

        //    tablaProductosTitulos.AddCell(celda00);
        //    _documento.Add(tablaProductosTitulos);

        //    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //    Paragraph pd = new Paragraph();
        //    pd.Alignment = Element.ALIGN_LEFT;
        //    pd.Leading = 10;
        //    pd.Add(new Phrase("Observación calidad: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    pd.SpacingAfter = 5;
        //    _documento.Add(pd);


        //    Paragraph p3 = new Paragraph();
        //    p3.Alignment = Element.ALIGN_LEFT;
        //    p3.Leading = 10;
        //    p3.Add(new Phrase(articulo.Obscalidad, new Font(Font.FontFamily.HELVETICA, 8)));
        //    pd.SpacingAfter = 5;
        //    _documento.Add(p3);

        //    _documento.Add(tablaDatosPrincipal);
        //    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //    //Datos del receptor
        //    float[] anchoColumnas = { 50f, 150F, 50f, 150F, 70f, 150f };
        //    PdfPTable tabla = new PdfPTable(anchoColumnas);
        //    tabla.DefaultCell.Border = Rectangle.NO_BORDER;
        //    tabla.SetTotalWidth(anchoColumnas);
        //    tabla.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tabla.LockedWidth = true;


            
        //    PdfPCell celda0 = new PdfPCell(new Phrase("AQL: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda1 = new PdfPCell(new Phrase(articulo.AQLCalidad.Descripcion, new Font(Font.FontFamily.HELVETICA, 8)));
        //    PdfPCell celda2 = new PdfPCell(new Phrase("Muestreo: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda3 = new PdfPCell(new Phrase(articulo.Muestreo.Descripcion, new Font(Font.FontFamily.HELVETICA, 8)));
        //    PdfPCell celda4 = new PdfPCell(new Phrase("Inspección:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda5 = new PdfPCell(new Phrase(articulo.Inspeccion.Descripcion, new Font(Font.FontFamily.HELVETICA, 8)));

        //    celda0.Border = Rectangle.NO_BORDER;
        //    celda1.Border = Rectangle.NO_BORDER;
        //    celda2.Border = Rectangle.NO_BORDER;
        //    celda3.Border = Rectangle.NO_BORDER;
        //    celda4.Border = Rectangle.NO_BORDER;
        //    celda5.Border = Rectangle.NO_BORDER;
        //    tabla.AddCell(celda0);
        //    tabla.AddCell(celda1);
        //    tabla.AddCell(celda2);
        //    tabla.AddCell(celda3);
        //    tabla.AddCell(celda4);
        //    tabla.AddCell(celda5);
         

        //    _documento.Add(tabla);

        //}
        private void AgregarDatosArticulos(int id)
        {
            try
            {
                OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(id);
                float[] anchoColumnasTablaDatos = { 780f };
                PdfPTable tablaDatosPrincipal1 = new PdfPTable(anchoColumnasTablaDatos);
                tablaDatosPrincipal1.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaDatosPrincipal1.SetTotalWidth(anchoColumnasTablaDatos);
                tablaDatosPrincipal1.SpacingBefore = 5;
                tablaDatosPrincipal1.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaDatosPrincipal1.LockedWidth = true;
                PdfPCell celdanada1 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                celdanada1.Border = Rectangle.NO_BORDER;
                tablaDatosPrincipal1.AddCell(celdanada1);
                _documento.Add(tablaDatosPrincipal1);

                Paragraph mo = new Paragraph();
                mo.Alignment = Element.ALIGN_CENTER;
                mo.Leading = 10;
                mo.Add(new Phrase("Modelo de Producción: " + orden.ModeloProduccion.Descripcion, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, new BaseColor(System.Drawing.Color.Navy))));
                _documento.Add(mo);




                ProcesoContext dbp = new ProcesoContext();
                //Datos del receptor
                Proceso prensa = dbp.Procesos.Find(5); /// prensa
                Paragraph prensatitulo = new Paragraph();

                List<VArticulosProduccion> articulosproduccion = new ArticulosProduccionContext().Database.SqlQuery<VArticulosProduccion>("select AP.Existe,Ap.Cantidad as Cantidad, A.Cref as Cref,AP.IDArtProd,A.Descripcion as Articulo,TA.Descripcion as TipoArticulo,C.Presentacion as Caracteristica,P.NombreProceso as Proceso,AP.IDOrden,AP.Cantidad,CU.Nombre as Unidad,AP.Indicaciones,AP.CostoPlaneado,AP.CostoReal,Ap.IDHE as IDHE from ArticuloProduccion as AP inner join Articulo as A on A.IDArticulo=AP.IDArticulo inner join TipoArticulo as TA on A.IDTipoArticulo=TA.IDTipoArticulo inner join Caracteristica as C on C.ID=AP.IDCaracteristica inner join Proceso as P on P.IDProceso=AP.IDProceso inner join c_ClaveUnidad as CU on CU.IDClaveUnidad=AP.IDClaveUnidad where AP.IDOrden=" + id + " and AP.IDProceso=" + prensa.IDProceso + " and TA.Descripcion='Maquina'").ToList();
                if (articulosproduccion.Count > 0)
                {
                    float[] anchoColumnasTablaDatpe = { 100f, 100f, 100f, 50f, 50f, 50f, 50f, 100f };
                    PdfPTable tablaDatosPre = new PdfPTable(anchoColumnasTablaDatpe);
                    tablaDatosPre.DefaultCell.Border = Rectangle.NO_BORDER;
                    tablaDatosPre.SetTotalWidth(anchoColumnasTablaDatpe);
                    tablaDatosPre.SpacingBefore = 5;
                    tablaDatosPre.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaDatosPre.LockedWidth = true;
                    PdfPCell celdamaquinapre = new PdfPCell(new Phrase("Prensa Maquina: ", new Font(Font.FontFamily.HELVETICA, 8)));
                    celdamaquinapre.Border = 1;
                    celdamaquinapre.HorizontalAlignment = Element.ALIGN_LEFT;
                    celdamaquinapre.BackgroundColor = CMYKColor.GRAY;
                    tablaDatosPre.AddCell(celdamaquinapre);
                    PdfPCell celdamaquinapre2 = new PdfPCell(new Phrase(articulosproduccion.FirstOrDefault().Articulo, new Font(Font.FontFamily.HELVETICA, 8)));
                    celdamaquinapre2.Border = 1;
                    celdamaquinapre2.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaDatosPre.AddCell(celdamaquinapre2);

                    decimal tiempo = articulosproduccion.FirstOrDefault().Cantidad;
                    int Horas = int.Parse(Math.Truncate(tiempo).ToString());
                    decimal Minutos = Math.Round((tiempo - Horas) * 60M, 0);



                    //    int IDhE = articulosproduccion.FirstOrDefault().IDHE;

                    try
                    {

                        Vueltas = ((especificacion.Cantidadxrollo / especificacion.productosalpaso) * (especificacion.largoproductomm + especificacion.gapavance)) / 254;
                        Progrenmetros = ((especificacion.Cantidadxrollo / especificacion.productosalpaso) * (especificacion.largoproductomm + especificacion.gapavance)) / 1000;
                        Rollos = ((orden.Cantidad * 1000) / especificacion.Cantidadxrollo);


                        PdfPCell PROGMAQ = new PdfPCell(new Phrase("Prog en Maq: ", new Font(Font.FontFamily.HELVETICA, 8)));
                        PROGMAQ.Border = 1;
                        PROGMAQ.HorizontalAlignment = Element.ALIGN_LEFT;
                        PROGMAQ.BackgroundColor = CMYKColor.GRAY;
                        tablaDatosPre.AddCell(PROGMAQ);
                        PdfPCell prog2 = new PdfPCell(new Phrase(Math.Round(Vueltas, 0).ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                        prog2.Border = 1;
                        prog2.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaDatosPre.AddCell(prog2);

                        PdfPCell PROGMAQ2 = new PdfPCell(new Phrase("Prog en Mts: ", new Font(Font.FontFamily.HELVETICA, 8)));
                        PROGMAQ2.Border = 1;
                        PROGMAQ2.HorizontalAlignment = Element.ALIGN_LEFT;
                        PROGMAQ2.BackgroundColor = CMYKColor.GRAY;
                        tablaDatosPre.AddCell(PROGMAQ2);
                        PdfPCell prog3 = new PdfPCell(new Phrase(Math.Round(Progrenmetros, 0).ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                        prog3.Border = 1;
                        prog3.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaDatosPre.AddCell(prog3);

                        PdfPCell PROGMAQ3 = new PdfPCell(new Phrase("Tiempo  ", new Font(Font.FontFamily.HELVETICA, 8)));
                        PROGMAQ3.Border = 1;
                        PROGMAQ3.HorizontalAlignment = Element.ALIGN_LEFT;
                        PROGMAQ3.BackgroundColor = CMYKColor.GRAY;
                        tablaDatosPre.AddCell(PROGMAQ3);
                        PdfPCell prog4 = new PdfPCell(new Phrase(Horas + " Horas " + Minutos + " Minutos".ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                        prog4.Border = 1;
                        prog4.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaDatosPre.AddCell(prog4);
                        tablaDatosPre.CompleteRow();

                        _documento.Add(tablaDatosPre);
                    }
                    catch (Exception er)

                    {
                        tablaDatosPre.CompleteRow();

                        _documento.Add(tablaDatosPre);
                    }
                }

                /////creando la tabla de prensa


                float columna1 = 300;
                float columna2 = 240;
                float columna3 = 200;

                float[] anchoColumnas = { columna1, columna2, columna3 };
                PdfPTable tabla = new PdfPTable(anchoColumnas);
                tabla.DefaultCell.Border = Rectangle.NO_BORDER;
                tabla.SetTotalWidth(anchoColumnas);
                tabla.HorizontalAlignment = Element.ALIGN_CENTER;
                tabla.LockedWidth = true;


                PdfPTable tabla1 = new PdfPTable(anchoColumnas);
                tabla1.DefaultCell.Border = Rectangle.NO_BORDER;
                tabla1.SetTotalWidth(anchoColumnas);
                tabla1.HorizontalAlignment = Element.ALIGN_CENTER;
                tabla1.LockedWidth = true;





                PdfPCell celdatitherr = new PdfPCell(new Phrase("Herramientas: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, new BaseColor(System.Drawing.Color.Brown))));

                tabla.AddCell(celdatitherr);



                PdfPCell celdatitins = new PdfPCell(new Phrase("Insumos: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, new BaseColor(System.Drawing.Color.Brown))));

                tabla.AddCell(celdatitins);


                PdfPCell celdatitintas = new PdfPCell(new Phrase("Tintas: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, new BaseColor(System.Drawing.Color.Brown))));

                tabla.AddCell(celdatitintas);


                if (prensa.UsaHerramientas)    //////suajes y plecas
                {
                   // List<VArticulosProduccion> articulosproduccionsuajes = new ArticulosProduccionContext().Database.SqlQuery<VArticulosProduccion>("select AP.Existe,Ap.Cantidad as Cantidad, A.Cref as Cref,AP.IDArtProd,A.Descripcion as Articulo,TA.Descripcion as TipoArticulo,C.Presentacion as Caracteristica,P.NombreProceso as Proceso,AP.IDOrden,AP.Cantidad,CU.Nombre as Unidad,AP.Indicaciones,AP.CostoPlaneado,AP.CostoReal from ArticuloProduccion as AP inner join Articulo as A on A.IDArticulo=AP.IDArticulo inner join TipoArticulo as TA on A.IDTipoArticulo=TA.IDTipoArticulo inner join Caracteristica as C on C.ID=AP.IDCaracteristica inner join Proceso as P on P.IDProceso=AP.IDProceso inner join c_ClaveUnidad as CU on CU.IDClaveUnidad=AP.IDClaveUnidad where AP.IDOrden=" + id + " and AP.IDProceso=5 and AP.IDTipoArticulo=2").ToList();
                 
                        PdfPCell celda01 = new PdfPCell(new Phrase("Sin suaje ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, new BaseColor(System.Drawing.Color.Brown))));

                        tabla1.AddCell(celda01);

                }


                if (prensa.UsaInsumos)    ////////// Cintas
                {
                    var listaasignado = new InventarioAlmacenContext().Database.SqlQuery<MaterialAsignado>("select * from materialasignado where orden=" + orden.IDOrden).ToList();
                    string cadenaasi = string.Empty;
                    foreach (MaterialAsignado mat in listaasignado)
                    {
                        FormulaSiaapi.Formulas fs = new FormulaSiaapi.Formulas();
                        //Caracteristica caracteristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + mat.idcaracteristica).ToList().FirstOrDefault();
                        double ancho = double.Parse(mat.ancho.ToString());
                        double largocinta = double.Parse(mat.largo.ToString());
                        //ancho = ancho / 1000;

                        decimal ml = mat.cantidad;
                        int metroslineales = (int)(Math.Round(ml, 0));

                        double cintas = Math.Round(double.Parse(metroslineales.ToString()) / largocinta, 2);
                        Articulo cintax = new ArticuloContext().Articulo.Find(mat.idmapri);
                        cadenaasi += cintax.Cref + "  -  " + mat.ancho + "  -  " + mat.cantidad + " Metros," + cintas + " Cintas \n";
                    }

                    if (cadenaasi == string.Empty)
                    {
                        cadenaasi = "No tiene material asignado";

                        PdfPCell celda02 = new PdfPCell(new Phrase(cadenaasi, new Font(Font.FontFamily.HELVETICA, 10)));
                        tabla1.AddCell(celda02);

                    }
                    else
                    {
                        PdfPCell celda02 = new PdfPCell(new Phrase(cadenaasi, new Font(Font.FontFamily.HELVETICA, 10)));
                        tabla1.AddCell(celda02);
                    }




                }  // fin de insumos 


                List<VArticulosProduccion> articulosproducciontintas = new ArticulosProduccionContext().Database.SqlQuery<VArticulosProduccion>("select AP.Existe,Ap.Cantidad as Cantidad, A.Cref as Cref,AP.IDArtProd,A.Descripcion as Articulo,TA.Descripcion as TipoArticulo,C.Presentacion as Caracteristica,P.NombreProceso as Proceso,AP.IDOrden,AP.Cantidad,CU.Nombre as Unidad,AP.Indicaciones,AP.CostoPlaneado,AP.CostoReal from ArticuloProduccion as AP inner join Articulo as A on A.IDArticulo=AP.IDArticulo inner join TipoArticulo as TA on A.IDTipoArticulo=TA.IDTipoArticulo inner join Caracteristica as C on C.ID=AP.IDCaracteristica inner join Proceso as P on P.IDProceso=AP.IDProceso inner join c_ClaveUnidad as CU on CU.IDClaveUnidad=AP.IDClaveUnidad where AP.IDOrden=" + id + " and AP.IDProceso=5 and AP.IDTipoArticulo=7").ToList();

                string cadena = string.Empty;
                foreach (VArticulosProduccion tinta in articulosproducciontintas)
                {

                    cadena += tinta.Articulo + "\n";

                }

                PdfPCell celda0 = new PdfPCell(new Phrase(cadena, new Font(Font.FontFamily.HELVETICA, 9)));
                tabla1.AddCell(celda0);






                tabla.CompleteRow();
                tabla1.CompleteRow();
                _documento.Add(tabla);
                _documento.Add(tabla1);
                ///////////////////////////////////////////////// embobinado ////////////////////

                try
                {




                    float columna1e = 80;
                    float columna2e = 320;
                    float columna3e = 340;

                    float[] anchoColumnase = { columna1e, columna2e, columna3e };
                    PdfPTable tablae = new PdfPTable(anchoColumnase);
                    tablae.DefaultCell.Border = Rectangle.NO_BORDER;
                    tablae.SetTotalWidth(anchoColumnase);
                    tablae.HorizontalAlignment = Element.ALIGN_CENTER;
                    tablae.LockedWidth = true;


                    PdfPTable tabla1e = new PdfPTable(anchoColumnase);
                    tabla1e.DefaultCell.Border = Rectangle.NO_BORDER;
                    tabla1e.SetTotalWidth(anchoColumnase);
                    tabla1e.HorizontalAlignment = Element.ALIGN_CENTER;
                    tabla1e.LockedWidth = true;





                    PdfPCell celdatitimage = new PdfPCell(new Phrase("Imagen: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, new BaseColor(System.Drawing.Color.Brown))));

                    tablae.AddCell(celdatitimage);


                    PdfPCell celdatitinse = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, new BaseColor(System.Drawing.Color.Brown))));

                    tablae.AddCell(celdatitinse);


                    PdfPCell celdatitinsee = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, new BaseColor(System.Drawing.Color.Brown))));

                    tablae.AddCell(celdatitinsee);




                    Image jpgembo = Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/Embobinado/" + Embobinado + ".jpg"));
                    jpgembo.ScaleToFit(55, 55);
                    PdfPCell celdaima = new PdfPCell(jpgembo);
                    tabla1e.AddCell(celdaima);







                    PdfPCell celda00 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, new BaseColor(System.Drawing.Color.Brown))));
                    tabla1e.AddCell(celda00);



                    PdfPCell celda000 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, new BaseColor(System.Drawing.Color.Brown))));
                    tabla1e.AddCell(celda000);

                    tablae.CompleteRow();
                    tabla1e.CompleteRow();
                    _documento.Add(tablae);
                    _documento.Add(tabla1e);


                }
                catch (Exception err)

                {
                    string mensajedeerror = err.Message;
                }
            }
            catch (Exception errr)
            {


            }
        }
        public PdfPTable agregaarticulossuaje(List<VArticulosProduccion> articulosproduccion, string proceso, float anchocolumna)
        {
            float[] anchoColumnasTablaProductos = { anchocolumna };
            PdfPTable tablaProductosPrincipal = new PdfPTable(anchoColumnasTablaProductos);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosPrincipal.SetTotalWidth(anchoColumnasTablaProductos);
            tablaProductosPrincipal.SpacingBefore = 10;
            tablaProductosPrincipal.SpacingAfter = 5;
            tablaProductosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosPrincipal.LockedWidth = true;

            foreach (VArticulosProduccion p in articulosproduccion)
            {
                //if (articulosproduccion.Count.Equals(2))
                //{

                //}

                int IDhE = p.IDHE;

                //   EspecificacionEtiquetas especificacion = new EspecificacionEtiquetasContext().EspecificacionEtiqueta.Where(s => s.IDHE == IDhE).ToList().FirstOrDefault();

                //Datos de los productos
                float[] tamanoColumnas = { anchocolumna };
                PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);

                //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
                tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductosTitulos.LockedWidth = true;

                ArticuloContext db = new ArticuloContext();


                string Cref = p.Cref;


                string tipo = "select idtipoarticulo as Dato from Articulo where Cref='" + p.Cref + "'";
                ClsDatoEntero idtipo = db.Database.SqlQuery<ClsDatoEntero>(tipo).ToList().FirstOrDefault();


                PdfPCell celda0 = new PdfPCell(new Phrase("Articulo", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
                celda0.BackgroundColor = colordefinido;

                PdfPCell celda01 = new PdfPCell(new Phrase(p.Articulo, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));



                celda01 = new PdfPCell(new Phrase(Cref + " " + p.Articulo, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

                tablaProductosTitulos.AddCell(celda01);





                PdfPCell celdaTitulos = new PdfPCell(tablaProductosTitulos);
                tablaProductosPrincipal.AddCell(celdaTitulos);



                //_documento.Add(tablaProductosPrincipal);

            }

            return tablaProductosPrincipal;


        }
        public PdfPTable agregaarticulos(List<VArticulosProduccion> articulosproduccion, string proceso, float anchocolumna)
        {
            float[] anchoColumnasTablaProductos = { anchocolumna };
            PdfPTable tablaProductosPrincipal = new PdfPTable(anchoColumnasTablaProductos);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosPrincipal.SetTotalWidth(anchoColumnasTablaProductos);
            tablaProductosPrincipal.SpacingBefore = 10;
            tablaProductosPrincipal.SpacingAfter = 5;
            tablaProductosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosPrincipal.LockedWidth = true;

            foreach (VArticulosProduccion p in articulosproduccion)
            {
                //if (articulosproduccion.Count.Equals(2))
                //{

                //}

                int IDhE = p.IDHE;

             //   EspecificacionEtiquetas especificacion = new EspecificacionEtiquetasContext().EspecificacionEtiqueta.Where(s => s.IDHE == IDhE).ToList().FirstOrDefault();

                //Datos de los productos
                float[] tamanoColumnas = { anchocolumna };
                PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);

                //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
                tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductosTitulos.LockedWidth = true;

                ArticuloContext db = new ArticuloContext();

               
                string Cref = p.Cref;
               

                string tipo =   "select idtipoarticulo as Dato from Articulo where Cref='" + p.Cref + "'";
                ClsDatoEntero idtipo = db.Database.SqlQuery<ClsDatoEntero>(tipo).ToList().FirstOrDefault();


                PdfPCell celda0 = new PdfPCell(new Phrase("Artículo", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
                celda0.BackgroundColor = colordefinido;

                PdfPCell celda01 = new PdfPCell(new Phrase(p.Articulo, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

                if (idtipo.Dato != 2) // No es Herramienta
                {
                    if (idtipo.Dato != 7)  //No es Titna
                    {
                        celda01 = new PdfPCell(new Phrase(Cref, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                    }
                    else // es tinta
                    {
                        celda01 = new PdfPCell(new Phrase("Tinta " + p.Articulo, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                    }
                }

                else //es herramienta
                {
                    celda01 = new PdfPCell(new Phrase(Cref + " " + p.Articulo, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                }





                PdfPCell celda2 = new PdfPCell(new Phrase("Presentacion", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
                PdfPCell celda21 = new PdfPCell(new Phrase(p.Caracteristica, new Font(Font.FontFamily.HELVETICA, 4, Font.BOLD)));
                celda2.BackgroundColor = colordefinido;

                PdfPCell celda4 = new PdfPCell(new Phrase("Cantidad", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
                PdfPCell celda41;
                if (idtipo.Dato == 6) // es cinta 
                {
                  //  p.Cantidad = p.Cantidad;
                    FormulaSiaapi.Formulas fs = new FormulaSiaapi.Formulas();
                    double ancho = fs.getvalor("ANCHO", p.Caracteristica);
                    double largocinta = fs.getvalor("LARGO", p.Caracteristica);
                    //ancho = ancho / 1000;
                   
                    decimal ml= ((orden.Cantidad * 1000M * (decimal.Parse(especificacion.largoproductomm.ToString()) + decimal.Parse(especificacion.gapavance.ToString()))) / 1000M)/ decimal.Parse(especificacion.cavidades.ToString())  * 1.03M;
                    int metroslineales = (int)(Math.Round(ml,0));

                    double cintas = double.Parse(metroslineales.ToString()) / largocinta;
                   
                   

                   // celda41 = new PdfPCell(new Phrase(p.Cantidad.ToString() + " " + p.Unidad +"\n"+ metroslineales +" METROS LINEALES __ TOTAL DE CINTAS:" +Math.Round(cintas,2), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                    celda41 = new PdfPCell(new Phrase( metroslineales + " METROS LINEALES \n TOTAL DE CINTAS:" + Math.Round(cintas, 2), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                }
                else
                {
                    celda41 = new PdfPCell(new Phrase(p.Cantidad.ToString() + " " + p.Unidad, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                }
                celda4.BackgroundColor = colordefinido;





                if (idtipo.Dato == 3) // Maquina
                {
                    tablaProductosTitulos.AddCell(celda0);
                    tablaProductosTitulos.AddCell(celda01);

                    //     tablaProductosTitulos.AddCell(celda4);
                    tablaProductosTitulos.AddCell(celda41);

                 
                }

                    if (idtipo.Dato == 2) // Herramienta
                    {
                        tablaProductosTitulos.AddCell(celda0);
                        tablaProductosTitulos.AddCell(celda01);
                      //  tablaProductosTitulos.AddCell(celda2);
                        tablaProductosTitulos.AddCell(celda21);


                    }
                    if (proceso == "Embobinado")
                    {
                        if (idtipo.Dato == 4 ) ///insumo
                        {


                            tablaProductosTitulos.AddCell(celda0);
                            tablaProductosTitulos.AddCell(celda01);
                            //tablaProductosTitulos.AddCell(celda4);
                            tablaProductosTitulos.AddCell(celda41);



                        }
                    }
                    if ( proceso == "Prensa") // ES PRENSA
                    {
                       
                                if (idtipo.Dato == 4)   /// Insumo Tinta
                                {
                        
                                    tablaProductosTitulos.AddCell(celda01);
                                    tablaProductosTitulos.AddCell(celda21);


                                }
                                if (idtipo.Dato == 7) //tinta
                                {

                                    tablaProductosTitulos.AddCell(celda01);
                                }
                                if (idtipo.Dato == 6)   /// Insumo Cinta
                                {
                                    //tablaProductosTitulos.AddCell(celda0);
                                    tablaProductosTitulos.AddCell(celda01);
                                    tablaProductosTitulos.AddCell(celda21); // presentacion
                                    tablaProductosTitulos.AddCell(celda41);



                                }
                }



                    PdfPCell celdaTitulos = new PdfPCell(tablaProductosTitulos);
                    tablaProductosPrincipal.AddCell(celdaTitulos);



                    //_documento.Add(tablaProductosPrincipal);

                }
          
            return tablaProductosPrincipal;

            
        }






        public void agregaDATOS()
        {
            float[] anchoColumnasTablaProductos = { 260f, 260f, 260f };
            
            //int cuantos = TEXTO.Length;


            //float[] tamanoenc = new float[cuantos];
            //for (int i = 0; i < TEXTO.Length; i++)
            //{
            //    tamanoenc[i] = 390f;

            //}



            //Datos de los productos
          
                PdfPTable tablaProductosTitulos = new PdfPTable(anchoColumnasTablaProductos);

                //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaProductosTitulos.SetTotalWidth(anchoColumnasTablaProductos);
                tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductosTitulos.LockedWidth = true;
            tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell celda0 = new PdfPCell(new Phrase("Prensista:_____________________________________________\nInicio: Fecha:____/____/____  Hora:____:____\nMetros producidos: _________________________\nMerma generada:_____________\nFinalizado: Fecha:____/____ Hora:____:____", new Font(Font.FontFamily.HELVETICA, 7)));
           

            celda0.Border = Rectangle.NO_BORDER;
           


            PdfPCell celda5 = new PdfPCell(new Phrase("Embobinador(a):____________________________________________\nInicio: Fecha:____/____/____  Hora:____:____\nRollos:_______________ Sobrante:____________\nTotal Mill:________________________\nFinalizado: Fecha:____/____ Hora:____:____", new Font(Font.FontFamily.HELVETICA, 7)));
           
            PdfPCell celda10 = new PdfPCell(new Phrase("Embobinado:___________________\nPeso:__________\nCaja: IMP.______ LISA______\nTipo de centro: IMP.______ Liso______\nPleca:_________\nMedidas:____________\nSustrato:____________", new Font(Font.FontFamily.HELVETICA, 7)));


            
            celda5.Border = Rectangle.NO_BORDER;
            
            celda10.Border = Rectangle.NO_BORDER;

            tablaProductosTitulos.AddCell(celda0);
               
                tablaProductosTitulos.AddCell(celda5);
            tablaProductosTitulos.AddCell(celda10);
            
            _documento.Add(tablaProductosTitulos);



        }

        public void agregaDATOS2()
        {
            float[] anchoColumnasTablaProductos = { 520f, 260f };


            PdfPTable tablaProductosTitulos = new PdfPTable(anchoColumnasTablaProductos);

            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosTitulos.SetTotalWidth(anchoColumnasTablaProductos);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;
            tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell celda0 = new PdfPCell(new Phrase("Observaciones:", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
           
            PdfPCell celda5 = new PdfPCell(new Phrase("Millares producidos:____________________\nRollos:_______________\nSobrante:_______________\nLote:________________\nFecha:____/____/____", new Font(Font.FontFamily.HELVETICA, 7)));
                       
            celda5.Border = Rectangle.NO_BORDER;
            

            tablaProductosTitulos.AddCell(celda0);
           
            tablaProductosTitulos.AddCell(celda5);
            



            _documento.Add(tablaProductosTitulos);



        }

        public void Firmas()
        {
            float[] anchoColumnasTablaProductos = { 195f,195f,195f,195f };


            PdfPTable tablaProductosTitulos = new PdfPTable(anchoColumnasTablaProductos);

            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosTitulos.SetTotalWidth(anchoColumnasTablaProductos);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;
            tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell celda0 = new PdfPCell(new Phrase("\nPRENSA\n", new Font(Font.FontFamily.HELVETICA, 7)));
            PdfPCell celda5 = new PdfPCell(new Phrase("\nEMBOBINADO\n", new Font(Font.FontFamily.HELVETICA, 7)));
            PdfPCell celda10 = new PdfPCell(new Phrase("\nCALIDAD\n", new Font(Font.FontFamily.HELVETICA, 7)));
            PdfPCell celda101 = new PdfPCell(new Phrase("\nALMACEN\n", new Font(Font.FontFamily.HELVETICA, 7)));

            celda0.Border = Rectangle.NO_BORDER;
            celda0.HorizontalAlignment = Element.ALIGN_CENTER;
            celda5.Border = Rectangle.NO_BORDER;
            celda5.HorizontalAlignment = Element.ALIGN_CENTER;
            celda10.Border = Rectangle.NO_BORDER;
            celda10.HorizontalAlignment = Element.ALIGN_CENTER;
            celda101.Border = Rectangle.NO_BORDER;
            celda101.HorizontalAlignment = Element.ALIGN_CENTER;


            tablaProductosTitulos.AddCell(celda0);
            tablaProductosTitulos.AddCell(celda5);
            tablaProductosTitulos.AddCell(celda10);
            tablaProductosTitulos.AddCell(celda101);

            _documento.Add(tablaProductosTitulos);


            PdfPTable tablaProductosF = new PdfPTable(anchoColumnasTablaProductos);

            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosF.SetTotalWidth(anchoColumnasTablaProductos);
            tablaProductosF.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosF.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosF.LockedWidth = true;
            tablaProductosF.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell C1 = new PdfPCell(new Phrase("_____________________________", new Font(Font.FontFamily.HELVETICA, 7)));
            PdfPCell C2 = new PdfPCell(new Phrase("_____________________________", new Font(Font.FontFamily.HELVETICA, 7)));
            PdfPCell C3 = new PdfPCell(new Phrase("_____________________________", new Font(Font.FontFamily.HELVETICA, 7)));
            PdfPCell C4 = new PdfPCell(new Phrase("_____________________________", new Font(Font.FontFamily.HELVETICA, 7)));

            C1.Border = Rectangle.NO_BORDER;
            C1.HorizontalAlignment = Element.ALIGN_CENTER;
            C2.Border = Rectangle.NO_BORDER;
            C2.HorizontalAlignment = Element.ALIGN_CENTER;
            C3.Border = Rectangle.NO_BORDER;
            C3.HorizontalAlignment = Element.ALIGN_CENTER;
            C4.Border = Rectangle.NO_BORDER;
            C4.HorizontalAlignment = Element.ALIGN_CENTER;


            tablaProductosF.AddCell(C1);
            tablaProductosF.AddCell(C2);
            tablaProductosF.AddCell(C3);
            tablaProductosF.AddCell(C4);

            _documento.Add(tablaProductosF);




        }
        public void DatosLiberacion()
        {
            //Datos de los productos
                float[] tamanoColumnas = { 780f };
            PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);
            tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;


            PdfPCell celda00 = new PdfPCell(new Phrase("Liberación de producto terminado", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));

            celda00.BackgroundColor = colordefinido;

            //celda00.BackgroundColor = new CMYKColor(0, 29, 50, 70);

            tablaProductosTitulos.AddCell(celda00);
            _documento.Add(tablaProductosTitulos);

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


           
            ////Datos del receptor
            //float[] anchoColumnas = { 50f, 150F, 50f, 150F, 70f, 150f };
            //PdfPTable tabla = new PdfPTable(anchoColumnas);
            //tabla.DefaultCell.Border = Rectangle.NO_BORDER;
            //tabla.SetTotalWidth(anchoColumnas);
            //tabla.HorizontalAlignment = Element.ALIGN_LEFT;
            //tabla.LockedWidth = true;



            //PdfPCell celda0 = new PdfPCell(new Phrase("Millares P: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda1 = new PdfPCell(new Phrase("________________________________", new Font(Font.FontFamily.HELVETICA, 8)));
            //PdfPCell celda2 = new PdfPCell(new Phrase("Rollos: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda3 = new PdfPCell(new Phrase("________________________________", new Font(Font.FontFamily.HELVETICA, 8)));


            //celda0.Border = Rectangle.NO_BORDER;
            //celda1.Border = Rectangle.NO_BORDER;
            //celda2.Border = Rectangle.NO_BORDER;
            //celda3.Border = Rectangle.NO_BORDER;

            //tabla.AddCell(celda0);
            //tabla.AddCell(celda1);
            //tabla.AddCell(celda2);
            //tabla.AddCell(celda3);

            //PdfPCell celda4 = new PdfPCell(new Phrase("Sobrante:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda5 = new PdfPCell(new Phrase("________________________________", new Font(Font.FontFamily.HELVETICA, 8)));
            //PdfPCell celda6 = new PdfPCell(new Phrase("Lote: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda7 = new PdfPCell(new Phrase("________________________________", new Font(Font.FontFamily.HELVETICA, 8)));


            //celda4.Border = Rectangle.NO_BORDER;
            //celda5.Border = Rectangle.NO_BORDER;
            //celda6.Border = Rectangle.NO_BORDER;
            //celda7.Border = Rectangle.NO_BORDER;

            //tabla.AddCell(celda4);
            //tabla.AddCell(celda5);
            //tabla.AddCell(celda6);
            //tabla.AddCell(celda7);

            //PdfPCell celda8 = new PdfPCell(new Phrase("Fecha: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda9 = new PdfPCell(new Phrase("________________________________", new Font(Font.FontFamily.HELVETICA, 8)));
            //PdfPCell celda10 = new PdfPCell(new Phrase("Liberó: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda11 = new PdfPCell(new Phrase("________________________________", new Font(Font.FontFamily.HELVETICA, 8)));


            //celda8.Border = Rectangle.NO_BORDER;
            //celda9.Border = Rectangle.NO_BORDER;
            //celda10.Border = Rectangle.NO_BORDER;
            //celda11.Border = Rectangle.NO_BORDER;

            //tabla.AddCell(celda8);
            //tabla.AddCell(celda9);
            //tabla.AddCell(celda10);
            //tabla.AddCell(celda11);

            //_documento.Add(tabla);
        }
        private void AgregarDatosLiberacion(int id)
        {
            DatosLiberacion();
            AgregarDatosCalidad(id);



            //Datos de los productos
            float[] tamanoColumnas = { 780f };
            PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);
            tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;


            //PdfPCell celda00 = new PdfPCell(new Phrase("Liberación de producto terminado", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            //celda00.BackgroundColor = new CMYKColor(0, 29, 50, 70);

            //tablaProductosTitulos.AddCell(celda00);
            //_documento.Add(tablaProductosTitulos);

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            //OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(id);
            //Clientes cliente = new ClientesContext().Clientes.Find(orden.Cliente.IDCliente);
            //Vendedor vendedor = new VendedorContext().Vendedores.Find(cliente.IDVendedor);
            //Articulo articulo = new ArticuloContext().Articulo.Find(orden.IDArticulo);
            //Datos del receptor
            float[] anchoColumnas = { 30f, 70f, 30f, 35f, 15f, 80f, 80f, 80f, 100f, 80f, 80f, 35f, 35f, 30f };
            PdfPTable tabla = new PdfPTable(anchoColumnas);
            tabla.DefaultCell.Border = Rectangle.NO_BORDER;
            tabla.SetTotalWidth(anchoColumnas);
            tabla.HorizontalAlignment = Element.ALIGN_LEFT;
            tabla.LockedWidth = true;



            PdfPCell celda0 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda1 = new PdfPCell(new Phrase("Check List", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda2 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda3 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda4 = new PdfPCell(new Phrase("#", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda5 = new PdfPCell(new Phrase("Millares", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda6 = new PdfPCell(new Phrase("Rollos", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda7 = new PdfPCell(new Phrase("Sobrante", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda8 = new PdfPCell(new Phrase("Lote", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda9 = new PdfPCell(new Phrase("Fecha", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda10 = new PdfPCell(new Phrase("Liberó", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda11 = new PdfPCell(new Phrase("Parcial", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda12 = new PdfPCell(new Phrase("Final", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda13 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));


            //celda0.BackgroundColor = colorencabezadodefinido;
            celda1.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            celda2.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            //celda3.BackgroundColor= new CMYKColor(0, 29, 50, 70);;
            celda4.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            celda5.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            celda6.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            celda7.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            celda8.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            celda9.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            celda10.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            celda11.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            celda12.BackgroundColor = new CMYKColor(0, 29, 50, 70);

            celda0.Border = Rectangle.NO_BORDER;

            celda3.Border = Rectangle.NO_BORDER;
            celda13.Border = Rectangle.NO_BORDER;

            tabla.AddCell(celda0);
            tabla.AddCell(celda1);
            tabla.AddCell(celda2);
            tabla.AddCell(celda3);
            tabla.AddCell(celda4);
            tabla.AddCell(celda5);
            tabla.AddCell(celda6);
            tabla.AddCell(celda7);
            tabla.AddCell(celda8);
            tabla.AddCell(celda9);
            tabla.AddCell(celda10);
            tabla.AddCell(celda11);
            tabla.AddCell(celda12);
            tabla.AddCell(celda13);

            PdfPCell celda00 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda01 = new PdfPCell(new Phrase("Embobinado", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda02 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda03 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda04 = new PdfPCell(new Phrase("1", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda05 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda06 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda07 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda08 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda09 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda010 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda011 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda012 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda013 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

            celda04.BackgroundColor = new CMYKColor(0, 29, 50, 70);



            celda00.Border = Rectangle.NO_BORDER;

            celda03.Border = Rectangle.NO_BORDER;
            celda013.Border = Rectangle.NO_BORDER;

            tabla.AddCell(celda00);
            tabla.AddCell(celda01);
            tabla.AddCell(celda02);
            tabla.AddCell(celda03);
            tabla.AddCell(celda04);
            tabla.AddCell(celda05);
            tabla.AddCell(celda06);
            tabla.AddCell(celda07);
            tabla.AddCell(celda08);
            tabla.AddCell(celda09);
            tabla.AddCell(celda010);
            tabla.AddCell(celda011);
            tabla.AddCell(celda012);
            tabla.AddCell(celda013);

            PdfPCell celda000 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda001 = new PdfPCell(new Phrase("Limpieza", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda002 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda003 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda004 = new PdfPCell(new Phrase("2", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda005 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda006 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda007 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda008 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda009 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0010 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0011 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0012 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0013 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));


            celda004.BackgroundColor = new CMYKColor(0, 29, 50, 70);


            celda000.Border = Rectangle.NO_BORDER;

            celda003.Border = Rectangle.NO_BORDER;
            celda0013.Border = Rectangle.NO_BORDER;

            tabla.AddCell(celda000);
            tabla.AddCell(celda001);
            tabla.AddCell(celda002);
            tabla.AddCell(celda003);
            tabla.AddCell(celda004);
            tabla.AddCell(celda005);
            tabla.AddCell(celda006);
            tabla.AddCell(celda007);
            tabla.AddCell(celda008);
            tabla.AddCell(celda009);
            tabla.AddCell(celda0010);
            tabla.AddCell(celda0011);
            tabla.AddCell(celda0012);
            tabla.AddCell(celda0013);

            PdfPCell celda0000 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda0001 = new PdfPCell(new Phrase("Telescopeo", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0002 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda0003 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda0004 = new PdfPCell(new Phrase("3", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0005 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0006 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0007 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0008 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0009 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00010 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00011 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00012 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00013 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

            celda0004.BackgroundColor = new CMYKColor(0, 29, 50, 70);



            celda0000.Border = Rectangle.NO_BORDER;
            celda0003.Border = Rectangle.NO_BORDER;
            celda00013.Border = Rectangle.NO_BORDER;

            tabla.AddCell(celda0000);
            tabla.AddCell(celda0001);
            tabla.AddCell(celda0002);
            tabla.AddCell(celda0003);
            tabla.AddCell(celda0004);
            tabla.AddCell(celda0005);
            tabla.AddCell(celda0006);
            tabla.AddCell(celda0007);
            tabla.AddCell(celda0008);
            tabla.AddCell(celda0009);
            tabla.AddCell(celda00010);
            tabla.AddCell(celda00011);
            tabla.AddCell(celda00012);
            tabla.AddCell(celda00013);

            PdfPCell celda00000 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda00001 = new PdfPCell(new Phrase("Tensión", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00002 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda00003 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda00004 = new PdfPCell(new Phrase("4", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00005 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00006 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00007 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00008 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00009 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000010 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000011 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000012 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000013 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));


            celda00004.BackgroundColor = new CMYKColor(0, 29, 50, 70);


            celda00000.Border = Rectangle.NO_BORDER;
            celda00003.Border = Rectangle.NO_BORDER;
            celda000013.Border = Rectangle.NO_BORDER;

            tabla.AddCell(celda00000);
            tabla.AddCell(celda00001);
            tabla.AddCell(celda00002);
            tabla.AddCell(celda00003);
            tabla.AddCell(celda00004);
            tabla.AddCell(celda00005);
            tabla.AddCell(celda00006);
            tabla.AddCell(celda00007);
            tabla.AddCell(celda00008);
            tabla.AddCell(celda00009);
            tabla.AddCell(celda000010);
            tabla.AddCell(celda000011);
            tabla.AddCell(celda000012);
            tabla.AddCell(celda000013);

            PdfPCell celda000000 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda000001 = new PdfPCell(new Phrase("Diametro", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000002 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda000003 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda000004 = new PdfPCell(new Phrase("5", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000005 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000006 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000007 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000008 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000009 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0000010 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0000011 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0000012 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0000013 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

            celda000004.BackgroundColor = new CMYKColor(0, 29, 50, 70);


            celda000000.Border = Rectangle.NO_BORDER;
            celda000003.Border = Rectangle.NO_BORDER;
            celda0000013.Border = Rectangle.NO_BORDER;

            tabla.AddCell(celda000000);
            tabla.AddCell(celda000001);
            tabla.AddCell(celda000002);
            tabla.AddCell(celda000003);
            tabla.AddCell(celda000004);
            tabla.AddCell(celda000005);
            tabla.AddCell(celda000006);
            tabla.AddCell(celda000007);
            tabla.AddCell(celda000008);
            tabla.AddCell(celda000009);
            tabla.AddCell(celda0000010);
            tabla.AddCell(celda0000011);
            tabla.AddCell(celda0000012);
            tabla.AddCell(celda0000013);

            PdfPCell celda0000000 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda0000001 = new PdfPCell(new Phrase("Centro('')", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0000002 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda0000003 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda0000004 = new PdfPCell(new Phrase("6", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0000005 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0000006 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0000007 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0000008 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0000009 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00000010 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00000011 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00000012 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00000013 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));


            celda0000004.BackgroundColor = new CMYKColor(0, 29, 50, 70);

            celda0000000.Border = Rectangle.NO_BORDER;
            celda0000003.Border = Rectangle.NO_BORDER;
            celda00000013.Border = Rectangle.NO_BORDER;

            tabla.AddCell(celda0000000);
            tabla.AddCell(celda0000001);
            tabla.AddCell(celda0000002);
            tabla.AddCell(celda0000003);
            tabla.AddCell(celda0000004);
            tabla.AddCell(celda0000005);
            tabla.AddCell(celda0000006);
            tabla.AddCell(celda0000007);
            tabla.AddCell(celda0000008);
            tabla.AddCell(celda0000009);
            tabla.AddCell(celda00000010);
            tabla.AddCell(celda00000011);
            tabla.AddCell(celda00000012);
            tabla.AddCell(celda00000013);

            PdfPCell celda00000000 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda00000001 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00000002 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda00000003 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda00000004 = new PdfPCell(new Phrase("7", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00000005 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00000006 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00000007 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00000008 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda00000009 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000000010 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000000011 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000000012 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000000013 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

            celda00000004.BackgroundColor = new CMYKColor(0, 29, 50, 70);


            celda00000000.Border = Rectangle.NO_BORDER;
            celda00000001.Border = Rectangle.NO_BORDER;
            celda00000002.Border = Rectangle.NO_BORDER;
            celda00000003.Border = Rectangle.NO_BORDER;
            celda000000013.Border = Rectangle.NO_BORDER;

            tabla.AddCell(celda00000000);
            tabla.AddCell(celda00000001);
            tabla.AddCell(celda00000002);
            tabla.AddCell(celda00000003);
            tabla.AddCell(celda00000004);
            tabla.AddCell(celda00000005);
            tabla.AddCell(celda00000006);
            tabla.AddCell(celda00000007);
            tabla.AddCell(celda00000008);
            tabla.AddCell(celda00000009);
            tabla.AddCell(celda000000010);
            tabla.AddCell(celda000000011);
            tabla.AddCell(celda000000012);
            tabla.AddCell(celda000000013);

            PdfPCell celda000000000 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda000000001 = new PdfPCell(new Phrase("Aprobado", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000000002 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda000000003 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda000000004 = new PdfPCell(new Phrase("8", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000000005 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000000006 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000000007 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000000008 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda000000009 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0000000010 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0000000011 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0000000012 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda0000000013 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));




            celda000000000.Border = Rectangle.NO_BORDER;
            celda000000003.Border = Rectangle.NO_BORDER;
            celda0000000013.Border = Rectangle.NO_BORDER;
            celda000000004.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            tabla.AddCell(celda000000000);
            tabla.AddCell(celda000000001);
            tabla.AddCell(celda000000002);
            tabla.AddCell(celda000000003);
            tabla.AddCell(celda000000004);
            tabla.AddCell(celda000000005);
            tabla.AddCell(celda000000006);
            tabla.AddCell(celda000000007);
            tabla.AddCell(celda000000008);
            tabla.AddCell(celda000000009);
            tabla.AddCell(celda0000000010);
            tabla.AddCell(celda0000000011);
            tabla.AddCell(celda0000000012);
            tabla.AddCell(celda0000000013);

            _documento.Add(tabla);

            //float[] anchoColumnasTablaDatos = { 600f };
            //PdfPTable tablaDatosPrincipal1 = new PdfPTable(anchoColumnasTablaDatos);
            //tablaDatosPrincipal1.DefaultCell.Border = Rectangle.NO_BORDER;
            //tablaDatosPrincipal1.SetTotalWidth(anchoColumnasTablaDatos);
            //tablaDatosPrincipal1.SpacingBefore = 10f;
            //tablaDatosPrincipal1.HorizontalAlignment = Element.ALIGN_LEFT;
            //tablaDatosPrincipal1.LockedWidth = true;
            //PdfPCell celdanada1 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            //celdanada1.Border = Rectangle.NO_BORDER;
            //tablaDatosPrincipal1.AddCell(celdanada1);
            //_documento.Add(tablaDatosPrincipal1);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //float[] tamaño = { 93f, 18f, 94f, 18f, 94f, 18f, 93f, 18f, 93f, 18f, 93f, 18f, 93f, 18f };
            //PdfPTable tablacheck = new PdfPTable(tamaño);
            ////tablacheck.DefaultCell.Border = Rectangle.NO_BORDER;
            //tablacheck.SetTotalWidth(tamaño);
            //tablacheck.HorizontalAlignment = Element.ALIGN_LEFT;
            //tablacheck.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            //tablacheck.LockedWidth = true;

            //PdfPCell celda000 = new PdfPCell(new Phrase("Embobinado", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda000c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            ////PdfPCell celda01 = new PdfPCell(new Phrase("Placa", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            ////PdfPCell celda01c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            ////PdfPCell celda02 = new PdfPCell(new Phrase("Suaje", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            ////PdfPCell celda02c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda03 = new PdfPCell(new Phrase("Centro C/Impresión", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda03c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda04 = new PdfPCell(new Phrase("Centro S/Impresión", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda04c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda05 = new PdfPCell(new Phrase("Telescopiado", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda05c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda06 = new PdfPCell(new Phrase("Piojos", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda06c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda07 = new PdfPCell(new Phrase("Manchas", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda07c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda08 = new PdfPCell(new Phrase("Gap", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //PdfPCell celda08c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));




            //tablacheck.AddCell(celda000);
            //tablacheck.AddCell(celda000c);
            ////tablacheck.AddCell(celda01);
            ////tablacheck.AddCell(celda01c);
            ////tablacheck.AddCell(celda02);
            ////tablacheck.AddCell(celda02c);
            //tablacheck.AddCell(celda03);
            //tablacheck.AddCell(celda03c);
            //tablacheck.AddCell(celda04);
            //tablacheck.AddCell(celda04c);
            //tablacheck.AddCell(celda05);
            //tablacheck.AddCell(celda05c);
            //tablacheck.AddCell(celda06);
            //tablacheck.AddCell(celda06c);
            //tablacheck.AddCell(celda07);
            //tablacheck.AddCell(celda07c);
            //tablacheck.AddCell(celda08);
            //tablacheck.AddCell(celda08c);

            //_documento.Add(tablacheck);
        }


        //private void AgregarDatosLiberacion(int id)
        //{




        //    //Datos de los productos
        //    float[] tamanoColumnas = { 780f };
        //    PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);
        //    tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
        //    tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaProductosTitulos.LockedWidth = true;


        //    PdfPCell celda00 = new PdfPCell(new Phrase("Liberación de producto terminado", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

        //    celda00.BackgroundColor = new CMYKColor(0, 29, 50, 70);

        //    tablaProductosTitulos.AddCell(celda00);
        //    _documento.Add(tablaProductosTitulos);

        //    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //    OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(id);
        //    Clientes cliente = new ClientesContext().Clientes.Find(orden.Cliente.IDCliente);
        //    Vendedor vendedor = new VendedorContext().Vendedores.Find(cliente.IDVendedor);
        //    Articulo articulo = new ArticuloContext().Articulo.Find(orden.IDArticulo);
        //    //Datos del receptor
        //    float[] anchoColumnas = { 50f, 150F, 50f, 150F, 70f, 150f };
        //    PdfPTable tabla = new PdfPTable(anchoColumnas);
        //    tabla.DefaultCell.Border = Rectangle.NO_BORDER;
        //    tabla.SetTotalWidth(anchoColumnas);
        //    tabla.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tabla.LockedWidth = true;



        //    PdfPCell celda0 = new PdfPCell(new Phrase("Millares P: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda1 = new PdfPCell(new Phrase("________________________________", new Font(Font.FontFamily.HELVETICA, 8)));
        //    PdfPCell celda2 = new PdfPCell(new Phrase("Rollos: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda3 = new PdfPCell(new Phrase("________________________________", new Font(Font.FontFamily.HELVETICA, 8)));


        //    celda0.Border = Rectangle.NO_BORDER;
        //    celda1.Border = Rectangle.NO_BORDER;
        //    celda2.Border = Rectangle.NO_BORDER;
        //    celda3.Border = Rectangle.NO_BORDER;

        //    tabla.AddCell(celda0);
        //    tabla.AddCell(celda1);
        //    tabla.AddCell(celda2);
        //    tabla.AddCell(celda3);

        //    PdfPCell celda4 = new PdfPCell(new Phrase("Sobrante:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda5 = new PdfPCell(new Phrase("________________________________", new Font(Font.FontFamily.HELVETICA, 8)));
        //    PdfPCell celda6 = new PdfPCell(new Phrase("Lote: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda7 = new PdfPCell(new Phrase("________________________________", new Font(Font.FontFamily.HELVETICA, 8)));


        //    celda4.Border = Rectangle.NO_BORDER;
        //    celda5.Border = Rectangle.NO_BORDER;
        //    celda6.Border = Rectangle.NO_BORDER;
        //    celda7.Border = Rectangle.NO_BORDER;

        //    tabla.AddCell(celda4);
        //    tabla.AddCell(celda5);
        //    tabla.AddCell(celda6);
        //    tabla.AddCell(celda7);

        //    PdfPCell celda8 = new PdfPCell(new Phrase("Fecha: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda9 = new PdfPCell(new Phrase("________________________________", new Font(Font.FontFamily.HELVETICA, 8)));
        //    PdfPCell celda10 = new PdfPCell(new Phrase("Liberó: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda11 = new PdfPCell(new Phrase("________________________________", new Font(Font.FontFamily.HELVETICA, 8)));


        //    celda8.Border = Rectangle.NO_BORDER;
        //    celda9.Border = Rectangle.NO_BORDER;
        //    celda10.Border = Rectangle.NO_BORDER;
        //    celda11.Border = Rectangle.NO_BORDER;

        //    tabla.AddCell(celda8);
        //    tabla.AddCell(celda9);
        //    tabla.AddCell(celda10);
        //    tabla.AddCell(celda11);

        //    _documento.Add(tabla);

        //    float[] anchoColumnasTablaDatos = { 600f };
        //    PdfPTable tablaDatosPrincipal1 = new PdfPTable(anchoColumnasTablaDatos);
        //    tablaDatosPrincipal1.DefaultCell.Border = Rectangle.NO_BORDER;
        //    tablaDatosPrincipal1.SetTotalWidth(anchoColumnasTablaDatos);
        //    tablaDatosPrincipal1.SpacingBefore = 10f;
        //    tablaDatosPrincipal1.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaDatosPrincipal1.LockedWidth = true;
        //    PdfPCell celdanada1 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
        //    celdanada1.Border = Rectangle.NO_BORDER;
        //    tablaDatosPrincipal1.AddCell(celdanada1);
        //    _documento.Add(tablaDatosPrincipal1);

        //    //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //    float[] tamaño = { 93f, 18f, 94f, 18f, 94f, 18f, 93f, 18f, 93f, 18f, 93f, 18f, 93f, 18f };
        //    PdfPTable tablacheck = new PdfPTable(tamaño);
        //    //tablacheck.DefaultCell.Border = Rectangle.NO_BORDER;
        //    tablacheck.SetTotalWidth(tamaño);
        //    tablacheck.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablacheck.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablacheck.LockedWidth = true;

        //    PdfPCell celda000 = new PdfPCell(new Phrase("Embobinado", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda000c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    //PdfPCell celda01 = new PdfPCell(new Phrase("Placa", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    //PdfPCell celda01c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    //PdfPCell celda02 = new PdfPCell(new Phrase("Suaje", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    //PdfPCell celda02c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda03 = new PdfPCell(new Phrase("Centro C/Impresión", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda03c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda04 = new PdfPCell(new Phrase("Centro S/Impresión", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda04c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda05 = new PdfPCell(new Phrase("Telescopiado", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda05c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda06 = new PdfPCell(new Phrase("Piojos", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda06c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda07 = new PdfPCell(new Phrase("Manchas", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda07c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda08 = new PdfPCell(new Phrase("Gap", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
        //    PdfPCell celda08c = new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));




        //    tablacheck.AddCell(celda000);
        //    tablacheck.AddCell(celda000c);
        //    //tablacheck.AddCell(celda01);
        //    //tablacheck.AddCell(celda01c);
        //    //tablacheck.AddCell(celda02);
        //    //tablacheck.AddCell(celda02c);
        //    tablacheck.AddCell(celda03);
        //    tablacheck.AddCell(celda03c);
        //    tablacheck.AddCell(celda04);
        //    tablacheck.AddCell(celda04c);
        //    tablacheck.AddCell(celda05);
        //    tablacheck.AddCell(celda05c);
        //    tablacheck.AddCell(celda06);
        //    tablacheck.AddCell(celda06c);
        //    tablacheck.AddCell(celda07);
        //    tablacheck.AddCell(celda07c);
        //    tablacheck.AddCell(celda08);
        //    tablacheck.AddCell(celda08c);

        //    _documento.Add(tablacheck);
        //}

        //private void AgregarDatosCalidad(int id)
        //{
        //    float[] tamanouest = { 260f, 260f, 260f };
        //    PdfPTable tablaM = new PdfPTable(tamanouest);
        //    tablaM.SetTotalWidth(tamanouest);
        //    tablaM.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaM.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaM.LockedWidth = true;
        //    tablaM.DefaultCell.Border = Rectangle.NO_BORDER;

        //    PdfPCell TituloMuestreo = new PdfPCell(new Phrase("Muestreo", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
        //    PdfPCell TituloMuestreo1 = new PdfPCell(new Phrase("Nivel Inspección", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
        //    PdfPCell TituloMuestreo2 = new PdfPCell(new Phrase("Tamaño Muestra", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));

        //    TituloMuestreo.BackgroundColor = colordefinido;
        //    TituloMuestreo1.BackgroundColor = colordefinido;
        //    TituloMuestreo2.BackgroundColor = colordefinido;

        //    tablaM.AddCell(TituloMuestreo);
        //    tablaM.AddCell(TituloMuestreo1);
        //    tablaM.AddCell(TituloMuestreo2);
        //    _documento.Add(tablaM);

        //    OrdenProduccion orden = new WorkinProcessContext().Database.SqlQuery<OrdenProduccion>("Select*from OrdenProduccion where idorden=" + id).ToList().FirstOrDefault();
        //    Articulo articulo = new WorkinProcessContext().Database.SqlQuery<Articulo>("Select*from Articulo where IDArticulo=" + orden.IDArticulo).ToList().FirstOrDefault();

        //    CalidadContext db = new CalidadContext();

        //    string Letra = "";
        //    var CalidadLetras = new CalidadLetraContext().CalidadLetras;
        //    if (articulo.IDInspeccion == 1)
        //    {
        //        foreach (CalidadLetra CalidadL in CalidadLetras)
        //        {
        //            if (orden.Cantidad >= CalidadL.LI_Lote_mill && orden.Cantidad <= CalidadL.LS_Lote_mill)
        //            {
        //                Letra = CalidadL.NGI1;
        //            }

        //        }


        //    }
        //    else if (articulo.IDInspeccion == 2)
        //    {
        //        foreach (CalidadLetra CalidadL in CalidadLetras)
        //        {
        //            if (orden.Cantidad >= CalidadL.LI_Lote_mill && orden.Cantidad <= CalidadL.LS_Lote_mill)
        //            {
        //                Letra = CalidadL.NGI2;
        //            }

        //        }
        //    }
        //    else if (articulo.IDInspeccion == 3)
        //    {
        //        foreach (CalidadLetra CalidadL in CalidadLetras)
        //        {
        //            if (orden.Cantidad >= CalidadL.LI_Lote_mill && orden.Cantidad <= CalidadL.LS_Lote_mill)
        //            {
        //                Letra = CalidadL.NGI3;
        //            }

        //        }
        //    }
        //    else
        //    {
        //        Letra = "";
        //    }
        //    if (Letra == "N/A")
        //    {
        //        throw new Exception("Artículo Inspección N/A");
        //    }
        //    try
        //    {
        //        CalidadLimiteAceptacion CodigoLetra = new WorkinProcessContext().Database.SqlQuery<CalidadLimiteAceptacion>("Select*from CalidadLimiteAceptacion where CodigoLetra='" + Letra + "'").ToList().FirstOrDefault();

        //    }
        //    catch (Exception err)
        //    {

        //    }
        //    ///inspeccion
        //    ///
        //    string ni = "";
        //    try
        //    {
        //        Inspeccion nivelInspeccion = new CalidadContext().Inspecciones.Where(s => s.IDInspeccion == articulo.IDInspeccion).FirstOrDefault();
        //        ni = nivelInspeccion.Descripcion;
        //    }
        //    catch (Exception err)
        //    {
        //        ni = "";
        //    }
        //    /// Muestreo
        //    /// 
        //    string m = "";
        //    try
        //    {
        //        Muestreo mues = new CalidadContext().Muestreos.Where(s => s.IDMuestreo == articulo.IDMuestreo).FirstOrDefault();
        //        m = mues.Descripcion;
        //    }
        //    catch (Exception err)
        //    {
        //        m = "";
        //    }
        //    /////////limite de aceptacion
        //    ///
        //    string Descripcion = "";
        //    string DescripcionAC = "";
        //    int limiteAceptacion = 0;
        //    int limiteRechazo = 0;
        //    decimal tamanoMuestra = 0;
        //    try
        //    {
        //        if (articulo.IDMuestreo == 1)
        //        {
        //            Descripcion = "RE1";
        //            DescripcionAC = "AC1";
        //        }
        //        if (articulo.IDMuestreo == 2)
        //        {
        //            Descripcion = "RE15";
        //            DescripcionAC = "AC15";
        //        }
        //        if (articulo.IDMuestreo == 3)
        //        {
        //            Descripcion = "RE25";
        //            DescripcionAC = "AC25";
        //        }
        //        if (articulo.IDMuestreo == 4)
        //        {
        //            Descripcion = "RE4";
        //            DescripcionAC = "AC4";
        //        }
        //        if (articulo.IDMuestreo == 5)
        //        {
        //            Descripcion = "RE65";
        //            DescripcionAC = "AC65";
        //        }
        //        if (articulo.IDMuestreo == 6)
        //        {
        //            Descripcion = "RE10";
        //            DescripcionAC = "AC10";
        //        }
        //        if (articulo.IDMuestreo == 0)
        //        {
        //            Descripcion = "";
        //            DescripcionAC = "";
        //        }

        //        try
        //        {

        //            string cadenaA = " select " + Descripcion + " as Dato from dbo.CalidadLimiteAceptacion where [CodigoLetra]='" + Letra + "'";
        //            ClsDatoEntero rechazo = db.Database.SqlQuery<ClsDatoEntero>(cadenaA).ToList().FirstOrDefault();
        //            limiteRechazo = rechazo.Dato;
        //        }
        //        catch (Exception err)
        //        {

        //        }
        //        try
        //        {

        //            string cadenaA = " select " + DescripcionAC + " as Dato from dbo.CalidadLimiteAceptacion where [CodigoLetra]='" + Letra + "'";
        //            ClsDatoEntero aceptacion = db.Database.SqlQuery<ClsDatoEntero>(cadenaA).ToList().FirstOrDefault();

        //            limiteAceptacion = aceptacion.Dato;

        //        }
        //        catch (Exception err)
        //        {

        //        }
        //        try
        //        {

        //            string cadenaA = " select TamanoMuestra as Dato from dbo.CalidadLimiteAceptacion where [CodigoLetra]='" + Letra + "'";
        //            ClsDatoDecimal tamMuestra = db.Database.SqlQuery<ClsDatoDecimal>(cadenaA).ToList().FirstOrDefault();

        //            tamanoMuestra = tamMuestra.Dato;

        //        }
        //        catch (Exception err)
        //        {

        //        }
        //    }
        //    catch (Exception err)
        //    {

        //    }
        //    float[] anchoColumnasTablaProductos = { 260f, 260f, 260f };

        //    //Datos de los productos

        //    PdfPTable tablaResMue = new PdfPTable(anchoColumnasTablaProductos);

        //    //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
        //    tablaResMue.SetTotalWidth(anchoColumnasTablaProductos);
        //    tablaResMue.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaResMue.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    tablaResMue.LockedWidth = true;
        //    tablaResMue.DefaultCell.Border = Rectangle.NO_BORDER;

        //    PdfPCell celdaM = new PdfPCell(new Phrase("Normal AQL: " + m, new Font(Font.FontFamily.HELVETICA, 7)));


        //    celdaM.Border = Rectangle.NO_BORDER;
        //    PdfPCell celdaI = new PdfPCell(new Phrase(ni, new Font(Font.FontFamily.HELVETICA, 7)));


        //    celdaI.Border = Rectangle.NO_BORDER;
        //    PdfPCell celdaTM = new PdfPCell(new Phrase(tamanoMuestra + ", Ac: " + limiteAceptacion + ", Re: " + limiteRechazo, new Font(Font.FontFamily.HELVETICA, 7)));


        //    celdaTM.Border = Rectangle.NO_BORDER;


        //    tablaResMue.AddCell(celdaM);
        //    tablaResMue.AddCell(celdaI);
        //    tablaResMue.AddCell(celdaTM);
        //    _documento.Add(tablaResMue);




        //}


        private void AgregarDatosCalidad(int id)
        {
            OrdenProduccion orden = new WorkinProcessContext().Database.SqlQuery<OrdenProduccion>("Select*from OrdenProduccion where idorden=" + id).ToList().FirstOrDefault();
            Articulo articulo = new WorkinProcessContext().Database.SqlQuery<Articulo>("Select*from Articulo where IDArticulo=" + orden.IDArticulo).ToList().FirstOrDefault();

            CalidadContext db = new CalidadContext();

            string Letra = "";
            var CalidadLetras = new CalidadLetraContext().CalidadLetras;
            if (articulo.IDInspeccion == 1)
            {
                foreach (CalidadLetra CalidadL in CalidadLetras)
                {
                    if (orden.Cantidad >= CalidadL.LI_Lote_mill && orden.Cantidad <= CalidadL.LS_Lote_mill)
                    {
                        Letra = CalidadL.NGI1;
                    }

                }


            }
            else if (articulo.IDInspeccion == 2)
            {
                foreach (CalidadLetra CalidadL in CalidadLetras)
                {
                    if (orden.Cantidad >= CalidadL.LI_Lote_mill && orden.Cantidad <= CalidadL.LS_Lote_mill)
                    {
                        Letra = CalidadL.NGI2;
                    }

                }
            }
            else if (articulo.IDInspeccion == 3)
            {
                foreach (CalidadLetra CalidadL in CalidadLetras)
                {
                    if (orden.Cantidad >= CalidadL.LI_Lote_mill && orden.Cantidad <= CalidadL.LS_Lote_mill)
                    {
                        Letra = CalidadL.NGI3;
                    }

                }
            }
            else
            {
                Letra = "";
            }
            if (Letra == "N/A")
            {
                throw new Exception("Artículo Inspección N/A");
            }
            try
            {
                CalidadLimiteAceptacion CodigoLetra = new WorkinProcessContext().Database.SqlQuery<CalidadLimiteAceptacion>("Select*from CalidadLimiteAceptacion where CodigoLetra='" + Letra + "'").ToList().FirstOrDefault();

            }
            catch (Exception err)
            {

            }
            ///inspeccion
            ///
            string ni = "";
            try
            {
                Inspeccion nivelInspeccion = new CalidadContext().Inspecciones.Where(s => s.IDInspeccion == articulo.IDInspeccion).FirstOrDefault();
                ni = nivelInspeccion.Descripcion;
            }
            catch (Exception err)
            {
                ni = "";
            }
            /// Muestreo
            /// 
            string m = "";
            try
            {
                Muestreo mues = new CalidadContext().Muestreos.Where(s => s.IDMuestreo == articulo.IDMuestreo).FirstOrDefault();
                m = mues.Descripcion;
            }
            catch (Exception err)
            {
                m = "";
            }
            /////////limite de aceptacion
            ///
            string Descripcion = "";
            string DescripcionAC = "";
            int limiteAceptacion = 0;
            int limiteRechazo = 0;
            decimal tamanoMuestra = 0;
            try
            {
                if (articulo.IDMuestreo == 1)
                {
                    Descripcion = "RE1";
                    DescripcionAC = "AC1";
                }
                if (articulo.IDMuestreo == 2)
                {
                    Descripcion = "RE15";
                    DescripcionAC = "AC15";
                }
                if (articulo.IDMuestreo == 3)
                {
                    Descripcion = "RE25";
                    DescripcionAC = "AC25";
                }
                if (articulo.IDMuestreo == 4)
                {
                    Descripcion = "RE4";
                    DescripcionAC = "AC4";
                }
                if (articulo.IDMuestreo == 5)
                {
                    Descripcion = "RE65";
                    DescripcionAC = "AC65";
                }
                if (articulo.IDMuestreo == 6)
                {
                    Descripcion = "RE10";
                    DescripcionAC = "AC10";
                }
                if (articulo.IDMuestreo == 0)
                {
                    Descripcion = "";
                    DescripcionAC = "";
                }

                try
                {

                    string cadenaA = " select " + Descripcion + " as Dato from dbo.CalidadLimiteAceptacion where [CodigoLetra]='" + Letra + "'";
                    ClsDatoEntero rechazo = db.Database.SqlQuery<ClsDatoEntero>(cadenaA).ToList().FirstOrDefault();
                    limiteRechazo = rechazo.Dato;
                }
                catch (Exception err)
                {

                }
                try
                {

                    string cadenaA = " select " + DescripcionAC + " as Dato from dbo.CalidadLimiteAceptacion where [CodigoLetra]='" + Letra + "'";
                    ClsDatoEntero aceptacion = db.Database.SqlQuery<ClsDatoEntero>(cadenaA).ToList().FirstOrDefault();

                    limiteAceptacion = aceptacion.Dato;

                }
                catch (Exception err)
                {

                }
                try
                {

                    string cadenaA = " select TamanoMuestra as Dato from dbo.CalidadLimiteAceptacion where [CodigoLetra]='" + Letra + "'";
                    ClsDatoDecimal tamMuestra = db.Database.SqlQuery<ClsDatoDecimal>(cadenaA).ToList().FirstOrDefault();

                    tamanoMuestra = tamMuestra.Dato;

                }
                catch (Exception err)
                {

                }
            }
            catch (Exception err)
            {

            }
            float[] tamanouest = { 260f, 520f };
            //float[] tamanouest = { 260f, 260f, 260f };


            PdfPTable tablaM = new PdfPTable(tamanouest);
            tablaM.SetTotalWidth(tamanouest);
            tablaM.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaM.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaM.LockedWidth = true;
            tablaM.DefaultCell.Border = Rectangle.NO_BORDER;

            float[] tamanomues = { 70f, 190f };
            PdfPTable tablaMues = new PdfPTable(tamanouest);
            PdfPCell TituloMuestreo = new PdfPCell(new Phrase("Muestreo", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            TituloMuestreo.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            PdfPCell celdaM = new PdfPCell(new Phrase("Normal AQL: " + m, new Font(Font.FontFamily.HELVETICA, 9)));
            tablaMues.AddCell(TituloMuestreo);
            tablaMues.AddCell(celdaM);

            tablaM.AddCell(tablaMues);

            float[] tamanoNT = { 240f, 240f, 20f, 20f };
            PdfPTable TableN = new PdfPTable(tamanoNT);
            PdfPCell Nivel = new PdfPCell(new Phrase("Nivel de Inspección", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell muest = new PdfPCell(new Phrase("Tamaño de muestra", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell AC = new PdfPCell(new Phrase("Ac", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell RE = new PdfPCell(new Phrase("Re", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));


            Nivel.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            muest.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            AC.BackgroundColor = new CMYKColor(0, 29, 50, 70); ;
            RE.BackgroundColor = new CMYKColor(0, 29, 50, 70);
            PdfPCell Nivel1 = new PdfPCell(new Phrase(ni, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell muest1 = new PdfPCell(new Phrase(tamanoMuestra.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell AC1 = new PdfPCell(new Phrase(limiteAceptacion.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell RE1 = new PdfPCell(new Phrase(limiteRechazo.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));


            TableN.AddCell(Nivel);
            TableN.AddCell(muest);
            TableN.AddCell(AC);
            TableN.AddCell(RE);
            TableN.AddCell(Nivel1);
            TableN.AddCell(muest1);
            TableN.AddCell(AC1);
            TableN.AddCell(RE1);


            tablaM.AddCell(TableN);

            _documento.Add(tablaM);
            //PdfPCell TituloMuestreo = new PdfPCell(new Phrase("Muestreo", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));


            //PdfPCell TituloMuestreo = new PdfPCell(new Phrase("Muestreo", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
            //PdfPCell TituloMuestreo1 = new PdfPCell(new Phrase("Nivel Inspección", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));
            //PdfPCell TituloMuestreo2 = new PdfPCell(new Phrase("Tamaño Muestra", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorencabezadodefinido)));

            //TituloMuestreo.BackgroundColor = colordefinido;
            //TituloMuestreo1.BackgroundColor = colordefinido;
            //TituloMuestreo2.BackgroundColor = colordefinido;

            //tablaM.AddCell(TituloMuestreo);
            //tablaM.AddCell(TituloMuestreo1);
            //tablaM.AddCell(TituloMuestreo2);
            //_documento.Add(tablaM);


            //float[] anchoColumnasTablaProductos = { 260f, 260f, 260f };

            ////Datos de los productos

            //PdfPTable tablaResMue = new PdfPTable(anchoColumnasTablaProductos);

            ////tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            //tablaResMue.SetTotalWidth(anchoColumnasTablaProductos);
            //tablaResMue.HorizontalAlignment = Element.ALIGN_LEFT;
            //tablaResMue.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            //tablaResMue.LockedWidth = true;
            //tablaResMue.DefaultCell.Border = Rectangle.NO_BORDER;

            //PdfPCell celdaM = new PdfPCell(new Phrase("Normal AQL: " + m, new Font(Font.FontFamily.HELVETICA, 7)));


            //celdaM.Border = Rectangle.NO_BORDER;
            //PdfPCell celdaI = new PdfPCell(new Phrase(ni, new Font(Font.FontFamily.HELVETICA, 7)));


            //celdaI.Border = Rectangle.NO_BORDER;
            //PdfPCell celdaTM = new PdfPCell(new Phrase(tamanoMuestra + ", Ac: " + limiteAceptacion + ", Re: " + limiteRechazo, new Font(Font.FontFamily.HELVETICA, 7)));


            //celdaTM.Border = Rectangle.NO_BORDER;


            //tablaResMue.AddCell(celdaM);
            //tablaResMue.AddCell(celdaI);
            //tablaResMue.AddCell(celdaTM);
            //_documento.Add(tablaResMue);




        }


        private void AgregarDatosEntrega(int id)
        {
           
            //Datos de la factura
            Paragraph p2 = new Paragraph();

            p2.SpacingAfter = 50;
            p2.Leading = 10;
            p2.Alignment = Element.ALIGN_RIGHT;
            string fecha = DateTime.Now.ToString("dd/MM/yyyy");

            p2.Add(new Phrase("\nFolio | ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add(new Phrase(id.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
            p2.Add(new Phrase("\nFecha  | ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add(new Phrase(fecha, new Font(Font.FontFamily.HELVETICA, 8)));

            Caracteristica ARTP = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Id="+orden.IDCaracteristica).FirstOrDefault();

            int idhe = ARTP.IDCotizacion;
            try
            {
                Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(idhe);
                XmlDocument documento = new XmlDocument();
                string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                documento.Load(nombredearchivo);
                especificacion = null;
                XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    especificacion = (ClsCotizador)serializerX.Deserialize(reader);
                }
            }
            catch(Exception err)
            {
                string mesnaje = err.Message;
            }

            p2.Add(new Phrase("\n\nCotizacion: " + idhe, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
           


            
            _documento.Add(p2);
        }

        private void AgregarTitulo(int id)
        {
            //Datos de la factura
            Paragraph p3 = new Paragraph();
            //p3.IndentationLeft = 50f;
            p3.SpacingAfter = 10;
            p3.Leading = -50;
            p3.Alignment = Element.ALIGN_CENTER;

            p3.Add(new Phrase("ORDEN DE PRODUCCIÓN "+id, new Font(Font.FontFamily.HELVETICA, 15, Font.BOLD)));

     


            p3.SpacingAfter = 5;
            _documento.Add(p3);
        }


        #endregion

    }

   



    #region Extensión de la clase pdfPageEvenHelper
    public class ITextEventsOrdenProducciontermo : PdfPageEventHelper
    {
        PdfContentByte cb;

        //// we will put the final number of pages in a template
        PdfTemplate headerTemplate, footerTemplate;

        //// this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;
        //// This is the contentbyte object of the writer
        //PdfContentByte cb;

        //// we will put the final number of pages in a template
        //PdfTemplate headerTemplate, footerTemplate;

        //// this is the BaseFont we are going to use for the header / footer
        //BaseFont bf = null;

        //// This keeps track of the creation time
        //DateTime PrintTime = DateTime.Now;


        //#region Fields
        //private string _header;
        //#endregion

        //#region Properties
        //public string Header
        //{
        //    get { return _header; }
        //    set { _header = value; }
        //}
        //#endregion


        //public override void OnOpenDocument(PdfWriter writer, Document document)
        //{
        //    try
        //    {
        //        PrintTime = DateTime.Now;
        //        bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        //        cb = writer.DirectContent;
        //        headerTemplate = cb.CreateTemplate(100, 100);
        //        footerTemplate = cb.CreateTemplate(50, 50);
        //    }
        //    catch (DocumentException)
        //    { }
        //    catch (System.IO.IOException)
        //    { }
        //}
        //public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        //{
        //    base.OnEndPage(writer, document);

        //    String text = "Página " + writer.PageNumber + " de ";


        //    {
        //        cb.BeginText();
        //        cb.SetFontAndSize(bf, 9);
        //        cb.SetTextMatrix(document.PageSize.GetRight(70), document.PageSize.GetBottom(30));
        //        cb.ShowText(text);
        //        cb.EndText();
        //        float len = bf.GetWidthPoint(text, 9);
        //        cb.AddTemplate(footerTemplate, document.PageSize.GetRight(70) + len, document.PageSize.GetBottom(30));

        //        float[] anchoColumasTablaTotales = { 600f };
        //        PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
        //        tabla.DefaultCell.Border = Rectangle.NO_BORDER;
        //        tabla.SetTotalWidth(anchoColumasTablaTotales);
        //        tabla.HorizontalAlignment = Element.ALIGN_CENTER;
        //        tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
        //        tabla.LockedWidth = true;
        //        tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(40), writer.DirectContent);

        //    }
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                //PrintTime = DateTime.Now;
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

                float[] anchoColumasTablaTotales = { 150f, 500f, 120f };
                PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
                tabla.DefaultCell.Border = Rectangle.NO_BORDER;
                tabla.SetTotalWidth(anchoColumasTablaTotales);
                tabla.HorizontalAlignment = Element.ALIGN_CENTER;
                tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                tabla.LockedWidth = true;
                tabla.AddCell(new Phrase("Class Labels  S. de R.L. de C.V \n Fecha de Revisión: 27-10-2020", new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.AddCell(new Phrase("EL FORMATO SOLO PUEDE SER VALIDO SI ES OBTENIDO DE LA LISTA MAESTRA", new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.AddCell(new Phrase("" + "\n" + TextRevision, new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                //tabla.AddCell(new Phrase(TextRevision, new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(40), writer.DirectContent);

            }



        }
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            headerTemplate.BeginText();
            headerTemplate.SetFontAndSize(bf, 12);
            headerTemplate.SetTextMatrix(0, 0);
            headerTemplate.ShowText((writer.PageNumber - 1).ToString());
            headerTemplate.EndText();

            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bf, 9);
            //footerTemplate.MoveText(550,30);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.ShowText((writer.PageNumber).ToString());
            footerTemplate.EndText();
        }


    }

    //public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
    //{
    //    base.OnEndPage(writer, document);


    //    String text = "Página " + writer.PageNumber + " ";
    //    //String TextRevision = "Rev. 2";


    //    {
    //        cb.BeginText();
    //        cb.SetFontAndSize(bf, 9);
    //        cb.SetTextMatrix(document.PageSize.GetRight(70), document.PageSize.GetBottom(30));
    //        //cb.MoveText(500,30);
    //        //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
    //        //cb.ShowText(text);
    //        cb.EndText();
    //        float len = bf.GetWidthPoint(text, 9);
    //        cb.AddTemplate(footerTemplate, document.PageSize.GetRight(70) + len, document.PageSize.GetBottom(30));

    //        float[] anchoColumasTablaTotales = { 480f,120f};
    //        PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
    //        tabla.DefaultCell.Border = Rectangle.NO_BORDER;
    //        tabla.SetTotalWidth(anchoColumasTablaTotales);
    //        tabla.HorizontalAlignment = Element.ALIGN_RIGHT;
    //        tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
    //        tabla.LockedWidth = true;
    //        tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
    //        tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
    //        tabla.AddCell(new Phrase("El formato solo puede ser valido si es obtenido de la lista maestra", new Font(Font.FontFamily.HELVETICA, 8)));
    //        tabla.AddCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 8)));
    //        tabla.WriteSelectedRows(0, -2, 150, document.PageSize.GetBottom(40), writer.DirectContent);

    //    }





    //public override void OnCloseDocument(PdfWriter writer, Document document)
    //{
    //    //base.OnCloseDocument(writer, document);

    //    ////headerTemplate.BeginText();
    //    ////headerTemplate.SetFontAndSize(bf, 12);
    //    ////headerTemplate.SetTextMatrix(0, 0);
    //    ////headerTemplate.ShowText((writer.PageNumber - 1).ToString());
    //    ////headerTemplate.EndText();

    //    //footerTemplate.BeginText();
    //    //footerTemplate.SetFontAndSize(bf, 9);
    //    ////footerTemplate.MoveText(550,30);
    //    //footerTemplate.SetTextMatrix(0, 0);
    //    //footerTemplate.ShowText((writer.PageNumber).ToString());
    //    //footerTemplate.EndText();
    //}
    //public override void OnCloseDocument(PdfWriter writer, Document document)
    //{
    //    base.OnCloseDocument(writer, document);

    //    footerTemplate.BeginText();
    //    footerTemplate.SetFontAndSize(bf, 9);
    //    //footerTemplate.MoveText(550,30);
    //    footerTemplate.SetTextMatrix(0, 0);
    //    //footerTemplate.ShowText((writer.PageNumber - 1).ToString());
    //    footerTemplate.EndText();
    //}
}
    #endregion






