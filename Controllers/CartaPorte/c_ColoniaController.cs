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
    public class c_ColoniaController : Controller
    {
        private c_ColoniaContext db = new c_ColoniaContext();

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "C_Colonia" : "C_Colonia";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "C_CodigoPostal" : "C_CodigoPostal";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "NomAsentamiento" : "NomAsentamiento";
            

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
            var elementos = from s in db.colonias
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.C_Colonia.Contains(searchString) || s.C_CodigoPostal.Contains(searchString) || s.NomAsentamiento.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Clave":
                    elementos = elementos.OrderBy(s => s.C_Colonia);
                    break;
                case "Codigo":
                    elementos = elementos.OrderBy(s => s.C_CodigoPostal);
                    break;
                case "Asentamiento":
                    elementos = elementos.OrderBy(s => s.NomAsentamiento);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.C_Colonia);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.colonias.OrderBy(e => e.IDColonia).Count(); // Total number of elements

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
        
        // GET: c_Colonia/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_Colonia c_Colonia = db.colonias.Find(id);
            if (c_Colonia == null)
            {
                return HttpNotFound();
            }
            return View(c_Colonia);
        }

        // GET: c_Colonia/Create
       

        public ActionResult CrearColonia()
        {
            return View();
        }

        // POST: c_Colonia/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult CrearColonia(c_Colonia colonia)
        {
            if (ModelState.IsValid)
            {
                db.colonias.Add(colonia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(colonia);
        }
        // GET: c_Colonia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_Colonia colonia = db.colonias.Find(id);
            if (colonia == null)
            {
                return HttpNotFound();
            }
            return View(colonia);
        }

        // POST: c_Colonia/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(c_Colonia colonia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(colonia).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(colonia);
        }


        // GET: c_Colonia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_Colonia c_Colonia = db.colonias.Find(id);
            if (c_Colonia == null)
            {
                return HttpNotFound();
            }
            return View(c_Colonia);
        }

        // POST: c_Colonia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_Colonia c_Colonia = db.colonias.Find(id);
            db.colonias.Remove(c_Colonia);
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
        public void ReporteColonia()
        {
            //Listado de datos
            var conf = db.colonias.ToList();
            //ExcelPackage.LicenseContext = LicenseContext.Commercial;
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Colonias");


            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:D1"].Style.Font.Size = 20;
            Sheet.Cells["A1:D1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:D1"].Style.Font.Bold = true;
            Sheet.Cells["A1:D1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Colonias");
            Sheet.Cells["A1:D1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:D2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:D2"].Style.Font.Size = 12;
            Sheet.Cells["A2:D2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:D2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("c_Colonia");
            Sheet.Cells["C2"].RichText.Add("c_CodigoPostal");
            Sheet.Cells["D2"].RichText.Add("Nombre del asentamiento");
            row = 3;
            foreach (var item in conf)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDColonia;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.C_Colonia;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.C_CodigoPostal;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.NomAsentamiento;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteColonias.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult PDFColonia()
        {
            var Cl = db.colonias.ToList();
            Reportec_Colonia report = new Reportec_Colonia();
            report.C = Cl;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteColonias.pdf");
        }
    }
}
