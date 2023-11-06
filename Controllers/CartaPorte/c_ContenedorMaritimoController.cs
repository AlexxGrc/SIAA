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

namespace SIAAPI.Controllers.CartaPorte
{
    public class c_ContenedorMaritimoController : Controller
    {
        private c_ContenedorMaritimoContext db = new c_ContenedorMaritimoContext();
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveContenedor" : "ClaveContenedor";
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
            var elementos = from s in db.contenedor
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.ClaveContenedor.Contains(searchString) || s.Descripcion.Contains(searchString) || s.FIvigencia.ToString().Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Clave":
                    elementos = elementos.OrderBy(s => s.ClaveContenedor);
                    break;
                case "Descripción":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                case "Fecha Inicial":
                    elementos = elementos.OrderBy(s => s.FIvigencia);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClaveContenedor);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.contenedor.OrderBy(e => e.IDCMaritimo).Count(); // Total number of elements

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
        
        // GET: c_ContenedorMaritimo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_ContenedorMaritimo c_ContenedorMaritimo = db.contenedor.Find(id);
            if (c_ContenedorMaritimo == null)
            {
                return HttpNotFound();
            }
            return View(c_ContenedorMaritimo);
        }

        // GET: c_ContenedorMaritimo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_ContenedorMaritimo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(c_ContenedorMaritimo c_ContenedorMaritimo)
        {
            if (ModelState.IsValid)
            {
                db.contenedor.Add(c_ContenedorMaritimo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_ContenedorMaritimo);
        }

        // GET: c_ContenedorMaritimo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_ContenedorMaritimo c_ContenedorMaritimo = db.contenedor.Find(id);
            if (c_ContenedorMaritimo == null)
            {
                return HttpNotFound();
            }
            return View(c_ContenedorMaritimo);
        }

        // POST: c_ContenedorMaritimo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(c_ContenedorMaritimo c_ContenedorMaritimo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_ContenedorMaritimo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_ContenedorMaritimo);
        }

        // GET: c_ContenedorMaritimo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_ContenedorMaritimo c_ContenedorMaritimo = db.contenedor.Find(id);
            if (c_ContenedorMaritimo == null)
            {
                return HttpNotFound();
            }
            return View(c_ContenedorMaritimo);
        }

        // POST: c_ContenedorMaritimo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_ContenedorMaritimo c_ContenedorMaritimo = db.contenedor.Find(id);
            db.contenedor.Remove(c_ContenedorMaritimo);
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
        public void ExcelContenedorMaritimo()
        {
            //Listado de datos
            var conf = db.contenedor.ToList();
            //ExcelPackage.LicenseContext = LicenseContext.Commercial;
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Contendor Marítimo");


            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:E1"].Style.Font.Size = 20;
            Sheet.Cells["A1:E1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:E1"].Style.Font.Bold = true;
            Sheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Contenedor Marítimo");
            Sheet.Cells["A1:E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:E2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:E2"].Style.Font.Size = 12;
            Sheet.Cells["A2:E2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:E2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("Clave Contenedor");
            Sheet.Cells["C2"].RichText.Add("Descripción");
            Sheet.Cells["D2"].RichText.Add("Fecha Inicio Vigencia");
            Sheet.Cells["E2"].RichText.Add("Fecha Final Vigencia");

            row = 3;
            foreach (var item in conf)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDCMaritimo;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.ClaveContenedor;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.FIvigencia;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.FFvigencia;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteContenedor.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult PDFContenedor()
        {
            var contM = db.contenedor.ToList();
            Reportes.Reportec_ContenedorMaritimo report = new Reportes.Reportec_ContenedorMaritimo();
            report.ConM = contM;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteContenedor.pdf");
        }
    }
}
