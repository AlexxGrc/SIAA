using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.ClasesProduccion;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Cotizador
{
    public class ClientePDF
    {
        [Key]
        int ID { get; set; }
        [Required]
        [DisplayName("Empresa:")]
        public String Empresa { get; set; }


        [DisplayName("En atencion a:")]
        [Required]
        public string Nombre { get; set; }

        [DisplayName("Vendedor")]
        public int IDVendedor { get; set; }

        public int IDCotizacion { get; set; }
    }

    public class XMLRedaccionCR
    {
        public string Encabezado1 { get; set; }

        public string Encabezado2 { get; set; }

        public string Encabezado3 { get; set; }

        public string Saludo { get; set; }

        public string Despedida { get; set; }

        public string Antefirma { get; set; }

        public string Pie1 { get; set; }

        public string Pie2 { get; set; }

        public string Pie3 { get; set; }

        public string Pie4 { get; set; }

        public string Pie5 { get; set; }

        public string Pie6 { get; set; }

        public string Pie7 { get; set; }

        public string Pie8 { get; set; }

        public string Pie9 { get; set; }

        public string Pie10 { get; set; }
    }

    public class DocumentoCotizacionRapida

    {
        Document doc = new Document(PageSize.LETTER);
        ClsCotizador cotizacion { get; set; }

        ClientePDF cliente = new ClientePDF();

        XMLRedaccionCR redaccion = new XMLRedaccionCR();

        public string nombreDocumento = string.Empty;

        public DocumentoCotizacionRapida(ClsCotizador _c, ClientePDF _cl, XMLRedaccionCR _r)
        {
            cotizacion = _c;
            cliente = _cl;
            redaccion = _r;

        }
        /// <summary>
        /// Crea el odf basado en la ruta del archivo
        /// </summary>
        /// <param name="Ruta"></param>
        public void crear(string Ruta, bool rango1 , bool rango2, bool rango3 , bool rango4)
        {


            doc = new Document(PageSize.LETTER);
            // Indicamos donde vamos a guardar el documento
            


           
            nombreDocumento = Ruta;

            //try
            //{
            //    if (File.Exists(nombreDocumento))
            //    {
            //        return;
            //    }

            //}
            //catch (Exception ERR)
            //{
            //    string mensajederror = ERR.Message;
            //    nombreDocumento = nombreDocumento + "1";
            //}

            PdfWriter writer = PdfWriter.GetInstance(doc,
                                        new FileStream(Ruta, FileMode.Create));

            // Le colocamos el título y el autor
            // **Nota: Esto no será visible en el documento

            doc.SetMargins(30, 10, 120, 30);
            addmetadatos(doc);
            

            // Abrimos el archivo
            doc.Open();
            agregaFecha();
            agregaImagenEncabezado();
            agregaEncabezado();
            AgregaSaludo();

            agregaRango(rango1,rango2,rango3,rango4); // le paso que rango voy a cotizar

            agregaDespedida();
            agregaantefirma();
            AgregaPie();
            agregaImagenPie();
            doc.Close();
            writer.Close();

        }

        public void agregaFecha()
        {
            Empresa empresa = new EmpresaContext().empresas.Find(2);

            PdfPTable tblfecha = new PdfPTable(1);
            tblfecha.WidthPercentage = 40;
            DateTime FECHA = DateTime.Now;

            PdfPCell clfecha = new PdfPCell(new Phrase(empresa.Estados.Estado +",  " + FECHA.ToLongDateString() , new Font(Font.FontFamily.COURIER, 8, Font.NORMAL)));
            clfecha.BorderWidth = 0;
        
            tblfecha.AddCell(clfecha);

            doc.Add(tblfecha);


        }

        public void agregaRango (bool rango1, bool rango2, bool rango3, bool rango4)
        {
            PdfPTable tblrango = new PdfPTable(5);
            tblrango.WidthPercentage = 100;
            string encabeza1 = "";
            string encabeza2 = "";
            string encabeza3 = "";
            string encabeza4 = "";


            if (rango1)
            {
                encabeza1 = cotizacion.Rango1.ToString();

            }


            if (rango2)
            {
                encabeza2 = cotizacion.Rango2.ToString();

            }

            if (rango3)
            {
                encabeza3 = cotizacion.Rango3.ToString();

            }

            if (rango4)
            {
                encabeza4 = cotizacion.Rango4.ToString();

            }


            PdfPCell clvacia = new PdfPCell(new Phrase("Millares", new Font(Font.FontFamily.COURIER, 12, Font.NORMAL)));
            clvacia.BorderWidth = 0;
            clvacia.BorderWidthBottom = 0;
            tblrango.AddCell(clvacia);

            PdfPCell clrango1 = new PdfPCell(new Phrase(encabeza1, new Font(Font.FontFamily.COURIER, 12, Font.NORMAL)));
            clrango1.BorderWidth = 0;
            clrango1.BorderWidthBottom = 0f;
            tblrango.AddCell(clrango1);

            PdfPCell clrango2 = new PdfPCell(new Phrase(encabeza2, new Font(Font.FontFamily.COURIER, 12, Font.NORMAL)));
            clrango2.BorderWidth = 0;
            clrango2.BorderWidthBottom =0f;
            tblrango.AddCell(clrango2);

            PdfPCell clrango3 = new PdfPCell(new Phrase(encabeza3, new Font(Font.FontFamily.COURIER, 12, Font.NORMAL)));
            clrango3.BorderWidth = 0;
            clrango3.BorderWidthBottom = 0f;
            tblrango.AddCell(clrango3);

            PdfPCell clrango5 = new PdfPCell(new Phrase(encabeza4, new Font(Font.FontFamily.COURIER, 12, Font.NORMAL)));
            clrango5.BorderWidth = 0;
            clrango5.BorderWidthBottom = 0;
            tblrango.AddCell(clrango5);


            ///** otro renglon
            ///
            PdfPCell clprecio = new PdfPCell(new Phrase("Precio", new Font(Font.FontFamily.COURIER, 10, Font.NORMAL)));
            clprecio.BorderWidth = 0;
            clprecio.BorderWidthBottom = 0f;
            tblrango.AddCell(clprecio);
           string precio1 = "";
           string  precio2 ="";
           string  precio3 ="";
           string  precio4 ="";


            if (rango1)
            {
                precio1 = cotizacion.precioconvenidos.precio1.ToString("C");

            }
           

            if (rango2)
            {
                precio2 = cotizacion.precioconvenidos.precio2.ToString("C");

            }
           
            if (rango3)
            {
                precio3 = cotizacion.precioconvenidos.precio3.ToString("C");

            }
           
            if (rango4)
            {
                precio4 = cotizacion.precioconvenidos.precio4.ToString("C"); 

            }
            
           
            

            PdfPCell clprecio1 = new PdfPCell(new Phrase(precio1, new Font(Font.FontFamily.COURIER, 10, Font.NORMAL)));
            clprecio1.BorderWidth = 0;
            clprecio1.BorderWidthBottom = 0;
            tblrango.AddCell(clprecio1);

            PdfPCell clprecio2 = new PdfPCell(new Phrase(precio2, new Font(Font.FontFamily.COURIER, 10, Font.NORMAL)));
            clprecio2.BorderWidth = 0;
            clprecio2.BorderWidthBottom = 0f;
            tblrango.AddCell(clprecio2);

            PdfPCell clprecio3 = new PdfPCell(new Phrase(precio3, new Font(Font.FontFamily.COURIER, 10, Font.NORMAL)));
            clprecio3.BorderWidth = 0;
            clprecio3.BorderWidthBottom = 0f;
            tblrango.AddCell(clprecio3);

            PdfPCell clprecio4 = new PdfPCell(new Phrase(precio4, new Font(Font.FontFamily.COURIER, 10, Font.NORMAL)));
            clprecio4.BorderWidth = 0;
            clprecio4.BorderWidthBottom = 0f;
            tblrango.AddCell(clprecio4);
            tblrango.SpacingAfter = 60;

            doc.Add(tblrango);
          

        }

        public void agregaImagenEncabezado()
        {
            try
            {
                Image jpg = Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/COTIZACIONENCABEZADO.jpg"));

                Paragraph paragraph = new Paragraph();
                paragraph.Alignment = Element.ALIGN_RIGHT;
                jpg.ScaleToFit(600f, 150f); //ancho y largo de la imagen
                jpg.Alignment = Image.ALIGN_RIGHT;
                jpg.SetAbsolutePosition(30f, 630f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera
                doc.Add(jpg);
                doc.Add(paragraph);
            }
            catch (Exception err)
            {

            }


        }


        public void agregaantefirma()
        {
            try
            {
                PdfPTable tblAntefirma = new PdfPTable(1);
                tblAntefirma.WidthPercentage = 40;
                DateTime FECHA = DateTime.Now;

                PdfPCell cltblAntefirma = new PdfPCell(new Phrase(redaccion.Antefirma, new Font(Font.FontFamily.COURIER, 8, Font.NORMAL)));
                cltblAntefirma.BorderWidth = 0;

                tblAntefirma.AddCell(cltblAntefirma);

                Vendedor ven = new VendedorContext().Vendedores.Find(cliente.IDVendedor);

                PdfPCell clvendedor = new PdfPCell(new Phrase(ven.Nombre, new Font(Font.FontFamily.COURIER, 8, Font.NORMAL)));
                clvendedor.BorderWidth = 0;

                tblAntefirma.AddCell(clvendedor);
                PdfPCell clvendedorm = new PdfPCell(new Phrase(ven.Mail, new Font(Font.FontFamily.COURIER, 8, Font.NORMAL)));
                clvendedorm.BorderWidth = 0;

                tblAntefirma.AddCell(clvendedorm);

                tblAntefirma.SpacingAfter = 80;

                doc.Add(tblAntefirma);
            }
            catch (Exception err)
            {

            }


        }

        public void agregaImagenPie()
        {
            try
            {
                Image jpg = Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/COTIZACIONPIE.jpg"));

                Paragraph paragraph = new Paragraph();
                paragraph.Alignment = Element.ALIGN_RIGHT;
                jpg.ScaleToFit(640f, 80f); //ancho y largo de la imagen
                jpg.Alignment = Image.ALIGN_RIGHT;
                jpg.SetAbsolutePosition(5f, 6f); //posisicon de la imagen comenzado en 0,0 parte inferir izquiera
                doc.Add(jpg);
                doc.Add(paragraph);
            }
            catch (Exception err)
            {

            }


        }

        public void agregaEncabezado()
        {


            PdfPTable tblencabezado = new PdfPTable(1);
            tblencabezado.WidthPercentage = 100;

            PdfPCell clNombre = new PdfPCell(new Phrase(cliente.Nombre, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
            clNombre.BorderWidth = 0;
            clNombre.BorderWidthBottom = 0f;

            PdfPCell clEmpresa = new PdfPCell(new Phrase(cliente.Empresa, new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL)));
            clEmpresa.BorderWidth = 0;
            clEmpresa.BorderWidthBottom = 0f;

        


            PdfPCell clVendedor = new PdfPCell(new Phrase("Presente", new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL)));
            clVendedor.BorderWidth = 0;
            clVendedor.BorderWidthBottom = 0f;

            tblencabezado.AddCell(clNombre);
            tblencabezado.AddCell(clEmpresa);
            tblencabezado.AddCell(clVendedor);

            tblencabezado.SpacingBefore = 30;
            tblencabezado.SpacingAfter = 30;


            doc.Add(tblencabezado);

            


        }

        public void agregaDespedida()
        {
           

            PdfPTable tbldespedida = new PdfPTable(1);
            tbldespedida.WidthPercentage = 100;

            PdfPCell cldespedida = new PdfPCell(new Phrase(redaccion.Despedida, new Font(Font.FontFamily.COURIER,10, Font.NORMAL)));
            cldespedida.BorderWidth = 0;
            cldespedida.BorderWidthBottom = 0f;
            tbldespedida.AddCell(cldespedida);
            tbldespedida.SpacingAfter = 40;
            doc.Add(tbldespedida);
        }



        public void AgregaSaludo()
        {

           
            PdfPTable tblsaludo = new PdfPTable(1);
            tblsaludo.WidthPercentage = 100;

            PdfPCell saludo = new PdfPCell(new Phrase("\t"+redaccion.Saludo+"\n\n", new Font(Font.FontFamily.COURIER, 10, Font.NORMAL)));
            saludo.BorderWidth = 0;
            saludo.BorderWidthBottom = 0;
            tblsaludo.AddCell(saludo);

            PdfPCell saludoBLANCO = new PdfPCell(new Phrase("", new Font(Font.FontFamily.COURIER, 10, Font.NORMAL)));
            saludoBLANCO.BorderWidth = 0;
            saludoBLANCO.BorderWidthBottom = 0;


            tblsaludo.AddCell(saludoBLANCO);



            PdfPCell Etiqueta1 = new PdfPCell(new Phrase("CARACTERISTICAS TECNICAS ", new Font(Font.FontFamily.COURIER, 8, Font.NORMAL)));
            Etiqueta1.BorderWidth = 0;


            tblsaludo.AddCell(Etiqueta1);

            PdfPCell ETIQUETA2 = new PdfPCell(new Phrase("ANCHO :" + cotizacion.anchoproductomm +" MM", new Font(Font.FontFamily.COURIER, 8, Font.NORMAL)));
            ETIQUETA2.BorderWidth = 0;

            tblsaludo.AddCell(ETIQUETA2);

            PdfPCell ETIQUETA3 = new PdfPCell(new Phrase("LARGO :" + cotizacion.largoproductomm + " MM", new Font(Font.FontFamily.COURIER, 8, Font.NORMAL)));
            ETIQUETA3.BorderWidth = 0;

            tblsaludo.AddCell(ETIQUETA3);

            Materiales material = new MaterialesContext().Materiales.Find(cotizacion.IDMaterial);

            PdfPCell ETIQUETA4 = new PdfPCell(new Phrase("MATERIAL :" + material.Descripcion + " MM", new Font(Font.FontFamily.COURIER, 8, Font.NORMAL)));
            ETIQUETA4.BorderWidth = 0;

            tblsaludo.AddCell(ETIQUETA4);

            if (cotizacion.Numerodecintas>0)
            {
                PdfPCell ETIQUETA5 = new PdfPCell(new Phrase("No de tintas :" + cotizacion.Numerodetintas + " ", new Font(Font.FontFamily.COURIER, 8, Font.NORMAL)));
                ETIQUETA5.BorderWidth = 0;

                tblsaludo.AddCell(ETIQUETA5);


                
            }

            tblsaludo.SpacingAfter = 30;
            doc.Add(tblsaludo);
        }

       

        public void AgregaPie()
        {
            PdfPTable tblPie = new PdfPTable(1);
            tblPie.WidthPercentage = 100;
            if (redaccion.Pie1=="Empty") { redaccion.Pie1 = string.Empty; }
               PdfPCell clpie1 = new PdfPCell(new Phrase(redaccion.Pie1, new Font(Font.FontFamily.COURIER, 6, Font.NORMAL)));
                        clpie1.BorderWidth = 0;
                        clpie1.BorderWidthBottom = 0f;
            tblPie.AddCell(clpie1);

            if (redaccion.Pie2 == "Empty") { redaccion.Pie2 = string.Empty; }
            PdfPCell clpie2 = new PdfPCell(new Phrase(redaccion.Pie2, new Font(Font.FontFamily.COURIER, 6, Font.NORMAL)));
            clpie2.BorderWidth = 0;
            clpie2.BorderWidthBottom = 0f;
            tblPie.AddCell(clpie2);

            if (redaccion.Pie3 == "Empty") { redaccion.Pie3 = string.Empty; }
            PdfPCell clpie3 = new PdfPCell(new Phrase(redaccion.Pie3, new Font(Font.FontFamily.COURIER, 6, Font.NORMAL)));
            clpie3.BorderWidth = 0;
            clpie3.BorderWidthBottom = 0f;
            tblPie.AddCell(clpie3);

            if (redaccion.Pie4 == "Empty") { redaccion.Pie4 = string.Empty; }
            PdfPCell clpie4 = new PdfPCell(new Phrase(redaccion.Pie4, new Font(Font.FontFamily.COURIER, 6, Font.NORMAL)));
            clpie4.BorderWidth = 0;
            clpie4.BorderWidthBottom = 0f;
            tblPie.AddCell(clpie4);

            if (redaccion.Pie5 == "Empty") { redaccion.Pie5 = string.Empty; }
            PdfPCell clpie5 = new PdfPCell(new Phrase(redaccion.Pie5, new Font(Font.FontFamily.COURIER, 6, Font.NORMAL)));
            clpie5.BorderWidth = 0;
            clpie5.BorderWidthBottom = 0;
            tblPie.AddCell(clpie5);

            if (redaccion.Pie6 == "Empty") { redaccion.Pie6 = string.Empty; }
            PdfPCell clpie6 = new PdfPCell(new Phrase(redaccion.Pie6, new Font(Font.FontFamily.COURIER, 6, Font.NORMAL)));
            clpie6.BorderWidth = 0;
            clpie6.BorderWidthBottom = 0f;
            tblPie.AddCell(clpie6);

            if (redaccion.Pie7 == "Empty") { redaccion.Pie7 = string.Empty; }
            PdfPCell clpie7 = new PdfPCell(new Phrase(redaccion.Pie7, new Font(Font.FontFamily.COURIER, 6, Font.NORMAL)));
            clpie7.BorderWidth = 0;
            clpie7.BorderWidthBottom = 0f;
            tblPie.AddCell(clpie7);

            if (redaccion.Pie8 == "Empty") { redaccion.Pie8 = string.Empty; }
            PdfPCell clpie8 = new PdfPCell(new Phrase(redaccion.Pie8, new Font(Font.FontFamily.COURIER, 6, Font.NORMAL)));
            clpie8.BorderWidth = 0;
            clpie8.BorderWidthBottom = 0f;
            tblPie.AddCell(clpie8);


            if (redaccion.Pie9 == "Empty") { redaccion.Pie9 = string.Empty; }
            PdfPCell clpie9 = new PdfPCell(new Phrase(redaccion.Pie9, new Font(Font.FontFamily.COURIER, 6, Font.NORMAL)));
            clpie9.BorderWidth = 0;
            clpie9.BorderWidthBottom = 0f;
            tblPie.AddCell(clpie9);


            if (redaccion.Pie10 == "Empty") { redaccion.Pie10 = string.Empty; }
            PdfPCell clpie10 = new PdfPCell(new Phrase(redaccion.Pie10, new Font(Font.FontFamily.COURIER, 6, Font.NORMAL)));
            clpie10.BorderWidth = 0;
            clpie10.BorderWidthBottom = 0f;
            tblPie.AddCell(clpie10);



            doc.Add(tblPie);


         
        }

        public void addmetadatos(Document doc)
        {
            doc.AddTitle("Cotizacion Rapida");
            doc.AddCreator("Vigma Consultores SAS de CV");
        }
   }

}