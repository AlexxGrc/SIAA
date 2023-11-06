using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.Models.Administracion;
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
    public class ReporteSaldoProveedor
    {
        #region Declaration
        private Document _documento;
        Font _fontStyle;
        Font _fontStylepedido;

        public string _tc = "";

        PdfWriter _writer;
        int _totalColumn = 10;

        PdfPTable tablaparaestepedido = new PdfPTable(9);
        PdfPTable tablaparaestepedidoMXN = new PdfPTable(9);
        PdfPTable tablaparaestepedidoEUR = new PdfPTable(9);
        PdfPTable saldosInsolutos = new PdfPTable(3);
        PdfPTable vacio = new PdfPTable(1);
        PdfPTable TT = new PdfPTable(3);
        PdfPTable _pdfTable = new PdfPTable(9);
      
        PdfPCell _PdfPCell;


        public int idproveedor { get; set; }
        

        MemoryStream _memoryStream = new MemoryStream();

        List<ProveedoresFacturas> proveedores = new List<ProveedoresFacturas>();
        #endregion
        public ClientesFacturasContext db = new ClientesFacturasContext();
        // aqui los puedes pasar como parametro a l reporte
        public CMYKColor colordefinido;
        public CMYKColor colordefinidoletra;
        public byte[] PrepareReport(int _idproveedor=0)

        {

            idproveedor = _idproveedor;
            var proveedores = this.GetProveedor(idproveedor);

            #region
            _documento = new Document(PageSize.LETTER, 0f, 0f, 0f, 0f);

            _documento.SetMargins(20f, 10f, 20f, 30f);

            _writer = PdfWriter.GetInstance(_documento, HttpContext.Current.Response.OutputStream);
            _writer.PageEvent = new ITextEvents();
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
          
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


            ClsColoresReporte colorl = new ClsColoresReporte(SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado);
            colordefinidoletra = colorl.color;

            this.AgregarLogo(logoempresa);
            this.ReportHeader();
            this.ReportBody();

         
            _documento.Close();


            HttpContext.Current.Response.ContentType = "pdf/application";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"Saldos.pdf" + "\"");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Write(_documento);
            HttpContext.Current.Response.End();

            return _memoryStream.ToArray();


        }

        //Obtener la lista del encabezado
        public ReporteSaldoProv GetEncabezado(int idproveedor)
        {
            ReporteSaldoProv encabezado = new ReporteSaldoProv();
            VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambioCadena(GETDATE(),'USD','MXN') as TC").ToList()[0];
            encabezado.TC = cambio.TC;

            if (idproveedor == 0)
            {
                encabezado.Rep = 100;
                encabezado.Titulo = "Todos los proveedores";

            }

            if (idproveedor != 0)
            {
                encabezado.Rep = 110;
                string cadenaE = "select * from dbo.Proveedores where   IDProveedor = " + idproveedor + " order by Empresa";
                Proveedor enc = db.Database.SqlQuery<Proveedor>(cadenaE).ToList().FirstOrDefault();
                encabezado.Titulo = "Proveedor: " + enc.Empresa + "";
            }

          
            return encabezado;

        }
        //Obtener la lista de los proveedores
        public List<ProveedoresFacturas> GetProveedor(int idproveedor)
        {
            List<ProveedoresFacturas> Listproveedor = new List<ProveedoresFacturas>();
            try
            {

                if (idproveedor == 0)
                {
                    string cadena = "select IDProveedor, Empresa, TelefonoUno as Telefono from dbo.Proveedores where Empresa in (select distinct Nombre_Proveedor from[dbo].[VEncFacturaSaldoProv] where ImporteSaldoInsoluto> 0) order by Empresa";
                    Listproveedor = db.Database.SqlQuery<ProveedoresFacturas>(cadena).ToList();
                }


                if (idproveedor != 0)
                {
                    string cadena = "select IDProveedor, Empresa, TelefonoUno as Telefono from dbo.Proveedores where Empresa in (select distinct Nombre_Proveedor from[dbo].[VEncFacturaSaldoProv] where ImporteSaldoInsoluto> 0 and IDProveedor = " + idproveedor + ") order by Empresa";
                    Listproveedor = db.Database.SqlQuery<ProveedoresFacturas>(cadena).ToList();
                }

            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }
            return Listproveedor;
        }

       
        public List<EncFacProv> GetFacturaUSD(int idproveedor)
        {
            List<EncFacProv> detpe = new List<EncFacProv>();
            string cadena = "select Id, Fecha,Serie, numero, Subtotal, Iva, Total, ImporteSaldoInsoluto, MonedaOrigen as moneda,tc from  [dbo].[VEncFacturaSaldoProv] where ImporteSaldoInsoluto>0 and IdProveedor = " + idproveedor + " and MonedaOrigen='USD' and Estado='A' order by fecha ";
            detpe = db.Database.SqlQuery<EncFacProv>(cadena).ToList();
            return detpe;

        }

        public List<EncFacProv> GetFacturaMXN(int idproveedor)
        {
            List<EncFacProv> detpe = new List<EncFacProv>();
            string cadena = "select Id, Fecha,Serie, numero, Subtotal, Iva, Total, ImporteSaldoInsoluto, MonedaOrigen as moneda,tc from  [dbo].[VEncFacturaSaldoProv] where ImporteSaldoInsoluto>0 and IdProveedor = " + idproveedor + " and MonedaOrigen='MXN' and Estado='A'  order by fecha";
            detpe = db.Database.SqlQuery<EncFacProv>(cadena).ToList();
            return detpe;

        }

        public List<EncFacProv> GetFacturaEUR(int idproveedor)
        {
            List<EncFacProv> detpe = new List<EncFacProv>();
            string cadena = "select Id, Fecha, Serie, numero, Subtotal, Iva, Total, ImporteSaldoInsoluto, MonedaOrigen as moneda,tc from  [dbo].[VEncFacturaSaldoProv] where ImporteSaldoInsoluto>0 and IdProveedor = " + idproveedor + " and MonedaOrigen='EUR' and Estado='A'  order by fecha";
            detpe = db.Database.SqlQuery<EncFacProv>(cadena).ToList();
            return detpe;
        }

        public MonedaSal GetMoneda(int idprov, int id)
        {
            MonedaSal detpe = new MonedaSal();
            string cadena = "select m.idMoneda, m.ClaveMoneda  from c_Moneda as m inner join EncFacturas as e on m.idmoneda=e.idmoneda  where e.idcliente=" + idprov + "  and e.id=" + id + "";
            detpe = db.Database.SqlQuery<MonedaSal>(cadena).ToList().FirstOrDefault();
            //proveedor = db.Database.SqlQuery<Proveedor>("selec * from Proveedores").ToList();
            return detpe;

        }





        private void ReportHeader()
        {
            #region Table head

            ReporteSaldoProv enc;
            enc= GetEncabezado(idproveedor);
            

                

                _fontStyle = FontFactory.GetFont("Tahoma", 12f, 1);
                _PdfPCell = new PdfPCell(new Phrase("Reporte de Saldo de Proveedores", _fontStyle));
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
                //_PdfPCell.BackgroundColor = BaseColor.WHITE;
                _PdfPCell.ExtraParagraphSpace = 0;
                //_PdfPCell.PaddingLeft = 100;
                _pdfTable.AddCell(_PdfPCell);

                    _pdfTable.CompleteRow();
                _documento.Add(_pdfTable);


                float[] anchoColumnasencabezado = { 100f, 150f, 250f, 100f };

                PdfPTable tablae = new PdfPTable(anchoColumnasencabezado);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;

                tablae.SetTotalWidth(anchoColumnasencabezado);
                tablae.SpacingBefore = 0;
                tablae.HorizontalAlignment = Element.ALIGN_LEFT;
                tablae.LockedWidth = true;
                Font _fontStyleencabezado = new Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD, colordefinidoletra);  // aqui podremos cambiar la fuente de LA FECHA y su tamanño

                PdfPCell _tablaencabezado1 = new PdfPCell(new Phrase("Precio Dolar: ", _fontStyleencabezado));
                _tablaencabezado1.Border = 0;
                _tablaencabezado1.FixedHeight = 20f;
                _tablaencabezado1.HorizontalAlignment = Element.ALIGN_RIGHT;
                _tablaencabezado1.VerticalAlignment = Element.ALIGN_MIDDLE;
                _tablaencabezado1.BackgroundColor = BaseColor.WHITE;
                tablae.AddCell(_tablaencabezado1);

                _tc = enc.TC.ToString("C");
                PdfPCell _tablaencabezado2 = new PdfPCell(new Phrase(_tc, _fontStyleencabezado));
                _tablaencabezado2.Border = 0;
                _tablaencabezado2.FixedHeight = 10f;
                _tablaencabezado2.HorizontalAlignment = Element.ALIGN_LEFT;
                _tablaencabezado2.VerticalAlignment = Element.ALIGN_MIDDLE;
                _tablaencabezado2.BackgroundColor = BaseColor.WHITE;
                tablae.AddCell(_tablaencabezado2);


                PdfPCell _tablaencabezado3 = new PdfPCell(new Phrase("Fecha de impresión: ", _fontStyleencabezado));
                _tablaencabezado3.Border = 0;
                _tablaencabezado3.FixedHeight = 10f;
                _tablaencabezado3.HorizontalAlignment = Element.ALIGN_RIGHT;
                _tablaencabezado3.VerticalAlignment = Element.ALIGN_MIDDLE;
                _tablaencabezado3.BackgroundColor = BaseColor.WHITE;
                tablae.AddCell(_tablaencabezado3);

                DateTime fecActual = DateTime.Today;
                string hoy = fecActual.ToString("dd/MM/yyyy");
                PdfPCell _tablaencabezado4 = new PdfPCell(new Phrase(hoy, _fontStyleencabezado));
                _tablaencabezado4.Border = 0;
                _tablaencabezado4.FixedHeight = 10f;
                _tablaencabezado4.HorizontalAlignment = Element.ALIGN_LEFT;
                _tablaencabezado4.VerticalAlignment = Element.ALIGN_MIDDLE;
                _tablaencabezado4.BackgroundColor = BaseColor.WHITE;
                tablae.AddCell(_tablaencabezado4);

                //tablaencabezado.CompleteRow();
              //  _documento.Add(tablaencabezado);
     


            float[] anchoColumnasenccontenido = { 100f, 20f, 80f, 100f, 80f, 100f, 40f,40f, 120f };
            PdfPTable tablaencabezado = new PdfPTable(anchoColumnasenccontenido);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Fecha", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablaencabezado.AddCell(_PdfPCell);


            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablaencabezado.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Nùmero", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablaencabezado.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Subtotal", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablaencabezado.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("IVA", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablaencabezado.AddCell(_PdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Total", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablaencabezado.AddCell(_PdfPCell);

			
            _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablaencabezado.AddCell(_PdfPCell);

            
            _PdfPCell = new PdfPCell(new Phrase("TC", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablaencabezado.AddCell(_PdfPCell);

            

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Importe Insoluto", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
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

            List<ProveedoresFacturas> proveedor = new List<ProveedoresFacturas>();
            proveedor = GetProveedor(idproveedor);
            foreach (ProveedoresFacturas prov in proveedor)
            {
                // creamos una tabla para imprimir los los datos del cliente
                // como son 4 columnas a imprimir 600entre las 4 

                int _idprov = prov.IDProveedor;
                float[] anchoColumnasclientes = { 30f, 250f, 150f, 170f };

                // 50 para id 3000 para el nombre 100 para el telefono y 150 para el vendedor

                PdfPTable tablaparaestecliente = new PdfPTable(anchoColumnasclientes);
                //tablaProductosPrincipal.DefaultCell.Border = Rectangle.NO_BORDER;
                tablaparaestecliente.SetTotalWidth(anchoColumnasclientes);
                tablaparaestecliente.SpacingBefore = 3;
                tablaparaestecliente.HorizontalAlignment = Element.ALIGN_LEFT;
                tablaparaestecliente.LockedWidth = true;

                Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                PdfPCell _PdfPCell1 = new PdfPCell(new Phrase(prov.IDProveedor.ToString(), _fontStylecliente));
                _PdfPCell1.Border = 0;
                _PdfPCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell1.BackgroundColor = BaseColor.WHITE;
                tablaparaestecliente.AddCell(_PdfPCell1);

                PdfPCell _PdfPCell2 = new PdfPCell(new Phrase(prov.Empresa, _fontStylecliente));
                _PdfPCell2.Border = 0;
                _PdfPCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell2.BackgroundColor = BaseColor.WHITE;
                tablaparaestecliente.AddCell(_PdfPCell2);

                PdfPCell _PdfPCell3 = new PdfPCell(new Phrase(prov.Telefono, _fontStylecliente));
                _PdfPCell3.Border = 0;
                _PdfPCell3.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell3.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell3.BackgroundColor = BaseColor.WHITE;
                tablaparaestecliente.AddCell(_PdfPCell3);

                //string nombrevendedor = new VendedorContext().Vendedores.Find(cliente.IDVendedor).Nombre;
                PdfPCell _PdfPCell4 = new PdfPCell(new Phrase("", _fontStylecliente));
                _PdfPCell4.Border = 0;
                _PdfPCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                _PdfPCell4.VerticalAlignment = Element.ALIGN_MIDDLE;
                _PdfPCell4.BackgroundColor = BaseColor.WHITE;
                tablaparaestecliente.AddCell(_PdfPCell4);

                _documento.Add(tablaparaestecliente);
                
                 //public List<EncFacProv> GetFacturaUSD(int idprov)
                List<EncFacProv> FACTURAUSD = new List<EncFacProv>();

                FACTURAUSD = GetFacturaUSD(_idprov); // aqui obtenemos los pedidos que corresponden a ese cliente
                float[] anchoColumnascontenido = { 100f, 20f, 80f, 100f, 80f, 100f, 40f, 40, 120f };


                acuclienteusd = 0M;
                foreach (EncFacProv fac in FACTURAUSD)
                {
                    // List<MonedaSal> moneda = new List<MonedaSal>();

                    //   moneda = GetMoneda(cliente.IDCliente, fac.ID);

                    tablaparaestepedido = new PdfPTable(anchoColumnascontenido);

                    _fontStylepedido = FontFactory.GetFont("Tahoma", 7f, Font.NORMAL);
                    PdfPCell _PdfPCellf = new PdfPCell(new Phrase(fac.Fecha.ToShortDateString(), _fontStylepedido));
                    _PdfPCellf.Border = 0;
                    _PdfPCellf.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellf.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCellf);



                    
                    PdfPCell _PdfPCellespacio = new PdfPCell(new Phrase(fac.Serie, _fontStylepedido));
                    _PdfPCellespacio.Border = 0;
                    _PdfPCellespacio.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellespacio.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCellespacio);


                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                    PdfPCell _PdfPCell2pedido = new PdfPCell(new Phrase(fac.Numero.ToString(), _fontStylepedido));
                    _PdfPCell2pedido.Border = 0;
                    _PdfPCell2pedido.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell2pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCell2pedido);


                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                    PdfPCell _PdfPCell3pedido = new PdfPCell(new Phrase(fac.Subtotal.ToString("C"), _fontStylepedido));
                    _PdfPCell3pedido.Border = 0;
                    _PdfPCell3pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell3pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCell3pedido);

                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                    PdfPCell _PdfPCell4pedido = new PdfPCell(new Phrase(fac.Iva.ToString("C"), _fontStylepedido));
                    _PdfPCell4pedido.Border = 0;
                    _PdfPCell4pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell4pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCell4pedido);



                    // _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                    PdfPCell _PdfPCell5pedido = new PdfPCell(new Phrase(fac.Total.ToString("C"), _fontStylepedido));
                    _PdfPCell5pedido.Border = 0;
                    _PdfPCell5pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell5pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCell5pedido);
                    //   _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, 1);
                    PdfPCell _PdfPCell6pedido = new PdfPCell(new Phrase(fac.Moneda, _fontStylepedido));
                    _PdfPCell6pedido.Border = 0;
                    _PdfPCell6pedido.HorizontalAlignment = Element.ALIGN_LEFT; // 
                    _PdfPCell6pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCell6pedido);


                    PdfPCell _PdfPCell61pedido = new PdfPCell(new Phrase(fac.TC.ToString("C"), _fontStylepedido));
                    _PdfPCell61pedido.Border = 0;
                    _PdfPCell61pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCell61pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCell61pedido);

                    //  _fontStylepedido = FontFactory.GetFont("Tahoma", 8f,  Font.NORMAL);
                    PdfPCell _PdfPCell7pedido = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    _PdfPCell7pedido.Border = 0;
                    _PdfPCell7pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCell7pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedido.AddCell(_PdfPCell7pedido);

                    acuclienteusd += fac.ImporteSaldoInsoluto;

                    tablaparaestepedido.CompleteRow();
                    tablaparaestepedido.SpacingAfter = 1;


                    _documento.Add(tablaparaestepedido);

                }
                Tacuclienteusd += acuclienteusd;



                List<EncFacProv> FACTURAMXN = new List<EncFacProv>();

                FACTURAMXN = GetFacturaMXN(_idprov); // aqui obtenemos los pedidos que corresponden a ese cliente

                acuclientMXN = 0M;
                foreach (EncFacProv fac in FACTURAMXN)
                {
                    // List<MonedaSal> moneda = new List<MonedaSal>();

                    //   moneda = GetMoneda(cliente.IDCliente, fac.ID);

                    tablaparaestepedidoMXN = new PdfPTable(anchoColumnascontenido);

                    _fontStylepedido = FontFactory.GetFont("Tahoma", 7f, Font.NORMAL);
                    PdfPCell _PdfPCellf = new PdfPCell(new Phrase(fac.Fecha.ToShortDateString(), _fontStylepedido));
                    _PdfPCellf.Border = 0;
                    _PdfPCellf.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellf.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCellf);


               


                    PdfPCell _PdfPCellespacio = new PdfPCell(new Phrase(fac.Serie, _fontStylepedido));
                    _PdfPCellespacio.Border = 0;
                    _PdfPCellespacio.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellespacio.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCellespacio);



                    PdfPCell _PdfPCell2pedido = new PdfPCell(new Phrase(fac.Numero.ToString(), _fontStylepedido));
                    _PdfPCell2pedido.Border = 0;
                    _PdfPCell2pedido.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell2pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCell2pedido);



                    PdfPCell _PdfPCell3pedido = new PdfPCell(new Phrase(fac.Subtotal.ToString("C"), _fontStylepedido));
                    _PdfPCell3pedido.Border = 0;
                    _PdfPCell3pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell3pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCell3pedido);


                    PdfPCell _PdfPCell4pedido = new PdfPCell(new Phrase(fac.Iva.ToString("C"), _fontStylepedido));
                    _PdfPCell4pedido.Border = 0;
                    _PdfPCell4pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell4pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCell4pedido);




                    PdfPCell _PdfPCell5pedido = new PdfPCell(new Phrase(fac.Total.ToString("C"), _fontStylepedido));
                    _PdfPCell5pedido.Border = 0;
                    _PdfPCell5pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell5pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCell5pedido);


                    PdfPCell _PdfPCell6pedido = new PdfPCell(new Phrase(fac.Moneda, _fontStylepedido));
                    _PdfPCell6pedido.Border = 0;
                    _PdfPCell6pedido.HorizontalAlignment = Element.ALIGN_LEFT; // 
                    _PdfPCell6pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCell6pedido);

                    PdfPCell _PdfPCell61pedido = new PdfPCell(new Phrase(fac.TC.ToString("C"), _fontStylepedido));
                    _PdfPCell61pedido.Border = 0;
                    _PdfPCell61pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCell61pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCell61pedido);


                    PdfPCell _PdfPCell7pedido = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    _PdfPCell7pedido.Border = 0;
                    _PdfPCell7pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCell7pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoMXN.AddCell(_PdfPCell7pedido);



                    tablaparaestepedidoMXN.CompleteRow();
                    tablaparaestepedidoMXN.SpacingAfter = 1;
                    _documento.Add(tablaparaestepedidoMXN);
                    acuclientMXN += fac.ImporteSaldoInsoluto;

                }
                TacuclientMXN += acuclientMXN;


                List<EncFacProv> FACTURAEUR = new List<EncFacProv>();

                FACTURAEUR = GetFacturaEUR(_idprov); // aqui obtenemos los pedidos que corresponden a ese cliente

                acuclientEUR = 0M;
                foreach (EncFacProv fac in FACTURAEUR)
                {
                    // List<MonedaSal> moneda = new List<MonedaSal>();

                    //   moneda = GetMoneda(cliente.IDCliente, fac.ID);

                    tablaparaestepedidoEUR = new PdfPTable(9);


                    _fontStylepedido = FontFactory.GetFont("Tahoma", 7f, Font.NORMAL);
                    PdfPCell _PdfPCellf = new PdfPCell(new Phrase(fac.Fecha.ToShortDateString(), _fontStylepedido));
                    _PdfPCellf.Border = 0;
                    _PdfPCellf.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellf.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCellf);


                  
                    PdfPCell _PdfPCellespacio = new PdfPCell(new Phrase(fac.Serie, _fontStylepedido));
                    _PdfPCellespacio.Border = 0;
                    _PdfPCellespacio.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellespacio.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCellespacio);



                   
                    PdfPCell _PdfPCell2pedido = new PdfPCell(new Phrase(fac.Numero.ToString(), _fontStylepedido));
                    _PdfPCell2pedido.Border = 0;
                    _PdfPCell2pedido.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell2pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell2pedido);


                    
                    PdfPCell _PdfPCell3pedido = new PdfPCell(new Phrase(fac.Subtotal.ToString("C"), _fontStylepedido));
                    _PdfPCell3pedido.Border = 0;
                    _PdfPCell3pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell3pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell3pedido);

                  
                    PdfPCell _PdfPCell4pedido = new PdfPCell(new Phrase(fac.Iva.ToString("C"), _fontStylepedido));
                    _PdfPCell4pedido.Border = 0;
                    _PdfPCell4pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell4pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell4pedido);



                    _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, Font.NORMAL);
                    PdfPCell _PdfPCell5pedido = new PdfPCell(new Phrase(fac.Total.ToString("C"), _fontStylepedido));
                    _PdfPCell5pedido.Border = 0;
                    _PdfPCell5pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell5pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell5pedido);

                   
                    PdfPCell _PdfPCell6pedido = new PdfPCell(new Phrase(fac.Moneda, _fontStylepedido));
                    _PdfPCell6pedido.Border = 0;
                    _PdfPCell6pedido.HorizontalAlignment = Element.ALIGN_LEFT; // 
                    _PdfPCell6pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell6pedido);


                    PdfPCell _PdfPCell61pedido = new PdfPCell(new Phrase(fac.TC.ToString("C"), _fontStylepedido));
                    _PdfPCell61pedido.Border = 0;
                    _PdfPCell61pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCell61pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell61pedido);

                   
                    PdfPCell _PdfPCell7pedido = new PdfPCell(new Phrase(fac.ImporteSaldoInsoluto.ToString("C"), _fontStylepedido));
                    _PdfPCell7pedido.Border = 0;
                    _PdfPCell7pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // 
                    _PdfPCell7pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell7pedido);



                    tablaparaestepedidoEUR.CompleteRow();
                    tablaparaestepedidoEUR.SpacingAfter = 1;
                    _documento.Add(tablaparaestepedidoEUR);
                    acuclientEUR += fac.ImporteSaldoInsoluto;
                }

                TacuclientEUR += acuclientEUR;

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
                    _PdfPCellUSD = new PdfPCell(new Phrase("", _fontStylepedido));
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

            }
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
                _PdfPCelM = new PdfPCell(new Phrase("", _fontStylepedido));
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
                _PdfPCelU = new PdfPCell(new Phrase("", _fontStylepedido));
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
                _PdfPCelw = new PdfPCell(new Phrase("", _fontStylepedido));
            }
            _PdfPCelw.Border = 0;
            _PdfPCelw.HorizontalAlignment = Element.ALIGN_LEFT; // 
            _PdfPCelw.VerticalAlignment = Element.ALIGN_MIDDLE;
            //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
            TT.AddCell(_PdfPCelw);
            _documento.Add(TT);







            #endregion

        }
        private void AgregarLogo(System.Drawing.Image logoEmpresa)
        {
            if (logoEmpresa == null)
                return;
            try
            {
                Image imagen = Image.GetInstance(logoEmpresa, BaseColor.BLACK);
                imagen.ScaleToFit(60, 40);
                //  imagen.Alignment = Element.ALIGN_TOP;
                imagen.SetAbsolutePosition(15f, (_documento.PageSize.Height - 50));
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



    public class ReporteSaldoProv
    {
        [Key]
        public int Rep { get; set; }

        [DisplayName("Precio dolar:")]
        public decimal TC { get; set; }
        public string Titulo { get; set; }

    }


    public class ProveedoresFacturas
    {
        [Key]
        public int IDProveedor { get; set; }

        public string Empresa { get; set; }

        public string Telefono { get; set; }

    }


    public class EncFacProv
    {
        [Key]
        public int ID { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public string Moneda { get; set; }
        public decimal ImporteSaldoInsoluto { get; set; }
        public decimal TC { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class MonedaSalP
    {
        [Key]
        public int IMoneda { get; set; }
        public string ClaveMoneda { get; set; }
    }



    public class ProveedorFacturasContext : DbContext
    {
        public ProveedorFacturasContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ProveedorFacturasContext>(null);
        }
           public DbSet<ProveedoresFacturas> Proveedores { get; set; }
        public DbSet<Models.Cfdi.EncfacturaProv> Factura { get; set; }


    }

  
}