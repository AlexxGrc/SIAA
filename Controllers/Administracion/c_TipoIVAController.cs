
using PagedList;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Login;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Data;
using System.Net;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;



namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class c_TipoIVAController : Controller
    {
        public c_TipoIVAContext db = new c_TipoIVAContext();

        // GET: c_TipoIVA

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripción" : "Descripción";
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
            var elementos = from s in db.c_TiposIVA
                            select s;

            
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Descripcion.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Tasa":
                    elementos = elementos.OrderBy(s => s.Tasa);
                    break;
                case "Descripción":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Tasa);
                    break;

            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_TiposIVA.OrderBy(e => e.IDTipoIVA).Count(); // Total number of elements

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

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación


            //return View(db.c_TipoIVA.ToList());
        }

        // GET: c_TipoIVA/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoIVA c_TipoIVA = db.c_TiposIVA.Find(id);
            if (c_TipoIVA == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoIVA);
        }

        // GET: c_TipoIVA/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_TipoIVA/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       
        public ActionResult Create([Bind(Include = "IDTipoIVA,Tasa,Descripcion")] c_TipoIVA c_TipoIVA)
        {
            if (ModelState.IsValid)
            {
                db.c_TiposIVA.Add(c_TipoIVA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_TipoIVA);
        }

        // GET: c_TipoIVA/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoIVA c_TipoIVA = db.c_TiposIVA.Find(id);
            if (c_TipoIVA == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoIVA);
        }

        // POST: c_TipoIVA/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       
        public ActionResult Edit([Bind(Include = "IDTipoIVA,Tasa,Descripcion")] c_TipoIVA c_TipoIVA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_TipoIVA).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_TipoIVA);
        }

        // GET: c_TipoIVA/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoIVA c_TipoIVA = db.c_TiposIVA.Find(id);
            if (c_TipoIVA == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoIVA);
        }

        // POST: c_TipoIVA/Delete/5
        [HttpPost, ActionName("Delete")]
      
        public ActionResult DeleteConfirmed(int id)
        {
            c_TipoIVA c_TipoIVA = db.c_TiposIVA.Find(id);
            db.c_TiposIVA.Remove(c_TipoIVA);
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
        public ActionResult ListadoTipoIva()
        {
            c_TipoIVAContext dba = new c_TipoIVAContext();

            var tipoIva = dba.c_TiposIVA.ToList();
            ReporteTipoIva report = new ReporteTipoIva();
            report.ti = tipoIva;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReportetipoIva.pdf");
        }


        //Mandamos este método para generar Excel
        public void GenerarExcelTipoIva()
        {
            //Listado de datos
            c_TipoIVAContext dba = new c_TipoIVAContext();
            var tipoiva = dba.c_TiposIVA.ToList();
            ExcelPackage Ep = new ExcelPackage();

            var Sheet = Ep.Workbook.Worksheets.Add("TipoIva");
            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:C1"].Style.Font.Size = 20;
            Sheet.Cells["A1:C1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:C1"].Style.Font.Bold = true;
            Sheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1:C1"].RichText.Add("Catálogo: Tipo Iva");
            Sheet.Cells["A1:C1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;


            row = 3;
            Sheet.Cells["A2:C2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:C2"].Style.Font.Size = 12;
            Sheet.Cells["A2:C2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:C2"].Style.Font.Bold = true;
            Sheet.Cells["A2:C2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:C2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);



            Sheet.Cells["A2"].Value = "ID";
            Sheet.Cells["B2"].Value = "Tasa";
            Sheet.Cells["C2"].Value = "Descripción";

            row = 3;


            foreach (var item in tipoiva)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDTipoIVA;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Tasa;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelTipoIva.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}
