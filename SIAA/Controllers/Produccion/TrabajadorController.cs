using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Produccion;
using System.Threading.Tasks;
using PagedList;


namespace SIAAPI.Controllers.Produccion
{
    public class TrabajadorController : Controller
    {
        private TrabajadorContext db = new TrabajadorContext();

        // GET: Trabajador
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.OficinaSortParm = String.IsNullOrEmpty(sortOrder) ? "Oficina" : "Oficina";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "Nombre";
            ViewBag.RFCSortParm = String.IsNullOrEmpty(sortOrder) ? "RFC" : "RFC";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
         
            var elementos = from s in db.Trabajadores.Include(t => t.Oficina)
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Nombre.Contains(searchString) || s.Oficina.NombreOficina.Contains(searchString) );
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Oficina":
                    elementos = elementos.OrderBy(s => s.Oficina.NombreOficina);
                    break;
                case "Nombre":
                    elementos = elementos.OrderBy(s => s.Nombre);
                    break;
               
                default:
                    elementos = elementos.OrderBy(s => s.Nombre);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Trabajadores.OrderBy(e => e.IDTrabajador).Count(); // Total number of elements

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
        //public ActionResult Index()
        //{


        //    var trabajadores = db.Trabajadores.Include(t => t.c_PeriocidadPago).Include(t => t.c_TipoContrato).Include(t => t.c_Tipojornada).Include(t => t.Oficina);
        //    return View(trabajadores.ToList());
        //}

        // GET: Trabajador/Details/5
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trabajador trabajador = db.Trabajadores.Find(id);
            if (trabajador == null)
            {
                return HttpNotFound();
            }
            return View(trabajador);
        }

        // GET: Trabajador/Create
        public ActionResult Create()
        {
           
            ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina");
            return View();
        }

        // POST: Trabajador/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDTrabajador,Nombre,Mail,Telefono,Photo,IDOficina,Activo,Notas")] Trabajador trabajador)
        {

            trabajador.FechaIngreso = DateTime.Now;
             HttpPostedFileBase archivo = Request.Files["Image1"];
            if (archivo.FileName != "")
            {
                trabajador.Photo = new byte[archivo.ContentLength];
                archivo.InputStream.Read(trabajador.Photo, 0, archivo.ContentLength);
            }
           
           try
            {
                db.Trabajadores.Add(trabajador);
                db.SaveChanges();
            

              
                return RedirectToAction("Index");
            }
            catch(Exception err)
            {

           

            ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina", trabajador.IDOficina);
            return View(trabajador);
            }
        }
       

        // GET: Trabajador/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trabajador trabajador = db.Trabajadores.Find(id);
            if (trabajador == null)
            {
                return HttpNotFound();
            }
          
            ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina", trabajador.IDOficina);
            return View(trabajador);
        }

        // POST: Trabajador/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDTrabajador,RFC,Nombre,Mail,Telefono,Photo,IDOficina,Activo,Notas")] Trabajador trabajador)
        {
            HttpPostedFileBase archivo = Request.Files["Image1"];
            if (archivo.FileName != "")
            {
                trabajador.Photo = new byte[archivo.ContentLength];
                archivo.InputStream.Read(trabajador.Photo, 0, archivo.ContentLength);
            }

            if (ModelState.IsValid)
            {
                db.Entry(trabajador).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        
            ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina", trabajador.IDOficina);
            return View(trabajador);
        }

        // GET: Trabajador/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trabajador trabajador = db.Trabajadores.Find(id);
            if (trabajador == null)
            {
                return HttpNotFound();
            }
            return View(trabajador);
        }

        // POST: Trabajador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trabajador trabajador = db.Trabajadores.Find(id);
            db.Trabajadores.Remove(trabajador);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> RenderImage(int id)
        {
            TrabajadorContext db = new TrabajadorContext();
            Trabajador item = await db.Trabajadores.FindAsync(id);

            byte[] photoBack = item.Photo;

            return File(photoBack, "image/png");
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
