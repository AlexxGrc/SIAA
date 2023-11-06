using iTextSharp.text;
using iTextSharp.text.pdf;

using SIAAPI.clasescfdi;
using SIAAPI.ClasesProduccion;
using SIAAPI.Models.Cfdi;
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


    public class ProductoRutaOCF
    {

        public string cantidad = string.Empty;
        public string descripcion = string.Empty;
        public string unidad = string.Empty;
        public float valorUnitario = 0.00f;
        public float importe = 0.00f;
        public string numIdentificacion = string.Empty;
        public int iddetordencompra { get; set; }

        public decimal suministro { get; set; }

        public string Presentacion = string.Empty;

        public string Observacion = string.Empty;
        public string ClaveProducto { get; internal set; }
        public string id { get; internal set; }
        public string c_unidad { get; internal set; }
        public string desc { get; internal set; }
        public float v_unitario = 0.00f;
        public float descuento { get; internal set; }

        public string Tipo = "Articulo";


    }

    public class ImpuestoRutaOCF
    {
        public string impuesto;
        public float tasa;
        public float importe;
    }

    public class RetencionRutaOCF
    {
        public string impuesto;
        public float importe;
    }

    public class DocumentoRutaOrdenCompraF
    {
        public string serie = string.Empty;
        public string folio = string.Empty;

        public string fecha = string.Empty;

        public string fechaRevision = string.Empty;
        public string fechaVencimiento = string.Empty;
        public decimal dias = 0;
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


        public string Proveedor = string.Empty;
        public string RFCproveedor = string.Empty;
        public string Telefonoproveedor = string.Empty;

        public string Observacion = string.Empty;

        public string cadenaOriginal = string.Empty;

        public float subtotal = 0.00f;
        public float total = 0.00f;
        public float descuento = 0.00f;

        public int IDALmacen = 0;

        public List<ProductoRutaOCF> productos = new List<ProductoRutaOCF>();
        public List<ImpuestoRutaOCF> impuestos = new List<ImpuestoRutaOCF>();
        public List<RetencionRutaOCF> retenciones = new List<RetencionRutaOCF>();

        public string totalEnLetra = string.Empty;
        public float totalImpuestosRetenidos = 0.00f;


        public string tipo_cambio { get; internal set; }

        public string UsodelCFDI { get; set; }
        public string DireccionProveedor { get; set; }

        public string Entregaren { get; set; }
        public string Autorizado = "Inactivo";
    }

    public class CreaRutaOrdenCompraPDFF
    {
        MemoryStream _memoryStream = new MemoryStream();
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;
        public CMYKColor colordefinido;
        public DocumentoRutaOrdenCompraF _templatePDF; //Objeto que contendra la información del documento pdf



        public CreaRutaOrdenCompraPDFF(System.Drawing.Image logo, DocumentoRutaOrdenCompraF Encorden)
        {
            _templatePDF = Encorden;
            ObtenerLetras();
            ClsColoresReporte colorr = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            colordefinido = colorr.color;
            //Trabajos con el documento XML
            _documento = new Document(PageSize.LETTER);
            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            //string nombreDocumento = rutaPDF;

            //Creamos el documento
            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEventsRutaOCF(); // invoca la clase que esta mas abajo correspondiente al pie de pagina

            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();
            _cb = _writer.DirectContentUnder;

            AgregarLogo(logo);
            AgregarCuadro();
            AgregarDatosEmisor("");
            AgregarDatosFactura();
          
                AgregarDatosReceptor();

            AgregarDatosF();
            //AgregarTotales();

            this.AgregarRecepciones();

            //     AgregarSellos();

            //Cerramoe el documento
            _documento.Close();

            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"" + _templatePDF.Proveedor + _templatePDF.serie + _templatePDF.folio + ".pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

        }

        private void AgregarRecepciones()
        {
            float[] anchoColumasTablapie = { 600f };
            PdfPTable tablapie = new PdfPTable(anchoColumasTablapie);
            tablapie.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            //tablapie.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapie.SetTotalWidth(anchoColumasTablapie);
            tablapie.HorizontalAlignment = Element.ALIGN_CENTER;
            tablapie.LockedWidth = true;

            tablapie.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            tablapie.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
            //tablapie.AddCell(new Phrase("" + _templatePDF.condicionesdepago, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablapie.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));


        }



        #region Leer datos del .xml





        #endregion

        #region Escribir datos en el .pdf

        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {
            if (logoEmpresa == null)
                return;
            try
            {
                Image jpg = Image.GetInstance(logoEmpresa, BaseColor.WHITE);

                Paragraph paragraph = new Paragraph();
                paragraph.Alignment = Element.ALIGN_RIGHT;
                jpg.ScaleToFit(140f, 140F); //ancho y largo de la imagen
                jpg.Alignment = Image.ALIGN_RIGHT;
                jpg.SetAbsolutePosition(30f, 680f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera
                _documento.Add(jpg);
                //  doc.Add(paragraph);
            }
            catch (Exception err)
            {

            }
        }
        private void AgregarCuadro()
        {
            _cb = _writer.DirectContentUnder;

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
            p1.IndentationLeft = 155f;
            p1.IndentationRight = 150f;
            p1.SpacingBefore = 30f;
            p1.Leading = 9;
            p1.Alignment = Element.ALIGN_CENTER;
            p1.Add(new Phrase(_templatePDF.Empresa, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.RFC, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.Telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.Add("\n");
            p1.Add(new Phrase("" + _templatePDF.Direccion, new Font(Font.FontFamily.HELVETICA, 8)));

            p1.Add("\n");
            p1.Add("\n");
            p1.Add(new Phrase(SIAAPI.Properties.Settings.Default.paginaoficial, new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
            p1.Add("\n");
            p1.Add(new Phrase(SIAAPI.Properties.Settings.Default.Eslogan, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.SpacingAfter = -80;

            _documento.Add(p1);
        }

        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("Ruta");
            _documento.AddSubject("Ruta Orden de Compra");
            _documento.AddTitle("Ruta de Orden de Compra");
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

            p2.Add(new Phrase("HISTORIA DE FACTURA NÚM: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add(new Phrase("\n" + _templatePDF.folio, new Font(Font.FontFamily.HELVETICA, 16)));




            p2.Add(new Phrase("\nFECHA " + DateTime.Now, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));

            p2.Add("\n");
            p2.Add(new Phrase("\n \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));

            p2.Add(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 8)));

            p2.SpacingAfter = 20;
            _documento.Add(p2);
        }
       private void DatosFact()
        {
            float[] anchoColumnasTablaDatos = { 600f };
            PdfPTable tablaDatosPrincipal = new PdfPTable(anchoColumnasTablaDatos);
            tablaDatosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.SpacingBefore = 5;
            tablaDatosPrincipal.SetTotalWidth(anchoColumnasTablaDatos);
            tablaDatosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosPrincipal.LockedWidth = true;

            //Datos del receptor
            float[] anchoColumnaspro = { 150f,150f,150f,150f};
            PdfPTable tablapro = new PdfPTable(anchoColumnaspro);
            tablapro.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapro.SetTotalWidth(anchoColumnaspro);
            tablapro.HorizontalAlignment = Element.ALIGN_LEFT;
            tablapro.LockedWidth = true;

            float[] anchoColumnas = { 150f, 150f, 150f, 150f};
            PdfPTable tabla1 = new PdfPTable(anchoColumnas);
            tabla1.DefaultCell.Border = Rectangle.BOX;
            tabla1.SetTotalWidth(anchoColumnas);
            tabla1.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla1.LockedWidth = true;




            PdfPCell celdaMO = new PdfPCell(new Phrase("FECHA FACTURA ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            //PdfPCell celdaCR = new PdfPCell(new Phrase("CONDICIONES".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            //PdfPCell celdaUS = new PdfPCell(new Phrase("USO DE CFDI".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaFO = new PdfPCell(new Phrase("FECHA REVISIÓN".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaME = new PdfPCell(new Phrase("FECHA VENCIMIENTO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaTI = new PdfPCell(new Phrase("DÍAS CRÉDITO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            // PdfPCell celdaAL = new PdfPCell(new Phrase("ALMACEN".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));





            celdaMO.BackgroundColor = colordefinido;
            //celdaUS.BackgroundColor = colordefinido;
            //celdaCR.BackgroundColor = colordefinido;
            celdaFO.BackgroundColor = colordefinido;
            celdaME.BackgroundColor = colordefinido;
            celdaTI.BackgroundColor = colordefinido;
            // celdaAL.BackgroundColor = colordefinido;

            tabla1.AddCell(celdaMO);
            //tabla1.AddCell(celdaCR);
            //tabla1.AddCell(celdaUS);
            tabla1.AddCell(celdaFO);
            tabla1.AddCell(celdaME);
            tabla1.AddCell(celdaTI);
            //  tabla1.AddCell(celdaAL);





            tabla1.CompleteRow();


            tabla1.AddCell(new Phrase(_templatePDF.fecha, new Font(Font.FontFamily.HELVETICA, 6)));
           
            tabla1.AddCell(new Phrase(_templatePDF.fechaRevision, new Font(Font.FontFamily.HELVETICA, 6)));

            tabla1.AddCell(new Phrase(_templatePDF.fechaVencimiento, new Font(Font.FontFamily.HELVETICA, 6)));


             PdfPCell celdatc = new PdfPCell(new Phrase(_templatePDF.dias.ToString(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            //celdatc.HorizontalAlignment = Element.ALIGN_CENTER;



            tabla1.AddCell(celdatc);

            // tabla1.AddCell(new Phrase(_templatePDF.IDALmacen.ToUpper(), new Font(Font.FontFamily.HELVETICA, 6)));

            tabla1.CompleteRow();

            tablaDatosPrincipal.AddCell(tabla1);
            //
            _documento.Add(tablaDatosPrincipal);
        }
        private void AgregarDatosReceptor()
        {
            float[] anchoColumnasTablaDatos = { 600f };
            PdfPTable tablaDatosPrincipal = new PdfPTable(anchoColumnasTablaDatos);
            tablaDatosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaDatosPrincipal.SpacingBefore = 5;
            tablaDatosPrincipal.SetTotalWidth(anchoColumnasTablaDatos);
            tablaDatosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaDatosPrincipal.LockedWidth = true;

            //Datos del receptor
            float[] anchoColumnaspro = { 80f, 220F, 20f, 80f, 200f };
            PdfPTable tablapro = new PdfPTable(anchoColumnaspro);
            tablapro.DefaultCell.Border = Rectangle.NO_BORDER;
            tablapro.SetTotalWidth(anchoColumnaspro);
            tablapro.HorizontalAlignment = Element.ALIGN_LEFT;
            tablapro.LockedWidth = true;

            Almacen almacen = new AlmacenContext().Almacenes.Find(_templatePDF.IDALmacen);
            ProveedorContext db = new ProveedorContext();
            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select  [IDProveedor] as Dato from [dbo].[Proveedores] where [Empresa] ='" + _templatePDF.Proveedor + "'").ToList()[0];
            int p = c.Dato;



            Proveedor provee = new ProveedorContext().Proveedores.Find(p);



            PdfPCell celda0 = new PdfPCell(new Phrase("PROVEEDOR: ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            PdfPCell celda1 = new PdfPCell(new Phrase(_templatePDF.Proveedor.ToUpper() + "\n DIRECCIÓN: " + provee.Calle + " " + provee.NoExt + " COL. " + provee.Colonia + ", " + provee.Municipio + " TEL." + provee.Telefonouno, new Font(Font.FontFamily.HELVETICA, 8)));
            PdfPCell celda2 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            PdfPCell celda3 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            if (_templatePDF.Entregaren == null)
            {
                _templatePDF.Entregaren = "Direccion Fiscal";
            }

            PdfPCell celda4 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));




            celda0.Border = Rectangle.NO_BORDER;
            celda1.Border = Rectangle.NO_BORDER;
            celda2.Border = Rectangle.NO_BORDER;
            celda3.Border = Rectangle.NO_BORDER;
            celda4.Border = Rectangle.NO_BORDER;

            tablapro.AddCell(celda0);
            tablapro.AddCell(celda1);
            tablapro.AddCell(celda2);
            tablapro.AddCell(celda3);
            tablapro.AddCell(celda4);

            tablapro.CompleteRow();
            _documento.Add(tablapro);


            DatosFact();

            float[] anchoColumnas = { 90f, 130f, 240f, 130f };
            PdfPTable tabla1 = new PdfPTable(anchoColumnas);
            tabla1.DefaultCell.Border = Rectangle.BOX;
            tabla1.SetTotalWidth(anchoColumnas);
            tabla1.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla1.LockedWidth = true;




            PdfPCell celdaMO = new PdfPCell(new Phrase("MONEDA ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            //PdfPCell celdaCR = new PdfPCell(new Phrase("CONDICIONES".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            //PdfPCell celdaUS = new PdfPCell(new Phrase("USO DE CFDI".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaFO = new PdfPCell(new Phrase("FORMA DE PAGO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaME = new PdfPCell(new Phrase("METODO DE PAGO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celdaTI = new PdfPCell(new Phrase("TIPO DE CAMBIO".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            // PdfPCell celdaAL = new PdfPCell(new Phrase("ALMACEN".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));





            celdaMO.BackgroundColor = colordefinido;
            //celdaUS.BackgroundColor = colordefinido;
            //celdaCR.BackgroundColor = colordefinido;
            celdaFO.BackgroundColor = colordefinido;
            celdaME.BackgroundColor = colordefinido;
            celdaTI.BackgroundColor = colordefinido;
            // celdaAL.BackgroundColor = colordefinido;

            tabla1.AddCell(celdaMO);
            //tabla1.AddCell(celdaCR);
            //tabla1.AddCell(celdaUS);
            tabla1.AddCell(celdaFO);
            tabla1.AddCell(celdaME);
            tabla1.AddCell(celdaTI);
            //  tabla1.AddCell(celdaAL);





            tabla1.CompleteRow();


            tabla1.AddCell(new Phrase(_templatePDF.claveMoneda, new Font(Font.FontFamily.HELVETICA, 6)));
            //tabla1.AddCell(new Phrase(_templatePDF.condicionesdepago, new Font(Font.FontFamily.HELVETICA, 6)));
            //tabla1.AddCell(new Phrase(_templatePDF.UsodelCFDI, new Font(Font.FontFamily.HELVETICA, 6)));

            string f_pago = DecodificadorSAT.getFormapago(_templatePDF.formaPago);


            tabla1.AddCell(new Phrase(f_pago.ToUpper(), new Font(Font.FontFamily.HELVETICA, 6)));



            string m_pago = DecodificadorSAT.getMetodo(_templatePDF.metodoPago);

            tabla1.AddCell(new Phrase(m_pago.ToUpper(), new Font(Font.FontFamily.HELVETICA, 6)));


            ///cambio de renglon
            ///
            //PdfPCell celdatc = new PdfPCell(new Phrase(_templatePDF.tipo_cambio, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            string tipodecambio = new ArticuloContext().Database.SqlQuery<ClsDatoDecimal>(" select dbo.GetTipocambioCadena(GETDATE(),'USD','MXN') as Dato").ToList().FirstOrDefault().Dato.ToString();
            PdfPCell celdatc = new PdfPCell(new Phrase(tipodecambio, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

            //celdatc.HorizontalAlignment = Element.ALIGN_CENTER;



            tabla1.AddCell(celdatc);

            // tabla1.AddCell(new Phrase(_templatePDF.IDALmacen.ToUpper(), new Font(Font.FontFamily.HELVETICA, 6)));

            tabla1.CompleteRow();

            tablaDatosPrincipal.AddCell(tabla1);
            //
            _documento.Add(tablaDatosPrincipal);




        }

        private string ReturnClavePago(string clave)
        {
            string valor = "";

            valor = clave;

            return valor.ToString().ToUpper();
        }


        private void AgregarDatosF()
        {


            Paragraph pRe = new Paragraph();
            //pR.IndentationLeft = 403f;
            pRe.SpacingAfter = 18;
            pRe.Leading = 10;
            pRe.Alignment = Element.ALIGN_CENTER;

            pRe.Add(new Phrase("\n FACTURAS: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            pRe.Add(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 16)));



            List<int> Factura = new EncfacturaProvContext().Database.SqlQuery<int>("select distinct(id) from  EncFacturaProv where ID= " + _templatePDF.folio).ToList();


            float[] tamanoColumnasrecep = { 100f, 75f, 190f, 75f, 75f, 75f };
            PdfPTable tablarecep = new PdfPTable(tamanoColumnasrecep);
            //tablaProductos.SpacingBefore = 1;
            //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablarecep.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablarecep.DefaultCell.BorderWidthLeft = 0.1f;
            tablarecep.DefaultCell.BorderWidthRight = 0.1f;
            tablarecep.DefaultCell.BorderWidthBottom = 0.2f;
            tablarecep.DefaultCell.BorderWidthTop = 0.1f;
            tablarecep.SetTotalWidth(tamanoColumnasrecep);
            tablarecep.HorizontalAlignment = Element.ALIGN_LEFT;
            tablarecep.LockedWidth = true;


            

            tablarecep.AddCell(new Phrase("SERIE", new Font(Font.FontFamily.HELVETICA, 6)));


            tablarecep.AddCell(new Phrase("NUMERO ", new Font(Font.FontFamily.HELVETICA, 6)));

            tablarecep.AddCell(new Phrase("PROVEEDOR", new Font(Font.FontFamily.HELVETICA, 6)));

            tablarecep.AddCell(new Phrase("SUBTOTAL", new Font(Font.FontFamily.HELVETICA, 6)));

            tablarecep.AddCell(new Phrase("IVA", new Font(Font.FontFamily.HELVETICA, 6)));
            tablarecep.AddCell(new Phrase("TOTAL", new Font(Font.FontFamily.HELVETICA, 6)));


            foreach (int recep in Factura)
            {
                EncfacturaProv recepcion = new EncfacturaProvContext().EncfacturaProveedores.Find(recep);
                PdfPCell celdafolio = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_CENTER, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.1f };
                celdafolio.Phrase = new Phrase(recepcion.Serie, new Font(Font.FontFamily.HELVETICA, 8));

                tablarecep.AddCell(celdafolio);

                //    tablarecep.AddCell(new Phrase(recepcion.Observacion, new Font(Font.FontFamily.HELVETICA, 6)));

                tablarecep.AddCell(new Phrase(recepcion.Numero.ToString(), new Font(Font.FontFamily.HELVETICA, 6)));


                tablarecep.AddCell(new Phrase(recepcion.Nombre_Proveedor, new Font(Font.FontFamily.HELVETICA, 6)));


                
                PdfPCell celdadife = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.1f, BorderWidthBottom = 0.1f };
                celdadife.Phrase = new Phrase(recepcion.Subtotal.ToString(), new Font(Font.FontFamily.HELVETICA, 8));




                tablarecep.AddCell(celdadife);


                tablarecep.AddCell(new Phrase(recepcion.IVA.ToString(), new Font(Font.FontFamily.HELVETICA, 6)));
               
                    tablarecep.AddCell(new Phrase(recepcion.Total.ToString(), new Font(Font.FontFamily.HELVETICA, 6)));
               


            }

            _documento.Add(pRe);

            _documento.Add(tablarecep);
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///


            Paragraph pRec = new Paragraph();
            //pR.IndentationLeft = 403f;
            pRec.SpacingAfter = 18;
            pRec.Leading = 10;
            pRec.Alignment = Element.ALIGN_CENTER;

            pRec.Add(new Phrase("\nRECEPCIONES: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            pRec.Add(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 16)));



            List<EncRecepcion> recepciones = new RecepcionContext().Database.SqlQuery<EncRecepcion>("select * from  EncRecepcion where documentoFactura= '" + _templatePDF.folio + "'").ToList();


            float[] tamanoColumnasrecepc = { 100f, 75f, 190f, 75f, 75f, 75f };
            PdfPTable tablarecepc = new PdfPTable(tamanoColumnasrecepc);
            //tablaProductos.SpacingBefore = 1;
            //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablarecepc.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablarecepc.DefaultCell.BorderWidthLeft = 0.1f;
            tablarecepc.DefaultCell.BorderWidthRight = 0.1f;
            tablarecepc.DefaultCell.BorderWidthBottom = 0.2f;
            tablarecepc.DefaultCell.BorderWidthTop = 0.1f;
            tablarecepc.SetTotalWidth(tamanoColumnasrecep);
            tablarecepc.HorizontalAlignment = Element.ALIGN_LEFT;
            tablarecepc.LockedWidth = true;


            tablarecepc.AddCell(new Phrase("RECEPCION", new Font(Font.FontFamily.HELVETICA, 6)));



            tablarecepc.AddCell(new Phrase("REALIZO", new Font(Font.FontFamily.HELVETICA, 6)));


            tablarecepc.AddCell(new Phrase("FECHA ", new Font(Font.FontFamily.HELVETICA, 6)));

            tablarecepc.AddCell(new Phrase("DIAS DIFERENCIA O/R", new Font(Font.FontFamily.HELVETICA, 6)));

            tablarecepc.AddCell(new Phrase("TICKET", new Font(Font.FontFamily.HELVETICA, 6)));

            tablarecepc.AddCell(new Phrase("FACTURA", new Font(Font.FontFamily.HELVETICA, 6)));
            int idordencomra = 0;
            int IDRecepcionFa = 0;
            foreach (var recep in recepciones)
            {
                EncfacturaProv fact = new EncfacturaProvContext().EncfacturaProveedores.Where(s => s.ID.ToString() ==  recep.DocumentoFactura).ToList().FirstOrDefault();
               
                EncRecepcion recepcion = new RecepcionContext().EncRecepciones.Find(recep.IDRecepcion);
                PdfPCell celdafolio = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_CENTER, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.1f };
                celdafolio.Phrase = new Phrase(recepcion.IDRecepcion.ToString(), new Font(Font.FontFamily.HELVETICA, 8));

                tablarecepc.AddCell(celdafolio);

                //    tablarecep.AddCell(new Phrase(recepcion.Observacion, new Font(Font.FontFamily.HELVETICA, 6)));

                tablarecepc.AddCell(new Phrase(recepcion.User.Username.ToString(), new Font(Font.FontFamily.HELVETICA, 6)));


                tablarecepc.AddCell(new Phrase(recepcion.Fecha.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 6)));


                 TimeSpan ts = DateTime.Parse(_templatePDF.fecha) - fact.Fecha ;

                // Difference in days.
                int differenceInDays = ts.Days;


                PdfPCell celdadife = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.1f, BorderWidthBottom = 0.1f };
                celdadife.Phrase = new Phrase(differenceInDays.ToString(), new Font(Font.FontFamily.HELVETICA, 8));




                tablarecepc.AddCell(celdadife);


                tablarecepc.AddCell(new Phrase(recepcion.DocumentoFactura.ToString(), new Font(Font.FontFamily.HELVETICA, 6)));
                try
                {
                    EncfacturaProv factura = new ProveedorFacturasContext().Factura.Where(s => s.ID.ToString() == recepcion.DocumentoFactura).ToList().FirstOrDefault();
                    if (factura == null)
                    {
                        throw new Exception("No hay factura");
                    }
                    tablarecepc.AddCell(new Phrase(factura.Serie + factura.Numero, new Font(Font.FontFamily.HELVETICA, 6)));
                }
                catch (Exception err)
                {
                    tablarecepc.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));
                }

                IDRecepcionFa = recepcion.IDRecepcion;
            }

            _documento.Add(pRec);

            _documento.Add(tablarecepc);


            ///////////////////////////////////////////////////////////////////////


            Paragraph pOC = new Paragraph();
            //pR.IndentationLeft = 403f;
            pOC.SpacingAfter = 18;
            pOC.Leading = 10;
            pOC.Alignment = Element.ALIGN_CENTER;

            pOC.Add(new Phrase("\nORDEN COMPRA: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            pOC.Add(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 16)));

            

            List<EncOrdenCompra> ordenCompras = new OrdenCompraContext().Database.SqlQuery<EncOrdenCompra>("select distinct(o.idordencompra), o.*  from  EncOrdenCompra as o inner join detrecepcion as r on o.idordencompra=r.idexterna inner join Encrecepcion as er on r.idrecepcion=er.idrecepcion where er.documentofactura='" + _templatePDF.folio + "'").ToList();


            float[] tamanoColumnasrecepcO = { 100f, 75f, 190f, 75f, 75f, 75f };
            PdfPTable tablarecepcO = new PdfPTable(tamanoColumnasrecepc);
            //tablaProductos.SpacingBefore = 1;
            //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablarecepcO.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablarecepcO.DefaultCell.BorderWidthLeft = 0.1f;
            tablarecepcO.DefaultCell.BorderWidthRight = 0.1f;
            tablarecepcO.DefaultCell.BorderWidthBottom = 0.2f;
            tablarecepcO.DefaultCell.BorderWidthTop = 0.1f;
            tablarecepcO.SetTotalWidth(tamanoColumnasrecep);
            tablarecepcO.HorizontalAlignment = Element.ALIGN_LEFT;
            tablarecepcO.LockedWidth = true;


            tablarecepcO.AddCell(new Phrase("OCOMPRA", new Font(Font.FontFamily.HELVETICA, 6)));



            tablarecepcO.AddCell(new Phrase("REALIZO", new Font(Font.FontFamily.HELVETICA, 6)));


            tablarecepcO.AddCell(new Phrase("FECHA ", new Font(Font.FontFamily.HELVETICA, 6)));

            tablarecepcO.AddCell(new Phrase("DIAS DIFERENCIA O/R", new Font(Font.FontFamily.HELVETICA, 6)));

            tablarecepcO.AddCell(new Phrase("TICKET", new Font(Font.FontFamily.HELVETICA, 6)));

            tablarecepcO.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));


            foreach (var recep in ordenCompras)
            {
                int idre = 0;
                EncOrdenCompra OC = new OrdenCompraContext().EncOrdenCompras.Find(recep.IDOrdenCompra);
                List<DetRecepcion> detalles = new PrefacturaContext().Database.SqlQuery<DetRecepcion>("Select * from detRecepcion where IDExterna=" + recep.IDOrdenCompra + " and status!='Cancelado'").ToList();
                foreach (DetRecepcion detalle in detalles)
                {

                    if (detalle.IDExterna > 0)
                    {
                        idre = detalle.IDRecepcion;
                    }
                }
                        EncRecepcion recepcion = new RecepcionContext().EncRecepciones.Find(idre);
                PdfPCell celdafolio = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_CENTER, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.1f };
                celdafolio.Phrase = new Phrase(recep.IDOrdenCompra.ToString(), new Font(Font.FontFamily.HELVETICA, 8));

                tablarecepcO.AddCell(celdafolio);

                //    tablarecep.AddCell(new Phrase(recepcion.Observacion, new Font(Font.FontFamily.HELVETICA, 6)));

                tablarecepcO.AddCell(new Phrase(OC.User.Username.ToString(), new Font(Font.FontFamily.HELVETICA, 6)));


                tablarecepcO.AddCell(new Phrase(OC.Fecha.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 6)));


                TimeSpan ts = DateTime.Parse(_templatePDF.fecha)- recep.Fecha ;

                // Difference in days.
                int differenceInDays = ts.Days;


                PdfPCell celdadife = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.1f, BorderWidthBottom = 0.1f };
                celdadife.Phrase = new Phrase(differenceInDays.ToString(), new Font(Font.FontFamily.HELVETICA, 8));




                tablarecepcO.AddCell(celdadife);


                tablarecepcO.AddCell(new Phrase(recepcion.DocumentoFactura.ToString(), new Font(Font.FontFamily.HELVETICA, 6)));
                try
                {
                    EncfacturaProv factura = new ProveedorFacturasContext().Factura.Find(recepcion.DocumentoFactura);
                    if (factura == null)
                    {
                        throw new Exception("No hay factura");
                    }
                    tablarecepcO.AddCell(new Phrase(factura.Serie + factura.Numero, new Font(Font.FontFamily.HELVETICA, 6)));
                }
                catch (Exception err)
                {
                    tablarecepcO.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));
                }


            }

            _documento.Add(pOC);

            _documento.Add(tablarecepcO);


            float[] anchoColumnasTablaProductos = { 590f };
            PdfPTable tablaProductosPrincipal = new PdfPTable(anchoColumnasTablaProductos);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosPrincipal.SetTotalWidth(anchoColumnasTablaProductos);
            tablaProductosPrincipal.SpacingBefore = 15;
            tablaProductosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosPrincipal.LockedWidth = true;


            //Datos de los productos
            float[] tamanoColumnas = { 60f, 60f, 70f, 200f, 70f, 60f, 70f };
            PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);

            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;



            PdfPCell celda0 = new PdfPCell(new Phrase("CLAVE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));

            PdfPCell celda1 = new PdfPCell(new Phrase("CANTIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda2 = new PdfPCell(new Phrase("RECEPCIONADO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda3 = new PdfPCell(new Phrase("DESCRIPCION", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda4 = new PdfPCell(new Phrase("VALOR UNITARIO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda5 = new PdfPCell(new Phrase("IMPORTE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda6 = new PdfPCell(new Phrase("REQUISICIONES", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));



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

            var result2 = from line in _templatePDF.productos
                          group line by new { line.descripcion, line.Presentacion, line.Observacion } into csLine
                          select new LineadeRutaproductosF
                          {
                              ClaveProducto = csLine.First().ClaveProducto,
                              Cantidad = csLine.Sum(c => decimal.Parse(c.cantidad)),
                              suministro = decimal.Parse(csLine.First().suministro.ToString()),
                              Unidad = csLine.First().c_unidad,
                              Descripcion = csLine.First().descripcion,
                              v_unitario = decimal.Parse(csLine.First().v_unitario.ToString()),
                              importe = csLine.Sum(c => Math.Round(decimal.Parse(c.importe.ToString()), 2)),
                              descuento = decimal.Parse(csLine.First().descuento.ToString()),
                              Presentacion = csLine.First().Presentacion,
                              Observacion = csLine.First().Observacion,
                              TipoArticulo = csLine.First().Tipo,
                              iddetordencompra = csLine.First().iddetordencompra
                          };

            PdfPCell celdaTitulos = new PdfPCell(tablaProductosTitulos);
            tablaProductosPrincipal.AddCell(celdaTitulos);
            float[] tamanoColumnasrequi = { 100f, 75f, 190f, 75f, 75f, 75f };
            PdfPTable tablarequi = new PdfPTable(tamanoColumnasrequi);
            Paragraph pR = new Paragraph();
            //pR.IndentationLeft = 403f;
            pR.SpacingAfter = 18;
            pR.Leading = 10;
            pR.Alignment = Element.ALIGN_CENTER;


            foreach (LineadeRutaproductosF p in result2)
            {

                float[] tamanoColumnasProductos = { 60f, 60f, 70f, 200f, 70f, 60f, 70f };
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


                tablaProductos.AddCell(new Phrase(p.ClaveProducto, new Font(Font.FontFamily.HELVETICA, 6)));

                tablaProductos.AddCell(new Phrase(p.Cantidad.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.suministro.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.Descripcion, new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdauni = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdauni.Phrase = new Phrase(p.v_unitario.ToString("#,#0.0000"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdauni);
                PdfPCell celdaimporte = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdaimporte.Phrase = new Phrase(p.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdaimporte);
                PdfPCell celdarequi = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_MIDDLE, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                //  
                List<DetRequisiciones> detalles = new PrefacturaContext().Database.SqlQuery<DetRequisiciones>("Select * from detrequisiciones as d inner join elementosOrdenCompra as e on d.iddetrequisiciones=e.iddetdocumento where e.iddetordencompra=" + p.iddetordencompra + " and status!='Cancelado'").ToList();

                string requis = "";
                string NR = "N/A";
                foreach (var nodo in detalles)
                {
                    string nodoR = "";

                    nodoR = nodo.IDRequisicion.ToString();

                    requis += nodoR + "\n";

                }
                if (requis != "")
                {
                    NR = requis;
                }
                celdarequi.Phrase = new Phrase(requis, new Font(Font.FontFamily.HELVETICA, 8));

                tablaProductos.AddCell(celdarequi);
                // AQUI CAMBIA DE RENGLON 
                //RFC PROVEEDOR
                ProveedorContext db = new ProveedorContext();
                ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select  [IDProveedor] as Dato from [dbo].[Proveedores] where [Empresa] ='" + _templatePDF.Proveedor + "'").ToList()[0];
                int p1 = c.Dato;

                string id = "select idarticulo as Dato from Articulo where cref='" + p.ClaveProducto + "'";
                ClsDatoEntero IDArticulo = db.Database.SqlQuery<ClsDatoEntero>(id).ToList().FirstOrDefault();

                string MATRIZ = "select cref as Dato from MatrizPrecioProv where idproveedor=" + p1 + " and idarticulo=" + IDArticulo.Dato;
                ClsDatoString cref = db.Database.SqlQuery<ClsDatoString>(MATRIZ).ToList().FirstOrDefault();


                if (cref != null)
                {


                    tablaProductos.AddCell(new Phrase(cref.Dato, new Font(Font.FontFamily.HELVETICA, 6)));
                }
                else
                {
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));

                }



                //tablaProductos.CompleteRow();


                PdfPCell celdaProductos = new PdfPCell(tablaProductos);
                celdaProductos.MinimumHeight = 20;
                tablaProductosPrincipal.AddCell(celdaProductos);

                 pR = new Paragraph();
                //pR.IndentationLeft = 403f;
                pR.SpacingAfter = 18;
                pR.Leading = 10;
                pR.Alignment = Element.ALIGN_CENTER;
                tablarequi = new PdfPTable(tamanoColumnasrequi);


                pR.Add(new Phrase("\nREQUISICIONES: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
                pR.Add(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 16)));




                List<int> Requisiciones2 = new EncReporteAVGContext().Database.SqlQuery<int>("select DISTINCT iddocumento from elementosOrdenCompra where iddetOrdenCompra=" + p.iddetordencompra).ToList();
               
                //tablaProductos.SpacingBefore = 1;
                //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
                tablarequi.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                tablarequi.DefaultCell.BorderWidthLeft = 0.1f;
                tablarequi.DefaultCell.BorderWidthRight = 0.1f;
                tablarequi.DefaultCell.BorderWidthBottom = 0.2f;
                tablarequi.DefaultCell.BorderWidthTop = 0.1f;
                tablarequi.SetTotalWidth(tamanoColumnasrequi);
                tablarequi.HorizontalAlignment = Element.ALIGN_LEFT;
                tablarequi.LockedWidth = true;


                tablarequi.AddCell(new Phrase("REQUISICION", new Font(Font.FontFamily.HELVETICA, 6)));

                tablarequi.AddCell(new Phrase("OBSERVACION", new Font(Font.FontFamily.HELVETICA, 6)));

                tablarequi.AddCell(new Phrase("REALIZO", new Font(Font.FontFamily.HELVETICA, 6)));


                tablarequi.AddCell(new Phrase("FECHA ", new Font(Font.FontFamily.HELVETICA, 6)));
                tablarequi.AddCell(new Phrase("FECHA REQUERIDA", new Font(Font.FontFamily.HELVETICA, 6)));
                tablarequi.AddCell(new Phrase("DIAS DIFERENCIA R/O", new Font(Font.FontFamily.HELVETICA, 6)));


                foreach (int elemento in Requisiciones2)
                {
                    EncRequisiciones requisicion = new RequisicionesContext().EncRequisicioness.Find(elemento);
                    PdfPCell celdafolio = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_CENTER, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.1f };
                    celdafolio.Phrase = new Phrase(requisicion.IDRequisicion.ToString(), new Font(Font.FontFamily.HELVETICA, 8));

                    tablarequi.AddCell(celdafolio);

                    tablarequi.AddCell(new Phrase(requisicion.Observacion, new Font(Font.FontFamily.HELVETICA, 6)));

                    tablarequi.AddCell(new Phrase(requisicion.User.Username.ToString(), new Font(Font.FontFamily.HELVETICA, 6)));


                    tablarequi.AddCell(new Phrase(requisicion.Fecha.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 6)));
                    tablarequi.AddCell(new Phrase(requisicion.FechaRequiere.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 6)));

                    TimeSpan ts = DateTime.Parse(_templatePDF.fecha) - requisicion.Fecha;

                    // Difference in days.
                    int differenceInDays = ts.Days;


                    PdfPCell celdadife = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.1f, BorderWidthBottom = 0.1f };
                    celdadife.Phrase = new Phrase(differenceInDays.ToString(), new Font(Font.FontFamily.HELVETICA, 8));




                    tablarequi.AddCell(celdadife);

                }
              
            }



            _documento.Add(tablaProductosPrincipal);
            _documento.Add(pR);
            _documento.Add(tablarequi);
            AgregarTotales();
           


        }

        private void AgregarDatosProductos()
        {


            Paragraph pRe = new Paragraph();
            //pR.IndentationLeft = 403f;
            pRe.SpacingAfter = 18;
            pRe.Leading = 10;
            pRe.Alignment = Element.ALIGN_CENTER;

            pRe.Add(new Phrase("\nRECEPCIONES: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            pRe.Add(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 16)));



            List<int> recepciones = new RecepcionContext().Database.SqlQuery<int>("select distinct(detrecepcion.IdRecepcion) from  detrecepcion inner join detordencompra on detordencompra.iddetordencompra=detrecepcion.iddetexterna where idordencompra= " + _templatePDF.folio).ToList();


            float[] tamanoColumnasrecep = { 100f, 75f, 75f, 75f, 75f, 75f };
            PdfPTable tablarecep = new PdfPTable(tamanoColumnasrecep);
            //tablaProductos.SpacingBefore = 1;
            //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablarecep.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablarecep.DefaultCell.BorderWidthLeft = 0.1f;
            tablarecep.DefaultCell.BorderWidthRight = 0.1f;
            tablarecep.DefaultCell.BorderWidthBottom = 0.2f;
            tablarecep.DefaultCell.BorderWidthTop = 0.1f;
            tablarecep.SetTotalWidth(tamanoColumnasrecep);
            tablarecep.HorizontalAlignment = Element.ALIGN_LEFT;
            tablarecep.LockedWidth = true;


            tablarecep.AddCell(new Phrase("RECEPCION", new Font(Font.FontFamily.HELVETICA, 6)));



            tablarecep.AddCell(new Phrase("REALIZO", new Font(Font.FontFamily.HELVETICA, 6)));


            tablarecep.AddCell(new Phrase("FECHA ", new Font(Font.FontFamily.HELVETICA, 6)));

            tablarecep.AddCell(new Phrase("DIAS DIFERENCIA O/R", new Font(Font.FontFamily.HELVETICA, 6)));

            tablarecep.AddCell(new Phrase("TICKET", new Font(Font.FontFamily.HELVETICA, 6)));

            tablarecep.AddCell(new Phrase("FACTURA", new Font(Font.FontFamily.HELVETICA, 6)));


            foreach (int recep in recepciones)
            {
                EncRecepcion recepcion = new RecepcionContext().EncRecepciones.Find(recep);
                PdfPCell celdafolio = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_CENTER, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.1f };
                celdafolio.Phrase = new Phrase(recepcion.IDRecepcion.ToString(), new Font(Font.FontFamily.HELVETICA, 8));

                tablarecep.AddCell(celdafolio);

                //    tablarecep.AddCell(new Phrase(recepcion.Observacion, new Font(Font.FontFamily.HELVETICA, 6)));

                tablarecep.AddCell(new Phrase(recepcion.User.Username.ToString(), new Font(Font.FontFamily.HELVETICA, 6)));


                tablarecep.AddCell(new Phrase(recepcion.Fecha.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 6)));


                TimeSpan ts = recepcion.Fecha - DateTime.Parse(_templatePDF.fecha);

                // Difference in days.
                int differenceInDays = ts.Days;


                PdfPCell celdadife = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.1f, BorderWidthBottom = 0.1f };
                celdadife.Phrase = new Phrase(differenceInDays.ToString(), new Font(Font.FontFamily.HELVETICA, 8));




                tablarecep.AddCell(celdadife);


                tablarecep.AddCell(new Phrase(recepcion.DocumentoFactura.ToString(), new Font(Font.FontFamily.HELVETICA, 6)));
                try
                {
                    EncfacturaProv factura = new ProveedorFacturasContext().Factura.Find(recepcion.DocumentoFactura);
                    if (factura == null)
                    {
                        throw new Exception("No hay factura");
                    }
                    tablarecep.AddCell(new Phrase(factura.Serie + factura.Numero, new Font(Font.FontFamily.HELVETICA, 6)));
                }
                catch (Exception err)
                {
                    tablarecep.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));
                }


            }

            _documento.Add(pRe);

            _documento.Add(tablarecep);


            Paragraph pOC = new Paragraph();
            //pR.IndentationLeft = 403f;
            pOC.SpacingAfter = 18;
            pOC.Leading = 10;
            pOC.Alignment = Element.ALIGN_CENTER;

            pOC.Add(new Phrase("\nORDEN COMPRA: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            pOC.Add(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 16)));

            _documento.Add(pOC);



            float[] anchoColumnasTablaProductos = { 600f };
            PdfPTable tablaProductosPrincipal = new PdfPTable(anchoColumnasTablaProductos);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosPrincipal.SetTotalWidth(anchoColumnasTablaProductos);
            tablaProductosPrincipal.SpacingBefore = 15;
            tablaProductosPrincipal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosPrincipal.LockedWidth = true;


            //Datos de los productos
            float[] tamanoColumnas = { 60f, 60f, 70f, 200f, 75f, 60f, 70f };
            PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnas);

            //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablaProductosTitulos.SetTotalWidth(tamanoColumnas);
            tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaProductosTitulos.LockedWidth = true;



            PdfPCell celda0 = new PdfPCell(new Phrase("CLAVE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));

            PdfPCell celda1 = new PdfPCell(new Phrase("CANTIDAD", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda2 = new PdfPCell(new Phrase("RECEPCIONADO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda3 = new PdfPCell(new Phrase("DESCRIPCION", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda4 = new PdfPCell(new Phrase("VALOR UNITARIO", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda5 = new PdfPCell(new Phrase("IMPORTE", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));
            PdfPCell celda6 = new PdfPCell(new Phrase("REQUISICIONES", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)));



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









            var result2 = from line in _templatePDF.productos
                          group line by new { line.descripcion, line.Presentacion, line.Observacion } into csLine
                          select new LineadeRutaproductosF
                          {
                              ClaveProducto = csLine.First().ClaveProducto,
                              Cantidad = csLine.Sum(c => decimal.Parse(c.cantidad)),
                              suministro = decimal.Parse(csLine.First().suministro.ToString()),
                              Unidad = csLine.First().c_unidad,
                              Descripcion = csLine.First().descripcion,
                              v_unitario = decimal.Parse(csLine.First().v_unitario.ToString()),
                              importe = csLine.Sum(c => Math.Round(decimal.Parse(c.importe.ToString()), 2)),
                              descuento = decimal.Parse(csLine.First().descuento.ToString()),
                              Presentacion = csLine.First().Presentacion,
                              Observacion = csLine.First().Observacion,
                              TipoArticulo = csLine.First().Tipo,
                              iddetordencompra = csLine.First().iddetordencompra
                          };

            PdfPCell celdaTitulos = new PdfPCell(tablaProductosTitulos);
            tablaProductosPrincipal.AddCell(celdaTitulos);


            foreach (LineadeRutaproductosF p in result2)
            {

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


                tablaProductos.AddCell(new Phrase(p.ClaveProducto, new Font(Font.FontFamily.HELVETICA, 6)));

                tablaProductos.AddCell(new Phrase(p.Cantidad.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.suministro.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.Descripcion, new Font(Font.FontFamily.HELVETICA, 8)));
                PdfPCell celdauni = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdauni.Phrase = new Phrase(p.v_unitario.ToString("#,#0.0000"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdauni);
                PdfPCell celdaimporte = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                celdaimporte.Phrase = new Phrase(p.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8));
                tablaProductos.AddCell(celdaimporte);
                PdfPCell celdarequi = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_MIDDLE, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.0f };
                //  
                String RequisicionesCadena = string.Empty;

                List<int> Requisiciones = new EncReporteAVGContext().Database.SqlQuery<int>("select iddocumento from elementosOrdenCompra where iddetOrdenCompra=" + p.iddetordencompra).ToList();

                foreach (int elemento in Requisiciones)
                {
                    RequisicionesCadena = elemento.ToString() + "\n";
                }
                celdarequi.Phrase = new Phrase(RequisicionesCadena, new Font(Font.FontFamily.HELVETICA, 8));

                tablaProductos.AddCell(RequisicionesCadena);
                // AQUI CAMBIA DE RENGLON 
                //RFC PROVEEDOR
                ProveedorContext db = new ProveedorContext();
                ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select  [IDProveedor] as Dato from [dbo].[Proveedores] where [Empresa] ='" + _templatePDF.Proveedor + "'").ToList()[0];
                int p1 = c.Dato;

                string id = "select idarticulo as Dato from Articulo where cref='" + p.ClaveProducto + "'";
                ClsDatoEntero IDArticulo = db.Database.SqlQuery<ClsDatoEntero>(id).ToList().FirstOrDefault();

                string MATRIZ = "select cref as Dato from MatrizPrecioProv where idproveedor=" + p1 + " and idarticulo=" + IDArticulo.Dato;
                ClsDatoString cref = db.Database.SqlQuery<ClsDatoString>(MATRIZ).ToList().FirstOrDefault();


                if (cref != null)
                {


                    tablaProductos.AddCell(new Phrase(cref.Dato, new Font(Font.FontFamily.HELVETICA, 6)));
                }
                else
                {
                    tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 6)));

                }



                tablaProductos.CompleteRow();


                PdfPCell celdaProductos = new PdfPCell(tablaProductos);
                celdaProductos.MinimumHeight = 20;
                tablaProductosPrincipal.AddCell(celdaProductos);


            }

            Paragraph pR = new Paragraph();
            //pR.IndentationLeft = 403f;
            pR.SpacingAfter = 18;
            pR.Leading = 10;
            pR.Alignment = Element.ALIGN_CENTER;

            pR.Add(new Phrase("\nREQUISICIONES: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            pR.Add(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 16)));




            List<int> Requisiciones2 = new EncReporteAVGContext().Database.SqlQuery<int>("select DISTINCT iddocumento from elementosOrdenCompra where idOrdenCompra=" + _templatePDF.folio).ToList();
            float[] tamanoColumnasrequi = { 100f, 150f, 75f, 75f, 75f, 75f };
            PdfPTable tablarequi = new PdfPTable(tamanoColumnasrequi);
            //tablaProductos.SpacingBefore = 1;
            //tablaProductos.DefaultCell.Border = Rectangle.NO_BORDER;
            tablarequi.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tablarequi.DefaultCell.BorderWidthLeft = 0.1f;
            tablarequi.DefaultCell.BorderWidthRight = 0.1f;
            tablarequi.DefaultCell.BorderWidthBottom = 0.2f;
            tablarequi.DefaultCell.BorderWidthTop = 0.1f;
            tablarequi.SetTotalWidth(tamanoColumnasrequi);
            tablarequi.HorizontalAlignment = Element.ALIGN_LEFT;
            tablarequi.LockedWidth = true;


            tablarequi.AddCell(new Phrase("REQUISICION", new Font(Font.FontFamily.HELVETICA, 6)));

            tablarequi.AddCell(new Phrase("OBSERVACION", new Font(Font.FontFamily.HELVETICA, 6)));

            tablarequi.AddCell(new Phrase("REALIZO", new Font(Font.FontFamily.HELVETICA, 6)));


            tablarequi.AddCell(new Phrase("FECHA ", new Font(Font.FontFamily.HELVETICA, 6)));
            tablarequi.AddCell(new Phrase("FECHA REQUERIDA", new Font(Font.FontFamily.HELVETICA, 6)));
            tablarequi.AddCell(new Phrase("DIAS DIFERENCIA R/O", new Font(Font.FontFamily.HELVETICA, 6)));


            foreach (int elemento in Requisiciones2)
            {
                EncRequisiciones requisicion = new RequisicionesContext().EncRequisicioness.Find(elemento);
                PdfPCell celdafolio = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_CENTER, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.0f, BorderWidthBottom = 0.1f };
                celdafolio.Phrase = new Phrase(requisicion.IDRequisicion.ToString(), new Font(Font.FontFamily.HELVETICA, 8));

                tablarequi.AddCell(celdafolio);

                tablarequi.AddCell(new Phrase(requisicion.Observacion, new Font(Font.FontFamily.HELVETICA, 6)));

                tablarequi.AddCell(new Phrase(requisicion.User.Username.ToString(), new Font(Font.FontFamily.HELVETICA, 6)));


                tablarequi.AddCell(new Phrase(requisicion.Fecha.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 6)));
                tablarequi.AddCell(new Phrase(requisicion.FechaRequiere.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 6)));

                TimeSpan ts = DateTime.Parse(_templatePDF.fecha) - requisicion.Fecha;

                // Difference in days.
                int differenceInDays = ts.Days;


                PdfPCell celdadife = new PdfPCell() { Rotation = 0, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0.1f, BorderWidthTop = 0.0f, BorderWidthRight = 0.1f, BorderWidthBottom = 0.1f };
                celdadife.Phrase = new Phrase(differenceInDays.ToString(), new Font(Font.FontFamily.HELVETICA, 8));




                tablarequi.AddCell(celdadife);

            }


            Paragraph pR2 = new Paragraph();
            //pR.IndentationLeft = 403f;
            pR2.SpacingAfter = 18;
            pR2.Leading = 10;
            pR2.Alignment = Element.ALIGN_CENTER;

            pR2.Add(new Phrase("\nRECEPCIONES: \n", new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            pR2.Add(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 16)));


            _documento.Add(tablaProductosPrincipal);
            AgregarTotales();
            _documento.Add(pR);
            _documento.Add(tablarequi);


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

            foreach (ImpuestoRutaOCF i in _templatePDF.impuestos)
            {
                tablaImportes.AddCell(new Phrase(i.impuesto + " " + i.tasa.ToString("F2") + "%:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(i.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
            }

            foreach (RetencionRutaOCF i in _templatePDF.retenciones)
            {
                tablaImportes.AddCell(new Phrase("Retencion " + i.impuesto + ": ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                tablaImportes.AddCell(new Phrase(i.importe.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));
            }

            tablaImportes.AddCell(new Phrase("Total:".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            tablaImportes.AddCell(new Phrase(_templatePDF.total.ToString("C"), new Font(Font.FontFamily.HELVETICA, 8)));

            tablaTotales.AddCell(new Phrase("IMPORTE CON LETRA: " + _templatePDF.totalEnLetra.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));
            tablaTotales.AddCell(tablaImportes);
            _documento.Add(tablaTotales);

            //float[] anchoColumnasobserva = { 80f, 520f };
            //PdfPTable tablaobservacion = new PdfPTable(anchoColumnasobserva);
            //tablaobservacion.DefaultCell.Border = Rectangle.NO_BORDER;
            //tablaobservacion.SetTotalWidth(anchoColumnasobserva);
            //tablaobservacion.HorizontalAlignment = Element.ALIGN_LEFT;
            //tablaobservacion.LockedWidth = true;

            //tablaobservacion.AddCell(new Phrase("Entregar en :".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //tablaobservacion.AddCell(new Phrase( almacen.Descripcion + " "+ almacen.DIRECCION.ToUpper() + ", " + almacen.Colonia.ToUpper() + ", "+ almacen.Municipio.ToUpper()+", Tel. "+ almacen.Telefono + " Resposable : " + almacen.Responsable.ToUpper(), new Font(Font.FontFamily.HELVETICA, 8)));



            //tablaobservacion.AddCell(new Phrase("OBSERVACION :".ToUpper(), new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            //tablaobservacion.AddCell(new Phrase(_templatePDF.Observacion, new Font(Font.FontFamily.HELVETICA, 8)));




            //      tablaDatosPrincipal.AddCell(tablaobservacion);
            //

            //_documento.Add(tablaobservacion);
        }

        private void AgregarSellos()
        {


            //float[] anchoColumnas = { 500f, 100f };
            //PdfPTable tablaSellosQR = new PdfPTable(anchoColumnas);
            //tablaSellosQR.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            //tablaSellosQR.SpacingBefore = 10.0f;
            //tablaSellosQR.DefaultCell.Border = Rectangle.NO_BORDER;
            //tablaSellosQR.SetTotalWidth(anchoColumnas);
            ////tablaSellosQR.HorizontalAlignment = Element.ALIGN_CENTER;
            //tablaSellosQR.LockedWidth = true;

            //float[] anchoColumnas1 = { 500f };
            //PdfPTable tablaSellos = new PdfPTable(anchoColumnas1);
            //tablaSellos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            //tablaSellos.DefaultCell.VerticalAlignment = Element.ALIGN_TOP;
            //tablaSellos.DefaultCell.Border = Rectangle.NO_BORDER;
            //tablaSellos.SetTotalWidth(anchoColumnas1);
            //tablaSellos.HorizontalAlignment = Element.ALIGN_CENTER;
            ////tablaSellos.LockedWidth = true;

            ////Agregamos el codigo QR al documento


            //_documento.Add(tablaSellosQR);
        }



        public string uso()
        {

            return DecodificadorSAT.getUso(_templatePDF.UsodelCFDI);

        }


        private void ObtenerLetras()
        {


            Numalet numaLet = new Numalet();
            numaLet.MascaraSalidaDecimal = "00/100 M.N.";
            numaLet.SeparadorDecimalSalida = "pesos";
            if (_templatePDF.claveMoneda == "USD" || _templatePDF.claveMoneda == "Dolar americano")
            {
                numaLet.SeparadorDecimalSalida = "dolares";
                numaLet.MascaraSalidaDecimal = "00/100";
            }
            if (_templatePDF.claveMoneda == "EUR" || _templatePDF.claveMoneda.ToUpper() == "EURO")
            {
                numaLet.SeparadorDecimalSalida = "Euros";
                numaLet.MascaraSalidaDecimal = "00/100";
            }
            numaLet.ApocoparUnoParteEntera = true;
            numaLet.LetraCapital = true;
            _templatePDF.totalEnLetra = numaLet.ToCustomString(double.Parse(_templatePDF.total.ToString()));


        }
        #endregion

    }


    public class LineadeRutaproductosF
    {

        public LineadeRutaproductosF() { }

        public string ClaveProducto { get; set; }
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; }

        public decimal suministro { get; set; }
        public string Descripcion { get; set; }
        public decimal v_unitario { get; set; }
        public decimal importe { get; set; }
        public decimal descuento { get; set; }
        public string Presentacion { get; set; }
        public int iddetOrdencompra { get; set; }
        public string Observacion { get; set; }

        public string TipoArticulo { get; set; }

        public int iddetordencompra { get; set; }
    }




    #region Extensión de la clase pdfPageEvenHelper
    public class ITextEventsRutaOCF : PdfPageEventHelper
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


            {
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
                tabla.AddCell(new Phrase("Este documento es una ruta de facturación ", new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.AddCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tabla.AddCell(new Phrase(TextRevision, new Font(Font.FontFamily.HELVETICA, 8)));
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
            //footerTemplate.ShowText((writer.PageNumber - 1).ToString());
            footerTemplate.EndText();
        }
    }
    #endregion



}

