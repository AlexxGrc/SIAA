using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Login;
using PagedList;

using System.IO;

namespace SIAAPI.Controllers
{
    [Authorize(Roles = "Administrador,Gerencia, Sistemas")]
    public class RolesController : Controller
    {
        private RolesContext db = new RolesContext();

        // GET: Roles
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.RolSortParm = String.IsNullOrEmpty(sortOrder) ? "Rol" : "Rol";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación


            var elementos = from s in db.Roless select s;

            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.ROleName.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Rol":
                    elementos = elementos.OrderBy(s => s.ROleName);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.RoleID);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Roless.OrderBy(e => e.RoleID).Count(); // Total number of elements

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
            try
            {

                return View(elementos.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception err)
            {
                return View(elementos);
            }
        }
        // GET: Roles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles roles = db.Roless.Find(id);
            if (roles == null)
            {
                return HttpNotFound();
            }
            return View(roles);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoleID,ROleName")] Roles roles)
        {
            if (ModelState.IsValid)
            {
                db.Roless.Add(roles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roles);
        }

        // GET: Roles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles roles = db.Roless.Find(id);
            if (roles == null)
            {
                return HttpNotFound();
            }
            return View(roles);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoleID,ROleName")] Roles roles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roles).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roles);
        }

        // GET: Roles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles roles = db.Roless.Find(id);
            if (roles == null)
            {
                return HttpNotFound();
            }
            return View(roles);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Roles roles = db.Roless.Find(id);
            db.Roless.Remove(roles);
            db.SaveChanges();
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
