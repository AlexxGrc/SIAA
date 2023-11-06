using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Cfdi;
using SIAAPI.Models.Administracion;
using System.Globalization;
using SIAAPI.Facturas;
using System.Collections.Generic;
using PagedList;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Mail;
using SIAAPI.ViewModels.Comercial;
using System.Xml;
using System.Net;
using System.Data.SqlClient;
using SIAAPI.Reportes;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using SIAAPI.clasescfdi;
using static SIAAPI.Models.Comercial.ClienteRepository;
using System.Collections;
using SIAAPI.Models.Login;
using static SIAAPI.clasescfdi.ClsCartaporte2;
using SIAAPI.Models.CartaPorte;
using System.IO.Compression;

namespace SIAAPI.Controllers.Cfdi
{

    [Authorize(Roles = "Administrador,Facturacion,Ventas,Sistemas,Almacenista,Comercial,Gerencia, GerenteVentas, GestionCalidad")]
    public class FacturaAllController : Controller
    {
        EncfacturaContext db = new EncfacturaContext();
        EncfacturasSaldosContext dbs = new EncfacturasSaldosContext();


        // GET: FacturaComplemento

        public ActionResult Index(string Numero, string SerieFac, string ClieFac, string sortOrder, string currentFilter, int? page, int? PageSize, string Fechainicio, string Fechafinal, string FacPag, string Estado = "A")
        {
            //string ConsultaSql = "select * from Encfacturas ";
            string ConsultaSql = "Select top 500 * from dbo.EncfacturasSaldos";
            string cadenaSQl = string.Empty;

            if (SerieFac == null)
            {
                SerieFac = "MX";
            }
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
                        Filtro += " and  Serie=''";
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
                        Filtro += " and  Numero=" + Numero + "";
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
                        Filtro += " and  Nombre_cliente='" + ClieFac + "'";
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
                            Filtro += " and  pagada='1' ";
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
                            Filtro += " and  pagada='0' ";
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
                            Filtro += " and  Estado='C'";
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
                            Filtro += " and Estado='A'";
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
                dbs.Database.CommandTimeout = 300;

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

                ViewBag.Monto = "0";
                try
                {


                    ClsDatoDecimal data = db.Database.SqlQuery<ClsDatoDecimal>("select sum(total*tc) as Dato from EncFacturas where fecha between  DATEADD(day, -30, getdate()) AND getdate()").FirstOrDefault();
                    ViewBag.Monto = data.Dato;

                }
                catch (Exception err)
                {
                    string mensaje = err.Message;
                }

                ViewBag.Cobrado = "0";
                try
                {


                    ClsDatoDecimal data = db.Database.SqlQuery<ClsDatoDecimal>("select sum(total*tc) as Dato from EncFacturas where fecha between  DATEADD(day, -30, getdate()) AND getdate() and pagada='1'").FirstOrDefault();
                    ViewBag.Cobrado = data.Dato;

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
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25" , Selected = true},
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

                int pageNumber = (page ?? 1);
                int pageSize = (PageSize ?? 25);
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

        //public ActionResult Index(string Numero, string SerieFac, string ClieFac, string sortOrder, string currentFilter, int? page, int? PageSize, string Fechainicio, string Fechafinal, string FacPag, string Estado = "A")
        //{
        //    //string ConsultaSql = "select * from Encfacturas ";
        //    string ConsultaSql = "Select top 500 * from dbo.EncfacturasSaldos";
        //    string cadenaSQl = string.Empty;

        //    if (SerieFac== null)
        //    {
        //        SerieFac = "MX";
        //    }
        //    try
        //    {

        //        //Buscar Facturas: Pagadas o no pagadas
        //        var FacPagLst = new List<SelectListItem>();


        //        var EstadoLst = new List<SelectListItem>();
        //        //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });

        //        EstadoLst.Add(new SelectListItem { Text = "Cancelada", Value = "C" });
        //        EstadoLst.Add(new SelectListItem { Text = "Activas", Value = "A" });

        //        ViewData["Estado"] = EstadoLst;

        //        ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");

        //        ViewBag.Estadoseleccionado = Estado;  /// mandar el viewbag el parametro que viene de la pagina anterior
        //        //Facturas Pagadas
        //        FacPagLst.Add(new SelectListItem { Text = "Pagada", Value = "SI" });
        //        FacPagLst.Add(new SelectListItem { Text = "NoPagada", Value = "NO" });

        //        ViewData["FacPag"] = FacPagLst;



        //        ViewBag.FacPag = new SelectList(FacPagLst, "Value", "Text");

        //        ViewBag.Facpagseleccionado = FacPag; /// mandar el viewbag el parametro que viene de la pagina anterior

        //        //Buscar Serie Factura
        //        var SerLst = new List<string>();
        //        var SerQry = from d in db.encfacturas
        //                     orderby d.Serie
        //                     select d.Serie;

        //        SerLst.AddRange(SerQry.Distinct());

        //        ViewBag.SerieFac = new SelectList(SerLst);

        //        ViewBag.SerieFacseleccionado = SerieFac; /// mandar el viewbag el parametro que viene de la pagina anterior

        //        ViewBag.sumatoria = "";

        //        ViewBag.ClieFac = new ClienteRepository().GetClientesFactura();

        //        ViewBag.ClieFacseleccionado = ClieFac;/// mandar el viewbag el parametro que viene de la pagina anterior


        //        string ConsultaSqlResumen = "select Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from EncfacturasSaldos ";
        //        string ConsultaAgrupado = "group by Moneda order by Moneda ";
        //        string Filtro = string.Empty;



        //        string Orden = " order by fecha desc , serie , numero desc ";

        //        ///tabla filtro: serie
        //        if (!String.IsNullOrEmpty(SerieFac))
        //        {
        //            if (Filtro == string.Empty)
        //            {
        //                Filtro = "where Serie='" + SerieFac + "'";
        //            }
        //            else
        //            {
        //                Filtro += "and  Serie='" + SerieFac + "'";
        //            }

        //        }

        //        if (String.IsNullOrEmpty(SerieFac))
        //        {
        //            if (Filtro == string.Empty)
        //            {
        //                Filtro = "where Serie=''";
        //            }
        //            else
        //            {
        //                Filtro += " and  Serie=''";
        //            }

        //        }
        //        //Buscar por numero
        //        if (!String.IsNullOrEmpty(Numero))
        //        {
        //            if (Filtro == string.Empty)
        //            {
        //                Filtro = "where Numero=" + Numero + "";
        //            }
        //            else
        //            {
        //                Filtro += " and  Numero=" + Numero + "";
        //            }

        //        }

        //        ///tabla filtro: Nombre Cliente
        //        if (!String.IsNullOrEmpty(ClieFac))
        //        {

        //            if (Filtro == string.Empty)
        //            {
        //                Filtro = "where Nombre_cliente='" + ClieFac + "'";
        //            }
        //            else
        //            {
        //                Filtro += " and  Nombre_cliente='" + ClieFac + "'";
        //            }

        //        }

        //        ///tabla filtro: Factura pagada/no pagada && Serie, Nombre Cliente
        //        if (FacPag != "Todas")
        //        {
        //            if (FacPag == "SI")
        //            {
        //                if (Filtro == string.Empty)
        //                {
        //                    Filtro = "where pagada='1' ";
        //                }
        //                else
        //                {
        //                    Filtro += " and  pagada='1' ";
        //                }
        //            }
        //            if (FacPag == "NO")
        //            {
        //                if (Filtro == string.Empty)
        //                {
        //                    Filtro = "where pagada='0' ";
        //                }
        //                else
        //                {
        //                    Filtro += " and  pagada='0' ";
        //                }
        //            }
        //        }


        //        if (Estado != "Todos")
        //        {
        //            if (Estado == "C")
        //            {
        //                if (Filtro == string.Empty)
        //                {
        //                    Filtro = "where Estado='C'";
        //                }
        //                else
        //                {
        //                    Filtro += " and  Estado='C'";
        //                }
        //            }
        //            if (Estado == "A")
        //            {
        //                if (Filtro == string.Empty)
        //                {
        //                    Filtro = "where  Estado='A'";
        //                }
        //                else
        //                {
        //                    Filtro += " and Estado='A'";
        //                }
        //            }
        //        }



        //        if (!String.IsNullOrEmpty(Fechainicio) && String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
        //        {
        //            if (Filtro == string.Empty)
        //            {
        //                Filtro = "where  Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechainicio + " 23:59:59.999' ";
        //            }
        //            else
        //            {
        //                Filtro += " and Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechainicio + " 23:59:59.999'";
        //            }
        //        }


        //        if (!String.IsNullOrEmpty(Fechainicio) && !String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
        //        {
        //            if (Filtro == string.Empty)
        //            {
        //                Filtro = "where  Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999' ";
        //            }
        //            else
        //            {
        //                Filtro += " and Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999'";
        //            }
        //        }


        //        ViewBag.CurrentSort = sortOrder;
        //        ViewBag.SerieSortParm = String.IsNullOrEmpty(sortOrder) ? "Serie" : "";
        //        ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "Numero" : "";
        //        ViewBag.FechaSortParm = sortOrder == "Fecha" ? "Fecha" : "";
        //        ViewBag.ClienteSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre_Cliente" : "";




        //        // Pass filtering string to view in order to maintain filtering when paging
        //        ViewBag.Fechainicio = Fechainicio;
        //        ViewBag.Fechafinal = Fechafinal;

        //        //Ordenacion

        //        switch (sortOrder)
        //        {
        //            case "Activa":
        //                Orden = " order by  Estado ";
        //                break;
        //            case "Serie":
        //                Orden = " order by  serie , numero desc ";
        //                break;
        //            case "Numero":
        //                Orden = " order by   numero asc ";
        //                break;
        //            case "Fecha":
        //                Orden = " order by fecha ";
        //                break;
        //            case "Nombre_Cliente":
        //                Orden = " order by  Nombre_cliente ";
        //                break;
        //            default:
        //                Orden = " order by fecha desc , serie , numero desc ";
        //                break;
        //        }


        //        // si no ha seleccionado nada muestra las facturas del ultimo mes 

        //        if (Filtro == " where  Estado='A'")
        //        {
        //            Filtro += " and Fecha >='" + DateTime.Now.AddDays(-30).Year + "-" + DateTime.Now.AddDays(-30).Month + "-" + DateTime.Now.AddDays(-30).Day + "'  and  Fecha<'" + DateTime.Now.AddDays(1).Year + "-" + DateTime.Now.AddDays(1).Month + "-" + DateTime.Now.AddDays(1).Day + "' ";
        //            ViewBag.Fechainicio = "" + DateTime.Now.AddDays(-30).Year + "-" + DateTime.Now.AddDays(-30).Month + "-" + DateTime.Now.AddDays(-30).Day + "";
        //            ViewBag.Fechafinal = "" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "";
        //        }

        //        //var elementos = from s in db.encfacturas
        //        //select s;

        //        cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;
        //        dbs.Database.CommandTimeout = 300;

        //        //var elementos = db.Database.SqlQuery<Encfacturas>(cadenaSQl).ToList();
        //        var elementos = dbs.Database.SqlQuery<EncfacturasSaldos>(cadenaSQl).ToList();

        //        ViewBag.sumatoria = "";
        //        try
        //        {

        //            var SumaLst = new List<string>();
        //            var SumaQry = ConsultaSqlResumen + " " + Filtro + " " + ConsultaAgrupado;
        //            List<ResumenFac> data = db.Database.SqlQuery<ResumenFac>(SumaQry).ToList();
        //            ViewBag.sumatoria = data;

        //        }
        //        catch (Exception err)
        //        {
        //            string mensaje = err.Message;
        //        }

        //        //Paginación
        //        // DROPDOWNLIST FOR UPDATING PAGE SIZE
        //        //int count = db.Encfacturas.OrderBy(e => e.Serie).Count();// Total number of elements
        //        int count = elementos.Count();// Total number of elements
        //        // Populate DropDownList
        //        ViewBag.PageSize = new List<SelectListItem>()
        //    {
        //        new SelectListItem { Text = "10", Value = "10" },
        //        new SelectListItem { Text = "25", Value = "25" , Selected = true},
        //        new SelectListItem { Text = "50", Value = "50" },
        //        new SelectListItem { Text = "100", Value = "100" },
        //        new SelectListItem { Text = "Todos", Value = count.ToString() }
        //     };

        //        int pageNumber = (page ?? 1);
        //        int pageSize = (PageSize ?? 25);
        //        ViewBag.psize = pageSize;


        //        return View(elementos.ToPagedList(pageNumber, pageSize));
        //        //Paginación
        //    }
        //    catch(Exception err)
        //    {
        //        string mensaje = err.Message;

        //        var reshtml = Server.HtmlEncode(cadenaSQl);

        //        return Content(reshtml);
        //    }
        //}

        public void FacturacionAnoVsAnterior()
        {
            EncfacturaContext db = new EncfacturaContext();
            ClsDatoEntero anoactual = db.Database.SqlQuery<ClsDatoEntero>("select year(getdate()) as Dato").ToList()[0];
            ClsDatoEntero anoanterior = db.Database.SqlQuery<ClsDatoEntero>("select (year(getdate())-1) as Dato").ToList()[0];
            ClsDatoEntero mesact = db.Database.SqlQuery<ClsDatoEntero>("select (month(getdate())+1) as Dato").ToList()[0];
            var cadEjecini = "update DiferenciaFac set AnoActualMXN = 0, AnoActualUSD = 0, AnoActualTotMXN = 0, AnoAnteriorMXN = 0, AnoAnteriorUSD = 0, AnoAnteriorTotMXN = 0";
            db.Database.ExecuteSqlCommand(cadEjecini);

            DateTime fecAct = DateTime.Today;
            string FA = fecAct.ToString("dd/MM/yyyy");

            List<c_Moneda> monedamxn = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
            int mxn = monedamxn.Select(s => s.IDMoneda).FirstOrDefault();
            List<c_Moneda> monedausd = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='USD'").ToList();
            int usd = monedausd.Select(s => s.IDMoneda).FirstOrDefault();
            string cadena = "";
            decimal AcMXN = 0;
            decimal AcUSD = 0;
            decimal AcTot = 0;
            decimal AnMXN = 0;
            decimal AnUSD = 0;
            decimal AnTot = 0;

            //Actualizo la tabla con los datos de los totales de la suma de los pedidos del año actual y el año anterior
            int n = 1;

            while (n < 13)
            {
                //Año actual
                ClsDatoEntero ActExisten = db.Database.SqlQuery<ClsDatoEntero>("select count(*) as Dato from EncFacturas where year(Fecha) = " + anoactual.Dato + " and month(Fecha)  = " + n + "").ToList()[0];
                if (ActExisten.Dato != 0)
                {
                    var cadMXNAct = "select case WHEN sum(Total) IS NULL THEN (0) ELSE sum(Total) END AS Dato from EncFacturas where Estado != 'C'  and year(Fecha) = " + anoactual.Dato + " and month(Fecha)  = " + n + " and IDMoneda = " + mxn + "";
                    var cadUSDAct = "select case WHEN sum(Total) IS NULL THEN (0) ELSE sum(Total) END AS Dato from EncFacturas where Estado != 'C' and year(Fecha) = " + anoactual.Dato + " and month(Fecha)  = " + n + " and IDMoneda = " + usd + "";
                    var cadtotAct = "select case WHEN sum(Total*TC) IS NULL THEN (0) ELSE sum(Total*TC) END AS Dato from EncFacturas where Estado != 'C' and year(Fecha) = " + anoactual.Dato + " and month(Fecha)  = " + n + " ";
                    ClsDatoDecimal ActMXN = db.Database.SqlQuery<ClsDatoDecimal>(cadMXNAct).ToList().FirstOrDefault();
                    ClsDatoDecimal ActUSD = db.Database.SqlQuery<ClsDatoDecimal>(cadUSDAct).ToList().FirstOrDefault();
                    ClsDatoDecimal ActTot = db.Database.SqlQuery<ClsDatoDecimal>(cadtotAct).ToList().FirstOrDefault();
                    AcMXN = ActMXN.Dato;
                    AcUSD = ActUSD.Dato;
                    AcTot = ActTot.Dato;
                }
                else
                {
                    AcMXN = 0;
                    AcUSD = 0;
                    AcTot = 0;
                }
                //Año anterior
                ClsDatoEntero AntExisten = db.Database.SqlQuery<ClsDatoEntero>("select count(*) as Dato from EncFacturas where year(Fecha) = " + anoanterior.Dato + " and month(Fecha)  = " + n + "").ToList()[0];
                if (AntExisten.Dato != 0)
                {
                    var cadMXNAnt = "select case WHEN sum(Total) IS NULL THEN (0) ELSE sum(Total) END AS Dato from EncFacturas where Estado != 'C' and year(Fecha) = " + anoanterior.Dato + " and month(Fecha)  = " + n + " and IDMoneda = " + mxn + "";
                    var cadUSDAnt = "select case WHEN sum(Total) IS NULL THEN (0) ELSE sum(Total) END AS Dato from EncFacturas where Estado != 'C' and year(Fecha) = " + anoanterior.Dato + " and month(Fecha)  = " + n + " and IDMoneda = " + usd + "";
                    var cadtotAnt = "select case WHEN sum(Total*TC) IS NULL THEN (0) ELSE sum(Total*TC) END AS Dato from EncFacturas where Estado != 'C' and year(Fecha) = " + anoanterior.Dato + " and month(Fecha)  = " + n + " ";
                    ClsDatoDecimal AntMXN = db.Database.SqlQuery<ClsDatoDecimal>(cadMXNAnt).ToList().FirstOrDefault();
                    ClsDatoDecimal AntUSD = db.Database.SqlQuery<ClsDatoDecimal>(cadUSDAnt).ToList().FirstOrDefault();
                    ClsDatoDecimal AntTot = db.Database.SqlQuery<ClsDatoDecimal>(cadtotAnt).ToList().FirstOrDefault();
                    AnMXN = AntMXN.Dato;
                    AnUSD = AntUSD.Dato;
                    AnTot = AntTot.Dato;
                }
                else
                {
                    AnMXN = 0;
                    AnUSD = 0;
                    AnTot = 0;
                }

                // Se actualizan los valores de la tabla
                var cadEjec = "update DiferenciaFac set AnoActualMXN = " + AcMXN + ", AnoActualUSD = " + AcUSD + ", AnoActualTotMXN = " + AcTot + "";
                cadEjec += ", AnoAnteriorMXN = " + AnMXN + ", AnoAnteriorUSD = " + AnUSD + ", AnoAnteriorTotMXN = " + AnTot + " ";
                cadEjec += "where IDMes = " + n + "";
                db.Database.ExecuteSqlCommand(cadEjec);

                ////Diferencia Año Actual - Año Anterior
                //var caddif = "select (AnoActualTotMXN - AnoAnteriorTotMXN) as Dato from DiferenciaPed where IDMes = " + n + "";
                //ClsDatoDecimal difMXN = db.Database.SqlQuery<ClsDatoDecimal>(caddif).ToList()[0];
                //var cadEjecDif = "update DiferenciaPed set DiferenciaMXN =" + difMXN.Dato + " where IDMes = " + n + "";
                //db.Database.ExecuteSqlCommand(cadEjecDif);

                n++;
            }
            //Diferencia Año Actual - Año Anterior
            //var cadEjecDif = "update DiferenciaPed set DiferenciaMXN = (AnoActualTotMXN - AnoAnteriorTotMXN)";
            //db.Database.ExecuteSqlCommand(cadEjecDif);


            // Cargo los datos actualizados para el reporte
            cadena = "select * from DiferenciaFac ";
            var datos = db.Database.SqlQuery<DiferenciaFac>(cadena).ToList();
            ViewBag.datos = datos;


            ExcelPackage Ep = new ExcelPackage();
            //Crear la hoja y poner el nombre de la pestaña del libro
            var Sheet = Ep.Workbook.Worksheets.Add("Facturas");

            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:I1"].Style.Font.Size = 20;
            Sheet.Cells["A1:I1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:I3"].Style.Font.Bold = true;
            Sheet.Cells["A1:I1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Facturación Año Actual vs Año Anterior");

            row = 2;
            Sheet.Cells["A1:I1"].Style.Font.Size = 12;
            Sheet.Cells["A1:I1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A2:I2"].Style.Font.Bold = true;
            //Subtitulo según el filtrado del periodo de datos
            row = 2;
            Sheet.Cells[string.Format("A2", row)].Value = "Fecha Reporte";
            Sheet.Cells[string.Format("C2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            Sheet.Cells[string.Format("C2", row)].Value = fecAct;

            //En la fila3 se da el formato a el encabezado
            row = 3;
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 10;
            Sheet.Cells["A3:I3"].Style.Font.Bold = true;
            Sheet.Cells["A3:I3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A3:I3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            // Se establece el nombre que identifica a cada una de las columnas de datos. 
            Sheet.Cells["C3:E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["C3:E3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Goldenrod);
            Sheet.Cells[string.Format("C3", row)].Value = anoactual.Dato;

            Sheet.Cells["F3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["F3:H3"].Style.Font.Color.SetColor(Color.White);
            Sheet.Cells["F3:H3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.MidnightBlue);
            Sheet.Cells[string.Format("F3", row)].Value = anoanterior.Dato;
            //Aplicar borde doble al rango de celdas A3:Q3
            Sheet.Cells["C3:H3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;


            row = 4;
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 10;
            Sheet.Cells["A4:I4"].Style.Font.Bold = true;
            Sheet.Cells["A4:I4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A4:I4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            Sheet.Cells["C4:E4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleGoldenrod);
            Sheet.Cells["F4:H4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PowderBlue);
            // Se establece el nombre que identifica a cada una de las columnas de datos.
            Sheet.Cells["A4"].RichText.Add("No");
            Sheet.Cells["B4"].RichText.Add("Mes");
            Sheet.Cells["C4"].RichText.Add("MXN");
            Sheet.Cells["D4"].RichText.Add("USD");
            Sheet.Cells["E4"].RichText.Add("Total MXN");
            Sheet.Cells["F4"].RichText.Add("MXN");
            Sheet.Cells["G4"].RichText.Add("USD");
            Sheet.Cells["H4"].RichText.Add("Total MXN");
            Sheet.Cells["I4"].RichText.Add("TotalMXN(AñoActual - AñoAnterior)");

            //Aplicar borde doble al rango de celdas A3:Q3
            Sheet.Cells["A4:I4"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
            // Se establecen los formatos para las celdas: Fecha, Moneda
            row = 5;
            Sheet.Cells.Style.Font.Bold = false;
            foreach (DiferenciaFac item in ViewBag.datos)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDMes;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Mes;
                Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("C{0}", row)].Value = item.AnoActualMXN;
                Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("D{0}", row)].Value = item.AnoActualUSD;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("E{0}", row)].Value = item.AnoActualTotMXN;
                Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("F{0}", row)].Value = item.AnoAnteriorMXN;
                Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("G{0}", row)].Value = item.AnoAnteriorUSD;
                Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("H{0}", row)].Value = item.AnoAnteriorTotMXN;
                Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("I{0}", row)].Value = item.AnoActualTotMXN - item.AnoAnteriorTotMXN;
                row++;
            }

            //Se genera el archivo y se descarga
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "FacturacionAnoVsAnterior.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
            //return Redirect("/blah");
        }
        public void Descargarxml(int id)
        {
            // Obtener contenido del archivo
            var elemento = db.encfacturas.Single(m => m.ID == id);

            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=Factura-" + elemento.Serie+ elemento.Numero +".Xml");
            System.Web.HttpContext.Current.Response.WriteFile(System.Web.HttpContext.Current.Server.MapPath("~/Xmlfacturas/"+ elemento.UUID + ".Xml"));

        }                                              


        


        public ActionResult DescargarPdf(int id)
        {
            // Obtener contenido del archivo
            var elemento = db.encfacturas.Single(m => m.ID == id);



            string rutaArchivo = elemento.UUID + ".xml";
            string fileName = System.Web.HttpContext.Current.Server.MapPath("~/Xmlfacturas/" + rutaArchivo);
            string xmlString = System.IO.File.ReadAllText(fileName);

            //Console.WriteLine("Esto es mi mensaje : entre aqui" );
            // toma la empresa que tenga el id 1 via de mientras 
            var empresa = db.Empresa.Single(m => m.IDEmpresa == 2);

            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);



            List<EncPrefactura> prefactura = new PrefacturaContext().Database.SqlQuery<EncPrefactura>("select * from encprefactura where Seriedigital='" + elemento.Serie + "' and [NumeroDigital]=" + elemento.Numero ).ToList();
            Generador.CreaPDF documento = null;
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            if (prefactura.Count==0 || elemento.NotaCredito==true)  /// checa prefactura
            {
                documento = new Generador.CreaPDF(xmlString, logoempresa, "Tel. " + empresa.Telefono + " " + empresa.mail, true);
              
                return new FilePathResult(documento.nombreDocumento, contentType);
            }
            else
            {
                EncPrefactura prefact = prefactura[0];
                documento = new Generador.CreaPDF(xmlString, logoempresa, prefact, "Tel. " + empresa.Telefono +" " + empresa.mail, true);
              
                return new FilePathResult(documento.nombreDocumento, contentType);
            }

        }

       


        public ActionResult EnviarPdf(int id)
        {
            // Obtener contenido del archivo
            var elemento = db.encfacturas.Single(m => m.ID == id);

            Clientes Cliente = null;

            try
            {

                ClsDatoEntero idcliente = db.Database.SqlQuery<ClsDatoEntero>("select IDCliente as Dato from [dbo].[Clientes] where Nombre='" + elemento.Nombre_cliente + "'").ToList()[0];

                ClientesContext clientes = new ClientesContext();
                Cliente = clientes.Clientes.Find(idcliente.Dato);
            }

            catch(Exception err)
            {
                string error = err.Message;
                Cliente = null;
            }

            try
            {
                if (string.IsNullOrEmpty(Cliente.CorreoCfdi))
                {
                    var reshtml = Server.HtmlEncode("No hay correo cfdi registrado o no existe registro del cliente");

                    return Content(reshtml);
                }
            }
            catch(Exception err)
            {

                string error = err.Message;
                var reshtml = Server.HtmlEncode("No hay correo cfdi registrado o no existe registro del cliente ");

                return Content(reshtml);

            }

            string rutaArchivo = elemento.UUID + ".xml";
            string fileName=    System.Web.HttpContext.Current.Server.MapPath("~/Xmlfacturas/" + rutaArchivo);
            string xmlString = System.IO.File.ReadAllText(fileName);

         
            //Console.WriteLine("Esto es mi mensaje : entre aqui" );
            // toma la empresa que tenga el id 1 via de mientras 
            var empresa = db.Empresa.Single(m => m.IDEmpresa == 2);

            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);


            List<EncPrefactura> prefactura = new PrefacturaContext().Database.SqlQuery<EncPrefactura>("select * from encprefactura where Seriedigital='" + elemento.Serie + "' and [NumeroDigital]=" + elemento.Numero).ToList();
            Generador.CreaPDF documento = null;
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;

            if (prefactura.Count == 0)  /// checa prefactura
            {
                documento = new Generador.CreaPDF(xmlString, logoempresa, "Tel. " + empresa.Telefono + " " + empresa.mail, false);

            
            }
            else
            {
                EncPrefactura prefact = prefactura.FirstOrDefault();
                documento = new Generador.CreaPDF(xmlString, logoempresa, prefact, "Tel. " + empresa.Telefono + " " + empresa.mail, true);

             
            }




        //    Generador.CreaPDF documento = new Generador.CreaPDF(xmlString, logoempresa, "Tel. " + empresa.Telefono +" " + empresa.mail, false);

            string correo = string.Empty;
            string password = string.Empty;
            string correofirma = string.Empty;
            string servidor = "smtp.gmail.com";
            int puerto = 587;
            string nombrecorreo = empresa.RazonSocial;
            string asunto = "Factura";
            string titulo = "Gracias por su preferencia  ";
            string cuerpo = "Adjuntamos su factura electronica";
            string firma = "Si tiene alguna duda o comentario no replique este mail, favor de escribir a";
            string copiaoculta = string.Empty;

            XmlDocument xmail = new XmlDocument();
           xmail.Load(System.Web.HttpContext.Current.Server.MapPath("~/configcorreofactura.xml"));
           

                XmlNode mailnode = xmail.FirstChild;
                try
                {
                foreach (XmlNode elementomail in mailnode.ChildNodes)
                {
                    
                    if (elementomail.Name=="correo")
                        {
                            correo = elementomail.InnerText;
                        }
                        if (elementomail.Name == "password")
                    {
                        password = elementomail.InnerText;
                    }
                    if (elementomail.Name == "CorreofirmaFactura")
                    {
                        correofirma = elementomail.InnerText;
                    }
                    if (elementomail.Name == "servidor")
                    {
                        servidor = elementomail.InnerText;
                    }

                    if (elementomail.Name == "puerto")
                    {
                        puerto = Int32.Parse( elementomail.InnerText);
                    }

                    if (elementomail.Name == "nombre")
                    {
                        nombrecorreo = elementomail.InnerText;
                    }

                    if (elementomail.Name == "asunto")
                    {
                        asunto = elementomail.InnerText;
                    }

                    if (elementomail.Name == "titulo")
                    {
                        titulo = elementomail.InnerText;
                    }
                    if (elementomail.Name == "cuerpo")
                    {
                        cuerpo = elementomail.InnerText;
                    }
                    if (elementomail.Name == "copiaoculta")
                    {
                        copiaoculta = elementomail.InnerText;
                    }

                    
                }
                }
                catch (Exception err)
                {
                    string error = err.Message;
                }



            try
            {


                SmtpClient mySmtpClient = new SmtpClient();

                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = false;
                System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential(correo, password);
                mySmtpClient.Credentials = basicAuthenticationInfo;
                mySmtpClient.Host = servidor;
                mySmtpClient.Port = puerto;
                mySmtpClient.EnableSsl = false;
                mySmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                // add from,to mailaddresses
                MailAddress from = new MailAddress(correo, nombrecorreo);
                // MailAddress to = new MailAddress(Cliente.CorreoCfdi, elemento.Nombre_cliente);

                Cliente.CorreoCfdi.Replace(';', ',');
                string[] mails = Cliente.CorreoCfdi.Split(',');


                if (mails.Length == 0)
                {
                    throw new Exception("No hay Correos registrados");
                }


                MailAddress to = new MailAddress(mails[0], elemento.Nombre_cliente);
                MailMessage myMail = new MailMessage(from, to);


                if (mails.Length > 1)
                {
                    for (int it = 1; it <= mails.Length - 1; it++)
                    {
                        myMail.CC.Add(mails[it]);
                    }
                }
                //Con copia a 
                myMail.Bcc.Add(copiaoculta);


                // set subject and encoding
                myMail.Subject = asunto;
                myMail.SubjectEncoding = Encoding.UTF8;


                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                sb.Append("<html>");
                sb.Append("<head><title>" + titulo +"</title></ head >");

                sb.Append("</head>");
                sb.Append("<body>");
                sb.Append("<h4>" + cuerpo+ " " + elemento.Serie + elemento.Numero + " del dia " + elemento.Fecha.ToShortDateString() + " </h4>");
                sb.Append("<h6>" + firma + "</h6 >" + correofirma);

                sb.Append("</body></html>");
                // set body-message and encoding
                myMail.Body = sb.ToString();
                //_memoryStream.Position = 0;
                myMail.Attachments.Add(new Attachment(documento.nombreDocumento));

                string archivoxml = System.Web.HttpContext.Current.Server.MapPath("~/Xmlfacturas/" + elemento.UUID + ".xml");
                myMail.Attachments.Add(new Attachment(archivoxml));


                myMail.BodyEncoding = Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;

                mySmtpClient.Send(myMail);
           
            
            }
            catch (Exception err)
            {
                return Content("<html><body><h2>tu mail no se ha enviado</h2><div>"+err.Message+"</div></body></html>", "text/html");
            }
            //   return RedirectToAction("Index");
            return Content("<html><body>tu mail se ha enviado correctamente</body></html>", "text/html");
            //return File(nombrededocumento, "application / pdf", nombrededocumento); 
        }
        public ActionResult ConsultaSaldo()
        {
            MultiFacturasSDK.SDKRespuesta respuesta = new ClsFactura().consulta_saldo();
            ViewBag.Saldo = "0";
            if (respuesta.Codigo_MF_Texto == "OK")

            {
                //string archivoTXT = System.Web.HttpContext.Current.Server.MapPath("~/sdk2/timbrados/respuesta.ini");
                //StreamReader resultado = new StreamReader(archivoTXT);
                //string line;
                //// Read and display lines from the file until the end of
                //// the file is reached.
                //while ((line = resultado.ReadLine()) != null)
                //{
                //    string[] valores = line.Split('=');
                //    if (valores[0].ToUpper() == "SALDO")
                //    {
                //        ViewBag.Saldo = valores[2];
                //    }
                //}

            }
            return View();

        }


