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
    public class c_ClaveUnidadPesoController : Controller
    {
        private c_ClaveUnidadPesoContext db = new c_ClaveUnidadPesoContext();

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveUnidad" : "ClaveUnidad";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "Nombre";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripción" : "Descripción";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Nota" : "Nota";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "FIVigencia" : "FIVigencia";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "FFVigencia" : "FFVigencia";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Simbolo" : "Simbolo";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Bandera" : "Bandera";
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
            var elementos = from s in db.UnidadPeso
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.ClaveUnidad.Contains(searchString) || s.Nombre.Contains(searchString) || s.Simbolo.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveUnidad":
                    elementos = elementos.OrderBy(s => s.ClaveUnidad);
                    break;
                case "Nombre":
                    elementos = elementos.OrderBy(s => s.Nombre);
                    break;
                case "Simbolo":
                    elementos = elementos.OrderBy(s => s.Simbolo);
                    break;

                default:
                    elementos = elementos.OrderBy(s => s.ClaveUnidad);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.UnidadPeso.OrderBy(e => e.IDPeso).Count(); // Total number of elements

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

       

        // GET: c_ClaveUnidadPeso/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_ClaveUnidadPeso c_ClaveUnidadPeso = db.UnidadPeso.Find(id);
            if (c_ClaveUnidadPeso == null)
            {
                return HttpNotFound();
            }
            return View(c_ClaveUnidadPeso);
        }

        // GET: c_ClaveUnidadPeso/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_ClaveUnidadPeso/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(c_ClaveUnidadPeso c_ClaveUnidadPeso)
        {
            if (ModelState.IsValid)
            {
                db.UnidadPeso.Add(c_ClaveUnidadPeso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_ClaveUnidadPeso);
        }

        // GET: c_ClaveUnidadPeso/Edit/5
        public ActionResult Edit(int id)
        {
            var elemento = db.UnidadPeso.Single(m => m.IDPeso == id);
            return View(elemento);
        }

        // POST: c_ClaveUnidadPeso/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection formCollection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.UnidadPeso.Single(m => m.IDPeso == id);
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

        // GET: c_ClaveUnidadPeso/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_ClaveUnidadPeso c_ClaveUnidadPeso = db.UnidadPeso.Find(id);
            if (c_ClaveUnidadPeso == null)
            {
                return HttpNotFound();
            }
            return View(c_ClaveUnidadPeso);
        }

        // POST: c_ClaveUnidadPeso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_ClaveUnidadPeso c_ClaveUnidadPeso = db.UnidadPeso.Find(id);
            db.UnidadPeso.Remove(c_ClaveUnidadPeso);
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

        public ActionResult ListadoUnidadPeso()
        {

            var UnidadPeso = db.UnidadPeso.ToList();
            Reportec_ClaveUnidadPeso report = new Reportec_ClaveUnidadPeso();
            report.ClaveUnidadPeso = UnidadPeso;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteUnidadPeso.pdf");
        }

        public void GenerarExcelUnidadPeso()
        {
            //Listado de datos
           //c_ClaveUnidadPeso dba = new c_ClaveUnidadPeso();
            var Unidad = db.UnidadPeso.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Unidad");
         


            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:H1"].Style.Font.Size = 20;
            Sheet.Cells["A1:H1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:H1"].Style.Font.Bold = true;
            Sheet.Cells["A1:H1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Clave Unidad Peso");
            Sheet.Cells["A1:H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:H2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:H2"].Style.Font.Size = 12;
            Sheet.Cells["A2:H2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:H2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("Clave Unidad");
            Sheet.Cells["B2"].RichText.Add("Nombre");
            Sheet.Cells["C2"].RichText.Add("Descripción");
            Sheet.Cells["D2"].RichText.Add("Nota");
            Sheet.Cells["E2"].RichText.Add("Fecha Inicio");
            Sheet.Cells["F2"].RichText.Add("Fecha Final");
            Sheet.Cells["G2"].RichText.Add("Simbolo");
            Sheet.Cells["H2"].RichText.Add("Bandera");

            row = 3;
            foreach (var item in Unidad)
            {

               Sheet.Cells[string.Format("A{0}:E{0}", row)].Style.WrapText = true;  
                Sheet.Cells[string.Format("A{0}", row)].Value = item.ClaveUnidad;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Nombre;
                //Sheet.Column(3).Width = 150;
                //Sheet.Column(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //Sheet.Column(3).Style.VerticalAlignment = ExcelVerticalAlignment.Justify;
                //Sheet.Column(3).Style.WrapText = true;
                //Sheet.Cells[string.Format("C{0}", row)].Style.WrapText = true;
               
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("C{0}", row)].Style.WrapText = true;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Nota;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.FIVigencia;

                //Sheet.Cells[string.Format("E{0}", row)].Value = item.FFVigencia;

                if (item.FFVigencia == null)
                {
                    Sheet.Cells[string.Format("F{0}", row)].Value = "";
                }
                else
                {
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.FFVigencia;
                }
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Simbolo;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Bandera;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelEmbalaje.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

    }
}
