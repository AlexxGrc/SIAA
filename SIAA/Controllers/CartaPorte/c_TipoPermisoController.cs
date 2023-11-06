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
    public class c_TipoPermisoController : Controller
    {
        private c_TipoPermisoContext db = new c_TipoPermisoContext();
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "Clave" : "Clave";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveT" : "ClaveT";
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
            var elementos = from s in db.permiso
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.Clave.Contains(searchString) || s.Descripcion.Contains(searchString) || s.FIVigencia.ToString().Contains(searchString));

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
                case "FechaInicio":
                    elementos = elementos.OrderBy(s => s.FIVigencia);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Clave);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.permiso.OrderBy(e => e.IDTipoPermiso).Count(); // Total number of elements

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
       

        // GET: c_TipoPermiso/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoPermiso c_TipoPermiso = db.permiso.Find(id);
            if (c_TipoPermiso == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoPermiso);
        }

        // GET: c_TipoPermiso/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_TipoPermiso/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(c_TipoPermiso c_TipoPermiso)
        {
            if (ModelState.IsValid)
            {
                db.permiso.Add(c_TipoPermiso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_TipoPermiso);
        }

        // GET: c_TipoPermiso/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoPermiso c_TipoPermiso = db.permiso.Find(id);
            if (c_TipoPermiso == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoPermiso);
        }

        // POST: c_TipoPermiso/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(c_TipoPermiso c_TipoPermiso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_TipoPermiso).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_TipoPermiso);
        }

        // GET: c_TipoPermiso/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoPermiso c_TipoPermiso = db.permiso.Find(id);
            if (c_TipoPermiso == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoPermiso);
        }

        // POST: c_TipoPermiso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_TipoPermiso c_TipoPermiso = db.permiso.Find(id);
            db.permiso.Remove(c_TipoPermiso);
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
        public void ExcelTipoPermiso()
        {
            //Listado de datos
            var conf = db.permiso.ToList();
            //ExcelPackage.LicenseContext = LicenseContext.Commercial;
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Tipo Permiso");


            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:F1"].Style.Font.Size = 20;
            Sheet.Cells["A1:F1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:F1"].Style.Font.Bold = true;
            Sheet.Cells["A1:F1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado Tipo Permiso");
            Sheet.Cells["A1:F1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:F2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:F2"].Style.Font.Size = 12;
            Sheet.Cells["A2:F2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:F2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("Clave");
            Sheet.Cells["C2"].RichText.Add("Descripción");
            Sheet.Cells["D2"].RichText.Add("Clave Transporte");
            Sheet.Cells["E2"].RichText.Add("Fecha de inicio de vigencia");
            Sheet.Cells["F2"].RichText.Add("Fecha de fin de vigencia");

            row = 3;
            foreach (var item in conf)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDTipoPermiso;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Clave;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.ClaveT;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.FIVigencia;
                Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.FFVigencia;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteTipoPermiso.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult PDFTipoPermiso()
        {
            var tp = db.permiso.ToList();
            Reportes.Reportec_TipoPermiso report = new Reportes.Reportec_TipoPermiso();
            report.Tpermiso = tp;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteTipoPermiso.pdf");
        }
    }
}
