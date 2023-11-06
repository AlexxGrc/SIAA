using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.clasescfdi;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;


namespace SIAAPI.Reportes
{
    public class AntiguedadSaldos
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;
        Font _fontStylepedido;
        Font _fontStylsumas;
        Font _fontStylsumasT;


        public string _tc = "";
        PdfWriter _writer;
        int _totalColumn = 10;

        PdfPTable tablaparaestepedido = new PdfPTable(9);
        PdfPTable tablaparaestecontacto = new PdfPTable(5);
        PdfPTable tablaparaestepedidoMXN = new PdfPTable(9);
        PdfPTable tablaparaestepedidoEUR = new PdfPTable(9);
        PdfPTable saldosInsolutos = new PdfPTable(3);
        PdfPTable saldosPorfecha = new PdfPTable(9);
        PdfPTable saldosPorfechaUSD = new PdfPTable(9);
        PdfPTable sumasPorfechaUSD = new PdfPTable(9);
        PdfPTable sumasPorfecha = new PdfPTable(9);
        PdfPTable vacio = new PdfPTable(1);
        PdfPTable TT = new PdfPTable(3);
        PdfPTable _pdfTable = new PdfPTable(9);
        PdfPTable tablae = new PdfPTable(10);
        PdfPTable tablalinea = new PdfPTable(1);





        public int idvendedor { get; set; }
        public int idcliente { get; set; }

        MemoryStream _memoryStream = new MemoryStream();

        List<Clientes> clientes = new List<Clientes>();
      
        #endregion
        public ClientesFacturasAContext db = new ClientesFacturasAContext();
        // aqui los puedes pasar como parametro a l reporte
        public CMYKColor colordefinido;
        public CMYKColor colorfuente;
        public byte[] PrepareReport(int _idcliente)

