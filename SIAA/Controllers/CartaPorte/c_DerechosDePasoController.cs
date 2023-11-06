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
    public class c_DerechosDePasoController : Controller
    {
        private c_DerechosDePasoContext db = new c_DerechosDePasoContext();
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "Clave" : "Clave";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Derecho de paso" : "Derecho de paso";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Entre" : "Entre";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Hasta" : "Hasta";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "OtorgaR" : "OtorgaR";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "OtorgaR" : "OtorgaR";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Cuestionario" : "Cuestionario";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Fecha de Inicio" : "Fecha de Inicio";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Fecha de Terminación" : "Fecha de Terminación";

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
            var elementos = from s in db.derecho
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.ClavePaso.Contains(searchString) || s.DerechoPaso.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Clave":
                    elementos = elementos.OrderBy(s => s.ClavePaso);
                    break;
                case "Descripción":
                    elementos = elementos.OrderBy(s => s.DerechoPaso);
                    break;
                case "Fecha Inicial":
                    elementos = elementos.OrderBy(s => s.Entre);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClavePaso);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.derecho.OrderBy(e => e.IDDerecho).Count(); // Total number of elements

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
        

        // GET: c_DerechosDePaso/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_DerechosDePaso c_derechosdepaso = db.derecho.Find(id);
            if (c_derechosdepaso == null)
            {
                return HttpNotFound();
            }
            return View(c_derechosdepaso);
        }

        // GET: c_DerechosDePaso/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_DerechosDePaso/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(c_DerechosDePaso c_derechosdepaso)
        {
            if (ModelState.IsValid)
            {
                db.derecho.Add(c_derechosdepaso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_derechosdepaso);
        }

        // GET: c_DerechosDePaso/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_DerechosDePaso c_derechosdepaso = db.derecho.Find(id);
            if (c_derechosdepaso == null)
            {
                return HttpNotFound();
            }
            return View(c_derechosdepaso);
        }

        // POST: c_DerechosDePaso/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(c_DerechosDePaso c_derechosdepaso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_derechosdepaso).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_derechosdepaso);
        }

        // GET: c_DerechosDePaso/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_DerechosDePaso c_derechosdepaso = db.derecho.Find(id);
            if (c_derechosdepaso == null)
            {
                return HttpNotFound();
            }
            return View(c_derechosdepaso);
        }

        // POST: c_DerechosDePaso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_DerechosDePaso c_derechosdepaso = db.derecho.Find(id);
            db.derecho.Remove(c_derechosdepaso);
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
        public void ExcelDerechoPaso()
        {
            //Listado de datos
            var conf = db.derecho.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Derecho de Paso");


            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:I1"].Style.Font.Size = 20;
            Sheet.Cells["A1:I1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:I1"].Style.Font.Bold = true;
            Sheet.Cells["A1:I1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado Derechos de Paso");
            Sheet.Cells["A1:I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:I2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:I2"].Style.Font.Size = 12;
            Sheet.Cells["A2:I2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:I2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("Clave del derecho de paso");
            Sheet.Cells["C2"].RichText.Add("Derecho de paso");
            Sheet.Cells["D2"].RichText.Add("Entre");
            Sheet.Cells["E2"].RichText.Add("Hasta");
            Sheet.Cells["F2"].RichText.Add("Otorga/Recibe");
            Sheet.Cells["G2"].RichText.Add("Concesionario");
            Sheet.Cells["H2"].RichText.Add("Fecha de inicio de vigencia");
            Sheet.Cells["I2"].RichText.Add("Fecha de fin de vigencia");

            row = 3;
            foreach (var item in conf)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDDerecho;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.ClavePaso;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.DerechoPaso;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Entre;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Hasta;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.OtorgaRecibe;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Concesionario;
                Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.FIvigencia;
                Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.FFvigencia;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteDerechodePaso.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult PDFDerechoPaso()
        {
            var DP = db.derecho.ToList();
            Reportes.Reportec_DerechosDePaso report = new Reportes.Reportec_DerechosDePaso();
            report.Derechop = DP;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteDerechodePaso.pdf");
        }
    }
}
