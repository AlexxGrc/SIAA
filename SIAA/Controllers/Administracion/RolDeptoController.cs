using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.miempresa;
using SIAAPI.Models.Login;

namespace SIAAPI.Controllers.Administracion
{
    public class RolDeptoController : Controller
    {
        private RolDeptoContext db = new RolDeptoContext();
        private VRolDeptoContext dbr = new VRolDeptoContext();
        public DoctoSGCContext dbs = new DoctoSGCContext();
        public DepartamentosContext dbd = new DepartamentosContext();

        // GET: RolDepto
        public ActionResult Index()
        {       
            List <VRolDepto> listaind = dbr.Database.SqlQuery<VRolDepto>("select B.ID, R.RoleName as Rol, D.Nombre as Departamento from dbo.RolDepto B inner join dbo.Roles R on B.IDRol = R.RoleID inner join Departamentos D on B.IDDepartamento = D.IDDepartamento  order by D.Nombre").ToList();
            ViewBag.listaind = listaind;

            return View(listaind);
        }

        // GET: RolDepto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VRolDepto rolDepto = dbr.Database.SqlQuery<VRolDepto>("select B.ID, R.RoleName as Rol, D.Nombre as Departamento from dbo.RolDepto B inner join dbo.Roles R on B.IDRol = R.RoleID inner join Departamentos D on B.IDDepartamento = D.IDDepartamento").ToList().FirstOrDefault();
            ViewBag.rolDepto = rolDepto;

            if (rolDepto == null)
            {
                return HttpNotFound();
            }
            return View(rolDepto);
        }

        // GET: RolDepto/Create
        public ActionResult Create()
        {
            RolesContext dba = new RolesContext();
            ViewBag.IDDepartamento = new SelectList(dbd.Departamentos, "IDDepartamento", "Nombre");
            ViewBag.IDRol = new SelectList(dba.Roless, "RoleID", "ROleName");
            return View();
        }

        // POST: RolDepto/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RolDepto rolDepto)
        {
            if (ModelState.IsValid)
            {
                db.RolDepto.Add(rolDepto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rolDepto);
        }

        // GET: RolDepto/Edit/5
        public ActionResult Edit(int? id)
        {
            RolesContext dba = new RolesContext();
            RolDepto rolDepto = dbr.Database.SqlQuery<RolDepto>("Select * from dbo.RolDepto where ID =" +id).ToList().FirstOrDefault();
            ViewBag.IDDepartamento = new SelectList(dbd.Departamentos, "IDDepartamento", "Nombre", rolDepto.IDDepartamento);
            ViewBag.IDRol = new SelectList(dba.Roless, "RoleID", "ROleName", rolDepto.IDRol);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //RolDepto rolDepto = db.RolDepto.Find(id);
            if (rolDepto == null)
            {
                return HttpNotFound();
            }
            return View(rolDepto);
        }

        // POST: RolDepto/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,IDDepartamento,IDRol")] RolDepto rolDepto)
        {

            if (ModelState.IsValid)
            {
                db.Entry(rolDepto).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rolDepto);
        }

        // GET: RolDepto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VRolDepto rolDepto = dbr.Database.SqlQuery<VRolDepto>("select B.ID, R.RoleName as Rol, D.Nombre as Departamento from dbo.RolDepto B inner join dbo.Roles R on B.IDRol = R.RoleID inner join Departamentos D on B.IDDepartamento = D.IDDepartamento").ToList().FirstOrDefault();
            ViewBag.rolDepto = rolDepto;
            if (rolDepto == null)
            {
                return HttpNotFound();
            }
            return View(rolDepto);
        }

        // POST: RolDepto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RolDepto rolDepto = db.RolDepto.Find(id);
            db.RolDepto.Remove(rolDepto);
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
