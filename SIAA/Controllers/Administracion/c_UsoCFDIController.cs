using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Administracion;
using PagedList;

using System.IO;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;


namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class c_UsoCFDIController : Controller
    {
        private c_UsoCFDIContext db = new c_UsoCFDIContext();

        // GET: c_UsoCFDI
        
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.C1SortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveCFDI" : "ClaveCFDI";
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
            var elementos = from s in db.c_UsoCFDIS
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.ClaveCFDI.Contains(searchString) || s.Descripcion.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveCFDI":
                    elementos = elementos.OrderBy(s => s.ClaveCFDI);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClaveCFDI);

                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_UsoCFDIS.OrderBy(e => e.IDUsoCFDI).Count(); // Total number of elements

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

        // GET: c_UsoCFDI/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_UsoCFDI c_UsoCFDI = db.c_UsoCFDIS.Find(id);
            if (c_UsoCFDI == null)
            {
                return HttpNotFound();
            }
            return View(c_UsoCFDI);
        }

        // GET: c_UsoCFDI/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_UsoCFDI/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDUsoCFDI,ClaveCFDI,Descripcion")] c_UsoCFDI c_UsoCFDI)
        {
            if (ModelState.IsValid)
            {
                db.c_UsoCFDIS.Add(c_UsoCFDI);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_UsoCFDI);
        }

        // GET: c_UsoCFDI/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_UsoCFDI c_UsoCFDI = db.c_UsoCFDIS.Find(id);
            if (c_UsoCFDI == null)
            {
                return HttpNotFound();
            }
            return View(c_UsoCFDI);
        }

        // POST: c_UsoCFDI/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDUsoCFDI,ClaveCFDI,Descripcion")] c_UsoCFDI c_UsoCFDI)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_UsoCFDI).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_UsoCFDI);
        }

        // GET: c_UsoCFDI/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_UsoCFDI c_UsoCFDI = db.c_UsoCFDIS.Find(id);
            if (c_UsoCFDI == null)
            {
                return HttpNotFound();
            }
            return View(c_UsoCFDI);
        }

        // POST: c_UsoCFDI/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_UsoCFDI c_UsoCFDI = db.c_UsoCFDIS.Find(id);
            db.c_UsoCFDIS.Remove(c_UsoCFDI);
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
      
        public ActionResult ListadoUsoCFDI()
        {
            c_UsoCFDIContext dba = new c_UsoCFDIContext();
            var uso = dba.c_UsoCFDIS.ToList();
            ReporteUsoCFDI report = new ReporteUsoCFDI();
            report.usoCFDI = uso;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteUsoCFDI.pdf");
        }


        public void GenerarExcelUsoCFDI()
        {
            //Listado de datos
            c_UsoCFDIContext dba = new c_UsoCFDIContext();
            var usoCFDI = dba.c_UsoCFDIS.ToList();
            ExcelPackage Ep = new ExcelPackage();

            var Sheet = Ep.Workbook.Worksheets.Add("UsoCFDI");
            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            int row = 1;
            //Fijar la fuente para EL RANGO DE CELDAS A1:B1
            Sheet.Cells["A1:B1"].Style.Font.Size = 20;
            Sheet.Cells["A1:B1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:B1"].Style.Font.Bold = true;
            Sheet.Cells["A1:B1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1:B1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A1"].RichText.Add("Catalogo: Uso CFDI");

            row = 2;
            //Fijar la fuente para EL RANGO DE CELDAS A2:B2
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 12;
            Sheet.Cells["A2:C2"].Style.Font.Bold = true;
            Sheet.Cells["A2:C2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:C2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            //Sheet.Cells["A1"].Value = "Clave";
            //Sheet.Cells["B1"].Value = "Descripción";
            Sheet.Cells["A2"].RichText.Add("Clave CFDI");
            Sheet.Cells["B2"].RichText.Add("Descripción");
            Sheet.Cells["A2:B2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            row = 3;
            foreach (var item in usoCFDI)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.ClaveCFDI;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Descripcion;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteUsoCFDIExcel.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}
