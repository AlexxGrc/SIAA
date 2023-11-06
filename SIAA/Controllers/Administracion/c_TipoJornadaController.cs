
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
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class c_TipoJornadaController : Controller
    {
        private c_TipoJornadaContext db = new c_TipoJornadaContext();
        // GET: c_TipoJornada
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.C1SortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveJornada" : "ClaveJornada";
            ViewBag.C2SortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";

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
            var elementos = from s in db.c_TipoJornadas
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.ClaveTipoComprobante.Contains(searchString) || s.Descripcion.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClavePeriocidad":
                    elementos = elementos.OrderBy(s => s.ClaveTipoComprobante);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClaveTipoComprobante);

                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_TipoJornadas.OrderBy(e => e.IdTipoJornada).Count(); // Total number of elements

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

        // GET:  c_TipoJornada/Details/5
        public ActionResult Details(int id)
        {
           var elemento = db.c_TipoJornadas.Single(m => m.IdTipoJornada == id);
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

        // POST: c_Banco/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int id, c_Banco collection)
        {
            var elemento = db.c_TipoJornadas.Single(m => m.IdTipoJornada == id);
            return View(elemento);
        }

        // GET: c_TipoJornada/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_TipoJornada/Create
        [HttpPost]
        public ActionResult Create(c_TipoJornada elemento)
        {
            try
            {
                // TODO: Add insert logic here
                
                db.c_TipoJornadas.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: c_TipoJornada/Edit/5
        public ActionResult Edit(int id)
        {
            var elemento = db.c_TipoJornadas.Single(m => m.IdTipoJornada == id);
            return View(elemento);
            
        }

        // POST: c_TipoJornada/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.c_TipoJornadas.Single(m => m.IdTipoJornada == id);
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

        // GET: c_TipoJornada/Delete/5
        
        public ActionResult Delete(int id)
        {
            var elemento = db.c_TipoJornadas.Single(m => m.IdTipoJornada == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }

       
        // POST: c_TipoJornada/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var elemento = db.c_TipoJornadas.Single(m => m.IdTipoJornada == id);
                db.c_TipoJornadas.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }

        public ActionResult ListadoJornada()
        {
            c_TipoJornadaContext dba = new c_TipoJornadaContext();

            var jornada = dba.c_TipoJornadas.ToList();
            ReporteTipoJornada report = new ReporteTipoJornada();
            report.Jornada = jornada;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteTipoJornada.pdf");
        }


        public void GenerarExcelJornada()
        {
            //Listado de datos
            c_TipoJornadaContext dba = new c_TipoJornadaContext();
            var jornada = dba.c_TipoJornadas.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Tipo Jornada");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:E1"].Style.Font.Size = 20;
            Sheet.Cells["A1:E1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:E1"].Style.Font.Bold = true;
            Sheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Tipo Jornada");
            Sheet.Cells["A1:E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:E2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:E2"].Style.Font.Size = 12;
            Sheet.Cells["A2:E2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:E2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("Clave");
            Sheet.Cells["B2"].RichText.Add("Descripcion");
            row = 3;
            foreach (var item in jornada)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.ClaveTipoComprobante;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Descripcion;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelTipoJornada.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}
