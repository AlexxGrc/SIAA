using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Comercial;
using PagedList;
using SIAAPI.Models.Inventarios;

namespace SIAAPI.Controllers.Inventarios
{
    public class FamAlmController : Controller
    {
       
        private FamAlmContext db = new FamAlmContext();
        private VFamAlmContext dbv = new VFamAlmContext();

        // GET: FamAlm
        [Authorize(Roles = "Administrador")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            string ConsultaSql = "select * from VfamAlm";
            string cadenaSQl = string.Empty;
            string Orden = "order by Familia";
            string Filtro = string.Empty;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.FamSortParm = String.IsNullOrEmpty(sortOrder) ? "Familia" : "";
            ViewBag.AlmSortParm = String.IsNullOrEmpty(sortOrder) ? "Almacen" : "Almacen";

            //Buscar 
            if (!String.IsNullOrEmpty(searchString))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where familia like '%" + searchString + "%' or almacen like '%" + searchString + "%' or CCodFam like '%" + searchString + "%'";
                }
                else
                {
                    Filtro += " and familia like '%" + searchString + "%' or almacen like '%" + searchString + "%' or CCodFam like '%" + searchString + "%'";
                }

            }
            //ordenar
            switch (sortOrder)
            {
                case "Familia":
                    Orden = " order by  Familia desc ";
                    break;
                case "Almacen":
                    Orden = " order by  Almacen, Familia desc ";
                    break;

                default:
                    Orden = " order by Familia ";
                    break;
            }

            ViewBag.CurrentFilter = searchString;
            //Paginación
            cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;
            var elementos = dbv.Database.SqlQuery<VFamAlm>(cadenaSQl).ToList();

            //Paginación
            int count = dbv.VFamAlms.OrderBy(e => e.ID).Count(); // Total number of elements

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

        // GET: FamAlm/Details/5
        public ActionResult Details(int? id)
        {
            var elemento = dbv.Database.SqlQuery<VFamAlm>("select * from VfamAlm where  ID= " + id + "").ToList().FirstOrDefault();

            if (elemento == null)
            {
                return RedirectToAction("Index");
            }
            return View(elemento);
        }

        // GET: FamAlm/Create
        public ActionResult Create()
        {
            FamiliaContext dbf = new FamiliaContext();
            AlmacenContext dba = new AlmacenContext();
            ViewBag.IDFamilia = new SelectList(dbf.Familias, "IDFamilia", "Descripcion");
            ViewBag.IDAlmacen = new SelectList(dba.Almacenes, "IDAlmacen", "Descripcion");

            return View();
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(FamAlm famAlm)
        {
            try
            {
                string cadena = "Insert into FamAlm(IDFAmilia, IDAlmacen) Values(" + famAlm.IDFamilia + ", " + famAlm.IDAlmacen + ")";
                db.Database.ExecuteSqlCommand(cadena);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(famAlm);
            }


        }

        // GET: FamAlm/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var elementos = db.Database.SqlQuery<FamAlm>("select * from famAlm where ID= " + id + "").ToList()[0];
            FamiliaContext dbf = new FamiliaContext();
            AlmacenContext dba = new AlmacenContext();
            ViewBag.IDFamilia = new SelectList(dbf.Familias, "IDFamilia", "Descripcion", elementos.IDFamilia);
            ViewBag.IDAlmacen = new SelectList(dba.Almacenes, "IDAlmacen", "Descripcion", elementos.IDAlmacen);
            return View(elementos);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FamAlm famAlm)
        {     

            string cadena = "Update FamAlm Set IDFAmilia = " + famAlm.IDFamilia + ", IDAlmacen = " + famAlm.IDAlmacen + " where ID= " + famAlm.ID + " ";
            db.Database.ExecuteSqlCommand(cadena);
            return RedirectToAction("Index");

            return View(famAlm);
        }

        // GET: FamAlm/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var elemento = dbv.Database.SqlQuery<VFamAlm>("select * from VfamAlm where ID= " + id + "").ToList().FirstOrDefault();

            return View(elemento);
        }

        // POST: FamAlm/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            string cadena = "Delete from FamAlm where ID= " + id + "";
            db.Database.ExecuteSqlCommand(cadena);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