        public ActionResult ValidarClie()
        {
           
           
            string querySQL2 = "update dbo.EncFacturas set IDCliente = t2.IDCliente from dbo.EncFacturas t1, dbo.Clientes as t2  where t1.Nombre_Cliente = t2.Nombre";
            db.Database.ExecuteSqlCommand(querySQL2);

           

            return RedirectToAction("Index");
        }

        public ActionResult CreatedesdeArchivo()
        {
            return View();
        }

       
        [HttpPost]
        public ActionResult CreatedesdeArchivo(FormCollection collection)
        {
            Encfacturas elemento = new Encfacturas();

            try
            {
                /*Aqui voy intentar guardar la imagen que me pase la vista*/
                try
                {
                    HttpPostedFileBase archivo = Request.Files["Imag1"];
                    //archivo.SaveAs(Path.Combine(directory, Path.GetFileName(archivo.FileName)));
                    // Generador.CreaPDF(Path.Combine(@".\facturas", Path.GetFileName(archivo.FileName)));
                    StreamReader reader = new StreamReader(archivo.InputStream);
                    String contenidoxml = reader.ReadToEnd();


                    Generador.CreaPDF temp = new Generador.CreaPDF(contenidoxml);


                    try
                    {
                        EncfacturaContext dbp = new EncfacturaContext();
                        Encfacturas proveedorenlabase = dbp.Database.SqlQuery<Encfacturas>("select * from Encfacturas where UUID='" + temp._templatePDF.folioFiscalUUID + "'").ToList()[0];
                        int IDpbase = proveedorenlabase.ID; /// si la consulta no devolvio fila lanazara una excepcion 
                        string mensajealusuario = "LA FACTURA YA SE ENCUENTRA EN EL SISTEMA ";
                        ViewBag.Mensajeerror = mensajealusuario;
                        return View();
                    }
                    catch (Exception err)
                    {
                        //string error = err.Message;
                        String mensaje = err.Message;

                    }


                    elemento.Fecha = System.DateTime.Parse(temp._templatePDF.fechaEmisionCFDI);
                    elemento.Serie = temp._templatePDF.serie;
                    elemento.Numero = Int32.Parse(temp._templatePDF.folio);
                    elemento.Nombre_cliente = temp._templatePDF.receptor.razonSocial;
                    elemento.Subtotal = decimal.Parse(temp._templatePDF.subtotal.ToString());
                    elemento.Total = decimal.Parse(temp._templatePDF.total.ToString());
                    elemento.IVA = elemento.Total - elemento.Subtotal;

                    int idcliente = 0;

                    try
                    {
                        ClsDatoEntero clientecapturado = db.Database.SqlQuery<ClsDatoEntero>("select IDCLIENTE AS dato from clientes NOMBRE='" + elemento.Nombre_cliente + "'").ToList()[0];
                        idcliente = clientecapturado.Dato;
                    }

                    catch
                    {

                    }

                    List<c_Moneda> clavemoneda;
                    clavemoneda = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE claveMoneda='" + temp._templatePDF.claveMoneda + "'").ToList();
                    int clave = clavemoneda.Select(s => s.IDMoneda).FirstOrDefault();
                    elemento.IDMoneda = clave;
                    elemento.Moneda = temp._templatePDF.claveMoneda;
                    elemento.Estado = "A";
                    elemento.TC = decimal.Parse(temp._templatePDF.tipo_cambio);
                    elemento.IDCliente = idcliente;
                    // elemento.RutaXML = contenidoxml;

                    elemento.FechaRevision= System.DateTime.Parse(temp._templatePDF.fechaEmisionCFDI);
                    elemento.FechaVencimiento = System.DateTime.Parse(temp._templatePDF.fechaEmisionCFDI);

                    elemento.Anticipo = false;
                        if (temp._templatePDF.productos.Count==1 && temp._templatePDF.productos[0].desc.Contains("Anticipo") && temp._templatePDF.TipoDecomprobrante=="I")
                    {
                        elemento.Anticipo = true;
                    }
                    elemento.NotaCredito = false;
                    if (temp._templatePDF.productos.Count == 1 && (temp._templatePDF.TipoDeRelacion=="01" || temp._templatePDF.TipoDeRelacion == "03") && temp._templatePDF.TipoDecomprobrante == "E")
                    {
                        elemento.NotaCredito = true;
                    }


                    elemento.RutaXML = "";

                  
                    elemento.UUID = temp._templatePDF.folioFiscalUUID;

                    try
                    {
                        EscribeEnArchivo(contenidoxml, elemento.UUID + ".xml", true);
                    }
                    catch(Exception err)
                    {
                        string mensaje = err.Message;
                    }



                    elemento.IDMetododepago = temp._templatePDF.metodoPago;

                  
                    elemento.ConPagos = false;

                    db.encfacturas.Add(elemento);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ERR)
                {
                    ViewBag.Mensajeerror = ERR.InnerException.Message;
                    return View();
                }

              
            }
            catch(Exception ERR2)
            {
                string Mensaje = ERR2.Message;
                ViewBag.Mensajeerror = "Este archivo Xml no contiene una factura valida";
                return View();
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

        public ActionResult Darpagada(int id)
        {
            Encfacturas factura = new EncfacturaContext().encfacturas.Find(id);
            try
            {
                new EncfacturaContext().Database.ExecuteSqlCommand("update encfacturas set pagada='1' where id=" + id);
                new EncfacturaContext().Database.ExecuteSqlCommand("update saldofactura importesaldoinsoluto=0 , importepagado=total where idfactura=" + id);   

            }
            catch(Exception err)
            {

            }
            return RedirectToAction("Index", new { Numero = factura.Numero, SerieFac = factura.Serie });
        }
        [HttpPost]
        public JsonResult cancelarFactura(int id)
        {
            var elemento = db.encfacturas.Single(m => m.ID == id);

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(elemento.RutaXML));
            StreamReader reader = new StreamReader(stream);
            String contenidoxml = reader.ReadToEnd();

            String cadenaxml = System.Web.HttpContext.Current.Server.MapPath("~/Documentostemporales/cancelar.xml");

            StreamWriter sw = new StreamWriter(cadenaxml, true);

            sw.Write(contenidoxml);
            sw.Close();

            Generador.CreaPDF temp = new Generador.CreaPDF(contenidoxml);


            try
            {
                //bool salida = new ClsFactura().cancelarbyid(temp._templatePDF.folioFiscalUUID, contenidoxml);



                if (temp._templatePDF.UUIDrelacionados.Count() > 0 && (temp._templatePDF.TipoDeRelacion == "03" || temp._templatePDF.TipoDeRelacion == "01"))
                {
                    try
                    {
                        string folio = temp._templatePDF.UUIDrelacionados[0].UUID;
                        decimal total = temp._templatePDF.total;
                        Encfacturas factura = new EncfacturaContext().encfacturas.Where(s => s.UUID == folio).FirstOrDefault();
                        new EncfacturaContext().Database.ExecuteSqlCommand("update saldofactura set ImportePagado=(ImportePagado - " + total + ") where serie ='" + factura.Serie + "' and numero =" + factura.Numero);
                        new EncfacturaContext().Database.ExecuteSqlCommand("update saldofactura set  ImporteSaldoInsoluto=(Total -importepagado) where serie ='" + factura.Serie + "' and numero =" + factura.Numero);

                        new EncfacturaContext().Database.ExecuteSqlCommand("update saldofactura set  ImporteSaldoAnterior=(ImporteSaldoAnterior + " + total + ") where serie ='" + factura.Serie + "' and numero =" + factura.Numero);

                    }
                    catch (Exception err)
                    {

                    }
                }
                else if (temp._templatePDF.UUIDrelacionados.Count() > 0 && (temp._templatePDF.TipoDeRelacion == "07"))
                {
                    string uuidAnticipo = temp._templatePDF.UUIDrelacionados[0].UUID;

                    EncfacturaContext db = new EncfacturaContext();
                    Encfacturas factura = new EncfacturaContext().Database.SqlQuery<Encfacturas>("select * from Encfacturas where UUID='" + uuidAnticipo + "'").ToList().FirstOrDefault();
                    int IDFacturaAnticipo = factura.ID;

                    try
                    {
                        string cadenasaldo = "update dbo.saldofactura set importesaldoinsoluto=0 where idfactura=" + id;
                        db.Database.ExecuteSqlCommand(cadenasaldo);
                        db.Database.ExecuteSqlCommand("delete  from AplicacionAnticipos where IDFacturaAnticipo =" + IDFacturaAnticipo);
                       

                    }
                    catch (SqlException err)
                    {
                        string mensajeerror = err.Message;
                    }

                }
                else
                {
                    string cadenasaldo = "update dbo.saldofactura set importesaldoinsoluto=0 where idfactura=" + id;
                    db.Database.ExecuteSqlCommand(cadenasaldo);
                }
                string cadenasql = "update encfacturas set Estado='C' where ID=" + id;

                db.Database.ExecuteSqlCommand(cadenasql);
                string cadenasale = "update encprefactura set idfacturadigital=0,seriedigital='',numerodigital=0,status='Activo' where idfacturadigital= " + id;
                db.Database.ExecuteSqlCommand(cadenasale);
                //CANCELAR DOCUMENTO RELCIONADO
                try
                {
                    NotasCredito notasCredito = new NotasCreditoContext().Database.SqlQuery<NotasCredito>("select*from NotasCredito where IDFacturaNota="+ id).FirstOrDefault();

                    db.Database.ExecuteSqlCommand("update documentorelacionado set StatusDocto='C' where IDDocumentoRelacionado="+ notasCredito.IDDocumentoRelacionado);
                }
                catch (Exception ERR)
                {

                }

                //try
                //{
                //    List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                //    int UserID = userid.Select(s => s.UserID).FirstOrDefault();
                //    //registro de cancelacion

                //    string cadena = "insert into RegistroCancelacionFacturas(IDFactura, Fecha, Usuario) values (" + id + ",SYSDATETIME()," + UserID + ")";
                //    db.Database.ExecuteSqlCommand(cadena);

                //}
                //catch (Exception err)
                //{

                //}
                return Json(new HttpStatusCodeResult(200));



            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }

        }


        //[HttpPost]
        //public JsonResult cancelarFactura(int id)
        //{
        //    var elemento = db.encfacturas.Single(m => m.ID == id);

        //    var stream = new MemoryStream(Encoding.UTF8.GetBytes(elemento.RutaXML));
        //    StreamReader reader = new StreamReader(stream);
        //    String contenidoxml = reader.ReadToEnd();

        //    String cadenaxml = System.Web.HttpContext.Current.Server.MapPath("~/Documentostemporales/cancelar.xml");

        //    StreamWriter sw = new StreamWriter(cadenaxml, true);

        //    sw.Write(contenidoxml);
        //    sw.Close();

        //    Generador.CreaPDF temp = new Generador.CreaPDF(contenidoxml);


        //    try
        //    {
        //        bool salida = true;/*new ClsFactura().cancelarbyid(temp._templatePDF.folioFiscalUUID, contenidoxml);*/


        //        if (salida)
        //        {
        //            if (temp._templatePDF.UUIDrelacionados.Count() > 0 && (temp._templatePDF.TipoDeRelacion == "03" || temp._templatePDF.TipoDeRelacion == "01"))
        //            {
        //                try
        //                {
        //                    string folio = temp._templatePDF.UUIDrelacionados[0].UUID;
        //                    decimal total = temp._templatePDF.total;
        //                    Encfacturas factura = new EncfacturaContext().encfacturas.Where(s => s.UUID == folio).FirstOrDefault();
        //                    new EncfacturaContext().Database.ExecuteSqlCommand("update saldofactura set ImportePagado=(ImportePagado - " + total + ") where serie ='" + factura.Serie + "' and numero =" + factura.Numero);
        //                    new EncfacturaContext().Database.ExecuteSqlCommand("update saldofactura set  ImporteSaldoInsoluto=(Total -importepagado) where serie ='" + factura.Serie + "' and numero =" + factura.Numero);

        //                    new EncfacturaContext().Database.ExecuteSqlCommand("update saldofactura set  ImporteSaldoAnterior=(ImporteSaldoAnterior + " + total + ") where serie ='" + factura.Serie + "' and numero =" + factura.Numero);
        //                }
        //                catch (Exception err)
        //                {

        //                }
        //            }
        //            else
        //            {
        //                string cadenasaldo = "update dbo.saldofactura set importesaldoinsoluto=0 where idfactura=" + id;
        //                db.Database.ExecuteSqlCommand(cadenasaldo);
        //            }
        //            string cadenasql = "update encfacturas set Estado='C' where ID=" + id;

        //            db.Database.ExecuteSqlCommand(cadenasql);
        //            string cadenasale = "update encprefactura set idfacturadigital=0,seriedigital='',numerodigital=0,status='Activo' where idfacturadigital= " + id;
        //            db.Database.ExecuteSqlCommand(cadenasale);

        //            try
        //            {
        //                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
        //                int UserID = userid.Select(s => s.UserID).FirstOrDefault();
        //                //registro de cancelacion

        //                string cadena = "insert into RegistroCancelacionFacturas(IDFactura, Fecha, Usuario) values (" + id + ",SYSDATETIME()," + UserID + ")";
        //                db.Database.ExecuteSqlCommand(cadena);

        //            }
        //            catch (Exception err)
        //            {

        //            }
        //            return Json(new HttpStatusCodeResult(200));
        //        }
        //        else
        //        {
        //            return Json(new HttpStatusCodeResult(500));
        //        }


        //    }
        //    catch (Exception err)
        //    {
        //        return Json(500, err.Message);
        //    }

        //}

        public ActionResult AddendaEnvanses(int id)
        {
            AddendaEnvases elemento = new AddendaEnvases();

            elemento.ID = id;
            elemento.OrdenCompra = "";
            elemento.Albaran = "";
            return View(elemento);
        }





        [HttpPost]
        public ActionResult AddendaEnvanses(AddendaEnvases modelo)
        {
            var elemento = db.encfacturas.Single(m => m.ID == modelo.ID);


            string rutaArchivo = elemento.UUID + ".xml";
            string fileName = System.Web.HttpContext.Current.Server.MapPath("~/Xmlfacturas/" + rutaArchivo);
            string xmlString = System.IO.File.ReadAllText(fileName);


            var stream = new MemoryStream(Encoding.ASCII.GetBytes(xmlString));
            StreamReader reader = new StreamReader(stream);
            string contenidoxml = reader.ReadToEnd();

            try
            {


                Generador.CreaPDF factura = new Generador.CreaPDF(contenidoxml);
                StringBuilder adde = new StringBuilder();
                contenidoxml = contenidoxml.Substring(0, contenidoxml.Length - 19);
                contenidoxml = contenidoxml.Substring(0,38)+ Environment.NewLine + contenidoxml.Substring(38, (contenidoxml.Length - 38));
                contenidoxml.Replace("?>", "?>"+ @Environment.NewLine);
                int posicion = contenidoxml.IndexOf("<cfdi:Emisor");
                contenidoxml=contenidoxml.Substring(0, posicion) + Environment.NewLine +"\t"+ contenidoxml.Substring(posicion, (contenidoxml.Length - posicion)) ;
                posicion = contenidoxml.IndexOf("<cfdi:Receptor");
                contenidoxml = contenidoxml.Substring(0, posicion) + Environment.NewLine+ "\t"  + contenidoxml.Substring(posicion, (contenidoxml.Length - posicion)) ;
                posicion = contenidoxml.IndexOf("<cfdi:Conceptos>");
                contenidoxml = contenidoxml.Substring(0, posicion) + Environment.NewLine + "\t\t" + contenidoxml.Substring(posicion, (contenidoxml.Length - posicion));
                posicion = contenidoxml.IndexOf("</cfdi:Conceptos>");
                contenidoxml = contenidoxml.Substring(0, posicion) + Environment.NewLine + "\t\t" + contenidoxml.Substring(posicion, (contenidoxml.Length - posicion)) ;

                posicion = contenidoxml.IndexOf("<cfdi:Impuestos");
                contenidoxml = contenidoxml.Substring(0, posicion) + Environment.NewLine + "\t\t" + contenidoxml.Substring(posicion, (contenidoxml.Length - posicion)) ;
                posicion = contenidoxml.IndexOf("</cfdi:Impuestos");
                contenidoxml = contenidoxml.Substring(0, posicion) + Environment.NewLine + "\t\t" + contenidoxml.Substring(posicion, (contenidoxml.Length - posicion)) ;

                posicion = contenidoxml.IndexOf("<cfdi:Complemento>");
                contenidoxml = contenidoxml.Substring(0, posicion) + Environment.NewLine + "\t" + contenidoxml.Substring(posicion, (contenidoxml.Length - posicion)) ;
                posicion = contenidoxml.IndexOf("</cfdi:Complemento");
                contenidoxml = contenidoxml.Substring(0, posicion) + Environment.NewLine + "\t" + contenidoxml.Substring(posicion, (contenidoxml.Length - posicion)) ;




                adde.Append(contenidoxml );

                char comilla = (char)34;
                adde.Append("<cfdi:Addenda>\n");
                adde.Append("\t\t<eu:AddendaEU xmlns:eu="+ comilla+"http://factura.envasesuniversales.com/addenda/eu"+comilla+" xsi:schemaLocation="+comilla+ "http://factura.envasesuniversales.com/addenda/eu http://factura.envasesuniversales.com/addenda/eu/EU_Addenda.xsd" + comilla+">\n");
                adde.Append("\t\t\t<eu:TipoFactura>\n");
                adde.Append("\t\t\t<eu:IdFactura>Factura</eu:IdFactura>\n");
                adde.Append("\t\t\t\t<eu:Version>1.0</eu:Version>\n");
                adde.Append("\t\t\t\t<eu:FechaMensaje>" + DateTime.Now.ToString("yyyy-MM-dd") + "</eu:FechaMensaje>\n");
                adde.Append("\t\t\t</eu:TipoFactura>\n");
                adde.Append("\t\t\t<eu:TipoTransaccion>\n");
                adde.Append("\t\t\t\t<eu:IdTransaccion>Con_Pedido</eu:IdTransaccion>\n");
                adde.Append("\t\t\t<eu:Transaccion>" + factura._templatePDF.serie + factura._templatePDF.folio + "</eu:Transaccion>\n");
                adde.Append("\t\t\t</eu:TipoTransaccion>\n");
                adde.Append("\t\t<eu:OrdenesCompra>\n");
                adde.Append("\t\t\t\t<eu:Secuencia consec=" + comilla+"1"+comilla+">\n");

                adde.Append("\t\t\t\t\t<eu:IdPedido>" + modelo.OrdenCompra+ "</eu:IdPedido>\n");
                adde.Append("\t\t\t\t\t<eu:EntradaAlmacen>\n");
                adde.Append("\t\t\t\t\t\t<eu:Albaran>" + modelo.Albaran+ "</eu:Albaran>\n");
                adde.Append("\t\t\t\t\t</eu:EntradaAlmacen>\n");
                adde.Append("\t\t\t\t</eu:Secuencia>\n");
                adde.Append("\t\t</eu:OrdenesCompra>\n");
                adde.Append("\t\t<eu:Moneda>\n");

                adde.Append("\t\t\t<eu:MonedaCve>" + factura._templatePDF.claveMoneda+ "</eu:MonedaCve>\n");

                adde.Append("\t\t\t\t<eu:TipoCambio>" + factura._templatePDF.tipo_cambio+ "</eu:TipoCambio>\n");
                adde.Append("\t\t\t\t<eu:SubtotalM>" + factura._templatePDF.subtotal + "</eu:SubtotalM>\n");
                adde.Append("\t\t\t\t<eu:TotalM>" + factura._templatePDF.total + "</eu:TotalM>\n");
                adde.Append("\t\t\t\t<eu:ImpuestoM>" + factura._templatePDF.totalImpuestosRetenidos + "</eu:ImpuestoM>\n");
                adde.Append("\t\t</eu:Moneda>\n");
                adde.Append("\t\t</eu:AddendaEU>\n");
                adde.Append("\t</cfdi:Addenda>");
                adde.Append("</cfdi:Comprobante>");

                var stream2 = new MemoryStream(Encoding.ASCII.GetBytes(adde.ToString()));

                return File(stream2, "text/plain", "Addenda" + factura._templatePDF.serie + factura._templatePDF.folio + ".xml");
                
            }
            catch (Exception err)
            {
                string MENSAJE = err.Message;
                RedirectToAction("Index"); 
            }
            RedirectToAction("Index");
            return null;
        }
    



        public ActionResult VPagos(int id, List<VEncPagos> enc)
        {
            try
            {

                VEncPagos encFac = db.Database.SqlQuery<VEncPagos>("select ID, Nombre_Cliente, Numero as NoFactura, Total from dbo.EncFacturas where ID = " + id + "").ToList()[0];
            
            ViewBag.Nombre = encFac.Nombre_cliente;
            ViewBag.NoFactura = encFac.NoFactura;
            ViewBag.Total = encFac.Total; 

            }
            catch (Exception err)
            {
                string MENSAJE = err.Message;
                ViewBag.Nombre = "";
                ViewBag.NoFactura = "";
                ViewBag.Total = "";
            }

           

                List<VPagos> elemento = db.Database.SqlQuery<VPagos>("select P.[FechaPago],  D.ImporteSaldoInsoluto, D.importepagado, D.NoParcialidad, D.StatusDocto from dbo.PagoFactura as P right join ([dbo].[DocumentoRelacionado]as D left join [dbo].[PagoFacturaSPEI] as S on D.IDPagoFactura = S.IDPagoFactura) on P.[IDPagoFactura] = D.[IDPagoFactura] where  D.IDFactura = " + id + " order by NoParcialidad ").ToList();
            if (elemento == null)
            {
                return HttpNotFound();
            }
                return View(elemento);
           
        }







        /////////////////////////////////REPORTE POR HORA/////////////////////////////////////////////////////////////////////////////////////

        public ActionResult CreaReporteEstado()
        {

            var EstadoLst = new List<SelectListItem>();

            EstadoLst.Add(new SelectListItem { Text = "Todos", Value = "T" });
            EstadoLst.Add(new SelectListItem { Text = "Activo", Value = "A" });
            EstadoLst.Add(new SelectListItem { Text = "Cancelado", Value = "C" });


            ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");


            return View();
        }



        /////////////////////////////////REPORTE POR CLIENTE/////////////////////////////////////////////////////////////////////////////////////


        public ActionResult CreaReporteporFecha()
        {
            //Buscar Cliente
            ClientesContext dbc = new ClientesContext();
            var cliente = dbc.Clientes.OrderBy(m => m.Nombre).ToList();
            List<SelectListItem> listaCliente = new List<SelectListItem>();
            listaCliente.Add(new SelectListItem { Text = "--Selecciona Cliente--", Value = "0" });

            var EstadoLst = new List<SelectListItem>();
            EstadoLst.Add(new SelectListItem { Text = "Todos", Value = "T" });
            EstadoLst.Add(new SelectListItem { Text = "Activo", Value = "A" });
            EstadoLst.Add(new SelectListItem { Text = "Cancelado", Value = "C" });
            ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");

            //Facturas Pagadas
            var PagoLst = new List<SelectListItem>();
            PagoLst.Add(new SelectListItem { Text = "Todas", Value = "T" });
            PagoLst.Add(new SelectListItem { Text = "Pagada", Value = "SI" });
            PagoLst.Add(new SelectListItem { Text = "NoPagada", Value = "NO" });
            ViewBag.Pagada = new SelectList(PagoLst, "Value", "Text");

            foreach (var m in cliente)
            {
                listaCliente.Add(new SelectListItem { Text = m.Nombre, Value = m.IDCliente.ToString() });
            }
            ViewBag.cliente = listaCliente;
            return View();
        }

      





        //////////////////////////////////MONEDA////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult CreaReporteporMoneda()
        {
            //Buscar 
            c_MonedaContext dbc = new c_MonedaContext();
            var mone = dbc.c_Monedas.OrderBy(m => m.Descripcion).ToList();
            List<SelectListItem> listaMonedas = new List<SelectListItem>();
            listaMonedas.Add(new SelectListItem { Text = "--Selecciona Moneda--", Value = "0" });

            var EstadoLst = new List<SelectListItem>();
            EstadoLst.Add(new SelectListItem { Text = "Todos", Value = "T" });
            EstadoLst.Add(new SelectListItem { Text = "Activo", Value = "A" });
            EstadoLst.Add(new SelectListItem { Text = "Cancelado", Value = "C" });

            //ViewData["Estado"] = EstadoLst;
            ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");

            foreach (var m in mone)
            {
                listaMonedas.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDMoneda.ToString() });
            }
            ViewBag.moneda = listaMonedas;
            return View();
        }


    
        public FileResult DescargaExcel(string Numero, string SerieFac, string ClieFac, string sortOrder, string currentFilter, string Fechainicio, string Fechafinal, string FacPag, string Estado = "A")
        {
            string ConsultaSql = "select * from Encfacturas ";
            string cadenaSQl = string.Empty;
            try
            {

             

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
                if (!String.IsNullOrEmpty(ClieFac) && ClieFac!="Todas")
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

                if (Filtro == "where  Estado='A'")
                {
                    Filtro += " and Fecha >='" + DateTime.Now.AddDays(-30).Year + "-" + DateTime.Now.AddDays(-30).Month + "-" + DateTime.Now.AddDays(-30).Day + "'  and  Fecha<'" + DateTime.Now.AddDays(1).Year + "-" + DateTime.Now.AddDays(1).Month + "-" + DateTime.Now.AddDays(1).Day + "' ";
                   
                }

                //var elementos = from s in db.encfacturas
                //select s;
                cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

                var elementos = db.Database.SqlQuery<Encfacturas>(cadenaSQl).ToList();
                string renglon = string.Empty;
                renglon = "Fecha,Serie,Numero,Nombre de cliente,Subtotal,IVA,Total,Moneda, Tipo de cambio, Esta Pagada,  UUID, Estado, Metodo de pago \n";
                foreach (Encfacturas Ele in elementos)
                {
                    
                    renglon +=Ele.Fecha+","+ Ele.Serie + "," + Ele.Numero + "," + Ele.Nombre_cliente.Replace(","," ") + "," + Ele.Subtotal + "," + Ele.IVA + "," + Ele.Total + "," + Ele.Moneda + "," + Ele.TC + "," + Ele.pagada + "," + Ele.UUID + "," + Ele.Estado + "," + Ele.IDMetododepago + "\n";
                }

                if (renglon!=string.Empty)
                {
                    renglon = renglon.TrimEnd('\n');  // le quita el enter
                }


                var stream = new MemoryStream(Encoding.ASCII.GetBytes(renglon));

                return File(stream, "text/plain", "Listado de Facturas.csv");

                //Response.Clear();
                //Response.AddHeader("Content-Disposition", "attachment; filename=adressenbestand.csv");
                //Response.ContentType = "text/csv";
                //Response.Write(renglon);
                //Response.End();

                //return new HttpStatusCodeResult(HttpStatusCode.OK);

            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                string renglon = "Tu consulta causo un problema o no presionaste el boton aplicar antes de este boton\n " + ConsultaSql +"\n" + err.Message;

             //   var filresult = File(new System.Text.UTF8Encoding().GetBytes(renglon), "application/csv", "downloaddocuments.csv");
                // return filresult;

                var stream = new MemoryStream(Encoding.ASCII.GetBytes(renglon));

                return File(stream, "text/plain", "Listado de Factura.csv");
             


            }
        }


