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
    public class c_CodigoTransporteAereoController : Controller
    {
        private c_CodigoTransporteAereoContext db = new c_CodigoTransporteAereoContext();

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "Clave" : "";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Nacionalidad" : "Nacionalidad";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Aereolinea" : "Aereolinea";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Designador" : "Designador";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "FIvigencia" : "FIvigencia";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "FFvigencia" : "FFvigencia";
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
            var elementos = from s in db.Transporte
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.Clave.Contains(searchString) || s.Nacionalidad.Contains(searchString) || s.Aerolinea.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Clave":
                    elementos = elementos.OrderBy(s => s.Clave);
                    break;
                case "Nacionalidad":
                    elementos = elementos.OrderBy(s => s.Nacionalidad);
                    break;
                case "Aereolinea":
                    elementos = elementos.OrderBy(s => s.Aerolinea);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Clave);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Transporte.OrderBy(e => e.IDTransAereo).Count(); // Total number of elements

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


        

        // GET: c_CodigoTransporteAereo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_CodigoTransporteAereo c_CodigoTransporteAereo = db.Transporte.Find(id);
            if (c_CodigoTransporteAereo == null)
            {
                return HttpNotFound();
            }
            return View(c_CodigoTransporteAereo);
        }

        // GET: c_CodigoTransporteAereo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_CodigoTransporteAereo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(c_CodigoTransporteAereo c_CodigoTransporteAereo)
        {
            if (ModelState.IsValid)
            {
                db.Transporte.Add(c_CodigoTransporteAereo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_CodigoTransporteAereo);
        }

        // GET: c_CodigoTransporteAereo/Edit/5
        public ActionResult Edit(int? id)
        {
            
            var elemento = db.Transporte.Single(m => m.IDTransAereo == id);
            return View(elemento);
        }

        // POST: c_CodigoTransporteAereo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection formCollection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.Transporte.Single(m => m.IDTransAereo == id);
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

        // GET: c_CodigoTransporteAereo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_CodigoTransporteAereo c_CodigoTransporteAereo = db.Transporte.Find(id);
            if (c_CodigoTransporteAereo == null)
            {
                return HttpNotFound();
            }
            return View(c_CodigoTransporteAereo);
        }

        // POST: c_CodigoTransporteAereo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_CodigoTransporteAereo c_CodigoTransporteAereo = db.Transporte.Find(id);
            db.Transporte.Remove(c_CodigoTransporteAereo);
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

        public ActionResult ListadoTransporte()
        {
            var transporte = db.Transporte.ToList();
            ReporteCodigoTransporteAereo report = new ReporteCodigoTransporteAereo();
            report.TransAereo = transporte;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteTransporteAereo.pdf");
        }
        public void GenerarExcelTransporte()
        {
            //Listado de datos
            var bancos = db.Transporte.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Transporte Aéreo");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:G1"].Style.Font.Size = 20;
            Sheet.Cells["A1:G1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:G1"].Style.Font.Bold = true;
            Sheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Transporte Aéreo");
            Sheet.Cells["A1:G1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:G2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:G2"].Style.Font.Size = 12;
            Sheet.Cells["A2:G2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:G2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("Clave");
            Sheet.Cells["C2"].RichText.Add("Nacionalidad");
            Sheet.Cells["D2"].RichText.Add("Aereolínea");
            Sheet.Cells["E2"].RichText.Add("Designador");
            Sheet.Cells["F2"].RichText.Add("Fecha Inicial");
            Sheet.Cells["G2"].RichText.Add("Fecha Final");


            row = 3;
            foreach (var item in bancos)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDTransAereo;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Clave;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Nacionalidad;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Aerolinea;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Designador;
                Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.FIvigencia;


                //Sheet.Cells[string.Format("E{0}", row)].Value = item.FFVigencia;

                if (item.FFvigencia == null)
                {
                    Sheet.Cells[string.Format("G{0}", row)].Value = "";
                }
                else
                {
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.FFvigencia;
                }
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelTransporteAereo.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

    }
}
