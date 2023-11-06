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

namespace SIAAPI.Controllers.CartaPorte
{
    public class c_MunicipioController : Controller
    {
        private c_MunicipioContext db = new c_MunicipioContext();

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "Municipio" : "Municipio";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Estado" : "Estado";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
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
            var elementos = from s in db.municipio
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.C_Municipio.ToString().Contains(searchString) || s.C_Estado.Contains(searchString) || s.Descripcion.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Municipio":
                    elementos = elementos.OrderBy(s => s.C_Municipio);
                    break;
                case "Estado":
                    elementos = elementos.OrderBy(s => s.C_Estado);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.C_Municipio);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.municipio.OrderBy(e => e.IDMunicipio).Count(); // Total number of elements

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

        

        // GET: c_Municipio/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_Municipio c_Municipio = db.municipio.Find(id);
            if (c_Municipio == null)
            {
                return HttpNotFound();
            }
            return View(c_Municipio);
        }

        // GET: c_Municipio/Create
       

        public ActionResult CrearMunicipio()
        {
            return View();
        }

        // POST: c_Municipio/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CrearMunicipio(c_Municipio municipio)
        {
            if (ModelState.IsValid)
            {
                db.municipio.Add(municipio);
                db.SaveChanges();
                //string cadenasql = "insert into [dbo].[c_Municipio](C_Municipio, C_Estado, Descripcion) values ('" + municipio.C_Municipio + "','" + municipio.C_Estado + "','" + municipio.Descripcion +"')";
                //new c_MunicipioContext().Database.ExecuteSqlCommand(cadenasql);
                return RedirectToAction("Index");
            }

            return View(municipio);
        }

        // GET: c_Municipio/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_Municipio c_Municipio = db.municipio.Find(id);
            if (c_Municipio == null)
            {
                return HttpNotFound();
            }
            return View(c_Municipio);
        }

        // POST: c_Municipio/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(c_Municipio municipio)
        {
            if (ModelState.IsValid)
            {
               db.Entry(municipio).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(municipio);
        }

        // GET: c_Municipio/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_Municipio municipio = db.municipio.Find(id);
            if (municipio == null)
            {
                return HttpNotFound();
            }
            return View(municipio);
        }

        // POST: c_Municipio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_Municipio municipio = db.municipio.Find(id);
            db.municipio.Remove(municipio);
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
        public ActionResult PDFMunicipios()
        {
            var muni = db.municipio.ToList();
            Reportes.Reportec_Municipio report = new Reportes.Reportec_Municipio();
            report.munic = muni;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteMunicipios.pdf");
        }


        public void ExcelMunicipios()
        {
            //Listado de datos
            var mun = db.municipio.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Municipios");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:D1"].Style.Font.Size = 20;
            Sheet.Cells["A1:D1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:D1"].Style.Font.Bold = true;
            Sheet.Cells["A1:D1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Municipios");
            Sheet.Cells["A1:D1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:D2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:D2"].Style.Font.Size = 12;
            Sheet.Cells["A2:D2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:D2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("Municipio");
            Sheet.Cells["C2"].RichText.Add("Estado");
            Sheet.Cells["D2"].RichText.Add("Descripción");

            row = 3;
            foreach (var item in mun)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDMunicipio;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.C_Municipio;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.C_Estado;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Descripcion;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteMunicipio.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}
