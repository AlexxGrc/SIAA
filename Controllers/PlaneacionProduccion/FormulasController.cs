using PagedList;
using SIAAPI.ClasesProduccion;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.PlaneacionProduccion;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.PlaneacionProduccion
{
    public class FormulasController : Controller
    {
        FormulaContext db = new FormulaContext();
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "IDProceso" : "";
            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "IDTipoArticulo" : "IDTipoArticulo";
            ViewBag.CSortParm = String.IsNullOrEmpty(sortOrder) ? "FormulasP" : "FormulasP";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;

            //Paginación
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
     

            //Paginación
            var elementos = from s in db.Formulas
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {



                elementos = elementos.Where(s => s.IDProceso.ToString().Contains(searchString) || s.IDTipoArticulo.ToString().Contains(searchString) || s.FormulaP.ToString().Contains(searchString));

            }
            //Ordenacion

            switch (sortOrder)
            {
                case "Proceso":
                    elementos = elementos.OrderBy(s => s.IDProceso);
                    break;
                case "TipodeArticulo":
                    elementos = elementos.OrderBy(s => s.IDTipoArticulo);
                    break;
                case "FormulasP":
                    elementos = elementos.OrderBy(s => s.FormulaP);
                    break;

                default:
                    elementos = elementos.OrderByDescending(s => s.IDFormula);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Formulas.OrderBy(e => e.IDProceso).Count(); // Total number of elements

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
            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }
        // POST: /Delete/5

        public ActionResult Delete(int id)
        {
            try
            {
                var elemento = db.Formulas.Single(m => m.IDFormula == id);
                db.Formulas.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }


        //////////////////////////////////////////CATALOGO////////////////////////////

        public ActionResult AgregarFormula()
        {
            //Buscar Cliente
            ProcesoContext dbc = new ProcesoContext();

            var modelo = dbc.Procesos.OrderBy(m => m.NombreProceso).ToList();

            List<SelectListItem> listaModelo = new List<SelectListItem>();
            listaModelo.Add(new SelectListItem { Text = "--Selecciona Proceso--", Value = "0" });


            foreach (var m in modelo)
            {
                listaModelo.Add(new SelectListItem { Text = m.NombreProceso, Value = m.IDProceso.ToString() });


            }
            ViewBag.modelo = listaModelo;


            ArticuloContext ta = new ArticuloContext();
            ViewBag.IDTipoArticulo = new SelectList(ta.TipoArticulo.Where(s => s.Descripcion.Equals("Maquina") || s.Descripcion.Equals("Mano de obra")), "IDTipoArticulo", "Descripcion");
            return View();
        }


        [HttpPost]
        public ActionResult AgregarFormula(int? IDProceso, int? IDTipoArticulo, string Formula, FormCollection collection)
        {
            FormulaContext dbc = new FormulaContext();

            try
            {
                // TODO: Add insert logic here
                dbc.Database.ExecuteSqlCommand("insert into Formulas ([IDProceso],[IDTipoArticulo],[FormulaP]) values(" + IDProceso + "," + IDTipoArticulo + ",'" + Formula + "')");
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }

        }
    }
}