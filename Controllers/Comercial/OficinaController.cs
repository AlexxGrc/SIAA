using CrystalDecisions.CrystalReports.Engine;
using PagedList;
using SIAAPI.Models.Comercial;
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

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Administrador,Gerencia, Comercial,Sistemas,AdminProduccion, Compras, Almacenista, Ventas, Produccion")]
    public class OficinaController : Controller
    {
        private OficinaContext db = new OficinaContext();
        // GET: Oficina
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NombreOficinaSortParm = String.IsNullOrEmpty(sortOrder) ? "NombreOficina" : "NombreOficina";
            ViewBag.ResponsableSortParm = String.IsNullOrEmpty(sortOrder) ? "Responsable" : "Responsable";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.Oficinas
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.NombreOficina.Contains(searchString) || s.Responsable.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "NombreOficina":
                    elementos = elementos.OrderBy(s => s.NombreOficina);
                    break;
                case "Responsable":
                    elementos = elementos.OrderBy(s => s.Responsable);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.NombreOficina);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Oficinas.OrderBy(e => e.IDOficina).Count(); // Total number of elements

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
        //    var lista = from e in db.Oficinas
        //                orderby e.IDOficina
        //                select e;
        //    return View(lista);
        //}

        // GET: Oficina/Details/5
        public ActionResult Details(int id)
        {
            var elemento = db.Oficinas.Single(m => m.IDOficina == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }
            return View(elemento);
        }

        // GET: Oficina/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Oficina/Create
        [HttpPost]
        public ActionResult Create(Oficina elemento)
        {
            try
            {
                // TODO: Add insert logic here

                var db = new OficinaContext();
                db.Oficinas.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Oficina/Edit/5
        public ActionResult Edit(int id)
        {
            var elemento = db.Oficinas.Single(m => m.IDOficina == id);
            return View(elemento);
        }

        // POST: Oficina/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.Oficinas.Single(m => m.IDOficina == id);
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

        // GET: Oficina/Delete/5
        public ActionResult Delete(int id)
        {
            var elemento = db.Oficinas.Single(m => m.IDOficina == id);
            return View(elemento);
        }

        // POST: Oficina/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var db = new OficinaContext();
                var elemento = db.Oficinas.Single(m => m.IDOficina == id);
                db.Oficinas.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult ListaOficinas()
        {
            OficinaContext db = new OficinaContext();
            var oficinas = db.Oficinas.ToList();
            ReporteListaOficinas report = new ReporteListaOficinas();
            report.ListaDatosBD_Oficina = oficinas;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "application/pdf", "ReporteOficinas.pdf");
        }

        public void GenerarExcelOficina()
        {
            //Listado de datos
            OficinaContext dba = new OficinaContext();
            var oficinas = dba.Oficinas.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Oficinas");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:E1"].Style.Font.Size = 20;
            Sheet.Cells["A1:E1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:E1"].Style.Font.Bold = true;
            Sheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Oficinas");
            Sheet.Cells["A1:E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:E2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:E2"].Style.Font.Size = 12;
            Sheet.Cells["A2:E2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:E2"].Style.Font.Bold = true;
            Sheet.Cells["A2:E2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:E2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("Nombre de la oficina");
            Sheet.Cells["C2"].RichText.Add("Responsable");
            Sheet.Cells["D2"].RichText.Add("Teléfono");
            Sheet.Cells["E2"].RichText.Add("Extensión");

            row = 3;
            foreach (var item in oficinas)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDOficina;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.NombreOficina;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Responsable;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Telefono;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Extension;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelOficina.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}
