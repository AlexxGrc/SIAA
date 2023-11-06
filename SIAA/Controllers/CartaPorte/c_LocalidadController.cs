using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
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
    public class c_LocalidadController : Controller
    {
        private c_LocalidadContext db = new c_LocalidadContext();

        // GET: c_Localidad
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.LocalidadSortParm = String.IsNullOrEmpty(sortOrder) ? "C_Localidad" : "C_Localidad";
            ViewBag.EstadoSortParm = String.IsNullOrEmpty(sortOrder) ? "C_Estado" : "C_Estado";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            
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
            var elementos = from s in db.Localidad
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.C_Localidad.Contains(searchString) || s.Descripcion.Contains(searchString) || s.C_Estado.ToString().Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Clave":
                    elementos = elementos.OrderBy(s => s.C_Estado);
                    break;
                case "Descripción":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.C_Estado);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Localidad.OrderBy(e => e.IDLocalidad).Count(); // Total number of elements

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

        // GET: c_Localidad/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_Localidad c_Localidad = db.Localidad.Find(id);
            if (c_Localidad == null)
            {
                return HttpNotFound();
            }
            return View(c_Localidad);
        }

        // GET: c_Localidad/Create

        public ActionResult CrearLocalidad()
        {
            return View();
        }

        // POST: c_Localidad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CrearLocalidad(c_Localidad localidad)
        {
            if (ModelState.IsValid)
            {
                db.Localidad.Add(localidad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(localidad);
        }

        // GET: c_Localidad/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_Localidad localidad = db.Localidad.Find(id);
            if (localidad == null)
            {
                return HttpNotFound();
            }
            return View(localidad);
        }

        // POST: c_Localidad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(c_Localidad localidad)
        {
            if (ModelState.IsValid)
            {
                db.Localidad.Add(localidad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(localidad);
        }

        // GET: c_Localidad/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_Localidad c_Localidad = db.Localidad.Find(id);
            if (c_Localidad == null)
            {
                return HttpNotFound();
            }
            return View(c_Localidad);
        }

        // POST: c_Localidad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_Localidad c_Localidad = db.Localidad.Find(id);
            db.Localidad.Remove(c_Localidad);
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

        public void ExcelLocalidad()
        {
            //Listado de datos
            var conf = db.Localidad.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Localidada");


            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:D1"].Style.Font.Size = 20;
            Sheet.Cells["A1:D1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:D1"].Style.Font.Bold = true;
            Sheet.Cells["A1:D1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Localidad");
            Sheet.Cells["A1:D1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:D2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:D2"].Style.Font.Size = 12;
            Sheet.Cells["A2:D2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:D2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("ID Localidad");
            Sheet.Cells["B2"].RichText.Add("Localidad");
            Sheet.Cells["C2"].RichText.Add("Estado");
            Sheet.Cells["D2"].RichText.Add("Descripción");

            row = 3;
            foreach (var item in conf)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDLocalidad;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.C_Localidad;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.C_Estado;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Descripcion;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteLocalidad.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult PDFTipodeServicio()
        {
            var lod = db.Localidad.ToList();
            Reportec_Localidad report = new Reportec_Localidad();
            report.C_L = lod;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteLocalidad.pdf");
        }
    }
}
