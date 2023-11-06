using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Comercial;
using PagedList;

using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;

using System.IO;

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Administrador,Gerencia, Comercial,Sistemas,AdminProduccion, Compras, Almacenista, Ventas, Produccion")]
    public class TipoArticuloController : Controller
    {
        private ArticuloContext db = new ArticuloContext();

        //// GET: TipoArticulo
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.TipoArticulo
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.TipoArticulo.OrderBy(e => e.IDTipoArticulo).Count(); // Total number of elements

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

        //public ActionResult Index()
        //{
        //    return View(db.TipoArticulo.ToList());
        //}

        // GET: TipoArticulo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoArticulo tipoArticulo = db.TipoArticulo.Find(id);
            if (tipoArticulo == null)
            {
                return HttpNotFound();
            }
            return View(tipoArticulo);
        }

        // GET: TipoArticulo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoArticulo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDTipoArticulo,Descripcion")] TipoArticulo tipoArticulo)
        {
            if (ModelState.IsValid)
            {
                db.TipoArticulo.Add(tipoArticulo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoArticulo);
        }

        // GET: TipoArticulo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoArticulo tipoArticulo = db.TipoArticulo.Find(id);
            if (tipoArticulo == null)
            {
                return HttpNotFound();
            }
            return View(tipoArticulo);
        }

        // POST: TipoArticulo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDTipoArticulo,Descripcion")] TipoArticulo tipoArticulo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoArticulo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoArticulo);
        }

        // GET: TipoArticulo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoArticulo tipoArticulo = db.TipoArticulo.Find(id);
            if (tipoArticulo == null)
            {
                return HttpNotFound();
            }
            return View(tipoArticulo);
        }

        // POST: TipoArticulo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoArticulo tipoArticulo = db.TipoArticulo.Find(id);
            db.TipoArticulo.Remove(tipoArticulo);
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
        public ActionResult ListaTipoArticulo()
        {
            ArticuloContext db = new ArticuloContext();
            var tiposarticulos = db.TipoArticulo.ToList();
            RprtLstTipoArticulo report = new RprtLstTipoArticulo();
            report.ListaDatosBD_TipoArticulo = tiposarticulos;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "application/pdf", "ReporteTiposArticulo.pdf");
        }

        public void GenerarExcelTípoArt()
        {
            //Listado de datos
            ArticuloContext db = new ArticuloContext();
            var tiposarticulos = db.TipoArticulo.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("TipoArtículo");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:B1"].Style.Font.Size = 20;
            Sheet.Cells["A1:B1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:B1"].Style.Font.Bold = true;
            Sheet.Cells["A1:B1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("TipoArticulo");
            Sheet.Cells["A1:B1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:B2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:B2"].Style.Font.Size = 12;
            Sheet.Cells["A2:B2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:B2"].Style.Font.Bold = true;
            Sheet.Cells["A2:B2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:B2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("Descripción");
            row = 3;
            foreach (var item in tiposarticulos)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDTipoArticulo;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Descripcion;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelTipoArticulo.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}
