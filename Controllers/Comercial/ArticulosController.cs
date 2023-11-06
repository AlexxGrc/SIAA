using PagedList;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Inventarios;
using SIAAPI.Models.Login;
using SIAAPI.Models.Produccion;
using SIAAPI.ViewModels.Articulo;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using SIAAPI.Reportes;
using SIAAPI.ClasesProduccion;
using System.Xml;
using System.Xml.Serialization;
using SIAAPI.Models.Cfdi;
using System.Text;
using SIAAPI.Controllers.Cfdi;
using System.Globalization;

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Administrador, Cliente, Gerencia, Comercial,Sistemas,AdminProduccion, Compras, Almacenista, Ventas, Produccion,GerenteVentas,Facturacion, Diseno, Calidad, ConsultaSuaje, GestionCalidad")]
    public class ArticulosController : Controller
    {
        private ArticuloContext db = new ArticuloContext();
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, string Familia)
        {
            //Buscar Serie Factura

            var SerLst = new List<string>();

            ViewBag.Familias = new FamiliaRepository().GetFamilias();
            ViewBag.Familia = Familia;

            if (sortOrder == string.Empty)
            {
                sortOrder = "Cref";
            }

            if (sortOrder == null)
            {
                sortOrder = "Cref";
            }

            VPArticuloContext db = new VPArticuloContext();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "Cref" : "Cref";
            ViewBag.CSortParm = String.IsNullOrEmpty(sortOrder) ? "Familia" : "Familia";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            ViewBag.CurrentFilter = searchString;
            //Paginación

            if (Familia == "")
            {
                Familia = null;
            }
            string cadena = "select TOP 50 * from VPArticulo where Tipo='Articulo' ";
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                if (string.IsNullOrEmpty(Familia))
                {
                    cadena = "select distinct(VPArticulo.IDArticulo), VPArticulo.* from VPArticulo left join Caracteristica on VPArticulo.IDArticulo=Caracteristica.Articulo_IDArticulo where VPArticulo.cref like '%" + searchString + "%' or VPArticulo.Descripcion like '%" + searchString + "%' or Caracteristica.Presentacion like '%" + searchString + "%'";
                }
                else
                {

                    cadena = "select  distinct(VPArticulo.IDArticulo), VPArticulo.* from VPArticulo left join Caracteristica on VPArticulo.IDArticulo=Caracteristica.Articulo_IDArticulo where (VPArticulo.cref like '%" + searchString + "%' or VPArticulo.Descripcion like '%" + searchString + "%' or Caracteristica.Presentacion like '%" + searchString + "%') and IDFamilia =" + Familia + " ";

                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Familia) && (Familia != "-"))
                {
                    cadena = "select * from VPArticulo where IDFamilia=" + Familia + " ";
                }
                else
                {
                    cadena = "select Top 50 * from VPArticulo where Tipo='Articulo' ";
                }
            }



            //Ordenacion

            switch (sortOrder)
            {
                case "Cref":
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        cadena = cadena + " order by cref";
                    }
                    else
                    {
                        cadena = cadena + " order by left(cref,2), RIGHT(cref,3)";
                    }

                    break;
                //case "Descripcion":
                //    cadena = cadena + " order by Descripcion";
                //    break;
                //case "Familia":
                //    cadena = cadena + " order by Familia";
                //    break;
                default:
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        cadena = cadena + " order by cref";
                    }
                    else
                    {
                        cadena = cadena + " order by left(cref,2), RIGHT(cref,3)";
                    }
                    break;



            }


            List<VPArticulo> elementos = db.Database.SqlQuery<VPArticulo>(cadena).ToList<VPArticulo>();
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = elementos.Count;

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
        public ActionResult EditaArticulo(int? id)
        {
            ArticuloContext db = new ArticuloContext();
            var elemento = db.Articulo.Single(m => m.IDArticulo == id);
            ViewBag.claveproducto = elemento.Cref;
            ViewBag.descripcion = elemento.Descripcion;
            ViewBag.minimoventa = elemento.MinimoVenta;
            ViewBag.minimocompra = elemento.MinimoCompra;
            ViewBag.preciounico = elemento.Preciounico;
            ViewBag.controlai = elemento.CtrlStock;
            ViewBag.manejamas = elemento.ManejoCar;
            ViewBag.devolucion = elemento.ExistenDev;
            ViewBag.ccodigo = elemento.bCodigodebarra;
            ViewBag.codigob = elemento.Codigodebarras;
            ViewBag.eskit = elemento.esKit;
            ViewBag.generaorden = elemento.GeneraOrden;
            ViewBag.obsoleto = elemento.obsoleto;
            ViewBag.nombreimagen = elemento.nameFoto;
            ViewBag.id = id;
            ViewBag.StockMin = elemento.StockMin;
            ViewBag.StockMax = elemento.StockMax;
            FamiliaContext dbfam = new FamiliaContext();
            ViewBag.IDFamilia = new SelectList(dbfam.Familias.OrderBy(m => m.Descripcion), "IDFamilia", "Descripcion", elemento.IDFamilia);
            // ViewBag.IDFamilia = new FamiliaRepository().GetFamilias();
            ArticuloContext dbta = new ArticuloContext();
            ViewBag.IDTipoArticulo = new SelectList(dbta.TipoArticulo, "IDTipoArticulo", "Descripcion", elemento.IDTipoArticulo);
            c_MonedaContext dbmo = new c_MonedaContext();
            ViewBag.IDMoneda = new SelectList(dbmo.c_Monedas, "IDMoneda", "Descripcion", elemento.IDMoneda);
            c_ClaveUnidadContext dbcu = new c_ClaveUnidadContext();
            ViewBag.IDClaveUnidad = new SelectList(dbcu.c_ClaveUnidades, "IDClaveUnidad", "Nombre", elemento.IDClaveUnidad);
            ViewBag.IDAQL = new SelectList(dbta.AQLCalidad, "IDAQL", "Descripcion", elemento.IDAQL);
            ViewBag.IDInspeccion = new SelectList(dbta.Inspeccion, "IDInspeccion", "Descripcion", elemento.IDInspeccion);
            ViewBag.IDMuestreo = new SelectList(dbta.Muestreo, "IDMuestreo", "Descripcion", elemento.IDMuestreo);
            ViewBag.StockMin = elemento.StockMin;
            ViewBag.StockMax = elemento.StockMax;
            return PartialView();
        }
        public ActionResult editArticulo(mArticulo Art)
        {
            try
            {
                ArticuloContext db = new ArticuloContext();
                Articulo articulo = db.Articulo.Find(Art.IDArticulo);
                Articulo newArt = new Articulo();
                newArt.IDArticulo = Art.IDArticulo;
                newArt.Cref = Art.Cref;
                newArt.Descripcion = Art.Descripcion;
                newArt.IDFamilia = Art.IDFamilia;
                newArt.IDTipoArticulo = Art.IDTipoArticulo;
                //newArt.Preciounico = Art.Preciounico;
                int Preciounico = 0;
                if (Art.Preciounico.Equals(true)) { Preciounico = 1; }
                newArt.IDMoneda = Art.IDMoneda;
                //newArt.CtrlStock = Art.CtrlStock;
                int CtrlStock = 0;
                if (Art.CtrlStock.Equals(true)) { CtrlStock = 1; }
                //newArt.ManejoCar = Art.ManejoCar;
                int ManejoCar = 0;
                if (Art.ManejoCar.Equals(true)) { ManejoCar = 1; }
                newArt.IDClaveUnidad = Art.IDClaveUnidad;
                //newArt.bCodigodebarra = Art.bCodigodebarra;
                int bCodigodebarra = 0;
                if (Art.bCodigodebarra.Equals(true)) { bCodigodebarra = 1; }
                newArt.Codigodebarras = Art.Codigodebarras;
                newArt.Obscalidad = Art.Obscalidad;
                //newArt.ExistenDev = Art.ExistenDev;
                int ExistenDev = 0;
                if (Art.ExistenDev.Equals(true)) { ExistenDev = 1; }
                newArt.IDAQL = Art.IDAQL;
                newArt.IDInspeccion = Art.IDInspeccion;
                newArt.IDMuestreo = Art.IDMuestreo;
                //newArt.esKit = Art.esKit;
                int esKit = 0;
                if (Art.esKit.Equals(true)) { esKit = 1; }
                if (Art.fileIMG == null)
                {
                    newArt.nameFoto = articulo.nameFoto;
                }
                else
                {
                    newArt.nameFoto = saveIMG(Art.fileIMG, newArt.nameFoto);
                }
                //newArt.GeneraOrden = Art.GeneraOrden;
                int GeneraOrden = 0;
                if (Art.GeneraOrden.Equals(true)) { GeneraOrden = 1; }
                //newArt.obsoleto = Art.obsoleto;
                int obsoleto = 0;
                if (Art.obsoleto.Equals(true)) { obsoleto = 1; }
                newArt.MinimoVenta = Art.MinimoVenta;
                newArt.MinimoCompra = Art.MinimoCompra;
                newArt.StockMin = Art.StockMin;
                newArt.StockMax = Art.StockMax;
                db.Database.ExecuteSqlCommand("update [dbo].[Articulo] set CREF='" + newArt.Cref + "', [Descripcion]='" + newArt.Descripcion + "',[Preciounico]=" + Preciounico + ",[CtrlStock]=" + CtrlStock + ",[ManejoCar]=" + ManejoCar + ",[Obscalidad]='" + newArt.Obscalidad + "',[ExistenDev]=" + ExistenDev + ",[IDAQL]=" + newArt.IDAQL + ",[bCodigodebarra]=" + bCodigodebarra + ",[Codigodebarras]='" + newArt.Codigodebarras + "',[esKit]=" + esKit + ",[nameFoto]='" + newArt.nameFoto + "',[GeneraOrden]=" + GeneraOrden + ",[IDClaveUnidad]=" + newArt.IDClaveUnidad + ",[IDMoneda]=" + newArt.IDMoneda + ",[IDFamilia]=" + newArt.IDFamilia + ",[IDMuestreo]=" + newArt.IDMuestreo + ",[IDTipoArticulo]=" + newArt.IDTipoArticulo + ",[IDInspeccion]=" + newArt.IDInspeccion + ",[obsoleto]=" + obsoleto + ",[MinimoVenta]=" + newArt.MinimoVenta + ",[MinimoCompra]=" + newArt.MinimoCompra + ", StockMin = " + newArt.StockMin + ", StockMax = " + newArt.StockMax + " where IDArticulo=" + newArt.IDArticulo + "");


                try
                {
                    List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                    int UserID = userid.Select(s => s.UserID).FirstOrDefault();

                    //sobreescribir cotización
                    string insert = "insert into RegistroEdicionArticulo(IDUsuario, Fecha, IDArticulo) values (" + UserID + ", sysdatetime()," + newArt.IDArticulo + ")";
                    db.Database.ExecuteSqlCommand(insert);
                }
                catch (Exception err)
                {

                }

                return RedirectToAction("Details", new { id = newArt.IDArticulo });


            }
            catch (Exception ex)
            {

                string mensaje = ex.Message;
                return Json(new HttpStatusCodeResult(500, ex.Message + ex.InnerException));
            }

        }
        public ActionResult AgregaArticulo()
        {
            //FamiliaContext dbfam = new FamiliaContext();
            //ViewBag.IDFamilia = new SelectList(dbfam.Familias, "IDFamilia", "Descripcion");
            ViewBag.IDFamilia = new FamiliaRepository().GetFamilias();
            ArticuloContext dbta = new ArticuloContext();
            ViewBag.IDTipoArticulo = new SelectList(dbta.TipoArticulo, "IDTipoArticulo", "Descripcion");
            c_MonedaContext dbmo = new c_MonedaContext();
            ViewBag.IDMoneda = new SelectList(dbmo.c_Monedas, "IDMoneda", "Descripcion");
            c_ClaveUnidadContext dbcu = new c_ClaveUnidadContext();
            ViewBag.IDClaveUnidad = new SelectList(dbcu.c_ClaveUnidades, "IDClaveUnidad", "Nombre");
            ViewBag.IDAQL = new SelectList(dbta.AQLCalidad, "IDAQL", "Descripcion");
            ViewBag.IDInspeccion = new SelectList(dbta.Inspeccion, "IDInspeccion", "Descripcion");
            ViewBag.IDMuestreo = new SelectList(dbta.Muestreo, "IDMuestreo", "Descripcion");
            return PartialView();
        }

        public ActionResult AgregaArticuloIsrael()
        {

            //List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            //int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            //ClsDatoEntero entero = new ArticuloContext().Database.SqlQuery<ClsDatoEntero>("select count(UserRolesID) as dato from UserRole where (roleid=8 or roleid=9 or roleid=13 or roleid=1 or roleid=5) and userid="+ usuario).FirstOrDefault();
            /////permitir crar suajes
            /////
            //if (entero.Dato>0)
            //{
            //    ViewBag.IDFamilia = new FamiliaRepository().GetFamilias();
            //}
            //else
            //{
            //    FamiliaContext dbfam = new FamiliaContext();
            //    ViewBag.IDFamilia = new SelectList(dbfam.Familias.Where(s=> s.IDFamilia!= 11 && s.IDFamilia!= 69 && s.IDFamilia != 71 && s.IDFamilia != 72 && s.IDFamilia != 75 && s.IDFamilia != 76 && s.IDFamilia != 77 && s.IDFamilia != 80 && s.IDFamilia != 81 && s.IDFamilia != 91 && s.IDFamilia != 93 && s.IDFamilia != 70 && s.IDFamilia != 95).OrderBy(s => s.Descripcion), "IDFamilia", "Descripcion");
            //}
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            //FamiliaContext dbfam = new FamiliaContext();
            //ViewBag.IDFamilia = new SelectList(dbfam.Familias, "IDFamilia", "Descripcion");
            if (usuario != 329)
            {
                ViewBag.IDFamilia = new FamiliaRepository().GetFamiliasSinSuajes();

            }
            else
            {
                ViewBag.IDFamilia = new FamiliaRepository().GetFamilias();
            }

            ArticuloContext dbta = new ArticuloContext();
            ViewBag.IDTipoArticulo = new SelectList(dbta.TipoArticulo, "IDTipoArticulo", "Descripcion");
            c_MonedaContext dbmo = new c_MonedaContext();
            ViewBag.IDMoneda = new SelectList(dbmo.c_Monedas, "IDMoneda", "Descripcion");
            c_ClaveUnidadContext dbcu = new c_ClaveUnidadContext();
            ViewBag.IDClaveUnidad = new SelectList(dbcu.c_ClaveUnidades, "IDClaveUnidad", "Nombre");
            ViewBag.IDAQL = new SelectList(dbta.AQLCalidad, "IDAQL", "Descripcion");
            ViewBag.IDInspeccion = new SelectList(dbta.Inspeccion, "IDInspeccion", "Descripcion");
            ViewBag.IDMuestreo = new SelectList(dbta.Muestreo, "IDMuestreo", "Descripcion");
            return PartialView();
        }


        public ActionResult addArticulo(mArticulo Art)
        {
            try
            {
                Articulo newArt = new Articulo();
                newArt.IDArticulo = Art.IDArticulo;
                Art.Cref.Replace("/", "//");
                Art.Descripcion.Replace("/", "//");
                Art.Cref.Replace("'", "''");
                Art.Descripcion.Replace("'", "''");


                newArt.Cref = Art.Cref;
                newArt.Descripcion = Art.Descripcion;

                newArt.IDFamilia = Art.IDFamilia;
                newArt.IDTipoArticulo = Art.IDTipoArticulo;
                newArt.Preciounico = Art.Preciounico;
                newArt.IDMoneda = Art.IDMoneda;
                newArt.CtrlStock = Art.CtrlStock;
                newArt.ManejoCar = Art.ManejoCar;
                newArt.IDClaveUnidad = Art.IDClaveUnidad;
                newArt.bCodigodebarra = Art.bCodigodebarra;
                newArt.Codigodebarras = Art.Codigodebarras;
                newArt.Obscalidad = Art.Obscalidad;
                newArt.ExistenDev = Art.ExistenDev;
                newArt.IDAQL = 1;
                newArt.IDInspeccion = 2;
                newArt.IDMuestreo = 3;
                newArt.esKit = Art.esKit;
                newArt.nameFoto = saveIMG(Art.fileIMG, newArt.nameFoto);
                newArt.GeneraOrden = Art.GeneraOrden;
                newArt.obsoleto = Art.obsoleto;
                newArt.MinimoVenta = Art.MinimoVenta;
                newArt.MinimoCompra = Art.MinimoCompra;
                newArt.StockMin = Art.StockMin;
                newArt.StockMax = Art.StockMax;

                ArticuloContext dba = new ArticuloContext();
                dba.Articulo.Add(newArt);
                dba.SaveChanges();



                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                if (ex.InnerException.ToString().Contains("Duplica"))
                {
                    return Json(new HttpStatusCodeResult(500, "El articulo intenta ser duplicado, posiblemente presionaste el boton agregar 2 veces"));
                }
                else
                {
                    return Json(new HttpStatusCodeResult(500, ex.Message + ex.InnerException));
                }
            }
        }

        public ActionResult addArticuloIsrael(mArticulo Art)
        {
            try
            {
                string Cref = "";
                string Descripcion = "";
                Articulo newArt = new Articulo();
                newArt.IDArticulo = Art.IDArticulo;
                if (Art.Cref.Contains("'"))
                {
                    Cref = Art.Cref.Replace("'", "''");
                }
                else
                {
                    Cref = Art.Cref;
                }
                if (Art.Descripcion.Contains("'"))
                {
                    Descripcion = Art.Descripcion.Replace("'", "''");
                }
                else
                {
                    Descripcion = Art.Descripcion;
                }

                newArt.Cref = Cref;
                newArt.Descripcion = Descripcion;
                newArt.IDFamilia = Art.IDFamilia;
                newArt.IDTipoArticulo = Art.IDTipoArticulo;
                newArt.Preciounico = Art.Preciounico;
                newArt.IDMoneda = Art.IDMoneda;
                newArt.CtrlStock = Art.CtrlStock;
                newArt.ManejoCar = Art.ManejoCar;
                newArt.IDClaveUnidad = Art.IDClaveUnidad;
                newArt.bCodigodebarra = Art.bCodigodebarra;
                newArt.Codigodebarras = Art.Codigodebarras;
                newArt.Obscalidad = Art.Obscalidad;
                newArt.ExistenDev = Art.ExistenDev;
                newArt.IDAQL = 1;
                newArt.IDInspeccion = 2;
                newArt.IDMuestreo = 3;
                newArt.esKit = Art.esKit;
                newArt.nameFoto = saveIMG(Art.fileIMG, newArt.nameFoto);
                newArt.GeneraOrden = Art.GeneraOrden;
                newArt.obsoleto = Art.obsoleto;
                newArt.MinimoVenta = Art.MinimoVenta;
                newArt.MinimoCompra = Art.MinimoCompra;
                newArt.StockMin = Art.StockMin;
                newArt.StockMax = Art.StockMax;

                ArticuloContext dba = new ArticuloContext();
                //string cadena = "INSERT INTO [dbo].[Articulo] ([Cref],[Descripcion],[Preciounico],[CtrlStock],[ManejoCar],[Obscalidad],[ExistenDev],[IDAQL],[bCodigodebarra],[Codigodebarras],[esKit],[nameFoto],[GeneraOrden],[IDClaveUnidad],[IDMoneda],[IDFamilia],[IDMuestreo],[IDTipoArticulo],[IDInspeccion],[obsoleto],[MinimoVenta],[MinimoCompra],[StockMin],[StockMax])VALUES('"+ newArt.Cref+"','"+ newArt.Descripcion+ "','"+ newArt.Preciounico+ "','"+ newArt.CtrlStock+ "','"+ newArt.ManejoCar+"','"+ newArt.Obscalidad+"','"+ newArt.ExistenDev+"','"+ newArt.IDAQL+"','"+ newArt.bCodigodebarra+"','"+ newArt.Codigodebarras+"','"+ newArt.esKit+"','"+ newArt.nameFoto+"','"+ newArt.GeneraOrden+"','"+ newArt.IDClaveUnidad+"','"+ newArt.IDMoneda+"','"+ newArt.IDFamilia+"','"+ newArt.IDMuestreo+"','"+ newArt.IDTipoArticulo+"','"+ newArt.IDInspeccion+"','"+ newArt.obsoleto+"','"+ newArt.MinimoVenta+"','"+ newArt.MinimoCompra+"','"+ newArt.StockMin+"','"+ newArt.StockMax+"')" ;
                //dba.Database.ExecuteSqlCommand(cadena);
                dba.Articulo.Add(newArt);
                dba.SaveChanges();



                return RedirectToAction("PresentacionIsrael", "Articulos", new { IDFamilia = newArt.IDFamilia, IDArticulo = newArt.IDArticulo, Cref = newArt.Cref, Descripcion = newArt.Descripcion });

            }
            catch (Exception ex)
            {
                if (ex.InnerException.ToString().Contains("UNIQUE KEY"))
                {
                    return Json(new HttpStatusCodeResult(500, "El articulo intenta ser duplicado en la Referencia o posiblemente presionaste el boton agregar 2 veces"));
                }
                if (ex.InnerException.ToString().Contains("Duplica"))
                {
                    return Json(new HttpStatusCodeResult(500, "El articulo intenta ser duplicado o posiblemente presionaste el boton agregar 2 veces"));
                }
                else
                {
                    return Json(new HttpStatusCodeResult(500, ex.Message + ex.InnerException));
                }
            }
        }

        public string saveIMG(HttpPostedFileBase file, string name)
        {
            try
            {
                if ((file != null))
                {

                    string ext = Path.GetExtension(file.FileName);
                    string newName = Guid.NewGuid().ToString();

                    newName = newName + ext;
                    string path = Server.MapPath("~/imagenes/Upload/");
                    newName = newName.ToUpper();
                    file.SaveAs(path + newName);

                    if (System.IO.File.Exists(path + name))
                    {
                        System.IO.File.Delete(path + name);
                    }

                    return newName;
                }
                else
                {
                    return name;
                }


            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
        public ActionResult ListadoPresentaciones(int? id, int idarticulo)
        {
            Articulo articulo = db.Articulo.Find(idarticulo);
            ViewBag.nombrearticulo = articulo.Descripcion;
            ViewBag.idarticulo = id;
            ViewBag.cref = articulo.Cref;
            ViewBag.idfamilia = articulo.IDFamilia;
            LPresentacionSchema dat = new LPresentacionSchema();
            List<LPresentacion> listado = db.Database.SqlQuery<LPresentacion>("Select [ID] ,[IDPresentacion],[Cotizacion] ,[version] ,[Presentacion] ,[jsonPresentacion] ,[Articulo_IDArticulo] ,[obsoleto],IDCotizacion from Caracteristica where Articulo_IDArticulo = " + id + " order by IDPresentacion").ToList();

            return PartialView("listado", new { idart = articulo.IDArticulo });
        }
        [HttpPost]
        public JsonResult Deleteitempres(int? id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from Caracteristica where ID=" + id);

                return Json(true);
                //return Json(idarticulo, JsonRequestBehavior.AllowGet);

            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        [HttpPost]
        public JsonResult Deleteplaneacion(int? id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("update Caracteristica  set cotizacion=0, version=0 where ID=" + id);

                return Json(true);
                //return Json(idarticulo, JsonRequestBehavior.AllowGet);

            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }
        public ActionResult Presentacion(int? id, int? IDArticulo)
        {
            List<SIAAPI.Models.Administracion.Materiales> materiales = new List<SIAAPI.Models.Administracion.Materiales>();
            string cadenaM = "Select* from Materiales where Obsoleto <> '1'";
            materiales = new ArticuloContext().Database.SqlQuery<SIAAPI.Models.Administracion.Materiales>(cadenaM).ToList();
            List<SelectListItem> listaArticulo = new List<SelectListItem>();
            listaArticulo.Add(new SelectListItem { Text = "--Todos los Materiales--", Value = "0" });

            foreach (var m in materiales)
            {
                listaArticulo.Add(new SelectListItem { Text = m.Clave, Value = m.ID.ToString() });
            }

            ViewBag.Materiales = listaArticulo;
            List<AtributodeFamilia> atributos = db.Database.SqlQuery<AtributodeFamilia>("select * from AtributodeFamilia where IDFamilia='" + id + "'").ToList();
            ViewBag.IDArticulo = IDArticulo;
            return PartialView(atributos);
        }

        public ActionResult PresentacionIsrael(int? IDFamilia, int? IDArticulo, string Cref = "", string Descripcion = "")
        {
            List<SIAAPI.Models.Administracion.Materiales> materiales = new List<SIAAPI.Models.Administracion.Materiales>();
            string cadenaM = "Select* from Materiales where Obsoleto <> '1'";
            materiales = new ArticuloContext().Database.SqlQuery<SIAAPI.Models.Administracion.Materiales>(cadenaM).ToList();
            List<SelectListItem> listaArticulo = new List<SelectListItem>();
            listaArticulo.Add(new SelectListItem { Text = "--Todos los Materiales--", Value = "0" });

            foreach (var m in materiales)
            {
                listaArticulo.Add(new SelectListItem { Text = m.Clave, Value = m.ID.ToString() });
            }

            ViewBag.Materiales = listaArticulo;

            List<AtributodeFamilia> atributos = db.Database.SqlQuery<AtributodeFamilia>("select * from AtributodeFamilia where IDFamilia='" + IDFamilia + "'").ToList();
            ViewBag.Cref = Cref;
            ViewBag.Descripcion = Descripcion;
            ViewBag.IDArticulo = IDArticulo;

            return View(atributos);
        }

        [HttpPost]
        public JsonResult addPresentacion(string Presentacion, int? idarticulo)
        {
            try
            {
                Articulo articulo = db.Articulo.Find(idarticulo);
                ViewBag.nombrearticulo = articulo.Descripcion;
                ViewBag.idarticulo = idarticulo;

                Presentacion = Presentacion.TrimEnd(',');
                string[] arraydatos;

                arraydatos = Presentacion.Split(',');
                string acc = null;
                for (int i = 0; i < arraydatos.Length; i++)
                {
                    string cuenta = arraydatos[i];

                    string[] arraydatoscortados;
                    arraydatoscortados = cuenta.Split(':');
                    for (int j = 0; j < arraydatoscortados.Length; j++)
                    {

                        string dato = "?" + arraydatoscortados[j] + "?";

                        if (j + 1 == arraydatoscortados.Length)
                        {
                            acc = acc + dato + ",";
                        }
                        else
                        {
                            acc = acc + dato + ":";
                        }



                    }


                }
                string quitarcoma = acc.TrimEnd(',');
                string jsonPresentacion = "{" + quitarcoma.Replace('?', '"') + "}";
                int NewIDP = db.Database.SqlQuery<int>("SELECT ISNULL(MAX(IDPRESENTACION)+1,0) from Caracteristica where Articulo_IDArticulo = " + idarticulo + " ").FirstOrDefault();
                //int NewIDP = Convert.ToInt32(db.Database.SqlQuery("Select MAX(IDPresentacion)+1 from Caracteristica where Articulo_IDArticulo = ", pre.Articulo_IDArticulo));
                NewIDP = NewIDP > 0 ? NewIDP : 1;
                db.Database.ExecuteSqlCommand("insert into Caracteristica ( IDPresentacion, Cotizacion, version, Presentacion, jsonPresentacion, Articulo_IDArticulo )  values (" + NewIDP + ",0,0,'" + Presentacion + "','" + jsonPresentacion + "'," + idarticulo + ")");


                return Json(true);
                //return Json(idarticulo, JsonRequestBehavior.AllowGet);

            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        //   [HttpPost]
        public ActionResult addPresentacionIsrael(string Presentacion, int? idarticulo)
        {
            try
            {
                Articulo articulo = new ArticuloContext().Articulo.Find(idarticulo);
                ViewBag.nombrearticulo = articulo.Descripcion;
                ViewBag.idarticulo = idarticulo;
                Presentacion = Presentacion.TrimEnd(',');
                string[] arraydatos;

                arraydatos = Presentacion.Split(',');
                string acc = null;
                for (int i = 0; i < arraydatos.Length; i++)
                {
                    string cuenta = arraydatos[i];

                    string[] arraydatoscortados;
                    arraydatoscortados = cuenta.Split(':');
                    for (int j = 0; j < arraydatoscortados.Length; j++)
                    {

                        string dato = "?" + arraydatoscortados[j] + "?";

                        if (j + 1 == arraydatoscortados.Length)
                        {
                            acc = acc + dato + ",";
                        }
                        else
                        {
                            acc = acc + dato + ":";
                        }



                    }


                }
                string quitarcoma = acc.TrimEnd(',');
                string jsonPresentacion = "{" + quitarcoma.Replace('?', '"') + "}";
                int NewIDP = db.Database.SqlQuery<int>("SELECT ISNULL(MAX(IDPRESENTACION)+1,0) from Caracteristica where Articulo_IDArticulo = " + idarticulo + " ").FirstOrDefault();
                //int NewIDP = Convert.ToInt32(db.Database.SqlQuery("Select MAX(IDPresentacion)+1 from Caracteristica where Articulo_IDArticulo = ", pre.Articulo_IDArticulo));
                NewIDP = NewIDP > 0 ? NewIDP : 1;
                db.Database.ExecuteSqlCommand("insert into Caracteristica ( IDPresentacion, Cotizacion, version, Presentacion, jsonPresentacion, Articulo_IDArticulo )  values (" + NewIDP + ",0,0,'" + Presentacion + "','" + jsonPresentacion + "'," + idarticulo + ")");


                return RedirectToAction("Index", new { sortOrder = "", currentFilter = "", searchString = articulo.Cref, page = 1, Familia = "" });

                //return Json(idarticulo, JsonRequestBehavior.AllowGet);

            }
            catch (Exception err)
            {
                Articulo articulo = new ArticuloContext().Articulo.Find(idarticulo);
                return RedirectToAction("Index", new { sortOrder = "", currentFilter = "", searchString = articulo.Cref, page = 1, Familia = "" });
            }
        }

        public ActionResult EditaPresentacion(int? id)
        {
            VCaracteristicaContext db = new VCaracteristicaContext();
            VCaracteristica caracteristica = db.VCaracteristica.Find(id);

            ViewBag.Presentacion = caracteristica.Presentacion;

            Articulo articulo = new ArticuloContext().Articulo.Find(caracteristica.Articulo_IDArticulo);
            ViewBag.IDFamilia = articulo.IDFamilia;
            ViewBag.id = id;

            List<AtributodeFamilia> atributos = db.Database.SqlQuery<AtributodeFamilia>("select * from AtributodeFamilia where IDFamilia='" + articulo.IDFamilia + "'").ToList();

            List<SIAAPI.Models.Administracion.Materiales> materiales = new List<SIAAPI.Models.Administracion.Materiales>();
            string cadenaM = "Select* from Materiales where Obsoleto <> '1'";
            materiales = db.Database.SqlQuery<SIAAPI.Models.Administracion.Materiales>(cadenaM).ToList();
            List<SelectListItem> listaArticulo = new List<SelectListItem>();
            listaArticulo.Add(new SelectListItem { Text = "--Todos los Materiales--", Value = "0" });

            foreach (var m in materiales)
            {
                listaArticulo.Add(new SelectListItem { Text = m.Clave, Value = m.ID.ToString() });
            }

            ViewBag.Materiales = listaArticulo;

            return PartialView(atributos);
        }

        public JsonResult editarPresentacion(int? id, string Presentacion)
        {
            VCaracteristicaContext db = new VCaracteristicaContext();
            Presentacion = Presentacion.TrimEnd(',');
            string[] arraydatos;

            arraydatos = Presentacion.Split(',');
            string acc = null;
            for (int i = 0; i < arraydatos.Length; i++)
            {
                string cuenta = arraydatos[i];

                string[] arraydatoscortados;
                arraydatoscortados = cuenta.Split(':');
                for (int j = 0; j < arraydatoscortados.Length; j++)
                {

                    string dato = "?" + arraydatoscortados[j] + "?";

                    if (j + 1 == arraydatoscortados.Length)
                    {
                        acc = acc + dato + ",";
                    }
                    else
                    {
                        acc = acc + dato + ":";
                    }



                }


            }
            string quitarcoma = acc.TrimEnd(',');
            string jsonPresentacion = "{" + quitarcoma.Replace('?', '"') + "}";

            db.Database.ExecuteSqlCommand("update [dbo].[Caracteristica] set [Presentacion]='" + Presentacion + "',[jsonPresentacion]='" + jsonPresentacion + "' where ID=" + id);

            try
            {
                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int UserID = userid.Select(s => s.UserID).FirstOrDefault();

                //sobreescribir cotización
                string insert = "insert into RegistroEdicionCaracteristica(IDUsuario, Fecha, IDC) values (" + UserID + ", sysdatetime()," + id + ")";
                db.Database.ExecuteSqlCommand(insert);
            }
            catch (Exception err)
            {

            }
            VCaracteristica caracteristica = db.VCaracteristica.Find(id);
            // return RedirectToAction("ListadoPresentaciones", new { id = caracteristica.Articulo_IDArticulo });
            return Json(new { valid = true, message = "lISTO" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RArticulo()
        {
            FamiliaContext db = new FamiliaContext();
            //Filtro familia
            List<Familia> familias = new List<Familia>();
            string cadena = "select * from familia order by Descripcion";
            familias = db.Database.SqlQuery<Familia>(cadena).ToList();
            List<SelectListItem> listafamilia = new List<SelectListItem>();
            listafamilia.Add(new SelectListItem { Text = "--Todas las familias--", Value = "0" });
            foreach (var m in familias)
            {
                listafamilia.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDFamilia.ToString() });
            }
            ViewBag.idfamilia = listafamilia;
            fam elemento = new fam();
            return View(elemento);
        }

        [HttpPost]
        public ActionResult RArticulo(fam modelo, FormCollection coleccion)
        {
            FamiliaContext db = new FamiliaContext();
            VArticuloRepContext dba = new VArticuloRepContext();
            VCaracteristicaRepContext dbc = new VCaracteristicaRepContext();
            VKitRepContext dbk = new VKitRepContext();

            string cual = coleccion.Get("Enviar");

            int idfam = modelo.idfamilia;
            string cadenaart = "";
            string cadenacar = "";
            string cadenakit = "";
            string Fecha = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            if (cual == "Generar reporte")
            {
                return View();
            }
            if (cual == "Generar excel")
            {
                if (idfam == 0)
                {
                    cadenaart = "select * from VArticuloRep where Estado != 'Obsoleto' order by Familia, cref";
                    cadenacar = "select * from VCaracteristicaRep where Estado != 'Obsoleto'  order by Familia, cref";
                    cadenakit = "select * from VKitRep order by Familia, cref;";
                }
                else
                {
                    cadenaart = "select * from VArticuloRep where Estado != 'Obsoleto' and IDFamilia = " + idfam + " order by Familia, cref";
                    cadenacar = "select * from VCaracteristicaRep where Estado != 'Obsoleto' and IDFamilia = " + idfam + " order by Familia, cref";
                    cadenakit = "select * from VKitRep where IDFamilia = " + idfam + "  order by Familia, cref;";
                }
                var datosart = dba.Database.SqlQuery<VArticuloRep>(cadenaart).ToList();
                ViewBag.datosArt = datosart;
                var datoscar = dbc.Database.SqlQuery<VCaracteristicaRep>(cadenacar).ToList();
                ViewBag.datosCar = datoscar;
                var datoskit = dbc.Database.SqlQuery<VKitRep>(cadenakit).ToList();
                ViewBag.datoskit = datoskit;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Articulos");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:S1"].Style.Font.Size = 20;
                Sheet.Cells["A1:S1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:S3"].Style.Font.Bold = true;
                Sheet.Cells["A1:S1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:S1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Articulos");

                row = 2;
                Sheet.Cells["A1:S1"].Style.Font.Size = 12;
                Sheet.Cells["A1:S1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:S1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:S2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = Fecha;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:S3"].Style.Font.Bold = true;
                Sheet.Cells["A3:S3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:S3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Clave");
                Sheet.Cells["B3"].RichText.Add("Articulo");
                Sheet.Cells["C3"].RichText.Add("Estado");
                Sheet.Cells["D3"].RichText.Add("Mínimo Venta");
                Sheet.Cells["E3"].RichText.Add("Mínimo Compra");
                Sheet.Cells["F3"].RichText.Add("Clave Unidad");
                Sheet.Cells["G3"].RichText.Add("Unidad");
                Sheet.Cells["H3"].RichText.Add("Moneda");
                Sheet.Cells["I3"].RichText.Add("Familia");
                Sheet.Cells["J3"].RichText.Add("TipoArticulo");
                Sheet.Cells["K3"].RichText.Add("Precio Único");
                Sheet.Cells["L3"].RichText.Add("Control Stock");
                Sheet.Cells["M3"].RichText.Add("Más de una presentación");
                Sheet.Cells["N3"].RichText.Add("Es Kit");
                Sheet.Cells["O3"].RichText.Add("Genera Orden de Producción");
                Sheet.Cells["P3"].RichText.Add("Observaciones de calidad");
                Sheet.Cells["Q3"].RichText.Add("Existen devoluciones");
                Sheet.Cells["R3"].RichText.Add("Muestreo");
                Sheet.Cells["S3"].RichText.Add("Nivel de Inspección");


                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:S3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VArticuloRep item in ViewBag.datosart)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.Cref;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Articulo;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Estado;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.MinimoVenta;
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.MinimoCompra;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.ClaveUnidad;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Unidad;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Moneda;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.Familia;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.TipoArticulo;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.Preciounico;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.CtrlStock;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.Masdeunapresentacion;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.esKit;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.GeneraOP;
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.Obscalidad;
                    Sheet.Cells[string.Format("Q{0}", row)].Value = item.ExistenDevoluciones;
                    Sheet.Cells[string.Format("R{0}", row)].Value = item.Muestreo;
                    Sheet.Cells[string.Format("S{0}", row)].Value = item.NivelInspeccion;
                    row++;
                }

                //Hoja No. 2
                Sheet = Ep.Workbook.Worksheets.Add("Presentaciones");
                row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:M1"].Style.Font.Size = 20;
                Sheet.Cells["A1:M1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:M3"].Style.Font.Bold = true;
                Sheet.Cells["A1:M1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Presentaciones de los Artículos");

                row = 2;
                Sheet.Cells["A1:M1"].Style.Font.Size = 12;
                Sheet.Cells["A1:M1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:M1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:M2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = Fecha;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:M3"].Style.Font.Bold = true;
                Sheet.Cells["A3:M3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:M3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Clave");
                Sheet.Cells["B3"].RichText.Add("Artículo");
                Sheet.Cells["C3"].RichText.Add("Presentación");
                Sheet.Cells["D3"].RichText.Add("No. Presentación");
                Sheet.Cells["E3"].RichText.Add("ID Cotización");
                Sheet.Cells["F3"].RichText.Add("Estado");
                Sheet.Cells["G3"].RichText.Add("Existencia");
                Sheet.Cells["H3"].RichText.Add("Por llegar");
                Sheet.Cells["I3"].RichText.Add("Apartado");
                Sheet.Cells["J3"].RichText.Add("Disponibilidad");
                Sheet.Cells["K3"].RichText.Add("Stock Mínimo");
                Sheet.Cells["L3"].RichText.Add("Stock Máximo");
                Sheet.Cells["M3"].RichText.Add("Familia");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:M3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VCaracteristicaRep itemD in ViewBag.datoscar)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = itemD.cref;
                    Sheet.Cells[string.Format("B{0}", row)].Value = itemD.Articulo;
                    Sheet.Cells[string.Format("C{0}", row)].Value = itemD.Presentacion;
                    Sheet.Cells[string.Format("D{0}", row)].Value = itemD.IDPresentacion;
                    Sheet.Cells[string.Format("E{0}", row)].Value = itemD.IDCotizacion;
                    Sheet.Cells[string.Format("F{0}", row)].Value = itemD.Estado;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("G{0}", row)].Value = itemD.Existencia;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("H{0}", row)].Value = itemD.PorLlegar;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("I{0}", row)].Value = itemD.Apartado;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("J{0}", row)].Value = itemD.Disponibilidad;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("K{0}", row)].Value = itemD.StockMin;
                    Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("L{0}", row)].Value = itemD.StockMax;
                    Sheet.Cells[string.Format("M{0}", row)].Value = itemD.Familia;

                    row++;
                }

                //Hoja No. 3
                Sheet = Ep.Workbook.Worksheets.Add("Kits");
                row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:J1"].Style.Font.Size = 20;
                Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:J3"].Style.Font.Bold = true;
                Sheet.Cells["A1:J1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Kits de los Artículos");

                row = 2;
                Sheet.Cells["A1:J1"].Style.Font.Size = 12;
                Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:J2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = Fecha;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:J3"].Style.Font.Bold = true;
                Sheet.Cells["A3:J3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:J3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Clave");
                Sheet.Cells["B3"].RichText.Add("Artículo");
                Sheet.Cells["C3"].RichText.Add("Familia");
                Sheet.Cells["D3"].RichText.Add("Clave Artículo Componente");
                Sheet.Cells["E3"].RichText.Add("Articulo Componente");
                Sheet.Cells["F3"].RichText.Add("Presentacion componente");
                Sheet.Cells["G3"].RichText.Add("Cantidad");
                Sheet.Cells["H3"].RichText.Add("Compuesto por");
                Sheet.Cells["I3"].RichText.Add("Almacen");
                Sheet.Cells["J3"].RichText.Add("Precio Único");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:J3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VKitRep itemk in ViewBag.datoskit)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = itemk.Cref;
                    Sheet.Cells[string.Format("B{0}", row)].Value = itemk.Descripcion;
                    Sheet.Cells[string.Format("C{0}", row)].Value = itemk.Familia;
                    int idarticulo = itemk.IDArticuloComp;

                    ClsDatoEntero countart = db.Database.SqlQuery<ClsDatoEntero>("select count(IDArticulo) as Dato from Articulo where IDArticulo =" + idarticulo + "").ToList()[0];
                    if (countart.Dato != 0)
                    {
                        Articulo art = new ArticuloContext().Articulo.Find(idarticulo);
                        string clave = art.Cref;
                        string articulo = art.Descripcion;
                        Sheet.Cells[string.Format("D{0}", row)].Value = clave;
                        Sheet.Cells[string.Format("E{0}", row)].Value = articulo;
                    }
                    int id = itemk.IDCaracteristica;
                    ClsDatoEntero countcar = db.Database.SqlQuery<ClsDatoEntero>("select count(ID) as Dato from caracteristica where ID = " + id + "").ToList()[0];
                    if (countcar.Dato != 0)
                    {
                        if (itemk.IDCaracteristica != 0)
                        {
                            Caracteristica car = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + id).FirstOrDefault();
                            string pres = car.Presentacion;
                            Sheet.Cells[string.Format("F{0}", row)].Value = pres;
                        }
                    }

                    Sheet.Cells[string.Format("G{0}", row)].Value = itemk.Cantidad;
                    Sheet.Cells[string.Format("H{0}", row)].Value = itemk.composicion;
                    Sheet.Cells[string.Format("I{0}", row)].Value = itemk.Almacen;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("J{0}", row)].Value = itemk.Precio;
                    Sheet.Cells[string.Format("K{0}", row)].Value = itemk.Preciounico;

                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "Articulos.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult MatrizPrecio(int? id)
        {

            List<MatrizPrecio> elementos = db.Database.SqlQuery<MatrizPrecio>("select * from [dbo].[MatrizPrecio] where IDArticulo =" + id + "").ToList();
            Articulo articulo = db.Articulo.Find(id);
            ViewBag.id = id;
            ViewBag.nombrearticulo = articulo.Descripcion;

            ViewBag.rangoinferiormp = .01;
            ViewBag.rangosuperiormp = 999.99;

            ClsDatoEntero countRangoP = db.Database.SqlQuery<ClsDatoEntero>("select count(idMatrizPrecio) as Dato from MatrizPrecio where IDArticulo =" + id + "").ToList()[0];
            if (countRangoP.Dato != 0)
            {
                List<MatrizPrecio> rangoP = db.Database.SqlQuery<MatrizPrecio>("select * from [dbo].[MatrizPrecio] where idMatrizPrecio=(SELECT MAX(idMatrizPrecio) from MatrizPrecio where IDArticulo=" + id + ") and IDArticulo =" + id + "").ToList();
                decimal nump = rangoP.Select(s => s.RangSup).FirstOrDefault();
                decimal rangocorrectop = nump + .01M;
                ViewBag.rangoinferiormp = rangocorrectop;
                ViewBag.rangosuperiormp = rangocorrectop + .01M;
            }
            ViewBag.countRangoP = countRangoP.Dato;
            return PartialView(elementos);

        }



        [HttpPost]
        public ActionResult InsertarMatrizPrecio(int? idmp, decimal? ranginfmp, decimal? rangsupmp, decimal? preciomp)
        {
            Articulo articulo = db.Articulo.Find(idmp);
            try
            {
                if (articulo.Preciounico == true)
                {
                    db.Database.ExecuteSqlCommand("Exec GuardaPrecioUnico " + idmp + "," + preciomp);
                }
                else
                {
                    string cadena = "insert into MatrizPrecio (IDArticulo, Ranginf, RangSup, Precio) values (" + idmp + "," + ranginfmp + "," + rangsupmp + "," + preciomp + ")";
                    db.Database.ExecuteSqlCommand(cadena);
                }

            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
            //return RedirectToAction("MatrizPrecio", new { id = idmp });
            return RedirectToAction("Details", new { id = idmp });
        }


        [HttpPost]
        public JsonResult EdititemMatrizPrecio(int id, decimal rangi, decimal rangs, decimal precio)
        {
            try
            {
                db.Database.ExecuteSqlCommand("update [dbo].[MatrizPrecio] set [RangInf]=" + rangi + ", [RangSup]='" + rangs + "' , Precio='" + precio + "' where IDMatrizPrecio=" + id);


                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        [HttpPost]
        public JsonResult DeleteitemMatrizPrecio(int id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from [dbo].[MatrizPrecio] where [idMatrizPrecio]='" + id + "'");
                return Json(true);

            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult MatrizCosto(int? id)
        {

            List<MatrizCosto> elementos = db.Database.SqlQuery<MatrizCosto>("select * from [dbo].[MatrizCosto] where IDArticulo =" + id + "").ToList();

            Articulo articulo = db.Articulo.Find(id);
            ViewBag.id = id;
            ViewBag.nombrearticulo = articulo.Descripcion;

            ViewBag.rangoinferior = .01;
            ViewBag.rangosuperior = 999.99;

            ClsDatoEntero countRango = db.Database.SqlQuery<ClsDatoEntero>("select count(idMatrizCosto) as Dato from MatrizCosto where IDArticulo =" + id + "").ToList()[0];
            if (countRango.Dato != 0)
            {
                List<MatrizCosto> rango = db.Database.SqlQuery<MatrizCosto>("select * from [dbo].[MatrizCosto] where idMatrizCosto=(SELECT MAX(idMatrizCosto) from MatrizCosto where IDArticulo=" + id + ") and IDArticulo =" + id + "").ToList();
                decimal num = rango.Select(s => s.RangSup).FirstOrDefault();
                decimal rangocorrecto = num + .01M;
                ViewBag.rangoinferior = rangocorrecto;
                ViewBag.rangosuperior = rangocorrecto + .01M;
            }


            return PartialView(elementos);
        }



        [HttpPost]
        public ActionResult InsertarMatrizCosto(int? id, decimal? ranginf, decimal? rangsup, decimal? precio)
        {
            Articulo articulo = db.Articulo.Find(id);
            try
            {

                db.Database.ExecuteSqlCommand("insert into MatrizCosto(IDArticulo, Ranginf, RangSup, Precio) values (" + id + "," + ranginf + "," + rangsup + "," + precio + ")");


            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
            return RedirectToAction("Details", new { id = id });
        }


        [HttpPost]
        public JsonResult EdititemMatrizCosto(int id, decimal rangi, decimal rangs, decimal precio)
        {
            try
            {
                string cadena = "update [dbo].[MatrizCosto] set [RangInf]=" + rangi + ", [RangSup]='" + rangs + "' , Precio='" + precio + "' where IDMatrizCosto=" + id;
                db.Database.ExecuteSqlCommand(cadena);


                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        [HttpPost]
        public JsonResult DeleteitemMatrizCosto(int id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from [dbo].[MatrizCosto] where [idMatrizCosto]='" + id + "'");
                return Json(true);

            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        [HttpPost]
        public JsonResult Formula(int? IDArticulo, int? IDProceso)
        {


            try

            {
                ClsDatoEntero var = db.Database.SqlQuery<ClsDatoEntero>("select count(IDArticulo) as Dato from MaquinaProceso where IDArticulo=" + IDArticulo).ToList()[0];

                if (var.Dato == 0)
                {
                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[MaquinaProceso]([IDArticulo],[IDProceso])VALUES (" + IDArticulo + "," + IDProceso + ")");

                }
                else
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[MaquinaProceso] set [IDArticulo]=" + IDArticulo + ", [IDProceso]=" + IDProceso + " where IDArticulo=" + IDArticulo);


                }
                return Json(true);
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                return Json(500, err.Message);
            }
        }

        public ActionResult AgregarProceso(int? id)
        {
            Articulo articulo = new ArticuloContext().Articulo.Find(id);
            ViewBag.NombreArticulo = articulo.Descripcion;
            ViewBag.IDArticulo = id;
            ProcesoContext db = new ProcesoContext();

            ClsDatoEntero var = db.Database.SqlQuery<ClsDatoEntero>("select count(IDArticulo) as Dato from MaquinaProceso where IDArticulo=" + id).ToList()[0];
            if (var.Dato == 0)
            {
                ViewBag.IDProceso = new SelectList(db.Procesos, "IDProceso", "NombreProceso");
            }
            else
            {
                ClsDatoEntero idart = db.Database.SqlQuery<ClsDatoEntero>("select IDProceso as Dato from MaquinaProceso where IDArticulo=" + id).ToList()[0];

                ViewBag.IDProceso = new SelectList(db.Procesos, "IDProceso", "NombreProceso", idart.Dato);
            }

            return PartialView();
        }
        [HttpPost]
        public ActionResult Obsoleto(int? id, int? obsoleto)
        {
            try
            {

                db.Database.ExecuteSqlCommand("update [dbo].[Caracteristica] set [obsoleto]=" + obsoleto + " where ID=" + id);


                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }

        }


        private ProveedorContext dbp = new ProveedorContext();
        private c_MonedaContext dbm = new c_MonedaContext();
        public ActionResult VMatrizPrecioProv(int? id)
        {


            List<VMatrizPrecioProv> elementos = db.Database.SqlQuery<VMatrizPrecioProv>("select IDMatrizPrecio, IDProveedor, Empresa, IDArticulo, Descripcion, RangInf, RangSup, Precio, IDMoneda, Moneda from [dbo].[VMatrizPrecioProv] where IDArticulo = " + id + "").ToList();
            Articulo articulo = db.Articulo.Find(id);
            ViewBag.id = id;
            ViewBag.nombrearticulo = articulo.Descripcion;
            ViewBag.rangoinferior = .01;
            ViewBag.rangosuperior = 999.99;

            ViewBag.IDMoneda = new MonedaRepository().GetMoneda();
            ViewBag.IDProveedor = new ProveedorAllRepository().GetProveedor();
            ClsDatoEntero countRango = db.Database.SqlQuery<ClsDatoEntero>("select count(idMatrizPrecio) as Dato from VMatrizPrecioProv where IDArticulo =" + id + "").ToList()[0];
            if (countRango.Dato != 0)
            {
                List<VMatrizPrecioProv> rango = db.Database.SqlQuery<VMatrizPrecioProv>("select * from [dbo].[VMatrizPrecioProv] where idMatrizPrecio=(SELECT MAX(idMatrizPrecio) from VMatrizPrecioProv where IDArticulo=" + id + ") and IDArticulo =" + id + "").ToList();
                decimal num = rango.Select(s => s.RangSup).FirstOrDefault();
                decimal rangocorrecto = num + .01M;
                ViewBag.rangoinferior = rangocorrecto;
                ViewBag.rangosuperior = rangocorrecto + .01M;
            }


            return PartialView(elementos);
        }


        public ActionResult InsertarMatrizPrecioProv(int? id)
        {

            Articulo articulo = db.Articulo.Find(id);
            ViewBag.id = id;
            ViewBag.nombrearticulo = articulo.Descripcion;

            ViewBag.rangoinferior = .01;
            ViewBag.rangosuperior = 999.99;
            ViewBag.IDMoneda = new MonedaRepository().GetMoneda();
            ViewBag.IDProveedor = new ProveedorAllRepository().GetProveedor();

            return RedirectToAction("InsertarMatrizPrecioProv", new { id = id });
        }


        [HttpPost]
        public ActionResult InsertarMatrizPrecioProv(int? id, int? idprov, string CrefP, decimal? RangInf, decimal? RangSup, decimal? precio, decimal? idmoneda, string Observacion)
        {

            try
            {
                // TODO: Add insert logic here
                db.Database.ExecuteSqlCommand("insert into MatrizPrecioProv(IDProveedor,Cref, Ranginf, RangSup, Precio, IDArticulo, IDMoneda,Observacion) values (" + idprov + ",'" + CrefP + "'," + RangInf + "," + RangSup + "," + precio + "," + id + "," + idmoneda + ",'" + Observacion + "')");
                return Json(true);
            }
            catch (Exception err)
            {
                ViewBag.IDMoneda = new MonedaRepository().GetMoneda();
                ViewBag.IDProveedor = new ProveedorAllRepository().GetProveedor();
                return Json(500, err.Message);

            }
            return RedirectToAction("MatrizPrecioProv", new { id = id });
        }
        [HttpPost]
        public JsonResult EdititemMatrizPrecioProv(int? id, decimal? RangInf, decimal? RangSup, decimal? precio, string observacion = "")
        {
            try
            {
                db.Database.ExecuteSqlCommand("update [dbo].[MatrizPrecioProv] set [RangInf]=" + RangInf + ", [RangSup]=" + RangSup + " , Precio=" + precio + ", observacion= '" + observacion + "' where IDMatrizPrecio=" + id);

                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        [HttpPost]
        public JsonResult DeleteitemMatrizPrecioProv(int id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from [dbo].[MatrizPrecioProv] where [idMatrizPrecio]='" + id + "'");
                return Json(true);

            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }



        public ActionResult Inventario(int IDPresentacion)
        {
            Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDPresentacion).ToList().FirstOrDefault();



            Articulo arti = new ArticuloContext().Articulo.Find(cara.Articulo_IDArticulo);



            ViewBag.Articulo = arti;

            ViewBag.Presentacion = cara;

            var elementos = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDCaracteristica == cara.ID);


            return View(elementos);


        }



        public ActionResult Kardex(int IDAlmacen, int IDArticulo, int IDCaracteristica, DateTime? FechaI, DateTime? FechaF, int? page, int? PageSize)
        {
            List<VMovimientoAlmacen> elementoskardex;

            Articulo articulo = new ArticuloContext().Articulo.Find(IDArticulo);

            ViewBag.Articulo = articulo.Descripcion;

            //Caracteristica caracteristica = new ArticuloContext().Caracteristica.Find(IDCaracteristica);


            //ViewBag.Caracteristica = caracteristica.Presentacion;




            //Buscar SearchString
            if ((FechaI == null) || (FechaF == null))
            {
                elementoskardex = new VMovimientoAlmacenContext().Database.SqlQuery<VMovimientoAlmacen>("select * from VMovimientoAlmacen where IDArticulo=" + IDArticulo + " and IDCaracteristica=" + IDCaracteristica + " and IDAlmacen=" + IDAlmacen + " order by fechaMovimiento desc,hora desc").ToList();
            }
            else
            {
                elementoskardex = new VMovimientoAlmacenContext().Database.SqlQuery<VMovimientoAlmacen>("select * from VMovimientoAlmacen where IDArticulo=" + IDArticulo + " and IDCaracteristica=" + IDCaracteristica + " and IDAlmacen=" + IDAlmacen + " and FechaMovimiento >='" + FechaI.Value.Year + "/" + FechaI.Value.Month + "/" + FechaI.Value.Day + "' and FechaMovimiento<='" + FechaF.Value.Year + "/" + FechaF.Value.Month + "/" + FechaF.Value.Day + "' order by fechamovimiento desc, hora desc").ToList();
            }


            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = elementoskardex.Count(); // Total number of elements

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

            //return View(elementos.ToPagedList(pageNumber, pageSize));
            return View(elementoskardex.ToPagedList(pageNumber, pageSize));
            //Paginación

        }

        [HttpPost]
        public ActionResult GeneraPlotter(int? idarticulo, int? idpresentacion, int? genera, int? idplotter)
        {
            try
            {
                if (genera == 1)
                {
                    db.Database.ExecuteSqlCommand("insert into [dbo].[ArticulosGOPPlotter] (IDArticulo, IDPresentacion) values (" + idarticulo + "," + idpresentacion + ")");
                }
                else
                {
                    db.Database.ExecuteSqlCommand("delete from [dbo].[ArticulosGOPPlotter] where IDPlotter=" + idplotter);
                }




                //db.Database.ExecuteSqlCommand("update [dbo].[ArticulosGOPPlotter] set [GeneraOPR]='" + genera + "' where IDCaracteristica=" + id);


                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }

        }
        // POST: Articulos/Details/5


        public ActionResult Details(int id, int pagina = 1, string searchString = "", string Familia = "")
        {
            ArticuloContext db = new ArticuloContext();
            var elemento = db.Articulo.Single(m => m.IDArticulo == id);
            ViewBag.id = int.Parse(elemento.IDArticulo.ToString());
            ViewBag.cref = elemento.Cref;
            ViewBag.articulo = elemento.Descripcion;
            ViewBag.articulos = elemento;
            ViewBag.pagina = pagina;
            ViewBag.searchString = searchString;
            ViewBag.Familia = Familia;
            ViewBag.ActiveTab = 1;
            return View(elemento);
        }

        /// <summary>
        /// Inserta un componente del Kit
        /// </summary>
        /// <param name="IDArticuloComp"></param>
        /// <param name="id"></param>
        /// <param name="cantidad"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult InsertarComponente(int IDArticuloComp, int id, decimal cantidad, string tipo)
        {
            int porc = 0;
            int porp = 0;
            //try
            //{
            if (tipo.Equals("C"))
            {
                porc = 1;
            }
            else if (tipo.Equals("P"))
            {
                porp = 1;
            }
            try
            {
                string clave = db.Database.SqlQuery<string>("select cref from Articulo where idArticulo=" + IDArticuloComp + " ").FirstOrDefault();

                db.Database.ExecuteSqlCommand("INSERT INTO Kit([IDArticulo],[IDArticuloComp],[Cantidad],[Clave],[porcantidad],[porporcentaje]) values ('" + id + "','" + IDArticuloComp + "','" + cantidad + "', '" + clave + "','" + porc + "','" + porp + "')");

            }
            catch { }
            //  ACGREGAR COSTO KIT
            try
            {

                int idmonedaArticuloKit = 0;
                try
                {
                    Articulo articuloKit = new ArticuloContext().Articulo.Find(id);
                    idmonedaArticuloKit = articuloKit.IDMoneda;
                }
                catch (Exception err)
                {

                }
                //idarticulo = idarticulo del kit

                //borrarmatriz costo del idarticulo


                db.Database.ExecuteSqlCommand("delete from MatrizCosto where idarticulo=" + id);

                bool esporporcentaje = esporcentaje(id);




                decimal sumacantidadesK = sumacantidades(id);

                decimal costokit = 0;

                List<Kit> kit = new KitContext().Database.SqlQuery<Kit>("select*from Kit where idarticulo=" + id).ToList();
                foreach (Kit componentes in kit)
                {

                    Articulo articuloComponenete = new ArticuloContext().Articulo.Find(componentes.IDArticuloComp);

                    decimal costocomponente = 0M;
                    int monedacomponente = 0;
                    decimal costoconvertido = 0M;
                    string cadenaCosto = "dbo.getcosto(0, " + componentes.IDArticuloComp + ", " + componentes.Cantidad + ") as dato";
                    try
                    {
                        costocomponente = db.Database.SqlQuery<ClsDatoDecimal>(cadenaCosto).ToList().FirstOrDefault().Dato;

                    }
                    catch (Exception err)
                    {

                    }


                    monedacomponente = articuloComponenete.IDMoneda;
                    //PREGUNTAR SI TOMAR EL TC DEL DÍA DEL INGRESO PARA CONVERTIRLO
                    // costoconveritido = convetir el costocomponente a la moneda del articulo del kit

                    try
                    {
                        DateTime fecha = DateTime.Now;
                        string fecha1 = fecha.ToString("yyyy/MM/dd");

                        List<c_Moneda> monedaorigen;
                        monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
                        int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();




                        VCambio tcambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha1 + "'," + monedacomponente + "," + idmonedaArticuloKit + ") as TC").ToList().FirstOrDefault();
                        decimal tCambio = tcambio.TC;

                        try
                        {
                            costoconvertido = costocomponente * tCambio;
                        }
                        catch (Exception err)
                        {

                        }
                    }
                    catch (Exception err)
                    {

                    }

                    if (esporporcentaje)
                    {
                        costokit = costokit + (costoconvertido * cantidad);
                    }
                    else
                    {
                        costokit = costokit + (costoconvertido * (cantidad / sumacantidadesK));

                    }
                }

                try
                {
                    try
                    {

                        db.Database.ExecuteSqlCommand("insert into MatrizCosto(IDArticulo, Ranginf, RangSup, Precio) values (" + id + ",0.01,99999," + costokit + ")");


                    }
                    catch (Exception err)
                    {
                        return Json(500, err.Message);
                    }
                }
                catch (Exception ERR)
                {

                }
                //crear matrizcosto del kit con el rago 0.01 hasta 99999 con el costodelkit y la monedad del kit
            }
            catch (Exception err)
            {

            }

            ViewBag.elementos = null;
            ViewBag.form = null;


            return RedirectToAction("Details", new { id = id });

        }
        [HttpPost]
        public JsonResult EdititemKit(int id, decimal cantidad, decimal precio, int idalmacen)
        {
            try
            {
                KitContext car = new KitContext();
                db.Database.ExecuteSqlCommand("update [dbo].[Kit] set [Cantidad]=" + cantidad + ", [Precio]=" + precio + ", idalmacen=" + idalmacen + "  where IDKit=" + id);


                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        public bool esporcentaje(int idarticulo)
        {
            bool es = false;
            try
            {
                string cadena = "select  dbo.elkitesporcentaje (" + idarticulo + ") as dato";
                ClsDatoBool clsDato = db.Database.SqlQuery<ClsDatoBool>(cadena).ToList().FirstOrDefault();
                es = clsDato.Dato;
            }
            catch (Exception err)
            {

            }
            return es;
        }
        public decimal sumacantidades(int idarticulo)
        {
            decimal suma = 0M;
            try
            {
                string cadena = "select dbo.sumacantidadeskit (" + idarticulo + ") as dato";
                ClsDatoDecimal clsDato = db.Database.SqlQuery<ClsDatoDecimal>(cadena).ToList().FirstOrDefault();
                suma = clsDato.Dato;
            }
            catch (Exception err)
            {

            }
            return suma;
        }
        public ActionResult AgregarCarritoGeneral(FormCollection coleccion, int? id, string searchString = "")

        {
            ViewBag.searchString = searchString;
            decimal cantidad = decimal.Parse(coleccion.Get("Cantidad").ToString());
            int idCara = int.Parse(coleccion.Get("id").ToString());

            ArticuloContext db = new ArticuloContext();

            Caracteristica c = db.Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + id).ToList()[0];
            VCambio cantidadm = db.Database.SqlQuery<VCambio>("select Articulo.MinimoCompra as TC from Caracteristica inner join Articulo on Articulo.IDArticulo=Caracteristica.Articulo_IDArticulo where Caracteristica.ID=" + id).ToList()[0];
            // decimal cantidad= cantidadm.TC;
            Articulo articulo = db.Articulo.Find(c.Articulo_IDArticulo);
            if (cantidad == 0)
            {
                cantidad = 1;
            }
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            // CarritoContext carrito = new CarritoContext();
            //    var consulta = carrito.Carritos.Where(a => a.IDCaracteristica.Equals(id)).FirstOrDefault();
            List<CarritoC> numero;
            numero = db.Database.SqlQuery<CarritoC>("SELECT * FROM [dbo].[CarritoC] where IDCaracteristica='" + idCara + "' and usuario='" + usuario + "'").ToList();
            int consulta = numero.Select(s => s.IDCaracteristica).FirstOrDefault();


            if (consulta != 0)
            {


            }
            else
            {
                string cadena = "insert into CarritoC (usuario, IDCaracteristica, Cantidad, Precio, IDMoneda,IDProveedor) values ('" + usuario + "'," + id + ",'" + cantidad + "',dbo.GetCosto(" + c.Articulo_IDArticulo + "," + cantidad + "),'" + articulo.IDMoneda + "',0)";

                db.Database.ExecuteSqlCommand(cadena);
            }

            return RedirectToAction("Details", new { id = c.Articulo_IDArticulo });


        }


        public ActionResult ImprimirEtiquetasC(int id, int idpresentacion)
        {
            Empresa empresa = new EmpresaContext().empresas.Find(2);
            string cadena = "select * from VArtCaracteristica where IDCaracteristica=" + id;
            List<VArtCaracteristica> elementos = new VArtCaracteristicaContext().Database.SqlQuery<VArtCaracteristica>(cadena).ToList();
            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            VArtCaracteristica art = new VArtCaracteristicaContext().VArtC.Find(id);
            if (elementos.Count > 0)
            {
                string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                Reportes.CrearEtiquetaPreArt documento = new Reportes.CrearEtiquetaPreArt(logoempresa, elementos, id, idpresentacion);
                return new FilePathResult(documento.nombreDocumento, contentType);
            }
            else
            {
                return Content("No hay lotes");
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


        public List<Caracteristica> GetlistCaracteristica(int idarticulo)
        {
            List<Caracteristica> presenta = new List<Caracteristica>();
            try
            {
                string cadenaf = "select* from dbo.Caracteristica where Articulo_IDArticulo = " + idarticulo + " order by ID";
                presenta = db.Database.SqlQuery<Caracteristica>(cadenaf).ToList();
            }
            catch (Exception err)
            {
                //string mensajedeerror = err.Message;
            }
            return presenta;
        }

        public ActionResult ReporteArticulo()
        {
            List<Articulo> articulos = new List<Articulo>();
            string cadena = "select * from Articulo order by Descripcion";
            articulos = db.Database.SqlQuery<Articulo>(cadena).ToList();
            List<SelectListItem> listaArticulo = new List<SelectListItem>();
            listaArticulo.Add(new SelectListItem { Text = "--Todos los artículos--", Value = "0" });

            foreach (var m in articulos)
            {
                listaArticulo.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDArticulo.ToString() });
            }
            ViewBag.articulo = listaArticulo;
            return View();

        }

        [HttpPost]
        public ActionResult ReporteArticulo(ReportePorArticulo modelo, FormCollection coleccion, ArticuloRe v)
        {
            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {
                try
                {

                    ArticuloContext dbc = new ArticuloContext();
                    Articulo ven = dbc.Articulo.Find(v.IDArticulo);
                }
                catch (Exception ERR)
                {

                }
                int idArticulo = v.IDArticulo;
                ReportePorArticulo report = new ReportePorArticulo();
                //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"), DateTime.Parse("2019-07-30"), idcliente);
                byte[] abytes = report.PrepareReport(idArticulo);
                return File(abytes, "application/pdf");
            }
            else
            {

                //Listado de datos
                ArticuloContext db = new ArticuloContext();
                var ARTICULOS = db.Articulo.ToList();
                ExcelPackage Ep = new ExcelPackage();
                var Sheet = Ep.Workbook.Worksheets.Add("Articulos");

                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:H1"].Style.Font.Size = 20;
                Sheet.Cells["A1:H1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:H1"].Style.Font.Bold = true;
                Sheet.Cells["A1:H1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Artículos");
                Sheet.Cells["A1:H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                row = 2;
                Sheet.Cells["A2:H2"].Style.Font.Name = "Calibri";
                Sheet.Cells["A2:H2"].Style.Font.Size = 12;
                Sheet.Cells["A2:H2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                Sheet.Cells["A2:H2"].Style.Font.Bold = true;
                Sheet.Cells["A2:H2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A2:H2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);




                Sheet.Cells["B2"].RichText.Add("Clave");
                Sheet.Cells["A2"].RichText.Add("Descripción");
                Sheet.Cells["C2"].RichText.Add("Clave Unidad");
                Sheet.Cells["D2"].RichText.Add("Moneda");
                Sheet.Cells["E2"].RichText.Add("Familia");
                Sheet.Cells["F2"].RichText.Add("Tipo Articulo");
                Sheet.Cells["G2"].RichText.Add("Min. Compra");
                Sheet.Cells["H2"].RichText.Add("Min. Venta");

                row = 3;
                foreach (var item in ARTICULOS)
                {
                    string unidad = new c_ClaveUnidadContext().c_ClaveUnidades.Find(item.IDClaveUnidad).ClaveUnidad;
                    string moneda = new c_MonedaContext().c_Monedas.Find(item.IDMoneda).ClaveMoneda;
                    string FAMILIA = new FamiliaContext().Familias.Find(item.IDFamilia).Descripcion;
                    string TIPOA = new ArticuloContext().TipoArticulo.Find(item.IDTipoArticulo).Descripcion;

                    Sheet.Cells[string.Format("A{0}:H{0}", row)].Style.Font.Name = "Calibri";
                    Sheet.Cells[string.Format("A{0}:H{0}", row)].Style.Font.Size = 12;
                    Sheet.Cells[string.Format("A{0}:H{0}", row)].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                    Sheet.Cells[string.Format("A{0}:H{0}", row)].Style.Font.Bold = true;
                    Sheet.Cells[string.Format("A{0}:H{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}:H{0}", row)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.Cref;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Descripcion;
                    Sheet.Cells[string.Format("C{0}", row)].Value = unidad;
                    Sheet.Cells[string.Format("D{0}", row)].Value = moneda;
                    Sheet.Cells[string.Format("E{0}", row)].Value = FAMILIA;
                    Sheet.Cells[string.Format("F{0}", row)].Value = TIPOA;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.MinimoCompra;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.MinimoVenta;

                    row++;

                    //Presentaciones del articulo
                    //VCaracteristicaContext db1 = new VCaracteristicaContext();
                    //var carac = db1.VCaracteristica.ToList();

                    List<Caracteristica> presenta = new List<Caracteristica>();
                    presenta = GetlistCaracteristica(item.IDArticulo);


                    var numero = 0;
                    foreach (Caracteristica pres in presenta)
                    {

                        numero += 1;
                        try
                        {
                            Sheet.Cells[string.Format("A{0}", row)].Value = numero;
                        }
                        catch (Exception err)
                        {

                        }

                        Sheet.Cells[string.Format("B{0}", row)].Value = pres.Presentacion;
                        Sheet.Cells[string.Format("C{0}", row)].Value = "";
                        Sheet.Cells[string.Format("D{0}", row)].Value = "";
                        Sheet.Cells[string.Format("E{0}", row)].Value = "";
                        Sheet.Cells[string.Format("F{0}", row)].Value = "";
                        Sheet.Cells[string.Format("G{0}", row)].Value = "";
                        Sheet.Cells[string.Format("H{0}", row)].Value = "";
                        row++;
                    }

                    //row++;


                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelArticulos.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");

            }
            //return Redirect("index");
        }

        public ActionResult ReporteArticuloPorFamilia()
        {

            List<Familia> familia = new List<Familia>();
            string cadenaf = "SELECT*FROM Familia order by Descripcion";
            familia = db.Database.SqlQuery<Familia>(cadenaf).ToList();
            List<SelectListItem> listafamilia = new List<SelectListItem>();
            listafamilia.Add(new SelectListItem { Text = "--Todas las familias--", Value = "0" });

            foreach (var m in familia)
            {
                listafamilia.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDFamilia.ToString() });
            }
            ViewBag.familia = listafamilia;

            return View();
        }

        [HttpPost]
        public ActionResult ReporteArticuloPorFamilia(ReportePorFamilia modelo, ArticuloFE A)
        {
            int idfamilia = A.IDFamilia;
            try
            {

                FamiliaContext dbc = new FamiliaContext();
                Familia cls = dbc.Familias.Find(A.IDFamilia);
            }
            catch (Exception ERR)
            {

            }

            ReportePorFamilia report = new ReportePorFamilia();
            //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"), DateTime.Parse("2019-07-30"), idcliente);
            byte[] abytes = report.PrepareReport(idfamilia);
            return File(abytes, "application/pdf");
            //return Redirect("index");
        }

        public ActionResult ReporteArticuloPorMoneda()
        {
            //Moneda
            List<c_Moneda> moneda = new List<c_Moneda>();
            string cadenam = "SELECT * FROM c_Moneda order by Descripcion";
            moneda = db.Database.SqlQuery<c_Moneda>(cadenam).ToList();
            List<SelectListItem> listamoneda = new List<SelectListItem>();
            listamoneda.Add(new SelectListItem { Text = "--Todas las monedas--", Value = "0" });

            foreach (var m in moneda)
            {
                listamoneda.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDMoneda.ToString() });
            }
            ViewBag.moneda = listamoneda;
            return View();

        }

        [HttpPost]
        public ActionResult ReporteArticuloPorMoneda(ReportePorMoneda modelo, MonedaRe m)
        {
            int idMoneda = m.IDMoneda;
            try
            {

                ArticuloContext dbc = new ArticuloContext();
                //Articulo ven = dbc.Articulo.Find(m.IDArticulo);
            }
            catch (Exception ERR)
            {

            }

            ReportePorMoneda report = new ReportePorMoneda();
            //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"), DateTime.Parse("2019-07-30"), idcliente);
            byte[] abytes = report.prepararReporte(idMoneda);
            return File(abytes, "application/pdf");
            //return Redirect("index");
        }

        public ActionResult ReporteArticuloPorUnidad()
        {
            //Moneda
            List<c_ClaveUnidad> unidad = new List<c_ClaveUnidad>(); //aqui seleccionar el catalogo de datos
            string cadenam = "select * from c_ClaveUnidad order by Nombre";
            unidad = db.Database.SqlQuery<c_ClaveUnidad>(cadenam).ToList();
            List<SelectListItem> listaunidad = new List<SelectListItem>();
            listaunidad.Add(new SelectListItem { Text = "--Todas las unidades--", Value = "0" });

            foreach (var m in unidad)
            {
                listaunidad.Add(new SelectListItem { Text = m.Nombre, Value = m.IDClaveUnidad.ToString() }); //se modifica en base a c_XXXX en Models/Administracion/..
            }
            ViewBag.unidad = listaunidad;
            return View();

        }

        public ActionResult getJsonCaracteristicaArticulo(int id)
        {
            var presentacion = new ArticuloRepository().GetCaracteristicaPorArticulo(id);
            return Json(presentacion, JsonRequestBehavior.AllowGet); ;

        }

        public IEnumerable<SelectListItem> getPresentacionPorArticulo(int ida)
        {
            var presentacion = new ArticuloRepository().GetCaracteristicaPorArticulo(ida);
            return presentacion;

        }
        [HttpPost]
        public JsonResult Deleteitem(int id)
        {

            try
            {
                db.Database.ExecuteSqlCommand("delete from [dbo].Kit where idArticuloComp=" + id + "");


                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }
        [HttpPost]
        public ActionResult ReporteArticuloPorUnidad(ReportePorUnidad modelo, UnidadGetSet u)
        {
            int idUnidad = u.IDClaveUnidad;
            try
            {

                ArticuloContext dbc = new ArticuloContext();
                //Articulo ven = dbc.Articulo.Find(m.IDArticulo);
            }
            catch (Exception ERR)
            {

            }

            ReportePorUnidad report = new ReportePorUnidad();
            //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"), DateTime.Parse("2019-07-30"), idcliente);
            byte[] abytes = report.prepararReporte(idUnidad);
            return File(abytes, "application/pdf");
            //return Redirect("index");
        }

        public ActionResult ReporteArticuloPorTipoArticulo()
        {
            //Moneda
            List<TipoArticulo> tipoart = new List<TipoArticulo>(); //aqui seleccionar el catalogo de datos
            string cadenam = "select * from TipoArticulo order by Descripcion";
            tipoart = db.Database.SqlQuery<TipoArticulo>(cadenam).ToList();
            List<SelectListItem> listatipoart = new List<SelectListItem>();
            listatipoart.Add(new SelectListItem { Text = "--Todos los tipos de artículo--", Value = "0" });

            foreach (var m in tipoart)
            {
                listatipoart.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDTipoArticulo.ToString() }); //se modifica en base a c_XXXX en Models/Administracion/..
            }
            ViewBag.tipoarticulo = listatipoart;
            return View();

        }

        [HttpPost]
        public ActionResult ReporteArticuloPorTipoArticulo(ReportePorTipoArticulo modelo, TipoArticuloGetSet ta)
        {
            int idTipoArticulo = ta.IDTipoArticulo;
            try
            {

                ArticuloContext dbc = new ArticuloContext();
                //Articulo ven = dbc.Articulo.Find(m.IDArticulo);
            }
            catch (Exception ERR)
            {

            }

            ReportePorTipoArticulo report = new ReportePorTipoArticulo();
            //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"), DateTime.Parse("2019-07-30"), idcliente);
            byte[] abytes = report.prepararReporte(idTipoArticulo);
            return File(abytes, "application/pdf");
            //return Redirect("index");
        }

        public JsonResult getarticulosblando(string buscar)
        {
            buscar = buscar.Remove(buscar.Length - 1);
            var Articulos = new ArticuloContext().Articulo.Where(s => s.Cref.Contains(buscar) && s.esKit == false).OrderBy(S => S.Cref);

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (Articulo art in Articulos)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.Cref + " " + art.Descripcion;
                elemento.Value = art.IDArticulo.ToString();
                opciones.Add(elemento);
            }

            return Json(opciones, JsonRequestBehavior.AllowGet);
        }



        public ActionResult InsertarComponenteKit(int? idarticulo)
        {

            ViewBag.idarticuloKit = idarticulo;

            //Articulos
            ArticuloContext dbar = new ArticuloContext();
            var datosArticulo = dbar.Articulo.Where(i => i.esKit == false).OrderBy(i => i.Cref).ToList();
            List<SelectListItem> liAC = new List<SelectListItem>();
            liAC.Add(new SelectListItem { Text = "--Selecciona un Articulo--", Value = "0" });
            foreach (var a in datosArticulo)
            {
                liAC.Add(new SelectListItem { Text = a.Cref + " | " + a.Descripcion, Value = a.IDArticulo.ToString() });

            }
            ViewBag.IDArticulo = liAC;
            ViewBag.PresentacionList = getPresentacionPorArticulo(0);





            return View();
        }

        // POST: Inventario/Create
        [HttpPost]

        public ActionResult InsertarComponenteKit(Kit elemento, FormCollection coleccion)
        {

            ArticuloContext dbart = new ArticuloContext();

            int porc = 0;
            int porp = 0;

            try
            {
                string clave = db.Database.SqlQuery<string>("select cref from Articulo where idArticulo=" + elemento.IDArticuloComp + " ").FirstOrDefault();

                db.Database.ExecuteSqlCommand("INSERT INTO Kit([IDArticulo],[IDArticuloComp],[Cantidad],[Clave],[porcantidad],[porporcentaje], idCaracteristica) values ('" + elemento.IDArticulo + "','" + elemento.IDArticuloComp + "','" + elemento.Cantidad + "', '" + clave + "','" + elemento.porcantidad + "','" + elemento.porporcentaje + "'," + elemento.IDCaracteristica + ")");

                return RedirectToAction("Details", "Articulos", new { id = elemento.IDArticulo });
            }
            catch (Exception ex)
            {
                return View("Error", ex);

            }
        }

        public ActionResult MostrarCotizacion(int id)
        {
            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(id);
            XmlDocument documento = new XmlDocument();
            string nombredearchivo = archivocotizacion.Ruta + "\\" + archivocotizacion.NombreArchivo + ".xml";
            documento.Load(nombredearchivo);
            elemento = null;
            XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
            using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                elemento = (ClsCotizador)serializer.Deserialize(reader);
                reader.Close();
            }
            return View(elemento);
        }
        public ActionResult SubirArchivoPres(int? idc, int idarticulo)
        {
            ViewBag.ID = idc;
            ViewBag.idarticulo = idarticulo;
            SubirArchivosModelo modelo = new SubirArchivosModelo();
            Session["IDcaracteristicadesubirarchivopresentacion"] = idc;
            Session["IDarticulodesubirarchivopresentacion"] = idarticulo;
            return PartialView("SubirArchivoPres", new { id = idc, ida = idarticulo });

        }



        [HttpPost]
        public ActionResult SubirArchivoPres(HttpPostedFileBase file, FormCollection collection)
        {

            int caracteristica = int.Parse(collection.Get("IDCaracteristicaDocumento"));
            int idF = int.Parse(Session["IDcaracteristicadesubirarchivopresentacion"].ToString());
            int ida = int.Parse(Session["IDarticulodesubirarchivopresentacion"].ToString());

            SubirArchivosModelo modelo = new SubirArchivosModelo();
            string extension = Path.GetExtension(file.FileName).ToLower();

            if (file != null && file.ContentLength > 0)
            {

                if (extension == ".pdf")
                {
                    string ruta = Server.MapPath("~/ArtPresentacionAdd/");
                    ruta += "Pdf_" + caracteristica + "_" + file.FileName;
                    string cad = "insert into [dbo].[ArtPresentacionAdd]([IDCaracteristica], [IDArticulo], [RutaArchivo], nombreArchivo) values(" + idF + "," + ida + ", '" + ruta + "','" + "Doc_" + caracteristica + "_" + file.FileName + "' )";
                    new ArtPresentacionAddContext().Database.ExecuteSqlCommand(cad);
                    modelo.SubirArchivo(ruta, file);
                    ViewBag.Error = modelo.error;
                    ViewBag.Correcto = modelo.Confirmacion;
                }
                else
                {
                    ViewBag.Mensajeerror1 = "No se pudo subir el archivo";
                }
                if (extension == ".xml")
                {
                    string ruta = Server.MapPath("~/ArtPresentacionAdd/");
                    ruta += "Xml_" + caracteristica + "_" + file.FileName;
                    string cad = "insert into [dbo].[ArtPresentacionAdd] ([IDCaracteristica], [IDArticulo], [RutaArchivo], nombreArchivo) values(" + idF + "," + ida + ",'" + ruta + "','" + "Xml_" + caracteristica + "_" + file.FileName + "' )";
                    new ArtPresentacionAddContext().Database.ExecuteSqlCommand(cad);
                    modelo.SubirArchivo(ruta, file);
                    ViewBag.Error = modelo.error;
                    ViewBag.Correcto = modelo.Confirmacion;
                }
                else
                {
                    ViewBag.Mensajeerror1 = "No se pudo subir el archivo";
                }
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".jfif")
                {
                    string ruta = Server.MapPath("~/ArtPresentacionAdd/");
                    ruta += "Imagen_" + caracteristica + "_" + file.FileName;
                    if ((file != null))
                    {

                        string ext = Path.GetExtension(file.FileName);
                        string newName = Guid.NewGuid().ToString();

                        newName = newName + ext;
                        string path = Server.MapPath("~/ArtPresentacionAdd/");
                        newName = newName.ToUpper();
                        file.SaveAs(path + newName);

                        if (System.IO.File.Exists(ruta))
                        {
                            System.IO.File.Delete(ruta);
                        }

                    }



                    string cad = "insert into [dbo].[ArtPresentacionAdd] ([IDCaracteristica], [IDArticulo], [RutaArchivo], nombreArchivo) values(" + caracteristica + "," + ida + ", '" + ruta + "','" + "Imagen_" + caracteristica + "_" + file.FileName + "' )";
                    new ArtPresentacionAddContext().Database.ExecuteSqlCommand(cad);
                    modelo.SubirArchivo(ruta, file);
                    ViewBag.Error = modelo.error;
                    ViewBag.Correcto = modelo.Confirmacion;
                }
                else
                {
                    ViewBag.Mensajeerror1 = "No se pudo subir el archivo";
                }
            }

            return RedirectToAction("Details", new { id = ida });
        }

        [HttpPost]
        public ActionResult EditarStock(LPresentacion presentacion, FormCollection coleccion)
        {

            int idp = 0;
            string caracteristica = coleccion.Get("IDCaracteristicaP");
            int ida = 0;
            string articulo = coleccion.Get("IDArticuloP");
            decimal stockMin = 0;
            string minimo = coleccion.Get("StockMin");
            decimal stockMax = 0;
            string maximo = coleccion.Get("StockMax");
            int IDAlmacen = 0;
            string almacen = coleccion.Get("IDAlmacen");
            try
            {
                idp = int.Parse(caracteristica);

                ida = int.Parse(articulo);
                stockMin = decimal.Parse(minimo);
                stockMax = decimal.Parse(maximo);
                IDAlmacen = int.Parse(almacen);
            }
            catch (Exception err)
            {

            }
            try
            {
                string cadena = "insert into [StockVSAlmacen] (IDCaracteristica,IDAlmacen, StockMin,StockMax) values(" + idp + "," + IDAlmacen + "," + stockMin + "," + stockMax + ")";
                db.Database.ExecuteSqlCommand(cadena);
                //db.Database.ExecuteSqlCommand("update [dbo].[Caracteristica] set StockMin=" + stockMin + ", [StockMax]=" + stockMax + " where ID=" + idp + "");

            }
            catch (Exception err)
            {

            }

            return RedirectToAction("Details", new { id = ida });
        }


        [HttpPost]
        public JsonResult EdititemStock(int id, decimal StockMin, string StockMax, int IDAlmacen)
        {
            try
            {
                CarritoContext car = new CarritoContext();
                db.Database.ExecuteSqlCommand("update [dbo].[StockVSAlmacen] set [StockMin]=" + StockMin + ", [StockMax]='" + StockMax + "', IDAlmacen=" + IDAlmacen + "  where ID=" + id);


                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();


                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        [HttpPost]
        public JsonResult DeleteitemStock(int id)
        {
            try
            {
                CarritoContext car = new CarritoContext();
                car.Database.ExecuteSqlCommand("delete from StockVSAlmacen where ID=" + id);

                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        public ActionResult DescargarPDFPre(int iddocto, int idart)
        {
            // Obtener contenido del archivo
            ArtPresentacionAddContext dbp = new ArtPresentacionAddContext();
            ArtPresentacionAdd elemento = dbp.ArtPresentacionAdd.Find(iddocto);
            string ruta = elemento.RutaArchivo;
            string extension;
            string rutacarpeta = Server.MapPath("~/ArtPresentacionAdd/" + elemento.nombreArchivo);

            extension = System.IO.Path.GetExtension(elemento.nombreArchivo);
            extension = extension.ToLower();
            string contentType = "";


            if (extension == ".pdf")
            {
                contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            if (extension == ".xml")
            {
                //var stream = new MemoryStream(Encoding.ASCII.GetBytes(elemento.RutaArchivo.ToString()));
                contentType = System.Net.Mime.MediaTypeNames.Text.Xml;
                return File(elemento.RutaArchivo, contentType);


                //return File(rutacarpeta, contentType);
                //return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            if (extension == ".doc" || extension == ".docx")
            {
                string path = ruta;
                System.IO.FileInfo file = new System.IO.FileInfo(path);
                Response.AddHeader("content-disposition", "attachment; filename=" + elemento.nombreArchivo);
                Response.ContentType = "application/msword";//vnd.ms-word.document"; //x-zip-compressed";

                Response.WriteFile(rutacarpeta);
                Response.Flush();
                Response.End();
            }
            if (extension == ".xlsx" || extension == ".xls")
            {

                string direccion = elemento.RutaArchivo;
                System.IO.FileStream fs = null;

                fs = System.IO.File.Open(direccion, System.IO.FileMode.Open);
                byte[] btFile = new byte[fs.Length];
                fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
                fs.Close();

                Response.AddHeader("Content-disposition", "attachment; filename=" + elemento.nombreArchivo);
                Response.ContentType = "application/octet-stream";
                Response.BinaryWrite(btFile);
                Response.Flush();
                Response.End();

            }
            if (extension == ".prn" || extension == ".txt")
            {
                contentType = System.Net.Mime.MediaTypeNames.Text.RichText;
                return File(elemento.RutaArchivo.ToString(), contentType);
            }
            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".jfif")
            {
                contentType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            if (extension == ".gif")
            {
                contentType = System.Net.Mime.MediaTypeNames.Image.Gif;
                return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            return RedirectToAction("Details", new { id = idart });
        }

        public ActionResult EliminarArchivoPres(int iddocto, int idart)
        {
            ArtPresentacionAddContext db = new ArtPresentacionAddContext();
            string cad = "delete from [dbo].[ArtPresentacionAdd] where ID= " + iddocto + "";
            new ArtPresentacionAddContext().Database.ExecuteSqlCommand(cad);
            return RedirectToAction("Details", new { id = idart });
        }


        public ActionResult ConsultaSuaje(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, string eje, string avance, string clave, string th, string familia = "", string material = "", string Estado = "")
        {
            if (sortOrder == string.Empty)
            {
                sortOrder = "Cref";
            }
            if (sortOrder == null)
            {
                sortOrder = "Cref";
            }
            ViewBag.Caracteristica = null;
            ViewBag.Articulo = null;
            ViewBag.avanceseleccionada = avance;
            ViewBag.ejeseleccionada = eje;
            ViewBag.thseleccionada = th;
            ViewBag.claveseleccionada = clave;
            //Filtro familia
            List<Familia> familias = new List<Familia>();
            string cadena2 = "select * from familia where (descripcion like'%suaje%' or descripcion like '%pleca%')  order by descripcion";
            familias = db.Database.SqlQuery<Familia>(cadena2).ToList();
            List<SelectListItem> listafamilia = new List<SelectListItem>();
            listafamilia.Add(new SelectListItem { Text = "--Todas las familias--", Value = "0" });
            foreach (var m in familias)
            {
                listafamilia.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDFamilia.ToString() });
            }
            ViewBag.familia = listafamilia;
            //familia = ViewBag.familiaseleccionada;

            var materialLst = new List<SelectListItem>();
            materialLst.Add(new SelectListItem { Text = "Todos los materiales", Value = "0" });
            materialLst.Add(new SelectListItem { Text = "Arsec", Value = "Arsec" });
            materialLst.Add(new SelectListItem { Text = "BOPP", Value = "BOPP" });
            materialLst.Add(new SelectListItem { Text = "BOPP + Laminado", Value = "BOPP + Laminado" });
            materialLst.Add(new SelectListItem { Text = "Cartulina", Value = "Cartulina" });
            materialLst.Add(new SelectListItem { Text = "Cartulina + Laminado", Value = "Cartulina + Laminado" });
            materialLst.Add(new SelectListItem { Text = "Couche", Value = "Couche" });
            materialLst.Add(new SelectListItem { Text = "Couche+Laminado", Value = "Couche+Laminado" });
            materialLst.Add(new SelectListItem { Text = "Crystal Clear", Value = "Crystal Clear" });
            materialLst.Add(new SelectListItem { Text = "Estate 4", Value = "Estate 4" });
            materialLst.Add(new SelectListItem { Text = "Jac Chromo", Value = "Jac Chromo" });
            materialLst.Add(new SelectListItem { Text = "Kimdura", Value = "Kimdura" });
            materialLst.Add(new SelectListItem { Text = "Nylon + Adhesivo", Value = "Nylon + Adhesivo" });
            materialLst.Add(new SelectListItem { Text = "Nylon", Value = "Nylon" });
            materialLst.Add(new SelectListItem { Text = "Termico Sintético", Value = "Termico Sintetico" });
            materialLst.Add(new SelectListItem { Text = "Thermal Transfer", Value = "Thermal Tranfer" });
            materialLst.Add(new SelectListItem { Text = "Tyvek", Value = "Tyvek" });
            materialLst.Add(new SelectListItem { Text = "Valeron", Value = "Valeron" });
            //ViewData["material"] = materialLst;
            ViewBag.material = new SelectList(materialLst, "Value", "Text");


            var ListaEstado = new List<SelectListItem>();
            ListaEstado.Add(new SelectListItem { Text = "Todos los materiales", Value = "N/A" });
            ListaEstado.Add(new SelectListItem { Text = "Activos", Value = "0" });
            ListaEstado.Add(new SelectListItem { Text = "Obsoletos", Value = "1" });
            ViewBag.Estado = new SelectList(ListaEstado, "Value", "Text");

            //material = 
            ViewBag.materialSeleccionado = material;
            ViewBag.estadoSeleccionado = Estado;
            ViewBag.familiaseleccionada = familia;

            if (Session["IDCaracteristica"] != null)
            {
                string idc = Session["IDCaracteristica"].ToString();
                Caracteristica carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + idc).FirstOrDefault();
                Articulo arti = new ArticuloContext().Articulo.Find(carac.Articulo_IDArticulo);
                ViewBag.Caracteristica = carac;
                ViewBag.Articulo = arti;
            }

            ViewBag.CurrentSort = sortOrder;

            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "ID" : "ID";


            if (searchString == null)
            {
                searchString = currentFilter;
            }


            // Pass filtering string to view in order to maintain filtering when paging


            string cadenaeje = "EJE:" + eje;
            string cadenaavance = "AVANCE:" + avance;
            string cadenath = "TH:" + th;
            string cadenamaterial = "MATERIAL:" + material;




            string ConsultaSql = "select * from Caracteristica as c inner join articulo as a on  a.idarticulo=c.articulo_idarticulo inner join familia as f on f.idfamilia=a.idfamilia";
            string Filtro = string.Empty;
            if (material == "0")
            {
                material = string.Empty;
            }

            ///////////   Clave  //////////
            if (!String.IsNullOrEmpty(clave))
            {
                if (Filtro == string.Empty)
                {

                    Filtro = " where a.cref like '%" + clave + "%'";

                }
                else
                {

                    Filtro += " and a.cref like '%" + clave + "%'";
                }
            }

            if (Estado != "N/A")
            {
                if (Filtro == string.Empty)
                {

                    Filtro = " where (a.obsoleto= '" + Estado + "' and c.obsoleto='" + Estado + "') ";

                }
                else
                {

                    Filtro += " and a.obsoleto = '" + Estado + "' and c.obsoleto='" + Estado + "') ";
                }
            }
            ///////////   Clave   //////////

            string FiltroPres = string.Empty;


            if (!String.IsNullOrEmpty(eje) || !String.IsNullOrEmpty(avance) || !String.IsNullOrEmpty(th) || !String.IsNullOrEmpty(material))
            {
                if (Filtro != string.Empty)
                {
                    if (!String.IsNullOrEmpty(eje))
                    {
                        if (String.IsNullOrEmpty(FiltroPres))
                        {
                            FiltroPres = ("c.presentacion like '%" + cadenaeje + "%'");
                        }
                        else
                        {
                            FiltroPres += (" AND c.presentacion like '%" + cadenaeje + "%'");
                        }
                    }
                    if (!String.IsNullOrEmpty(avance))
                    {
                        if (String.IsNullOrEmpty(FiltroPres))
                        {
                            FiltroPres = (" c.presentacion like '%" + cadenaavance + "%'");
                        }
                        else
                        {
                            FiltroPres += (" AND c.presentacion like '%" + cadenaavance + "%'");
                        }
                    }
                    if (!String.IsNullOrEmpty(th))
                    {
                        if (String.IsNullOrEmpty(FiltroPres))
                        {
                            FiltroPres = (" c.presentacion like '%" + cadenath + "%'");
                        }
                        else
                        {
                            FiltroPres += (" AND c.presentacion like '%" + cadenath + "%'");
                        }
                    }
                    if (!String.IsNullOrEmpty(material))
                    {
                        if (String.IsNullOrEmpty(FiltroPres))
                        {
                            FiltroPres = (" c.presentacion like '%" + cadenamaterial + "%'");
                        }
                        else
                        {
                            FiltroPres += (" AND c.presentacion like '%" + cadenamaterial + "%'");
                        }
                    }

                    if (String.IsNullOrEmpty(familia))
                    {
                        familia = "0";
                    }
                    if (familia != "0")
                    {


                        Filtro += " and (a.idfamilia= " + int.Parse(familia.ToString()) + ")";

                    }



                    Filtro += " and (" + FiltroPres + ")";

                }

                if (Filtro == string.Empty)
                {

                    if (!String.IsNullOrEmpty(eje))
                    {
                        if (String.IsNullOrEmpty(FiltroPres))
                        {
                            FiltroPres = ("c.presentacion like '%" + cadenaeje + "%'");
                        }
                        else
                        {
                            FiltroPres += (" AND c.presentacion like '%" + cadenaeje + "%'");
                        }
                    }
                    if (!String.IsNullOrEmpty(avance))
                    {
                        if (String.IsNullOrEmpty(FiltroPres))
                        {
                            FiltroPres = (" c.presentacion like '%" + cadenaavance + "%'");
                        }
                        else
                        {
                            FiltroPres += (" AND c.presentacion like '%" + cadenaavance + "%'");
                        }
                    }
                    if (!String.IsNullOrEmpty(th))
                    {
                        if (String.IsNullOrEmpty(FiltroPres))
                        {
                            FiltroPres = (" c.presentacion like '%" + cadenath + "%'");
                        }
                        else
                        {
                            FiltroPres += (" AND c.presentacion like '%" + cadenath + "%'");
                        }
                    }
                    if (!String.IsNullOrEmpty(material))
                    {
                        if (String.IsNullOrEmpty(FiltroPres))
                        {
                            FiltroPres = (" c.presentacion like '%" + cadenamaterial + "%'");
                        }
                        else
                        {
                            FiltroPres += (" AND c.presentacion like '%" + cadenamaterial + "%'");
                        }
                    }
                    Filtro = " where (" + FiltroPres + ")";

                }

            }


            ///// Familia///// 
            if (String.IsNullOrEmpty(familia))
            {
                familia = "0";
            }
            if (familia == "0")
            {

                if (Filtro == string.Empty)
                {

                    Filtro = " where (f.descripcion like'%suaje%' or f.descripcion like '%pleca%') ";
                }
                else
                {
                    Filtro += " and  (f.descripcion like'%suaje%' or f.descripcion like '%pleca%') ";

                }

            }
            else
            {
                if (Filtro == string.Empty)
                {

                    Filtro = " where (a.idfamilia= " + int.Parse(familia.ToString()) + ")";
                }
                else
                {
                    Filtro += " and (a.idfamilia= " + int.Parse(familia.ToString()) + ") ";

                }
            }
            ////fin familia  ////
            ////////


            // Pass filtering string to view in order to maintain filtering when paging
            //ViewBag.eje = eje;
            //ViewBag.avance = avance;
            //ViewBag.clave = clave;

            string orden = sortOrder;

            //Ordenacion

            switch (sortOrder)
            {
                case "Cref":
                    if (!string.IsNullOrEmpty(clave))
                    {
                        orden = "ORDER BY convert (int, substring(REPLACE(LTRIM(RTRIM(cref)), '" + clave + "', ''), patindex('%[^0]%', REPLACE(LTRIM(RTRIM(cref)), '" + clave + "', '')), 10))";
                    }
                    else
                    {
                        orden = " order by cref";
                    }

                    break;
            }

            ArticuloContext cc = new ArticuloContext();

            string cadenaSQl = ConsultaSql + " " + Filtro + " " + orden;
            var elementos1 = new List<Caracteristica>();
            try
            {
                elementos1 = cc.Database.SqlQuery<Caracteristica>(cadenaSQl).ToList();
            }
            catch (Exception err)
            {
                orden = " order by cref";
                cadenaSQl = ConsultaSql + " " + Filtro + " " + orden;

                elementos1 = cc.Database.SqlQuery<Caracteristica>(cadenaSQl).ToList();
            }
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = cc.Caracteristica.OrderBy(e => e.ID).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25", Selected = true  },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = elementos1.Count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos1.ToPagedList(pageNumber, pageSize));
        }

        public void ExcelSuajes()
        {
            //Listado de datos

            List<Articulo> familias = new List<Articulo>();
            string cadena2 = "select a.* from articulo as a inner join  familia  as f on f.idfamilia=a.idfamilia where (f.descripcion like'%suaje%' or f.descripcion like '%pleca%')  order by a.idarticulo";
            familias = db.Database.SqlQuery<Articulo>(cadena2).ToList();


            //var bancos = dba.c_Bancos.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Suajes / Plecas");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:M1"].Style.Font.Size = 20;
            Sheet.Cells["A1:M1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:M1"].Style.Font.Bold = true;
            Sheet.Cells["A1:M1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado general de Suajes y Plecas");
            Sheet.Cells["A1:M1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;





            foreach (var item in familias)
            {
                //Sheet.Cells["A"+row+":U"+row+""].Style.Font.Size = 20;
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Name = "Calibri";
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Bold = true;
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A" + row + ":F" + row + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Name = "Calibri";
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Size = 12;
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Bold = true;

                Sheet.Cells["A" + row + ""].RichText.Add("CLAVE");
                Sheet.Cells["B" + row + ""].RichText.Add("DESCRIPCIÓN");
                Sheet.Cells["C" + row + ""].RichText.Add("FAMILIA");
                Sheet.Cells["D" + row + ""].RichText.Add("ESTADO");
                Sheet.Cells["E" + row + ""].RichText.Add("UNIDAD");
                Sheet.Cells["F" + row + ""].RichText.Add("MONEDA");
                row++;
                string familia = new Models.Comercial.FamiliaContext().Familias.Find(item.IDFamilia).Descripcion;
                string unidad = new c_ClaveUnidadContext().c_ClaveUnidades.Find(item.IDClaveUnidad).ClaveUnidad;
                string moneda = new c_MonedaContext().c_Monedas.Find(item.IDMoneda).ClaveMoneda;
                string estado = "ACTIVO";
                if (item.obsoleto)
                {
                    estado = "OBSOLETO";
                }
                Sheet.Cells[string.Format("A{0}", row)].Value = item.Cref;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("C{0}", row)].Value = familia;

                Sheet.Cells[string.Format("D{0}", row)].Value = estado;
                Sheet.Cells[string.Format("E{0}", row)].Value = unidad;
                Sheet.Cells[string.Format("F{0}", row)].Value = moneda;

                row++;

                List<Caracteristica> caracteristicas = new List<Caracteristica>();
                string cadenaCara = "select * from Caracteristica where articulo_idarticulo=" + item.IDArticulo;
                caracteristicas = db.Database.SqlQuery<Caracteristica>(cadenaCara).ToList();


                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Name = "Calibri";
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Size = 12;
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Bold = true;

                Sheet.Cells["A" + row + ""].RichText.Add("NP");
                Sheet.Cells["B" + row + ""].RichText.Add("PRESENTACIÓN");
                Sheet.Cells["C" + row + ""].RichText.Add("ESTADO");
                row++;
                foreach (Caracteristica cara in caracteristicas)
                {
                    string estadoCara = "ACTIVA";
                    if (cara.obsoleto)
                    {
                        estadoCara = "OBSOLETA";
                    }

                    Sheet.Cells[string.Format("A{0}", row)].Value = cara.IDPresentacion;
                    Sheet.Cells[string.Format("B{0}", row)].Value = cara.Presentacion;
                    Sheet.Cells[string.Format("C{0}", row)].Value = estadoCara;

                    row++;
                }

                row++;

            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelSuajes.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();

        }


        public ActionResult ConsultaRodillo(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, string eje, string maquina, string clave, string diente, string familia = "")
        {
            if (sortOrder == string.Empty)
            {
                sortOrder = "Cref";
            }
            if (sortOrder == null)
            {
                sortOrder = "Cref";
            }
            ViewBag.Caracteristica = null;
            ViewBag.Articulo = null;
            ViewBag.avanceseleccionada = maquina;
            ViewBag.ejeseleccionada = eje;
            ViewBag.thseleccionada = diente;
            ViewBag.claveseleccionada = clave;
            //Filtro familia
            List<Familia> familias = new List<Familia>();
            string cadena2 = "select * from familia where (descripcion like'%Rodillo%' or descripcion like '%Rodillos%')  order by descripcion";
            familias = db.Database.SqlQuery<Familia>(cadena2).ToList();
            List<SelectListItem> listafamilia = new List<SelectListItem>();
            listafamilia.Add(new SelectListItem { Text = "--Todas los Rodillos--", Value = "0" });
            foreach (var m in familias)
            {
                listafamilia.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDFamilia.ToString() });
            }
            ViewBag.familia = listafamilia;
            //familia = ViewBag.familiaseleccionada;

            ViewBag.familiaseleccionada = familia;


            ViewBag.CurrentSort = sortOrder;

            if (searchString == null)
            {
                searchString = currentFilter;
            }

            string ConsultaSql = "select * from Caracteristica as c inner join articulo as a on  a.idarticulo=c.articulo_idarticulo inner join familia as f on f.idfamilia=a.idfamilia where (f.Descripcion like '%RODILLOS%' or f.Descripcion like '%RODILLO%')";
            string Filtro = string.Empty;

            if (!String.IsNullOrEmpty(clave))
            {
                if (Filtro == string.Empty)
                {

                    Filtro = " and a.cref like '%" + clave + "%'";

                }
                else
                {

                    Filtro += " where a.cref like '%" + clave + "%'";
                }
            }

            string cadenaeje = "Eje maquina:" + eje;
            string cadenamaquina = "Maquina:" + maquina;
            string cadenadiente = "Numero de dientes:" + diente;


            if (!String.IsNullOrEmpty(eje))
            {
                if (Filtro == string.Empty)
                {

                    Filtro = " and c.presentacion like '%" + cadenaeje + "%'";

                }
                else
                {

                    Filtro += " and c.presentacion like '%" + cadenaeje + "%'";
                }
            }

            if (!String.IsNullOrEmpty(maquina))
            {
                if (Filtro == string.Empty)
                {

                    Filtro = " and c.presentacion like '%" + cadenamaquina + "%'";

                }
                else
                {

                    Filtro += " and c.presentacion like '%" + cadenamaquina + "%'";
                }
            }

            if (!String.IsNullOrEmpty(diente))
            {
                if (Filtro == string.Empty)
                {

                    Filtro = " and c.presentacion like '%" + cadenadiente + "%'";

                }
                else
                {

                    Filtro += " and c.presentacion like '%" + cadenadiente + "%'";
                }
            }

            //string orden = sortOrder;
            ArticuloContext cc = new ArticuloContext();

            string cadenaSQl = ConsultaSql + " " + Filtro;
            var elementos1 = cc.Database.SqlQuery<Caracteristica>(cadenaSQl).ToList();
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = cc.Caracteristica.OrderBy(e => e.ID).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true   },
                new SelectListItem { Text = "25", Value = "25"},
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = elementos1.Count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos1.ToPagedList(pageNumber, pageSize));
        }







        //////////////////////Reporte Excel/////////////////////////////////////

        public void ExcelRodillos()
        {
            //Listado de datos

            List<Articulo> familias = new List<Articulo>();
            string cadena2 = "select a.* from articulo as a inner join  familia  as f on f.idfamilia=a.idfamilia where (f.descripcion like'%Rodilllos%' or f.descripcion like '%Rodillo%')  order by a.idarticulo";
            familias = db.Database.SqlQuery<Articulo>(cadena2).ToList();


            //var bancos = dba.c_Bancos.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Rodillos");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:M1"].Style.Font.Size = 20;
            Sheet.Cells["A1:M1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:M1"].Style.Font.Bold = true;
            Sheet.Cells["A1:M1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Rodillos");
            Sheet.Cells["A1:M1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;





            foreach (var item in familias)
            {
                //Sheet.Cells["A"+row+":U"+row+""].Style.Font.Size = 20;
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Name = "Calibri";
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Bold = true;
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A" + row + ":F" + row + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Name = "Calibri";
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Size = 12;
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Bold = true;

                Sheet.Cells["A" + row + ""].RichText.Add("CLAVE");
                Sheet.Cells["B" + row + ""].RichText.Add("DESCRIPCIÓN");
                Sheet.Cells["C" + row + ""].RichText.Add("FAMILIA");
                Sheet.Cells["D" + row + ""].RichText.Add("ESTADO");
                Sheet.Cells["E" + row + ""].RichText.Add("UNIDAD");
                Sheet.Cells["F" + row + ""].RichText.Add("MONEDA");
                row++;
                string familia = new Models.Comercial.FamiliaContext().Familias.Find(item.IDFamilia).Descripcion;
                string unidad = new c_ClaveUnidadContext().c_ClaveUnidades.Find(item.IDClaveUnidad).ClaveUnidad;
                string moneda = new c_MonedaContext().c_Monedas.Find(item.IDMoneda).ClaveMoneda;
                string estado = "ACTIVO";
                if (item.obsoleto)
                {
                    estado = "OBSOLETO";
                }
                Sheet.Cells[string.Format("A{0}", row)].Value = item.Cref;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("C{0}", row)].Value = familia;

                Sheet.Cells[string.Format("D{0}", row)].Value = estado;
                Sheet.Cells[string.Format("E{0}", row)].Value = unidad;
                Sheet.Cells[string.Format("F{0}", row)].Value = moneda;

                row++;

                List<Caracteristica> caracteristicas = new List<Caracteristica>();
                string cadenaCara = "select * from Caracteristica where articulo_idarticulo=" + item.IDArticulo;
                caracteristicas = db.Database.SqlQuery<Caracteristica>(cadenaCara).ToList();


                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Name = "Calibri";
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Size = 12;
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                Sheet.Cells["A" + row + ":F" + row + ""].Style.Font.Bold = true;

                Sheet.Cells["A" + row + ""].RichText.Add("NP");
                Sheet.Cells["B" + row + ""].RichText.Add("PRESENTACIÓN");
                Sheet.Cells["C" + row + ""].RichText.Add("ESTADO");
                row++;
                foreach (Caracteristica cara in caracteristicas)
                {
                    string estadoCara = "ACTIVA";
                    if (cara.obsoleto)
                    {
                        estadoCara = "OBSOLETA";
                    }

                    Sheet.Cells[string.Format("A{0}", row)].Value = cara.IDPresentacion;
                    Sheet.Cells[string.Format("B{0}", row)].Value = cara.Presentacion;
                    Sheet.Cells[string.Format("C{0}", row)].Value = estadoCara;

                    row++;
                }

                row++;

            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelRodillos.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();

        }

        public ActionResult MaterialCostos(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {


            if (sortOrder == string.Empty)
            {
                sortOrder = "Cref";
            }

            if (sortOrder == null)
            {
                sortOrder = "Cref";
            }

            ArticuloContext db = new ArticuloContext();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "Cref" : "Cref";
            ViewBag.CSortParm = String.IsNullOrEmpty(sortOrder) ? "Familia" : "Familia";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            ViewBag.CurrentFilter = searchString;



            var articulos = db.Articulo.Where(x => x.Cref.StartsWith(searchString)).ToList();

            ViewBag.Clave = searchString;
            //Ordenacion

            switch (sortOrder)
            {
                case "Cref":
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        articulos.OrderBy(z => z.Cref);
                    }
                    else
                    {
                        articulos.OrderBy(z => z.Cref);

                    }

                    break;

                default:
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        articulos.OrderBy(z => z.Cref);
                    }
                    else
                    {
                        articulos.OrderBy(z => z.Cref);
                    }
                    break;



            }



            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = articulos.Count;

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50", Selected = true },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(articulos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }

        [HttpPost]
        public ActionResult MaterialCostos(string Clave, decimal Costo, FormCollection coleccion)
        {
            ArticuloContext db = new ArticuloContext();
            List<Articulo> articulos = new List<Articulo>();
            if (Clave != "" && Clave != null && Costo > 0)
            {
                int longclave = Clave.Length;
                articulos = db.Articulo.Where(x => x.Cref.StartsWith(Clave)).ToList();
                foreach (Articulo articulo in articulos)
                {

                    db.Database.ExecuteSqlCommand("delete from  MatrizCosto where idArticulo=" + articulo.IDArticulo);
                    //MatrizCosto matriz = new MatrizCosto();
                    //matriz.RangInf = 0M;
                    //matriz.RangSup = 999999M;
                    //matriz.Precio = Costo;
                    //matriz.IDArticulo = articulo.IDArticulo;
                    //db.MatrizCostos.Add(matriz);
                    //db.SaveChanges();
                    db.Database.ExecuteSqlCommand("insert into MatrizCosto (RangInf, RangSup,Precio,IDArticulo) values (0,999999," + Costo + "," + articulo.IDArticulo + ");");


                }
            }


            var elementos = articulos;

            int count = elementos.Count;

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25" , Selected = true},
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            int pageNumber = 1;
            int pageSize = 25;
            ViewBag.psize = 25;

            return View(elementos.ToPagedList(pageNumber, pageSize));

        }
    }

}


