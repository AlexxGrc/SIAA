
using PagedList;
using SIAAPI.Models.Login;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers
{
    [Authorize(Roles = "Administrador,Gerencia, Sistemas,Compras")]
    public class UsuarioController : Controller
    {
     

        private UserContext db = new UserContext();
        
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "Nombre";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.Users select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Username.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Nombre":
                    elementos = elementos.OrderByDescending(s => s.Username);
                    break;

                default:
                    elementos = elementos.OrderByDescending(s => s.Username);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Users.OrderBy(e => e.UserID).Count(); // Total number of elements

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

        // GET: Proveedor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User usuario = db.Users.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Proveedor/Create
        public ActionResult Create(string mensaje)
        {
            ViewBag.Mensaje = mensaje;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User usuario)
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + usuario.Username+ "'").ToList();

            if (userid.Count()>0)
            {
                string Mensaje = "Ya existe un usuario con este nombre, intente nuevamente.";
                return RedirectToAction("Create", new { mensaje= Mensaje });
            }
            if (ModelState.IsValid)
            {
                
                string pass = MD5P(usuario.Password);

                db.Database.ExecuteSqlCommand("insert into [dbo].[User]([Username],[Password],[EmailID],[Estado]) values ('" + usuario.Username + "','" +pass + "','" + usuario.EmailID + "','Noasignado')");
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        private string MD5P(string password)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(password));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

      
       



        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User usuario = db.Users.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User usuario)
        {
            
            if (ModelState.IsValid)
            {
               
                string pass = MD5P(usuario.Password);
                db.Database.ExecuteSqlCommand("update [dbo].[User] set [Username]='" + usuario.Username + "',[Password]='" + pass + "', [EmailID]='" + usuario.EmailID + "' where UserID='" + usuario.UserID + "'");
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usuario);
        }


        // GET: Proveedor/Delete/5
        public ActionResult Delete(int? id, string Mensaje = "")
        {
            ViewBag.Mensaje = Mensaje;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User usuario = db.Users.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Proveedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string Mensaje = "";
            try
            {
                UserRole rol = new RolesContext().Database.SqlQuery<UserRole>("select*from UserRole where userid= " + id).ToList().FirstOrDefault();
                if (rol != null)
                {
                    Mensaje = "No se puede eliminar el usuario, tiene un rol asignado";
                    return RedirectToAction("Delete", new { Mensaje = Mensaje });
                }
                User usuario = db.Users.Find(id);
                db.Users.Remove(usuario);
                db.Database.ExecuteSqlCommand("delete from [dbo].[User] where UserID=" + id);

            }
            catch (Exception err)
            {
                Mensaje = "No se puede eliminar el usuario, tiene un documento relacionado";
                return RedirectToAction("Delete", new { Mensaje = Mensaje });

            }

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