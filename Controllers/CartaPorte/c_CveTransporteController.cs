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
    public class c_CveTransporteController : Controller
    {
        private c_CveTransporteContext db = new c_CveTransporteContext();

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "Clave" : "Clave";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "FIVigencia" : "FIVigencia";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "FFVigencia" : "FFVigencia";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "FPublicacion" : "FPublicacion";

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
            var elementos = from s in db.transport
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.Clave.ToString().Contains(searchString) || s.Descripcion.Contains(searchString) || s.FIvigencia.ToString().Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Clave":
                    elementos = elementos.OrderBy(s => s.Clave);
                    break;
                case "Descripción":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                case "Fecha Inicial":
                    elementos = elementos.OrderBy(s => s.FIvigencia);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Clave);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.transport.OrderBy(e => e.IDCve).Count(); // Total number of elements

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
       

        // GET: c_CveTransporte/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_CveTransporte CveTransporte = db.transport.Find(id);
            if (CveTransporte == null)
            {
                return HttpNotFound();
            }
            return View(CveTransporte);
        }

        // GET: c_CveTransporte/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_CveTransporte/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(c_CveTransporte CveTransporte)
        {
            if (ModelState.IsValid)
            {
                db.transport.Add(CveTransporte);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(CveTransporte);
        }

        // GET: c_CveTransporte/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_CveTransporte CveTransporte = db.transport.Find(id);
            if (CveTransporte == null)
            {
                return HttpNotFound();
            }
            return View(CveTransporte);
        }

        // POST: c_CveTransporte/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(c_CveTransporte CveTransporte)
        {
            if (ModelState.IsValid)
            {
                db.Entry(CveTransporte).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(CveTransporte);
        }

        // GET: c_CveTransporte/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_CveTransporte CveTransporte = db.transport.Find(id);
            if (CveTransporte == null)
            {
                return HttpNotFound();
            }
            return View(CveTransporte);
        }

        // POST: c_CveTransporte/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_CveTransporte CveTransporte = db.transport.Find(id);
            db.transport.Remove(CveTransporte);
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
        public void Transporte()
        {
            //Listado de datos
            var conf = db.transport.ToList();
            //ExcelPackage.LicenseContext = LicenseContext.Commercial;
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Transporte");


            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:H1"].Style.Font.Size = 20;
            Sheet.Cells["A1:H1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:H1"].Style.Font.Bold = true;
            Sheet.Cells["A1:H1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado Clave de Transporte");
            Sheet.Cells["A1:H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:H2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:H2"].Style.Font.Size = 12;
            Sheet.Cells["A2:H2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:H2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("Clave");
            Sheet.Cells["C2"].RichText.Add("Descripción");
            Sheet.Cells["D2"].RichText.Add("FIVigencia");
            Sheet.Cells["E2"].RichText.Add("FFVigencia");
            Sheet.Cells["F2"].RichText.Add("Versión");
            Sheet.Cells["G2"].RichText.Add("Revisión");
            Sheet.Cells["H2"].RichText.Add("Fecha de Publicación");
            row = 3;
            foreach (var item in conf)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDCve;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Clave;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.FIvigencia;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.FFvigencia;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Version;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Revision;
                Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Fpublicacion;


                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteTransporte.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult PDFTransporte()
        {
            var CTR = db.transport.ToList();
            Reportec_CveTransporte report = new Reportec_CveTransporte();
            report.CTrans = CTR;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteTransporte.pdf");
        }
    }
}
