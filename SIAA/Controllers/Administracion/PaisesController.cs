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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using SIAAPI.Reportes;

namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class PaisesController : Controller
    {
        
        private PaisesContext db = new PaisesContext();
     
        // GET: Paises
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            //return View(db.Paises.ToList());
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CodigoSortParm = String.IsNullOrEmpty(sortOrder) ? "Codigo" : "";
            ViewBag.PaisSortParm = String.IsNullOrEmpty(sortOrder) ? "Pais" : "Pais";
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
            var elementos = from s in db.Paises
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Codigo.Contains(searchString) || s.Pais.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Codigo":
                    elementos = elementos.OrderBy(s => s.Codigo);
                    break;
                case "Pais":
                    elementos = elementos.OrderBy(s => s.Pais);
                    break;
               
                default:
                    elementos = elementos.OrderBy(s => s.Pais);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Paises.OrderBy(e => e.IDPais).Count(); // Total number of elements

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

        // GET: Paises/Details/5
      
        public ActionResult Details(int? id)
        {

            var elemento = db.Paises.Single(m => m.IDPais == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }
            return View(elemento);
        }

        // GET: Paises/Create
     
        public ActionResult Create()
        {
            return View();
        }

        // POST: Paises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Paises paises)
        {
            try
            {
                // TODO: Add insert logic here

                var db = new PaisesContext();
                db.Paises.Add(paises);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Paises/Edit/5
     
        public ActionResult Edit(int? id)
        {

            var elemento = db.Paises.Single(m => m.IDPais == id);
            return View(elemento);
        }

        // POST: Paises/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.Paises.Single(m => m.IDPais == id);
                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(elemento);
            }
            catch
            {
                return View();
            }
        }

        // GET: Paises/Delete/5
      
        public ActionResult Delete(int? id)
        {
            var elemento = db.Paises.Single(m => m.IDPais == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }

        // POST: Paises/Delete/5
      
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, FormCollection collection)
        {
            try
            {
                var elemento = db.Paises.Single(m => m.IDPais == id);
                db.Paises.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }
       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult ListadoPaises()
        {
            PaisesContext dba = new PaisesContext();

            var paises = dba.Paises.ToList();
            ReportePaises report = new ReportePaises();
            report.ps = paises;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReportePaises.pdf");
        }


        //Mandamos este método para generar Excel
        public void GenerarExcelPaises()
        {
            //Listado de datos
            PaisesContext dba = new PaisesContext();
            var tipoiva = dba.Paises.ToList();
            ExcelPackage Ep = new ExcelPackage();

            var Sheet = Ep.Workbook.Worksheets.Add("Paises");
            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:C1"].Style.Font.Size = 20;
            Sheet.Cells["A1:C1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:C1"].Style.Font.Bold = true;
            Sheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1:C1"].RichText.Add("Catálogo: Paises");
            Sheet.Cells["A1:C1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;


            row = 3;
            Sheet.Cells["A2:C2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:C2"].Style.Font.Size = 12;
            Sheet.Cells["A2:C2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:C2"].Style.Font.Bold = true;
            Sheet.Cells["A2:C2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:C2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);



            Sheet.Cells["A2"].Value = "ID";
            Sheet.Cells["B2"].Value = "Codigo";
            Sheet.Cells["C2"].Value = "Pais";

            row = 3;


            foreach (var item in tipoiva)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDPais;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Codigo;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Pais;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelPaises.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}
