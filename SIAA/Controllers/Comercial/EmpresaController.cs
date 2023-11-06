using PagedList;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class EmpresaController : Controller
    {
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            EmpresaRepository db = new EmpresaRepository();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.RazonSocialSortParm = String.IsNullOrEmpty(sortOrder) ? "RazonSocial" : "RazonSocial";
            ViewBag.RFCSortParm = String.IsNullOrEmpty(sortOrder) ? "RFC" : "RFC";
            ViewBag.SiglasSortParm = String.IsNullOrEmpty(sortOrder) ? "Siglas" : "Siglas";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.GetEmpresaslistacorta()
                        select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.RazonSocial.Contains(searchString) || s.RFC.Contains(searchString) || s.Siglas.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "RazonSocial":
                    elementos = elementos.OrderBy(s => s.RazonSocial);
                    break;
                case "RFC":
                    elementos = elementos.OrderBy(s => s.RFC);
                    break;
                case "Siglas":
                    elementos = elementos.OrderBy(s => s.Siglas);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Siglas);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.GetEmpresaslistacorta().OrderBy(e => e.IDEmpresa).Count(); // Total number of elements

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
       
        public ActionResult Edit(int id)
        {
            EmpresaContext db = new EmpresaContext();
            var elementooriginial = db.empresas.Single(m => m.IDEmpresa == id);
            var listaestados = new EstadosRepository().GetEstados();
            var listaregimenes = new c_RegimenFiscalRepository().GetRegimenes();
            ViewBag.Listadeestados = listaestados;
            ViewBag.Listaderegimenes = listaregimenes;
            
            return View(elementooriginial);
            
        }

        // POST: c_impuesto/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult Edit(int id, FormCollection collection)
        {
            EmpresaContext db = new EmpresaContext();
            try
            {
                var elemento = db.empresas.Single(m => m.IDEmpresa == id);
                HttpPostedFileBase archivo = Request.Files["Image1"];
                if (archivo.FileName != "")
                {
                    elemento.Logo = new byte[archivo.ContentLength];
                    archivo.InputStream.Read(elemento.Logo, 0, archivo.ContentLength);
                }

                if (TryUpdateModel(elemento))
                {
                     db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(elemento);
            }
            catch(Exception err)
            {
                return View();
            }
        }

        public ActionResult Details(int id)
        {
            EmpresaRepository db = new EmpresaRepository();
            var elemento = db.GetEmpresaDetalle(id);
            return View(elemento);
        }

        public async Task<ActionResult> RenderImage(int id)
        {
            EmpresaContext db = new EmpresaContext();
            Empresa item = await db.empresas.FindAsync(id);

            byte[] photoBack = item.Logo;

            return File(photoBack, "image/png");
        }

        // GET: /Delete/5

        public ActionResult Delete(int id)
        {
            EmpresaContext db = new EmpresaContext();
            var elemento = db.empresas.Single(m => m.IDEmpresa == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }


        // POST: /Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            EmpresaContext db = new EmpresaContext();
            try
            {
                var elemento = db.empresas.Single(m => m.IDEmpresa == id);
                db.empresas.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }

    }

   
}