        {
            
            idcliente = _idcliente;
            clientes = this.GetClientes(idcliente);

            #region
            _documento = new Document(PageSize.LETTER, 0f, 0f, 0f, 0f);

            _documento.SetMargins(20f, 10f, 20f, 30f);

            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEvents();
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            tablae.WidthPercentage = 100;
            tablae.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaparaestepedido.WidthPercentage = 100;
            tablaparaestepedido.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaparaestepedidoEUR.WidthPercentage = 100;
            tablaparaestepedidoEUR.HorizontalAlignment = Element.ALIGN_LEFT;
            tablaparaestepedidoMXN.WidthPercentage = 100;
            tablaparaestepedidoMXN.HorizontalAlignment = Element.ALIGN_LEFT;
            _documento.Open();

            // _pdfTable.SetWidths(new float[] { 40f, 40f, 40f, 260f, 60f, 60f, 50f, 50f});
            #endregion
            Empresa empresa = new EmpresaContext().empresas.Find(2);
            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);
            ClsColoresReporte colorr = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorReporte);
            colordefinido = colorr.color;

            ClsColoresReporte colorf = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado);
            this.colorfuente = colorf.color;

            this.AgregarLogo(logoempresa);
            this.ReportHeader();
            this.ReportBody();

            //  _pdfTable.HeaderRows = 4;
            //_documento.Add(_pdfTable);
            _documento.Close();


            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"Saldos.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();


        }


        public ReporteSaldoA GetEncabezado(int idcliente)
        {
            ReporteSaldoA encabezado = new ReporteSaldoA();
            VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambioCadena(GETDATE(),'USD','MXN') as TC").ToList()[0];
            encabezado.TC = cambio.TC;

            if (idcliente == 0)
            {
                encabezado.Rep = 100;
                encabezado.Titulo = "Todos los clientes";

            }

            if (idcliente != 0)
            {
                //////////////////////////// trae aqui
                encabezado.Rep = 110;
                string cadenaE = "select * from dbo.Clientes where IDCliente = " + idcliente + "";
                Clientes enc = db.Database.SqlQuery<Clientes>(cadenaE).ToList().FirstOrDefault();
                encabezado.Titulo = "Cliente: '" + enc.Nombre + "'";
            }


            return encabezado;

        }

        public List<Clientes> GetClientes(int idcliente)
        {
            List<Clientes> clientes = new List<Clientes>();

            try
            {
                if (idcliente != 0)
                {
                    string cadena = "select c.* from Clientes as c  where c.IDCliente=" + idcliente;
                    clientes = db.Database.SqlQuery<Clientes>(cadena).ToList();
                }
                else
                {
                    string cadena = "SELECT distinct c.idcliente, c.* FROM CLIENTES AS C INNER JOIN enCfacturas as e ON C.IDCLIENTE=e.idcliente inner join SaldoFactura as s on s.idFactura=e.id  where s.ImporteSaldoInsoluto>0 and e.Estado='A' order by c.nombre, c.idcliente";
                    clientes = db.Database.SqlQuery<Clientes>(cadena).ToList();
                }



            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }


            return clientes;

        }


        public ContactosClie GetContactoFinanzas(int idCliente)
        {
            ContactosClie detcont = new ContactosClie();
            string cadena = "select * from dbo.ContactosClie where (Puesto= 'FINANZAS' or puesto = 'PAGOS') and IDCliente = " + idCliente + "";
            detcont = db.Database.SqlQuery<ContactosClie>(cadena).ToList().FirstOrDefault();
            return detcont;
        }

        public List<EncFacA> GetFacturaUSD(int idCliente)
        {
            List<EncFacA> detpe = new List<EncFacA>();
            string cadena = "select distinct e.id, e.numero, e.serie, e.subtotal, e.iva, e.total, e.moneda, e.fechaVencimiento, e.FechaRevision, s.ImporteSaldoInsoluto, e.Fecha,e.TC  from enCfacturas as e inner join SaldoFactura as s on s.idFactura=e.id  where s.ImporteSaldoInsoluto>0 and idcliente=" + idCliente + " and e.moneda='USD' and e.Estado='A'";
            detpe = db.Database.SqlQuery<EncFacA>(cadena).ToList();
            //proveedor = db.Database.SqlQuery<Proveedor>("selec * from Proveedores").ToList();
            return detpe;

        }

        public List<EncFacA> GetFacturaMXN(int idCliente)
        {
            List<EncFacA> detpe = new List<EncFacA>();
            string cadena = "select distinct e.id, e.numero, e.serie, e.subtotal, e.iva, e.total, e.moneda, e.fechaVencimiento, e.FechaRevision, s.ImporteSaldoInsoluto, e.Fecha,e.TC  from enCfacturas as e inner join SaldoFactura as s on s.idFactura=e.id  where s.ImporteSaldoInsoluto>0 and idcliente=" + idCliente + " and e.moneda='MXN' and e.Estado='A'";
            detpe = db.Database.SqlQuery<EncFacA>(cadena).ToList();
            //proveedor = db.Database.SqlQuery<Proveedor>("selec * from Proveedores").ToList();
            return detpe;

        }
        public List<EncFacA> GetSumaTotalMXN(int idCliente)
        {
            List<EncFacA> detpe = new List<EncFacA>();
            //string cadena = "SELECT SUM(subtabla.SUMA) as Dato FROM (select distinct e.id, e.numero, e.serie, e.subtotal, e.iva, e.total, e.moneda, e.fechaVencimiento, e.FechaRevision, s.ImporteSaldoInsoluto AS SUMA, e.Fecha,e.TC  from enCfacturas as e inner join SaldoFactura as s on s.idFactura=e.id  where s.ImporteSaldoInsoluto>0 and idcliente="+idCliente +" and e.moneda='MXN' and e.Estado='A') AS subtabla";
            string cadena = "select distinct e.id, e.numero, e.serie, e.subtotal, e.iva, e.total, e.moneda, e.fechaVencimiento, e.FechaRevision, s.ImporteSaldoInsoluto, e.Fecha,e.TC  from enCfacturas as e inner join SaldoFactura as s on s.idFactura=e.id  where s.ImporteSaldoInsoluto>0 and idcliente=" + idCliente + " and e.moneda='MXN' and e.Estado='A'";

            detpe = db.Database.SqlQuery<EncFacA>(cadena).ToList();
            //proveedor = db.Database.SqlQuery<Proveedor>("selec * from Proveedores").ToList();
            return detpe;

        }
        public List<EncFacA> GetSumaTotalUSD(int idCliente)
        {
            List<EncFacA> detpe = new List<EncFacA>();
            //string cadena = "SELECT SUM(subtabla.SUMA) as Dato FROM (select distinct e.id, e.numero, e.serie, e.subtotal, e.iva, e.total, e.moneda, e.fechaVencimiento, e.FechaRevision, s.ImporteSaldoInsoluto AS SUMA, e.Fecha,e.TC  from enCfacturas as e inner join SaldoFactura as s on s.idFactura=e.id  where s.ImporteSaldoInsoluto>0 and idcliente="+idCliente +" and e.moneda='MXN' and e.Estado='A') AS subtabla";
            string cadena = "select distinct e.id, e.numero, e.serie, e.subtotal, e.iva, e.total, e.moneda, e.fechaVencimiento, e.FechaRevision, s.ImporteSaldoInsoluto, e.Fecha,e.TC  from enCfacturas as e inner join SaldoFactura as s on s.idFactura=e.id  where s.ImporteSaldoInsoluto>0 and idcliente=" + idCliente + " and e.moneda='USD' and e.Estado='A'";

            detpe = db.Database.SqlQuery<EncFacA>(cadena).ToList();
            //proveedor = db.Database.SqlQuery<Proveedor>("selec * from Proveedores").ToList();
            return detpe;

        }

        public List<EncFacA> GetFacturaEUR(int idCliente)
        {
            List<EncFacA> detpe = new List<EncFacA>();
            string cadena = "select distinct e.id, e.numero, e.serie, e.subtotal, e.iva, e.total, e.moneda, e.fechaVencimiento, e.FechaRevision, s.ImporteSaldoInsoluto, e.Fecha,e.TC  from enCfacturas as e inner join SaldoFactura as s on s.idFactura=e.id  where s.ImporteSaldoInsoluto>0 and idcliente=" + idCliente + " and e.moneda='EUR' and e.Estado='A' ";
            detpe = db.Database.SqlQuery<EncFacA>(cadena).ToList();
            //proveedor = db.Database.SqlQuery<Proveedor>("selec * from Proveedores").ToList();
            return detpe;

        }

        public MonedaSalA GetMoneda(int idCliente, int id)
        {
            MonedaSalA detpe = new MonedaSalA();
            string cadena = "select m.idMoneda, m.ClaveMoneda  from c_Moneda as m inner join EncFacturas as e on m.idmoneda=e.idmoneda  where e.idcliente=" + idCliente + "  and e.id=" + id + "";
            detpe = db.Database.SqlQuery<MonedaSalA>(cadena).ToList().FirstOrDefault();
            //proveedor = db.Database.SqlQuery<Proveedor>("selec * from Proveedores").ToList();
            return detpe;

        }


        private void ReportHeader()
        {
            #region Table head


            ReporteSaldoA enc;
            enc = GetEncabezado(idcliente);
            PdfPCell _PdfPCell;

            _fontStyle = FontFactory.GetFont("Tahoma", 12f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Reporte Antiguedad de Saldos", _fontStyle));
            _PdfPCell.Colspan = _totalColumn;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.Border = 0;

            _PdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
            _PdfPCell = new PdfPCell(new Phrase(enc.Titulo.ToString(), _fontStyle));
            _PdfPCell.Colspan = _totalColumn;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.Border = 0;

            _PdfPCell.ExtraParagraphSpace = 0;
            //  _PdfPCell.PaddingLeft = 100;
            _pdfTable.AddCell(_PdfPCell);

            _pdfTable.CompleteRow();

            _documento.Add(_pdfTable);

            float[] anchoColumnasfechas = { 100f, 50f, 25f, 225f, 100f, 100f };

            PdfPTable tablafechas = new PdfPTable(anchoColumnasfechas);
            //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

            tablafechas.SetTotalWidth(anchoColumnasfechas);
            tablafechas.SpacingBefore = 0;
            tablafechas.HorizontalAlignment = Element.ALIGN_LEFT;
            tablafechas.LockedWidth = true;
            Font _fontStylefecha = new Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

            PdfPCell _tablafechas1 = new PdfPCell(new Phrase("", _fontStylefecha));
            _tablafechas1.Border = 0;
            _tablafechas1.FixedHeight = 20f;
            _tablafechas1.HorizontalAlignment = Element.ALIGN_RIGHT;
            _tablafechas1.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas1.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas1);


            PdfPCell _tablafechas2 = new PdfPCell(new Phrase("", _fontStylefecha));
            _tablafechas2.Border = 0;
            _tablafechas2.FixedHeight = 10f;
            _tablafechas2.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablafechas2.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas2.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas2);

            PdfPCell _tablafechas3 = new PdfPCell(new Phrase("", _fontStylefecha));
            _tablafechas3.Border = 0;
            _tablafechas3.FixedHeight = 10f;
            _tablafechas3.HorizontalAlignment = Element.ALIGN_LEFT;
            _tablafechas3.VerticalAlignment = Element.ALIGN_MIDDLE;
            _tablafechas3.BackgroundColor = BaseColor.WHITE;
            tablafechas.AddCell(_tablafechas3);


            PdfPCell _tablafechas4 = new PdfPCell(new Phrase("", _fontStylefecha));
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


            float[] anchoColumnasenccontenido = { 50f, 40f, 60f, 60f, 100f, 40f, 70f, 60f,60f,60f};
            PdfPTable tablaencabezado = new PdfPTable(anchoColumnasenccontenido);

            tablaencabezado.WidthPercentage = 100;
            tablaencabezado.HorizontalAlignment = Element.ALIGN_LEFT;

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1,colorfuente);
            _PdfPCell = new PdfPCell(new Phrase("Fecha", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;

            tablaencabezado.AddCell(_PdfPCell);

            

            _PdfPCell = new PdfPCell(new Phrase("No.", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido; ;
            tablaencabezado.AddCell(_PdfPCell);


            _PdfPCell = new PdfPCell(new Phrase("Fecha Rev.", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido; ;
            tablaencabezado.AddCell(_PdfPCell);


            _PdfPCell = new PdfPCell(new Phrase("Fecha Ven.", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablaencabezado.AddCell(_PdfPCell);


            _PdfPCell = new PdfPCell(new Phrase("Total", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido; ;
            tablaencabezado.AddCell(_PdfPCell);


            
            _PdfPCell = new PdfPCell(new Phrase("TC", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablaencabezado.AddCell(_PdfPCell);


            _PdfPCell = new PdfPCell(new Phrase("Al corriente", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablaencabezado.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("0 - 30 días", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido; ;
            tablaencabezado.AddCell(_PdfPCell);

            _PdfPCell = new PdfPCell(new Phrase("31 - 90 días", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido; ;
            tablaencabezado.AddCell(_PdfPCell);


            _PdfPCell = new PdfPCell(new Phrase("91 o más días", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido; ;
            tablaencabezado.AddCell(_PdfPCell);


            _documento.Add(tablae);

            _documento.Add(tablaencabezado);


            #endregion




        }

        private void ReportBody()
        {

            #region Table Body


            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            //  
            decimal acuclienteusd = 0M;
            decimal acuclientMXN = 0M;
            decimal acuclientEUR = 0M;
            decimal Tacuclienteusd = 0M;
            decimal TacuclientMXN = 0M;
            decimal TacuclientEUR = 0M;
            decimal sumaTotalfechacorriente = 0M;
            decimal sumaTotalfecha030 = 0M;
            decimal sumaTotalfecha3190 = 0M;
            decimal sumaTotalfecha91 = 0M;

            decimal sumaTotalfechacorrienteUSD = 0M;
            decimal sumaTotalfecha030USD = 0M;
            decimal sumaTotalfecha3190USD = 0M;
            decimal sumaTotalfecha91USD = 0M;





            foreach (Clientes cliente in clientes)
            {
                // creamos una tabla para imprimir los los datos del cliente
                // como son 4 columnas a imprimir 600entre las 4 


                float[] anchoColumnasclientes = { 30f, 250f, 150f, 170f };

                // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

                PdfPTable tablaparaestecliente = new PdfPTable(anchoColumnasclientes);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaparaestecliente.SetTotalWidth(anchoColumnasclientes);
                tablaparaestecliente.SpacingBefore = 3;
                tablaparaestecliente.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaparaestecliente.LockedWidth = true;
                tablaparaestecliente.WidthPercentage = 100;
                tablaparaestecliente.HorizontalAlignment = Element.ALIGN_LEFT;

                Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                PdfPCell _PdfPCell1 = new PdfPCell(new Phrase(cliente.noExpediente, _fontStylecliente));
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

                string nombrevendedor = new VendedorContext().Vendedores.Find(cliente.IDVendedor).Nombre;
                PdfPCell _PdfPCell4 = new PdfPCell(new Phrase(nombrevendedor, new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD)));
                _PdfPCell4.Border = 0;
                _PdfPCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell4.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell4.BackgroundColor = BaseColor.WHITE;
                tablaparaestecliente.AddCell(_PdfPCell4);


                _documento.Add(tablaparaestecliente);



                //-----
                try
                {
                    ContactosClie DatosContacto = new ContactosClie();
                    DatosContacto = GetContactoFinanzas(cliente.IDCliente);

                    float[] anchoColumnasContacto = { 100f, 150f, 100, 150f, 50f };


                    tablaparaestecontacto = new PdfPTable(anchoColumnasContacto);
                    tablaparaestecontacto.WidthPercentage = 100;
                    tablaparaestecontacto.HorizontalAlignment = Element.ALIGN_LEFT;
                    _fontStylepedido = FontFactory.GetFont("Tahoma", 7f, Font.BOLD);

                    PdfPCell _PdfPCellContacto1 = new PdfPCell(new Phrase("Contacto cobranza: ", _fontStylepedido));
                    _PdfPCellContacto1.Border = 0;
                    _PdfPCellContacto1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _PdfPCellContacto1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestecontacto.AddCell(_PdfPCellContacto1);
                    PdfPCell _PdfPCellContacto2 = new PdfPCell(new Phrase(DatosContacto.Nombre, _fontStylepedido));
                    _PdfPCellContacto2.Border = 0;
                    _PdfPCellContacto2.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellContacto2.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestecontacto.AddCell(_PdfPCellContacto2);

                    PdfPCell _PdfPCellContacto3 = new PdfPCell(new Phrase(DatosContacto.Telefono, _fontStylepedido));
                    _PdfPCellContacto3.Border = 0;
                    _PdfPCellContacto3.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellContacto3.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestecontacto.AddCell(_PdfPCellContacto3);

                    PdfPCell _PdfPCellContacto4 = new PdfPCell(new Phrase(DatosContacto.Email, _fontStylepedido));
                    _PdfPCellContacto4.Border = 0;
                    _PdfPCellContacto4.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellContacto4.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestecontacto.AddCell(_PdfPCellContacto4);

                    PdfPCell _PdfPCellContacto5 = new PdfPCell(new Phrase("", _fontStylepedido));
                    _PdfPCellContacto5.Border = 0;
                    _PdfPCellContacto5.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellContacto5.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestecontacto.AddCell(_PdfPCellContacto5);

                    _documento.Add(tablaparaestecontacto);
                    DatosContacto = null;
                }
                catch (Exception err)
                { }
                //-----



                List<EncFacA> FACTURAUSD = new List<EncFacA>();

                FACTURAUSD = GetFacturaUSD(cliente.IDCliente); // aqui obtenemos los pedidos que corresponden a ese cliente
                float[] anchoColumnascontenido = { 50f, 40f, 60f, 60f, 100f, 40f, 70f, 60f, 60f, 60f };
                acuclienteusd = 0M;
                foreach (EncFacA fac in FACTURAUSD)
                {


                    ClsOperacionesFecha fechas = new ClsOperacionesFecha();
                    fechas.fechaini = fac.FechaVencimiento;
                    fechas.fechafin = DateTime.Now;


                    // List<MonedaSal> moneda = new List<MonedaSal>();

                    //   moneda = GetMoneda(cliente.IDCliente, fac.ID);


                    tablaparaestepedido = new PdfPTable(anchoColumnascontenido);
                    tablaparaestepedido.WidthPercentage = 100;
                    tablaparaestepedido.HorizontalAlignment = Element.ALIGN_LEFT;
                    _fontStylepedido = FontFactory.GetFont("Tahoma", 7f, Font.NORMAL);

                    PdfPCell _PdfPCellf = new PdfPCell(new Phrase(fac.Fecha.ToShortDateString(), _fontStylepedido));
                    _PdfPCellf.Border = 0;
                    _PdfPCellf.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellf.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCellf);



                    PdfPCell _PdfPCellespacio = new PdfPCell(new Phrase(fac.Serie + " "+ fac.Numero.ToString(), _fontStylepedido));
                    _PdfPCellespacio.Border = 0;
                    _PdfPCellespacio.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellespacio.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCellespacio);


                    

                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                    PdfPCell _PdfPCell3pedido = new PdfPCell(new Phrase(fac.FechaRevision.ToShortDateString(), _fontStylepedido));
                    _PdfPCell3pedido.Border = 0;
                    _PdfPCell3pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell3pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCell3pedido);

                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                    PdfPCell _PdfPCell4pedido = new PdfPCell(new Phrase(fac.FechaVencimiento.ToShortDateString(), _fontStylepedido));
                    _PdfPCell4pedido.Border = 0;
                    _PdfPCell4pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell4pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCell4pedido);



                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                    PdfPCell _PdfPCell5pedido = new PdfPCell(new Phrase(fac.Total.ToString("C") + " "+ fac.Moneda, _fontStylepedido));
                    _PdfPCell5pedido.Border = 0;
                    _PdfPCell5pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell5pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCell5pedido);
                    //   _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                 

                    PdfPCell _PdfPCell61pedido = new PdfPCell(new Phrase(fac.TC.ToString("C"), _fontStylepedido));
                    _PdfPCell61pedido.Border = 0;
                    _PdfPCell61pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCell61pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCell61pedido);



                    PdfPCell _PdfPCellii;
                    if (fechas.getcorriente())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCellii = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    }
                    else
                    {
                        _PdfPCellii = new PdfPCell(new Phrase(0.ToString("C"), _fontStylepedido));
                    }

                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);

                    _PdfPCellii.Border = 0;
                    _PdfPCellii.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _PdfPCellii.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCellii);





                    PdfPCell _PdfPCell7pedido;
                    if (fechas.get30())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                         _PdfPCell7pedido = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    }
                    else
                    {
                         _PdfPCell7pedido = new PdfPCell(new Phrase(0.ToString("C"), _fontStylepedido));
                    }
                    _PdfPCell7pedido.Border = 0;
                    _PdfPCell7pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCell7pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCell7pedido);
                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);


                    PdfPCell _PdfPCell2pedido;
                    if (fechas.get3190())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCell2pedido = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    }
                    else
                    {
                        _PdfPCell2pedido = new PdfPCell(new Phrase(0.ToString("C"), _fontStylepedido));
                    }
                                       
                    
                    _PdfPCell2pedido.Border = 0;
                    _PdfPCell2pedido.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _PdfPCell2pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCell2pedido);


                    PdfPCell _PdfPCelli;
                    if (fechas.get91mas())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCelli = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    }
                    else
                    {
                        _PdfPCelli = new PdfPCell(new Phrase(0.ToString("C"), _fontStylepedido));
                    }

                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                   
                    _PdfPCelli.Border = 0;
                    _PdfPCelli.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _PdfPCelli.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCelli);


                   


                    acuclienteusd += fac.ImporteSaldoInsoluto;

                    tablaparaestepedido.CompleteRow();
                    tablaparaestepedido.SpacingAfter = 1;
                    _documento.Add(tablaparaestepedido);

                }
                Tacuclienteusd += acuclienteusd;



                List<EncFacA> FACTURAMXN = new List<EncFacA>();
                List<EncFacA> FACTURASALDOMXN = new List<EncFacA>();

                FACTURAMXN = GetFacturaMXN(cliente.IDCliente); // aqui obtenemos los pedidos que corresponden a ese cliente
                FACTURASALDOMXN = GetSumaTotalMXN(cliente.IDCliente);
                acuclientMXN = 0M;
                foreach (EncFacA fac in FACTURAMXN)
                {

                    ClsOperacionesFecha fechas = new ClsOperacionesFecha();
                    fechas.fechaini = fac.FechaVencimiento;
                    fechas.fechafin = DateTime.Now;


                    // List<MonedaSal> moneda = new List<MonedaSal>();

                    //   moneda = GetMoneda(cliente.IDCliente, fac.ID);

                    tablaparaestepedidoMXN = new PdfPTable(anchoColumnascontenido);
                    tablaparaestepedidoMXN.WidthPercentage = 100;
                    tablaparaestepedidoMXN.HorizontalAlignment = Element.ALIGN_LEFT;
                    _fontStylepedido = FontFactory.GetFont("Tahoma", 7f, Font.NORMAL);
                    PdfPCell _PdfPCellf = new PdfPCell(new Phrase(fac.Fecha.ToShortDateString(), _fontStylepedido));
                    _PdfPCellf.Border = 0;
                    _PdfPCellf.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellf.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCellf);





                    PdfPCell _PdfPCellespacio = new PdfPCell(new Phrase(fac.Serie + " " + fac.Numero.ToString(), _fontStylepedido));
                    _PdfPCellespacio.Border = 0;
                    _PdfPCellespacio.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellespacio.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCellespacio);




                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                    PdfPCell _PdfPCell3pedido = new PdfPCell(new Phrase(fac.FechaRevision.ToShortDateString(), _fontStylepedido));
                    _PdfPCell3pedido.Border = 0;
                    _PdfPCell3pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell3pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCell3pedido);

                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                    PdfPCell _PdfPCell4pedido = new PdfPCell(new Phrase(fac.FechaVencimiento.ToShortDateString(), _fontStylepedido));
                    _PdfPCell4pedido.Border = 0;
                    _PdfPCell4pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell4pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCell4pedido);



                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                    PdfPCell _PdfPCell5pedido = new PdfPCell(new Phrase(fac.Total.ToString("C") + " " + fac.Moneda, _fontStylepedido));
                    _PdfPCell5pedido.Border = 0;
                    _PdfPCell5pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell5pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCell5pedido);
                    

                    PdfPCell _PdfPCell61pedido = new PdfPCell(new Phrase(fac.TC.ToString("C"), _fontStylepedido));
                    _PdfPCell61pedido.Border = 0;
                    _PdfPCell61pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCell61pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCell61pedido);

                    PdfPCell _PdfPCellii;
                    if (fechas.getcorriente())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCellii = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    }
                    else
                    {
                        _PdfPCellii = new PdfPCell(new Phrase(0.ToString("C"), _fontStylepedido));
                    }

                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);

                    _PdfPCellii.Border = 0;
                    _PdfPCellii.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _PdfPCellii.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCellii);





                    PdfPCell _PdfPCell7pedido;
                    if (fechas.get30())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCell7pedido = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    }
                    else
                    {
                        _PdfPCell7pedido = new PdfPCell(new Phrase(0.ToString("C"), _fontStylepedido));
                    }
                    _PdfPCell7pedido.Border = 0;
                    _PdfPCell7pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCell7pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCell7pedido);
                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);


                    PdfPCell _PdfPCell2pedido;
                    if (fechas.get3190())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCell2pedido = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    }
                    else
                    {
                        _PdfPCell2pedido = new PdfPCell(new Phrase(0.ToString("C"), _fontStylepedido));
                    }


                    _PdfPCell2pedido.Border = 0;
                    _PdfPCell2pedido.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _PdfPCell2pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCell2pedido);


                    PdfPCell _PdfPCelli;
                    if (fechas.get91mas())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCelli = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    }
                    else
                    {
                        _PdfPCelli = new PdfPCell(new Phrase(0.ToString("C"), _fontStylepedido));
                    }

                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);

                    _PdfPCelli.Border = 0;
                    _PdfPCelli.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _PdfPCelli.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCelli);

                    tablaparaestepedidoMXN.CompleteRow();
                    tablaparaestepedidoMXN.SpacingAfter = 1;
                    _documento.Add(tablaparaestepedidoMXN);
                    acuclientMXN += fac.ImporteSaldoInsoluto;

                }
                TacuclientMXN += acuclientMXN;


                List<EncFacA> FACTURAEUR = new List<EncFacA>();
                List<EncFacA> FACTURASUMAUSD = new List<EncFacA>();

                FACTURAEUR = GetFacturaEUR(cliente.IDCliente);
                FACTURASUMAUSD =GetSumaTotalUSD(cliente.IDCliente);
                // aqui obtenemos los pedidos que corresponden a ese cliente


                acuclientEUR = 0M;
                foreach (EncFacA fac in FACTURAEUR)
                {
                    ClsOperacionesFecha fechas = new ClsOperacionesFecha();
                    fechas.fechaini = fac.FechaVencimiento;
                    fechas.fechafin = DateTime.Now;

                    // List<MonedaSal> moneda = new List<MonedaSal>();

                    //   moneda = GetMoneda(cliente.IDCliente, fac.ID);

                    tablaparaestepedidoEUR = new PdfPTable(anchoColumnascontenido);

                    tablaparaestepedidoEUR.WidthPercentage = 100;
                    tablaparaestepedidoEUR.HorizontalAlignment = Element.ALIGN_LEFT;
                    _fontStylepedido = FontFactory.GetFont("Tahoma", 7f, Font.NORMAL);
                    PdfPCell _PdfPCellf = new PdfPCell(new Phrase(fac.Fecha.ToShortDateString(), _fontStylepedido));
                    _PdfPCellf.Border = 0;
                    _PdfPCellf.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellf.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCellf);




                    PdfPCell _PdfPCellespacio = new PdfPCell(new Phrase(fac.Serie + " " + fac.Numero.ToString(), _fontStylepedido));
                    _PdfPCellespacio.Border = 0;
                    _PdfPCellespacio.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellespacio.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCellespacio);




                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                    PdfPCell _PdfPCell3pedido = new PdfPCell(new Phrase(fac.FechaRevision.ToShortDateString(), _fontStylepedido));
                    _PdfPCell3pedido.Border = 0;
                    _PdfPCell3pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell3pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell3pedido);

                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                    PdfPCell _PdfPCell4pedido = new PdfPCell(new Phrase(fac.FechaVencimiento.ToShortDateString(), _fontStylepedido));
                    _PdfPCell4pedido.Border = 0;
                    _PdfPCell4pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell4pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell4pedido);



                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                    PdfPCell _PdfPCell5pedido = new PdfPCell(new Phrase(fac.Total.ToString("C") + " " + fac.Moneda, _fontStylepedido));
                    _PdfPCell5pedido.Border = 0;
                    _PdfPCell5pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell5pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell5pedido);
                    
                    PdfPCell _PdfPCell61pedido = new PdfPCell(new Phrase(fac.TC.ToString("C"), _fontStylepedido));
                    _PdfPCell61pedido.Border = 0;
                    _PdfPCell61pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCell61pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell61pedido);


                    PdfPCell _PdfPCellii;
                    if (fechas.getcorriente())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCellii = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    }
                    else
                    {
                        _PdfPCellii = new PdfPCell(new Phrase(0.ToString("C"), _fontStylepedido));
                    }

                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);

                    _PdfPCellii.Border = 0;
                    _PdfPCellii.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _PdfPCellii.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCellii);





                    PdfPCell _PdfPCell7pedido;
                    if (fechas.get30())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCell7pedido = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    }
                    else
                    {
                        _PdfPCell7pedido = new PdfPCell(new Phrase(0.ToString("C"), _fontStylepedido));
                    }
                    _PdfPCell7pedido.Border = 0;
                    _PdfPCell7pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCell7pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell7pedido);
                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);


                   

                    PdfPCell _PdfPCell2pedido;
                    if (fechas.get3190())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCell2pedido = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    }
                    else
                    {
                        _PdfPCell2pedido = new PdfPCell(new Phrase(0.ToString("C"), _fontStylepedido));
                    }


                    _PdfPCell2pedido.Border = 0;
                    _PdfPCell2pedido.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _PdfPCell2pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell2pedido);


                    PdfPCell _PdfPCelli;
                    if (fechas.get91mas())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCelli = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    }
                    else
                    {
                        _PdfPCelli = new PdfPCell(new Phrase(0.ToString("C"), _fontStylepedido));
                    }

                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);

                    _PdfPCelli.Border = 0;
                    _PdfPCelli.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _PdfPCelli.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCelli);

                    tablaparaestepedidoEUR.CompleteRow();
                    tablaparaestepedidoEUR.SpacingAfter = 1;
                    _documento.Add(tablaparaestepedidoEUR);
                    acuclientEUR += fac.ImporteSaldoInsoluto;
                }

                TacuclientEUR += acuclientEUR;

                /////////////// saldos por fechas MXN
                ///
                decimal Tsumaimpormx30 = 0;
                decimal Tsumaimpormx3190 = 0M;
                decimal Tsumaimpormx91 = 0M;
                decimal Tsumaimpormxcorriente = 0;
                decimal sumaimpormx30 = 0M;
                decimal sumaimpormx3190 = 0M;
                decimal sumaimpormx91 = 0M;
                decimal sumaimpormxcorriente = 0M;
                string monedaFca = "";

                foreach (EncFacA fac in FACTURASALDOMXN)
                {
                    ClsOperacionesFecha fechas = new ClsOperacionesFecha();
                    fechas.fechaini = fac.FechaVencimiento;
                    fechas.fechafin = DateTime.Now;
                    //saldosPorfecha = new PdfPTable(10);

                    _fontStylepedido = FontFactory.GetFont("Tahoma", 6f, 1);


                    PdfPCell _PdfPCellii;
                    if (fechas.getcorriente())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCellii = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                        sumaimpormxcorriente += fac.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        _PdfPCellii = new PdfPCell(new Phrase("$0.00", _fontStylepedido));
                    }


                    PdfPCell _PdfPCellSaldoFecha;
                    if (fechas.get30())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCellSaldoFecha = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                        sumaimpormx30 += fac.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        _PdfPCellSaldoFecha = new PdfPCell(new Phrase("$0.00", _fontStylepedido));
                    }
                    _PdfPCellSaldoFecha.Border = 0;
                    _PdfPCellSaldoFecha.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCellSaldoFecha.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    

                    PdfPCell _PdfPCellSaldoFecha1;
                    if (fechas.get3190())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCellSaldoFecha1 = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                        sumaimpormx3190 += fac.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        _PdfPCellSaldoFecha1 = new PdfPCell(new Phrase("$0.00", _fontStylepedido));
                    }


                    _PdfPCellSaldoFecha1.Border = 0;
                    _PdfPCellSaldoFecha1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _PdfPCellSaldoFecha1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    

                    PdfPCell _PdfPCellSaldoFecha2;
                    if (fechas.get91mas())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCellSaldoFecha2 = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                        sumaimpormx91 += fac.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        _PdfPCellSaldoFecha2 = new PdfPCell(new Phrase("$0.00", _fontStylepedido));
                    }

                    monedaFca = fac.Moneda;

                }
                Tsumaimpormxcorriente += sumaimpormxcorriente;
                Tsumaimpormx30 += sumaimpormx30;
                Tsumaimpormx3190 += sumaimpormx3190;
                Tsumaimpormx91 += sumaimpormx91;
                string MonFa = monedaFca;
                //float[] anchoColumnascontenidos = { 100f, 100f, 60f, 60f, 100f, 100f, 70f, 60f, 60f, 60f };
                float[] anchoColumnasenccontenidos = { 100f, 40f, 60f, 60f, 100f, 40f, 70f, 60f, 60f, 60f };
                saldosPorfecha = new PdfPTable(anchoColumnasenccontenidos);

                saldosPorfecha.WidthPercentage = 100;
                saldosPorfecha.HorizontalAlignment = Element.ALIGN_LEFT;

                // saldosPorfecha = new PdfPTable(10);
                _fontStylsumas = FontFactory.GetFont("Tahoma", 7f, Font.BOLD);
                
                PdfPCell _PdfPCelMsc;

                PdfPCell _PdfPCel1 = new PdfPCell(new Phrase("", _fontStylsumas));
                _PdfPCel1.Border = 0;
                _PdfPCel1.HorizontalAlignment = Element.ALIGN_LEFT; // 
                _PdfPCel1.VerticalAlignment = Element.ALIGN_MIDDLE;
                saldosPorfecha.AddCell(_PdfPCel1);
                PdfPCell _PdfPCel2 = new PdfPCell(new Phrase("", _fontStylsumas));
                _PdfPCel2.Border = 0;
                _PdfPCel2.HorizontalAlignment = Element.ALIGN_LEFT; // 
                _PdfPCel2.VerticalAlignment = Element.ALIGN_MIDDLE;
                saldosPorfecha.AddCell(_PdfPCel2);
                PdfPCell _PdfPCel3 = new PdfPCell(new Phrase("", _fontStylsumas));
                _PdfPCel3.Border = 0;
                _PdfPCel3.HorizontalAlignment = Element.ALIGN_LEFT; // 
                _PdfPCel3.VerticalAlignment = Element.ALIGN_MIDDLE;
                saldosPorfecha.AddCell(_PdfPCel3);
                PdfPCell _PdfPCel4 = new PdfPCell(new Phrase("", _fontStylsumas));
                _PdfPCel4.Border = 0;
                _PdfPCel4.HorizontalAlignment = Element.ALIGN_LEFT; // 
                _PdfPCel4.VerticalAlignment = Element.ALIGN_MIDDLE;
                saldosPorfecha.AddCell(_PdfPCel4);
                PdfPCell _PdfPCel5 = new PdfPCell(new Phrase("", _fontStylsumas));
                _PdfPCel5.Border = 0;
                _PdfPCel5.HorizontalAlignment = Element.ALIGN_LEFT; // 
                _PdfPCel5.VerticalAlignment = Element.ALIGN_MIDDLE;
                saldosPorfecha.AddCell(_PdfPCel5);
                PdfPCell _PdfPCel6;
                if (Tsumaimpormx30 >0 || Tsumaimpormxcorriente >0 || Tsumaimpormx91 >0 || Tsumaimpormx3190>0)
                {
                  _PdfPCel6 = new PdfPCell(new Phrase("MXN: ", _fontStylsumas));
                }
                else
                {
                   _PdfPCel6 = new PdfPCell(new Phrase("", _fontStylsumas));
                }
                
                _PdfPCel6.Border = 0;
                _PdfPCel6.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                _PdfPCel6.VerticalAlignment = Element.ALIGN_MIDDLE;
                saldosPorfecha.AddCell(_PdfPCel6);
                if (Tsumaimpormxcorriente > 0)
                {
                    _PdfPCelMsc = new PdfPCell(new Phrase(Tsumaimpormxcorriente.ToString("C"), _fontStylsumas));
                }
                else
                {
                    _PdfPCelMsc = new PdfPCell(new Phrase("$0.00", _fontStylsumas));
                }

                _PdfPCelMsc.Border = 0;
                _PdfPCelMsc.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                _PdfPCelMsc.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                saldosPorfecha.AddCell(_PdfPCelMsc);

                PdfPCell _PdfPCelMs;
                if (Tsumaimpormx30 > 0)
                {
                    _PdfPCelMs = new PdfPCell(new Phrase(Tsumaimpormx30.ToString("C"), _fontStylsumas));
                }
                else
                {
                    _PdfPCelMs = new PdfPCell(new Phrase("$0.00", _fontStylsumas));
                }
                _PdfPCelMs.Border = 0;
                _PdfPCelMs.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                _PdfPCelMs.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                saldosPorfecha.AddCell(_PdfPCelMs);


                PdfPCell _PdfPCelMs1;
                if (Tsumaimpormx3190 > 0)
                {
                    _PdfPCelMs1 = new PdfPCell(new Phrase(Tsumaimpormx3190.ToString("C"), _fontStylsumas));
                }
                else
                {
                    _PdfPCelMs1 = new PdfPCell(new Phrase("$0.00", _fontStylsumas));
                }
                _PdfPCelMs1.Border = 0;
                _PdfPCelMs1.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                _PdfPCelMs1.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                saldosPorfecha.AddCell(_PdfPCelMs1);
                PdfPCell _PdfPCelM2;
                if (Tsumaimpormx91 > 0)
                {
                    _PdfPCelM2 = new PdfPCell(new Phrase(Tsumaimpormx91.ToString("C"), _fontStylsumas));
                }
                else
                {
                    _PdfPCelM2 = new PdfPCell(new Phrase("$0.00", _fontStylsumas));
                }

                _PdfPCelM2.Border = 0;
                _PdfPCelM2.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                _PdfPCelM2.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                saldosPorfecha.AddCell(_PdfPCelM2);

               

                if (MonFa=="MXN")
                {
                    _documento.Add(saldosPorfecha);
                }
                

                /////////////// saldos por fechas USD
                ///
                decimal TsumaimporUS30 = 0;
                decimal TsumaimporUS3190 = 0M;
                decimal TsumaimporUS91 = 0M;
                decimal TsumaimporUScorriente = 0;
                decimal sumaimporUS30 = 0M;
                decimal sumaimporUS3190 = 0M;
                decimal sumaimporUS91 = 0M;
                decimal sumaimporUScorriente = 0M;
                string monedafacusd = "";

                foreach (EncFacA fac in FACTURASUMAUSD)
                {
                    ClsOperacionesFecha fechas = new ClsOperacionesFecha();
                    fechas.fechaini = fac.FechaVencimiento;
                    fechas.fechafin = DateTime.Now;
                    //saldosPorfechaUSD = new PdfPTable(10);
                    float[] anchoColumnasenccontenidosusd = { 100f, 40f, 60f, 60f, 100f, 40f, 70f, 60f, 60f, 60f };
                    saldosPorfechaUSD = new PdfPTable(anchoColumnasenccontenidosusd);

                    saldosPorfechaUSD.WidthPercentage = 100;
                    saldosPorfechaUSD.HorizontalAlignment = Element.ALIGN_LEFT;


                    PdfPCell _PdfPCellii;
                    if (fechas.getcorriente())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                       // _PdfPCellii = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                        sumaimporUScorriente += fac.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        _PdfPCellii = new PdfPCell(new Phrase(0.ToString("C"), _fontStylsumas));
                    }


                    PdfPCell _PdfPCellSaldoFecha;
                    if (fechas.get30())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCellSaldoFecha = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylsumas));
                        sumaimporUS30 += fac.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        _PdfPCellSaldoFecha = new PdfPCell(new Phrase(0.ToString("C"), _fontStylsumas));
                    }
                    _PdfPCellSaldoFecha.Border = 0;
                    _PdfPCellSaldoFecha.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCellSaldoFecha.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                   
                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);


                    PdfPCell _PdfPCellSaldoFecha1;
                    if (fechas.get3190())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCellSaldoFecha1 = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylsumas));
                        sumaimporUS3190 += fac.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        _PdfPCellSaldoFecha1 = new PdfPCell(new Phrase(0.ToString("C"), _fontStylsumas));
                    }


                    _PdfPCellSaldoFecha1.Border = 0;
                    _PdfPCellSaldoFecha1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _PdfPCellSaldoFecha1.VerticalAlignment = Element.ALIGN_MIDDLE;
                   


                    PdfPCell _PdfPCellSaldoFecha2;
                    if (fechas.get91mas())
                    {
                        //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                        _PdfPCellSaldoFecha2 = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylsumas));
                        sumaimporUS91 += fac.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        _PdfPCellSaldoFecha2 = new PdfPCell(new Phrase(0.ToString("C"), _fontStylsumas));
                    }


                    monedafacusd = fac.Moneda;
                }
                TsumaimporUScorriente += sumaimporUScorriente;
                TsumaimporUS30 += sumaimporUS30;
                TsumaimporUS3190 += sumaimporUS3190;
                TsumaimporUS91 += sumaimporUS91;
                string monFacUsd = monedafacusd;

                //saldosPorfechaUSD = new PdfPTable(10);


                PdfPCell _PdfPCelUsc;

                    PdfPCell _PdfPCel1U = new PdfPCell(new Phrase("", _fontStylsumas));
                    _PdfPCel1U.Border = 0;
                    _PdfPCel1U.HorizontalAlignment = Element.ALIGN_LEFT; // 
                    _PdfPCel1U.VerticalAlignment = Element.ALIGN_MIDDLE;
                    saldosPorfechaUSD.AddCell(_PdfPCel1U);
                    PdfPCell _PdfPCel2U = new PdfPCell(new Phrase("", _fontStylsumas));
                    _PdfPCel2U.Border = 0;
                    _PdfPCel2U.HorizontalAlignment = Element.ALIGN_LEFT; // 
                    _PdfPCel2U.VerticalAlignment = Element.ALIGN_MIDDLE;
                    saldosPorfechaUSD.AddCell(_PdfPCel2U);
                    PdfPCell _PdfPCel3U = new PdfPCell(new Phrase("", _fontStylsumas));
                    _PdfPCel3U.Border = 0;
                    _PdfPCel3U.HorizontalAlignment = Element.ALIGN_LEFT; // 
                    _PdfPCel3U.VerticalAlignment = Element.ALIGN_MIDDLE;
                    saldosPorfechaUSD.AddCell(_PdfPCel3U);
                    PdfPCell _PdfPCel4U = new PdfPCell(new Phrase("", _fontStylsumas));
                    _PdfPCel4U.Border = 0;
                    _PdfPCel4U.HorizontalAlignment = Element.ALIGN_LEFT; // 
                    _PdfPCel4U.VerticalAlignment = Element.ALIGN_MIDDLE;
                    saldosPorfechaUSD.AddCell(_PdfPCel4U);
                    PdfPCell _PdfPCel5U = new PdfPCell(new Phrase("", _fontStylsumas));
                    _PdfPCel5U.Border = 0;
                    _PdfPCel5U.HorizontalAlignment = Element.ALIGN_LEFT; // 
                    _PdfPCel5U.VerticalAlignment = Element.ALIGN_MIDDLE;
                    saldosPorfechaUSD.AddCell(_PdfPCel5U);
                    PdfPCell _PdfPCel6U;
                    if (TsumaimporUS30 > 0 || TsumaimporUScorriente > 0 || TsumaimporUS91 > 0 || TsumaimporUS3190 > 0)
                    {
                        _PdfPCel6U = new PdfPCell(new Phrase("USD: ", _fontStylsumas));
                    }
                    else
                    {
                        _PdfPCel6U = new PdfPCell(new Phrase("$0.00", _fontStylsumas));
                    }

                    _PdfPCel6U.Border = 0;
                    _PdfPCel6U.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCel6U.VerticalAlignment = Element.ALIGN_MIDDLE;
                    saldosPorfechaUSD.AddCell(_PdfPCel6U);
                    if (TsumaimporUScorriente > 0)
                    {
                        _PdfPCelUsc = new PdfPCell(new Phrase(TsumaimporUScorriente.ToString("C"), _fontStylsumas));
                    }
                    else
                    {
                        _PdfPCelUsc = new PdfPCell(new Phrase("$0.00", _fontStylsumas));
                    }

                    _PdfPCelUsc.Border = 0;
                    _PdfPCelUsc.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCelUsc.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    saldosPorfechaUSD.AddCell(_PdfPCelUsc);

                    PdfPCell _PdfPCelUs;
                    if (TsumaimporUS30 > 0)
                    {
                        _PdfPCelUs = new PdfPCell(new Phrase(TsumaimporUS30.ToString("C"), _fontStylsumas));
                    }
                    else
                    {
                        _PdfPCelUs = new PdfPCell(new Phrase("$0.00", _fontStylsumas));
                    }
                    _PdfPCelUs.Border = 0;
                    _PdfPCelUs.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCelUs.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    saldosPorfechaUSD.AddCell(_PdfPCelUs);


                    PdfPCell _PdfPCelUs1;
                    if (TsumaimporUS3190 > 0)
                    {
                        _PdfPCelUs1 = new PdfPCell(new Phrase(TsumaimporUS3190.ToString("C"), _fontStylsumas));
                    }
                    else
                    {
                        _PdfPCelUs1 = new PdfPCell(new Phrase("$0.00", _fontStylsumas));
                    }
                    _PdfPCelUs1.Border = 0;
                    _PdfPCelUs1.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCelUs1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    saldosPorfechaUSD.AddCell(_PdfPCelUs1);
                    PdfPCell _PdfPCelU2;
                    if (TsumaimporUS91 > 0)
                    {
                        _PdfPCelU2 = new PdfPCell(new Phrase(TsumaimporUS91.ToString("C"), _fontStylsumas));
                    }
                    else
                    {
                        _PdfPCelU2 = new PdfPCell(new Phrase("$0.00", _fontStylsumas));
                    }

                    _PdfPCelU2.Border = 0;
                    _PdfPCelU2.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCelU2.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    saldosPorfechaUSD.AddCell(_PdfPCelU2);

                if (monFacUsd == "USD")
                {
                    _documento.Add(saldosPorfechaUSD);
                }


                



















                //if(acuclienteusd !=0 && acuclientMXN !=0 && acuclientEUR != 0){
                saldosInsolutos = new PdfPTable(3);

                _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                PdfPCell _PdfPCellUSD;
                if (acuclienteusd > 0)
                {
                    _PdfPCellUSD = new PdfPCell(new Phrase("Importe Insoluto USD: " + acuclienteusd.ToString("C"), _fontStylepedido));
                }
                else
                {
                    _PdfPCellUSD = new PdfPCell(new Phrase("$0.00", _fontStylepedido));
                }
                _PdfPCellUSD.Border = 0;
                _PdfPCellUSD.HorizontalAlignment = Element.ALIGN_LEFT; // 
                _PdfPCellUSD.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                saldosInsolutos.AddCell(_PdfPCellUSD);
                _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                PdfPCell _PdfPCellMXN;
                if (acuclientMXN > 0)
                {

                    _PdfPCellMXN = new PdfPCell(new Phrase("Importe Insoluto MXN: " + acuclientMXN.ToString("C"), _fontStylepedido));
                }
                else
                {
                    _PdfPCellMXN = new PdfPCell(new Phrase("", _fontStylepedido));
                }

                _PdfPCellMXN.Border = 0;
                _PdfPCellMXN.HorizontalAlignment = Element.ALIGN_LEFT; // 
                _PdfPCellMXN.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                saldosInsolutos.AddCell(_PdfPCellMXN);
                _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                PdfPCell _PdfPCellEUR;
                if (acuclientEUR > 0)
                {

                    _PdfPCellEUR = new PdfPCell(new Phrase("Importe Insoluto EUR: " + acuclientEUR.ToString("C"), _fontStylepedido));
                }
                else
                {
                    _PdfPCellEUR = new PdfPCell(new Phrase("", _fontStylepedido));
                }

                _PdfPCellEUR.Border = 0;
                _PdfPCellEUR.HorizontalAlignment = Element.ALIGN_LEFT; // 
                _PdfPCellEUR.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                saldosInsolutos.AddCell(_PdfPCellEUR);




                _documento.Add(saldosInsolutos);
                vacio = new PdfPTable(1);

                _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                PdfPCell _PdfPCel = new PdfPCell(new Phrase("", _fontStylepedido));
                _PdfPCel.Border = 0;
                _PdfPCel.HorizontalAlignment = Element.ALIGN_LEFT; // 
                _PdfPCel.VerticalAlignment = Element.ALIGN_MIDDLE;
                //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                vacio.AddCell(_PdfPCel);
                _documento.Add(vacio);
                sumaTotalfechacorriente += Tsumaimpormxcorriente;
                sumaTotalfecha030 += Tsumaimpormx30;
                sumaTotalfecha3190 += Tsumaimpormx3190;
                sumaTotalfecha91 += Tsumaimpormx91;
                sumaTotalfechacorrienteUSD += TsumaimporUScorriente;
                sumaTotalfecha030USD += TsumaimporUS30;
                sumaTotalfecha3190USD += TsumaimporUS3190;
                sumaTotalfecha91USD += TsumaimporUS91;

            }
            _fontStylsumasT = FontFactory.GetFont("Tahoma", 8f, Font.BOLD);
            /////////////////////////////////////////////////////////// total al corriente mxn
            ///
            ///
            float[] anchoColumnasenccontenidossumasPorfecha = { 100f, 40f, 60f, 60f, 100f, 40f, 70f, 60f, 60f, 60f };
            sumasPorfecha = new PdfPTable(anchoColumnasenccontenidossumasPorfecha);

            sumasPorfecha.WidthPercentage = 100;
            sumasPorfecha.HorizontalAlignment = Element.ALIGN_LEFT;


            PdfPCell _PdfPCelTM;

            PdfPCell _PdfPCel1TM = new PdfPCell(new Phrase("", _fontStylsumasT));
            _PdfPCel1TM.Border = 0;
            _PdfPCel1TM.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel1TM.VerticalAlignment = Element.ALIGN_MIDDLE;
            sumasPorfecha.AddCell(_PdfPCel1TM);
            PdfPCell _PdfPCel2TM = new PdfPCell(new Phrase("", _fontStylsumasT));
            _PdfPCel2TM.Border = 0;
            _PdfPCel2TM.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel2TM.VerticalAlignment = Element.ALIGN_MIDDLE;
            sumasPorfecha.AddCell(_PdfPCel2TM);
            PdfPCell _PdfPCel3TM = new PdfPCell(new Phrase("", _fontStylsumasT));
            _PdfPCel3TM.Border = 0;
            _PdfPCel3TM.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel3TM.VerticalAlignment = Element.ALIGN_MIDDLE;
            sumasPorfecha.AddCell(_PdfPCel3TM);
            PdfPCell _PdfPCel4TM = new PdfPCell(new Phrase("", _fontStylsumasT));
            _PdfPCel4TM.Border = 0;
            _PdfPCel4TM.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel4TM.VerticalAlignment = Element.ALIGN_MIDDLE;
            sumasPorfecha.AddCell(_PdfPCel4TM);
            PdfPCell _PdfPCel5TM = new PdfPCell(new Phrase("", _fontStylsumasT));
            _PdfPCel5TM.Border = 0;
            _PdfPCel5TM.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel5TM.VerticalAlignment = Element.ALIGN_MIDDLE;
            sumasPorfecha.AddCell(_PdfPCel5TM);
            PdfPCell _PdfPCel6TM;
            if (sumaTotalfecha030 > 0 || sumaTotalfecha3190 > 0 || sumaTotalfecha91 > 0 || sumaTotalfechacorriente > 0)
            {
                _PdfPCel6TM = new PdfPCell(new Phrase("MXN: ", _fontStylsumasT));
            }
            else
            {
                _PdfPCel6TM = new PdfPCell(new Phrase("$0.00", _fontStylsumasT));
            }

            _PdfPCel6TM.Border = 0;
            _PdfPCel6TM.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel6TM.VerticalAlignment = Element.ALIGN_MIDDLE;
            sumasPorfecha.AddCell(_PdfPCel6TM);
            if (sumaTotalfechacorriente > 0)
            {
                _PdfPCelTM = new PdfPCell(new Phrase(sumaTotalfechacorriente.ToString("C"), _fontStylsumasT));
            }
            else
            {
                _PdfPCelTM = new PdfPCell(new Phrase("$0.00", _fontStylsumasT));
            }

            _PdfPCelTM.Border = 0;
            _PdfPCelTM.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCelTM.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            sumasPorfecha.AddCell(_PdfPCelTM);

            PdfPCell _PdfPCelTTM;
            if (sumaTotalfecha030 > 0)
            {
                _PdfPCelTTM = new PdfPCell(new Phrase(sumaTotalfecha030.ToString("C"), _fontStylsumasT));
            }
            else
            {
                _PdfPCelTTM = new PdfPCell(new Phrase("$0.00", _fontStylsumasT));
            }
            _PdfPCelTTM.Border = 0;
            _PdfPCelTTM.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCelTTM.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            sumasPorfecha.AddCell(_PdfPCelTTM);


            PdfPCell _PdfPCel1TMM;
            if (sumaTotalfecha3190 > 0)
            {
                _PdfPCel1TMM = new PdfPCell(new Phrase(sumaTotalfecha3190.ToString("C"), _fontStylsumasT));
            }
            else
            {
                _PdfPCel1TMM = new PdfPCell(new Phrase("$0.00", _fontStylsumasT));
            }
            _PdfPCel1TMM.Border = 0;
            _PdfPCel1TMM.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel1TMM.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            sumasPorfecha.AddCell(_PdfPCel1TMM);
            PdfPCell _PdfPCel2TMM;
            if (sumaTotalfecha91 > 0)
            {
                _PdfPCel2TMM = new PdfPCell(new Phrase(sumaTotalfecha91.ToString("C"), _fontStylsumasT));
            }
            else
            {
                _PdfPCel2TMM = new PdfPCell(new Phrase("", _fontStylsumasT));
            }

            _PdfPCel2TMM.Border = 0;
            _PdfPCel2TMM.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel2TMM.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            sumasPorfecha.AddCell(_PdfPCel2TMM);

            
                _documento.Add(sumasPorfecha);



            ////////////////////////////////////////////////
            ///
            /////////////////////////////////////////////////////////// total al corriente USD
            ///
            ///
            float[] anchoColumnasenccontenidossumasPorfechaUSD = { 100f, 40f, 60f, 60f, 100f, 40f, 70f, 60f, 60f, 60f };
            sumasPorfechaUSD = new PdfPTable(anchoColumnasenccontenidossumasPorfechaUSD);

            sumasPorfechaUSD.WidthPercentage = 100;
            sumasPorfechaUSD.HorizontalAlignment = Element.ALIGN_LEFT;


            

            PdfPCell _PdfPCel1TMU = new PdfPCell(new Phrase("", _fontStylsumasT));
            _PdfPCel1TMU.Border = 0;
            _PdfPCel1TMU.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel1TMU.VerticalAlignment = Element.ALIGN_MIDDLE;
            sumasPorfechaUSD.AddCell(_PdfPCel1TM);
            PdfPCell _PdfPCel2TMU = new PdfPCell(new Phrase("", _fontStylsumasT));
            _PdfPCel2TMU.Border = 0;
            _PdfPCel2TMU.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel2TMU.VerticalAlignment = Element.ALIGN_MIDDLE;
            sumasPorfechaUSD.AddCell(_PdfPCel2TM);
            PdfPCell _PdfPCel3TMU = new PdfPCell(new Phrase("", _fontStylsumasT));
            _PdfPCel3TMU.Border = 0;
            _PdfPCel3TMU.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel3TMU.VerticalAlignment = Element.ALIGN_MIDDLE;
            sumasPorfechaUSD.AddCell(_PdfPCel3TMU);
            PdfPCell _PdfPCel4TMU = new PdfPCell(new Phrase("", _fontStylsumasT));
            _PdfPCel4TMU.Border = 0;
            _PdfPCel4TMU.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel4TMU.VerticalAlignment = Element.ALIGN_MIDDLE;
            sumasPorfechaUSD.AddCell(_PdfPCel4TMU);
            PdfPCell _PdfPCel5TMU = new PdfPCell(new Phrase("", _fontStylsumasT));
            _PdfPCel5TMU.Border = 0;
            _PdfPCel5TMU.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel5TMU.VerticalAlignment = Element.ALIGN_MIDDLE;
            sumasPorfechaUSD.AddCell(_PdfPCel5TMU);
            PdfPCell _PdfPCel6TMU;
            if (sumaTotalfecha030USD > 0 || sumaTotalfecha3190USD > 0 || sumaTotalfecha91USD > 0 || sumaTotalfechacorrienteUSD > 0)
            {
                _PdfPCel6TMU = new PdfPCell(new Phrase("USD: ", _fontStylsumasT));
            }
            else
            {
                _PdfPCel6TMU = new PdfPCell(new Phrase("$0.00", _fontStylsumasT));
            }

            _PdfPCel6TMU.Border = 0;
            _PdfPCel6TMU.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel6TMU.VerticalAlignment = Element.ALIGN_MIDDLE;
            sumasPorfechaUSD.AddCell(_PdfPCel6TMU);
            PdfPCell _PdfPCelTMU;
            if (sumaTotalfechacorrienteUSD > 0)
            {
                _PdfPCelTMU = new PdfPCell(new Phrase(sumaTotalfechacorrienteUSD.ToString("C"), _fontStylsumasT));
            }
            else
            {
                _PdfPCelTMU = new PdfPCell(new Phrase("$0.00", _fontStylsumasT));
            }

            _PdfPCelTMU.Border = 0;
            _PdfPCelTMU.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCelTMU.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            sumasPorfechaUSD.AddCell(_PdfPCelTMU);

            PdfPCell _PdfPCelTTMU;
            if (sumaTotalfecha030USD > 0)
            {
                _PdfPCelTTMU = new PdfPCell(new Phrase(sumaTotalfecha030USD.ToString("C"), _fontStylsumasT));
            }
            else
            {
                _PdfPCelTTMU = new PdfPCell(new Phrase("$0.00", _fontStylsumasT));
            }
            _PdfPCelTTMU.Border = 0;
            _PdfPCelTTMU.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCelTTMU.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            sumasPorfechaUSD.AddCell(_PdfPCelTTMU);


            PdfPCell _PdfPCel1TMMU;
            if (sumaTotalfecha3190USD > 0)
            {
                _PdfPCel1TMMU = new PdfPCell(new Phrase(sumaTotalfecha3190USD.ToString("C"), _fontStylsumasT));
            }
            else
            {
                _PdfPCel1TMMU = new PdfPCell(new Phrase("$0.00", _fontStylsumasT));
            }
            _PdfPCel1TMMU.Border = 0;
            _PdfPCel1TMMU.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel1TMMU.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            sumasPorfechaUSD.AddCell(_PdfPCel1TMMU);
            PdfPCell _PdfPCel2TMMU;
            if (sumaTotalfecha91USD > 0)
            {
                _PdfPCel2TMMU = new PdfPCell(new Phrase(sumaTotalfecha91USD.ToString("C"), _fontStylsumasT));
            }
            else
            {
                _PdfPCel2TMMU = new PdfPCell(new Phrase("$0.00", _fontStylsumasT));
            }

            _PdfPCel2TMMU.Border = 0;
            _PdfPCel2TMMU.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCel2TMMU.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            sumasPorfechaUSD.AddCell(_PdfPCel2TMMU);


            _documento.Add(sumasPorfechaUSD);














            vacio = new PdfPTable(1);

            _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
            PdfPCell _PdfPCelX = new PdfPCell(new Phrase("", _fontStylepedido));
            _PdfPCelX.Border = 0;
            _PdfPCelX.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCelX.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            vacio.AddCell(_PdfPCelX);
            _documento.Add(vacio);
            vacio = new PdfPTable(1);

            _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
            PdfPCell _PdfPCelQ = new PdfPCell(new Phrase("", _fontStylepedido));
            _PdfPCelQ.Border = 0;
            _PdfPCelQ.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCelQ.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            vacio.AddCell(_PdfPCelQ);
            _documento.Add(vacio);

            vacio = new PdfPTable(1);

            _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
            PdfPCell _PdfPCelW = new PdfPCell(new Phrase("", _fontStylepedido));
            _PdfPCelW.Border = 0;
            _PdfPCelW.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCelW.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            vacio.AddCell(_PdfPCelW);
            _documento.Add(vacio);
            vacio = new PdfPTable(1);

            _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
            PdfPCell _PdfPCelZ = new PdfPCell(new Phrase("", _fontStylepedido));
            _PdfPCelZ.Border = 0;
            _PdfPCelZ.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCelZ.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            vacio.AddCell(_PdfPCelZ);
            _documento.Add(vacio);

            TT = new PdfPTable(3);

            _fontStylepedido = FontFactory.GetFont("Tahoma", 10f, 1);

            PdfPCell _PdfPCelM;
            if (TacuclientMXN > 0)
            {
                _PdfPCelM = new PdfPCell(new Phrase("Total MXN: " + TacuclientMXN.ToString("C"), _fontStylepedido));
            }
            else
            {
                _PdfPCelM = new PdfPCell(new Phrase("$0.00", _fontStylepedido));
            }



            _PdfPCelM.Border = 0;
            _PdfPCelM.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCelM.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            TT.AddCell(_PdfPCelM);



            PdfPCell _PdfPCelU;
            if (Tacuclienteusd > 0)
            {

                _PdfPCelU = new PdfPCell(new Phrase("Total USD: " + Tacuclienteusd.ToString("C"), _fontStylepedido));
            }
            else
            {
                _PdfPCelU = new PdfPCell(new Phrase("$0.00", _fontStylepedido));
            }


            _PdfPCelU.Border = 0;
            _PdfPCelU.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCelU.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            TT.AddCell(_PdfPCelU);



            PdfPCell _PdfPCelw;
            if (acuclientEUR > 0)
            {

                _PdfPCelw = new PdfPCell(new Phrase("Importe Insoluto EUR: " + acuclientEUR.ToString("C"), _fontStylepedido));
            }
            else
            {
                _PdfPCelw = new PdfPCell(new Phrase("$0.00", _fontStylepedido));
            }



            _PdfPCelw.Border = 0;
            _PdfPCelw.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCelw.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            TT.AddCell(_PdfPCelw);
            _documento.Add(TT);
         
        }
        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {
            if (logoEmpresa == null)
                return;
            try
            {
                Image imagen = Image.GetInstance(logoEmpresa, BaseColor.BLACK);
                imagen.ScaleToFit(120, 100);
                //  imagen.Alignment = Element.ALIGN_TOP;
                imagen.SetAbsolutePosition(15f, (_documento.PageSize.Height - 50));
                // _doc.Add(paragraph);
                _documento.Add(imagen);
            }
            catch (Exception err)
            {

            }
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

        #endregion


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

    public class ReporteSaldoA
    {
        [Key]
        public int Rep { get; set; }

        [DisplayName("Precio dolar:")]
        public decimal TC { get; set; }
        public string Titulo { get; set; }

    }

    public class ClientesFacturasA
    {
        [Key]
        public int IDCliente { get; set; }

        public string Nombre { get; set; }

        public string Telefono { get; set; }

        public int IDVendedor { get; set; }

        //public virtual Vendedor vendedor { get; set; }
    }


    public class EncFacA
    {
        [Key]
        public int ID { get; set; }
        public string Serie { get; set; }
        public int Numero { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public string Moneda { get; set; }
        public decimal ImporteSaldoInsoluto { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaRevision { get; set; }

        public decimal TC { get; set; }
    }

    public class MonedaSalA
    {
        [Key]
        public int IMoneda { get; set; }
        public string ClaveMoneda { get; set; }
    }



    public class ClientesFacturasAContext : DbContext
    {
        public ClientesFacturasAContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ClientesFacturasAContext>(null);
        }
        public DbSet<EncFac> Factura { get; set; }
        public DbSet<ClientesFacturas> Clientes { get; set; }


    }


}
