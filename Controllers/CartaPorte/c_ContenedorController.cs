using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SIAAPI.Reportes;
using SIAAPI.Models.CartaPorte;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Globalization;

namespace SIAAPI.Controllers.CartaPorte
{
    public class c_ContenedorController : Controller
    {
        private c_ContenedorContext db = new c_ContenedorContext();

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "c_Colonia" : "c_Colonia";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "C_CodigoPostal" : "C_CodigoPostal";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "NomAsentamiento" : "NomAsentamiento";
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
            var elementos = from s in db.Contenedor
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.C_Colonia.Contains(searchString) || s.C_CodigoPostal.ToString().Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "C_Colonia":
                    elementos = elementos.OrderBy(s => s.C_Colonia);
                    break;
                case "C_CodigoPostal":
                    elementos = elementos.OrderBy(s => s.C_CodigoPostal);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.C_Colonia);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Contenedor.OrderBy(e => e.IDContenedor).Count(); // Total number of elements

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

        // GET: c_Contenedor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_Contenedor c_Contenedor = db.Contenedor.Find(id);
            if (c_Contenedor == null)
            {
                return HttpNotFound();
            }
            return View(c_Contenedor);
        }

        // GET: c_Contenedor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_Contenedor/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(c_Contenedor c_Contenedor)
        {
            if (ModelState.IsValid)
            {
                db.Contenedor.Add(c_Contenedor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_Contenedor);
        }

        // GET: c_Contenedor/Edit/5
        public ActionResult Edit(int id)
        {
           
            var elemento = db.Contenedor.Single(m => m.IDContenedor == id);
            return View(elemento);
        }

        // POST: c_Contenedor/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection formCollection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.Contenedor.Single(m => m.IDContenedor == id);
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

        // GET: c_Contenedor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_Contenedor c_Contenedor = db.Contenedor.Find(id);
            if (c_Contenedor == null)
            {
                return HttpNotFound();
            }
            return View(c_Contenedor);
        }

        // POST: c_Contenedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_Contenedor c_Contenedor = db.Contenedor.Find(id);
            db.Contenedor.Remove(c_Contenedor);
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

        public ActionResult ListadoContenedor()
        {
            var Tipo = db.Contenedor.ToList();
            ReporteContenedor report = new ReporteContenedor();
            report.Conte = Tipo;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteContenedor.pdf");
        }

        public void GenerarExcelContenedor()
        {
            //Listado de datos
            var bancos = db.Contenedor.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Tipo Contenedor");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:E1"].Style.Font.Size = 20;
            Sheet.Cells["A1:E1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:E1"].Style.Font.Bold = true;
            Sheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Tipo Contenedor");
            Sheet.Cells["A1:E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:E2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:E2"].Style.Font.Size = 12;
            Sheet.Cells["A2:E2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:E2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("Colonia");
            Sheet.Cells["B2"].RichText.Add("Código Postal");
            Sheet.Cells["C2"].RichText.Add("Norma de Asentamiento");
            Sheet.Cells["D2"].RichText.Add("Fecha de Inicio");
            Sheet.Cells["E2"].RichText.Add("Fecha de Fin");

            row = 3;
            foreach (var item in bancos)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.C_Colonia;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.C_CodigoPostal;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.NomAsentamiento;
                Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.FIVigencia;
                if (item.FFVigencia == null)
                {
                    Sheet.Cells[string.Format("E{0}", row)].Value = "";
                }
                else
                {
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.FFVigencia;
                }

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelContenedor.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}
