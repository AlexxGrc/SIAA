using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SIAAPI.Models.Administracion;
using PagedList;
using System.IO;

using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;

using SIAAPI.Reportes.Crystal;

namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class c_TipoComprobanteController : Controller
    {
        private c_TipoComprobanteContext db = new c_TipoComprobanteContext();

        // GET: c_TipoComprobante
     
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.C1SortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveTipoComprobante" : "ClaveTipoComprobante";
            ViewBag.C2SortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";

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
            var elementos = from s in db.c_TipoComprobantes
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.ClaveTipoComprobante.Contains(searchString) || s.Descripcion.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveTipoComprobante":
                    elementos = elementos.OrderBy(s => s.ClaveTipoComprobante);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClaveTipoComprobante);

                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_TipoComprobantes.OrderBy(e => e.IDTipoComprobante).Count(); // Total number of elements

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
        }
       
        public ActionResult Details(int id)
        {
            var elemento = db.c_TipoComprobantes.Single(m => m.IDTipoComprobante == id);
            return View(elemento);
        }
        // GET: c_TipoComprobante/Create
    
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_TipoComprobante/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDTipoComprobante,ClaveTipoComprobante,Descripcion,ValorMaximo")] c_TipoComprobante c_TipoComprobante)
        {
            if (ModelState.IsValid)
            {
                db.c_TipoComprobantes.Add(c_TipoComprobante);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_TipoComprobante);
        }

        // GET: c_TipoComprobante/Edit/5
     
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoComprobante c_TipoComprobante = db.c_TipoComprobantes.Find(id);
            if (c_TipoComprobante == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoComprobante);
        }

        // POST: c_TipoComprobante/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDTipoComprobante,ClaveTipoComprobante,Descripcion,ValorMaximo")] c_TipoComprobante c_TipoComprobante)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_TipoComprobante).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_TipoComprobante);
        }

        // GET: c_TipoComprobante/Delete/5
     
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_TipoComprobante c_TipoComprobante = db.c_TipoComprobantes.Find(id);
            if (c_TipoComprobante == null)
            {
                return HttpNotFound();
            }
            return View(c_TipoComprobante);
        }

        // POST: c_TipoComprobante/Delete/5
       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_TipoComprobante c_TipoComprobante = db.c_TipoComprobantes.Find(id);
            db.c_TipoComprobantes.Remove(c_TipoComprobante);
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
      
      
        public ActionResult ListadoTipoComprobante()
        {
            c_TipoComprobanteContext dba = new c_TipoComprobanteContext();

            var TipoComprobante = dba.c_TipoComprobantes.ToList();
            ReporteTipoComprobante report = new ReporteTipoComprobante();
            report.tcomprobante = TipoComprobante;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteTipoComprobante.pdf");
        }


        public void GenerarExcelTipoComprobante()
        {
            //Listado de datos
            c_TipoComprobanteContext dba = new c_TipoComprobanteContext();
            var tipoComprobante = dba.c_TipoComprobantes.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Tipo Comprobante");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:E1"].Style.Font.Size = 20;
            Sheet.Cells["A1:E1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:E1"].Style.Font.Bold = true;
            Sheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Tipo Comprobante");
            Sheet.Cells["A1:E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:E2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:E2"].Style.Font.Size = 12;
            Sheet.Cells["A2:E2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:E2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("ID Tipo Comprobante");
            Sheet.Cells["B2"].RichText.Add("Clave Tipo Comprobante");
            Sheet.Cells["C2"].RichText.Add("Descripción");
            Sheet.Cells["D2"].RichText.Add("Valor Maximo");
            row = 3;
            foreach (var item in tipoComprobante)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDTipoComprobante;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.ClaveTipoComprobante;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.ValorMaximo;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelTipoComprobante.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }


    }

}