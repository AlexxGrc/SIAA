using PagedList;
using SIAAPI.Models.Administracion;
using System;
using System.Collections.Generic;
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
    public class c_MonedaController : Controller
    {
        private c_MonedaContext db = new c_MonedaContext();
        // GET: c_Moneda
     
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            //var lista = from e in db.c_Monedas
            //            orderby e.IDMoneda
            //            select e;
            //return View(lista);
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveMoneda" : "ClaveMoneda";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.c_Monedas
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.ClaveMoneda.Contains(searchString) || s.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveProdServ":
                    elementos = elementos.OrderBy(s => s.ClaveMoneda);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClaveMoneda);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_Monedas.OrderBy(e => e.IDMoneda).Count(); // Total number of elements

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

        // GET: c_Moneda/Details/5
      
        public ActionResult Details(int id)
        {
            var elemento = db.c_Monedas.Single(m => m.IDMoneda == id);
            return View(elemento);
        }



        // GET: c_Moneda/Create
     
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_Moneda/Create
        [HttpPost]
       
        [ValidateAntiForgeryToken]
        public ActionResult Create(c_Moneda elemento)
        {
            try
            {
                var db = new c_MonedaContext();
                db.c_Monedas.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: c_Moneda/Edit/5
       
        public ActionResult Edit(int id)
        {
            var elemento = db.c_Monedas.Single(m => m.IDMoneda == id);
            return View(elemento);
        }

        // POST: c_Moneda/Edit/5
       
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {

                var elemento = db.c_Monedas.Single(m => m.IDMoneda == id);
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
        // GET: /Delete/5
      
        public ActionResult Delete(int id)
        {
            var elemento = db.c_Monedas.Single(m => m.IDMoneda == id);
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
                var elemento = db.c_Monedas.Single(m => m.IDMoneda == id);
                db.c_Monedas.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }

        public ActionResult ListadoMoneda()
        {
            c_MonedaContext dba = new c_MonedaContext();

            var monedas = dba.c_Monedas.ToList();
            ReporteMoneda report = new ReporteMoneda();
            report.moneda = monedas;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteMoneda.pdf");
        }


        public void GenerarExcelMoneda()
        {
            //Listado de datos
            c_MonedaContext dba = new c_MonedaContext();
            var moneda = dba.c_Monedas.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Tipo Moneda");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:E1"].Style.Font.Size = 20;
            Sheet.Cells["A1:E1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:E1"].Style.Font.Bold = true;
            Sheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Moneda");
            Sheet.Cells["A1:E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:E2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:E2"].Style.Font.Size = 12;
            Sheet.Cells["A2:E2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:E2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("Clave Moneda");
            Sheet.Cells["B2"].RichText.Add("Descripción");
            Sheet.Cells["C2"].RichText.Add("NoDecimales");
            Sheet.Cells["D2"].RichText.Add("Variación");
            row = 3;
            foreach (var item in moneda)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.ClaveMoneda;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.NoDecimales;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Variacion;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelMoneda.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

    }
}
