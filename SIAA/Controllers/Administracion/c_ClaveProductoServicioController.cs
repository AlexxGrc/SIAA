
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
    public class c_ClaveProductoServicioController : Controller
    {
        private c_ClaveProductoServicioContext db = new c_ClaveProductoServicioContext();
        // GET: c_ClaveProductoServicio

       
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveFormaPago" : "ClaveFormaPago";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.c_ClaveProductoServicios
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                 elementos = elementos.Where(s => s.ClaveProdServ.Contains(searchString) || s.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveProdServ":
                    elementos = elementos.OrderBy(s => s.ClaveProdServ);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClaveProdServ);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_ClaveProductoServicios.OrderBy(e => e.IDProdServ).Count(); // Total number of elements

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
        [Authorize(Roles = "Administrador, Gerencia,Sistemas")]
        public ActionResult Details(int id)
        {
            var elemento = db.c_ClaveProductoServicios.Single(m => m.IDProdServ == id);
            return View(elemento);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Details(int id, FormCollection collection)
        {
            var elemento = db.c_ClaveProductoServicios.Single(m => m.IDProdServ == id);
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
       
        public ActionResult Create(c_ClaveProductoServicio elemento)
        {
            try
            {
                // TODO: Add insert logic here

                var db = new c_ClaveProductoServicioContext();
                db.c_ClaveProductoServicios.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception error)
            {
                return View();
            }
        }

        // GET: c_ClaveProductoServicio/Edit/5
      
        public ActionResult Edit(int id)
        {
            var elemento = db.c_ClaveProductoServicios.Single(m => m.IDProdServ == id);
            if (elemento == null)
            {
                return View();
            }
            ViewBag.NoElementsPage = elemento;
            return View(elemento);

        }

        // POST: c_ClaveProductoServicio/Edit/5
       
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.c_ClaveProductoServicios.Single(m => m.IDProdServ == id);
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
            var elemento = db.c_ClaveProductoServicios.Single(m => m.IDProdServ == id);
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
                var elemento = db.c_ClaveProductoServicios.Single(m => m.IDProdServ == id);
                db.c_ClaveProductoServicios.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }
      

        public ActionResult ListadoProductoServicio()
        {
            c_ClaveProductoServicioContext dba = new c_ClaveProductoServicioContext();
            var produServ = dba.c_ClaveProductoServicios.ToList();
            ReporteProductoServicios report = new ReporteProductoServicios();
            report.ProductoServicio = produServ;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteProducto-Servicio.pdf");
        }

        public void GenerarExcelProductoServicio()
        {
            //Listado de datos
            c_ClaveProductoServicioContext dba = new c_ClaveProductoServicioContext();
            var produServ = dba.c_ClaveProductoServicios.ToList();
            ExcelPackage Ep = new ExcelPackage();

            var Sheet = Ep.Workbook.Worksheets.Add("RegimenFiscal");
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
            Sheet.Cells["A1"].RichText.Add("Catalogo: Producto o Servicio");

            row = 2;
            //Fijar la fuente para EL RANGO DE CELDAS A2:B2
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 12;
            Sheet.Cells["A2:B2"].Style.Font.Bold = true;
            Sheet.Cells["A2:B2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:B2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            //Sheet.Cells["A1"].Value = "Clave";
            //Sheet.Cells["B1"].Value = "Descripción";
            Sheet.Cells["A2"].RichText.Add("Clave del Producto o Servicio");
            Sheet.Cells["B2"].RichText.Add("Descripción");
            Sheet.Cells["A2:B2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            row = 3;
            foreach (var item in produServ)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.ClaveProdServ;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Descripcion;


                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteProductoServicioExcel.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }

}