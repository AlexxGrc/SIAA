using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SIAAPI.Models.Administracion;
using PagedList;

using System.IO;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;

namespace SIAAPI.Controllers
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class c_MetodoPagoController : Controller
    {
        private c_MetodoPagoContext db = new c_MetodoPagoContext();
        // GET: c_MetodoPago
      
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
         {
            //var lista = from e in db.c_MetodoPagos
            //            orderby e.IDMetodoPago
            //            select e;
            //return View(lista);
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveMetodoPago" : "ClaveMetodoPago";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.c_MetodoPagos
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.ClaveMetodoPago.Contains(searchString) || s.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveProdServ":
                    elementos = elementos.OrderBy(s => s.ClaveMetodoPago);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClaveMetodoPago);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_MetodoPagos.OrderBy(e => e.IDMetodoPago).Count(); // Total number of elements

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

        // GET: c_MetodoPago/Details/5
     
        public ActionResult Details(int id)
        {
            var elemento = db.c_MetodoPagos.Single(m => m.IDMetodoPago == id);
            return View(elemento);
        }
        [HttpPost] 
        public ActionResult Details(int id, c_MetodoPago collection)
        {
            var elemento = db.c_MetodoPagos.Single(m => m.IDMetodoPago == id);
            return View(elemento);
        }
        // GET: c_MetodoPago/Create
    
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_MetodoPago/Create
     
        [HttpPost]

        public ActionResult Create(c_MetodoPago elemento)
        {
            try
            {
                // TODO: Add insert logic here
                var db = new c_MetodoPagoContext();
                db.c_MetodoPagos.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: c_MetodoPago/Edit/5
    
        public ActionResult Edit(int id)
        {
            var elemento = db.c_MetodoPagos.Single(m => m.IDMetodoPago == id);
            return View(elemento);
         
        }

        // POST: c_MetodoPago/Edit/5
      
        [HttpPost]
        public ActionResult Edit(int id, c_MetodoPago collection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.c_MetodoPagos.Single(m => m.IDMetodoPago == id);
                if (TryUpdateModel(elemento))
                    db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        // GET: /Delete/5
       
        public ActionResult Delete(int id)
        {
            var elemento = db.c_MetodoPagos.Single(m => m.IDMetodoPago == id);
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
                var elemento = db.c_MetodoPagos.Single(m => m.IDMetodoPago == id);
                db.c_MetodoPagos.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }
     
        

        public ActionResult ListadoMetodosDePago()
        {
            c_MetodoPagoContext dba = new c_MetodoPagoContext();

            var metodos = dba.c_MetodoPagos.ToList();
            ReporteMetodoDePago report = new ReporteMetodoDePago();
            report.metodosdepago = metodos;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteMetodosDePago.pdf");
        }

        public void GenerarExcelMetodosDePago()
        {
            //Listado de datos
            c_MetodoPagoContext dba = new c_MetodoPagoContext();
            var metodosdePago = dba.c_MetodoPagos.ToList();
            ExcelPackage Ep = new ExcelPackage();


            var Sheet = Ep.Workbook.Worksheets.Add("MetodosDePago");
            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            int row = 1;
            //Fijar la fuente para EL RANGO DE CELDAS A1:B1
            Sheet.Cells["A1:B1"].Style.Font.Size = 20;
            Sheet.Cells["A1:B1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:B1"].Style.Font.Bold = true;
            Sheet.Cells["A1:B1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1:B1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A1"].RichText.Add("Catalogo: Condiciones de Pago");

            row = 2;
            //Fijar la fuente para EL RANGO DE CELDAS A2:B2
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 12;
            Sheet.Cells["A2:C2"].Style.Font.Bold = true;
            Sheet.Cells["A2:C2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:C2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            //Sheet.Cells["A1"].Value = "Clave";
            //Sheet.Cells["B1"].Value = "Descripción";
            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("ClaveMetodoDePago");
            Sheet.Cells["C2"].RichText.Add("Descripción");
            Sheet.Cells["A2:C2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            row = 3;
            foreach (var item in metodosdePago)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDMetodoPago;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.ClaveMetodoPago;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteMetodosDePago.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }

}


