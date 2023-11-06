using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Cfdi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace SIAAPI.Reportes
{
    public class ReporteListadoFClie
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;
        PdfWriter _writer;
        int _totalColumn = 8;

        public DateTime fechaini { get; set; }


        public DateTime fechafin { get; set; }
        public int idcliente { get; set; }

        public decimal acumulasubtotalxUSD =0;
        public decimal  acumuladescuentoxUSD = 0;
        public decimal  acumulaivaxUSD = 0;
        public decimal  acumuatotalxUSD = 0;

       public decimal acumulasubtotalxEUR = 0;
       public decimal acumuladescuentoxEUR = 0;
       public decimal acumulaivaxEUR = 0;
       public decimal acumuatotalxEUR = 0;
     
       public decimal acumulasubtotalxMNX = 0;
       public decimal acumuladescuentoxMNX = 0;
       public decimal acumulaivaxMNX = 0;
        public decimal acumuatotalxMNX = 0;
        PdfPTable _pdfTable = new PdfPTable(8);
        PdfPTable tablae = new PdfPTable(13);


        PdfPCell _PdfPCell;

        
        MemoryStream _memoryStream = new MemoryStream();

        List<ReporteFacturas> facturas = new List<ReporteFacturas>();
        #endregion

        public VEncFacturaFContext db = new VEncFacturaFContext();
        // aqui los puedes pasar como parametro al reporte

        public byte[] PrepareReport(DateTime _fechaini, DateTime _fechafin, int _idcliente  )

        {
            fechaini = _fechaini;
            fechafin = _fechafin;
            idcliente = _idcliente;

            facturas = this.GetFacturas(fechaini, fechafin, idcliente);


            #region
            _documento = new Document(PageSize.LETTER, 0f, 0f, 0f, 0f);

            _documento.SetMargins(20f, 10f, 20f, 30f);

            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEventsRepLFac();
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            //tablae.WidthPercentage = 100;
            //tablae.HorizontalAlignment = Element.ALIGN_LEFT;
            _documento.Open();

           // _pdfTable.SetWidths(new float[] { 40f, 40f, 40f, 260f, 60f, 60f, 50f, 50f});
            #endregion
            this.ReportHeader();
            this.ReportBody();

          //  _pdfTable.HeaderRows = 4;
            //_documento.Add(_pdfTable);
            _documento.Close();


            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"ListadoFacturasClientes.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();
            
            return _memoryStream.ToArray();


        }

        //Obtener la lista de vendedores en un periodo de fechas
        

        public List<ReporteFacturas> GetFacturas(DateTime fechaini, DateTime fechafin, int idcliente)
        {
            List<ReporteFacturas> facturas = new List<ReporteFacturas>();
            try
            {
                string FI = fechaini.Year.ToString() + "-" + fechaini.Month.ToString() + "-" + fechaini.Day.ToString();
                string FF = fechafin.Year.ToString() + "-" + fechafin.Month.ToString() + "-" + fechafin.Day.ToString();
                //string cadena = "select * from [dbo].[EncfacturasSaldos] where Fecha >= '" + FI + "' and Fecha <='" + FF + "' and Nombre_Cliente = '" + cliente.Dato + "' ";
                string cadena = "select ID,  Serie, Numero,Fecha, IDCliente, Nombre_Cliente, Subtotal, Descuento, IVA, Total, MonedaOrigen, TC, Estado from [dbo].[VEncFacturaF] ";
                if (idcliente == 0)
                {
                    cadena = cadena + "where Fecha>='" + FI + " 00:00:01' and Fecha <='" + FF + " 23:59:59' ";
                }
                else
                {

                    ClsDatoString cliente = db.Database.SqlQuery<ClsDatoString>("select Nombre as Dato  from [dbo].[Clientes] where IDCliente = '" + idcliente + "'").ToList()[0];
                    cadena = cadena + "where Fecha>='" + FI + " 00:00:01' and Fecha <='" + FF + " 23:59:59' and Nombre_Cliente = '" + cliente.Dato + "' ";
                }
                    
                facturas = db.Database.SqlQuery<ReporteFacturas>(cadena).ToList();
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return facturas;
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


            Image jpg = Image.GetInstance(byteArrayToImage(empresa.Logo), BaseColor.WHITE);

            jpg.ScaleToFit(105f, 75f);

            CMYKColor colorletratitulo;

            colorletratitulo = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado).color;

                PdfPCell imageCell = new PdfPCell(jpg);
                _PdfPCell = new PdfPCell((imageCell));
                _PdfPCell.Border = 0;
                _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 15f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Resumen de facturas", _fontStyle));
            _PdfPCell.Colspan = _totalColumn;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.Border = 0;
            _PdfPCell.BackgroundColor = BaseColor.WHITE;
            _PdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_PdfPCell);
            _documento.Add(_pdfTable);


            float[] anchoColumnasfechas = { 100f, 50f, 25f, 225f, 100f, 100f };

            PdfPTable tablafechas = new PdfPTable(anchoColumnasfechas);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablafechas.SetTotalWidth(anchoColumnasfechas);
            tablafechas.SpacingBefore = 0;
            tablafechas.HorizontalAlignment = Element.ALIGN_LEFT;
            tablafechas.LockedWidth = true;
            Font _fontStylefecha = new Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

            PdfPCell _tablafechas1 = new PdfPCell(new Phrase("Periodo del ", _fontStylefecha));
            _tablafechas1.Border = 0;
            _tablafechas1.FixedHeight = 20f;
            _tablafechas1.HorizontalAlignment = Element.ALIGN_RIGHT;
            _tablafechas1.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas1.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas1);

            string FI = fechaini.ToString("dd/MM/yyyy");
            PdfPCell _tablafechas2 = new PdfPCell(new Phrase(FI, _fontStylefecha));
            _tablafechas2.Border = 0;
            _tablafechas2.FixedHeight = 10f;
            _tablafechas2.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablafechas2.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas2.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas2);

            PdfPCell _tablafechas3 = new PdfPCell(new Phrase("al ", _fontStylefecha));
            _tablafechas3.Border = 0;
            _tablafechas3.FixedHeight = 10f;
            _tablafechas3.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablafechas3.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas3.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas3);

            string FF = fechafin.ToString("dd/MM/yyyy");
            PdfPCell _tablafechas4 = new PdfPCell(new Phrase(FF, _fontStylefecha));
            _tablafechas4.Border = 0;
            _tablafechas4.FixedHeight = 10f;
            _tablafechas4.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablafechas4.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas4.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas4);

            PdfPCell _tablafechas5 = new PdfPCell(new Phrase("Fecha de impresión: ", _fontStylefecha));
            _tablafechas5.Border = 0;
            _tablafechas5.FixedHeight = 10f;
            _tablafechas5.HorizontalAlignment = Element.ALIGN_RIGHT;
            _tablafechas5.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas5.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas5);

            DateTime fecAct = DateTime.Today;
            string FA = fecAct.ToString("dd/MM/yyyy");
            PdfPCell _tablafechas6 = new PdfPCell(new Phrase(FA, _fontStylefecha));
            _tablafechas6.Border = 0;
            _tablafechas6.FixedHeight = 10f;
            _tablafechas6.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablafechas6.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas6.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas6);
            //tablafechas.CompleteRow();
            _documento.Add(tablafechas);

            CMYKColor colordefinido;

            colordefinido = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte).color;


            //Encabezado de la tabla de datos
            float[] anchoColumnasEncfactura = { 35f, 35f, 50f, 100f, 50f, 40f, 70f, 100f, 50f, 40f };

            PdfPTable tablae = new PdfPTable(anchoColumnasEncfactura);
            tablae.WidthPercentage = 100;
            tablae.HorizontalAlignment = Element.ALIGN_LEFT;


             _fontStyle = FontFactory.GetFont("Tahoma", 7f,colorletratitulo);
            _PdfPCell = new PdfPCell(new Phrase("Serie", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Numero", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Fecha", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);


            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Cliente", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);


            

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Subtotal", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Descuento", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("IVA", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Total", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            //_PdfPCell = new PdfPCell(new Phrase("Moneda", _fontStyle));
            //_PdfPCell.Border = 0;
            //_PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            //_PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = colordefinido;
            //tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Estado", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            //_fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("TC", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);
            _documento.Add(tablae);


            #endregion




        }

        private void ReportBody()
        {

            #region Table Body


            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            //  



            foreach (ReporteFacturas fact in facturas)
            {


                // Cuerpo del reporte

                float[] anchoColumnasfactura = { 35f, 35f, 50f, 100f, 50f, 40f, 70f, 100f, 50f, 40f };

                // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

                PdfPTable tablaparaestafactura = new PdfPTable(anchoColumnasfactura);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaparaestafactura.SetTotalWidth(anchoColumnasfactura);
                tablaparaestafactura.SpacingBefore = 3;
                tablaparaestafactura.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaparaestafactura.LockedWidth = true;

                Font _fontStyleparaestaFactura = new Font(Font.FontFamily.HELVETICA, 5, Font.NORMAL);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                if (fact.Estado == "C")
                {
                    fact.Subtotal = 0;
                    fact.Descuento = 0;
                    fact.IVA = 0;
                    fact.Total = 0;
                   

                }

                string fecha = fact.fecha.ToShortDateString();
                string ID = Convert.ToString(fact.ID);


                string subtotal = fact.Subtotal.ToString("C");
                string descuento = fact.Descuento.ToString("C");// COn esto le indico que lo comverta a formato meneda y luego a string para que los millares a aparezcan separados por coma
                string iva = fact.IVA.ToString("C");
                string total = fact.Total.ToString("C");

              

                PdfPCell _PdfPCell1pedido = new PdfPCell(new Phrase(fact.Serie, _fontStyleparaestaFactura));
                _PdfPCell1pedido.Border = 0;
                _PdfPCell1pedido.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell1pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell1pedido);


                PdfPCell _PdfPCell00pedido = new PdfPCell(new Phrase(fact.numero.ToString(), _fontStyleparaestaFactura));
                _PdfPCell00pedido.Border = 0;
                _PdfPCell00pedido.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell00pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell00pedido);

                PdfPCell _PdfPCell2pedido = new PdfPCell(new Phrase(fecha, _fontStyleparaestaFactura));
                _PdfPCell2pedido.Border = 0;
                _PdfPCell2pedido.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell2pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell2pedido);

                PdfPCell _PdfPCell3pedido = new PdfPCell(new Phrase(fact.Nombre_Cliente, _fontStyleparaestaFactura));
                _PdfPCell3pedido.Border = 0;
                _PdfPCell3pedido.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                _PdfPCell3pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores

                tablaparaestafactura.AddCell(_PdfPCell3pedido);
                

                PdfPCell _PdfPCell6pedido = new PdfPCell(new Phrase(subtotal, _fontStyleparaestaFactura));
                _PdfPCell6pedido.Border = 0;
                _PdfPCell6pedido.HorizontalAlignment = Element.ALIGN_RIGHT;
                _PdfPCell6pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell6pedido);

                PdfPCell _PdfPCell7pedido = new PdfPCell(new Phrase(descuento, _fontStyleparaestaFactura));
                _PdfPCell7pedido.Border = 0;
                _PdfPCell7pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                _PdfPCell7pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell7pedido);

                PdfPCell _PdfPCell8pedido = new PdfPCell(new Phrase(iva, _fontStyleparaestaFactura));
                _PdfPCell8pedido.Border = 0;
                _PdfPCell8pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                _PdfPCell8pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell8pedido);


                PdfPCell _PdfPCell9pedido = new PdfPCell(new Phrase(total+ "   "+ fact.MonedaOrigen, _fontStyleparaestaFactura));
                _PdfPCell9pedido.Border = 0;
                _PdfPCell9pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                _PdfPCell9pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell9pedido);


                string es = "";
                if (fact.Estado == "A")
                {
                    es = "Activo";  
                }
                else
                {
                    es = "Cancelado";
                }

                PdfPCell _PdfPCell12pedido = new PdfPCell(new Phrase(es, _fontStyleparaestaFactura));
                _PdfPCell12pedido.Border = 0;
                _PdfPCell12pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                _PdfPCell12pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell12pedido);

                PdfPCell _PdfPCell13pedido = new PdfPCell(new Phrase(fact.TC.ToString("C"), _fontStyleparaestaFactura));
                _PdfPCell13pedido.Border = 0;
                _PdfPCell13pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                _PdfPCell13pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell13pedido);

               

                 tablaparaestafactura.CompleteRow();

                        // ya que imprimimos en renglon acumulamos cifras
            if  (fact.MonedaOrigen == "USD" && fact.Estado!="C")
                { 
                acumulasubtotalxUSD += fact.Subtotal;
                acumuladescuentoxUSD += fact.Descuento;
                acumulaivaxUSD += fact.IVA;
                acumuatotalxUSD += fact.Total;
                }
                if (fact.MonedaOrigen == "EUR" && fact.Estado != "C")
                {
                    acumulasubtotalxEUR += fact.Subtotal;
                    acumuladescuentoxEUR += fact.Descuento;
                    acumulaivaxEUR += fact.IVA;
                    acumuatotalxEUR += fact.Total;
                }
                if (fact.MonedaOrigen == "MXN" && fact.Estado != "C")
                {
                    acumulasubtotalxMNX += fact.Subtotal;
                    acumuladescuentoxMNX += fact.Descuento;
                    acumulaivaxMNX += fact.IVA;
                    acumuatotalxMNX += fact.Total;
                }
                // imprimimos la tabla que corresponde a un solo renglon del cliente
              

                _documento.Add(tablaparaestafactura);





            } //Foreach(ReporteFacturas fact in facturas)

            // cuando termine de imprimir el listado de facturas

            //podemos imprimir los totales 

            

            float[] anchoColumnasTotfactura = { 50, 90f, 90f, 90f, 100f };

            // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

            PdfPTable tablatotalesfactura = new PdfPTable(anchoColumnasTotfactura);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablatotalesfactura.SetTotalWidth(anchoColumnasTotfactura);
            tablatotalesfactura.SpacingBefore = 30;
            tablatotalesfactura.HorizontalAlignment = Element.ALIGN_LEFT;
            tablatotalesfactura.LockedWidth = true;
            Font _fontStyleFactura = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD);

            /////////////

            PdfPCell _titTotFac10 = new PdfPCell(new Phrase("Moneda", _fontStyleFactura));
            _titTotFac10.Border = 0;
            _titTotFac10.HorizontalAlignment = Element.ALIGN_LEFT; // como es moneda lo alineamos a la derecha
            _titTotFac10.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_titTotFac10);

            PdfPCell _titTotFac11 = new PdfPCell(new Phrase("Subtotal", _fontStyleFactura));
           _titTotFac11.Border = 0;
           _titTotFac11.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _titTotFac11.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_titTotFac11);

            PdfPCell _titTotFac12 = new PdfPCell(new Phrase("Descuento", _fontStyleFactura));
            _titTotFac12.Border = 0;
            _titTotFac12.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _titTotFac12.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_titTotFac12);


            PdfPCell _titTotFac13 = new PdfPCell(new Phrase("IVA", _fontStyleFactura));
            _titTotFac13.Border = 0;
            _titTotFac13.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _titTotFac13.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_titTotFac13);




            PdfPCell _titTotFac14 = new PdfPCell(new Phrase("Total", _fontStyleFactura));
            _titTotFac14.Border = 0;
            _titTotFac14.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _titTotFac14.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_titTotFac14);




            /////////

            PdfPCell _TotFac10 = new PdfPCell(new Phrase("MNX", _fontStyleFactura));
            _TotFac10.Border = 0;
            _TotFac10.HorizontalAlignment = Element.ALIGN_LEFT; // como es moneda lo alineamos a la derecha
            _TotFac10.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_TotFac10);

            PdfPCell _TotFac11 = new PdfPCell(new Phrase(acumulasubtotalxMNX.ToString("C"), _fontStyleFactura));
            _TotFac11.Border = 0;
            _TotFac11.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac11.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_TotFac11);

            PdfPCell _TotFac12 = new PdfPCell(new Phrase(acumuladescuentoxMNX.ToString("C"), _fontStyleFactura));
            _TotFac12.Border = 0;
            _TotFac12.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac12.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_TotFac12);


            PdfPCell _TotFac13 = new PdfPCell(new Phrase(acumulaivaxMNX.ToString("C"), _fontStyleFactura));
            _TotFac13.Border = 0;
            _TotFac13.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac13.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_TotFac13);




            PdfPCell _TotFac14 = new PdfPCell(new Phrase(acumuatotalxMNX.ToString("C"), _fontStyleFactura));
            _TotFac14.Border = 0;
            _TotFac14.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac14.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_TotFac14);



            float[] anchoColumnasTotfacturausd = { 50, 90f, 90f, 90f, 100f };

            // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

            PdfPTable tablatotalesfacturausd = new PdfPTable(anchoColumnasTotfacturausd);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablatotalesfacturausd.SetTotalWidth(anchoColumnasTotfactura);
            tablatotalesfacturausd.SpacingBefore = 3;
            tablatotalesfacturausd.HorizontalAlignment = Element.ALIGN_LEFT;
            tablatotalesfacturausd.LockedWidth = true;
            Font _fontStyleFacturausd = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD);





            PdfPCell _TotFac20 = new PdfPCell(new Phrase("USD", _fontStyleFacturausd));
            _TotFac20.Border = 0;
            _TotFac20.HorizontalAlignment = Element.ALIGN_LEFT; // como es moneda lo alineamos a la derecha
            _TotFac20.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfacturausd.AddCell(_TotFac20);

            PdfPCell _TotFac21 = new PdfPCell(new Phrase(acumulasubtotalxUSD.ToString("C"), _fontStyleFacturausd));
            _TotFac21.Border = 0;
            _TotFac21.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac21.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfacturausd.AddCell(_TotFac21);

            PdfPCell _TotFac22 = new PdfPCell(new Phrase(acumuladescuentoxUSD.ToString("C"), _fontStyleFacturausd));
            _TotFac22.Border = 0;
            _TotFac22.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac22.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfacturausd.AddCell(_TotFac22);


            PdfPCell _TotFac23 = new PdfPCell(new Phrase(acumulaivaxUSD.ToString("C"), _fontStyleFacturausd));
            _TotFac23.Border = 0;
            _TotFac23.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac23.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfacturausd.AddCell(_TotFac23);

            PdfPCell _TotFac24 = new PdfPCell(new Phrase(acumuatotalxUSD.ToString("C"), _fontStyleFacturausd));
            _TotFac24.Border = 0;
            _TotFac24.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac24.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfacturausd.AddCell(_TotFac24);



            float[] anchoColumnasTotfacturaeu = { 50, 90f, 90f, 90f, 100f };

            // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

            PdfPTable tablatotalesfacturaeu = new PdfPTable(anchoColumnasTotfacturaeu);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
            tablatotalesfacturaeu.SetTotalWidth(anchoColumnasTotfactura);
            tablatotalesfacturaeu.SpacingBefore = 3;
            tablatotalesfacturaeu.HorizontalAlignment = Element.ALIGN_LEFT;
            tablatotalesfacturaeu.LockedWidth = true;
            Font _fontStyleFacturaeu= new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD);



            PdfPCell _TotFac30 = new PdfPCell(new Phrase("EUR", _fontStyleFacturaeu));
            _TotFac30.Border = 0;
            _TotFac30.HorizontalAlignment = Element.ALIGN_LEFT; // como es moneda lo alineamos a la derecha
            _TotFac30.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfacturaeu.AddCell(_TotFac30);

            PdfPCell _TotFac31 = new PdfPCell(new Phrase(acumulasubtotalxEUR.ToString("C"), _fontStyleFacturaeu));
            _TotFac31.Border = 0;
            _TotFac31.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac31.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfacturaeu.AddCell(_TotFac31);

            PdfPCell _TotFac32 = new PdfPCell(new Phrase(acumuladescuentoxEUR.ToString("C"), _fontStyleFacturaeu));
            _TotFac32.Border = 0;
            _TotFac32.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac32.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfacturaeu.AddCell(_TotFac32);


            PdfPCell _TotFac33 = new PdfPCell(new Phrase(acumulaivaxEUR.ToString("C"), _fontStyleFacturaeu));
            _TotFac33.Border = 0;
            _TotFac33.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac33.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfacturaeu.AddCell(_TotFac33);




            PdfPCell _TotFac34 = new PdfPCell(new Phrase(acumuatotalxEUR.ToString("C"), _fontStyleFacturaeu));
            _TotFac34.Border = 0;
            _TotFac34.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac34.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfacturaeu.AddCell(_TotFac34);

            tablatotalesfactura.CompleteRow();
            tablatotalesfacturausd.CompleteRow();
            tablatotalesfacturaeu.CompleteRow();

            _documento.Add(tablatotalesfactura);
            _documento.Add(tablatotalesfacturausd);
            _documento.Add(tablatotalesfacturaeu);
            #endregion
        }
    }
       



