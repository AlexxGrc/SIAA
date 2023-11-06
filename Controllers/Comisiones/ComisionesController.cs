using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Comercial;//
using SIAAPI.Models.Comisiones;
using System.Collections;
using SIAAPI.clasescfdi;
using SIAAPI.Models.Administracion;
using SIAAPI.ViewModels.Comercial;
using SIAAPI.Models;
using SIAAPI.ViewModels.Comisiones;
using SIAAPI.Reportes;
using SIAAPI.Models.Cfdi;
using SIAAPI.ViewModels.Cfdi;
using PagedList;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Globalization;
using SIAAPI.ClasesProduccion;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using SIAAPI.Models.Login;

namespace SIAAPI.Controllers.Comisiones
{
    public class ComisionesController : Controller
    {
        PrefacturaContext db = new PrefacturaContext();
        CierreVentasContext db2 = new CierreVentasContext();
        CierreComisionesContext db3 = new CierreComisionesContext();
        // GET: Comisiones
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult CierreVentas()
        {
            CierreVentas periodo = new CierreVentas();
            periodo.IDMes = DateTime.Now.Month;
            periodo.Ano = DateTime.Now.Year;
            ViewBag.IDMes = new SelectList(db2.c_MesesBD, "IDMes", "Mes", periodo.IDMes);

            return View(periodo);
        }

        [HttpPost]
        public ActionResult CierreVentas(int IDMes, int Ano)//FormCollection collecion)
        {
            var vendedores = new VendedorContext().Vendedores.Where(s => s.Activo == true).ToList();

            ViewBag.Mes = IDMes;
            ViewBag.Ano = Ano;
            ViewBag.IDMes = new SelectList(db2.c_MesesBD, "IDMes", "Mes");

            string cadenaV = "select * from CierreVentas where ano=" + Ano + " and idmes=" + IDMes + "";
            List<CierreVentas> datosrep = db.Database.SqlQuery<CierreVentas>(cadenaV).ToList();
            try
            {
                if (datosrep.Count <= 0)
                {
                    foreach (Vendedor vendedor in vendedores)
                    {
                        int IDVendedor = vendedor.IDVendedor;
                        int NumPedidos = c_NumDePedidos(vendedor.IDVendedor, IDMes, Ano);
                        decimal VentasMXN = c_VentasMXN(vendedor.IDVendedor, IDMes, Ano);
                        decimal VentasUSD = c_VentasUSD(vendedor.IDVendedor, IDMes, Ano);
                        decimal Cuota = r_Cuota(vendedor.IDVendedor, IDMes, Ano);
                        decimal CuotaAlcanzada = c_CuotaAlcanzada(vendedor.IDVendedor, IDMes, Ano);


                        decimal IDMonedaCA = r_MonedaCA(vendedor.IDVendedor, IDMes, Ano);
                        decimal Comision = r_Comision(vendedor.IDVendedor, IDMes, Ano);

                        decimal ComisionReal = 0;
                        decimal operacion = 0;
                        decimal operacion1 = 0;
                        decimal operacion2 = 0;
                        try
                        {
                            operacion = (CuotaAlcanzada * 100) / Cuota;

                            operacion1 = (operacion / 100);

                            operacion2 = operacion1 * Comision;
                        }
                        catch ( Exception err)
                        {

                        }
                        string cadenatipoc = "Select idtipocuota as Dato from vendedor where idvendedor=" + IDVendedor +"";
                        ClsDatoEntero IDTIPOC = db.Database.SqlQuery<ClsDatoEntero>(cadenatipoc).ToList().FirstOrDefault();
                        if (IDTIPOC.Dato == 1)
                        {
                            ComisionReal = Comision;
                        }
                        else
                        {
                            if (CuotaAlcanzada > Cuota || CuotaAlcanzada == Cuota)
                            {

                                ComisionReal = Comision;

                            }
                            else
                            {
                                ComisionReal = operacion2;
                                //ComisionReal = Comision;
                            }

                        }

                        
                        if (NumPedidos != 0)
                        {
                            db2.Database.ExecuteSqlCommand("insert into CierreVentas(IDMes, Ano, IDVendedor, NumPedidos, VentasMXN, VentasUSD, Cuota, CuotaAlcanzada, IDMonedaCA, Comision) values(" + IDMes + ", " + Ano + ", " + IDVendedor + ", " + NumPedidos + ", " + VentasMXN + ", " + VentasUSD + ", " + Cuota + ", " + CuotaAlcanzada + ", " + IDMonedaCA + ", " + ComisionReal + ")");
                        }

                    }
                    return RedirectToAction("ListaCierreVentas", new { ANIO = Ano, IDMes = IDMes });
                }
                else
                {
                 return   RedirectToAction("ListaCierreVentas", new { ANIO = Ano, IDMes = IDMes });
                }
            }
            catch (Exception err)
            {
                return Json(err.Message);
            }

          
        }