        public FileResult DescargaPoliza(string Numero, string SerieFac, string ClieFac, string sortOrder, string currentFilter, string Fechainicio, string Fechafinal, string FacPag, string Estado = "A")
        {
            string ConsultaSql = "select * from Encfacturas ";
            string cadenaSQl = string.Empty;
            try
            {



                string Filtro = string.Empty;



                string Orden = " order by fecha desc , serie , numero desc ";
                if (SerieFac != "Todas")
                {
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
                            Filtro = "where Serie=' '";
                        }
                        else
                        {
                            Filtro += "and  Serie=' '";
                        }

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
                if (!String.IsNullOrEmpty(ClieFac) && ClieFac != "Todas")
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


                if (Estado != "Todas")
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

                if (Filtro == "where  Estado='A'")
                {
                    Filtro += " and Fecha >='" + DateTime.Now.AddDays(-30).Year + "-" + DateTime.Now.AddDays(-30).Month + "-" + DateTime.Now.AddDays(-30).Day + "'  and  Fecha<'" + DateTime.Now.AddDays(1).Year + "-" + DateTime.Now.AddDays(1).Month + "-" + DateTime.Now.AddDays(1).Day + "' ";

                }


                cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

                var elementos = db.Database.SqlQuery<Encfacturas>(cadenaSQl).ToList();

                StringBuilder renglon = new StringBuilder();
                Char comillas = new Char();
                comillas = System.Char.Parse("\""); //comillas ;


                string cuentaVentas = string.Empty;
                string VentasConcepto = string.Empty;
                string cuentaiva16 = string.Empty;
                string conceptoiva16 = string.Empty;
                string VersionCOI = string.Empty;

                try
                {
                    XmlDocument xm = new XmlDocument();
                    xm.Load(System.Web.HttpContext.Current.Server.MapPath("~/configCuentas.xml"));


                    XmlNode cuentasnode = xm.FirstChild;
                    try
                    {
                        foreach (XmlNode cuentas in cuentasnode.ChildNodes)
                        {

                            if (cuentas.Name == "VentasCuenta")
                            {
                                cuentaVentas = cuentas.InnerText;
                            }
                            if (cuentas.Name == "VentasConcepto")
                            {
                                VentasConcepto = cuentas.InnerText;
                            }
                            if (cuentas.Name == "IVACuenta16")
                            {
                                cuentaiva16 = cuentas.InnerText;
                            }
                            if (cuentas.Name == "IVAConcepto16")
                            {
                                conceptoiva16 = cuentas.InnerText;
                            }

                            if (cuentas.Name == "versionCOI")
                            {
                                VersionCOI = cuentas.InnerText;
                            }


                        }
                    }
                    catch (Exception err)
                    {
                        throw new Exception("Hay un error con una configuracion de cuentas Contables " + err.Message);
                    }
                }

                catch (Exception err)
                {
                    throw new Exception("Hay un error con tu archivo de configuracion de cuentas Contables " + err.Message);
                }

                decimal acusubtotal = 0;
                decimal acuiva = 0;
                decimal acutotal = 0;

                renglon.Append("<?xml version=" + comillas + "1.0" + comillas + " encoding=" + comillas + "utf-8" + comillas + " standalone=" + comillas + "yes" + comillas + "?>\n");
                renglon.Append("<DATAPACKET Version=" + comillas + "2.0" + comillas + ">\n");
                renglon.Append("<METADATA>\n");
                renglon.Append("<FIELDS>\n");
                renglon.Append("<FIELD attrname=" + comillas + "VersionCOI" + comillas + " fieldtype=" + comillas + "i2" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "TipoPoliz" + comillas + " fieldtype=" + comillas + "string" + comillas + " WIDTH=" + comillas + "2" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "DiaPoliz" + comillas + " fieldtype=" + comillas + "string" + comillas + " WIDTH=" + comillas + "2" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "ConcepPoliz" + comillas + " fieldtype=" + comillas + "string" + comillas + " WIDTH=" + comillas + "120" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "Partidas" + comillas + " fieldtype=" + comillas + "nested" + comillas + ">\n");
                renglon.Append("<FIELDS>\n");
                renglon.Append("<FIELD attrname=" + comillas + "Cuenta" + comillas + " fieldtype=" + comillas + "string" + comillas + " WIDTH=" + comillas + "21" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "Depto" + comillas + " fieldtype=" + comillas + "i4" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "ConceptoPol" + comillas + " fieldtype=" + comillas + "string" + comillas + " WIDTH=" + comillas + "120" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "Monto" + comillas + " fieldtype=" + comillas + "r8" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "TipoCambio" + comillas + " fieldtype=" + comillas + "r8" + comillas + " />\n");
                renglon.Append("<FIELD attrname=" + comillas + "DebeHaber" + comillas + " fieldtype=" + comillas + "string" + comillas + " WIDTH=" + comillas + "1" + comillas + " />\n");
                renglon.Append("</FIELDS>\n");
                renglon.Append("<PARAMS />\n");
                renglon.Append("</FIELD>\n");
                renglon.Append("</FIELDS>\n");
                renglon.Append("<PARAMS />\n");
                renglon.Append("</METADATA>\n");
                renglon.Append("<ROWDATA>\n");
                renglon.Append("<ROW VersionCOI=" + comillas + VersionCOI + comillas + " TipoPoliz=" + comillas + "Dr" + comillas + " DiaPoliz=" + comillas + "1" + comillas + " ConcepPoliz=" + comillas + "Poliza de Ventas" + comillas + ">\n");
                renglon.Append("<Partidas>\n");

                string TIPO = "D";
                foreach (Encfacturas Ele in elementos)
                {
                    try
                    {
                        if (Ele.IDCliente == 0)
                        {
                            throw new Exception("El Cliente " + Ele.Nombre_cliente + " no tiene correspondencia en el Siaapi ");
                        }



                        decimal Monto = Ele.Total;
                        Clientes cliente = new ClientesContext().Clientes.Find(Ele.IDCliente);

                        string cuenta = cliente.cuentaContable;

                        if (cliente.cuentaContable == string.Empty)
                        {
                            throw new Exception("El Cliente " + Ele.Nombre_cliente + " no tiene cuenta contable registrada ");
                        }
                        String REF = "";

                        try
                        {

                            REF = "FACTURA " + Ele.Serie + " " + Ele.Numero + "  " + Ele.Nombre_cliente.Substring(0, 20);
                        }
                        catch (Exception ERR)

                        {
                            REF = "FACTURA " + Ele.Serie + " " + Ele.Numero + "  " + Ele.Nombre_cliente;


                        }
                        if (Ele.Estado == "A")
                        {
                            if (Ele.Moneda == "MXN")
                            {
                                acusubtotal = acusubtotal + Ele.Subtotal;
                                acuiva = acuiva + Ele.IVA;
                                acutotal = acutotal + Ele.Total;

                            }
                            else
                            {
                                acusubtotal = acusubtotal + Math.Round(Ele.Subtotal * Ele.TC, 2);
                                acuiva = acuiva + Math.Round(Ele.IVA * Ele.TC, 2);
                                acutotal = acutotal + Math.Round(Ele.Total * Ele.TC, 2);
                                Monto = Math.Round(Ele.Total * Ele.TC, 2);
                            }
                            renglon.Append("<ROWPartidas Cuenta=" + comillas + cuenta + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas + REF + comillas + " Monto=" + comillas + Monto + comillas + " TipoCambio=" + comillas + Ele.TC + comillas + " DebeHaber=" + comillas + TIPO + comillas + " />\n");
                        }
                        else
                        {
                            REF = REF + " CANCELADA";
                            Monto = 0;
                        }

                    }
                    catch (Exception err)
                    {
                        string mensaje = err.Message;
                        string rengloncliente = "Error : " + err.Message;

                        var streamcliente = new MemoryStream(Encoding.UTF8.GetBytes(rengloncliente));

                        return File(streamcliente, "text/plain", "Error.txt");
                    }




                }


                renglon.Append("<ROWPartidas Cuenta=" + comillas + cuentaVentas + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas + VentasConcepto + comillas + " Monto=" + comillas + acusubtotal + comillas + " TipoCambio=" + comillas + 1 + comillas + " DebeHaber=" + comillas + "H" + comillas + " />\n");
                renglon.Append("<ROWPartidas Cuenta=" + comillas + cuentaiva16 + comillas + " Depto=" + comillas + "0" + comillas + " ConceptoPol=" + comillas + conceptoiva16 + comillas + " Monto=" + comillas + acuiva + comillas + " TipoCambio=" + comillas + 1 + comillas + " DebeHaber=" + comillas + "H" + comillas + " />\n");

                renglon.Append("</Partidas>\n");
                renglon.Append("</ROW>\n");
                renglon.Append("</ROWDATA>\n");
                renglon.Append("</DATAPACKET>\n");


                var stream = new MemoryStream(Encoding.UTF8.GetBytes(renglon.ToString()));

                return File(stream, "text/plain", "PolizaFacturacion.Pol");


            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                string renglon = "Error : " + err.Message;

                var stream = new MemoryStream(Encoding.UTF8.GetBytes(renglon));

                return File(stream, "text/plain", "Error.txt");



            }
        }

        public ActionResult CreaXmlFacturas()
        {
            DateTime fechax = DateTime.Parse("2020-01-01");
            var facturas = new EncfacturaContext().encfacturas.Where(s=> s.Fecha>=fechax).ToList();
            try
            {
                foreach (Encfacturas factura in facturas)
                {
                    if (factura.RutaXML.Length > 1)
                    {
                        Generador.CreaPDF temp = new Generador.CreaPDF(factura.RutaXML.ToString());
                       string fileName = temp._templatePDF.folioFiscalUUID + ".xml";

                        EscribeEnArchivo(factura.RutaXML, fileName, true);
                    }
                }
                ViewBag.Error = "Termine";
            }
            catch (Exception err)
            {
                ViewBag.Error = "Este error ocurrio" + err.Message;
            }

            return View();

        }

        public ActionResult CreaXmlFacturasT()
        {
           // DateTime fechax = DateTime.Parse("2019-08-01");
            var facturas = new EncfacturaContext().encfacturas.ToList();
            try
            {
                foreach (Encfacturas factura in facturas)
                {
                    if( factura.RutaXML.Length > 1)
                   { Generador.CreaPDF temp = new Generador.CreaPDF(factura.RutaXML.ToString());
                        string fileName = temp._templatePDF.folioFiscalUUID + ".xml";

                        EscribeEnArchivo(factura.RutaXML, fileName, true);
                    }
                }
                ViewBag.Error = "Termine";
            }
            catch (Exception err)
            {
                ViewBag.Error = "Este error ocurrio" + err.Message;
            }

            return View();

        }

        public static void EscribeEnArchivo(string contenido, string rutaArchivo, bool sobrescribir = true)
        {
            string archivoxml = System.Web.HttpContext.Current.Server.MapPath("~/Xmlfacturas/"+ rutaArchivo);

            using (FileStream fs = System.IO.File.Create(archivoxml))
            {
                AddText(fs, contenido);
            }

        }


        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        public ActionResult reporteSaldos()
        {
            List<ClientesFacturas> clientes = new List<ClientesFacturas>();
            string cadena = "select C.IDCliente, C.Nombre from Clientes as c inner join tempClienteSaldo as t on c.IDCliente=t.IDCliente order by c.Nombre";
            clientes = db.Database.SqlQuery<ClientesFacturas>(cadena).ToList();
            List<SelectListItem> listacliente = new List<SelectListItem>();
            listacliente.Add(new SelectListItem { Text = "--Selecciona Cliente--", Value = "0" });

            foreach (var m in clientes)
            {
                listacliente.Add(new SelectListItem { Text = m.Nombre, Value = m.IDCliente.ToString() });
            }
            ViewBag.cliente = listacliente;
            return View();
        }


        [HttpPost]
        public ActionResult reporteSaldos(ReporteSaldoInsoluto r, ClientesFacturas C)
        {
            int idcliente = C.IDCliente;
            try
            {

                ClientesContext dbc = new ClientesContext();
                Clientes cls = dbc.Clientes.Find(C.IDCliente);
            }
            catch (Exception ERR)
            {

            }

            try
            {
                string cadena = "drop table [dbo].[tempClienteSaldo]";

                db.Database.ExecuteSqlCommand(cadena);
            }
            catch (Exception err)
            {
                string mensajeerror = err.Message;
            }
            string cadena1 = "CREATE TABLE [dbo].[tempClienteSaldo]([IDCliente] [int] NULL) ON [PRIMARY]";
            db.Database.ExecuteSqlCommand(cadena1);
            string cadena2 = "insert into [tempClienteSaldo] select distinct e.IDCliente from EncFacturas as e inner join SaldoFactura as s on e.ID=s.IDFactura where s.ImporteSaldoInsoluto>0";
            db.Database.ExecuteSqlCommand(cadena2);


            ReporteSaldoInsoluto report = new ReporteSaldoInsoluto();
            byte[] abytes = report.PrepareReport(idcliente);
            return File(abytes, "application/pdf");
        }

        

        public ActionResult AntiguedadSaldos()
        {
            List<Clientes> clientes = new List<Clientes>();
            string cadena = "select * from Clientes order by Nombre";
            clientes = db.Database.SqlQuery<Clientes>(cadena).ToList();
            List<SelectListItem> listacliente = new List<SelectListItem>();
            listacliente.Add(new SelectListItem { Text = "--Selecciona Cliente--", Value = "0" });

            foreach (var m in clientes)
            {
                listacliente.Add(new SelectListItem { Text = m.Nombre, Value = m.IDCliente.ToString() });
            }
            ViewBag.cliente = listacliente;




            return View();
        }

