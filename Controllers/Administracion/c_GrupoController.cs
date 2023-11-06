
using PagedList;
using SIAAPI.Models.Administracion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;

namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador,Gerencia, Comercial,Sistemas,AdminProduccion, Compras, Almacenista, Ventas, Produccion")]
    public class c_GrupoController : Controller
    {
        private c_GrupoContext db = new c_GrupoContext();
        // GET: c_Grupo
     
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            //var lista = from e in db.c_Grupos
            //            orderby e.IDGrupo
            //            select e;
            //return View(lista);
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveGrupo" : "ClaveGrupo";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";

            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.c_Grupos
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Descripcion.Contains(searchString) || s.ClaveGrupo.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveGrupo":
                    elementos = elementos.OrderBy(s => s.ClaveGrupo);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_Grupos.OrderBy(e => e.IDGrupo).Count(); // Total number of elements

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

        // GET: c_Grupo/Details/5
      
        public ActionResult Details(int id)
        {
            var elemento = db.c_Grupos.Single(m => m.IDGrupo == id);
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

        // POST: c_Grupo/Details/5
        [HttpPost]
       
        [ValidateAntiForgeryToken]
        public ActionResult Details(int id, c_Banco collection)
        {
            var elemento = db.c_Grupos.Single(m => m.IDGrupo == id);
            return View(elemento);
        }

        // GET: c_Grupo/Create
       
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_Grupo/Create
        [HttpPost]
      
        public ActionResult Create(c_Grupo elemento)
        {
            try
            {
              
                db.c_Grupos.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
            
        }

        // GET: c_Grupo/Edit/5
      
        public ActionResult Edit(int id)
        {
            var elemento = db.c_Grupos.Single(m => m.IDGrupo == id);
            return View(elemento);
        }

        // POST: c_Grupo/Edit/5
        [HttpPost]
     
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.c_Grupos.Single(m => m.IDGrupo == id);
                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(elemento);
            }
            catch
            {
                return View();
            }
        }

        // GET: c_Grupo/Delete/5
       
        public ActionResult Delete(int id)
        {
            var elemento = db.c_Grupos.Single(m => m.IDGrupo == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }

        // POST: c_Grupo/Delete/5
        [HttpPost]
     
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var elemento = db.c_Grupos.Single(m => m.IDGrupo == id);
                db.c_Grupos.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }

        }
       
        public ActionResult ListadoGrupo()
        {
            c_GrupoContext dba = new c_GrupoContext();

            var grupos = dba.c_Grupos.ToList();
            ReporteGrupos report = new ReporteGrupos();
            report.grupo = grupos;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteGrupo.pdf");
        }


        public void GenerarExcelGrupo()
        {
            //Listado de datos
            c_GrupoContext dba = new c_GrupoContext();
            var grupos = dba.c_Grupos.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Grupo");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:E1"].Style.Font.Size = 20;
            Sheet.Cells["A1:E1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:E1"].Style.Font.Bold = true;
            Sheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Grupos");
            Sheet.Cells["A1:E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:E2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:E2"].Style.Font.Size = 12;
            Sheet.Cells["A2:E2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:E2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("Descripción");
            Sheet.Cells["B2"].RichText.Add("Clave");
            row = 3;
            foreach (var item in grupos)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.ClaveGrupo;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteGrupos.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }


    }
}
