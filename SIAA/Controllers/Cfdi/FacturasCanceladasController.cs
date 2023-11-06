using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Xml;
using System.Net;
using System.Data.SqlClient;
using SIAAPI.Reportes;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using SIAAPI.clasescfdi;
using static SIAAPI.Models.Comercial.ClienteRepository;
using SIAAPI.Models.Login;
using System.Xml.Serialization;
using PagedList;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Cfdi;

namespace SIAAPI.Controllers.Cfdi
{

    [Authorize(Roles = "Administrador,Facturacion,Ventas,Sistemas,Almacenista,Comercial,Gerencia,GerenteVentas,Contabilidad")]  
    public class FacturasCanceladasController : Controller
    {
        EncFacturaCanContext dbs = new EncFacturaCanContext();
        EncfacturaContext db = new EncfacturaContext();
        // GET: FacturasCanceladas 
        public ActionResult Index(string Numero, string ClieFac, string sortOrder, string currentFilter, int? page, int? PageSize, string Fechainicio, string Fechafinal, string FacPag, string SerieFac ,string Estado = "C")
        {
            //string ConsultaSql = "select * from Encfacturas ";
            string ConsultaSql = "Select top 500 * from dbo.VEncFacturaCan";
            string cadenaSQl = string.Empty;
            try
            {

                //Buscar Facturas: Pagadas o no pagadas
                var FacPagLst = new List<SelectListItem>();


                var EstadoLst = new List<SelectListItem>();
                //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });

                EstadoLst.Add(new SelectListItem { Text = "Cancelada", Value = "C" });
                //EstadoLst.Add(new SelectListItem { Text = "Activas", Value = "A" });

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


                string ConsultaSqlResumen = "select Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from VEncFacturaCan ";
                string ConsultaAgrupado = "group by Moneda order by Moneda ";
                string Filtro = string.Empty;

                string Orden = " order by fecha desc , serie , numero desc ";

                ///tabla filtro: serie
                if (!String.IsNullOrEmpty(SerieFac))
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = " where Serie='" + SerieFac + "'";
                    }
                    else
                    {
                        Filtro += " and  Serie='" + SerieFac + "'";
                    }

                }

                if (String.IsNullOrEmpty(SerieFac))
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = " where Serie=''";
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
                        Filtro = " where Numero=" + Numero + "";
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
                        Filtro = " where Nombre_cliente='" + ClieFac + "'";
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
                            Filtro = " where pagada='1' ";
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
                            Filtro = " where pagada='0' ";
                        }
                        else
                        {
                            Filtro += " and  pagada='0' ";
                        }
                    }
                }


                //if (Estado != "Todos")
                //{
                    if (Estado == "C")
                    {
                        if (Filtro == string.Empty)
                        {
                            Filtro = " where Estado='C'";
                        }
                        else
                        {
                            Filtro += " and  Estado='C'";
                        }
                    //}
                    //if (Estado == "A")
                    //{
                    //    if (Filtro == string.Empty)
                    //    {
                    //        Filtro = "where  Estado='A'";
                    //    }
                    //    else
                    //    {
                    //        Filtro += "and Estado='A'";
                    //    }
                    //}
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

                if (Filtro == " where  Estado='C'")
                {
                    Filtro += " and Fecha >='" + DateTime.Now.AddDays(-30).Year + "-" + DateTime.Now.AddDays(-30).Month + "-" + DateTime.Now.AddDays(-30).Day + "'  and  Fecha<'" + DateTime.Now.AddDays(1).Year + "-" + DateTime.Now.AddDays(1).Month + "-" + DateTime.Now.AddDays(1).Day + "' ";
                    ViewBag.Fechainicio = "" + DateTime.Now.AddDays(-30).Year + "-" + DateTime.Now.AddDays(-30).Month + "-" + DateTime.Now.AddDays(-30).Day + "";
                    ViewBag.Fechafinal = "" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "";
                }


                cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

                var elementos = dbs.Database.SqlQuery<VEncFacturaCan>(cadenaSQl).ToList();

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


        public ActionResult EstadosSAT(int id, List<VEncFacturaCan> enc)
        {
            try
            {
               
                VEncFacturaCan FacCan = dbs.Database.SqlQuery<VEncFacturaCan>("select * from dbo.VEncFacturaCan where ID = " + id + "").ToList().FirstOrDefault();
                ViewBag.Serie = FacCan.Serie;
                ViewBag.Numero = FacCan.Numero;
                ViewBag.Nombre = FacCan.Nombre_cliente;
                ViewBag.uuid = FacCan.UUID;
                ViewBag.Total = FacCan.Total;
                ViewBag.IDFactura = id;
                ViewBag.Estado = "C";
                FacCan.ID = id;

            }
            catch (Exception err)
            {
                SIAAPI.Models.Cfdi.Encfacturas factura = new SIAAPI.Models.Cfdi.EncfacturasSaldosContext().Database.SqlQuery<SIAAPI.Models.Cfdi.Encfacturas>("select*from EncFacturas where id= " + id).ToList().FirstOrDefault();

                string MENSAJE = err.Message;
                ViewBag.Serie = factura.Serie;
                ViewBag.Numero = factura.Numero;
                ViewBag.Nombre = factura.Nombre_cliente;
                ViewBag.uuid = factura.UUID;
                ViewBag.Total = factura.Total;
                ViewBag.IDFactura = id;
                ViewBag.Estado = factura.Estado;


            }

            List<EncFacturaCan> elemento = db.Database.SqlQuery<EncFacturaCan>("select * from dbo.EncFacturaCan where IDFactura = " + id + " order by IDCan desc").ToList();
            if (elemento == null)
            {
                return HttpNotFound();
            }
            return View(elemento);
        }
 

    }
}