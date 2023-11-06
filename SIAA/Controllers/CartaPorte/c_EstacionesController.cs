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
    public class c_EstacionesController : Controller
    {
        private c_EstacionesContext db = new c_EstacionesContext();
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "Clave" : "Clave";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Clave_ID" : "Clave_ID";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Nacionalidad" : "Nacionalidad";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Designador" : "Designador";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Linea" : "Linea";
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
            var elementos = from s in db.estacion
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.Clave.ToString().Contains(searchString) || s.Descripcion.Contains(searchString) || s.FIVigencia.ToString().Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Clave":
                    elementos = elementos.OrderBy(s => s.Clave);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                case "FIVigencia":
                    elementos = elementos.OrderBy(s => s.FIVigencia);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Clave);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.estacion.OrderBy(e => e.IDEstacion).Count(); // Total number of elements

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


        // GET: c_Estaciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_Estaciones Estaciones = db.estacion.Find(id);
            if (Estaciones == null)
            {
                return HttpNotFound();
            }
            return View(Estaciones);
        }

        // GET: c_Estaciones/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_Estaciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( c_Estaciones Estaciones)
        {
            if (ModelState.IsValid)
            {
                db.estacion.Add(Estaciones);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Estaciones);
        }

        // GET: c_Estaciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_Estaciones Estaciones = db.estacion.Find(id);
            if (Estaciones == null)
            {
                return HttpNotFound();
            }
            return View(Estaciones);
        }

        // POST: c_Estaciones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(c_Estaciones Estaciones)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Estaciones).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Estaciones);
        }

        // GET: c_Estaciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_Estaciones Estaciones = db.estacion.Find(id);
            if (Estaciones == null)
            {
                return HttpNotFound();
            }
            return View(Estaciones);
        }

        // POST: c_Estaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_Estaciones Estaciones = db.estacion.Find(id);
            db.estacion.Remove(Estaciones);
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
        public ActionResult PDFestaciones()
        {
            var ba = db.estacion.ToList();
            Reportec_Estaciones report = new Reportec_Estaciones();
            report.EST = ba;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReportEstaciones.pdf");
        }


        public void ExcelEstaciones()
        {
            //Listado de datos
            var clpstc = db.estacion.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Estaciones aeroportuarias y estaciones férreas");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:I1"].Style.Font.Size = 20;
            Sheet.Cells["A1:I1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:I1"].Style.Font.Bold = true;
            Sheet.Cells["A1:I1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Estaciones aeroportuarias y estaciones férreas");
            Sheet.Cells["A1:I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:I2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:I2"].Style.Font.Size = 12;
            Sheet.Cells["A2:I2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:I2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("ID Estación");
            Sheet.Cells["B2"].RichText.Add("Clave Transporte");
            Sheet.Cells["C2"].RichText.Add("Clave Identificación");
            Sheet.Cells["D2"].RichText.Add("Descripción");
            Sheet.Cells["E2"].RichText.Add("Nacionalidad");
            Sheet.Cells["F2"].RichText.Add("Designador IATA");
            Sheet.Cells["G2"].RichText.Add("Línea Ferrea");
            Sheet.Cells["H2"].RichText.Add("Fecha de Inicio");
            Sheet.Cells["I2"].RichText.Add("Fecha de Terminación");

            row = 3;
            foreach (var item in clpstc)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDEstacion;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Clave;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Clave_ID;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Nacionalidad;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Designador;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Linea;
                Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.FIVigencia;
                Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.FFVigencia;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReportEstacion.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}