        public ActionResult ListaCierreVentas(int ANIO, int IDMes, string Enviar = "Aplicar")
        {
            if (ANIO == 0 || IDMes == 0)
            {
                ANIO = DateTime.Now.Year;
                IDMes = DateTime.Now.Month;
            }

            String fec = DateTime.Now.ToShortDateString();
            string cadenaV = "select * from CierreVentas where ano=" + ANIO + " and idmes=" + IDMes + "";
            List<CierreVentas> datosrep = db.Database.SqlQuery<CierreVentas>(cadenaV).ToList();

            ViewBag.IDMes = new SelectList(db2.c_MesesBD, "IDMes", "Mes");
            ViewBag.ANIO = new SelectList(GetAnios(), "Value", "Text");

            string cual = Enviar;
            if (cual == "Aplicar")
            {

                try
                {


                    if (datosrep.Count <= 0)
                    {
                        ViewBag.Mensaje = "No hay Cierre de este mes";
                    }

                }
                catch (Exception err)
                {
                    ViewBag.Mensaje = "Hay un error en la consulta de datos";
                }

                return View(datosrep);
            }

            if (cual == "Reporte")
            {
                ReporteCierreVentas report = new ReporteCierreVentas();
                byte[] abytes = report.PrepareReport(ANIO, IDMes);
                return File(abytes, "application/pdf", "ReporteCierreVentas.pdf");
            }

            if (cual == "Excel")
            {

                List<VCierreVentas> cierrevtas = new List<VCierreVentas>();
                VCierreVentasContext db3 = new VCierreVentasContext();
                string cadenaCV = "select * from dbo.VCierreVentas  where IDMes = " + IDMes + " and Año= " + ANIO + "";
                cierrevtas = db3.Database.SqlQuery<VCierreVentas>(cadenaCV).ToList();

                ExcelPackage Ep = new ExcelPackage();
                var Sheet = Ep.Workbook.Worksheets.Add("Cierre Ventas");

                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:J1"].Style.Font.Size = 20;
                Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:J1"].Style.Font.Bold = true;
                Sheet.Cells["A1:J1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Cierre de Ventas");
                Sheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                row = 2;
                Sheet.Cells["A2:J2"].Style.Font.Size = 12;
                Sheet.Cells["A2:J2"].Style.Font.Name = "Calibri";
                Sheet.Cells["A2:J2"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:J2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos

                c_MesesContext db = new c_MesesContext();
                var meses = db.c_Meses.Single(m => m.IDMes == IDMes);
                string mesnombre = meses.Mes.ToString();

                Sheet.Cells[string.Format("A2", row)].Value = "Mes";
                Sheet.Cells[string.Format("B2", row)].Value = mesnombre;
                Sheet.Cells[string.Format("D2", row)].Value = "Año";
                Sheet.Cells[string.Format("E2", row)].Value = ANIO;
                Sheet.Cells[string.Format("F2", row)].Value = "Fecha Impresión";
                Sheet.Cells[string.Format("G2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("G2", row)].Value = fec;


                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:J3"].Style.Font.Bold = true;
                Sheet.Cells["A3:J3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:J3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Mes");
                Sheet.Cells["B3"].RichText.Add("Año");
                Sheet.Cells["C3"].RichText.Add("Vendedor");
                Sheet.Cells["D3"].RichText.Add("Pedidos Totales");
                Sheet.Cells["E3"].RichText.Add("Ventas MXN");
                Sheet.Cells["F3"].RichText.Add("Ventas USD");
                Sheet.Cells["G3"].RichText.Add("Cuota Establecida");
                Sheet.Cells["H3"].RichText.Add("Cuota Alcanzada");
                Sheet.Cells["I3"].RichText.Add("Moneda de Cuota");
                Sheet.Cells["J3"].RichText.Add("Comisión");


                row = 4;
                foreach (var item in cierrevtas)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.Mes;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Año;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Vendedor;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.PedidosTotales;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.VentasMXN;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.VentasUSD;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.CuotaEstablecida;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.CuotaAlcanzada;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.Moneda;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.Comision;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReportecierreVentas.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return View(datosrep);
            }


            return View(datosrep);
        }

        //public ActionResult ListaCierreComisiones(int ANIO, int IDMes, string Enviar = "Aplicar")
        //{
        //    if (ANIO == 0 || IDMes == 0)
        //    {
        //        ANIO = DateTime.Now.Year;
        //        IDMes = DateTime.Now.Month;
        //    }
        //    string cadena = "select * from ComisionVendedor where anioCom=" + ANIO + " and mesCom=" + IDMes + " order by IDVendedor";
        //    List<CierreComisiones> datosrep = db.Database.SqlQuery<CierreComisiones>(cadena).ToList();

        //    String fec = DateTime.Now.ToShortDateString();

        //    ViewBag.IDMes = new SelectList(db2.c_MesesBD, "IDMes", "Mes";
        //    ViewBag.ANIO = new SelectList(GetAnios(), "Value", "Text");

        //    string cual = Enviar;
        //    if (cual == "Aplicar")
        //    {

        //        try
        //        {


        //            if (datosrep.Count <= 0)
        //            {
        //                ViewBag.Mensaje = "No hay Cierre de este mes";
        //            }

        //        }
        //        catch (Exception err)
        //        {
        //            ViewBag.Mensaje = "Hay un error en la consulta de datos";
        //        }

        //        return View(datosrep);
        //    }

        //    if (cual == "Reporte")
        //    {
        //        ReporteCierreComisiones report = new ReporteCierreComisiones();
        //        byte[] abytes = report.PrepareReport(ANIO, IDMes);
        //        return File(abytes, "application/pdf", "ReporteCierreComisiones.pdf");
        //    }

        //    if (cual == "Excel")
        //    {

        //        List<VComisionVendedor> cierrecom = new List<VComisionVendedor>();
        //        List<VArticuloDet> artdet = new List<VArticuloDet>();
        //        VComisionVendedorContext db3 = new VComisionVendedorContext();
        //        string cadenaCV = "select * from VComisionVendedor  where mesCom = " + IDMes + " and anioCom = " + ANIO + "";
        //        cierrecom = db3.Database.SqlQuery<VComisionVendedor>(cadenaCV).ToList();
        //        ViewBag.Cierrecom = cierrecom;

        //        /////

        //        List<VArticuloDet> listaart = new List<VArticuloDet>();

        //        ExcelPackage Ep = new ExcelPackage();
        //        var Sheet = Ep.Workbook.Worksheets.Add("Cierre Comisiones");

        //        int row = 1;
        //        //Fijar la fuente para A1:Q1
        //        Sheet.Cells["A1:X1"].Style.Font.Size = 20;
        //        Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
        //        Sheet.Cells["A1:X1"].Style.Font.Bold = true;
        //        Sheet.Cells["A1:X1"].Style.Font.Color.SetColor(Color.DarkBlue);
        //        Sheet.Cells["A1:X1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
        //        Sheet.Cells["A1"].RichText.Add("Cierre de Comisiones");
        //        Sheet.Cells["A1:X1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

        //        row = 2;
        //        Sheet.Cells["A2:X2"].Style.Font.Size = 12;
        //        Sheet.Cells["A2:X2"].Style.Font.Name = "Calibri";
        //        Sheet.Cells["A2:X2"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
        //        Sheet.Cells["A2:X2"].Style.Font.Bold = true;
        //        //Subtitulo según el filtrado del periodo de datos

        //        c_MesesContext db = new c_MesesContext();
        //        var meses = db.c_Meses.Single(m => m.IDMes == IDMes);
        //        string mesnombre = meses.Mes.ToString();

        //        Sheet.Cells[string.Format("A2", row)].Value = "Mes";
        //        Sheet.Cells[string.Format("B2", row)].Value = mesnombre;
        //        Sheet.Cells[string.Format("D2", row)].Value = "Año";
        //        Sheet.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //        Sheet.Cells[string.Format("E2", row)].Value = ANIO;
        //        Sheet.Cells[string.Format("F2", row)].Value = "Fecha Impresión";
        //        Sheet.Cells[string.Format("G2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
        //        Sheet.Cells[string.Format("G2", row)].Value = fec;


        //        //En la fila3 se da el formato a el encabezado
        //        row = 3;
        //        Sheet.Cells.Style.Font.Name = "Calibri";
        //        Sheet.Cells.Style.Font.Size = 10;
        //        Sheet.Cells["A3:X3"].Style.Font.Bold = true;
        //        Sheet.Cells["A3:X3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //        Sheet.Cells["A3:X3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //        // Se establece el nombre que identifica a cada una de las columnas de datos. 

        //        Sheet.Cells["A3"].RichText.Add("ID");
        //        Sheet.Cells["B3"].RichText.Add("Fecha Factura");
        //        Sheet.Cells["C3"].RichText.Add("Fecha Pedido");
        //        Sheet.Cells["D3"].RichText.Add("Vendedor");
        //        Sheet.Cells["E3"].RichText.Add("Cliente");
        //        Sheet.Cells["F3"].RichText.Add("Serie");
        //        Sheet.Cells["G3"].RichText.Add("NoFactura");
        //        Sheet.Cells["H3"].RichText.Add("Subtotal");
        //        Sheet.Cells["I3"].RichText.Add("Rentabilidad");
        //        Sheet.Cells["J3"].RichText.Add("Penalizaciones");
        //        Sheet.Cells["K3"].RichText.Add("Comisión");
        //        Sheet.Cells["L3"].RichText.Add("Tipo");
        //        Sheet.Cells["M3"].RichText.Add("Pagada");
        //        Sheet.Cells["N3"].RichText.Add("Costo");
        //        Sheet.Cells["O3"].RichText.Add("Base Comisionable");
        //        Sheet.Cells["P3"].RichText.Add("Moneda Factura");
        //        Sheet.Cells["Q3"].RichText.Add("Moneda Costo");
        //        Sheet.Cells["R3"].RichText.Add("Monto Rentabilidad");
        //        Sheet.Cells["S3"].RichText.Add("Monto Comisión");
        //        Sheet.Cells["T3"].RichText.Add("TC");
        //        Sheet.Cells["U3"].RichText.Add("Monto Comisión Pesos");
        //        Sheet.Cells["V3"].RichText.Add("No Pedido");
        //        Sheet.Cells["W3"].RichText.Add("Año Comisión");
        //        Sheet.Cells["X3"].RichText.Add("Mes Comisión");


        //        row = 4;
        //        foreach (var item in cierrecom)
        //        {
        //            ///
        //            string cadenasql = "select  E.SerieDigital, E.NumeroDigital, A.Cref, A.[Descripcion] as Articulo, D.Presentacion, D.Cantidad,dbo.getcosto(0,D.IDArticulo, D.Cantidad) as CostoArticulo, M.ClaveMoneda, D.Costo as PrecioCliente from[dbo].[DetPrefactura] D inner join[dbo].[EncPrefactura] E on   D.[IDPrefactura] = E.[IDPrefactura] inner join ([dbo].[Articulo] A inner join c_Moneda M on A.IDMoneda = M.IDMoneda) on D.IDArticulo = A.IDArticulo where SerieDigital = '" + item.Serie + "'  and NumeroDigital = '" + item.NoFactura + "'";
        //            List<VArticuloDet> articulo = db2.Database.SqlQuery<VArticuloDet>(cadenasql).ToList();
        //            ViewBag.articulo = articulo;

        //            foreach (VArticuloDet a in ViewBag.articulo)
        //            {
        //                listaart.Add(a);
        //                ViewBag.articiuloall = listaart;
        //            }
        //            ///

        //            Sheet.Cells[string.Format("A{0}", row)].Value = item.IDComisionVendedor;
        //            Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
        //            Sheet.Cells[string.Format("B{0}", row)].Value = item.FechaFac;
        //            Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
        //            Sheet.Cells[string.Format("C{0}", row)].Value = item.FechaPed;
        //            Sheet.Cells[string.Format("D{0}", row)].Value = item.Vendedor;
        //            Sheet.Cells[string.Format("E{0}", row)].Value = item.Cliente;
        //            Sheet.Cells[string.Format("F{0}", row)].Value = item.Serie;
        //            Sheet.Cells[string.Format("G{0}", row)].Value = item.NoFactura;
        //            Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //            Sheet.Cells[string.Format("H{0}", row)].Value = item.Subtotal;

        //            Sheet.Cells[string.Format("I{0}", row)].Value = item.Rentabilidad;
        //            Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //            Sheet.Cells[string.Format("J{0}", row)].Value = item.Penalizaciones;

        //            Sheet.Cells[string.Format("K{0}", row)].Value = item.Comision;
        //            Sheet.Cells[string.Format("L{0}", row)].Value = item.Tipo;
        //            if (item.Pagada == true)
        //            {
        //                Sheet.Cells[string.Format("M{0}", row)].Value = "SI";
        //            }
        //            else
        //            {
        //                Sheet.Cells[string.Format("M{0}", row)].Value = "NO";
        //            }
        //            Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //            Sheet.Cells[string.Format("N{0}", row)].Value = item.Costo;
        //            Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //            Sheet.Cells[string.Format("O{0}", row)].Value = item.BaseComisionable;

        //            Sheet.Cells[string.Format("P{0}", row)].Value = item.MonedaFactura;
        //            Sheet.Cells[string.Format("Q{0}", row)].Value = item.MonedaCosto;
        //            Sheet.Cells[string.Format("R{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //            Sheet.Cells[string.Format("R{0}", row)].Value = item.MontoRentabilidad;
        //            Sheet.Cells[string.Format("S{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //            Sheet.Cells[string.Format("S{0}", row)].Value = item.MontoComision;

        //            Sheet.Cells[string.Format("T{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //            Sheet.Cells[string.Format("T{0}", row)].Value = item.TC;
        //            Sheet.Cells[string.Format("U{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //            Sheet.Cells[string.Format("U{0}", row)].Value = item.comisionpesos;
        //            Sheet.Cells[string.Format("V{0}", row)].Value = item.NoPedido;
        //            Sheet.Cells[string.Format("W{0}", row)].Value = item.aniocom;
        //            Sheet.Cells[string.Format("X{0}", row)].Value = item.Mes;
        //            row++;
        //        }


        //        ///Hoja 2
        //        ///
        //        Sheet = Ep.Workbook.Worksheets.Add("Detalles");
        //        row = 1;
        //        //Fijar la fuente para A1:Q1
        //        Sheet.Cells["A1:I1"].Style.Font.Size = 20;
        //        Sheet.Cells["A1:I1"].Style.Font.Name = "Calibri";
        //        Sheet.Cells["A1:I3"].Style.Font.Bold = true;
        //        Sheet.Cells["A1:I1"].Style.Font.Color.SetColor(Color.DarkBlue);
        //        Sheet.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
        //        Sheet.Cells["A1"].RichText.Add("Detalle de Pedidos");

        //        row = 2;
        //        Sheet.Cells["A1:I1"].Style.Font.Size = 12;
        //        Sheet.Cells["A1:I1"].Style.Font.Name = "Calibri";
        //        Sheet.Cells["A1:I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
        //        Sheet.Cells["A2:I2"].Style.Font.Bold = true;
        //        //Subtitulo según el filtrado del periodo de datos
        //        row = 2;

        //        Sheet.Cells[string.Format("A2", row)].Value = "Mes";
        //        Sheet.Cells[string.Format("B2", row)].Value = mesnombre;
        //        Sheet.Cells[string.Format("D2", row)].Value = "Año";
        //        Sheet.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //        Sheet.Cells[string.Format("E2", row)].Value = ANIO;
        //        Sheet.Cells[string.Format("F2", row)].Value = "Fecha Impresión";
        //        Sheet.Cells[string.Format("G2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
        //        Sheet.Cells[string.Format("G2", row)].Value = fec;

        //        //En la fila3 se da el formato a el encabezado
        //        row = 3;
        //        Sheet.Cells.Style.Font.Name = "Calibri";
        //        Sheet.Cells.Style.Font.Size = 10;
        //        Sheet.Cells["A3:I3"].Style.Font.Bold = true;
        //        Sheet.Cells["A3:I3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //        Sheet.Cells["A3:I3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

        //        // Se establece el nombre que identifica a cada una de las columnas de datos.
        //        Sheet.Cells["A3"].RichText.Add("Serie");
        //        Sheet.Cells["B3"].RichText.Add("Numero");
        //        Sheet.Cells["C3"].RichText.Add("Clave");
        //        Sheet.Cells["D3"].RichText.Add("Articulo");
        //        Sheet.Cells["E3"].RichText.Add("Presentación"); ;
        //        Sheet.Cells["F3"].RichText.Add("Cantidad");
        //        Sheet.Cells["G3"].RichText.Add("Costo Articulo");
        //        Sheet.Cells["H3"].RichText.Add("Moneda");
        //        Sheet.Cells["I3"].RichText.Add("Precio Cliente");

        //        //Aplicar borde doble al rango de celdas A3:Q3
        //        Sheet.Cells["A3:I3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

        //        // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
        //        // Se establecen los formatos para las celdas: Fecha, Moneda
        //        row = 4;
        //        Sheet.Cells.Style.Font.Bold = false;
        //        foreach (VArticuloDet itemD in ViewBag.articiuloall)
        //        {
        //            Sheet.Cells[string.Format("A{0}", row)].Value = itemD.SerieDigital;
        //            Sheet.Cells[string.Format("B{0}", row)].Value = itemD.NumeroDigital;
        //            Sheet.Cells[string.Format("C{0}", row)].Value = itemD.Cref;
        //            Sheet.Cells[string.Format("D{0}", row)].Value = itemD.Articulo;
        //            Sheet.Cells[string.Format("E{0}", row)].Value = itemD.Presentacion;
        //            Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "0.0000";
        //            Sheet.Cells[string.Format("F{0}", row)].Value = itemD.Cantidad;
        //            Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //            Sheet.Cells[string.Format("G{0}", row)].Value = itemD.CostoArticulo;
        //            Sheet.Cells[string.Format("H{0}", row)].Value = itemD.ClaveMoneda;
        //            Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //            Sheet.Cells[string.Format("I{0}", row)].Value = itemD.PrecioCliente;
        //            row++;
        //        }
        //        Sheet.Cells["A:AZ"].AutoFitColumns();
        //        Response.Clear();
        //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        Response.AddHeader("content-disposition", "attachment: filename=" + "ReportecierreComision.xlsx");
        //        Response.BinaryWrite(Ep.GetAsByteArray());
        //        Response.End();
        //        return View(datosrep);
        //    }

        //    return View(datosrep);
        //}

        public ActionResult CierreComisiones(string Mensaje="")
        {
            ViewBag.Mensaje = Mensaje;
            VPagoComi periodo = new VPagoComi();
            //CierreComisiones periodo = new CierreComisiones();
            //VPagoComis periodo = new VPagoComis();
            periodo.IDMes = DateTime.Now.Month;
            periodo.Ano = DateTime.Now.Year;
            ViewBag.IDMes = new SelectList(db2.c_MesesBD, "IDMes", "Mes", periodo.IDMes);
            ViewBag.ANIO = GetAnios();
            //ViewBag.IDVendedor = new SelectList(db2.VendedorBD, "IDVendedor", "Nombre");

            return View(periodo);
        }

        [HttpPost]
        public ActionResult CierreComisiones(VPagoComi periodo, FormCollection coleccion, string Enviar = "Aplicar")
        {
            #region

            #endregion
            try
            {
                string cual = Enviar;
                if (cual == "Excel")
                {
                    return RedirectToAction("ListaCierreComisiones", new { ANIO = periodo.Ano, IDMes = periodo.IDMes, Enviar=cual });
                }
            }
            catch (Exception err)
            {

            }
            try
            {

                c_Meses mes = new c_MesesContext().c_Meses.Find(periodo.IDMes);

                  List<DetComisionesPagos> pagos = new CierreComisionesContext().Database.SqlQuery<DetComisionesPagos>("select d.* from  DetComisionesPagos  as d inner join comisionesPagos as c on c.IDComisionesP=d.IDComisionesP where  c.cierre='0'").ToList();


                foreach (DetComisionesPagos d in pagos)
                {
                    new VendedorContext().Database.ExecuteSqlCommand("delete from DetComisionesPagos where IDDetComisionesP=" + d.IDDetComisionesP);
                }

                new VendedorContext().Database.ExecuteSqlCommand("delete from  ComisionesPagos where cierre='0'");

               
                int conteo = new VendedorContext().Database.SqlQuery<ClsDatoEntero>(" Select count([IDComisionesP]) as Dato from ComisionesPagos where cierre='0'").ToList().FirstOrDefault().Dato;

                if (conteo == 0)
                {

                    //string cadena = "select convert (int, ROW_NUMBER() OVER (ORDER BY p.fechapago)) as ID, p.fechapago, r.importepagado,IIF(r.idmoneda=180,'MXN','USD') AS Monedapago, f.serie, f.numero,f.fecha,f.IDCliente,f.subtotal,f.IDMoneda, V.IDVendedor from (((((pagofactura as p inner join documentorelacionado as r on p.idpagofactura=r.idpagofactura)inner join encfacturas  as f on r.idfactura=f.id) inner join encprefactura as pre on f.numero=pre.numerodigital) inner join Clientes as C on f.idcliente=C.IDCliente) inner join Vendedor as V on C.IDVendedor=V.IDVendedor) where  p.Estado=1 and p.liquidada='0' and r.importeSaldoinsoluto<10 and f.anticipo='0' and f.notacredito='0' and f.serie<> '' and p.fechapago >='2021-01-01' order by f.numero;";

                    string cadena = "select distinct p.IDPagoFactura FROM PagoFactura as p  inner join DocumentoRelacionado as d on d.IDPagoFactura=p.IDPagoFactura where d.ImporteSaldoInsoluto<10 and  d.serie='MX' and d.statusdocto='A' and p.Estado='1' and p.liquidada='0'";
                    List<PagoFacturaCom> datos = db.Database.SqlQuery<PagoFacturaCom>(cadena).ToList();
                    //List<VPagoComi> datos = db.Database.SqlQuery<VPagoComi>(cadena).ToList();
                    int contador = 0;
                    foreach (PagoFacturaCom elemento in datos)
                    {
                        List<DocumentoRelacionado> relacionados = new PagoFacturaContext().Database.SqlQuery<DocumentoRelacionado>("select*from DocumentoRelacionado where IDPagoFactura =" + elemento.IDPagoFactura + " and StatusDocto='A'").ToList();
                        try
                        {
                            if (relacionados.Count() != 0)
                            {
                                int num = 0;
                                string insert = "insert into ComisionesPagos (Mes, Anno, IDPago) values('" + mes.Mes + "', '" + periodo.Ano + "'," + elemento.IDPagoFactura + ")";

                                db.Database.ExecuteSqlCommand(insert);



                                List<ComisionesPagos> numero;
                                numero = db.Database.SqlQuery<ComisionesPagos>("SELECT * FROM [dbo].[ComisionesPagos] WHERE IDComisionesP = (SELECT MAX(IDComisionesP) from ComisionesPagos)").ToList();
                                num = numero.Select(s => s.IDComisionesP).FirstOrDefault();

                                foreach (DocumentoRelacionado documento in relacionados)
                                {
                                    Encfacturas encfacturas = new EncfacturasSaldosContext().Database.SqlQuery<Encfacturas>("select*from EncFacturas where ID=" + documento.IDFactura).ToList().FirstOrDefault();
                                    Clientes cliente = new ClientesContext().Clientes.Find(encfacturas.IDCliente);
                                    string insertDet = "insert into DetComisionesPagos (IDComisionesP, IDDFactura, IDVendedor) values('" + num + "', '" + documento.IDFactura + "'," + cliente.IDVendedor + ")";

                                    db.Database.ExecuteSqlCommand(insertDet);

                                }
                            }

                        }
                        catch (Exception err)
                        {

                        }
                       


                    }
                    return RedirectToAction("CierreComisiones", new { Mensaje="Cierre de comisiones listo" });
                }
                if (conteo>0)
                {
                    return RedirectToAction("ListaCierreComisiones", new { ANIO = periodo.Ano, IDMes = periodo.IDMes, Enviar="Excel" });
                }

            }
            catch (Exception err)
            {

            }


            ViewBag.Mes = periodo.IDMes;
            ViewBag.Ano = periodo.Ano;
            ViewBag.IDMes = new SelectList(db2.c_MesesBD, "IDMes", "Mes");

            return View(periodo);
        }




        //public ActionResult ListaCierreComisiones(int ANIO, int IDMes, string Enviar = "Aplicar")
        //{
        //    if (ANIO == 0 || IDMes == 0)
        //    {
        //        ANIO = DateTime.Now.Year;
        //        IDMes = DateTime.Now.Month;
        //    }


        //    string cadenaven = "select distinct(IDVendedor) from ComisionVendedor where cierre='0' order by IDVendedor";

        //    List<CierreComisiones> datosven = db.Database.SqlQuery<CierreComisiones>(cadenaven).ToList();
        //    ViewBag.datosven = datosven;
        //    //ClsDatoEntero nvendedores = db.Database.SqlQuery<ClsDatoEntero>("select count(distinct(IDVendedor)) as Dato from ComisionVendedor where anioCom = " + ANIO + " and mesCom = " + IDMes + "").ToList()[0];



        //    string cadena = "select * from ComisionVendedor where  cierre='0'";
        //    List<CierreComisiones> datosrep = db.Database.SqlQuery<CierreComisiones>(cadena).ToList();

        //    String fec = DateTime.Now.ToShortDateString();

        //    ViewBag.IDMes = new SelectList(db2.c_MesesBD, "IDMes", "Mes");
        //    ViewBag.ANIO = new SelectList(GetAnios(), "Value", "Text");

        //    string cual = Enviar;
        //    if (cual == "Aplicar")
        //    {

        //        try
        //        {


        //            //if (datosrep.Count <= 0)
        //            //{
        //            //    ViewBag.Mensaje = "No hay Cierre de este mes";
        //            //}

        //        }
        //        catch (Exception err)
        //        {
        //            ViewBag.Mensaje = "Hay un error en la consulta de datos";
        //        }

        //        //return View(datosrep);
        //    }

        //    if (cual == "Reporte")
        //    {
        //        ReporteCierreComisiones report = new ReporteCierreComisiones();
        //        byte[] abytes = report.PrepareReport(ANIO, IDMes);
        //        return File(abytes, "application/pdf", "ReporteCierreComisiones.pdf");
        //    }
        //    try
        //    {
        //        if (cual == "Excel")
        //        {
        //            VComisionVendedorContext db3 = new VComisionVendedorContext();
        //            c_Meses mes = new c_MesesContext().c_Meses.Find(IDMes);


        //            try
        //            {
        //                string update = "delete from ComisionVendedor where cierre='0'";
        //                new CierreComisionesContext().Database.ExecuteSqlCommand(update);

        //            }
        //            catch (Exception err)
        //            {

        //            }
        //            List<Vendedores> listVendedores = new List<Vendedores>();

        //            string cadenaV = "select distinct v.IDVendedor, v.Nombre as NombreVendedor from ComisionesPagos as c inner join DetComisionesPagos as d on c.IDComisionesP=d.IDComisionesP inner join Vendedor as v on d.IDVendedor=v.IDVendedor where   cierre='0' order by v.Nombre";
        //            listVendedores = db3.Database.SqlQuery<Vendedores>(cadenaV).ToList();

        //            ExcelPackage Ep = new ExcelPackage();


        //            foreach (Vendedores vendedores in listVendedores)
        //            {
        //                List<ComisionesPagos> cierrecom = new List<ComisionesPagos>();

        //                string cadenaCV = "select distinct c.*from ComisionesPagos as c inner join DetComisionesPagos as d on d.IDComisionesP=c.IDComisionesP  where cierre='0' and  d.idvendedor=" + vendedores.IDVendedor;
        //                cierrecom = db3.Database.SqlQuery<ComisionesPagos>(cadenaCV).ToList();

        //                var Sheet = Ep.Workbook.Worksheets.Add(vendedores.NombreVendedor);

        //                int row = 1;
        //                //Fijar la fuente para A1:Q1
        //                Sheet.Cells["A1:AA1"].Style.Font.Size = 20;
        //                Sheet.Cells["A1:AA1"].Style.Font.Name = "Calibri";
        //                Sheet.Cells["A1:AA1"].Style.Font.Bold = true;
        //                Sheet.Cells["A1:AA1"].Style.Font.Color.SetColor(Color.DarkBlue);
        //                Sheet.Cells["A1:AA1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
        //                Sheet.Cells["A1"].RichText.Add("Cierre de Comisiones");
        //                Sheet.Cells["A1:AA1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

        //                row = 2;
        //                Sheet.Cells["A2:AA2"].Style.Font.Size = 12;
        //                Sheet.Cells["A2:AA2"].Style.Font.Name = "Calibri";
        //                Sheet.Cells["A2:AA2"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
        //                Sheet.Cells["A2:AA2"].Style.Font.Bold = true;
        //                //Subtitulo según el filtrado del periodo de datos



        //                Sheet.Cells[string.Format("A2", row)].Value = "Mes";
        //                Sheet.Cells[string.Format("B2", row)].Value = mes.Mes;
        //                Sheet.Cells[string.Format("D2", row)].Value = "Año";
        //                Sheet.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                Sheet.Cells[string.Format("E2", row)].Value = ANIO;
        //                Sheet.Cells[string.Format("F2", row)].Value = "Fecha Impresión";
        //                Sheet.Cells[string.Format("G2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
        //                Sheet.Cells[string.Format("G2", row)].Value = DateTime.Now;



        //                row++;
        //                Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#90a1f8");

        //                colFromHex = System.Drawing.ColorTranslator.FromHtml("#95FAD7");
        //                Sheet.Cells[string.Format("A{0}:AA{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //                Sheet.Cells[string.Format("A{0}:AA{0}", row)].Style.Fill.BackgroundColor.SetColor(colFromHex);

        //                Sheet.Cells[string.Format("A{0}", row)].RichText.Add("No. Pago");
        //                Sheet.Cells[string.Format("B{0}", row)].RichText.Add("Fecha Pago");
        //                Sheet.Cells[string.Format("C{0}", row)].RichText.Add("Serie");
        //                Sheet.Cells[string.Format("D{0}", row)].RichText.Add("Folio");

        //                Sheet.Cells[string.Format("E{0}", row)].Value = "No. Factura";
        //                Sheet.Cells[string.Format("F{0}", row)].Value = "Fecha Factura";
        //                Sheet.Cells[string.Format("G{0}", row)].Value = "Prefactura";
        //                Sheet.Cells[string.Format("H{0}", row)].Value = "Fecha Prefactura";
        //                Sheet.Cells[string.Format("I{0}", row)].Value = "Cliente";

        //                Sheet.Cells[string.Format("J{0}", row)].Value = "Clave";
        //                Sheet.Cells[string.Format("K{0}", row)].Value = "Prsentación";
        //                Sheet.Cells[string.Format("L{0}", row)].Value = "Costo";
        //                Sheet.Cells[string.Format("M{0}", row)].Value = "Cantidad";
        //                Sheet.Cells[string.Format("N{0}", row)].Value = "Importe Prefactura S/Iva";
        //                Sheet.Cells[string.Format("O{0}", row)].Value = "Comisionable";
        //                Sheet.Cells[string.Format("P{0}", row)].Value = "Monto Comisionable";
        //                Sheet.Cells[string.Format("Q{0}", row)].Value = "% Rentabilidad";
        //                Sheet.Cells[string.Format("R{0}", row)].Value = "% Comisión";
        //                Sheet.Cells[string.Format("S{0}", row)].Value = "Monto Rentabilidad";
        //                Sheet.Cells[string.Format("T{0}", row)].Value = "Monto Comisión";
        //                Sheet.Cells[string.Format("U{0}", row)].Value = "Moneda Fac";
        //                Sheet.Cells[string.Format("V{0}", row)].Value = "Monto Comisión MXN";
        //                Sheet.Cells[string.Format("W{0}", row)].Value = "Total";
        //                Sheet.Cells[string.Format("X{0}", row)].Value = "Subtotal";
        //                Sheet.Cells[string.Format("Y{0}", row)].Value = "TC";
        //                Sheet.Cells[string.Format("Z{0}", row)].Value = "No.Pedido";
        //                Sheet.Cells[string.Format("AA{0}", row)].Value = "Fecha Pedido";
        //                row++;


        //                //En la fila3 se da el formato a el encabezado
        //                //row = 3;
        //                Sheet.Cells.Style.Font.Name = "Calibri";
        //                Sheet.Cells.Style.Font.Size = 10;
        //                Sheet.Cells["A3:AA3"].Style.Font.Bold = true;

        //                foreach (var item in cierrecom)
        //                {
        //                    PagoFactura pago = new PagoFacturaContext().PagoFacturas.Find(item.IDPago);

        //                    string detComi = "select*from DetComisionesPagos where IDComisionesP =" + item.IDComisionesP + " order by idvendedor";
        //                    List<DetComisionesPagos> relacionados = new PagoFacturaContext().Database.SqlQuery<DetComisionesPagos>(detComi).ToList();

        //                    //row++;
        //                    foreach (DetComisionesPagos det in relacionados)
        //                    {
        //                        Encfacturas fac = new EncfacturasSaldosContext().Database.SqlQuery<Encfacturas>("select*from encfacturas where id=" + det.IDDFactura).ToList().FirstOrDefault();
        //                        EncPrefactura detalles = new PrefacturaContext().Database.SqlQuery<EncPrefactura>("Select * from EncPrefactura where IDFacturaDigital=" + det.IDDFactura + " and status!='Cancelado'").ToList().FirstOrDefault();

        //                        if (detalles != null)
        //                        {
        //                            Clientes clientes = new ClientesContext().Clientes.Find(detalles.IDCliente);

        //                            try
        //                            {



        //                                ////Articulos
        //                                ///



        //                                try
        //                                {

        //                                    string detPrefa = "select*from DetPrefactura where IDPrefactura =" + detalles.IDPrefactura;
        //                                    List<DetPrefactura> DetP = new PagoFacturaContext().Database.SqlQuery<DetPrefactura>(detPrefa).ToList();

        //                                    foreach (DetPrefactura prefactura in DetP)
        //                                    {


        //                                        Articulo articulo = new ArticuloContext().Articulo.Find(prefactura.IDArticulo);
        //                                        Caracteristica caracteristica = new CarritoContext().Database.SqlQuery<Caracteristica>("select*from Caracteristica where id=" + prefactura.Caracteristica_ID).ToList().FirstOrDefault();

        //                                        decimal Costo = 0M;

        //                                        try
        //                                        {
        //                                            Costo = c_Costo(prefactura.Caracteristica_ID, prefactura.IDArticulo, prefactura.Cantidad, fac.ID, prefactura.IDPrefactura);//
        //                                        }
        //                                        catch (Exception err)
        //                                        {

        //                                        }

        //                                        bool escomisionableart = r_escomisionale(articulo.IDArticulo);
        //                                        string Comisionable = "No es comisionable";
        //                                        if (escomisionableart)
        //                                        {
        //                                            Comisionable = "Es Comisionable";
        //                                        }
        //                                        decimal basecomisionable = 0M;
        //                                        try
        //                                        {
        //                                            //importe det detprefactura
        //                                            basecomisionable = c_basecomisionable(prefactura.IDDetPrefactura);
        //                                        }
        //                                        catch (Exception err)
        //                                        {

        //                                        }


        //                                        decimal Rentabilidad = 0;
        //                                        try
        //                                        {
        //                                            Rentabilidad = (1 - (Costo / basecomisionable));
        //                                        }
        //                                        catch (Exception err)
        //                                        {
        //                                            Rentabilidad = 0.3M;
        //                                        }
        //                                        //        //
        //                                        //        decimal Penalizaciones = 0;//


        //                                        decimal Comision = 0;
        //                                        int IDPedido = 0;
        //                                        DateTime FechaPedido = DateTime.Now;
        //                                        try
        //                                        {

        //                                            elementosprefactura ele = new PrefacturaContext().Database.SqlQuery<elementosprefactura>("select*from elementosprefactura where iddetprefactura=" + prefactura.IDDetPrefactura).ToList().FirstOrDefault();
        //                                            if (ele.documento == "Remision")
        //                                            {
        //                                                SIAAPI.clasescfdi.ClsRastreaDA rastrea = new SIAAPI.clasescfdi.ClsRastreaDA();
        //                                                List<SIAAPI.clasescfdi.NodoTrazo> nodos = rastrea.getDocumentoAnterior("Remision", ele.iddocumento, "Encabezado");
        //                                                foreach (SIAAPI.clasescfdi.NodoTrazo nodo in nodos)
        //                                                {
        //                                                    IDPedido = nodo.ID;
        //                                                }


        //                                            }
        //                                            if (ele.documento == "Pedido")
        //                                            {
        //                                                IDPedido = ele.iddocumento;
        //                                            }

        //                                            EncPedido pedido = new PedidoContext().EncPedidos.Find(IDPedido);
        //                                            FechaPedido = pedido.Fecha;
        //                                            Comision = r_Comision(vendedores.IDVendedor, pedido.Fecha.Month, pedido.Fecha.Year);
        //                                        }
        //                                        catch (Exception err)
        //                                        {

        //                                        }


        //                                        string Tipo = "0";
        //                                        bool Pagada = false;
        //                                        string MonedaFactura = "";
        //                                        c_MonedaContext C = new c_MonedaContext();


        //                                        try
        //                                        {
        //                                            MonedaFactura = C.c_Monedas.Find(fac.IDMoneda).ClaveMoneda;
        //                                        }
        //                                        catch (Exception err)
        //                                        {

        //                                        }
        //                                        string MonedaCosto = "";
        //                                        try
        //                                        {
        //                                            MonedaCosto = C.c_Monedas.Find(fac.IDMoneda).ClaveMoneda;
        //                                        }
        //                                        catch (Exception err)
        //                                        {

        //                                        }



        //                                        decimal montoPenalizacion = 0;



        //                                        List<c_Moneda> monedadestino = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();

        //                                        VCambio cambio;
        //                                        int monedaorigen = fac.IDMoneda;
        //                                        string fecha = FechaPedido.ToString("yyyy/MM/dd");

        //                                        try
        //                                        {
        //                                            cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + monedaorigen + ",180) as TC").ToList().FirstOrDefault();

        //                                        }
        //                                        catch (Exception err)
        //                                        {
        //                                            cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "',180,180) as TC").ToList().FirstOrDefault();
        //                                            string mensajeerror = err.Message;
        //                                        }



        //                                        //        ClsDatoDecimal can = db.Database.SqlQuery<ClsDatoDecimal>("select subtotal as Dato from encpedido where idpedido=" + pedido).ToList().FirstOrDefault();

        //                                        //        decimal subtotal = can.Dato;
        //                                        decimal basecomisionablereal = basecomisionable - montoPenalizacion;


        //                                        if (basecomisionablereal < 0)
        //                                        {
        //                                            basecomisionablereal = 0;
        //                                        }
        //                                        if (Rentabilidad < 0)
        //                                        {
        //                                            Rentabilidad = 0;
        //                                        }

        //                                        decimal MontoRentabilidad = 0M;
        //                                        decimal MontoComision = 0M;
        //                                        decimal porcenComi = (Comision / 100);
        //                                        if (vendedores.IDVendedor == 5)
        //                                        {

        //                                            try
        //                                            {
        //                                                //basecomisionablereal = basecomisionablereal* 0.1M;
        //                                                if (basecomisionablereal < 0)
        //                                                {
        //                                                    basecomisionablereal = 0;
        //                                                }
        //                                            }
        //                                            catch (Exception err)
        //                                            {

        //                                            }

        //                                            try
        //                                            {
        //                                                MontoComision = basecomisionablereal * porcenComi;
        //                                            }
        //                                            catch (Exception err)
        //                                            {

        //                                            }
        //                                        }
        //                                        else
        //                                        {

        //                                            try
        //                                        {
        //                                            basecomisionablereal = basecomisionablereal * Rentabilidad;
        //                                                if (basecomisionablereal < 0)
        //                                                {
        //                                                    basecomisionablereal = 0;
        //                                                }
        //                                            }
        //                                        catch (Exception err)
        //                                        {

        //                                        }

        //                                        try
        //                                        {
        //                                            MontoComision = Math.Round(basecomisionablereal, 3) * (Comision / 100);
        //                                        }
        //                                        catch (Exception err)
        //                                        {

        //                                        }
        //                                    }
        //                                        MontoRentabilidad = basecomisionable * Rentabilidad;
        //                                    // YA IGUALAMOS LA MOEDA DEL COSTO CON LA MONEDA DE LAPREFACTURA


        //                                    decimal comisionpesos = 0;
        //                                        decimal tc = 0;
        //                                        try
        //                                        {
        //                                            tc = Convert.ToDecimal(cambio.TC);
        //                                        }
        //                                        catch (Exception err)
        //                                        {
        //                                            tc = 0;
        //                                        }

        //                                        try
        //                                        {
        //                                            comisionpesos = MontoComision * tc;
        //                                        }
        //                                        catch (Exception err)
        //                                        {

        //                                        }

        //                                        string insert = "";

        //                                        if (escomisionableart)
        //                                        {
        //                                            insert = "insert into ComisionVendedor (NoPedido, FechaFac, FechaPed, IDVendedor, IDCliente, Serie, NoFactura, Subtotal, Rentabilidad, Penalizaciones,Comision, Tipo, Pagada,Costo,MonedaFactura, MonedaCosto, MontoRentabilidad, MontoComision, anioCom,mesCom, BaseComisionable, TC, ComisionPesos) " +
        //                                                "values(" + IDPedido + ", '" + fac.Fecha + "', '" + FechaPedido + "', " + clientes.IDVendedor + ", " + clientes.IDCliente + ", '" + fac.Serie + "', " + fac.Numero + ", " + prefactura.Importe + ", " + Rentabilidad + ", " + montoPenalizacion + ", " + Comision + ", " + Tipo + ", '0', " + Costo + ", '" + MonedaFactura + "', '" + MonedaCosto + "', " + MontoRentabilidad + ", " + MontoComision + "," + ANIO + "," + IDMes + "," + basecomisionablereal + "," + tc + "," + comisionpesos + ")";

        //                                            try
        //                                            {
        //                                                db2.Database.ExecuteSqlCommand(insert);


        //                                            }
        //                                            catch (Exception err)
        //                                            {
        //                                                string men = err.Message;
        //                                            }
        //                                        }


        //                                        ////
        //                                        ///

        //                                        Sheet.Cells[string.Format("A{0}", row)].Value = item.IDPago;
        //                                        Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
        //                                        Sheet.Cells[string.Format("B{0}", row)].Value = pago.FechaPago;
        //                                        Sheet.Cells[string.Format("C{0}", row)].Value = pago.Serie;
        //                                        Sheet.Cells[string.Format("D{0}", row)].Value = pago.Folio;


        //                                        ////
        //                                        ///
        //                                        Sheet.Cells[string.Format("E{0}", row)].Value = fac.Serie + " " + fac.Numero;
        //                                        Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
        //                                        Sheet.Cells[string.Format("F{0}", row)].Value = fac.Fecha;
        //                                        Sheet.Cells[string.Format("G{0}", row)].Value = detalles.Serie + " " + detalles.Numero;
        //                                        Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
        //                                        Sheet.Cells[string.Format("H{0}", row)].Value = detalles.Fecha;
        //                                        Sheet.Cells[string.Format("I{0}", row)].Value = clientes.Nombre;

        //                                        /////
        //                                        ///



        //                                        Sheet.Cells[string.Format("J{0}", row)].Value = articulo.Cref;
        //                                        Sheet.Cells[string.Format("K{0}", row)].Value = caracteristica.IDPresentacion;

        //                                        Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //                                        Sheet.Cells[string.Format("L{0}", row)].Value = Math.Round(Costo, 4);

        //                                        Sheet.Cells[string.Format("M{0}", row)].Value = prefactura.Cantidad;

        //                                        Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //                                        Sheet.Cells[string.Format("N{0}", row)].Value = Math.Round(prefactura.Importe, 4);

        //                                        Sheet.Cells[string.Format("O{0}", row)].Value = Comisionable;

        //                                        Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //                                        Sheet.Cells[string.Format("P{0}", row)].Value = Math.Round(basecomisionablereal, 4);

        //                                        //Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "#. ##%";
        //                                        if (Rentabilidad < 0)
        //                                        {
        //                                            colFromHex = System.Drawing.ColorTranslator.FromHtml("#E74C3C");
        //                                            Sheet.Cells[string.Format("Q{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //                                            Sheet.Cells[string.Format("Q{0}", row)].Style.Fill.BackgroundColor.SetColor(colFromHex);

        //                                        }
        //                                        Sheet.Cells[string.Format("Q{0}", row)].Value = Math.Round(Rentabilidad, 4);

        //                                        //Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "#. ##%";

        //                                        Sheet.Cells[string.Format("R{0}", row)].Value = porcenComi;

        //                                        Sheet.Cells[string.Format("S{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //                                        Sheet.Cells[string.Format("S{0}", row)].Value = Math.Round(MontoRentabilidad, 4);
        //                                        Sheet.Cells[string.Format("T{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //                                        Sheet.Cells[string.Format("T{0}", row)].Value = Math.Round(MontoComision, 4);

        //                                        Sheet.Cells[string.Format("U{0}", row)].Value = MonedaFactura;

        //                                        Sheet.Cells[string.Format("V{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //                                        Sheet.Cells[string.Format("V{0}", row)].Value = Math.Round(comisionpesos, 4);
        //                                        Sheet.Cells[string.Format("W{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //                                        Sheet.Cells[string.Format("W{0}", row)].Value =prefactura.Importe;
        //                                        Sheet.Cells[string.Format("X{0}", row)].Style.Numberformat.Format = "$#,##0.00";
        //                                        Sheet.Cells[string.Format("X{0}", row)].Value = prefactura.ImporteTotal;
        //                                        Sheet.Cells[string.Format("Y{0}", row)].Value = tc;
        //                                        Sheet.Cells[string.Format("Z{0}", row)].Value = IDPedido;
        //                                        Sheet.Cells[string.Format("AA{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
        //                                        Sheet.Cells[string.Format("AA{0}", row)].Value = FechaPedido;
        //                                        row++;

        //                                    }


        //                                }
        //                                catch (Exception er)
        //                                {

        //                                }
        //                            }
        //                            catch (Exception err)
        //                            {

        //                            }


        //                        }


        //                    }

        //                }
        //                Sheet.Cells["A:AZ"].AutoFitColumns();
        //            }




        //            Response.Clear();
        //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            Response.AddHeader("content-disposition", "attachment: filename=" + "ReportecierreComision.xlsx");
        //            Response.BinaryWrite(Ep.GetAsByteArray());
        //            Response.End();

        //        }

        //    }
        //    catch (Exception err)
        //    {

        //    }
        //    return View(datosrep);


        //}


        public ActionResult ListaCierreComisiones(int ANIO, int IDMes, string Enviar = "Aplicar")
        {
            if (ANIO == 0 || IDMes == 0)
            {
                ANIO = DateTime.Now.Year;
                IDMes = DateTime.Now.Month;
            }


            string cadenaven = "select distinct(IDVendedor) from ComisionVendedor where cierre='0' order by IDVendedor";

            List<CierreComisiones> datosven = db.Database.SqlQuery<CierreComisiones>(cadenaven).ToList();
            ViewBag.datosven = datosven;
            //ClsDatoEntero nvendedores = db.Database.SqlQuery<ClsDatoEntero>("select count(distinct(IDVendedor)) as Dato from ComisionVendedor where anioCom = " + ANIO + " and mesCom = " + IDMes + "").ToList()[0];



         
            String fec = DateTime.Now.ToShortDateString();

            ViewBag.IDMes = new SelectList(db2.c_MesesBD, "IDMes", "Mes");
            ViewBag.ANIO = new SelectList(GetAnios(), "Value", "Text");

            string cual = Enviar;
            if (cual == "Aplicar")
            {

                try
                {


                    //if (datosrep.Count <= 0)
                    //{
                    //    ViewBag.Mensaje = "No hay Cierre de este mes";
                    //}

                }
                catch (Exception err)
                {
                    ViewBag.Mensaje = "Hay un error en la consulta de datos";
                }

                //return View(datosrep);
            }

            if (cual == "Reporte")
            {
                ReporteCierreComisiones report = new ReporteCierreComisiones();
                byte[] abytes = report.PrepareReport(ANIO, IDMes);
                return File(abytes, "application/pdf", "ReporteCierreComisiones.pdf");
            }
            try
            {
                if (cual == "Excel")
                {
                    VComisionVendedorContext db3 = new VComisionVendedorContext();
                    c_Meses mes = new c_MesesContext().c_Meses.Find(IDMes);


                    try
                    {
                        string update = "delete from ComisionVendedor where cierre='0'";
                        new CierreComisionesContext().Database.ExecuteSqlCommand(update);

                    }
                    catch (Exception err)
                    {

                    }
                   
                    ExcelPackage Ep = new ExcelPackage();


             
                        List<ComisionesPagos> cierrecom = new List<ComisionesPagos>();

                        string cadenaCV = "select distinct c.*from ComisionesPagos as c inner join DetComisionesPagos as d on d.IDComisionesP=c.IDComisionesP inner join Vendedor as v on v.idvendedor=d.idvendedor where cierre='0' ";
                        cierrecom = db3.Database.SqlQuery<ComisionesPagos>(cadenaCV).ToList();

                        var Sheet = Ep.Workbook.Worksheets.Add("Cierre comisiones");

                        int row = 1;
                        //Fijar la fuente para A1:Q1
                        Sheet.Cells["A1:AA1"].Style.Font.Size = 20;
                        Sheet.Cells["A1:AA1"].Style.Font.Name = "Calibri";
                        Sheet.Cells["A1:AA1"].Style.Font.Bold = true;
                        Sheet.Cells["A1:AA1"].Style.Font.Color.SetColor(Color.DarkBlue);
                        Sheet.Cells["A1:AA1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        Sheet.Cells["A1"].RichText.Add("Cierre de Comisiones");
                        Sheet.Cells["A1:AA1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                        row = 2;
                        Sheet.Cells["A2:AA2"].Style.Font.Size = 12;
                        Sheet.Cells["A2:AA2"].Style.Font.Name = "Calibri";
                        Sheet.Cells["A2:AA2"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                        Sheet.Cells["A2:AA2"].Style.Font.Bold = true;
                        //Subtitulo según el filtrado del periodo de datos



                        Sheet.Cells[string.Format("A2", row)].Value = "Mes";
                        Sheet.Cells[string.Format("B2", row)].Value = mes.Mes;
                        Sheet.Cells[string.Format("D2", row)].Value = "Año";
                        Sheet.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        Sheet.Cells[string.Format("E2", row)].Value = ANIO;
                        Sheet.Cells[string.Format("F2", row)].Value = "Fecha Impresión";
                        Sheet.Cells[string.Format("G2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                        Sheet.Cells[string.Format("G2", row)].Value = DateTime.Now;



                        row++;
                        Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#90a1f8");

                        colFromHex = System.Drawing.ColorTranslator.FromHtml("#95FAD7");
                        Sheet.Cells[string.Format("A{0}:AA{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}:AA{0}", row)].Style.Fill.BackgroundColor.SetColor(colFromHex);

                        Sheet.Cells[string.Format("A{0}", row)].RichText.Add("");
                        Sheet.Cells[string.Format("B{0}", row)].Value = "Vendedor";
                        Sheet.Cells[string.Format("C{0}", row)].Value = "Cliente";
                        Sheet.Cells[string.Format("D{0}", row)].Value = "Días de crédito";

                        Sheet.Cells[string.Format("E{0}", row)].Value = "No. Pedido";
                        Sheet.Cells[string.Format("F{0}", row)].Value = "Fecha Pedido";
                        Sheet.Cells[string.Format("G{0}", row)].Value = "Mes de Pedido";
                        Sheet.Cells[string.Format("H{0}", row)].Value = "No. Factura";
                        Sheet.Cells[string.Format("I{0}", row)].Value = "Fecha Factura";

                        Sheet.Cells[string.Format("J{0}", row)].Value = "Mes Factura";
                        Sheet.Cells[string.Format("K{0}", row)].Value = "Subtotal";
                        Sheet.Cells[string.Format("L{0}", row)].Value = "Costo";
                        Sheet.Cells[string.Format("M{0}", row)].Value = "TC";
                        Sheet.Cells[string.Format("N{0}", row)].Value = "Moneda Fac";
                        Sheet.Cells[string.Format("O{0}", row)].Value = "Moneda Costo";
                        Sheet.Cells[string.Format("P{0}", row)].Value = "Año Comisión";
                        Sheet.Cells[string.Format("Q{0}", row)].Value = "Mes Comisión";
                        Sheet.Cells[string.Format("R{0}", row)].Value = "Monto Rentabilidad";
                        Sheet.Cells[string.Format("S{0}", row)].Value = "% Rentabilidad";
                        Sheet.Cells[string.Format("T{0}", row)].Value = "% Comisión";
                        Sheet.Cells[string.Format("U{0}", row)].Value = "Monto Comisión";
                    Sheet.Cells[string.Format("V{0}", row)].Value = "Monto Comisión Pesos";

                    row++;


                        //En la fila3 se da el formato a el encabezado
                        //row = 3;
                        Sheet.Cells.Style.Font.Name = "Calibri";
                        Sheet.Cells.Style.Font.Size = 10;
                        Sheet.Cells["A3:AA3"].Style.Font.Bold = true;

                        foreach (var item in cierrecom)
                        {
                            PagoFactura pago = new PagoFacturaContext().PagoFacturas.Find(item.IDPago);

                            string detComi = "select*from DetComisionesPagos where IDComisionesP =" + item.IDComisionesP + " order by idvendedor";
                            List<DetComisionesPagos> relacionados = new PagoFacturaContext().Database.SqlQuery<DetComisionesPagos>(detComi).ToList();

                            //row++;
                            foreach (DetComisionesPagos det in relacionados)
                            {
                                Encfacturas fac = new EncfacturasSaldosContext().Database.SqlQuery<Encfacturas>("select*from encfacturas where id=" + det.IDDFactura).ToList().FirstOrDefault();
                                EncPrefactura detalles = new PrefacturaContext().Database.SqlQuery<EncPrefactura>("Select * from EncPrefactura where IDFacturaDigital=" + det.IDDFactura + " and status!='Cancelado'").ToList().FirstOrDefault();
                            Vendedor vendedores = new VendedorContext().Vendedores.Find(det.IDVendedor);
                                if (detalles != null)
                                {
                                    Clientes clientes = new ClientesContext().Clientes.Find(detalles.IDCliente);
                                CondicionesPago condiciones = new CondicionesPagoContext().CondicionesPagos.Find(clientes.IDCondicionesPago);

                                    try
                                    {



                                        ////Articulos
                                        ///



                                        try
                                        {

                                            string detPrefa = "select*from DetPrefactura where IDPrefactura =" + detalles.IDPrefactura;
                                            List<DetPrefactura> DetP = new PagoFacturaContext().Database.SqlQuery<DetPrefactura>(detPrefa).ToList();

                                            foreach (DetPrefactura prefactura in DetP)
                                            {


                                                Articulo articulo = new ArticuloContext().Articulo.Find(prefactura.IDArticulo);
                                                Caracteristica caracteristica = new CarritoContext().Database.SqlQuery<Caracteristica>("select*from Caracteristica where id=" + prefactura.Caracteristica_ID).ToList().FirstOrDefault();

                                                decimal Costo = 0M;

                                                try
                                                {
                                                    Costo = c_Costo(prefactura.Caracteristica_ID, prefactura.IDArticulo, prefactura.Cantidad, fac.ID, prefactura.IDPrefactura);//
                                                }
                                                catch (Exception err)
                                                {

                                                }

                                                bool escomisionableart = r_escomisionale(articulo.IDArticulo);
                                                string Comisionable = "No es comisionable";
                                                if (escomisionableart)
                                                {
                                                    Comisionable = "Es Comisionable";
                                                }
                                                decimal basecomisionable = 0M;
                                                try
                                                {
                                                    //importe det detprefactura
                                                    basecomisionable = c_basecomisionable(prefactura.IDDetPrefactura);
                                                }
                                                catch (Exception err)
                                                {

                                                }


                                                decimal Rentabilidad = 0;
                                                try
                                                {
                                                    Rentabilidad = (1 - (Costo / basecomisionable));
                                                }
                                                catch (Exception err)
                                                {
                                                    Rentabilidad = 0.3M;
                                                }
                                                //        //
                                                //        decimal Penalizaciones = 0;//


                                                decimal Comision = 0;
                                                int IDPedido = 0;
                                                DateTime FechaPedido = DateTime.Now;
                                                try
                                                {

                                                    elementosprefactura ele = new PrefacturaContext().Database.SqlQuery<elementosprefactura>("select*from elementosprefactura where iddetprefactura=" + prefactura.IDDetPrefactura).ToList().FirstOrDefault();
                                                    if (ele.documento == "Remision")
                                                    {
                                                        SIAAPI.clasescfdi.ClsRastreaDA rastrea = new SIAAPI.clasescfdi.ClsRastreaDA();
                                                        List<SIAAPI.clasescfdi.NodoTrazo> nodos = rastrea.getDocumentoAnterior("Remision", ele.iddocumento, "Encabezado");
                                                        foreach (SIAAPI.clasescfdi.NodoTrazo nodo in nodos)
                                                        {
                                                            IDPedido = nodo.ID;
                                                        }


                                                    }
                                                    if (ele.documento == "Pedido")
                                                    {
                                                        IDPedido = ele.iddocumento;
                                                    }

                                                    EncPedido pedido = new PedidoContext().EncPedidos.Find(IDPedido);
                                                    FechaPedido = pedido.Fecha;
                                                    Comision = r_Comision(vendedores.IDVendedor, pedido.Fecha.Month, pedido.Fecha.Year);
                                                }
                                                catch (Exception err)
                                                {

                                                }


                                                string Tipo = "0";
                                                bool Pagada = false;
                                                string MonedaFactura = "";
                                                c_MonedaContext C = new c_MonedaContext();


                                                try
                                                {
                                                    MonedaFactura = C.c_Monedas.Find(fac.IDMoneda).ClaveMoneda;
                                                }
                                                catch (Exception err)
                                                {

                                                }
                                                string MonedaCosto = "";
                                                try
                                                {
                                                    MonedaCosto = C.c_Monedas.Find(fac.IDMoneda).ClaveMoneda;
                                                }
                                                catch (Exception err)
                                                {

                                                }



                                                decimal montoPenalizacion = 0;



                                                List<c_Moneda> monedadestino = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();

                                                VCambio cambio;
                                                int monedaorigen = fac.IDMoneda;
                                                string fecha = FechaPedido.ToString("yyyy/MM/dd");

                                                try
                                                {
                                                    cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + monedaorigen + ",180) as TC").ToList().FirstOrDefault();

                                                }
                                                catch (Exception err)
                                                {
                                                    cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "',180,180) as TC").ToList().FirstOrDefault();
                                                    string mensajeerror = err.Message;
                                                }


                                                decimal basecomisionablereal = basecomisionable - montoPenalizacion;


                                                if (basecomisionablereal < 0)
                                                {
                                                    basecomisionablereal = 0;
                                                }
                                                if (Rentabilidad < 0)
                                                {
                                                    Rentabilidad = 0;
                                                }

                                                decimal MontoRentabilidad = 0M;
                                                decimal MontoComision = 0M;
                                                decimal porcenComi = (Comision / 100);
                                                if (vendedores.IDVendedor == 5)
                                                {

                                                    try
                                                    {
                                                        //basecomisionablereal = basecomisionablereal* 0.1M;
                                                        if (basecomisionablereal < 0)
                                                        {
                                                            basecomisionablereal = 0;
                                                        }
                                                    }
                                                    catch (Exception err)
                                                    {

                                                    }

                                                    try
                                                    {
                                                        MontoComision = basecomisionablereal * porcenComi;
                                                    }
                                                    catch (Exception err)
                                                    {

                                                    }
                                                }
                                                else
                                                {

                                                    try
                                                    {
                                                        basecomisionablereal = basecomisionablereal * Rentabilidad;
                                                        if (basecomisionablereal < 0)
                                                        {
                                                            basecomisionablereal = 0;
                                                        }
                                                    }
                                                    catch (Exception err)
                                                    {

                                                    }

                                                    try
                                                    {
                                                        MontoComision = Math.Round(basecomisionablereal, 3) * (Comision / 100);
                                                    }
                                                    catch (Exception err)
                                                    {

                                                    }
                                                }
                                                MontoRentabilidad = basecomisionable * Rentabilidad;
                                                // YA IGUALAMOS LA MOEDA DEL COSTO CON LA MONEDA DE LAPREFACTURA


                                                decimal comisionpesos = 0;
                                                decimal tc = 0;
                                                try
                                                {
                                                    tc = Convert.ToDecimal(cambio.TC);
                                                }
                                                catch (Exception err)
                                                {
                                                    tc = 0;
                                                }

                                                try
                                                {
                                                    comisionpesos = MontoComision * tc;
                                                }
                                                catch (Exception err)
                                                {

                                                }

                                                string insert = "";

                                                if (escomisionableart)
                                                {
                                                    insert = "insert into ComisionVendedor (NoPedido, FechaFac, FechaPed, IDVendedor, IDCliente, Serie, NoFactura, Subtotal, Rentabilidad, Penalizaciones,Comision, Tipo, Pagada,Costo,MonedaFactura, MonedaCosto, MontoRentabilidad, MontoComision, anioCom,mesCom, BaseComisionable, TC, ComisionPesos) " +
                                                        "values(" + IDPedido + ", '" + fac.Fecha + "', '" + FechaPedido + "', " + clientes.IDVendedor + ", " + clientes.IDCliente + ", '" + fac.Serie + "', " + fac.Numero + ", " + prefactura.Importe + ", " + Rentabilidad + ", " + montoPenalizacion + ", " + Comision + ", " + Tipo + ", '0', " + Costo + ", '" + MonedaFactura + "', '" + MonedaCosto + "', " + MontoRentabilidad + ", " + MontoComision + "," + ANIO + "," + IDMes + "," + basecomisionablereal + "," + tc + "," + comisionpesos + ")";

                                                    try
                                                    {
                                                        db2.Database.ExecuteSqlCommand(insert);


                                                    }
                                                    catch (Exception err)
                                                    {
                                                        string men = err.Message;
                                                    }
                                                }


                                                ////
                                                ///

                                                Sheet.Cells[string.Format("A{0}", row)].Value ="";
                                                //Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                                                Sheet.Cells[string.Format("B{0}", row)].Value = vendedores.Nombre;
                                                Sheet.Cells[string.Format("C{0}", row)].Value = clientes.Nombre;
                                                Sheet.Cells[string.Format("D{0}", row)].Value = condiciones.DiasCredito;


                                                ////
                                                ///
                                                Sheet.Cells[string.Format("E{0}", row)].Value = IDPedido;
                                                Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                                                Sheet.Cells[string.Format("F{0}", row)].Value = FechaPedido;
                                            c_Meses c_MesesPed = new c_MesesContext().c_Meses.Find(FechaPedido.Month);
                                                Sheet.Cells[string.Format("G{0}", row)].Value = c_MesesPed.Mes;
                                                //Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                                                Sheet.Cells[string.Format("H{0}", row)].Value = fac.Serie+ " "+fac.Numero;
                                                Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                                                Sheet.Cells[string.Format("I{0}", row)].Value = fac.Fecha;

                                            /////
                                            ///


                                            c_Meses c_MesesFac = new c_MesesContext().c_Meses.Find(fac.Fecha.Month);
                                            Sheet.Cells[string.Format("J{0}", row)].Value = c_MesesFac.Mes;
                                            Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                                            Sheet.Cells[string.Format("K{0}", row)].Value = Math.Round(prefactura.Importe, 4);

                                            Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                                            Sheet.Cells[string.Format("L{0}", row)].Value = Math.Round(Costo, 4);
                                         
                                            Sheet.Cells[string.Format("M{0}", row)].Value = tc;

                                            
                                                Sheet.Cells[string.Format("N{0}", row)].Value =MonedaFactura;

                                                Sheet.Cells[string.Format("O{0}", row)].Value = MonedaCosto;
                                            Sheet.Cells[string.Format("P{0}", row)].Value = ANIO.ToString();
                                            Sheet.Cells[string.Format("Q{0}", row)].Value = mes.Mes;
                                            if (Rentabilidad < 0)
                                            {
                                                colFromHex = System.Drawing.ColorTranslator.FromHtml("#E74C3C");
                                                Sheet.Cells[string.Format("R{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                                Sheet.Cells[string.Format("R{0}", row)].Style.Fill.BackgroundColor.SetColor(colFromHex);

                                            }
                                            Sheet.Cells[string.Format("R{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                                            Sheet.Cells[string.Format("R{0}", row)].Value = Math.Round(MontoRentabilidad, 4);
                                          
                                                Sheet.Cells[string.Format("S{0}", row)].Value = Math.Round(Rentabilidad, 4);

                                                //Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "#. ##%";
                                               
                                                Sheet.Cells[string.Format("T{0}", row)].Value = Math.Round(porcenComi, 4);

                                                //Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "#. ##%";

                                                Sheet.Cells[string.Format("U{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                                                Sheet.Cells[string.Format("U{0}", row)].Value = Math.Round(MontoComision, 4);

                                               
                                                Sheet.Cells[string.Format("V{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                                                Sheet.Cells[string.Format("V{0}", row)].Value = Math.Round(comisionpesos, 4);
                                               
                                                row++;

                                            }


                                        }
                                        catch (Exception er)
                                        {

                                        }
                                    }
                                    catch (Exception err)
                                    {

                                    }


                                }


                            

                        }
                        Sheet.Cells["A:AZ"].AutoFitColumns();
                    }




                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment: filename=" + "ReportecierreComision.xlsx");
                    Response.BinaryWrite(Ep.GetAsByteArray());
                    Response.End();

                }

            }
            catch (Exception err)
            {

            }
            return View();


        }

        public DateTime r_FechaPedido(int IDPedido)//Rastrear la fecha del pedido
        {
            //DateTime fechaPedido = DateTime.Now;
            //string cadena0 = "select Fecha as Dato from EncPedido where IDPedido = "+IDPedido+"";
            //ClsDatoString consulta = db.Database.SqlQuery<ClsDatoString>(cadena0).ToList().FirstOrDefault();
            EncPedido pedido = new PedidoContext().EncPedidos.Find(IDPedido);


            DateTime fechaPedido = pedido.Fecha;

            return fechaPedido;
        }

        public string r_Pedidos(int IDPrefactura)//Rastrear pedidos
        {
            string pedidos = string.Empty;
            try
            {
                if (IDPrefactura < 751)
                {
                    List<DetPrefactura> documentosEncPre = new PrefacturaContext().DetPrefactura.Where(s => s.IDPrefactura == IDPrefactura).ToList();
                    Hashtable documentoshash = new Hashtable();
                    foreach (DetPrefactura elemento in documentosEncPre)
                    {
                        string llave = elemento.Proviene + elemento.IDExterna;
                        if (!documentoshash.ContainsKey(llave))
                        { documentoshash.Add(llave, elemento); }
                    }
                    foreach (DictionaryEntry de in documentoshash)
                    {
                        DetPrefactura elementop = (DetPrefactura)documentoshash[de.Key];
                        if (elementop.Proviene == "Pedido")
                        {
                            pedidos += elementop.IDExterna + " ";
                        }
                        if (elementop.Proviene == "Remision")
                        {
                            ClsRastreaDA rastrea = new ClsRastreaDA();
                            List<NodoTrazo> nodos = rastrea.getDocumentoAnterior(elementop.Proviene, elementop.IDExterna, "Encabezado");
                            foreach (NodoTrazo nodo in nodos)
                            {
                                pedidos += nodo.ID + " ";
                            }
                        }
                    }
                    pedidos = pedidos.Trim();
                }
                if (IDPrefactura >= 751)
                {
                    List<elementosprefactura> documentos = new PrefacturaContext().elementosprefacturas.Where(s => s.idprefactura == IDPrefactura).ToList();
                    Hashtable documentoshash = new Hashtable();
                    foreach (elementosprefactura elemento in documentos)
                    {
                        string llave = elemento.documento + elemento.iddocumento;
                        if (!documentoshash.ContainsKey(llave))
                        { documentoshash.Add(llave, elemento); }
                    }
                    foreach (DictionaryEntry de in documentoshash)
                    {
                        elementosprefactura elementop = (elementosprefactura)documentoshash[de.Key];
                        if (elementop.documento == "Pedido")
                        {
                            pedidos += elementop.iddocumento + " ";
                        }
                        if (elementop.documento == "Remision")
                        {
                            ClsRastreaDA rastrea = new ClsRastreaDA();
                            List<NodoTrazo> nodos = rastrea.getDocumentoAnterior(elementop.documento, elementop.iddocumento, "Encabezado");
                            foreach (NodoTrazo nodo in nodos)
                            {
                                pedidos += nodo.ID + " ";
                            }
                        }

                    }
                    pedidos = pedidos.Trim();
                }

            }
            catch (Exception err)
            {

            }
            pedidos = pedidos.Trim();

            return pedidos;
        }

        //public decimal c_Rentabilidad(int IDPrefactura)
        //{
        //    decimal rentabilidad = 0;

        //    string cadena0 = "select sum(costo) as Dato from detPrefactura where IDPrefactura ="+IDPrefactura+"";
        //    ClsDatoDecimal costo = db.Database.SqlQuery<ClsDatoDecimal>(cadena0).ToList().FirstOrDefault();
        //    string cadena1 = "select sum(costo) as Dato from detPrefactura where IDPrefactura =" + IDPrefactura + "";
        //    ClsDatoDecimal subtotal = db.Database.SqlQuery<ClsDatoDecimal>(cadena1).ToList().FirstOrDefault();
        //    rentabilidad = 1 - (costo.Dato /subtotal.Dato);

        //    return rentabilidad;
        //}

        public decimal C_Penalizaciones()
        {
            decimal penalizacion = 0;

            return penalizacion;
        }

        public decimal c_Comision(int IDVendedor, int IDMes, int Ano)
        {
            decimal comision = 0;
            string cadena = "select Comision as Dato from CierreVentas where IDVendedor = " + IDVendedor + " and IDMes = " + IDMes + " and Ano = " + Ano + "";
            ClsDatoDecimal comi = db.Database.SqlQuery<ClsDatoDecimal>(cadena).ToList().FirstOrDefault();

            comision = comi.Dato;

            return comision;
        }

        public string r_Tipo()
        {
            string tipo = "";

            return tipo;
        }

        public bool r_Pagada()
        {
            bool pagada = false;

            return pagada;
        }

        public int comisionablearticulo(EncPrefactura prefactura)
        {
            
            var articulos = new PrefacturaContext().DetPrefactura.Where(s => s.IDPrefactura == prefactura.IDPrefactura);
            int es = 0;
            foreach (DetPrefactura detalle in articulos)
            {
                if (r_escomisionale(detalle.IDArticulo))
                {
                    es = 1;
                    es++;
                }
              
              
            }

            return es;
        }

        //public decimal c_Costo(EncPrefactura prefactura, int idpedido)
        //{
        //    string Monedaprefactura = prefactura.c_Moneda.ClaveMoneda; /// USD MXN 
        //    var articulos = new PrefacturaContext().DetPrefactura.Where(s => s.IDPrefactura == prefactura.IDPrefactura);
        //    decimal acumuladorcosto = 0;
        //    foreach (DetPrefactura detalle in articulos)
        //    {
        //        if (r_escomisionale(detalle.IDArticulo))
        //        {
        //            decimal costoxarti = 0;
        //            try
        //            {

        //                ClsDatoDecimal can = db.Database.SqlQuery<ClsDatoDecimal>("select cantidad as Dato from DetPedido where idarticulo= " + detalle.IDArticulo + " and idpedido=" + idpedido).ToList().FirstOrDefault();
        //                ClsDatoDecimal costoArt = db.Database.SqlQuery<ClsDatoDecimal>("select costo as Dato from DetPedido where idarticulo= " + detalle.IDArticulo + " and idpedido=" + idpedido).ToList().FirstOrDefault();






        //                string cadenacostoxarticulo = "select dbo.getcosto(0," + detalle.IDArticulo + "," + can.Dato + ") as Dato";
        //                costoxarti = db2.Database.SqlQuery<ClsDatoDecimal>(cadenacostoxarticulo).FirstOrDefault().Dato;

        //                Articulo Ar = db.Database.SqlQuery<Articulo>("select * from articulo where idarticulo= " + detalle.IDArticulo).ToList().FirstOrDefault();
        //                string Monedaarticulo = "USD";

        //                //if (Ar.IDFamilia == 10 || Ar.IDFamilia == 27 || Ar.IDFamilia == 19 || Ar.IDFamilia == 63)
        //                //{
        //                //    Monedaarticulo = "USD";
        //                //}
        //                //else
        //                //{
        //                    c_MonedaContext C = new c_MonedaContext();
        //                    Monedaarticulo = C.c_Monedas.Find(Ar.IDMoneda).ClaveMoneda;
        //                //}













        //                if (costoxarti == 0)
        //                {
        //                    try
        //                    {
        //                        DetOrdenCompra mc = new OrdenCompraContext().DetOrdenCompras.Where(s => s.IDArticulo == detalle.IDArticulo).OrderByDescending(s => s.IDDetOrdenCompra).FirstOrDefault();
        //                        costoxarti = mc.Costo;
        //                        C = new c_MonedaContext();
        //                        Monedaarticulo = C.c_Monedas.Find(mc.OrdenCompra.IDMoneda).ClaveMoneda;
        //                    }
        //                    catch (Exception err)
        //                    {
        //                        costoxarti = 0;
        //                    }
        //                }

        //                if (costoxarti == 0)  // sino tiene costo en ordendecompra va a lamatriz precioproveedor
        //                {
        //                    try
        //                    {
        //                        MatrizPrecioProv mc = new MatrizPrecioProvContext().MatrizPP.Where(s => s.IDArticulo == detalle.IDArticulo).FirstOrDefault();
        //                        costoxarti = mc.Precio;
        //                         C = new c_MonedaContext();
        //                        Monedaarticulo = C.c_Monedas.Find(mc.IDMoneda).ClaveMoneda;
        //                    }
        //                    catch (Exception err)
        //                    {
        //                        costoxarti = 0;
        //                    }
        //                }







        //                if (costoxarti == 0)  // sino no tiene las anteriores va a la matriz de costo
        //                {
        //                    MatrizCosto mc = new ArticuloContext().MatrizCostos.Where(s => s.IDArticulo == detalle.IDArticulo).FirstOrDefault();
        //                    costoxarti = mc.Precio;
        //                     C = new c_MonedaContext();
        //                    Monedaarticulo = C.c_Monedas.Find(Monedaarticulo).ClaveMoneda;

        //                }

        //                if (costoxarti == 0)  // sino no tiene las anteriores va a la matriz de costo
        //                {
        //                    int iddetpedido = 0;

        //                    if (detalle.Proviene == "Pedido")
        //                    {
        //                        DetPedido detallepedido = new PedidoContext().DetPedido.Find(detalle.IDDetExterna);
        //                        iddetpedido = detallepedido.IDDetPedido;


        //                    }

        //                    if (detalle.Proviene == "Remision")
        //                    {
        //                        DetRemision detalleremision = new RemisionContext().DetRemisiones.Find(detalle.IDDetExterna);

        //                        iddetpedido = detalleremision.IDExterna;
        //                    }


        //                    int idordenproduccion = 0;
        //                    decimal cantidadproduccida = 0;

        //                    try
        //                    {
        //                        SIAAPI.Models.Produccion.OrdenProduccion orden = db.Database.SqlQuery
        //                                <SIAAPI.Models.Produccion.OrdenProduccion>("select  * from OrdenProduccion where IDDetPedido=" + iddetpedido + " and EstadoOrden<>'Cancelada' ").ToList().FirstOrDefault();

        //                        cantidadproduccida = orden.Cantidad;

        //                        ClsDatoDecimal costo = new ArticuloContext().Database.SqlQuery<ClsDatoDecimal>("select sum(costoplaneado) as Dato from articuloproduccion where idorden =" + orden.IDOrden).FirstOrDefault();
        //                        try
        //                        {
        //                            costoxarti = costo.Dato / orden.Cantidad;
        //                        }
        //                        catch (Exception err)
        //                        {

        //                        }

        //                    }
        //                    catch (Exception err)
        //                    {
        //                        string mensajeerr = err.Message;
        //                        idordenproduccion = 0;
        //                        cantidadproduccida = 0;
        //                        costoxarti = 0;

        //                    }

        //                }

        //                string cadenaaa = "select [dbo].[GetTipocambioCadena]('" + prefactura.Fecha.Year + "-" + prefactura.Fecha.Month + "-" + prefactura.Fecha.Day + "','" + Monedaarticulo + "','" + Monedaprefactura + "') as Dato";
        //                decimal tipocambio = db2.Database.SqlQuery<ClsDatoDecimal>(cadenaaa).FirstOrDefault().Dato;

        //                acumuladorcosto += ((costoxarti * tipocambio) * detalle.Cantidad);



        //            }

        //            catch (Exception err)
        //            {

        //            }

        //        }

        //    }

        //    return acumuladorcosto;
        //}

        public decimal c_Costo(int idcaracteristica, int idarticulo, decimal cantidad, int IDFactura, int IDPrefactura)
        {
             string Monedaarticulo = "USD";
            string Monedaprefactura = "";
            decimal costoxarti = 0;
            decimal acumuladorcosto = 0;
            Articulo articulo = new ArticuloContext().Articulo.Find(idarticulo);
            Encfacturas encfacturas = new EncfacturasSaldosContext().Database.SqlQuery<Encfacturas>("select*from encfacturas where id="+IDFactura).ToList().FirstOrDefault();
            EncPrefactura prefactura = new PrefacturaContext().EncPrefactura.Find(IDPrefactura);
                    c_MonedaContext C = new c_MonedaContext();
                    Monedaarticulo = C.c_Monedas.Find(articulo.IDMoneda).ClaveMoneda;
            Monedaprefactura= C.c_Monedas.Find(prefactura.IDMoneda).ClaveMoneda;
            try
                    {
                        
                        Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("Select * from Caracteristica where ID=" + idcaracteristica).ToList().FirstOrDefault();

                        ClsCotizador elemento;
                        if (cara.IDCotizacion!=0)
                        {
                            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(cara.IDCotizacion);
                            XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
                            try
                            {
                                XmlDocument documento = new XmlDocument();
                                string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                                documento.Load(nombredearchivo);


                                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                                {
                                    // Call the Deserialize method to restore the object's state.
                                    elemento = (ClsCotizador)serializerX.Deserialize(reader);
                                }
                            }
                            catch (Exception er)
                            {
                                string mensajedeerror = er.Message;
                                elemento = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
                            }

                            decimal rango0 = 0.0M;
                            decimal rango1 = elemento.Rango1 + 0.01M;
                            decimal rango2 = elemento.Rango2 + 0.01M;
                            decimal rango3 = elemento.Rango3 + 0.01M;
                            decimal rango4 = elemento.Rango4 + 0.01M;

                            if (cantidad >= rango0 && cantidad <= elemento.Rango1)
                            {
                                costoxarti = elemento.Costo1;
                            }
                            if (cantidad >= rango1 && cantidad <= elemento.Rango2)
                            {
                                costoxarti = elemento.Costo2;
                            }
                            if (cantidad >= rango2 && cantidad <= elemento.Rango3)
                            {
                                costoxarti = elemento.Costo3;
                            }
                            if (cantidad >= rango3 && cantidad <= elemento.Rango4)
                            {
                                costoxarti = elemento.Costo4;
                            }
                            if (cantidad > elemento.Rango4)
                            {
                                costoxarti = elemento.Costo4;
                            }
                            string cadenaaa = "select [dbo].[GetTipocambioCadena]('" + encfacturas.Fecha.Year + "-" + encfacturas.Fecha.Month + "-" + encfacturas.Fecha.Day + "','USD','" + Monedaprefactura + "') as Dato";
                            decimal tipocambio = db2.Database.SqlQuery<ClsDatoDecimal>(cadenaaa).FirstOrDefault().Dato;

                            acumuladorcosto += ((costoxarti * tipocambio) * cantidad);
                        }
                        else
                        {
                            
                            try
                            {

                                
                                string cadenacostoxarticulo = "select dbo.getcosto(0," + idarticulo + "," + cantidad + ") as Dato";
                                costoxarti = db2.Database.SqlQuery<ClsDatoDecimal>(cadenacostoxarticulo).FirstOrDefault().Dato;




                        if (costoxarti == 0)
                        {
                            try
                            {
                                 //costoxarti = mc.Costo;
                                DetOrdenCompra mc = new ArticuloContext().Database.SqlQuery<DetOrdenCompra>("Select * from DetOrdenCompra where IDArticulo=" + idarticulo + " and caracteristica_id=" + idcaracteristica + " order by IDDetordencompra desc").ToList().FirstOrDefault();
                                EncOrdenCompra orden = new OrdenCompraContext().EncOrdenCompras.Find(mc.IDOrdenCompra);
                                costoxarti = mc.Costo;
                                C = new c_MonedaContext();
                                Monedaarticulo = C.c_Monedas.Find(orden.IDMoneda).ClaveMoneda;
                            }
                            catch (Exception err)
                            {
                                costoxarti = 0;
                            }
                        }

                        if (costoxarti == 0)  // sino tiene costo en ordendecompra va a lamatriz precioproveedor si es materia prima
                        { 
                                    try
                                    {
                                        MatrizPrecioProv mc = new MatrizPrecioProvContext().MatrizPP.Where(s => s.IDArticulo == idarticulo).FirstOrDefault();
                                        costoxarti = mc.Precio;
                                        C = new c_MonedaContext();
                                        Monedaarticulo = C.c_Monedas.Find(mc.IDMoneda).ClaveMoneda;
                                    }
                                    catch (Exception err)
                                    {
                                        costoxarti = 0;
                                    }
                                }

                            }

                            catch (Exception err)
                            {

                            }
                    /// el articulo viene en mxn y la prefactura tambien
                    /// porque se hace el gettipocambiocadena
                            string cadenaaa = "select [dbo].[GetTipocambioCadena]('" + encfacturas.Fecha.Year + "-" + encfacturas.Fecha.Month + "-" + encfacturas.Fecha.Day + "','"+Monedaarticulo+"','" + Monedaprefactura + "') as Dato";
                            decimal tipocambio = db2.Database.SqlQuery<ClsDatoDecimal>(cadenaaa).FirstOrDefault().Dato;
                    if (acumuladorcosto==0) 
                    {
                        acumuladorcosto += ((costoxarti * tipocambio) * cantidad);
                    }
                            
                    }

                    }

                    catch (Exception err)
                    {

                    }
            
            return acumuladorcosto;
        }

        public decimal c_basecomisionable(int iddetPrefactura)
        {
            var articulos = new PrefacturaContext().DetPrefactura.Where(s => s.IDDetPrefactura == iddetPrefactura);
            decimal acumuladorcosto = 0;
            foreach (DetPrefactura detalle in articulos)
            {
                if (r_escomisionale(detalle.IDArticulo))
                {

                 acumuladorcosto += (detalle.Importe);
                }

            }

            ///AQUI VA CODIGO PARA QUITAR DE LA BASE COMISIONABLE EL MONTO DE LA MULTA DE UNA FACTURA 

            return acumuladorcosto;
        }

        public string r_MonedaFactura()
        {
            string monedafactura = "";

            return monedafactura;
        }

        public string r_MonedaCosto()
        {
            string monedacosto = "";

            return monedacosto;
        }

        public decimal c_MontoRentabilidad()
        {
            decimal montorentabilidad = 0;

            return montorentabilidad;
        }

        public decimal c_MontoComision()
        {
            decimal montocomision = 0;

            return montocomision;
        }

        public bool r_escomisionale(int IDArticuloComp)//Rastrear para saber si es comisionable
        {
            bool comisionable = true;

            //DetPrefactura NO comisionable
            //string cadena0 = "select count(*) from DetPrefactura inner join ArticuloNOC on DetPrefactura.IDArticulo=ArticuloNOC.IDArticulo where ArticuloNOC.IDArticulo = " + IDArticuloComp + "";
            //ClsDatoEntero DetPreF_NOC = db.Database.SqlQuery<ClsDatoEntero>(cadena0).ToList().FirstOrDefault();
            //Articulo NO comisionable
            string cadena1 = "select a.* from Articulo as A inner join ArticuloNOC as ANC on ANC.IDArticulo = A.IDArticulo where ANC.IDArticulo = " + IDArticuloComp + "";
            Articulo Articulo_NOC = db.Database.SqlQuery<Articulo>(cadena1).ToList().FirstOrDefault();
            try
            {
                if (Articulo_NOC != null) //Si es diferente de 0 el articulo o el detprefactura es NO comisionable
                {
                    comisionable = false;
                    return comisionable;
                }


                try
                {
                    string cadenaa = "select a.* from Articulo as A inner join Familia as f on f.idfamilia = A.idfamilia where a.IDArticulo = " + IDArticuloComp + " and  (f.descripcion like 'suaje%' or f.descripcion like 'pleca%')";
                    Articulo fnoComisi = db.Database.SqlQuery<Articulo>(cadenaa).ToList().FirstOrDefault();

                    if (fnoComisi!=null)
                    {
                        comisionable = false;
                        return comisionable;
                    }

                }
                catch (Exception err)
                {

                }
                try
                {
                    Articulo arcinta = new ArticuloContext().Articulo.Find(IDArticuloComp);
                    if (arcinta.IDTipoArticulo==6)
                    {
                        comisionable = false;
                        return comisionable;
                    }
                    if (arcinta.IDTipoArticulo == 7)
                    {
                        comisionable = false;
                        return comisionable;
                    }
                    if (arcinta.IDTipoArticulo == 2)
                    {
                        comisionable = false;
                        return comisionable;
                    }
                }
                catch (Exception err)
                {

                }
            }
            catch (Exception err)
            {
                comisionable = false;
            }

            return comisionable;
        }
        public int r_IDVendedor(int IDPedido)//Rastrear ID de Vendedor
        {
            int IDVendedor = 0;

            List<EncPedido> listEncPedido = new PedidoContext().EncPedidos.Where(s => s.IDPedido == IDPedido).ToList();

            foreach (EncPedido elemento in listEncPedido)
            {
                IDVendedor = elemento.IDVendedor;
            }

            return IDVendedor;
        }

        public int c_NumDePedidos(int IDVendedor, int IDMes, int Ano)//Calcular el total de pedidos de un vendedor
        {
            int numpedidos = 0;
            //List<EncPedido> listEncPedido = new PedidoContext().EncPedidos.Where(s => s.IDVendedor == IDVendedor).ToList();
            string cadena0 = "select Count(*) as Dato from EncPedido where IDVendedor = " + IDVendedor + " and YEAR(Fecha) = " + Ano + " and  MONTH(Fecha) = " + IDMes + " AND STATUS <>'CANCELADO'";
            ClsDatoEntero consulta = db.Database.SqlQuery<ClsDatoEntero>(cadena0).ToList().FirstOrDefault();

            numpedidos = consulta.Dato;

            return numpedidos;
        }

        public decimal c_VentasMXN(int IDVendedor, int IDMes, int Ano)//Calcular el total de Ventas en MXN (180)
        {
            decimal totalMXN = 0;
            try
            {

                //List<EncPedido> listEncPedido = new PedidoContext().EncPedidos.Where(s => s.IDPedido == IDPedido).ToList();
                string cadena = "select SUM(Subtotal) as Dato from EncPedido where IDVendedor = " + IDVendedor + " and YEAR(Fecha) = " + Ano + " and MONTH(Fecha) = " + IDMes + " and IDMoneda = 180 AND STATUS <>'CANCELADO'";
                ClsDatoDecimal consulta = db.Database.SqlQuery<ClsDatoDecimal>(cadena).ToList().FirstOrDefault();

                totalMXN = consulta.Dato;

                return totalMXN;
            }
            catch (Exception err)
            {

            }
            return totalMXN;
        }

        public decimal c_VentasUSD(int IDVendedor, int IDMes, int Ano)//Calcular el total de Ventas en USD (181)
        {
            decimal totalUSD = 0;
            try
            {
                //List<EncPedido> listEncPedido = new PedidoContext().EncPedidos.Where(s => s.IDPedido == IDPedido).ToList();
                string cadena = "select SUM(Subtotal) as Dato from EncPedido where IDVendedor = " + IDVendedor + " and YEAR(Fecha) = " + Ano + " and  MONTH(Fecha) = " + IDMes + " and IDMoneda = 181 AND  STATUS<>'CANCELADO'";
                ClsDatoDecimal consulta = db.Database.SqlQuery<ClsDatoDecimal>(cadena).ToList().FirstOrDefault();

                totalUSD = consulta.Dato;

                return totalUSD;
            }
            catch (Exception err)
            {

            }
            return totalUSD;

        }

        public decimal c_VentasUSDaMXN(int IDVendedor, int IDMes, int Ano)//Calcular el total de Ventas de USD a MXN
        {
            decimal totalUSDaMXN = 0;
            try
            {

                //List<EncPedido> listEncPedido = new PedidoContext().EncPedidos.Where(s => s.IDPedido == IDPedido).ToList();
                string cadena0 = "select Sum(Subtotal * TipoCambio) from EncPedido where IDVendedor = " + IDVendedor + " and YEAR(Fecha) = " + Ano + " and  MONTH(Fecha) = " + IDMes + " and IDMoneda = 181 and status<>'cancelado'";
                ClsDatoDecimal consulta = db.Database.SqlQuery<ClsDatoDecimal>(cadena0).ToList().FirstOrDefault();

                totalUSDaMXN = consulta.Dato;

                return totalUSDaMXN;
            }
            catch (Exception err)
            {

            }
            return totalUSDaMXN;

        }

        public decimal c_VentasMXNaUSD(int IDVendedor, int IDMes, int Ano)//Calcular el total de Ventas en USD (181)
        {
            decimal totalMXNaUSD = 0;
            try
            {

                //List<EncPedido> listEncPedido = new PedidoContext().EncPedidos.Where(s => s.IDPedido == IDPedido).ToList();
                string consulta0 = "select Sum(round(Subtotal / [dbo].GetTipocambio(fecha,181,180),2)) as Dato from EncPedido where IDVendedor = " + IDVendedor + " and YEAR(Fecha) = " + Ano + " and  MONTH(Fecha) = " + IDMes + " and IDMoneda = 180 and status<>'cancelado'";
                ClsDatoDecimal consulta = db.Database.SqlQuery<ClsDatoDecimal>(consulta0).ToList().FirstOrDefault();

                totalMXNaUSD = consulta.Dato;

                return totalMXNaUSD;
            }
            catch (Exception err)
            {

            }

            return totalMXNaUSD;

        }

        public decimal r_Cuota(int IDVendedor, int IDMes, int Ano)//Rastrear Cuota
        {
            decimal cuota = 0;
            try
            {

                //List<Vendedor> listEncPedido = new VendedorContext().Vendedores.Where(s => s.IDPedido == IDPedido).ToList();
                string cadena0 = "select count(*) as Dato from Vendedor where IDVendedor = " + IDVendedor + " and IDTipoCuota = 2";
                ClsDatoEntero consulta0 = db.Database.SqlQuery<ClsDatoEntero>(cadena0).ToList().FirstOrDefault();


                if (consulta0.Dato != 0) //diferente de 0 SI hay vendedores con cuota mensual
                {
                    //Cuota mensual del mes y año especifico cuando el vendedor tiene cuota mensual
                    string cadena1Cuota = "select CM.Cuota as Dato from Vendedor as V inner join CuotaMensual as CM on V.IDVendedor = CM.IDVendedor where V.IDVendedor = " + IDVendedor + " and CM.Ano = " + Ano + " and CM.IDMes = " + IDMes + "";
                    ClsDatoDecimal consulta1 = db.Database.SqlQuery<ClsDatoDecimal>(cadena1Cuota).ToList().FirstOrDefault();
                    cuota = consulta1.Dato;
                }
                else
                {
                    //Cuota cuando el vendedor no tiene cuota mensual
                    string cadena2Cuota = "select CuotaVendedor as Dato from Vendedor where IDVendedor = " + IDVendedor + "  and IDTipoCuota = 1";
                    ClsDatoDecimal consulta2 = db.Database.SqlQuery<ClsDatoDecimal>(cadena2Cuota).ToList().FirstOrDefault();
                    cuota = consulta2.Dato;
                }


                return cuota;
            }
            catch (Exception err)
            {

            }
            return cuota;

        }

        public decimal c_CuotaAlcanzada(int IDVendedor, int IDMes, int Ano)//Calcular el total de cuota alzanzada
        {
            decimal cuotaAlcanzada = 0;
            try
            {
                Vendedor vendedor = new VendedorContext().Vendedores.Find(IDVendedor);

                string Monedacadena = "MXN";
                if (vendedor.IDTipoCuota == 2)
                {
                    //Moneda desde CuotaMensual
                    string cadenaMonMens = "select M.ClaveMoneda as Dato from Vendedor as V inner join CuotaMensual as CM on V.IDVendedor = CM.IDVendedor inner join c_Moneda as M on M.IDMoneda = Cm.IDMoneda where V.IDVendedor = " + IDVendedor + " and CM.Ano = " + Ano + " and CM.IDMes = " + IDMes + "";
                    ClsDatoString consMonMens = db.Database.SqlQuery<ClsDatoString>(cadenaMonMens).ToList().FirstOrDefault();
                    Monedacadena = consMonMens.Dato;
                }

                if (vendedor.IDTipoCuota == 1)
                {
                    string cadenaMonFij = "select M.ClaveMoneda as Dato from Vendedor as V inner join c_Moneda as M on M.IDMoneda = V.IDMoneda where V.IDVendedor = " + IDVendedor + "";
                    ClsDatoString consMonFij = db.Database.SqlQuery<ClsDatoString>(cadenaMonFij).ToList().FirstOrDefault();

                    Monedacadena = consMonFij.Dato;
                }



                decimal dato1 = 0M;
                decimal dato2 = 0M;



                if (Monedacadena == "MXN")
                {
                    dato1 = c_VentasMXN(IDVendedor, IDMes, Ano);
                    dato2 = c_VentasUSDaMXN(IDVendedor, IDMes, Ano);
                }
                if (Monedacadena == "USD")
                {
                    dato1 = c_VentasUSD(IDVendedor, IDMes, Ano);
                    dato2 = c_VentasMXNaUSD(IDVendedor, IDMes, Ano);
                }



                cuotaAlcanzada = dato1 + dato2;
                return cuotaAlcanzada;
            }
            catch (Exception err)
            {

            }
            return cuotaAlcanzada;

        }

        public int r_MonedaCA(int IDVendedor, int IDMes, int Ano)//Calcular el total de cuota alzanzada
        {
            int MonedaCA = 180;
            try
            {
                Vendedor vendedor = new VendedorContext().Vendedores.Find(IDVendedor);

                int Monedacadena = 180;
                if (vendedor.IDTipoCuota == 2)
                {
                    //Moneda desde CuotaMensual
                    string cadenaMonMens = "select M.IDMoneda as Dato from Vendedor as V inner join CuotaMensual as CM on V.IDVendedor = CM.IDVendedor inner join c_Moneda as M on M.IDMoneda = Cm.IDMoneda where V.IDVendedor = " + IDVendedor + " and CM.Ano = " + Ano + " and CM.IDMes = " + IDMes + "";
                    ClsDatoEntero consMonMens = db.Database.SqlQuery<ClsDatoEntero>(cadenaMonMens).ToList().FirstOrDefault();
                    Monedacadena = consMonMens.Dato;
                }

                if (vendedor.IDTipoCuota == 1)
                {
                    string cadenaMonFij = "select M.IDMoneda as Dato from Vendedor as V inner join c_Moneda as M on M.IDMoneda = V.IDMoneda where V.IDVendedor = " + IDVendedor + "";
                    ClsDatoEntero consMonFij = db.Database.SqlQuery<ClsDatoEntero>(cadenaMonFij).ToList().FirstOrDefault();

                    Monedacadena = consMonFij.Dato;
                }

                MonedaCA = Monedacadena;
                return MonedaCA;
            }
            catch (Exception err)
            {

            }
            return MonedaCA;

        }

        public decimal r_Comision(int IDVendedor, int IDMes, int Ano)//Rastrear Comision
        {
            decimal comision = 0;
            try
            {

                //List<Vendedor> listEncPedido = new VendedorContext().Vendedores.Where(s => s.IDPedido == IDPedido).ToList();
                string cadena0 = "select count(*) as Dato from Vendedor where IDVendedor = " + IDVendedor + " and IDTipoCuota = 2";
                ClsDatoEntero consulta0 = db.Database.SqlQuery<ClsDatoEntero>(cadena0).ToList().FirstOrDefault();


                if (consulta0.Dato != 0) //diferente de 0 SI hay vendedores con comision mensual
                {
                    //Comision mensual del mes y año especifico cuando el vendedor tiene cuota mensual
                    string cadena1Comision = "select CM.PorcComision as Dato from Vendedor as V inner join CuotaMensual as CM on V.IDVendedor = CM.IDVendedor where V.IDVendedor = " + IDVendedor + " and CM.Ano = " + Ano + " and CM.IDMes = " + IDMes + "";
                    ClsDatoDecimal consulta1 = db.Database.SqlQuery<ClsDatoDecimal>(cadena1Comision).ToList().FirstOrDefault();
                    comision = consulta1.Dato;
                }
                else
                {
                    //Comision cuando el vendedor no tiene cuota mensual
                    string cadena2Comision = "select Comision as Dato from Vendedor where IDVendedor = " + IDVendedor + " and IDTipoCuota = 1";
                    ClsDatoDecimal consulta2 = db.Database.SqlQuery<ClsDatoDecimal>(cadena2Comision).ToList().FirstOrDefault();
                    comision = consulta2.Dato;
                }

                return comision;
            }
            catch (Exception err)
            {

            }
            return comision;

        }

        public ActionResult ReporteCierreVentas()
        {
            //CierreVentasContext db3 = new CierreVentasContext();
            //var cierresventas = db3.CierreVentas.ToList();

            string cadenaCV = "select * from CierreVentas";
            List<CierreVentas> cierresventas = db2.Database.SqlQuery<CierreVentas>(cadenaCV).ToList();

            Rprt_CierreVentas report = new Rprt_CierreVentas();
            report.ListaDatosBD_CierreVentas = cierresventas;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "application/pdf", "Reporte_Cierrre de ventas.pdf");
        }

        public ActionResult ReporteComisiones()
        {
            //CierreComisionesContext db4 = new CierreComisionesContext();
            //var cierrecomisiones = db4.CierreComisiones.ToList();

            string cadenaCC = "select * from ComisionVendedor";
            List<CierreComisiones> cierrecomisiones = db3.Database.SqlQuery<CierreComisiones>(cadenaCC).ToList();

            Rprt_Comisiones report = new Rprt_Comisiones();
            report.ListaDatosBD_CierreComisiones = cierrecomisiones;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "application/pdf", "Reporte_Cierrre de comisiones.pdf");
        }

        public List<SelectListItem> GetAnios()
        {


            List<SelectListItem> lista = new List<SelectListItem>();
            for (int i = DateTime.Now.Year - 1; i <= DateTime.Now.Year + 1; i++)
            {
                var countrytip = new SelectListItem()
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                };
                if (DateTime.Now.Year == i)
                { countrytip.Selected = true; }
                lista.Insert(0, countrytip);
            }

            return lista;
        }




        public ActionResult Penalizacionfactura(string Numero, string SerieFac, string ClieFac, string sortOrder, string currentFilter, int? page, int? PageSize, string Fechainicio, string Fechafinal, string FacPag, string Estado = "A")
        {
            EncfacturaContext db = new EncfacturaContext();
            EncfacturasSaldosContext dbs = new EncfacturasSaldosContext();

            //string ConsultaSql = "select * from Encfacturas ";
            string ConsultaSql = "Select top 100 * from dbo.EncfacturasSaldos";
            string cadenaSQl = string.Empty;
            try
            {

                //Buscar Facturas: Pagadas o no pagadas
                var FacPagLst = new List<SelectListItem>();


                var EstadoLst = new List<SelectListItem>();
                //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });

                EstadoLst.Add(new SelectListItem { Text = "Cancelada", Value = "C" });
                EstadoLst.Add(new SelectListItem { Text = "Activas", Value = "A" });

                ViewData["Estado"] = EstadoLst;

                ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");

                ViewBag.Estadoseleccionado = Estado;  /// mandar el viewbag el parametro que viene de la pagina anterior
                //Facturas Pagadas
                FacPagLst.Add(new SelectListItem { Text = "Pagada", Value = "SI" });
                FacPagLst.Add(new SelectListItem { Text = "NoPagada", Value = "NO" });

                ViewData["FacPag"] = FacPagLst;



                ViewBag.FacPag = new SelectList(FacPagLst, "Value", "Text");

                ViewBag.Facpagseleccionado = FacPag; /// mandar el viewbag el parametro que viene de la pagina anterior

                //Buscar Serie Factura
                var SerLst = new List<string>();
                var SerQry = from d in db.encfacturas
                             orderby d.Serie
                             select d.Serie;

                SerLst.AddRange(SerQry.Distinct());

                ViewBag.SerieFac = new SelectList(SerLst);

                ViewBag.SerieFacseleccionado = SerieFac; /// mandar el viewbag el parametro que viene de la pagina anterior

                ViewBag.sumatoria = "";

                
                ViewBag.ClieFac = new ClienteRepository().GetClientesFactura();

                ViewBag.ClieFacseleccionado = ClieFac;/// mandar el viewbag el parametro que viene de la pagina anterior


                //string ConsultaSqlResumen = "select Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from encfacturas ";
                string ConsultaSqlResumen = "select Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from EncfacturasSaldos ";
                string ConsultaAgrupado = "group by Moneda order by Moneda ";
                string Filtro = string.Empty;



                string Orden = " order by fecha desc , serie , numero desc ";

                ///tabla filtro: serie
                if (!String.IsNullOrEmpty(SerieFac))
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where Serie='" + SerieFac + "'";
                    }
                    else
                    {
                        Filtro += "and  Serie='" + SerieFac + "'";
                    }

                }

                if (String.IsNullOrEmpty(SerieFac))
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where Serie=''";
                    }
                    else
                    {
                        Filtro += "and  Serie=''";
                    }

                }
                //Buscar por numero
                if (!String.IsNullOrEmpty(Numero))
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where Numero=" + Numero + "";
                    }
                    else
                    {
                        Filtro += "and  Numero=" + Numero + "";
                    }

                }

                ///tabla filtro: Nombre Cliente
                if (!String.IsNullOrEmpty(ClieFac))
                {

                    if (Filtro == string.Empty)
                    {
                        Filtro = "where Nombre_cliente='" + ClieFac + "'";
                    }
                    else
                    {
                        Filtro += "and  Nombre_cliente='" + ClieFac + "'";
                    }

                }

                ///tabla filtro: Factura pagada/no pagada && Serie, Nombre Cliente
                if (FacPag != "Todas")
                {
                    if (FacPag == "SI")
                    {
                        if (Filtro == string.Empty)
                        {
                            Filtro = "where pagada='1' ";
                        }
                        else
                        {
                            Filtro += "and  pagada='1' ";
                        }
                    }
                    if (FacPag == "NO")
                    {
                        if (Filtro == string.Empty)
                        {
                            Filtro = "where pagada='0' ";
                        }
                        else
                        {
                            Filtro += "and  pagada='0' ";
                        }
                    }
                }


                if (Estado != "Todos")
                {
                    if (Estado == "C")
                    {
                        if (Filtro == string.Empty)
                        {
                            Filtro = "where Estado='C'";
                        }
                        else
                        {
                            Filtro += "and  Estado='C'";
                        }
                    }
                    if (Estado == "A")
                    {
                        if (Filtro == string.Empty)
                        {
                            Filtro = "where  Estado='A'";
                        }
                        else
                        {
                            Filtro += "and Estado='A'";
                        }
                    }
                }



                if (!String.IsNullOrEmpty(Fechainicio) && String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where  Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechainicio + " 23:59:59.999' ";
                    }
                    else
                    {
                        Filtro += " and Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechainicio + " 23:59:59.999'";
                    }
                }


                if (!String.IsNullOrEmpty(Fechainicio) && !String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where  Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999' ";
                    }
                    else
                    {
                        Filtro += " and Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999'";
                    }
                }


                ViewBag.CurrentSort = sortOrder;
                ViewBag.SerieSortParm = String.IsNullOrEmpty(sortOrder) ? "Serie" : "";
                ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "Numero" : "";
                ViewBag.FechaSortParm = sortOrder == "Fecha" ? "Fecha" : "";
                ViewBag.ClienteSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre_Cliente" : "";




                // Pass filtering string to view in order to maintain filtering when paging
                ViewBag.Fechainicio = Fechainicio;
                ViewBag.Fechafinal = Fechafinal;

                

                //Ordenacion

                switch (sortOrder)
                {
                    case "Activa":
                        Orden = " order by  Estado ";
                        break;
                    case "Serie":
                        Orden = " order by  serie , numero desc ";
                        break;
                    case "Numero":
                        Orden = " order by   numero asc ";
                        break;
                    case "Fecha":
                        Orden = " order by fecha ";
                        break;
                    case "Nombre_Cliente":
                        Orden = " order by  Nombre_cliente ";
                        break;
                    default:
                        Orden = " order by fecha desc , serie , numero desc ";
                        break;
                }


                // si no ha seleccionado nada muestra las facturas del ultimo mes 

                if (Filtro == " where  Estado='A'")
                {
                    Filtro += " and Fecha >='" + DateTime.Now.AddDays(-30).Year + "-" + DateTime.Now.AddDays(-30).Month + "-" + DateTime.Now.AddDays(-30).Day + "'  and  Fecha<'" + DateTime.Now.AddDays(1).Year + "-" + DateTime.Now.AddDays(1).Month + "-" + DateTime.Now.AddDays(1).Day + "' ";
                    ViewBag.Fechainicio = "" + DateTime.Now.AddDays(-30).Year + "-" + DateTime.Now.AddDays(-30).Month + "-" + DateTime.Now.AddDays(-30).Day + "";
                    ViewBag.Fechafinal = "" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "";
                }

                //var elementos = from s in db.encfacturas
                //select s;

                cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

                //var elementos = db.Database.SqlQuery<Encfacturas>(cadenaSQl).ToList();
                var elementos = dbs.Database.SqlQuery<EncfacturasSaldos>(cadenaSQl).ToList();


                ViewBag.sumatoria = "";
                try
                {

                    var SumaLst = new List<string>();
                    var SumaQry = ConsultaSqlResumen + " " + Filtro + " " + ConsultaAgrupado;
                    List<ResumenFac> data = db.Database.SqlQuery<ResumenFac>(SumaQry).ToList();
                    ViewBag.sumatoria = data;

                }
                catch (Exception err)
                {
                    string mensaje = err.Message;
                }

                //Paginación
                // DROPDOWNLIST FOR UPDATING PAGE SIZE
                //int count = db.Encfacturas.OrderBy(e => e.Serie).Count();// Total number of elements
                int count = elementos.Count();// Total number of elements
                // Populate DropDownList
                ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

                int pageNumber = (page ?? 1);
                int pageSize = (PageSize ?? 10);
                ViewBag.psize = pageSize;


                return View(elementos.ToPagedList(pageNumber, pageSize));
                //Paginación
            }
            catch (Exception err)
            {
                string mensaje = err.Message;

                var reshtml = Server.HtmlEncode(cadenaSQl);

                return Content(reshtml);
            }
        }


        public ActionResult PenalizarF(int idfactura, string serie)
        {
            Penalizacion_Factura periodo = new Penalizacion_Factura();
            //CierreComisiones periodo = new CierreComisiones();
            //VPagoComis periodo = new VPagoComis();
            periodo.IdFactura = idfactura;
            periodo.Mes = DateTime.Now.Month;
            periodo.Periodo = DateTime.Now.Year;
            ViewBag.IDMes = new SelectList(db2.c_MesesBD, "IDMes", "Mes", periodo.Mes);
            ViewBag.ANIO = GetAnios();
            ViewBag.serie = serie;
            //ViewBag.IDVendedor = new SelectList(db2.VendedorBD, "IDVendedor", "Nombre");

            return View(periodo);



        }

        // POST: SolicitudDiseno/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PenalizarF(FormCollection coleccion)
        {

           
            string IdFactura = coleccion.Get("IdFactura");
            string Motivo = coleccion.Get("Motivo");
            string Monto = coleccion.Get("Monto");
            string Mes = coleccion.Get("IDMes");
            string Periodo = coleccion.Get("Periodo");
            string serie= coleccion.Get("serie");

            int numero = Convert.ToInt16(IdFactura);
            string cadena0 = "Select id as Dato from encfacturas where numero=" + numero + " and serie='" + serie +"'";
            ClsDatoEntero id = db.Database.SqlQuery<ClsDatoEntero>(cadena0).ToList().FirstOrDefault();


            try
            {
                string cadena = "INSERT INTO [dbo].[PenalizacionFactura]([Idfactura],[Motivo],[Monto],[Mes],[Periodo])VALUES" +
                    "(" + id.Dato + ",'" + Motivo + "'," + Convert.ToDecimal(Monto) + "," + Mes + "," +Periodo + ");";
                var db = new Penalizacion_FacturaContext();
                db.Database.ExecuteSqlCommand(cadena);
                //db.SolicitudDiseno.Add(elemento);
                //db.SaveChanges();
                return RedirectToAction("ListaPenalizacionFacturas");
            }
            catch (Exception err)
            {
                int idf = id.Dato;
                return View( new { IdFactura = idf });
            }
            
        }
        /// <summary>
        /// ///////////////////////////////lista penalizaciones facturas
        /// </summary>
        /// <returns></returns>
        /// 

        public ActionResult ListaPenalizacionFacturas(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            
            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.searchString = searchString;
            Penalizacion_FacturaContext dbp= new Penalizacion_FacturaContext();
            //Paginación
            var elementos = from s in dbp.PFacturas
                            select s;

            //Busqueda
            if (searchString != null)
            {
                elementos = elementos.Where(s => s.IdFactura.Equals(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "IdFactura":
                    elementos = elementos.OrderBy(s => s.IdFactura);
                    break;
                //case "Municipio":
                //    elementos = elementos.OrderBy(s => s.Municipio);
                //    break;
                //case "Estado":
                //    elementos = elementos.OrderBy(s => s.Estados.Estado);
                //    break;
                //case "Grupo":
                //    elementos = elementos.OrderBy(s => s.c_Grupo.Descripcion);
                //    break;
                default:
                    elementos = elementos.OrderBy(s => s.IdFactura);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = dbp.PFacturas.OrderBy(e => e.IdFactura).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));

            //Paginación
        }

        //verifica p cliente

        public decimal VerificarPCliente(int IDCliente, decimal Monto, int idfactura)//Calcular el total de Ventas de USD a MXN
        {
            decimal saldo = 0;
            decimal penalizacion = 0;
            decimal aplicado = 0;
            try
            {
                string cadenamonto = "Select resta as Dato from PenalizacionCliente where idcliente=" + IDCliente + " and resta>0";
                ClsDatoDecimal montoC = db.Database.SqlQuery<ClsDatoDecimal>(cadenamonto).ToList().FirstOrDefault();

                string cadenaIDPenalizacionCliente = "Select IDPenalizacionCliente as Dato from PenalizacionCliente where idcliente=" + IDCliente + " and resta>0";
                ClsDatoEntero IDPenalizacionCliente = db.Database.SqlQuery<ClsDatoEntero>(cadenaIDPenalizacionCliente).ToList().FirstOrDefault();

                saldo = montoC.Dato;
                
                if(saldo < Monto)
                {
                    penalizacion = saldo;
                }
                if (saldo>= Monto)
                {
                    penalizacion = Monto;
                }

                string cadena2 = "INSERT INTO [dbo].[AplicacionPCliente]([IDPenalizacionCliente],[IdCliente], [Mes],[Periodo],[Monto],[Idfactura])VALUES" +
                    "(" + IDPenalizacionCliente.Dato + "," + IDCliente + "," + DateTime.Now.Month + "," + DateTime.Now.Year + "," + penalizacion + ","+idfactura+");";

                
              db.Database.ExecuteSqlCommand(cadena2);

                return penalizacion;
            }
            catch (Exception err)
            {

            }
            return penalizacion;

        }

        public ActionResult PenalizacionCliente()
        {
            ViewBag.ClieFac = new ClienteRepository().GetClientesFactura();

            return View();
        }

        // POST: SolicitudDiseno/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PenalizacionCliente(FormCollection coleccion)
        {
            string cliente = coleccion.Get("ClieFac");
            string Motivo = coleccion.Get("Motivo");
            string Monto = coleccion.Get("Monto");
            string Aplicado = coleccion.Get("Aplicado");
            //string Resta = coleccion.Get("Resta");
            
            try
            {
                var db = new Penalizacion_ClienteContext();
                string idcl = "select idcliente as Dato from Clientes where Nombre='" +cliente + "'";
                ClsDatoEntero idcliente = db.Database.SqlQuery<ClsDatoEntero>(idcl).ToList().FirstOrDefault();
                               

                string cadena = "INSERT INTO [dbo].[PenalizacionCliente]([IdCliente],[Cliente],[Motivo],[Monto],[Resta], fecha)VALUES" +
                    "(" + idcliente.Dato + ",'"+ cliente+"','" + Motivo + "'," + Convert.ToDecimal(Monto) + "," + Convert.ToDecimal(Monto) + ",'"+ DateTime.Now.Year+"-"+ DateTime.Now.Month +"-"+ DateTime.Now.Day  +"');";
               
                db.Database.ExecuteSqlCommand(cadena);

                //string cadena0 = "select max(idPenalizacionCliente) as Dato from PenalizacionCliente where idcliente=" + idcliente.Dato + "and Monto=" + Convert.ToDecimal(Monto);
                //ClsDatoEntero idpe = db.Database.SqlQuery<ClsDatoEntero>(cadena0).ToList().FirstOrDefault();

                ////decimal resta = Convert.ToDecimal(Monto) - Convert.ToDecimal(Aplicado);



                //string cadena2 = "INSERT INTO [dbo].[AplicacionPCliente]([IDPenalizacionCliente],[IdCliente], [Mes],[Periodo],[Monto],[Idfactura])VALUES" +
                //    "(" + idpe.Dato + "," + idcliente.Dato + "," + DateTime.Now.Month + "," + DateTime.Now.Year + "," + Convert.ToDecimal(Monto) + ", 0);";

                //db.Database.ExecuteSqlCommand(cadena2);

                return RedirectToAction("ListaPenalizacionClientes");
            }
            catch (Exception err)
            {
                ViewBag.ClieFac = new ClienteRepository().GetClientesFactura();

                return View();
            }
           
        }
        public ActionResult ListaPenalizacionClientes(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "IDCliente" : "IDCliente";
            

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.searchString = searchString;
            //Paginación
            Penalizacion_ClienteContext dbc = new Penalizacion_ClienteContext();
            var elementos = from s in dbc.PClientes
                            select s;
            

            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.cliente.Nombre.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "IDCliente":
                    elementos = elementos.OrderBy(s => s.cliente.IDCliente);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.IdCliente);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = dbc.PClientes.OrderBy(e => e.IdCliente).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));

            //Paginación
        }

        public ActionResult Finalizar()
        {
            try
            { 
                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            List<PagoFactura> numero = db.Database.SqlQuery<PagoFactura>("SELECT * FROM [dbo].[PagoFactura] WHERE IDPAGOFACTURA = (SELECT MAX(IDPAGOFACTURA) from PagoFactura)").ToList();
                int pago = numero.Select(s => s.IDPagoFactura).FirstOrDefault();

                try
                {
                    string cadena = "insert into CierreComisiones (fecha,Usuario,IDPagoFinal)values (sysdatetime()," + usuario + "," + pago + ")";

                    db.Database.ExecuteSqlCommand(cadena);
                        
                        }
                catch (Exception err) {
                }

            }
            catch (Exception err)
            {

            }
            new CierreComisionesContext().Database.ExecuteSqlCommand("update pagofactura set liquidada='1' where liquidada='0'");
            new CierreComisionesContext().Database.ExecuteSqlCommand("update ComisionVendedor set cierre='1'where cierre='0'");
            new CierreComisionesContext().Database.ExecuteSqlCommand("update ComisionesPagos set cierre='1' where cierre='0'");
            return RedirectToAction("CierreComisiones", new { Mensaje="Cierre terminado"});
        }

      }


    public class Vendedores
    {
        public int IDVendedor { get; set; }
        public string  NombreVendedor { get; set; }
       
    }
}