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
    [Authorize(Roles = "Administrador,Gerencia, Sistemas,Compras")]
    public class UserRoleController : Controller
    {
        private UserRoleContext ur = new UserRoleContext();
        private RolesContext r = new RolesContext();
        private UserContext u = new UserContext();
        // GET: UserRole
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "Nombre";
            ViewBag.RolSortParm = String.IsNullOrEmpty(sortOrder) ? "Rol" : "Rol";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación

           
              var elementos = from s in ur.UserRoles.Include(s => s.Roles).Include(s => s.User) select s;
       

            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.User.Username.Contains(searchString) || s.Roles.ROleName.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Nombre":
                    elementos = elementos.OrderBy(s => s.User.Username);
                    break;
                case "Municipio":
                    elementos = elementos.OrderBy(s => s.Roles.ROleName);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.UserRolesID);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = ur.UserRoles.OrderBy(e => e.UserRolesID).Count(); // Total number of elements

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
                string error = err.Message;
                return View(elementos);
            }
            //Paginación
        }
        //public ActionResult Index()
        //{
        //    var userRoles = ur.UserRoles.Include(u => u.Roles).Include(u => u.User);
        //    return View(userRoles.ToList());
        //}

        // GET: UserRole/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRole userRole = ur.UserRoles.Find(id);
            if (userRole == null)
            {
                return HttpNotFound();
            }
            return View(userRole);
        }
        public ActionResult Asignar()
        {
            ViewBag.RoleID = new SelectList(r.Roless, "RoleID", "ROleName");
            ViewBag.UserID = new SelectList(u.Users.Where(c => c.Estado.Equals("Noasignado")), "UserID", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Asignar(UserRole userole)
        {
           
            if (ModelState.IsValid)
            {
                ur.UserRoles.Add(userole);
                ur.Database.ExecuteSqlCommand("update [dbo].[User] set [Estado]='Asignado' where UserID='" + userole.UserID + "'");
                ur.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleID = new SelectList(r.Roless, "RoleID", "ROleName", userole.RoleID);
            ViewBag.UserID = new SelectList(u.Users.Where(c => c.Estado.Equals("Noasignado")), "UserID", "Username", userole.UserID);
            return View(userole);
        }

        public ActionResult DobleAsignar()
        {
            ViewBag.RoleID = new SelectList(r.Roless, "RoleID", "ROleName");
            ViewBag.UserID = new SelectList(u.Users.Where(c => c.Estado.Equals("Asignado")).OrderBy( c=> c.Username), "UserID", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DobleAsignar(UserRole userole)
        {

            if (ModelState.IsValid)
            {
                ur.UserRoles.Add(userole);
                ur.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleID = new SelectList(r.Roless, "RoleID", "ROleName", userole.RoleID);
            ViewBag.UserID = new SelectList(u.Users.Where(c => c.Estado.Equals("Asignado")), "UserID", "Username", userole.UserID);
            return View(userole);
        }
        // GET: UserRole/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRole userRole = ur.UserRoles.Find(id);
            if (userRole == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleID = new SelectList(ur.Roless, "RoleID", "ROleName", userRole.RoleID);
            ViewBag.UserID = new SelectList(ur.Users.Where(c => c.Estado.Equals("Asignado")), "UserID", "Username", userRole.UserID);
          
            return View(userRole);
        }

        // POST: UserRole/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                ur.Entry(userRole).State = System.Data.Entity.EntityState.Modified;
                ur.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleID = new SelectList(ur.Roless, "RoleID", "ROleName", userRole.RoleID);
            ViewBag.UserID = new SelectList(ur.Users.Where(c => c.Estado.Equals("Asignado")), "UserID", "Username", userRole.UserID);
            return View(userRole);
        }

        // GET: UserRole/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRole userRole = ur.UserRoles.Find(id);
            if (userRole == null)
            {
                return HttpNotFound();
            }
            return View(userRole);
        }

        // POST: UserRole/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserRole userRole = ur.UserRoles.Find(id);
            ur.UserRoles.Remove(userRole);
            ur.Database.ExecuteSqlCommand("update [dbo].[User] set [Estado]='Noasignado' where UserID='" + userRole.UserID + "'");
            ur.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ur.Dispose();
            }
            base.Dispose(disposing);
        }
       

    }
}
