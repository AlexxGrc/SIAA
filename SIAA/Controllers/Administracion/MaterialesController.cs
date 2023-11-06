using PagedList;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;


namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador,Facturacion,Gerencia,Sistemas,Compras,GerenteVentas")]
    public class MaterialesController : Controller
    {

      
        // GET: Materiales
        private MaterialesContext db = new MaterialesContext();
        public ActionResult Index(string sortOrder, string Descripcion, string currentFilter, string searchString, int? page, int? PageSize, string Obs, string Fechainicio, string Fechafinal)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "Clave" : "";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.MonedaSortParm = String.IsNullOrEmpty(sortOrder) ? "Moneda" : "Moneda";

            var Obsoletos = new List<SelectListItem>();

            //Obsoletos
              Obsoletos.Add(new SelectListItem { Text = "Activo", Value = "si" });
              Obsoletos.Add(new SelectListItem { Text = "Obsoleto", Value = "no" });

            ViewData["Obsoletos"] = Obsoletos;
            
            ViewBag.Obsoletos= new SelectList(Obsoletos, "Value", "Text");

            string ConsultaSql = "select * from Materiales ";
            string Filtro = "";

            ViewBag.Descripcion = new MaterialesRepository().GetMaterialesbyDescripcion();

            ViewBag.Descripcionseleccionado = Descripcion;

           

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;

            //Paginación
            if (searchString != null)
            {

                ViewBag.searchString = searchString;
            }
            else
            {
                searchString = "";
            }
            

          
           

            var elementos = from s in db.Materiales

                            select s;

           
                if (Obs != "Todas" && Obs != null)
            {
                if (Obs.ToLower() == "si")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = " where Obsoleto='1' ";
                          //  elementos = elementos.Where(s => s.Clave.Contains(searchString) || s.Descripcion.Contains(searchString) || s.Moneda.Contains(searchString) || s.Obsoleto == true);
                        }
                    
                }
                if (Obs.ToLower() == "no")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = " where Obsoleto='0' ";
                            //elementos = elementos.Where(s => s.Clave.Contains(searchString) || s.Descripcion.Contains(searchString) || s.Moneda.Contains(searchString) || s.Obsoleto == false);
                        }
                    
                }
                              

                //elementos = elementos.Where(s => s.Clave.Contains(searchString) || s.Descripcion.Contains(searchString) || s.Moneda.Contains(searchString) || s.Obsoleto == false);

            }

            if (!String.IsNullOrEmpty(searchString))
            {

                if (Filtro == string.Empty)
                {
                    Filtro = " where Descripcion like '%" + searchString + "%' or clave like '%" + searchString + "%'";
                }
                else
                {
                    Filtro += " and  like '%" + searchString + "%' or clave like '%" + searchString + "%'";
                }

            }

            if (!String.IsNullOrEmpty(Fechainicio) && String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
            {
                if (Filtro == string.Empty)
                {
                    Filtro = " where  PrecioAct BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechainicio + " 23:59:59.999' ";
                }
                else
                {
                    Filtro += " and PrecioAct BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechainicio + " 23:59:59.999'";
                }
            }


            if (!String.IsNullOrEmpty(Fechainicio) && !String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where  PrecioAct BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999' ";
                }
                else
                {
                    Filtro += " and PrecioAct BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999'";
                }

            }


            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;
            
            switch (sortOrder)
            {
                case "Clave":
                    elementos = elementos.OrderBy(s => s.Clave);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                case "Moneda":
                    elementos = elementos.OrderBy(s => s.Moneda);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ID);
                    break;
            }




            string cadenaSQl = ConsultaSql + " " + Filtro + " order by descripcion " ;
            var elementos1 = db.Database.SqlQuery<Materiales>(cadenaSQl).ToList();


            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Materiales.OrderBy(e => e.ID).Count(); // Total number of elements

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
            return View(elementos1.ToPagedList(pageNumber, pageSize));
            //Paginación
            //return View();
        }
        private ActionResult NotFound()
        {
            throw new NotImplementedException();
        }
        // GET: Materiales/Details/5
        public ActionResult Details(int? id, int page = 1, int pagesize = 10, string searchString = "", string Descripcion = "", string Fechainicio = "", string Fechafinal = "")
        {
            var elemento = db.Materiales.Single(m => m.ID == id);
            if (elemento == null)
            {
                return NotFound();
            }
            ViewBag.page = page;
            ViewBag.pagesize = pagesize;
            ViewBag.searchString = searchString;
            ViewBag.Descripcionseleccionado = Descripcion;
            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;
            return View(elemento);
        }
        // POST: Cotizaciones/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int id, FormCollection collection, int page = 1, int pagesize = 10, string searchString = "", string Descripcion = "", string Fechainicio = "", string Fechafinal = "")
        {
            var elemento = db.Materiales.Single(m => m.ID == id);
            return View(elemento);
        }

        // GET: Materiales/Create
        public ActionResult Create(int page = 1, int pagesize = 10, string searchString = "", string Descripcion = "", string Fechainicio = "", string Fechafinal = "")
        {

            List<Familia> ven = new List<Familia>();
            string cadena = "select * from Familia order by Descripcion";
            ven = db.Database.SqlQuery<Familia>(cadena).ToList();
            List<SelectListItem> listaven = new List<SelectListItem>();
            listaven.Add(new SelectListItem { Text = "--Selecciona Familia--", Value = "0" });

            foreach (var m in ven)
            {
                listaven.Add(new SelectListItem { Text = m.Descripcion, Value = m.CCodFam });
            }
            ViewBag.IDFamilia = listaven;
            //ViewBag.IDFamilia = new FamiliaRepository().GetFamilias();



            ViewBag.page = page;
            ViewBag.pagesize = pagesize;
            ViewBag.searchString = searchString;
            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;
            ViewBag.Descripcionseleccionado = Descripcion;

            return View();
        }

        // POST: Materiales/Create
        [HttpPost]
        public ActionResult Create(Materiales materiales, FormCollection collection, int page = 1, int pagesize = 10, string searchString = "", string Descripcion = "", string Fechainicio = "", string Fechafinal = "")
        {

            materiales.Fam = collection.Get("IDFamilia");
            try
            {
                // TODO: Add insert logic here
                //Familia newArt = new Familia();
                var db = new MaterialesContext();
                //newArt.CCodFam = materiales.Fam;

                //db.Materiales.Add(newArt);
                db.Materiales.Add(materiales);
                db.SaveChanges();
                //db.Database.ExecuteSqlCommand("update");
                return RedirectToAction("Index", new
                {
                    page = page,
                    PageSize = pagesize,
                    searchString = searchString,
                    Descripcionseleccionado = Descripcion,
                    fecini = Fechainicio,
                    fecfin = Fechafinal
                });
            }
            catch
            {
                return View();
            }

        
           
        }

        // GET: Materiales/Edit/5
        public ActionResult Edit(int id, int page = 1, int pagesize = 10, string searchString = "", string Descripcion="", string Fechainicio = "", string Fechafinal = "")
        {
            var elemento = db.Materiales.Single(m => m.ID == id);

            List<Familia> ven = new List<Familia>();
            string cadena = "select * from Familia order by Descripcion";
            ven = db.Database.SqlQuery<Familia>(cadena).ToList();
            List<SelectListItem> listaven = new List<SelectListItem>();
            listaven.Add(new SelectListItem { Text = "--Selecciona Familia--", Value = "0" });

            foreach (var m in ven)
            {
                if(elemento.Fam== m.CCodFam)
                {
                    listaven.Add(new SelectListItem { Text = m.Descripcion, Value = m.CCodFam , Selected= true});
                }
                else
                {
                    listaven.Add(new SelectListItem { Text = m.Descripcion, Value = m.CCodFam });
                }
                
            }

            ViewBag.IDFamilia = new SelectList(new FamiliaContext().Familias, "CCodFam", "Descripcion", elemento.Fam);

            //ViewBag.IDFamilia = listaven;



          
            ViewBag.page = page;
            ViewBag.pagesize = pagesize;
            ViewBag.searchString = searchString;
            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;
            ViewBag.Descripcionseleccionado = Descripcion;

            return View(elemento);
        }

        // POST: Materiales/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Materiales materiales,FormCollection collection, int page = 1, int pagesize = 10, string searchString = "", string Descripcion="",  string Fechainicio = "", string Fechafinal = "")
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.Materiales.Single(m => m.ID == id);
                elemento.Fam = collection.Get("IDFamilia");
                if (TryUpdateModel(materiales))
                {
                    //elemento.Precio = materiales.Precio;
                    //db.SaveChanges();
                    string actualizar = "UPDATE [dbo].[Materiales] SET [CLAVE] ='"+materiales.Clave+"' ,[Descripcion] ='"+materiales.Descripcion+"' " +
                        ",[Fam] ='"+ elemento.Fam + "' ,[Ancho] ='"+materiales.Ancho+"' ,[Largo] ='"+materiales.Largo+ "' ,[Adhesivo] ='"+materiales.Adhesivo+"'," +
                        "[Moneda] ='"+materiales.Moneda+ "' ,[Precio] ='"+materiales.Precio+ "' ,[Completo] = '"+materiales.Completo+"'," +
                        "[Calibre] = '"+materiales.Calibre+ "',[Solicitarprecio] ='"+materiales.Solicitarprecio+ "' ,[CalibreEsp] ='"+materiales.CalibreEsp+"'," +
                        "[Tcompra] = '"+materiales.Tcompra+ "',[PrecioAct] ='"+materiales.PrecioAct+ "' ,[Proveedor] ='"+materiales.Proveedor+"' ," +
                        "[Respaldo] = '"+materiales.Respaldo+ "',[Gramaje] ='"+materiales.Gramaje+ "' ,[ClaveEt] = '"+materiales.ClaveEt+"'," +
                        "[Plazo] = '"+materiales.Plazo+ "',[Condiciones] ='' ,[Obsoleto] = '"+materiales.Obsoleto+ "' WHERE id=" + id;

                        db.Database.ExecuteSqlCommand(actualizar);
                    return RedirectToAction("Index", new { page = page, PageSize = pagesize, searchString = searchString, Descripcionseleccionado = Descripcion, fecini = Fechainicio, fecfin = Fechafinal
                });
                }
                return View(materiales);
            }
            catch
            {
                return View();
            }
        }

        // GET: Materiales/Delete/5
        public ActionResult Obsoleto(int id, int page = 1, int pagesize = 10, string searchString = "")
        {

            ViewBag.page = page;
            ViewBag.pagesize = pagesize;
            ViewBag.searchString = searchString;
            return View();
        }

        // POST: Materiales/Delete/5
        [HttpPost]
        public ActionResult Obsoleto(int id, FormCollection collection, int page = 1, int pagesize = 10, string searchString = "")
        {
            try
            {
                // TODO: Add delete logic here
                string cadena = "update[dbo].[Materiales] set obsoleto = 'False' where ID = "+ id +"";
                db.Database.ExecuteSqlCommand(cadena);
                return RedirectToAction("Index", new { page = page, PageSize = pagesize, searchString = searchString });
            }
            catch
            {
                return View();
            }
        }
    }
}
