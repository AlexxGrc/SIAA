using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace SIAAPI.Reportes
{
    public class ReportePedidosVendedor
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;
        PdfWriter _writer;
        int _totalColumn = 8;



        PdfPTable _pdfTable = new PdfPTable(8);
        PdfPTable tablae = new PdfPTable(8);


        PdfPCell _PdfPCell;


        public DateTime fechaini { get; set; }


        public DateTime fechafin { get; set; }
        public int IDVendedor { get; set; }

        MemoryStream _memoryStream = new MemoryStream();

        List<VendedorPe> vendedores = new List<VendedorPe>();
        #endregion();

        public VendedorPeContext db = new VendedorPeContext();
        // aqui los puedes pasar como parametro al reporte

        public byte[] PrepareReport(DateTime _fechaini, DateTime _fechafin, int idVendedor)

        {
            fechaini = _fechaini;
            fechafin = _fechafin;

            vendedores = this.GetPedidoVen(idVendedor);


            #region
            _documento = new Document(PageSize.LETTER, 0f, 0f, 0f, 0f);

            _documento.SetMargins(20f, 10f, 20f, 30f);

            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEvents();
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            tablae.WidthPercentage = 100;
            tablae.HorizontalAlignment = Element.ALIGN_LEFT;
            _documento.Open();

            // _pdfTable.SetWidths(new float[] { 40f, 40f, 40f, 260f, 60f, 60f, 50f, 50f});
            #endregion
            this.ReportHeader();
            this.ReportBody();

            //  _pdfTable.HeaderRows = 4;
            //_documento.Add(_pdfTable);
            _documento.Close();


            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"Pedidos por Vendedors.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();


        }

        //Obtener la lista de vendedores en un periodo de fechas
        public List<VendedorPe> GetPedidoVen(int idVendedor)
        {
            List<VendedorPe> vendedores = new List<VendedorPe>();
            try
            {
                string cadena = "select  distinct E.IDVendedor, V.Nombre, V.CuotaVendedor, M.ClaveMoneda from [dbo].[EncPedido] as E inner join [dbo].[Vendedor] as V on E.IDVendedor = V.IDVendedor inner join [dbo].[c_Moneda] as M on V.IDMoneda = M.IDMoneda where e.IDVendedor= "+ idVendedor +" and fecha>='" + fechaini.Year + "/" + fechaini.Month + "/" + fechaini.Day + " 00:00:01' and fecha <='" + fechafin.Year + "/" + fechafin.Month + "/" + fechafin.Day + " 23:59:59' and e.status<>'cancelado' order by nombre";
                vendedores = db.Database.SqlQuery<VendedorPe>(cadena).ToList();
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return vendedores;
        }

        //Obtener la lista de clientes del vendedores en un periodo de fechas
        public List<ClientesPed> GetPedidoCl(int idvendedor)
        {
            List<ClientesPed> clientes = new List<ClientesPed>();
            try
            {
                string cadena = "Select distinct P.IDCliente,  C.Nombre, C.Telefono from dbo.EncPedido as P inner join Clientes as C on P.IDCliente = C.IDCliente where P.IDVendedor= " + idvendedor + " and fecha>='" + fechaini.Year + "/" + fechaini.Month + "/" + fechaini.Day + " 00:00:01' and fecha <='" + fechafin.Year + "/" + fechafin.Month + "/" + fechafin.Day + " 23:59:59' and p.status<>'cancelado' order by C.Nombre";
                clientes = db.Database.SqlQuery<ClientesPed>(cadena).ToList();
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return clientes;
        }

        //Obtener la lista de pedidos de los clientes en un periodo de fechas
        public List<ReporteVen> GetPedidoP(int idcliente, int idvendedor)
        {
            List<ReporteVen> pedido = new List<ReporteVen>();
            try
            {
                string cadena = "SELECT P.idPedido, p.fecha,p.subtotal, p.iva, p.total, p.IDMoneda, CAST(1 AS DECIMAL(8,2)) as TC, p.status, CAST(0 AS DECIMAL(8,2)) as TotalDls FROM clientes AS c INNER JOIN encPedido AS p ON c.IDCliente=p.idcliente where p.IDVendedor="+idvendedor+" and  p.IdCliente=" + idcliente + " and fecha>='" + fechaini.Year + "/" + fechaini.Month + "/" + fechaini.Day + " 00:00:01' and fecha <='" + fechafin.Year + "/" + fechafin.Month + "/" + fechafin.Day + " 23:59:59' and p.status<>'Cancelado' order by p.fecha";
                pedido = db.Database.SqlQuery<ReporteVen>(cadena).ToList();
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return pedido;
        }
        //Obtener los detalles de un pedido

        public List<detVen> GetdetVen(int idPedido)
        {
            List<detVen> detpe = new List<detVen>();
            try
            {
                string cadena = "SELECT d.IDPEdido, a.Cref, d.Cantidad, d.suministro from detpedido as d  inner join encpedido as e on d.IDPedido=e.IDpedido inner join articulo as a on d.idarticulo=a.idarticulo and e.IDpedido=" + idPedido + " and e.status<>'cancelado'";
                detpe = db.Database.SqlQuery<detVen>(cadena).ToList();
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return detpe;

        }



        private void ReportHeader()
        {
            #region Table head

            Image jpg = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/logo.jpg"));
            PdfPCell imageCell = new PdfPCell(jpg);
            _PdfPCell = new PdfPCell((imageCell));
            _PdfPCell.Border = 0;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 15f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Reporte de Pedidos en Dolares por Vendedor", _fontStyle));
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


            //_fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
            //_PdfPCell = new PdfPCell(new Phrase("   ", _fontStyle)); 
            //_PdfPCell.Colspan = _totalColumn;

            //_PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_PdfPCell.Border = 0;
            //_PdfPCell.BackgroundColor = BaseColor.WHITE;
            //_PdfPCell.ExtraParagraphSpace = 0;
            //_pdfTable.AddCell(_PdfPCell);



            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Fecha", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            tablae.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Pedido", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            tablae.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Subtotal", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            tablae.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("IVA", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            tablae.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Total", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            tablae.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Status", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            tablae.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("TC", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            tablae.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Total Can", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            tablae.AddCell(_PdfPCell);
            //_pdfTable.CompleteRow();

            _documento.Add(tablae);




            #endregion




        }

        private void ReportBody()
        {

            #region Table Body


            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            //  
            decimal acumuladodlsvendedor = 0M;

            foreach (VendedorPe vendedor in vendedores)
            {
                float[] anchoColumnasvendedor = { 100f, 200f, 100f, 200f };
                PdfPTable tablaparaestevendedor = new PdfPTable(anchoColumnasvendedor);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaparaestevendedor.SetTotalWidth(anchoColumnasvendedor);
                tablaparaestevendedor.SpacingBefore = 3;
                tablaparaestevendedor.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaparaestevendedor.LockedWidth = true;
                Font _fontStylevendedor = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                PdfPCell _vendedorCell1 = new PdfPCell(new Phrase("ID Vendedor: " + vendedor.IDVendedor.ToString(), _fontStylevendedor));
                _vendedorCell1.Border = 0;
                _vendedorCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                _vendedorCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                _vendedorCell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                tablaparaestevendedor.AddCell(_vendedorCell1);

                PdfPCell _vendedorCell2 = new PdfPCell(new Phrase(vendedor.Nombre.ToString(), _fontStylevendedor));
                _vendedorCell2.Border = 0;
                _vendedorCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                _vendedorCell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                _vendedorCell2.BackgroundColor = BaseColor.LIGHT_GRAY;
                tablaparaestevendedor.AddCell(_vendedorCell2);

                PdfPCell _vendedorCell3 = new PdfPCell(new Phrase("Cuota: " + vendedor.CuotaVendedor.ToString("C"), _fontStylevendedor));
                _vendedorCell3.Border = 0;
                _vendedorCell3.HorizontalAlignment = Element.ALIGN_LEFT;
                _vendedorCell3.VerticalAlignment = Element.ALIGN_MIDDLE;
                _vendedorCell3.BackgroundColor = BaseColor.LIGHT_GRAY;
                tablaparaestevendedor.AddCell(_vendedorCell3);


                PdfPCell _vendedorCell4 = new PdfPCell(new Phrase(vendedor.ClaveMoneda.ToString(), _fontStylevendedor));
                _vendedorCell4.Border = 0;
                _vendedorCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                _vendedorCell4.VerticalAlignment = Element.ALIGN_MIDDLE;
                _vendedorCell4.BackgroundColor = BaseColor.LIGHT_GRAY;
                tablaparaestevendedor.AddCell(_vendedorCell4);



                _documento.Add(tablaparaestevendedor);


                List<ClientesPed> clientes = new List<ClientesPed>();
                clientes = GetPedidoCl(vendedor.IDVendedor);
                foreach (ClientesPed cliente in clientes)
                {
                    // creamos una tabla para imprimir los los datos del cliente
                    // como son 4 columnas a imprimir 600entre las 4 

                    float[] anchoColumnasclientes = { 50f, 250f, 300f };

                    // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

                    PdfPTable tablaparaestecliente = new PdfPTable(anchoColumnasclientes);
                    //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                    tablaparaestecliente.SetTotalWidth(anchoColumnasclientes);
                    tablaparaestecliente.SpacingBefore = 3;
                    tablaparaestecliente.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaparaestecliente.LockedWidth = true;

                    Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                    PdfPCell _PdfPCell1 = new PdfPCell(new Phrase(cliente.IDCliente.ToString(), _fontStylecliente));
                    _PdfPCell1.Border = 0;
                    _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell1.BackgroundColor = BaseColor.WHITE;
                    tablaparaestecliente.AddCell(_PdfPCell1);

                    PdfPCell _PdfPCell2 = new PdfPCell(new Phrase(cliente.Nombre, _fontStylecliente));
                    _PdfPCell2.Border = 0;
                    _PdfPCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell2.BackgroundColor = BaseColor.WHITE;
                    tablaparaestecliente.AddCell(_PdfPCell2);

                    PdfPCell _PdfPCell3 = new PdfPCell(new Phrase(cliente.Telefono, _fontStylecliente));
                    _PdfPCell3.Border = 0;
                    _PdfPCell3.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell3.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCell3.BackgroundColor = BaseColor.WHITE;
                    tablaparaestecliente.AddCell(_PdfPCell3);



                    // añadimos la tabla al documento principal para que la imprimia

                    _documento.Add(tablaparaestecliente);

                    // descpues de que imprima el cliente queremos que imprima los pedidos del cliente

                    List<ReporteVen> pedidos = new List<ReporteVen>();

                    pedidos = GetPedidoP(cliente.IDCliente, vendedor.IDVendedor); // aqui obtenemos los pedidos que corresponden a ese cliente

                    // antes de entrar al foreach de los pedidos

                    // preparado los acumuladores para imprimir totales por cluentes si quisiera pero su quiero

                    decimal acumulasubtotalxcliente = 0;
                    decimal acumulaivaxcliente = 0;
                    decimal acumuatotalxcliente = 0;
                    decimal acumuladlsxcliente = 0;

                    foreach (ReporteVen pedido in pedidos)
                    {
                        /// en pedido tengo cada pedido del cliente
                        /// ahora cramos la tabla con el formato para los peddos en base a una tabla 
                        /// diviendo 600 entre el numero de columnas que quiero
                        /// en este caso 9 , le pongo f al frente del numero para indicar que es un numero flotante ya que puedo harclo decimalmente
                        /// 
                        float[] anchoColumnaspedidos = { 50f, 30f, 20f, 80f, 100f, 100f, 50f, 50f, 50f, 70f };

                        // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

                        PdfPTable tablaparaestepedido = new PdfPTable(anchoColumnaspedidos);
                        //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                        tablaparaestepedido.SetTotalWidth(anchoColumnaspedidos);
                        tablaparaestepedido.SpacingBefore = 3;
                        tablaparaestepedido.HorizontalAlignment = Element.ALIGN_LEFT;
                        tablaparaestepedido.LockedWidth = true;

                        Font _fontStylepedido = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);  // aqui podremos cambiar la fuente de lcientes y su tamanño



                        // antes que todo debemos verificar si el pedido esta cancelado o solo cargamos activos ? 
                        //puse condicion de activos, pero la borro jaja
                        // esta bien
                        // si nos piden que aparezcan los cancelados
                        /// seria asi
                        /// 


                        if (pedido.status == "Cancelado")
                        {
                            pedido.subtotal = 0;
                            pedido.IVA = 0;
                            pedido.total = 0;

                        }

                        string fecha = pedido.fecha.ToShortDateString();
                        string idpedido = Convert.ToString(pedido.IdPedido);


                        string subtotal = pedido.subtotal.ToString("C"); // COn esto le indico que lo comverta a formato meneda y luego a string para que los millares a aparezcan separados por coma
                        string iva = pedido.IVA.ToString("C");
                        string total = pedido.total.ToString("C");

                        string clavemoneda = new c_MonedaContext().c_Monedas.Find(pedido.IDMoneda).ClaveMoneda; // esta es la moneda del pedido
                        string clavemonedafinal = "USD";

                        // voy a compara si la meneda el pedido viene en dolares o en otra moneda si viene en otra moneda el totalen dolares vendra del tipo de cambio de ese dia

                        if (clavemoneda == clavemonedafinal)
                        {

                            pedido.TC = 1;
                            pedido.TotalDls = pedido.subtotal; /// por que son dolares
                        }
                        else
                        {
                            // voy al tipo de cambio de ese dia
                            string cadenadetipo = "SELECT [dbo].[GetTipocambioCadena] ('" + pedido.fecha.Year + "/" + pedido.fecha.Month + "/" + pedido.fecha.Day + "','" + clavemonedafinal + "','" + clavemoneda + "') as Dato";
                            Decimal tcdeldia = db.Database.SqlQuery<ClsDatoDecimal>(cadenadetipo).ToList().FirstOrDefault().Dato;

                            pedido.TC = tcdeldia;
                            pedido.TotalDls = pedido.subtotal / tcdeldia; // aqui obtengo la conversion


                        }

                        acumuladodlsvendedor += pedido.TotalDls;
                        string totaldls = pedido.TotalDls.ToString("C");


                        // como ya tenemos los totales ya podemos imprimir su tabla de ese puro pedido

                        // las columas sera fecha, idpedido, subtotal, iva, total,moneda,status, tc, totalendolares

                        PdfPCell _PdfPCell1pedido = new PdfPCell(new Phrase(fecha, _fontStylepedido));
                        _PdfPCell1pedido.Border = 0;
                        _PdfPCell1pedido.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell1pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                        tablaparaestepedido.AddCell(_PdfPCell1pedido);

                        PdfPCell _PdfPCellespacio = new PdfPCell(new Phrase("", _fontStylepedido));
                        _PdfPCellespacio.Border = 0;
                        _PdfPCellespacio.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCellespacio.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                        tablaparaestepedido.AddCell(_PdfPCellespacio);



                        PdfPCell _PdfPCell2pedido = new PdfPCell(new Phrase(idpedido, _fontStylepedido));
                        _PdfPCell2pedido.Border = 0;
                        _PdfPCell2pedido.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell2pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                        tablaparaestepedido.AddCell(_PdfPCell2pedido);



                        PdfPCell _PdfPCell3pedido = new PdfPCell(new Phrase(subtotal, _fontStylepedido));
                        _PdfPCell3pedido.Border = 0;
                        _PdfPCell3pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                        _PdfPCell3pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                        tablaparaestepedido.AddCell(_PdfPCell3pedido);


                        PdfPCell _PdfPCell4pedido = new PdfPCell(new Phrase(iva, _fontStylepedido));
                        _PdfPCell4pedido.Border = 0;
                        _PdfPCell4pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                        _PdfPCell4pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                        tablaparaestepedido.AddCell(_PdfPCell4pedido);




                        PdfPCell _PdfPCell5pedido = new PdfPCell(new Phrase(total, _fontStylepedido));
                        _PdfPCell5pedido.Border = 0;
                        _PdfPCell5pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                        _PdfPCell5pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                        tablaparaestepedido.AddCell(_PdfPCell5pedido);

                        PdfPCell _PdfPCell6pedido = new PdfPCell(new Phrase(clavemoneda, _fontStylepedido));
                        _PdfPCell6pedido.Border = 0;
                        _PdfPCell6pedido.HorizontalAlignment = Element.ALIGN_LEFT; // 
                        _PdfPCell6pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                        tablaparaestepedido.AddCell(_PdfPCell6pedido);


                        PdfPCell _PdfPCell7pedido = new PdfPCell(new Phrase(pedido.status, _fontStylepedido));
                        _PdfPCell7pedido.Border = 0;
                        _PdfPCell7pedido.HorizontalAlignment = Element.ALIGN_LEFT; // 
                        _PdfPCell7pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                        tablaparaestepedido.AddCell(_PdfPCell7pedido);

                        PdfPCell _PdfPCell8pedido = new PdfPCell(new Phrase(pedido.TC.ToString("C"), _fontStylepedido));
                        _PdfPCell8pedido.Border = 0;
                        _PdfPCell8pedido.HorizontalAlignment = Element.ALIGN_LEFT; // 
                        _PdfPCell8pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                        tablaparaestepedido.AddCell(_PdfPCell8pedido);

                        PdfPCell _PdfPCell9pedido = new PdfPCell(new Phrase(totaldls, _fontStylepedido));
                        _PdfPCell9pedido.Border = 0;
                        _PdfPCell9pedido.HorizontalAlignment = Element.ALIGN_LEFT; // 
                        _PdfPCell9pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                        tablaparaestepedido.AddCell(_PdfPCell9pedido);

                        tablaparaestepedido.CompleteRow();

                        // ya que imprimimos en renglon acumulamos cifras

                        acumulasubtotalxcliente += pedido.subtotal;
                        acumulaivaxcliente += pedido.IVA;
                        acumuatotalxcliente += pedido.total;
                        acumuladlsxcliente += pedido.TotalDls;

                        // imprimimos la tabla que corresponde a un solo renglon del cliente

                        _documento.Add(tablaparaestepedido);




                        List<detVen> detpedidos = new List<detVen>();

                        detpedidos = GetdetVen(pedido.IdPedido); // aqui obtenemos los pedidos que corresponden a ese cliente

                        // antes de entrar al foreach de los pedidos

                        // preparado los acumuladores para imprimir totales por cluentes si quisiera pero su quiero

                        foreach (detVen dpedido in detpedidos)
                        {
                            // creamos una tabla para imprimir los los datos del cliente
                            // como son 4 columnas a imprimir 600entre las 4 

                            float[] anchoColumnasdep = { 100f, 100f, 100f, 250f, 100f, 50f };

                            // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

                            PdfPTable tablaparaestedepedido = new PdfPTable(anchoColumnasdep);
                            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                            tablaparaestedepedido.SetTotalWidth(anchoColumnasdep);
                            tablaparaestedepedido.SpacingBefore = 3;
                            tablaparaestedepedido.HorizontalAlignment = Element.ALIGN_LEFT;
                            tablaparaestedepedido.LockedWidth = true;

                            Font _fontStyledep = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL);  // aqui podremos cambiar la fuente de lcientes y su tamanño


                            PdfPCell _PdfPCell2dep = new PdfPCell(new Phrase("CREF: " + dpedido.Cref, _fontStyledep));
                            _PdfPCell2dep.Border = 0;
                            _PdfPCell2dep.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell2dep.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _PdfPCell2dep.BackgroundColor = BaseColor.WHITE;
                            tablaparaestedepedido.AddCell(_PdfPCell2dep);


                            PdfPCell _PdfPCell1depp = new PdfPCell(new Phrase("Cantidad: " + dpedido.cantidad.ToString(), _fontStyledep));
                            _PdfPCell1depp.Border = 0;
                            _PdfPCell1depp.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell1depp.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _PdfPCell1depp.BackgroundColor = BaseColor.WHITE;
                            tablaparaestedepedido.AddCell(_PdfPCell1depp);

                            PdfPCell _PdfPCell1dep = new PdfPCell(new Phrase("Suministro: " + dpedido.suministro.ToString(), _fontStyledep));
                            _PdfPCell1dep.Border = 0;
                            _PdfPCell1dep.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell1dep.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _PdfPCell1dep.BackgroundColor = BaseColor.WHITE;
                            tablaparaestedepedido.AddCell(_PdfPCell1dep);



                            PdfPCell _PdfPCell3dep = new PdfPCell(new Phrase("", _fontStyledep));
                            _PdfPCell3dep.Border = 0;
                            _PdfPCell3dep.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell3dep.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _PdfPCell3dep.BackgroundColor = BaseColor.WHITE;
                            tablaparaestedepedido.AddCell(_PdfPCell3dep);

                            //string nombrevendedor = new VendedorContext().Vendedores.Find(cliente.IDVendedor).Nombre;
                            PdfPCell _PdfPCell4dep = new PdfPCell(new Phrase("", _fontStyledep));
                            _PdfPCell4dep.Border = 0;
                            _PdfPCell4dep.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell4dep.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _PdfPCell4dep.BackgroundColor = BaseColor.WHITE;
                            tablaparaestedepedido.AddCell(_PdfPCell4dep);

                            PdfPCell _PdfPCell5dep = new PdfPCell(new Phrase("", _fontStyledep));
                            _PdfPCell5dep.Border = 0;
                            _PdfPCell5dep.HorizontalAlignment = Element.ALIGN_LEFT;
                            _PdfPCell5dep.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _PdfPCell5dep.BackgroundColor = BaseColor.WHITE;
                            tablaparaestedepedido.AddCell(_PdfPCell5dep);

                            // _pdfTable.CompleteRow();

                            // añadimos la tabla al documento princlipal para que la imprimia

                            _documento.Add(tablaparaestedepedido);
                        }


                    }

                    // cuando termine de imprimir pedidos

                    //podemos imprimir los totales por cliente

                    // ocupamos la misma estructura de pedido a 9 renglones

                    float[] anchoColumnassubtotal = { 30f, 60f, 100f, 100f, 100f, 50f, 50f, 50f, 60f };

                    // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

                    PdfPTable tablaparatotalesdelcliente = new PdfPTable(anchoColumnassubtotal);
                    //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                    tablaparatotalesdelcliente.SetTotalWidth(anchoColumnassubtotal);
                    tablaparatotalesdelcliente.SpacingBefore = 3;
                    tablaparatotalesdelcliente.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaparatotalesdelcliente.LockedWidth = true;

                    Font _fontStyletotalescliente = new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño



                    string subtotalacu = acumulasubtotalxcliente.ToString("C"); // COn esto le indico que lo comverta a formato meneda y luego a string para que los millares a aparezcan separados por coma
                    string ivaacu = acumulaivaxcliente.ToString("C");
                    string totalacu = acumuatotalxcliente.ToString("C");
                    string totaldslacu = acumuladlsxcliente.ToString("C");


                    PdfPCell _PdfPCell1total = new PdfPCell(new Phrase("", _fontStyletotalescliente)); // va sin imprimir nada es decir dejaamos esta celada en blano dado que no vamos a sumar los idpedidos o fechas
                    _PdfPCell1total.Border = 0;
                    _PdfPCell1total.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell1total.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparatotalesdelcliente.AddCell(_PdfPCell1total);


                    PdfPCell _PdfPCell2total = new PdfPCell(new Phrase("", _fontStyletotalescliente));
                    _PdfPCell2total.Border = 0;
                    _PdfPCell2total.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell2total.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparatotalesdelcliente.AddCell(_PdfPCell2total);



                    PdfPCell _PdfPCell3total = new PdfPCell(new Phrase("Importe Subtotal: " + subtotalacu, _fontStyletotalescliente));
                    _PdfPCell3total.Border = 0;
                    _PdfPCell3total.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell3total.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparatotalesdelcliente.AddCell(_PdfPCell3total);

                    PdfPCell _PdfPCell4total = new PdfPCell(new Phrase("Importe IVA: " + ivaacu, _fontStyletotalescliente));
                    _PdfPCell4total.Border = 0;
                    _PdfPCell4total.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell4total.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparatotalesdelcliente.AddCell(_PdfPCell4total);

                    PdfPCell _PdfPCell5total = new PdfPCell(new Phrase("Importe Total: " + totalacu, _fontStyletotalescliente));
                    _PdfPCell5total.Border = 0;
                    _PdfPCell5total.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell5total.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparatotalesdelcliente.AddCell(_PdfPCell5total);


                    PdfPCell _PdfPCell6total = new PdfPCell(new Phrase("", _fontStyletotalescliente));
                    _PdfPCell6total.Border = 0;
                    _PdfPCell6total.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell6total.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparatotalesdelcliente.AddCell(_PdfPCell6total);


                    PdfPCell _PdfPCell7total = new PdfPCell(new Phrase("", _fontStyletotalescliente));
                    _PdfPCell7total.Border = 0;
                    _PdfPCell7total.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell7total.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparatotalesdelcliente.AddCell(_PdfPCell7total);
                    PdfPCell _PdfPCell9total = new PdfPCell(new Phrase("Importe Total USD: " + totaldslacu, _fontStyletotalescliente));
                    _PdfPCell9total.Border = 0;
                    _PdfPCell9total.HorizontalAlignment = Element.ALIGN_LEFT; // 
                    _PdfPCell9total.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparatotalesdelcliente.AddCell(_PdfPCell9total);

                    PdfPCell _PdfPCell8total = new PdfPCell(new Phrase("", _fontStyletotalescliente));
                    _PdfPCell8total.Border = 0;
                    _PdfPCell8total.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell8total.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparatotalesdelcliente.AddCell(_PdfPCell8total);

                    

                    tablaparatotalesdelcliente.CompleteRow();


                    /// ups
                    ///  ahora imprimimos el total 
                    ///  
                    _documento.Add(tablaparatotalesdelcliente);


                    /// como me muero de ganas de ver como vamos, vamos a ver que pasa 

                    #endregion
                } // fin de los clientes
                float[] anchoColumnassubtotalven = { 300f,280f };

                // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

                PdfPTable tablaparatotalesvendedor = new PdfPTable(anchoColumnassubtotalven);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaparatotalesvendedor.SetTotalWidth(anchoColumnassubtotalven);
                tablaparatotalesvendedor.SpacingBefore = 3;
                tablaparatotalesvendedor.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaparatotalesvendedor.LockedWidth = true;
                Font _fontStyletotalesven= new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño


                PdfPCell _PdfPCellblanca = new PdfPCell(new Phrase("", _fontStyletotalesven)); // va sin imprimir nada es decir dejaamos esta celada en blano dado que no vamos a sumar los idpedidos o fechas
                _PdfPCellblanca.Border = 0;
                _PdfPCellblanca.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCellblanca.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparatotalesvendedor.AddCell(_PdfPCellblanca);


                PdfPCell _PdfPCell2totalvendedor = new PdfPCell(new Phrase("TOTAL USD VENDEDOR: " + acumuladodlsvendedor.ToString("C"), _fontStyletotalesven));
                _PdfPCell2totalvendedor.Border = 0;
                _PdfPCell2totalvendedor.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell2totalvendedor.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                tablaparatotalesvendedor.AddCell(_PdfPCell2totalvendedor);





                tablaparatotalesvendedor.CompleteRow();


                /// ups
                ///  ahora imprimimos el total 
                ///  
                _documento.Add(tablaparatotalesvendedor);

            }
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



    public class ReporteVen
    {
        [Display(Name = "fecha")]
        public DateTime fecha { get; set; }

        [Key]
        [Display(Name = "idPedido")]
        public int IdPedido { get; set; }


        [Display(Name = "subtotal")]
        public Decimal subtotal { get; set; }

        [Display(Name = "IVA")]
        public Decimal IVA { get; set; }

        [Display(Name = "total")]
        public Decimal total { get; set; }

        public int IDMoneda { get; set; }
        public decimal TC { get; set; }

        public string status { get; set; }

        public decimal TotalDls { get; set; }

    }

    public class VendedorPe
    {
        [Key]
        public int IDVendedor { get; set; }

        public string Nombre { get; set; }
        public decimal CuotaVendedor { get; set; }
        public string ClaveMoneda { get; set; }


    }
    public class ClientesPed
    {
        [Key]
        public int IDCliente { get; set; }

        public string Nombre { get; set; }

        public string Telefono { get; set; }
    }
    public class detVen
    {
        [Key]
        public int IDPedido { get; set; }
        public string Cref { get; set; }

        public decimal cantidad { get; set; }
        public decimal suministro { get; set; }
    }
    public class VendedorPeContext : DbContext
    {
        public VendedorPeContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<VendedorPeContext>(null);
        }
        public DbSet<ReporteVen> Pedidos { get; set; }
        public DbSet<ClientesPed> Clientes { get; set; }
        public DbSet<detVen> DetPedido { get; set; }
        //public DbSet<VendedorPedidoDls> Vendedor { get; set; }


    }



}