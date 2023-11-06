using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.clasescfdi;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace SIAAPI.Reportes
{

    public class CreaEntregaPDF
    {
      public   List<VDetEntregaRemision> Entregas = new List<VDetEntregaRemision>();
        MemoryStream _memoryStream = new MemoryStream();
        private Document _documento; //Objeto para escribir el pdf
        BaseFont _fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        BaseFont _fuenteContenido = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        PdfWriter _writer;
        PdfContentByte _cb;

        RemisionContext BD = new RemisionContext();

        public int IDEntrega;
        public string Ruta;
        public string Chofer;
        public DateTime Fecha;


        public Empresa empresa;

        public CreaEntregaPDF(System.Drawing.Image logo, int identrega,  Empresa emp)
        {

            int IDEntrega = identrega;
            empresa = emp;

            
            EntregaRemisionesContext db = new EntregaRemisionesContext();

            VEntregaR entregaRemision = new EntregaRemisionesContext().Database.SqlQuery<VEntregaR>("SELECT*FROM VEntregaR WHERE ID="+ IDEntrega).ToList().FirstOrDefault();
            Ruta = entregaRemision.Ruta;
            Chofer = entregaRemision.Chofer;
            Fecha = entregaRemision.Fecha;
            string listaR = "SELECT *from VDetEntregaRemision where identregar=" + IDEntrega;
             Entregas = db.Database.SqlQuery<VDetEntregaRemision>(listaR).ToList();
            if (Entregas.Count() == 0)
            {
                listaR = "select*from VDetEntregaRemisionSF where identregar=" + IDEntrega;
                Entregas = new SIAAPI.Models.Comercial.EntregaRemisionesContext().Database.SqlQuery<SIAAPI.Models.Comercial.VDetEntregaRemision>(listaR).ToList();

            }
            //ObtenerLetras();
            //Trabajos con el documento XML
            _documento = new Document(PageSize.LETTER.Rotate());
        

            //string nombreDocumento = Path.GetTempFileName() + ".pdf";
            //string nombreDocumento = rutaPDF;

            //Creamos el documento
            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEventsEntrega(); // invoca la clase que esta mas abajo correspondiente al pie de pagina

            //Agregamos propiedades al documento
            AgregaPropiedadesDocumento();

            //Abrimos el documento
            _documento.Open();


            AgregarLogo(logo);
            AgregarCuadro();
            AgregarDatosEmisor("");
            
            AgregarDatosEntrega();
            AgregarTitulo();
            AgregarDatosProductos();
         
            _documento.Close();

            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"Entrega-" + entregaRemision.Ruta+ ".pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

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
            imagen.ScaleToFit(100, 80);
            imagen.Alignment = Element.ALIGN_TOP;
            Chunk logo = new Chunk(imagen, 1, -80);
            _documento.Add(logo);
        }

        private void AgregarCuadro()
        {
            _cb = _writer.DirectContentUnder;
            //_cb.SaveState();
            //_cb.BeginText();
            //_cb.MoveText(1, 1);
            //_cb.SetFontAndSize(_fuenteTitulos, 8);
            //_cb.ShowText("Faustino Rojas Arelano");
            //_cb.EndText();
            //_cb.RestoreState();

            //Agrego cuadro al documento
            _cb.SetColorStroke(new CMYKColor(0, 29, 50, 70)); //Color de la linea
            _cb.SetColorFill(new CMYKColor(0, 29, 50, 70)); // Color del relleno
            _cb.SetLineWidth(3.5f); //Tamano de la linea
            _cb.Rectangle(408, 694, 20, 100);
            _cb.FillStroke();
        }

        private void AgregaPropiedadesDocumento()
        {
            _documento.AddAuthor("VIGMA CONSULTORES SAS DE CV");
            _documento.AddCreator("DOCUMENTO GENERADO DESDE C#");
            _documento.AddKeywords("Entrega Remisión");
            _documento.AddSubject("Entrega Remisión");
            _documento.AddTitle("ENTREGA REMISIÓN");
            _documento.SetMargins(5, 5, 5, 5);
        }


        private void AgregarDatosProductos()
        {
            CMYKColor colorcelda = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;
            CMYKColor colorTITULO = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;


            float[] tamanoColumnasPrincipal = { 780f };
            PdfPTable tabla = new PdfPTable(tamanoColumnasPrincipal);
            tabla.SetTotalWidth(tamanoColumnasPrincipal);
            tabla.HorizontalAlignment = Element.ALIGN_LEFT;
            tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            tabla.LockedWidth = true;



            float[] tamanoColumnastitulos = { 50f, 340f, 50f, 340f };
           

            float[] tamanoColumnasVacioa = { 780f };
          


            float[] tamanoColumnasF = {100f, 100f, 100f, 100f, 200f, 180f};
           

            RemisionContext db = new RemisionContext();


            string Cadenafactura = string.Empty;




          

            foreach (VDetEntregaRemision p in Entregas)
            {
                PdfPTable tablaProductosTitulos = new PdfPTable(tamanoColumnastitulos);
                tablaProductosTitulos.SetTotalWidth(tamanoColumnastitulos);
                tablaProductosTitulos.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductosTitulos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductosTitulos.LockedWidth = true;

                PdfPTable tablaVacia = new PdfPTable(tamanoColumnasVacioa);
                tablaVacia.SetTotalWidth(tamanoColumnasVacioa);
                tablaVacia.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaVacia.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaVacia.LockedWidth = true;


                PdfPTable tablaProductos = new PdfPTable(tamanoColumnasF);

                //tablaProductosTitulos.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaProductos.SetTotalWidth(tamanoColumnasF);
                tablaProductos.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductos.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaProductos.LockedWidth = true;

                PdfPCell celda0 = new PdfPCell(new Phrase("REMISIÓN", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                PdfPCell celda1 = new PdfPCell(new Phrase("FACTURA", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                PdfPCell celda4 = new PdfPCell(new Phrase("CAJAS", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                PdfPCell celda5 = new PdfPCell(new Phrase("PAQUETES", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                PdfPCell celda6 = new PdfPCell(new Phrase("OBSERVACION", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                PdfPCell celda7 = new PdfPCell(new Phrase("PERSONA QUIEN RECIBE \n NOMBRE-FIRMA", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                //celda0.BackgroundColor = colorcelda;
                //celda1.BackgroundColor = colorcelda;
                //celda4.BackgroundColor = colorcelda;
                //celda5.BackgroundColor = colorcelda;
                //celda6.BackgroundColor = colorcelda;
                //celda7.BackgroundColor = colorcelda;
                tablaProductos.AddCell(celda0);
                tablaProductos.AddCell(celda1);
                tablaProductos.AddCell(celda4);
                tablaProductos.AddCell(celda5);
                tablaProductos.AddCell(celda6);
                tablaProductos.AddCell(celda7);

                                    
                    PdfPCell cliente = new PdfPCell(new Phrase("Cliente", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorTITULO)));
                    PdfPCell clienteN = new PdfPCell(new Phrase(p.Cliente, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                    PdfPCell Domicilio = new PdfPCell(new Phrase("Entregar en:", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, colorTITULO)));
                    PdfPCell DomicilioN = new PdfPCell(new Phrase(p.Entrega, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                    cliente.BackgroundColor = colorcelda;
                    Domicilio.BackgroundColor = colorcelda;
                    tablaProductosTitulos.AddCell(cliente);
                    tablaProductosTitulos.AddCell(clienteN);
                    tablaProductosTitulos.AddCell(Domicilio);
                    tablaProductosTitulos.AddCell(DomicilioN);

                    PdfPCell celdat10n = new PdfPCell(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));

                    celdat10n.Border = Rectangle.NO_BORDER;
                    celdat10n.Colspan = 4;

                    tablaProductosTitulos.AddCell(celdat10n);

                EncRemision remision = new RemisionContext().EncRemisiones.Find(p.IDRemision); 
                   
                    string cadenadias = "select DiaEntLu from entrega where idcliente=" + remision.IDCliente;
                    ClsDatoBool DiaEntLu = db.Database.SqlQuery<ClsDatoBool>(cadenadias).ToList().FirstOrDefault();
                    string cadenadiasm = "select DiaEntMa from entrega where idcliente=" + remision.IDCliente;
                    ClsDatoBool DiaEntMa = db.Database.SqlQuery<ClsDatoBool>(cadenadias).ToList().FirstOrDefault();
                    string cadenadiasmi = "select DiaEntMi from entrega where idcliente=" + remision.IDCliente;
                    ClsDatoBool DiaEntMi = db.Database.SqlQuery<ClsDatoBool>(cadenadias).ToList().FirstOrDefault();
                    string cadenadiasju = "select DiaEntJu from entrega where idcliente=" + remision.IDCliente;
                    ClsDatoBool DiaEntJu = db.Database.SqlQuery<ClsDatoBool>(cadenadias).ToList().FirstOrDefault();
                    string cadenadiasvi = "select DiaEntVi from entrega where idcliente=" + remision.IDCliente;
                    ClsDatoBool DiaEntVi = db.Database.SqlQuery<ClsDatoBool>(cadenadias).ToList().FirstOrDefault();
                    string cadenadiaH = "select HorarioEnt from entrega where idcliente=" + remision.IDCliente;
                    ClsDatoString HorarioEnt = db.Database.SqlQuery<ClsDatoString>(cadenadiaH).ToList().FirstOrDefault();
                    
                    //List<Entrega> diasentrega = db.Database.SqlQuery<Entrega>(cadenadias).ToList();
                    //foreach (Entrega veces in diasentrega)
                    //{
                    //    veces.HorarioEnt
                    //}

                    string L = "";
                    string M = "";
                    string Mi = "";
                    string J = "";
                    string V = "";
                    string hora = "";
                    /////La mayotia de los clientes no tienen dias de entrega ni hora//////
                    try{ if (DiaEntLu.Dato) { L = "Lu "; } } catch (Exception err){}
                    try { if (DiaEntMa.Dato) { M = "Ma "; } } catch (Exception err) { }
                    try { if (DiaEntMi.Dato) { Mi = "Mi "; } } catch (Exception err) { }
                    try { if (DiaEntJu.Dato) { J = "Ju "; } } catch (Exception err) { }
                    try { if (DiaEntVi.Dato) { V = "Vi "; } } catch (Exception err) { }
                    try { hora = HorarioEnt.Dato; } catch (Exception err) { }






                    PdfPCell celdaT = new PdfPCell(new Phrase("Días  y hora de entrega", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                   // celdaT.BackgroundColor = colorcelda;
                    celdaT.Colspan = 2;
                    tablaProductosTitulos.AddCell(celdaT);

                    PdfPCell celdaD = new PdfPCell(new Phrase(L + M+ Mi+ J+ V + " | " + hora , new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));
                    
                    celdaD.Colspan = 2;
                    tablaProductosTitulos.AddCell(celdaD);
                    //PdfPCell celdap1 = new PdfPCell(tablaProductosTitulos);
                    //tablaVacia.AddCell(celdap1);
                    //PdfPCell celdaProductos = new PdfPCell(tablaProductos);

                    //tablaVacia.AddCell(celdaProductos);

             


                


                
                tablaProductos.AddCell(new Phrase(p.IDRemision.ToString(), new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase(p.NoFactura, new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
                tablaProductos.AddCell(new Phrase("\n\n", new Font(Font.FontFamily.HELVETICA, 8)));



                PdfPCell celdaTtablaProductos = new PdfPCell(tablaProductos);
                celdaTtablaProductos.Colspan = 4;
                PdfPCell celdaTtablaProductos1 = new PdfPCell(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
                celdaTtablaProductos1.Colspan = 4;
                tablaProductosTitulos.AddCell(celdaTtablaProductos);
                tablaProductosTitulos.AddCell(celdaTtablaProductos1);
                Cadenafactura = "";

                PdfPCell celdaTitulos = new PdfPCell(tablaProductosTitulos);
                tablaVacia.AddCell(celdaTitulos);
                _documento.Add(tablaVacia);
            }

        

            //PdfPCell celdaProductos = new PdfPCell(tablaProductos);


            //tablaVacia.AddCell(celdaProductos);
            //_documento.Add(tablaProductosTitulos);

        


        }


        private void AgregarDatosEmisor(String Telefono)
        {
            //Datos del emisor
            Paragraph p1 = new Paragraph();
            p1.IndentationLeft = 150f;
            p1.SpacingAfter = 20;
            p1.Leading = 9;

            p1.Add(new Phrase(empresa.RazonSocial, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase(empresa.RFC, new Font(Font.FontFamily.HELVETICA, 10)));
            p1.Add("\n");
            p1.Add(new Phrase(empresa.Telefono, new Font(Font.FontFamily.HELVETICA, 8)));
            p1.Add("\n");
            p1.Add(new Phrase(empresa.Calle +" " + empresa.NoExt +" " + empresa.NoInt + "\n" + empresa.Colonia + "\n" + empresa.Municipio + "\n" + empresa.Estados.Estado, new Font(Font.FontFamily.HELVETICA, 8)));

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


       
        private void AgregarDatosEntrega()
        {
            //Datos de la factura
            Paragraph p2 = new Paragraph();
            String TextRevision = "REV. 2";
            p2.IndentationLeft = 600f;
            p2.SpacingAfter = 20;
            p2.Leading = 10;
            p2.Alignment = Element.ALIGN_CENTER;

            p2.Add(new Phrase(Ruta, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD)));
            p2.Add(new Phrase("\n", new Font(Font.FontFamily.HELVETICA, 16)));

            p2.Add(new Phrase("\nFecha  | ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add(new Phrase(Fecha.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 8)));
            p2.Add(new Phrase("\nCódigo | ", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD)));
            p2.Add(new Phrase("FSG32 ", new Font(Font.FontFamily.HELVETICA, 8)));
           p2.Add(new Phrase(TextRevision, new Font(Font.FontFamily.HELVETICA, 8)));


            p2.Add("\n\n");
            p2.Add(new Phrase(Chofer, new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD)));


            p2.SpacingAfter = 32;
            _documento.Add(p2);
        }

        private void AgregarTitulo()
        {
            //Datos de la factura
            Paragraph p3 = new Paragraph();
            p3.IndentationLeft = 50f;
            p3.SpacingAfter = 10;
            p3.Leading = 2;
            p3.Alignment = Element.ALIGN_CENTER;

            p3.Add(new Phrase("CONTROL DE RUTA", new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD)));
           


           p3.SpacingAfter = 5;
            _documento.Add(p3);
        }


        #endregion

    }

   



    #region Extensión de la clase pdfPageEvenHelper
    public class ITextEventsEntrega : PdfPageEventHelper
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
            //p2.IndentationLeft = 600f;
            //p2.SpacingAfter = 20;

            cb.BeginText();
            cb.SetFontAndSize(bf, 9);
            cb.SetTextMatrix(document.PageSize.GetRight(90), document.PageSize.GetBottom(30));
            //cb.MoveText(500,30);
            //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
            //cb.ShowText(text);
            cb.EndText();
            float len = bf.GetWidthPoint(text, 9);
            cb.AddTemplate(footerTemplate, document.PageSize.GetRight(70) + len, document.PageSize.GetBottom(30));
            float[] anchoColumasTablaTotales = { 770f, 700f };
            PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
            tabla.DefaultCell.Border = Rectangle.NO_BORDER;
            tabla.SetTotalWidth(anchoColumasTablaTotales);
         
            tabla.HorizontalAlignment = Element.ALIGN_RIGHT;
            tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            tabla.LockedWidth = true;
       
            tabla.AddCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 8)));
            tabla.AddCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8)));
           
            tabla.WriteSelectedRows(0, -1, 5, document.PageSize.GetBottom(40), writer.DirectContent);

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


