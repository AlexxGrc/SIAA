using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using SIAAPI.Models.CartaPorte;
using SIAAPI.Reportes;

namespace SIAAPI.Controllers.CartaPorte
{
    public class c_TipoEmbalajeController : Controller
    {
        private c_TipoEmbalajeContext db = new c_TipoEmbalajeContext();

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveDeAsignacion" : "ClaveDeAsignacion";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
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
            var elementos = from s in db.TipoEmbalaje
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.ClaveDeAsignacion.Contains(searchString) || s.Descripcion.Contains(searchString)); 

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveDeAsignacion":
                    elementos = elementos.OrderBy(s => s.ClaveDeAsignacion);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                
                default:
                    elementos = elementos.OrderBy(s => s.ClaveDeAsignacion);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.TipoEmbalaje.OrderBy(e => e.IDTEmbalaje).Count(); // Total number of elements

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


        

        // GET: c_TipoEmbalaje/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoEmbalaje c_TipoEmbalaje = db.TipoEmbalaje.Find(id);
            if (c_TipoEmbalaje == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoEmbalaje);
        }

        // GET: c_TipoEmbalaje/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_TipoEmbalaje/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(c_TipoEmbalaje c_TipoEmbalaje)
        {
            if (ModelState.IsValid)
            {
                db.TipoEmbalaje.Add(c_TipoEmbalaje);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_TipoEmbalaje);
        }

        // GET: c_TipoEmbalaje/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoEmbalaje c_TipoEmbalaje = db.TipoEmbalaje.Find(id);
            if (c_TipoEmbalaje == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoEmbalaje);
        }

        // POST: c_TipoEmbalaje/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(c_TipoEmbalaje c_TipoEmbalaje)
        {
            if (ModelState.IsValid)
            {
                db.TipoEmbalaje.Add(c_TipoEmbalaje);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_TipoEmbalaje);
        }

        // GET: c_TipoEmbalaje/Delete/5
        public ActionResult Delete(int? id)
        {

            var elemento = db.TipoEmbalaje.Single(m => m.IDTEmbalaje == id);
            return View(elemento);
        }

        // POST: c_TipoEmbalaje/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_TipoEmbalaje c_TipoEmbalaje = db.TipoEmbalaje.Find(id);
            db.TipoEmbalaje.Remove(c_TipoEmbalaje);
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


        public ActionResult ListadoEmbalaje()
        {
            var bancos = db.TipoEmbalaje.ToList();
            ReporteTipoEmbalaje report = new ReporteTipoEmbalaje();
            report.Embalaje = bancos;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteEmbalaje.pdf");
        }
        public void GenerarExcelEmbalaje()
        {
            //Listado de datos
            var bancos = db.TipoEmbalaje.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Embalaje");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:E1"].Style.Font.Size = 20;
            Sheet.Cells["A1:E1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:E1"].Style.Font.Bold = true;
            Sheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado Tipo Embalaje");
            Sheet.Cells["A1:E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:E2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:E2"].Style.Font.Size = 12;
            Sheet.Cells["A2:E2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:E2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("Clave Asignación");
            Sheet.Cells["C2"].RichText.Add("Descripción");
            Sheet.Cells["D2"].RichText.Add("Fecha Inicio");
            Sheet.Cells["E2"].RichText.Add("Fecha Final");

            row = 3;
            foreach (var item in bancos)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDTEmbalaje;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.ClaveDeAsignacion;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.FIVigencia;

                //Sheet.Cells[string.Format("E{0}", row)].Value = item.FFVigencia;

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
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelEmbalaje.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

    }
}
