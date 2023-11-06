using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.clasescfdi;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace SIAAPI.Reportes
{

    public class ProductoRe
    {

        public string cantidad = string.Empty;
        public string descripcion = string.Empty;
        public string unidad = string.Empty;
        public float valorUnitario = 0.00f;
        public float importe = 0.00f;
        public string numIdentificacion = string.Empty;

        public string Presentacion = string.Empty;

        public string Observacion = string.Empty;
        public string ClaveProducto { get; internal set; }
        public string id { get; internal set; }
        public string c_unidad { get; internal set; }
        public string desc { get; internal set; }
        public float v_unitario = 0.00f;
        public float descuento { get; internal set; }

        public int IDPresentacion { get; set; }

        public string almacen { get; set; }

        public int idarticulo { get; set; }
    }

    public class ImpuestoRe
    {
        public string impuesto;
        public float tasa;
        public float importe;
    }

    public class RetencionRe
    {
        public string impuesto;
        public float importe;
    }

    public class DocumentoRe
    {
        public string serie = string.Empty;
        public string folio = string.Empty;

        public string fecha = string.Empty;

        public string fechaRequerida = string.Empty;

        public string lugarExpedicion = string.Empty;
        public string formaPago = string.Empty;
        public string metodoPago = string.Empty;
        public string claveMoneda = string.Empty;
        public string condicionesdepago = string.Empty;
        public string firmadefinanzas = string.Empty;
        public string firmadecompras = string.Empty;

        public string Empresa = string.Empty;
        public string Direccion = string.Empty;
        public string Telefono = string.Empty;
        public string RFC = string.Empty;


        public string Cliente = string.Empty;
        public string RFCCliente = string.Empty;
        public string TelefonoCliente = string.Empty;

        public string Observacion = string.Empty;

        public string cadenaOriginal = string.Empty;

        public float subtotal = 0.00f;
        public float total = 0.00f;
        public float descuento = 0.00f;

        public int IDRemision = 0;

        public List<ProductoRe> productos = new List<ProductoRe>();
        public List<ImpuestoRe> impuestos = new List<ImpuestoRe>();
        public List<RetencionRe> retenciones = new List<RetencionRe>();

        public string totalEnLetra = string.Empty;
        public float totalImpuestosRetenidos = 0.00f;


        public string tipo_cambio { get; internal set; }

        public string UsodelCFDI { get; set; }

        public string Almacen { get; set; }
    }

    public class CreaRePDF
    {
        MemoryStream _memoryStream = new MemoryStream();
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;
        public DocumentoRe _templatePDF; //Objeto que contendra la información del documento pdf
        XmlDocument xDoc; // Objeto para abrir el archivo xml
        CMYKColor colordefinido;
        CMYKColor colorfuente;

        public CreaRePDF(System.Drawing.Image logo, DocumentoRe Remision)
        {
			ClsColoresReporte colorr = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            colordefinido = colorr.color;
            colorfuente = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;
            _templatePDF = Remision;
            ObtenerLetras();
            //Trabajos con el documento XML
            _documento = new Document(PageSize.LETTER);
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            //string nombreDocumento = rutaPDF;

            //Creamos el documento
            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEventsRe(); // invoca la clase que esta mas abajo correspondiente al pie de pagina

            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();


            AgregarLogo(logo);
            AgregarCuadro();
            AgregarDatosEmisor("");
           AgregarDatosFactura();

            AgregarDatosReceptorEmisor();

            AgregarDatosProductos();
            AgregarTotales();

            Agregarpie();

            //     AgregarSellos();

            //Cerramoe el documento
            _documento.Close();

            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"" + _templatePDF.Cliente + _templatePDF.serie + _templatePDF.folio + ".pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

        }

        private void Agregarpie()
        {
            float[] anchoColumasTablapie = { 600f };
            PdfPTable tablapie = new PdfPTable(anchoColumasTablapie);
            tablapie.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            //tablapie.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapie.SetTotalWidth(anchoColumasTablapie);
            tablapie.HorizontalAlignment = Element.ALIGN_CENTER;
            tablapie.LockedWidth = true;

            tablapie.AddCell(new Phrase("Horario de entrega de Lunes a Viernes de 8:00 am a 5:30 pm.", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            //tablapie.AddCell(new Phrase("SE RECIBIRA UNICA Y EXCLUSIVAMENTE LA CANTIDAD EXPUESTA EN ESTE PEDIDO, POR LO QUE NO SE ADMITIRA FALTANTE O SOBRANTE ALGUNO.", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            tablapie.AddCell(new Phrase("Condiciones de Pago: " + _templatePDF.condicionesdepago, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //tablapie.AddCell(new Phrase("El proveedor se obliga a vender y entregar los productos y/o en proporcionar los servicios especificados en este documento, de acuerdo con los términos, condiciones y cláusulas del Pedido, y de sus cláusulas especiales, modificaciones o complementos.Todo lo anterior, constituye un acuerdo final y completo entre las partes.Los términos, condiciones y cláusulas de éste Pedido se encuentran en este documento.Las obligaciones del comprador quedan expresamente limitadas en los términos, condiciones y clausulas aquí contenidas.Cualquier otro término, condición o cláusula que proponga el Proveedor no será valida a menos que sea aceptada por escrito por el Comprador.Nos reservamos el derecho a realizar Auditoria en las instalaciones del Proveedor.\n\nDocumentos para recepcion indispensables\n-Factura original y copia\n-Pedido\n-Certificado de Calidad", new Font(Font.FontFamily.HELVETICA, 6)));

            float[] anchoColumasTablaFirmas = { 500f, 100f };
            PdfPTable tablafirmas = new PdfPTable(anchoColumasTablaFirmas);
            tablafirmas.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafirmas.DefaultCell.Border = Rectangle.NO_BORDER;
            tablafirmas.SetTotalWidth(anchoColumasTablaFirmas);
            tablafirmas.HorizontalAlignment = Element.ALIGN_CENTER;
            tablafirmas.LockedWidth = true;



            tablafirmas.AddCell(new Phrase("\nCliente", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            //tablafirmas.AddCell(new Phrase("Firma de conformidad", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            StringBuilder codigoQR = new StringBuilder();
            codigoQR.Append("http://" + SIAAPI.Properties.Settings.Default.Nombredelaaplicacion + "/EncRemision/Details/" + _templatePDF.folio);


            BarcodeQRCode pdfCodigoQR = new BarcodeQRCode(codigoQR.ToString(), 1, 1, null);
            Image img = pdfCodigoQR.GetImage();
            img.SpacingAfter = 0.0f;
            img.SpacingBefore = 0.0f;
            img.BorderWidth = 1.0f;
            //img.ScalePercent(100, 78);
            //img.border


            tablafirmas.AddCell(img);

            tablafirmas.AddCell(new Phrase("__________________________________\n Firma de conformidad", new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
            //tablafirmas.AddCell(new Phrase(_templatePDF.firmadefinanzas, new Font(Font.FontFamily.HELVETICA, 6)));
            tablafirmas.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));

            _documento.Add(tablapie);
            _documento.Add(tablafirmas);
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
            imagen.ScaleToFit(140, 100);
            imagen.Alignment = Element.ALIGN_TOP;
            Chunk logo = new Chunk(imagen, 1, -40);
            _documento.Add(logo);
        }

        private void AgregarCuadro()
        {
            _cb = _writer.DirectContentUnder;
          

            //Agrego cuadro al documento
            _cb.SetColorStroke(colordefinido); //Color de la linea
            _cb.SetColorFill(colordefinido); // Color del relleno
            _cb.SetLineWidth(3.5f); //Tamano de la linea
            _cb.Rectangle(408, 694, 20, 100);
            _cb.FillStroke();
        }

        private void AgregarDatosEmisor(String Telefono)
        {
            //Datos del emisor
            Paragraph p1 = new Paragraph();
			  p1.SpacingAfter = 20;
            p1.IndentationLeft = 150f;

            p1.Leading = 9;

            p1.Add(new Phrase(_templatePDF.Empresa, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.RFC, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.Telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.Direccion, new Font(Font.FontFamily.HELVETICA, 8)));

            p1.Add("\n");
            p1.SpacingAfter = -70;
            p1.Add("\n");
            p1.Add("\n");

            //if (_templatePDF.emisor.telefono != string.Empty)
            //{
            //    p1.Add(new Phrase("Tel." + _templatePDF.emisor.telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            //    p1.Add("\n");
            //}
            _documento.Add(p1);
        }

        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("Remisión");
            _documento.AddSubject("Remisión");
            _documento.AddTitle("REMISIÓN");
            _documento.SetMargins(5, 5, 5, 5);
        }

        private void AgregarDatosFactura()
        {
            //Datos de la factura
            Paragraph p2 = new Paragraph();
            p2.IndentationLeft = 403f;
            p2.SpacingAfter = 18;
            p2.Leading = 10;
            p2.Alignment = Element.ALIGN_CENTER;

            p2.Add(new Phrase("REMISIÓN NÚM: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add(new Phrase("\n" + _templatePDF.serie + " " + _templatePDF.folio, new Font(Font.FontFamily.HELVETICA, 16)));

            p2.Add(new Phrase("\nFECHA " + _templatePDF.fecha, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));

            p2.Add("\n");
            p2.Add(new Phrase("\nFECHA EN LA QUE SE REQUIERE \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add(new Phrase(_templatePDF.fechaRequerida, new Font(Font.FontFamily.HELVETICA, 8)));

            p2.Add(new Phrase("\nCODIGO: FSG-27 ", new Font(Font.FontFamily.HELVETICA, 8)));

            p2.SpacingAfter = 20;
            _documento.Add(p2);
        }

       private void AgregarDatosReceptorEmisor()
        {
            EncRemision remision = new RemisionContext().EncRemisiones.Find(_templatePDF.IDRemision);

            Clientes cliente = new ClientesContext().Clientes.Find(remision.IDCliente);

            float[] anchoColumnasTablaDatos = { 600f };
            PdfPTable tablaDatosPrincipal = new PdfPTable(anchoColumnasTablaDatos);
            tablaDatosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.SpacingBefore = 15;
            tablaDatosPrincipal.SetTotalWidth(anchoColumnasTablaDatos);
            tablaDatosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosPrincipal.LockedWidth = true;

            //Datos del receptor
            float[] anchoColumnaspro = { 230f, 230F, 140f };
            PdfPTable tablapro = new PdfPTable(anchoColumnaspro);
            tablapro.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapro.SetTotalWidth(anchoColumnaspro);
            tablapro.HorizontalAlignment = Element.ALIGN_LEFT;
            tablapro.LockedWidth = true;



            PdfPCell celda0 = new PdfPCell(new Phrase("CLIENTE: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD,colorfuente)));
            celda0.BackgroundColor = colordefinido;
            PdfPCell celda3 = new PdfPCell(new Phrase("ENTREGAR EN: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD,colorfuente)));
            celda3.BackgroundColor = colordefinido;
            PdfPCell celdaobserva = new PdfPCell(new Phrase("OBSERVACION :".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD,colorfuente)));
            celdaobserva.BackgroundColor = colordefinido;

            PdfPCell celda1 = new PdfPCell(new Phrase(cliente.Nombre+"\n"+cliente.Calle+" " + cliente.NumExt+" "+ cliente.NumInt+"\n"+ cliente.Colonia+"\n"+cliente.Municipio+","+ cliente.Estados.Estado +" \n"+ _templatePDF.TelefonoCliente, new Font(Font.FontFamily.HELVETICA, 8)));
           

            PdfPCell celda4 = new PdfPCell(new Phrase(remision.Entrega + " ", new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celdaobser = new PdfPCell(new Phrase(remision.Observacion, new Font(Font.FontFamily.HELVETICA, 8)));







            tablapro.AddCell(celda0);
            tablapro.AddCell(celda3);
            tablapro.AddCell(celdaobserva);

            tablapro.AddCell(celda1);
            tablapro.AddCell(celda4);
            tablapro.AddCell(celdaobser);
            //tablapro.AddCell(celda5);
            //tablapro.AddCell(celda6);

            tablapro.CompleteRow();
            _documento.Add(tablapro);

            float[] anchoColumnas = { 60f, 80F, 90f, 90f, 70f, 65f, 75F, 70f };
            PdfPTable tabla = new PdfPTable(anchoColumnas);
            tabla.DefaultCell.Border = Rectangle.BOX;
            tabla.SetTotalWidth(anchoColumnas);
            tabla.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.LockedWidth = true;



            PdfPCell celdaMO = new PdfPCell(new Phrase("MONEDA ", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, colorfuente)));
            PdfPCell celdaUS = new PdfPCell(new Phrase("USO DE CFDI".ToUpper(), new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, colorfuente)));
            PdfPCell celdaFO = new PdfPCell(new Phrase("FORMA DE PAGO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, colorfuente)));
            PdfPCell celdaME = new PdfPCell(new Phrase("METODO DE PAGO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, colorfuente)));
            PdfPCell celdaTI = new PdfPCell(new Phrase("TIPO DE CAMBIO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, colorfuente)));
            PdfPCell celdaAL = new PdfPCell(new Phrase("O.COMPRA".ToUpper(), new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, colorfuente)));
            PdfPCell celdaFE = new PdfPCell(new Phrase("CANT EXACTO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, colorfuente)));
            PdfPCell celdaCE = new PdfPCell(new Phrase("CERTIFICADO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, colorfuente)));




            celdaMO.BackgroundColor = colordefinido;
            celdaUS.BackgroundColor = colordefinido;
            celdaFO.BackgroundColor = colordefinido;
            celdaME.BackgroundColor = colordefinido;
            celdaTI.BackgroundColor = colordefinido;
            celdaAL.BackgroundColor = colordefinido;
            celdaFE.BackgroundColor = colordefinido;
            celdaCE.BackgroundColor = colordefinido;
            tabla.AddCell(celdaMO);
            tabla.AddCell(celdaUS);
            tabla.AddCell(celdaFO);
            tabla.AddCell(celdaME);
            tabla.AddCell(celdaTI);
            tabla.AddCell(celdaAL);
            tabla.AddCell(celdaFE);
            tabla.AddCell(celdaCE);
            tabla.CompleteRow();


            tabla.AddCell(new Phrase(_templatePDF.claveMoneda, new Font(Font.FontFamily.HELVETICA, 6)));
            tabla.AddCell(new Phrase(_templatePDF.UsodelCFDI, new Font(Font.FontFamily.HELVETICA, 6)));

            string f_pago = DecodificadorSAT.getFormapago(_templatePDF.formaPago);


            tabla.AddCell(new Phrase(f_pago.ToUpper(), new Font(Font.FontFamily.HELVETICA, 6)));



            string m_pago = DecodificadorSAT.getMetodo(_templatePDF.metodoPago);

            tabla.AddCell(new Phrase(m_pago.ToUpper(), new Font(Font.FontFamily.HELVETICA, 6)));


            ///cambio de renglon
            ///
            PdfPCell celdatc = new PdfPCell(new Phrase(_templatePDF.tipo_cambio, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            celdatc.HorizontalAlignment = Element.ALIGN_CENTER;



            tabla.AddCell(celdatc);
            string oc = "";
            try
            {
                ClsRastreaOC rastrea = new ClsRastreaOC(_templatePDF.IDRemision);
                oc = rastrea.RastreadeRemision();

            }
            catch (Exception err)
            {
                oc = string.Empty;
            }
            
           
            tabla.AddCell(new Phrase(oc.ToUpper(), new Font(Font.FontFamily.HELVETICA, 6)));
            if (cliente.FacturacionExacta)
            {

                tabla.AddCell(new Phrase("SI", new Font(Font.FontFamily.HELVETICA, 8)));
            }
            else
            {
                tabla.AddCell(new Phrase("NO", new Font(Font.FontFamily.HELVETICA, 8)));
            }

            if (cliente.CertificadoCalidad)
            {
               
                tabla.AddCell(new Phrase("SI", new Font(Font.FontFamily.HELVETICA, 8)));
            }
            else
            {
               
                tabla.AddCell(new Phrase("NO", new Font(Font.FontFamily.HELVETICA, 8)));
            }
        

            tabla.CompleteRow();

            float[] anchoColumnasobserva = { 80f, 520f };
            PdfPTable tablaobservacion = new PdfPTable(anchoColumnasobserva);
            tablaobservacion.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaobservacion.SetTotalWidth(anchoColumnasobserva);
            tablaobservacion.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaobservacion.LockedWidth = true;

            



            tablaDatosPrincipal.AddCell(tabla);
          
            tablaDatosPrincipal.AddCell(tablaobservacion);
            //
            _documento.Add(tablaDatosPrincipal);

        }

        private string ReturnClavePago(string clave)
        {
            string valor = "";

            valor = clave;

            return valor.ToString().ToUpper();
        }


       
        private void AgregarDatosProductos()
        {
            float[] anchoColumnasTablaProductos = { 600f };
            PdfPTable tablaProductosPrincipal = new PdfPTable(anchoColumnasTablaProductos);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosPrincipal.SetTotalWidth(anchoColumnasTablaProductos);
            tablaProductosPrincipal.SpacingBefore = 15;
            tablaProductosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosPrincipal.LockedWidth = true;


            //Datos de los productos
            float[] tamanoColumnas = { 60f, 50f, 70f, 230f, 75f, 60f, 55f };
            PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);

            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;



            PdfPCell celda0 = new PdfPCell(new Phrase("CLAVE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda1 = new PdfPCell(new Phrase("CANTIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda2 = new PdfPCell(new Phrase("CLAVE UNIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda3 = new PdfPCell(new Phrase("DESCRIPCION", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda4 = new PdfPCell(new Phrase("VALOR UNITARIO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda5 = new PdfPCell(new Phrase("IMPORTE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda6 = new PdfPCell(new Phrase("DESCUENTO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));



            celda0.BackgroundColor = colordefinido;
            celda1.BackgroundColor = colordefinido;
            celda2.BackgroundColor = colordefinido;
            celda3.BackgroundColor = colordefinido;
            celda4.BackgroundColor = colordefinido;
            celda5.BackgroundColor = colordefinido;
            celda6.BackgroundColor = colordefinido;

            tablaProductosTitulos.AddCell(celda0);
            tablaProductosTitulos.AddCell(celda1);
            tablaProductosTitulos.AddCell(celda2);
            tablaProductosTitulos.AddCell(celda3);
            tablaProductosTitulos.AddCell(celda4);
            tablaProductosTitulos.AddCell(celda5);
            tablaProductosTitulos.AddCell(celda6);





            float[] tamanoColumnasProductos = { 60f, 50f, 70f, 230f, 75f, 60f, 55f };
            PdfPTable tablaProductos = new PdfPTable(tamanoColumnas);
            //tablaProductos.SpacingBefore = 1;
            //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductos.DefaultCell.BorderWidthLeft = 0.1f;
            tablaProductos.DefaultCell.BorderWidthRight = 0.0f;
            tablaProductos.DefaultCell.BorderWidthBottom = 0.0f;
            tablaProductos.DefaultCell.BorderWidthTop = 0.0f;
            tablaProductos.SetTotalWidth(tamanoColumnas);
            tablaProductos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductos.LockedWidth = true;






            var result2 = from line in _templatePDF.productos
                          group line by new { line.descripcion, line.Presentacion, line.Observacion } into csLine
                          select new LineadeproductosRe
                          {
                              ClaveProducto = csLine.First().ClaveProducto,
                              Cantidad = csLine.Sum(c => decimal.Parse(c.cantidad)),
                              Unidad = csLine.First().c_unidad,
                              Descripcion = csLine.First().descripcion,
                              v_unitario = decimal.Parse(csLine.First().v_unitario.ToString()),
                              importe = csLine.Sum(c => Math.Round(decimal.Parse(c.importe.ToString()), 2)),
                              descuento = decimal.Parse(csLine.First().descuento.ToString()),
                              Presentacion = csLine.First().Presentacion,
                              Observacion = csLine.First().Observacion,

                          };


            foreach (LineadeproductosRe p in result2)
            {
                tablaProductos.AddCell(new Phrase(p.ClaveProducto, new Font(Font.FontFamily.HELVETICA, 8)));

                tablaProductos.AddCell(new Phrase(p.Cantidad.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.Unidad, new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.Descripcion, new Font(Font.FontFamily.HELVETICA, 8)));
                    PdfPCell celdauni = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                    celdauni.Phrase = new Phrase(p.v_unitario.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdauni);
                    PdfPCell celdaimporte = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                    celdaimporte.Phrase = new Phrase(p.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdaimporte);
                    PdfPCell celdadescuento = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                    celdadescuento.Phrase = new Phrase(p.descuento.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));

                tablaProductos.AddCell(celdadescuento);
                // AQUI CAMBIA DE RENGLON 


                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.Presentacion, new Font(Font.FontFamily.TIMES_ROMAN, 6)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

                // otro renglon si tiene observacion

                if (!string.IsNullOrEmpty(p.Observacion))
                {
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaProductos.AddCell(new Phrase(p.Observacion, new Font(Font.FontFamily.TIMES_ROMAN, 6)));
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                }



            }


            PdfPCell celdaTitulos = new PdfPCell(tablaProductosTitulos);
            tablaProductosPrincipal.AddCell(celdaTitulos);
            PdfPCell celdaProductos = new PdfPCell(tablaProductos);
            celdaProductos.MinimumHeight = 300;
            tablaProductosPrincipal.AddCell(celdaProductos);
            _documento.Add(tablaProductosPrincipal);
        }

        private void AgregarTotales()
        {

            float[] anchoColumasTablaTotales = { 420f, 180f };
            PdfPTable tablaTotales = new PdfPTable(anchoColumasTablaTotales);
            tablaTotales.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaTotales.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaTotales.SetTotalWidth(anchoColumasTablaTotales);
            tablaTotales.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaTotales.LockedWidth = true;

            float[] anchoColumnas = { 110, 70f };
            PdfPTable tablaImportes = new PdfPTable(anchoColumnas);
            tablaImportes.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaImportes.SetTotalWidth(anchoColumnas);
            tablaImportes.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            //tablaImportes.HorizontalAlignment = Element.ALIGN_RIGHT;
            tablaImportes.LockedWidth = true;
            if (_templatePDF.descuento > 0.00f)
            {
                float subtotal = _templatePDF.subtotal + _templatePDF.descuento;
                tablaImportes.AddCell(new Phrase("SUBTOTAL ANTES DE DESC.:", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(subtotal.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));

                tablaImportes.AddCell(new Phrase("Descuento:".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(_templatePDF.descuento.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
            }

            tablaImportes.AddCell(new Phrase("SUBTOTAL:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaImportes.AddCell(new Phrase(_templatePDF.subtotal.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));

            foreach (ImpuestoRe i in _templatePDF.impuestos)
            {
                tablaImportes.AddCell(new Phrase(i.impuesto + " " + i.tasa.ToString("F2") + "%:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(i.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
            }

            foreach (RetencionRe i in _templatePDF.retenciones)
            {
                tablaImportes.AddCell(new Phrase("Retencion " + i.impuesto + ": ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(i.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
            }

            tablaImportes.AddCell(new Phrase("Total:".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaImportes.AddCell(new Phrase(_templatePDF.total.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));

            tablaTotales.AddCell(new Phrase("IMPORTE CON LETRA: " + _templatePDF.totalEnLetra.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            tablaTotales.AddCell(tablaImportes);
            _documento.Add(tablaTotales);
        }

        private void AgregarSellos()
        {


            float[] anchoColumnas = { 500f, 100f };
            PdfPTable tablaSellosQR = new PdfPTable(anchoColumnas);
            tablaSellosQR.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaSellosQR.SpacingBefore = 10.0f;
            tablaSellosQR.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaSellosQR.SetTotalWidth(anchoColumnas);
            //tablaSellosQR.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaSellosQR.LockedWidth = true;

            float[] anchoColumnas1 = { 500f };
            PdfPTable tablaSellos = new PdfPTable(anchoColumnas1);
            tablaSellos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaSellos.DefaultCell.VerticalAlignment = Element.ALIGN_TOP;
            tablaSellos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaSellos.SetTotalWidth(anchoColumnas1);
            tablaSellos.HorizontalAlignment = Element.ALIGN_CENTER;
            //tablaSellos.LockedWidth = true;

            //Agregamos el codigo QR al documento


            _documento.Add(tablaSellosQR);
        }



        public string uso()
        {

            return DecodificadorSAT.getUso(_templatePDF.UsodelCFDI);

        }


        private void ObtenerLetras()
        {


            Numalet numaLet = new Numalet();
            numaLet.MascaraSalidaDecimal = "00/100 M.N.";
            if (_templatePDF.claveMoneda == "Peso Mexicano")
            {
                numaLet.SeparadorDecimalSalida = "pesos";
            }
            if (_templatePDF.claveMoneda == "Dolar americano")
            {
                numaLet.SeparadorDecimalSalida = "dolares";
                numaLet.MascaraSalidaDecimal = "00/100";
            }
            if (_templatePDF.claveMoneda.ToUpper() == "EURO")
            {
                numaLet.SeparadorDecimalSalida = "Euros";
                numaLet.MascaraSalidaDecimal = "00/100";
            }
            _templatePDF.totalEnLetra = numaLet.ToCustomString(double.Parse(_templatePDF.total.ToString()));


        }
        #endregion

    }


    public class LineadeproductosRe
    {

        public LineadeproductosRe() { }

        public string ClaveProducto { get; set; }
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; }
        public string Descripcion { get; set; }
        public decimal v_unitario { get; set; }
        public decimal importe { get; set; }
        public decimal descuento { get; set; }
        public string Presentacion { get; set; }

        public string Observacion { get; set; }
    }




    #region Extensión de la clase pdfPageEvenHelper
    public class ITextEventsRe : PdfPageEventHelper
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


            String text = "Página " + writer.PageNumber + " ";
            String TextRevision = "REV. 2";

            cb.BeginText();
            cb.SetFontAndSize(bf, 9);
            cb.SetTextMatrix(document.PageSize.GetRight(70), document.PageSize.GetBottom(30));
            //cb.MoveText(500,30);
            //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
            //cb.ShowText(text);
            cb.EndText();
            float len = bf.GetWidthPoint(text, 9);
            cb.AddTemplate(footerTemplate, document.PageSize.GetRight(70) + len, document.PageSize.GetBottom(30));

            float[] anchoColumasTablaTotales = { 480f, 120f };
            PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
            tabla.DefaultCell.Border = Rectangle.NO_BORDER;
            tabla.SetTotalWidth(anchoColumasTablaTotales);
            tabla.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.LockedWidth = true;
            tabla.AddCell(new Phrase("Este documento es una Remisión ", new Font(Font.FontFamily.HELVETICA, 8)));
            tabla.AddCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 8)));
            tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
            tabla.AddCell(new Phrase(TextRevision, new Font(Font.FontFamily.HELVETICA, 8)));
            tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(40), writer.DirectContent);

        }


        
        //}

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bf, 9);
            //footerTemplate.MoveText(550,30);
            footerTemplate.SetTextMatrix(0, 0);
            //footerTemplate.ShowText((writer.PageNumber - 1).ToString());
            footerTemplate.EndText();
        }
    }
    #endregion



}