#region Extensión de la clase pdfPageEvenHelper
public class ITextEventsRepLFacClie : PdfPageEventHelper
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
            footerTemplate = cb.CreateTemplate(60,40);
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
            cb.SetFontAndSize(bf, 7);
            cb.SetTextMatrix(document.PageSize.GetRight(60), document.PageSize.GetBottom(20));
            //cb.MoveText(500,30);
            //cb.ShowText("Este comprobante es una representación impresa de un CFDI");
            cb.ShowText(text);
            cb.EndText();
            float len = bf.GetWidthPoint(text, 7);
            cb.AddTemplate(footerTemplate, document.PageSize.GetRight(60) + len, document.PageSize.GetBottom(20));

            float[] anchoColumasTablaTotales = { 600f };
            PdfPTable tabla = new PdfPTable(anchoColumasTablaTotales);
            //tabla.DefaultCell.Border = Rectangle.NO_BORDER;
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
    #endregion

} 

   
    public class fechasReporteFacClie
        {
        private DateTime? fecha = DateTime.Now;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime fechaini { get { return fecha ?? DateTime.Now; } set { fecha = value; } }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime fechafin { get; set; }
        public int idcliente { get; set; }

    }
    public class ReporteFacturasClie
    {
        [Key]
        [Display(Name = "idFactura")]
        public int ID { get; set; }
        [Display(Name = "Serie")]
        public String Serie { get; set; }
        [Display(Name = "Numero")]
        public int numero { get; set; }
        [Display(Name = "IDCliente")]
        public int IDCliente { get; set; }
        [Display(Name = "Cliente")]
        public String Nombre_Cliente { get; set; }

        [Display(Name = "fecha")]
        public DateTime fecha { get; set; }

        [Display(Name = "subtotal")]
        public Decimal Subtotal { get; set; }
        [Display(Name = "Descuento")]
        public Decimal Descuento { get; set; }

        [Display(Name = "IVA")]
        public Decimal IVA { get; set; }

        [Display(Name = "Total")]
        public Decimal Total { get; set; }
        [Display(Name = "IDMoneda")]
        public int IDMoneda { get; set; }

        [Display(Name = "Moneda")]
        public string MonedaOrigen { get; set; }
        public decimal TC { get; set; }

        public string Estado { get; set; }
        public decimal TotalDls { get; set; }
    

}

}


