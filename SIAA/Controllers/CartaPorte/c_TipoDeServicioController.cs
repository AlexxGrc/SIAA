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
    public class c_TipoDeServicioController : Controller
    {
        private c_TipoDeServicioContext db = new c_TipoDeServicioContext();
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "Clave" : "Clave";
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
            var elementos = from s in db.tiposervicio
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.Clave.Contains(searchString) || s.Descripcion.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Clave":
                    elementos = elementos.OrderBy(s => s.Clave);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                case "fecha":
                    elementos = elementos.OrderBy(s => s.FIVigencia);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Clave);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.tiposervicio.OrderBy(e => e.IDTServicio).Count(); // Total number of elements

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
        

        // GET: c_TipoDeServicio/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoDeServicio c_TipoDeServicio = db.tiposervicio.Find(id);
            if (c_TipoDeServicio == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoDeServicio);
        }

        // GET: c_TipoDeServicio/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_TipoDeServicio/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( c_TipoDeServicio TipoDeServicio)
        {
            if (ModelState.IsValid)
            {
                db.tiposervicio.Add(TipoDeServicio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(TipoDeServicio);
        }

        // GET: c_TipoDeServicio/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoDeServicio TipoDeServicio = db.tiposervicio.Find(id);
            if (TipoDeServicio == null)
            {
                return HttpNotFound();
            }
            return View(TipoDeServicio);
        }

        // POST: c_TipoDeServicio/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( c_TipoDeServicio TipoDeServicio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(TipoDeServicio).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(TipoDeServicio);
        }

        // GET: c_TipoDeServicio/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoDeServicio TipoDeServicio = db.tiposervicio.Find(id);
            if (TipoDeServicio == null)
            {
                return HttpNotFound();
            }
            return View(TipoDeServicio);
        }

        // POST: c_TipoDeServicio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_TipoDeServicio TipoDeServicio = db.tiposervicio.Find(id);
            db.tiposervicio.Remove(TipoDeServicio);
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
        public void TipoDeServicio()
        {
            //Listado de datos
            var conf = db.tiposervicio.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("TipoServicio");


            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:E1"].Style.Font.Size = 20;
            Sheet.Cells["A1:E1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:E1"].Style.Font.Bold = true;
            Sheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Reporte Tipo de Servicio");
            Sheet.Cells["A1:E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:E2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:E2"].Style.Font.Size = 12;
            Sheet.Cells["A2:E2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:E2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("Clave");
            Sheet.Cells["C2"].RichText.Add("Descripción");
            Sheet.Cells["D2"].RichText.Add("Fecha de Inicio de Vigencia");
            Sheet.Cells["E2"].RichText.Add("Fecha de Fin de Vigencia");

            row = 3;
            foreach (var item in conf)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDTServicio;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Clave;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.FIVigencia;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.FFVigencia;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteTipodeServicio.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult PDFTipodeServicio()
        {
            var ts = db.tiposervicio.ToList();
            Reportec_TipoDeServicio report = new Reportec_TipoDeServicio();
            report.Tservicio= ts;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteTipodeServicio.pdf");
        }
    }
}
