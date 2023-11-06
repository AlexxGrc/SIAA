using PagedList;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Comercial
{
  
    public class KitController : Controller
    {
        private ArticuloContext db = new ArticuloContext();
        private KitContext dbkit = new KitContext();
        // GET: Kit
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
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
            //Paginación
            var elementos = from s in db.Articulo where s.esKit == true
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Cref.Contains(searchString) || s.Descripcion.Contains(searchString) || s.Familia.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                case "Cref":
                    elementos = elementos.OrderBy(s => s.Cref);
                    break;
                case "Familia":
                    elementos = elementos.OrderBy(s => s.Familia.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.IDArticulo);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Articulo.OrderBy(e => e.IDArticulo).Count(); // Total number of elements

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
    
        public ActionResult AgregaKit(string busqueda,string searchString,int? id,int? idarticuloc)
        {
            ArticuloContext articulo = new ArticuloContext();
            //,int? IDArticulo, int? IDArticuloComp, decimal? cantidad, string tipo,int? inserta
            Articulo ar = articulo.Articulo.Find(id);
            ViewBag.articulo = ar.Descripcion;

            ViewBag.id = id;
         
            ViewBag.elementos = null;
            ViewBag.form = null;
            if (!string.IsNullOrEmpty(searchString))
            {
                //ClsDatoEntero tipoarticulo = db.Database.SqlQuery<ClsDatoEntero>("select IDTipoArticulo as Dato from TipoArticulo where Descripcion='Articulo'").ToList()[0];
                //ClsDatoEntero tipoinsumo = db.Database.SqlQuery<ClsDatoEntero>("select IDTipoArticulo as Dato from TipoArticulo where Descripcion='Insumo'").ToList()[0];

                var elementos = from s in db.Articulo
                                where (s.Cref.Contains(searchString) || s.Descripcion.Contains(searchString) || s.Familia.Descripcion.Contains(searchString)) && (s.esKit==false)
                                select s;
            //Busqueda
           // elementos = elementos.Where(s => s.Cref.Contains(searchString) || s.Descripcion.Contains(searchString) || s.Familia.Descripcion.Contains(searchString));

                if (idarticuloc != null)
                {
                   

                   // List<SelectListItem> art = new List<SelectListItem>();
                   // Articulo ar= articulo.Articulo.Find(id);
                   //art.Add(new SelectListItem { Text = ar.Descripcion, Value = ar.IDArticulo.ToString() });
                   // ViewBag.articulo = art;

                    List<SelectListItem> artc = new List<SelectListItem>();
                    Articulo arc = articulo.Articulo.Find(idarticuloc);
                    artc.Add(new SelectListItem { Text = arc.Descripcion, Value = arc.IDArticulo.ToString() });
                    ViewBag.articuloc = artc;

                    //ViewBag.IDArticulo = new SelectList(articulo.Articulo.Where(s => s.IDArticulo.Equals(id)), "IDArticulo", "Descripcion");
                    //ViewBag.IDArticuloComp = new SelectList(articulo.Articulo.Where(s => s.IDArticulo.Equals(idarticuloc)), "IDArticulo", "Descripcion");

                    ViewBag.form = 1;
                }
                else
                {
                ViewBag.form = null;
                }
             


                List<VKit> datosb = db.Database.SqlQuery<VKit>("select Kit.IDArticulo,(select nameFoto from Articulo where IDArticulo=Kit.IDArticuloComp) as Imagen,Kit.IDKit,(select Descripcion from Articulo where IDArticulo=Kit.IDArticulo) as Articulo, Articulo.Descripcion, Kit.Cantidad, Kit.porcantidad, Kit.porporcentaje from Kit inner join Articulo on Articulo.IDArticulo = Kit.IDArticuloComp  where Kit.IDArticulo='" + id + "'").ToList();
                ViewBag.datos = datosb;

                ClsDatoEntero kitb = db.Database.SqlQuery<ClsDatoEntero>("select count(IDKit) as Dato from Kit where IDArticulo='" + id + "'").ToList()[0];
                if (kitb.Dato == 0)
                {
                    ViewBag.datos = null;
                }
                   ViewBag.elementos = 1;
                return View(elementos);
            }
            
            List<VKit> datos = db.Database.SqlQuery<VKit>("select Kit.IDArticulo,(select nameFoto from Articulo where IDArticulo=Kit.IDArticuloComp) as Imagen,Kit.IDKit,(select Descripcion from Articulo where IDArticulo=Kit.IDArticulo) as Articulo, Articulo.Descripcion, Kit.Cantidad, Kit.porcantidad, Kit.porporcentaje from Kit inner join Articulo on Articulo.IDArticulo = Kit.IDArticuloComp where Kit.IDArticulo='" + id + "'").ToList();
            ViewBag.datos = datos;

            ClsDatoEntero kit = db.Database.SqlQuery<ClsDatoEntero>("select count(IDKit) as Dato from Kit where IDArticulo='" + id + "'").ToList()[0];
            if (kit.Dato == 0)
            {
                ViewBag.datos = null;
            }
            return View();
        }



        [HttpPost]
        public ActionResult Insertar(int IDArticuloComp, int id, decimal cantidad, string tipo)
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
                if (cantidad > 0 )
                {
                    db.Database.ExecuteSqlCommand("INSERT INTO Kit([IDArticulo],[IDArticuloComp],[Cantidad],[Clave],[porcantidad],[porporcentaje]) values ('" + id + "','" + IDArticuloComp + "','" + cantidad + "', 'null','" + porc + "','" + porp + "')");
                }
            }
            catch(Exception err)
            {
                string mensaje = err.Message;
            }
            ViewBag.elementos = null;
            ViewBag.form = null;

            List<VKit> datosb = db.Database.SqlQuery<VKit>("select Kit.IDKit,(select Descripcion from Articulo where IDArticulo=Kit.IDArticulo) as Articulo, Articulo.Descripcion, Kit.Cantidad, Kit.porcantidad, Kit.porporcentaje from Kit inner join Articulo on Articulo.IDArticulo = Kit.IDArticuloComp  where Kit.IDArticulo='" + id + "'").ToList();
            ViewBag.datos = datosb;



            //        return Json(true);
            //    }
            //        catch (Exception err)
            //        {
            //            return Json(500, err.Message);
            //}

           //return View("AgregaKit"); 
            return RedirectToAction("AgregaKit", new { id = id });
            //return RedirectToAction("AgregaKit",IDArticulo);
        }


        [HttpPost]
        public JsonResult Edititem(int id, decimal cantidad)
        {
            try
            {
                db.Database.ExecuteSqlCommand("update [dbo].[Kit] set [Cantidad]=" + cantidad + " where IDKit=" + id);
                
             
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        [HttpPost]
        public JsonResult Deleteitem(int id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from [dbo].[Kit] where [kit]='" + id + "'");

           
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

    }
}