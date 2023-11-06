using SIAAPI.Models.Login;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using SIAAPI.Reportes;
using SIAAPI.Models.CartaPorte;
using System.Globalization;

namespace SIAAPI.Controllers.CartaPorte
{
    public class c_TipoEstacionController : Controller
    {
        private c_TipoEstacionContext db = new c_TipoEstacionContext();

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "DescripcionEstacion" : "DescripcionEstacion";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveTransporte" : "ClaveTransporte";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "FIVigencia" : "FIVigencia";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "FFVigencia" : "FFVigencia";
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
            var elementos = from s in db.TipoEstacion
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.DescripcionEstacion.Contains(searchString) || s.ClaveTransporte.ToString().Contains(searchString) || s.FIVigencia.ToString().Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "DescripcionEstacion":
                    elementos = elementos.OrderBy(s => s.DescripcionEstacion);
                    break;
                case "ClaveTransporte":
                    elementos = elementos.OrderBy(s => s.ClaveTransporte);
                    break;
                case "FIVigencia":
                    elementos = elementos.OrderBy(s => s.FIVigencia);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.DescripcionEstacion);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.TipoEstacion.OrderBy(e => e.IDTEstacion).Count(); // Total number of elements

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

        

        // GET: c_TipoEstacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoEstacion c_TipoEstacion = db.TipoEstacion.Find(id);
            if (c_TipoEstacion == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoEstacion);
        }

        // GET: c_TipoEstacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_TipoEstacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(c_TipoEstacion c_TipoEstacion)
        {
            if (ModelState.IsValid)
            {
                db.TipoEstacion.Add(c_TipoEstacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_TipoEstacion);
        }

        // GET: c_TipoEstacion/Edit/5
        public ActionResult Edit(int id)
        {
            

            var elemento = db.TipoEstacion.Single(m => m.IDTEstacion == id);
            return View(elemento);
        }

        // POST: c_TipoEstacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection formCollection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.TipoEstacion.Single(m => m.IDTEstacion == id);
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

        // GET: c_TipoEstacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoEstacion c_TipoEstacion = db.TipoEstacion.Find(id);
            if (c_TipoEstacion == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoEstacion);
        }

        // POST: c_TipoEstacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_TipoEstacion c_TipoEstacion = db.TipoEstacion.Find(id);
            db.TipoEstacion.Remove(c_TipoEstacion);
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

        public ActionResult ListadoEstacion()
        {
            var Tipo = db.TipoEstacion.ToList();
            ReporteTipoEstacion report = new ReporteTipoEstacion();
            report.TipoEstacion = Tipo;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteTipoEstacion.pdf");
        }

        public void GenerarExcelEstacion()
        {
            //Listado de datos
            var bancos = db.TipoEstacion.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Tipo Estación");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:E1"].Style.Font.Size = 20;
            Sheet.Cells["A1:E1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:E1"].Style.Font.Bold = true;
            Sheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Tipo Estacion");
            Sheet.Cells["A1:E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:E2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:E2"].Style.Font.Size = 12;
            Sheet.Cells["A2:E2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:E2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("Clave Estación");
            Sheet.Cells["B2"].RichText.Add("Descripción");
            Sheet.Cells["C2"].RichText.Add("Clave Transporte");
            Sheet.Cells["D2"].RichText.Add("Fecha de Inicio");
            Sheet.Cells["E2"].RichText.Add("Fecha de Fin");

            row = 3;
            foreach (var item in bancos)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.ClaveEstacion;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.DescripcionEstacion;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.ClaveTransporte;
                Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.FIVigencia;
                if (item.FFVigencia == null)
                {
                    Sheet.Cells[string.Format("E{0}", row)].Value = "";
                }
                else
                {
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.FFVigencia;
                }

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelBanco.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

    }
}
