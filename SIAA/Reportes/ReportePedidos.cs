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
    public class ReportePedidos
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;
        PdfWriter _writer;
        int _totalColumn = 8;

        PdfPTable _pdfTable = new PdfPTable(8);

        PdfPCell _PdfPCell;

        public DateTime fechaini { get; set; }


        public DateTime fechafin { get; set; }

        MemoryStream _memoryStream = new MemoryStream();

        //List<ClientesPedido> clientes = new List<ClientesPedido>();
        #endregion
        public ClientesPedidoContext db = new ClientesPedidoContext();
        // aqui los puedes pasar como parametro a l reporte

        public byte[] PrepareReport(DateTime _fechaini, DateTime _fechafin)

        {
            fechaini = _fechaini;
            fechafin = _fechafin;

            //clientes = this.GetClientes();

            #region
            _documento = new Document(PageSize.LETTER, 0f, 0f, 0f, 0f);

            _documento.SetMargins(20f, 10f, 20f, 30f);

            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEvents();
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            _documento.Open();

            // _pdfTable.SetWidths(new float[] { 40f, 40f, 40f, 260f, 60f, 60f, 50f, 50f});
            #endregion
            this.ReportHeader();
            this.ReportBody();

            //  _pdfTable.HeaderRows = 4;
            //_documento.Add(_pdfTable);
            _documento.Close();


            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"Pedidos.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();


        }


        public List<ReportePedido> GetRep()
        {
            List<ReportePedido> rep = new List<ReportePedido>();
            try
            {
                string cadena = "SELECT P.idPedido, p.fecha,  p.subtotal, p.iva, p.total, p.IDMoneda, CAST(1 AS DECIMAL(8,2)) as TC, p.status, CAST(0 AS DECIMAL(8,2)) as TotalDls FROM clientes AS c INNER JOIN encPedido AS p ON c.IDCliente=p.idcliente where p.status='activo' and fecha>='" + fechaini.Year + "/" + fechaini.Month + "/" + fechaini.Day + " 00:00:01' and fecha <='" + fechafin.Year + "/" + fechafin.Month + "/" + fechafin.Day + " 23:59:59'";
                rep = db.Database.SqlQuery<ReportePedido>(cadena).ToList();
                //proveedor = db.Database.SqlQuery<Proveedor>("select * from Proveedores").ToList();
            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }

            return rep;

        }

        //public List<ClientesPedido> GetClientes()
        //{
        //    List<ClientesPedido> clientes = new List<ClientesPedido>();
        //    try
        //    {
        //        string cadena = "SELECT c.IDCliente, c.nombre as Nombre, c.Telefono , c.IDVendedor from clientes as c inner join encPedido as p on c.idcliente=p.idcliente where c.status='activo' and fecha>='" + fechaini.Year + "/" + fechaini.Month + "/" + fechaini.Day + " 00:00:01' and fecha <='" + fechafin.Year + "/" + fechafin.Month + "/" + fechafin.Day + " 23:59:59'";
        //        clientes = db.Database.SqlQuery<ClientesPedido>(cadena).ToList();


        //    }
        //    catch (SqlException err)
        //    {
        //        string mensajedeerror = err.Message;
        //    }

        //    //proveedor = db.Database.SqlQuery<Proveedor>("select * from Proveedores").ToList();
        //    return clientes;

        //}

        public List<detpedido> Getdetpedido(int idPedido)
        {
            List<detpedido> detpe = new List<detpedido>();
            string cadena = "SELECT d.IDPEdido, a.Cref, d.Cantidad, d.suministro, d.importe,  d.importeiva, d.importetotal from detpedido as d  inner join encpedido as e on d.IDPedido=e.IDpedido inner join articulo as a on d.idarticulo=a.idarticulo and e.IDpedido=" + idPedido + "";
            detpe = db.Database.SqlQuery<detpedido>(cadena).ToList();
            //proveedor = db.Database.SqlQuery<Proveedor>("selec * from Proveedores").ToList();
            return detpe;

        }


        private void ReportHeader()
        {
            #region Table head
            DateTime Hoy = DateTime.Today;
            string fecha_actual = Hoy.ToString("dd/MM/yyyy");
            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
            _PdfPCell = new PdfPCell(new Phrase(fecha_actual, _fontStyle));
            _PdfPCell.Colspan = _totalColumn;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            _PdfPCell.Border = 0;
            _PdfPCell.BackgroundColor = BaseColor.WHITE;
            _PdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_PdfPCell);



            Image jpg = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/imagenes/logo.jpg"));
            PdfPCell imageCell = new PdfPCell(jpg);
            _PdfPCell = new PdfPCell((imageCell));
            _PdfPCell.Border = 0;
            _pdfTable.AddCell(_PdfPCell);



            _fontStyle = FontFactory.GetFont("Tahoma", 15f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Reporte de Pedidos", _fontStyle));
            _PdfPCell.Colspan = _totalColumn;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.Border = 0;
            _PdfPCell.BackgroundColor = BaseColor.WHITE;
            _PdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_PdfPCell);



            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
            _PdfPCell = new PdfPCell(new Phrase("   ", _fontStyle)); // aqui te pongo la descripcion del almacen
            _PdfPCell.Colspan = _totalColumn;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.Border = 0;
            _PdfPCell.BackgroundColor = BaseColor.WHITE;
            _PdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Pedido", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Fecha", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Importe", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Importe total", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = BaseColor.GRAY;
            _pdfTable.AddCell(_PdfPCell);
            //_pdfTable.CompleteRow();

            _documento.Add(_pdfTable);

            #endregion




        }

        private void ReportBody()
        {

            #region Table Body
            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            //  
            List<ReportePedido> pedidos = new List<ReportePedido>();

            pedidos = GetRep();


            

            foreach (ReportePedido pedido in pedidos)
            {


                //if (pedido.status == "Cancelado")
                //{
                //    pedido.subtotal = 0;
                //    pedido.IVA = 0;
                //    pedido.total = 0;

                //}

                string fecha = pedido.fecha.ToShortDateString();
                string idpedido = Convert.ToString(pedido.IdPedido);


                string subtotal = pedido.subtotal.ToString("C"); // COn esto le indico que lo comverta a formato meneda y luego a string para que los millares a aparezcan separados por coma
                string iva = pedido.IVA.ToString("C");
                string total = pedido.total.ToString("C");

                //string clavemoneda = new c_MonedaContext().c_Monedas.Find(pedido.IDMoneda).ClaveMoneda; // esta es la moneda del pedido
                //string clavemonedafinal = "USD";

                // voy a compara si la meneda el pedido viene en dolares o en otra moneda si viene en otra moneda el totalen dolares vendra del tipo de cambio de ese dia

                //if (clavemoneda == clavemonedafinal)
                //{
                //    pedido.TC = 1;
                //    pedido.TotalDls = pedido.total; /// por que son dolares
                //}
                //else
                //{
                //    // voy al tipo de cambio de ese dia
                //    string cadenadetipo = "SELECT [dbo].[GetTipocambioCadena] ('" + pedido.fecha.Year + "/" + pedido.fecha.Month + "/" + pedido.fecha.Day + "','" + clavemoneda + "','" + clavemonedafinal + "') as Dato";
                //    Decimal tcdeldia = db.Database.SqlQuery<ClsDatoDecimal>(cadenadetipo).ToList().FirstOrDefault().Dato;

                //    pedido.TC = tcdeldia;
                //    pedido.TotalDls = pedido.total * pedido.TC; // aqui obtengo la conversion

                //}
                //string totaldls = pedido.TotalDls.ToString("C");

                

                // creamos una tabla para imprimir los los datos del cliente
                // como son 4 columnas a imprimir 600entre las 4 

                float[] anchoColumnasclientes = { 150f, 450f };

                // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

                PdfPTable tablaparaestecliente = new PdfPTable(anchoColumnasclientes);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaparaestecliente.SetTotalWidth(anchoColumnasclientes);
                tablaparaestecliente.SpacingBefore = 3;
                tablaparaestecliente.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaparaestecliente.LockedWidth = true;

                Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                PdfPCell _PdfPCell1 = new PdfPCell(new Phrase(pedido.IdPedido.ToString(), _fontStylecliente));
                _PdfPCell1.Border = 0;
                _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell1.BackgroundColor = BaseColor.WHITE;
                tablaparaestecliente.AddCell(_PdfPCell1);

                PdfPCell _PdfPCell2 = new PdfPCell(new Phrase(fecha, _fontStylecliente));
                _PdfPCell2.Border = 0;
                _PdfPCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell2.BackgroundColor = BaseColor.WHITE;
                tablaparaestecliente.AddCell(_PdfPCell2);


                _documento.Add(tablaparaestecliente);

                List<detpedido> detpedidos = new List<detpedido>();

                    detpedidos = Getdetpedido(pedido.IdPedido); // aqui obtenemos los pedidos que corresponden a ese cliente
                
                    foreach (detpedido dpedido in detpedidos)
                    {
                        // creamos una tabla para imprimir los los datos del cliente
                        // como son 4 columnas a imprimir 600entre las 4 

                        float[] anchoColumnasdep = { 85.5f, 85.5f, 150f, 69.7f, 69.7f, 69.7f, 69.7f };

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



                        PdfPCell _PdfPCell3dep = new PdfPCell(new Phrase("$ " + dpedido.importe.ToString(), _fontStyledep));
                        _PdfPCell3dep.Border = 0;
                        _PdfPCell3dep.HorizontalAlignment = Element.ALIGN_LEFT;
                        _PdfPCell3dep.VerticalAlignment = Element.ALIGN_MIDDLE;
                        _PdfPCell3dep.BackgroundColor = BaseColor.WHITE;
                        tablaparaestedepedido.AddCell(_PdfPCell3dep);

                    PdfPCell _PdfPCelliva = new PdfPCell(new Phrase("", _fontStyledep));
                    _PdfPCelliva.Border = 0;
                    _PdfPCelliva.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCelliva.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _PdfPCelliva.BackgroundColor = BaseColor.WHITE;
                    tablaparaestedepedido.AddCell(_PdfPCelliva);




                    //string nombrevendedor = new VendedorContext().Vendedores.Find(cliente.IDVendedor).Nombre;
                    PdfPCell _PdfPCell4dep = new PdfPCell(new Phrase("$ " + dpedido.importetotal.ToString(), _fontStyledep));
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
           



                #endregion
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



    public class ReportePedido
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
        //public virtual c_Moneda  Moneda { get; set; }

        public decimal TC { get; set; }

        public string status { get; set; }

        public decimal TotalDls { get; set; }

    }

    //public class ClientePedido
    //{
    //    [Key]
    //    public int IDCliente { get; set; }

    //    public string Nombre { get; set; }

    //    public string Telefono { get; set; }

    //    public int IDVendedor { get; set; }

    //    //public virtual Vendedor vendedor { get; set; }
    //}

    public class detpedido
    {
        [Key]
        public int IDPedido { get; set; }
        public string Cref { get; set; }

        public decimal cantidad { get; set; }
        public decimal suministro { get; set; }
        public decimal importe { get; set; }
        public decimal importetotal { get; set; }
        public decimal importeiva { get; set; }

    }
    public class PedidosContext : DbContext
    {
        public PedidosContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<PedidosContext>(null);
        }
        public DbSet<ReportePedido> Pedidos { get; set; }
        //public DbSet<ClientePedido> Clientes { get; set; }
        public DbSet<detpedido> DetPedido { get; set; }

    }



}