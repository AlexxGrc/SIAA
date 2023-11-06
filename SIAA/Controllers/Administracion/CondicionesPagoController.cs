using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SIAAPI.Models.Administracion;
using PagedList;
using System.Collections.Generic;

using System.IO;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;


namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class CondicionesPagoController : Controller
    {
        private CondicionesPagoContext db = new CondicionesPagoContext();

        // GET: CondicionesPagoes
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveCondicionesPago" : "ClaveCondicionesPago";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.CondicionesPagos
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {

                elementos = elementos.Where(s => s.ClaveCondicionesPago.Contains(searchString) || s.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveCondicionesPago":
                    elementos = elementos.OrderBy(s => s.ClaveCondicionesPago);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                //case "DiasCredito":
                //    elementos = elementos.OrderByDescending(s => s.DiasCredito);
                //break;
                default:
                    elementos = elementos.OrderBy(s => s.ClaveCondicionesPago);
                    break;

            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.CondicionesPagos.OrderBy(e => e.IDCondicionesPago).Count(); // Total number of elements

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

        // GET: CondicionesPagoes/Details/5
        public ActionResult Details(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        CondicionesPago condicionesPago = db.CondicionesPagos.Find(id);
        if (condicionesPago == null)
        {
            return HttpNotFound();
        }
        return View(condicionesPago);
    }

    // GET: CondicionesPagoes/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: CondicionesPagoes/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "IDCondicionesPago,ClaveCondicionesPago,Descripcion,DiasCredito")] CondicionesPago condicionesPago)
    {
        if (ModelState.IsValid)
        {
                db.CondicionesPagos.Add(condicionesPago);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(condicionesPago);
    }

    // GET: CondicionesPagoes/Edit/5
    public ActionResult Edit(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        CondicionesPago condicionesPago = db.CondicionesPagos.Find(id);
        if (condicionesPago == null)
        {
            return HttpNotFound();
        }
        return View(condicionesPago);
    }

    // POST: CondicionesPagoes/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "IDCondicionesPago,ClaveCondicionesPago,Descripcion,DiasCredito")] CondicionesPago condicionesPago)
    {
        if (ModelState.IsValid)
        {
            db.Entry(condicionesPago).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(condicionesPago);
    }

    // GET: CondicionesPagoes/Delete/5
    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        CondicionesPago condicionesPago = db.CondicionesPagos.Find(id);
        if (condicionesPago == null)
        {
            return HttpNotFound();
        }
        return View(condicionesPago);
    }

    // POST: CondicionesPagoes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        CondicionesPago condicionesPago = db.CondicionesPagos.Find(id);
            db.CondicionesPagos.Remove(condicionesPago);
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
   
        public ActionResult ListadoCondicionesPago()
        {
            CondicionesPagoContext dba = new CondicionesPagoContext();
            var condiPago = dba.CondicionesPagos.ToList();
            ReporteCondicionesPago report = new ReporteCondicionesPago();
            report.ConPago = condiPago;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteCondicionesPago.pdf");
        }


        public void GenerarExcelCondicionesPago()
        {
            //Listado de datos
            CondicionesPagoContext dba = new CondicionesPagoContext();
            var condiPago = dba.CondicionesPagos.ToList();
            ExcelPackage Ep = new ExcelPackage();

            var Sheet = Ep.Workbook.Worksheets.Add("CondicionesPagos");
            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            int row = 1;
            //Fijar la fuente para EL RANGO DE CELDAS A1:B1
            Sheet.Cells["A1:C1"].Style.Font.Size = 20;
            Sheet.Cells["A1:C1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:C1"].Style.Font.Bold = true;
            Sheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1:C1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A1"].RichText.Add("Catalogo: Condiciones de Pago");

            row = 2;
            //Fijar la fuente para EL RANGO DE CELDAS A2:B2
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 12;
            Sheet.Cells["A2:C2"].Style.Font.Bold = true;
            Sheet.Cells["A2:C2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:C2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            Sheet.Cells["A2"].RichText.Add("Clave");
            Sheet.Cells["B2"].RichText.Add("Descripción");
            Sheet.Cells["C2"].RichText.Add("Días de Crédito");
            Sheet.Cells["A2:C2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            row = 3;
            foreach (var item in condiPago)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.ClaveCondicionesPago;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.DiasCredito;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteCondicionesPagoExcel.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

    }

}