        [HttpPost]
        public ActionResult AntiguedadSaldos(AntiguedadSaldos r, ClientesFacturasA C, FormCollection coleccion)
        {
            int idcliente = C.IDCliente;

            string cual = coleccion.Get("Enviar");
            string cadena = "";

            if (cual == "Generar reporte")
            {
                try
                {
                    ClientesContext dbc = new ClientesContext();
                    Clientes cls = dbc.Clientes.Find(C.IDCliente);
                }
                catch (Exception ERR)
                {

                }


                try
                {
                    string cadena21 = "drop table [dbo].[tempClienteSaldo]";

                    db.Database.ExecuteSqlCommand(cadena21);
                }
                catch (Exception err)
                {
                    string mensajeerror = err.Message;
                }
                string cadena1 = "CREATE TABLE [dbo].[tempClienteSaldo]([IDCliente] [int] NULL) ON [PRIMARY]";
                db.Database.ExecuteSqlCommand(cadena1);
                string cadena2 = "insert into [tempClienteSaldo] select distinct e.IDCliente from EncFacturas as e inner join SaldoFactura as s on e.ID=s.IDFactura where s.ImporteSaldoInsoluto>0";
                db.Database.ExecuteSqlCommand(cadena2);

                AntiguedadSaldos report1 = new AntiguedadSaldos();
                byte[] abytes1 = report1.PrepareReport(idcliente);
                return File(abytes1, "application/pdf");
            }
            else if (cual == "Generar Excel")
            {
                if (idcliente == 0)
                {
                    cadena = "Select ID, Serie, Numero, Fecha, Nombre_Cliente,  Total, Moneda, TC, ImportePagado,  FechaVencimiento, ImporteSaldoInsoluto, FechaRevision, oficina, Vendedor, noExpediente from[dbo].[EncFacturaOfVen]   Where Estado = 'A'  and ImporteSaldoInsoluto> 0 Order by Nombre_Cliente Asc, Numero desc";
                }
                else
                {
                    cadena = "Select ID, Serie, Numero, Fecha, Nombre_Cliente, Total, Moneda, TC, ImportePagado,  FechaVencimiento, ImporteSaldoInsoluto, FechaRevision, oficina, Vendedor, noExpediente from[dbo].[EncFacturaOfVen]   Where Estado = 'A'  and ImporteSaldoInsoluto> 0 and  IDCliente = " + idcliente + " Order by Nombre_Cliente Asc, Numero desc";
                }


                var facturassaldos = dbs.Database.SqlQuery<EncFacturaOfVen>(cadena).ToList();
                ViewBag.facturassaldos = facturassaldos;
                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Antiguedad Saldos");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:U1"].Style.Font.Size = 20;
                Sheet.Cells["A1:U1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:U3"].Style.Font.Bold = true;
                Sheet.Cells["A1:U1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:U1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Antiguedad de Saldos");

                row = 2;
                Sheet.Cells["A1:U1"].Style.Font.Size = 12;
                Sheet.Cells["A1:U1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:U1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:U2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                string Fec = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = Fec;
                //Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                //Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                //Sheet.Cells[string.Format("E2", row)].Value = FF;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:X3"].Style.Font.Bold = true;
                Sheet.Cells["A3:X3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:X3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.

                Sheet.Cells["A3"].RichText.Add("Serie");
                Sheet.Cells["B3"].RichText.Add("Numero");
                Sheet.Cells["C3"].RichText.Add("Fecha");
                Sheet.Cells["D3"].RichText.Add("No.Expediente");
                Sheet.Cells["E3"].RichText.Add("Cliente");
                Sheet.Cells["F3"].RichText.Add("Subtotal");
                Sheet.Cells["G3"].RichText.Add("Iva");
                Sheet.Cells["H3"].RichText.Add("Total");
                Sheet.Cells["I3"].RichText.Add("Moneda");
                Sheet.Cells["J3"].RichText.Add("Tipo de Cambio");
                Sheet.Cells["K3"].RichText.Add("Importe Pagado");
                Sheet.Cells["L3"].RichText.Add("Al Corriente");
                Sheet.Cells["M3"].RichText.Add("0-7 días");
                Sheet.Cells["N3"].RichText.Add("8-14 días");
                Sheet.Cells["O3"].RichText.Add("14-21 días");
                Sheet.Cells["P3"].RichText.Add("22-30 días");
                Sheet.Cells["Q3"].RichText.Add("31-60 días");
                Sheet.Cells["R3"].RichText.Add("61-90 días");
                Sheet.Cells["S3"].RichText.Add("91 o mas");
                //Sheet.Cells["M3"].RichText.Add("Saldo Insoluto");
                Sheet.Cells["T3"].RichText.Add("Fecha de Revisión");
                Sheet.Cells["U3"].RichText.Add("Fecha de Vencimiento");
                Sheet.Cells["V3"].RichText.Add("Oficina");
                Sheet.Cells["W3"].RichText.Add("Vendedor");
              
                //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
                //Sheet.Cells["A3"].Value = "Serie";
                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:X3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (EncFacturaOfVen item in ViewBag.facturassaldos)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.Serie;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Numero;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                    if (item.NoExpediente == null)
                    {
                        Sheet.Cells[string.Format("D{0}", row)].Value = "S/N";
                    }
                    else
                    {
                        Sheet.Cells[string.Format("D{0}", row)].Value = item.NoExpediente;
                    }
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Nombre_cliente;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Subtotal;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.IVA;

                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.Moneda;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "#,##0.00";
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.TC;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.ImportePagado;
                    ClsOperacionesFecha fechas = new ClsOperacionesFecha();
                    try
                    {
                        fechas.fechaini = DateTime.Parse(item.FechaVencimiento.ToString());
                        fechas.fechafin = DateTime.Now;
                    }
                    catch (Exception err)
                    {

                    }
                   
                    if (fechas.getcorriente())
                    {
                        Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("L{0}", row)].Value = item.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("L{0}", row)].Value = 0;
                    }

                    if (fechas.get7())
                    {

                        Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("M{0}", row)].Value = item.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("M{0}", row)].Value = 0;
                    }

                    if (fechas.get814())
                    {

                        Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("N{0}", row)].Value = item.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("N{0}", row)].Value = 0;
                    }

                    if (fechas.get1421())
                    {

                        Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("O{0}", row)].Value = item.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("O{0}", row)].Value = 0;
                    }

                    if (fechas.get2230())
                    {

                        Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("P{0}", row)].Value = item.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("P{0}", row)].Value = 0;
                    }

                    

                    if (fechas.get3160())
                    {
                        Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("Q{0}", row)].Value = item.ImporteSaldoInsoluto;

                    }
                    else
                    {
                        Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("Q{0}", row)].Value = 0;
                    }

                    if (fechas.get6190())
                    {
                        Sheet.Cells[string.Format("R{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("R{0}", row)].Value = item.ImporteSaldoInsoluto;

                    }
                    else
                    {
                        Sheet.Cells[string.Format("R{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("R{0}", row)].Value = 0;
                    }

                    if (fechas.get91mas())
                    {
                        Sheet.Cells[string.Format("S{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("S{0}", row)].Value = item.ImporteSaldoInsoluto;
                    }
                    else
                    {
                        Sheet.Cells[string.Format("S{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("S{0}", row)].Value = 0;
                    }
                    //Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    //Sheet.Cells[string.Format("L{0}", row)].Value = item.ImporteSaldoInsoluto;
                    Sheet.Cells[string.Format("T{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("T{0}", row)].Value = item.FechaRevision;
                    Sheet.Cells[string.Format("U{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("U{0}", row)].Value = item.FechaVencimiento;
                    Sheet.Cells[string.Format("V{0}", row)].Value = item.Oficina;
                    Sheet.Cells[string.Format("W{0}", row)].Value = item.Vendedor;
                  

                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelFacturasCliente.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");



            AntiguedadSaldos report = new AntiguedadSaldos();
            byte[] abytes = report.PrepareReport(idcliente);
            return File(abytes, "application/pdf");
        }

        public ActionResult AntiguedadSaldosV()
        {
            List<Vendedor> ven = new List<Vendedor>();
            string cadena = "select  * from Vendedor order by Vendedor.Nombre";
            ven = db.Database.SqlQuery<Vendedor>(cadena).ToList();
            List<SelectListItem> listaven = new List<SelectListItem>();
            listaven.Add(new SelectListItem { Text = "--Selecciona Vendedor--", Value = "0" });

            foreach (var m in ven)
            {
                listaven.Add(new SelectListItem { Text = m.Nombre, Value = m.IDVendedor.ToString() });
            }
            ViewBag.vendedor = listaven;




            return View();
        }

        [HttpPost]
        public ActionResult AntiguedadSaldosV(AntiguedadSaldosVen r, ClientesFacturasAV C)
        {
            int idvendedor = C.IDVendedor;
            try
            {

                VendedorContext dbc = new VendedorContext();
                Vendedor cls = dbc.Vendedores.Find(C.IDVendedor);
            }
            catch (Exception ERR)
            {

            }



            AntiguedadSaldosVen report = new AntiguedadSaldosVen();
            byte[] abytes = report.PrepareReport(idvendedor);
            return File(abytes, "application/pdf");
        }
 		public ActionResult FechaRevision(int id)
        {
            var elemento = db.encfacturas.Single(m => m.ID == id);
            return View(elemento);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult FechaRevision(Encfacturas encF, FormCollection collection, int id)
        {
            encF.FechaRevision = DateTime.Parse(collection.Get("FechaRevision"));

            DateTime FechaRe = Convert.ToDateTime(encF.FechaRevision);
           
            try
            {
                
                var elemento = db.encfacturas.Single(m => m.ID == id);
               
                

                ClsDatoEntero contarfac = db.Database.SqlQuery<ClsDatoEntero>("select EncPrefactura.idCondicionesPago as dato from EncFacturas inner join EncPrefactura on EncFacturas.numero=EncPrefactura.numero and EncFacturas.serie=[EncPrefactura].serie and EncPrefactura.IDCondicionesPago <> 10 and EncPrefactura.IDCondicionesPago <>1 and EncPrefactura.IDCondicionesPago <>2  where EncFacturas.id=" + id).ToList().FirstOrDefault();
                int conPago = contarfac.Dato;
                string d = "select DiasCredito as dato from [CondicionesPago]  where idCondicionesPago=" + conPago;
                    ClsDatoDecimal DIASCRE = db.Database.SqlQuery<ClsDatoDecimal>(d).ToList().FirstOrDefault();
                decimal diasCredito = DIASCRE.Dato;
               

                DateTime FechaVen = FechaRe.AddDays(Convert.ToInt32(diasCredito));

                string cadena = "update EncFacturas set FechaRevision='" + FechaRe.Year + "-" + FechaRe.Month + "-" + FechaRe.Day + "', FechaVencimiento='"+ FechaVen.Year + "-" + FechaVen.Month + "-" + FechaVen.Day + "' where ID=" + id;
                db.Database.ExecuteSqlCommand(cadena);





            }
            catch (Exception err)
            {
                return View("index");

            }

            

            return View();

        }


        public void GenerarExcelFacturas(int num)
        {

            //Generar el listado de datos
            if (num == 1)
            {
                //var facturassaldos = dbs.EncfacturasSaldos.ToList();
                //Todas las facturas
                var facturassaldos = dbs.Database.SqlQuery<VEncfacturasSaldosVen>("select * from [dbo].[VEncfacturasSaldosVen]").ToList();
                ViewBag.facturassaldos = facturassaldos;
            }
            if (num == 12)
            {
                ////Facturas de los ultimos 12 meses
                DateTime fecha = DateTime.Parse(DateTime.Now.Year + "-" + DateTime.Now.Month + "-01").AddMonths(-12);

                var facturassaldos = dbs.Database.SqlQuery<VEncfacturasSaldosVen>("select * from [dbo].[VEncfacturasSaldosVen] where Fecha >= '" + fecha.Year + "-" + fecha.Month + " -01'").ToList();
                ViewBag.facturassaldos = facturassaldos;
            }
            if (num == 6)
            {
                //Facturas de los ultimos 6 meses

                DateTime fecha = DateTime.Parse(DateTime.Now.Year + "-" + DateTime.Now.Month + "-01").AddMonths(-6);
                var facturassaldos = dbs.Database.SqlQuery<VEncfacturasSaldosVen>("select * from [dbo].[VEncfacturasSaldosVen] where Fecha >= '" + fecha.Year + "-" + fecha.Month + " -01'").ToList();
                ViewBag.facturassaldos = facturassaldos;
            }
            if (num == 3)
            {
                //Facturas de los ultimos 9 meses
                DateTime fecha = DateTime.Parse(DateTime.Now.Year + "-" + DateTime.Now.Month + "-01").AddMonths(-3);
                var facturassaldos = dbs.Database.SqlQuery<VEncfacturasSaldosVen>("select * from [dbo].[VEncfacturasSaldosVen] where Fecha >= '" + fecha.Year + "-" + fecha.Month + " -01'").ToList();
                ViewBag.facturassaldos = facturassaldos;
            }
            ExcelPackage Ep = new ExcelPackage();
            //Crear la hoja y poner el nombre de la pestaña del libro
            var Sheet = Ep.Workbook.Worksheets.Add("Facturas");

            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:V1"].Style.Font.Size = 20;
            Sheet.Cells["A1:V1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:V3"].Style.Font.Bold = true;
            Sheet.Cells["A1:V1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:V1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Facturas");

            row = 2;
            Sheet.Cells["A1:V1"].Style.Font.Size = 12;
            Sheet.Cells["A1:V1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:V1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A2:V2"].Style.Font.Bold = true;
            //Subtitulo según el filtrado de datos
            if (num == 1)
            {
                Sheet.Cells["A2"].RichText.Add("Listado de todas las Facturas");
            }
            if (num == 12)
            {
                Sheet.Cells["A2"].RichText.Add("Listado de las Facturas de los 12 últimos meses");
            }
            if (num == 6)
            {
                Sheet.Cells["A2"].RichText.Add("Listado de las Facturas de los 6 últimos meses");
            }
            if (num == 3)
            {
                Sheet.Cells["A2"].RichText.Add("Listado de las Facturas de los 3 últimos meses");
            }

            //En la fila3 se da el formato a el encabezado
            row = 3;
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 10;
            Sheet.Cells["A3:V3"].Style.Font.Bold = true;
            Sheet.Cells["A3:V3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A3:V3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            // Se establece el nombre que identifica a cada una de las columnas de datos.
            Sheet.Cells["A3"].RichText.Add("Serie");
            Sheet.Cells["B3"].RichText.Add("Numero");
            Sheet.Cells["C3"].RichText.Add("Fecha");
            Sheet.Cells["D3"].RichText.Add("Cliente");
            Sheet.Cells["E3"].RichText.Add("Subtotal");
            Sheet.Cells["F3"].RichText.Add("Iva");
            Sheet.Cells["G3"].RichText.Add("Total");
            Sheet.Cells["H3"].RichText.Add("Moneda");
            Sheet.Cells["I3"].RichText.Add("Tipo de Cambio Factura");
            Sheet.Cells["J3"].RichText.Add("Tipo de Cambio Del Día");
            Sheet.Cells["K3"].RichText.Add("Metodo de pago");
            Sheet.Cells["L3"].RichText.Add("UUID");
            Sheet.Cells["M3"].RichText.Add("Estado");
            Sheet.Cells["N3"].RichText.Add("Pagada");
            Sheet.Cells["O3"].RichText.Add("Importe Pagado");
            Sheet.Cells["P3"].RichText.Add("Saldo Insoluto");
            Sheet.Cells["Q3"].RichText.Add("Fecha de Revisión");
            Sheet.Cells["R3"].RichText.Add("Fecha de Vencimiento");
            Sheet.Cells["S3"].RichText.Add("Dias Vencidos");
            Sheet.Cells["T3"].RichText.Add("Telefono");
            Sheet.Cells["U3"].RichText.Add("Vendedor");
            Sheet.Cells["V3"].RichText.Add("Oficina");
            //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
            //Sheet.Cells["A3"].Value = "Serie";

            //Aplicar borde doble al rango de celdas A3:Q3
            Sheet.Cells["A3:S3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
            // Se establecen los formatos para las celdas: Fecha, Moneda
            row = 4;
            Sheet.Cells.Style.Font.Bold = false;
            foreach (var item in ViewBag.facturassaldos)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.Serie;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Numero;
                Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Nombre_cliente;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Subtotal;
                Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("F{0}", row)].Value = item.IVA;
                Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Moneda;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.TC;
                string fecfac = item.Fecha.Year.ToString() + "-" + item.Fecha.Month.ToString() + "-" + item.Fecha.Day.ToString();
                decimal tcd = db.Database.SqlQuery<ClsDatoDecimal>("select [dbo].[GetTipocambioCadena]('" + fecfac + "','USD', 'MXN') as Dato").ToList().FirstOrDefault().Dato;
                Sheet.Cells[string.Format("J{0}", row)].Value = tcd;

                Sheet.Cells[string.Format("K{0}", row)].Value = item.IDMetododepago;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.UUID;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.Estado;
                //Como item.pagada es de tipo booleano (verdadero o falso), se imprime en pantalla (si o no), según corresponda.
                if (item.pagada == false)
                {
                    Sheet.Cells[string.Format("N{0}", row)].Value = "No";
                }
                else
                {
                    Sheet.Cells[string.Format("N{0}", row)].Value = "Si";
                }
                Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("O{0}", row)].Value = item.ImportePagado;
                Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("P{0}", row)].Value = item.ImporteSaldoInsoluto;

                Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.FechaRevision;
                Sheet.Cells[string.Format("R{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("R{0}", row)].Value = item.FechaVencimiento;
                ClsOperacionesFecha fecha = new ClsOperacionesFecha();
                fecha.fechaini = item.FechaVencimiento;
                fecha.fechafin = DateTime.Now;
                int diasvencidos = fecha.getDiferencia();
                if (item.ImporteSaldoInsoluto < 0.10M)
                { diasvencidos = 0; }

                Sheet.Cells[string.Format("S{0}", row)].Value = diasvencidos;
                Sheet.Cells[string.Format("T{0}", row)].Value = item.Telefono;
                Sheet.Cells[string.Format("U{0}", row)].Value = item.Vendedor;
                Sheet.Cells[string.Format("V{0}", row)].Value = item.NombreOficina;
                row++;
            }
            //Se genera el archivo y se descarga

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelFacturas.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult EntreFechasLFac()
        {
            fechasReporteFac elemento = new fechasReporteFac();


            return View(elemento);
        }


        [HttpPost]
        public ActionResult EntreFechasLFac(ReporteListadoF modelo, FormCollection coleccion)
        {
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {
                ReporteListadoF report = new ReporteListadoF();
                //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"),DateTime.Parse( "2019-07-30"));
                byte[] abytes = report.PrepareReport(modelo.fechaini, modelo.fechafin);
                return File(abytes, "application/pdf", "ReporteSaldoFactura.pdf");
            }
            else
            {
                EncFacturaOfVenContext dbe = new EncFacturaOfVenContext();
                string cadena = "select * from [dbo].[EncFacturaOfVen] where Fecha >= '" + FI + "' and Fecha <='" + FF + "'";
                var facturassaldos = dbe.Database.SqlQuery<EncFacturaOfVen>(cadena).ToList();
                ViewBag.facturassaldos = facturassaldos;
                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Facturas");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:X1"].Style.Font.Size = 20;
                Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:X3"].Style.Font.Bold = true;
                Sheet.Cells["A1:X1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:X1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Facturas");

                row = 2;
                Sheet.Cells["A1:X1"].Style.Font.Size = 12;
                Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:X1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:X2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:X3"].Style.Font.Bold = true;
                Sheet.Cells["A3:X3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:X3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Serie");
                Sheet.Cells["B3"].RichText.Add("Numero");
                Sheet.Cells["C3"].RichText.Add("Fecha");
                Sheet.Cells["D3"].RichText.Add("Cliente");
                Sheet.Cells["E3"].RichText.Add("Subtotal");
                Sheet.Cells["F3"].RichText.Add("Iva");
                Sheet.Cells["G3"].RichText.Add("Total");
                Sheet.Cells["H3"].RichText.Add("Moneda");
                Sheet.Cells["I3"].RichText.Add("Tipo de Cambio");
                Sheet.Cells["J3"].RichText.Add("Metodo de pago");
                Sheet.Cells["K3"].RichText.Add("UUID");
                Sheet.Cells["L3"].RichText.Add("Estado");
                Sheet.Cells["M3"].RichText.Add("Pagada");
                Sheet.Cells["N3"].RichText.Add("Importe Pagado");
                Sheet.Cells["O3"].RichText.Add("Saldo Insoluto");
                Sheet.Cells["P3"].RichText.Add("Fecha de Revisión");
                Sheet.Cells["Q3"].RichText.Add("Fecha de Vencimiento");
              
                Sheet.Cells["R3"].RichText.Add("IDOficina");
                Sheet.Cells["S3"].RichText.Add("Oficina");
                Sheet.Cells["T3"].RichText.Add("IDVendedor");
                Sheet.Cells["U3"].RichText.Add("Vendedor");
                Sheet.Cells["V3"].RichText.Add("TipoCliente");
                //Sheet.Cells["W3"].RichText.Add("Fecha Alta");



                //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
                //Sheet.Cells["A3"].Value = "Serie";

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:X3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (var item in ViewBag.facturassaldos)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.Serie;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Numero;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Nombre_cliente;
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Subtotal;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.IVA;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Moneda;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.TC;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.IDMetododepago;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.UUID;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Estado;
                    //Como item.pagada es de tipo booleano (verdadero o falso), se imprime en pantalla (si o no), según corresponda.
                    if (item.pagada == false)
                    {
                        Sheet.Cells[string.Format("M{0}", row)].Value = "No";
                    }
                    else
                    {
                        Sheet.Cells[string.Format("M{0}", row)].Value = "Si";
                    }
                    Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.ImportePagado;
                    Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.ImporteSaldoInsoluto;

                    Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.FechaRevision;
                    Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("Q{0}", row)].Value = item.FechaVencimiento;
             
                    Sheet.Cells[string.Format("R{0}", row)].Value = item.IDOficina;
                    Sheet.Cells[string.Format("S{0}", row)].Value = item.Oficina;
                    Sheet.Cells[string.Format("T{0}", row)].Value = item.IDVendedor;
                    Sheet.Cells[string.Format("U{0}", row)].Value = item.Vendedor;


                    Sheet.Cells[string.Format("V{0}", row)].Value = item.TipoCliente;

                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelFacturas.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            //return Redirect("index");
        }

        public ActionResult EntreFechasLFacClie()
        {
            fechasReporteFacClie elemento = new fechasReporteFacClie();
            ClientesContext dbc = new ClientesContext();
            //ViewBag.idcliente = new SelectList(dbc.Clientes, "IDCliente", "Nombre");
            ViewBag.idcliente = new ClienteAllRepository().GetClientes();
            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasLFacClie(ReporteListadoFClie modelo, FormCollection coleccion)
        {
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();
            int idClie = modelo.idcliente;
            string cual = coleccion.Get("Enviar");
            string cadena = "";
            if (cual == "Generar reporte")
            {
                ReporteListadoFClie report = new ReporteListadoFClie();
                //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"),DateTime.Parse( "2019-07-30"));
                byte[] abytes = report.PrepareReport(modelo.fechaini, modelo.fechafin, modelo.idcliente);
                return File(abytes, "application/pdf", "ListadoFacturasClientes.pdf");
            }
            else
            {
                if (modelo.idcliente == 0)
                {
                    cadena = "select * from [dbo].[EncfacturasSaldos] where Fecha >= '" + FI + "' and Fecha <='" + FF + "' ";
                }
                else
                {
                    ClsDatoString cliente = db.Database.SqlQuery<ClsDatoString>("select Nombre as Dato  from [dbo].[Clientes] where IDCliente = '" + modelo.idcliente + "'").ToList()[0];
                    cadena = "select * from [dbo].[EncfacturasSaldos] where Fecha >= '" + FI + "' and Fecha <='" + FF + "' and Nombre_Cliente = '" + cliente.Dato + "' ";
                }


                List<EncfacturasSaldos> facturassaldos = dbs.Database.SqlQuery<EncfacturasSaldos>(cadena).ToList();
                ViewBag.facturassaldos = facturassaldos;
                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Facturas");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:S1"].Style.Font.Size = 20;
                Sheet.Cells["A1:S1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:S3"].Style.Font.Bold = true;
                Sheet.Cells["A1:S1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:S1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Facturas");

                row = 2;
                Sheet.Cells["A1:S1"].Style.Font.Size = 12;
                Sheet.Cells["A1:S1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:S1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:S2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:S3"].Style.Font.Bold = true;
                Sheet.Cells["A3:S3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:S3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Serie");
                Sheet.Cells["B3"].RichText.Add("Numero");
                Sheet.Cells["C3"].RichText.Add("Fecha");
                Sheet.Cells["D3"].RichText.Add("Cliente");
                Sheet.Cells["E3"].RichText.Add("Subtotal");
                Sheet.Cells["F3"].RichText.Add("Iva");
                Sheet.Cells["G3"].RichText.Add("Total");
                Sheet.Cells["H3"].RichText.Add("Moneda");
                Sheet.Cells["I3"].RichText.Add("Tipo de Cambio");
                Sheet.Cells["J3"].RichText.Add("Metodo de pago");
                Sheet.Cells["K3"].RichText.Add("UUID");
                Sheet.Cells["L3"].RichText.Add("Estado");
                Sheet.Cells["M3"].RichText.Add("Pagada");
                Sheet.Cells["N3"].RichText.Add("Importe Pagado");
                Sheet.Cells["O3"].RichText.Add("Saldo Insoluto");
                Sheet.Cells["P3"].RichText.Add("Fecha de Revisión");
                Sheet.Cells["Q3"].RichText.Add("Fecha de Vencimiento");
                //Sheet.Cells["R3"].RichText.Add("Telefono");
                Sheet.Cells["R3"].RichText.Add("Vendedor");
   //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
                //Sheet.Cells["A3"].Value = "Serie";

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:R3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (EncfacturasSaldos item in ViewBag.facturassaldos)
                {

                   

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.Serie;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Numero;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Nombre_cliente;
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Subtotal;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.IVA;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Moneda;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.TC;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.IDMetododepago;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.UUID;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Estado;
                    //Como item.pagada es de tipo booleano (verdadero o falso), se imprime en pantalla (si o no), según corresponda.
                    if (item.pagada == false)
                    {
                        Sheet.Cells[string.Format("M{0}", row)].Value = "No";
                    }
                    else
                    {
                        Sheet.Cells[string.Format("M{0}", row)].Value = "Si";
                    }
                    Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.ImportePagado;
                    Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.ImporteSaldoInsoluto;

                    Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.FechaRevision;
                    Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("Q{0}", row)].Value = item.FechaVencimiento;
                    //Sheet.Cells[string.Format("R{0}", row)].Value = item.Telefono;
                    string cadenaf = "select * from encprefactura where seriedigital='" + item.Serie + "' and numerodigital=" + item.Numero;

                    try
                    {
                        EncPrefactura prefac = new PrefacturaContext().Database.SqlQuery<EncPrefactura>(cadenaf).ToList().FirstOrDefault();

                        Vendedor vendedor = new VendedorContext().Vendedores.Find(prefac.IDVendedor);
                        Sheet.Cells[string.Format("R{0}", row)].Value = vendedor.Nombre;
                    }
                    catch (Exception err)
                    {
                        Sheet.Cells[string.Format("R{0}", row)].Value = "Sin vendedor";
                    }
                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelFacturas.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            //return Redirect("index");
        }

        public ActionResult EntreFechasNC()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasNC(EFecha modelo, FormCollection coleccion)
        {
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {

            }
            if (cual == "Generar excel")
            {

                string cadena = "select * from [dbo].[Encfacturas] where Serie = 'N' and (Fecha >= '" + FI + "' and Fecha <='" + FF + "')";
                var notas = dbs.Database.SqlQuery<Encfacturas>(cadena).ToList();
                ViewBag.notas = notas;
                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Notas de Crédito");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:J1"].Style.Font.Size = 20;
                Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:J3"].Style.Font.Bold = true;
                Sheet.Cells["A1:J1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Notas de Crédito");

                row = 2;
                Sheet.Cells["A1:J1"].Style.Font.Size = 12;
                Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:J2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:J3"].Style.Font.Bold = true;
                Sheet.Cells["A3:J3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:J3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Serie");
                Sheet.Cells["B3"].RichText.Add("Numero");
                Sheet.Cells["C3"].RichText.Add("Fecha");
                Sheet.Cells["D3"].RichText.Add("Cliente");
                Sheet.Cells["E3"].RichText.Add("Subtotal");
                Sheet.Cells["F3"].RichText.Add("Iva");
                Sheet.Cells["G3"].RichText.Add("Total");
                Sheet.Cells["H3"].RichText.Add("Moneda");
                Sheet.Cells["I3"].RichText.Add("UUID");
                Sheet.Cells["J3"].RichText.Add("Estado");

                //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
                //Sheet.Cells["A3"].Value = "Serie";

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:R3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (Encfacturas item in ViewBag.notas)
                {


                    Sheet.Cells[string.Format("A{0}", row)].Value = item.Serie;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Numero;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Nombre_cliente;
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Subtotal;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.IVA;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Moneda;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.UUID;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.Estado;

                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelNC.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }


        public ActionResult HFacturacion()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult HFacturacion(EFecha modelo, FormCollection coleccion)
        {
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();
            decimal antesT = 0;
            decimal enT = 0;
            decimal fueraT = 0;
            decimal antesTP = 0;
            decimal enTP = 0;
            decimal fueraTP = 0;
            decimal Tot = 0;

            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {

            }
            if (cual == "Generar excel")
            {
                //Elimino la tabla temporal



                string quita = "IF OBJECT_ID(N'dbo.TempFacturacion', N'U') IS NOT NULL drop table dbo.TempFacturacion";
                db.Database.ExecuteSqlCommand(quita);
                //Crea la tabla temporal
                string crea = "SELECT * INTO dbo.TempFacturacion FROM dbo.VFacturacion WHERE FechaFac >= '" + FI + " 00:00:00.1' and FechaFac <= '" + FF + " 23:59:59.999' order by Ticket;";
                db.Database.ExecuteSqlCommand(crea);

                string cadena = "select * from dbo.TempFacturacion";
                TempFacturacionContext dbf = new TempFacturacionContext();
                var datos = dbf.Database.SqlQuery<TempFacturacion>(cadena).ToList();

                ViewBag.datos = datos;
                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Facturación");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:J1"].Style.Font.Size = 20;
                Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:J3"].Style.Font.Bold = true;
                Sheet.Cells["A1:J1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Facturación");

                row = 2;
                Sheet.Cells["A1:J1"].Style.Font.Size = 12;
                Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:J2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                Sheet.Cells[string.Format("G2", row)].Value = "Fecha Reporte";
                Sheet.Cells[string.Format("H2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                DateTime fecAct = DateTime.Today;
                Sheet.Cells[string.Format("H2", row)].Value = fecAct;
                row = 3;
                //En la fila3 se da el formato a el encabezado
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:N3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:N3"].Style.Font.Bold = true;

                // Se establece el nombre que identifica a cada una de las columnas de datos. 

                Sheet.Cells["A3:E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["L3:N3"].Style.Font.Color.SetColor(Color.Brown);
                Sheet.Cells["A3:E3"].Style.Fill.BackgroundColor.SetColor(Color.Goldenrod);
                Sheet.Cells[string.Format("A3", row)].Value = "Facturas";

                Sheet.Cells["F3:I3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["F3:I3"].Style.Font.Color.SetColor(Color.White);
                Sheet.Cells["F3:I3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.MidnightBlue);
                Sheet.Cells[string.Format("F3", row)].Value = "Prefactura";

                // Se establece el nombre que identifica a cada una de las columnas de datos. 
                Sheet.Cells["J3:K3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["L3:N3"].Style.Font.Color.SetColor(Color.Brown);
                Sheet.Cells["J3:K3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Goldenrod);
                Sheet.Cells[string.Format("J3", row)].Value = "Remisión";

                Sheet.Cells["L3:N3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["L3:N3"].Style.Font.Color.SetColor(Color.White);
                Sheet.Cells["L3:N3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.MidnightBlue);
                Sheet.Cells[string.Format("L3", row)].Value = "Pedido";

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["C3:H3"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                row = 4;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A4:S4"].Style.Font.Bold = true;
                Sheet.Cells["A4:S4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A4:E4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleGoldenrod);
                Sheet.Cells["A4"].RichText.Add("IDOperacion");
                Sheet.Cells["B4"].RichText.Add("Serie");
                Sheet.Cells["C4"].RichText.Add("Numero");
                Sheet.Cells["D4"].RichText.Add("Fecha");
                Sheet.Cells["E4"].RichText.Add("Estado");

                Sheet.Cells["F4:I4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PowderBlue);
                Sheet.Cells["F4"].RichText.Add("ID");
                Sheet.Cells["G4"].RichText.Add("Serie");
                Sheet.Cells["H4"].RichText.Add("Numero");
                Sheet.Cells["I4"].RichText.Add("Fecha");

                Sheet.Cells["J4:K4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleGoldenrod);
                Sheet.Cells["J4"].RichText.Add("No.");
                Sheet.Cells["K4"].RichText.Add("Fecha");

                Sheet.Cells["L4:N4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PowderBlue);
                Sheet.Cells["L4"].RichText.Add("No.");
                Sheet.Cells["M4"].RichText.Add("Fecha");
                Sheet.Cells["N4"].RichText.Add("Fecha Compromiso");

                Sheet.Cells["O4:S4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                Sheet.Cells["O4"].RichText.Add("Se Facturo desde");
                Sheet.Cells["P4"].RichText.Add("Fecha Factura -Fecha Prefactura");
                Sheet.Cells["Q4"].RichText.Add("Fecha Factura -Fecha Remisión");
                Sheet.Cells["R4"].RichText.Add("Fecha Factura -Fecha Pedido");
                Sheet.Cells["S4"].RichText.Add("Fecha Factura -Fecha Compromiso");

                //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
                //Sheet.Cells["A3"].Value = "Serie";

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A4:N4"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 5;
                Sheet.Cells.Style.Font.Bold = false;
                int renglones = datos.Count + row;
                Sheet.Cells["S4:S" + renglones].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                foreach (TempFacturacion item in ViewBag.datos)
                {


                    Sheet.Cells[string.Format("A{0}", row)].Value = item.Ticket;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.SerieDigital;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.NumeroDigital;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.FechaFac;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Estado;

                    Sheet.Cells[string.Format("F{0}", row)].Value = item.IDPrefactura;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.SeriePre;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.NumeroPre;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.FechaPrefactura;

                    Sheet.Cells[string.Format("J{0}", row)].Value = item.iDRemision;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.FechaRem;

                    Sheet.Cells[string.Format("L{0}", row)].Value = item.IDPedido;
                    Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.FechaPed;
                    Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.FechaRequiere;

                    Sheet.Cells[string.Format("O{0}", row)].Value = item.Proviene;
                    int dias1 = DiasHabiles(item.FechaPrefactura, item.FechaFac);
                    int dias2 = DiasHabiles(item.FechaRem, item.FechaFac);
                    int dias3 = DiasHabiles(item.FechaPed, item.FechaFac);
                    int dias4 = DiasHabiles(item.FechaRequiere, item.FechaFac);
                    Sheet.Cells[string.Format("P{0}", row)].Value = dias1;
                    Sheet.Cells[string.Format("Q{0}", row)].Value = dias2;
                    Sheet.Cells[string.Format("R{0}", row)].Value = dias3;
                    if (dias4 < 0)
                    {
                        Sheet.Cells[string.Format("S{0}", row)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                        Sheet.Cells[string.Format("S{0}", row)].Value = dias4;
                        antesT += 1;
                    }
                    if (dias4 == 0)
                    {
                        Sheet.Cells[string.Format("S{0}", row)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PapayaWhip);
                        Sheet.Cells[string.Format("S{0}", row)].Value = dias4;
                        enT += 1;
                    }
                    if (dias4 > 0)
                    {
                        Sheet.Cells[string.Format("S{0}", row)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCoral);
                        Sheet.Cells[string.Format("S{0}", row)].Value = dias4;
                        fueraT += 1;
                    }
                    row++;
                }
                Tot = antesT + enT + fueraT;

                int nreg = renglones + 1;
                Sheet.Cells["A" + nreg + ":D" + nreg].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A" + nreg + ":D" + nreg].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkCyan);
                Sheet.Cells["A" + nreg + ":D" + nreg].Style.Font.Color.SetColor(Color.White);
                Sheet.Cells["A" + nreg + ":D" + nreg].Style.Font.Bold = true;
                Sheet.Cells["A" + nreg].RichText.Add("Total Facturas");
                Sheet.Cells["B" + nreg].RichText.Add("Antes de Tiempo");
                Sheet.Cells["C" + nreg].RichText.Add("En Tiempo");
                Sheet.Cells["D" + nreg].RichText.Add("Fuera de Tiempo");
                nreg += 1;
                Sheet.Cells[string.Format("A" + nreg)].Value = Tot;
                Sheet.Cells[string.Format("B" + nreg)].Value = antesT;
                Sheet.Cells[string.Format("C" + nreg)].Value = enT;
                Sheet.Cells[string.Format("D" + nreg)].Value = fueraT;
                nreg += 1;
                antesTP = (100 / Tot) * antesT;
                enTP = (100 / Tot) * enT;
                fueraTP = (100 / Tot) * fueraT;

                Sheet.Cells[string.Format("A" + nreg)].Style.Numberformat.Format = "#0\\.00%";
                Sheet.Cells[string.Format("A" + nreg)].Value = "100";
                Sheet.Cells[string.Format("B" + nreg)].Style.Numberformat.Format = "#0\\.00%";
                Sheet.Cells[string.Format("B" + nreg)].Value = antesTP;
                Sheet.Cells[string.Format("C" + nreg)].Style.Numberformat.Format = "#0\\.00%";
                Sheet.Cells[string.Format("C" + nreg)].Value = enTP;
                Sheet.Cells[string.Format("D" + nreg)].Style.Numberformat.Format = "#0\\.00%";
                Sheet.Cells[string.Format("D" + nreg)].Value = fueraTP;
                //Sheet.Cells[string.Format("E" + nreg)].Value = "%";


                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelNC.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }

        public int DiasHabiles(DateTime firstDay, DateTime lastDay)
        {
            List<DateTime> bankHolidays = new List<DateTime>();
            DateTime uno = DateTime.Parse(DateTime.Now.Year + "/05/01");
            DateTime dos = DateTime.Parse(DateTime.Now.Year + "/05/05");
            DateTime tres = DateTime.Parse(DateTime.Now.Year + "/12/24");
            DateTime cuatro = DateTime.Parse(DateTime.Now.Year + "/12/25");
            DateTime cinco = DateTime.Parse(DateTime.Now.Year + "/12/12");
            DateTime seis = DateTime.Parse(DateTime.Now.Year + "/12/31");
            DateTime siete = DateTime.Parse(DateTime.Now.Year + "/01/01");
            DateTime ocho = DateTime.Parse(DateTime.Now.Year + "/12/24");
            DateTime nueve = DateTime.Parse(DateTime.Now.Year + "/09/16");
            bankHolidays.Add(uno);
            bankHolidays.Add(dos);
            bankHolidays.Add(tres);
            bankHolidays.Add(cuatro);
            bankHolidays.Add(cinco);
            bankHolidays.Add(seis);
            bankHolidays.Add(siete);
            bankHolidays.Add(ocho);
            bankHolidays.Add(nueve);

            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            //if (firstDay > lastDay)
            //    throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days;// + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = (int)firstDay.DayOfWeek;
                int lastDayOfWeek = (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            foreach (DateTime bankHoliday in bankHolidays)
            {
                DateTime bh = bankHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return businessDays;
        }

        public ActionResult EntreFechasFV()
        {
            EFecha elemento = new EFecha();
            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasFV(EFecha modelo, FormCollection coleccion)
        {
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            string cual = coleccion.Get("Enviar");
            if (cual == "Excel con Costo")
            {
                int fila = 3;
                int row = 1;
                decimal totalMXNVen = 0;
                decimal totalUSDVen = 0;
                decimal totalMXN = 0;
                decimal totalUSD = 0;
                decimal subtMXN = 0;
                decimal ivaMXN = 0;
                decimal totMXN = 0;
                decimal GsubtMXN = 0;
                decimal GivaMXN = 0;
                decimal GtotMXN = 0;
                decimal subtUSD = 0;
                decimal ivaUSD = 0;
                decimal totUSD = 0;
                decimal GsubtUSD = 0;
                decimal GivaUSD = 0;
                decimal GtotUSD = 0;
                string cadenaV = "select distinct C.IDVendedor, V.Nombre, V.CuotaVendedor, M.ClaveMoneda from [dbo].[Encfacturas] E inner join (Clientes C inner join ([dbo].[Vendedor] as V inner join [dbo].[c_Moneda] as M on V.IDMoneda = M.IDMoneda) on C.IDVendedor = V.IDVendedor) on E.IDcliente= C.IDCliente where Serie = 'MX' and (Fecha >= '" + FI + " 00:00:00.1' and Fecha <='" + FF + " 23:59:59.999') order by V.Nombre";
                var vendedor = dbs.Database.SqlQuery<VendedorPedidoDls>(cadenaV).ToList();
                ViewBag.vendedor = vendedor;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Facturación");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.

                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:P1"].Style.Font.Size = 20;
                Sheet.Cells["A1:P1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:P1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:P1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1:P1"].Style.Font.Bold = true;
                Sheet.Cells["A1"].RichText.Add("Listado de Facturas en Dolares por Vendedor");

                row = 2;
                Sheet.Cells["A2:P2"].Style.Font.Size = 12;
                Sheet.Cells["A2:P2"].Style.Font.Name = "Calibri";
                Sheet.Cells["A2:P2"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:P2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos

                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                Sheet.Cells[string.Format("F2", row)].Value = "TC";
                decimal tcd = db.Database.SqlQuery<ClsDatoDecimal>("select dbo.GetTipocambio(GETDATE(),(select idMoneda from C_MONEDA WHERE clavemoneda='USD'),(select idMoneda from C_MONEDA WHERE clavemoneda='MXN')  )  as Dato").ToList()[0].Dato;
                Sheet.Cells[string.Format("G2", row)].Style.Numberformat.Format = "#,##0.00";
                Sheet.Cells[string.Format("G2", row)].Value = tcd;
                //En la fila3 se da el formato a el encabezado

                row = 4;

                foreach (VendedorPedidoDls item in ViewBag.vendedor)
                {
                    fila = row;
                    Sheet.Cells.Style.Font.Name = "Calibri";
                    Sheet.Cells.Style.Font.Size = 10;
                    Sheet.Cells["A" + fila + ":P" + fila].Style.Font.Bold = true;
                    Sheet.Cells["A" + fila + ":P" + fila].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells["A" + fila + ":P" + fila].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                    // Se establecen los datos del vendedor.
                    Sheet.Cells["A" + fila].RichText.Add("No. Vendedor");
                    Sheet.Cells["B" + fila].RichText.Add("Vendedor");
                    Sheet.Cells["C" + fila].RichText.Add("Cuota");
                    Sheet.Cells["D" + fila].RichText.Add("Moneda");

                    //Aplicar borde doble al rango de celdas A3:Q3
                    Sheet.Cells["A" + fila + ":P" + fila].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                    // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                    // Se establecen los formatos para las celdas: Fecha, Moneda
                    Sheet.Cells.Style.Font.Bold = false;

                    fila += 1;
                    row = fila;

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDVendedor;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Nombre;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.CuotaVendedor;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.ClaveMoneda;

                    fila += 1;
                    row = fila;
                    Sheet.Cells.Style.Font.Name = "Calibri";
                    Sheet.Cells.Style.Font.Size = 10;
                    Sheet.Cells["A" + fila + ":P" + fila].Style.Font.Bold = true;
                    Sheet.Cells["A" + fila + ":P" + fila].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells["A" + fila + ":P" + fila].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    // Se establecen los datos del vendedor.
                    Sheet.Cells["A" + fila].RichText.Add("Ticket");
                    Sheet.Cells["B" + fila].RichText.Add("Serie");
                    Sheet.Cells["C" + fila].RichText.Add("Numero");
                    Sheet.Cells["D" + fila].RichText.Add("Fecha");
                    Sheet.Cells["E" + fila].RichText.Add("No. Cliente");
                    Sheet.Cells["F" + fila].RichText.Add("Nombre");
                    Sheet.Cells["G" + fila].RichText.Add("Subtotal");
                    Sheet.Cells["H" + fila].RichText.Add("IVA");
                    Sheet.Cells["I" + fila].RichText.Add("Total");
                    Sheet.Cells["J" + fila].RichText.Add("Moneda Factura");
                    Sheet.Cells["K" + fila].RichText.Add("TC");
                    Sheet.Cells["L" + fila].RichText.Add("Estado");
                    Sheet.Cells["M" + fila].RichText.Add("Total MXN");
                    Sheet.Cells["N" + fila].RichText.Add("Total USD");
                    Sheet.Cells["O" + fila].RichText.Add("Rentabilidad");
                    Sheet.Cells["P" + fila].RichText.Add("Costo");
                    string cadenaF = "select  F.[ID], F.[Serie], F.[Numero], F.[Fecha], F.IDCliente, C.Nombre, F.[Subtotal],F.[IVA], F.[Total], F.[Estado], F.[TC], F.[IDMoneda], M.ClaveMoneda as MonedaFactura, V.IDVendedor, V.Nombre as Vendedor from [dbo].[Encfacturas] F 	inner join Clientes C on F.IDcliente= C.IDCliente inner join [dbo].[Vendedor] as V on C.IDVendedor = V.IDVendedor inner join [dbo].[c_Moneda] as M on F.IDMoneda = M.IDMoneda where Serie = 'MX' and (Fecha >= '" + FI + " 00:00:00.1' and Fecha <='" + FF + " 23:59:59.999') and V.IDVendedor=" + item.IDVendedor + " order by V.Nombre, C.Nombre, F.Numero";
                    var factura = dbs.Database.SqlQuery<FactVen>(cadenaF).ToList();
                    ViewBag.factura = factura;
                    totalMXNVen = 0;
                    totalUSDVen = 0;
                    subtMXN = 0;
                    ivaMXN = 0;
                    totMXN = 0;
                    subtUSD = 0;
                    ivaUSD = 0;
                    totUSD = 0;

                    fila += 1;
                    row = fila;

                    foreach (FactVen itemf in ViewBag.factura)
                    {

                        EncPrefactura prefactura = new PrefacturaContext().EncPrefactura.Where(s => s.SerieDigital == itemf.Serie && s.NumeroDigital == itemf.Numero.ToString()).FirstOrDefault();

                        decimal Rentabilidad = 0;
                        decimal Costo = 0;
                        try
                        {
                            if (prefactura != null)
                            {
                                string NoPedido = r_Pedidos(prefactura.IDPrefactura);//
                                string[] pedidos = NoPedido.Split(' ');
                                int pedido = int.Parse(pedidos[0]);
                                DateTime FechaPedido = r_FechaPedido(pedido);//
                              Costo = c_Costo(prefactura, pedido);//
                                //int comisionableart = comisionablearticulo(prefactura);

                                decimal basecomisionable = c_basecomisionable(prefactura);
                              
                                try
                                {
                                    Rentabilidad = 1 - (Costo / basecomisionable);
                                }
                                catch (Exception err)
                                {
                                    Rentabilidad = 0.3M;
                                }
                                //
                                string MonedaFactura = "";
                                try
                                {
                                    MonedaFactura = prefactura.c_Moneda.ClaveMoneda;
                                }
                                catch (Exception err)
                                {

                                }
                                string MonedaCosto = "";
                                try
                                {
                                    MonedaCosto = prefactura.c_Moneda.ClaveMoneda;
                                }
                                catch (Exception err)
                                {

                                }

                                //string cadena0 = "Select monto as Dato from PenalizacionFactura where idfactura=" +;
                                //ClsDatoDecimal costo = db.Database.SqlQuery<ClsDatoDecimal>(cadena0).ToList().FirstOrDefault();

                                decimal montoPenalizacion = 0;

                                //if (costo == null)
                                //{
                                //    //montoPenalizacion = 0;

                                //    montoPenalizacion = VerificarPCliente(itemf.IDCliente, elemento.subtotal, elemento.ID);

                                //}
                                //else
                                //{
                                //    montoPenalizacion = costo.Dato;
                                //}

                                VCambio cambio;
                                int monedaorigen = prefactura.c_Moneda.IDMoneda;
                                string fecha = FechaPedido.ToString("yyyy/MM/dd");
                                List<c_Moneda> monedadestino = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
                                int destino = monedadestino.Select(s => s.IDMoneda).FirstOrDefault();

                                try
                                {
                                    cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + monedaorigen + "," + destino + ") as TC").ToList()[0];

                                }
                                catch (Exception err)
                                {
                                    cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "',180,180) as TC").ToList()[0];
                                    string mensajeerror = err.Message;
                                }

                              

                                decimal MontoRentabilidad = 0;
                                decimal MontoComision = 0;
                                ClsDatoDecimal can = db.Database.SqlQuery<ClsDatoDecimal>("select subtotal as Dato from encpedido where idpedido=" + pedido).ToList().FirstOrDefault();

                                decimal subtotal = can.Dato;
                                decimal basecomisionablereal = subtotal - montoPenalizacion - (subtotal - basecomisionable);


                                if (basecomisionablereal < 0)
                                {
                                    basecomisionablereal = 0;
                                }
                                if (Rentabilidad < 0)
                                {
                                    Rentabilidad = 0;
                                }

                             
                                if (item.IDVendedor == 5)
                                {

                                    try
                                    {
                                        MontoRentabilidad = basecomisionablereal - Costo;
                                        if (MontoRentabilidad < 0)
                                        {
                                            MontoRentabilidad = 0;
                                        }
                                    }
                                    catch (Exception err)
                                    {

                                    }

                                   
                                }
                                else
                                {

                                    try
                                    {
                                        MontoRentabilidad = basecomisionablereal * Rentabilidad;
                                    }
                                    catch (Exception err)
                                    {

                                    }

                                   
                                }




                            }
                        }
                        catch (Exception err)
                        {
                            string mensaje = err.Message;
                        }


                        Sheet.Cells[string.Format("A{0}", row)].Value = itemf.ID;
                        Sheet.Cells[string.Format("B{0}", row)].Value = itemf.Serie;
                        Sheet.Cells[string.Format("C{0}", row)].Value = itemf.Numero;
                        Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                        Sheet.Cells[string.Format("D{0}", row)].Value = itemf.Fecha;
                        Sheet.Cells[string.Format("E{0}", row)].Value = itemf.IDCliente;
                        Sheet.Cells[string.Format("F{0}", row)].Value = itemf.Nombre;
                        Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("G{0}", row)].Value = itemf.Subtotal;
                        Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("H{0}", row)].Value = itemf.IVA;
                        Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("I{0}", row)].Value = itemf.Total;
                        Sheet.Cells[string.Format("J{0}", row)].Value = itemf.MonedaFactura;
                        Sheet.Cells[string.Format("K{0}", row)].Value = itemf.TC;
                        if (itemf.Estado != "C")
                        {
                            Sheet.Cells[string.Format("L{0}", row)].Value = itemf.Estado;
                        }
                        else
                        {
                            Sheet.Cells["L" + fila].Style.Font.Color.SetColor(Color.Red);
                            Sheet.Cells[string.Format("L{0}", row)].Value = itemf.Estado;
                        }

                        if (itemf.MonedaFactura == "USD")
                        {
                            //Pesos

                            Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("M{0}", row)].Value = itemf.Total * itemf.TC;
                            //Dolares
                            Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("N{0}", row)].Value = itemf.Total;
                            if (itemf.Estado != "C")
                            {
                                totalMXNVen += itemf.Total * itemf.TC;
                                totalUSDVen += itemf.Total;
                                totalMXN += itemf.Total * itemf.TC;
                                totalUSD += itemf.Total;

                                subtUSD += itemf.Subtotal;
                                ivaUSD += itemf.IVA;
                                totUSD += itemf.Total;
                                GsubtUSD += itemf.Subtotal;
                                GivaUSD += itemf.Total;
                                GtotUSD += itemf.Total;
                            }
                        }
                        if (itemf.MonedaFactura == "MXN")
                        {//Pesos
                            Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("M{0}", row)].Value = itemf.Total;
                            //Dolares
                            Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("N{0}", row)].Value = itemf.Total / tcd;
                            if (itemf.Estado != "C")
                            {
                                totalMXNVen += itemf.Total;
                                totalUSDVen += itemf.Total / tcd;
                                totalMXN += itemf.Total;
                                totalUSD += itemf.Total / tcd;
                                subtMXN += itemf.Subtotal;
                                ivaMXN += itemf.IVA;
                                totMXN += itemf.Total;
                                GsubtMXN += itemf.Subtotal;
                                GivaMXN += itemf.Total;
                                GtotMXN += itemf.Total;
                            }
                        }

                        //Rentabilidad
                        Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "#0.00%";
                        Sheet.Cells[string.Format("O{0}", row)].Value = Rentabilidad;
                        //Costo
                        Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("P{0}", row)].Value = Costo;

                        fila += 1;
                        row = fila;

                    }
                    Sheet.Cells.Style.Font.Name = "Calibri";
                    Sheet.Cells.Style.Font.Size = 10;
                    Sheet.Cells["F" + fila + ":N" + (fila + 1)].Style.Font.Bold = true;
                    Sheet.Cells["F" + fila + ":N" + (fila + 1)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells["F" + fila + ":N" + (fila + 1)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGoldenrodYellow);
                    Sheet.Cells[string.Format("F{0}", row)].Value = "Total MXN";
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = subtMXN;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = ivaMXN;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("I{0}", row)].Value = totMXN;
                    Sheet.Cells[string.Format("L{0}", row)].Value = "Total Vendedor";
                    //Pesos
                    Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("M{0}", row)].Value = totalMXNVen;
                    //Dolares
                    Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("N{0}", row)].Value = totalUSDVen;
                    fila += 1;
                    row = fila;
                    Sheet.Cells[string.Format("F{0}", row)].Value = "Total USD";
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = subtUSD;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = ivaUSD;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("I{0}", row)].Value = totUSD;
                    fila += 1;
                    row = fila;
                    row++;
                }

                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelNC.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            if (cual == "Excel")
            {
                int fila = 3;
                int row = 1;
                decimal totalMXNVen = 0;
                decimal totalUSDVen = 0;
                decimal totalMXN = 0;
                decimal totalUSD = 0;
                decimal subtMXN = 0;
                decimal ivaMXN = 0;
                decimal totMXN = 0;
                decimal GsubtMXN = 0;
                decimal GivaMXN = 0;
                decimal GtotMXN = 0;
                decimal subtUSD = 0;
                decimal ivaUSD = 0;
                decimal totUSD = 0;
                decimal GsubtUSD = 0;
                decimal GivaUSD = 0;
                decimal GtotUSD = 0;
                string cadenaV = "select distinct C.IDVendedor, V.Nombre, V.CuotaVendedor, M.ClaveMoneda from [dbo].[Encfacturas] E inner join (Clientes C inner join ([dbo].[Vendedor] as V inner join [dbo].[c_Moneda] as M on V.IDMoneda = M.IDMoneda) on C.IDVendedor = V.IDVendedor) on E.IDcliente= C.IDCliente where Serie = 'MX' and (Fecha >= '" + FI + " 00:00:00.1' and Fecha <='" + FF + " 23:59:59.999') order by V.Nombre";
                var vendedor = dbs.Database.SqlQuery<VendedorPedidoDls>(cadenaV).ToList();
                ViewBag.vendedor = vendedor;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Facturación");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.

                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:N1"].Style.Font.Size = 20;
                Sheet.Cells["A1:N1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:N1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:N1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1:N1"].Style.Font.Bold = true;
                Sheet.Cells["A1"].RichText.Add("Listado de Facturas en Dolares por Vendedor");

                row = 2;
                Sheet.Cells["A2:N2"].Style.Font.Size = 12;
                Sheet.Cells["A2:N2"].Style.Font.Name = "Calibri";
                Sheet.Cells["A2:N2"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:N2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos

                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                Sheet.Cells[string.Format("F2", row)].Value = "TC";
                decimal tcd = db.Database.SqlQuery<ClsDatoDecimal>("select dbo.GetTipocambio(GETDATE(),(select idMoneda from C_MONEDA WHERE clavemoneda='USD'),(select idMoneda from C_MONEDA WHERE clavemoneda='MXN')  )  as Dato").ToList()[0].Dato;
                Sheet.Cells[string.Format("G2", row)].Style.Numberformat.Format = "#,##0.00";
                Sheet.Cells[string.Format("G2", row)].Value = tcd;
                //En la fila3 se da el formato a el encabezado

                row = 4;

                foreach (VendedorPedidoDls item in ViewBag.vendedor)
                {
                    fila = row;
                    Sheet.Cells.Style.Font.Name = "Calibri";
                    Sheet.Cells.Style.Font.Size = 10;
                    Sheet.Cells["A" + fila + ":N" + fila].Style.Font.Bold = true;
                    Sheet.Cells["A" + fila + ":N" + fila].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells["A" + fila + ":N" + fila].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                    // Se establecen los datos del vendedor.
                    Sheet.Cells["A" + fila].RichText.Add("No. Vendedor");
                    Sheet.Cells["B" + fila].RichText.Add("Vendedor");
                    Sheet.Cells["C" + fila].RichText.Add("Cuota");
                    Sheet.Cells["D" + fila].RichText.Add("Moneda");

                    //Aplicar borde doble al rango de celdas A3:Q3
                    Sheet.Cells["A" + fila + ":N" + fila].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                    // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                    // Se establecen los formatos para las celdas: Fecha, Moneda
                    Sheet.Cells.Style.Font.Bold = false;

                    fila += 1;
                    row = fila;

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDVendedor;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Nombre;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.CuotaVendedor;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.ClaveMoneda;

                    fila += 1;
                    row = fila;
                    Sheet.Cells.Style.Font.Name = "Calibri";
                    Sheet.Cells.Style.Font.Size = 10;
                    Sheet.Cells["A" + fila + ":N" + fila].Style.Font.Bold = true;
                    Sheet.Cells["A" + fila + ":N" + fila].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells["A" + fila + ":N" + fila].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    // Se establecen los datos del vendedor.
                    Sheet.Cells["A" + fila].RichText.Add("Ticket");
                    Sheet.Cells["B" + fila].RichText.Add("Serie");
                    Sheet.Cells["C" + fila].RichText.Add("Numero");
                    Sheet.Cells["D" + fila].RichText.Add("Fecha");
                    Sheet.Cells["E" + fila].RichText.Add("No. Cliente");
                    Sheet.Cells["F" + fila].RichText.Add("Nombre");
                    Sheet.Cells["G" + fila].RichText.Add("Subtotal");
                    Sheet.Cells["H" + fila].RichText.Add("IVA");
                    Sheet.Cells["I" + fila].RichText.Add("Total");
                    Sheet.Cells["J" + fila].RichText.Add("Moneda Factura");
                    Sheet.Cells["K" + fila].RichText.Add("TC");
                    Sheet.Cells["L" + fila].RichText.Add("Estado");
                    Sheet.Cells["M" + fila].RichText.Add("Total MXN");
                    Sheet.Cells["N" + fila].RichText.Add("Total USD");

                    string cadenaF = "select  F.[ID], F.[Serie], F.[Numero], F.[Fecha], F.IDCliente, C.Nombre, F.[Subtotal],F.[IVA], F.[Total], F.[Estado], F.[TC], F.[IDMoneda], M.ClaveMoneda as MonedaFactura, V.IDVendedor, V.Nombre as Vendedor from [dbo].[Encfacturas] F 	inner join Clientes C on F.IDcliente= C.IDCliente inner join [dbo].[Vendedor] as V on C.IDVendedor = V.IDVendedor inner join [dbo].[c_Moneda] as M on F.IDMoneda = M.IDMoneda where Serie = 'MX' and (Fecha >= '" + FI + " 00:00:00.1' and Fecha <='" + FF + " 23:59:59.999') and V.IDVendedor=" + item.IDVendedor + " order by V.Nombre, C.Nombre, F.Numero";
                    var factura = dbs.Database.SqlQuery<FactVen>(cadenaF).ToList();
                    ViewBag.factura = factura;
                    totalMXNVen = 0;
                    totalUSDVen = 0;
                    subtMXN = 0;
                    ivaMXN = 0;
                    totMXN = 0;
                    subtUSD = 0;
                    ivaUSD = 0;
                    totUSD = 0;

                    fila += 1;
                    row = fila;

                    foreach (FactVen itemf in ViewBag.factura)
                    {
                        Sheet.Cells[string.Format("A{0}", row)].Value = itemf.ID;
                        Sheet.Cells[string.Format("B{0}", row)].Value = itemf.Serie;
                        Sheet.Cells[string.Format("C{0}", row)].Value = itemf.Numero;
                        Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                        Sheet.Cells[string.Format("D{0}", row)].Value = itemf.Fecha;
                        Sheet.Cells[string.Format("E{0}", row)].Value = itemf.IDCliente;
                        Sheet.Cells[string.Format("F{0}", row)].Value = itemf.Nombre;
                        Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("G{0}", row)].Value = itemf.Subtotal;
                        Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("H{0}", row)].Value = itemf.IVA;
                        Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("I{0}", row)].Value = itemf.Total;
                        Sheet.Cells[string.Format("J{0}", row)].Value = itemf.MonedaFactura;
                        Sheet.Cells[string.Format("K{0}", row)].Value = itemf.TC;
                        if (itemf.Estado != "C")
                        {
                            Sheet.Cells[string.Format("L{0}", row)].Value = itemf.Estado;
                        }
                        else
                        {
                            Sheet.Cells["L" + fila].Style.Font.Color.SetColor(Color.Red);
                            Sheet.Cells[string.Format("L{0}", row)].Value = itemf.Estado;
                        }

                        if (itemf.MonedaFactura == "USD")
                        {
                            //Pesos

                            Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("M{0}", row)].Value = itemf.Total * itemf.TC;
                            //Dolares
                            Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("N{0}", row)].Value = itemf.Total;
                            if (itemf.Estado != "C")
                            {
                                totalMXNVen += itemf.Total * itemf.TC;
                                totalUSDVen += itemf.Total;
                                totalMXN += itemf.Total * itemf.TC;
                                totalUSD += itemf.Total;

                                subtUSD += itemf.Subtotal;
                                ivaUSD += itemf.IVA;
                                totUSD += itemf.Total;
                                GsubtUSD += itemf.Subtotal;
                                GivaUSD += itemf.Total;
                                GtotUSD += itemf.Total;
                            }
                        }
                        if (itemf.MonedaFactura == "MXN")
                        {//Pesos
                            Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("M{0}", row)].Value = itemf.Total;
                            //Dolares
                            Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("N{0}", row)].Value = itemf.Total / tcd;
                            if (itemf.Estado != "C")
                            {
                                totalMXNVen += itemf.Total;
                                totalUSDVen += itemf.Total / tcd;
                                totalMXN += itemf.Total;
                                totalUSD += itemf.Total / tcd;
                                subtMXN += itemf.Subtotal;
                                ivaMXN += itemf.IVA;
                                totMXN += itemf.Total;
                                GsubtMXN += itemf.Subtotal;
                                GivaMXN += itemf.Total;
                                GtotMXN += itemf.Total;
                            }
                        }


                        fila += 1;
                        row = fila;

                    }
                    Sheet.Cells.Style.Font.Name = "Calibri";
                    Sheet.Cells.Style.Font.Size = 10;
                    Sheet.Cells["F" + fila + ":N" + (fila + 1)].Style.Font.Bold = true;
                    Sheet.Cells["F" + fila + ":N" + (fila + 1)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells["F" + fila + ":N" + (fila + 1)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGoldenrodYellow);
                    Sheet.Cells[string.Format("F{0}", row)].Value = "Total MXN";
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = subtMXN;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = ivaMXN;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("I{0}", row)].Value = totMXN;
                    Sheet.Cells[string.Format("L{0}", row)].Value = "Total Vendedor";
                    //Pesos
                    Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("M{0}", row)].Value = totalMXNVen;
                    //Dolares
                    Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("N{0}", row)].Value = totalUSDVen;
                    fila += 1;
                    row = fila;
                    Sheet.Cells[string.Format("F{0}", row)].Value = "Total USD";
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = subtUSD;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = ivaUSD;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("I{0}", row)].Value = totUSD;
                    fila += 1;
                    row = fila;
                    row++;
                }

                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelNC.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
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
        public decimal c_Costo(EncPrefactura prefactura, int idpedido)
        {
            string Monedaprefactura = prefactura.c_Moneda.ClaveMoneda; /// USD MXN 
            var articulos = new PrefacturaContext().DetPrefactura.Where(s => s.IDPrefactura == prefactura.IDPrefactura);
            decimal acumuladorcosto = 0;
            foreach (DetPrefactura detalle in articulos)
            {
                if (r_escomisionale(detalle.IDArticulo))
                {
                    decimal costoxarti = 0;
                    try
                    {

                        ClsDatoDecimal can = db.Database.SqlQuery<ClsDatoDecimal>("select cantidad as Dato from DetPedido where idarticulo= " + detalle.IDArticulo + " and idpedido=" + idpedido).ToList().FirstOrDefault();
                        ClsDatoDecimal costoArt = db.Database.SqlQuery<ClsDatoDecimal>("select costo as Dato from DetPedido where idarticulo= " + detalle.IDArticulo + " and idpedido=" + idpedido).ToList().FirstOrDefault();






                        string cadenacostoxarticulo = "select dbo.getcosto(0," + detalle.IDArticulo + "," + can.Dato + ") as Dato";
                        costoxarti = new c_MonedaContext().Database.SqlQuery<ClsDatoDecimal>(cadenacostoxarticulo).FirstOrDefault().Dato;

                        Articulo Ar = db.Database.SqlQuery<Articulo>("select * from articulo where idarticulo= " + detalle.IDArticulo).ToList().FirstOrDefault();
                        string Monedaarticulo = "USD";

                        //if (Ar.IDFamilia == 10 || Ar.IDFamilia == 27 || Ar.IDFamilia == 19 || Ar.IDFamilia == 63)
                        //{
                        //    Monedaarticulo = "USD";
                        //}
                        //else
                        //{
                        c_MonedaContext C = new c_MonedaContext();
                        Monedaarticulo = C.c_Monedas.Find(Ar.IDMoneda).ClaveMoneda;
                        //}


                        if (costoxarti == 0)
                        {
                            try
                            {
                                DetOrdenCompra mc = new OrdenCompraContext().DetOrdenCompras.Where(s => s.IDArticulo == detalle.IDArticulo).OrderByDescending(s => s.IDDetOrdenCompra).FirstOrDefault();
                                costoxarti = mc.Costo;
                                C = new c_MonedaContext();
                                Monedaarticulo = C.c_Monedas.Find(mc.OrdenCompra.IDMoneda).ClaveMoneda;
                            }
                            catch (Exception err)
                            {
                                costoxarti = 0;
                            }
                        }

                        if (costoxarti == 0)  // sino tiene costo en ordendecompra va a lamatriz precioproveedor
                        {
                            try
                            {
                                MatrizPrecioProv mc = new MatrizPrecioProvContext().MatrizPP.Where(s => s.IDArticulo == detalle.IDArticulo).FirstOrDefault();
                                costoxarti = mc.Precio;
                                C = new c_MonedaContext();
                                Monedaarticulo = C.c_Monedas.Find(mc.IDMoneda).ClaveMoneda;
                            }
                            catch (Exception err)
                            {
                                costoxarti = 0;
                            }
                        }







                        if (costoxarti == 0)  // sino no tiene las anteriores va a la matriz de costo
                        {
                            MatrizCosto mc = new ArticuloContext().MatrizCostos.Where(s => s.IDArticulo == detalle.IDArticulo).FirstOrDefault();
                            costoxarti = mc.Precio;
                            C = new c_MonedaContext();
                            Monedaarticulo = C.c_Monedas.Find(Monedaarticulo).ClaveMoneda;

                        }

                        if (costoxarti == 0)  // sino no tiene las anteriores va a la matriz de costo
                        {
                            int iddetpedido = 0;

                            if (detalle.Proviene == "Pedido")
                            {
                                DetPedido detallepedido = new PedidoContext().DetPedido.Find(detalle.IDDetExterna);
                                iddetpedido = detallepedido.IDDetPedido;


                            }

                            if (detalle.Proviene == "Remision")
                            {
                                DetRemision detalleremision = new RemisionContext().DetRemisiones.Find(detalle.IDDetExterna);

                                iddetpedido = detalleremision.IDExterna;
                            }


                            int idordenproduccion = 0;
                            decimal cantidadproduccida = 0;

                            try
                            {
                                SIAAPI.Models.Produccion.OrdenProduccion orden = db.Database.SqlQuery
                                        <SIAAPI.Models.Produccion.OrdenProduccion>("select  * from OrdenProduccion where IDDetPedido=" + iddetpedido + " and EstadoOrden<>'Cancelada' ").ToList().FirstOrDefault();

                                cantidadproduccida = orden.Cantidad;

                                ClsDatoDecimal costo = new ArticuloContext().Database.SqlQuery<ClsDatoDecimal>("select sum(costoplaneado) as Dato from articuloproduccion where idorden =" + orden.IDOrden).FirstOrDefault();
                                try
                                {
                                    costoxarti = costo.Dato / orden.Cantidad;
                                }
                                catch (Exception err)
                                {

                                }

                            }
                            catch (Exception err)
                            {
                                string mensajeerr = err.Message;
                                idordenproduccion = 0;
                                cantidadproduccida = 0;
                                costoxarti = 0;

                            }

                        }

                        string cadenaaa = "select [dbo].[GetTipocambioCadena]('" + prefactura.Fecha.Year + "-" + prefactura.Fecha.Month + "-" + prefactura.Fecha.Day + "','" + Monedaarticulo + "','" + Monedaprefactura + "') as Dato";
                        decimal tipocambio = new ArticuloContext().Database.SqlQuery<ClsDatoDecimal>(cadenaaa).FirstOrDefault().Dato;

                        acumuladorcosto += ((costoxarti * tipocambio) * detalle.Cantidad);



                    }

                    catch (Exception err)
                    {

                    }

                }

            }

            return acumuladorcosto;
        }
        public decimal c_basecomisionable(EncPrefactura prefactura)
        {
            string Monedaprefactura = prefactura.c_Moneda.ClaveMoneda; /// USD MXN 
            var articulos = new PrefacturaContext().DetPrefactura.Where(s => s.IDPrefactura == prefactura.IDPrefactura);
            decimal acumuladorcosto = 0;
            foreach (DetPrefactura detalle in articulos)
            {
                if (r_escomisionale(detalle.IDArticulo))
                {

                    acumuladorcosto += (detalle.Importe);
                }

            }

            return acumuladorcosto;
        }

        public bool r_escomisionale(int IDArticuloComp)//Rastrear para saber si es comisionable
        {
            bool comisionable = true;

         
            //Articulo NO comisionable
            string cadena1 = "select count(*) as Dato from Articulo as A inner join ArticuloNOC as ANC on ANC.IDArticulo = A.IDArticulo where ANC.IDArticulo = " + IDArticuloComp + "";
            ClsDatoEntero Articulo_NOC = db.Database.SqlQuery<ClsDatoEntero>(cadena1).ToList().FirstOrDefault();
            try
            {
                if (Articulo_NOC.Dato != 0) //Si es diferente de 0 el articulo o el detprefactura es NO comisionable
                {
                    comisionable = false;
                }
            }
            catch (Exception err)
            {
                comisionable = false;
            }

            return comisionable;
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
        public ActionResult RSuajes()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult RSuajes(EFecha modelo, FormCollection coleccion)
        {
            VSuajesComprasContext dbCom = new VSuajesComprasContext();
            VSuajesFacturasContext dbFac = new VSuajesFacturasContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {

            }
            if (cual == "Generar excel")
            {
                string cadenaCli = "select * from VSuajesFacturas where Fecha >= '" + FI + " 00:00:00.1' and Fecha <='" + FF + " 23:59:59.999' ";
                string cadenaPro = "select * from VSuajesCompras where Fecha >= '" + FI + " 00:00:00.1' and Fecha <='" + FF + " 23:59:59.999' ";
                var suajeClie = dbFac.Database.SqlQuery<VSuajesFacturas>(cadenaCli).ToList();
                ViewBag.suajeClie = suajeClie;
                var suajePro = dbCom.Database.SqlQuery<VSuajesCompras>(cadenaPro).ToList();
                ViewBag.suajePro = suajePro;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Compras");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:K1"].Style.Font.Size = 20;
                Sheet.Cells["A1:K1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:K3"].Style.Font.Bold = true;
                Sheet.Cells["A1:K1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:K1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Compras");

                row = 2;
                Sheet.Cells["A1:K1"].Style.Font.Size = 12;
                Sheet.Cells["A1:K1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:K1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:K2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:K3"].Style.Font.Bold = true;
                Sheet.Cells["A3:K3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:K3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("No. OC");
                Sheet.Cells["B3"].RichText.Add("Estado");
                Sheet.Cells["C3"].RichText.Add("Fecha");
                Sheet.Cells["D3"].RichText.Add("TipoCambio");
                Sheet.Cells["E3"].RichText.Add("Moneda");
                Sheet.Cells["F3"].RichText.Add("Proveedor");
                Sheet.Cells["G3"].RichText.Add("Clave Artículo");
                Sheet.Cells["H3"].RichText.Add("Artículo");
                Sheet.Cells["I3"].RichText.Add("Cantidad");
                Sheet.Cells["J3"].RichText.Add("Costo");
                Sheet.Cells["K3"].RichText.Add("Observación");

                //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
                //Sheet.Cells["A3"].Value = "Serie";

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:R3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VSuajesCompras item in ViewBag.suajePro)
                {


                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDOrdenCompra;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Status;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = "#,##0.00";
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.TipoCambio;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.ClaveMoneda;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Empresa;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Cref;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Descripcion;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "#,##0.00";
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.Cantidad;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.000";
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.Costo;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.Observacion;

                    row++;
                }

                //Crear la hoja y poner el nombre de la pestaña del libro
                Sheet = Ep.Workbook.Worksheets.Add("Ventas");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:L1"].Style.Font.Size = 20;
                Sheet.Cells["A1:L1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:L3"].Style.Font.Bold = true;
                Sheet.Cells["A1:L1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Ventas");

                row = 2;
                Sheet.Cells["A1:L1"].Style.Font.Size = 12;
                Sheet.Cells["A1:L1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:L1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:L2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:L3"].Style.Font.Bold = true;
                Sheet.Cells["A3:L3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:L3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Serie");
                Sheet.Cells["B3"].RichText.Add("Numero");
                Sheet.Cells["C3"].RichText.Add("Fecha");
                Sheet.Cells["D3"].RichText.Add("TipoCambio");
                Sheet.Cells["E3"].RichText.Add("Moneda");
                Sheet.Cells["F3"].RichText.Add("Cliente");
                Sheet.Cells["G3"].RichText.Add("Clave Artículo");
                Sheet.Cells["H3"].RichText.Add("Artículo");
                Sheet.Cells["I3"].RichText.Add("Cantidad");
                Sheet.Cells["J3"].RichText.Add("Costo");
                Sheet.Cells["K3"].RichText.Add("Importe");
                Sheet.Cells["L3"].RichText.Add("Observación");

                //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
                //Sheet.Cells["A3"].Value = "Serie";

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:R3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VSuajesFacturas itemc in ViewBag.suajeClie)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = itemc.SerieDigital;
                    Sheet.Cells[string.Format("B{0}", row)].Value = itemc.NumeroDigital;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = itemc.Fecha;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = "#,##0.00";
                    Sheet.Cells[string.Format("D{0}", row)].Value = itemc.TC;
                    Sheet.Cells[string.Format("E{0}", row)].Value = itemc.ClaveMoneda;
                    Sheet.Cells[string.Format("F{0}", row)].Value = itemc.Nombre_Cliente;
                    Sheet.Cells[string.Format("G{0}", row)].Value = itemc.Cref;
                    Sheet.Cells[string.Format("H{0}", row)].Value = itemc.Descripcion;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "#,##0.00";
                    Sheet.Cells[string.Format("I{0}", row)].Value = itemc.Cantidad;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "#,##0.000";
                    Sheet.Cells[string.Format("J{0}", row)].Value = itemc.Costo;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "#,##0.000";
                    Sheet.Cells[string.Format("K{0}", row)].Value = itemc.Importe;
                    Sheet.Cells[string.Format("L{0}", row)].Value = itemc.Observacion;
                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "RSuajes.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }

        public JsonResult getClientesB(string buscar)
        {
            buscar = buscar.Remove(buscar.Length - 1);
            var Articulos = new ClientesContext().Clientes.Where(s => s.Nombre.Contains(buscar)).OrderBy(S => S.Nombre);

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (Clientes art in Articulos)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.Nombre;
                elemento.Value = art.IDCliente.ToString();
                opciones.Add(elemento);
            }

            return Json(opciones, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CrearCartaP(int id)
        {
            int IDCliente = 0;
            ViewBag.TienePerfil = false;
            Encfacturas factura = new EncfacturaContext().encfacturas.Find(id);

            //PerfilCartaPorte perfil = new PerfilCartaPorteContext().Database.SqlQuery<PerfilCartaPorte>("select*from perfilCartaPorte where idemisor="+ factura.IDCliente).FirstOrDefault();
            ClsCartaporte2 EncabezadoCartaPorte = new ClsCartaporte2();
            EncabezadoCartaPorte.IDFactura = id;

            EncabezadoCartaPorte.Moneda = factura.c_Moneda.ClaveMoneda;
            ViewBag.IDFactura = id;
            EncabezadoCartaPorte.NoFactura = factura.Numero;
            EncabezadoCartaPorte.SerieF = factura.Serie;
            EncabezadoCartaPorte.IDFactura = factura.ID;
            EncabezadoCartaPorte.subtotal = factura.Subtotal;
            EncabezadoCartaPorte.total = factura.Total;


            EncPrefactura prefactura = new PrefacturaContext().EncPrefactura.Where(x => (x.NumeroDigital == factura.Numero.ToString() && x.SerieDigital == factura.Serie)).FirstOrDefault();
            Clientes cliente = new ClientesContext().Clientes.Find(prefactura.IDCliente);
            List<DetPrefactura> merca = new PrefacturaContext().DetPrefactura.Where(x => x.IDPrefactura == prefactura.IDPrefactura).ToList();
            List<MercanciaCP> listamerca = new List<MercanciaCP>();
            ViewBag.IDCliente = cliente.IDCliente;
            foreach (DetPrefactura dp in merca)
            {
                MercanciaCP mercancia = new MercanciaCP();
                Articulo articulo = new ArticuloContext().Articulo.Find(dp.IDArticulo);
                mercancia.CodigoFamiliaPS = new c_ClaveProdSTCCContext().produc.Find(new FamiliaContext().Familias.Find(articulo.IDFamilia).IDClaveSTCC).Clave;
                mercancia.cantidad = dp.Cantidad;
                mercancia.Descripcion = articulo.Descripcion;
                mercancia.Claveunidad = articulo.c_ClaveUnidad.ClaveUnidad;
                mercancia.materialpeligroso = "NO";
                mercancia.PesoKg = 1;
                mercancia.valor = dp.Importe;
                mercancia.clavemoneda = new c_MonedaContext().c_Monedas.Find(prefactura.IDMoneda).ClaveMoneda; //prefactura.c_Moneda.ClaveMoneda;
                listamerca.Add(mercancia);
            }

            //ViewBag.ListEntrega = GetEntregaCliente(cliente.IDCliente);
            //ViewBag.ListOrigen = GetOrigen();



            ViewBag.Listamercancia = listamerca;

            ViewBag.prefactura = prefactura;
            ViewBag.factura = factura;
            ViewBag.cliente = cliente;

            Empresa empresa = new EmpresaContext().empresas.Find(2);
            ViewBag.empresa = empresa;

            //if (perfil != null)
            //{
            //    ViewBag.TienePerfil = true;
            //    ViewBag.CartaPortePerfil = perfil;
            //}

            return View(EncabezadoCartaPorte);




        }
        [HttpPost]
        public ActionResult CrearCartaP(FormCollection collection, ClsCartaporte2 carta)
        {

            bool timbrado = false;
            carta.DistanciaRecorrida = decimal.Parse(collection.Get("DistanciaRecorrida"));
            //FacturaComplementoCartasP facturaCartaPorte = new FacturaCartaPorteContext().Database.SqlQuery<FacturaComplementoCartasP>("select*from FacturaCartaPorte where IDFactura="+carta.IDFactura).ToList().FirstOrDefault();
            //if (facturaCartaPorte !=null)
            //{
            //    timbrado = true;
            //}
            EncPrefactura prefactura = new PrefacturaContext().EncPrefactura.Where(x => (x.NumeroDigital == carta.NoFactura.ToString() && x.SerieDigital == carta.SerieF)).FirstOrDefault();
            Encfacturas encfacturas = new EncfacturasSaldosContext().Database.SqlQuery<Encfacturas>("select*from EncFacturas where numero='" + prefactura.NumeroDigital + "' and serie='" + prefactura.SerieDigital + "'").ToList().FirstOrDefault();
            List<DetPrefactura> detalles = new PrefacturaContext().DetPrefactura.SqlQuery("select * from [DetPrefactura] where IDPrefactura=" + prefactura.IDPrefactura).ToList();


            ClsCartaporte2 factura = new ClsCartaporte2();
            EmpresaContext dbe = new EmpresaContext();

            Empresa emisora = dbe.empresas.Find(2);

            factura.Regimen = emisora.c_RegimenFiscal.ClaveRegimenFiscal.ToString();

            factura.Tipodecombrobante = "I";
            factura.Moneda = prefactura.c_Moneda.ClaveMoneda;
            if (factura.Moneda == "MXN")
            {
                //factura.tipodecambio = "1";
            }
            else
            {
                c_Moneda monedanacional = new c_MonedaContext().c_Monedas.SqlQuery("select * from c_moneda where clavemoneda ='MXN'").ToList()[0];
                //  factura.tipodecambio = dbe.Database.SqlQuery<ClsDatoDecimal>("SELECT [dbo].[GetTipocambio] ('" + DateTime.Now.ToString("yyyy/MM/dd") + "'," + prefactura.IDMoneda + "," + monedanacional.IDMoneda + ") as Dato").ToList()[0].ToString();
                //factura.tipodecambio = prefactura.TipoCambio.ToString();

            }


            //factura.formadepago = prefactura.c_FormaPago.ClaveFormaPago;
            factura.uso = prefactura.c_UsoCFDI.ClaveCFDI;

            FolioCartaPContext dbf = new FolioCartaPContext();

            FolioCartaP folio = dbf.FoliosC.Find(1);

            factura._serie = folio.Serie;
            factura._folio = (folio.Numero + 1).ToString();

            //factura.valoriva = 0.16M;
            //factura.Descuento = prefactura.Descuento;


            factura.Emisora = emisora;

            Empresa Receptor = new Empresa();
            Receptor.RFC = prefactura.Clientes.RFC;
            Receptor.RazonSocial = prefactura.Clientes.Nombre;
            Receptor.Calle = prefactura.Clientes.Calle;
            Receptor.NoExt = prefactura.Clientes.NumExt;
            Receptor.NoInt = prefactura.Clientes.NumInt;
            Receptor.CP = prefactura.Clientes.CP;
            factura.Receptora = Receptor;




            List<MercanciaCP> conceptos = new List<MercanciaCP>();
            decimal PesoTotal = 0;
            int contador = 0;
            int NumTotalMercancias = 0;
            foreach (var detalle in detalles)
            {
                decimal pesoMerca = decimal.Parse(collection.Get("PesoMercancia" + contador));
                MercanciaCP mercancia = new MercanciaCP();
                Articulo articulo = new ArticuloContext().Articulo.Find(detalle.IDArticulo);
                c_ClaveProductoServicio c_Clave = new c_ClaveProdSTCCContext().Database.SqlQuery<c_ClaveProductoServicio>("select cl.* from familia as f inner join c_ClaveProductoServicio as cl on cl.IDProdServ=f.IDProdServ where f.IDFamilia=" + articulo.IDFamilia).ToList().FirstOrDefault();

                mercancia.CodigoFamiliaPS = c_Clave.ClaveProdServ.ToString();
                mercancia.cantidad = detalle.Cantidad;
                mercancia.Descripcion = articulo.Descripcion;
                mercancia.Unidad = articulo.c_ClaveUnidad.Nombre;
                mercancia.Claveunidad = articulo.c_ClaveUnidad.ClaveUnidad;
                mercancia.materialpeligroso = "No";
                mercancia.PesoKg = pesoMerca;
                mercancia.IDIdentificador = contador;
                mercancia.valor = detalle.Importe;
                mercancia.clavemoneda = new c_MonedaContext().c_Monedas.Find(prefactura.IDMoneda).ClaveMoneda; //prefactura.c_Moneda.ClaveMoneda;
                conceptos.Add(mercancia);
                contador++;
                PesoTotal += pesoMerca;
                NumTotalMercancias++;

            }
            List<ConceptosCarta> conceptosCarta = new List<ConceptosCarta>();
            foreach (var detalle in detalles)
            {

                ConceptosCarta mercancia = new ConceptosCarta();
                Articulo articulo = new ArticuloContext().Articulo.Find(detalle.IDArticulo);
                mercancia.Descripcion = articulo.Descripcion;
                mercancia.ID = articulo.Cref;
                mercancia.Unidad = articulo.c_ClaveUnidad.Nombre;
                mercancia.cantidad = detalle.Cantidad;
                mercancia.valorUnitario = detalle.Costo;
                mercancia.Importe = detalle.Importe;
                mercancia.ClaveProdServ = new c_ClaveProdSTCCContext().produc.Find(new FamiliaContext().Familias.Find(articulo.IDFamilia).IDClaveSTCC).Clave;
                mercancia.ClaveUnidad = articulo.c_ClaveUnidad.ClaveUnidad;
                conceptosCarta.Add(mercancia);
                contador++;


            }

            factura.Listaconceptos.conceptos = conceptos;
            factura.Listacon.conceptos = conceptosCarta;
            factura.PesoTotal = PesoTotal;
            factura.NumTotalMercancias = NumTotalMercancias;

            factura.IDTransporte = carta.IDTransporte;
            factura.IDOperador = carta.IDOperador;


            DomicilioDestino destino = new DomicilioDestino();
            Entrega entrega = new EntregaContext().Entregas.Find(carta.IDDomicilioentrega);
            destino.IDUbicacion = "DE" + Completa(entrega.IDEntrega);
            destino.TipoUbicacion = "Destino";
            destino.RFCRemitenteDestinatario = Receptor.RFC;
            destino.FechaHoraSalidadLlegada = carta.FechaLlegada;
            destino.DistanciaRecorrida = carta.DistanciaRecorrida;
            destino.Calle = entrega.CalleEntrega;
            destino.NumeroExterior = entrega.NumExtEntrega;
            destino.NumeroInterior = entrega.NumIntentrega;
            destino.Colonia = entrega.c_Colonia;
            destino.Localidad = entrega.c_Localidad;
            destino.Referencia = entrega.Referencia;
            destino.Municipio = entrega.c_Municipio;
            destino.Estado = entrega.c_Estado;
            destino.Pais = entrega.c_Pais;
            destino.CodigoPostal = entrega.CPEntrega;

            DomicilioOrigen Origen = new DomicilioOrigen();
            Origen vorigen = new OrigenContext().Origen.Find(carta.IDDomicilioOrigen);
            Origen.IDUbicacion = "OR" + Completa(vorigen.IDOrigen);
            Origen.TipoUbicacion = "Origen";
            Origen.RFCRemitenteDestinatario = emisora.RFC;
            Origen.FechaHoraSalidadLlegada = carta.FechaSalida;
            Origen.NumeroInterior = vorigen.NumInt;
            Origen.Calle = vorigen.Calle;
            Origen.NumeroExterior = vorigen.NumExt;
            Origen.Colonia = vorigen.c_Colonia;
            Origen.Localidad = vorigen.c_Localidad;
            Origen.Referencia = vorigen.Referencia;
            Origen.Municipio = vorigen.c_Municipio;
            Origen.Estado = vorigen.c_Estado;
            Origen.Pais = vorigen.c_Pais;
            Origen.CodigoPostal = vorigen.CP;

            InformacionOperador Operador = new InformacionOperador();
            Models.Administracion.VOperadores voperador = new Models.Administracion.ChoferesContext().VOperadores.Find(carta.IDOperador);

            Operador.RFC = voperador.RFC;
            Operador.NoLicencia = voperador.NoLicencia;

            factura.Destino = destino;
            factura.Origen = Origen;
            factura.Operador = Operador;

            ClsCartaporte2 cartaporte2 = new ClsCartaporte2();
            MultiFacturasSDK.MFSDK sdk = factura.construircartaporte();
            MultiFacturasSDK.SDKRespuesta respuesta = factura.timbrar(sdk);




            var reshtml = Server.HtmlEncode(respuesta.Codigo_MF_Texto + "->" + respuesta.CFDI);

            string resp = string.Empty;
            //-----------------------------------------------validacion que no se repita el cfdi -------------------------------------
            bool pasa = false;
            try
            {
                if (respuesta.Codigo_MF_Texto.Contains("OK"))
                {
                    pasa = true;
                    try
                    {
                        FacturaComplementoCartasP facturagrabada = new FacturaCartaPorteContext().FacturaCartaPorte.ToList().Where(x => (x.UUID == respuesta.UUID)).ToList().FirstOrDefault();
                        if (facturagrabada == null)
                        {
                            //return Content("Posiblemente no tiene factura");
                        }
                        else
                        {
                            return Content("UUID Existente");
                        }
                    }
                    catch (Exception err)
                    {
                        pasa = true;
                    }
                    try
                    {
                        Encfacturas facturae = new EncfacturaContext().encfacturas.Where(s => s.UUID == respuesta.UUID).ToList().FirstOrDefault();
                        if (facturae == null)
                        {
                            //throw new Exception("Posbiblemente no tiene factura");
                        }
                        else
                        {
                            return Content("UUID Existente");
                            pasa = false;
                        }
                    }
                    catch (Exception err)
                    {
                        pasa = true;
                    }
                }


            }
            catch (Exception err)
            {

            }


            //----------------------------------------------------------------------------------------------------------------
            int Mes = DateTime.Now.Month;
            int Ann = DateTime.Now.Year;
            if (respuesta.Codigo_MF_Texto.Contains("OK"))
            {
                //GeneradorCartaPorte.CreaPDFCP temp2 = new GeneradorCartaP.CreaPDFCartaP(respuesta.CFDI);
                FacturaComplementoCartasP elemento2 = new FacturaComplementoCartasP();
                try
                {
                    string insert = "Insert into FacturaComplementoCartasP(UUID,Mes,Anno,Fecha,IDFacturaViene, IDOperador,XMLRuta) values ('" + respuesta.UUID + "'," + Mes + "," + Ann + ",sysdatetime()," + encfacturas.ID + "," + carta.IDOperador + ",'" + respuesta.CFDI + "')";

                    db.Database.ExecuteSqlCommand(insert);
                }
                catch (Exception err)
                {

                }
                //elemento2.Fecha = System.DateTime.Parse(temp2._templatePDF.fechaEmisionCFDI);
                //elemento2.IDFacturaViene = factura.IDFactura;
                //elemento2.XMLRuta = respuesta.CFDI;
                //elemento2.Mes = Mes;
                //elemento2.Anno = Ann;
                //elemento2.IDOperador = carta.IDOperador;
                ////elemento2.Fecha = DateTime.Now;
                //elemento2.UUID = respuesta.UUID;

                //new FacturaCartaPorteContext().FacturaCartaPorte.Add(elemento2);
                //new FacturaCartaPorteContext().SaveChanges();

                db.Database.ExecuteSqlCommand("update foliocartaP set numero='" + factura._folio + "'");

                FacturaComplementoCartasP facturagrabada = new FacturaCartaPorteContext().FacturaCartaPorte.ToList().Where(x => (x.UUID == elemento2.UUID)).ToList().FirstOrDefault();


                //GRABA XML 

                try
                {


                    //GeneradorCartaPorte.CreaPDFCP temp = new GeneradorCartaPorte.CreaPDFCP(respuesta.RutaXML.ToString());
                    string fileName = respuesta.UUID + ".xml";

                    EscribeEnArchivoCartaPorte(respuesta.CFDI.ToString(), fileName, Mes, Ann, true);
                    //try
                    //{
                    //    string insert = "Insert into FacturaComplementoCartasP(UUID,Mes,Anno,Fecha,IDFacturaViene, IDOperador,XMLRuta) values ('" + respuesta.UUID + "'," + Mes + "," + Ann + ",sysdatetime()," + encfacturas.ID + ","+carta.IDOperador+",'"+respuesta.CFDI+"')";

                    //    db.Database.ExecuteSqlCommand(insert);
                    //}
                    //catch (Exception err)
                    //{

                    //}
                }
                catch (Exception err)
                {
                    string mensajedeerror = err.Message;
                }
                ///FACTURAS A EMPACAR

                return Content("SI TIMBRO");

            }
            else
            {

                //db.Database.ExecuteSqlCommand("update encprefactura SET Timbrando='False' where IDPrefactura=" + prefactura.IDPrefactura);
                return Content("No timbro");

            }


        }
        public string Completa(int valor) // acompleta a dos digitos el valor
        {
            string cc = valor.ToString();

            cc = valor.ToString("D6");

            return cc;
        }

        [HttpPost]
        public JsonResult GuardarPer(int IDOrigen, int IDEntrega, decimal Distancia, int IDTransporte, int IDOperador, int IDEmisor, int IDFactura)
        {
            Empresa empresa = new EmpresaContext().empresas.Find(2);
            int IDReceptor = empresa.IDEmpresa;

            try
            {
                if (IDEntrega != 0)
                {
                    string insert = "insert into perfilcartaporte(idorigen,iddestino,distanciarecorrida,idtransporte,idoperador,idemisor,idreceptor) " +
                   "values (" + IDOrigen + "," + IDEntrega + "," + Distancia + "," + IDTransporte + "," + IDOperador + "," + IDEmisor + "," + IDReceptor + ")";

                    db.Database.ExecuteSqlCommand(insert);
                }
               
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        public IEnumerable<SelectListItem> GetEntregaCliente(int IDCliente)
        {
            var estado = new ReporitoryEntrega().GetEntregaCliente(IDCliente);
            return estado;

        }


        public IEnumerable<SelectListItem> GetOrigen()
        {
            var estado = new ReporitoryOrigen().GetDomicilioOrigen();
            return estado;

        }


        public JsonResult GetOrigenB(int id) //checar la unidad del articulo
        {
            Origen origen = new OrigenContext().Origen.Find(id);

            DatosOrigen nuevo = new DatosOrigen();
            nuevo.calle = origen.Calle;
            nuevo.colonia = origen.c_Colonia;
            nuevo.cp = origen.CP;
            nuevo.estado = origen.c_Estado;
            nuevo.localidad = origen.c_Localidad;
            nuevo.municipio = origen.c_Municipio;
            nuevo.pais = origen.c_Pais;
            nuevo.referencia = origen.Referencia;
            nuevo.numext = origen.NumExt;
            nuevo.numint = origen.NumInt;

            var result = new { Result = "Successed", obj = nuevo };
            return Json(nuevo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutotransporte(int id) //checar la unidad del articulo
        {
            ParqueVehicular vehiculo = new ParqueVehicularDBContext().ParqueVe.Find(id);

            //DatosOrigen nuevo = new DatosOrigen();
            //nuevo.calle = origen.Calle;
            //nuevo.colonia = origen.c_Colonia;
            //nuevo.cp = origen.CP;
            //nuevo.estado = origen.c_Estado;
            //nuevo.localidad = origen.c_Localidad;
            //nuevo.municipio = origen.c_Municipio;
            //nuevo.pais = origen.c_Pais;
            //nuevo.referencia = origen.Referencia;
            //nuevo.numext = origen.NumExt;
            //nuevo.numint = origen.NumInt;

            //var result = new { Result = "Successed", obj = nuevo };
            return Json(vehiculo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEntregaB(int id) //checar la unidad del articulo
        {
            Entrega entrega = new EntregaContext().Entregas.Find(id);

            DatosEntrega nuevo = new DatosEntrega();
            nuevo.calle = entrega.CalleEntrega;
            nuevo.colonia = entrega.c_Colonia;
            nuevo.cp = entrega.CPEntrega;
            nuevo.estado = entrega.c_Estado;
            nuevo.localidad = entrega.c_Localidad;
            nuevo.municipio = entrega.c_Municipio;
            nuevo.pais = entrega.c_Pais;
            nuevo.referencia = entrega.Referencia;
            nuevo.numext = entrega.NumExtEntrega;
            nuevo.numint = entrega.NumIntentrega;

            //var result = new { Result = "Successed", obj = nuevo };
            return Json(nuevo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOperador(int id) //checar la unidad del articulo
        {
            VOperadores vOperador = new ChoferesContext().Database.SqlQuery<VOperadores>("select*from VOperadores where IDChofer=" + id).ToList().FirstOrDefault();

            DatosOperador nuevo = new DatosOperador();
            nuevo.RFC = vOperador.RFC;
            nuevo.Nombre = vOperador.Nombre;
            nuevo.NoLicencia = vOperador.NoLicencia;
            nuevo.Calle = vOperador.Calle;
            nuevo.c_Colonia = vOperador.c_Colonia;
            nuevo.c_Localidad = vOperador.c_Localidad;
            nuevo.c_Municipio = vOperador.c_Municipio;
            nuevo.c_Estado = vOperador.c_Estado;
            nuevo.c_Pais = vOperador.c_Pais;
            nuevo.Referencia = vOperador.Referencia;
            nuevo.NumInt = vOperador.NumInt;
            nuevo.NumExt = vOperador.NumExt;
            nuevo.CP = vOperador.CP;

            //var result = new { Result = "Successed", obj = nuevo };
            return Json(nuevo, JsonRequestBehavior.AllowGet);
        }


        public static void EscribeEnArchivoCartaPorte(string contenido, string rutaArchivo, int Mes, int Ann, bool sobrescribir = true)
        {

            string nombredecarpeta = Mes + "-" + Ann;
            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/XmlfacturasCartaPorte/" + nombredecarpeta)))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/XmlfacturasCartaPorte/" + nombredecarpeta));
            }
            string archivoxml = System.Web.HttpContext.Current.Server.MapPath("~/XmlfacturasCartaPorte/" + nombredecarpeta + "/" + rutaArchivo);

            //string archivoxml = System.Web.HttpContext.Current.Server.MapPath("~/XmlfacturasCartaPorte/" + rutaArchivo);

            using (FileStream fs = System.IO.File.Create(archivoxml))
            {
                AddText(fs, contenido);
            }

        }
        public ActionResult PDFCartaP(int id)
        {
            // Obtener contenido del archivo
            var elemento = new FacturaCartaPorteContext().FacturaCartaPorte.Single(m => m.ID == id);



            string rutaArchivo = elemento.UUID + ".xml";
            string carpeta = elemento.Mes + "-" + elemento.Anno;
            string fileName = System.Web.HttpContext.Current.Server.MapPath("~/XmlfacturasCartaPorte/" + carpeta + "/" + rutaArchivo);
            string xmlString = System.IO.File.ReadAllText(fileName);

            var empresa = db.Empresa.Single(m => m.IDEmpresa == 2);

            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);



            GeneradorCartaPorte.CreaPDFCP documento = null;
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            FacturaComplementoCartasP archivocotizacion = new FacturaCartaPorteContext().FacturaCartaPorte.Find(id);

            documento = new GeneradorCartaPorte.CreaPDFCP(xmlString, elemento.IDOperador, logoempresa, "Tel. " + empresa.Telefono + "gfinanzas@class-labels.com", true);

            return new FilePathResult(documento.nombreDocumento, contentType);


        }
        
        public ActionResult FacturasCartaPorte(string Numero, string ClieFac, string sortOrder, string currentFilter, int? page, int? PageSize, string Fechainicio, string Fechafinal, string FacPag, string Estado = "A", string SerieFac = "MX")
        {
            //string ConsultaSql = "select * from Encfacturas ";
            string ConsultaSql = "Select top 500 * from dbo.EncfacturasSaldos";
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
                        Filtro = "where Serie='MX'";
                    }
                    else
                    {
                        Filtro += "and  Serie='MX'";
                    }
                    SerieFac = "B";

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
                        Filtro = " where  Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechainicio + " 23:59:59.999' ";
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
                        Filtro = " where  Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999' ";
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


                cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

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
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25" , Selected = true},
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

                int pageNumber = (page ?? 1);
                int pageSize = (PageSize ?? 25);
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


        ////////////////CANCELAR
        ///
        public ActionResult CancelarMotivo(int id, string Viene="")
        {

            //RecepcionContext db1 = new RecepcionContext();

            RegistroCancelacionFacturas elemento = new RegistroCancelacionFacturas();

            elemento.IDFactura = id;
            if (Viene!="")
            {
                elemento.FViene = Viene;
            }
            else
            {
                elemento.FViene = "Factura";
            }


            ViewBag.Motivo = new SelectList(new MotivoCancelacionContext().MotivoCancelacions, "IDCancelacion", "DescripcionCan");



            return View(elemento);
        }

        [HttpPost]
        public ActionResult CancelarMotivo(FormCollection datos)
        {

            //RecepcionContext db1 = new RecepcionContext();
            RegistroCancelacionFacturas motivo = new RegistroCancelacionFacturas();
            motivo.IDFactura = int.Parse(datos.Get("IDFactura"));
            motivo.FViene = datos.Get("FViene");
            motivo.Motivo = int.Parse(datos.Get("Motivo"));

            try
            {
                motivo.FolioFiscal = datos.Get("FolioFiscal");
            }
            catch (Exception err)
            {

            }
            
            try
            {
                var elemento = db.encfacturas.Single(m => m.ID == motivo.IDFactura);

                var stream = new MemoryStream(Encoding.UTF8.GetBytes(elemento.RutaXML));
                StreamReader reader = new StreamReader(stream);
                String contenidoxml = reader.ReadToEnd();

                String cadenaxml = System.Web.HttpContext.Current.Server.MapPath("~/Documentostemporales/cancelar.xml");

                StreamWriter sw = new StreamWriter(cadenaxml, true);

                sw.Write(contenidoxml);
                sw.Close();

                Generador.CreaPDF temp = new Generador.CreaPDF(contenidoxml);

                ///funcion
                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int UserID = userid.Select(s => s.UserID).FirstOrDefault();

                bool salida = false;
                try
                {
                    MotivosCancelacion motivos = new MotivoCancelacionContext().MotivoCancelacions.Find(motivo.Motivo);
                    string ClaveMotivo = motivos.ClaveCan;

                    if (ClaveMotivo == "01")
                    {
                        if (motivo.FolioFiscal == null)
                        {
                            return Content("Falta agregar Folio Fiscal ");
                        }
                    }
                    else
                    {
                        motivo.FolioFiscal = "";
                    }
                    if (motivo.FViene=="CartaP")
                    {
                        FacturaComplementoCartasP cartasP = new FacturaCartaPorteContext().Database.SqlQuery<FacturaComplementoCartasP>("select*from FacturaComplementoCartasP where estado='A' and idfactura="+motivo.IDFactura).FirstOrDefault();
                        salida = new ClsFactura().cancela40(cartasP.UUID, motivo.FolioFiscal, ClaveMotivo, motivo.IDFactura, UserID, "CartaP");
                        if (salida)
                        {
                            db.Database.ExecuteSqlCommand("update FacturaComplementoCartasP set estado='A' where idfactura=" + motivo.IDFactura);
                        }
                    }
                    else
                    {
                        salida = new ClsFactura().cancela40(temp._templatePDF.folioFiscalUUID, motivo.FolioFiscal, ClaveMotivo, motivo.IDFactura, UserID, "Factura");

                    }
                }
                catch (Exception ERR)
                {

                }
                if (salida)
                {
                 //   List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                 //int UserID = userid.Select(s => s.UserID).FirstOrDefault();

                 //   string insert = "insert into EstadoFacturasSat (Estado, IDFactura,Fecha,Usario) values " +
                 //       "('C'," + motivo.IDFactura + ",sysdatetime()," + UserID + ")";

                 //   db.Database.ExecuteSqlCommand(insert);


                }
                    //if (salida)
                    //{
                    //    if (temp._templatePDF.UUIDrelacionados.Count() > 0 && (temp._templatePDF.TipoDeRelacion == "03" || temp._templatePDF.TipoDeRelacion == "01"))
                    //    {
                    //        try
                    //        {
                    //            string folio = temp._templatePDF.UUIDrelacionados[0].UUID;
                    //            decimal total = temp._templatePDF.total;
                    //            Encfacturas factura = new EncfacturaContext().encfacturas.Where(s => s.UUID == folio).FirstOrDefault();
                    //            new EncfacturaContext().Database.ExecuteSqlCommand("update saldofactura set ImportePagado=(ImportePagado - " + total + ") where serie ='" + factura.Serie + "' and numero =" + factura.Numero);
                    //            new EncfacturaContext().Database.ExecuteSqlCommand("update saldofactura set  ImporteSaldoInsoluto=(Total -importepagado) where serie ='" + factura.Serie + "' and numero =" + factura.Numero);

                    //            new EncfacturaContext().Database.ExecuteSqlCommand("update saldofactura set  ImporteSaldoAnterior=(ImporteSaldoAnterior + " + total + ") where serie ='" + factura.Serie + "' and numero =" + factura.Numero);

                    //        }
                    //        catch (Exception err)
                    //        {

                    //        }
                    //    }
                    //    else
                    //    {
                    //        string cadenasaldo = "update dbo.saldofactura set importesaldoinsoluto=0 where idfactura=" + motivo.IDFactura;
                    //        db.Database.ExecuteSqlCommand(cadenasaldo);
                    //    }
                    //    string cadenasql = "update encfacturas set Estado='C' where ID=" + motivo.IDFactura;

                    //    db.Database.ExecuteSqlCommand(cadenasql);
                    //    string cadenasale = "update encprefactura set idfacturadigital=0,seriedigital='',numerodigital=0,status='Activo' where idfacturadigital= " + motivo.IDFactura;
                    //    db.Database.ExecuteSqlCommand(cadenasale);



                    try
                    {
           

                    string cadena = "insert into RegistroCancelacionFacturas(IDFactura, Fecha, Usuario,FolioFiscal,FViene,Motivo) values (" + motivo.IDFactura + ",SYSDATETIME()," + UserID + ",'" + motivo.FolioFiscal + "','" + motivo.FViene + "'," + motivo.Motivo + ")";
                    db.Database.ExecuteSqlCommand(cadena);

                }
                catch (Exception err)
                {

                }
                //}


                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                return RedirectToAction("Index");
            }
        }
        public JsonResult DatosCancelar(int id, string motivo, string uuidsusti) //checar la unidad del articulo
        {
            EncfacturaContext db = new EncfacturaContext();
            Encfacturas factura = db.encfacturas.Single(m => m.ID == id);

            Certificados certi = new CertificadosContext().certificados.Find(2);
            Empresa empre = new EmpresaContext().empresas.Find(2);


            DatosCancelacion nuevo = new DatosCancelacion();
            nuevo.usuario = certi.UsuarioMultifacturas;
            nuevo.usuario = certi.PassCertificado;
            nuevo.accion = "Cacelar";
            nuevo.produccion = "SI";
            nuevo.uuid = factura.UUID;
            nuevo.rfc = empre.RFC;
            nuevo.password = motivo;
            nuevo.folioSustitucion = uuidsusti;
            nuevo.b64Cer = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certi.Nombredelcertificado)).ToString();
            nuevo.b64Key = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/CertificadosClientes/" + certi.Nombredelkey)).ToString();



            //var result = new { Result = "Successed", obj = nuevo };
            return Json(nuevo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EnviarCorreo(string Numero, string SerieFac, string ClieFac, string sortOrder, string currentFilter, string Fechainicio, string Fechafinal, string FacPag, string Estado = "A")
        {
            ViewBag.Numero = Numero;
            ViewBag.SerieFac = SerieFac;
            ViewBag.ClieFac = ClieFac;
            ViewBag.sortOrder = sortOrder;
            ViewBag.currentFilter = currentFilter;
            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;
            ViewBag.FacPag = FacPag;
            ViewBag.Estado = Estado;

            ViewBag.Ruta = "";
            return View();
        }
        [HttpPost]

        public FileResult DescargaMasivaXML(string Numero, string SerieFac, string ClieFac, string sortOrder, string currentFilter, string Fechainicio, string Fechafinal, string FacPag, string Estado = "A", string[] Correo = null)
        {
            //string ConsultaSql = "select * from Encfacturas ";
            string ConsultaSql = "Select * from dbo.EncfacturasSaldos";
            string cadenaSQl = string.Empty;
            string carpetaZip = "";
            try
            {

                //Buscar Facturas: Pagadas o no pagadas


                string ConsultaSqlResumen = "select Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from EncfacturasSaldos ";
                string ConsultaAgrupado = "group by Moneda order by Moneda ";
                string Filtro = string.Empty;

                string Orden = " order by fecha desc , serie , numero desc ";

                ///tabla filtro: serie
                ///
                if (SerieFac != "Todas")
                {
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
                            Filtro = "where Serie='B'";
                        }
                        else
                        {
                            Filtro += "and  Serie='B'";
                        }
                        SerieFac = "B";

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

                        if (ClieFac != "Todas")
                        {
                            Filtro = "where Nombre_cliente='" + ClieFac + "'";
                        }
                    }
                    else
                    {
                        if (ClieFac != "Todas")
                        {
                            Filtro += "and  Nombre_cliente='" + ClieFac + "'";
                        }

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
                        Filtro = " where  Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechainicio + " 23:59:59.999' ";
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
                        Filtro = " where  Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999' ";
                    }
                    else
                    {
                        Filtro += " and Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999'";
                    }
                }



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


                cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

                var elementos = dbs.Database.SqlQuery<EncfacturasSaldos>(cadenaSQl).ToList();
                string carpeta = "ListadoFacturas-" + DateTime.Now.Month + "-" + DateTime.Now.Year + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;


                try
                {
                    int contador = 0;
                    foreach (EncfacturasSaldos factura in elementos)
                    {
                        if (factura.RutaXML.Length > 1)
                        {
                            Generador.CreaPDF temp = new Generador.CreaPDF(factura.RutaXML.ToString());
                            string fileName = "Factura " + temp._templatePDF.serie + " " + temp._templatePDF.folio;



                            try
                            {
                                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/CarpetasFacturas/" + carpeta + "/" + fileName + ".xml")))
                                {
                                    //si existe deserealizarlo y obtener el uuid
                                    // si el uuid del xml que existe es igual al uuid del for, que reemplace ese XML
                                    contador++;
                                    fileName = fileName + "-" + contador;

                                }
                                else
                                {
                                    contador = 0;
                                }
                                fileName = fileName + ".xml";
                                EscribeArchivoXML(factura.RutaXML, fileName, carpeta, true);

                            }
                            catch (Exception err)
                            {

                            }

                        }
                    }
                    carpetaZip = carpeta + ".zip";


                    //string comando = "winrar a " + System.Web.HttpContext.Current.Server.MapPath("~/" + carpetaZip) + " " + System.Web.HttpContext.Current.Server.MapPath("~/" + carpeta);


                    //string path = System.Web.HttpContext.Current.Server.MapPath("~/CMD.txt");
                    //string Text = "ejecut";
                    //System.IO.File.WriteAllText(path, Text);
                    try
                    {
                        string archivoOriginal = System.Web.HttpContext.Current.Server.MapPath("~/CarpetasFacturas/" + carpeta);
                        string directotorioDestino = System.Web.HttpContext.Current.Server.MapPath("~/CarpetasFacturas/" + carpetaZip);

                       //Comprimir
                        ZipFile.CreateFromDirectory(archivoOriginal, directotorioDestino);

                    }
                    catch (Exception err)
                    {
                        //throw new Exception("ejecuto el comando" + comando + " " + err);

                    }
                    //ExecuteCommand(comando, carpetaZip);

                    ViewBag.Error = "Termine";
                }
                catch (Exception err)
                {
                    ViewBag.Error = "Este error ocurrio" + err.Message;
                }




            }
            catch (Exception err)
            {

            }

            var ruta = "";
            string contentType = "";
            try
            {
                ruta = System.Web.HttpContext.Current.Server.MapPath("~/" + carpetaZip);
                contentType = System.Net.Mime.MediaTypeNames.Application.Zip;

                //return new FilePathResult(ruta, contentType);

                // utilizando la librería DotNetZip 

                // string nombrearchivo = Path.GetFileName(carpetaZip).Trim();
                // byte[] fl = System.IO.File.ReadAllBytes(Server.MapPath("~/" + nombrearchivo));
                //return File(fl, System.Net.Mime.MediaTypeNames.Application.Octet, carpetaZip);

                //if (System.IO.File.Exists(ruta))
                //{

                //    return RedirectToAction("EnviarCorreo", new { ruta = ruta });

                //}



            }
            catch (Exception err)
            {

            }


            var empresa = db.Empresa.Single(m => m.IDEmpresa == 2);

            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            //string ruta = "";

            try
            {

                string correo = string.Empty;
                string password = string.Empty;
                string dominio = string.Empty;
                string servidor = "mail.class-labels.com";
                int puerto = 587;
                string nombrecorreo = empresa.RazonSocial;
                string asunto = "Facturas.Zip";
                string titulo = "";
                string cuerpo = "Sus facturas han sido descargadas;\n \n" + ruta;
                string firma = "";
                string copiaoculta = string.Empty;
                bool enablessl = true;
                string enablesslstring = "false";

                XmlDocument xmail = new XmlDocument();
                xmail.Load(System.Web.HttpContext.Current.Server.MapPath("~/configzipfactura.xml"));


                XmlNode mailnode = xmail.FirstChild;
                try
                {
                    foreach (XmlNode elementomail in mailnode.ChildNodes)
                    {

                        if (elementomail.Name == "correo")
                        {
                            correo = elementomail.InnerText;
                        }
                        if (elementomail.Name == "password")
                        {
                            password = elementomail.InnerText;
                        }
                        if (elementomail.Name == "dominio")
                        {
                            dominio = elementomail.InnerText;
                        }
                        if (elementomail.Name == "servidor")
                        {
                            servidor = elementomail.InnerText;
                        }

                        if (elementomail.Name == "puerto")
                        {
                            puerto = Int32.Parse(elementomail.InnerText);
                        }

                        if (elementomail.Name == "nombre")
                        {
                            nombrecorreo = elementomail.InnerText;
                        }

                        if (elementomail.Name == "asunto")
                        {
                            asunto = elementomail.InnerText;
                        }

                        if (elementomail.Name == "Titulo")
                        {
                            titulo = elementomail.InnerText;
                        }
                        if (elementomail.Name == "cuerpo")
                        {
                            cuerpo = elementomail.InnerText;
                        }
                        if (elementomail.Name == "copiaoculta")
                        {
                            copiaoculta = elementomail.InnerText;
                        }
                        if (elementomail.Name == "EnableSsl")
                        {
                            enablesslstring = elementomail.InnerText;
                            if (enablesslstring == "true")
                            {
                                enablessl = true;
                            }
                            else
                            {
                                enablessl = false;
                            }
                        }

                    }
                }
                catch (Exception err)
                {
                    string error = err.Message;
                }



                try
                {


                    SmtpClient mySmtpClient = new SmtpClient();

                    // set smtp-client with basicAuthentication
                    mySmtpClient.UseDefaultCredentials = false;
                    System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential(correo, password);

                    mySmtpClient.EnableSsl = enablessl;
                    mySmtpClient.Host = servidor;
                    mySmtpClient.Port = puerto;
                    mySmtpClient.Credentials = basicAuthenticationInfo;

                    MailAddress from = new MailAddress(correo, nombrecorreo);
                    // MailAddress to = new MailAddress(Cliente.CorreoCfdi, elemento.Nombre_cliente);


                    string clientes = correo.Replace(';', ',');
                    string[] mails = clientes.Split(',');


                    if (Correo.Length == 0)
                    {
                        throw new Exception("No hay Correos registrados");
                    }

                    MailMessage myMail = new MailMessage();
                    try
                    {
                        for (int it = 0; it < Correo.Length; it++)
                        {
                            myMail.To.Add(Correo[it]);
                        }
                    }
                    catch (Exception err)
                    {
                        //return Content("<html><body>El mail de destinatario es incorrecto</body></html>", "text/html");

                    }


                    //Con copia a 
                    myMail.IsBodyHtml = true;
                    myMail.From = from;


                    // set subject and encoding

                    myMail.Bcc.Add(copiaoculta);

                    myMail.Subject = asunto;
                    myMail.SubjectEncoding = Encoding.UTF8;


                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    sb.Append("<html>");
                    sb.Append("<head><title>" + titulo + "</title></ head >");

                    sb.Append("</head>");
                    sb.Append("<body>");
                    sb.Append("<h4>" + cuerpo + "  </h4>");
                    sb.Append("<h5><a href = " + dominio + "CarpetasFacturas" + "/" + carpetaZip + " > Descargar Facturas</a></h5>");
                    //sb.Append("<h6>" + firma + "</h6 >" + correofirma);

                    sb.Append("</body></html>");
                    // set body-message and encoding
                    myMail.Body = sb.ToString();
                    //_memoryStream.Position = 0;
                    //myMail.Attachments.Add(new Attachment(documento.nombreDocumento));

                    //string archivoxml = System.Web.HttpContext.Current.Server.MapPath("~/Xmlfacturas/" + elemento.UUID + ".xml");
                    // myMail.Attachments.Add(new Attachment(ruta));


                    myMail.BodyEncoding = Encoding.UTF8;
                    // text or html
                    myMail.IsBodyHtml = true;

                    mySmtpClient.Send(myMail);




                    //return View();
                }
                catch (Exception ERR2)
                {
                    string Mensaje = ERR2.Message;
                    ViewBag.Mensajeerror = "No Fue Posible Enviar el Correo";
                    //return View();
                }


            }
            catch (Exception err)
            {

            }





            var stream = new MemoryStream(Encoding.UTF8.GetBytes(""));

            return File(stream, "text/plain", "DescargaMasiva.Pol");

            ////return File(fl, System.Net.Mime.MediaTypeNames.Application.Octet, carpetaZip);
            ///



            //return RedirectToAction("Index", "FacturaAll");
        }
        public static void EscribeArchivoXML(string contenido, string rutaArchivo, string carpeta, bool sobrescribir = true)
        {
            string nombredecarpeta = carpeta;
            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/CarpetasFacturas/" + nombredecarpeta)))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/CarpetasFacturas/" + nombredecarpeta));
            }
            string archivoxml = System.Web.HttpContext.Current.Server.MapPath("~/CarpetasFacturas/" + nombredecarpeta + "/" + rutaArchivo);

            using (FileStream fs = System.IO.File.Create(archivoxml))
            {
                AddText(fs, contenido);
            }

        }


        public JsonResult VerMotivo(int id) //checar la unidad del articulo
        {
            bool habilitar = false;
            MotivosCancelacion entrega = new MotivoCancelacionContext().MotivoCancelacions.Find(id);
            if (entrega.IDCancelacion == 2)
            {
                habilitar = true;
            }


            //var result = new { Result = "Successed", obj = nuevo };
            return Json(habilitar, JsonRequestBehavior.AllowGet);
        }



        public ActionResult ConsultaEstadoFactura(int IDFactura)
        {
            Encfacturas encfacturas = new EncfacturaContext().encfacturas.Find(IDFactura);
            string rutaXML = System.Web.HttpContext.Current.Server.MapPath("~/XMLfacturas/" + encfacturas.UUID + ".xml");

            MultiFacturasSDK.SDKRespuesta respuesta = new ClsFactura().consultaEstadoFactura(rutaXML);
            ViewBag.Status = "";

            string texto = string.Empty;

            string archivoTXT = System.Web.HttpContext.Current.Server.MapPath("~/sdk2/timbrados/factura_respuesta.ini");
            StreamReader re = new StreamReader(archivoTXT);
            string entrada = null;

            while ((entrada = re.ReadLine()) != null)
            {
                if (entrada.Contains("Estado"))
                {
                    string[] array = entrada.Split('=');
                    texto = array[1];
                }

            }

            ViewBag.Status = texto;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();

            string insert = "insert into EncFacturaCan (IDFactura ,EstadoFactura,FechaConsulta,EstadoCFDI, IDUsuarioConsulta) values ('" + IDFactura + "','" + encfacturas.Estado + "',sysdatetime(),'" + texto + "','" + UserID + "')";
            db.Database.ExecuteSqlCommand(insert);

            return RedirectToAction("EstadosSat", "FacturasCanceladas", new { id = IDFactura });

        }

        public ActionResult XMLAcuse(int? IDFactura)
        {
            AcuseCancelacionF elemento = new EstadoFactSATContext().Database.SqlQuery<AcuseCancelacionF>("select*from AcuseCancelacionF where idfactura=" + IDFactura).ToList().FirstOrDefault();
            Encfacturas encfacturas = new EncfacturaContext().encfacturas.Find(IDFactura);
            string nombreArchivo = "Acuse" + encfacturas.Serie + " " + encfacturas.Numero + ".xml";

            string ruta = System.Web.HttpContext.Current.Server.MapPath("~/XMLAcuseCF/" + nombreArchivo);
            try
            {
                //Pass the filepath and filename to the StreamWriter Constructor

                StreamWriter sw = new StreamWriter(ruta);
                //Write a line of text
                sw.WriteLine(elemento.Acuse);
                //Write a second line of text
                //sw.WriteLine("From the StreamWriter class");
                //Close the file
                sw.Close();



            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            string contentType = System.Net.Mime.MediaTypeNames.Text.Xml;
            return File(ruta, contentType);


        }
        AcuseCancelacionFContext dbo = new AcuseCancelacionFContext();

        public ActionResult DescargarPDFAcuse(int? id)
        {
            var elemento = dbo.AcuseCancelacionFac.Single(m => m.IDFactura == id);

            string xmlString = elemento.Acuse;
            //string fileName = System.Web.HttpContext.Current.Server.MapPath("~/Xmlfacturas/" + rutaArchivo);
            // string xmlString = System.IO.File.ReadAllText(rutaArchivo);

            var empresa = db.Empresa.Single(m => m.IDEmpresa == 2);

            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);



            List<AcuseCancelacionF> prefactura = new AcuseCancelacionFContext().Database.SqlQuery<AcuseCancelacionF>("select * from AcuseCancelacionF where IDFactura=" + id).ToList();
            AcuseCancelacion.CreaPDF documento = null;
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            if (prefactura.Count == 0)  /// checa prefactura
            {
                documento = new AcuseCancelacion.CreaPDF(xmlString, logoempresa);

                return new FilePathResult(documento.nombreDocumento, contentType);
            }
            else
            {
                AcuseCancelacionF prefact = prefactura[0];
                documento = new AcuseCancelacion.CreaPDF(xmlString, logoempresa, prefact);

                return new FilePathResult(documento.nombreDocumento, contentType);
            }

        }


        public ActionResult ReporteFechasCompleto()
        {
            fechasReporteFac elemento = new fechasReporteFac();


            return View(elemento);
        }


        [HttpPost]
        public ActionResult ReporteFechasCompleto(fechasReporteFac modelo, FormCollection coleccion)
        {
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            string cual = coleccion.Get("Enviar");

            EncFacturaOfVenContext dbe = new EncFacturaOfVenContext();
            string cadena = "select * from [dbo].[EncfacturasSaldos] where Fecha >= '" + FI + "' and Fecha <='" + FF + "'";
            var facturassaldos = dbe.Database.SqlQuery<EncfacturasSaldos>(cadena).ToList();
            ViewBag.facturassaldos = facturassaldos;
            ExcelPackage Ep = new ExcelPackage();
            //Crear la hoja y poner el nombre de la pestaña del libro
            var Sheet = Ep.Workbook.Worksheets.Add("Facturas");

            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:X1"].Style.Font.Size = 20;
            Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:X3"].Style.Font.Bold = true;
            Sheet.Cells["A1:X1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:X1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Facturas");

            row = 2;
            Sheet.Cells["A1:X1"].Style.Font.Size = 12;
            Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:X1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A2:X2"].Style.Font.Bold = true;
            //Subtitulo según el filtrado del periodo de datos
            row = 2;
            Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
            Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            Sheet.Cells[string.Format("B2", row)].Value = FI;
            Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
            Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            Sheet.Cells[string.Format("E2", row)].Value = FF;
            //En la fila3 se da el formato a el encabezado
            row = 3;
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 10;
            Sheet.Cells["A3:X3"].Style.Font.Bold = true;
            Sheet.Cells["A3:X3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A3:X3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            // Se establece el nombre que identifica a cada una de las columnas de datos.
            Sheet.Cells["A3"].RichText.Add("Serie");
            Sheet.Cells["B3"].RichText.Add("Numero");
            Sheet.Cells["C3"].RichText.Add("Fecha");
            Sheet.Cells["D3"].RichText.Add("Cliente");
            Sheet.Cells["E3"].RichText.Add("Subtotal");
            Sheet.Cells["F3"].RichText.Add("Iva");
            Sheet.Cells["G3"].RichText.Add("Total");
            Sheet.Cells["H3"].RichText.Add("Moneda");
            Sheet.Cells["I3"].RichText.Add("Tipo de Cambio");
            Sheet.Cells["J3"].RichText.Add("Metodo de pago");
            Sheet.Cells["K3"].RichText.Add("UUID");
            Sheet.Cells["L3"].RichText.Add("Estado");
            Sheet.Cells["M3"].RichText.Add("Pagada");
            Sheet.Cells["N3"].RichText.Add("Importe Pagado");
            Sheet.Cells["O3"].RichText.Add("Saldo Insoluto");
            Sheet.Cells["P3"].RichText.Add("Fecha de Revisión");
            Sheet.Cells["Q3"].RichText.Add("Fecha de Vencimiento");
            //Sheet.Cells["R3"].RichText.Add("Telefono");



            //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
            //Sheet.Cells["A3"].Value = "Serie";

            //Aplicar borde doble al rango de celdas A3:Q3
            Sheet.Cells["A3:X3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
            // Se establecen los formatos para las celdas: Fecha, Moneda
            row = 4;
            Sheet.Cells.Style.Font.Bold = false;
            foreach (EncfacturasSaldos item in facturassaldos)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.Serie;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Numero;
                Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Nombre_cliente;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Subtotal;
                Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("F{0}", row)].Value = item.IVA;
                Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Moneda;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.TC;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.IDMetododepago;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.UUID;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.Estado;
                //Como item.pagada es de tipo booleano (verdadero o falso), se imprime en pantalla (si o no), según corresponda.
                if (item.pagada == false)
                {
                    Sheet.Cells[string.Format("M{0}", row)].Value = "No";
                }
                else
                {
                    Sheet.Cells[string.Format("M{0}", row)].Value = "Si";
                }
                Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("N{0}", row)].Value = item.ImportePagado;
                Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("O{0}", row)].Value = item.ImporteSaldoInsoluto;

                Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("P{0}", row)].Value = item.FechaRevision;
                Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.FechaVencimiento;
                //Sheet.Cells[string.Format("R{0}", row)].Value = item.Telefono;

                row++;
            }

            //HOJA 2
            Sheet = Ep.Workbook.Worksheets.Add("NOTAS DE CREDITO");

            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:X1"].Style.Font.Size = 20;
            Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:X3"].Style.Font.Bold = true;
            Sheet.Cells["A1:X1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:X1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Facturas");

            row = 2;
            Sheet.Cells["A1:X1"].Style.Font.Size = 12;
            Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:X1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A2:X2"].Style.Font.Bold = true;
            //Subtitulo según el filtrado del periodo de datos
            row = 2;
            Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
            Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            Sheet.Cells[string.Format("B2", row)].Value = FI;
            Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
            Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            Sheet.Cells[string.Format("E2", row)].Value = FF;
            //En la fila3 se da el formato a el encabezado
            row = 3;
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 10;
            Sheet.Cells["A3:X3"].Style.Font.Bold = true;
            Sheet.Cells["A3:X3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A3:X3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            // Se establece el nombre que identifica a cada una de las columnas de datos.
            Sheet.Cells["A3"].RichText.Add("Serie");
            Sheet.Cells["B3"].RichText.Add("Numero");
            Sheet.Cells["C3"].RichText.Add("Fecha");
            Sheet.Cells["D3"].RichText.Add("Cliente");
            Sheet.Cells["E3"].RichText.Add("Subtotal");
            Sheet.Cells["F3"].RichText.Add("Iva");
            Sheet.Cells["G3"].RichText.Add("Total");
            Sheet.Cells["H3"].RichText.Add("Moneda");
            Sheet.Cells["I3"].RichText.Add("Tipo de Cambio");
            Sheet.Cells["J3"].RichText.Add("Metodo de pago");
            Sheet.Cells["K3"].RichText.Add("UUID");
            Sheet.Cells["L3"].RichText.Add("Estado");
            Sheet.Cells["M3"].RichText.Add("Pagada");
            Sheet.Cells["N3"].RichText.Add("Importe Pagado");
            Sheet.Cells["O3"].RichText.Add("Saldo Insoluto");
            Sheet.Cells["P3"].RichText.Add("Fecha de Revisión");
            Sheet.Cells["Q3"].RichText.Add("Fecha de Vencimiento");
            //Sheet.Cells["R3"].RichText.Add("Telefono");

            //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
            //Sheet.Cells["A3"].Value = "Serie";

            //Aplicar borde doble al rango de celdas A3:Q3
            Sheet.Cells["A3:X3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
            // Se establecen los formatos para las celdas: Fecha, Moneda
            row = 4;
            Sheet.Cells.Style.Font.Bold = false;
            cadena = "select * from [dbo].[EncfacturasSaldos] where serie='N' and Fecha >= '" + FI + "' and Fecha <='" + FF + "'";
            facturassaldos = dbe.Database.SqlQuery<EncfacturasSaldos>(cadena).ToList();

            foreach (EncfacturasSaldos item in facturassaldos)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.Serie;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Numero;
                Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Nombre_cliente;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Subtotal;
                Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("F{0}", row)].Value = item.IVA;
                Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Moneda;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.TC;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.IDMetododepago;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.UUID;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.Estado;
                //Como item.pagada es de tipo booleano (verdadero o falso), se imprime en pantalla (si o no), según corresponda.
                if (item.pagada == false)
                {
                    Sheet.Cells[string.Format("M{0}", row)].Value = "No";
                }
                else
                {
                    Sheet.Cells[string.Format("M{0}", row)].Value = "Si";
                }
                Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("N{0}", row)].Value = item.ImportePagado;
                Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("O{0}", row)].Value = item.ImporteSaldoInsoluto;

                Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("P{0}", row)].Value = item.FechaRevision;
                Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.FechaVencimiento;
                //Sheet.Cells[string.Format("R{0}", row)].Value = item.Telefono;


                row++;
            }
            //HOJA 3
            Sheet = Ep.Workbook.Worksheets.Add("ANTICIPOS");

            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:X1"].Style.Font.Size = 20;
            Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:X3"].Style.Font.Bold = true;
            Sheet.Cells["A1:X1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:X1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Facturas");

            row = 2;
            Sheet.Cells["A1:X1"].Style.Font.Size = 12;
            Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:X1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A2:X2"].Style.Font.Bold = true;
            //Subtitulo según el filtrado del periodo de datos
            row = 2;
            Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
            Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            Sheet.Cells[string.Format("B2", row)].Value = FI;
            Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
            Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            Sheet.Cells[string.Format("E2", row)].Value = FF;
            //En la fila3 se da el formato a el encabezado
            row = 3;
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 10;
            Sheet.Cells["A3:X3"].Style.Font.Bold = true;
            Sheet.Cells["A3:X3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A3:X3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            // Se establece el nombre que identifica a cada una de las columnas de datos.
            Sheet.Cells["A3"].RichText.Add("Serie");
            Sheet.Cells["B3"].RichText.Add("Numero");
            Sheet.Cells["C3"].RichText.Add("Fecha");
            Sheet.Cells["D3"].RichText.Add("Cliente");
            Sheet.Cells["E3"].RichText.Add("Subtotal");
            Sheet.Cells["F3"].RichText.Add("Iva");
            Sheet.Cells["G3"].RichText.Add("Total");
            Sheet.Cells["H3"].RichText.Add("Moneda");
            Sheet.Cells["I3"].RichText.Add("Tipo de Cambio");
            Sheet.Cells["J3"].RichText.Add("Metodo de pago");
            Sheet.Cells["K3"].RichText.Add("UUID");
            Sheet.Cells["L3"].RichText.Add("Estado");
            Sheet.Cells["M3"].RichText.Add("Pagada");
            Sheet.Cells["N3"].RichText.Add("Importe Pagado");
            Sheet.Cells["O3"].RichText.Add("Saldo Insoluto");
            Sheet.Cells["P3"].RichText.Add("Fecha de Revisión");
            Sheet.Cells["Q3"].RichText.Add("Fecha de Vencimiento");
            //Sheet.Cells["R3"].RichText.Add("Telefono");

            //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
            //Sheet.Cells["A3"].Value = "Serie";

            //Aplicar borde doble al rango de celdas A3:Q3
            Sheet.Cells["A3:X3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
            // Se establecen los formatos para las celdas: Fecha, Moneda
            row = 4;
            Sheet.Cells.Style.Font.Bold = false;
            cadena = "select * from [dbo].[EncfacturasSaldos] where serie='A' and Fecha >= '" + FI + "' and Fecha <='" + FF + "'";
            facturassaldos = dbe.Database.SqlQuery<EncfacturasSaldos>(cadena).ToList();

            foreach (EncfacturasSaldos item in facturassaldos)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.Serie;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Numero;
                Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Nombre_cliente;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Subtotal;
                Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("F{0}", row)].Value = item.IVA;
                Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Moneda;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.TC;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.IDMetododepago;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.UUID;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.Estado;
                //Como item.pagada es de tipo booleano (verdadero o falso), se imprime en pantalla (si o no), según corresponda.
                if (item.pagada == false)
                {
                    Sheet.Cells[string.Format("M{0}", row)].Value = "No";
                }
                else
                {
                    Sheet.Cells[string.Format("M{0}", row)].Value = "Si";
                }
                Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("N{0}", row)].Value = item.ImportePagado;
                Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("O{0}", row)].Value = item.ImporteSaldoInsoluto;

                Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("P{0}", row)].Value = item.FechaRevision;
                Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.FechaVencimiento;
                //Sheet.Cells[string.Format("R{0}", row)].Value = item.Telefono;


                row++;
            }

            //HOJA 4
            Sheet = Ep.Workbook.Worksheets.Add("INGRESOS MXN");

            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:X1"].Style.Font.Size = 20;
            Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:X3"].Style.Font.Bold = true;
            Sheet.Cells["A1:X1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:X1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Facturas");

            row = 2;
            Sheet.Cells["A1:X1"].Style.Font.Size = 12;
            Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:X1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A2:X2"].Style.Font.Bold = true;
            //Subtitulo según el filtrado del periodo de datos
            row = 2;
            Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
            Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            Sheet.Cells[string.Format("B2", row)].Value = FI;
            Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
            Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            Sheet.Cells[string.Format("E2", row)].Value = FF;
            //En la fila3 se da el formato a el encabezado
            row = 3;
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 10;
            Sheet.Cells["A3:X3"].Style.Font.Bold = true;
            Sheet.Cells["A3:X3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A3:X3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            // Se establece el nombre que identifica a cada una de las columnas de datos.
            Sheet.Cells["A3"].RichText.Add("Serie");
            Sheet.Cells["B3"].RichText.Add("Numero");
            Sheet.Cells["C3"].RichText.Add("Fecha");
            Sheet.Cells["D3"].RichText.Add("Cliente");
            Sheet.Cells["E3"].RichText.Add("Subtotal");
            Sheet.Cells["F3"].RichText.Add("Iva");
            Sheet.Cells["G3"].RichText.Add("Total");
            Sheet.Cells["H3"].RichText.Add("Moneda");
            Sheet.Cells["I3"].RichText.Add("Tipo de Cambio");
            Sheet.Cells["J3"].RichText.Add("Metodo de pago");
            Sheet.Cells["K3"].RichText.Add("UUID");
            Sheet.Cells["L3"].RichText.Add("Estado");
            Sheet.Cells["M3"].RichText.Add("Pagada");
            Sheet.Cells["N3"].RichText.Add("Importe Pagado");
            Sheet.Cells["O3"].RichText.Add("Saldo Insoluto");
            Sheet.Cells["P3"].RichText.Add("Fecha de Revisión");
            Sheet.Cells["Q3"].RichText.Add("Fecha de Vencimiento");
            //Sheet.Cells["R3"].RichText.Add("Telefono");

            //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
            //Sheet.Cells["A3"].Value = "Serie";

            //Aplicar borde doble al rango de celdas A3:Q3
            Sheet.Cells["A3:X3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
            // Se establecen los formatos para las celdas: Fecha, Moneda
            row = 4;
            Sheet.Cells.Style.Font.Bold = false;
            cadena = "select * from [dbo].[EncfacturasSaldos] where moneda='MXN' and Fecha >= '" + FI + "' and Fecha <='" + FF + "'";
            facturassaldos = dbe.Database.SqlQuery<EncfacturasSaldos>(cadena).ToList();

            foreach (EncfacturasSaldos item in facturassaldos)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.Serie;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Numero;
                Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Nombre_cliente;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Subtotal;
                Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("F{0}", row)].Value = item.IVA;
                Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Moneda;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.TC;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.IDMetododepago;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.UUID;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.Estado;
                //Como item.pagada es de tipo booleano (verdadero o falso), se imprime en pantalla (si o no), según corresponda.
                if (item.pagada == false)
                {
                    Sheet.Cells[string.Format("M{0}", row)].Value = "No";
                }
                else
                {
                    Sheet.Cells[string.Format("M{0}", row)].Value = "Si";
                }
                Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("N{0}", row)].Value = item.ImportePagado;
                Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("O{0}", row)].Value = item.ImporteSaldoInsoluto;

                Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("P{0}", row)].Value = item.FechaRevision;
                Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.FechaVencimiento;
                //Sheet.Cells[string.Format("R{0}", row)].Value = item.Telefono;


                row++;
            }


            //HOJA 5
            Sheet = Ep.Workbook.Worksheets.Add("INGRESOS USD");

            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:X1"].Style.Font.Size = 20;
            Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:X3"].Style.Font.Bold = true;
            Sheet.Cells["A1:X1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:X1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Facturas");

            row = 2;
            Sheet.Cells["A1:X1"].Style.Font.Size = 12;
            Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:X1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A2:X2"].Style.Font.Bold = true;
            //Subtitulo según el filtrado del periodo de datos
            row = 2;
            Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
            Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            Sheet.Cells[string.Format("B2", row)].Value = FI;
            Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
            Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            Sheet.Cells[string.Format("E2", row)].Value = FF;
            //En la fila3 se da el formato a el encabezado
            row = 3;
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 10;
            Sheet.Cells["A3:X3"].Style.Font.Bold = true;
            Sheet.Cells["A3:X3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A3:X3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            // Se establece el nombre que identifica a cada una de las columnas de datos.
            Sheet.Cells["A3"].RichText.Add("Serie");
            Sheet.Cells["B3"].RichText.Add("Numero");
            Sheet.Cells["C3"].RichText.Add("Fecha");
            Sheet.Cells["D3"].RichText.Add("Cliente");
            Sheet.Cells["E3"].RichText.Add("Subtotal");
            Sheet.Cells["F3"].RichText.Add("Iva");
            Sheet.Cells["G3"].RichText.Add("Total");
            Sheet.Cells["H3"].RichText.Add("Moneda");
            Sheet.Cells["I3"].RichText.Add("Tipo de Cambio");
            Sheet.Cells["J3"].RichText.Add("Metodo de pago");
            Sheet.Cells["K3"].RichText.Add("UUID");
            Sheet.Cells["L3"].RichText.Add("Estado");
            Sheet.Cells["M3"].RichText.Add("Pagada");
            Sheet.Cells["N3"].RichText.Add("Importe Pagado");
            Sheet.Cells["O3"].RichText.Add("Saldo Insoluto");
            Sheet.Cells["P3"].RichText.Add("Fecha de Revisión");
            Sheet.Cells["Q3"].RichText.Add("Fecha de Vencimiento");
            //Sheet.Cells["R3"].RichText.Add("Telefono");

            //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
            //Sheet.Cells["A3"].Value = "Serie";

            //Aplicar borde doble al rango de celdas A3:Q3
            Sheet.Cells["A3:X3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
            // Se establecen los formatos para las celdas: Fecha, Moneda
            row = 4;
            Sheet.Cells.Style.Font.Bold = false;
            cadena = "select * from [dbo].[EncfacturasSaldos] where moneda='USD' and Fecha >= '" + FI + "' and Fecha <='" + FF + "'";
            facturassaldos = dbe.Database.SqlQuery<EncfacturasSaldos>(cadena).ToList();

            foreach (EncfacturasSaldos item in facturassaldos)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.Serie;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Numero;
                Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Nombre_cliente;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Subtotal;
                Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("F{0}", row)].Value = item.IVA;
                Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Moneda;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.TC;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.IDMetododepago;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.UUID;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.Estado;
                //Como item.pagada es de tipo booleano (verdadero o falso), se imprime en pantalla (si o no), según corresponda.
                if (item.pagada == false)
                {
                    Sheet.Cells[string.Format("M{0}", row)].Value = "No";
                }
                else
                {
                    Sheet.Cells[string.Format("M{0}", row)].Value = "Si";
                }
                Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("N{0}", row)].Value = item.ImportePagado;
                Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("O{0}", row)].Value = item.ImporteSaldoInsoluto;

                Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("P{0}", row)].Value = item.FechaRevision;
                Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.FechaVencimiento;
                //Sheet.Cells[string.Format("R{0}", row)].Value = item.Telefono;


                row++;
            }

            //HOJA 6
            Sheet = Ep.Workbook.Worksheets.Add("CANCELADAS");

            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:X1"].Style.Font.Size = 20;
            Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:X3"].Style.Font.Bold = true;
            Sheet.Cells["A1:X1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:X1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Facturas");

            row = 2;
            Sheet.Cells["A1:X1"].Style.Font.Size = 12;
            Sheet.Cells["A1:X1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:X1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A2:X2"].Style.Font.Bold = true;
            //Subtitulo según el filtrado del periodo de datos
            row = 2;
            Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
            Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            Sheet.Cells[string.Format("B2", row)].Value = FI;
            Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
            Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            Sheet.Cells[string.Format("E2", row)].Value = FF;
            //En la fila3 se da el formato a el encabezado
            row = 3;
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 10;
            Sheet.Cells["A3:X3"].Style.Font.Bold = true;
            Sheet.Cells["A3:X3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A3:X3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            // Se establece el nombre que identifica a cada una de las columnas de datos.
            Sheet.Cells["A3"].RichText.Add("Serie");
            Sheet.Cells["B3"].RichText.Add("Numero");
            Sheet.Cells["C3"].RichText.Add("Fecha");
            Sheet.Cells["D3"].RichText.Add("Cliente");
            Sheet.Cells["E3"].RichText.Add("Subtotal");
            Sheet.Cells["F3"].RichText.Add("Iva");
            Sheet.Cells["G3"].RichText.Add("Total");
            Sheet.Cells["H3"].RichText.Add("Moneda");
            Sheet.Cells["I3"].RichText.Add("Tipo de Cambio");
            Sheet.Cells["J3"].RichText.Add("Metodo de pago");
            Sheet.Cells["K3"].RichText.Add("UUID");
            Sheet.Cells["L3"].RichText.Add("Estado");
            Sheet.Cells["M3"].RichText.Add("Pagada");
            Sheet.Cells["N3"].RichText.Add("Importe Pagado");
            Sheet.Cells["O3"].RichText.Add("Saldo Insoluto");
            Sheet.Cells["P3"].RichText.Add("Fecha de Revisión");
            Sheet.Cells["Q3"].RichText.Add("Fecha de Vencimiento");
            //Sheet.Cells["R3"].RichText.Add("Telefono");

            //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
            //Sheet.Cells["A3"].Value = "Serie";

            //Aplicar borde doble al rango de celdas A3:Q3
            Sheet.Cells["A3:X3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
            // Se establecen los formatos para las celdas: Fecha, Moneda
            row = 4;
            Sheet.Cells.Style.Font.Bold = false;
            cadena = "select * from [dbo].[EncfacturasSaldos] where Estado='C' and Fecha >= '" + FI + "' and Fecha <='" + FF + "'";
            facturassaldos = dbe.Database.SqlQuery<EncfacturasSaldos>(cadena).ToList();

            foreach (EncfacturasSaldos item in facturassaldos)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.Serie;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Numero;
                Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Nombre_cliente;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Subtotal;
                Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("F{0}", row)].Value = item.IVA;
                Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Moneda;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.TC;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.IDMetododepago;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.UUID;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.Estado;
                //Como item.pagada es de tipo booleano (verdadero o falso), se imprime en pantalla (si o no), según corresponda.
                if (item.pagada == false)
                {
                    Sheet.Cells[string.Format("M{0}", row)].Value = "No";
                }
                else
                {
                    Sheet.Cells[string.Format("M{0}", row)].Value = "Si";
                }
                Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("N{0}", row)].Value = item.ImportePagado;
                Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells[string.Format("O{0}", row)].Value = item.ImporteSaldoInsoluto;

                Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("P{0}", row)].Value = item.FechaRevision;
                Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.FechaVencimiento;
                //Sheet.Cells[string.Format("R{0}", row)].Value = item.Telefono;


                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelFacturas.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
            return Redirect("/blah");
        }

    }
    public class DatosCancelacion
    {
        public string usuario { get; set; }
        public string pass { get; set; }
        public string accion { get; set; }
        public string produccion { get; set; }
        public string uuid { get; set; }
        public string rfc { get; set; }
        public string password { get; set; }
        public string motivo { get; set; }
        public string folioSustitucion { get; set; }
        public string b64Cer { get; set; }
        public string b64Key { get; set; }
    }

    public class DatosOrigen
    {
        public string calle { get; set; }
        public string numext { get; set; }
        public string numint { get; set; }
        public string colonia { get; set; }
        public string localidad { get; set; }
        public string municipio { get; set; }
        public string estado { get; set; }
        public string pais { get; set; }
        public string cp { get; set; }
        public string referencia { get; set; }
    }
    public class DatosEntrega
    {
        public string calle { get; set; }
        public string numext { get; set; }
        public string numint { get; set; }
        public string colonia { get; set; }
        public string localidad { get; set; }
        public string municipio { get; set; }
        public string estado { get; set; }
        public string pais { get; set; }
        public string cp { get; set; }
        public string referencia { get; set; }
    }

    public class DatosOperador
    {
        public string RFC { get; set; }
        public string Nombre { get; set; }
        public string NumExt { get; set; }
        public string NumInt { get; set; }
        public string NoLicencia { get; set; }
        public string Calle { get; set; }
        public string Referencia { get; set; }
        public string c_Pais { get; set; }
        public string c_Estado { get; set; }
        public string c_Municipio { get; set; }
        public string c_Localidad { get; set; }
        public string c_Colonia { get; set; }

        public string CP { get; set; }
    }
}
