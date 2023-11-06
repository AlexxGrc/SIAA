using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Cfdi;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace SIAAPI.Reportes
{
    public class ReporteporProveedor
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;
        PdfWriter _writer;
        int _totalColumn = 8;

        public DateTime fechaini { get; set; }

        public int idProveedor { get; set; }

        public string Nota { get; set; }

        public DateTime fechafin { get; set; }

        public decimal acumulasubtotalxMNX = 0;
        public decimal acumulasubtotalxUSD = 0;
        public decimal acumulasubtotalxEUR = 0;

        PdfPTable _pdfTable = new PdfPTable(8);
        PdfPTable tablae = new PdfPTable(13);

        PdfPCell _PdfPCell;

        MemoryStream _memoryStream = new MemoryStream();

        List<ReportefenoProv> facturas = new List<ReportefenoProv>();
        #endregion

        //p1ublic VEncFacturaFContext db = new VEncFacturaFContext();
        public ClientesContext db = new ClientesContext();
        // aqui los puedes pasar como parametro al reporte

        public byte[] PrepareReport(DateTime _fechaini, DateTime _fechafin, int _idProveedor, string _nota)
        {
            fechaini = _fechaini;
            fechafin = _fechafin;
            idProveedor = _idProveedor;
            Nota = _nota;

            facturas = this.GetFacturas();
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
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"ListadoGeneralProveedor.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();
        }
        //Obtener la lista de vendedores en un periodo de fechas
        public List<ReportefenoProv> GetFacturas()
        {
            List<ReportefenoProv> facturas = new List<ReportefenoProv>();
            try
            {
                string cadena = "select ID,FechaPago, SerieP, FolioP,  RFC,Empresa, ClaveFormaPago, Descripcion as FormaPago, ClaveDivisa, NoOperacion, Monto, RFCBancoEmisor, NombreBancoEmisor, CuentaEmisor, RFCBancoReceptor,NombreBancoReceptor, CuentaReceptor, IDTipoCadenaPago,Estado, UUID from VPagoFacturaProvT";
                cadena = cadena + " where FechaPago>='" + fechaini.Year + "/" + fechaini.Month + "/" + fechaini.Day + " 00:00:01' and FechaPago <='" + fechafin.Year + "/" + fechafin.Month + "/" + fechafin.Day + " 23:59:59' and IDProveedor=" + idProveedor;
                facturas = db.Database.SqlQuery<ReportefenoProv>(cadena).ToList();

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
            _PdfPCell = new PdfPCell(new Phrase("Reporte General del Proveedor", _fontStyle));
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
            float[] anchoColumnasEncfactura = { 35f, 50f, 130f, 35f, 35f, 50f, 40f, 60f, 50f, 40f };

            PdfPTable tablae = new PdfPTable(anchoColumnasEncfactura);
            tablae.WidthPercentage = 100;
            tablae.HorizontalAlignment = Element.ALIGN_LEFT;
            _fontStyle = FontFactory.GetFont("Tahoma", 7f, colorletratitulo);

            _PdfPCell = new PdfPCell(new Phrase("Fecha de pago", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);
            
            _PdfPCell = new PdfPCell(new Phrase("RFC", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("Empresa", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("Clave divisa", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("Serie", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("Folio", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("Monto", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("RFC banco", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("Cuenta emisor", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablae.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("Estado", _fontStyle));
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

            foreach (ReportefenoProv fact in facturas)
            {
                // Cuerpo del reporte

                float[] anchoColumnasfactura = { 35f, 60f, 140f, 45f, 35f, 50f, 55f, 60f, 55f, 50f };

                // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

                PdfPTable tablaparaestafactura = new PdfPTable(anchoColumnasfactura);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaparaestafactura.SetTotalWidth(anchoColumnasfactura);
                tablaparaestafactura.SpacingBefore = 3;
                tablaparaestafactura.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaparaestafactura.LockedWidth = true;

                Font _fontStyleparaestaFactura = new Font(Font.FontFamily.HELVETICA, 5, Font.NORMAL);  // aqui podremos cambiar la fuente de lcientes y su tamanño
                
                string fechapago = fact.FechaPago.Year + "/" + fact.FechaPago.Month + "/" + fact.FechaPago.Day;
                string monto = fact.Monto.ToString();

                PdfPCell _PdfPCell1pedido = new PdfPCell(new Phrase(fechapago, _fontStyleparaestaFactura));
                _PdfPCell1pedido.Border = 0;
                _PdfPCell1pedido.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell1pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell1pedido);


                PdfPCell _PdfPCell00pedido = new PdfPCell(new Phrase(fact.RFC, _fontStyleparaestaFactura));
                _PdfPCell00pedido.Border = 0;
                _PdfPCell00pedido.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell00pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell00pedido);

                PdfPCell _PdfPCell2pedido = new PdfPCell(new Phrase(fact.Empresa, _fontStyleparaestaFactura));
                _PdfPCell2pedido.Border = 0;
                _PdfPCell2pedido.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell2pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell2pedido);

                PdfPCell _PdfPCell3pedido = new PdfPCell(new Phrase(fact.ClaveDivisa, _fontStyleparaestaFactura));
                _PdfPCell3pedido.Border = 0;
                _PdfPCell3pedido.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell3pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell3pedido);

                PdfPCell _PdfPCell6pedido = new PdfPCell(new Phrase(fact.SerieP.ToString(), _fontStyleparaestaFactura));
                _PdfPCell6pedido.Border = 0;
                _PdfPCell6pedido.HorizontalAlignment = Element.ALIGN_CENTER;
                _PdfPCell6pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                tablaparaestafactura.AddCell(_PdfPCell6pedido);

                PdfPCell _PdfPCell7pedido = new PdfPCell(new Phrase(fact.FolioP.ToString(), _fontStyleparaestaFactura));
                _PdfPCell7pedido.Border = 0;
                _PdfPCell7pedido.HorizontalAlignment = Element.ALIGN_CENTER; // como es moneda lo alineamos a la derecha
                _PdfPCell7pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                tablaparaestafactura.AddCell(_PdfPCell7pedido);

                PdfPCell _PdfPCell8pedido = new PdfPCell(new Phrase(monto, _fontStyleparaestaFactura));
                _PdfPCell8pedido.Border = 0;
                _PdfPCell8pedido.HorizontalAlignment = Element.ALIGN_CENTER; // como es moneda lo alineamos a la derecha
                _PdfPCell8pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                tablaparaestafactura.AddCell(_PdfPCell8pedido);


                PdfPCell _PdfPCell9pedido = new PdfPCell(new Phrase(fact.RFCBancoEmisor, _fontStyleparaestaFactura));
                _PdfPCell9pedido.Border = 0;
                _PdfPCell9pedido.HorizontalAlignment = Element.ALIGN_CENTER; // como es moneda lo alineamos a la derecha
                _PdfPCell9pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell9pedido);

                PdfPCell _PdfPCell13pedido = new PdfPCell(new Phrase(fact.CuentaEmisor, _fontStyleparaestaFactura));
                _PdfPCell13pedido.Border = 0;
                _PdfPCell13pedido.HorizontalAlignment = Element.ALIGN_CENTER; // 
                _PdfPCell13pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparaestafactura.AddCell(_PdfPCell13pedido);


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
                _PdfPCell12pedido.HorizontalAlignment = Element.ALIGN_CENTER; // 
                _PdfPCell12pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                tablaparaestafactura.AddCell(_PdfPCell12pedido);
                
                tablaparaestafactura.CompleteRow();
                
                _documento.Add(tablaparaestafactura);

                if (fact.ClaveDivisa == "USD" && fact.Estado == "A")
                {
                    acumulasubtotalxUSD += fact.Monto;
                }
                if (fact.ClaveDivisa == "EUR" && fact.Estado == "A")
                {
                    acumulasubtotalxEUR += fact.Monto;
                }
                if (fact.ClaveDivisa == "MXN" && fact.Estado == "A")
                {
                    acumulasubtotalxMNX += fact.Monto;
                }
            }

            // cuando termine de imprimir el listado de facturas

            //podemos imprimir los totales 

            float[] anchoColumnasTotfactura = { 150, 100f, 100f, 100f, 100f };

            // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

            PdfPTable tablatotalesfactura = new PdfPTable(anchoColumnasTotfactura);
            tablatotalesfactura.SetTotalWidth(anchoColumnasTotfactura);
            tablatotalesfactura.SpacingBefore = 30;
            tablatotalesfactura.HorizontalAlignment = Element.ALIGN_LEFT;
            tablatotalesfactura.LockedWidth = true;
            Font _fontStyleFactura = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD);

            /////////////

            PdfPCell _titTotFac10 = new PdfPCell(new Phrase("", _fontStyleFactura));
            _titTotFac10.Border = 0;
            _titTotFac10.HorizontalAlignment = Element.ALIGN_LEFT; // como es moneda lo alineamos a la derecha
            _titTotFac10.VerticalAlignment = Element.ALIGN_MIDDLE;
            tablatotalesfactura.AddCell(_titTotFac10);

            PdfPCell _titTotFac11 = new PdfPCell(new Phrase("", _fontStyleFactura));
            _titTotFac11.Border = 0;
            _titTotFac11.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _titTotFac11.VerticalAlignment = Element.ALIGN_MIDDLE;
            tablatotalesfactura.AddCell(_titTotFac11);

            PdfPCell _titTotFac12 = new PdfPCell(new Phrase("", _fontStyleFactura));
            _titTotFac12.Border = 0;
            _titTotFac12.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _titTotFac12.VerticalAlignment = Element.ALIGN_MIDDLE;
            tablatotalesfactura.AddCell(_titTotFac12);


            PdfPCell _titTotFac13 = new PdfPCell(new Phrase("Moneda", _fontStyleFactura));
            _titTotFac13.Border = 0;
            _titTotFac13.HorizontalAlignment = Element.ALIGN_LEFT; // como es moneda lo alineamos a la derecha
            _titTotFac13.VerticalAlignment = Element.ALIGN_MIDDLE;
            tablatotalesfactura.AddCell(_titTotFac13);

            PdfPCell _titTotFac14 = new PdfPCell(new Phrase("Monto Total", _fontStyleFactura));
            _titTotFac14.Border = 0;
            _titTotFac14.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _titTotFac14.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_titTotFac14);

            /////////

            PdfPCell _TotFac10 = new PdfPCell(new Phrase("", _fontStyleFactura));
            _TotFac10.Border = 0;
            _TotFac10.HorizontalAlignment = Element.ALIGN_LEFT; // como es moneda lo alineamos a la derecha
            _TotFac10.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_TotFac10);

            PdfPCell _TotFac11 = new PdfPCell(new Phrase("", _fontStyleFactura));
            _TotFac11.Border = 0;
            _TotFac11.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac11.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_TotFac11);

            PdfPCell _TotFac12 = new PdfPCell(new Phrase("", _fontStyleFactura));
            _TotFac12.Border = 0;
            _TotFac12.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac12.VerticalAlignment = Element.ALIGN_MIDDLE;
            tablatotalesfactura.AddCell(_TotFac12);


            PdfPCell _TotFac13 = new PdfPCell(new Phrase("MXN", _fontStyleFactura));
            _TotFac13.Border = 0;
            _TotFac13.HorizontalAlignment = Element.ALIGN_LEFT; // como es moneda lo alineamos a la derecha
            _TotFac13.VerticalAlignment = Element.ALIGN_MIDDLE;
            tablatotalesfactura.AddCell(_TotFac13);

            PdfPCell _TotFac14 = new PdfPCell(new Phrase(acumulasubtotalxMNX.ToString("C"), _fontStyleFactura));
            _TotFac14.Border = 0;
            _TotFac14.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac14.VerticalAlignment = Element.ALIGN_MIDDLE;
            tablatotalesfactura.AddCell(_TotFac14);

            /////////////////////////////////////////////////////////////////////////
            PdfPCell _TotFac15 = new PdfPCell(new Phrase("", _fontStyleFactura));
            _TotFac15.Border = 0;
            _TotFac15.HorizontalAlignment = Element.ALIGN_LEFT; // como es moneda lo alineamos a la derecha
            _TotFac15.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_TotFac15);

            PdfPCell _TotFac16 = new PdfPCell(new Phrase("", _fontStyleFactura));
            _TotFac16.Border = 0;
            _TotFac16.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac16.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_TotFac16);

            PdfPCell _TotFac17 = new PdfPCell(new Phrase("", _fontStyleFactura));
            _TotFac17.Border = 0;
            _TotFac17.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac17.VerticalAlignment = Element.ALIGN_MIDDLE;
            tablatotalesfactura.AddCell(_TotFac17);

            PdfPCell _TotFac18 = new PdfPCell(new Phrase("USD", _fontStyleFactura));
            _TotFac18.Border = 0;
            _TotFac18.HorizontalAlignment = Element.ALIGN_LEFT; // como es moneda lo alineamos a la derecha
            _TotFac18.VerticalAlignment = Element.ALIGN_MIDDLE;
            tablatotalesfactura.AddCell(_TotFac18);

            PdfPCell _TotFac19 = new PdfPCell(new Phrase(acumulasubtotalxUSD.ToString("C"), _fontStyleFactura));
            _TotFac19.Border = 0;
            _TotFac19.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac19.VerticalAlignment = Element.ALIGN_MIDDLE;
            tablatotalesfactura.AddCell(_TotFac19);

            /////////////////////////////////////////////////////////////////////////////

            PdfPCell _TotFac20 = new PdfPCell(new Phrase("", _fontStyleFactura));
            _TotFac20.Border = 0;
            _TotFac20.HorizontalAlignment = Element.ALIGN_LEFT; // como es moneda lo alineamos a la derecha
            _TotFac20.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_TotFac20);

            PdfPCell _TotFac21 = new PdfPCell(new Phrase("", _fontStyleFactura));
            _TotFac21.Border = 0;
            _TotFac21.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac21.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            tablatotalesfactura.AddCell(_TotFac21);

            PdfPCell _TotFac22 = new PdfPCell(new Phrase("", _fontStyleFactura));
            _TotFac22.Border = 0;
            _TotFac22.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac22.VerticalAlignment = Element.ALIGN_MIDDLE;
            tablatotalesfactura.AddCell(_TotFac22);

            PdfPCell _TotFac23 = new PdfPCell(new Phrase("EUR", _fontStyleFactura));
            _TotFac23.Border = 0;
            _TotFac23.HorizontalAlignment = Element.ALIGN_LEFT; // como es moneda lo alineamos a la derecha
            _TotFac23.VerticalAlignment = Element.ALIGN_MIDDLE;
            tablatotalesfactura.AddCell(_TotFac23);

            PdfPCell _TotFac24 = new PdfPCell(new Phrase(acumulasubtotalxEUR.ToString("C"), _fontStyleFactura));
            _TotFac24.Border = 0;
            _TotFac24.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
            _TotFac24.VerticalAlignment = Element.ALIGN_MIDDLE;
            tablatotalesfactura.AddCell(_TotFac24);

            tablatotalesfactura.CompleteRow();

            _documento.Add(tablatotalesfactura);

            float[] anchoColumnasPresenta = { 250f, 350f };

            PdfPTable tablaPresenta = new PdfPTable(anchoColumnasPresenta);

            tablaPresenta.SetTotalWidth(anchoColumnasPresenta);
            tablaPresenta.SpacingBefore = 10;
            tablaPresenta.HorizontalAlignment = Element.ALIGN_RIGHT;
            tablaPresenta.LockedWidth = true;
            Font _fontStylePresenta = new Font(Font.FontFamily.TIMES_ROMAN, 6, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

            PdfPCell _tablaPresenta1 = new PdfPCell(new Phrase("NOTA: ", _fontStylePresenta)); //IDOficina.ToString()
            _tablaPresenta1.Border = 0;
            _tablaPresenta1.HorizontalAlignment = Element.ALIGN_RIGHT;
            _tablaPresenta1.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablaPresenta1.FixedHeight = 10f;
            tablaPresenta.AddCell(_tablaPresenta1);

            PdfPCell _tablaPresenta2 = new PdfPCell(new Phrase(Nota, _fontStylePresenta));
            _tablaPresenta2.Border = 0;
            _tablaPresenta2.FixedHeight = 10f;
            _tablaPresenta2.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablaPresenta2.VerticalAlignment = Element.ALIGN_MIDDLE;

            tablaPresenta.AddCell(_tablaPresenta2);

            tablaPresenta.CompleteRow();
            _documento.Add(tablaPresenta);
            #endregion
        }
    }

    #region Extensión de la clase pdfPageEvenHelper
    public class ITextEventsRepLFactPr : PdfPageEventHelper
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
                footerTemplate = cb.CreateTemplate(60, 40);
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
                tabla.SetTotalWidth(anchoColumasTablaTotales);
                tabla.HorizontalAlignment = Element.ALIGN_CENTER;
                tabla.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                tabla.LockedWidth = true;
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
    public class fechasReporteFactPr
    {
        private DateTime? fecha = DateTime.Now;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime fechaini { get { return fecha ?? DateTime.Now; } set { fecha = value; } }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime fechafin { get; set; }
    }
    public class ReporteFacturascPro
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