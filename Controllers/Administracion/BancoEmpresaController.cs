using SIAAPI.Models.Administracion;
using System;
using System.Linq;
using System.Web.Mvc;
using static SIAAPI.Models.Comercial.EmpresaRepository;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;


namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class BancoEmpresaController : Controller
    {
        // GET: BancoEmpresa
        private BancoEmpresaContext db = new BancoEmpresaContext();

        public ActionResult Index()
        {
            //private VBcoEmpresaContext db = VBcoEmpresaContext();
            var lista = from e in db.VBcoEmpresa
                        orderby e.Nombre
                        select e;

            return View(lista);
        }

        // GET: BancoEmpresa/Details/5
      
        public ActionResult Details(int id)
        {
            var elemento = db.VBcoEmpresa.Single(m => m.IDBancoEmpresa == id);
            if (elemento == null)
            {
                return NotFound();
            }
            return View(elemento);
        }

        private ActionResult NotFound()
        {
            throw new NotImplementedException();
        }

        // POST: BancoEmpresa/Details/5
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int id, VBcoEmpresa collection)
        {
            var elemento = db.BancoEmpresa.Single((System.Linq.Expressions.Expression<Func<BancoEmpresa, bool>>)(m => m.IDBancoEmpresa == id));

            return View(elemento);
        }

        // GET: c_Banco/Create
     
        public ActionResult Create()
        {
            var listaempresa = new EmpresaNombre().GetEmpresa();
            ViewBag.datosEmpresa = listaempresa;
            var listabancos = new BancoRepository().GetBanco();
            ViewBag.datosBanco = listabancos;
            var ListMoneda = new MonedaRepository().GetMoneda();
            ViewBag.datosMoneda = ListMoneda;
            return View();
        }

        // POST: BancoEmpresa/Create
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BancoEmpresa elemento)
        {
            try
            {
                // TODO: Add insert logic here

                var db = new BancoEmpresaContext();
                db.BancoEmpresa.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: BancoEmpresa/Edit/5
      
        public ActionResult Edit(int id)
        {
            var elemento = db.BancoEmpresa.Single(m => m.IDBancoEmpresa == id);
            ViewBag.IDEmpresa = new SelectList(db.Empresa, "IDEmpresa", "RazonSocial", elemento.IDEmpresa);
            ViewBag.IDBanco = new SelectList(db.c_Banco, "IDBanco", "Nombre", elemento.IDBanco);
            ViewBag.CuentaBanco = elemento.CuentaBanco;
            ViewBag.IDMoneda = new SelectList(db.c_Moneda, "IDMoneda", "Descripcion", elemento.IDMoneda);
            return View(elemento);

        }

        // POST: BancoEmpresa/Edit/5
      
        [HttpPost]
        public ActionResult Edit(BancoEmpresa bancoEmpresa)
    {
             // TODO: Add update logic here

                if (ModelState.IsValid)
                {
                
                    db.Entry(bancoEmpresa).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
      
            ViewBag.IDEmpresa = new SelectList(db.Empresa, "IDEmpresa", "RazonSocial", bancoEmpresa.IDEmpresa);
            ViewBag.IDBanco = new SelectList(db.c_Banco, "IDBanco", "Nombre", bancoEmpresa.IDBanco);
            ViewBag.CuentaBanco = bancoEmpresa.CuentaBanco;
            ViewBag.IDMoneda = new SelectList(db.c_Moneda, "IDMoneda", "Descripcion", bancoEmpresa.IDMoneda);
            return View(bancoEmpresa);        
        }

        // GET: /Delete/5
       
        public ActionResult Delete(int id)
    {
        var elemento = db.VBcoEmpresa.Single(m => m.IDBancoEmpresa == id);
        if (elemento == null)
        {
            return HttpNotFound();
        }

        return View(elemento);
    }


        // POST: a/Delete/5
       
        [HttpPost]
    public ActionResult Delete(int id, FormCollection collection)
    {
        try
        {
                //var elemento = db.BancoEmpresa.Single(m => m.IDBancoEmpresa == id);
                //db.BancoEmpresa.Remove(elemento);
                db.Database.ExecuteSqlCommand("delete from BancoEmpresa where IDBancoEmpresa = " + id + "");
               
            return RedirectToAction("Index");

        }
        catch
        {
            return View();
        }
    }
}



}
