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
    public class ReporteSaldoInsoluto
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
        PdfPTable tablae = new PdfPTable(10);
              

        PdfPCell _PdfPCell;


    
		   public int idcliente { get; set; }

        MemoryStream _memoryStream = new MemoryStream();

        List<ClientesFacturas> clientes = new List<ClientesFacturas>();
        #endregion
        public ClientesFacturasContext db = new ClientesFacturasContext();
        // aqui los puedes pasar como parametro a l reporte
		 public CMYKColor colordefinido;
        public byte[] PrepareReport(int _idcliente)

        {
           
			idcliente=_idcliente;
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
            ClsColoresReporte colorr = new ClsColoresReporte("Marron dorado");
            colordefinido = colorr.color;

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
          

 		public ReporteSaldo GetEncabezado(int idcliente)
        {
            ReporteSaldo encabezado = new ReporteSaldo();
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

        public List<ClientesFacturas> GetClientes(int idcliente)
        {
            List<ClientesFacturas> clientes = new List<ClientesFacturas>();
   		
            try
            {
                if (idcliente != 0)
                {
                    string cadena = "select C.IDCliente, C.Nombre, C.Telefono, C.IDVendedor from Clientes as c inner join tempClienteSaldo as t on c.IDCliente=t.IDCliente where c.IDCliente=" + idcliente;
                    clientes = db.Database.SqlQuery<ClientesFacturas>(cadena).ToList();
                }
                else
                {
                    string cadena = "select C.IDCliente, C.Nombre, C.Telefono, C.IDVendedor from Clientes as c inner join tempClienteSaldo as t on c.IDCliente=t.IDCliente order by c.Nombre";
                    clientes = db.Database.SqlQuery<ClientesFacturas>(cadena).ToList();
                }
               


            }
            catch (SqlException err)
            {
                string mensajedeerror = err.Message;
            }

            
            return clientes;

        }

        public List<EncFac> GetFacturaUSD(int idCliente)
        {
            List<EncFac> detpe = new List<EncFac>();
            string cadena = "select distinct e.id, e.numero, e.serie, e.subtotal, e.iva, e.total, e.moneda, s.ImporteSaldoInsoluto, e.Fecha,e.TC  from enCfacturas as e inner join SaldoFactura as s on s.idFactura=e.id  where s.ImporteSaldoInsoluto>0 and idcliente=" + idCliente + " and e.moneda='USD' and e.Estado='A'";
            detpe = db.Database.SqlQuery<EncFac>(cadena).ToList();
            //proveedor = db.Database.SqlQuery<Proveedor>("selec * from Proveedores").ToList();
            return detpe;

        }

        public List<EncFac> GetFacturaMXN(int idCliente)
        {
            List<EncFac> detpe = new List<EncFac>();
            string cadena = "select distinct e.id, e.numero, e.serie, e.subtotal, e.iva, e.total, e.moneda, s.ImporteSaldoInsoluto, e.Fecha,e.TC  from enCfacturas as e inner join SaldoFactura as s on s.idFactura=e.id  where s.ImporteSaldoInsoluto>0 and idcliente=" + idCliente + " and e.moneda='MXN' and e.Estado='A'";
            detpe = db.Database.SqlQuery<EncFac>(cadena).ToList();
            //proveedor = db.Database.SqlQuery<Proveedor>("selec * from Proveedores").ToList();
            return detpe;

        }

        public List<EncFac> GetFacturaEUR(int idCliente)
        {
            List<EncFac> detpe = new List<EncFac>();
            string cadena = "select distinct e.id, e.numero, e.serie, e.subtotal, e.iva, e.total, e.moneda, s.ImporteSaldoInsoluto, e.Fecha,e.TC  from enCfacturas as e inner join SaldoFactura as s on s.idFactura=e.id  where s.ImporteSaldoInsoluto>0 and idcliente=" + idCliente + " and e.moneda='EUR' and e.Estado='A' ";
            detpe = db.Database.SqlQuery<EncFac>(cadena).ToList();
            //proveedor = db.Database.SqlQuery<Proveedor>("selec * from Proveedores").ToList();
            return detpe;

        }

        public MonedaSal GetMoneda(int idCliente, int id)
        {
            MonedaSal detpe = new MonedaSal();
            string cadena = "select m.idMoneda, m.ClaveMoneda  from c_Moneda as m inner join EncFacturas as e on m.idmoneda=e.idmoneda  where e.idcliente=" + idCliente + "  and e.id=" + id + "";
            detpe = db.Database.SqlQuery<MonedaSal>(cadena).ToList().FirstOrDefault();
            //proveedor = db.Database.SqlQuery<Proveedor>("selec * from Proveedores").ToList();
            return detpe;

        }


        private void ReportHeader()
        {
            #region Table head


            ReporteSaldo enc;
            enc = GetEncabezado(idcliente);
            PdfPCell _PdfPCell;
          
            _fontStyle = FontFactory.GetFont("Tahoma", 12f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Reporte de Saldo de Clientes", _fontStyle));
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


            float[] anchoColumnasenccontenido = { 100f, 20f, 80f, 100f, 80f, 100f, 40f,40f, 120f };
            PdfPTable tablaencabezado = new PdfPTable(anchoColumnasenccontenido);

         

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _PdfPCell = new PdfPCell(new Phrase("Fecha", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
			_PdfPCell.BackgroundColor = colordefinido;
            
            tablaencabezado.AddCell(_PdfPCell);

      
         
            _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;;
            tablaencabezado.AddCell(_PdfPCell);

      
            _PdfPCell = new PdfPCell(new Phrase("Nùmero", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;;
            tablaencabezado.AddCell(_PdfPCell);

          
            _PdfPCell = new PdfPCell(new Phrase("Subtotal", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;;
            tablaencabezado.AddCell(_PdfPCell);

           
            _PdfPCell = new PdfPCell(new Phrase("IVA", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablaencabezado.AddCell(_PdfPCell);

           
            _PdfPCell = new PdfPCell(new Phrase("Total", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;;
            tablaencabezado.AddCell(_PdfPCell);

            
            _PdfPCell = new PdfPCell(new Phrase("", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablaencabezado.AddCell(_PdfPCell);

			
            _PdfPCell = new PdfPCell(new Phrase("TC", _fontStyle));
            _PdfPCell.Border = 0;
            _PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _PdfPCell.BackgroundColor = colordefinido;
            tablaencabezado.AddCell(_PdfPCell);

     
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
            foreach (ClientesFacturas cliente in clientes)
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

                Font _fontStylecliente = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD);  // aqui podremos cambiar la fuente de lcientes y su tamanño

                PdfPCell _PdfPCell1 = new PdfPCell(new Phrase("", _fontStylecliente));
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


                List<EncFac> FACTURAUSD = new List<EncFac>();

                FACTURAUSD = GetFacturaUSD(cliente.IDCliente); // aqui obtenemos los pedidos que corresponden a ese cliente
                float[] anchoColumnascontenido = { 80f, 40f, 80f, 90f, 70f, 90, 40f, 40, 90 };
                acuclienteusd = 0M;
                foreach (EncFac fac in FACTURAUSD)
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



                List<EncFac> FACTURAMXN = new List<EncFac>();

                FACTURAMXN = GetFacturaMXN(cliente.IDCliente); // aqui obtenemos los pedidos que corresponden a ese cliente

                acuclientMXN = 0M;
                foreach (EncFac fac in FACTURAMXN)
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


                List<EncFac> FACTURAEUR = new List<EncFac>();

                FACTURAEUR = GetFacturaEUR(cliente.IDCliente); // aqui obtenemos los pedidos que corresponden a ese cliente


                acuclientEUR = 0M;
                foreach (EncFac fac in FACTURAEUR)
                {
                    // List<MonedaSal> moneda = new List<MonedaSal>();

                    //   moneda = GetMoneda(cliente.IDCliente, fac.ID);

                    tablaparaestepedidoEUR = new PdfPTable(anchoColumnascontenido);


                    _fontStylepedido = FontFactory.GetFont("Tahoma", 7f, Font.NORMAL);
                    PdfPCell _PdfPCellf = new PdfPCell(new Phrase(fac.Fecha.ToShortDateString(), _fontStylepedido));
                    _PdfPCellf.Border = 0;
                    _PdfPCellf.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellf.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCellf);


                

                    _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, Font.NORMAL);
                    PdfPCell _PdfPCellespacio = new PdfPCell(new Phrase(fac.Serie, _fontStylepedido));
                    _PdfPCellespacio.Border = 0;
                    _PdfPCellespacio.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCellespacio.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCellespacio);
                                                 
                    

                    _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, Font.NORMAL);
                    PdfPCell _PdfPCell2pedido = new PdfPCell(new Phrase(fac.Numero.ToString(), _fontStylepedido));
                    _PdfPCell2pedido.Border = 0;
                    _PdfPCell2pedido.HorizontalAlignment = Element.ALIGN_LEFT;
                    _PdfPCell2pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell2pedido);


                    _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, Font.NORMAL);
                    PdfPCell _PdfPCell3pedido = new PdfPCell(new Phrase(fac.Subtotal.ToString("C"), _fontStylepedido));
                    _PdfPCell3pedido.Border = 0;
                    _PdfPCell3pedido.HorizontalAlignment = Element.ALIGN_RIGHT; // como es moneda lo alineamos a la derecha
                    _PdfPCell3pedido.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //_PdfPCell.BackgroundColor = BaseColor.YELLOW; // aqui no le vamos a poner colores
                    tablaparaestepedidoEUR.AddCell(_PdfPCell3pedido);

                    _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, Font.NORMAL);
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

                    _fontStylepedido = FontFactory.GetFont("Tahoma", 8f, Font.NORMAL);
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

    public class ReporteSaldo
    {
        [Key]
        public int Rep { get; set; }

        [DisplayName("Precio dolar:")]
        public decimal TC { get; set; }
        public string Titulo { get; set; }

    }

    public class ClientesFacturas
    {
        [Key]
        public int IDCliente { get; set; }

        public string Nombre { get; set; }

        public string Telefono { get; set; }

        public int IDVendedor { get; set; }

        //public virtual Vendedor vendedor { get; set; }
    }


    public class EncFac
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

        public decimal TC { get; set; }
    }

    public class MonedaSal
    {
        [Key]
        public int IMoneda { get; set; }
       public string ClaveMoneda { get; set; }
    }



    public class ClientesFacturasContext : DbContext
    {
        public ClientesFacturasContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ClientesFacturasContext>(null);
        }
        public DbSet<EncFac> Factura { get; set; }
        public DbSet<ClientesFacturas> Clientes { get; set; }
        

    }


}
