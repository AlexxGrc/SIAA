
using PagedList;
using SIAAPI.Models.Administracion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;

namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class c_FormaPagoController : Controller
    {
        private c_FormaPagoContext db = new c_FormaPagoContext();
        // GET: c_FormaPago
       
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            //var lista = from e in db.c_FormaPagos
            //            orderby e.IDFormaPago
            //            select e;
            //return View(lista);
            // Not sure here
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveFormaPago" : "ClaveFormaPago";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";

            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.c_FormaPagos
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.ClaveFormaPago.Contains(searchString) || s.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveProdServ":
                    elementos = elementos.OrderBy(s => s.ClaveFormaPago);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClaveFormaPago);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_FormaPagos.OrderBy(e => e.IDFormaPago).Count(); // Total number of elements

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

        // GET: c_ClaveProductoServicio/Details/5
      
        public ActionResult Details(int id)
        {
            var elemento = db.c_FormaPagos.Single(m => m.IDFormaPago == id);
            return View(elemento);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public ActionResult Details(int id, FormCollection collection)
        {
            var elemento = db.c_FormaPagos.Single(m => m.IDFormaPago == id);
            return View(elemento);

        }

        // GET: c_ClaveProductoServicio/Create
      
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_ClaveProductoServicio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
       

        public ActionResult Create(c_FormaPago elemento)
        {
            try
            {
                // TODO: Add insert logic here

                var db = new c_FormaPagoContext();
                db.c_FormaPagos.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception error)
            {
                string mensajederror = error.Message;
                return View();
            }
        }




        // GET: c_FormaPago/Edit/5
       
        public ActionResult Edit(int id)
        {
            var elemento = db.c_FormaPagos.Single(m => m.IDFormaPago == id);
            db.SaveChanges();
            return View(elemento);
        }

        // POST: c_FormaPago/Edit/5
     
        [HttpPost]
         public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                var elemento = db.c_FormaPagos.Single(m => m.IDFormaPago == id);
            if (TryUpdateModel(elemento))
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View (elemento);
         }
            catch
            {
                return View();
            }
        }

        // GET: /Delete/5
      
        public ActionResult Delete(int id)
        {
            var elemento = db.c_FormaPagos.Single(m => m.IDFormaPago == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }


        // POST: /Delete/5
      
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var elemento = db.c_FormaPagos.Single(m => m.IDFormaPago == id);
                db.c_FormaPagos.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }
     
        public ActionResult ListadoFormaPago()
        {
            c_FormaPagoContext dba = new c_FormaPagoContext();

            var formapago = dba.c_FormaPagos.ToList();
            ReporteFormadePago report = new ReporteFormadePago();
            report.Pago = formapago;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteFormaPago.pdf");
        }

        public void GenerarExcelFormaPago()
        {
            //Listado de datos
            c_FormaPagoContext dba = new c_FormaPagoContext();
            var formapago = dba.c_FormaPagos.ToList();
            ExcelPackage Ep = new ExcelPackage();

            var Sheet = Ep.Workbook.Worksheets.Add("FormaPago");
            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:C1"].Style.Font.Size = 20;
            Sheet.Cells["A1:C1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:C1"].Style.Font.Bold = true;
            Sheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Catálogo: Formas de Pago");
            Sheet.Cells["A1:C1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;


            row = 2;
            Sheet.Cells["A2:C2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:C2"].Style.Font.Size = 12;
            Sheet.Cells["A2:C2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:C2"].Style.Font.Bold = true;
            Sheet.Cells["A2:C2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:C2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);



            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("Clave");
            Sheet.Cells["C2"].RichText.Add("Descripción");

            row = 3;


            foreach (var item in formapago)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDFormaPago;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.ClaveFormaPago;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelFormaPago.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

    }

}



