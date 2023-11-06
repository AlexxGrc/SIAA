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
    public class c_NumAutorizacionNavieroController : Controller
    {
        private c_NumAutorizacionNavieroContext db = new c_NumAutorizacionNavieroContext();
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;

            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Numero Autorizado" : "Numero Autorizado";
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
            var elementos = from s in db.numAut
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.NumAutorizado.Contains(searchString) || s.FIVigencia.ToString().Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "NumAutorizado":
                    elementos = elementos.OrderBy(s => s.NumAutorizado);
                    break;
                case "Fecha Inicio":
                    elementos = elementos.OrderBy(s => s.FIVigencia);
                    break;
                case "Fecha Final":
                    elementos = elementos.OrderBy(s => s.FFVigencia);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.NumAutorizado);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.numAut.OrderBy(e => e.IDANaviero).Count(); // Total number of elements

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


        // GET: c_NumAutorizacionNaviero/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_NumAutorizacionNaviero c_NumAutorizacionNaviero = db.numAut.Find(id);
            if (c_NumAutorizacionNaviero == null)
            {
                return HttpNotFound();
            }
            return View(c_NumAutorizacionNaviero);
        }

        // GET: c_NumAutorizacionNaviero/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_NumAutorizacionNaviero/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(c_NumAutorizacionNaviero c_NumAutorizacionNaviero)
        {
            if (ModelState.IsValid)
            {
                db.numAut.Add(c_NumAutorizacionNaviero);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_NumAutorizacionNaviero);
        }

        // GET: c_NumAutorizacionNaviero/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_NumAutorizacionNaviero NumAutorizacionNaviero = db.numAut.Find(id);
            if (NumAutorizacionNaviero == null)
            {
                return HttpNotFound();
            }
            return View(NumAutorizacionNaviero);
        }

        // POST: c_NumAutorizacionNaviero/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(c_NumAutorizacionNaviero NumAutorizacionNaviero)
        {
            if (ModelState.IsValid)
            {
                db.Entry(NumAutorizacionNaviero).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(NumAutorizacionNaviero);
        }

        // GET: c_NumAutorizacionNaviero/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_NumAutorizacionNaviero c_NumAutorizacionNaviero = db.numAut.Find(id);
            if (c_NumAutorizacionNaviero == null)
            {
                return HttpNotFound();
            }
            return View(c_NumAutorizacionNaviero);
        }

        // POST: c_NumAutorizacionNaviero/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_NumAutorizacionNaviero c_NumAutorizacionNaviero = db.numAut.Find(id);
            db.numAut.Remove(c_NumAutorizacionNaviero);
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
        public void ExcelNumAN()
        {
            //Listado de datos
            var conf = db.numAut.ToList();
            //ExcelPackage.LicenseContext = LicenseContext.Commercial;
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Agente Naviero");


            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:D1"].Style.Font.Size = 20;
            Sheet.Cells["A1:D1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:D1"].Style.Font.Bold = true;
            Sheet.Cells["A1:D1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado Agente Naviero Consignatario");
            Sheet.Cells["A1:D1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:D2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:D2"].Style.Font.Size = 12;
            Sheet.Cells["A2:D2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:D2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("No. Autorizado");
            Sheet.Cells["C2"].RichText.Add("Fecha Inicio Vigencia");
            Sheet.Cells["D2"].RichText.Add("Fecha Final Vigencia");

            row = 3;
            foreach (var item in conf)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDANaviero;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.NumAutorizado;
                Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.FIVigencia;
                Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.FFVigencia;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelNumAutorizacion.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult PDFNum()
        {
            var Numauto = db.numAut.ToList();
            Reportes.Reportec_NumAutorizacionNaviero report = new Reportes.Reportec_NumAutorizacionNaviero();
            report.auto = Numauto;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteNumAutorizacion.pdf");
        }
    }
}
