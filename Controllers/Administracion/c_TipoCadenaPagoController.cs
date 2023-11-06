
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;


namespace SIAAPI.Models.Administracion
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class c_TipoCadenaPagoController : Controller
    {
        private c_TipoCadenaPagoContext db = new c_TipoCadenaPagoContext();

        // GET: c_TipoCadenaPago
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.C1SortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveCadenaPago" : "";
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
            var elementos = from s in db.c_TipoCadenaPagos
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Clave.Contains(searchString) || s.Descripcion.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveCadenaPago":
                    elementos = elementos.OrderBy(s => s.Clave);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Clave);

                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_TipoCadenaPagos.OrderBy(e => e.IDTipoCadenaPago).Count(); // Total number of elements

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

        //public ActionResult Details(int id)
        //{
        //    var elemento = db.c_TipoCadenaPagos.Single(m => m.IDTipoCadenaPago == id);
        //    return View(elemento);
        //}

        // GET: c_TipoCadenaPago/Details/5
   
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoCadenaPago c_TipoCadenaPago = db.c_TipoCadenaPagos.Find(id);
            if (c_TipoCadenaPago == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoCadenaPago);
        }

        // GET: c_TipoCadenaPago/Create
      
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_TipoCadenaPago/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDTipoCadenaPago,Clave,Descripcion")] c_TipoCadenaPago c_TipoCadenaPago)
        {
            if (ModelState.IsValid)
            {
                db.c_TipoCadenaPagos.Add(c_TipoCadenaPago);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_TipoCadenaPago);
        }

        // GET: c_TipoCadenaPago/Edit/5
      
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoCadenaPago c_TipoCadenaPago = db.c_TipoCadenaPagos.Find(id);
            if (c_TipoCadenaPago == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoCadenaPago);
        }

        // POST: c_TipoCadenaPago/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDTipoCadenaPago,Clave,Descripcion")] c_TipoCadenaPago c_TipoCadenaPago)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_TipoCadenaPago).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_TipoCadenaPago);
        }

        // GET: c_TipoCadenaPago/Delete/5
     
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoCadenaPago c_TipoCadenaPago = db.c_TipoCadenaPagos.Find(id);
            if (c_TipoCadenaPago == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoCadenaPago);
        }

        // POST: c_TipoCadenaPago/Delete/5
     
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_TipoCadenaPago c_TipoCadenaPago = db.c_TipoCadenaPagos.Find(id);
            db.c_TipoCadenaPagos.Remove(c_TipoCadenaPago);
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
      

        public ActionResult ListadoTipoCadenaPago()
        {
            c_TipoCadenaPagoContext dba = new c_TipoCadenaPagoContext();

            var tp = dba.c_TipoCadenaPagos.ToList();
            ReporteTipoCadenaPago report = new ReporteTipoCadenaPago();
            report.tipoCadenaPago = tp;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteTipoCadenadePago.pdf");
        }

        //Método para la generar los reportes en excel
        public void GenerarExcelTipoCadenaPago()
        {
            //Listado de datos
            c_TipoCadenaPagoContext dba = new c_TipoCadenaPagoContext();
            var bancos = dba.c_TipoCadenaPagos.ToList();
            ExcelPackage Ep = new ExcelPackage();

            var Sheet = Ep.Workbook.Worksheets.Add("TipoCadenadePago");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:C1"].Style.Font.Size = 20;
            Sheet.Cells["A1:C1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:C1"].Style.Font.Bold = true;
            Sheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Cadena de Pago");
            Sheet.Cells["A1:C1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:C2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:C2"].Style.Font.Size = 12;
            Sheet.Cells["A2:C2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:C2"].Style.Font.Bold = true;
            Sheet.Cells["A2:C2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:C2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("Clave");
            Sheet.Cells["C2"].RichText.Add("Descripción");

            row = 3;
            foreach (var item in bancos)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDTipoCadenaPago;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Clave;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelTipoCadenadePago.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}
